using Core.Application;
using Core.Application.Attribute;
using Core.MarketingLead.Domain;
using Core.MarketingLead.Enum;
using Core.MarketingLead.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using Core.Reports;
using Core.Users.Domain;
using DocumentFormat.OpenXml.Drawing.Charts;
using Ical.Net.ExtensionMethods;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class LeadPerformanceFranchiseeDetailsService : ILeadPerformanceFranchiseeDetailsService
    {
        private readonly IFranchiseeLeadPerformanceFactory _franchiseeLeadPerformanceFactory;
        private readonly IRepository<LeadPerformanceFranchiseeDetails> _leadPerformanceFranchiseeDetailsRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<OrganizationRoleUserFranchisee> _organizationRoleUserFranchiseeRepository;
        private readonly IClock _clock;
        private readonly IRepository<WebLeadData> _webLeadDataRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<CallDetailData> _callDetailDataRepository;
        private readonly IRepository<RoutingNumber> _routingNumberRepository;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepository;
        private readonly IProductReportService _productReportService;


        public LeadPerformanceFranchiseeDetailsService(IFranchiseeLeadPerformanceFactory franchiseeLeadPerformanceFactory,
            IUnitOfWork unitOfWork, IClock clock, IProductReportService productReportService
            )
        {
            _franchiseeLeadPerformanceFactory = franchiseeLeadPerformanceFactory;
            _leadPerformanceFranchiseeDetailsRepository = unitOfWork.Repository<LeadPerformanceFranchiseeDetails>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _clock = clock;
            _webLeadDataRepository = unitOfWork.Repository<WebLeadData>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _callDetailDataRepository = unitOfWork.Repository<CallDetailData>();
            _routingNumberRepository = unitOfWork.Repository<RoutingNumber>();
            _marketingLeadCallDetailRepository = unitOfWork.Repository<MarketingLeadCallDetail>();
            _productReportService = productReportService;
            _organizationRoleUserFranchiseeRepository = unitOfWork.Repository<OrganizationRoleUserFranchisee>();
        }

        public LeadPerformanceListModel GetFranchiseePerformance(LeadPerformanceFranchiseeFilter filter)
        {
            var orgRoleUser = _organizationRoleUserRepository.Table;
            var leadRepository = _leadPerformanceFranchiseeDetailsRepository.Table.Where(
                 x => x.FranchiseeId == filter.FranchiseeId && x.CategoryId == filter.CategoryId).ToList();
            var collection = leadRepository.Select(x => _franchiseeLeadPerformanceFactory.CreateViewModel(x,
               x.DataRecorderMetaData.ModifiedBy != null ? orgRoleUser.FirstOrDefault(x1 => x1.UserId == x.DataRecorderMetaData.ModifiedBy) :
               orgRoleUser.FirstOrDefault(x1 => x1.UserId == x.DataRecorderMetaData.CreatedBy))).OrderBy(x => x.Id).ToList();
            if (leadRepository.Count() > 0)
            {
                var listModel = new LeadPerformanceListModel()
                {
                    LeadPerformanceFranchiseeViewData = collection.OrderByDescending(x => x.Id),
                    TotalCount = leadRepository.Count()
                };
                return listModel;
            }
            else
                return default;
        }

        public void Save(LeadPerformanceEditModel franchiseeViewModel, long franchiseeId)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentMonthDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).Date;
            var currentMonthLastDate = currentMonthDate.AddMonths(1).Date;
            var franchiseeLeadPerformanceList = _leadPerformanceFranchiseeDetailsRepository.Table.Where(x => x.FranchiseeId == franchiseeId
             && x.Month == currentMonth && (x.DateTime >= currentMonthDate && x.DateTime <= currentMonthLastDate)).OrderByDescending(x => x.Id).ToList();
            if (franchiseeLeadPerformanceList.Count() > 0)
            {
                foreach (var franchiseeLeadPerformance in franchiseeLeadPerformanceList)
                {
                    franchiseeLeadPerformance.IsActive = false;
                    franchiseeLeadPerformance.IsNew = false;
                    _leadPerformanceFranchiseeDetailsRepository.Save(franchiseeLeadPerformance);
                }
                SaveDomainForSEO(franchiseeViewModel.SeoCost, LeadPerformanceEnum.SEOCOST, franchiseeId, (franchiseeViewModel.SeoCostBillingPeriodId), franchiseeViewModel.IsSEOActive);
                SaveDomain(franchiseeViewModel.PpcSpend, LeadPerformanceEnum.PPCSPEND, franchiseeId);
            }
            else
            {
                SaveDomainForSEO(franchiseeViewModel.SeoCost, LeadPerformanceEnum.SEOCOST, franchiseeId, (franchiseeViewModel.SeoCostBillingPeriodId), franchiseeViewModel.IsSEOActive);
                SaveDomain(franchiseeViewModel.PpcSpend, LeadPerformanceEnum.PPCSPEND, franchiseeId);
            }
        }

        private bool SaveDomain(string amount, LeadPerformanceEnum leadPerformanceEnum, long? franchiseeId)
        {
            var leadPerformanceDomain = _franchiseeLeadPerformanceFactory.CreateDomain(Double.Parse(amount), leadPerformanceEnum, franchiseeId);
            leadPerformanceDomain.IsNew = true;
            leadPerformanceDomain.FranchiseeId = franchiseeId.GetValueOrDefault();
            _leadPerformanceFranchiseeDetailsRepository.Save(leadPerformanceDomain);
            return true;
        }

        private bool SaveDomainForSEO(string amount, LeadPerformanceEnum leadPerformanceEnum, long? franchiseeId, long? weekId, bool isSEOActive)
        {
            var leadPerformanceDomain = _franchiseeLeadPerformanceFactory.CreateDomain(Double.Parse(amount), leadPerformanceEnum, franchiseeId);
            leadPerformanceDomain.week = weekId;
            leadPerformanceDomain.IsSEOActive = isSEOActive;
            leadPerformanceDomain.IsNew = true;
            leadPerformanceDomain.FranchiseeId = franchiseeId.GetValueOrDefault();
            _leadPerformanceFranchiseeDetailsRepository.Save(leadPerformanceDomain);
            return true;
        }

        public LeadPerformanceNationalListModel GetLeadPerformanceNationalList(LeadPerformanceFranchiseeFilter filter)
        {
            var isFirstParsed = false;
            var totalCount = default(long);
            var getFrontExeFranchiseeIds = new List<long>();
            var totalCountForFranchiseesList = new LeadPerformanceNationalListModel();
            var totalCountForAdjustedDataList = new List<LeadPerformanceFranchiseeNationalViewModel>();

            var count = new List<long>();
            var viewModel = new List<LeadPerformanceFranchiseeNationalViewModel>();
            var monthDuration = _productReportService.MonthsBetween((DateTime.UtcNow), (DateTime.UtcNow.AddMonths(-12)));

            var categoryIds = _routingNumberRepository.Table.Where(x => (x.CategoryId.Value == (long?)(RoutingNumberCategory.BusinessDirectories))).
                                    Select(y => y.PhoneLabel).ToList();


            var endDateTime = (DateTime.UtcNow).AddDays(1).Date;
            var startDate = endDateTime.AddMonths(-11);
            var firstDayOfMonth = new DateTime(endDateTime.AddMonths(-13).AddDays(-1).Date.Year, endDateTime.AddMonths(-13).AddDays(-1).Date.Month, 1);

            filter.StartDate = firstDayOfMonth;
            filter.EndDate = endDateTime;
            var startDateTime = (filter.StartDate.GetValueOrDefault());
            endDateTime = _clock.ToUtc(endDateTime);
            var listNationalWebLead = _webLeadDataRepository.Table.Where(x => (x.StartDate >= filter.StartDate && x.EndDate <= filter.EndDate)
                                               && !x.IsWeekly).ToList();
            var listCallDetailDomain = _callDetailDataRepository.Table.Where((x => (x.StartDate >= filter.StartDate)
                                     && !x.IsWeekly)).ToList();


            var phoneLabelList = _routingNumberRepository.Fetch(x => !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("Local Bus"));

            var listCallDetail = listCallDetailDomain.GroupBy(x => new { x.PhoneLabel, x.StartDate.Month }).
                                          Select(x => new CallDetailsViewModel
                                          {
                                              PhoneLabel = x.Key.PhoneLabel,
                                              Count = x.Sum(y => y.Count),
                                              PhoneNumber = x.Select(y => y.PhoneNumber).FirstOrDefault(),
                                              StartDate = x.OrderBy(y => y.StartDate).Select(y => y.StartDate).FirstOrDefault(),
                                              EndDate = x.OrderBy(y => y.EndDate).Select(y => y.EndDate).FirstOrDefault(),
                                              FranchiseeId = x.Where(x1 => x1.FranchiseeId != null).Select(x1 => x1.FranchiseeId).FirstOrDefault()
                                          }).ToList();

            var marketingLeadCallsWithoutCcAndLocalAndCorps = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDateTime && x.DateAdded <= endDateTime)
                             && (!x.PhoneLabel.StartsWith("CC") &&
                                    !x.PhoneLabel.StartsWith("CORP") && x.TransferToNumber != "")
                            ).ToList();

            filter.StartDate = startDateTime;
            filter.EndDate = endDateTime;

            var franchiseeNames = GetFranchiseeNames();
            if (filter.OrganizationRoleUserId != null)
            {
                getFrontExeFranchiseeIds = GetFrontOfficeFranchiseeIds(filter.OrganizationRoleUserId.GetValueOrDefault());
            }
            else if (filter.FranchiseeId != null && filter.FranchiseeId != 0)
            {
                franchiseeNames = franchiseeNames.Where(x => x.FranchiseeId == filter.FranchiseeId).ToList();
            }
            if (getFrontExeFranchiseeIds.Count() > 0)
            {
                franchiseeNames = franchiseeNames.Where(x => getFrontExeFranchiseeIds.Contains(x.FranchiseeId.GetValueOrDefault())).ToList();
            }
            var totalAmount = new List<long>();
            var sumOfAverageLeadCost = new List<double>();
            int indexForGettingAverage = 0;
            foreach (var franchisee in franchiseeNames)
            {
                var totalAmountEachMonth = new List<long>();
                var franchiseeViewModel = new FranchiseeViewModelForReport();
                int a = 0;
                filter.FranchiseeId = franchisee.FranchiseeId;
                totalCount = 0;
                listCallDetail = listCallDetailDomain.Where(x => (x.StartDate >= startDateTime.Date)
                && x.FranchiseeId == franchisee.FranchiseeId
                                     && !x.IsWeekly).ToList().GroupBy(x => new { x.PhoneLabel, x.StartDate.Month, x.StartDate.Year }).
                                          Select(x => new CallDetailsViewModel
                                          {
                                              PhoneLabel = x.Key.PhoneLabel,
                                              Count = x.Sum(y => y.Count),
                                              PhoneNumber = x.Select(y => y.PhoneNumber).FirstOrDefault(),
                                              StartDate = x.OrderBy(y => y.StartDate).Select(y => y.StartDate).FirstOrDefault(),
                                              EndDate = x.OrderBy(y => y.EndDate).Select(y => y.EndDate).FirstOrDefault(),
                                              FranchiseeId = x.Where(x1 => x1.FranchiseeId != null).Select(x1 => x1.FranchiseeId).FirstOrDefault(),
                                              Id = x.Select(x1 => x1.Id).ToList(),
                                          }).ToList();
                var listCallDetails = _callDetailDataRepository.Table.Where(x => (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                     && (x.StartDate >= startDateTime.Date)
                                     && !x.IsWeekly).ToList().GroupBy(x => new { x.PhoneLabel, x.StartDate.Month }).
                                          Select(x => new
                                          {
                                              x.Key.PhoneLabel,
                                              Count = x.Sum(y => y.Count),
                                              PhoneNumber = x.Select(y => y.PhoneNumber).FirstOrDefault(),
                                              StartDate = x.OrderBy(y => y.StartDate).Select(y => y.StartDate).FirstOrDefault(),
                                              EndDate = x.OrderBy(y => y.EndDate).Select(y => y.EndDate).FirstOrDefault(),
                                              FranchiseeId = x.Where(x1 => x1.FranchiseeId != null).Select(x1 => x1.FranchiseeId).FirstOrDefault(),
                                              Id = x.Select(x1 => x1.Id)
                                          }).ToList();
                var listCallDetailWithoutCcAndLocal = listCallDetails.Where(x => !x.PhoneLabel.StartsWith("CC") && !categoryIds.Contains(x.PhoneLabel));
                var listCallDetailWithoutCcAndLocalAndCorps = listCallDetailWithoutCcAndLocal.Where(x => !x.PhoneLabel.StartsWith("CORP"));
                var listCallDetailWithCcAndLocal = listCallDetail.Where(x => x.PhoneLabel.StartsWith("CC") || categoryIds.Contains(x.PhoneLabel));


                var adjustedCount = new List<long>();
                var total = new List<long>();
                var webCount = new List<long>();
                var adjustedLocalCount = new List<decimal>();
                var adjustedPPCCount = new List<decimal>();
                var adjustedNationalCount = new List<decimal>();
                var totalCountForAdjustedData = new LeadPerformanceFranchiseeNationalViewModel { };
                franchiseeViewModel.FranchiseeId = franchisee.FranchiseeId;
                franchiseeViewModel.FranchiseeName = franchisee.FranchiseeName;
                var index = 0;

                foreach (var month in monthDuration)
                {
                    string year = month.Item2.ToString().Substring(2);
                    if (!isFirstParsed)
                        totalCountForFranchiseesList.Months.Add(month.Item1 + "/" + year);

                    var firstDayOfEnd = (new DateTime(month.Item2, month.Item1, 1));
                    var daysInMonthStartDate = (DateTime.DaysInMonth(month.Item2, month.Item1));
                    var endDateToCompare = (new DateTime(month.Item2, month.Item1, daysInMonthStartDate)).AddMonths(1);
                    var localWebCount = CalculateWebLeadLocal(listNationalWebLead, franchisee.FranchiseeId, month);

                    var localPhoneNumberList = new List<string>();
                    string[] nationalPhoneNumberLocalListItems = { "8777900604", "8885243372" };

                    string[] localPhoneNumberLocalListItems = { "8882184616" };

                    string[] ppcPhoneNumberNatioListItems = { "8772170043", "8772168760", "8772168276",
                "8772167883", "8772164290", "8772160253" ,"8772159389","8772160212","8882137859","8882141020","8882028319","88882142551"};


                    var nationalDataAdjustedData = GetAdjustedDataLocal(filter, marketingLeadCallsWithoutCcAndLocalAndCorps, listCallDetail,
                  phoneLabelList.ToList(), categoryIds, listNationalWebLead, month, nationalPhoneNumberLocalListItems, true, false);

                    var localDataAdjustedData = GetAdjustedDataLocal(filter, marketingLeadCallsWithoutCcAndLocalAndCorps, listCallDetail,
                    phoneLabelList.ToList(), categoryIds, listNationalWebLead, month, localPhoneNumberLocalListItems, false, false);

                    var ppcDataAdjustedData = GetAdjustedDataLocal(filter, marketingLeadCallsWithoutCcAndLocalAndCorps, listCallDetail,
                    phoneLabelList.ToList(), categoryIds, listNationalWebLead, month, ppcPhoneNumberNatioListItems, false, true);



                    totalCount = (long)(localWebCount + localDataAdjustedData + nationalDataAdjustedData + ppcDataAdjustedData);
                    adjustedCount.Add(totalCount);

                    adjustedLocalCount.Add(localDataAdjustedData);
                    adjustedPPCCount.Add(ppcDataAdjustedData);
                    webCount.Add(localWebCount);
                    adjustedNationalCount.Add(nationalDataAdjustedData);
                    totalAmountEachMonth.Add(totalCount);
                    if (totalAmount.Count <= 12)
                    {
                        totalAmount.Add(totalCount);
                    }
                    else
                    {
                        var totalAmountToBeChange = totalAmount[index] + totalCount;
                        totalAmount[index] = totalAmountToBeChange;
                        index = index + 1;
                    }

                    a += 1;
                }

                isFirstParsed = true;
                var getLocalData = GetSeoAndPpcLocal(franchisee.FranchiseeId.GetValueOrDefault(), totalAmountEachMonth);

                sumOfAverageLeadCost = GetTotalSum(getLocalData.LeadWebSiteFranchiseeViewModel, indexForGettingAverage, sumOfAverageLeadCost);

                ++indexForGettingAverage;

                totalCountForAdjustedData.FranchiseeName = franchisee.FranchiseeName;
                totalCountForAdjustedData.CountAdjustedData = adjustedCount;
                totalCountForAdjustedData.IsZero = false;
                totalCountForAdjustedData.IsExpand = false;
                totalCountForAdjustedData.AdjustedDataLocal = adjustedLocalCount;
                totalCountForAdjustedData.AdjustedDataPPC = adjustedPPCCount;
                totalCountForAdjustedData.Total = totalAmount;
                totalCountForAdjustedData.WebCount = webCount;
                totalCountForAdjustedData.AdjustedDataNational = adjustedNationalCount;
                totalCountForAdjustedData.AdjustedDataLocal = (adjustedLocalCount);
                totalCountForAdjustedData.LeadWebSiteFranchiseeViewModel = getLocalData.LeadWebSiteFranchiseeViewModel;
                totalCountForFranchiseesList.LeadPerformanceFranchiseeNationalViewModel.Add(totalCountForAdjustedData);
                totalCountForAdjustedData.Average = (getLocalData.Average);
            }
            totalCountForFranchiseesList.TotalOfLeadCount = sumOfAverageLeadCost;
            return totalCountForFranchiseesList;
        }



        private long CalculateWebLeadLocal(List<WebLeadData> listLocalWebLead, long? franchiseeId, Tuple<int, int> month)
        {
            var webCount = listLocalWebLead.Where(i => i.FranchiseeId == franchiseeId && i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);
            //listLocalWebLead = listLocalWebLead.Where(x => x.FranchiseeId == franchiseeId && x.StartDate >= startDate && x.EndDate <= endDate).ToList();
            return webCount;
        }



        private int getAdjustedData(int value, int totalCount, int ccCount, int localCount, int webCount)
        {
            decimal count = default(decimal);
            if (totalCount != 0 && value != 0)
            {
                decimal devide = (decimal)localCount / ((decimal)totalCount);
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
                decimal websub = webCount + (totalCount);
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


        private decimal GetAdjustedDataLocal(LeadPerformanceFranchiseeFilter filter, List<MarketingLeadCallDetail>
           marketingLeadCallsWithoutCcAndLocalAndCorps, List<CallDetailsViewModel> listCallDetail, List<RoutingNumber> phoneLabelList,
           List<string> categoryIds, List<WebLeadData> listWebLead, Tuple<int, int> month, string[] localPhoneNumberList, bool isNational, bool isPPC)
        {
            if (isPPC)
            {
                phoneLabelList = phoneLabelList.Where(x => x.PhoneLabel.Contains("Google PPC")).ToList();
            }
            else if (!isNational)
                phoneLabelList = phoneLabelList.Where(x => localPhoneNumberList.Contains(x.PhoneNumber)).ToList();
            else if (isNational)
            {
                phoneLabelList = phoneLabelList.Where(x => localPhoneNumberList.Contains(x.PhoneNumber) ||
                x.PhoneLabel.StartsWith("Main Site US")).ToList();
            }
            var isMainSiteUsParsed = false;
            var isMarbleLifeParsed = false;
            listWebLead = listWebLead.Where(x => x.FranchiseeId == filter.FranchiseeId).ToList();
            listCallDetail = listCallDetail.Where(x => filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId).ToList();
            var adjustedDataByMonth = new List<int>();
            var adjustedDataByMonthSum = default(decimal);
            int webCount = 0;
            int phoneLabelCount = 0;
            marketingLeadCallsWithoutCcAndLocalAndCorps = marketingLeadCallsWithoutCcAndLocalAndCorps.Where(x => filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId).ToList();


            var firstDayOfMonth = new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, 1);

            var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC") && !categoryIds.Contains(x.PhoneLabel));
            var listCallDetailWithoutCcAndLocalAndCorps = listCallDetailWithoutCcAndLocal.Where(x => !x.PhoneLabel.StartsWith("CORP"));
            var listCallDetailWithCcAndLocal = listCallDetail.Where(x => x.PhoneLabel.StartsWith("CC") || categoryIds.Contains(x.PhoneLabel));

            var daysInMonth = DateTime.DaysInMonth(month.Item2, month.Item1);
            var startDateTime = _clock.ToUtc(new DateTime(month.Item2, month.Item1, 01));
            var endDateTime = _clock.ToUtc(new DateTime(month.Item2, month.Item1, daysInMonth)).AddDays(1);

            foreach (var phoneLabel in phoneLabelList)
            {
                var collection1 = listCallDetail.Where(x => (x.PhoneLabel.StartsWith(phoneLabel.PhoneLabel)));
                var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.PhoneNumber.Equals(phoneLabel.PhoneNumber)));
                if (!collection.Any())
                    continue;

                if (phoneLabel.PhoneLabel == "Main Site US")
                {
                    if (!isMainSiteUsParsed)
                    {
                        isMainSiteUsParsed = true;
                        collection = collection1;
                    }
                    else
                    {
                        isMainSiteUsParsed = true;
                        continue;
                    }

                }
                else if (phoneLabel.PhoneLabel == "Marblelife Local - All Referral")
                {
                    if (!isMarbleLifeParsed)
                    {
                        isMarbleLifeParsed = true;
                        collection = collection1;
                    }
                    else
                    {
                        isMarbleLifeParsed = true;
                        continue;
                    }
                }
                phoneLabelCount = phoneLabelCount + 1;
                var marketingLeadCallsWithoutCcAndLocalAndCorp = marketingLeadCallsWithoutCcAndLocalAndCorps.Where(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime).ToList();

                var row = listCallDetailWithCcAndLocal.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2);
                var ccCount = row.Where(i => i.PhoneLabel.StartsWith("CC")).Sum(i => i.Count);
                var localCount = row.Where(i => i.PhoneLabel.StartsWith("Local Bus")).Sum(i => i.Count);
                var totalCount = marketingLeadCallsWithoutCcAndLocalAndCorp.Count();
                var value = collection.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);
                webCount = listWebLead.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);

                var count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);

                adjustedDataByMonthSum += count;
            }
            return adjustedDataByMonthSum;
        }

        private List<FranchiseeViewModelForReport> GetFranchiseeNames()
        {
            var franchisee = _franchiseeRepository.Table.Where(x => x.Id > 2 && x.Id != 63).ToList();

            var franchiseeViewModel = franchisee.Select(x => CreateViewModel(x)).ToList();
            return franchiseeViewModel.OrderBy(x => x.FranchiseeName).ToList();
        }

        private FranchiseeViewModelForReport CreateViewModel(Franchisee franchisee)
        {
            return new FranchiseeViewModelForReport
            {
                FranchiseeName = franchisee.Organization.Name,
                FranchiseeId = franchisee.Organization.Id
            };
        }

        public LeadFranchiseeNationallListModel GetSeoAndPpcNational(LeadPerformanceFranchiseeFilter filter)
        {
            var isFirst = true;
            var franchiseeNames = GetFranchiseeNames();
            var leadPerformanceFranchiseeSeoDataAmount = default(double);
            List<LeadWebSiteFranchiseeLocalViewModel> leadWebSiteFranchiseeViewModelList = new List<LeadWebSiteFranchiseeLocalViewModel>();
            LeadFranchiseeNationallListModel leadFranchiseeLocalListModel = new LeadFranchiseeNationallListModel { };
            LeadFranchiseeNationallListModel leadFranchiseeLocalListModelForReturn = new LeadFranchiseeNationallListModel();
            var monthDuration = _productReportService.MonthsBetween(_clock.ToLocal(DateTime.UtcNow), _clock.ToLocal(DateTime.UtcNow.AddMonths(-12)));
            var endDate = DateTime.Now.Date;
            var listAmount = new List<double>();
            var startDate = DateTime.Now.Date.AddMonths(-13);
            var startDateForQuery = new DateTime(startDate.Year, startDate.Month, 01);
            var leadPerformanceFranchiseeDetails = _leadPerformanceFranchiseeDetailsRepository.Table.Where(x => x.DateTime >= startDateForQuery && x.DateTime <= endDate).ToList();
            var forEmptyValue = new LeadWebSiteFranchiseeLocalViewModel();
            foreach (var franchiseeName in franchiseeNames)
            {
                if (isFirst)
                {
                    for (int a1 = 0; a1 < 12; a1++)
                    {
                        forEmptyValue.IsZero = true;
                        forEmptyValue.CountLocal.Add(0);
                    }
                    leadFranchiseeLocalListModel.TotalCount.Add(forEmptyValue);


                }
                isFirst = false;
                listAmount = new List<double>();
                var leadWebSiteFranchiseeViewModel = new LeadWebSiteFranchiseeLocalViewModel { };
                foreach (var month in monthDuration)
                {
                    var date = new DateTime(month.Item2, month.Item1, 1).Date;
                    var leadPerformanceFranchiseeSeoData = leadPerformanceFranchiseeDetails.FirstOrDefault(x => x.Month == month.Item1
                     && x.DateTime == date && x.FranchiseeId == franchiseeName.FranchiseeId && x.IsActive && x.CategoryId == (long)LeadPerformanceEnum.SEOCOST);
                    if (leadPerformanceFranchiseeSeoData != null)
                    {
                        leadPerformanceFranchiseeSeoDataAmount = leadPerformanceFranchiseeSeoData.Amount;
                    }
                    var leadPerformanceFranchiseePpcData = leadPerformanceFranchiseeDetails.FirstOrDefault(x => x.Month == month.Item1
                          && x.DateTime == date && x.FranchiseeId == franchiseeName.FranchiseeId && x.IsActive && x.CategoryId == (long)LeadPerformanceEnum.PPCSPEND).Amount;
                    var totalSum = leadPerformanceFranchiseeSeoDataAmount + leadPerformanceFranchiseePpcData;
                    listAmount.Add(totalSum);
                }
                leadWebSiteFranchiseeViewModel.CountLocal = listAmount;
                leadWebSiteFranchiseeViewModel.IsZero = false;
                leadWebSiteFranchiseeViewModel.FranchiseeName = franchiseeName.FranchiseeName;
                leadFranchiseeLocalListModel.TotalCount.Add(leadWebSiteFranchiseeViewModel);
            }
            return leadFranchiseeLocalListModel;
        }

        public LeadFranchiseeLocalListModel GetSeoAndPpcLocal(long franchiseeId, List<long> totalCount)
        {
            var franchiseeNames = GetFranchiseeNames();
            var listOfSeo = new List<double>();
            var listOfPpc = new List<double>();
            var listOfTotal = new List<double>();
            var listOfAverage = new List<double>();
            LeadWebSiteFranchiseeViewModel leadWebSiteFranchiseeViewModel = new LeadWebSiteFranchiseeViewModel();
            List<LeadWebSiteFranchiseeViewModel> leadWebSiteFranchiseeViewModelList = new List<LeadWebSiteFranchiseeViewModel>();
            LeadFranchiseeLocalListModel leadFranchiseeLocalListModel = new LeadFranchiseeLocalListModel();
            var monthDuration = _productReportService.MonthsBetween(_clock.ToLocal(DateTime.UtcNow), _clock.ToLocal(DateTime.UtcNow.AddMonths(-12)));
            var endDate = DateTime.UtcNow.AddDays(1).Date;
            var startDate = DateTime.UtcNow.Date.AddMonths(-13);
            var startDateForQuery = new DateTime(startDate.Year, startDate.Month, 01);
            var leadPerformanceFranchiseeDetails = _leadPerformanceFranchiseeDetailsRepository.Table.Where(x => x.DateTime >= startDateForQuery && x.DateTime <= endDate && x.IsActive).OrderByDescending(x => x.Id).ToList();
            franchiseeNames = franchiseeNames.Where(x => x.FranchiseeId == franchiseeId).ToList();
            var index = 0;
            foreach (var month in monthDuration)
            {
                var average = default(double);
                var leadPerformanceFranchiseePpcAmount = default(double);
                var leadPerformanceFranchiseeSeoAmount = default(double);
                leadWebSiteFranchiseeViewModel = new LeadWebSiteFranchiseeViewModel { };
                var date = new DateTime(month.Item2, month.Item1, 1).Date;
                var daysInMonthStartDate = (DateTime.DaysInMonth(month.Item2, month.Item1));
                var endDateForQuery = new DateTime(month.Item2, month.Item1, daysInMonthStartDate).Date;
                var leadPerformanceFranchiseeSeoDataDomain = leadPerformanceFranchiseeDetails.FirstOrDefault(x => x.Month == month.Item1
                     && x.DateTime >= date && x.DateTime <= endDateForQuery && x.FranchiseeId == franchiseeId && x.IsActive && x.CategoryId == (long)LeadPerformanceEnum.SEOCOST);

                if (leadPerformanceFranchiseeSeoDataDomain == null)
                {
                    leadPerformanceFranchiseeSeoDataDomain = leadPerformanceFranchiseeDetails.FirstOrDefault(x =>
                     x.FranchiseeId == franchiseeId && x.IsActive && x.CategoryId == (long)LeadPerformanceEnum.SEOCOST);
                    leadPerformanceFranchiseeSeoAmount = leadPerformanceFranchiseeSeoDataDomain!=null? leadPerformanceFranchiseeSeoDataDomain.Amount:default(double);
                }
                else
                {
                    leadPerformanceFranchiseeSeoAmount = leadPerformanceFranchiseeSeoDataDomain.Amount;
                }
                var leadPerformanceFranchiseePpcDomain = leadPerformanceFranchiseeDetails.FirstOrDefault(x => x.Month == month.Item1
                      && x.DateTime == date && x.FranchiseeId == franchiseeId && x.IsActive && x.CategoryId == (long)LeadPerformanceEnum.PPCSPEND);


                if (leadPerformanceFranchiseePpcDomain == null)
                {
                    leadPerformanceFranchiseePpcDomain = leadPerformanceFranchiseeDetails.FirstOrDefault(x =>
                     x.FranchiseeId == franchiseeId && x.IsActive && x.CategoryId == (long)LeadPerformanceEnum.PPCSPEND);
                    leadPerformanceFranchiseePpcAmount = leadPerformanceFranchiseePpcDomain!=null? leadPerformanceFranchiseePpcDomain.Amount : default(double);
                }
                else
                {
                    leadPerformanceFranchiseePpcAmount = leadPerformanceFranchiseePpcDomain.Amount;
                }

                var total = leadPerformanceFranchiseeSeoAmount + leadPerformanceFranchiseePpcAmount;
                listOfSeo.Add(leadPerformanceFranchiseeSeoAmount);
                listOfPpc.Add(leadPerformanceFranchiseePpcAmount);
                listOfTotal.Add(total);
                //var getTotal = GetTotalSum(getLocalData.LeadWebSiteFranchiseeViewModel,
                //                                  indexForGettingAverage, sumOfAverageLeadCost);
                if (totalCount != null)
                {
                    if (totalCount[index] != 0)
                        average = total / totalCount[index];
                    else
                        average = 0;
                }
                index = index + 1;
                listOfAverage.Add(average);
            }
            leadWebSiteFranchiseeViewModel.WebSiteSeoCost = listOfSeo;
            leadWebSiteFranchiseeViewModel.WebSitePpcCost = listOfPpc;
            leadWebSiteFranchiseeViewModel.Total = listOfTotal;
            leadWebSiteFranchiseeViewModel.Average = listOfAverage;
            leadWebSiteFranchiseeViewModelList.Add(leadWebSiteFranchiseeViewModel);
            leadFranchiseeLocalListModel.Average = listOfAverage;
            leadFranchiseeLocalListModel.LeadWebSiteFranchiseeViewModel.AddRange(leadWebSiteFranchiseeViewModelList);
            return leadFranchiseeLocalListModel;
        }


        private List<long> GetTotal(List<LeadPerformanceFranchiseeNationalViewModel> leadPerformanceFranchiseeNationalViewModel)
        {
            var totalAdjustedData = leadPerformanceFranchiseeNationalViewModel.Select(x => x.CountAdjustedData).ToList();

            return default;
        }

        private List<long> GetFrontOfficeFranchiseeIds(long? organizationRoleUserId)
        {
            return _organizationRoleUserFranchiseeRepository.Table.Where(x => x.OrganizationRoleUserId == organizationRoleUserId && x.IsActive).Select(x => x.FranchiseeId).ToList();
        }

        public List<double> GetTotalSum(List<LeadWebSiteFranchiseeViewModel> model, int index, List<double> list)
        {
            int loopIndex = 0;
            var listOfList = model.Select(x => x.Total).ToList();
            var totalList = listOfList[0];
            if (index == 0)
            {
                list.AddRange(totalList);
            }
            else
            {
                foreach (var value in totalList)
                {
                    list[loopIndex] = list[loopIndex] + value;
                    loopIndex++;
                }
            }
            return list;
        }

    }

}
