using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.MarketingLead.Domain;
using Core.MarketingLead.Enum;
using Core.MarketingLead.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Reports;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class MarketingLeadChartReportService : IMarketingLeadChartReportService
    {
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepository;
        private readonly IRepository<WebLead> _webLeadRepository;
        private readonly IClock _clock;
        private readonly IProductReportService _productReportService;
        private readonly ISettings _seting;
        private readonly IMarketingLeadsFactory _marketingLeadFactory;
        private readonly IRepository<RoutingNumber> _routingNumberRepository;
        private readonly IRepository<CallDetailData> _callDetailDataRepository;
        private readonly IRepository<WebLeadData> _webLeadDataRepository;
        private readonly IRepository<Franchisee> _franchiseeDataRepository;
        private const string frontOfficeName = "FRONT OFFICE(MULTI LEVEL COVERAGE)";
        private const string officePersonName = "OFFICE PERSON";
        private const string respondWhenAvailable = "RESPOND WHEN AVAILABLE";
        private const string respondNextDay = "RESPONDS NEXT DAY";
        private readonly IMarketingLeadsReportService _marketingLeadService;
        public MarketingLeadChartReportService(IUnitOfWork unitOfWork, IClock clock, IProductReportService productReportService,
            ISettings setting, IMarketingLeadsFactory marketingLeadFactory, IMarketingLeadsReportService marketingLeadService)
        {
            _marketingLeadCallDetailRepository = unitOfWork.Repository<MarketingLeadCallDetail>();
            _webLeadRepository = unitOfWork.Repository<WebLead>();
            _clock = clock;
            _productReportService = productReportService;
            _seting = setting;
            _marketingLeadFactory = marketingLeadFactory;
            _routingNumberRepository = unitOfWork.Repository<RoutingNumber>();
            _callDetailDataRepository = unitOfWork.Repository<CallDetailData>();
            _webLeadDataRepository = unitOfWork.Repository<WebLeadData>();
            _franchiseeDataRepository = unitOfWork.Repository<Franchisee>();
            _marketingLeadService = marketingLeadService;
        }
        public MarketingLeadChartListModel GetPhoneVsWebReport(MarketingLeadReportFilter filter)
        {
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            filter.StartDate = filter.StartDate.Value.AddMonths(-1).Date;
            filter.EndDate = filter.StartDate.Value.AddMonths(-12).Date;

            var daysInMonthEndDate = DateTime.DaysInMonth(filter.EndDate.Value.Year, filter.EndDate.Value.Month);
            var firstDayOfEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, 1);
            var daysInMonthStartDate = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);
            var startDateToCompare = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonthStartDate);
            var startDate2 = startDateToCompare.AddDays(1);
            var startDate = _clock.ToUtc(firstDayOfEnd);
            var endDate = _clock.ToUtc(startDate2);

            var totalPhoneLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDate && x.DateAdded <= endDate)
                                   && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text))).ToList();

            var totalWebLeadList = _webLeadRepository.Table.Where(x => (x.CreatedDate >= firstDayOfEnd && x.CreatedDate < startDate2)
                                        && (string.IsNullOrEmpty(filter.URL) || x.URL.Equals(filter.URL))).ToList();

            var localPhoneLeadList = totalPhoneLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var localWebLeadList = totalWebLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var result = GetChartModel(totalPhoneLeadList, totalWebLeadList, localPhoneLeadList, localWebLeadList, startDateToCompare, firstDayOfEnd);

            return new MarketingLeadChartListModel
            {
                ChartData = result.Select(x => _marketingLeadFactory.CreateViewModel(x, null))
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModel(IList<MarketingLeadCallDetail> totalPhoneLeadList, IList<WebLead> totalWebLeadList,
            IList<MarketingLeadCallDetail> localPhoneLeadList, IList<WebLead> localWebLeadList, DateTime startDate, DateTime endDate)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var months = _productReportService.MonthsBetween(startDate, endDate);

            foreach (var item in months)
            {
                var daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                var startDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, 01));
                var endDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, daysInMonth));
                var startDateTimeForWeb = (new DateTime(item.Item2, item.Item1, 01));
                var endDateTimeForWeb = (new DateTime(item.Item2, item.Item1, daysInMonth)).AddDays(1);
                endDateTime = endDateTime.AddDays(1);
                var totalCallLeads = totalPhoneLeadList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);

                var totalWebLeads = totalWebLeadList.Count(x => x.CreatedDate >= startDateTimeForWeb && x.CreatedDate < endDateTimeForWeb);
                var localCallLeads = localPhoneLeadList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var localWebLeads = localWebLeadList.Count(x => x.CreatedDate >= startDateTimeForWeb && x.CreatedDate < endDateTimeForWeb);

                var totalcallLeadsWithAutoDialers = totalPhoneLeadList.Count(x => ((x.TransferToNumber == "") && !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("CORP") && x.DateAdded >= startDateTime && x.DateAdded <= endDateTime));
                var localcallLeadsWithAutoDialers = localPhoneLeadList.Count(x => ((x.TransferToNumber == "") && !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("CORP") && x.DateAdded >= startDateTime && x.DateAdded <= endDateTime));

                totalCallLeads = totalCallLeads - totalcallLeadsWithAutoDialers;
                localCallLeads = localCallLeads - localcallLeadsWithAutoDialers;

                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Item2, item.Item1, daysInMonth),
                    TotalCount = totalCallLeads,
                    Total = totalWebLeads + totalCallLeads,
                    LocalCount = localCallLeads,
                    Local = localWebLeads + localCallLeads
                };
                list.Add(model);
            }
            return list;
        }

        public MarketingLeadChartListModel GetBusVsPhoneReport(MarketingLeadReportFilter filter)
        {
            var categoryIds = _routingNumberRepository.Table.Where(x => (x.CategoryId.Value == (long?)(RoutingNumberCategory.BusinessDirectories))).
                                    Select(y => y.PhoneLabel).ToList();
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;


            filter.StartDate = filter.StartDate.Value.AddMonths(-1).Date;
            filter.EndDate = filter.StartDate.Value.AddMonths(-12).Date;


            var startDate = _clock.ToUtc(filter.StartDate.GetValueOrDefault());
            var endDate = _clock.ToUtc(filter.EndDate.GetValueOrDefault());

            var daysInMonthEndDate = DateTime.DaysInMonth(endDate.Year, endDate.Month);
            var firstDayOfEnd = new DateTime(endDate.Year, endDate.Month, 1);
            var daysInMonthStartDate = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            var startDateToCompare = new DateTime(startDate.Year, startDate.Month, daysInMonthStartDate);

            var endDates = _clock.ToUtc(startDateToCompare).AddDays(1);
            var startDates = _clock.ToUtc(firstDayOfEnd);

            var totalPhoneLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDates && x.DateAdded <= endDates)
                                    && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text))).ToList();

            var totalBusinessDirList = totalPhoneLeadList.Where(x => !categoryIds.Any() || (categoryIds.Contains(x.PhoneLabel))).ToList();

            var localBusinessDirList = totalPhoneLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                       && (!categoryIds.Any() || (categoryIds.Contains(x.PhoneLabel)))).ToList();

            var totalLocalPhoneLeads = totalPhoneLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var result = GetChartModelForBusReview(totalPhoneLeadList, totalBusinessDirList, localBusinessDirList, totalLocalPhoneLeads,
                            startDateToCompare, firstDayOfEnd);

            return new MarketingLeadChartListModel
            {
                ChartData = result.Select(x => _marketingLeadFactory.CreateViewModel(x, null))
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModelForBusReview(IList<MarketingLeadCallDetail> totalPhoneLeadList, IList<MarketingLeadCallDetail> totalBusinessDirList,
           IList<MarketingLeadCallDetail> localBusinessDirList, IList<MarketingLeadCallDetail> totalLocalPhoneLeads, DateTime startDate, DateTime endDate)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var months = _productReportService.MonthsBetween(startDate, endDate);

            foreach (var item in months)
            {
                var daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                var startDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, 01));
                var endDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, daysInMonth));
                endDateTime = endDateTime.AddDays(1);

                var totalPhoneLeads = totalPhoneLeadList.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime);
                var totalBusinessDir = totalBusinessDirList.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime);
                var localPhoneLeads = totalLocalPhoneLeads.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime);
                var localBusinessDir = localBusinessDirList.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime);

                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Item2, item.Item1, daysInMonth),
                    Total = totalPhoneLeads,
                    Local = localPhoneLeads,
                    LocalCount = localBusinessDir,
                    TotalCount = totalBusinessDir
                };
                list.Add(model);
            }
            return list;
        }

        public MarketingLeadChartListModel GetLocalVsNationalReport(MarketingLeadReportFilter filter)
        {
            var nationalUrlString = _seting.NationalUrlString;
            var nationalUrlList = nationalUrlString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var listToCompare = new List<string>();
            foreach (var item in nationalUrlList)
            {
                listToCompare.Add(item.ToUpper().Trim());
            }
            listToCompare.ToList();

            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;


            filter.StartDate = filter.StartDate.Value.AddMonths(-1).Date;
            filter.EndDate = filter.StartDate.Value.AddMonths(-12).Date;

            var daysInMonthEndDate = DateTime.DaysInMonth(filter.EndDate.Value.Year, filter.EndDate.Value.Month);
            var firstDayOfEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, 1);
            var daysInMonthStartDate = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);
            var startDateToCompare = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonthStartDate);
            var startDate2 = startDateToCompare.AddDays(1);
            var startDate = _clock.ToUtc(firstDayOfEnd);
            var endDate = _clock.ToUtc(startDate2);

            var totalWebLeadList = _webLeadRepository.Table.Where(x => (x.CreatedDate >= firstDayOfEnd && x.CreatedDate < startDate2)
                                    && (string.IsNullOrEmpty(filter.URL) || x.URL.Equals(filter.Text))).ToList();

            var totalLocalWebLeadList = totalWebLeadList.Where(x => !listToCompare.Contains(x.URL.ToUpper())).ToList();   //total 1829

            var totalNationalWebLeadList = totalWebLeadList.Where(x => listToCompare.Contains(x.URL.ToUpper())).ToList();  // total 1544

            var territoryNationaolLeadList = totalNationalWebLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();  //total 69

            var territoryLocalLeadList = totalLocalWebLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();  //total 90

            var result = GetChartModelForWebReview(totalLocalWebLeadList, totalNationalWebLeadList, territoryLocalLeadList, territoryNationaolLeadList,
                startDateToCompare, firstDayOfEnd, filter);

            return new MarketingLeadChartListModel
            {
                ChartData = result.Select(x => _marketingLeadFactory.CreateViewModel(x, null))
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModelForWebReview(IList<WebLead> totalLocalWebLeadList, IList<WebLead>
            totalNationalWebLeadList, IList<WebLead> territoryLocalLeadList, IList<WebLead> territoryNationalLeadList,
            DateTime startDate, DateTime endDate, MarketingLeadReportFilter filter)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var months = _productReportService.MonthsBetween(startDate, endDate);
            //totalLocalWebLeadList = totalLocalWebLeadList.Where(x => (x.FranchiseeId == filter.FranchiseeId || filter.FranchiseeId <= 0));
            //totalNationalWebLeadList = totalNationalWebLeadList.Where(x => (x.FranchiseeId == filter.FranchiseeId || filter.FranchiseeId <= 0));
            foreach (var item in months)
            {
                var daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                var startDateTime = (new DateTime(item.Item2, item.Item1, 01));
                var endDateTime = (new DateTime(item.Item2, item.Item1, daysInMonth));
                endDateTime = endDateTime.AddDays(1);
                var totalLocalLeads = totalLocalWebLeadList.Count(x => x.CreatedDate >= startDateTime && x.CreatedDate < endDateTime);
                var totalNationaLeads = totalNationalWebLeadList.Count(x => x.CreatedDate >= startDateTime && x.CreatedDate < endDateTime);
                var territoryLocalLeads = territoryLocalLeadList.Count(x => x.CreatedDate >= startDateTime && x.CreatedDate < endDateTime);
                var territoryNationaLeads = territoryNationalLeadList.Count(x => x.CreatedDate >= startDateTime && x.CreatedDate < endDateTime);
                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Item2, item.Item1, daysInMonth),
                    TotalCount = totalLocalLeads,
                    Total = totalLocalLeads + totalNationaLeads,
                    LocalCount = territoryLocalLeads,
                    Local = territoryLocalLeads + territoryNationaLeads
                };
                list.Add(model);
            }
            return list;
        }

        public MarketingLeadChartListModel GetSpamVsPhoneReport(MarketingLeadReportFilter filter)
        {
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            if (filter.StartDate != null)
            {
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
            }
            if (filter.EndDate.HasValue)
            {
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
            }

            filter.StartDate = filter.StartDate.Value.AddMonths(-1).Date;
            filter.EndDate = filter.StartDate.Value.AddMonths(-12).Date;


            var daysInMonthEndDate = DateTime.DaysInMonth(filter.EndDate.Value.Year, filter.EndDate.Value.Month);
            var firstDayOfEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, 1);
            var daysInMonthStartDate = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);
            var startDateToCompare = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonthStartDate);
            var startDate2 = startDateToCompare.AddDays(1);
            var startDate = _clock.ToUtc(firstDayOfEnd);
            var endDate = _clock.ToUtc(startDate2);

            var totalPhoneLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDate && x.DateAdded <= endDate)
                                    && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text)) && x.PhoneLabel != null).ToList();

            var totalSpamList = totalPhoneLeadList.Where(x => string.IsNullOrEmpty(x.TransferToNumber) && !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("CORP")).ToList();

            var localPhoneLeadList = totalPhoneLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var localSpamList = totalSpamList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var result = GetChartModelForSpam(totalPhoneLeadList, totalSpamList, localPhoneLeadList, localSpamList, startDateToCompare, firstDayOfEnd, filter);

            return new MarketingLeadChartListModel
            {
                ChartData = result.Select(x => _marketingLeadFactory.CreateViewModel(x, 5))
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModelForSpam(IList<MarketingLeadCallDetail> totalPhoneLeadList, IList<MarketingLeadCallDetail> totalSpamList,
            IList<MarketingLeadCallDetail> localPhoneLeadList, IList<MarketingLeadCallDetail> localSpamList, DateTime startDate, DateTime endDate, MarketingLeadReportFilter filter)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var months = _productReportService.MonthsBetween(startDate, endDate);
            //totalPhoneLeadList = totalPhoneLeadList.Where(x => x.FranchiseeId == filter.FranchiseeId || filter.FranchiseeId <= 0);
            foreach (var item in months)
            {
                var daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                var startDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, 01));
                var endDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, daysInMonth));
                endDateTime = endDateTime.AddDays(1);
                var totalPhoneLeads = totalPhoneLeadList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var totalSpams = totalSpamList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var localPhoneLeads = localPhoneLeadList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var localSpams = localSpamList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);

                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Item2, item.Item1, daysInMonth),
                    TotalCount = totalSpams,
                    Total = totalPhoneLeads,
                    LocalCount = localSpams,
                    Local = localPhoneLeads
                };
                list.Add(model);
            }
            return list;
        }

        public CallDetailReportListModel GetSummaryReport(MarketingLeadReportFilter filter)
        {
            var Collection = GetSummary(filter);

            return new CallDetailReportListModel
            {
                Summary = Collection,
            };
        }

        private CallDetailReportViewModel GetSummary(MarketingLeadReportFilter filter)
        {
            const int monthView = 1;
            const int weekView = 2;
            const int dayView = 3;
            const int yearView = 4;

            const int monthCount = -11;
            const int dayCount = -29;
            const int monthsForWeekCount = -3;
            const int yearCount = -13;
            var classTypes = _routingNumberRepository.Table.Where(x => (x.CategoryId.Value == 194)).Select(y => y.PhoneLabel).ToList();


            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            filter.StartDate = filter.StartDate.Value.Date;
            var model = new CallDetailReportViewModel { };
            model.lstHeader = new List<HeaderDataCollection>();

            if (filter.ViewTypeId == yearView)
            {
                filter.EndDate = filter.StartDate.Value.AddYears(yearCount).Date;
                var startDate = _clock.ToUtc(filter.StartDate.GetValueOrDefault());
                var endDate = _clock.ToUtc(filter.EndDate.GetValueOrDefault());

                var yearList = DateRangeHelperService.GetYearsBetween(filter.StartDate.Value, filter.EndDate.Value);

                var listCallDetail = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded <= filter.StartDate && x.DateAdded >= filter.EndDate)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Contains(filter.Text))).ToList();

                var listWebLeads = _webLeadRepository.Table.Where(x => (x.CreatedDate <= filter.StartDate && x.CreatedDate >= filter.EndDate)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (string.IsNullOrEmpty(filter.URL) || x.URL.Contains(filter.URL))).ToList();

                foreach (var year in yearList.OrderBy(x => x))
                {
                    var listCallDetailWithoutCCAndCORP = listCallDetail.Count(i => i.DateAdded.Value.Year == year && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && (i.TransferToNumber != ""));
                    var totalCallLeads = listCallDetail.Count(i => i.DateAdded.Value.Year == year);
                    var totalCCCount = listCallDetail.Count(i => i.DateAdded.Value.Year == year && i.PhoneLabel.StartsWith("CC"));
                    var totalCorpsCount = listCallDetail.Count(i => i.DateAdded.Value.Year == year && i.PhoneLabel.StartsWith("CORP"));
                    var totalWebLeads = listWebLeads.Count(i => i.CreatedDate.Year == year);
                    var busneissCount = listCallDetail.Count(i => i.DateAdded.Value.Year == year && (!classTypes.Any() || classTypes.Contains(i.PhoneLabel)));
                    var totalAutoDailerCount = listCallDetail.Count(i => i.DateAdded.Value.Year == year && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && (i.TransferToNumber == ""));
                    var headerModel = new HeaderDataCollection

                    {
                        HeaderYear = year,
                        CallLead = listCallDetailWithoutCCAndCORP,
                        WebLead = totalWebLeads,
                        Total = totalCallLeads + totalWebLeads,
                        CcCount = totalCCCount,
                        CorpsCount = totalCorpsCount,
                        BusinessCount = busneissCount,
                        TotalCallsDetailsCount = listCallDetailWithoutCCAndCORP + totalCorpsCount + totalCCCount + totalAutoDailerCount,
                        DirectResponseDetailsCount = (listCallDetailWithoutCCAndCORP) - busneissCount,
                        AutoDailerCount = totalAutoDailerCount
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.WebLead);
                model.GrandTotal = model.lstHeader.Sum(x => x.Total);
                model.CCTotal = model.lstHeader.Sum(x => x.CcCount);
                model.CorpsTotal = model.lstHeader.Sum(x => x.CorpsCount);
                model.DirectResponseDetailsTotal = model.lstHeader.Sum(x => x.DirectResponseDetailsCount);
                model.AutoDailerTotal = model.lstHeader.Sum(x => x.AutoDailerCount);
                model.CorpsTotal = model.lstHeader.Sum(x => x.CorpsCount);
                model.TotalCallsDetailsTotal = model.lstHeader.Sum(x => x.TotalCallsDetailsCount);
                model.BusneissTotal = model.lstHeader.Sum(x => x.BusinessCount);
            }

            if (filter.ViewTypeId == monthView)
            {
                filter.StartDate = filter.StartDate.Value.AddDays(1);
                var firstDayOfMonth = new DateTime(filter.StartDate.Value.AddMonths(monthCount).AddDays(-1).Date.Year, filter.StartDate.Value.AddMonths(monthCount).AddDays(-1).Date.Month, 1);
                filter.EndDate = firstDayOfMonth;
                //filter.EndDate = filter.StartDate.Value.AddMonths(monthCount).AddDays(-1).Date;
                var monthDuration = _productReportService.MonthsBetween(filter.StartDate.Value, filter.EndDate.Value);

                var startDate = _clock.ToUtc(filter.StartDate.GetValueOrDefault());
                var endDate = _clock.ToUtc(filter.EndDate.GetValueOrDefault());
                var listCallDetail = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded <= startDate && x.DateAdded >= endDate)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Contains(filter.Text))&& x.PhoneLabel!=null).ToList();

                var listWebLeads = _webLeadRepository.Table.Where(x => (x.CreatedDate < filter.StartDate && x.CreatedDate >= filter.EndDate)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (string.IsNullOrEmpty(filter.URL) || x.URL.Contains(filter.URL))).ToList();

                foreach (var month in monthDuration)
                {

                    var daysInMonth = DateTime.DaysInMonth(month.Item2, month.Item1);
                    var startDateTimeForWeb = (new DateTime(month.Item2, month.Item1, 01));
                    var endDateTimeForWeb = (new DateTime(month.Item2, month.Item1, daysInMonth)).AddDays(1);
                    var startDateTime = _clock.ToUtc(startDateTimeForWeb);
                    var endDateTime = _clock.ToUtc(endDateTimeForWeb);
                    //endDateTime = endDateTime.AddDays(1);
                    var listCallDetailWithoutCCAndCORP = listCallDetail.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && (i.TransferToNumber != ""));
                    var totalCallLeads = listCallDetail.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime);
                    var totalCCCount = listCallDetail.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime && i.PhoneLabel.StartsWith("CC"));
                    var totalAutoDailerCount = listCallDetail.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && (i.TransferToNumber == ""));
                    var totalWebLeads = listWebLeads.Count(i => i.CreatedDate >= startDateTimeForWeb && i.CreatedDate < endDateTimeForWeb);
                    var totalCorpsCount = listCallDetail.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime && i.PhoneLabel.StartsWith("CORP"));
                    var busneissCount = listCallDetail.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime && (!classTypes.Any() || classTypes.Contains(i.PhoneLabel)));
                    var headerModel = new HeaderDataCollection
                    {
                        HeaderMonth = month.Item1,
                        HeaderYear = month.Item2,
                        CallLead = listCallDetailWithoutCCAndCORP,
                        WebLead = totalWebLeads,
                        CcCount = totalCCCount,
                        Total = totalCallLeads + totalWebLeads,
                        CorpsCount = totalCorpsCount,
                        BusinessCount = busneissCount,
                        TotalCallsDetailsCount = listCallDetailWithoutCCAndCORP + totalCorpsCount + totalCCCount + totalAutoDailerCount,
                        DirectResponseDetailsCount = (listCallDetailWithoutCCAndCORP) - busneissCount,
                        AutoDailerCount = totalAutoDailerCount
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.WebLead);
                model.GrandTotal = model.lstHeader.Sum(x => x.Total);
                model.CCTotal = model.lstHeader.Sum(x => x.CcCount);
                model.CorpsTotal = model.lstHeader.Sum(x => x.CorpsCount);
                model.BusneissTotal = model.lstHeader.Sum(x => x.BusinessCount);
                model.TotalCallsDetailsTotal = model.lstHeader.Sum(x => x.TotalCallsDetailsCount);
                model.DirectResponseDetailsTotal = model.lstHeader.Sum(x => x.DirectResponseDetailsCount);
                model.AutoDailerTotal = model.lstHeader.Sum(x => x.AutoDailerCount);
            }

            else if (filter.ViewTypeId == dayView)
            {
                filter.EndDate = filter.StartDate.Value.AddDays(dayCount).AddDays(1).AddTicks(-1);
                var dayCollection = DateRangeHelperService.GetDaysCollection(filter.StartDate.Value, filter.EndDate.Value);

                var listCallDetail = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded <= filter.StartDate && x.DateAdded >= filter.EndDate)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Contains(filter.Text))).ToList();

                var listWebLeads = _webLeadRepository.Table.Where(x => (x.CreatedDate <= filter.StartDate && x.CreatedDate >= filter.EndDate)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (string.IsNullOrEmpty(filter.Text) || x.URL.Contains(filter.URL))).ToList();

                foreach (var day in dayCollection.OrderBy(x => x.Day))
                {
                    var dayToCompare = day.Date;
                    var endDayToCompare = day.Date.AddDays(1).AddTicks(-1);
                    dayToCompare = _clock.ToUtc(dayToCompare);
                    endDayToCompare = _clock.ToUtc(endDayToCompare);
                    var dayToCompareForWeb = day.Date;
                    var endDayToCompareForWeb = day.Date.AddDays(1).AddTicks(-1);
                    var listCallDetailWithoutCCAndCORP = listCallDetail.Count(i => i.DateAdded >= dayToCompare && i.DateAdded <= endDayToCompare && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && (i.TransferToNumber != ""));
                    var totalCCCount = listCallDetail.Count(i => i.DateAdded >= dayToCompare && i.DateAdded <= endDayToCompare && i.PhoneLabel.StartsWith("CC"));
                    var totalCallLeads = listCallDetail.Count(i => i.DateAdded >= dayToCompare && i.DateAdded <= endDayToCompare);
                    var totalWebLeads = listWebLeads.Count(i => i.CreatedDate >= dayToCompareForWeb && i.CreatedDate <= dayToCompareForWeb);
                    var busneissCount = listCallDetail.Count(i => i.DateAdded >= dayToCompare && i.DateAdded <= endDayToCompare && (!classTypes.Any() || classTypes.Contains(i.PhoneLabel)));
                    var totalCorpsCount = listCallDetail.Count(i => i.DateAdded >= dayToCompare && i.DateAdded <= endDayToCompare && i.PhoneLabel.StartsWith("CORP"));
                    var totalAutoDailerCount = listCallDetail.Count(i => i.DateAdded >= dayToCompare && i.DateAdded <= endDayToCompare && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && (i.TransferToNumber == ""));
                    var headerModel = new HeaderDataCollection
                    {
                        Start = day,
                        CallLead = listCallDetailWithoutCCAndCORP,
                        WebLead = totalWebLeads,
                        Total = totalCallLeads + totalWebLeads,
                        CcCount = totalCCCount,
                        BusinessCount = busneissCount,
                        TotalCallsDetailsCount = listCallDetailWithoutCCAndCORP + totalCorpsCount + totalCCCount + totalAutoDailerCount,
                        DirectResponseDetailsCount = listCallDetailWithoutCCAndCORP - busneissCount,
                        AutoDailerCount = totalAutoDailerCount,
                        CorpsCount = totalCorpsCount,
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.WebLead);
                model.GrandTotal = model.lstHeader.Sum(x => x.Total);
                model.CCTotal = model.lstHeader.Sum(x => x.CcCount);
                model.CorpsTotal = model.lstHeader.Sum(x => x.CorpsCount);
                model.DirectResponseDetailsTotal = model.lstHeader.Sum(x => x.DirectResponseDetailsCount);
                model.AutoDailerTotal = model.lstHeader.Sum(x => x.AutoDailerCount);
                model.CorpsTotal = model.lstHeader.Sum(x => x.CorpsCount);
                model.TotalCallsDetailsTotal = model.lstHeader.Sum(x => x.TotalCallsDetailsCount);
                model.BusneissTotal = model.lstHeader.Sum(x => x.BusinessCount);
            }

            else if (filter.ViewTypeId == weekView)
            {
                filter.EndDate = filter.StartDate.Value.AddMonths(monthsForWeekCount);
                var startDateWithUtc = _clock.ToUtc(filter.StartDate.GetValueOrDefault());
                var endDateWithUtc = _clock.ToUtc(filter.EndDate.GetValueOrDefault());
                var weekCollection = DateRangeHelperService.DayOfWeekCollection(endDateWithUtc, startDateWithUtc);
                var firstDateToCompare = weekCollection.FirstOrDefault() != null ? weekCollection.FirstOrDefault().AddDays(-6) : filter.StartDate;


                var lastDateToCompare = weekCollection.LastOrDefault() != null ? weekCollection.LastOrDefault().Date.AddDays(1).AddTicks(-1) : filter.EndDate;
                var listCallDetail = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= firstDateToCompare && x.DateAdded <= lastDateToCompare)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Contains(filter.Text))).ToList();

                var listWebLeads = _webLeadRepository.Table.Where(x => (x.CreatedDate >= firstDateToCompare && x.CreatedDate <= lastDateToCompare)
                                         && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                         && (string.IsNullOrEmpty(filter.URL) || x.URL.Contains(filter.URL))).ToList();

                if (!listCallDetail.Any())
                    return model;
                foreach (var day in weekCollection)
                {
                    var startDayForWeb = day.Date.AddDays(-6);
                    var endDayForWeb = day.Date.AddDays(1).AddTicks(-1);
                    var startDate = _clock.ToUtc(day.Date.AddDays(-6));
                    var endDate = _clock.ToUtc(day.Date.AddDays(1).AddTicks(-1));
                    var listCallDetails = listCallDetail.Where(i => i.DateAdded >= startDate && i.DateAdded <= endDate).ToList();
                    var totalPhoneLeads = listCallDetail.Count(i => i.DateAdded >= startDate && i.DateAdded <= endDate);
                    var totalCallLeads = listCallDetail.Count(i => i.DateAdded >= startDate && i.DateAdded <= endDate);
                    var totalCCCount = listCallDetail.Count(i => i.DateAdded >= startDate && i.DateAdded <= endDate && i.PhoneLabel.StartsWith("CC"));
                    var totalWebLeads = listWebLeads.Count(i => i.CreatedDate >= startDayForWeb && i.CreatedDate <= endDayForWeb);
                    var listCallDetailWithoutCCAndCORP = listCallDetail.Count(i => i.DateAdded >= startDate && i.DateAdded <= endDate && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && i.TransferToNumber != "");
                    var totalCorpsCount = listCallDetail.Count(i => i.DateAdded >= startDate && i.DateAdded <= endDate && i.PhoneLabel.StartsWith("CORP"));
                    var busneissCount = listCallDetails.Count(i => (!classTypes.Any() || classTypes.Contains(i.PhoneLabel)));
                    var totalAutoDailerCount = listCallDetail.Count(i => i.DateAdded >= startDate && i.DateAdded <= endDate && !i.PhoneLabel.StartsWith("CC") && !i.PhoneLabel.StartsWith("CORP") && (i.TransferToNumber == ""));
                    var headerModel = new HeaderDataCollection
                    {
                        Start = day.Date.AddDays(-6),
                        End = day.Date,
                        CallLead = listCallDetailWithoutCCAndCORP,
                        WebLead = totalWebLeads,
                        Total = totalPhoneLeads + totalWebLeads,
                        CcCount = totalCCCount,
                        CorpsCount = totalCorpsCount,
                        TotalCallsDetailsCount = listCallDetailWithoutCCAndCORP + totalCorpsCount + totalCCCount + totalAutoDailerCount,
                        BusinessCount = busneissCount,
                        DirectResponseDetailsCount = (listCallDetailWithoutCCAndCORP) - busneissCount,
                        AutoDailerCount = totalAutoDailerCount
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.WebLead);
                model.GrandTotal = model.lstHeader.Sum(x => x.Total);
                model.CCTotal = model.lstHeader.Sum(x => x.CcCount);
                model.CorpsTotal = model.lstHeader.Sum(x => x.CorpsCount);
                model.DirectResponseDetailsTotal = model.lstHeader.Sum(x => x.DirectResponseDetailsCount);
                model.AutoDailerTotal = model.lstHeader.Sum(x => x.AutoDailerCount);
                model.CorpsTotal = model.lstHeader.Sum(x => x.CorpsCount);
                model.TotalCallsDetailsTotal = model.lstHeader.Sum(x => x.TotalCallsDetailsCount);
                model.BusneissTotal = model.lstHeader.Sum(x => x.BusinessCount);
            }
            return model;
        }

        public MarketingLeadChartListModel GetLocalVsNationalPhoneReport(MarketingLeadReportFilter filter)
        {
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            if (filter.StartDate.Value.Date == _clock.UtcNow.Date)
                filter.StartDate = filter.StartDate.Value.Date.AddDays(-1);

            filter.StartDate = _clock.ToUtc(filter.StartDate.GetValueOrDefault());
            filter.EndDate = _clock.ToUtc(filter.EndDate.GetValueOrDefault());

            filter.StartDate = filter.StartDate.Value.Date.AddDays(1).AddTicks(-1);
            filter.EndDate = filter.StartDate.Value.AddDays(-6).Date;



            var startDate = _clock.ToUtc(filter.StartDate.GetValueOrDefault());
            var endDate = _clock.ToUtc(filter.EndDate.GetValueOrDefault());

            var totalCallLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded <= startDate && x.DateAdded >= endDate)
                         && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text))).ToList();

            var localCallLeadList = totalCallLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            //var nationalCallLeadList = totalCallLeadList.Where(x => x.FranchiseeId == null);
            var nationalCallLeadList = totalCallLeadList;


            var result = GetChartModelForPhoneReview(localCallLeadList, nationalCallLeadList,
                filter.StartDate.Value, filter.EndDate.Value, filter);

            return new MarketingLeadChartListModel
            {
                ChartData = result.Select(x => _marketingLeadFactory.CreateViewModel(x, 1))
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModelForPhoneReview(IList<MarketingLeadCallDetail> localCallLeadList, IList<MarketingLeadCallDetail>
            nationalCallLeadList, DateTime startDate, DateTime endDate, MarketingLeadReportFilter filter)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var dayCollection = DateRangeHelperService.GetDaysCollection(startDate, endDate);

            foreach (var item in dayCollection.OrderBy(x => x.Date))
            {
                var dayToCompare = _clock.ToUtc(item.Date);
                var endDayToComare = _clock.ToUtc(item.Date).AddDays(1).AddTicks(-1);

                var totalLeads = nationalCallLeadList.Count(x => x.DateAdded >= dayToCompare && x.DateAdded <= endDayToComare);

                var totalLocalLeads = localCallLeadList.Count(x => x.DateAdded >= dayToCompare && x.DateAdded <= endDayToComare);

                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day),
                    TotalCount = totalLeads,
                    Total = nationalCallLeadList.Count(),
                    LocalCount = totalLocalLeads,
                    Local = localCallLeadList.Count()
                };
                list.Add(model);
            }
            return list;
        }

        public MarketingLeadChartListModel GetDailyPhoneReport(MarketingLeadReportFilter filter)
        {
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow.Date.AddDays(-1).Date;

            if (filter.StartDate.Value.Date == _clock.UtcNow.Date)
                filter.StartDate = filter.StartDate.Value.AddDays(-1).Date;

            var selectedDate = filter.StartDate.Value.Date;
            filter.StartDate = _clock.ToUtc(selectedDate.Date);
            var startDate = _clock.ToUtc(selectedDate.Date).AddDays(-7);
            filter.EndDate = _clock.ToUtc(selectedDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59));

            var totalCallLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDate && x.DateAdded <= filter.EndDate)
                         && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text))).ToList();


            var localCallLeadList = totalCallLeadList.Where(x => x.FranchiseeId == filter.FranchiseeId || filter.FranchiseeId <= 0).ToList();

            var nationalCallLeadList = totalCallLeadList;

            var territoryLocalLeadList = localCallLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId));

            var result = GetChartModelForDailyReport(localCallLeadList, nationalCallLeadList,
                filter.StartDate.Value, filter.EndDate.Value);
            var chart = result.Select(x => _marketingLeadFactory.CreateViewModel(x, 2)).OrderBy(x => x.Date);

            return new MarketingLeadChartListModel
            {
                ChartData = chart
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModelForDailyReport(IList<MarketingLeadCallDetail> localCallLeadList, IList<MarketingLeadCallDetail>
            nationalCallLeadList, DateTime startDate, DateTime endDate)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var hours = DateRangeHelperService.GetHoursCollection(startDate, endDate);
            var startDateForLast7Days = startDate.AddDays(-6);
            var perLocalData = (decimal)0;
            var pernationalData = (decimal)0;
            var totalCallPerTime = (decimal)0;
            var dateForGraph = default(DateTime);
            foreach (var item in hours.OrderBy(x => x.TimeOfDay))
            {
                totalCallPerTime = 0;
                pernationalData = (decimal)0;
                totalCallPerTime = (decimal)0;
                perLocalData = (decimal)0;
                for (var item2 = startDateForLast7Days; item2 <= startDate; item2 = item2.AddDays(1))
                {
                    var tim2e = item.TimeOfDay;
                    var time = item2.Date + new TimeSpan(tim2e.Ticks);

                    var startToCompare = ((time).AddHours(-1));
                    var endToCompare = ((time));

                    dateForGraph = endToCompare;

                    var startToCompareForNational = _clock.ToUtc((time).Date);
                    var endToCompareForNational = _clock.ToUtc((time).Date.AddDays(1));


                    var totalLocalLeads = localCallLeadList.Count(x => x.DateAdded > startToCompare && x.DateAdded <= endToCompare);
                    var totalLeads = nationalCallLeadList.Count(x => x.DateAdded > startToCompare && x.DateAdded <= endToCompare);
                    var nationalCallLeadListForEachMonth = nationalCallLeadList.Count(x => x.DateAdded > startToCompareForNational && x.DateAdded <= endToCompareForNational);
                    var localCallLeadListForEachMonth = localCallLeadList.Count(x => x.DateAdded > startToCompareForNational && x.DateAdded <= endToCompareForNational);
                    var localCallLeadListForEachHour = localCallLeadList.Count(x => x.DateAdded > startToCompare && x.DateAdded <= endToCompare);

                    perLocalData += localCallLeadListForEachMonth != 0 ? Math.Round(((decimal)totalLocalLeads / (decimal)localCallLeadListForEachMonth) * 100, 2) : 0;
                    pernationalData += nationalCallLeadListForEachMonth != 0 ? Math.Round(((decimal)totalLeads / (decimal)nationalCallLeadListForEachMonth) * 100, 2) : 0;
                    totalCallPerTime += localCallLeadListForEachHour;
                }

                var model = new MarketingLeadChartViewModel
                {
                    Date = (item),
                    TotalCount = (int)Math.Ceiling(pernationalData / 7),
                    Total = (int)Math.Ceiling(totalCallPerTime / 7),
                    LocalCount = (int)Math.Ceiling(perLocalData / 7),
                    Local = localCallLeadList.Count(),
                    DateForGraph = dateForGraph
                };
                list.Add(model);
            }
            return list.OrderBy(x => x.Date).ToList();
        }

        public MarketingLeadChartListModel GetSeasonalLeadReport(MarketingLeadReportFilter filter)
        {
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            if (filter.StartDate.Value.Date == _clock.UtcNow.Date)
                filter.StartDate = filter.StartDate.Value.Date.AddMonths(-1);

            var daysInMonth = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);
            var startDate = _clock.ToUtc(new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonth)).AddDays(1);

            filter.EndDate = filter.StartDate.Value.AddMonths(-11).Date;
            var endDateWebLead = new DateTime(filter.StartDate.Value.AddYears(-5).Year, filter.StartDate.Value.Month, 1).Date;

            var startDateWebLead = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonth).AddDays(1).Date;

            var endDate = _clock.ToUtc(new DateTime(filter.StartDate.Value.AddYears(-4).Year, filter.StartDate.Value.Month, 1).Date.AddHours(23)
                .AddMinutes(59).AddSeconds(59));

            var routingNumberList = GetRouitngNumbers();

            var totalLeads = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= endDate && x.DateAdded <= startDate)
                                                                    && (routingNumberList.Contains(x.PhoneLabel) || string.IsNullOrEmpty(x.TransferToNumber)) && x.PhoneLabel != null).ToList();
            //var totalLeads = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= endDate && x.DateAdded <= startDate));

            var autoDialerColletion = totalLeads.Where(x => (string.IsNullOrEmpty(x.TransferToNumber)) &&
                 (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId) && !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("CORP")).ToList();

            var webLeadColletion = _webLeadRepository.Table.Where(x => (x.CreatedDate >= endDateWebLead && x.CreatedDate <= startDateWebLead)
                                        && (string.IsNullOrEmpty(filter.URL) || filter.URL.Equals(x.URL)) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();


            var printMediaLabels = GetPhoneLabelsByCategory((long)RoutingNumberCategory.PrintMedia);

            var printMediaColletion = totalLeads.Where(x => (printMediaLabels.Contains(x.PhoneLabel)) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var businessDirectoriesLabels = GetPhoneLabelsByCategory((long)RoutingNumberCategory.BusinessDirectories);

            var businessDirectoriesColletion = totalLeads.Where(x => (businessDirectoriesLabels.Contains(x.PhoneLabel)) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var webLocalLabels = GetPhoneLabelsByCategory((long)RoutingNumberCategory.PhoneWebLocal);

            var webLocalCollection = totalLeads.Where(x => (webLocalLabels.Contains(x.PhoneLabel)) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var webNationalLabels = GetPhoneLabelsByCategory((long)RoutingNumberCategory.PhoneWebNational);

            var webNationalCollection = totalLeads.Where(x => (webNationalLabels.Contains(x.PhoneLabel)) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var vanLabels = GetPhoneLabelsByCategory((long)RoutingNumberCategory.WRAPVan);

            var vanCollection = totalLeads.Where(x => (vanLabels.Contains(x.PhoneLabel)) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var result = GetChartModelForSeasonalReport(totalLeads, printMediaColletion, businessDirectoriesColletion, webLocalCollection, webNationalCollection,
               autoDialerColletion, vanCollection, webLeadColletion, filter.StartDate.Value, filter.EndDate.Value, filter);

            return new MarketingLeadChartListModel
            {
                ChartData = result
            };
        }

        private IList<string> GetPhoneLabelsByCategory(long categoryId)
        {
            return _routingNumberRepository.Table.Where(x => x.CategoryId == categoryId).Select(x => x.PhoneLabel).ToList();
        }

        private IList<string> GetRouitngNumbers()
        {
            return _routingNumberRepository.Table.Where(x => x.CategoryId == (long)RoutingNumberCategory.PrintMedia
                                                || x.CategoryId == (long)RoutingNumberCategory.BusinessDirectories
                                                || x.CategoryId == (long)RoutingNumberCategory.PhoneWebLocal
                                                || x.CategoryId == (long)RoutingNumberCategory.PhoneWebNational
                                                || x.CategoryId == (long)RoutingNumberCategory.WRAPVan).Select(y => y.PhoneLabel).ToList();
        }

        private List<MarketingLeadChartViewModel> GetChartModelForSeasonalReport(IList<MarketingLeadCallDetail> totalLeads, IList<MarketingLeadCallDetail> printMediaColletion, IList<MarketingLeadCallDetail> businessDirectoriesColletion,
            IList<MarketingLeadCallDetail> webLocalCollection, IList<MarketingLeadCallDetail> webNationalCollection, IList<MarketingLeadCallDetail> autoDialerColletion,
            IList<MarketingLeadCallDetail> vanCollection, IList<WebLead> webLeadColletion, DateTime startDate, DateTime endDate, MarketingLeadReportFilter filter)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var months = _productReportService.MonthsBetween(startDate, endDate);

            foreach (var item in months)
            {
                var startDateTimeForLastYear = _clock.ToUtc(new DateTime(item.Item2 - 1, 1, 01));
                var endDateTimeForLastYear = _clock.ToUtc(new DateTime(item.Item2 - 1, 12, 31)).AddDays(1);
                var startDateTimeForWebLastYear = new DateTime(item.Item2 - 1, 1, 01).Date;
                var endDateTimeForWebLastYear = new DateTime(item.Item2 - 1, 12, 31).AddDays(1).Date;

                var daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                var daysInMonthForLastYear = DateTime.DaysInMonth(item.Item2 - 1, item.Item1);
                var startDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, 01));
                var endDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, daysInMonth));
                var startDateTimeForWebLead = (new DateTime(item.Item2, item.Item1, 01)).Date;
                var endDateTimeForWebLead = (new DateTime(item.Item2, item.Item1, daysInMonth)).Date;
                endDateTime = endDateTime.AddDays(1);

                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Item2, item.Item1, daysInMonth).Date,
                    DateString = new DateTime(item.Item2 - 1, item.Item1, daysInMonthForLastYear).ToString("MMM, yyyy"),
                    LastYearDateString = ((item.Item2 - 1) + ", " + (item.Item2 - 2) + ", " + (item.Item2 - 3)).ToString(),
                    WebLeadCount = webLeadColletion.Count(i => i.CreatedDate >= startDateTimeForWebLead && i.CreatedDate <= endDateTimeForWebLead),
                    AutoDialerCount = autoDialerColletion.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime),
                    PrintMediaCount = printMediaColletion.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime),
                    BusinessDirectoriesCount = businessDirectoriesColletion.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime),
                    WebLocalCount = webLocalCollection.Count(i => i.DateAdded >= startDateTimeForWebLead && i.DateAdded <= endDateTimeForWebLead),
                    WebNationalCount = webNationalCollection.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime),
                    VanCount = vanCollection.Count(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime),
                };
                // changes so that bar should move with lines according to Franchisee
                totalLeads = totalLeads.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
                webLeadColletion = webLeadColletion.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
                int counts = totalLeads.Where(x => (x.FranchiseeId == filter.FranchiseeId || filter.FranchiseeId <= 0)).Count();
                var date = totalLeads.OrderByDescending(x => x.DateAdded).Where(i => i.DateAdded >= startDateTime && i.DateAdded <= endDateTime).Select(x => x.CallerId).Distinct();
                model.LocalCount = (totalLeads.Count(i => (i.DateAdded.Value.Month == item.Item1) && (i.DateAdded >= startDateTimeForLastYear && i.DateAdded <= endDateTimeForLastYear)) +
                            webLeadColletion.Count(i => i.CreatedDate.Month == item.Item1 && (i.CreatedDate >= startDateTimeForWebLastYear && i.CreatedDate <= endDateTimeForWebLastYear)));
                model.TotalCount = (totalLeads.Count(x => x.DateAdded.Value.Month == item.Item1 && (x.DateAdded.Value.Year <= item.Item2 && x.DateAdded.Value.Year >= endDateTimeForLastYear.Year - 2))
                            + webLeadColletion.Count(x => x.CreatedDate.Month == item.Item1 && (x.CreatedDate.Year <= item.Item2 && x.CreatedDate.Year >= endDateTimeForWebLastYear.Year - 2))) / 3;
                list.Add(model);
            }
            return list;
        }

        public CallDetailReportListModel GetAdjustedSummaryReport(MarketingLeadReportFilter filter)
        {
            var Collection = GetAdjustedSummary(filter);

            return new CallDetailReportListModel
            {
                Summary = Collection,
            };
        }

        private CallDetailReportViewModel GetAdjustedSummary(MarketingLeadReportFilter filter)
        {
            const int monthView = 1;
            const int weekView = 2;
            const int dayView = 3;
            const int yearView = 4;

            const int monthCount = -11;
            const int dayCount = -29;
            const int monthsForWeekCount = -3;
            const int yearCount = -13;

            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            filter.StartDate = filter.StartDate.Value.Date;

            var model = new CallDetailReportViewModel { };
            model.lstHeader = new List<HeaderDataCollection>();


            var phoneLabelList = _routingNumberRepository.Fetch(x => (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text))).ToList();
            phoneLabelList = phoneLabelList.Where(x => !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("Local Bus")).ToList();

            if (filter.ViewTypeId == yearView)
            {
                int webCount = 0;
                filter.EndDate = filter.StartDate.Value.AddYears(yearCount).Date;
                var listWebLead = _webLeadDataRepository.Table.Where(x => (x.StartDate <= filter.StartDate && x.EndDate >= filter.EndDate)
                    && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                    && !x.IsWeekly).ToList();
                var yearList = DateRangeHelperService.GetYearsBetween(filter.StartDate.Value, filter.EndDate.Value);

                var listCallDetail = _callDetailDataRepository.Table.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                         && (x.StartDate >= filter.EndDate)
                                         && !x.IsWeekly).AsEnumerable().GroupBy(x => new { x.PhoneLabel, x.StartDate.Year }).Select(x => new
                                         {
                                             x.Key.PhoneLabel,
                                             Count = x.Sum(y => y.Count),
                                             PhoneNumber = x.Select(y => y.PhoneNumber).
                                         FirstOrDefault(),
                                             StartDate = x.Select(y => y.StartDate).FirstOrDefault()
                                         }).ToList();

                foreach (var year in yearList.OrderBy(x => x))
                {
                    var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC"));
                    var ccCount = listCallDetail.Where(i => i.StartDate.Year == year && i.PhoneLabel.StartsWith("CC")).Sum(i => i.Count);
                    var localCount = listCallDetail.Where(i => i.StartDate.Year == year && i.PhoneLabel.StartsWith("Local Bus")).Sum(i => i.Count);
                    var totalCount = listCallDetailWithoutCcAndLocal.Where(i => i.StartDate.Year == year).Sum(i => i.Count);
                    var value = listCallDetailWithoutCcAndLocal.Where(i => i.StartDate.Year == year).Sum(i => i.Count);
                    var lists = listWebLead.Where(i => i.StartDate.Year == year).ToList();
                    if (lists.Count > 0)
                    {
                        webCount = lists.Where(i => i.StartDate.Year == year).Sum(i => i.Count);
                    }
                    else
                    {
                        webCount = 0;
                    }
                    int count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);

                    var headerModel = new HeaderDataCollection
                    {
                        HeaderYear = year,
                        adjustedData = count,
                        DirectResponseCount = count - localCount
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.GrandTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.DirectResponseTotal = model.lstHeader.Sum(x => x.DirectResponseCount);
            }

            else if (filter.ViewTypeId == monthView)
            {
                int webCount = 0;
                var firstDayOfMonth = new DateTime(filter.StartDate.Value.AddMonths(monthCount).AddDays(-1).Date.Year, filter.StartDate.Value.AddMonths(monthCount).AddDays(-1).Date.Month, 1);
                filter.EndDate = firstDayOfMonth;
                var listWebLead = _webLeadDataRepository.Table.Where(x => (x.StartDate <= filter.StartDate && x.EndDate >= filter.EndDate)
                    && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                    && !x.IsWeekly).ToList();
                var monthDuration = _productReportService.MonthsBetween(filter.StartDate.Value, filter.EndDate.Value);
                var listCallDetail = _callDetailDataRepository.Table.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && (x.StartDate >= filter.EndDate)
                                          && !x.IsWeekly).AsEnumerable().GroupBy(x => new { x.PhoneLabel, x.StartDate.Month }).
                                          Select(x => new
                                          {
                                              x.Key.PhoneLabel,
                                              Count = x.Sum(y => y.Count),
                                              PhoneNumber = x.Select(y => y.PhoneNumber).
                                          FirstOrDefault(),
                                              StartDate = x.Select(y => y.StartDate).FirstOrDefault()
                                          }).ToList();


                var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC")).ToList();
                var listCallDetailWithCcAndLocal = listCallDetail.Where(x => x.PhoneLabel.StartsWith("CC") || x.PhoneLabel.StartsWith("Local Bus")).ToList();

                foreach (var month in monthDuration)
                {
                    var row = listCallDetailWithCcAndLocal.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2);
                    var ccCount = row.Where(i => i.PhoneLabel.StartsWith("CC-")).Sum(i => i.Count);
                    var localCount = row.Where(i => i.PhoneLabel.StartsWith("Local Bus")).Sum(i => i.Count);
                    var totalCount = listCallDetailWithoutCcAndLocal.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);
                    var value = listCallDetailWithoutCcAndLocal.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);
                    webCount = listWebLead.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);

                    int count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);
                    //int Count1 = collection.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);
                    var headerModel = new HeaderDataCollection
                    {
                        HeaderMonth = month.Item1,
                        HeaderYear = month.Item2,
                        adjustedData = count,
                        DirectResponseCount = count - localCount
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.GrandTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.DirectResponseTotal = model.lstHeader.Sum(x => x.DirectResponseCount);

            }

            else if (filter.ViewTypeId == dayView)
            {
                int webCount = 0;
                filter.EndDate = filter.StartDate.Value.AddDays(dayCount).AddDays(1).AddTicks(-1);
                var listWebLead = _webLeadRepository.Table.Where(x => (x.CreatedDate <= filter.StartDate && x.CreatedDate >= filter.EndDate)
                    && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
                var dayCollection = DateRangeHelperService.GetDaysCollection(filter.StartDate.Value, filter.EndDate.Value);

                var listCallDetail = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded <= filter.StartDate && x.DateAdded >= filter.EndDate)
                                           && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

                foreach (var day in dayCollection.OrderBy(x => x.Day))
                {
                    var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("CC -"));
                    var ccCount = listCallDetail.Count(i => i.DateAdded.Value.Date == day.Date && i.PhoneLabel.StartsWith("CC"));
                    var localCount = listCallDetail.Count(i => i.DateAdded.Value.Date == day.Date && i.PhoneLabel.StartsWith("Local Bus"));
                    var totalCount = listCallDetailWithoutCcAndLocal.Count(i => i.DateAdded.GetValueOrDefault().Date == day.Date);
                    var value = listCallDetailWithoutCcAndLocal.Count(i => i.DateAdded.GetValueOrDefault().Date == day.Date);
                    var lists = listWebLead.Where(i => i.CreatedDate.Date == day.Date).ToList();
                    if (lists.Count > 0)
                    {
                        webCount = lists.Where(i => i.CreatedDate == day.Date).Count();
                    }
                    else
                    {
                        webCount = 0;
                    }
                    int count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);
                    var headerModel = new HeaderDataCollection
                    {
                        Start = day,
                        //Count = collection.Count(i => i.DateAdded.Date == day.Date)
                        adjustedData = count,
                        DirectResponseCount = count - localCount
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.GrandTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.DirectResponseTotal = model.lstHeader.Sum(x => x.DirectResponseCount);
            }

            else if (filter.ViewTypeId == weekView)
            {
                filter.EndDate = filter.StartDate.Value.AddMonths(monthsForWeekCount);
                int webCount = 0;
                var listWebLead = _webLeadDataRepository.Table.Where(x => (x.StartDate <= filter.StartDate && x.EndDate >= filter.EndDate)
                    && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                    && x.IsWeekly).ToList();
                var weekCollection = DateRangeHelperService.DayOfWeekCollection(filter.EndDate.Value, filter.StartDate.Value);
                var firstDateToCompare = weekCollection.FirstOrDefault() != null ? weekCollection.FirstOrDefault().AddDays(-6) : filter.StartDate;
                var lastDateToCompare = weekCollection.LastOrDefault() != null ? weekCollection.LastOrDefault().AddDays(1).AddTicks(-1) : filter.EndDate;

                var listCallDetail = _callDetailDataRepository.Table.Where(x => (x.StartDate >= firstDateToCompare && x.EndDate <= lastDateToCompare)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && x.IsWeekly).ToList();
                if (!listCallDetail.Any())
                    return model;

                var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC")).ToList();
                var listCallDetailWithCcAndLocal = listCallDetail.Where(x => x.PhoneLabel.StartsWith("CC") || x.PhoneLabel.StartsWith("Local Bus") && !x.PhoneLabel.StartsWith("CC- ")).ToList();

                foreach (var day in weekCollection)
                {
                    DateTime dayToCompare = day.Date.AddDays(-6);
                    var row = listCallDetailWithCcAndLocal.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date);
                    var ccCount = row.Where(i => i.PhoneLabel.StartsWith("CC")).Sum(i => i.Count);
                    var localCount = row.Where(i => i.PhoneLabel.StartsWith("Local Bus")).Sum(i => i.Count);
                    var listCallDetailWithoutCcAndLocalInList = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("Local Bus") && !x.PhoneLabel.StartsWith("CC- ")
                                                                     && x.StartDate == dayToCompare && x.EndDate == day.Date).ToList();
                    var totalCount = listCallDetailWithoutCcAndLocal.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date && i.StartDate == dayToCompare && i.EndDate == day.Date).Sum(i => i.Count);
                    var lists = listWebLead.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date).ToList();
                    if (lists.Count > 0)
                        webCount = lists.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date).Sum(i => i.Count);
                    else
                        webCount = 0;
                    var value = listCallDetailWithoutCcAndLocal.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date).Sum(i => i.Count);
                    int count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);
                    var headerModel = new HeaderDataCollection
                    {
                        Start = day.Date.AddDays(-6),
                        End = day.Date,
                        adjustedData = count,
                        DirectResponseCount = count - localCount
                    };
                    model.lstHeader.Add(headerModel);
                }
                model.PhoneLeadTotal = model.lstHeader.Sum(x => x.CallLead);
                model.WebLeadTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.GrandTotal = model.lstHeader.Sum(x => x.adjustedData);
                model.DirectResponseTotal = model.lstHeader.Sum(x => x.DirectResponseCount);
            }

            return model;
        }

        private int getAdjustedData(int value, int totalCount, int ccCount, int localCount, int webCount)
        {
            decimal count = default(decimal);
            if (totalCount != 0 && value != 0)
            {
                decimal devide = (decimal)localCount / ((decimal)totalCount);
                // devide = Math.Round(devide, 4);
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
                //  count = Math.Round(count, 4);
                decimal websub = webCount + (totalCount);
                if (websub != 0)
                {
                    decimal webDiv = webCount / websub;
                    //      webDiv = Math.Round(webDiv, 4);
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

        public MarketingLeadChartListModel GetCallDetailsReport(MarketingLeadReportFilter filter)
        {
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            if (filter.StartDate != null)
            {
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
            }
            if (filter.EndDate.HasValue)
            {
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
            }

            filter.StartDate = filter.StartDate.Value.AddMonths(-1).Date;
            filter.EndDate = filter.StartDate.Value.AddMonths(-12).Date;


            var daysInMonthEndDate = DateTime.DaysInMonth(filter.EndDate.Value.Year, filter.EndDate.Value.Month);
            var firstDayOfEnd = new DateTime(filter.EndDate.Value.Year, filter.EndDate.Value.Month, 1);
            var daysInMonthStartDate = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);
            var startDateToCompare = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonthStartDate);
            var startDate2 = startDateToCompare.AddDays(1);
            var startDate = _clock.ToUtc(firstDayOfEnd);
            var endDate = _clock.ToUtc(startDate2);

            var totalPhoneLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDate && x.DateAdded <= endDate)
                                    && (x.TransferToNumber != "")
                                    && (!x.PhoneLabel.StartsWith("CC")) && !x.PhoneLabel.StartsWith("CORP")).ToList();


            var totalCallOver2minList = totalPhoneLeadList.Where(x => x.CallDuration >= 2).ToList();
            var totalCallUnder2minList = totalPhoneLeadList.Where(x => x.CallDuration < 2).ToList();

            var localPhoneLeadList = totalPhoneLeadList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var localCallOver2minList = totalCallOver2minList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var localCallUnder2minList = totalCallUnder2minList.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var result = GetChartModelForCall(totalPhoneLeadList, totalCallOver2minList, localPhoneLeadList, localCallOver2minList, totalCallUnder2minList, localCallUnder2minList, startDateToCompare, firstDayOfEnd, filter);

            return new MarketingLeadChartListModel
            {
                ChartData = result
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModelForCall(IList<MarketingLeadCallDetail> totalPhoneLeadList, IList<MarketingLeadCallDetail> totalCallOver2minList,
            IList<MarketingLeadCallDetail> localPhoneLeadList, IList<MarketingLeadCallDetail> localCallOver2minList, IList<MarketingLeadCallDetail> totalCallUnder2minList, IList<MarketingLeadCallDetail> localCallUnder2minList, DateTime startDate, DateTime endDate, MarketingLeadReportFilter filter)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var months = _productReportService.MonthsBetween(startDate, endDate);
            //totalPhoneLeadList = totalPhoneLeadList.Where(x => x.FranchiseeId == filter.FranchiseeId || filter.FranchiseeId <= 0);
            foreach (var item in months)
            {

                var daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                var daysInMonthForLastYear = DateTime.DaysInMonth(item.Item2 - 1, item.Item1);
                var startDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, 01));
                var endDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, daysInMonth));
                endDateTime = endDateTime.AddDays(1);
                var totalPhoneLeads = totalPhoneLeadList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var localCallUnder2min = localCallUnder2minList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var localCallOver2min = localCallOver2minList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var localPhoneLeads = localPhoneLeadList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var totalCallOver2min = totalCallOver2minList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var totalCallUnder2min = totalCallUnder2minList.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var callUnder2minNational = totalPhoneLeads != 0 ? Math.Round(((decimal)totalCallOver2min / totalPhoneLeads) * 100, 2) : 0;
                var callUnder2minLocal = localPhoneLeads != 0 ? Math.Round(((decimal)localCallOver2min / localPhoneLeads) * 100, 2) : 0;


                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Item2, item.Item1, daysInMonth).Date,
                    DateString = new DateTime(item.Item2 - 1, item.Item1, daysInMonthForLastYear).ToString("MMM, yyyy"),
                    TotalCount = callUnder2minNational,
                    Total = totalPhoneLeads,
                    LocalCount = callUnder2minLocal,
                    Local = localPhoneLeads,
                    CallOver2min = localCallOver2min,
                    CallUnder2min = localCallUnder2min
                };
                list.Add(model);
            }
            return list;
        }

        public class MarketingLead
        {
            public string PhoneLabel { get; set; }
            public double Count { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime StartDate { get; set; }
        }

        public MarketingLeadChartListModel GetLocalSitePerformanceReport(MarketingLeadReportFilter filter)
        {

            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            if (filter.StartDate.Value.Date == _clock.UtcNow.Date)
                filter.StartDate = filter.StartDate.Value.Date.AddMonths(-1);

            var daysInMonth = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);
            var startDate = _clock.ToUtc(new DateTime(filter.StartDate.Value.AddYears(-4).Year, filter.StartDate.Value.Month, daysInMonth));

            filter.EndDate = filter.StartDate.Value.AddMonths(-11).Date;
            var endDateWebLead = new DateTime(filter.StartDate.Value.AddYears(-4).Year, filter.StartDate.Value.Month, 1).Date;

            var startDateWebLead = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonth).AddDays(1).Date;

            var endDate = _clock.ToUtc(new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, 1).Date);
            var webLocalLabels = GetPhoneLabelsByCategory((long)RoutingNumberCategory.PhoneWebLocal);

            var categoryIds = _routingNumberRepository.Table.Where(x => (x.CategoryId.Value == (long?)(RoutingNumberCategory.BusinessDirectories))).
                                    Select(y => y.PhoneLabel).ToList();


            var listCallDetail = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDate && x.DateAdded <= endDate)).ToList();

            var listCallDetailWithoutCCAndCorp = listCallDetail.Where(x => (x.TransferToNumber != "") && !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("CORP")
                            && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var webLocalCollection = listCallDetail.Where(x => (webLocalLabels.Contains(x.PhoneLabel)) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
            var listCallDetailLocalBus = listCallDetail.Where(x => (!categoryIds.Any() || (categoryIds.Contains(x.PhoneLabel))) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
            var listCallDetailGooglePPC = listCallDetail.Where(x => (x.PhoneLabel.StartsWith("Marblelife Local - Google PPC")) && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

            var webLeadColletion = _webLeadDataRepository.Table.Where(x => (x.StartDate >= endDateWebLead && x.EndDate <= startDateWebLead)
                 && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                 && !x.IsWeekly).ToList();

            var result = GetChartModelForLocalPerformance(listCallDetail, listCallDetailLocalBus, listCallDetailGooglePPC, webLocalCollection, webLeadColletion, listCallDetailWithoutCCAndCorp,
                filter.StartDate.Value, filter.EndDate.Value, filter);

            return new MarketingLeadChartListModel
            {
                ChartData = result
            };
        }

        private List<MarketingLeadChartViewModel> GetChartModelForLocalPerformance(List<MarketingLeadCallDetail> listCallDetail, List<MarketingLeadCallDetail>
         listCallDetailLocalBus, List<MarketingLeadCallDetail> listCallDetailGooglePPC, List<MarketingLeadCallDetail> webLocalCollection, List<WebLeadData> webLeadColletion, List<MarketingLeadCallDetail> listCallDetailWithoutCCAndCorp,
             DateTime startDate, DateTime endDate, MarketingLeadReportFilter filter)
        {
            var list = new List<MarketingLeadChartViewModel>();
            var months = _productReportService.MonthsBetween(startDate, endDate);
            var routingNumberList = GetRouitngNumbers();
            var listCallDetailForAdjuctedData = listCallDetail;
            listCallDetail = listCallDetail.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId) && (routingNumberList.Contains(x.PhoneLabel) || string.IsNullOrEmpty(x.TransferToNumber))).ToList();
            foreach (var item in months)
            {
                var startDateTimeForLastYear = _clock.ToUtc(new DateTime(item.Item2 - 1, item.Item1, 01));
                var daysInMonth = DateTime.DaysInMonth(item.Item2 - 1, item.Item1);
                var endDateTimeForLastYear = _clock.ToUtc(new DateTime(item.Item2 - 1, item.Item1, daysInMonth)).AddDays(1);
                var startDateTimeForLastTwoYear = _clock.ToUtc(new DateTime(item.Item2 - 2, item.Item1, 01));
                daysInMonth = DateTime.DaysInMonth(item.Item2 - 2, item.Item1);
                var endDateTimeForLastTwoYear = _clock.ToUtc(new DateTime(item.Item2 - 2, item.Item1, daysInMonth)).AddDays(1);
                var startDateTimeForLastThreeYear = _clock.ToUtc(new DateTime(item.Item2 - 3, item.Item1, 01));
                daysInMonth = DateTime.DaysInMonth(item.Item2 - 3, item.Item1);
                var endDateTimeForLastThreeYear = _clock.ToUtc(new DateTime(item.Item2 - 3, item.Item1, daysInMonth)).AddDays(1);

                var startDateTimeForWebLastYear = new DateTime(item.Item2 - 1, 1, 01);
                var endDateTimeForWebLastYear = new DateTime(item.Item2 - 1, 12, 31);
                daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                var daysInMonthForLastYear = DateTime.DaysInMonth(item.Item2 - 1, item.Item1);
                var daysInMonthForLast2YearDays = DateTime.DaysInMonth(item.Item2 - 2, item.Item1);
                var daysInMonthForLast3YearDays = DateTime.DaysInMonth(item.Item2 - 3, item.Item1);
                var startDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, 01).Date);
                var endDateTime = _clock.ToUtc(new DateTime(item.Item2, item.Item1, daysInMonth).Date);
                var daysInMontPreviousMonth = DateTime.DaysInMonth(item.Item2 - 1, item.Item1);
                var startDateTimeForWebLead = (new DateTime(item.Item2, item.Item1, 01));
                var endDateTimeForWebLead = (new DateTime(item.Item2, item.Item1, daysInMonth));
                var startDateTimeForWebLeadForLastYear = (new DateTime(item.Item2 - 1, item.Item1, 01));
                var endDateTimeForWebLeadForLastYear = (new DateTime(item.Item2 - 1, item.Item1, daysInMonthForLastYear));
                var startDateTimeForWebLeadForLastTwoYear = (new DateTime(item.Item2 - 2, item.Item1, 01));
                var endDateTimeForWebLeadForLastTwoYear = (new DateTime(item.Item2 - 2, item.Item1, daysInMonthForLast2YearDays));
                var startDateTimeForWebLeadForLastThreeYear = (new DateTime(item.Item2 - 3, item.Item1, 01));
                var endDateTimeForWebLeadForLastThreeYear = (new DateTime(item.Item2 - 3, item.Item1, daysInMonthForLast3YearDays));

                endDateTime = endDateTime.AddDays(1);
                var webLocalCollectionPerMonth = webLocalCollection.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var webLocalCollectionPerMonthForLastYear = webLocalCollection.Count(x => x.DateAdded >= startDateTimeForLastYear && x.DateAdded <= endDateTimeForWebLeadForLastYear);
                var webLocalCollectionPerMonthForLastTwoYear = webLocalCollection.Count(x => x.DateAdded >= startDateTimeForLastTwoYear && x.DateAdded <= endDateTimeForWebLeadForLastTwoYear);
                var webLocalCollectionPerMonthForLastThreeYear = webLocalCollection.Count(x => x.DateAdded >= startDateTimeForLastThreeYear && x.DateAdded <= endDateTimeForWebLeadForLastThreeYear);

                var listCallDetailLocalBusPerMonth = listCallDetailLocalBus.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var listCallDetailLocalBusPerMonthForLastYear = listCallDetailLocalBus.Count(x => x.DateAdded >= startDateTimeForLastYear && x.DateAdded <= endDateTimeForLastYear);
                var listCallDetailLocalBusPerMonthForLastTwoYear = listCallDetailLocalBus.Count(x => x.DateAdded >= startDateTimeForLastTwoYear && x.DateAdded <= endDateTimeForLastTwoYear);
                var listCallDetailLocalBusPerMonthForLastThreeYear = listCallDetailLocalBus.Count(x => x.DateAdded >= startDateTimeForLastThreeYear && x.DateAdded <= endDateTimeForLastThreeYear);

                var listCallDetailPerMonth = listCallDetailWithoutCCAndCorp.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var listCallDetailPerMonthForLastYear = listCallDetailWithoutCCAndCorp.Count(x => x.DateAdded >= startDateTimeForLastYear && x.DateAdded <= endDateTimeForLastYear);
                var listCallDetailPerMonthForLastTwoYear = listCallDetailWithoutCCAndCorp.Count(x => x.DateAdded >= startDateTimeForLastTwoYear && x.DateAdded <= endDateTimeForLastTwoYear);
                var listCallDetailPerMonthForLastThreeYear = listCallDetailWithoutCCAndCorp.Count(x => x.DateAdded >= startDateTimeForLastThreeYear && x.DateAdded <= endDateTimeForLastThreeYear);

                var busdir = listCallDetailPerMonth != 0 ? Math.Round(((decimal)listCallDetailLocalBusPerMonth / (decimal)listCallDetailPerMonth) * 100, 2) : 0;
                var busdirLastYear = listCallDetailPerMonthForLastYear != 0 ? Math.Round(((decimal)listCallDetailLocalBusPerMonthForLastYear / (decimal)listCallDetailPerMonthForLastYear) * 100, 2) : 0;
                var busdirLastTwoYear = listCallDetailPerMonthForLastTwoYear != 0 ? Math.Round(((decimal)listCallDetailLocalBusPerMonthForLastTwoYear / (decimal)listCallDetailPerMonthForLastTwoYear) * 100, 2) : 0;
                var busdirLastThreeYear = listCallDetailPerMonthForLastThreeYear != 0 ? Math.Round(((decimal)listCallDetailLocalBusPerMonthForLastThreeYear / (decimal)listCallDetailPerMonthForLastThreeYear) * 100, 2) : 0;


                var adjustedActualValueForPhoneAndWebLocal = (busdir * webLocalCollectionPerMonth) / 100;
                var adjustedActualValueForPhoneAndWebLocallastYear = (busdirLastYear * webLocalCollectionPerMonthForLastYear) / 100;
                var adjustedActualValueForPhoneAndWebLocallastTwoYear = (busdirLastTwoYear * webLocalCollectionPerMonthForLastTwoYear) / 100;
                var adjustedActualValueForPhoneAndWebLocallastThreeYear = (busdirLastThreeYear * webLocalCollectionPerMonthForLastThreeYear) / 100;


                var listCallDetailGooglePPCPerMonth = listCallDetailGooglePPC.Count(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime);
                var listCallDetailGooglePPCPerMonthlastYear = listCallDetailGooglePPC.Count(x => x.DateAdded >= startDateTimeForLastYear && x.DateAdded <= endDateTimeForLastYear);
                var listCallDetailGooglePPCPerMonthlastTwoYear = listCallDetailGooglePPC.Count(x => x.DateAdded >= startDateTimeForLastTwoYear && x.DateAdded <= endDateTimeForLastTwoYear);
                var listCallDetailGooglePPCPerMonthlastThreeYear = listCallDetailGooglePPC.Count(x => x.DateAdded >= startDateTimeForLastThreeYear && x.DateAdded <= endDateTimeForLastThreeYear);

                var googlePPCValue = (listCallDetailPerMonth != 0) ? (1 - (listCallDetailLocalBusPerMonth / listCallDetailPerMonth)) != 0 ? (decimal)((listCallDetailGooglePPCPerMonth) / (1 - (listCallDetailLocalBusPerMonth / listCallDetailPerMonth))) : default(decimal) : listCallDetailGooglePPCPerMonth;
                var googlePPCValueLastYear = (listCallDetailPerMonthForLastYear != 0) ? (1 - (listCallDetailLocalBusPerMonthForLastYear / listCallDetailPerMonthForLastYear)) != 0 ? (decimal)((listCallDetailGooglePPCPerMonthlastYear) / (1 - (listCallDetailLocalBusPerMonthForLastYear / listCallDetailPerMonthForLastYear))) : default(decimal) : listCallDetailGooglePPCPerMonthlastYear;
                var googlePPCValueLastTwoYear = (listCallDetailPerMonthForLastTwoYear != 0) ? (1 - (listCallDetailLocalBusPerMonthForLastTwoYear / listCallDetailPerMonthForLastTwoYear)) != 0 ? (decimal)((listCallDetailGooglePPCPerMonthlastTwoYear) / (1 - (listCallDetailLocalBusPerMonthForLastTwoYear / listCallDetailPerMonthForLastTwoYear))) : default(decimal) : listCallDetailGooglePPCPerMonthlastTwoYear;
                var googlePPCValueLastThreeYear = (listCallDetailPerMonthForLastThreeYear != 0) ? (1 - (listCallDetailLocalBusPerMonthForLastThreeYear / listCallDetailPerMonthForLastThreeYear)) != 0 ? (decimal)((listCallDetailGooglePPCPerMonthlastThreeYear) / (1 - (listCallDetailLocalBusPerMonthForLastThreeYear / listCallDetailPerMonthForLastThreeYear))) : default(decimal) : listCallDetailGooglePPCPerMonthlastThreeYear;

                var localValueForLastYear = adjustedActualValueForPhoneAndWebLocallastYear + googlePPCValueLastYear + webLocalCollection.Count(i => i.DateAdded >= startDateTimeForWebLeadForLastYear && i.DateAdded <= endDateTimeForWebLeadForLastYear);
                var localValueForLastTwoYear = adjustedActualValueForPhoneAndWebLocallastTwoYear + googlePPCValueLastTwoYear + webLocalCollection.Count(i => i.DateAdded >= startDateTimeForWebLeadForLastTwoYear && i.DateAdded <= endDateTimeForWebLeadForLastTwoYear);
                var localValueForLastThreeYear = adjustedActualValueForPhoneAndWebLocallastThreeYear + googlePPCValueLastThreeYear + webLocalCollection.Count(i => i.DateAdded >= startDateTimeForWebLeadForLastThreeYear && i.DateAdded <= endDateTimeForWebLeadForLastThreeYear);

                var model = new MarketingLeadChartViewModel
                {
                    Date = new DateTime(item.Item2, item.Item1, daysInMonth).Date,
                    DateString = new DateTime(item.Item2 - 1, item.Item1, daysInMonthForLastYear).ToString("MMM, yyyy"),
                    LastYearDateString = ((item.Item2 - 1) + ", " + (item.Item2 - 2) + ", " + (item.Item2 - 3)).ToString(),
                    WebLocalCount = webLocalCollection.Count(i => i.DateAdded >= startDateTimeForWebLead && i.DateAdded <= endDateTimeForWebLead),
                    PhoneWebLocalCount = Math.Round(adjustedActualValueForPhoneAndWebLocal),
                    GooglePPCCount = Math.Round(googlePPCValue)
                };
                model.LocalCount = Math.Round(localValueForLastYear, 0);
                model.TotalCount = Math.Round((localValueForLastYear + localValueForLastTwoYear + localValueForLastThreeYear) / 3, 0);
                list.Add(model);
            }
            return list;
        }


        public ManagementVsLocalChartListModel GetManagementVsLocalReport(ManagementChartReportFilter filter)
        {
            IEnumerable<IGrouping<DayOfWeek, MarketingLeadCallDetail>> groupedDataByDayOfWeek;
            int year = Convert.ToInt32(filter.Year);
            if (filter.Year == null && filter.Month == default(int))
            {
                filter.StartDate = _clock.UtcNow.Date.AddDays(-1).Date;
                filter.StartDate = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, 1);
            }
            else
            {
                if (filter.Month == 0)
                {
                    filter.Month = 1;
                }
                filter.StartDate = new DateTime(year, filter.Month, 1);
            }

            var selectedDate = filter.StartDate.Value.Date;
            var startDate = _clock.ToUtc(selectedDate.Date);
            filter.EndDate = _clock.ToUtc(selectedDate.AddMonths(1).Date);



            var totalCallLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDate && x.DateAdded <= filter.EndDate)
                         && (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId) && (x.TransferToNumber != "")
                                    && (!x.PhoneLabel.StartsWith("CC")) && !x.PhoneLabel.StartsWith("CORP")).ToList();


            var localCallLeadList = totalCallLeadList.Where(x => x.FranchiseeId == filter.FranchiseeId || filter.FranchiseeId <= 0).ToList();

            filter.StartDate = _clock.ToUtc(filter.StartDate.GetValueOrDefault());
            var managementVsLocalTableData = GetManagementVsLocalTableDataReport(localCallLeadList, filter.StartDate.Value, filter.EndDate.Value, out groupedDataByDayOfWeek);
            var managementVsLocalAverageData = GetManagementVsLocalAverageDataReport(groupedDataByDayOfWeek, filter.StartDate.Value, filter.EndDate.Value);

            managementVsLocalAverageData.Where(x => x.TotalCallsForDay == 0).Count();
            return new ManagementVsLocalChartListModel
            {
                TotalMissedCalls = localCallLeadList.Where(x => x.CallDuration < 2).Count(),
                TotalCalls = localCallLeadList.Count(),
                ManagementVsLocalAverageData = managementVsLocalAverageData,
                ManagementVsLocalTableData = managementVsLocalTableData,
                TotalCallsForDayForZeros = managementVsLocalAverageData.Where(x => x.TotalCallsForDay != 0).Count(),
                TotalCallsReceivedForDayForZeros = managementVsLocalAverageData.Where(x => x.TotalCallsReceivedForDay != 0).Count(),
                TotalMissedCallsForZeros = managementVsLocalAverageData.Where(x => x.TotalMissedCalls != 0).Count(),
                TotalMissedCallsForTheDayForZeros = managementVsLocalAverageData.Where(x => x.TotalMissedCallsForTheDay != 0).Count(),
            };
        }

        private List<ManagementAndLocalChartViewModel> GetManagementVsLocalAverageDataReport(IEnumerable<IGrouping<DayOfWeek, MarketingLeadCallDetail>> groupedDataByDayOfWeeks, DateTime startDate, DateTime endDate)
        {
            var isChecked = false;
            var managementChartList = new List<ManagementAndLocalChartViewModel>();
            foreach (var groupedDataByDayOfWeek in groupedDataByDayOfWeeks)
            {
                if (groupedDataByDayOfWeeks.Count() < 7 && !isChecked)
                {
                    isChecked = true;
                    var managementViewModelForUnGrouped = GetRecordsForMissingDayOfWeek(groupedDataByDayOfWeeks, startDate, endDate);
                    managementChartList.AddRange(managementViewModelForUnGrouped);
                }
                var managementAndLocalChartViewModel = new ManagementAndLocalChartViewModel();
                managementAndLocalChartViewModel.DayOfDate = DateTimeFormatInfo.CurrentInfo.GetDayName(groupedDataByDayOfWeek.Key);
                managementAndLocalChartViewModel.TotalCallsForDay = groupedDataByDayOfWeek.Count();
                managementAndLocalChartViewModel.TotalMissedCallsForTheDay = groupedDataByDayOfWeek.Where(x => x.CallDuration < 2).Count();
                managementAndLocalChartViewModel.TotalCallsReceivedForDay = groupedDataByDayOfWeek.Where(x => x.CallDuration >= 2).Count();
                managementAndLocalChartViewModel.Occurance = DateRangeHelperService.GetNumberOfDaysInBetween(groupedDataByDayOfWeek.Key, startDate.Date, startDate.Date.AddMonths(1).AddTicks(-1).Date);
                managementAndLocalChartViewModel.OrderBy = OrderByGroupedData(DateTimeFormatInfo.CurrentInfo.GetDayName(groupedDataByDayOfWeek.Key));
                managementChartList.Add(managementAndLocalChartViewModel);
            }

            return managementChartList.OrderBy(x => x.OrderBy).ToList();
        }
        private List<ManagementAndLocalChartGraphViewModel> GetManagementVsLocalTableDataReport(IList<MarketingLeadCallDetail> localCallLeadList, DateTime startDate, DateTime endDate, out IEnumerable<IGrouping<DayOfWeek, MarketingLeadCallDetail>> groupedDataByDayOfWeek)
        {
            var endDateForHours = startDate.Date.AddHours(23).AddMinutes(59).AddMilliseconds(59);
            var hours = DateRangeHelperService.GetHoursCollection(startDate.Date, endDateForHours);
            var groupedDatas = localCallLeadList.GroupBy(x => x.TimeZoneStartTime.DayOfWeek);
            groupedDataByDayOfWeek = groupedDatas;

            var totalCallsReceivedAParticularHour = default(decimal);
            var totalMissedCallsReceivedAParticularHour = default(decimal);
            var model = new ManagementAndLocalChartGraphViewModel();
            var managementListModel = new List<ManagementAndLocalChartGraphViewModel>();
            foreach (var item in hours.OrderBy(x => x.TimeOfDay))
            {
                var isChecked = false;
                List<ManagementAndLocalChartViewModel> managementViewModel = new List<ManagementAndLocalChartViewModel>();
                var tim2e = item.TimeOfDay;
                var starttime = startDate.Date + new TimeSpan(tim2e.Ticks);
                var endtime = (endDate.Date + new TimeSpan(tim2e.Ticks)).AddHours(-1);

                var startToCompare = ((starttime));
                var endToCompare = ((endtime));

                var startToCompareForNational = _clock.ToUtc((startToCompare).Date);
                var endToCompareForNational = _clock.ToUtc((endToCompare).Date.AddDays(1));

                foreach (var groupedData in groupedDatas)
                {
                    if (groupedDatas.Count() < 7 && !isChecked)
                    {
                        isChecked = true;
                        var managementViewModelForUnGrouped = GetRecordsForMissingDayOfWeek(groupedDatas, startToCompareForNational, endToCompareForNational);
                        managementViewModel.AddRange(managementViewModelForUnGrouped);
                    }
                    var managementModel = new ManagementAndLocalChartViewModel();
                    managementModel.Occurance = DateRangeHelperService.GetNumberOfDaysInBetween(groupedData.Key, startToCompareForNational, endToCompareForNational);
                    managementModel.DayOfDate = DateTimeFormatInfo.CurrentInfo.GetDayName(groupedData.Key);
                    managementModel.TotalMissedCalls = groupedData.Where(x => x.TimeZoneStartTime.Hour == item.TimeOfDay.Hours && x.CallDuration < 2).Count();
                    managementModel.OrderBy = OrderByGroupedData(DateTimeFormatInfo.CurrentInfo.GetDayName(groupedData.Key));
                    managementModel.ColumnColor = GetColumnColor(managementModel.DayOfDate);
                    totalCallsReceivedAParticularHour += groupedData.Where(x => (x.CallDuration >= 2 || x.CallDuration < 2) && x.TimeZoneStartTime.Hour == item.TimeOfDay.Hours).Count();
                    totalMissedCallsReceivedAParticularHour += groupedData.Where(x => x.CallDuration < 2 && x.TimeZoneStartTime.Hour == item.TimeOfDay.Hours).Count();
                    managementViewModel.Add(managementModel);
                }

                model = new ManagementAndLocalChartGraphViewModel
                {
                    BusinessHour = getBusinessHourStatus(item.TimeOfDay.Hours),
                    Hour = item.TimeOfDay.Hours,
                    ChartData = managementViewModel.OrderBy(x => x.OrderBy).ToList(),
                    Rowcolor = getHourColor(item.TimeOfDay.Hours),
                    TimeStatus = getTimeStatus(item.TimeOfDay.Hours),
                    TotalCallsReceivedAParticularHour = totalCallsReceivedAParticularHour,
                    TotalMissedCallsParticularHour = totalMissedCallsReceivedAParticularHour,
                };
                managementListModel.Add(model);
                totalCallsReceivedAParticularHour = 0;
                totalMissedCallsReceivedAParticularHour = 0;
            }
            return managementListModel;
        }
        private List<ManagementAndLocalChartViewModel> GetRecordsForMissingDayOfWeek(IEnumerable<IGrouping<DayOfWeek, MarketingLeadCallDetail>> groupedDataByDayOfWeeks, DateTime startToCompareForNational, DateTime endToCompareForNational)
        {
            var managementChartGraphViewModelList = new List<ManagementAndLocalChartViewModel>();

            if (groupedDataByDayOfWeeks.Any(x => x.Key == DayOfWeek.Monday))
            {
            }
            else
            {
                var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
                managementChartGraphViewModel = SetMissingGroupedData(DayOfWeek.Monday, startToCompareForNational, 1);
                managementChartGraphViewModelList.Add(managementChartGraphViewModel);
            }
            if (groupedDataByDayOfWeeks.Any(x => x.Key == DayOfWeek.Sunday))
            {
            }
            else
            {
                var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
                managementChartGraphViewModel = SetMissingGroupedData(DayOfWeek.Sunday, startToCompareForNational, 7);
                managementChartGraphViewModelList.Add(managementChartGraphViewModel);
            }
            if (groupedDataByDayOfWeeks.Any(x => x.Key == DayOfWeek.Tuesday))
            {
            }
            else
            {
                var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
                managementChartGraphViewModel = SetMissingGroupedData(DayOfWeek.Tuesday, startToCompareForNational, 2);
                managementChartGraphViewModelList.Add(managementChartGraphViewModel);
            }
            if (groupedDataByDayOfWeeks.Any(x => x.Key == DayOfWeek.Wednesday))
            {
            }
            else
            {
                var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
                managementChartGraphViewModel = SetMissingGroupedData(DayOfWeek.Wednesday, startToCompareForNational, 3);
                managementChartGraphViewModelList.Add(managementChartGraphViewModel);
            }
            if (groupedDataByDayOfWeeks.Any(x => x.Key == DayOfWeek.Thursday))
            {
            }
            else
            {
                var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
                managementChartGraphViewModel = SetMissingGroupedData(DayOfWeek.Thursday, startToCompareForNational, 4);
                managementChartGraphViewModelList.Add(managementChartGraphViewModel);
            }
            if (groupedDataByDayOfWeeks.Any(x => x.Key == DayOfWeek.Friday))
            {
            }
            else
            {
                var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
                managementChartGraphViewModel = SetMissingGroupedData(DayOfWeek.Friday, startToCompareForNational, 5);
                managementChartGraphViewModelList.Add(managementChartGraphViewModel);
            }
            if (groupedDataByDayOfWeeks.Any(x => x.Key == DayOfWeek.Saturday))
            {
            }
            else
            {
                var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
                managementChartGraphViewModel = SetMissingGroupedData(DayOfWeek.Saturday, startToCompareForNational, 6);
                managementChartGraphViewModelList.Add(managementChartGraphViewModel);
            }
            return managementChartGraphViewModelList;
        }


        private ManagementAndLocalChartViewModel SetMissingGroupedData(DayOfWeek day, DateTime startToCompareForNational, int order)
        {
            var managementChartGraphViewModel = new ManagementAndLocalChartViewModel();
            managementChartGraphViewModel.DayOfDate = day.ToString();
            managementChartGraphViewModel.TotalMissedCalls = 0;
            managementChartGraphViewModel.TotalCallsReceivedForDay = 0;
            managementChartGraphViewModel.TotalMissedCallsForTheDay = 0;
            managementChartGraphViewModel.TotalCallsForDay = 0;
            managementChartGraphViewModel.ColumnColor = GetColumnColor(day.ToString());
            managementChartGraphViewModel.OrderBy = order;
            managementChartGraphViewModel.Occurance = DateRangeHelperService.GetNumberOfDaysInBetween(day, startToCompareForNational, startToCompareForNational.Date.AddMonths(1).Date);
            return managementChartGraphViewModel;

        }
        private string getBusinessHourStatus(int hour)
        {
            switch (hour)
            {
                case 8:
                    {
                        return "Closes";
                    }
                case 17:
                    {
                        return "Business Hour";
                    }

                case 23:
                    {
                        return "Closes1";
                    }
                default:
                    {
                        return "Closes";
                    }
            }
        }
        private string GetColumnColor(string day)
        {
            switch (day)
            {
                case "Saturday":
                case "Sunday":
                    {
                        return "LightBlue";
                    }


                default:
                    {
                        return "White";
                    }
            }
        }
        private string getTimeStatus(int hour)
        {
            switch (hour)
            {
                case 8:
                    {
                        return "Late Arrival";
                    }

                case 12:
                    {
                        return "Lunch";
                    }

                case 16:
                    {
                        return "Early Close";
                    }
                default:
                    {
                        return "";
                    }
            }
        }

        private string getHourColor(int hour)
        {
            switch (hour)
            {
                case 8:
                case 12:
                case 16:
                    {
                        return "yellow";
                    }

                default:
                    {
                        return "white";
                    }
            }
        }

        private int OrderByGroupedData(string weekDays)
        {
            switch (weekDays)
            {
                case "Monday":
                    {
                        return 1;
                    }
                case "Tuesday":
                    {
                        return 2;
                    }
                case "Wednesday":
                    {
                        return 3;
                    }
                case "Thursday":
                    {
                        return 4;
                    }
                case "Friday":
                    {
                        return 5;
                    }
                case "Saturday":
                    {
                        return 6;
                    }
                case "Sunday":
                    {
                        return 7;
                    }
                default:
                    {
                        return 8;
                    }
            }
        }

        public ManagementCharViewModel GetManagementReport(ManagementChartReportFilter filter)
        {
            int year = Convert.ToInt32(filter.Year);
            if (filter.StartDate == null)
            {
                filter.StartDate = _clock.UtcNow.Date.AddDays(-1).Date;
                filter.StartDate = new DateTime(year, filter.Month, 1);
            }
            else
            {
                if (filter.Month == 0)
                {
                    filter.Month = 1;

                }
                filter.StartDate = new DateTime(year, filter.Month, 1);
            }
            List<ManagementChartListModel> managementChartListmodelList = new List<ManagementChartListModel>();
            List<string> monthCollectionLocal = new List<string>();
            var selectedDate = filter.StartDate.Value.Date;
            var startDate = _clock.ToUtc(selectedDate.Date);

            filter.EndDate = _clock.ToUtc(filter.StartDate.Value.AddMonths(-12).Date);

            var daysInMonthForLastYear = DateTime.DaysInMonth(startDate.Year, startDate.Month);
            var startDateForList = startDate.AddDays(daysInMonthForLastYear);
            var totalCallLeadList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded <= startDateForList && x.DateAdded >= filter.EndDate)
                          && (x.TransferToNumber != "") && (!x.PhoneLabel.StartsWith("CC")) && !x.PhoneLabel.StartsWith("CORP")).ToList();

            var franchiseeDomain = _franchiseeDataRepository.Table.Where(x => x.CategoryId != null).ToList();
            var managementChartListModelList = GetManagementReportData(franchiseeDomain, totalCallLeadList, startDate, out monthCollectionLocal, out managementChartListmodelList);
            var chartData = ManagementChartListModelForChart(managementChartListmodelList, filter.StartDate.GetValueOrDefault(), filter.FranchiseeId);
            return new ManagementCharViewModel
            {
                ManagementVsLocalAverageData = managementChartListModelList,
                MonthCollection = monthCollectionLocal,
                ManagementChartListModelForChart = chartData
            };
        }
        private List<ManagementChartListModel> GetManagementReportData(List<Franchisee> franchiseeList, List<MarketingLeadCallDetail> totalCallLeadList, DateTime startDate, out List<string> monthCollection, out List<ManagementChartListModel> managementChartListmodelLists)
        {
            var endDate = _clock.ToUtc(startDate.Date.AddMonths(12).Date);
            var months = _productReportService.MonthsBetween(startDate.Date, startDate.Date.AddMonths(-11));
            var managementChartListmodelList = new List<ManagementChartListModel>();
            var groupedDatas = totalCallLeadList.GroupBy(x => x.TimeZoneStartTime.DayOfWeek).ToList();
            var frontOfficeFranchisees = _franchiseeDataRepository.Table.Where(x => x.CategoryId == (long?)FranchiseeCallCategory.FRONTOFFICE).Select(x => x.Id).ToList();
            var officePersonFranchisees = _franchiseeDataRepository.Table.Where(x => x.CategoryId == (long?)FranchiseeCallCategory.OFFICEPERSON).Select(x => x.Id).ToList();
            var responseWhenAvailableFranchisees = _franchiseeDataRepository.Table.Where(x => x.CategoryId == (long?)FranchiseeCallCategory.RESPONDWHENAVAILABLE).Select(x => x.Id).ToList();
            var responseNextDayFranchisees = _franchiseeDataRepository.Table.Where(x => x.CategoryId == (long?)FranchiseeCallCategory.RESPONDSNEXTDAY).Select(x => x.Id).ToList();
            var managementChartListModelForChart = new List<ManagementChartListModelForChart>();
            List<string> monthCollectionLocal = null;

            foreach (var franchisee in franchiseeList)
            {
                var managementChartViewModelList = new List<ManagementChartViewModel>();
                var managementChartDayListModel = new ManagementChartDayListModel();
                var managementChartDayListModelList = new List<ManagementChartDayListModel>();
                var managementChartListmodel = new ManagementChartListModel();
                var arrayForMinCalculation = new List<double>();

                managementChartDayListModel.FranchiseeName = franchisee.Organization.Name;
                var minValue = default(double);
                var changeValue = default(double);
                monthCollectionLocal = new List<string>();
                foreach (var month in months)
                {
                    var zeroCount = 0;
                    var managementChartViewModel = new ManagementChartViewModel();
                    var perTotalReceivedCalls = default(decimal);

                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month.Item1);
                    var daysInMonthForLastYear = DateTime.DaysInMonth(month.Item2, month.Item1);
                    var startDateForMonth = _clock.ToUtc(new DateTime(month.Item2, month.Item1, 1).Date);
                    var endDateForMonth = _clock.ToUtc(new DateTime(month.Item2, month.Item1, daysInMonthForLastYear).Date).AddDays(1);
                    monthCollectionLocal.Add(startDateForMonth.ToString("MMM") + "'" + startDateForMonth.ToString("yy"));

                    foreach (var groupedData in groupedDatas)
                    {
                        var per = default(decimal);
                        var dayOfWeekCallLead = groupedData.Where(x => (x.FranchiseeId == franchisee.Id) && x.TimeZoneStartTime >= startDateForMonth && x.TimeZoneStartTime <= endDateForMonth && x.CallDuration >= 2).Count();
                        var totalDayCalls = groupedData.Where(x => (x.FranchiseeId == franchisee.Id) && x.TimeZoneStartTime >= startDateForMonth && x.TimeZoneStartTime <= endDateForMonth).Count();

                        if (totalDayCalls != 0)
                            per = Math.Round((((decimal)dayOfWeekCallLead * 100) / ((decimal)totalDayCalls)), 2);
                        perTotalReceivedCalls += per;
                        if (per != 0)
                        {
                            zeroCount++;
                        }
                    }
                    managementChartViewModel.FranchiseeId = franchisee.Id;
                    managementChartViewModel.MonthNumber = month.Item1;
                    if (zeroCount != 0)
                        managementChartViewModel.PerTotalReceivedCalls = (double)Math.Round(perTotalReceivedCalls / zeroCount);
                    else
                        managementChartViewModel.PerTotalReceivedCalls = 0;
                    arrayForMinCalculation.Add(managementChartViewModel.PerTotalReceivedCalls);
                    managementChartViewModel.Month = monthName;
                    minValue = getMinValue(arrayForMinCalculation);
                    changeValue = getChangeValue(arrayForMinCalculation);
                    managementChartViewModelList.Add(managementChartViewModel);
                }
                managementChartViewModelList.Add(new ManagementChartViewModel { Month = "Min", PerTotalReceivedCalls = minValue });
                managementChartViewModelList.Add(new ManagementChartViewModel { Month = "ChangeValue", PerTotalReceivedCalls = changeValue });
                managementChartDayListModel.ManagementChartViewModel = managementChartViewModelList.ToList();
                managementChartListmodel.CategoryName = franchisee.Category.Name;
                managementChartDayListModelList.Add(managementChartDayListModel);
                managementChartListmodel.ManagementVsLocalAverageData = (managementChartDayListModelList);
                if (managementChartViewModelList.Where(x => x.PerTotalReceivedCalls == 0).Count() == 14)
                    continue;

                managementChartListmodelList.Add(managementChartListmodel);
            }
            if (monthCollectionLocal != null)
            {
                monthCollectionLocal.Add("Min Value");
                monthCollectionLocal.Add("Change Value");
            }
            monthCollection = monthCollectionLocal;
            managementChartListmodelLists = managementChartListmodelList;
            var grouped = getGroupedDataForManagementData(managementChartListmodelList.GroupBy(x => x.CategoryName).ToList());

            return grouped;
        }


        private double getMinValue(List<double> minValueArray)
        {
            return minValueArray.Min();
        }
        private double getChangeValue(List<double> minValueArray)
        {
            return minValueArray.Max() - minValueArray.Min();
        }
        private List<ManagementChartListModel> getGroupedDataForManagementData(List<IGrouping<string, ManagementChartListModel>> groupedDatas)
        {

            var managementChartListModelList = new List<ManagementChartListModel>();

            foreach (var groupedData in groupedDatas)
            {
                var managementVsLocalAverageDataList = new List<ManagementChartDayListModel>();
                var managementVsLocalAverageData = new ManagementChartDayListModel();
                var managementChartListModel = new ManagementChartListModel();
                managementChartListModel.CategoryName = groupedData.Key;
                var groupedBy = groupedData.Select(x => x.ManagementVsLocalAverageData);

                foreach (var managementVsLocalAverageDatas in groupedData.Select(x => x.ManagementVsLocalAverageData))
                {
                    managementVsLocalAverageDataList.AddRange(managementVsLocalAverageDatas);
                }

                managementChartListModel.ManagementVsLocalAverageData = managementVsLocalAverageDataList;
                managementChartListModelList.Add(managementChartListModel);
            }
            return managementChartListModelList;
        }


        public List<ManagementChartListModelForChart> ManagementChartListModelForChart(IList<ManagementChartListModel> managementVsLocalAverageData, DateTime startDate, long? franchiseeId)
        {
            var managementChartListModelForChartList = new List<ManagementChartListModelForChart>();

            var months = _productReportService.MonthsBetween(startDate.Date.Date, startDate.Date.AddMonths(-11));
            var frontOfficeList = managementVsLocalAverageData.Where(x => x.CategoryName == frontOfficeName).Select(x => x.ManagementVsLocalAverageData).ToList();
            var officePersonList = managementVsLocalAverageData.Where(x => x.CategoryName == officePersonName).Select(x => x.ManagementVsLocalAverageData).ToList();
            var respondWhenAvailableList = managementVsLocalAverageData.Where(x => x.CategoryName == respondWhenAvailable).Select(x => x.ManagementVsLocalAverageData).ToList();
            var respondNextDayList = managementVsLocalAverageData.Where(x => x.CategoryName == respondNextDay).Select(x => x.ManagementVsLocalAverageData).ToList();



            var frontOfficeFranchiseeList = frontOfficeList.Select(x => x.Select(x1 => x1.ManagementChartViewModel)).ToList();
            var officePersonFranchiseeList = officePersonList.Select(x => x.Select(x1 => x1.ManagementChartViewModel)).ToList();
            var respondWhenAvailableFranchiseeList = respondWhenAvailableList.Select(x => x.Select(x1 => x1.ManagementChartViewModel)).ToList();
            var respondNextDayFranchiseeList = respondNextDayList.Select(x => x.Select(x1 => x1.ManagementChartViewModel)).ToList();


            foreach (var month in months)
            {
                var daysInMonth = DateTime.DaysInMonth(month.Item2, month.Item1);
                var frontOfficeCount = default(double);
                var officePersonCount = default(double);
                var respondsNextDay = default(double);
                var respondsWhenAvailable = default(double);
                var localCount = default(double);
                var managementChartListModelForChart = new ManagementChartListModelForChart();
                DateTime monthStart = _clock.ToUtc(new DateTime(month.Item2, month.Item1, 1).Date);
                managementChartListModelForChart.Date = new DateTime(month.Item2, month.Item1, daysInMonth);
                managementChartListModelForChart.Month = month.Item1;

                var frontOfficeCountForZero = 0;
                var officePersonCountForZero = 0;
                var respondsWhenAvailableForZero = 0;
                var respondsNextDayForZero = 0;
                foreach (var frontOffice in frontOfficeFranchiseeList)
                {
                    foreach (var frontOfficeValue in frontOffice)
                    {
                        if (frontOfficeValue.Where(x => x.MonthNumber == month.Item1).Select(x => x.PerTotalReceivedCalls).FirstOrDefault() == 0)
                        {
                            ++frontOfficeCountForZero;
                        }
                        frontOfficeCount += frontOfficeValue.Where(x => x.MonthNumber == month.Item1).Select(x => x.PerTotalReceivedCalls).FirstOrDefault();
                        if (franchiseeId != null)
                        {
                            localCount += frontOfficeValue.Where(x => x.MonthNumber == month.Item1 && x.FranchiseeId == franchiseeId).Select(x => x.PerTotalReceivedCalls).FirstOrDefault();
                        }
                    }
                }
                foreach (var officePerson in officePersonFranchiseeList)
                {
                    foreach (var officePersonValue in officePerson)
                    {
                        if (officePersonValue.Where(x => x.MonthNumber == month.Item1).Select(x => x.PerTotalReceivedCalls).FirstOrDefault() == 0)
                        {
                            ++officePersonCountForZero;
                        }
                        officePersonCount += officePersonValue.Where(x => x.MonthNumber == month.Item1).Average(x => x.PerTotalReceivedCalls);
                        if (franchiseeId != null)
                        {
                            localCount += officePersonValue.Where(x => x.MonthNumber == month.Item1 && x.FranchiseeId == franchiseeId).Select(x => x.PerTotalReceivedCalls).FirstOrDefault();
                        }
                    }
                }
                foreach (var respondWhenAvailable in respondWhenAvailableFranchiseeList)
                {
                    foreach (var respondWhenAvailableValue in respondWhenAvailable)
                    {
                        if (respondWhenAvailableValue.Where(x => x.MonthNumber == month.Item1).Select(x => x.PerTotalReceivedCalls).FirstOrDefault() == 0)
                        {
                            ++respondsWhenAvailableForZero;
                        }
                        respondsWhenAvailable += (double)respondWhenAvailableValue.Where(x => x.MonthNumber == month.Item1).Average(x => x.PerTotalReceivedCalls);
                        if (franchiseeId != null)
                        {
                            localCount += respondWhenAvailableValue.Where(x => x.MonthNumber == month.Item1 && x.FranchiseeId == franchiseeId).Select(x => x.PerTotalReceivedCalls).FirstOrDefault();
                        }
                    }
                }

                foreach (var respondNextDay in respondNextDayFranchiseeList)
                {
                    foreach (var respondNextDayValue in respondNextDay)
                    {
                        if (respondNextDayValue.Where(x => x.MonthNumber == month.Item1).Select(x => x.PerTotalReceivedCalls).FirstOrDefault() == 0)
                        {
                            ++respondsNextDayForZero;
                        }
                        respondsNextDay += (double)respondNextDayValue.Where(x => x.MonthNumber == month.Item1).Average(x => x.PerTotalReceivedCalls);
                        if (franchiseeId != null)
                        {
                            localCount += respondNextDayValue.Where(x => x.MonthNumber == month.Item1 && x.FranchiseeId == franchiseeId).Select(x => x.PerTotalReceivedCalls).FirstOrDefault();
                        }
                    }
                }
                managementChartListModelForChart.NationalCount = Math.Round((officePersonCount / (officePersonFranchiseeList.Count() - officePersonCountForZero) + respondsNextDay / (respondNextDayFranchiseeList.Count() - respondsNextDayForZero) + respondsWhenAvailable / (respondWhenAvailableFranchiseeList.Count() - respondsWhenAvailableForZero) + frontOfficeCount / (frontOfficeFranchiseeList.Count() - frontOfficeCountForZero)) / 4, 0);
                managementChartListModelForChart.OfficePersonCount = Math.Round(officePersonCount / (officePersonFranchiseeList.Count() - officePersonCountForZero), 0);
                managementChartListModelForChart.ResponseNextDayCount = Math.Round(respondsNextDay / (respondNextDayFranchiseeList.Count() - respondsNextDayForZero), 0);
                managementChartListModelForChart.ResponseWhenAvailableCount = Math.Round(respondsWhenAvailable / (respondWhenAvailableFranchiseeList.Count() - respondsWhenAvailableForZero), 0);
                managementChartListModelForChart.FrontOfficeCount = Math.Round(frontOfficeCount / (frontOfficeFranchiseeList.Count() - frontOfficeCountForZero), 0);
                managementChartListModelForChart.LocalCount = Math.Round(localCount, 0);
                managementChartListModelForChartList.Add(managementChartListModelForChart);
            }
            return managementChartListModelForChartList;

        }
    }
}
