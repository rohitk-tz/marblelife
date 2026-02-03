using Core.Application;
using Core.Application.Attribute;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class RoyaltyReportService : IRoyaltyReportService
    {
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRoyaltyReportFactory _royaltyReportFactory;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<MarketingClass> _marketingClassRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        private readonly IRoyaltyFeeSlabsFactory _royaltyFeeSlabFactory;

        public RoyaltyReportService(IUnitOfWork unitOfWork, IRoyaltyReportFactory royaltyReportFactory, IRoyaltyFeeSlabsFactory royaltyFeeSlabFactory)
        {
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _marketingClassRepository = unitOfWork.Repository<MarketingClass>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _royaltyReportFactory = royaltyReportFactory;
            _royaltyFeeSlabFactory = royaltyFeeSlabFactory;
        }
        public RoyaltyReportListModel GetRoyaltyReport(long salesDataUploadId)
        {
            var salesData = _salesDataUploadRepository.Get(salesDataUploadId);
            var franchiseeSales = _franchiseeSalesRepository.Table.Where(x => x.SalesDataUpload.Id == salesData.Id).ToList();

            var collection = _royaltyReportFactory.CreateCollectionModel(franchiseeSales);

            var marketingClass = _royaltyReportFactory.GetMarketingClasses();
            var serviceTypes = _royaltyReportFactory.GetServices();
            var serviceTypeCategories = _royaltyReportFactory.GetServiceTypeCategory();
            decimal ytdSales = 0;

            var query = _franchiseeSalesRepository.Table.Where(x => x.SalesDataUpload.FranchiseeId == salesData.FranchiseeId
                                                   && x.Invoice.GeneratedOn < salesData.PeriodStartDate
                                                   && x.Invoice.GeneratedOn.Year == salesData.PeriodStartDate.Year).SelectMany(x => x.Invoice.InvoiceItems);

            if (query.Any())
                ytdSales = query.Sum(m => m.Amount);

            var list = new RoyaltyReportListModel
            {
                FranchiseeId = salesData.Franchisee.Id,
                FranchiseeeName = salesData.Franchisee.Organization.Name,
                StartDate = salesData.PeriodStartDate,
                EndDate = salesData.PeriodEndDate,
                YTDSales = ytdSales,
                Collection = collection,
                Classes = marketingClass,
                Services = serviceTypes,
                ServiceTypeCategories = serviceTypeCategories,
                RoyaltyFeeSlabs = salesData.Franchisee.FeeProfile.RoyaltyFeeSlabs.Select(_royaltyFeeSlabFactory.CreateEditModel),
                CurrencyCode = salesData.Franchisee.Currency,
                CurrencyRate = salesData.CurrencyExchangeRate.Rate
            };
            return list;
        }
    }
}
