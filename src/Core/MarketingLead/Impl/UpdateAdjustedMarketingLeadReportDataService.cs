using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Impl;
using Core.MarketingLead.Domain;
using Core.MarketingLead.Enum;
using Core.Reports;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class UpdateAdjustedMarketingLeadReportDataService : IUpdateAdjustedMarketingLeadReportDataService
    {
        private readonly IRepository<RoutingNumber> _routingNumberRepository;
        private readonly IRepository<CallDetailData> _callDetailDataRepository;
        private readonly IRepository<WebLeadData> _webLeadDataRepository;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepository;
        private readonly IRepository<WebLead> _webLeadRepository;
        private readonly IProductReportService _productReportService;
        private readonly ILogService _logService;
        private IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private readonly IRepository<AdjustedCallDetailData> _adjustedCallDetailDataRepository;
        private readonly IRepository<Lookup> _lookUpRepository;
        public UpdateAdjustedMarketingLeadReportDataService(IUnitOfWork unitOfWork, IClock clock, ISettings settings, ILogService logService, IProductReportService productReportService)
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
            _adjustedCallDetailDataRepository = unitOfWork.Repository<AdjustedCallDetailData>();
            _lookUpRepository = unitOfWork.Repository<Lookup>();
        }
        public void UpdateData()
        {
            _logService.Info("starting update for Adjusted call details data : Adjusted CallDetails Analysis");
            UpdateCallDetailData();
            _logService.Info("Stop update for call details data : Adjusted CallDetails Analysis");
        }
        private void UpdateCallDetailData()
        {
            var startDate = _clock.UtcNow.AddDays(-4);
            var endDate = _clock.UtcNow;
            if (_settings.UpdateOldData)
            {
                var firstRecord = _marketingLeadCallDetailRepository.Table.OrderBy(x => x.DateAdded).FirstOrDefault();
                if (firstRecord == null)
                    return;

                startDate = firstRecord.DateAdded;
                endDate = _clock.UtcNow;
            }
            _unitOfWork.StartTransaction();

            var phoneLabelList = _routingNumberRepository.Table.Where(x => !x.PhoneLabel.StartsWith("CC-") && !x.PhoneLabel.StartsWith("Local Bus")).ToList();

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
                    var marketingCollection = _marketingLeadCallDetailRepository.Fetch(x => (x.DateAdded.Month == month.Item1 && x.DateAdded.Year == month.Item2)
                                        && x.PhoneLabel.Equals(phoneLabel) && x.DialedNumber.Equals(phoneNumber)).GroupBy(x => x.FranchiseeId);

                    var webLeadsCollection = _marketingLeadCallDetailRepository.Fetch(x => (x.DateAdded.Month == month.Item1 && x.DateAdded.Year == month.Item2)
                                        && x.PhoneLabel.Equals(phoneLabel) && x.DialedNumber.Equals(phoneNumber)).GroupBy(x => x.FranchiseeId);

                    var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = 1, IsNew = true };

                    var domain = new AdjustedCallDetailData
                    {
                        PhoneLabel = phoneLabel,
                        StartDate = firstDayOfMonth,
                        PhoneNumber = phoneNumber,
                        EndDate = lastDayOfMonth,
                        DataRecorderMetaData = dataRecorderMetaData,
                        FrequencyId = (long)AdjustedDataFrequencyType.Monthly,
                        Lookup = _lookUpRepository.Table.Where(x => x.Id == (long)AdjustedDataFrequencyType.Monthly).FirstOrDefault(),
                        IsNew = true
                    };

                    if (!marketingCollection.Any())
                    {
                        var inDBRecord = _adjustedCallDetailDataRepository.Fetch(x => (x.PhoneLabel.Equals(phoneLabel) && x.PhoneNumber.Equals(phoneNumber)
                                            && x.FranchiseeId == null && x.StartDate == firstDayOfMonth
                                            && x.FrequencyId == (long)AdjustedDataFrequencyType.Monthly)).FirstOrDefault();
                        if (inDBRecord == null)
                        {
                            domain.Count = 0;
                            domain.AdjustedData = 0;
                            domain.FranchiseeId = null;
                            _adjustedCallDetailDataRepository.Save(domain);
                        }
                    }

                    foreach (var item in marketingCollection)
                    {
                        int webCountTotal = 0;
                        int webCountForFranchisee = 0;
                        var inDBRecord = _adjustedCallDetailDataRepository.Fetch(x => (x.PhoneLabel.Equals(phoneLabel) && x.PhoneNumber.Equals(phoneNumber)
                                            && x.FranchiseeId == item.Key
                                            && x.StartDate == firstDayOfMonth && x.FrequencyId == (long)AdjustedDataFrequencyType.Monthly)).FirstOrDefault();

                        webCountTotal = _webLeadDataRepository.Table.Where(x => (x.StartDate == firstDayOfMonth && x.EndDate == lastDayOfMonth)).Sum(x=>x.Count);
                        webCountForFranchisee = _webLeadDataRepository.Table.Where(x => (x.StartDate == firstDayOfMonth.Date && x.EndDate == lastDayOfMonth.Date)
                                                   && (item.Key == null || x.FranchiseeId == item.Key)).Sum(x => x.Count);
                        var listCallDetailWithoutCcAndLocal = _marketingLeadCallDetailRepository.Table.Where(x => !x.PhoneLabel.StartsWith("CC-") && !x.PhoneLabel.StartsWith("Local Bus"));

                        if (inDBRecord == null)
                        {
                            domain.Count = item.Count();
                            domain.FranchiseeId = item.Key ?? null;
                            _adjustedCallDetailDataRepository.Save(domain);
                        }
                        else
                        {
                            inDBRecord.Count = item.Count();
                            _adjustedCallDetailDataRepository.Save(inDBRecord);
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
                            inDBRecord.Count = item.Count();
                            _callDetailDataRepository.Save(inDBRecord);
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
        private int getAdjustedData(int value, int totalCount, int ccCount, int localCount, int webCount)
        {
            decimal count = default(decimal);
            if (totalCount != 0)
            {
                decimal devide = (decimal)localCount / (decimal)totalCount;
                devide = Math.Round(devide, 4);
                decimal substract = (1 - (devide));
                if (substract != 0)
                {
                    count = (value) / substract;
                }
                else
                {
                    count = 0;
                }

                //decimal count = (value) / substract;
                count = Math.Round(count, 4);
                decimal websub = webCount + (totalCount - ccCount - localCount);
                if (websub != 0)
                {
                    decimal webDiv = webCount / websub;
                    webDiv = Math.Round(webDiv, 4);
                    count = (count) * (1 + webDiv);
                    return (int)Math.Round(count, 0);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
