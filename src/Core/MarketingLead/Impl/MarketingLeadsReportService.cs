using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Geo.Domain;
using Core.MarketingLead.Domain;
using Core.MarketingLead.Enum;
using Core.MarketingLead.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using Core.Reports;
using Core.Sales;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using Core.Users.Enum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class MarketingLeadsReportService : IMarketingLeadsReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISortingHelper _sortingHelper;
        private readonly IMarketingLeadsFactory _marketingleadsFactory;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepository;
        private readonly IRepository<CallDetailData> _callDetailDataRepository;
        private readonly IRepository<RoutingNumber> _routingNumberRepository;
        private readonly IRepository<WebLead> _webLeadRepository;
        private readonly IRepository<WebLeadData> _webLeadDataRepository;
        private readonly IRepository<HomeAdvisor> _homeAdvisorRepository;
        private readonly IClock _clock;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IExcelFileCreatorMarketingLead _excelFileCreatorMarketingLead;
        private readonly ICustomerService _customerService;
        private readonly IProductReportService _productReportService;
        private readonly IRepository<MarketingLeadCallDetailV2> _marketingLeadCallDetailV2Repository;
        private readonly IRepository<FranchiseeTechMailEmail> _franchiseeTechMailEmailRepository;
        private readonly IRepository<FranchiseeTechMailService> _franchiseeTechMailServiceRepository;
        private readonly IRepository<Phonechargesfee> _phonechargesfeeRepository;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<FranchiseeServiceFee> _franchiseeServiceFeeRepository;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IFranchiseeServiceFeeFactory _franchiseeServiceFeeFactory;
        private readonly IRepository<MarketingLeadCallDetailV2> _marketingLeadCallDetail2Repository;
        private readonly IRepository<MarketingLeadCallDetailV3> _marketingLeadCallDetail3Repository;
        private readonly IRepository<MarketingLeadCallDetailV4> _marketingLeadCallDetail4Repository;
        private readonly IRepository<MarketingLeadCallDetailV5> _marketingLeadCallDetail5Repository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<CallDetailsReportNotes> _callDetailsReportNotesRepository;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly IRepository<County> _countyRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<Role> _roleRepository;
        private ILogService _logService;
        public MarketingLeadsReportService(IUnitOfWork unitOfWork, IClock clock, ISortingHelper sortingHelper, IMarketingLeadsFactory marketingLeadsFactory,
            IExcelFileCreator excelFileCreator, ICustomerService customerService, IProductReportService productReportService, IFranchiseeServiceFeeFactory franchiseeServiceFeeFactory, IExcelFileCreatorMarketingLead excelFileCreatorMarketingLead, IOrganizationRoleUserInfoService organizationRoleUserInfoService, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _marketingleadsFactory = marketingLeadsFactory;
            _marketingLeadCallDetailRepository = unitOfWork.Repository<MarketingLeadCallDetail>();
            _callDetailDataRepository = unitOfWork.Repository<CallDetailData>();
            _sortingHelper = sortingHelper;
            _routingNumberRepository = unitOfWork.Repository<RoutingNumber>();
            _webLeadRepository = unitOfWork.Repository<WebLead>();
            _webLeadDataRepository = unitOfWork.Repository<WebLeadData>();
            _clock = clock;
            _excelFileCreator = excelFileCreator;
            _customerService = customerService;
            _productReportService = productReportService;
            _homeAdvisorRepository = unitOfWork.Repository<HomeAdvisor>();
            _marketingLeadCallDetailV2Repository = unitOfWork.Repository<MarketingLeadCallDetailV2>();
            _franchiseeTechMailEmailRepository = unitOfWork.Repository<FranchiseeTechMailEmail>();
            _franchiseeTechMailServiceRepository = unitOfWork.Repository<FranchiseeTechMailService>();
            _phonechargesfeeRepository = unitOfWork.Repository<Phonechargesfee>();
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _franchiseeServiceFeeRepository = unitOfWork.Repository<FranchiseeServiceFee>();
            _franchiseeServiceFeeFactory = franchiseeServiceFeeFactory;
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _marketingLeadCallDetail3Repository = unitOfWork.Repository<MarketingLeadCallDetailV3>();
            _marketingLeadCallDetail4Repository = unitOfWork.Repository<MarketingLeadCallDetailV4>();
            _marketingLeadCallDetail5Repository = unitOfWork.Repository<MarketingLeadCallDetailV5>();
            _excelFileCreatorMarketingLead = excelFileCreatorMarketingLead;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _personRepository = unitOfWork.Repository<Person>();
            _marketingLeadCallDetail2Repository = unitOfWork.Repository<MarketingLeadCallDetailV2>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _callDetailsReportNotesRepository = unitOfWork.Repository<CallDetailsReportNotes>();
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _countyRepository = unitOfWork.Repository<County>();
            _stateRepository = unitOfWork.Repository<State>();
            _roleRepository = unitOfWork.Repository<Role>();
            _logService = logService;
        }

        public CallDetailListModel GetCallDetailList(CallDetailFilter filter, int pageNumber, int pageSize)
        {
            var callDetailList = GetCallDetailList(filter);

            var finalcollection = callDetailList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var marketingClassIds = finalcollection.Select(x => x.Id).ToList();
            var marketingClassAPI2 = _marketingLeadCallDetail2Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var marketingClassAPI3 = _marketingLeadCallDetail3Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var marketingClassAPI4 = _marketingLeadCallDetail4Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var marketingClassAPI5 = _marketingLeadCallDetail5Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var callNotes = _callDetailsReportNotesRepository.Table.ToList();
            return new CallDetailListModel
            {
                Collection = finalcollection.Select(x => _marketingleadsFactory.CreateNewViewModel(x,
                marketingClassAPI3.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id),
                marketingClassAPI4.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id),
                marketingClassAPI5.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id),
                marketingClassAPI2.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id),
                callNotes.Where(y => x.CallerId == y.CallerId).ToList())).ToList(),
                //Collection = finalcollection.Select(_marketingleadsFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, callDetailList.Count())
            };
        }



        private IQueryable<MarketingLeadCallDetail> GetCallDetailList(CallDetailFilter filter)
        {
            var id = default(long?);
            if (filter.CallerId == "null" || filter.CallerId == null)
            {
                filter.CallerId = "0";
            }
            var categoryIds = new List<long>();

            if (filter.CategoryIds != null)
            {
                var categoryListIds = filter.CategoryIds.Split(',').ToArray();
                foreach (var item in categoryListIds)
                {
                    categoryIds.Add(Convert.ToInt64(item));
                }
            }
            var classTypes = _routingNumberRepository.Table.Where(x => (categoryIds.Contains(x.CategoryId.Value))).
                                    Select(y => y.PhoneLabel);

            if (filter.StartDate != null)
            {
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
            }
            if (filter.EndDate != null)
            {
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
            }
            if (filter.MarketingLeadId != null)
            {

                var marketingLeadCallDetails2 = _marketingLeadCallDetail2Repository.Get(filter.MarketingLeadId);
                if (marketingLeadCallDetails2 != null && marketingLeadCallDetails2.MarketingLeadCallDetailId != null)
                {
                    id = marketingLeadCallDetails2.MarketingLeadCallDetailId;
                }
            }
            if (filter.Office != null)
            {
            }
            var toDate = filter.EndDate.HasValue ? filter.EndDate.Value.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var list = _marketingLeadCallDetailRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                             && (string.IsNullOrEmpty(filter.Text) || (x.CallTransferType.Contains(filter.Text))
                             || (x.DialedNumber.Equals(filter.Text)) || (x.CallerId.Equals(filter.Text))
                             || (x.PhoneLabel.Contains(filter.Text)) || (x.TransferToNumber.Equals(filter.Text))
                             || (x.Id.ToString().Equals(filter.Text))
                             || (x.CallerId.Contains(filter.CallerId.Replace("-", "")))
                             || (x.InvoiceId.ToString().Equals(filter.Text)))
                             && (filter.ConvertedLead == null || (filter.ConvertedLead == 1 ? (x.Invoice != null && x.InvoiceId != null) : x.InvoiceId == null))
                             && (filter.MappedFranchisee == null || (filter.MappedFranchisee == 1 ? (x.FranchiseeId != null) : x.FranchiseeId == null))
                             && (filter.TagId <= 0 || x.TagId == filter.TagId)
                             && (filter.CallerId == "0" || x.CallerId.Contains(filter.CallerId.Replace("-", "")))
                             && (filter.StartDate == null || (x.DateAdded >= filter.StartDate))
                             && (toDate == null || (x.DateAdded <= toDate))
                             && (filter.CallTypeId <= 0 || (x.CallTypeId == filter.CallTypeId))
                             && (id == null || x.Id == id)
                             && (filter.TransferToNumber == "null" || filter.TransferToNumber == null || x.TransferToNumber.Contains(filter.TransferToNumber.Replace("-", "")))
                             );

            if (list.Count() == 0 && id != null)
            {
                list = _marketingLeadCallDetailRepository.Table.Where(x => x.Id == id);
            }
            var callDetails = list.Where(x => !classTypes.Any() || classTypes.Contains(x.PhoneLabel));
            callDetails = _sortingHelper.ApplySorting(callDetails, x => x.DateAdded, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "ID":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.Id, filter.SortingOrder);
                        break;
                    case "DialedNumber":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.DialedNumber, filter.SortingOrder);
                        break;
                    case "CallerId":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.CallerId, filter.SortingOrder);
                        break;
                    case "PhoneLabel":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.PhoneLabel, filter.SortingOrder);
                        break;
                    case "TransferTo":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.TransferToNumber, filter.SortingOrder);
                        break;
                    case "TransferType":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.CallTransferType, filter.SortingOrder);
                        break;
                    case "Date":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.DateAdded, filter.SortingOrder);
                        break;
                    case "CallType":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.CallType.Name, filter.SortingOrder);
                        break;
                    case "CallDuration":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.CallDuration, filter.SortingOrder);
                        break;
                    case "Franchisee":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "CalledFranchisee":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.CalledFranchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "InvoiceId":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.InvoiceId, filter.SortingOrder);
                        break;
                    case "Tag":
                        callDetails = _sortingHelper.ApplySorting(callDetails, x => x.Tag.Name, filter.SortingOrder);
                        break;
                }
            }
            return callDetails;
        }

        public RoutingNumberListModel GetRoutingNumberList(CallDetailFilter filter, int pageNumber, int pageSize)
        {
            var routinNumberList = GetRoutingNumberFilterList(filter);
            var finalcollection = routinNumberList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new RoutingNumberListModel
            {
                Collection = finalcollection.Select(_marketingleadsFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, routinNumberList.Count())
            };
        }

        private IQueryable<RoutingNumber> GetRoutingNumberFilterList(CallDetailFilter filter)
        {
            var routinNumberList = _routingNumberRepository.Table.Where(x => (filter.FranchiseeId <= 1 || x.FranchiseeId == filter.FranchiseeId)
                                    && (filter.TagId <= 0 || filter.TagId == x.TagId)
                                    && (string.IsNullOrEmpty(filter.Text)
                                    || x.PhoneNumber.Contains(filter.Text) || x.PhoneLabel.Contains(filter.Text)));

            routinNumberList = _sortingHelper.ApplySorting(routinNumberList, x => x.Id, (long)SortingOrder.Asc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "ID":
                        routinNumberList = _sortingHelper.ApplySorting(routinNumberList, x => x.Id, filter.SortingOrder);
                        break;
                    case "PhoneNumber":
                        routinNumberList = _sortingHelper.ApplySorting(routinNumberList, x => x.PhoneNumber, filter.SortingOrder);
                        break;
                    case "PhoneLabel":
                        routinNumberList = _sortingHelper.ApplySorting(routinNumberList, x => x.PhoneLabel, filter.SortingOrder);
                        break;
                    case "Franchisee":
                        routinNumberList = _sortingHelper.ApplySorting(routinNumberList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "Tag":
                        routinNumberList = _sortingHelper.ApplySorting(routinNumberList, x => x.Tag.Name, filter.SortingOrder);
                        break;
                }
            }
            return routinNumberList;
        }

        public bool UpdateFranchisee(long id, long? franchiseeId)
        {
            var routingNumber = _routingNumberRepository.Get(id);
            if (routingNumber != null)
            {
                routingNumber.FranchiseeId = franchiseeId <= 0 ? null : franchiseeId;
                _routingNumberRepository.Save(routingNumber);
                return true;
            }
            return false;
        }

        public WebLeadListViewModel GetWebLeadList(WebLeadFilter filter, int pageNumber, int pageSize)
        {
            var webleadsList = GetWebLeadList(filter);

            var finalcollection = webleadsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new WebLeadListViewModel
            {
                Collection = finalcollection.Select(_marketingleadsFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, webleadsList.Count())
            };
        }

        private IQueryable<WebLead> GetWebLeadList(WebLeadFilter filter)
        {
            if (filter.StartDate != null && filter.EndDate != null)
            {
                filter.StartDate = (filter.StartDate.GetValueOrDefault());
                filter.EndDate = (filter.EndDate.GetValueOrDefault()).AddDays(1);
            }
            var webLeadsDomain = _webLeadRepository.Table.Where(x => (filter.FranchiseeId <= 1 || x.FranchiseeId == filter.FranchiseeId)
                             && (string.IsNullOrEmpty(filter.Text)
                             || (x.Email.Contains(filter.Text)) || (x.FEmail.Contains(filter.Text))
                             || (x.Phone.Contains(filter.Text)) || (x.StreetAddress.Contains(filter.Text))
                             || (x.SurfaceType.Contains(filter.Text)) || (x.Contact.Contains(filter.Text))
                             || (x.Country.Contains(filter.Text))
                             || (x.ProvinceName.Contains(filter.Text))
                             || (x.City.Contains(filter.Text))
                             || (x.Id.ToString().Equals(filter.Text))
                             || (x.InvoiceId.ToString().Equals(filter.Text))
                             || (x.ZipCode.Contains(filter.Text)))
                             && (string.IsNullOrEmpty(filter.URL) || x.URL.Contains(filter.URL))
                             && (string.IsNullOrEmpty(filter.ZipCode) || x.ZipCode.Contains(filter.ZipCode))
                             && (string.IsNullOrEmpty(filter.Street) || x.StreetAddress.Contains(filter.Street))
                             && (string.IsNullOrEmpty(filter.City) || x.City.Contains(filter.City))
                             && (string.IsNullOrEmpty(filter.State) || x.ProvinceName.Contains(filter.State))
                             && (filter.ConvertedLead == null || (filter.ConvertedLead == 1 ? (x.Invoice != null && x.InvoiceId != null) : x.InvoiceId == null))
                             && (filter.StartDate == null || x.CreatedDate >= filter.StartDate)
                             && (filter.EndDate == null || x.CreatedDate < filter.EndDate)
                             //&& (string.IsNullOrEmpty(filter.Name) || (filter.Name.StartsWith(x.Firstname) && filter.Name.EndsWith(x.LastName)))
                             && (string.IsNullOrEmpty(filter.PropertyType) || x.PropertyType.ToLower().Contains(filter.PropertyType.ToLower()))).ToList();

            if (filter.Name != null)
            {
                webLeadsDomain = webLeadsDomain.Where(x => x.FullName.Contains(filter.Name)).ToList();
            }

            var webLeads = webLeadsDomain.AsQueryable();
            webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Id, (long)SortingOrder.Desc).OrderByDescending(t => t.CreatedDate);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "ID":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Id, filter.SortingOrder);
                        break;
                    //case "WebLeadId":
                    //    webLeads = _sortingHelper.ApplySorting(webLeads, x => x.WebLeadId, filter.SortingOrder);
                    //    break;
                    case "Name":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Firstname, filter.SortingOrder);
                        break;
                    case "Email":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Email, filter.SortingOrder);
                        break;
                    case "Phone":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Phone, filter.SortingOrder);
                        break;
                    case "PropertyType":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.PropertyType, filter.SortingOrder);
                        break;
                    case "SurfaceType":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.SurfaceType, filter.SortingOrder);
                        break;
                    case "Description":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.ServiceDesc, filter.SortingOrder);
                        break;
                    case "Contact":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Contact, filter.SortingOrder);
                        break;
                    case "Franchisee":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Franchisee == null ? "" : x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "FEmail":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.FEmail, filter.SortingOrder);
                        break;
                    case "Date":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.CreatedDate, filter.SortingOrder);
                        break;
                    case "URL":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.URL, filter.SortingOrder);
                        break;
                    case "InvoiceId":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.InvoiceId, filter.SortingOrder);
                        break;
                    case "Country":
                        webLeads = _sortingHelper.ApplySorting(webLeads, x => x.Country, filter.SortingOrder);
                        break;
                }
            }
            return webLeads;
        }

        public bool DownloadWebLeads(WebLeadFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<WebLeadInfoModel>();
            IEnumerable<WebLead> reportList = GetWebLeadList(filter).ToList();

            //prepare item collection
            foreach (var item in reportList)
            {
                try
                {
                    var model = _marketingleadsFactory.CreateViewModel(item);
                    reportCollection.Add(model);
                }
                catch(Exception ex)
                {
                    _logService.Error("Error in Download Web Leads: ", ex);
                }
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/webLead-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public bool DownloadMarketingLeads(CallDetailFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<CallDetailViewModel>();
            IEnumerable<MarketingLeadCallDetail> reportList = GetCallDetailList(filter).ToList();

            //prepare item collection
            foreach (var item in reportList)
            {
                var model = _marketingleadsFactory.CreateViewModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/marketingLead-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public bool DownloadRoutingNumber(CallDetailFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<RoutingNumberViewModel>();
            IEnumerable<RoutingNumber> reportList = GetRoutingNumberFilterList(filter).ToList();

            //prepare item collection
            foreach (var item in reportList)
            {
                var model = _marketingleadsFactory.CreateViewModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/routingNumber-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public bool UpdateTag(long id, long? tagId)
        {
            var routingNumber = _routingNumberRepository.Get(id);
            if (routingNumber != null)
            {
                routingNumber.TagId = tagId <= 0 ? null : tagId;
                _routingNumberRepository.Save(routingNumber);
                return true;
            }
            return false;
        }

        public WebLeadReportListModel GetWebLeadReport(MarketingLeadReportFilter filter)
        {
            var webLeadList = GetWebLeadReportList(filter);

            var list = webLeadList.Skip((filter.WebPageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

            return new WebLeadReportListModel
            {
                Collection = list,
                Filter = filter,
                PagingModel = new PagingModel(filter.WebPageNumber, filter.PageSize, webLeadList.Count())
            };
        }

        private IQueryable<WebLeadReportViewModel> GetWebLeadReportList(MarketingLeadReportFilter filter)
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



            List<WebLeadReportViewModel> list = new List<WebLeadReportViewModel>();
            var urlList = _webLeadRepository.Fetch(x => (string.IsNullOrEmpty(filter.URL) || x.URL.Equals(filter.URL))).Select(x => x.URL).Distinct();

            if (filter.ViewTypeId == yearView)
            {
                filter.EndDate = filter.StartDate.Value.AddYears(yearCount).Date;
                var yearList = DateRangeHelperService.GetYearsBetween(filter.StartDate.Value, filter.EndDate.Value);

                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);

                var listWebLead = _webLeadDataRepository.Fetch(x => (x.StartDate <= filter.StartDate && x.EndDate >= filter.EndDate)
                     && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                     && !x.IsWeekly);

                foreach (var item in urlList)
                {
                    var collection = listWebLead.Where(x => x.Url.Equals(item));
                    var model = new WebLeadReportViewModel { };
                    model.lstHeader = new List<HeaderDataWebLeadCollection>();
                    model.URL = item;
                    foreach (var year in yearList.OrderBy(x => x))
                    {
                        var headerModel = new HeaderDataWebLeadCollection
                        {
                            HeaderYear = year,
                            Count = collection.Where(i => i.StartDate.Year == year).Sum(i => i.Count)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }

            if (filter.ViewTypeId == monthView)
            {
                filter.EndDate = (filter.StartDate.Value.AddMonths(monthCount).AddDays(-1).Date).AddDays(1);
                filter.StartDate = (filter.StartDate.Value);
                var monthDuration = _productReportService.MonthsBetween(filter.StartDate.Value, filter.EndDate.Value);

                var listWebLead = _webLeadDataRepository.Table.Where(x => (x.StartDate <= filter.StartDate.Value && x.EndDate >= filter.EndDate.Value)
                                   && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                   && !x.IsWeekly).ToList();

                foreach (var item in urlList)
                {

                    var collection = listWebLead.Where(x => x.Url.Equals(item));
                    var model = new WebLeadReportViewModel { };
                    model.lstHeader = new List<HeaderDataWebLeadCollection>();
                    model.URL = item;
                    foreach (var month in monthDuration)
                    {
                        var firstDayOfEnd = new DateTime(month.Item2, month.Item1, 1);
                        var daysInMonthStartDate = DateTime.DaysInMonth(month.Item2, month.Item1);
                        var startDateToCompare = new DateTime(month.Item2, month.Item1, daysInMonthStartDate).AddDays(1);
                        //firstDayOfEnd = _clock.ToUtc(firstDayOfEnd);
                        //startDateToCompare = _clock.ToUtc(startDateToCompare);
                        var headerModel = new HeaderDataWebLeadCollection
                        {
                            HeaderMonth = month.Item1,
                            HeaderYear = month.Item2,
                            Count = collection.Where(i => i.StartDate >= firstDayOfEnd.Date && i.EndDate < startDateToCompare.Date).Sum(i => i.Count)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }

            else if (filter.ViewTypeId == dayView)
            {
                filter.EndDate = filter.StartDate.Value.AddDays(dayCount).AddDays(1).AddTicks(-1);
                var dayCollection = DateRangeHelperService.GetDaysCollection(filter.StartDate.Value, filter.EndDate.Value);

                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);

                var listWebLead = _webLeadRepository.Table.Where(x => (x.CreatedDate <= filter.StartDate && x.CreatedDate >= filter.EndDate)
                                        && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

                foreach (var item in urlList)
                {
                    var collection = listWebLead.Where(x => x.URL.Equals(item));
                    var model = new WebLeadReportViewModel { };
                    model.lstHeader = new List<HeaderDataWebLeadCollection>();
                    model.URL = item;
                    foreach (var day in dayCollection.OrderBy(x => x.Day))
                    {
                        var headerModel = new HeaderDataWebLeadCollection
                        {
                            StartOfWeek = day.Date,
                            Count = collection.Count(i => i.CreatedDate.Date == day.Date)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }

            else if (filter.ViewTypeId == weekView)
            {
                filter.EndDate = filter.StartDate.Value.AddMonths(monthsForWeekCount);
                var weekCollection = DateRangeHelperService.DayOfWeekCollection(filter.EndDate.Value, filter.StartDate.Value);
                var firstDateToComapre = weekCollection.FirstOrDefault() != null ? weekCollection.FirstOrDefault().AddDays(-6) : filter.StartDate;
                var lastDateToCompare = weekCollection.LastOrDefault() != null ? weekCollection.LastOrDefault().AddDays(1).AddTicks(-1) : filter.EndDate;

                firstDateToComapre = _clock.ToUtc(firstDateToComapre.Value);
                lastDateToCompare = _clock.ToUtc(lastDateToCompare.Value);

                var listWebLead = _webLeadDataRepository.Table.Where(x => (x.StartDate >= firstDateToComapre && x.EndDate <= lastDateToCompare)
                                       && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                       && x.IsWeekly).ToList();

                foreach (var item in urlList)
                {
                    var collection = listWebLead.Where(x => item.Equals(x.Url));

                    var model = new WebLeadReportViewModel { };
                    model.lstHeader = new List<HeaderDataWebLeadCollection>();
                    model.URL = item;
                    foreach (var day in weekCollection)
                    {
                        var headerModel = new HeaderDataWebLeadCollection
                        {
                            StartOfWeek = day.Date.AddDays(-6),
                            EndOfWeek = day.Date,
                            Count = collection.Where(i => i.StartDate.Date == day.Date.AddDays(-6) && i.EndDate.Date == day.Date).Sum(i => i.Count)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }
            var queryList = list.AsQueryable();
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Url":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.URL, filter.SortingOrder);
                        return queryList;

                    case "Count":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.GrandTotal, filter.SortingOrder);
                        return queryList;
                }
            }
            return queryList.OrderByDescending(x => x.GrandTotal);
        }

        public bool DownloadWebLeadReport(MarketingLeadReportFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<WebLeadReportViewModel>();
            IEnumerable<WebLeadReportViewModel> reportList = GetWebLeadReportList(filter);

            //prepare item collection
            foreach (var item in reportList)
            {
                reportCollection.Add(item);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/webLeadReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public CallDetailReportListModel GetCallDetailReport(MarketingLeadReportFilter filter)
        {
            var callDetailList = GetCallDetailReportList(filter);

            var finalcollection = callDetailList.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

            return new CallDetailReportListModel
            {
                Collection = finalcollection,
                Filter = filter,
                PagingModel = new PagingModel(filter.PageNumber, filter.PageSize, callDetailList.Count())
            };
        }

        public CallDetailReportListModel GetCallDetailReportAdjustedData(MarketingLeadReportFilter filter)
        {

            var callDetailList = GetCallDetailReportListAdjustedData(filter);

            //var adjustedDataCollection = GetAdjustedDataTotal(filter, callDetailList);
            var finalcollection = callDetailList.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

            return new CallDetailReportListModel
            {
                Collection = finalcollection,
                Filter = filter,
                PagingModel = new PagingModel(filter.PageNumber, filter.PageSize, callDetailList.Count()),
                //AdjuctedTotal = adjustedDataCollection
            };
        }
        private CallDetailReportViewModel GetAdjustedDataTotal(MarketingLeadReportFilter filter, IQueryable<CallDetailReportViewModel> collections)
        {
            CallDetailReportViewModel list = new CallDetailReportViewModel();
            var hdTotalCollection = new List<HeaderDataCollection>();
            const int monthView = 1;
            const int weekView = 2;
            const int dayView = 3;
            const int yearView = 4;
            var model = new CallDetailReportViewModel { };

            if (filter.ViewTypeId == monthView)
            {
                var existingFirst = collections.Count() > 0 ? collections.FirstOrDefault().lstHeader : null;

                var startingDate = existingFirst.FirstOrDefault();
                var endingDate = existingFirst.LastOrDefault();

                var daysInMonth = DateTime.DaysInMonth(endingDate.HeaderYear, endingDate.HeaderMonth);
                var startDateTime = _clock.ToUtc(new DateTime(startingDate.HeaderYear, startingDate.HeaderMonth, 01));
                var endDateTime = _clock.ToUtc(new DateTime(endingDate.HeaderYear, endingDate.HeaderMonth, daysInMonth)).AddDays(1);

                if (existingFirst != null)
                {
                    foreach (var item in existingFirst)
                    {
                        var s = new HeaderDataCollection();
                        s.HeaderYear = item.HeaderYear;
                        s.HeaderMonth = item.HeaderMonth;
                        var daysInMonthForCallDetails = DateTime.DaysInMonth(s.HeaderYear, s.HeaderMonth);
                        var startDateTimeForCallDetails = _clock.ToUtc(new DateTime(s.HeaderYear, s.HeaderMonth, 01));
                        var endDateTimeForCallDetails = _clock.ToUtc(new DateTime(s.HeaderYear, s.HeaderMonth, daysInMonthForCallDetails)).AddDays(1);

                        foreach (var collection in collections)
                        {
                            s.Count += collection.lstHeader.ToList().Where(x => x.HeaderYear == s.HeaderYear && x.HeaderMonth == s.HeaderMonth).Sum(x => x.Count);
                        }
                        hdTotalCollection.Add(s);
                    }
                }
                else
                {
                    var s = new HeaderDataCollection();
                    for (int i = 0; i < 12; i++)
                    {
                        s.Count = 0;
                        hdTotalCollection.Add(s);
                    }
                }

            }
            if (filter.ViewTypeId == yearView)
            {
                var existingFirst = collections.Count() > 0 ? collections.FirstOrDefault().lstHeader : null;

                if (existingFirst != null)
                {
                    var startingDate = existingFirst.FirstOrDefault();
                    var endingDate = existingFirst.LastOrDefault();
                    var daysInMonth = DateTime.DaysInMonth(endingDate.HeaderYear, endingDate.HeaderMonth);
                    var startDateTime = _clock.ToUtc(new DateTime(startingDate.HeaderYear, startingDate.HeaderMonth, 01));
                    var endDateTime = _clock.ToUtc(new DateTime(endingDate.HeaderYear, endingDate.HeaderMonth, daysInMonth));

                    foreach (var item in existingFirst)
                    {
                        var s = new HeaderDataCollection();
                        s.HeaderYear = item.HeaderYear;

                        var daysInMonthForCallDetails = DateTime.DaysInMonth(s.HeaderYear, s.HeaderMonth);
                        var startDateTimeForCallDetails = _clock.ToUtc(new DateTime(s.HeaderYear, s.HeaderMonth, 01));
                        var endDateTimeForCallDetails = _clock.ToUtc(new DateTime(s.HeaderYear, s.HeaderMonth, daysInMonthForCallDetails)).AddDays(1);


                        foreach (var collection in collections)
                        {
                            s.Count += collection.lstHeader.ToList().Where(x => x.HeaderYear == s.HeaderYear).Sum(x => x.Count);
                        }
                        hdTotalCollection.Add(s);
                    }
                }
                else
                {
                    var s = new HeaderDataCollection();
                    for (int i = 0; i < 14; i++)
                    {
                        s.Count = 0;
                        hdTotalCollection.Add(s);
                    }
                }

            }
            if (filter.ViewTypeId == dayView)
            {
                var existingFirst = collections.Count() > 0 ? collections.FirstOrDefault().lstHeader : null;
                if (existingFirst != null)
                {
                    var startingDate = existingFirst.FirstOrDefault();
                    var endingDate = existingFirst.LastOrDefault();
                    var daysInMonth = DateTime.DaysInMonth(endingDate.HeaderYear, endingDate.HeaderMonth);
                    var startDateTime = _clock.ToUtc(new DateTime(startingDate.HeaderYear, startingDate.HeaderMonth, 01));
                    var endDateTime = _clock.ToUtc(new DateTime(endingDate.HeaderYear, endingDate.HeaderMonth, daysInMonth));

                    foreach (var item in existingFirst)
                    {
                        var s = new HeaderDataCollection();
                        s.HeaderYear = item.HeaderYear;
                        s.Start = item.Start;

                        foreach (var collection in collections)
                        {
                            s.Count += collection.lstHeader.ToList().Where(x => x.Start == s.Start).Sum(x => x.Count);
                        }
                        hdTotalCollection.Add(s);
                    }
                }
                else
                {
                    var s = new HeaderDataCollection();
                    for (int i = 0; i < 29; i++)
                    {
                        s.Count = 0;
                        hdTotalCollection.Add(s);
                    }
                }
            }
            if (filter.ViewTypeId == weekView)
            {
                var existingFirst = collections.Count() > 0 ? collections.FirstOrDefault().lstHeader : null;
                if (!collections.Any())
                    return model;

                if (existingFirst != null)
                {
                    var startingDate = existingFirst.FirstOrDefault();
                    var endingDate = existingFirst.LastOrDefault();
                    var daysInMonth = DateTime.DaysInMonth(endingDate.HeaderYear, endingDate.HeaderMonth);
                    var startDateTime = _clock.ToUtc(new DateTime(startingDate.HeaderYear, startingDate.HeaderMonth, 01));
                    var endDateTime = _clock.ToUtc(new DateTime(endingDate.HeaderYear, endingDate.HeaderMonth, daysInMonth));

                    foreach (var item in existingFirst)
                    {
                        var s = new HeaderDataCollection();
                        s.Start = item.Start;
                        s.End = item.End;
                        foreach (var collection in collections)
                        {
                            s.Count += collection.lstHeader.ToList().Where(x => x.Start <= s.Start && x.End >= s.End).Sum(x => x.Count);
                        }
                        hdTotalCollection.Add(s);
                    }
                }
            }
            model.lstHeader = (hdTotalCollection);
            model.GrandTotal = model.lstHeader.Sum(x => x.Count);
            return model;
        }
        public IQueryable<CallDetailReportViewModel> GetCallDetailReportListAdjustedData(MarketingLeadReportFilter filter)
        {
            const int monthView = 1;
            const int weekView = 2;
            const int dayView = 3;
            const int yearView = 4;
            int count = 0;
            int value = 0;
            const int monthCount = -11;
            const int dayCount = -29;
            const int monthsForWeekCount = -3;
            const int yearCount = -13;
            bool isMainSiteUsParsed = false;
            bool isMarbleLifeParsed = false;
            if (filter.StartDate == null)
                filter.StartDate = _clock.UtcNow;

            filter.StartDate = filter.StartDate.Value.Date;

            List<CallDetailReportViewModel> list = new List<CallDetailReportViewModel>();

            var categoryIds = _routingNumberRepository.Table.Where(x => (x.CategoryId.Value == (long?)(RoutingNumberCategory.BusinessDirectories))).
                                    Select(y => y.PhoneLabel).ToList();
            var phoneLabelList = _routingNumberRepository.Fetch(x => ((string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text))));
            phoneLabelList = phoneLabelList.Where(x => !x.PhoneLabel.StartsWith("CC") && !x.PhoneLabel.StartsWith("Local Bus")).ToList();

            var daysInMonth = DateTime.DaysInMonth(filter.StartDate.Value.Year, filter.StartDate.Value.Month);
            var startDateTime = _clock.ToUtc(new DateTime(filter.StartDate.Value.Year - 2, filter.StartDate.Value.Month, 01));
            var endDateTime = _clock.ToUtc(new DateTime(filter.StartDate.Value.Year, filter.StartDate.Value.Month, daysInMonth)).AddDays(1);

            var marketingLeadCallsWithoutCcAndLocalAndCorps = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDateTime && x.DateAdded <= endDateTime)
                             && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId) && (!x.PhoneLabel.StartsWith("CC") &&
                                     !x.PhoneLabel.StartsWith("CORP") && x.TransferToNumber != "")
                             && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Contains(filter.Text))).ToList();


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
                                         && !x.IsWeekly).ToList().GroupBy(x => new { x.PhoneLabel, x.StartDate.Year }).Select(x => new
                                         {
                                             x.Key.PhoneLabel,
                                             Count = x.Sum(y => y.Count),
                                             PhoneNumber = x.Select(y => y.PhoneNumber).
                                         FirstOrDefault(),
                                             StartDate = x.Select(y => y.StartDate).FirstOrDefault()
                                         }).ToList();

                foreach (var phoneLabel in phoneLabelList)
                {

                    var collection1 = listCallDetail.Where(x => (x.PhoneLabel.StartsWith(phoneLabel.PhoneLabel)));
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.PhoneNumber.Equals(phoneLabel.PhoneNumber)));
                    if (!collection.Any())
                        continue;
                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();

                    if (!collection.Any())
                        continue;

                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;
                    if (phoneLabel.PhoneLabel == "Main Site US")
                    {
                        if (!isMainSiteUsParsed)
                        {
                            isMainSiteUsParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else if (phoneLabel.PhoneLabel == "Marblelife Local - All Referral")
                    {
                        if (!isMarbleLifeParsed)
                        {
                            isMarbleLifeParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }
                    }
                    foreach (var year in yearList.OrderBy(x => x))
                    {
                        var startDateForWithoutCcAndLocalAndCorp = _clock.ToUtc(new DateTime(year, 01, 01));
                        var endDateForWithoutCcAndLocalAndCorp = _clock.ToUtc(new DateTime(year, 12, 31).AddDays(1));

                        var marketingLeadCallsWithoutCcAndLocalAndCorps2 = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= startDateForWithoutCcAndLocalAndCorp && x.DateAdded <= endDateForWithoutCcAndLocalAndCorp)
                             && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId) && (!x.PhoneLabel.StartsWith("CC") &&
                                     !x.PhoneLabel.StartsWith("CORP") && x.TransferToNumber != "")
                             && (string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Contains(filter.Text))).ToList();


                        var marketingLeadCallsWithoutCcAndLocalAndCorp = marketingLeadCallsWithoutCcAndLocalAndCorps2.Where(x => x.DateAdded >= startDateForWithoutCcAndLocalAndCorp && x.DateAdded <= endDateForWithoutCcAndLocalAndCorp).ToList();
                        var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC") && !categoryIds.Contains(x.PhoneLabel));
                        var ccCount = listCallDetail.Where(i => i.StartDate.Year == year && i.PhoneLabel.StartsWith("CC")).Sum(i => i.Count);
                        var localCount = listCallDetail.Where(i => i.StartDate.Year == year && categoryIds.Contains(i.PhoneLabel)).Sum(i => i.Count);
                        var totalCount = marketingLeadCallsWithoutCcAndLocalAndCorp.Where(i => i.DateAdded.GetValueOrDefault().Year == year).Count();
                        value = collection.Where(i => i.StartDate.Year == year).Sum(i => i.Count);
                        //webCount = listWebLead.Where(i => i.StartDate.Year == year).Sum(i => i.Count);
                        var lists = listWebLead.Where(i => i.StartDate.Year == year).ToList();
                        if (lists.Count > 0)
                        {
                            webCount = lists.Where(i => i.StartDate.Year == year).Sum(i => i.Count);
                        }
                        else
                        {
                            webCount = 0;
                        }
                        count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);

                        var headerModel = new HeaderDataCollection
                        {
                            HeaderYear = year,
                            //Count = collection.Where(i => i.StartDate.Year == year).Sum(i => i.Count)
                            Count = count
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
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
                                     && !x.IsWeekly).ToList().GroupBy(x => new { x.PhoneLabel, x.StartDate.Month }).
                                          Select(x => new
                                          {
                                              x.Key.PhoneLabel,
                                              Count = x.Sum(y => y.Count),
                                              PhoneNumber = x.Select(y => y.PhoneNumber).FirstOrDefault(),
                                              StartDate = x.OrderBy(y => y.StartDate).Select(y => y.StartDate).FirstOrDefault(),
                                              EndDate = x.OrderBy(y => y.EndDate).Select(y => y.EndDate).FirstOrDefault(),
                                              FranchiseeId = x.Where(x1 => x1.FranchiseeId != null).Select(x1 => x1.FranchiseeId).FirstOrDefault(),
                                          }).ToList();

                var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC") && !categoryIds.Contains(x.PhoneLabel));
                var listCallDetailWithoutCcAndLocalAndCorps = listCallDetailWithoutCcAndLocal.Where(x => !x.PhoneLabel.StartsWith("CORP"));
                var listCallDetailWithCcAndLocal = listCallDetail.Where(x => x.PhoneLabel.StartsWith("CC") || categoryIds.Contains(x.PhoneLabel));

                foreach (var phoneLabel in phoneLabelList)
                {

                    var collection1 = listCallDetail.Where(x => (x.PhoneLabel.StartsWith(phoneLabel.PhoneLabel)));
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.PhoneNumber.Equals(phoneLabel.PhoneNumber)));
                    if (!collection.Any())
                        continue;

                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();
                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;
                    if (phoneLabel.PhoneLabel == "Main Site US")
                    {
                        if (!isMainSiteUsParsed)
                        {
                            isMainSiteUsParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else if (phoneLabel.PhoneLabel == "Marblelife Local - All Referral")
                    {
                        if (!isMarbleLifeParsed)
                        {
                            isMarbleLifeParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }
                    }
                    foreach (var month in monthDuration)
                    {
                        daysInMonth = DateTime.DaysInMonth(month.Item2, month.Item1);
                        startDateTime = _clock.ToUtc(new DateTime(month.Item2, month.Item1, 01));
                        endDateTime = _clock.ToUtc(new DateTime(month.Item2, month.Item1, daysInMonth)).AddDays(1);

                        var marketingLeadCallsWithoutCcAndLocalAndCorp = marketingLeadCallsWithoutCcAndLocalAndCorps.Where(x => x.DateAdded >= startDateTime && x.DateAdded <= endDateTime).ToList();

                        var row = listCallDetailWithCcAndLocal.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2);
                        var ccCount = row.Where(i => i.PhoneLabel.StartsWith("CC")).Sum(i => i.Count);
                        var localCount = row.Where(i => i.PhoneLabel.StartsWith("Local Bus")).Sum(i => i.Count);
                        var totalCount = marketingLeadCallsWithoutCcAndLocalAndCorp.Count();
                        value = collection.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);
                        webCount = listWebLead.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count);

                        count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);

                        var headerModel = new HeaderDataCollection
                        {
                            HeaderMonth = month.Item1,
                            HeaderYear = month.Item2,
                            Count = count
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);

                    //if (model.GrandTotal > 0)
                    list.Add(model);
                }

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

                foreach (var phoneLabel in phoneLabelList)
                {
                    var collection1 = listCallDetail.Where(x => (x.PhoneLabel.StartsWith(phoneLabel.PhoneLabel)));
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.DialedNumber.Equals(phoneLabel.PhoneNumber)));


                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();
                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;

                    if (phoneLabel.PhoneLabel == "Main Site US")
                    {
                        if (!isMainSiteUsParsed)
                        {
                            isMainSiteUsParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else if (phoneLabel.PhoneLabel == "Marblelife Local - All Referral")
                    {
                        if (!isMarbleLifeParsed)
                        {
                            isMarbleLifeParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }
                    }

                    foreach (var day in dayCollection.OrderBy(x => x.Day))
                    {
                        var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC") && !categoryIds.Contains(x.PhoneLabel) && x.TransferToNumber != "");
                        var ccCount = listCallDetail.Count(i => i.DateAdded.GetValueOrDefault().Date == day.Date && i.PhoneLabel.StartsWith("CC"));
                        var localCount = listCallDetail.Count(i => i.DateAdded.GetValueOrDefault().Date == day.Date && categoryIds.Contains(i.PhoneLabel));
                        var totalCount = listCallDetailWithoutCcAndLocal.Count(i => i.DateAdded.GetValueOrDefault().Date == day.Date);
                        value = collection.Count(i => i.DateAdded.GetValueOrDefault().Date == day.Date);
                        var lists = listWebLead.Where(i => i.CreatedDate.Date == day.Date).ToList();
                        if (lists.Count > 0)
                        {
                            webCount = lists.Where(i => i.CreatedDate == day.Date).Count();
                        }
                        else
                        {
                            webCount = 0;
                        }
                        count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);
                        var headerModel = new HeaderDataCollection
                        {
                            Start = day,
                            //Count = collection.Count(i => i.DateAdded.Date == day.Date)
                            Count = count
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
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

                foreach (var phoneLabel in phoneLabelList)
                {
                    var collection1 = listCallDetail.Where(x => (x.PhoneLabel.StartsWith(phoneLabel.PhoneLabel)));
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.PhoneNumber.Equals(phoneLabel.PhoneNumber)));


                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();
                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;

                    if (phoneLabel.PhoneLabel == "Main Site US")
                    {
                        if (!isMainSiteUsParsed)
                        {
                            isMainSiteUsParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }

                    }
                    else if (phoneLabel.PhoneLabel == "Marblelife Local - All Referral")
                    {
                        if (!isMarbleLifeParsed)
                        {
                            isMarbleLifeParsed = true;
                            collection = collection1;
                            model.PhoneNumber = "";
                        }
                        else
                        {
                            continue;
                        }
                    }

                    foreach (var day in weekCollection)
                    {
                        DateTime dayToCompare = day.Date.AddDays(-6);
                        var startDayToCompareForCalls = _clock.ToUtc(dayToCompare);
                        var endDayToCompareForCalls = _clock.ToUtc(day.AddDays(1));
                        var marketingLeadCallsWithoutCcAndLocalAndCorpsLocal = marketingLeadCallsWithoutCcAndLocalAndCorps.Where(x => (x.DateAdded >= startDayToCompareForCalls && x.DateAdded <= endDayToCompareForCalls)
                         ).ToList();

                        value = collection.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date).Sum(i => i.Count);
                        var listCallDetailWithoutCcAndLocal = listCallDetail.Where(x => !x.PhoneLabel.StartsWith("CC") && !categoryIds.Contains(x.PhoneLabel));
                        var ccCount = listCallDetail.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date && i.PhoneLabel.StartsWith("CC")).Sum(i => i.Count);
                        var localCount = listCallDetail.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date && categoryIds.Contains(i.PhoneLabel)).Sum(i => i.Count);
                        var totalCount = marketingLeadCallsWithoutCcAndLocalAndCorpsLocal.Count();
                        var lists = listWebLead.Where(i => i.StartDate == dayToCompare && i.EndDate == day.Date).ToList();
                        if (lists.Count > 0)
                            webCount = lists.Where(i => i.StartDate == dayToCompare.Date && i.EndDate == day.Date).Sum(i => i.Count);
                        else
                            webCount = 0;
                        count = getAdjustedData(value, totalCount, ccCount, localCount, webCount);
                        var headerModel = new HeaderDataCollection
                        {
                            Start = day.Date.AddDays(-6),
                            End = day.Date,
                            //Count = collection.Where(i => i.StartDate.Date == day.Date.AddDays(-6) && i.EndDate.Date == day.Date).Sum(i => i.Count)
                            Count = count
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }

            var queryList = list.AsQueryable();

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "PhoneLabel":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.PhoneLabel, filter.SortingOrder);
                        break;

                    case "GrandTotal":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.GrandTotal, filter.SortingOrder);
                        break;
                }
                return queryList;
            }

            return queryList.OrderByDescending(x => x.GrandTotal);

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

        private IQueryable<CallDetailReportViewModel> GetCallDetailReportList(MarketingLeadReportFilter filter)
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

            List<CallDetailReportViewModel> list = new List<CallDetailReportViewModel>();
            var phoneLabelList = _routingNumberRepository.Fetch(x => ((string.IsNullOrEmpty(filter.Text) || x.PhoneLabel.Equals(filter.Text))));

            if (filter.ViewTypeId == yearView)
            {
                filter.EndDate = filter.StartDate.Value.AddYears(yearCount).Date;
                var yearList = DateRangeHelperService.GetYearsBetween(filter.StartDate.Value, filter.EndDate.Value);

                var listCallDetail = _callDetailDataRepository.Table.Where(x => (x.StartDate <= filter.StartDate && x.EndDate >= filter.EndDate)
                                            && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                            && !x.IsWeekly).ToList();

                foreach (var phoneLabel in phoneLabelList)
                {
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.PhoneNumber.Equals(phoneLabel.PhoneNumber))).ToList();

                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();

                    if (!collection.Any())
                        continue;

                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;
                    foreach (var year in yearList.OrderBy(x => x))
                    {
                        var headerModel = new HeaderDataCollection
                        {
                            HeaderYear = year,
                            Count = collection.Where(i => i.StartDate.Year == year).Sum(i => i.Count)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }

            if (filter.ViewTypeId == monthView)
            {
                filter.EndDate = filter.StartDate.Value.AddMonths(monthCount).AddDays(-1).Date;
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
                var monthDuration = _productReportService.MonthsBetween(filter.StartDate.Value, filter.EndDate.Value);

                var listCallDetail = _callDetailDataRepository.Table.Where(x => (x.StartDate <= filter.StartDate && x.EndDate >= filter.EndDate)
                                           && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                           && !x.IsWeekly).ToList();

                foreach (var phoneLabel in phoneLabelList)
                {
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.PhoneNumber.Equals(phoneLabel.PhoneNumber))).ToList();

                    if (!collection.Any())
                        continue;

                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();
                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;
                    foreach (var month in monthDuration)
                    {
                        var headerModel = new HeaderDataCollection
                        {
                            HeaderMonth = month.Item1,
                            HeaderYear = month.Item2,
                            Count = collection.Where(i => i.StartDate.Month == month.Item1 && i.StartDate.Year == month.Item2).Sum(i => i.Count)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }

            else if (filter.ViewTypeId == dayView)
            {
                filter.EndDate = filter.StartDate.Value.AddDays(dayCount).AddDays(1).AddTicks(-1);
                var dayCollection = DateRangeHelperService.GetDaysCollection(filter.StartDate.Value, filter.EndDate.Value);

                var listCallDetail = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded <= filter.StartDate && x.DateAdded >= filter.EndDate)
                                           && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();

                foreach (var phoneLabel in phoneLabelList)
                {
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.DialedNumber.Equals(phoneLabel.PhoneNumber))).ToList();

                    if (!collection.Any())
                        continue;

                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();
                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;
                    foreach (var day in dayCollection.OrderBy(x => x.Day))
                    {
                        var headerModel = new HeaderDataCollection
                        {
                            Start = day,
                            Count = collection.Count(i => i.DateAdded.GetValueOrDefault().Date == day.Date)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }

            else if (filter.ViewTypeId == weekView)
            {
                filter.EndDate = filter.StartDate.Value.AddMonths(monthsForWeekCount);
                var weekCollection = DateRangeHelperService.DayOfWeekCollection(filter.EndDate.Value, filter.StartDate.Value);
                var firstDateToCompare = weekCollection.FirstOrDefault() != null ? weekCollection.FirstOrDefault().AddDays(-6) : filter.StartDate;
                var lastDateToCompare = weekCollection.LastOrDefault() != null ? weekCollection.LastOrDefault().AddDays(1).AddTicks(-1) : filter.EndDate;

                var listCallDetail = _callDetailDataRepository.Table.Where(x => (x.StartDate >= firstDateToCompare && x.EndDate <= lastDateToCompare)
                                          && (filter.FranchiseeId <= 0 || x.FranchiseeId == filter.FranchiseeId)
                                          && x.IsWeekly).ToList();

                foreach (var phoneLabel in phoneLabelList)
                {
                    var collection = listCallDetail.Where(x => (x.PhoneLabel.Equals(phoneLabel.PhoneLabel) && x.PhoneNumber.Equals(phoneLabel.PhoneNumber))).ToList();

                    if (!collection.Any())
                        continue;

                    var model = new CallDetailReportViewModel { };
                    model.lstHeader = new List<HeaderDataCollection>();
                    model.PhoneLabel = phoneLabel.PhoneLabel;
                    model.PhoneNumber = phoneLabel.PhoneNumber;
                    foreach (var day in weekCollection)
                    {
                        var headerModel = new HeaderDataCollection
                        {
                            Start = day.Date.AddDays(-6),
                            End = day.Date,
                            Count = collection.Where(i => i.StartDate.Date == day.Date.AddDays(-6) && i.EndDate.Date == day.Date).Sum(i => i.Count)
                        };
                        model.lstHeader.Add(headerModel);
                    }
                    model.GrandTotal = model.lstHeader.Sum(x => x.Count);
                    if (model.GrandTotal > 0)
                        list.Add(model);
                }
            }
            var queryList = list.AsQueryable();
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "PhoneLabel":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.PhoneLabel, filter.SortingOrder);
                        break;

                    case "GrandTotal":
                        queryList = _sortingHelper.ApplySorting(queryList, x => x.GrandTotal, filter.SortingOrder);
                        break;
                }
            }
            return queryList.OrderByDescending(x => x.GrandTotal);
        }

        public bool DownloadCallDetailReport(MarketingLeadReportFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<CallDetailReportViewModel>();
            var lst = new List<HeaderDataCollection>();
            IEnumerable<CallDetailReportViewModel> reportList = GetCallDetailReportList(filter);

            //prepare item collection
            foreach (var item in reportList)
            {
                reportCollection.Add(item);
                foreach (var subitem in item.lstHeader)
                {
                    lst.Add(subitem);
                }

            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/callDetailReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(lst, fileName);
        }

        public IQueryable<HomeAdvisor> GetHomeAdvisordData(HomeAdvisorReportFilter filter)
        {

            var homeAdvisorList = _homeAdvisorRepository.Table.Where(x => filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId).ToList();

            return homeAdvisorList.AsQueryable();

        }


        public HomeAdvisorReportListModel GetHomeAdvisorReport(HomeAdvisorReportFilter filter)
        {
            var homeAdvisorList = GetHomeAdvisordData(filter);

            var homeAdvisorViewModel = homeAdvisorList.Select(x => _marketingleadsFactory.CreateViewModelForHomeAdvisor(x));
            var finalcollection = homeAdvisorViewModel.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);

            return new HomeAdvisorReportListModel
            {
                Collection = finalcollection,
                PagingModel = new PagingModel(filter.PageNumber, filter.PageSize, homeAdvisorViewModel.Count()),
            };
        }

        public CallDetailListModelV2 GetCallDetailListV2(CallDetailFilter filter)
        {
            var callDetailList = GetLeadFlowListV2(filter);

            var finalcollection = callDetailList.ToList().Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

            return new CallDetailListModelV2
            {
                Collection = finalcollection.Select(_marketingleadsFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(filter.PageNumber, filter.PageSize, callDetailList.Count())
            };
        }
        private List<MarketingLeadCallDetailV2> GetLeadFlowListV2(CallDetailFilter filter)
        {
            var categoryIds = new List<long>();


            if (filter.StartDate != null)
            {
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
            }
            if (filter.EndDate != null)
            {
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
            }

            var toDate = filter.EndDate.HasValue ? filter.EndDate.Value.AddTicks(-1).AddDays(1) : (DateTime?)null;

            var callDetails = _marketingLeadCallDetailV2Repository.Table.Where(x => (filter.FranchiseeId <= 1 || x.FranchiseeId == filter.FranchiseeId)
                             && (string.IsNullOrEmpty(filter.Text) || ((x.CallStatus.Contains(filter.Text))
                             || (x.CallerId.Equals(filter.Text))
                             || (x.PhoneLabel.Contains(filter.Text)) || (x.TransferNumber.Equals(filter.Text))
                             || (x.Id.ToString().Equals(filter.Text))
                              || (x.Destination.Equals(filter.Text))))
                             && (filter.StartDate == null || (x.CallDate >= filter.StartDate))
                              && (filter.MappedFranchisee == null || (filter.MappedFranchisee == 1 ? (x.FranchiseeId != null && x.Destination != null) : (x.FranchiseeId == null && x.Destination == "")))
                             && (toDate == null || (x.CallDate <= toDate))).ToList();

            callDetails = callDetails.OrderByDescending(x => x.Id).ToList();

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "ID":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.Id).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.Id).ToList();
                        }
                        break;

                    case "DialedNumber":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.SourceNumber).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.SourceNumber).ToList();
                        }
                        break;
                    case "CallerId":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.CallerId).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.CallerId).ToList();
                        }
                        break;

                    case "PhoneLabel":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.PhoneLabel).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.PhoneLabel).ToList();
                        }
                        break;

                    case "TransferTo":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.TransferNumber).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.TransferNumber).ToList();
                        }
                        break;

                    case "Date":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.CallDate).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.CallDate).ToList();
                        }
                        break;

                    case "CallDuration":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.CallDuration).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.CallDuration).ToList();
                        }
                        break;

                    case "Destination":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.Destination).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.Destination).ToList();
                        }
                        break;

                    case "CallStatus":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.CallStatus).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.CallStatus).ToList();
                        }
                        break;

                    case "CallRoute":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.CallRoute).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.CallRoute).ToList();
                        }
                        break;

                    case "ZipCode":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.OrderByDescending(x => x.EnteredZipCode).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.OrderBy(x => x.EnteredZipCode).ToList();
                        }
                        break;
                    case "Franchisee":
                        if (filter.SortingOrder == 1)
                        {
                            callDetails = callDetails.Where(x => x.Franchisee != null).OrderByDescending(x => x.Franchisee.Organization.Name).ToList();
                        }
                        else
                        {
                            callDetails = callDetails.Where(x => x.Franchisee != null).OrderBy(x => x.Franchisee.Organization.Name).ToList();
                        }
                        break;

                }
            }
            return callDetails;
        }

        public bool DownloadLeadFlow(CallDetailFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<CallDetailViewModelV2>();
            var reportList = GetLeadFlowListV2(filter).ToList();

            //prepare item collection
            foreach (var item in reportList)
            {
                var model = _marketingleadsFactory.CreateViewModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/leadFlowReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }

        public FranchiseePhoneCallListModel GetFranchiseePhoneCalls(PhoneCallFilter filter)
        {
            var franchiseeTechMailEmailData = _franchiseeTechMailEmailRepository.Table.Where(x => (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
            var franchiseeTechMailGroupedData = franchiseeTechMailEmailData.GroupBy(x => x.Franchisee).ToList();

            var thisMonth = DateTime.Now.Date.ToString("MMMM") + "," + DateTime.Now.Date.Date.ToString("yyyy");
            var franchiseePhoneCallListModel = new FranchiseePhoneCallListModel();
            var franchiseePhoneCallViewModel = new FranchiseePhoneCallViewModel();
            var phoneCallFilter = new PhoneCallFilter();
            foreach (var franchiseeTechMail in franchiseeTechMailGroupedData)
            {
                var phonechargesfeeLists = _phonechargesfeeRepository.Table.Where(x => x.FranchiseeId == franchiseeTechMail.Key.Id).ToList();
                var phonechargesfeeList = phonechargesfeeLists.ToList();


                franchiseePhoneCallViewModel = new FranchiseePhoneCallViewModel();
                franchiseePhoneCallViewModel.SalesUpdationDate = GetLastUploadedBatch(franchiseeTechMail.Key.Id);
                franchiseePhoneCallViewModel.IsActive = false;
                franchiseePhoneCallViewModel.IsEdit = true;
                franchiseePhoneCallViewModel.IsEditCall = true;
                franchiseePhoneCallViewModel.IsEditDate = false;
                franchiseePhoneCallViewModel.FranchiseeName = franchiseeTechMail.Key.Organization.Name;
                franchiseePhoneCallViewModel.FranchiseeId = franchiseeTechMail.Key.Id;
                franchiseePhoneCallViewModel.Histry = franchiseeTechMail.OrderByDescending(x => x.Id).Select(x => _marketingleadsFactory.CreatePhoneViewModel(x, phonechargesfeeList.FirstOrDefault(x1 => x1.FranchiseetechmailserviceId == x.Id))).ToList();
                franchiseePhoneCallViewModel.PhoneCallViewModel = franchiseePhoneCallViewModel.Histry.FirstOrDefault();


                franchiseePhoneCallViewModel.InvoiceLIst = phonechargesfeeLists.OrderByDescending(x => x.Id).Select(x => _marketingleadsFactory.CreatePhoneInvoiceViewModel(x)).ToList();
                franchiseePhoneCallViewModel.Histry = franchiseePhoneCallViewModel.Histry.Where(x => thisMonth == x.MonthForDataRecorder).ToList();
                franchiseePhoneCallViewModel.PhoneCallViewModel.ListOfCallIVR = default(List<AutomationClassMarketingLead>);

                if (franchiseePhoneCallViewModel.Histry.Count() > 0)
                {
                    var stareDate = franchiseePhoneCallViewModel.Histry[0].DateOfChange.GetValueOrDefault();
                    filter.StartDate = (new DateTime(stareDate.Year, stareDate.Month, 1));
                    filter.EndDate = (filter.StartDate.GetValueOrDefault().AddMonths(1));
                    phoneCallFilter = new PhoneCallFilter()
                    {
                        FranchiseeId = franchiseeTechMail.Key.Id,
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate
                    };
                    var franchiseePhoneCall = GetFranchiseePhoneCallListForFranchisee(phoneCallFilter);
                    if (franchiseePhoneCall.TotalCount == franchiseePhoneCallViewModel.PhoneCallViewModel.CallCount)
                    {
                        franchiseePhoneCallViewModel.PhoneCallViewModel.ListOfCallIVR = franchiseePhoneCall.ListOfCallIVR;

                    }
                    //franchiseePhoneCallViewModel.PhoneCallViewModel.CallCount = franchiseePhoneCall.TotalCount;
                    //franchiseePhoneCallViewModel.PhoneCallViewModel.ListOfCallIVR = franchiseePhoneCall.ListOfCallIVR;
                }

                if (franchiseePhoneCallViewModel.PhoneCallViewModel.CallCount == 0 && franchiseePhoneCallViewModel.Histry.Count() == 0)
                {
                    filter.StartDate = _clock.ToUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
                    filter.EndDate = _clock.ToUtc(filter.StartDate.GetValueOrDefault().AddMonths(1));
                    phoneCallFilter = new PhoneCallFilter()
                    {
                        FranchiseeId = franchiseeTechMail.Key.Id,
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate
                    };
                    var franchiseePhoneCall = GetFranchiseePhoneCallListForFranchisee(phoneCallFilter);
                    if (franchiseePhoneCall.CallCount > 0)
                    {
                        franchiseePhoneCallViewModel.PhoneCallViewModel.ListOfCallIVR = franchiseePhoneCall.ListOfCallIVR;

                    }
                    franchiseePhoneCallViewModel.PhoneCallViewModel.CallCount = franchiseePhoneCall.TotalCount;
                    franchiseePhoneCallViewModel.PhoneCallViewModel.ListOfCallIVR = franchiseePhoneCall.ListOfCallIVR;

                }
                franchiseePhoneCallListModel.Collection.Add(franchiseePhoneCallViewModel);
            }

            franchiseePhoneCallListModel.Collection = franchiseePhoneCallListModel.Collection.OrderBy(x => x.FranchiseeName).ToList();

            return franchiseePhoneCallListModel;
        }

        public bool SaveFranchiseePhoneCalls(PhoneCallFilter filter)
        {
            try
            {
                if (filter.Id == 0)
                {
                    var franchiseetechmailemail = new FranchiseeTechMailEmail()
                    {
                        DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                        UserId = filter.UserId,
                        ChargesForPhone = filter.PhoneCost,
                        FranchiseeId = filter.FranchiseeId.Value,
                        IsNew = true,
                        CallCount = filter.PhoneCost.Value,
                        DateForCharges = _clock.ToUtc(DateTime.Now.Date)
                    };
                    _franchiseeTechMailEmailRepository.Save(franchiseetechmailemail);
                    return true;
                }
                else
                {
                    var franchiseeTechMailEmail = _franchiseeTechMailEmailRepository.Get(filter.Id.GetValueOrDefault());
                    franchiseeTechMailEmail.ChargesForPhone = filter.PhoneCost;
                    franchiseeTechMailEmail.UserId = filter.UserId;
                    franchiseeTechMailEmail.CallCount = filter.PhoneCost.Value;
                    _franchiseeTechMailEmailRepository.Save(franchiseeTechMailEmail);
                    return true;
                }
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public bool EditFranchiseePhoneCalls(PhoneCallEditModel filter)
        {
            try
            {
                var franchiseePhoneCalls = new FranchiseeTechMailEmail()
                {
                    CallCount = filter.CallCount,
                    ChargesForPhone = filter.ChargesForPhone,
                    UserId = filter.UserId,
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                    FranchiseeId = filter.FranchiseeId,
                    IsNew = true,
                    DateForCharges = filter.DateOfChange,
                };
                _franchiseeTechMailEmailRepository.Save(franchiseePhoneCalls);
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public bool GeneratePhoneCallInvoice(PhoneCallInvoiceEditModel filter)
        {
            var franchiseetechmailservice = _franchiseeTechMailEmailRepository.IncludeMultiple(x => x.Franchisee).FirstOrDefault(x => x.Id == filter.Id);

            var CurrencyExchangeRate = GetCurrencyExchangeRate(franchiseetechmailservice.Franchisee, _clock.ToUtc(DateTime.Now));

            if (franchiseetechmailservice != null)
            {
                var phoneChargeChargesList = _phonechargesfeeRepository.Table.OrderByDescending(x => x.Id).Where(x => x.FranchiseeId == franchiseetechmailservice.FranchiseeId && !x.IsInvoiceGenerated &&
                                                                       x.IsInvoiceInQueue).ToList();

                if (phoneChargeChargesList.Count() == 0)
                {
                    var phoneChargeCharges = new Phonechargesfee()
                    {
                        FranchiseeId = franchiseetechmailservice.FranchiseeId.GetValueOrDefault(),
                        IsInvoiceGenerated = false,
                        Amount = (decimal)(franchiseetechmailservice.ChargesForPhone * franchiseetechmailservice.CallCount),
                        FranchiseetechmailserviceId = franchiseetechmailservice.Id,
                        DateCreated = _clock.ToUtc(DateTime.Now).Date,
                        CurrencyExchangeRateId = CurrencyExchangeRate != null ? CurrencyExchangeRate.Id : default(long?),
                        IsNew = true,
                        IsInvoiceInQueue = true,
                    };

                    _phonechargesfeeRepository.Save(phoneChargeCharges);
                    var franchiseeSeriveId = CreateFranchiseeServiceFee(phoneChargeCharges);
                    phoneChargeCharges.FranchiseeservicefeeId = franchiseeSeriveId;

                    phoneChargeCharges.IsNew = false;
                    _phonechargesfeeRepository.Save(phoneChargeCharges);
                }
                else
                {
                    var phoneChargeChargesDomain = phoneChargeChargesList.FirstOrDefault();
                    phoneChargeChargesDomain.Amount = (franchiseetechmailservice.ChargesForPhone != null && franchiseetechmailservice.CallCount != null) ? (decimal)(franchiseetechmailservice.ChargesForPhone * franchiseetechmailservice.CallCount) : 0;

                    var franchiseeSeriveId = CreateFranchiseeServiceFee(phoneChargeChargesDomain);
                    phoneChargeChargesDomain.FranchiseeservicefeeId = franchiseeSeriveId;
                    phoneChargeChargesDomain.FranchiseetechmailserviceId = franchiseetechmailservice.Id;
                    phoneChargeChargesDomain.IsNew = false;
                    phoneChargeChargesDomain.IsInvoiceInQueue = true;
                    _phonechargesfeeRepository.Save(phoneChargeChargesDomain);
                    //CreateFranchiseeServiceFee(phoneChargeChargesDomain);
                }



            }
            else
            {
                return false;
            }
            return true;
        }

        private CurrencyExchangeRate GetCurrencyExchangeRate(Franchisee franchisee, DateTime currentdate)
        {
            long countryId = franchisee.Organization.Address != null ? franchisee.Organization.Address.First().CountryId : 0;
            var currencyExchangeRate = new CurrencyExchangeRate();
            if (countryId > 0)
            {
                currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime.Year == currentdate.Year && x.DateTime.Month == currentdate.Month
                                        && x.DateTime.Day == currentdate.Day).OrderByDescending(y => y.DateTime).FirstOrDefault();

                if (currencyExchangeRate == null)
                    currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(y => y.DateTime).First();
                return currencyExchangeRate;
            }
            else
            {
                return currencyExchangeRate;
            }
        }


        private long? CreateFranchiseeServiceFee(Phonechargesfee model)
        {

            if (model.FranchiseeservicefeeId == null)
            {
                var domain = _franchiseeServiceFeeFactory.CreateDomainForServiceFeeForCalls(model);
                domain.Id = 0;
                domain.IsNew = true;
                _franchiseeServiceFeeRepository.Save(domain);
                return domain.Id;
            }
            else
            {
                var domain = _franchiseeServiceFeeFactory.CreateDomainForServiceFeeForCalls(model);
                domain.Id = model.FranchiseeservicefeeId.GetValueOrDefault();
                domain.IsNew = false;
                _franchiseeServiceFeeRepository.Save(domain);
                return domain.Id;

            }
            return default(long?);
        }

        public bool EditFranchiseePhoneCallsByBulk(PhoneCallEditByBulkModel filter)
        {
            try
            {
                foreach (var id in filter.PhoneIdList)
                {

                    var franchiseePhoneCalls = _franchiseeTechMailEmailRepository.Table.FirstOrDefault(x => x.Id == id);

                    franchiseePhoneCalls.IsNew = true;
                    franchiseePhoneCalls.CallCount = filter.CallCount;
                    franchiseePhoneCalls.ChargesForPhone = filter.ChargesForPhone;
                    franchiseePhoneCalls.DateForCharges = filter.DateOfChange;
                    _franchiseeTechMailEmailRepository.Save(franchiseePhoneCalls);
                    if (filter.IsGenerateInvoice.GetValueOrDefault())
                    {
                        var phoneCallInvoiceEditModel = new PhoneCallInvoiceEditModel()
                        {
                            UserId = filter.UserId,
                            Id = franchiseePhoneCalls.Id
                        };
                        GeneratePhoneCallInvoice(phoneCallInvoiceEditModel);
                    }
                }
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public FranchiseePhoneCallBulkListModel GetFranchiseePhoneCallsBulkList(PhoneCallFilter filter)
        {
            if (filter.Date != null)
            {
                filter.FranchiseeId = 0;
                filter.StartDate = new DateTime(filter.Date.GetValueOrDefault().Year, filter.Date.GetValueOrDefault().Month, 1);
                filter.EndDate = new DateTime(filter.Date.GetValueOrDefault().Year, filter.Date.GetValueOrDefault().Month, DateTime.DaysInMonth(filter.Date.GetValueOrDefault().Year, filter.Date.GetValueOrDefault().Month)).AddDays(1);
            }
            var phonechargesfeeList = _phonechargesfeeRepository.Table.Where(x => (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
            var franchiseeTechMailEmailData = _franchiseeTechMailEmailRepository.Table.Where(x => (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId)
            && (filter.Date == null || (x.DateForCharges >= filter.StartDate && x.DateForCharges <= filter.EndDate))).OrderByDescending(x => x.Id).ToList();


            var franchiseePhoneCallListModel = new FranchiseePhoneCallBulkListModel();


            franchiseePhoneCallListModel.Collection = franchiseeTechMailEmailData.OrderByDescending(x => x.Id).Select(x => _marketingleadsFactory.CreatePhoneViewModel(x, phonechargesfeeList.FirstOrDefault(x1 => x1.FranchiseetechmailserviceId == x.Id))).ToList();

            if (filter.Date != null)
            {
                franchiseePhoneCallListModel.Collection = franchiseePhoneCallListModel.Collection.ToList().OrderByDescending(x => x.FranchiseeName).ToList();
            }
            else if (filter.FranchiseeId != null)
            {
                franchiseePhoneCallListModel.Collection = franchiseePhoneCallListModel.Collection.ToList().OrderByDescending(x => x.CurrentDate).ToList();
            }
            if (filter.PropName == "MonthYear")
            {
                if (filter.Order == 0)
                {
                    franchiseePhoneCallListModel.Collection = franchiseePhoneCallListModel.Collection.ToList().OrderBy(x => x.Month).ToList();
                }
                else
                {
                    franchiseePhoneCallListModel.Collection = franchiseePhoneCallListModel.Collection.ToList().OrderByDescending(x => x.Month).ToList();
                }
            }
            else if (filter.PropName == "FranchiseeName")
            {
                if (filter.Order == 0)
                {
                    franchiseePhoneCallListModel.Collection = franchiseePhoneCallListModel.Collection.ToList().OrderBy(x => x.FranchiseeName).ToList();
                }
                else
                {
                    franchiseePhoneCallListModel.Collection = franchiseePhoneCallListModel.Collection.ToList().OrderByDescending(x => x.FranchiseeName).ToList();
                }
            }
            return franchiseePhoneCallListModel;
        }

        public bool SaveFranchiseePhoneCallsByBulk(PhoneCallEditByBulkList model)
        {
            DateTime today = _clock.ToUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1));
            DateTime endOfMonth = _clock.ToUtc(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))).AddDays(1);

            var franchiseePhoneCallsDomainList = _franchiseeTechMailEmailRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId).ToList();
            var franchiseePhoneCallsList = franchiseePhoneCallsDomainList;


            franchiseePhoneCallsList = franchiseePhoneCallsList.Where(x => x.DataRecorderMetaData.DateModified != null ?
             (x.DataRecorderMetaData.DateModified >= today && x.DataRecorderMetaData.DateModified < endOfMonth) : (x.DataRecorderMetaData.DateCreated >= today && x.DataRecorderMetaData.DateCreated < endOfMonth)).ToList();

            var franchiseePhoneCallsIdListInDb = franchiseePhoneCallsList.Select(x => x.Id).ToList();
            var franchiseePhoneCallsIdListFromModel = model.Collection.Select(x => x.Id).Where(x => x != 0).ToList();

            foreach (var franchiseePhoneCall in model.Collection)
            {
                if (franchiseePhoneCall.Id == 0)
                {

                    var franchiseePhoneCalls = new FranchiseeTechMailEmail()
                    {
                        DateForCharges = franchiseePhoneCall.DateOfChange,
                        FranchiseeId = franchiseePhoneCall.FranchiseeId,
                        UserId = model.UserId,
                        ChargesForPhone = franchiseePhoneCall.ChargesForPhone,
                        IsNew = true,
                        DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                        CallCount = franchiseePhoneCall.CallCount
                    };
                    _franchiseeTechMailEmailRepository.Save(franchiseePhoneCalls);
                    _unitOfWork.SaveChanges();

                    if (franchiseePhoneCall.IsInvoiceGenerated.GetValueOrDefault())
                    {
                        var phoneCallInvoiceEditModel = new PhoneCallInvoiceEditModel()
                        {
                            UserId = model.UserId,
                            Id = franchiseePhoneCall.Id != 0 ? franchiseePhoneCall.Id : franchiseePhoneCalls.Id
                        };
                        GeneratePhoneCallInvoiceForMultiple(phoneCallInvoiceEditModel);
                    }
                }
                else
                {
                    var franchiseePhoneCallsDomain = _franchiseeTechMailEmailRepository.Get(franchiseePhoneCall.Id);

                    franchiseePhoneCallsDomain.DateForCharges = franchiseePhoneCall.DateOfChange;
                    franchiseePhoneCallsDomain.FranchiseeId = franchiseePhoneCall.FranchiseeId;
                    franchiseePhoneCallsDomain.UserId = model.UserId;
                    franchiseePhoneCallsDomain.ChargesForPhone = franchiseePhoneCall.ChargesForPhone;
                    franchiseePhoneCallsDomain.CallCount = franchiseePhoneCall.CallCount;
                    _franchiseeTechMailEmailRepository.Save(franchiseePhoneCallsDomain);
                    //_unitOfWork.SaveChanges();

                    if (franchiseePhoneCall.IsInvoiceGenerated.GetValueOrDefault())
                    {

                        var phoneCallInvoiceEditModel = new PhoneCallInvoiceEditModel()
                        {
                            UserId = model.UserId,
                            Id = franchiseePhoneCall.Id
                        };
                        GeneratePhoneCallInvoiceForMultiple(phoneCallInvoiceEditModel);
                    }
                }
            }


            var differenceInCallsIdList = franchiseePhoneCallsIdListInDb.Except(franchiseePhoneCallsIdListFromModel);

            foreach (var differenceInCallsId in differenceInCallsIdList)
            {
                //_unitOfWork.StartTransaction();
                var franchiseePhoneCallsDomain = _franchiseeTechMailEmailRepository.Get(x => x.Id == differenceInCallsId);
                franchiseePhoneCallsDomain.IsDeleted = true;
                _franchiseeTechMailEmailRepository.Delete(franchiseePhoneCallsDomain);
                var phoneChargesDomain = _phonechargesfeeRepository.Table.FirstOrDefault(x => x.FranchiseetechmailserviceId == differenceInCallsId);
                if (phoneChargesDomain != null)
                {

                    if (phoneChargesDomain.FranchiseeservicefeeId != null)
                    {
                        phoneChargesDomain.IsDeleted = true;
                    }
                    phoneChargesDomain.IsDeleted = true;
                    _phonechargesfeeRepository.Save(phoneChargesDomain);
                }
                _unitOfWork.SaveChanges();
            }
            return true;

        }


        private string GetLastUploadedBatch(long franchiseeId)
        {
            var salesData = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.StatusId == (long)SalesDataUploadStatus.Parsed && x.IsActive)
                .OrderByDescending(x => x.PeriodEndDate).FirstOrDefault();
            if (salesData == null)
                return null;
            var uploadStartdate = salesData.PeriodEndDate.AddDays(1);

            if (salesData.Franchisee.FeeProfile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
            {
                var day = DateTime.Now.DayOfWeek;
                var start = DateTime.Now.Date;
                int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;

                if (day == DayOfWeek.Monday)
                    return DateTime.Now.ToShortDateString();
                else if (day == DayOfWeek.Tuesday)
                    return DateTime.Now.AddDays(6).ToShortDateString();
                else if (day == DayOfWeek.Wednesday)
                    return DateTime.Now.AddDays(5).ToShortDateString();
                else if (day == DayOfWeek.Thursday)
                    return DateTime.Now.AddDays(4).ToShortDateString();
                else if (day == DayOfWeek.Friday)
                    return DateTime.Now.AddDays(3).ToShortDateString();
                else if (day == DayOfWeek.Saturday)
                    return DateTime.Now.AddDays(2).ToShortDateString();
                else
                    return DateTime.Now.AddDays(1).ToShortDateString();
            }
            else
            {

                if (uploadStartdate.Day == 1 && uploadStartdate.Month == DateTime.Now.Date.Month && uploadStartdate.Year == DateTime.Now.Date.Year)
                {
                    var monthlyStartdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);
                    return monthlyStartdate.ToShortDateString();
                }
                else
                {
                    var monthlyStartdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    return monthlyStartdate.ToShortDateString();
                }
            }
        }
        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public FranchiseePhoneCallBulkListModel GetFranchiseePhoneCallList(PhoneCallFilter filter)
        {

            var franchiseeTechMailEmailData = _franchiseeTechMailEmailRepository.Table.Where(x => (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId)).ToList();
            var franchiseeTechMailGroupedData = franchiseeTechMailEmailData.GroupBy(x => x.Franchisee).ToList();

            var thisMonth = DateTime.Now.Date.ToString("MMMM") + "," + DateTime.Now.Date.Date.ToString("yyyy");
            if (filter.StartDate != null)
            {
                thisMonth = filter.StartDate.GetValueOrDefault().Date.ToString("MMMM") + "," + filter.StartDate.GetValueOrDefault().Date.ToString("yyyy");
            }

            var franchiseePhoneCallListModel = new FranchiseePhoneCallListModel();

            var franchiseePhoneCallViewModel = new FranchiseePhoneCallViewModel();
            franchiseeTechMailGroupedData = franchiseeTechMailGroupedData.Where(x => x.Key.Id == filter.FranchiseeId).ToList();
            foreach (var franchiseeTechMail in franchiseeTechMailGroupedData)
            {
                var phonechargesfeeLists = _phonechargesfeeRepository.Table.Where(x => x.FranchiseeId == franchiseeTechMail.Key.Id).ToList();
                var phonechargesfeeList = phonechargesfeeLists.Where(x => !x.IsInvoiceGenerated).ToList();


                franchiseePhoneCallViewModel = new FranchiseePhoneCallViewModel();
                franchiseePhoneCallViewModel.SalesUpdationDate = GetLastUploadedBatch(franchiseeTechMail.Key.Id);
                franchiseePhoneCallViewModel.IsActive = false;
                franchiseePhoneCallViewModel.IsEdit = true;
                franchiseePhoneCallViewModel.IsEditCall = true;
                franchiseePhoneCallViewModel.IsEditDate = false;
                franchiseePhoneCallViewModel.FranchiseeName = franchiseeTechMail.Key.Organization.Name;
                franchiseePhoneCallViewModel.FranchiseeId = franchiseeTechMail.Key.Id;
                franchiseePhoneCallViewModel.Histry = franchiseeTechMail.OrderByDescending(x => x.Id).Select(x => _marketingleadsFactory.CreatePhoneViewModel(x, phonechargesfeeList.FirstOrDefault(x1 => x1.FranchiseetechmailserviceId == x.Id))).ToList();
                franchiseePhoneCallViewModel.PhoneCallViewModel = franchiseePhoneCallViewModel.Histry.FirstOrDefault();
                franchiseePhoneCallViewModel.InvoiceLIst = phonechargesfeeLists.OrderByDescending(x => x.Id).Select(x => _marketingleadsFactory.CreatePhoneInvoiceViewModel(x)).ToList();
                franchiseePhoneCallViewModel.Histry = franchiseePhoneCallViewModel.Histry.Where(x => thisMonth == x.Month).ToList();
                franchiseePhoneCallListModel.Collection.Add(franchiseePhoneCallViewModel);
            }
            return default(FranchiseePhoneCallBulkListModel);
        }

        private bool GeneratePhoneCallInvoiceForMultiple(PhoneCallInvoiceEditModel filter)
        {
            var franchiseetechmailservice = _franchiseeTechMailEmailRepository.IncludeMultiple(x => x.Franchisee).FirstOrDefault(x => x.Id == filter.Id);

            var CurrencyExchangeRate = GetCurrencyExchangeRate(franchiseetechmailservice.Franchisee, _clock.ToUtc(DateTime.Now));

            if (franchiseetechmailservice != null)
            {
                var phoneChargeChargesList = _phonechargesfeeRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.FranchiseetechmailserviceId == franchiseetechmailservice.Id);

                if (phoneChargeChargesList == null)
                {
                    var phoneChargeCharges = new Phonechargesfee()
                    {
                        FranchiseeId = franchiseetechmailservice.FranchiseeId.GetValueOrDefault(),
                        IsInvoiceGenerated = false,
                        Amount = (decimal)(franchiseetechmailservice.ChargesForPhone * franchiseetechmailservice.CallCount),
                        FranchiseetechmailserviceId = franchiseetechmailservice.Id,
                        DateCreated = _clock.ToUtc(DateTime.Now).Date,
                        CurrencyExchangeRateId = CurrencyExchangeRate != null ? CurrencyExchangeRate.Id : default(long?),
                        IsNew = true,
                        IsInvoiceInQueue = true,
                    };

                    _phonechargesfeeRepository.Save(phoneChargeCharges);
                    var franchiseeSeriveId = CreateFranchiseeServiceFee(phoneChargeCharges);
                    phoneChargeCharges.FranchiseeservicefeeId = franchiseeSeriveId;

                    phoneChargeCharges.IsNew = false;
                    _phonechargesfeeRepository.Save(phoneChargeCharges);
                }
                else
                {
                    var phoneChargeChargesDomain = phoneChargeChargesList;
                    phoneChargeChargesDomain.Amount = (franchiseetechmailservice.ChargesForPhone != null && franchiseetechmailservice.CallCount != null) ? (decimal)(franchiseetechmailservice.ChargesForPhone * franchiseetechmailservice.CallCount) : 0;

                    var franchiseeSeriveId = CreateFranchiseeServiceFee(phoneChargeChargesDomain);
                    phoneChargeChargesDomain.FranchiseeservicefeeId = franchiseeSeriveId;
                    phoneChargeChargesDomain.FranchiseetechmailserviceId = franchiseetechmailservice.Id;
                    phoneChargeChargesDomain.IsNew = false;
                    phoneChargeChargesDomain.IsInvoiceInQueue = true;
                    _phonechargesfeeRepository.Save(phoneChargeChargesDomain);
                    //CreateFranchiseeServiceFee(phoneChargeChargesDomain);
                }

            }
            else
            {
                return false;
            }
            return true;
        }

        public AutomationBackUpCallListModel GetAutomationBackUpReport(AutomationBackUpFilter filter, long userId, long roleUserId)
        {
            if (filter.StartDate != null)
            {
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value.AddDays(2));
            }
            var marketingLeadClassList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= filter.StartDate)
                                                             && x.DateAdded <= filter.EndDate).ToList();

            var marketingLeadClass3List = _marketingLeadCallDetail3Repository.Table.Where(x => (x.MarketingLeadCallDetail.DateAdded >= filter.StartDate)
                                                               && x.MarketingLeadCallDetail.DateAdded <= filter.EndDate).ToList();




            var marketingLeadClass3ListNoZipCode = marketingLeadClass3List.Where(x => x.CallflowEnteredZip == null).ToList();
            var franchiseeList = new List<Franchisee>();
            var assignedFranchiseeIdList = new List<long>();
            if (roleUserId == (long)RoleType.SuperAdmin)
            {
                franchiseeList = _franchiseeRepository.Table.Where(x => x.Organization.IsActive && (filter.FranchiseeId == 0 || x.Id == filter.FranchiseeId)).ToList();
            }
            if (roleUserId == (long)RoleType.FranchiseeAdmin)
            {
                assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId).Select(x => x.OrganizationId).ToList();
                franchiseeList = _franchiseeRepository.Table.Where(x => x.Organization.IsActive && assignedFranchiseeIdList.Contains(x.Organization.Id)).ToList();

            }
            var organizationAddress = franchiseeList.Select(x => x.Organization.Address).ToList();

            var addresses = (from sublist in organizationAddress
                             from Zip in sublist
                             select Zip).ToList();
            var zipCodeList = new List<string>();
            zipCodeList.AddRange(addresses.Where(x => x.Zip != null).Select(x => x.Zip.Code));
            zipCodeList.AddRange(addresses.Select(x => x.ZipCode));

            marketingLeadClass3List = marketingLeadClass3List.Where(x => !x.MarketingLeadCallDetail.PhoneLabel.StartsWith("Local Bus")).ToList();

            var marketingLeadClass3ListNoMatch = GetMarketClassNoMatch(marketingLeadClass3List, zipCodeList);

            var marketingLeadClass3ListWithTransferToNumber = marketingLeadClass3ListNoMatch.Where(x => x.TransferToNumber != null).ToList();
            marketingLeadClass3ListNoMatch = marketingLeadClass3ListNoMatch.Where(x => x.Route == "NO BUYER" || x.Route == "ANSWER").ToList();
            marketingLeadClass3ListNoMatch = marketingLeadClass3ListNoMatch.GroupBy(x => x.Id).Select(x => x.FirstOrDefault())
                                                                         .ToList();
            var totalCount = marketingLeadClass3ListNoMatch.Count();
            marketingLeadClass3ListNoMatch = marketingLeadClass3ListNoMatch.Take(300).ToList();
            var automationBackUpCallViewListModel = new List<AutomationBackUpCallViewModel>();
            var automationBackUpCallViewModel = new AutomationBackUpCallViewModel();
            var automationBackUpCallModel = new AutomationBackUpCallModel();
            var automationBackUpCallListModel = new List<AutomationBackUpCallModel>();
            var personTeresaList = _personRepository.Table.Where(x => x.FirstName == "Teresa" && x.LastName == "Smith").ToList();
            var personKimList = _personRepository.Table.Where(x => x.FirstName == "Rochelle " && x.MiddleName == "K" && x.LastName == "VanderMeir").ToList();
            var transferToNumerForKim = new List<string>();
            var transferToNumerForTeresa = new List<string>();

            foreach (var phoneNumber in personTeresaList)
            {
                transferToNumerForTeresa.AddRange(phoneNumber.Phones.Select(x => x.Number));
                transferToNumerForTeresa = transferToNumerForTeresa.Distinct().ToList();
            }

            foreach (var phoneNumber in personKimList)
            {
                transferToNumerForKim.AddRange(phoneNumber.Phones.Select(x => x.Number));
                transferToNumerForKim = transferToNumerForKim.Distinct().ToList();
                transferToNumerForKim.Add("3029568217");
            }

            foreach (var franchisee in franchiseeList)
            {
                //automationBackUpCallModel.ListOfNumberOfIVRCalls = new List<AutomationClassMarketingLead>();
                automationBackUpCallModel = new AutomationBackUpCallModel();
                automationBackUpCallListModel = new List<AutomationBackUpCallModel>();


                //var marketingLeadClassListFranchiseeWise = marketingLeadClassList.Where(x => x.FranchiseeId ==
                //                                                            franchisee.Organization.Id || x.CalledFranchiseeId == franchisee.Organization.Id).ToList();

                var marketingLeadClassListFranchiseeWise = marketingLeadClassList.Where(x => (x.CalledFranchiseeId == null && x.FranchiseeId == franchisee.Organization.Id) || (x.CalledFranchiseeId != null && x.CalledFranchiseeId == franchisee.Organization.Id)).ToList();

                if (franchisee.Organization.Id == 66 || franchisee.Organization.Id == 89)
                {
                    marketingLeadClassListFranchiseeWise = marketingLeadClassListFranchiseeWise.Where(x => x.CalledFranchiseeId == null).ToList();
                }

                var marketingLeadClassListFranchiseeWiseIds = marketingLeadClassListFranchiseeWise.Select(x => x.Id).ToList();

                var marketingLeadClassList3FranchiseeWise = _marketingLeadCallDetail3Repository.Table.Where(x => marketingLeadClassListFranchiseeWiseIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
                var marketingLeadClassList3TransFerToTeresaNumber = marketingLeadClassListFranchiseeWise.Where(x => transferToNumerForTeresa.Contains(x.TransferToNumber)).ToList();

                var marketingLeadClassList3TransFerToKimNumber = marketingLeadClassListFranchiseeWise.Where(x => transferToNumerForKim.Contains(x.TransferToNumber)).ToList();


                automationBackUpCallViewModel = new AutomationBackUpCallViewModel();
                automationBackUpCallViewModel.FranchiseeName = franchisee.Organization.Name;
                automationBackUpCallViewModel.FranchiseeId = franchisee.Organization.Id;


                if (marketingLeadClassListFranchiseeWise.Count() == 0)
                {
                    automationBackUpCallModel.CallsToKim = 0;
                    automationBackUpCallModel.CallsToTeresa = 0;
                    automationBackUpCallModel.IVRCalls = 0;
                    automationBackUpCallModel.NumberOfNoCalls = 0;
                }
                else
                {
                    if (marketingLeadClassList3FranchiseeWise.Count() > 0)
                    {

                        automationBackUpCallModel.CallsToTeresa = marketingLeadClassList3TransFerToTeresaNumber.Count();
                        automationBackUpCallModel.ListOfCallToTeresa = marketingLeadClassList3TransFerToTeresaNumber.Select(x => _franchiseeServiceFeeFactory.CreateViewModel(marketingLeadClassList3FranchiseeWise.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), x)).ToList();

                        automationBackUpCallModel.ListOfCallToTeresa = automationBackUpCallModel.ListOfCallToTeresa.GroupBy(x => x.Id).Select(x => x.FirstOrDefault())
                                                                         .ToList();

                        automationBackUpCallModel.CallsToKim = marketingLeadClassList3TransFerToKimNumber.Count();
                        automationBackUpCallModel.ListOfCallToKim = marketingLeadClassList3TransFerToKimNumber.Select(x => _franchiseeServiceFeeFactory.CreateViewModel(marketingLeadClassList3FranchiseeWise.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), x)).ToList();

                        automationBackUpCallModel.ListOfCallToKim = automationBackUpCallModel.ListOfCallToKim.GroupBy(x => x.Id).Select(x => x.FirstOrDefault())
                                                                         .ToList();
                        automationBackUpCallModel.IsExpand = false;
                        automationBackUpCallModel.IsIVRCallClick = false;
                        automationBackUpCallModel.IsKimCallClick = false;
                        automationBackUpCallModel.IsNoCallClick = false;
                        automationBackUpCallModel.IsTeresaCallClick = false;

                        var iVRCalls = automationBackUpCallModel.CallsToTeresa + automationBackUpCallModel.CallsToKim;
                        automationBackUpCallModel.IVRCalls = iVRCalls;
                        automationBackUpCallModel.ListOfNumberOfIVRCalls.AddRange(automationBackUpCallModel.ListOfCallToTeresa);
                        automationBackUpCallModel.ListOfNumberOfIVRCalls.AddRange(automationBackUpCallModel.ListOfCallToKim);

                    }

                    else
                    {
                        continue;
                    }
                }


                //automationBackUpCallListModel.Add(automationBackUpCallModel);
                automationBackUpCallViewModel.AutomationBackUpCallModel = automationBackUpCallModel;
                automationBackUpCallViewListModel.Add(automationBackUpCallViewModel);
            }

            return new AutomationBackUpCallListModel()
            {
                Collection = automationBackUpCallViewListModel,
                NoCallMatch = marketingLeadClass3ListNoMatch,
                TotalCount = totalCount
            };
        }

        public bool DownloadMarketingLeadsWithNewRows(CallDetailFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<CallDetailViewModel>();
            IEnumerable<MarketingLeadCallDetail> reportList = GetCallDetailList(filter).ToList();
            var marketingClassIds = reportList.Select(x => x.Id).ToList();

            var marketingClassAPI2 = _marketingLeadCallDetail2Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var marketingClassAPI3 = _marketingLeadCallDetail3Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var marketingClassAPI4 = _marketingLeadCallDetail4Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var marketingClassAPI5 = _marketingLeadCallDetail5Repository.Table.Where(x => marketingClassIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
            var callNotes = _callDetailsReportNotesRepository.Table.ToList();
            //prepare item collection
            var model = reportList.Select(x => _marketingleadsFactory.CreateNewViewModel(x, marketingClassAPI3.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), marketingClassAPI4.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), marketingClassAPI5.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), marketingClassAPI2.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), callNotes.Where(y => x.CallerId == y.CallerId).ToList())).ToList();
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/marketingLead-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreatorMarketingLead.CreateExcelDocument(model, fileName, filter.DownloadColumnList);
        }

        private List<AutomationClassMarketingLead> GetMarketClassNoMatch(List<MarketingLeadCallDetailV3> marketingLeadCallDetailV3, List<string> zipCodeList)
        {
            var marketingLeadCallDetailV3WithoutZipCode = marketingLeadCallDetailV3.Where(x => x.CallflowEnteredZip == null).ToList();

            var marketingLeadCallDetailV3MoMatch = marketingLeadCallDetailV3.Where(x => x.CallflowDestination == null).ToList();

            var marketingLeadCallDetailV3MoMatch2 = marketingLeadCallDetailV3.Where(x => x.CallflowEnteredZip != null
                                                                             && x.CallflowDestination == null && !zipCodeList.Contains(x.CallflowEnteredZip)).ToList();

            marketingLeadCallDetailV3MoMatch.AddRange(marketingLeadCallDetailV3MoMatch2);
            var marketingLeadCallDetailV3WithoutDestination = marketingLeadCallDetailV3MoMatch.Select(x => _franchiseeServiceFeeFactory.CreateViewModel(x, x.MarketingLeadCallDetail)).ToList();

            marketingLeadCallDetailV3WithoutDestination = marketingLeadCallDetailV3WithoutDestination.OrderByDescending(x => x.ZipCode).ToList();


            return marketingLeadCallDetailV3WithoutDestination.OrderBy(x => x.Id).ToList();
        }
        private List<AutomationClassMarketingLead> GetMarketClassNoMatchForFranchisee(List<MarketingLeadCallDetailV3> marketingLeadCallDetailV3, List<string> zipCodeList)
        {
            var marketingLeadCallDetailV3WithoutZipCode = marketingLeadCallDetailV3.Where(x => x.CallflowEnteredZip == null).ToList();
            var marketingLeadCallDetailV3MoMatch = marketingLeadCallDetailV3WithoutZipCode.Where(x => x.CallflowSourceRoute == null && x.CallflowDestination == null).ToList();
            var marketingLeadCallDetailV3WithoutDestination = marketingLeadCallDetailV3MoMatch.Select(x => _franchiseeServiceFeeFactory.CreateViewModel(x, x.MarketingLeadCallDetail)).ToList();

            return marketingLeadCallDetailV3WithoutDestination;
        }

        public AutomationBackUpCallFranchiseeModel GetFranchiseePhoneCallListForFranchisee(PhoneCallFilter filter)
        {
            var automationBackUpCallFranchiseeModel = new AutomationBackUpCallFranchiseeModel();
            if (filter.StartDate != null)
            {
                filter.EndDate = _clock.ToUtc(new DateTime(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month, DateTime.DaysInMonth(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month)).AddDays(2));
                filter.StartDate = _clock.ToUtc(new DateTime(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month, 1));
            }

            var franchiseeTechMailEmailData = _franchiseeTechMailEmailRepository.Table.Where(x => (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId)
            && (x.DateForCharges >= filter.StartDate && x.DateForCharges <= filter.EndDate)).OrderByDescending(x => x.Id).ToList();


            if (franchiseeTechMailEmailData.Count() > 0)
            {

                automationBackUpCallFranchiseeModel.CallCount = franchiseeTechMailEmailData.FirstOrDefault().CallCount;
                automationBackUpCallFranchiseeModel.ChargesOfCalls = franchiseeTechMailEmailData.FirstOrDefault().ChargesForPhone;
                automationBackUpCallFranchiseeModel.ListOfCallIVR = new List<AutomationClassMarketingLead>();
                automationBackUpCallFranchiseeModel.TotalCount = 0;

                //return automationBackUpCallFranchiseeModel;
            }

            var marketingLeadClassList = _marketingLeadCallDetailRepository.Table.Where(x => (x.DateAdded >= filter.StartDate)
                                                             && x.DateAdded <= filter.EndDate).ToList();


            var franchiseeList = _franchiseeRepository.Table.Where(x => x.Organization.IsActive && (x.Id == filter.FranchiseeId)).ToList();

            var organizationAddress = franchiseeList.Select(x => x.Organization.Address).ToList();

            var addresses = (from sublist in organizationAddress
                             from Zip in sublist
                             select Zip).ToList();

            var zipCodeList = new List<string>();
            zipCodeList.AddRange(addresses.Where(x => x.Zip != null).Select(x => x.Zip.Code));
            zipCodeList.AddRange(addresses.Select(x => x.ZipCode));


            var automationBackUpCallViewListModel = new List<AutomationBackUpCallViewModel>();
            var automationBackUpCallViewModel = new AutomationBackUpCallViewModel();
            var automationBackUpCallModel = new AutomationBackUpCallModel();
            var automationBackUpCallListModel = new List<AutomationBackUpCallModel>();
            var personTeresaList = _personRepository.Table.Where(x => x.FirstName == "Teresa" && x.LastName == "Smith").ToList();
            var personKimList = _personRepository.Table.Where(x => x.FirstName == "Rochelle " && x.MiddleName == "K" && x.LastName == "VanderMeir").ToList();
            var transferToNumerForKim = new List<string>();
            var transferToNumerForTeresa = new List<string>();

            foreach (var phoneNumber in personTeresaList)
            {
                transferToNumerForTeresa.AddRange(phoneNumber.Phones.Select(x => x.Number));
                transferToNumerForTeresa = transferToNumerForTeresa.Distinct().ToList();
            }

            foreach (var phoneNumber in personKimList)
            {
                transferToNumerForKim.AddRange(phoneNumber.Phones.Select(x => x.Number));
                transferToNumerForKim = transferToNumerForKim.Distinct().ToList();
                transferToNumerForKim.Add("3029568217");
            }




            foreach (var franchisee in franchiseeList)
            {
                automationBackUpCallModel = new AutomationBackUpCallModel();
                automationBackUpCallListModel = new List<AutomationBackUpCallModel>();
                //var marketingLeadClassListFranchiseeWise = marketingLeadClassList.Where(x => x.FranchiseeId ==
                //                                                            franchisee.Organization.Id).ToList();

                var marketingLeadClassListFranchiseeWise = marketingLeadClassList.Where(x => (x.CalledFranchiseeId == null && x.FranchiseeId == franchisee.Organization.Id) || (x.CalledFranchiseeId != null && x.CalledFranchiseeId == franchisee.Organization.Id)).ToList();

                var marketingLeadClassListFranchiseeWiseIds = marketingLeadClassListFranchiseeWise.Select(x => x.Id).ToList();

                var marketingLeadClassList3FranchiseeWise = _marketingLeadCallDetail3Repository.Table.Where(x => marketingLeadClassListFranchiseeWiseIds.Contains(x.MarketingLeadCallDetailId.Value)).ToList();
                var marketingLeadClassList3TransFerToTeresaNumber = marketingLeadClassListFranchiseeWise.Where(x => transferToNumerForTeresa.Contains(x.TransferToNumber)).ToList();

                var marketingLeadClassList3TransFerToKimNumber = marketingLeadClassListFranchiseeWise.Where(x => transferToNumerForKim.Contains(x.TransferToNumber)).ToList();


                if (marketingLeadClassListFranchiseeWise.Count() == 0)
                {
                    automationBackUpCallModel.IVRCalls = 0;
                }
                else
                {
                    if (marketingLeadClassList3FranchiseeWise.Count() > 0)
                    {
                        automationBackUpCallModel.CallsToTeresa = marketingLeadClassList3TransFerToTeresaNumber.Count();

                        automationBackUpCallModel.CallsToKim = marketingLeadClassList3TransFerToKimNumber.Count();

                        automationBackUpCallModel.IVRCalls = automationBackUpCallModel.CallsToTeresa + automationBackUpCallModel.CallsToKim;

                        automationBackUpCallModel.ListOfCallToTeresa = marketingLeadClassList3TransFerToTeresaNumber.Select(x => _franchiseeServiceFeeFactory.CreateViewModel(marketingLeadClassList3FranchiseeWise.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), x)).ToList();

                        automationBackUpCallModel.ListOfCallToKim = marketingLeadClassList3TransFerToKimNumber.Select(x => _franchiseeServiceFeeFactory.CreateViewModel(marketingLeadClassList3FranchiseeWise.FirstOrDefault(x1 => x1.MarketingLeadCallDetailId == x.Id), x)).ToList();


                        automationBackUpCallModel.ListOfNumberOfIVRCalls.AddRange(automationBackUpCallModel.ListOfCallToTeresa);
                        automationBackUpCallModel.ListOfNumberOfIVRCalls.AddRange(automationBackUpCallModel.ListOfCallToKim);

                    }

                    else
                    {
                        automationBackUpCallModel.IVRCalls = 0;
                    }
                }

            }
            if (automationBackUpCallFranchiseeModel.CallCount == (long?)automationBackUpCallModel.IVRCalls)
            {
                automationBackUpCallFranchiseeModel.ListOfCallIVR = automationBackUpCallModel.ListOfNumberOfIVRCalls;
            }
            if (automationBackUpCallFranchiseeModel.CallCount == 0 && automationBackUpCallModel.IVRCalls > 0)
            {
                automationBackUpCallFranchiseeModel.CallCount = (long?)automationBackUpCallModel.IVRCalls;
            }
            return new AutomationBackUpCallFranchiseeModel()
            {
                TotalCount = (long?)automationBackUpCallModel.IVRCalls,
                ListOfCallIVR = automationBackUpCallModel.ListOfNumberOfIVRCalls,
                CallCount = automationBackUpCallFranchiseeModel.CallCount,
                ChargesOfCalls = automationBackUpCallFranchiseeModel.ChargesOfCalls

            };
        }

        public bool SaveCallDetailsReportNotes(CallDetailsReportNotesViewModel filter)
        {
            var calldetailnotes = _callDetailsReportNotesRepository.Table.Where(x => x.CallerId == filter.CallerId).ToList();
            var persionEmail = _personRepository.Table.FirstOrDefault(x => x.Id == filter.UserId).Email;
            foreach (var note in calldetailnotes)
            {
                note.IsActive = false;
                _callDetailsReportNotesRepository.Save(note);
            }
            var calldetailNote = new CallDetailsReportNotes
            {
                CallerId = filter.CallerId.Replace("-", ""),
                Notes = filter.CallNote,
                PreferredContactNumber = filter.PreferredContactNumber,
                FirstName = filter.FirstName,
                LastName = filter.LastName,
                Company = filter.Company,
                Office = filter.Office,
                ZipCode = filter.ZipCode,
                ResultingAction = filter.ResultingAction,
                HouseNumber = filter.HouseNumber,
                Street = filter.Street,
                City = filter.City,
                State = filter.State,
                Country = filter.Country,
                Timestamp = _clock.ToUtc(filter.Timestamp.GetValueOrDefault()), //DateTime.UtcNow(filter.Timestamp); //filter.Timestamp..GetValueOrDefault(),
                DataRecorderMetaData = new DataRecorderMetaData(filter.UserId.GetValueOrDefault()),
                IsNew = true,
                UserRole = filter.UserRole,
                CreatedBy = persionEmail,
                EmailId = filter.Email
            };
            _callDetailsReportNotesRepository.Save(calldetailNote);
            return true;
        }

        public CallDetailsReportNotesListViewModel GetCallDetailsReportNotes(CallDetailNotesFilter filter)
        {
            var callDetailNotesList = GetCallDetailNotesList(filter).OrderByDescending(x => x.Id);

            var finalcollection = callDetailNotesList.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

            //var marketingLeadList = _marketingLeadCallDetailRepository.Table.AsQueryable().OrderByDescending(x=>x.Id).Select(x => x.CallerId).Take(10000);
            var list = new CallDetailsReportNotesListViewModel();
            list.CallDetailsReportNotesHistory = new List<CallDetailsReportNotesHistoryModel>();
            foreach (var callNote in finalcollection)
            {
                //var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(callNote.DataRecorderMetaData.CreatedBy.Value);
                //var uploadedBy = userInfo.Email;
                //var callMatched = GetCallMatchedwithNotes(marketingLeadList.ToList(), callNote.CallerId);
                var note = new CallDetailsReportNotesHistoryModel
                {
                    CallId = callNote.MarketingLeadIdFromCallDetailsReport,
                    Id = callNote.Id,
                    CallerId = callNote.CallerId,
                    CallNote = callNote.Notes,
                    //CreatedBy = uploadedBy,
                    CreatedBy = callNote.CreatedBy,
                    CreatedOn = Convert.ToDateTime(_clock.ToLocal(callNote.Timestamp)),
                    CallMatched = callNote.MarketingLeadId != null ? true : false,
                    UserRole = callNote.UserRole,
                    IsExpend = false,
                    MarketingLeadId = callNote.MarketingLeadId,
                    Office = callNote.Office,
                    ResultingAction = callNote.ResultingAction,
                    Email = callNote.EmailId,

                    //EditModel
                    PreferredContactNumber = callNote.PreferredContactNumber,
                    FirstName = callNote.FirstName,
                    LastName = callNote.LastName,
                    Company = callNote.Company,
                    ZipCode = callNote.ZipCode,
                    HouseNumber = callNote.HouseNumber,
                    Street = callNote.Street,
                    City = callNote.City,
                    State = callNote.State,
                    Country = callNote.Country,
                    IsEdited = callNote.EditTimestamp != null ? true : false
                };
                list.CallDetailsReportNotesHistory.Add(note);
            }
            return new CallDetailsReportNotesListViewModel
            {
                Collection = list,
                Filter = filter,
                PagingModel = new PagingModel(filter.PageNumber, filter.PageSize, callDetailNotesList.Count())
            };
        }

        private IQueryable<CallDetailsReportNotes> GetCallDetailNotesList(CallDetailNotesFilter filter)
        {
            if (filter.StartDate != null)
            {
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
            }
            if (filter.EndDate != null)
            {
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
            }
            var toDate = filter.EndDate.HasValue ? filter.EndDate.Value.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var list = _callDetailsReportNotesRepository.Table.Where(x => (filter.CallerId == null || x.CallerId.Contains(filter.CallerId.Replace("-", "")))
                                                                        && (filter.StartDate == null || (x.Timestamp >= filter.StartDate))
                                                                        && (filter.EndDate == null || (x.Timestamp <= toDate))
                                                                        && (filter.Office == null || x.Office.Contains(filter.Office))
                                                                        && (filter.ResultingAction == null || x.ResultingAction.Contains(filter.ResultingAction))
                                                                        );

            return list;
        }

        public IEnumerable<FranchiseeDropdownListItem> GetOfficeCollection()
        {
            var franchiseeCollection = _unitOfWork.Repository<County>().Table.Where(z => z.FranchiseeName != null).Select(x => x.FranchiseeName).Distinct().ToList(); ;
            return franchiseeCollection.Select(f => new FranchiseeDropdownListItem
            { Display = f, Value = f }).OrderBy(x => x.Display).ToList();
        }

        public bool GetCallMatchedwithNotes(List<string> callerId, string Id)
        {
            var callerIdIsPresent = callerId.Any(x => x == Id) ? true : false;
            return callerIdIsPresent;
        }

        public IEnumerable<FranchiseeDropdownListItem> GetFranchiseeNameValuePair()
        {
            var franchiseeCollection = _unitOfWork.Repository<Franchisee>();
            return franchiseeCollection.Table.Select(f => new FranchiseeDropdownListItem
            { Display = f.Organization.Name, Value = f.Organization.Name, IsActive = f.Organization.IsActive }).OrderBy(x => x.Display).ToArray();
        }

        public bool DownloadCallNotesHistory(CallDetailNotesFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var callNotesCollection = new List<CallNotesHistoryViewModel>();
            var notesList = GetCallDetailNotesList(filter).OrderByDescending(x => x.Id).ToList();
            var marketingleadcalldetail = _marketingLeadCallDetailRepository.Table.ToList();
            var marketingleadcalldetail2 = _marketingLeadCallDetail2Repository.Table.ToList();
            var marketingleadcalldetail3 = _marketingLeadCallDetail3Repository.Table.ToList();
            var marketingleadcalldetail4 = _marketingLeadCallDetail4Repository.Table.ToList();

            foreach (var item in notesList)
            {
                if (item.MarketingLeadId != null)
                {
                    var marketinglead2 = marketingleadcalldetail2.FirstOrDefault(x => x.Id == item.MarketingLeadId);
                    var marketinglead = marketingleadcalldetail.FirstOrDefault(x => x.Id == marketinglead2.MarketingLeadCallDetailId);
                    var marketinglead3 = marketingleadcalldetail3.FirstOrDefault(x => x.MarketingLeadCallDetailId == marketinglead.Id);
                    var marketinglead4 = marketingleadcalldetail4.FirstOrDefault(x => x.MarketingLeadCallDetailId == marketinglead.Id);
                    var model = CreateNotesHistoryViewModel(item, marketinglead, marketinglead2, marketinglead3, marketinglead4);
                    callNotesCollection.Add(model);
                }
                else
                {
                    var model = CreateNotesHistoryViewModelForBlankData(item);
                    callNotesCollection.Add(model);
                }
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/CallNotesHistory-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(callNotesCollection, fileName);
        }

        private CallNotesHistoryViewModel CreateNotesHistoryViewModel(CallDetailsReportNotes item, MarketingLeadCallDetail marketinglead, MarketingLeadCallDetailV2 marketinglead2, MarketingLeadCallDetailV3 marketinglead3, MarketingLeadCallDetailV4 marketinglead4)
        {
            var model = new CallNotesHistoryViewModel
            {
                Id = item != null && item.MarketingLeadId != null && item.MarketingLeadIdFromCallDetailsReport != null ? item.MarketingLeadIdFromCallDetailsReport : null,
                CallerId = item != null && item.CallerId != null ? item.CallerId : "",
                Note = item != null && item.Notes != null ? item.Notes : "",
                AddedBy = item != null && item.CreatedBy != null ? item.CreatedBy : "",
                AddedOn = item != null && item.Timestamp != null ? Convert.ToDateTime(_clock.ToLocal(item.Timestamp)) : (DateTime)default,
                CallMatched = item != null && item.MarketingLeadId != null ? "Yes" : "No",
                Role = item != null && item.UserRole != null ? item.UserRole : "",
                Office = item != null && item.Office != null ? item.Office : "",
                ResultingAction = item != null && item.ResultingAction != null ? item.ResultingAction : "",

                FirstName = item != null && item.FirstName != null ? item.FirstName : "",
                LastName = item != null && item.LastName != null ? item.LastName : "",
                Company = item != null && item.Company != null ? item.Company : "",
                ZipCode = item != null && item.ZipCode != null ? item.ZipCode : "",
                Number = item != null && item.HouseNumber != null ? item.HouseNumber : "",
                Street = item != null && item.Street != null ? item.Street : "",
                City = item != null && item.City != null ? item.City : "",
                State = item != null && item.State != null ? item.State : "",
                Country = item != null && item.Country != null ? item.Country : "",
                PreferredContactNumber = item != null && item.PreferredContactNumber != null ? item.PreferredContactNumber : null,
                Email = item != null && item.EmailId != null ? item.EmailId : "",

                DateAndTimeOfCall = marketinglead != null ? Convert.ToDateTime(_clock.ToLocal(marketinglead.DateAdded.GetValueOrDefault())) : (DateTime?)null,
                PhoneLabel = marketinglead != null && marketinglead.PhoneLabel != null ? marketinglead.PhoneLabel : "",
                TransferToNumber = marketinglead != null && marketinglead.TransferToNumber != null ? marketinglead.TransferToNumber : "",
                CallDuration = marketinglead != null && marketinglead.CallDuration != null ? marketinglead.CallDuration : (long)default,
                DialedNumber_dnis = marketinglead != null && marketinglead.DialedNumber != null ? marketinglead.DialedNumber : "",
                FranchiseeFromInvocaAPI = marketinglead != null && marketinglead.Franchisee != null && marketinglead.Franchisee.Organization != null && marketinglead.Franchisee.Organization.Name != null ? marketinglead.Franchisee.Organization.Name : "",

                FindMeList = marketinglead2 != null && marketinglead2.FindMeList != null ? marketinglead2.FindMeList : "",
                CallRoute_MappedByZipCode = marketinglead2 != null && marketinglead2.CallRoute != null ? marketinglead2.CallRoute : "",

                RingCount = marketinglead3 != null && marketinglead3.RingCount_CallFlow != null ? marketinglead3.RingCount_CallFlow : null,
                RingSeconds = marketinglead3 != null && marketinglead3.RingSeconds_CallFlow != null ? marketinglead3.RingSeconds_CallFlow : null,
                RecordedSeconds = marketinglead3 != null && marketinglead3.RecordedSeconds_Recording != null ? marketinglead3.RecordedSeconds_Recording : null,
                CallFlowEnteredZip = marketinglead3 != null && marketinglead3.CallflowEnteredZip != null ? marketinglead3.CallflowEnteredZip : null,
                TranscriptionStatus = marketinglead3 != null && marketinglead3.TranscriptionStatus_CallAnalytics != null ? marketinglead3.TranscriptionStatus_CallAnalytics : null,

                MissedCall = marketinglead4 != null && marketinglead4.MissedCall_CallMetrics != null ? marketinglead4.MissedCall_CallMetrics : null
            };
            return model;
        }
        private CallNotesHistoryViewModel CreateNotesHistoryViewModelForBlankData(CallDetailsReportNotes item)
        {
            var model = new CallNotesHistoryViewModel
            {
                Id = item != null && item.MarketingLeadId != null && item.MarketingLeadIdFromCallDetailsReport != null ? item.MarketingLeadIdFromCallDetailsReport : null,
                CallerId = item != null && item.CallerId != null ? item.CallerId : "",
                Note = item != null && item.Notes != null ? item.Notes : "",
                AddedBy = item != null && item.CreatedBy != null ? item.CreatedBy : "",
                AddedOn = item != null && item.Timestamp != null ? Convert.ToDateTime(_clock.ToLocal(item.Timestamp)) : (DateTime)default,
                CallMatched = item != null && item.MarketingLeadId != null ? "Yes" : "No",
                Role = item != null && item.UserRole != null ? item.UserRole : "",
                Office = item != null && item.Office != null ? item.Office : "",
                ResultingAction = item != null && item.ResultingAction != null ? item.ResultingAction : "",

                FirstName = "",
                LastName = "",
                Company = "",
                ZipCode = "",
                Number = "",
                Street = "",
                City = "",
                State = "",
                Country = "",
                PreferredContactNumber = null,
                Email = "",

                DateAndTimeOfCall = (DateTime?)null,
                PhoneLabel = "",
                TransferToNumber = "",
                CallDuration = (long)default,
                DialedNumber_dnis =  "",
                FranchiseeFromInvocaAPI =  "",

                FindMeList = "",
                CallRoute_MappedByZipCode = "",

                RingCount = null,
                RingSeconds = null,
                RecordedSeconds = null,
                CallFlowEnteredZip = null,
                TranscriptionStatus = null,

                MissedCall = null
            };
            return model;
        }

        public bool EditCallDetailsReportNotes(EditCallDetailsReportNotesViewModel filter)
        {
            var calldetailnotes = _callDetailsReportNotesRepository.Table.Where(x => x.Id == filter.Id).ToList();
            var persionEmail = _personRepository.Table.FirstOrDefault(x => x.Id == filter.UserId).Email;

            foreach(var note in calldetailnotes)
            {
                if (note.CallerId != filter.CallerId)
                {
                    note.MarketingLeadId = null;
                }
                note.Id = filter.Id;
                note.CallerId = filter.CallerId;
                note.Notes = filter.CallNote;
                //note.Timestamp = _clock.ToUtc(filter.Timestamp.GetValueOrDefault());
                note.EditTimestamp = DateTime.UtcNow;
                note.PreferredContactNumber = filter.PreferredContactNumber;
                note.EmailId = filter.Email;
                note.FirstName = filter.FirstName;
                note.LastName = filter.LastName;
                note.Company = filter.Company;
                note.Office = filter.Office;
                note.ZipCode = filter.ZipCode;
                note.ResultingAction = filter.ResultingAction;
                note.HouseNumber = filter.HouseNumber;
                note.Street = filter.Street;
                note.City = filter.City;
                note.State = filter.State;
                note.Country = filter.Country;
                note.UserRole = filter.UserRole;
                note.CreatedBy = persionEmail;
                note.IsNew = false;
                _callDetailsReportNotesRepository.Save(note);
            }
            return true;
        }
    }
}
