using System;
using Core.Application.Attribute;
using Core.Organizations;
using Core.Billing.Domain;
using Core.Application;
using System.Linq;
using Core.Sales.ViewModel;
using System.Collections.Generic;
using Core.Organizations.Domain;
using Core.Users.Enum;
using Core.MarketingLead.Domain;
using Core.Scheduler.Domain;
using Core.Application.Impl;
using System.Data;
using Core.Application.Enum;
using System.Reflection;
using System.ComponentModel;
using Core.Sales.Domain;

namespace Core.Sales.Impl
{

    public class SalesFunnelNationalService : ISalesFunnelNationalService
    {
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IRepository<Organization> _organizationRepo;
        private readonly IRepository<InvoicePayment> _invoicePaymentRepo;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepo;
        private readonly IRepository<WebLead> _webLeadRepo;
        private readonly IRepository<MarketingLeadCallDetail> _marketingLeadCallDetailRepo;
        private readonly IRepository<JobScheduler> _jobSchedulerRepo;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepo;
        private readonly ISortingHelper _sortingHelper;
        private IOrderedQueryable<SalesFunnelNationalViewModel> invoiceList;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IDownloadFileHelperService _downloadFileHelperService;
        private IOrderedQueryable<SalesFunnelLocalViewModel> localList;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly ISalesFunnelFactory _salesFunnelfactory;
        private readonly IRepository<InvoiceItem> _invoiceItemsRepository;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesrepository;
        private readonly IClock _clock;
        public SalesFunnelNationalService(IUnitOfWork unitOfWork, IFranchiseeSalesService franchiseeSalesService, ISortingHelper sortingHelper, IExcelFileCreator excelFileCreator,
            IDownloadFileHelperService downloadFileHelperService, ISalesFunnelFactory salesFunnelFactory, IClock clock)
        {
            _franchiseeSalesService = franchiseeSalesService;
            _invoicePaymentRepo = unitOfWork.Repository<InvoicePayment>();
            _organizationRepo = unitOfWork.Repository<Organization>();
            _organizationRoleUserRepo = unitOfWork.Repository<OrganizationRoleUser>();
            _webLeadRepo = unitOfWork.Repository<WebLead>();
            _jobSchedulerRepo = unitOfWork.Repository<JobScheduler>();
            _franchiseeSalesRepo = unitOfWork.Repository<FranchiseeSales>();
            _marketingLeadCallDetailRepo = unitOfWork.Repository<MarketingLeadCallDetail>();
            _sortingHelper = sortingHelper;
            _excelFileCreator = excelFileCreator;
            _downloadFileHelperService = downloadFileHelperService;
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _salesFunnelfactory = salesFunnelFactory;
            _invoiceItemsRepository = unitOfWork.Repository<InvoiceItem>();
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _franchiseeSalesrepository = unitOfWork.Repository<FranchiseeSales>();
            _clock = clock;
        }

        public SalesFunnelNationalListModel GetSalesFunnelNationalList(SalesFunnelNationalListFilter filter)
        {
            IEnumerable<SalesFunnelNationalViewModel> salesFunnelList = new List<SalesFunnelNationalViewModel>();
            IEnumerable<SalesFunnelNationalBestViewModel> bestFranchiseeFunnelList = new List<SalesFunnelNationalBestViewModel>();
            salesFunnelList = getFunnelData(filter);
            var bestFrnachisee = new SalesFunnelNationalBestViewModel()
            {
                TechCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.TechCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                SaleCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SaleCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                AvgTicketCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.AvgTicket).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimateConvertionCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.EstimateConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimatesCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.EstimatesCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.JobsCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneAnswerPerCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneAnswerPer).Select(x => x.FranchiseeName).FirstOrDefault(),
                RoyalityJobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.RoyalityJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.Sales).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCloseRateCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesCloseRate).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobConvertionCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.JobConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesPerTech).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountOverTwoMinCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneLeadCountOverTwoMin).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechPerMonthCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesPerTechPerMonth).Select(x => x.FranchiseeName).FirstOrDefault(),
                WebLeadCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.WebLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                InvoicePerCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.InvoicePer).Select(x => x.FranchiseeName).FirstOrDefault(),
            };
            var salesFunnelData = new SalesFunnelNationalListModel()
            {
                Collection = salesFunnelList,
                StartDate = filter.PeriodStartDate.Value,
                EndDate = filter.PeriodEndDate.Value.AddDays(-1),
                BestFranchiseeCollection = bestFrnachisee
            };

            return salesFunnelData;
        }


        public int getMonthsCount(DateTime startDate, DateTime endDate)
        {
            int count = 0;
            for (DateTime i = startDate; i <= endDate; i = i.AddMonths(1))
            {
                ++count;

            }
            return count;
        }

        public bool CreateExcelForNatioanlFunnel(SalesFunnelNationalListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            IEnumerable<SalesFunnelNationalBestViewModel> bestSalesFunnelList = new List<SalesFunnelNationalBestViewModel>();
            var franchiseeInvoices = getFunnelData(filter).ToList();
            var ds = new DataSet();
            var ds1 = new DataSet();
            List<SalesFunnelNationalBestViewModel> bestFranchiseeFunnelList = new List<SalesFunnelNationalBestViewModel>();
            var bestFrnachisee = new SalesFunnelNationalBestViewModel()
            {
                TechCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.TechCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                SaleCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SaleCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                AvgTicketCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.AvgTicket).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimateConvertionCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.EstimateConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimatesCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.EstimatesCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobsCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.JobsCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneAnswerPerCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.PhoneAnswerPer).Select(x => x.FranchiseeName).FirstOrDefault(),
                RoyalityJobsCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.RoyalityJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.Sales).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCloseRateCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SalesCloseRate).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.PhoneLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobConvertionCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.JobConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SalesPerTech).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountOverTwoMinCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.PhoneLeadCountOverTwoMin).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechPerMonthCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SalesPerTechPerMonth).Select(x => x.FranchiseeName).FirstOrDefault(),
                WebLeadCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.WebLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                InvoicePerCountFranchisee = franchiseeInvoices.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.InvoicePer).Select(x => x.FranchiseeName).FirstOrDefault(),

            };
            var salesFunnelExcelList = franchiseeInvoices.Select(_salesFunnelfactory.CreateListModel).ToList();
            ds.Tables.Add(_excelFileCreator.ListToDataTable(salesFunnelExcelList, "Sales Funnel National"));
            DataRow row = ds.Tables[0].NewRow();

            SetValueFromInstanceInDataRow(bestFrnachisee, bestFrnachisee.GetType(), row);
            ds.Tables[0].Rows.Add(row);
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/Macro_Sales_Funnel-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(ds, fileName);

        }

        private static void SetValueFromInstanceInDataRow(SalesFunnelNationalBestViewModel obj, Type typeOfObject, DataRow row)
        {
            row[1] = obj.TechCountFranchisee;
            row[2] = obj.SaleCountFranchisee;
            row[3] = obj.WebLeadCountFranchisee;
            row[4] = obj.PhoneLeadCountFranchisee;
            row[5] = obj.PhoneLeadCountOverTwoMinCountFranchisee;
            row[6] = obj.EstimatesCountFranchisee;
            row[7] = obj.JobsCountFranchisee;
            row[8] = obj.RoyalityJobsCountFranchisee;
            row[9] = obj.SalesCountFranchisee;
            row[10] = obj.AvgTicketCountFranchisee;
            row[11] = obj.SalesPerTechCountFranchisee;
            row[12] = obj.SalesPerTechPerMonthCountFranchisee;
            row[13] = obj.PhoneAnswerPerCountFranchisee;
            row[14] = obj.EstimateConvertionCountFranchisee;
            row[15] = obj.JobConvertionCountFranchisee;
            row[16] = obj.InvoicePerCountFranchisee;
            row[17] = obj.SalesCloseRateCountFranchisee;

        }

        private static void SetValueFromInstanceInDataRowForLocal(SalesFunnelNationalBestViewModel obj, Type typeOfObject, DataRow row)
        {
            row[2] = obj.TechCountFranchisee;
            row[3] = obj.SaleCountFranchisee;
            row[4] = obj.WebLeadCountFranchisee;
            row[5] = obj.PhoneLeadCountFranchisee;
            row[6] = obj.PhoneLeadCountOverTwoMinCountFranchisee;
            row[7] = obj.EstimatesCountFranchisee;
            row[8] = obj.JobsCountFranchisee;
            row[9] = obj.RoyalityJobsCountFranchisee;
            row[10] = obj.SalesCountFranchisee;
            row[11] = obj.AvgTicketCountFranchisee;
            row[12] = obj.SalesPerTechCountFranchisee;
            row[13] = obj.SalesPerTechPerMonthCountFranchisee;
            row[14] = obj.PhoneLeadCountOverTwoMinCountFranchisee;
            row[15] = obj.EstimateConvertionCountFranchisee;
            row[16] = obj.JobConvertionCountFranchisee;
            row[17] = obj.InvoicePerCountFranchisee;
            row[18] = obj.SalesCloseRateCountFranchisee;
            row[19] = obj.MissedCallsCountFranchisee;
            row[20] = obj.LostEstimateCountFranchisee;
            row[21] = obj.LostJobsCountFranchisee;
            row[22] = obj.TotalJobsCountFranchisee;
            row[23] = obj.TotalCallsCountFranchisee;
        }
        public bool CreateExcelForLocalFunnel(SalesFunnelNationalListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            IEnumerable<SalesFunnelNationalBestViewModel> bestSalesFunnelList = new List<SalesFunnelNationalBestViewModel>();
            var salesFunnelLocalData = getFunnelDataLocal(filter).ToList();
            var ds = new DataSet();
            IEnumerable<SalesFunnelNationalBestViewModel> bestFranchiseeFunnelList = new List<SalesFunnelNationalBestViewModel>();

            var bestFrnachisee = new SalesFunnelNationalBestViewModel()
            {
                TechCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.TechCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                SaleCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SaleCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                AvgTicketCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.AvgTicket).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimateConvertionCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.EstimateConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimatesCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.EstimatesCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobsCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.JobsCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneAnswerPerCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneAnswerPer).Select(x => x.FranchiseeName).FirstOrDefault(),
                RoyalityJobsCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.RoyalityJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.Sales).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCloseRateCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesCloseRate).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobConvertionCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.JobConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesPerTech).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountOverTwoMinCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneLeadCountOverTwoMin).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechPerMonthCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesPerTechPerMonth).Select(x => x.FranchiseeName).FirstOrDefault(),
                WebLeadCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.WebLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                InvoicePerCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.InvoicePer).Select(x => x.FranchiseeName).FirstOrDefault(),
                LostEstimateCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.LostEstimate).Select(x => x.FranchiseeName).FirstOrDefault(),
                LostJobsCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.LostJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                TotalCallsCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.TotalCalls).Select(x => x.FranchiseeName).FirstOrDefault(),
                MissedCallsCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.MissedCalls).Select(x => x.FranchiseeName).FirstOrDefault(),
                TotalJobsCountFranchisee = salesFunnelLocalData.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.TotalJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
            };
            var salesFunnelExcelList = salesFunnelLocalData.Select(_salesFunnelfactory.CreateListModel).ToList();
            ds.Tables.Add(_excelFileCreator.ListToDataTable(salesFunnelExcelList, "Sales Funnel Local"));
            DataRow row = ds.Tables[0].NewRow();

            SetValueFromInstanceInDataRowForLocal(bestFrnachisee, bestFrnachisee.GetType(), row);
            ds.Tables[0].Rows.Add(row);
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/Macro_Sales_Funnel-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(ds, fileName);

        }

        public IEnumerable<SalesFunnelNationalViewModel> getFunnelData(SalesFunnelNationalListFilter filter)
        {
            int months = 12;
            int sumOfTechUsers = 0;
            int sumOfSalesUsers = 0;
            int sumOfWebLeadsUsers = 0;
            int sumOfPhoneLeadUsers = 0;
            int sumOfPhoneLeadUsersOverTwoMin = 0;
            int sumOfEstimate = 0;
            int sumOfJobs = 0;
            int sumOfRoyalityJobs = 0;
            decimal sumOfSales = 0;
            decimal bestAvgTicket = 0;
            if (filter.PeriodEndDate != null && filter.PeriodStartDate != null)
            {
                months = getMonthsCount(filter.PeriodStartDate.Value, filter.PeriodEndDate.Value);
                filter.PeriodEndDate = filter.PeriodEndDate.Value.AddDays(1);
            }
            else
            {
                int year = DateTime.Now.Year - 1;
                filter.PeriodStartDate = new DateTime(year, 1, 1);
                filter.PeriodEndDate = new DateTime(year, 12, 31);
                filter.PeriodEndDate = filter.PeriodEndDate.Value.AddDays(1);
            }
            List<SalesFunnelNationalViewModel> salesFunnelList = new List<SalesFunnelNationalViewModel>();
            var franchiseeList = _organizationRepo.Table.Where(x => ((filter.FranchiseeId == 0 || x.Id == filter.FranchiseeId) && (x.Id != 63)) && x.IsActive).ToList();

            var salesData = _organizationRoleUserRepo.Table.Where(x => filter.PeriodStartDate == null || (x.Person.DataRecorderMetaData != null && x.IsActive
                                                            && x.Person.DataRecorderMetaData.DateCreated <= filter.PeriodEndDate && x.Person.DataRecorderMetaData.DateCreated <= filter.PeriodEndDate)).ToList();

            var salesTech = _organizationRoleUserRepo.Table.Where(x => (x.Person.DataRecorderMetaData != null && x.IsActive
                                                            && x.Person.DataRecorderMetaData.DateCreated <= filter.PeriodEndDate)).ToList();

            var webLeadData = _webLeadRepo.Table.Where(x => filter.PeriodStartDate == null || (x.CreatedDate >= filter.PeriodStartDate && x.CreatedDate <= filter.PeriodEndDate)).ToList();
            var phoneLeadData = _marketingLeadCallDetailRepo.Table.Where(x => filter.PeriodStartDate == null || (x.DateAdded >= filter.PeriodStartDate && x.DateAdded <= filter.PeriodEndDate) &&
                                         !x.PhoneLabel.StartsWith("CC") && !String.IsNullOrEmpty(x.TransferToNumber) && !x.PhoneLabel.StartsWith("CORP")).ToList();
            var jobSchedulerData = _jobSchedulerRepo.Table.Where(x => filter.PeriodStartDate == null || (x.StartDate >= filter.PeriodStartDate && x.EndDate <= filter.PeriodEndDate && x.IsActive)).ToList();
            var totalSales = _franchiseeSalesRepo.Table.Where(x => filter.PeriodStartDate == null || (x.DataRecorderMetaData != null
            && x.Invoice.GeneratedOn >= filter.PeriodStartDate && x.Invoice.GeneratedOn <= filter.PeriodEndDate)).ToList();

            var startDateForJobs = filter.PeriodStartDate.Value.Date;
            var endDateForJobs = filter.PeriodEndDate.Value.Date;
            var weeklySalesDataIds = _salesDataUploadRepository.Table.Where(x => (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId) && x.PeriodStartDate >= startDateForJobs &&
                                            x.PeriodStartDate < endDateForJobs).Select(x => x.Id).ToList();



            filter.PeriodStartDate = _clock.ToUtc(filter.PeriodStartDate.Value.Date);
            filter.PeriodEndDate = _clock.ToUtc(filter.PeriodEndDate.Value.Date);

            var royalityJobsLists = _franchiseeSalesrepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                    && (weeklySalesDataIds.Contains(x.SalesDataUploadId.Value))
                    && ((x.Invoice.InvoiceItems.Any(y => y.ItemTypeId != null && y.ItemTypeId == (long)LookupTypes.Service))) &&
                    ((filter.PeriodStartDate == null || x.Invoice.GeneratedOn >= startDateForJobs) && (filter.PeriodEndDate == null || x.Invoice.GeneratedOn < endDateForJobs))).ToList();

            var franchisees = _franchiseeRepository.Table.ToList();
            foreach (var franchisee in franchiseeList)
            {
                if (franchisee.Id == 1 || franchisee.Id == 2)
                {
                    continue;
                }
                var franchiseeServices = new List<long>();
                var franchiseeServicesForRoyality = franchisees.FirstOrDefault(fs => fs.Id == franchisee.Id);
                if (franchiseeServicesForRoyality != null)
                {
                    franchiseeServices = franchiseeServicesForRoyality.FranchiseeServices.Where(x => x.CalculateRoyalty == true && x.IsActive == true).Select(x => x.ServiceTypeId).ToList();
                }
                var royalityJobsList = royalityJobsLists.Where(x => x.Invoice.InvoiceItems.Any(y => (y.ItemTypeId != null) && (franchiseeServices.Count() > 1 && y.ItemId.HasValue && franchiseeServices.Contains(y.ItemId.Value)))).OrderByDescending(x => x.Id).ToList();

                var royalityJobs = new List<long?>();


                if (filter.FranchiseeId > 1)
                {
                    royalityJobs = royalityJobsList.Where(x => x.FranchiseeId == filter.FranchiseeId).Select(x => x.InvoiceId).ToList();
                }
                else
                {
                    royalityJobs = royalityJobsList.Where(x => x.FranchiseeId == franchisee.Id).Select(x => x.InvoiceId).ToList();
                }
                var royalityInvoices = _invoiceItemsRepository.Table.Where(x => royalityJobs.Contains(x.InvoiceId) && x.ItemTypeId == (long)LookupTypes.Service && (franchiseeServices.Count() > 1 && x.ItemId.HasValue && franchiseeServices.Contains(x.ItemId.Value))).ToList();

                var salesCount = salesTech.Where(x => x.OrganizationId == franchisee.Id && x.RoleId == (long)RoleType.SalesRep).Count();
                var techCount = salesTech.Where(x => x.OrganizationId == franchisee.Id && x.RoleId == (long)RoleType.Technician).Count();
                var webCount = webLeadData.Where(x => x.FranchiseeId == franchisee.Id).Count();
                var phoneLeadCount = phoneLeadData.Where(x => x.FranchiseeId == franchisee.Id).Count();
                var phoneLeadCountOverTwoMin = phoneLeadData.Where(x => x.FranchiseeId == franchisee.Id && x.CallDuration >= 2).Count();
                var jobsCount = jobSchedulerData.Where((x => x.FranchiseeId == franchisee.Id && x.JobId != null)).Select(x=>x.JobId).Distinct().Count();
                var estimateCount = jobSchedulerData.Where((x => x.FranchiseeId == franchisee.Id && x.EstimateId != null)).Count();
                var totalSalesForFranchisee = totalSales.Where(x => x.FranchiseeId == franchisee.Id).Where(x1 => x1.Invoice.InvoiceItems.Count() > 0).Sum(x1 => x1.Invoice.InvoiceItems.Sum(x2 => x2.Amount));
                //var totalRoyalityJobs = totalSales.Where(x => x.FranchiseeId == franchisee.Id).Select(x => x.QbInvoiceNumber).Distinct().Count();
                var totalRoyalityJobs = royalityInvoices.Where(x => ((x.Invoice.GeneratedOn >= startDateForJobs && x.Invoice.GeneratedOn < endDateForJobs)
                                                                      )).Distinct().Count();
                var totalRoyalityJobs2 = royalityInvoices.Where(x => ((x.Invoice.GeneratedOn >= startDateForJobs && x.Invoice.GeneratedOn < endDateForJobs)
                                                                      )).Distinct().ToList();
                var avgTicket = totalRoyalityJobs != 0 ? (double)totalSalesForFranchisee / (double)totalRoyalityJobs : 0;
                var salesByTech = techCount != 0 ? (double)totalSalesForFranchisee / (double)techCount : 0;
                var salesByTechPerMonth = months != 0 ? (double)salesByTech / (double)months : 0;
                var phoneAnsweredOverTwoMin = phoneLeadCount != 0 ? (((double)phoneLeadCountOverTwoMin / (double)phoneLeadCount) * 100) : 0;
                var convertToEstimatePer = phoneLeadCountOverTwoMin != 0 ? ((double)((double)estimateCount / (double)phoneLeadCountOverTwoMin) * 100) : 0;
                var convertToJobPer = estimateCount != 0 ? ((double)((double)jobsCount / (double)estimateCount) * 100) : 0;
                var convertToInvoicePer = estimateCount != 0 ? (((double)totalRoyalityJobs / (double)estimateCount) * 100) : 0;
                var salesCloseRate = (webCount + phoneLeadCount) != 0 ? ((double)totalRoyalityJobs / ((double)webCount + (double)phoneLeadCount)) * 100 : 0;

                sumOfTechUsers += techCount;
                sumOfSalesUsers += salesCount;
                sumOfWebLeadsUsers += webCount;
                sumOfPhoneLeadUsers += phoneLeadCount;
                sumOfPhoneLeadUsersOverTwoMin += phoneLeadCountOverTwoMin;
                sumOfEstimate += estimateCount;
                sumOfJobs += jobsCount;
                sumOfRoyalityJobs += totalRoyalityJobs;
                sumOfSales += totalSalesForFranchisee;
                var salesFunnelNationalData = new SalesFunnelNationalViewModel()
                {
                    TechCount = techCount,
                    SaleCount = salesCount,
                    AvgTicket = Convert.ToDecimal(avgTicket),
                    EstimateConvertion = convertToEstimatePer,
                    JobConvertion = convertToJobPer,
                    InvoicePer = convertToInvoicePer,

                    EstimatesCount = estimateCount,
                    JobsCount = jobsCount,
                    FranchiseeName = franchisee.Name,
                    FranchiseeId = franchisee.Id,
                    PhoneAnswerPer = phoneAnsweredOverTwoMin,
                    PhoneLeadCount = phoneLeadCount,
                    RoyalityJobs = totalRoyalityJobs,
                    Sales = totalSalesForFranchisee,
                    SalesCloseRate = salesCloseRate,
                    SalesPerTech = Convert.ToDecimal(salesByTech),
                    SalesPerTechPerMonth = Convert.ToDecimal(salesByTechPerMonth),
                    WebLeadCount = webCount,
                    PhoneLeadCountOverTwoMin = phoneLeadCountOverTwoMin
                };
                salesFunnelList.Add(salesFunnelNationalData);
            }
            var bestFrnachisee = new SalesFunnelNationalBestViewModel()
            {
                TechCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.TechCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                SaleCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SaleCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                AvgTicketCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.AvgTicket).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimateConvertionCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.EstimateConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                EstimatesCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.EstimatesCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.JobsCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneAnswerPerCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.PhoneAnswerPer).Select(x => x.FranchiseeName).FirstOrDefault(),
                RoyalityJobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.RoyalityJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.Sales).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesCloseRateCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SalesCloseRate).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.PhoneLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                JobConvertionCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.JobConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SalesPerTech).Select(x => x.FranchiseeName).FirstOrDefault(),
                PhoneLeadCountOverTwoMinCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.PhoneLeadCountOverTwoMin).Select(x => x.FranchiseeName).FirstOrDefault(),
                SalesPerTechPerMonthCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.SalesPerTechPerMonth).Select(x => x.FranchiseeName).FirstOrDefault(),
                WebLeadCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.WebLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                InvoicePerCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National").OrderByDescending(x => x.InvoicePer).Select(x => x.FranchiseeName).FirstOrDefault(),
            };

            filter.PropName = filter.PropName == "" ? null : filter.PropName;
            if (filter.PropName != null)
            {
                salesFunnelList = getSortedList(salesFunnelList, filter);
            }
            else
            {
                salesFunnelList = _sortingHelper.ApplySorting(salesFunnelList.AsQueryable(), x => x.SalesCloseRate, (long)SortingOrder.Desc).ToList();
            }
            salesFunnelList.Add(new SalesFunnelNationalViewModel
            {
                TechCount = sumOfTechUsers,
                SaleCount = sumOfSalesUsers,
                WebLeadCount = sumOfWebLeadsUsers,
                PhoneLeadCountOverTwoMin = sumOfPhoneLeadUsersOverTwoMin,
                JobsCount = sumOfJobs,
                EstimatesCount = sumOfEstimate,
                Sales = sumOfSales,
                RoyalityJobs = sumOfRoyalityJobs,
                PhoneLeadCount = sumOfPhoneLeadUsers,
                FranchiseeName = "National",
                AvgTicket = sumOfRoyalityJobs != 0 ? (Convert.ToDecimal((double)sumOfSales / (double)sumOfRoyalityJobs)) : 0,
                SalesPerTech = sumOfTechUsers != 0 ? (Convert.ToDecimal((double)sumOfSales / (double)sumOfTechUsers)) : 0,
                SalesPerTechPerMonth = sumOfTechUsers != 0 ? (Convert.ToDecimal((double)sumOfSales / (double)sumOfTechUsers) / months) : 0,
                PhoneAnswerPer = sumOfPhoneLeadUsers != 0 ? ((double)sumOfPhoneLeadUsersOverTwoMin / (double)sumOfPhoneLeadUsers) * 100 : 0,
                EstimateConvertion = sumOfPhoneLeadUsersOverTwoMin != 0 ? ((double)sumOfEstimate / (double)sumOfPhoneLeadUsersOverTwoMin) * 100 : 0,
                JobConvertion = sumOfEstimate != 0 ? ((double)sumOfJobs / (double)sumOfEstimate) * 100 : 0,
                InvoicePer = sumOfEstimate != 0 ? ((double)sumOfRoyalityJobs / (double)sumOfEstimate) * 100 : 0,
                SalesCloseRate = sumOfWebLeadsUsers + sumOfPhoneLeadUsers != 0 ? ((double)sumOfRoyalityJobs / (double)(sumOfWebLeadsUsers + sumOfPhoneLeadUsers)) * 100 : 0,
            });
            var isPresent = salesFunnelList.Any(x => x.FranchiseeName != "National");
            salesFunnelList.Add(new SalesFunnelNationalViewModel
            {
                TechCount = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.TechCount) : 0,
                SaleCount = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.SaleCount) : 0,
                WebLeadCount = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.WebLeadCount) : 0,
                PhoneLeadCountOverTwoMin = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.PhoneLeadCountOverTwoMin) : 0,
                JobsCount = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.JobsCount) : 0,
                EstimatesCount = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.EstimatesCount) : 0,
                Sales = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.Sales) : 0,
                RoyalityJobs = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.RoyalityJobs) : 0,
                PhoneLeadCount = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.PhoneLeadCount) : 0,
                FranchiseeName = "Best",
                AvgTicket = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.AvgTicket) : 0,
                SalesPerTech = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.SalesPerTech) : 0,
                SalesPerTechPerMonth = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.SalesPerTechPerMonth) : 0,
                PhoneAnswerPer = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.PhoneAnswerPer) : 0,
                EstimateConvertion = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.EstimateConvertion) : 0,
                JobConvertion = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.JobConvertion) : 0,
                InvoicePer = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.InvoicePer) : 0,
                SalesCloseRate = isPresent ? salesFunnelList.Where(x => x.FranchiseeName != "National").Max(x => x.SalesCloseRate) : 0,
            });
            return salesFunnelList;
        }
        public List<SalesFunnelNationalViewModel> getSortedList(List<SalesFunnelNationalViewModel> model, SalesFunnelNationalListFilter filter)
        {

            switch (filter.PropName)
            {
                case "FranchiseeName":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.FranchiseeName, filter.Order);
                    break;
                case "TechCount":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.TechCount, filter.Order);
                    break;
                case "SalesCount":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SaleCount, filter.Order);
                    break;
                case "WebCount":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.WebLeadCount, filter.Order);
                    break;
                case "PhoneLeads":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.PhoneLeadCount, filter.Order);
                    break;
                case "PhoneLeadsOverTwoMIn":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.PhoneLeadCountOverTwoMin, filter.Order);
                    break;
                case "EstimateCount":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.EstimatesCount, filter.Order);
                    break;
                case "JobCount":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.JobsCount, filter.Order);
                    break;
                case "Sales":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.Sales, filter.Order);
                    break;
                case "AveTicket":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.AvgTicket, filter.Order);
                    break;
                case "SalesPerTech":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SalesPerTech, filter.Order);
                    break;
                case "SalesPerTechPermonths":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.PhoneAnswerPer, filter.Order);
                    break;
                case "PhoneAnsweredOverTwoMinPerPhone":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SalesPerTechPerMonth, filter.Order);
                    break;
                case "EstimatePer":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.EstimateConvertion, filter.Order);
                    break;
                case "JobPer":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.JobConvertion, filter.Order);
                    break;
                case "ConvertToInvoice":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.InvoicePer, filter.Order);
                    break;
                case "SalesCloseRate":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SalesCloseRate, filter.Order);
                    break;
                case "phoneAnswerPer":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.PhoneAnswerPer, filter.Order);
                    break;
                case "RoyalityJobs":
                    invoiceList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.RoyalityJobs, filter.Order);
                    break;

            }
            return invoiceList.ToList();
        }


        public class Valuess
        {
            public string FranchiseeName { get; set; }
            public int Value { get; set; }
        }
        public SalesFunnelNationalViewModel getNationalData(DateTime startDate, DateTime endDate, List<WebLead> webLeadData, List<MarketingLeadCallDetail> phoneLeadData, List<JobScheduler> jobSchedulerData, List<FranchiseeSales> franchiseeSales,
            List<OrganizationRoleUser> orgRoleUser, List<OrganizationRoleUser> techSales, List<FranchiseeSales> royalityJobs)
        {
            var royalityJobsCount = default(long);
            var activeFranchiseeId = _organizationRepo.Table.Where(x => x.IsActive && (x.Id != 1 && x.Id != 2)).Select(x => x.Id).ToList();
            var activeFranchiseeIds = _organizationRepo.Table.Where(x => x.IsActive && (x.Id != 1 && x.Id != 2 && x.Id != 63)).Select(x => x.Id).ToList();
            var franchiseeList = _franchiseeRepository.Table.Where(x => activeFranchiseeIds.Contains(x.Id)).OrderBy(x=>x.Organization.Name).ToList();
            foreach (var franchisee in franchiseeList)
            {
                var royalityJob = royalityJobs.Where(x => x.FranchiseeId == franchisee.Id).Select(x => x.InvoiceId).ToList();
                var franchiseeServices = franchisee.FranchiseeServices.Where(x => x.CalculateRoyalty == true && x.IsActive == true).Select(x => x.ServiceTypeId).ToList();
                var royalityJobsList = royalityJobs.Where(x => x.Invoice.InvoiceItems.Any(y => (y.ItemTypeId != null) && (franchiseeServices.Count() > 1 && y.ItemId.HasValue && franchiseeServices.Contains(y.ItemId.Value)))
                && (x.FranchiseeId == franchisee.Id)).OrderByDescending(x => x.Id).ToList();
                var royalityInvoices = _invoiceItemsRepository.Table.Where(x => royalityJob.Contains(x.InvoiceId) && x.ItemTypeId == (long)LookupTypes.Service && (franchiseeServices.Count() > 1 && x.ItemId.HasValue && franchiseeServices.Contains(x.ItemId.Value))).ToList();

                var franchiseeName = franchisee.Organization.Name;
                var countRoyality = royalityInvoices.Where(x => ((x.Invoice.GeneratedOn >= startDate.Date && x.Invoice.GeneratedOn < endDate.Date)
                                            )).Distinct().Count();


                royalityJobsCount += royalityInvoices.Where(x => ((x.Invoice.GeneratedOn >= startDate.Date && x.Invoice.GeneratedOn < endDate.Date)
                                          )).Distinct().Count();
            }
            int months = getMonthsCount(startDate, endDate.AddDays(-1));
            //endDate = endDate.AddDays(1);
            var salesData = orgRoleUser.Where(x => activeFranchiseeIds.Contains(x.OrganizationId)).ToList();

            var webLeadDataCount = webLeadData.Where(x => x.FranchiseeId != null && activeFranchiseeIds.Contains(x.FranchiseeId.Value)).Count();
            phoneLeadData = phoneLeadData.Where(x => x.FranchiseeId != null && (activeFranchiseeIds.Contains(x.FranchiseeId.Value))).ToList();
            var phoneLeadDataOver2minCount = phoneLeadData.Where(x => x.CallDuration > 2).Count();
            var phoneLeadDataCount = phoneLeadData.Count();
            var totalRoyalityJobs2 = franchiseeSales.Select(x => x.QbInvoiceNumber).Distinct().Count();
            franchiseeSales = franchiseeSales.Where(x => x.Invoice.GeneratedOn >= startDate.Date && x.Invoice.GeneratedOn <= endDate.Date && activeFranchiseeIds.Contains(x.FranchiseeId)).ToList();

            var totalSalesCount = franchiseeSales.Where(x1 => x1.Invoice.InvoiceItems.Count() > 0).Sum(x1 => x1.Invoice.InvoiceItems.Sum(x2 => x2.Amount));
            var techUserCount = techSales.Where(x => x.RoleId == (long)RoleType.Technician && (activeFranchiseeIds.Contains(x.OrganizationId))).Count();
            var SalesUserCount = techSales.Where(x => x.RoleId == (long)RoleType.SalesRep && (activeFranchiseeIds.Contains(x.OrganizationId))).Count();

            var jobsCount = jobSchedulerData.Where(x => x.JobId != null).Count();
            var estimatesCount = jobSchedulerData.Where(x => x.EstimateId != null).Count();
            var totalRoyalityJobs = royalityJobsCount;
            var salesFunnelNationalViewModel = new SalesFunnelNationalViewModel
            {

                TechCount = techUserCount,
                SaleCount = SalesUserCount,
                WebLeadCount = webLeadDataCount,
                PhoneLeadCountOverTwoMin = phoneLeadDataOver2minCount,
                JobsCount = jobsCount,
                EstimatesCount = estimatesCount,
                Sales = totalSalesCount,
                RoyalityJobs = totalRoyalityJobs,
                PhoneLeadCount = phoneLeadDataCount,
                FranchiseeName = "National",
                AvgTicket = totalRoyalityJobs != 0 ? Convert.ToDecimal((double)totalSalesCount / (double)totalRoyalityJobs) : 0,
                SalesPerTech = techUserCount != 0 ? Convert.ToDecimal((double)totalSalesCount / (double)techUserCount) : 0,
                SalesPerTechPerMonth = techUserCount != 0 ? Convert.ToDecimal((double)totalSalesCount / (double)techUserCount) / months : 0,
                PhoneAnswerPer = phoneLeadDataCount != 0 ? ((double)phoneLeadDataOver2minCount / (double)phoneLeadDataCount) * 100 : 0,
                EstimateConvertion = phoneLeadDataOver2minCount != 0 ? ((double)estimatesCount / (double)phoneLeadDataOver2minCount) * 100 : 0,
                JobConvertion = estimatesCount != 0 ? ((double)jobsCount / (double)estimatesCount) * 100 : 0,
                InvoicePer = estimatesCount != 0 ? ((double)totalRoyalityJobs / (double)estimatesCount) * 100 : 0,
                SalesCloseRate = webLeadDataCount + phoneLeadDataCount != 0 ? ((double)totalRoyalityJobs / (double)(webLeadDataCount + phoneLeadDataCount)) * 100 : 0,
            };
            return salesFunnelNationalViewModel;
        }

        public List<SalesFunnelLocalViewModel> getSortedListLocal(List<SalesFunnelLocalViewModel> model, SalesFunnelNationalListFilter filter)
        {

            switch (filter.PropName)
            {
                case "FranchiseeName":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.FranchiseeName, filter.Order);
                    break;
                case "TechCount":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.TechCount, filter.Order);
                    break;
                case "SalesCount":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SaleCount, filter.Order);
                    break;
                case "WebCount":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.WebLeadCount, filter.Order);
                    break;
                case "PhoneLeads":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.PhoneLeadCount, filter.Order);
                    break;
                case "PhoneLeadsOverTwoMIn":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.PhoneLeadCountOverTwoMin, filter.Order);
                    break;
                case "EstimateCount":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.EstimatesCount, filter.Order);
                    break;
                case "JobCount":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.JobsCount, filter.Order);
                    break;
                case "Sales":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.Sales, filter.Order);
                    break;
                case "AveTicket":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.AvgTicket, filter.Order);
                    break;
                case "SalesPerTech":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SalesPerTech, filter.Order);
                    break;
                case "SalesPerTechPermonths":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SalesPerTechPerMonth, filter.Order);
                    break;
                case "PhoneAnsweredOverTwoMinPerPhone":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SalesPerTechPerMonth, filter.Order);
                    break;
                case "EstimatePer":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.EstimateConvertion, filter.Order);
                    break;
                case "JobPer":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.JobConvertion, filter.Order);
                    break;
                case "ConvertToInvoice":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.InvoicePer, filter.Order);
                    break;
                case "SalesCloseRate":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.SalesCloseRate, filter.Order);
                    break;
                case "phoneAnswerPer":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.PhoneAnswerPer, filter.Order);
                    break;
                case "RoyalityJobs":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.RoyalityJobs, filter.Order);
                    break;
                case "MissedCalls":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.MissedCalls, filter.Order);
                    break;
                case "LostEstimate":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.LostEstimate, filter.Order);
                    break;
                case "LostJobs":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.LostJobs, filter.Order);
                    break;
                case "TotalJobs":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.TotalJobs, filter.Order);
                    break;
                case "TotalCalls":
                    localList = _sortingHelper.ApplySorting(model.AsQueryable(), x => x.TotalCalls, filter.Order);
                    break;

            }
            return localList.ToList();
        }
        public SalesFunnelNationalListModel GetSalesFunnelLocalList(SalesFunnelNationalListFilter filter)
        {
            var bestFrnachisee = new SalesFunnelNationalBestViewModel();
            IEnumerable<SalesFunnelLocalViewModel> salesFunnelList = new List<SalesFunnelLocalViewModel>();
            IEnumerable<SalesFunnelLocalViewModel> bestFranchiseeFunnelList = new List<SalesFunnelLocalViewModel>();
            salesFunnelList = getFunnelDataLocal(filter);
            if (salesFunnelList != null)
            {
                bestFrnachisee = new SalesFunnelNationalBestViewModel()
                {
                    TechCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.TechCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                    SaleCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SaleCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                    AvgTicketCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.AvgTicket).Select(x => x.FranchiseeName).FirstOrDefault(),
                    EstimateConvertionCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.EstimateConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                    EstimatesCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.EstimatesCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                    JobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.JobsCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                    PhoneAnswerPerCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneAnswerPer).Select(x => x.FranchiseeName).FirstOrDefault(),
                    RoyalityJobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.RoyalityJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                    SalesCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.Sales).Select(x => x.FranchiseeName).FirstOrDefault(),
                    SalesCloseRateCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesCloseRate).Select(x => x.FranchiseeName).FirstOrDefault(),
                    PhoneLeadCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                    JobConvertionCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.JobConvertion).Select(x => x.FranchiseeName).FirstOrDefault(),
                    SalesPerTechCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesPerTech).Select(x => x.FranchiseeName).FirstOrDefault(),
                    PhoneLeadCountOverTwoMinCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.PhoneLeadCountOverTwoMin).Select(x => x.FranchiseeName).FirstOrDefault(),
                    SalesPerTechPerMonthCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.SalesPerTechPerMonth).Select(x => x.FranchiseeName).FirstOrDefault(),
                    WebLeadCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.WebLeadCount).Select(x => x.FranchiseeName).FirstOrDefault(),
                    InvoicePerCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.InvoicePer).Select(x => x.FranchiseeName).FirstOrDefault(),
                    LostEstimateCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.LostEstimate).Select(x => x.FranchiseeName).FirstOrDefault(),
                    LostJobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.LostJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                    TotalCallsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.TotalCalls).Select(x => x.FranchiseeName).FirstOrDefault(),
                    MissedCallsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.MissedCalls).Select(x => x.FranchiseeName).FirstOrDefault(),
                    TotalJobsCountFranchisee = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Best").OrderByDescending(x => x.TotalJobs).Select(x => x.FranchiseeName).FirstOrDefault(),
                };
            }

            var salesFunnelData = new SalesFunnelNationalListModel()
            {
                LocalCollection = salesFunnelList,
                StartDate = filter.PeriodStartDate.Value,
                EndDate = filter.PeriodEndDate.Value.AddDays(-1),
                BestFranchiseeCollection = bestFrnachisee
            };

            return salesFunnelData;
        }

        public IEnumerable<SalesFunnelLocalViewModel> getFunnelDataLocal(SalesFunnelNationalListFilter filter)
        {
            var franchiseeServices = new List<long>();
            if (filter.FranchiseeId == 1 || filter.FranchiseeId == 2)
            {
                return null;
            }
            int months = 12;

            if (filter.PeriodEndDate != null && filter.PeriodStartDate != null)
            {
                months = getMonthsCount(filter.PeriodStartDate.Value, filter.PeriodEndDate.Value);
                filter.PeriodEndDate = filter.PeriodEndDate.Value.AddDays(1);
            }
            else
            {
                int year = DateTime.Now.Year - 1;
                filter.PeriodStartDate = new DateTime(year, 1, 1);
                filter.PeriodEndDate = new DateTime(year, 12, 31);
                filter.PeriodEndDate = filter.PeriodEndDate.Value.AddDays(1);
            }
            var periodStartDateForWebLead = filter.PeriodStartDate.Value;
            var periodEndDateForWebLead = filter.PeriodEndDate.Value;

            var startForRoyality = filter.PeriodStartDate.Value.Date;
            var endForRoyality = filter.PeriodEndDate.Value.Date;

            filter.PeriodEndDate = _clock.ToUtc(filter.PeriodEndDate.Value);
            filter.PeriodStartDate = _clock.ToUtc(filter.PeriodStartDate.Value);
            List<SalesFunnelLocalViewModel> salesFunnelList = new List<SalesFunnelLocalViewModel>();
            var franchisee = _organizationRepo.Table.Where(x => (filter.FranchiseeId == 0 || x.Id == filter.FranchiseeId) && x.IsActive).FirstOrDefault();
            if (franchisee == null)
                return null;
            var orgRoleUser = _organizationRoleUserRepo.Table.Where(x => filter.PeriodStartDate == null || (x.Person.DataRecorderMetaData.DateCreated >= filter.PeriodStartDate && x.Person.DataRecorderMetaData.DateCreated <= filter.PeriodEndDate) && x.IsActive).ToList();
            var salesTech = _organizationRoleUserRepo.Table.Where(x => (x.Person.DataRecorderMetaData != null && x.IsActive
                                                          && x.Person.DataRecorderMetaData.DateCreated <= filter.PeriodEndDate)).ToList();
            var webLeadData = _webLeadRepo.Table.Where(x => filter.PeriodStartDate == null || (x.CreatedDate >= periodStartDateForWebLead && x.CreatedDate <= periodEndDateForWebLead)).ToList();
            var phoneLeadData = _marketingLeadCallDetailRepo.Table.Where(x => filter.PeriodStartDate == null || (x.DateAdded >= filter.PeriodStartDate && x.DateAdded <= filter.PeriodEndDate) &&
                                        !x.PhoneLabel.StartsWith("CC") && !String.IsNullOrEmpty(x.TransferToNumber) && !x.PhoneLabel.StartsWith("CORP")).ToList();
            var jobSchedulerData = _jobSchedulerRepo.Table.Where(x => filter.PeriodStartDate == null || (x.StartDate >= filter.PeriodStartDate && x.EndDate <= filter.PeriodEndDate) && x.IsActive).ToList();
            var totalSales = _franchiseeSalesRepo.Table.Where(x => x.Invoice.GeneratedOn >= startForRoyality && x.Invoice.GeneratedOn <= endForRoyality).AsEnumerable().ToList();


            var weeklySalesDataId = _salesDataUploadRepository.Table.Where(x => x.PeriodStartDate >= startForRoyality && x.PeriodStartDate < endForRoyality).ToList();
            var weeklySalesDataIdForLocal = weeklySalesDataId.Where(x => (filter.FranchiseeId == 0 || x.FranchiseeId == filter.FranchiseeId)).Select(x => x.Id).ToList();

            var weeklySalesDataIds = weeklySalesDataId.Select(x => x.Id).ToList();
            var royalityJobsListsNational = _franchiseeSalesrepository.Table.Where(x => x.SalesDataUploadId.HasValue && weeklySalesDataIds.Contains(x.SalesDataUploadId.Value) &&
                    ((x.Invoice.InvoiceItems.Any(y => y.ItemTypeId != null && y.ItemTypeId == (long)LookupTypes.Service))) &&
                    ((filter.PeriodStartDate == null || x.Invoice.GeneratedOn >= startForRoyality) && (filter.PeriodEndDate == null || x.Invoice.GeneratedOn < endForRoyality))).ToList();

            var royalityJobsLists = royalityJobsListsNational.Where(x => x.SalesDataUploadId.HasValue && weeklySalesDataIdForLocal.Contains(x.SalesDataUploadId.Value) && x.FranchiseeId == filter.FranchiseeId).ToList();

            var franchisees = _franchiseeRepository.Table.ToList();
            var franchiseeServicesForRoyality = franchisees.FirstOrDefault(fs => fs.Id == franchisee.Id);
            if (franchiseeServicesForRoyality != null)
            {
                franchiseeServices = franchiseeServicesForRoyality.FranchiseeServices.Where(x => x.CalculateRoyalty == true && x.IsActive == true).Select(x => x.ServiceTypeId).ToList();
            }
            var royalityInvoices = new List<InvoiceItem>();
            for (DateTime i = filter.PeriodStartDate.GetValueOrDefault(); i <= filter.PeriodEndDate.Value.AddDays(-1); i = i.AddMonths(1))
            {
                var startDate = i.Date;


                var endDate = i.AddMonths(1).Date;
                if (endDate > filter.PeriodEndDate)
                {
                    endDate = filter.PeriodEndDate.Value.Date;
                }
                var startDateForWebLead = startDate;
                var endDateForWebLead = endDate;
                startDate = _clock.ToUtc(startDate);
                endDate = _clock.ToUtc(endDate);



                var royalityJobsList = royalityJobsLists.Where(x => x.Invoice.InvoiceItems.Any(y => (y.ItemTypeId != null) && (franchiseeServices.Count() > 1 && y.ItemId.HasValue && franchiseeServices.Contains(y.ItemId.Value)))
                && (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                                           ).OrderByDescending(x => x.Id).ToList();

                var royalityJobs = new List<long?>();


                if (filter.FranchiseeId > 1)
                {
                    royalityJobs = royalityJobsList.Where(x => x.FranchiseeId == filter.FranchiseeId).Select(x => x.InvoiceId).ToList();
                }
                else
                {
                    royalityJobs = royalityJobsList.Where(x => x.FranchiseeId == franchisee.Id).Select(x => x.InvoiceId).ToList();
                }
                royalityInvoices = _invoiceItemsRepository.Table.Where(x => royalityJobs.Contains(x.InvoiceId) && x.ItemTypeId == (long)LookupTypes.Service && (franchiseeServices.Count() > 1 && x.ItemId.HasValue && franchiseeServices.Contains(x.ItemId.Value))).ToList();

                //endDate = endDate.AddDays(-1);
                var salesCount = salesTech.Where(x => x.OrganizationId == filter.FranchiseeId && x.RoleId == (long)RoleType.SalesRep && (x.Person.DataRecorderMetaData.DateCreated <= endDate)).Count();
                var techCount = salesTech.Where(x => x.OrganizationId == filter.FranchiseeId && x.RoleId == (long)RoleType.Technician && (x.Person.DataRecorderMetaData.DateCreated <= endDate)).Count();
                var webCount = webLeadData.Where(x => x.FranchiseeId == filter.FranchiseeId && (x.CreatedDate >= startDateForWebLead && x.CreatedDate < endDateForWebLead)).Count();
                var phoneLeadCount = phoneLeadData.Where(x => (x.DateAdded >= startDate && x.DateAdded <= endDate) && x.FranchiseeId == filter.FranchiseeId).Count();
                var phoneLeadCountOverTwoMin = phoneLeadData.Where(x => (x.DateAdded >= startDate && x.DateAdded <= endDate) && x.FranchiseeId == filter.FranchiseeId && x.CallDuration >= 2).Count();
                var jobsCount = jobSchedulerData.Where((x => ((x.StartDate >= startDate && x.EndDate <= endDate)) && x.FranchiseeId == filter.FranchiseeId && x.JobId != null)).Select(x=>x.JobId).Distinct().Count();

                var estimateCount = jobSchedulerData.Where((x => ((x.StartDate >= startDate && x.EndDate <= endDate))
                                                          && x.EstimateId != null && x.FranchiseeId == filter.FranchiseeId)).Count();
                var totalSalesForFranchisee = (totalSales.Where(x => x.FranchiseeId == filter.FranchiseeId && (x.Invoice.GeneratedOn >= startDateForWebLead && x.Invoice.GeneratedOn < endDateForWebLead)).Where(x1 => x1.Invoice.InvoiceItems.Count() > 0).Sum(x1 => x1.Invoice.InvoiceItems.Sum(x2 => x2.Amount)));
                var totalRoyalityJobs = royalityInvoices.Where(x => ((x.Invoice.GeneratedOn >= startDateForWebLead && x.Invoice.GeneratedOn < endDateForWebLead)
                                                                       )).Distinct().Count();
                var totalRoyalityJobsList = royalityInvoices.Where(x => (x.Invoice.GeneratedOn >= startDateForWebLead && x.Invoice.GeneratedOn <= endDateForWebLead)).ToList();
                var avgTicket = totalRoyalityJobs != 0 ? totalSalesForFranchisee / totalRoyalityJobs : 0;
                var salesByTech = techCount != 0 ? totalSalesForFranchisee / techCount : 0;
                var salesByTechPerMonth = months != 0 ? salesByTech / months : 0;
                var phoneAnsweredOverTwoMin = phoneLeadCount != 0 ? ((double)phoneLeadCountOverTwoMin / (double)phoneLeadCount) * 100 : 0;
                var convertToEstimatePer = phoneLeadCountOverTwoMin != 0 ? ((double)estimateCount / (double)phoneLeadCountOverTwoMin) * 100 : 0;
                var convertToJobPer = estimateCount != 0 ? ((double)jobsCount / (double)estimateCount) * 100 : 0;
                var convertToInvoicePer = estimateCount != 0 ? ((double)totalRoyalityJobs / (double)estimateCount) * 100 : 0;
                var salesCloseRate = (webCount + phoneLeadCount) != 0 ? ((double)totalRoyalityJobs / (double)(webCount + phoneLeadCount)) * 100 : 0;

                var missedCalls = phoneLeadCount - phoneLeadCountOverTwoMin;
                var lostEstimates = phoneLeadCountOverTwoMin - estimateCount;
                var lostJobs = estimateCount - jobsCount;
                var totalJobs = totalRoyalityJobs;
                var totalCalls = missedCalls + lostEstimates + lostJobs + totalJobs;



                var salesFunnelNationalData = new SalesFunnelLocalViewModel()
                {
                    TechCount = techCount,
                    SaleCount = salesCount,
                    AvgTicket = avgTicket,
                    EstimateConvertion = convertToEstimatePer,
                    JobConvertion = convertToJobPer,
                    InvoicePer = convertToInvoicePer,
                    Month = i.Month,
                    EstimatesCount = estimateCount,
                    JobsCount = jobsCount,
                    FranchiseeName = franchisee.Name,
                    FranchiseeId = filter.FranchiseeId,
                    PhoneAnswerPer = phoneAnsweredOverTwoMin,
                    PhoneLeadCount = phoneLeadCount,
                    RoyalityJobs = totalRoyalityJobs,
                    Sales = totalSalesForFranchisee,
                    SalesCloseRate = salesCloseRate,
                    SalesPerTech = salesByTech,
                    SalesPerTechPerMonth = salesByTechPerMonth,
                    WebLeadCount = webCount,
                    PhoneLeadCountOverTwoMin = phoneLeadCountOverTwoMin,
                    MissedCalls = missedCalls,
                    LostEstimate = lostEstimates,
                    LostJobs = lostJobs,
                    TotalCalls = totalCalls,
                    TotalJobs = totalJobs
                };
                salesFunnelList.Add(salesFunnelNationalData);
            }

            filter.PropName = filter.PropName == "" ? null : filter.PropName;
            if (filter.PropName != null)
            {
                salesFunnelList = getSortedListLocal(salesFunnelList, filter);
            }
            var national = getNationalData(filter.PeriodStartDate.Value, filter.PeriodEndDate.Value, webLeadData, phoneLeadData, jobSchedulerData, totalSales, orgRoleUser, salesTech, royalityJobsListsNational);

            var totalJobsNational = national.RoyalityJobs;
            var lostJobsNational = national.EstimatesCount - national.JobsCount;
            var lostEstimateNational = national.PhoneLeadCountOverTwoMin - national.EstimatesCount;
            var missedCallsNational = Convert.ToInt32(national.PhoneLeadCount - national.PhoneLeadCountOverTwoMin);
            var totalCallsNational = missedCallsNational + missedCallsNational + lostJobsNational + totalJobsNational;
            salesFunnelList.Add(new SalesFunnelLocalViewModel
            {
                TechCount = national.TechCount,
                SaleCount = national.SaleCount,
                WebLeadCount = national.WebLeadCount,
                PhoneLeadCountOverTwoMin = national.PhoneLeadCountOverTwoMin,
                JobsCount = national.JobsCount,
                EstimatesCount = national.EstimatesCount,
                Sales = national.Sales,
                RoyalityJobs = national.RoyalityJobs,
                PhoneLeadCount = national.PhoneLeadCount,
                FranchiseeName = "National",
                AvgTicket = national.AvgTicket,
                SalesPerTech = national.SalesPerTech,
                SalesPerTechPerMonth = national.SalesPerTechPerMonth,
                PhoneAnswerPer = national.PhoneAnswerPer,
                EstimateConvertion = national.EstimateConvertion,
                JobConvertion = national.JobConvertion,
                InvoicePer = national.InvoicePer,
                SalesCloseRate = national.SalesCloseRate,
                TotalJobs = Convert.ToInt32(totalJobsNational),
                TotalCalls = Convert.ToInt32(totalCallsNational),
                LostJobs = Convert.ToInt32(lostJobsNational),
                LostEstimate = Convert.ToInt32(lostEstimateNational),
                MissedCalls = Convert.ToInt32(missedCallsNational),
                FranchiseeId = franchisee.Id,
            });
            salesFunnelList.Add(new SalesFunnelLocalViewModel
            {
                TechCount = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.TechCount) / months, 2),
                SaleCount = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.SaleCount) / months, 2),
                WebLeadCount = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.WebLeadCount) / months, 2),
                PhoneLeadCountOverTwoMin = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.PhoneLeadCountOverTwoMin) / months, 2),
                JobsCount = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.JobsCount) / months, 2),
                EstimatesCount = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.EstimatesCount) / months, 2),
                Sales = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.Sales) / months, 2),
                RoyalityJobs = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.RoyalityJobs) / months, 2),
                PhoneLeadCount = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.PhoneLeadCount) / months, 2),
                FranchiseeName = "Average",
                AvgTicket = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.AvgTicket) / months, 2),
                SalesPerTech = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.SalesPerTech) / months, 2),
                SalesPerTechPerMonth = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.SalesPerTechPerMonth) / months, 2),
                PhoneAnswerPer = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.PhoneAnswerPer) / months, 2),
                EstimateConvertion = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.EstimateConvertion) / months, 2),
                JobConvertion = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.JobConvertion) / months, 2),
                InvoicePer = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.InvoicePer) / months, 2),
                SalesCloseRate = Math.Round(salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.SalesCloseRate) / months, 2),
                MissedCalls = salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.MissedCalls) / months,
                LostEstimate = salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.LostEstimate) / months,
                LostJobs = salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.LostJobs) / months,
                TotalCalls = salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.TotalCalls) / months,
                TotalJobs = salesFunnelList.Where(x => x.FranchiseeName != "National").Sum(x => x.TotalJobs) / months
            });
            salesFunnelList.Add(new SalesFunnelLocalViewModel
            {
                TechCount = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.TechCount),
                SaleCount = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.SaleCount),
                WebLeadCount = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.WebLeadCount),
                PhoneLeadCountOverTwoMin = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.PhoneLeadCountOverTwoMin),
                JobsCount = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.JobsCount),
                EstimatesCount = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.EstimatesCount),
                Sales = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.Sales),
                RoyalityJobs = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.RoyalityJobs),
                PhoneLeadCount = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.PhoneLeadCount),
                FranchiseeName = "Best",
                AvgTicket = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.AvgTicket),
                SalesPerTech = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.SalesPerTech),
                SalesPerTechPerMonth = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.SalesPerTechPerMonth),
                PhoneAnswerPer = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.PhoneAnswerPer),
                EstimateConvertion = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.EstimateConvertion),
                JobConvertion = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.JobConvertion),
                InvoicePer = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.InvoicePer),
                SalesCloseRate = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.SalesCloseRate),
                MissedCalls = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.MissedCalls),
                LostEstimate = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.LostEstimate),
                LostJobs = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.LostJobs),
                TotalCalls = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.TotalCalls),
                TotalJobs = salesFunnelList.Where(x => x.FranchiseeName != "National" && x.FranchiseeName != "Average").Max(x => x.TotalJobs),
            });
            return salesFunnelList;
        }

        public SalesFunnelLocalGraphListModel GenerateChartData(SalesFunnelNationalListFilter filter)
        {
            var graphData = getFunnelDataLocalForGraph(filter);
            return new SalesFunnelLocalGraphListModel
            {
                ChartData = graphData,
                Franchisee = ""

            };
        }
        public IEnumerable<SalesFunnelLocalGraphViewModel> getFunnelDataLocalForGraph(SalesFunnelNationalListFilter filter)
        {
            var franchiseeServices = new List<long>();
            if (filter.FranchiseeId == 1 || filter.FranchiseeId == 2)
            {
                return null;
            }
            int months = 12;
            if (filter.PeriodEndDate != null && filter.PeriodStartDate != null)
            {
                months = getMonthsCount(filter.PeriodStartDate.Value, filter.PeriodEndDate.Value);
                filter.PeriodEndDate = filter.PeriodEndDate.Value.AddDays(1);
            }
            else
            {
                int year = DateTime.Now.Year - 1;
                filter.PeriodStartDate = new DateTime(year, 1, 1);
                filter.PeriodEndDate = new DateTime(year, 12, 31);
                filter.PeriodEndDate = filter.PeriodEndDate.Value.AddDays(1);
            }
            List<SalesFunnelLocalGraphViewModel> salesFunnelList = new List<SalesFunnelLocalGraphViewModel>();
            var startForRoyality = filter.PeriodStartDate.Value.Date;
            var endForRoyality = filter.PeriodEndDate.Value.Date;
            filter.PeriodStartDate = _clock.ToUtc(filter.PeriodStartDate.Value);
            filter.PeriodEndDate = _clock.ToUtc(filter.PeriodEndDate.Value);
            var franchisee = _organizationRepo.Table.Where(x => (filter.FranchiseeId == 0 || x.Id == filter.FranchiseeId) && x.IsActive).FirstOrDefault();
            var phoneLeadData = _marketingLeadCallDetailRepo.Table.Where(x => (filter.PeriodStartDate == null || (x.DateAdded >= filter.PeriodStartDate && x.DateAdded <= filter.PeriodEndDate)) &&
                                        !x.PhoneLabel.StartsWith("CC") && !String.IsNullOrEmpty(x.TransferToNumber) && !x.PhoneLabel.StartsWith("CORP")).ToList();

            if (franchisee == null)
                return null;

            var weeklySalesDataIds = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == filter.FranchiseeId && x.PeriodStartDate >= startForRoyality && x.PeriodStartDate < endForRoyality).Select(x => x.Id).ToList();
            var royalityJobsLists = _franchiseeSalesrepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                   && (weeklySalesDataIds.Contains(x.SalesDataUploadId.Value))
                   && ((x.Invoice.InvoiceItems.Any(y => y.ItemTypeId != null && y.ItemTypeId == (long)LookupTypes.Service))) &&
                   ((filter.PeriodStartDate == null || x.Invoice.GeneratedOn >= startForRoyality) && (filter.PeriodEndDate == null || x.Invoice.GeneratedOn < endForRoyality))).ToList();

            var franchisees = _franchiseeRepository.Table.ToList();
            var royalityInvoices = new List<InvoiceItem>();

            for (DateTime i = filter.PeriodStartDate.GetValueOrDefault(); i <= filter.PeriodEndDate.Value.AddDays(-1); i = i.AddMonths(1))
            {

                var startDate = i.Date;
                var endDate = i.AddMonths(1).Date;
                var startDateForWebLead = startDate;
                var endDateForWebLead = endDate;
                startDate = _clock.ToUtc(startDate);
                endDate = _clock.ToUtc(endDate);

                var franchiseeServicesForRoyality = franchisees.FirstOrDefault(fs => fs.Id == franchisee.Id);
                if (franchiseeServicesForRoyality != null)
                {
                    franchiseeServices = franchiseeServicesForRoyality.FranchiseeServices.Where(x => x.CalculateRoyalty == true && x.IsActive == true).Select(x => x.ServiceTypeId).ToList();
                }
                var royalityJobsList = royalityJobsLists.Where(x => x.Invoice.InvoiceItems.Any(y => (y.ItemTypeId != null) && (franchiseeServices.Count() > 1 && y.ItemId.HasValue && franchiseeServices.Contains(y.ItemId.Value)))
                                            ).OrderByDescending(x => x.Id).ToList();

                var royalityJobs = new List<long?>();


                if (filter.FranchiseeId > 1)
                {
                    royalityJobs = royalityJobsList.Where(x => x.FranchiseeId == filter.FranchiseeId).Select(x => x.InvoiceId).ToList();
                }
                else
                {
                    royalityJobs = royalityJobsList.Where(x => x.FranchiseeId == franchisee.Id).Select(x => x.InvoiceId).ToList();
                }
                royalityInvoices = _invoiceItemsRepository.Table.Where(x => royalityJobs.Contains(x.InvoiceId) && x.ItemTypeId == (long)LookupTypes.Service && (franchiseeServices.Count() > 1 && x.ItemId.HasValue && franchiseeServices.Contains(x.ItemId.Value))).ToList();


                var webLeadData = _webLeadRepo.Table.Where(x => (filter.PeriodStartDate == null || (x.CreatedDate >= startDateForWebLead && x.CreatedDate <= endDateForWebLead))).ToList();

                var jobSchedulerData = _jobSchedulerRepo.Table.Where(x => (filter.PeriodStartDate == null || (x.StartDate >= startDate && x.EndDate <= endDate)) && x.IsActive).ToList();
                var totalSales = _franchiseeSalesRepo.Table.Where(x => filter.PeriodStartDate == null || (x.DataRecorderMetaData != null
                                                                 && x.Invoice.GeneratedOn >= startDate.Date && x.Invoice.GeneratedOn <= endDate)).AsEnumerable().ToList();
                var webCount = webLeadData.Where(x => x.FranchiseeId == filter.FranchiseeId).Count();
                var phoneLeadCount = phoneLeadData.Where(x => (x.FranchiseeId == filter.FranchiseeId) && (x.DateAdded >= startDate && x.DateAdded <= endDate)).Count();
                var phoneLeadCountOverTwoMin = phoneLeadData.Where(x => (x.FranchiseeId == filter.FranchiseeId) && x.CallDuration >= 2 && (x.DateAdded >= startDate && x.DateAdded <= endDate)).Count();
                var jobsCount = jobSchedulerData.Where((x => ((x.StartDate >= startDate && x.EndDate <= endDate)) && x.FranchiseeId == filter.FranchiseeId && x.JobId != null)).Select(x => x.JobId).Distinct().Count();
                var estimateCount = jobSchedulerData.Where((x => x.FranchiseeId == filter.FranchiseeId && x.EstimateId != null && ((x.StartDate >= startDate && x.EndDate <= endDate)))).Count();
                var totalRoyalityJobs = royalityInvoices.Where(x => ((x.Invoice.GeneratedOn >= startDateForWebLead && x.Invoice.GeneratedOn < endDateForWebLead)
                                                                       )).Distinct().Count();
                var phoneAnsweredOverTwoMin = phoneLeadCount != 0 ? ((double)phoneLeadCountOverTwoMin / (double)phoneLeadCount) * 100 : 0;
                var convertToEstimatePer = phoneLeadCountOverTwoMin != 0 ? ((double)estimateCount / (double)phoneLeadCountOverTwoMin) * 100 : 0;
                var convertToJobPer = estimateCount != 0 ? ((double)jobsCount / (double)estimateCount) * 100 : 0;
                var convertToInvoicePer = estimateCount != 0 ? ((double)totalRoyalityJobs / (double)estimateCount) * 100 : 0;
                var salesCloseRate = (webCount + phoneLeadCount) != 0 ? ((double)totalRoyalityJobs / (double)(webCount + phoneLeadCount)) * 100 : 0;

                var missedCalls = phoneLeadCount - phoneLeadCountOverTwoMin;
                var lostEstimates = phoneLeadCountOverTwoMin - estimateCount;
                var lostJobs = estimateCount - jobsCount;
                var totalJobs = totalRoyalityJobs;
                var totalCalls = missedCalls + lostEstimates + lostJobs + totalJobs;

                var salesFunnelNationalData = new SalesFunnelLocalGraphViewModel()
                {
                    Date = i,
                    DateString = i.ToShortDateString(),
                    DayOfWeek = i.DayOfWeek.ToString(),

                    //LostEstimateCount = lostEstimates,
                    //LostJobsCount = lostJobs,
                    TotalJobsCount = totalJobs,
                    MissedCallsCount = missedCalls,
                    //ConvertToEstimateCount = (int)Math.Round(convertToEstimatePer, 0),
                    //ConvertToJobCount = (int)Math.Round(convertToJobPer, 0),
                    //ConvertToInvoiceCount = (int)Math.Round(convertToInvoicePer, 0),
                    SalesCloseRateCount = (int)Math.Round(salesCloseRate, 0),
                    PhoneAnsweredCount = (int)Math.Round(phoneAnsweredOverTwoMin, 0),
                    LastYearDateString = i.Date.ToShortDateString()
                };
                if(convertToJobPer <= 100)
                {
                    salesFunnelNationalData.ConvertToJobCount = (int)Math.Round(convertToJobPer, 0);
                }
                if (convertToJobPer > 100)
                {
                    salesFunnelNationalData.Status = "Invalid Data";
                }
                if (convertToEstimatePer <= 100)
                {
                    salesFunnelNationalData.ConvertToEstimateCount = (int)Math.Round(convertToEstimatePer, 0);
                }
                if (convertToInvoicePer <= 100)
                {
                    salesFunnelNationalData.ConvertToInvoiceCount = (int)Math.Round(convertToInvoicePer, 0);
                }
                if (lostEstimates > 0)
                {
                    salesFunnelNationalData.LostEstimateCount = lostEstimates;
                }
                if (lostJobs > 0)
                {
                    salesFunnelNationalData.LostJobsCount = lostJobs;
                }
                salesFunnelList.Add(salesFunnelNationalData);
            }

            return salesFunnelList;
        }
    }
}
