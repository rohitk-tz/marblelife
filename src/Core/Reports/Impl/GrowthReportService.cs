using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Users.Enum;
using System.Collections.Generic;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class GrowthReportService : IGrowthReportService
    {
        private readonly IRepository<FranchiseeSalesInfo> _franchiseeSalesInfoRepository;
        private readonly IFranchiseeSalesInfoReportFactory _franchiseeSalesInfoFactory;
        private readonly ISortingHelper _sortingHelper;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IFileService _fileService;
        private readonly IClock _clock;
        private readonly IRepository<Franchisee> _franchiseeRepository;

        public GrowthReportService(IUnitOfWork unitOfWork, IFranchiseeSalesInfoReportFactory franchiseeSalesInfoFactory, ISortingHelper sortingHelper,
            IExcelFileCreator excelFileCreator, IFileService fileService, IClock clock)
        {
            _franchiseeSalesInfoRepository = unitOfWork.Repository<FranchiseeSalesInfo>();
            _franchiseeSalesInfoFactory = franchiseeSalesInfoFactory;
            _sortingHelper = sortingHelper;
            _excelFileCreator = excelFileCreator;
            _fileService = fileService;
            _clock = clock;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
        }

        public GrowthReportListModel GetGrowthReport(GrowthReportFilter filter, int pageNumber, int pageSize)
        {
            IList<FranchiseeSalesInfoList> franchiseeSalesInfo = GrowthReportFilterList(filter);

            var list = franchiseeSalesInfo.Select(x => _franchiseeSalesInfoFactory.CreateViewModel(x, filter)).OrderByDescending(x => x.AverageGrowth).ToList();

            var finalcollection = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new GrowthReportListModel
            {
                Collection = finalcollection,
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, list.Count())
            };
        }

        private IList<FranchiseeSalesInfoList> GrowthReportFilterList(GrowthReportFilter filter)
        {
            if (filter.Year <= 0)
                filter.Year = _clock.UtcNow.Year;

            var salesInfo = _franchiseeSalesInfoRepository.Table.Where(x => (filter.FranchiseeId <= 1 || x.FranchiseeId == filter.FranchiseeId)
                               && (x.Year == filter.Year || x.Year == (filter.Year - 1)) && (filter.ClassTypeId <= 0 || x.ClassTypeId == filter.ClassTypeId)
                               && (filter.ServiceTypeId <= 0 || x.ServiceTypeId == filter.ServiceTypeId));

            var franchiseeList = _franchiseeRepository.Table.Where(x => (filter.FranchiseeId <= 1 || x.Id == filter.FranchiseeId)).ToList();

            var list = new List<FranchiseeSalesInfoList>();
            foreach (var franchisee in franchiseeList)
            {
                var model = new FranchiseeSalesInfoList
                {
                    FranchiseeId = franchisee.Id,
                    Franchisee = franchisee.Organization.Name,
                    FranchiseeSalesInfo = salesInfo.Where(x => x.FranchiseeId == franchisee.Id),
                    CurrencyCode = franchisee.Currency,
                };

                if (model.FranchiseeSalesInfo.Any())
                    list.Add(model);
            }

            return list;
        }

        public bool DownloadGrowthReport(GrowthReportFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<GrowthReportViewModel>();
            IList<FranchiseeSalesInfoList> franchiseeInvoiceList = GrowthReportFilterList(filter).ToList();

            //prepare item collection
            foreach (var item in franchiseeInvoiceList)
            {
                var model = _franchiseeSalesInfoFactory.CreateViewModel(item, filter);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/growthReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);
        }
    }
}
