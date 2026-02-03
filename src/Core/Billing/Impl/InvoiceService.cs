using System;
using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Application;
using Core.Application.Attribute;
using Core.Sales.Domain;
using Core.Organizations;
using Core.Geo.Impl;
using System.Linq;
using Core.Organizations.Domain;
using Core.Geo;
using Core.Application.ViewModel;
using Core.Users.Enum;
using Core.Application.Impl;
using System.Collections.Generic;
using Core.Billing.Enum;
using Ionic.Zip;
using Core.Sales;
using System.Data;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using Core.Sales.ViewModel;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class InvoiceService : IInvoiceService
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IInvoiceFactory _invoiceFactory;
        private readonly AddressFactory _addressFactory;
        private readonly IPhoneFactory _phoneFactory;
        private readonly InvoiceItemFactory _invoiceItemFactory;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesrepository;
        private readonly IFranchiseeSalesPaymentService _paymentService;
        private readonly IPaymentFactory _paymentFactory;

        private readonly IRepository<InvoicePayment> _invoicePaymentRepository;
        private readonly IRepository<FranchiseeLoanSchedule> _franchiseeLoanSchedulerRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<ServiceFeeInvoiceItem> _serviceFeeInvoiceItemRepository;
        private readonly IInvoicePaymentService _invoicePaymentService;

        private readonly IRepository<InvoiceItem> _invoiceItemRepositiory;
        private readonly IRepository<LateFeeInvoiceItem> _latefeeInvoiceItemRepository;
        private readonly IRepository<InterestRateInvoiceItem> _interestRateInvoiceItemRepository;
        private readonly IRepository<RoyaltyInvoiceItem> _royaltyInvoiceItemRepository;
        private readonly IDownloadFileHelperService _downloadFileHelperService;
        private readonly IUnitOfWork _unitoOfWork;
        private readonly IClock _clock;
        private readonly IEmailFactory _emailFactory;
        private readonly IRepository<OneTimeProjectFee> _oneTimeProjectFeeRepository;
        private readonly IRepository<InvoiceAddress> _invoiceAddressRepository;

        private readonly IRepository<FranchiseeAccountCredit> _franchiseeAccountCreditRepository;
        public InvoiceService(IUnitOfWork unitOfWork, IInvoiceFactory invoiceFactory, IFranchiseeSalesService franchiseeSalesService, AddressFactory addressFactory,
            InvoiceItemFactory invoiceItemFactory, IPhoneFactory phoneFactory, IPaymentFactory paymentFactory,
            ISortingHelper sortingHelper, IExcelFileCreator excelFileCreator, IFranchiseeSalesPaymentService paymentService, IInvoicePaymentService invoicePaymentService,
            IDownloadFileHelperService downloadFileHelperService, IClock clock, IEmailFactory emailFactory)
        {
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _invoiceFactory = invoiceFactory;
            _customerRepository = unitOfWork.Repository<Customer>();
            _franchiseeSalesService = franchiseeSalesService;
            _addressFactory = addressFactory;
            _invoiceItemFactory = invoiceItemFactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _phoneFactory = phoneFactory;
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _sortingHelper = sortingHelper;
            _excelFileCreator = excelFileCreator;
            _franchiseeSalesrepository = unitOfWork.Repository<FranchiseeSales>();
            _paymentService = paymentService;
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
            _paymentRepository = unitOfWork.Repository<Payment>();
            _invoicePaymentService = invoicePaymentService;
            _paymentFactory = paymentFactory;
            _serviceFeeInvoiceItemRepository = unitOfWork.Repository<ServiceFeeInvoiceItem>();
            _invoiceItemRepositiory = unitOfWork.Repository<InvoiceItem>();
            _latefeeInvoiceItemRepository = unitOfWork.Repository<LateFeeInvoiceItem>();
            _interestRateInvoiceItemRepository = unitOfWork.Repository<InterestRateInvoiceItem>();
            _royaltyInvoiceItemRepository = unitOfWork.Repository<RoyaltyInvoiceItem>();
            _downloadFileHelperService = downloadFileHelperService;
            _unitoOfWork = unitOfWork;
            _clock = clock;
            _emailFactory = emailFactory;
            _oneTimeProjectFeeRepository = unitOfWork.Repository<OneTimeProjectFee>();
            _invoiceAddressRepository = unitOfWork.Repository<InvoiceAddress>();
            _franchiseeLoanSchedulerRepository = unitOfWork.Repository<FranchiseeLoanSchedule>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _franchiseeAccountCreditRepository = unitOfWork.Repository<FranchiseeAccountCredit>();
        }

        public Invoice Save(InvoiceEditModel model)
        {
            var invoice = _invoiceFactory.CreateDomain(model);

            _invoiceRepository.Save(invoice);

            foreach (var item in model.Payments)
            {
                var payment = _paymentService.Save(item);
                _invoicePaymentService.Save(invoice.Id, payment.Id);
            }
            return invoice;
        }

        public void SavePaymentItem(Invoice inDb, FranchiseeSalesPaymentEditModel model)
        {
            //_invoiceFactory.AddPaymentItemToInvoice(inDb, model);
            //_invoiceRepository.Save(inDb);
            var payment = _paymentService.Save(model);
            _invoicePaymentService.Save(inDb.Id, payment.Id);
        }

        public InvoiceDetailsViewModel InvoiceDetails(long invoiceId)
        {
            string[] stringSeparators = new string[] { "2018", "2020" };
            string[] firstNames = invoiceId.ToString().Split(stringSeparators, StringSplitOptions.None);
            if (firstNames.Length > 1)
            {
                invoiceId = Convert.ToInt64(firstNames[1]);
            }
            var franchiseeSalesData = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(invoiceId);
            var franchiseeDetails = _franchiseeRepository.Get(franchiseeSalesData.FranchiseeId);
            var customerInfo = franchiseeSalesData.Customer;
            var customerAddress = _invoiceAddressRepository.Table.Where(x => x.InvoiceId == invoiceId).FirstOrDefault();

            return new InvoiceDetailsViewModel
            {
                //InvoiceId = franchiseeSalesData.CustomerInvoiceId > 0 ? franchiseeSalesData.CustomerInvoiceId : franchiseeSalesData.InvoiceId,
                //InvoiceId =  franchiseeSalesData.InvoiceId,
                InvoiceId = franchiseeSalesData.InvoiceId,
                Email = franchiseeDetails.Organization.Email,
                Customer = franchiseeSalesData.Customer.Name,
                ContactPerson = franchiseeSalesData.Customer.ContactPerson,
                FranchiseeName = franchiseeDetails.Organization.Name,
                //QBInvoiceNumber = franchiseeSalesData.QbInvoiceNumber,
                QBInvoiceNumber = franchiseeSalesData.CustomerQbInvoiceId > 0 ? franchiseeSalesData.CustomerQbInvoiceId.ToString() : franchiseeSalesData.QbInvoiceNumber,
                TotalAmount = franchiseeSalesData.Invoice.InvoiceItems.ToList().Sum(x => x.Amount),
                TotalPayment = franchiseeSalesData.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount),
                GrandTotal = (franchiseeSalesData.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) - franchiseeSalesData.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)),
                Address = _addressFactory.CreateViewModel(customerAddress),
                Payments = _paymentService.CreatePaymentModelCollection(franchiseeSalesData.Invoice.InvoicePayments),
                InvoiceItems = franchiseeSalesData.Invoice.InvoiceItems.Select(x => _invoiceItemFactory.CreateViewModel(x)).ToList(),
                GeneratedOn = franchiseeSalesData.Invoice.GeneratedOn,
                PhoneNumbers = customerAddress != null ? franchiseeDetails.Organization.Phones.Select(x => _phoneFactory.CreateEditModel(x, customerAddress.Phone)).ToList() : franchiseeDetails.Organization.Phones.Select(x => _phoneFactory.CreateEditModel(x)).ToList(),
                CurrencyCode = franchiseeDetails.Currency,
                PhoneNumber = franchiseeSalesData.Customer.Phone,
                FranchiseeAddress = franchiseeDetails.Organization != null ? _addressFactory.CreateViewModelForFranchisee(franchiseeDetails.Organization.Address.FirstOrDefault()) : null,
                CustomerEmails = customerAddress != null ? customerInfo.CustomerEmails.Select(x => _emailFactory.CreateEditModel(x, customerAddress.EmailId)).ToList() : customerInfo.CustomerEmails.Select(x => _emailFactory.CreateEditModel(x)).ToList(),
                FranchiseePhone = franchiseeDetails.Organization != null && franchiseeDetails.Organization.Phones.Count() > 1 ? franchiseeDetails.Organization.Phones.Where(x => x.TypeId == 1).Select(x => x.Number).FirstOrDefault() : ""
            };
        }

        public InvoiceListModel GetInvoiceList(InvoiceListFilter filter, int pageNumber, int pageSize)
        {
            var totalUnpaidAmount = default(decimal?);
            
            IQueryable<FranchiseeInvoice> invoiceList = InvoiceFilterList(filter);
            //invoiceList = invoiceList.Where(x => x.InvoiceId == 204626).AsQueryable();
            if (invoiceList.Count() > 0)
            {
                var invoiceIdList = invoiceList.Select(x => x.InvoiceId).ToList();
                totalUnpaidAmount = GetTotalUnPaidAmount(invoiceIdList);
            }
            else
            {
                totalUnpaidAmount = 0;
            }
            var franchiseeName = filter.FranchiseeId > 1 ? _organizationRepository.Table.FirstOrDefault(x => x.Id == filter.FranchiseeId).Name : "";

            var finalcollection = invoiceList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var collection = finalcollection.Select(_invoiceFactory.CreateViewModel).ToList();


            return new InvoiceListModel
            {
                Collection = collection,
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, invoiceList.Count()),
                TotalUnPaidAmount = totalUnpaidAmount,
                FranchiseeName = franchiseeName
            };
        }

        public InvoiceListModel GetDownloadedInvoiceList(long[] invoiceIds)
        {
            var invoiceList = _franchiseeInvoiceRepository.Table.Where(x => (invoiceIds.Contains(x.InvoiceId)))
                .OrderByDescending(x => x.Invoice.DataRecorderMetaData.DateCreated).ToList();

            return new InvoiceListModel
            {
                Collection = invoiceList.Select(_invoiceFactory.CreateViewModel).ToList(),
            };
        }



        private IQueryable<FranchiseeInvoice> InvoiceFilterList(InvoiceListFilter filter)
        {
            var invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                && (filter.SalesDataUploadId < 1 || x.SalesDataUploadId == filter.SalesDataUploadId)
                && (x.Invoice != null && x.Invoice.InvoiceItems.Any())
                && (filter.StatusId < 1 || (x.Invoice.Lookup.Id == filter.StatusId))
                && (string.IsNullOrEmpty(filter.Text) || ((x.Franchisee.Organization.Name.Contains(filter.Text))
                || (x.InvoiceId.ToString().Equals(filter.Text))))
                && (filter.DueDateStart == null || x.Invoice.DueDate >= filter.DueDateStart)
                && (filter.DueDateEnd == null || x.Invoice.DueDate <= filter.DueDateEnd)
                && (filter.LateFeeTypeId < 1 || (x.Invoice.InvoiceItems.Any(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.LateFeeTypeId == filter.LateFeeTypeId)))
                && (filter.PeriodStartDate == null
                || (x.SalesDataUpload != null ? x.SalesDataUpload.PeriodEndDate >= filter.PeriodStartDate
                : x.Invoice.InvoiceItems.Any(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.EndDate >= filter.PeriodStartDate)))
                && (filter.PeriodEndDate == null
                || (x.SalesDataUpload != null ? x.SalesDataUpload.PeriodEndDate <= filter.PeriodEndDate
                : x.Invoice.InvoiceItems.Any(y => y.LateFeeInvoiceItem != null && y.LateFeeInvoiceItem.EndDate <= filter.PeriodEndDate)))
                && (filter.PaymentDateStart == null || x.Invoice.InvoicePayments.Any(y => y.Payment.Date >= filter.PaymentDateStart))
                && (filter.PaymentDateEnd == null || x.Invoice.InvoicePayments.Any(y => y.Payment.Date <= filter.PaymentDateEnd)));

            if(filter.UndownloadedInvoice == "undownloadedInvoice")
            {
                invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                && (x.IsDownloaded.Equals(false)));
            }
            //if(filter.Accounting > 0)
            //{
            //    invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
            //    x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
            //    .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
            //    && (x.Invoice.InvoiceItems.Where(y => y.Description.Contains("for Book-keeping") || y.Description.Contains("for Book-keeping(var)")).Sum(a => a.Amount) == filter.Accounting));
            //}
            if (filter.LoanAndLoanInt > 0)
            {
                invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                && (x.Invoice.InvoiceItems.Where(y => y.Description.Contains("GEOFencing") || y.Description.Contains("Uncategorized Income") || y.Description.Contains("SURGICAL STRIKE Marble Home Lst")).Sum(a => a.Amount) == filter.LoanAndLoanInt));
            }
            if (filter.ISQFT > 0)
            {
                invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                && (x.Invoice.InvoiceItems.Where(y => y.Description.Contains("ISQFT")).Sum(a => a.Amount) == filter.ISQFT));
            }
            if (filter.BackUpCharges > 0)
            {
                invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                && (x.Invoice.InvoiceItems.Where(y => y.Description.Contains("(See Phone data for call details)")).Sum(a => a.Amount) == filter.BackUpCharges));
            }
            if (filter.WebSEO > 0)
            {
                invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                && (x.Invoice.InvoiceItems.Where(y => y.Description.Contains("for SEO Charges")).Sum(a => a.Amount) == filter.WebSEO));
            }
            if (filter.IsAdfund.HasValue && filter.IsAdfund.Value)
            {
                invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                 && (x.Invoice.InvoiceItems.Any(y => y.ItemTypeId == (long)InvoiceItemType.AdFund)));
            }
            if (filter.IsRoyality.HasValue && filter.IsRoyality.Value)
            {
                invoiceList = _franchiseeInvoiceRepository.IncludeMultiple(x => x.Franchisee, x => x.Franchisee.Organization, x => x.Invoice,
                x => x.Invoice.InvoiceItems, x => x.SalesDataUpload, x => x.Invoice.InvoicePayments, x => x.Invoice.DataRecorderMetaData, x => x.Invoice.Lookup)
                .Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                 && (x.Invoice.InvoiceItems.Any(y => y.ItemTypeId == (long)InvoiceItemType.RoyaltyFee)));
            }

            invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc).OrderByDescending(x => x.Id);

            if (!string.IsNullOrEmpty(filter.SortingColumn))
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.InvoiceId, filter.SortingOrder);
                        break;
                    case "Name":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "StartDate":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.SalesDataUpload.PeriodStartDate, filter.SortingOrder);
                        break;
                    case "EndDate":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.SalesDataUpload.PeriodEndDate, filter.SortingOrder);
                        break;
                    case "DueDate":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.DueDate, filter.SortingOrder);
                        break;
                    case "UploadedOn":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.SalesDataUpload.DataRecorderMetaData.DateCreated, filter.SortingOrder);
                        break;
                    case "TotalSales":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.SalesDataUpload.TotalAmount, filter.SortingOrder);
                        break;
                    case "PaidAmount":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.SalesDataUpload.PaidAmount, filter.SortingOrder);
                        break;
                    case "Status":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.Lookup.Name, filter.SortingOrder);
                        break;
                    case "IsDownloaded":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.IsDownloaded, filter.SortingOrder);
                        break;
                    case "PaymentDate":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.InvoicePayments.FirstOrDefault().Payment.Date, filter.SortingOrder);
                        break;
                    case "SEOCost":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.InvoiceItems.
                        FirstOrDefault(y => y.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges)
                        .ServiceFeeInvoiceItem.ServiceFeeTypeId, filter.SortingOrder);
                        break;
                    case "LoanAndLoanInt":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.InvoiceItems.Where(y => y.ItemTypeId == (long)InvoiceItemType.LoanServiceFee).FirstOrDefault().Amount, filter.SortingOrder);
                        break;
                    case "ISQFT":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.InvoiceItems.Where(y => y.ItemTypeId == (long)InvoiceItemType.LoanServiceFee && y.Description.Contains("ISQFT")).FirstOrDefault().Amount, filter.SortingOrder);
                        break;
                    case "WebSEO":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.InvoiceItems.Where(y => y.ItemTypeId == (long)InvoiceItemType.ServiceFee && y.Description.Contains("SEO Charges")).FirstOrDefault().Amount, filter.SortingOrder);
                        break;
                    case "BackUpPhone":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.InvoiceItems.Where(y => y.ItemTypeId == (long)InvoiceItemType.ServiceFee && y.Description.Contains("(See Phone data for call details)")).FirstOrDefault().Amount, filter.SortingOrder);
                        break;
                    case "AdFundOrRoyalty":
                        invoiceList = _sortingHelper.ApplySorting(invoiceList, x => x.Invoice.InvoiceItems.Any(y => y.ItemTypeId == (long)InvoiceItemType.AdFund), filter.SortingOrder);
                        break;
                }
            }
            return invoiceList;
        }

        public InvoiceDetailsViewModel FranchiseeInvoiceDetails(long invoiceId)
        {
            var loanAmountRemaining = default(decimal);
            var franchiseeInvoiceData = _franchiseeInvoiceRepository.Table.Where(x => x.InvoiceId == invoiceId).FirstOrDefault();
            var invoiceItemsList = franchiseeInvoiceData.Invoice.InvoiceItems.Select(x => x.Id).ToList();

            var currentDate = _clock.ToUtc(DateTime.Now);
            var franchiseeLoanScheduler = _franchiseeLoanSchedulerRepository.Table.Where(x => invoiceItemsList.Contains(x.InvoiceItemId.Value)).ToList();
            if (franchiseeLoanScheduler.Count() > 0)
            {
                var franchiseeLoan = franchiseeLoanScheduler.LastOrDefault();
                if (franchiseeLoan != null)
                {
                    loanAmountRemaining = franchiseeLoan.Balance;
                }
                else
                {
                    loanAmountRemaining = default(decimal);
                }
            }
            if (franchiseeInvoiceData == null)
                return new InvoiceDetailsViewModel { };

            var endDate = _clock.UtcNow;
            if (franchiseeInvoiceData.Invoice.InvoiceItems.FirstOrDefault().ItemTypeId == (long)InvoiceItemType.LateFees)
            {
                endDate = franchiseeInvoiceData.Invoice.InvoiceItems.OrderByDescending(x => x.LateFeeInvoiceItem.EndDate).Select(x => x.LateFeeInvoiceItem.EndDate).FirstOrDefault();
            }

            var invoiceDetail = new InvoiceDetailsViewModel
            {
                InvoiceId = franchiseeInvoiceData.InvoiceId,
                Email = franchiseeInvoiceData.Franchisee.Organization.Email,
                FranchiseeName = franchiseeInvoiceData.Franchisee.Organization.Name,
                TotalAmount = franchiseeInvoiceData.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount),
                TotalPayment = franchiseeInvoiceData.Invoice.InvoiceItems.ToList().Sum(x => x.Amount),
                GrandTotal = (franchiseeInvoiceData.Invoice.InvoiceItems.ToList().Sum(x => x.Amount) - franchiseeInvoiceData.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount)),
                Address = _addressFactory.CreateViewModel(franchiseeInvoiceData.Franchisee.Organization.Address.FirstOrDefault()),
                Payments = _paymentService.CreatePaymentModelCollection(franchiseeInvoiceData.Invoice.InvoicePayments), //franchiseeInvoiceData.Invoice.InvoicePayments.Select(x => _paymentFactory.CreateViewModel(x.Payment)).ToList(),
                InvoiceItems = franchiseeInvoiceData.Invoice.InvoiceItems.Select(x => _invoiceItemFactory.CreateViewModel(x)).ToList(),
                GeneratedOn = franchiseeInvoiceData.Invoice.GeneratedOn,
                DueDate = franchiseeInvoiceData.Invoice.DueDate,
                PhoneNumbers = franchiseeInvoiceData.Franchisee.Organization.Phones.Select(x => _phoneFactory.CreateEditModel(x)).ToList(),
                SalesAmount = franchiseeInvoiceData.SalesDataUpload != null ? franchiseeInvoiceData.SalesDataUpload.TotalAmount : 0,
                CurrencyCode = franchiseeInvoiceData.Franchisee.Currency,
                StatusId = franchiseeInvoiceData.Invoice.StatusId,
                InvoiceDate = franchiseeInvoiceData.InvoiceDate != null ? franchiseeInvoiceData.InvoiceDate : endDate,
                UploadEndDate = franchiseeInvoiceData.SalesDataUpload != null ? franchiseeInvoiceData.SalesDataUpload.PeriodEndDate : franchiseeInvoiceData.Invoice.GeneratedOn.Date,
                LoanAmount = franchiseeInvoiceData.Invoice.InvoiceItems.Where(x => x.ServiceFeeInvoiceItem != null
                                && x.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.Loan).ToList().Sum(x => x.Amount),
                RemainingLoanAmount = loanAmountRemaining
            };
            return invoiceDetail;
        }


        public bool CreateExcelForAllFiles(InvoiceListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            //string adFundInvoiceFile;
            //string royaltyInvoieFile;
            //string adFundPaymentFile;
            //string royaltyPaymentFile;

            //List<string> files = new List<string>();

            var franchiseeInvoices = InvoiceFilterList(filter).ToList();
            var invoiceIds = franchiseeInvoices.Select(x => x.InvoiceId).ToArray();

            var royaltyInvoiceList = _downloadFileHelperService.CreateDataForRoyaltyInvoiceFilter(franchiseeInvoices);
            var adFundInvoiceList = _downloadFileHelperService.CreateDataForAdFundInvoiceFilter(franchiseeInvoices);
            var royaltyPaymentList = _downloadFileHelperService.CreateDataForRoyaltyPaymentFilter(franchiseeInvoices);
            var adFundPaymentList = _downloadFileHelperService.CreateDataForAdFundPaymentFilter(franchiseeInvoices);

            var webSeoPaymentList = _downloadFileHelperService.CreateDataForWebSeoPaymentFilter(franchiseeInvoices);
            var webSeoInvoiceList = _downloadFileHelperService.CreateDataForWebSeoInvoiceFilter(franchiseeInvoices);

            var ds = new DataSet();
            ds.Tables.Add(_excelFileCreator.ListToDataTable(royaltyInvoiceList, "Royalty_invoice(s)"));
            ds.Tables.Add(_excelFileCreator.ListToDataTable(adFundInvoiceList, "AdFund_invoice(s)"));
            ds.Tables.Add(_excelFileCreator.ListToDataTable(webSeoInvoiceList, "WebSeo_invoice(s)"));

            ds.Tables.Add(_excelFileCreator.ListToDataTable(royaltyPaymentList, "Royalty_payment(s)"));
            ds.Tables.Add(_excelFileCreator.ListToDataTable(adFundPaymentList, "AdFund_Payment(s)"));
            ds.Tables.Add(_excelFileCreator.ListToDataTable(webSeoPaymentList, "WebSeo_Payment(s)"));

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/QB_Invoice_Payment-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(ds, fileName);

            //if (royaltyInvoiceResult && royaltyPaymentResult && adfundInvoiceResult && adFundPaymentResult)
            //{
            //    fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/invoice_payment-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".zip";
            //    ZipFile zip = new ZipFile();
            //    zip.AddFiles(files, false, "");
            //    zip.Save(fileName);
            //    return true;
            //}
            //return false;
        }

        public bool CreateExcelAdfund(long[] invoiceIds, out string fileName)
        {
            List<long> adfundInvoiceIds = new List<long>();
            fileName = string.Empty;
            var invoices = _invoiceRepository.Table.Where(x => invoiceIds.Contains(x.Id)).ToList();
            foreach (var invoiceId in invoiceIds)
            {
                var invoice = invoices.Where(x => x.Id == invoiceId).FirstOrDefault();
                var isInvoiceAddFund = invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.AdFund);
                if (isInvoiceAddFund)
                {
                    adfundInvoiceIds.Add(invoiceId);
                }
            }
            if (adfundInvoiceIds.Count > 0)
            {
                var adFundInvoiceList = _downloadFileHelperService.CreateDataForAdFundInvoice(adfundInvoiceIds.ToArray());
                var webSeoInvoiceList = _downloadFileHelperService.CreateDataForWebSeoInvoice(adfundInvoiceIds.ToArray());
                var finalInvoiceList = new List<DownloadInvoiceModel>();
                finalInvoiceList.AddRange(adFundInvoiceList);
                finalInvoiceList.AddRange(webSeoInvoiceList);
                if (finalInvoiceList.Count > 0)
                {
                    var ds = new DataTable();
                    ds = _excelFileCreator.ListToDataTable(finalInvoiceList);
                    fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/Adfund-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".csv";
                    _excelFileCreator.ToCSVWriter(finalInvoiceList, fileName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        public bool CreateExcelRoyality(long[] invoiceIds, out string fileName)
        {
            List<long> royaltyInvoiceIds = new List<long>();
            fileName = string.Empty;
            var invoices = _invoiceRepository.Table.Where(x => invoiceIds.Contains(x.Id)).ToList();
            foreach(var invoiceId in invoiceIds)
            {
                var invoice = invoices.Where(x => x.Id == invoiceId).FirstOrDefault();
                var isInvoiceAddFund = invoice.InvoiceItems.Any(x => x.ItemTypeId == (long)InvoiceItemType.AdFund);
                if (!isInvoiceAddFund)
                {
                    royaltyInvoiceIds.Add(invoiceId);
                }
            }
            if (royaltyInvoiceIds.Count > 0)
            {
                var royaltyInvoiceList = _downloadFileHelperService.CreateDataForRoyaltyInvoice(royaltyInvoiceIds.ToArray());
                var webSeoInvoiceList = _downloadFileHelperService.CreateDataForWebSeoRoyaltyInvoice(royaltyInvoiceIds.ToArray());
                var finalInvoiceList = new List<DownloadInvoiceModel>();
                finalInvoiceList.AddRange(royaltyInvoiceList);
                finalInvoiceList.AddRange(webSeoInvoiceList);
                if (finalInvoiceList.Count > 0)
                {
                    var ds = new DataTable();
                    ds = _excelFileCreator.ListToDataTable(finalInvoiceList);
                    fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/Royalty-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".csv";
                    _excelFileCreator.ToCSVWriter(finalInvoiceList, fileName);
                    return true;
                }
            }           
            return false;
        }

        public bool DownloadInvoiceListFile(long[] invoiceIds, out string fileName)
        {
            fileName = string.Empty;
            var invoiceCollection = new List<InvoiceViewModel>();
            var franchiseeInvoices = _franchiseeInvoiceRepository.Table.Where(x => invoiceIds.Contains(x.InvoiceId)).ToList();


            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                if (item.Invoice == null)
                {
                    continue;
                }
                var model = _invoiceFactory.CreateViewModel(item);
                invoiceCollection.Add(model);
            }
            invoiceCollection.Add(new InvoiceViewModel
            {

                AdFund = -1,
                Royalty = -1,
                PayableAmount = -1,
                FranchiseeName = "",
                InterestRate = -1,
                TotalSales = -1,
                AccountCredit = -1,
                InvoiceId = -1,
                SalesDataLateFee = -1,
                PaidAmount = -1,
                MinRoyaltyAmount = -1,
                AccruedAmount = -1,
                RoyaltyLateFee = -1,
                IsSEOCostApplied = null,
                FixedAccountingCharges = -1,
                VariableAccountingCharges = -1,
                LoanAndLoanIntCharges = -1,
                ISQFTCharges = -1,
                WebSEOCharges = -1,
                BackUpPhoneNumber = -1,
                OneTimeCharges = -1,
                RecruitingFee = -1,
                PayrollProcessing = -1,
                CheckSum = -1,
                NationalCharges = -1,
                FranchiseeEmailFee = -1
            });
            invoiceCollection.Add(new InvoiceViewModel
            {

                AdFund = invoiceCollection.Sum(x => x.AdFund) + 1,
                Royalty = invoiceCollection.Sum(x => x.Royalty) + 1,
                PayableAmount = invoiceCollection.Sum(x => x.PayableAmount) + 1,
                FranchiseeName = "Total",
                InterestRate = -1,
                TotalSales = -1,
                AccountCredit = -1,
                InvoiceId = -1,
                SalesDataLateFee = -1,
                PaidAmount = -1,
                MinRoyaltyAmount = invoiceCollection.Sum(x => x.MinRoyaltyAmount) + 1,
                AccruedAmount = -1,
                RoyaltyLateFee = -1,
                IsSEOCostApplied = null,
                FixedAccountingCharges = -1,
                VariableAccountingCharges = -1,
                LoanAndLoanIntCharges = -1,
                ISQFTCharges = -1,
                WebSEOCharges = -1,
                BackUpPhoneNumber = -1,
                OneTimeCharges = -1,
                RecruitingFee = -1,
                PayrollProcessing = -1,
                CheckSum = -1,
                NationalCharges = -1,
                FranchiseeEmailFee = -1
            });

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/invoice-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(invoiceCollection, fileName);
        }

        public bool DownloadInvoiceListAllFile(InvoiceListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            var invoiceCollection = new List<InvoiceViewModel>();
            var franchiseeInvoices = InvoiceFilterList(filter).ToList();

            //prepare item collection
            foreach (var item in franchiseeInvoices)
            {
                if(item.Invoice == null)
                {
                    continue;
                }
                var model = _invoiceFactory.CreateViewModel(item);
                invoiceCollection.Add(model);
            }

            invoiceCollection.Add(new InvoiceViewModel
            {

                AdFund = -1,
                Royalty = -1,
                PayableAmount = -1,
                FranchiseeName = "",
                InterestRate = -1,
                TotalSales = -1,
                AccountCredit = -1,
                InvoiceId = -1,
                SalesDataLateFee = -1,
                PaidAmount = -1,
                MinRoyaltyAmount = -1,
                AccruedAmount = -1,
                RoyaltyLateFee = -1,
                IsSEOCostApplied = null,
                FixedAccountingCharges = -1,
                VariableAccountingCharges = -1,
                LoanAndLoanIntCharges = -1,
                ISQFTCharges = -1,
                WebSEOCharges = -1,
                BackUpPhoneNumber = -1,
                OneTimeCharges = -1,
                RecruitingFee = -1,
                PayrollProcessing = -1,
                CheckSum = -1,
                NationalCharges = -1,
                FranchiseeEmailFee = -1
            });
            invoiceCollection.Add(new InvoiceViewModel
            {

                AdFund = invoiceCollection.Sum(x => x.AdFund) + 1,
                Royalty = invoiceCollection.Sum(x => x.Royalty) + 1,
                PayableAmount = invoiceCollection.Sum(x => x.PayableAmount) + 1,
                FranchiseeName = "Total",
                InterestRate = -1,
                TotalSales = -1,
                AccountCredit = -1,
                InvoiceId = -1,
                SalesDataLateFee = -1,
                PaidAmount = -1,
                MinRoyaltyAmount = invoiceCollection.Sum(x => x.MinRoyaltyAmount) + 1,
                AccruedAmount = -1,
                RoyaltyLateFee = -1,
                IsSEOCostApplied = null,
                FixedAccountingCharges = -1,
                VariableAccountingCharges = -1,
                LoanAndLoanIntCharges = -1,
                ISQFTCharges =-1,
                WebSEOCharges = -1,
                BackUpPhoneNumber = -1,
                OneTimeCharges = -1,
                RecruitingFee = -1,
                PayrollProcessing = -1,
                CheckSum = -1,
                NationalCharges = -1,
                FranchiseeEmailFee = -1
            });
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/invoice-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(invoiceCollection, fileName);
        }

        public DeleteInvoiceResponseModel DeleteInvoiceItem(long invoiceItemId)
        {
            var response = new DeleteInvoiceResponseModel();
            var invoiceitem = _invoiceItemRepositiory.Get(invoiceItemId);

            if (invoiceitem != null && (invoiceitem.ItemTypeId == (long)InvoiceItemType.LateFees
                 || invoiceitem.ItemTypeId == (long)InvoiceItemType.InterestRatePerAnnum
                 || (invoiceitem.ItemTypeId == (long)InvoiceItemType.ServiceFee
                 && invoiceitem.ServiceFeeInvoiceItem != null
                 && (invoiceitem.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.Bookkeeping || invoiceitem.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.VarBookkeeping
                 || invoiceitem.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.NationalCharge || invoiceitem.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.OneTimeProject))
                 || (invoiceitem.ItemTypeId == (long)InvoiceItemType.RoyaltyFee
                 && invoiceitem.RoyaltyInvoiceItem != null && invoiceitem.RoyaltyInvoiceItem.Percentage == null)))
            {
                if (invoiceitem.LateFeeInvoiceItem != null)
                    _latefeeInvoiceItemRepository.Delete(invoiceitem.LateFeeInvoiceItem);
                if (invoiceitem.InterestRateInvoiceItem != null)
                    _interestRateInvoiceItemRepository.Delete(invoiceitem.InterestRateInvoiceItem);
                if (invoiceitem.RoyaltyInvoiceItem != null)
                    _royaltyInvoiceItemRepository.Delete(invoiceitem.RoyaltyInvoiceItem);
                if (invoiceitem.ServiceFeeInvoiceItem != null)
                {
                    if (invoiceitem.ServiceFeeInvoiceItem.ServiceFeeTypeId == (long)ServiceFeeType.OneTimeProject)
                    {
                        var otpList = _oneTimeProjectFeeRepository.Fetch(x => x.InvoiceItemId == invoiceitem.Id);
                        foreach (var otp in otpList)
                        {
                            otp.InvoiceItemId = null;
                            _oneTimeProjectFeeRepository.Save(otp);
                        }
                    }
                    _serviceFeeInvoiceItemRepository.Delete(invoiceitem.ServiceFeeInvoiceItem);
                }

                var invoice = _invoiceRepository.Get(invoiceitem.InvoiceId);
                if (invoice.InvoiceItems.Count == 1 && invoice.Id == invoiceitem.InvoiceId)
                {
                    _invoiceRepository.Delete(invoiceitem.InvoiceId);
                    _franchiseeInvoiceRepository.Delete(x => x.InvoiceId == invoice.Id);
                    response.IsLastItem = true;
                }
                _invoiceItemRepositiory.Delete(invoiceitem);

                if (!response.IsLastItem)
                {
                    var amount = invoice.InvoiceItems.Sum(x => x.Amount);
                    if (amount == 0)
                    {
                        invoice.StatusId = (long)InvoiceStatus.Paid;
                        _invoiceRepository.Save(invoice);
                        response.IsStatusChanged = true;
                    }
                }
                response.IsSuccess = true;
                response.Response = "Item Deleted successfully.";
                return response;
            }
            response.IsSuccess = false;
            return response;
        }

        public bool MarkInvoicesAsDownloaded(long[] invoiceIds)
        {
            foreach (var invoiceId in invoiceIds)
            {
                var franchiseeInvoice = _franchiseeInvoiceRepository.Get(x => x.InvoiceId.Equals(invoiceId));

                franchiseeInvoice.IsDownloaded = true;
                _franchiseeInvoiceRepository.Save(franchiseeInvoice);
            }
            return true;
        }

        public InvoiceDetailsViewModel InvoicePaymentDetails(long invoiceId)
        {
            var info = FranchiseeInvoiceDetails(invoiceId);
            var franchiseeInvoiceData = _franchiseeInvoiceRepository.Table.FirstOrDefault(x => x.InvoiceId == invoiceId);
            var accountCreditList = _franchiseeAccountCreditRepository.Table.Where(x => x.FranchiseeId == franchiseeInvoiceData.FranchiseeId).OrderByDescending(x => x.CreditedOn).ToList();
            info.SumByCategory = GetSumByCategory(accountCreditList);
            return info;
        }
        private SumByCategory GetSumByCategory(List<FranchiseeAccountCredit> accountCreditList)
        {
            var adFundAmount = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.AdFund).Select(x => x);
            return new SumByCategory()
            {
                TotalByAdFund = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.AdFund).Sum(x => x.RemainingAmount),
                TotalByTotalSales = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.AllSalesCredit).Sum(x => x.RemainingAmount),
                TotalByRoyality = accountCreditList.Where(x => x.CreditTypeId == (long?)AccountCreditType.Royalty).Sum(x => x.RemainingAmount),
            };
        }
        private decimal? GetTotalUnPaidAmount(List<long> invoiceList)
        {
            var invoiceDomainList = _invoiceItemRepositiory.Table.Where(x => invoiceList.Contains(x.InvoiceId) && (x.Invoice.StatusId == (long?)InvoiceStatus.Unpaid || x.Invoice.StatusId == (long?)InvoiceStatus.PartialPaid)).ToList();
            var totalInvoiceAmount = invoiceDomainList.Sum(x => x.Amount);
            var invoicePaymentIdsList = _invoicePaymentRepository.Table.Where(x => invoiceList.Contains(x.InvoiceId) && (x.Invoice.StatusId == (long?)InvoiceStatus.Unpaid || x.Invoice.StatusId == (long?)InvoiceStatus.PartialPaid)).Select(x => x.PaymentId).ToList();
            var invoicePaymentList = _paymentRepository.Table.Where(x => invoicePaymentIdsList.Contains(x.Id)).ToList();
            var totalInvoiceAmountPaid = invoicePaymentList.Sum(x => x.Amount);
            var unpaidAmount = totalInvoiceAmount - totalInvoiceAmountPaid;
            return unpaidAmount;
        }

        public bool SaveInvoiceReconciliationNotes(InvoiceReconciliationNotesModel model)
        {
            var invoice = _invoiceRepository.Table.Where(x => x.Id == model.Id).FirstOrDefault();
            if(invoice != null)
            {
                invoice.ReconciliationNotes = model.ReconciliationNotes;
                _invoiceRepository.Save(invoice);
            }
            return true;
        }
    }
}
