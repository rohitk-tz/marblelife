using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Billing.Domain;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class CustomerFileUploadPollingAgent : ICustomerFileUploadPollingAgent
    {
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private ILogService _logService;
        private IClock _clock;
        private IFileService _fileService;
        private IRepository<CustomerFileUpload> _customerFileUploadRepository;
        private IRepository<State> _stateRepository;
        private IRepository<City> _cityRepository;
        private IRepository<Customer> _customerRepository;
        private IRepository<CustomerEmail> _customerEmailRepository;
        private IRepository<InvoiceAddress> _invoiceAddressRepository;
        private IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private IUnitOfWork _unitOfWork;
        private readonly ISettings _settings;
        private readonly ICustomerService _customerService;
        private readonly ISalesInvoiceFactory _salesInvoiceFactory;
        public CustomerFileUploadPollingAgent(IUnitOfWork unitOfWork, ILogService logService, IFileService fileService, ISettings settings,
            ICustomerService customerService, IClock clock, ISalesInvoiceFactory salesInvoiceFactory, IFranchiseeSalesService franchiseeSalesService)
        {
            _unitOfWork = unitOfWork;
            _customerFileUploadRepository = unitOfWork.Repository<CustomerFileUpload>();
            _logService = logService;
            _fileService = fileService;
            _clock = clock;
            _settings = settings;
            _customerService = customerService;
            _customerRepository = unitOfWork.Repository<Customer>();
            _salesInvoiceFactory = salesInvoiceFactory;
            _invoiceAddressRepository = unitOfWork.Repository<InvoiceAddress>();
            _stateRepository = unitOfWork.Repository<State>();
            _cityRepository = unitOfWork.Repository<City>();
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _franchiseeSalesService = franchiseeSalesService;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
        }
        public void ParseCustomerFile()
        {
            // get customer file to parse
            var customerFileToParse = _customerFileUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded)
                .OrderBy(x => x.DataRecorderMetaData.DateCreated).FirstOrDefault();

            if (customerFileToParse == null)
            {
                _logService.Debug("No Customer file found for parsing");
                return;
            }
            // Update status to parseInProgress
            customerFileToParse.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
            _customerFileUploadRepository.Save(customerFileToParse);

            _unitOfWork.SaveChanges();

            DataTable data;
            IList<CustomerCreateEditModel> collection;
            var sb = new StringBuilder();
            var customerName = "";
            var customerId = default(long);
            try
            {
                var filePath = MediaLocationHelper.FilePath(customerFileToParse.File.RelativeLocation, customerFileToParse.File.Name).ToFullPath();
                data = CustomerExcelFileParser.ReadExcel(filePath);

                var customerFileParser = ApplicationManager.DependencyInjection.Resolve<ICustomerFileParser>();
                collection = customerFileParser.PrepareDomainFromDataTable(data);

                foreach (var record in collection)
                {

                    if (record.Status == 1)
                    {
                        _logService.Info(string.Format("Changing Data For Customer :{0} ", record.Name));
                        customerName = record.Name;
                        customerId = record.Id;
                        _unitOfWork.StartTransaction();
                        var stats = SaveModel(record, customerFileToParse);
                        sb.Append(stats.Logs);

                        _unitOfWork.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Updating Customer Information having Name {0} and Id {1} : ", customerName, customerId));
                _logService.Error("Exception: ", ex);
                _unitOfWork.Rollback();
            }

            //Update Customer file upload
            customerFileToParse.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            customerFileToParse.StatusId = (long)SalesDataUploadStatus.Parsed;
            SaveCustomerFileUpload(customerFileToParse);
        }

        private void SaveCustomerFileUpload(CustomerFileUpload customerFileUpload)
        {
            try
            {
                _unitOfWork.StartTransaction();

                _customerFileUploadRepository.Save(customerFileUpload);

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        private SaveModelStats SaveModel(CustomerCreateEditModel record, CustomerFileUpload customerDataUpload)
        {
            var stats = new SaveModelStats();
            var customer = _customerRepository.Get(record.Id);

            var franchiseeSales = _franchiseeSalesRepository.Table.Where(x => x.CustomerId == customer.Id && x.Invoice != null).OrderByDescending(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();
            if (franchiseeSales != null)
            {
                var franchiseeSale = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(franchiseeSales.InvoiceId.GetValueOrDefault());
                if (franchiseeSale != null)
                {
                    franchiseeSale.SubClassTypeId = record.SubMarketingClassId;
                    franchiseeSale.ClassTypeId = record.MarketingClassId.GetValueOrDefault();
                    franchiseeSale.IsNew = false;
                    _franchiseeSalesRepository.Save(franchiseeSale);
                }
            }
            if (customer != null)
                customer.ClassTypeId = record.MarketingClassId.GetValueOrDefault();

            if (franchiseeSales != null && customer != null)
            {
                var franchiseeCustomer = franchiseeSales.Customer;
                if (franchiseeCustomer != null)
                    SaveLastInvoice(franchiseeSales.InvoiceId, record.Address, franchiseeCustomer.Phone, franchiseeCustomer.CustomerEmails.ToList());
            }
            var customerDomain = SaveCustomerInfo(customer, record.Address);
            customer.Name = record.Name;

            if (customerDomain != default(Customer))
                _customerRepository.Save(customer);

            stats.CustomerId = customer.Id;
            return stats;
        }

        private Customer SaveCustomerInfo(Customer customer, AddressEditModel address)
        {
            try
            {
                var addressDomain = _salesInvoiceFactory.CreateDomain(address);
                customer.Address = addressDomain;
                return customer;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Updating Customer Information {0} having Customer Id {1}  : ", ex.Message, customer.Id));
                _unitOfWork.Rollback();
                return new Customer();
            }
        }
        private bool SaveLastInvoice(long? lastInvoiceId, AddressEditModel address, string phoneNumber, List<CustomerEmail> emails)
        {
            var invoiceAddress = _invoiceAddressRepository.Table.Where(x => x.InvoiceId == lastInvoiceId).FirstOrDefault();
            var stateDomain = _stateRepository.Get(address.StateId);
            var cityDomain = _cityRepository.Get(address.CityId.GetValueOrDefault());
            var emailId = "";
            if (emails.Count() > 0)
            {
                foreach (var email in emails)
                {
                    emailId += email.Email + " , ";
                }
            }
            if (emailId != "")
            {
                var index = emailId.LastIndexOf(',');
                if (index != -1)
                {
                    emailId = emailId.Substring(0, index);
                }
            }
            if (invoiceAddress != null)
            {
                invoiceAddress.AddressLine1 = address != null ? address.AddressLine1 : null;
                invoiceAddress.AddressLine2 = address != null ? address.AddressLine2 : null;
                invoiceAddress.CountryId = invoiceAddress != null ? invoiceAddress.CountryId : default(long?);
                invoiceAddress.CityId = address != null && address.CityId != default(long?) && address.CityId != default(long) ? address.CityId : default(long?);
                invoiceAddress.StateId = address != null && address.StateId != default(long) && address.StateId != null ? address.StateId : default(long?);
                invoiceAddress.ZipId = address != null ? address.ZipId : default(long?);
                invoiceAddress.ZipCode = address != null ? address.ZipCode : null;
                invoiceAddress.CityName = cityDomain != null ? cityDomain.Name : address.City;
                invoiceAddress.StateName = stateDomain != null ? stateDomain.Name : address.State;
                invoiceAddress.CountryId = address != null ? address.CountryId : default(long?);
                invoiceAddress.Phone = phoneNumber;
                invoiceAddress.EmailId = emailId;
            }
            try
            {
                if (invoiceAddress != null)
                    _invoiceAddressRepository.Save(invoiceAddress);
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Updating Customer Information {0} having Invoice Id {1}  : ", ex.Message, lastInvoiceId));
                _unitOfWork.Rollback();
                return false;
            }
        }
        class SaveModelStats
        {
            public long CustomerId;
            public string Logs;
        }

    }
}
