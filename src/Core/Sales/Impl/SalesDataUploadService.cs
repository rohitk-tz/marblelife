using System;
using Core.Application.Attribute;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Application;
using System.Linq;
using Core.Application.ViewModel;
using Core.Users.Enum;
using Core.Application.Impl;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Sales.Enum;
using Core.Organizations.Enum;
using Core.Billing.Enum;
using Core.Organizations;
using System.Collections.Generic;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesDataUploadService : ISalesDataUploadService
    {
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly ISalesDataUploadFactory _salesDataUploadFactory;
        private readonly IFileService _fileService;
        private readonly ISortingHelper _sortingHelper;
        private readonly ILogService _logService;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<FeeProfile> _feeProfileRepository;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private readonly IFranchiseeAccountCreditFactory _franchiseeAccountCreditFactory;
        private readonly IRepository<FranchiseeAccountCredit> _franchiseeAccountCreditRepository;
        private readonly IRepository<AnnualSalesDataUpload> _annualSalesDataUploadRepository;
        private readonly IRepository<FranchiseeDocumentType> _franchiseeDocumentTypeRepository;
        private readonly IRepository<FranchiseDocument> _franchiseeDocumentRepository;
        private readonly IRepository<Organizations.Domain.DocumentType> _documentTypeRepository;
        private readonly IRepository<Franchisee> _organizationRepository;
        private readonly IRepository<Perpetuitydatehistry> _perpetuitydatehistryRepository;

        public SalesDataUploadService(IUnitOfWork unitOfWork, ISalesDataUploadFactory salesDataUploadFactory, IFileService fileService,
            ISortingHelper sortingHelper, ILogService logService, IClock clock, ISettings settings, IFranchiseeAccountCreditFactory franchiseeAccountCreditFactory)
        {
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _salesDataUploadFactory = salesDataUploadFactory;
            _fileService = fileService;
            _sortingHelper = sortingHelper;
            _logService = logService;
            _feeProfileRepository = unitOfWork.Repository<FeeProfile>();
            _unitOfWork = unitOfWork;
            _clock = clock;
            _settings = settings;
            _franchiseeAccountCreditFactory = franchiseeAccountCreditFactory;
            _franchiseeAccountCreditRepository = unitOfWork.Repository<FranchiseeAccountCredit>();
            _annualSalesDataUploadRepository = unitOfWork.Repository<AnnualSalesDataUpload>();
            _franchiseeDocumentRepository = unitOfWork.Repository<FranchiseDocument>();
            _documentTypeRepository = unitOfWork.Repository<Organizations.Domain.DocumentType>();
            _franchiseeDocumentTypeRepository = unitOfWork.Repository<FranchiseeDocumentType>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _perpetuitydatehistryRepository = unitOfWork.Repository<Perpetuitydatehistry>();
        }

        public void Save(SalesDataUploadCreateModel model)
        {
            var salesdataUpload = _salesDataUploadFactory.CreateDomain(model);
            if (_franchiseeRepository.Get(model.FranchiseeId).Organization.Address != null)
            {
                long countryId = _franchiseeRepository.Get(model.FranchiseeId).Organization.Address.First().CountryId;
                salesdataUpload.CurrencyExchangeRateId = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(y => y.DateTime).First().Id;
            }
            var file = _fileService.SaveModel(model.File);
            salesdataUpload.FileId = file.Id;
            _salesDataUploadRepository.Save(salesdataUpload);
            if (model.IsAnnualUpload && model.AnnualFile != null)
            {
                model.CurrencyExchareRateId = salesdataUpload.CurrencyExchangeRateId;
                model.DataRecorderMetaData = salesdataUpload.DataRecorderMetaData;
                SaveAnnualUpload(model, salesdataUpload);
            }
        }

        public bool isValidUpload(SalesDataUploadCreateModel model)
        {

            var startDate = new DateTime(Convert.ToInt32(model.Year), 1, 1);
            var endDate = new DateTime(Convert.ToInt32(model.Year), 12, 31);

            var isValid = _annualSalesDataUploadRepository.Table.Any(x => x.FranchiseeId == model.FranchiseeId && x.PeriodStartDate == startDate && x.PeriodEndDate == endDate &&
                                                                          x.DataRecorderMetaData.DateCreated.Year == DateTime.Now.Year &&
                                                                          x.StatusId == (long)SalesDataUploadStatus.Parsed && x.AuditActionId != (long)AuditActionType.Rejected);
            if (isValid)
            {
                model.Message = FeedbackMessageModel.CreateWarningMessage("Annual File Already Uploaded!!");
                return false;
            }
            else
            {
                isValid = true;
            }
            return isValid;
        }
        public void SaveAnnualUpload(SalesDataUploadCreateModel model, SalesDataUpload salesdataUpload)
        {
            model.AuditActionId = (long)AuditActionType.Pending;
            var annualUpload = _salesDataUploadFactory.CreateAnnualUploadDomain(model, salesdataUpload);
            var annualFile = _fileService.SaveModel(model.AnnualFile);
            annualUpload.FileId = annualFile.Id;
            annualUpload.CurrencyExchangeRateId = model.CurrencyExchareRateId;
            annualUpload.IsAuditAddressParsing = true;
            _annualSalesDataUploadRepository.Save(annualUpload);
        }

        public SalesDataUploadListModel GetBatchList(SalesDataListFilter filter, int pageNumber, int pageSize)
        {
            var salesData = _salesDataUploadRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.FranchiseeId == filter.FranchiseeId)
                             && (filter.StatusId < 1 || x.StatusId == filter.StatusId)
                             && (x.IsActive)
                             && (string.IsNullOrEmpty(filter.Text) || ((x.Id.ToString().Equals(filter.Text))))
                             && ((filter.PeriodEndDate == null || x.PeriodStartDate >= filter.PeriodStartDate)
                             && (filter.PeriodEndDate == null || x.PeriodEndDate <= filter.PeriodEndDate)));
            salesData = _sortingHelper.ApplySorting(salesData, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Id, filter.SortingOrder);
                        break;
                    case "Franchisee":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Franchisee.Organization.Name, filter.SortingOrder);
                        break;
                    case "StartDate":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.PeriodStartDate, filter.SortingOrder);
                        break;
                    case "EndDate":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.PeriodEndDate, filter.SortingOrder);
                        break;
                    case "Frequency":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Franchisee.FeeProfile.Lookup.Name, filter.SortingOrder);
                        break;
                    case "TotalAmount":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.TotalAmount, filter.SortingOrder);
                        break;
                    case "PaidAmount":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.PaidAmount, filter.SortingOrder);
                        break;
                    case "Customers":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.NumberOfCustomers, filter.SortingOrder);
                        break;
                    case "Invoices":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.NumberOfInvoices, filter.SortingOrder);
                        break;
                    case "Status":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.Lookup.Name, filter.SortingOrder);
                        break;
                    case "UploadedDate":
                        salesData = _sortingHelper.ApplySorting(salesData, x => x.DataRecorderMetaData.DateCreated, filter.SortingOrder);
                        break;
                }
            }


            var list = salesData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new SalesDataUploadListModel()
            {
                Collection = list.Select(_salesDataUploadFactory.CreateListModel).ToList(),
                PagingModel = new PagingModel(pageNumber, pageSize, salesData.Count()),
                Filter = filter
            };
        }

        public DateTime? GetLastUploadedBatch(long franchiseeId)
        {
            var salesData = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.StatusId != (long)SalesDataUploadStatus.Failed && x.IsActive)
                .OrderByDescending(x => x.PeriodEndDate).FirstOrDefault();
            if (salesData == null)
                return null;
            var uploadStartdate = salesData.PeriodEndDate.AddDays(1);
            if (salesData.Franchisee.FeeProfile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
            {
                if (uploadStartdate.DayOfWeek == DayOfWeek.Monday)
                    return uploadStartdate;
                else
                {
                    var validDate = StartOfWeek(uploadStartdate, DayOfWeek.Monday);
                    return validDate;
                }
            }
            else
            {
                if (uploadStartdate.Day == 1)
                    return uploadStartdate;
                else
                {
                    var monthlyStartdate = new DateTime(uploadStartdate.Year, uploadStartdate.Month, 1).AddMonths(1);
                    return monthlyStartdate;
                }
            }
        }

        public DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public SalesDataUpload GetSalesDataUploadByFranchiseeId(long franchiseeId)
        {
            return _salesDataUploadRepository.Table.Where(x => x.Franchisee.Id == franchiseeId && x.IsActive)
                .OrderByDescending(x => x.DataRecorderMetaData.DateModified).FirstOrDefault();
        }

        public bool Delete(long id)
        {
            var record = _salesDataUploadRepository.Get(id);
            if (record == null) return true;

            if (record.StatusId == (long)SalesDataUploadStatus.Failed)
            {
                _salesDataUploadRepository.Delete(record);
                return true;
            }

            CleanDataForSalesDataUploadId(id);
            _salesDataUploadRepository.Delete(record);

            return true;
        }

        public bool Reparse(long id)
        {
            var record = _salesDataUploadRepository.Get(id);
            if (record == null) throw new Exception("Record doesn't exist");

            if (record.StatusId != (long)SalesDataUploadStatus.Failed)
            {
                CleanDataForSalesDataUploadId(id);
            }

            record.StatusId = (long)SalesDataUploadStatus.Uploaded;
            record.IsInvoiceGenerated = false;
            record.TotalAmount = 0;
            record.PaidAmount = 0;
            record.NumberOfCustomers = 0;
            record.NumberOfInvoices = 0;
            record.NumberOfParsedRecords = 0;
            record.NumberOfFailedRecords = 0;

            _salesDataUploadRepository.Save(record);
            return true;
        }

        private void CleanDataForSalesDataUploadId(long id)
        {
            var franchiseeSalesRepo = _unitOfWork.Repository<FranchiseeSales>();
            var franchiseeSalesPaymentRepo = _unitOfWork.Repository<FranchiseeSalesPayment>();
            var invoiceRepo = _unitOfWork.Repository<Invoice>();
            var franchiseeInvoiceRepo = _unitOfWork.Repository<FranchiseeInvoice>();
            var invoiceItemRepo = _unitOfWork.Repository<InvoiceItem>();
            var paymentRepo = _unitOfWork.Repository<Payment>();
            var paymentItemRepo = _unitOfWork.Repository<PaymentItem>();
            var invoicePaymentRepo = _unitOfWork.Repository<InvoicePayment>();
            var oneTimeProjectFeeRepo = _unitOfWork.Repository<OneTimeProjectFee>();
            var loanInvoiceItemRepo = _unitOfWork.Repository<FranchiseeLoanSchedule>();
            var accountCreditRepo = _unitOfWork.Repository<AccountCredit>();
            var accountCreditItemRepo = _unitOfWork.Repository<AccountCreditItem>();
            var serviceFeeInvoiceItemRepo = _unitOfWork.Repository<ServiceFeeInvoiceItem>();

            #region Adjusting Sales Spread Across Multiple SDUs
            var franchiseeSalesHavingPaymentsForOtherSDU =
                (from fs in franchiseeSalesRepo.Table
                 join
                    ip in invoicePaymentRepo.Table on fs.InvoiceId equals ip.InvoiceId
                 join
                    fsp in franchiseeSalesPaymentRepo.Table on ip.PaymentId equals fsp.PaymentId
                 where fs.SalesDataUploadId == id && fsp.SalesDataUploadId != id
                 select new
                 {
                     FranchiseeSales = fs,
                     SalesDataUploadId = fsp.SalesDataUploadId
                 }).ToArray();

            foreach (var item in franchiseeSalesHavingPaymentsForOtherSDU)
            {
                item.FranchiseeSales.SalesDataUploadId = item.SalesDataUploadId;
                franchiseeSalesRepo.Save(item.FranchiseeSales);
            }
            #endregion

            #region Allocate Credit For Royalties or Ad Fund Paid
            var paidFranchiseeInvoices = franchiseeInvoiceRepo.Table.Where(x => x.SalesDataUploadId == id &&
                (x.Invoice.StatusId == (long)InvoiceStatus.Paid || x.Invoice.StatusId == (long)InvoiceStatus.PartialPaid)).ToList();

            foreach (var item in paidFranchiseeInvoices)
            {
                var franchiseeAccountCredit = _franchiseeAccountCreditFactory.CreateDomain(item);
                _franchiseeAccountCreditRepository.Save(franchiseeAccountCredit);
            }
            #endregion

            var frInvoiceIds = franchiseeInvoiceRepo.Table.Where(x => x.SalesDataUploadId == id).Select(x => x.InvoiceId).ToList();

            var servicefeeInvoiceItemIds = invoiceItemRepo.Table.Where(x => x.ItemTypeId == (long)InvoiceItemType.ServiceFee
                                         && frInvoiceIds.Contains(x.InvoiceId)).Select(x => x.Id);
            #region Reset OneTimeProject fee for Re-Calculation
            var otpFee = serviceFeeInvoiceItemRepo.Fetch(x => x.ServiceFeeTypeId == (long)ServiceFeeType.OneTimeProject
                            && servicefeeInvoiceItemIds.Contains(x.Id)).FirstOrDefault();

            if (otpFee != null)
            {
                var list = oneTimeProjectFeeRepo.Table.Where(x => x.InvoiceItemId == otpFee.Id).ToList();
                foreach (var item in list)
                {
                    item.InvoiceItemId = null;
                    oneTimeProjectFeeRepo.Save(item);
                }
            }
            #endregion

            #region Reset Loan for Re-Calculation

            var loanInvoiceItemIds = invoiceItemRepo.Table.Where(x => x.ItemTypeId == (long)InvoiceItemType.LoanServiceFee
                                         && frInvoiceIds.Contains(x.InvoiceId)).Select(x => x.Id);

            var franchiseeLoan = loanInvoiceItemRepo.Fetch(x => x.InvoiceItemId != null && loanInvoiceItemIds.Contains(x.InvoiceItemId.Value)).FirstOrDefault();
            if (franchiseeLoan != null)
            {
                franchiseeLoan.InvoiceItemId = null;
                franchiseeLoan.OverPaidAmount = 0;
                franchiseeLoan.CalculateReschedule = true;
                loanInvoiceItemRepo.Save(franchiseeLoan);
            }
            #endregion

            // DO NOT DELETE PAID ROYALTY INVOICES, AS THEY ARE LINKED WITH ACCOUNT CREDITS. THEIR ASSOCIATION WITH SDU SHOULD BE DELETED.

            #region Delete Unpaid Royalty AdFund Invoices

            var unpaidFranchiseeInvoices = franchiseeInvoiceRepo.Table.Where(x => x.SalesDataUploadId == id &&
                (x.Invoice.StatusId == (long)InvoiceStatus.Unpaid));

            var invoiceItemIds = invoiceItemRepo.Table.Where(ii => unpaidFranchiseeInvoices.Select(ufi => ufi.InvoiceId).Contains(ii.InvoiceId))
                .Select(ii => ii.Id);

            var royaltyInvoiceItemRepo = _unitOfWork.Repository<RoyaltyInvoiceItem>();
            var adFundInvoiceItemRepo = _unitOfWork.Repository<AdFundInvoiceItem>();
            var lateFeeInvoiceItemRepo = _unitOfWork.Repository<LateFeeInvoiceItem>();
            var intRateInvoiceItemRepo = _unitOfWork.Repository<InterestRateInvoiceItem>();

            royaltyInvoiceItemRepo.Delete(rr => invoiceItemIds.Contains(rr.Id));
            adFundInvoiceItemRepo.Delete(rr => invoiceItemIds.Contains(rr.Id));
            lateFeeInvoiceItemRepo.Delete(rr => invoiceItemIds.Contains(rr.Id));
            intRateInvoiceItemRepo.Delete(rr => invoiceItemIds.Contains(rr.Id));

            serviceFeeInvoiceItemRepo.Delete(sf => invoiceItemIds.Contains(sf.Id));

            invoiceItemRepo.Delete(x => invoiceItemIds.Contains(x.Id));
            invoiceRepo.Delete(x => unpaidFranchiseeInvoices.Select(ufi => ufi.InvoiceId).Contains(x.Id));

            #endregion            

            franchiseeInvoiceRepo.Delete(x => x.SalesDataUploadId == id);

            #region "Delete Account Credit Records Against SDU"
            var queryAccountCreditIds = franchiseeSalesRepo.Table.Where(x => x.SalesDataUploadId == id).Select(x => x.AccountCreditId);
            accountCreditItemRepo.Delete(aci => queryAccountCreditIds.Contains(aci.AccountCreditId));
            accountCreditRepo.Delete(ac => queryAccountCreditIds.Contains(ac.Id));
            #endregion

            var queryInvoiceIds = franchiseeSalesRepo.Table.Where(x => x.SalesDataUploadId == id).Select(x => x.InvoiceId);
            var queryPaymentIds = franchiseeSalesPaymentRepo.Table.Where(x => x.SalesDataUploadId == id).Select(x => x.PaymentId);

            // delete payment item
            paymentItemRepo.Delete(p => queryPaymentIds.Contains(p.PaymentId));
            //Delete Payment
            paymentRepo.Delete(x => queryPaymentIds.Contains(x.Id));
            invoicePaymentRepo.Delete(x => queryInvoiceIds.Contains(x.InvoiceId) || queryPaymentIds.Contains(x.PaymentId));

            invoiceItemRepo.Delete(x => queryInvoiceIds.Contains(x.InvoiceId));
            invoiceRepo.Delete(x => queryInvoiceIds.Contains(x.Id));

            franchiseeSalesRepo.Delete(fs => fs.SalesDataUploadId == id);

        }

        public bool DoesOverlappingDatesExist(long franchiseeId, DateTime startDate, DateTime endDate)
        {
            return _salesDataUploadRepository.Table.Any(x => x.FranchiseeId == franchiseeId && x.StatusId != (long)SalesDataUploadStatus.Failed && ((x.PeriodStartDate <= startDate && x.PeriodEndDate >= endDate) ||
                                                                                            (x.PeriodStartDate <= startDate && x.PeriodEndDate >= startDate) ||
                                                                                            (x.PeriodStartDate <= endDate && x.PeriodEndDate >= endDate) ||
                                                                                            (x.PeriodStartDate >= startDate && x.PeriodEndDate <= endDate)
                                                                                            && x.IsActive));
        }

        public bool CheckValidRangeForSalesUpload(SalesDataUploadCreateModel model)
        {
            var applyDateValidation = _settings.ApplyDateValidation;
            if (model.PeriodStartDate > _clock.UtcNow)
            {
                model.Message = FeedbackMessageModel.CreateWarningMessage("Can't upload Sales Data for future dates.");
                return false;
            }
            if (applyDateValidation)
            {
                var salesdataList = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId && x.StatusId != (long)SalesDataUploadStatus.Failed
                                     && x.IsActive).OrderByDescending(x => x.PeriodEndDate);
                if (!salesdataList.Any())
                    return true;
                else
                {
                    var firstupload = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId && x.StatusId != (long)SalesDataUploadStatus.Failed
                                      && x.IsActive).OrderBy(x => x.PeriodEndDate).FirstOrDefault();
                    var UploadList = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId
                    && x.StatusId != (long)SalesDataUploadStatus.Failed && x.IsActive && x.PeriodStartDate == model.PeriodStartDate &&
                    x.PeriodEndDate == model.PeriodEndDate).OrderByDescending(x => x.PeriodEndDate).ToList();
                    
                    if (firstupload == null)
                        return true;
                    if ((firstupload.PeriodStartDate > model.PeriodStartDate || firstupload.PeriodStartDate > model.PeriodEndDate) && model.FranchiseeId != 74)
                    {
                        model.Message = FeedbackMessageModel.CreateWarningMessage("can't upload sales data earlier than your first upload.");
                        return false;
                    }
                    var lastUpdate = salesdataList.FirstOrDefault();
                    if (lastUpdate == null)
                        return false;
                    var feeprofile = _feeProfileRepository.Get(model.FranchiseeId);
                    if (feeprofile == null)
                        return false;
                    if (feeprofile.PaymentFrequencyId == null || feeprofile.PaymentFrequencyId == (long)PaymentFrequency.Monthly)
                    {
                        var expectedUpdateStartDate = lastUpdate.PeriodEndDate.Date.AddDays(1);
                        var expectedUpdateEndDate = expectedUpdateStartDate.Date.AddMonths(1).AddDays(-1);
                        if (model.PeriodStartDate.Date > expectedUpdateStartDate)
                        {
                            model.Message = FeedbackMessageModel.CreateWarningMessage("Please Upload files for Previous date range");
                            return false;
                        }
                        else return true;
                    }
                    if (feeprofile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
                    {
                        var expectedUpdateStartDate = lastUpdate.PeriodEndDate.Date.AddDays(1);
                        var expectedUpdateEndDate = expectedUpdateStartDate.Date.AddDays(6);
                        if (model.PeriodStartDate.Date > expectedUpdateStartDate)
                        {
                            model.Message = FeedbackMessageModel.CreateWarningMessage("Please Upload files for Previous date range");
                            return false;
                        }
                    }
                    if (UploadList.Count > 0)
                    {
                        model.Message = FeedbackMessageModel.CreateWarningMessage("Can't upload sales data again for same date range.");
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }



        public bool CheckForExpiringDocument(SalesDataUploadCreateModel model)
        {
            var todayUtcDte = DateTime.UtcNow.Date;
            var expiringDateForFranchisee = (_settings.ExpiringDateForFranchisee).Date;
            var franchiseeDocuments = _franchiseeDocumentTypeRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId && x.IsActive).Select(x => x.DocumentTypeId).ToList();
            var franchisee = _franchiseeRepository.Get(model.FranchiseeId);


            var expiryYear = franchisee.RegistrationDate != null ? franchisee.RegistrationDate.GetValueOrDefault().Year :
                                franchisee.Organization.DataRecorderMetaData.DateCreated.Year;

            if (expiringDateForFranchisee > todayUtcDte)
            {
                string message = "Please Upload the Following Pending Documents : <br/>";
                var isValidation = false;
                var documentsIdList = new List<long>() { 3, 15, 16, 19, 11, 5 };
                documentsIdList = documentsIdList.Intersect(franchiseeDocuments).ToList();
                var yearList = new List<string>() { DateTime.Now.Year.ToString() };

                var franchiseeDocumentList = _franchiseeDocumentRepository.Table.Where(x => documentsIdList.Contains(x.DocumentTypeId.Value) &&
                                                    model.FranchiseeId == x.FranchiseeId).ToList();

                isValidation = IsAllDocumentUploaded(expiryYear, franchiseeDocumentList, documentsIdList, model.FranchiseeId, out message, model);


                if (isValidation)
                {
                    model.Message = FeedbackMessageModel.CreateWarningMessage(message);
                    return false;
                }
            }
            else
            {
                string message = "Please Upload the Following Pending Documents : <br/>";
                var isValidation = false;
                var documentsIdList = new List<long>() { 3, 5, 11, 15, 16, 17, 18, 19 };
                //documentsIdList = documentsIdList.Intersect(franchiseeDocuments).ToList();
                var yearList = new List<string>() { DateTime.Now.Year.ToString() };

                var franchiseeDocumentList = _franchiseeDocumentRepository.Table.Where(x => documentsIdList.Contains(x.DocumentTypeId.Value) &&
                                                    model.FranchiseeId == x.FranchiseeId).ToList();

                isValidation = IsAllDocumentUploaded(expiryYear, franchiseeDocumentList, documentsIdList, model.FranchiseeId, out message, model);


                if (isValidation)
                {
                    model.Message = FeedbackMessageModel.CreateWarningMessage(message);
                    return false;
                }
            }
            return true;
        }


        private bool IsAllDocumentUploaded(long expiryYear, List<FranchiseDocument> franchiseeDocumentList, List<long> documentsIdList, long? franchiseeId, out string message, SalesDataUploadCreateModel model)
        {
            //if (expiryYear >= 2021)
            //{
            //    message = "";
            //    return false;
            //}

            var isValidation = false;
            message = "";
            var frannchiseeDocument = _franchiseeDocumentRepository.Table.OrderByDescending(x => x.FranchiseeId == franchiseeId && documentsIdList.Contains(x.DocumentTypeId.Value)).ToList();
            var perpetuityOnFranchiseeList = _perpetuitydatehistryRepository.Table.Where(x => x.IsPerpetuity == true && x.FranchiseeId != null).Select(x => x.FranchiseeId).ToList();
            var franchiseeDocumentType = _franchiseeDocumentTypeRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId).ToList();

            //var resaleCertificatePerpetuity = _franchiseeDocumentTypeRepository.Table.OrderByDescending(x => x.Id).Any(x => x.DocumentTypeId == (long?)DocumentEnum.ResaleCertificate && x.IsPerpetuity && x.FranchiseeId == franchiseeId && x.IsActive);
            //var declareCertificatePerpetuity = frannchiseeDocument.OrderByDescending(x => x.Id).Any(x => x.DocumentTypeId == (long?)DocumentEnum.ResaleCertificate && !x.IsRejected && x.FranchiseeId == franchiseeId && x.UploadFor == "2021");
            //var isResaleCetificateUploaded = franchiseeDocumentList.Any(x => x.DocumentTypeId == (long?)DocumentEnum.ResaleCertificate && x.FranchiseeId == franchiseeId && x.UploadFor == "2021");

            //var isPresent = franchiseeDocumentList.Any(x => x.UploadFor == "2021" && x.DocumentTypeId == (long)DocumentEnum.ResaleCertificate);
            //if (!isPresent && documentsIdList.Contains((long)DocumentEnum.ResaleCertificate))
            //{
            //    message += "Resale Certificate " + " Not Uploaded for Year " + "2021" + " <br/><br/> ";
            //    isValidation = true;
            //}

            //var isPresent = franchiseeDocumentList.Any(x => x.UploadFor == "2021" && x.DocumentTypeId == (long)DocumentEnum.AnnualTaxFiling);
            //if (!isPresent && documentsIdList.Contains((long)DocumentEnum.AnnualTaxFiling))
            //{
            //    message += "Annual Tax Filling " + " Not Uploaded for Year " + "2021" + " <br/><br/> ";
            //    isValidation = true;
            //}

            //isPresent = franchiseeDocumentList.Any(x => x.UploadFor == "2021" && x.DocumentTypeId == (long)DocumentEnum.MaterialPurchase);
            //if (!isPresent && documentsIdList.Contains((long)DocumentEnum.MaterialPurchase))
            //{
            //    message += "Material Purchase History " + " Not Uploaded for Year " + "2021" + " <br/><br/> ";
            //    isValidation = true;
            //}

            //isPresent = franchiseeDocumentList.Any(x => x.UploadFor == "2021" && x.DocumentTypeId == (long)DocumentEnum.PAndL);
            //if (!isPresent && documentsIdList.Contains((long)DocumentEnum.MaterialPurchase))
            //{
            //    message += "Annual P&L - Profit & Loss Statement " + " Not Uploaded for Year " + "2021" + " <br/><br/> ";
            //    isValidation = true;
            //}

            //isPresent = franchiseeDocumentList.Any(x => x.UploadFor == "2021" && x.DocumentTypeId == (long)DocumentEnum.BalanceSheet);
            //if (!isPresent && documentsIdList.Contains((long)DocumentEnum.MaterialPurchase))
            //{
            //    message += "Annual Balance Sheet " + " Not Uploaded for Year " + "2021" + " <br/><br/> ";
            //    isValidation = true;
            //}

            var isPresent = false;

            int currentYear = DateTime.Now.Year;
            // Get the second week of January
            DateTime firstDayOfMonth = new DateTime(currentYear, 1, 1);
            int offsetToSecondMonday = ((7 - (int)firstDayOfMonth.DayOfWeek) % 7) + 7;
            DateTime secondMonday = firstDayOfMonth.AddDays(offsetToSecondMonday);
            List<long> ides = new List<long> { 69, 67, 66, 89, 63, 68 };

            // Get first week of jan
            int offsetToFirstMonday = (7 - (int)firstDayOfMonth.DayOfWeek + (int)DayOfWeek.Monday) % 7;
            DateTime firstMonday = firstDayOfMonth.AddDays(offsetToFirstMonday);

            // Get 2nd Monday of May
            DateTime mayFirst = new DateTime(currentYear, 5, 1);
            int offset = ((int)DayOfWeek.Monday - (int)mayFirst.DayOfWeek + 7) % 7;
            DateTime firstMondayOfMay = mayFirst.AddDays(offset);
            DateTime secondMondayOfMay = firstMondayOfMay.AddDays(7);

            isPresent = franchiseeDocumentList.Any(x => x.UploadFor == DateTime.Now.Year.ToString() && x.DocumentTypeId == (long)DocumentEnum.COI);
            if (!isPresent && !ides.Contains(model.FranchiseeId))
            {
                if (model.PeriodStartDate >= secondMonday)
                {
                    message += "COI / SALES TAX CERTIFICATE " + " Not Uploaded for Year " + currentYear + " <br/><br/> ";
                    isValidation = true;
                }
            }

            if (perpetuityOnFranchiseeList.Contains(model.FranchiseeId))        //perpetuity is on
            {
                isPresent = franchiseeDocumentList.Any(x => x.DocumentTypeId == (long)DocumentEnum.ResaleCertificate);
                if (!isPresent && !ides.Contains(model.FranchiseeId))
                {
                    if (model.PeriodStartDate >= secondMonday)
                    {
                        message += "RESALE / SALES TAX CERTIFICATE " + " Not Uploaded for Year " + currentYear + " <br/><br/> ";
                        isValidation = true;
                    }
                }
            }
            else if (!perpetuityOnFranchiseeList.Contains(model.FranchiseeId))  //perpetuity is off
            {
                isPresent = franchiseeDocumentList.Any(x => x.UploadFor == DateTime.Now.Year.ToString() && x.DocumentTypeId == (long)DocumentEnum.ResaleCertificate);
                if (!isPresent && !ides.Contains(model.FranchiseeId))
                {
                    if (model.PeriodStartDate >= secondMonday)
                    {
                        message += "RESALE / SALES TAX CERTIFICATE " + " Not Uploaded for Year " + currentYear + " <br/><br/> ";
                        isValidation = true;
                    }
                }
            }
            //New

            DateTime thirdMonday = GetThirdMondayOfMarch(currentYear);
            isPresent = franchiseeDocumentList.Any(x => x.UploadFor == (DateTime.Now.Year - 1).ToString() && x.DocumentTypeId == (long)DocumentEnum.PAndL);
            if (!isPresent && !ides.Contains(model.FranchiseeId) && franchiseeDocumentType.FirstOrDefault(x => x.DocumentTypeId == (long)DocumentEnum.PAndL).IsActive)
            {
                if(model.PeriodStartDate >= thirdMonday)
                {
                    message += "Annual P&L - Profit & Loss Statement " + " Not Uploaded for Year " + (DateTime.Now.Year - 1) + " <br/><br/> ";
                    isValidation = true;
                }
            }
            isPresent = franchiseeDocumentList.Any(x => x.UploadFor == (DateTime.Now.Year - 1).ToString() && x.DocumentTypeId == (long)DocumentEnum.BalanceSheet);
            if (!isPresent && !ides.Contains(model.FranchiseeId) && franchiseeDocumentType.FirstOrDefault(x => x.DocumentTypeId == (long)DocumentEnum.BalanceSheet).IsActive)
            {
                if (model.PeriodStartDate >= thirdMonday)
                {
                    message += "Annual Balance Sheet " + " Not Uploaded for Year " + (DateTime.Now.Year - 1) + " <br/><br/> ";
                    isValidation = true;
                }
            }
            isPresent = franchiseeDocumentList.Any(x => x.UploadFor == (DateTime.Now.Year - 1).ToString() && x.DocumentTypeId == (long)DocumentEnum.AnnualPayble);
            if (!isPresent && !ides.Contains(model.FranchiseeId) && franchiseeDocumentType.FirstOrDefault(x => x.DocumentTypeId == (long)DocumentEnum.AnnualPayble).IsActive)
            {
                if (model.PeriodStartDate >= thirdMonday)
                {
                    message += "Accounts Payable " + " Not Uploaded for Year " + (DateTime.Now.Year - 1) + " <br/><br/> ";
                    isValidation = true;
                }
            }
            isPresent = franchiseeDocumentList.Any(x => x.UploadFor == (DateTime.Now.Year - 1).ToString() && x.DocumentTypeId == (long)DocumentEnum.AnnulaReceivable);
            if (!isPresent && !ides.Contains(model.FranchiseeId) && franchiseeDocumentType.FirstOrDefault(x => x.DocumentTypeId == (long)DocumentEnum.AnnulaReceivable).IsActive)
            {
                if (model.PeriodStartDate >= thirdMonday)
                {
                    message += "Accounts Receivable " + " Not Uploaded for Year " + (DateTime.Now.Year - 1) + " <br/><br/> ";
                    isValidation = true;
                }
            }
            isPresent = franchiseeDocumentList.Any(x => x.UploadFor == (DateTime.Now.Year - 1).ToString() && x.DocumentTypeId == (long)DocumentEnum.MaterialPurchase);
            if (!isPresent && !ides.Contains(model.FranchiseeId) && franchiseeDocumentType.FirstOrDefault(x => x.DocumentTypeId == (long)DocumentEnum.MaterialPurchase).IsActive)
            {
                if (model.PeriodStartDate >= thirdMonday)
                {
                    message += "Material Purchase History " + " Not Uploaded for Year " + (DateTime.Now.Year - 1) + " <br/><br/> ";
                    isValidation = true;
                }
            }
            isPresent = franchiseeDocumentList.Any(x => x.UploadFor == (DateTime.Now.Year - 1).ToString() && x.DocumentTypeId == (long)DocumentEnum.AnnualTaxFiling);
            if(!isPresent && !ides.Contains(model.FranchiseeId) && franchiseeDocumentType.FirstOrDefault(x => x.DocumentTypeId == (long)DocumentEnum.AnnualTaxFiling).IsActive)
            {
                if(model.PeriodStartDate >= firstMondayOfMay)
                {
                    message += "Annual Tax Filing " + " Not Uploaded for Year " + (DateTime.Now.Year - 1) + " <br/><br/> ";
                    isValidation = true;
                }
            }

            return isValidation;
        }



        public void Update(SalesDataUploadCreateModel model)
        {
            model.Id = 0;
            model.StatusId = (long)SalesDataUploadStatus.Uploaded;
            var salesdataUpload = _salesDataUploadFactory.CreateDomain(model);
            var file = _fileService.SaveModel(model.File);
            salesdataUpload.FileId = file.Id;
            _salesDataUploadRepository.Save(salesdataUpload);
        }

        public AnnualUploadValidationModel GetAnnualUploadInfo(AnnualUploadValidationModel model)
        {

            var lastUploadStartDate = model.PeriodStartDate; // dec, current year 
            const int year = 2017;
            var lastUploaYear = _clock.ToUtc(DateTime.Now).Year - 1;
            string annualUploadYears = "";
            if (lastUploadStartDate.Year != year)
            {
                var lastYear = lastUploadStartDate.Year - 1;
                lastUploadStartDate = new DateTime(lastYear, 12, 1);
            }

            var lastStartDate2019 = new DateTime(lastUploaYear, 1, 1);
            var lastEndDate2019 = new DateTime(lastUploaYear, 12, 31);
            var startDate = GetLastUploadDateOfYear(lastUploadStartDate, model.PaymentFrequencyId);
            model.UploadStartDate = new DateTime(startDate.Year, 1, 1);  //1/1/2018
            model.UploadEndDate = new DateTime(startDate.Year, 12, 31); //31/12/2018

            var startDateFor2018 = model.UploadStartDate;
            var endDateFor2018 = model.UploadEndDate;

            //DateTime previousUploadStartDate = new DateTime(year, 1, 1);
            //DateTime previousUploadEndDate = new DateTime(year, 12, 31);


            var inDBAnnualUpload2019 = _annualSalesDataUploadRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId
                                    && x.PeriodStartDate == lastStartDate2019 && x.PeriodEndDate == lastEndDate2019
                                    && x.StatusId == (long)SalesDataUploadStatus.Parsed && x.AuditActionId != (long)AuditActionType.Rejected).ToList();

            var anyUploadThisYear = _salesDataUploadRepository.Table.Where(x => x.FranchiseeId == model.FranchiseeId
                                        && (x.PeriodStartDate.Year == startDate.Year || x.PeriodEndDate.Year == startDate.Year)).Any();

            if (!inDBAnnualUpload2019.Any() && (model.PeriodEndDate.Year == DateTime.Now.Year))
            {
                annualUploadYears += lastEndDate2019.Year + ",";
            }
            if (inDBAnnualUpload2019.Any())
            {
                model.IsAnnualUpload = true;
            }


            if (annualUploadYears.Length > 1)
                annualUploadYears = annualUploadYears.Remove(annualUploadYears.Length - 1);


            annualUploadYears = annualUploadYears.Replace(",", " and ");
            model.AnnualUploadYears = annualUploadYears;

            if ((model.FranchiseeId == 91 || model.FranchiseeId == 22 || model.FranchiseeId == 17 || model.FranchiseeId == 6 || model.FranchiseeId == 10
                          || model.FranchiseeId == 27 || model.FranchiseeId == 45 || model.FranchiseeId == 72 || model.FranchiseeId == 38
                             || model.FranchiseeId == 70 || model.FranchiseeId == 78 || model.FranchiseeId == 77
                             || model.FranchiseeId == 79 || model.FranchiseeId == 3 || model.FranchiseeId == 67) && !inDBAnnualUpload2019.Any())
            {
                model.IsAnnualUpload = true;
            }
            // End of changes to be Deleted

            if (model.PeriodStartDate.Year < _clock.ToUtc(DateTime.Now).Year)
            {
                model.IsAnnualUpload = true;
            }
            return model;
        }

        public DateTime GetLastUploadDateOfYear(DateTime lastUploadStartDate, long? paymentFrequencyId)
        {
            if (paymentFrequencyId == null || paymentFrequencyId == (long)PaymentFrequency.Monthly)
                return GetLastMonthUploadDate(lastUploadStartDate);
            else
                return GetLastWeekUploadDate(lastUploadStartDate);
        }

        private DateTime GetLastWeekUploadDate(DateTime lastUploadStartDate)
        {
            const int lastMonthOfYear = 12;
            var daysInMonth = DateTime.DaysInMonth(lastUploadStartDate.Year, lastMonthOfYear);
            for (int day = daysInMonth; day > 0; day--)
            {
                DateTime currentDateTime = new DateTime(lastUploadStartDate.Year, lastMonthOfYear, day);
                if (currentDateTime.DayOfWeek == DayOfWeek.Monday)
                    return currentDateTime;
            }
            return lastUploadStartDate;
        }

        private DateTime GetLastMonthUploadDate(DateTime lastUploadStartDate)
        {
            const int lastMonthOfYear = 12;
            lastUploadStartDate = new DateTime(lastUploadStartDate.Year, lastMonthOfYear, 01);
            return lastUploadStartDate;
        }

        static DateTime GetThirdMondayOfMarch(int year)
        {
            // Step 1: Get March 1st of the given year
            DateTime firstDayOfMarch = new DateTime(year, 3, 1);

            // Step 2: Find the first Monday
            int offset = (int)DayOfWeek.Monday - (int)firstDayOfMarch.DayOfWeek;
            if (offset < 0) offset += 7; // Adjust if March 1st is after Monday

            DateTime firstMonday = firstDayOfMarch.AddDays(offset);

            // Step 3: Move forward two more Mondays (i.e., 14 days)
            DateTime thirdMonday = firstMonday.AddDays(14);

            return thirdMonday;
        }
    }
}
