using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Billing.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesInvoiceService : ISalesInvoiceService
    {
        private readonly ISortingHelper _sortingHelper;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        private readonly ISalesInvoiceFactory _salesInvoicefactory;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IFileService _fileService;
        private readonly IRepository<InvoiceFileUpload> _invoiceFileUploadRepository;

        public SalesInvoiceService(IUnitOfWork unitOfWork, ISortingHelper sortingHelper, IExcelFileCreator excelFileCreator, IClock clock,
            ISalesInvoiceFactory salesInvoiceFactory, IFranchiseeSalesService franchiseeSalesService, IFileService fileService)
        {
            _sortingHelper = sortingHelper;
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _salesInvoicefactory = salesInvoiceFactory;
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _franchiseeSalesService = franchiseeSalesService;
            _fileService = fileService;
            _invoiceFileUploadRepository = unitOfWork.Repository<InvoiceFileUpload>();
        }
        public bool DownloadInvoiceFile(SalesDataListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            var invoiceCollection = new List<SalesInvoiceViewModel>();
            IQueryable<FranchiseeSales> franchiseeSales = _franchiseeSalesService.GetSaleFilterData(filter);
            var invoiceList = franchiseeSales.Where(x => x.Invoice != null).SelectMany(fs => fs.Invoice.InvoiceItems).ToList();

            //prepare item collection
            foreach (var item in invoiceList)
            {
                var model = _salesInvoicefactory.CreateViewModel(item);
                invoiceCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/sales_InvoiceData-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(invoiceCollection, fileName);
        }

        public void Save(CustomerFileUploadCreateModel model)
        {
            var customerFileUpload = _salesInvoicefactory.CreatDoamin(model);
            var file = _fileService.SaveModel(model.File);
            customerFileUpload.FileId = file.Id;
            _invoiceFileUploadRepository.Save(customerFileUpload);
        }
    }
}
