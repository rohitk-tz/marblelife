using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Impl;
using Core.MarketingLead.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Reports;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class UpdateMarketingLeadReportDataService : IUpdateMarketingLeadReportDataService
    {
        private readonly IRepository<RoutingNumber> _routingNumberRepository;
        private readonly IRepository<CallDetailData> _callDetailDataRepository;
        private readonly IRepository<WebLeadData> _webLeadDataRepository;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<WebLead> _webLeadRepository;
        private readonly IProductReportService _productReportService;
        private readonly ILogService _logService;
        private IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        public UpdateMarketingLeadReportDataService(IUnitOfWork unitOfWork, IClock clock, ISettings settings, ILogService logService, IProductReportService productReportService)

        {
            _routingNumberRepository = unitOfWork.Repository<RoutingNumber>();
            _callDetailDataRepository = unitOfWork.Repository<CallDetailData>();
            _marketingLeadCallDetailRepository = unitOfWork.Repository<MarketingLeadCallDetail>();
            _webLeadRepository = unitOfWork.Repository<WebLead>();
            _webLeadDataRepository = unitOfWork.Repository<WebLeadData>();
            _productReportService = productReportService;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _settings = settings;
            _logService = logService;
            _organizationRepository = unitOfWork.Repository<Organization>();
        }

        public void UpdateData()
        {
            _logService.Info("starting update for call details data : CallDetails Analysis");
            UpdateCallDetailData();
            _logService.Info("Stop update for call details data : CallDetails Analysis");

            _logService.Info("starting update for web leads : WebLeads Analysis");
            //UpdateWebLeadData();
            _logService.Info("Stop update for web leads : WebLeads Analysis");
        }

        private void UpdateCallDetailData()
        {
            var startDate = _clock.UtcNow.AddDays(-180);
            var endDate = _clock.UtcNow;
            if (_settings.UpdateOldData)
            {

                startDate = _settings.CallDetailStartDate;
                endDate = _clock.ToUtc(DateTime.UtcNow);
            }
            _unitOfWork.StartTransaction();

            var phoneLabelList = _routingNumberRepository.Table.ToList();
            foreach (var phoneLabel in phoneLabelList)
            {
                _logService.Info(string.Format("Updating data for phoneLabel " + phoneLabel.PhoneLabel));

                var monthDuration = _productReportService.MonthsBetween(startDate, endDate);
                UpdateMonthlyCallDetailCollection(monthDuration, phoneLabel.PhoneLabel, phoneLabel.PhoneNumber);

                var weekCollection = DateRangeHelperService.DayOfWeekCollection(startDate, endDate);
                UpdateWeeklyCallDetailCollection(weekCollection, phoneLabel.PhoneLabel, phoneLabel.PhoneNumber);

            }
            _logService.Info("Updation of call details data calculation Ends. : CallDetail Analysis");
        }

        private void UpdateMonthlyCallDetailCollection(IEnumerable<Tuple<int, int>> monthDuration, string phoneLabel, string phoneNumber)
        {
            foreach (var month in monthDuration.OrderBy(x => x.Item2))
            {
                _logService.Info(string.Format("Updating data for month " + month.Item1 + "/" + month.Item2));
                var firstDayOfMonth = new DateTime(month.Item2, month.Item1, 1).Date;
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                try
                {
                    var collection = _marketingLeadCallDetailRepository.Fetch(x => (x.DateAdded.Value.Month == month.Item1 && x.DateAdded.Value.Year == month.Item2)
                                        && x.PhoneLabel.Equals(phoneLabel) && x.DialedNumber.Equals(phoneNumber)).GroupBy(x => x.FranchiseeId);

                    var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = 1, IsNew = true };

                    var domain = new CallDetailData
                    {
                        PhoneLabel = phoneLabel,
                        StartDate = firstDayOfMonth,
                        PhoneNumber = phoneNumber,
                        EndDate = lastDayOfMonth,
                        DataRecorderMetaData = dataRecorderMetaData,
                        IsWeekly = false,
                        IsNew = true
                    };
                    if (!collection.Any())
                    {
                        var inDBRecord = _callDetailDataRepository.Fetch(x => (x.PhoneLabel.Equals(phoneLabel) && x.PhoneNumber.Equals(phoneNumber)
                                            && x.FranchiseeId == null
                                            && x.StartDate == firstDayOfMonth && !x.IsWeekly)).FirstOrDefault();
                        if (inDBRecord == null)
                        {
                            domain.Count = 0;
                            domain.FranchiseeId = null;
                            _callDetailDataRepository.Save(domain);
                        }
                    }

                    foreach (var item in collection)
                    {
                        var inDBRecord = _callDetailDataRepository.Fetch(x => (x.PhoneLabel.Equals(phoneLabel) && x.PhoneNumber.Equals(phoneNumber)
                                            && x.StartDate == firstDayOfMonth && !x.IsWeekly)).FirstOrDefault();

                        if (inDBRecord == null)
                        {
                            domain.Count = item.Count();
                            domain.FranchiseeId = item.Key ?? null;
                            _callDetailDataRepository.Save(domain);
                        }
                        else
                        {
                            if (inDBRecord.FranchiseeId != item.Key && item.Key != null)
                            {
                                inDBRecord.Count = item.Count();
                                inDBRecord.IsNew = false;
                                inDBRecord.FranchiseeId = item.Key;
                                _callDetailDataRepository.Save(inDBRecord);
                            }
                            else
                            {
                                inDBRecord.Count = item.Count();
                                _callDetailDataRepository.Save(inDBRecord);
                            }
                            if (item.Key == null)
                            {
                               var franchiseeId = GetFranchiseeNameByPhoneNumber(phoneNumber);
                                if (inDBRecord.FranchiseeId == null)
                                {
                                    inDBRecord.FranchiseeId = franchiseeId;
                                }
                                inDBRecord.IsNew = false;
                                _callDetailDataRepository.Save(inDBRecord);
                            }
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Error("Exception - " + ex.StackTrace);
                }
                finally
                {
                    _unitOfWork.ResetContext();
                }
                _logService.Info(string.Format("End Updating callDetail data for month " + month.Item1 + "/" + month.Item2));
            }
        }

        private void UpdateWeeklyCallDetailCollection(IEnumerable<DateTime> weekCollection, string phoneLabel, string phoneNumber)
        {
            foreach (var week in weekCollection.OrderBy(x => x.Date))
            {
                var startOfWeek = week.Date.AddDays(-6);
                try
                {
                    startOfWeek = startOfWeek.Date;
                    var endOfWeek = week.Date.AddDays(1).AddTicks(-1);
                    _logService.Info(string.Format("Updating data for Week " + startOfWeek + "/" + week.Date));

                    var collection = _marketingLeadCallDetailRepository.Fetch(x => (x.DateAdded >= startOfWeek && x.DateAdded <= endOfWeek)
                                        && x.PhoneLabel.Equals(phoneLabel) && x.DialedNumber.Equals(phoneNumber)).GroupBy(x => x.FranchiseeId);

                    var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = 1, IsNew = true };

                    var domain = new CallDetailData
                    {
                        PhoneLabel = phoneLabel,
                        StartDate = startOfWeek,
                        PhoneNumber = phoneNumber,
                        EndDate = endOfWeek.Date,
                        DataRecorderMetaData = dataRecorderMetaData,
                        IsWeekly = true,
                        IsNew = true
                    };
                    if (!collection.Any())
                    {
                        domain.Count = 0;
                        domain.FranchiseeId = null;
                        _callDetailDataRepository.Save(domain);
                    }
                    foreach (var item in collection)
                    {
                        var inDBRecord = _callDetailDataRepository.Fetch(x => (x.PhoneLabel.Equals(phoneLabel) && x.PhoneNumber.Equals(phoneNumber)
                                            && x.FranchiseeId == item.Key
                                            && x.IsWeekly && x.StartDate == startOfWeek && x.EndDate == week.Date)).FirstOrDefault();

                        if (inDBRecord == null)
                        {
                            domain.Count = item.Count();
                            domain.FranchiseeId = item.Key ?? null;
                            _callDetailDataRepository.Save(domain);
                        }
                        else
                        {
                            if (inDBRecord.FranchiseeId != item.Key && item.Key != null)
                            {
                                inDBRecord.Count = item.Count();
                                inDBRecord.IsNew = false;
                                inDBRecord.FranchiseeId = item.Key;
                                _callDetailDataRepository.Save(inDBRecord);
                            }
                            else
                            {
                                inDBRecord.Count = item.Count();
                                inDBRecord.IsNew = false;
                                _callDetailDataRepository.Save(inDBRecord);
                            }
                            if (item.Key == null)
                            {
                                inDBRecord.FranchiseeId = GetFranchiseeNameByPhoneNumber(phoneNumber);
                                inDBRecord.IsNew = false;
                                _callDetailDataRepository.Save(inDBRecord);
                            }
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Error("Exception - " + ex.StackTrace);
                }
                finally
                {
                    _unitOfWork.ResetContext();
                }
                _logService.Info(string.Format("End Updating callDetail data for Week " + week.Date));
            }
        }

        private void UpdateWebLeadData()
        {
            var startDate = _clock.UtcNow.AddDays(-4);
            var endDate = _clock.UtcNow;
            if (_settings.UpdateOldData)
            {
                var firstRecord = _webLeadRepository.Table.OrderBy(x => x.CreatedDate).FirstOrDefault();
                if (firstRecord == null)
                    return;

                startDate = firstRecord.CreatedDate;
                endDate = _clock.UtcNow;
            }
            _unitOfWork.StartTransaction();

            var urlList = _webLeadRepository.Table.Select(x => x.URL).Distinct().ToList();

            foreach (var url in urlList)
            {
                _logService.Info(string.Format("Updating data for URL " + url));

                var monthDuration = _productReportService.MonthsBetween(startDate, endDate);
                UpdateMonthlyWebLeadCollection(monthDuration, url);

                var weekCollection = DateRangeHelperService.DayOfWeekCollection(startDate, endDate);
                UpdateWeeklyWebLeadCollection(weekCollection, url);

            }
            _logService.Info("Updation of WebLead data calculation Ends. : WebLead Analysis");

        }

        private void UpdateMonthlyWebLeadCollection(IEnumerable<Tuple<int, int>> monthDuration, string url)
        {
            foreach (var month in monthDuration.OrderBy(x => x.Item2))
            {
                _logService.Info(string.Format("Updating data for month " + month.Item1 + "/" + month.Item2));
                var firstDayOfMonth = new DateTime(month.Item2, month.Item1, 1).Date;
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                try
                {
                    var collection = _webLeadRepository.Fetch(x => (x.CreatedDate.Month == month.Item1 && x.CreatedDate.Year == month.Item2)
                                            && (x.URL.Equals(url))).GroupBy(x => x.FranchiseeId);

                    var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = 1, IsNew = true };
                    var domain = new WebLeadData
                    {
                        Url = url,
                        StartDate = firstDayOfMonth,
                        EndDate = lastDayOfMonth,
                        IsWeekly = false,
                        IsNew = true,
                        DataRecorderMetaData = dataRecorderMetaData,
                    };
                    if (!collection.Any())
                    {
                        domain.Count = 0;
                        domain.FranchiseeId = null;
                        _webLeadDataRepository.Save(domain);
                    }
                    foreach (var item in collection)
                    {
                        var inDBRecord = _webLeadDataRepository.Fetch(x => (x.Url.Equals(url) && x.FranchiseeId == item.Key
                                             && x.StartDate == firstDayOfMonth && !x.IsWeekly)).FirstOrDefault();
                        if (inDBRecord == null)
                        {
                            domain.Count = item.Count();
                            domain.FranchiseeId = item.Key ?? null;
                            _webLeadDataRepository.Save(domain);
                        }
                        else
                        {
                            inDBRecord.Count = item.Count();
                            _webLeadDataRepository.Save(inDBRecord);
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Error("Exception - " + ex.StackTrace);
                }
                finally
                {
                    _unitOfWork.ResetContext();
                }
                _logService.Info(string.Format("End Updating WebLead data for month " + month.Item1 + "/" + month.Item2));
            }
        }

        private void UpdateWeeklyWebLeadCollection(IEnumerable<DateTime> weekCollection, string url)
        {
            foreach (var week in weekCollection.OrderBy(x => x.Date))
            {
                var startOfWeek = week.Date.AddDays(-6);
                try
                {
                    startOfWeek = startOfWeek.Date;
                    var endOfWeek = week.Date.AddDays(1).AddTicks(-1);
                    _logService.Info(string.Format("Updating WebLead data for Week " + startOfWeek + "/" + week.Date));

                    var collection = _webLeadRepository.Fetch(x => (x.CreatedDate >= startOfWeek && x.CreatedDate <= endOfWeek)
                                        && x.URL.Equals(url)).GroupBy(x => x.FranchiseeId);

                    var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = 1, IsNew = true };

                    var domain = new WebLeadData
                    {
                        Url = url,
                        StartDate = startOfWeek,
                        EndDate = endOfWeek.Date,
                        DataRecorderMetaData = dataRecorderMetaData,
                        IsWeekly = true,
                        IsNew = true
                    };
                    if (!collection.Any())
                    {
                        domain.Count = 0;
                        domain.FranchiseeId = null;
                        _webLeadDataRepository.Save(domain);
                    }
                    foreach (var item in collection)
                    {
                        var inDBRecord = _webLeadDataRepository.Fetch(x => (x.Url.Equals(url) && x.FranchiseeId == item.Key
                                            && x.IsWeekly && x.StartDate == startOfWeek && x.EndDate == week.Date)).FirstOrDefault();

                        if (inDBRecord == null)
                        {
                            domain.Count = item.Count();
                            domain.FranchiseeId = item.Key ?? null;
                            _webLeadDataRepository.Save(domain);
                        }
                        else
                        {
                            inDBRecord.Count = item.Count();
                            _webLeadDataRepository.Save(inDBRecord);
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Error("Exception - " + ex.StackTrace);
                }
                finally
                {
                    _unitOfWork.ResetContext();
                }
                _logService.Info(string.Format("End Updating WebLead data for Week " + week.Date));
            }
        }

        private long? GetFranchiseeNameByPhoneNumber(string phoneNumber)
        {
            var franchiseeList = _organizationRepository.Table.Where(x => x.Phones.Any()
                                   && x.IsActive).ToList();

            var franchiseeId = default(long?);
            foreach (var franchisee in franchiseeList)
            {
                try
                {
                    var phoneNumbers = franchisee.Phones.Select(p => p.Number).ToList();
                    if (phoneNumbers.Contains(phoneNumber))
                    {
                        franchiseeId = franchisee.Id;
                        break;
                    }

                }
                catch
                {
                    franchiseeId = default(long?);
                    break;
                }

            }
            return franchiseeId;
        }
    }
}

