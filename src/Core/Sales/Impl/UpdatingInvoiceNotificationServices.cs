using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Geo.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using Core.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class UpdatingInvoiceNotificationServices : IUpdatingInvoiceNotificationServices
    {
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IJobFactory _jobFactory;
        private readonly IRepository<UpdateMarketingClassfileupload> _updateMarketingClassfileuploadRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        private readonly IRepository<MarketingClass> _marketingClassRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<Zip> _zipRepository;
        private IFileService _fileService;
        private readonly IRepository<InvoiceAddress> _invoiceAddressRepository;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRepository<PaymentItem> _paymentItemRepository;
        private readonly IRepository<InvoicePayment> _invoicePaymentRepository;
        private int newlyAddedZipCodes = 0;

        public UpdatingInvoiceNotificationServices(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock, IJobFactory jobFactory, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _fileService = fileService;
            _stateRepository = unitOfWork.Repository<State>();
            _cityRepository = unitOfWork.Repository<City>();
            _updateMarketingClassfileuploadRepository = unitOfWork.Repository<UpdateMarketingClassfileupload>();
            _jobFactory = jobFactory;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _marketingClassRepository = unitOfWork.Repository<MarketingClass>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _zipRepository = unitOfWork.Repository<Zip>();
            _invoiceAddressRepository = unitOfWork.Repository<InvoiceAddress>();
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _paymentItemRepository = unitOfWork.Repository<PaymentItem>();
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
        }


        public void ProcessRecords()
        {
            _unitOfWork.StartTransaction();

            var stats = new SaveModelStats();
            stats.Logs = new StringBuilder();
            var geoCodeParsedFile = new UpdateMarketingClassfileupload();

            _logService.Info("Starting Updating Invoice Data");
            if (!ApplicationManager.Settings.ParseUpdateInvoiceFile)
            {
                _logService.Info("Invoice Updation turned off!");
                return;
            }
            var updateInvoiceFiles = GetiInvoiceUpdationToParse();

            if (updateInvoiceFiles == null || !updateInvoiceFiles.Any())
            {
                _logService.Debug("No file found for parsing for invoice Updation");
                return;
            }
            try
            {
                long? fileId = default(long?);
                var sb = new StringBuilder();
                DataTable data;
                var updateInvoiceCollection = new List<UpdateInvoiceEditModel>();
                foreach (UpdateMarketingClassfileupload file in updateInvoiceFiles)
                {
                    try
                    {
                        fileId = file.Id;
                        file.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
                        _updateMarketingClassfileuploadRepository.Save(file);
                        _unitOfWork.SaveChanges();
                        var filePath = MediaLocationHelper.FilePath(file.File.RelativeLocation, file.File.Name).ToFullPath();
                        geoCodeParsedFile = file;
                        var sheetIds = ZipExcelFileParser.GetSheetIds(filePath);
                        CreateLogFile(sb, "UpdateSales_" + fileId);
                        SaveZipFileUpload(file, stats);
                        var isErrorExist = false;
                        string message;
                        foreach (var sheetId in sheetIds)
                        {
                            decimal totalSalesDataUpdated = 0;
                            decimal totalPaymentDataUpdated = 0;
                            _logService.Info("Started recording updating Invoice data!");
                            data = UpdateInvoiceFileParser1.ReadExcelZip(filePath, sheetId);
                            var updateInvoiceParser = ApplicationManager.DependencyInjection.Resolve<IUpdateInvoiceFileParser>();
                            updateInvoiceCollection = updateInvoiceParser.PrepareDomainFromDataTableForUpdateInvoice(data).ToList();

                            if (!updateInvoiceParser.CheckForValidHeader(data, out message))
                            {
                                stats.Logs.Append(Log("Please upload correct File! " + message));
                                sb.Append(stats.Logs);
                                CreateLogFile(sb, "UpdateSales_" + geoCodeParsedFile.Id);
                                MarkUploadAsFailed(file, stats);
                                isErrorExist = true;

                                // _unitOfWork.SaveChanges();
                                break;
                            }
                            updateInvoiceCollection = updateInvoiceCollection.Where(x => x.IsChanged == 1).ToList();
                            string invoiceId = "";
                            string serviceList = "";
                            foreach (var updateInvoice in updateInvoiceCollection)
                            {

                                if (updateInvoice.Id != 0)
                                {

                                    stats.Logs.Append(Log("Updating InvoiceId: " + updateInvoice.InvoiceId));
                                    var invoiceItem = _invoiceItemRepository.Get(updateInvoice.Id);
                                    var serviceDomain = _serviceTypeRepository.Table.FirstOrDefault(x => x.Name.ToUpper() == updateInvoice.NewService.ToUpper());
                                    if (serviceDomain != null && invoiceItem != null)
                                    {
                                        if (serviceDomain != null && invoiceItem != null)
                                        {
                                            stats.Logs.Append(Log(string.Format("Updating Franchisee Sales For {0} InvoiceId From ServiceClass {1} to {2}: ", updateInvoice.InvoiceId, invoiceItem.ServiceType.Name, serviceDomain.Name)));
                                        }
                                        invoiceItem.ItemId = serviceDomain.Id;
                                        _invoiceItemRepository.Save(invoiceItem);
                                    }
                                }
                                else
                                {
                                    var invoiceItem2 = _invoiceItemRepository.Table.Where(x => x.Description == updateInvoice.Description).ToList();
                                    var invoiceItem = _invoiceItemRepository.Table.FirstOrDefault(x => x.InvoiceId == updateInvoice.InvoiceId && x.Description == updateInvoice.Description);
                                    var serviceDomain = _serviceTypeRepository.Table.FirstOrDefault(x => x.Name.ToUpper() == updateInvoice.NewService.ToUpper());
                                    if (serviceDomain != null && invoiceItem != null)
                                    {
                                        if (serviceDomain != null && invoiceItem != null)
                                        {
                                            stats.Logs.Append(Log(string.Format("Updating Franchisee Sales For {0} InvoiceId From ServiceClass {1} to {2}: ", updateInvoice.InvoiceId, invoiceItem.ServiceType.Name, serviceDomain.Name)));
                                        }
                                        invoiceItem.ItemId = serviceDomain.Id;
                                        _invoiceItemRepository.Save(invoiceItem);
                                    }
                                    else
                                    {
                                        if (invoiceItem == null)
                                            invoiceId = invoiceId + updateInvoice.InvoiceId + ",";
                                        if (serviceDomain == null && updateInvoice.NewService != null)
                                            serviceList = serviceList + updateInvoice.NewService + ",";
                                    }

                                }
                                var franchiseeSales = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.InvoiceId == updateInvoice.InvoiceId);



                                if (franchiseeSales != null)
                                {

                                    var marketingClass = _marketingClassRepository.Table.FirstOrDefault(x => x.Name.ToUpper() == updateInvoice.NewClass.ToUpper() ||
                                    x.Name.ToUpper().StartsWith(updateInvoice.NewClass.ToUpper()));

                                    var customerDomain = _customerRepository.Table.FirstOrDefault(x => x.Id == franchiseeSales.CustomerId);
                                    if (marketingClass != null)
                                    {
                                        if (franchiseeSales.MarketingClass.Name != marketingClass.Name)
                                        {
                                            stats.Logs.Append(Log(string.Format("Updating Franchisee Sales For {0} InvoiceId From MarketingClass {1} to {2}: ", updateInvoice.InvoiceId, franchiseeSales.MarketingClass.Name, marketingClass.Name)));
                                        }
                                        totalSalesDataUpdated += franchiseeSales.Amount;
                                        franchiseeSales.ClassTypeId = marketingClass.Id;
                                        _franchiseeSalesRepository.Save(franchiseeSales);
                                    }
                                    var city = _cityRepository.Table.FirstOrDefault(x => x.Name == updateInvoice.City);

                                    if (city != null)
                                    {
                                        if (customerDomain.Address.CityId != city.Id)
                                            stats.Logs.Append(Log(string.Format("Updating Customer's City For {0} InvoiceId For Customer {1} from City {2} to {3}: ", updateInvoice.InvoiceId, franchiseeSales.Customer.Name, customerDomain.Address.CityName, city.Name)));
                                        customerDomain.Address.CityId = city.Id;
                                    }

                                    var state = _stateRepository.Table.FirstOrDefault(x => x.Name == updateInvoice.State || x.ShortName == updateInvoice.State);

                                    if (state != null)
                                    {
                                        if (customerDomain.Address.StateId != state.Id)
                                            stats.Logs.Append(Log(string.Format("Updating Customer's State For {0} InvoiceId For Customer {1} from State {2} to {3}: ", updateInvoice.InvoiceId, franchiseeSales.Customer.Name, customerDomain.Address.StateName, state.Name)));
                                        customerDomain.Address.StateId = state.Id;
                                    }

                                    var zip = _zipRepository.Table.FirstOrDefault(x => x.Code == updateInvoice.ZipCode);

                                    if (zip != null)
                                    {
                                        if (customerDomain.Address.ZipCode != zip.Code)
                                            stats.Logs.Append(Log(string.Format("Updating Customer's Zip For {0} InvoiceId For Customer {1} from Zip {2} to {3}: ", updateInvoice.InvoiceId, franchiseeSales.Customer.Name, customerDomain.Address.ZipCode, zip.Code)));
                                        customerDomain.Address.ZipId = zip.Id;
                                    }
                                    if (customerDomain.Address.AddressLine1 != updateInvoice.AddressLine1)
                                    {
                                        stats.Logs.Append(Log(string.Format("Updating Franchisee Sales For {0} InvoiceId From AddressLine1 {1} to {2}: ", updateInvoice.InvoiceId, customerDomain.Address.AddressLine1, updateInvoice.AddressLine1)));
                                    }
                                    customerDomain.Address.AddressLine1 = updateInvoice.AddressLine1;
                                    if (customerDomain.Address.AddressLine2 != updateInvoice.AddressLine2)
                                    {
                                        stats.Logs.Append(Log(string.Format("Updating Franchisee Sales For {0} InvoiceId From AddressLine2 {1} to {2}: ", updateInvoice.InvoiceId, customerDomain.Address.AddressLine2, updateInvoice.AddressLine2)));
                                    }
                                    customerDomain.Address.AddressLine2 = updateInvoice.AddressLine2;
                                    customerDomain.Address.CityName = updateInvoice.City;
                                    customerDomain.Address.StateName = updateInvoice.State;
                                    customerDomain.Address.ZipCode = updateInvoice.ZipCode;
                                    _customerRepository.Save(customerDomain);


                                    var franchiseeSalesPayment = _franchiseeSalesPaymentRepository.Table.Where(x => x.FranchiseeSalesId == franchiseeSales.Id).ToList();

                                    if (franchiseeSalesPayment.Count() > 0)
                                    {

                                        var franchiseeSalesPaymentInvoice = franchiseeSalesPayment.FirstOrDefault(x => x.InvoiceId == updateInvoice.InvoiceId);
                                        var serviceDomain = _serviceTypeRepository.Table.FirstOrDefault(x => x.Name.ToUpper() == updateInvoice.NewService.ToUpper());
                                        if (franchiseeSalesPaymentInvoice != null && serviceDomain != null)
                                        {
                                            var paymentId = franchiseeSalesPaymentInvoice.PaymentId;
                                            var paymentInvoice = _paymentItemRepository.Table.FirstOrDefault(x => x.PaymentId == paymentId);
                                            paymentInvoice.ItemId = serviceDomain.Id;
                                            _paymentItemRepository.Save(paymentInvoice);

                                            var paymentInvoiceItems = _invoicePaymentRepository.Table.Where(x => x.InvoiceId == updateInvoice.InvoiceId).ToList();
                                            var invoiceItems = _invoiceItemRepository.Table.Where(x => x.InvoiceId == updateInvoice.InvoiceId).ToList();
                                            if (invoiceItems.Count() == 1 && paymentInvoiceItems.Count() > 1)
                                            {
                                                foreach (var paymentInvoiceItem in paymentInvoiceItems)
                                                {
                                                    totalPaymentDataUpdated += paymentInvoiceItem.Payment.Amount;
                                                    paymentInvoice = _paymentItemRepository.Table.FirstOrDefault(x => x.PaymentId == paymentInvoiceItem.PaymentId);
                                                    paymentInvoice.ItemId = serviceDomain.Id;
                                                    _paymentItemRepository.Save(paymentInvoice);
                                                }
                                            }
                                        }

                                    }
                                }

                                var invoiceAddress = _invoiceAddressRepository.Table.FirstOrDefault(x => x.InvoiceId == updateInvoice.InvoiceId);
                                if (invoiceAddress != null)
                                {

                                    var city = _cityRepository.Table.FirstOrDefault(x => x.Name == updateInvoice.City);
                                    if (city != null)
                                    {
                                        if (invoiceAddress.CityId != city.Id)
                                            stats.Logs.Append(Log(string.Format("Updating Customer's City For {0} InvoiceId For Customer {1} from City {2} to {3}: ", updateInvoice.InvoiceId, franchiseeSales.Customer.Name, invoiceAddress.CityName, city.Name)));
                                        invoiceAddress.CityId = city.Id;
                                    }

                                    var state = _stateRepository.Table.FirstOrDefault(x => x.Name == updateInvoice.State);
                                    if (invoiceAddress?.StateId != state?.Id)
                                    {
                                        stats?.Logs?.Append(Log(string.Format(
                                            "Updating Customer's State For {0} InvoiceId For Customer {1} from State {2} to {3}: ",
                                            updateInvoice?.InvoiceId.ToString() ?? "NULL",
                                            franchiseeSales?.Customer?.Name ?? "UNKNOWN CUSTOMER",
                                            invoiceAddress?.StateName ?? "UNKNOWN STATE",
                                            state?.Name ?? "UNKNOWN NEW STATE"
                                        )));
                                    }

                                    if (invoiceAddress != null && state != null)
                                    {
                                        invoiceAddress.StateId = state.Id;
                                    }

                                    var zip = _zipRepository.Table.FirstOrDefault(x => x.Code == updateInvoice.ZipCode);
                                    if (zip != null)
                                    {
                                        if (invoiceAddress.ZipId != zip.Id)
                                            stats.Logs.Append(Log(string.Format("Updating Customer's Zip For {0} InvoiceId For Customer {1} from Zip {2} to {3}: ", updateInvoice.InvoiceId, franchiseeSales.Customer.Name, invoiceAddress.ZipCode, zip.Code)));
                                        invoiceAddress.ZipId = zip.Id;
                                    }

                                    invoiceAddress.AddressLine1 = updateInvoice.AddressLine1;
                                    invoiceAddress.AddressLine2 = updateInvoice.AddressLine2;
                                    invoiceAddress.CityName = updateInvoice.City;
                                    invoiceAddress.StateName = updateInvoice.State;
                                    invoiceAddress.ZipCode = updateInvoice.ZipCode;
                                    _invoiceAddressRepository.Save(invoiceAddress);
                                }

                                //Updating Description
                                if (updateInvoice.Id > 0)
                                {
                                    try
                                    {
                                        if (updateInvoice.NewDescription != null && updateInvoice.NewDescription != "")
                                        {
                                            stats.Logs.Append(Log(string.Format("Updating Description: {0} -To- {1} for Id: {2}", updateInvoice.Description, updateInvoice.NewDescription, updateInvoice.Id)));
                                            var invoiceItem = _invoiceItemRepository.Table.FirstOrDefault(x => x.Description == updateInvoice.Description && x.Id == updateInvoice.Id);
                                            if (updateInvoice.NewDescription == updateInvoice.FinalDescription && invoiceItem != null)
                                            {
                                                invoiceItem.Description = updateInvoice.NewDescription;
                                                _invoiceItemRepository.Save(invoiceItem);
                                            }
                                            else
                                            {
                                                stats.Logs.Append(Log(string.Format("New Description and Final Description is not Same or Description Is Not Available In the System for Id: {0}", updateInvoice.Id)));
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        stats.Logs.Append(Log(string.Format("Error in Updating the Description for Id: {0} and the Error is: {1}", updateInvoice.Id, ex)));
                                    }
                                }
                                _unitOfWork.SaveChanges();
                            }
                            sb.Append(stats.Logs);
                            CreateLogFile(sb, "UpdateSales_" + fileId);

                            file.StatusId = (long)SalesDataUploadStatus.Parsed;
                            _updateMarketingClassfileuploadRepository.Save(file);
                            _unitOfWork.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        file.StatusId = (long)SalesDataUploadStatus.Failed;
                        _updateMarketingClassfileuploadRepository.Save(file);
                        stats.Logs.Append(Log(string.Format("Error: File is not correct")));
                        sb.Append(stats.Logs);
                        CreateLogFile(sb, "UpdateSales_" + fileId);
                    }
                }
            }
            catch (Exception e1)
            {

            }
            finally
            {
                _unitOfWork.SaveChanges();
            }
            _logService.Info("End Updating Invoice Data");
        }

        private void MarkUploadAsFailed(UpdateMarketingClassfileupload geoCodeFile, SaveModelStats stats)
        {
            geoCodeFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            geoCodeFile.StatusId = (long)SalesDataUploadStatus.Failed;

            _updateMarketingClassfileuploadRepository.Save(geoCodeFile);
        }

        private IEnumerable<UpdateMarketingClassfileupload> GetiInvoiceUpdationToParse()
        {
            //Check if parsed and not exist in FranchiseeInvoice
            var zipDataUploadIds = _updateMarketingClassfileuploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded).ToList();
            return zipDataUploadIds;
        }
        private void CreateLogFile(StringBuilder sb, string fileName)
        {
            var path = MediaLocationHelper.GetZipMediaLocation().Path + "\\" + fileName;

            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(sb);
            }
        }
        private static string Log(string message)
        {
            return DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss:fff tt") + " \t\t " + message + "\r\n\r\n";
        }

        private void SaveZipFileUpload(UpdateMarketingClassfileupload customerFileUpload, SaveModelStats stats)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //   _unitOfWork.StartTransaction();
                var fileModel = PrepareLogFileModel("UpdateSales_" + customerFileUpload.Id);
                var file = _fileService.SaveModel(fileModel);
                customerFileUpload.ParsedLogFileId = file.Id;
                _updateMarketingClassfileuploadRepository.Save(customerFileUpload);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                stats.sb.Append(sb);
                LogException(stats.sb, ex);
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }
        private static void LogException(StringBuilder sb, Exception ex)
        {
            sb.Append(Log("Error - " + ex.Message));
            sb.Append(Log("Error - " + ex.StackTrace));
            if (ex.InnerException != null && ex.InnerException.StackTrace != null)
                sb.Append(Log("Error - " + ex.InnerException.StackTrace));
        }
        private FileModel PrepareLogFileModel(string name)
        {
            var fileModel = new FileModel();
            fileModel.Name = name;
            fileModel.Caption = name;
            fileModel.MimeType = "application/text";
            fileModel.RelativeLocation = MediaLocationHelper.GetZipMediaLocation().Path.ToRelativePath();
            fileModel.Size = new FileInfo(MediaLocationHelper.GetZipMediaLocation().Path + "\\" + name).Length;
            return fileModel;
        }
        class SaveModelStats
        {
            public long CountyId;
            public StringBuilder Logs;
            public StringBuilder LogsForCounty;
            public StringBuilder LogsForZipCode;
            public StringBuilder sb;
            public StringBuilder sbForCounties;
            public StringBuilder sbForZipCode;
        }
    }
}
