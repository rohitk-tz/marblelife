using Core.Organizations.Domain;
using Core.Organizations.ViewModels;
using Core.Application.Attribute;
using Core.Application;
using Core.Sales.ViewModel;
using System.Linq;
using Core.Application.ViewModel;
using Core.Users.Enum;
using Core.Application.Impl;
using System.Collections.Generic;
using System;
using Core.Organizations.ViewModel;
using Core.Sales.Domain;
using Core.Billing.Domain;
using Core.Scheduler.ViewModel;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeSalesService : IFranchiseeSalesService
    {
        private readonly IRepository<FranchiseeSales> _franchiseeSalesrepository;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentrepository;
        private readonly IFranchiseeSalesFactory _franchiseeSalesFactory;
        private readonly ISortingHelper _sortingHelper;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IRepository<UpdateMarketingClassfileupload> _updateMarketingClassrepository;
        private readonly IRepository<InvoiceItem> _invoiceItemrepository;
        private readonly IRepository<Invoice> _invoicerepository;
        private readonly IFileService _fileService;
        private readonly IRepository<UpdateMarketingClassfileupload> _updateMarketingClassfileuploadrepository;
        public FranchiseeSalesService(IUnitOfWork unitOfWork, IFranchiseeSalesFactory franchiseeSalesFactory,
            ISortingHelper sortingHelper, IExcelFileCreator excelFileCreator, IFileService fileService)
        {
            _franchiseeSalesrepository = unitOfWork.Repository<FranchiseeSales>();
            _franchiseeSalesPaymentrepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _sortingHelper = sortingHelper;
            _franchiseeSalesFactory = franchiseeSalesFactory;
            _excelFileCreator = excelFileCreator;
            _updateMarketingClassrepository = unitOfWork.Repository<UpdateMarketingClassfileupload>();
            _invoiceItemrepository = unitOfWork.Repository<InvoiceItem>();
            _invoicerepository = unitOfWork.Repository<Invoice>();
            _fileService = fileService;
            _updateMarketingClassfileuploadrepository= unitOfWork.Repository<UpdateMarketingClassfileupload>();
        }
        public FranchiseeSales Save(FranchiseeSalesEditModel model)
        {
            var franchiseeSales = _franchiseeSalesFactory.CreateDomain(model);
            _franchiseeSalesrepository.Save(franchiseeSales);
            return franchiseeSales;
        }

        public FranchiseeSales Get(string qbInvoiceNumber, long franchiseeId, string customerName)
        {
            var franchiseeSalesList = _franchiseeSalesrepository.Fetch(x => x.QbInvoiceNumber.Equals(qbInvoiceNumber) && x.FranchiseeId == franchiseeId).ToArray();
            if (!franchiseeSalesList.Any())
                return null;

            var name = customerName.Split(new[] { ' ', ',' }).OrderBy(x => x);
            var fullName = string.Join(" ", name);

            foreach (var item in franchiseeSalesList)
            {
                var inDBCustoerName = item.Customer.Name.Split(new[] { ' ', ',' }).OrderBy(x => x);
                var fullNameInDb = string.Join(" ", inDBCustoerName);
                if (fullName.Equals(fullNameInDb))
                    return item;
                else continue;
            }
            return null;
        }

        public SalesDataListViewModel GetSalesData(SalesDataListFilter filter, int pageNumber, int pageSize)
        {
            IQueryable<FranchiseeSales> collection = GetSaleFilterData(filter);
            var query = collection.Where(x => x.Invoice != null);

            var count = query.Count();
            var finalCollection = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new SalesDataListViewModel()
            {
                Collection = finalCollection.Select(_franchiseeSalesFactory.CreateViewModel).ToList(),
                PagingModel = new PagingModel(pageNumber, pageSize, count),
                Filter = filter
            };
        }

        public IQueryable<FranchiseeSales> GetSaleFilterData(SalesDataListFilter filter)
        {
            var collection = _franchiseeSalesrepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                    && (filter.SalesDataUploadId < 1 || x.SalesDataUploadId == filter.SalesDataUploadId)
                    && (filter.CustomerId < 1 || x.Customer.Id == filter.CustomerId)
                    && (filter.MarketingClassId < 1 || (x.MarketingClass.Id == filter.MarketingClassId))
                    && (string.IsNullOrEmpty(filter.CustomerName) || (x.Customer.Name.Contains(filter.CustomerName)))
                    && (string.IsNullOrEmpty(filter.QbInvoiceNumber) || (x.QbInvoiceNumber.Equals(filter.QbInvoiceNumber)))
                    && (string.IsNullOrEmpty(filter.Text)
                    || (x.Customer.Name.Contains(filter.Text))
                    || (x.Franchisee.Organization.Name.Contains(filter.Text))
                    || (x.Customer.Address.AddressLine1.Contains(filter.Text))
                    || (x.Invoice.Id.ToString().Equals(filter.Text))
                    || (x.Customer.Address.AddressLine2.Contains(filter.Text)) || (x.Customer.Address.City.Name.Contains(filter.Text))
                    || (x.Customer.Address.State.Name.Contains(filter.Text)) || (x.Customer.Address.Zip.Code.Contains(filter.Text))) &&
                    ((filter.PeriodStartDate == null || x.Invoice.GeneratedOn >= filter.PeriodStartDate) && (filter.PeriodEndDate == null || x.Invoice.GeneratedOn <= filter.PeriodEndDate)));
            collection = _sortingHelper.ApplySorting(collection, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "InvoiceId":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Id, filter.SortingOrder);
                        break;
                    case "Name":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Customer.Name.ToString(), filter.SortingOrder);
                        break;
                    case "FranchiseeName":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "InvoiceDate":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Invoice.GeneratedOn, filter.SortingOrder);
                        break;
                    case "QBInvoice":
                        collection = _sortingHelper.ApplySorting(collection, x => x.QbInvoiceNumber, filter.SortingOrder);
                        break;
                    case "StreetAddress":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Customer.Address.AddressLine1, filter.SortingOrder);
                        break;
                    case "City":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Customer.Address.City.Name, filter.SortingOrder);
                        break;
                    case "State":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Customer.Address.State.Name, filter.SortingOrder);
                        break;
                    case "ZipCode":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Customer.Address.ZipCode, filter.SortingOrder);
                        break;
                    case "Country":
                        collection = _sortingHelper.ApplySorting(collection, x => x.Customer.Address.Country.Name, filter.SortingOrder);
                        break;
                    case "TotalAmount":
                        collection = _sortingHelper.ApplySorting(collection, x => x.SalesDataUpload.TotalAmount, filter.SortingOrder);
                        break;
                    case "PaidAmount":
                        collection = _sortingHelper.ApplySorting(collection, x => x.SalesDataUpload.PaidAmount, filter.SortingOrder);
                        break;
                    case "MarketingClass":
                        collection = _sortingHelper.ApplySorting(collection, x => x.MarketingClass.Name, filter.SortingOrder);
                        break;

                }
            }
            return collection;
        }

        public FranchiseeSales GetLastInvoiceDetails(long customerId)
        {
            return _franchiseeSalesrepository.Table.Where(x => x.CustomerId == customerId && x.Invoice != null).OrderByDescending(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();
        }

        public decimal GetSalesOfCustomer(long customerId)
        {
            var invoiceIds = _franchiseeSalesrepository.Table.Where(x => x.CustomerId == customerId && x.Invoice != null).Select(y => y.InvoiceId);
            var payments = _franchiseeSalesPaymentrepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)
                                && x.Payment != null).Select(y => y.Payment).ToList();

            return payments.Sum(p => p.Amount);
        }

        public FranchiseeSales GetFranchiseeSalesByInvoiceId(long invoiceId)
        {
            return _franchiseeSalesrepository.IncludeMultiple(x => x.MarketingClass, x1 => x1.CurrencyExchangeRate).Where(x => x.Invoice != null && x.InvoiceId == invoiceId).FirstOrDefault();
        }

        public bool DownloadSalesDataFile(SalesDataListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            var invoiceCollection = new List<FranchiseeSalesViewModel>();
            IQueryable<FranchiseeSales> invoices = GetSaleFilterData(filter);
            // var franchiseeSalesList = _franchiseeSalesrepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId.Value)).ToList();
            var saleData = invoices.Where(x => x.Invoice != null).ToList();
            //prepare item collection
            foreach (var item in saleData)
            {
                if (item != null && item.Customer != null && item.Customer.Address != null)
                {
                    var model = _franchiseeSalesFactory.CreateViewModel(item);
                    invoiceCollection.Add(model);
                }
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/salesData-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(invoiceCollection, fileName);
        }

        public FranchiseeSales GetCustomerLastFranchisee(long customerId)
        {
            var franchiseeDetail = _franchiseeSalesrepository.Table.Where(x => x.CustomerId == customerId).OrderByDescending(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();
            return franchiseeDetail;
        }

        public UpdateMarketingClassListModel UpdateSalesData(UpdateMarketingClassInfoListFilter filter)
        {
            var startDate = new DateTime();
            var endDate = new DateTime();

            if (filter.PeriodEndDate == null)
            {
                startDate = new DateTime(filter.Year, 1, 01);
                endDate = new DateTime(filter.Year, 12, 31);
            }
            else
            {
                startDate = filter.PeriodStartDate.Value;
                endDate = filter.PeriodEndDate.Value;
            }

            var invoiceIdList = _invoicerepository.Table.Where(x => (x.GeneratedOn >= startDate &&
               x.GeneratedOn <= endDate)).OrderByDescending(x=>x.Id).Select(x => x.Id).ToList();
            var invoiceItemsList = _invoiceItemrepository.Table.Where(x => invoiceIdList.Contains(x.InvoiceId) && x.ItemTypeId == 91).OrderByDescending(x=>x.Id).AsQueryable();
            var franchiseeSales = _franchiseeSalesrepository.Table.Where(x => invoiceIdList.Contains(x.InvoiceId.Value)).ToList();

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        invoiceItemsList = _sortingHelper.ApplySorting(invoiceItemsList, x => x.Id, filter.SortingOrder);
                        break;
                    case "GeneratedOn":
                        invoiceItemsList = _sortingHelper.ApplySorting(invoiceItemsList, x => x.Invoice.GeneratedOn, filter.SortingOrder);
                        break;
                    case "InvoiceId":
                        invoiceItemsList = _sortingHelper.ApplySorting(invoiceItemsList, x => x.InvoiceId, filter.SortingOrder);
                        break;

                }
            }

            var count = invoiceItemsList.Count();
            var finalCollection = invoiceItemsList.ToList();
            if (!filter.IsFromDownload)
            {
                finalCollection = invoiceItemsList.ToList().Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
            }
            
            return new UpdateMarketingClassListModel()
            {
                Collection = finalCollection.Select(x => _franchiseeSalesFactory.CreateDomainInvoiceModel(x, franchiseeSales.FirstOrDefault(x1 => x1.InvoiceId == x.InvoiceId))).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(filter.PageNumber, filter.PageSize, count),
            };
        }

        public bool DownloadInvoiceAllList(UpdateMarketingClassInfoListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            var invoiceCollection = new List<DownloadAllInvoiceModel>();
            var franchiseeInvoices = UpdateSalesData(filter);

            //prepare item collection
            foreach (var item in franchiseeInvoices.Collection)
            {
                var model = _franchiseeSalesFactory.CreateDomainDownloadInvoiceModel(item);
                invoiceCollection.Add(model);
            }

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/SalesData-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(invoiceCollection, fileName);
        }

        public void SaveFile(CustomerFileUploadCreateModel model)
        {
            var customerFileUpload = _franchiseeSalesFactory.CreateViewModel(model);
            var file = _fileService.SaveModel(model.File);
            customerFileUpload.FileId = file.Id;
            _updateMarketingClassrepository.Save(customerFileUpload);
        }

        public ZipDataUploadListModel GetUpdateSalesList(ZipDataListFilter filter, int pageNumber, int pageSize)
        {
            var zipData = _updateMarketingClassfileuploadrepository.Table.Where(x => (filter.StatusId < 1 || x.StatusId == filter.StatusId)
                                  && (string.IsNullOrEmpty(filter.text) || ((x.Id.ToString().Equals(filter.text)))));
            zipData = _sortingHelper.ApplySorting(zipData, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        zipData = _sortingHelper.ApplySorting(zipData, x => x.Id, filter.SortingOrder);
                        break;
                    case "Status":
                        zipData = _sortingHelper.ApplySorting(zipData, x => x.Lookup.Name, filter.SortingOrder);
                        break;
                    case "UploadedOn":
                        zipData = _sortingHelper.ApplySorting(zipData, x => x.DataRecorderMetaData.DateCreated, filter.SortingOrder);
                        break;
                }
            }

            var list = zipData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new ZipDataUploadListModel()
            {
                Collection = list.Select(_franchiseeSalesFactory.CreateViewModel).ToList(),
                PagingModel = new PagingModel(pageNumber, pageSize, zipData.Count()),
                Filter = filter
            };
        }

        public long GetTotalNumberOfInvoices(long customerId)
        {
            return _franchiseeSalesrepository.Table.Where(x => x.CustomerId == customerId && x.Invoice != null).OrderByDescending(y => y.DataRecorderMetaData.DateCreated).Count();
        }
    }
}
