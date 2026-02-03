using Core.Sales.ViewModel;
using Core.Application;
using Core.Sales.Domain;
using Core.Application.Attribute;
using System.Linq;
using Core.Application.ViewModel;
using Core.Organizations.Domain;
using Core.Users.Enum;
using System.Text.RegularExpressions;
using Core.Application.Impl;
using System.Collections.Generic;
using System;
using Core.Geo.ViewModel;
using Core.Geo;
using Core.MarketingLead.Domain;
using Core.Geo.Domain;
using Core.Reports.Domain;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerFactory _customerFactory;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IRepository<CustomerFileUpload> _customerFileUploadRepository;
        private readonly IFileService _fileService;
        private readonly IAddressFactory _addressFactory;
        private readonly IEmailFactory _emailFactory;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<Zip> _zipRepository;
        private readonly IRepository<CustomerEmailAPIRecord> _customerEmailApiRepository;
        private ILogService _logService;

        public CustomerService(IUnitOfWork unitOfWork, ICustomerFactory customerFactory, ISortingHelper sortingHelper, IExcelFileCreator excelFileCreator,
            IFileService fileService, IAddressFactory addressFactory, IEmailFactory emailFactory, ILogService logService)
        {
            _customerRepository = unitOfWork.Repository<Customer>();
            _customerFactory = customerFactory;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _sortingHelper = sortingHelper;
            _excelFileCreator = excelFileCreator;
            _fileService = fileService;
            _customerFileUploadRepository = unitOfWork.Repository<CustomerFileUpload>();
            _addressFactory = addressFactory;
            _emailFactory = emailFactory;
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _addressRepository = unitOfWork.Repository<Address>();
            _cityRepository = unitOfWork.Repository<City>();
            _stateRepository = unitOfWork.Repository<State>();
            _countryRepository = unitOfWork.Repository<Country>();
            _zipRepository = unitOfWork.Repository<Zip>();
            _customerEmailApiRepository = unitOfWork.Repository<CustomerEmailAPIRecord>();
            _logService = logService;
        }

        public CustomerEditModel Get(long id)
        {
            if (id == 0)
            {
                return new CustomerEditModel();
            }
            var customerInfo = _customerRepository.Table.Where(x => x.Id == id).FirstOrDefault();
            var customer = _customerRepository.Get(id);

            var customerEditModel = new CustomerEditModel
            {
                Name = customerInfo.Name,
                MarketingClass = customer.MarketingClass != null ? customer.MarketingClass.Name : customerInfo.MarketingClass.Name,
                PhoneNumber = customerInfo.Phone,
                Address = _addressFactory.CreateEditModel(customerInfo.Address),
                Emails = customerInfo.CustomerEmails.Select(x => _emailFactory.CreateEditModel(x)).ToList(),
                // Franchisee = customerInfo..Name,
                ContactPerson = customerInfo.ContactPerson,
                Id = customerInfo.Id,
                MarketingClassId = customer.MarketingClass != null ? customer.MarketingClass.Id : customerInfo.MarketingClass.Id,
                AddressId = customerInfo.AddressId,
                DataRecorderMetaData = customerInfo.DataRecorderMetaData,
                ReceiveNotification = customerInfo.ReceiveNotification
            };
            if (!customerEditModel.Emails.Any())
                customerEditModel.Emails = new List<EmailEditModel> { new EmailEditModel() };
            return customerEditModel;
        }

        public void Save(CustomerEditModel model)
        {

            var inDBCustomer = _customerRepository.Get(model.Id);
            if (inDBCustomer != null)
                model.DateCreated = inDBCustomer.DateCreated;

            var inDBCustomerEmails = _customerEmailRepository.Table.Where(x => x.CustomerId == model.Id).ToList();
            var emailIds = model.Emails.Where(x => x.Id > 0).Select(x => x.Id).ToArray();
            var countryId = _countryRepository.Table.Where(x => x.Name == model.Address.Country).Select(x => x.Id).FirstOrDefault();
            var stateId = _stateRepository.Table.Where(x => x.Name == model.Address.State).Select(x => x.Id).FirstOrDefault();
            var cityId = _stateRepository.Table.Where(x => x.Name == model.Address.City).Select(x => x.Id).FirstOrDefault();
            var zipId = _zipRepository.Table.Where(x => x.Code == model.Address.ZipCode).Select(x => x.Id).FirstOrDefault();

            var addressEditModel = new AddressEditModel
            {
                AddressType = model.Address.AddressType,
                AddressLine1 = model.Address.AddressLine1,
                AddressLine2 = model.Address.AddressLine2,
            };

            addressEditModel.Country = model.Address.Country;
            addressEditModel.CountryId = countryId;

            if (stateId == default(long))
            {
                addressEditModel.State = model.Address.State;
            }
            else
            {
                addressEditModel.StateId = stateId;
            }

            if (cityId == default(long))
            {
                addressEditModel.City = model.Address.City;
            }
            else
            {
                addressEditModel.CityId = cityId;
            }

            if (zipId == default(long))
            {
                addressEditModel.ZipCode = model.Address.ZipCode;
            }
            else
            {
                addressEditModel.ZipId = zipId;
            }

            if (_customerRepository.Table.Where(x => x.AddressId == model.AddressId).ToList().Count() == 1)
            {
                model.Address.CountryId = addressEditModel.CountryId;
                model.Address.Country = addressEditModel.Country;
                model.Address.StateId = addressEditModel.StateId;
                model.Address.State = addressEditModel.State;
                model.Address.City = addressEditModel.City;
                model.Address.CityId = addressEditModel.CityId;
                model.Address.ZipCode = addressEditModel.ZipCode;
                model.Address.ZipId = addressEditModel.ZipId;
            }

            else if (_customerRepository.Table.Where(x => x.AddressId == model.AddressId).ToList().Count() > 1)
            {
                var addressModels = _customerRepository.Table.Where(x => x.Id == model.Id).Select(x => x.Address).FirstOrDefault();

                var addressDomanin = _customerFactory.CreateDomain(addressEditModel);
                _addressRepository.Save(addressDomanin);
                model.AddressId = addressDomanin.Id;
                model.Address.Id = addressDomanin.Id;
                model.Address.CountryId = addressEditModel.CountryId;
                model.Address.Country = addressEditModel.Country;
                model.Address.StateId = addressEditModel.StateId;
                model.Address.State = addressEditModel.State;
                model.Address.City = addressEditModel.City;
                model.Address.CityId = addressEditModel.CityId;
                model.Address.ZipCode = addressEditModel.ZipCode;
                model.Address.ZipId = addressEditModel.ZipId;
            }
            foreach (var item in inDBCustomerEmails)
            {
                if (!emailIds.Contains(item.Id))
                {
                    var emailToDelete = inDBCustomerEmails.Where(x => x.Id == item.Id).FirstOrDefault();
                    if (emailToDelete != null)
                        _customerEmailRepository.Delete(emailToDelete);
                }
            }

            //var customer = _customerFactory.CreateDomain(model);
            var customer = _customerFactory.CreateCustomerDomain(model);
            _customerRepository.Save(customer);
        }


        public bool DoesCustomerExists(string email)
        {
            return _customerRepository.Count(x => x.CustomerEmails.Any(e => e.Email.Equals(email))) > 0;
        }

        public IEnumerable<Customer> GetCustomerByEmail(IList<string> emails, List<Customer> customerRepository)
        {
            try
            {
                //return _customerRepository.Fetch(x => x.CustomerEmails.Any(e => emails.Contains(e.Email)));
                return customerRepository.Where(x => x.CustomerEmails.Any(e => emails.Contains(e.Email))).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IEnumerable<Customer> GetCustomerByPhone(string phone, List<Customer> customerRepository)
        {
            try
            {
                phone = Regex.Replace(phone, @"[^\d]", "");
                //return _customerRepository.Fetch(x => x.Phone.Equals(phone));
                return customerRepository.Where(x => x.Phone.Equals(phone)).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Customer GetCustomerByNameAndAddress(string name, AddressEditModel address, List<Customer> customerRepository)
        {
            try
            {
                return customerRepository.Where(x =>
                x.Name.ToLower().Equals(name.ToLower()) &&
                x.Address != null &&
                (string.IsNullOrEmpty(address.AddressLine1) || string.IsNullOrEmpty(x.Address.AddressLine1) || address.AddressLine1 == x.Address.AddressLine1)
                &&
                (string.IsNullOrEmpty(address.AddressLine2) || string.IsNullOrEmpty(x.Address.AddressLine2) || address.AddressLine2 == x.Address.AddressLine2)
                &&
                (string.IsNullOrEmpty(address.City) || (string.IsNullOrEmpty(x.Address.CityName) && x.Address.City == null) || address.City == x.Address.CityName || address.City == x.Address.City.Name)
                &&
                (address.StateId < 1 || x.Address.StateId == null || address.StateId == x.Address.StateId)
                &&
                (string.IsNullOrEmpty(address.ZipCode) || (string.IsNullOrEmpty(x.Address.ZipCode) && x.Address.Zip == null) || address.ZipCode == x.Address.ZipCode || address.ZipCode == x.Address.Zip.Code)
            ).FirstOrDefault();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public Customer SaveCustomer(CustomerCreateEditModel model)
        {
            var indbCustomer = _customerRepository.Table.FirstOrDefault(x => x.Id == model.Id);
            var customer = _customerFactory.CreateDomain(model, indbCustomer);
            _customerRepository.Save(customer);

            return customer;
        }


        public CustomerListModel GetCustomers(CustomerListFilter filter, int pageNumber, int pageSize)
        {
            var customers = CustomerFilterList(filter);

            var finalcollection = customers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new CustomerListModel
            {
                Collection = finalcollection.Select(_customerFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, customers.Count())
            };
        }

        private IQueryable<Customer> CustomerFilterList(CustomerListFilter filter)
        {
            IQueryable<Customer> customerDomain = null;
            var endDate = filter.DateModified.HasValue ? filter.DateModified.Value.AddTicks(-1).AddDays(1) : (DateTime?)null;
            var toDate = filter.ToDate.HasValue ? filter.ToDate.Value.AddDays(1) : (DateTime?)null;

            var idConverted = 0;
            var receiveNotificationFilterVal = false;
            int.TryParse(filter.Text, out idConverted);
            bool.TryParse(filter.Text, out receiveNotificationFilterVal);

            if (filter.AdvancedSearchBy == null)
            {
                var query = from c in _customerRepository.Table
                            join fs in _franchiseeSalesRepository.Table
                                on c.Id equals fs.CustomerId into fsGroup
                            select new { c, fsGroup };

                customerDomain = query.Where(q =>
                        (filter.FranchiseeId <= 1 ||
                         q.fsGroup.Any(fs => fs.FranchiseeId == filter.FranchiseeId))
                    && ((filter.FromDate == null && toDate == null) ||
                         q.fsGroup.Any(fs => fs.Invoice != null &&
                                             fs.Invoice.GeneratedOn >= filter.FromDate &&
                                             fs.Invoice.GeneratedOn <= toDate))
                    && (string.IsNullOrEmpty(filter.Text) ||
                         q.c.Name.Contains(filter.Text) ||
                         q.c.CustomerEmails.Any(m => m.Email.Contains(filter.Text)) ||
                         q.c.Address.AddressLine1.Contains(filter.Text) ||
                         q.c.Address.AddressLine2.Contains(filter.Text) ||
                         q.c.Address.City.Name.Contains(filter.Text) ||
                         q.c.Address.State.Name.Contains(filter.Text) ||
                         q.c.Address.CityName.Contains(filter.Text) ||
                         q.c.Address.StateName.Contains(filter.Text) ||
                         q.c.Address.Zip.Code.Contains(filter.Text) ||
                         q.c.Address.Country.Name.Contains(filter.Text) ||
                         q.c.Phone.Contains(filter.Text) ||
                         q.c.MarketingClass.Name.Contains(filter.Text) ||
                         q.c.Id == idConverted ||
                         q.c.ContactPerson.Equals(filter.Text))
                    && (string.IsNullOrEmpty(filter.ReceiveNotification) ||
                         q.c.ReceiveNotification == receiveNotificationFilterVal)
                    && (filter.DateCreated == null || q.c.DateCreated >= filter.DateCreated)
                    && (endDate == null || q.c.DateCreated <= endDate)
                    )
                    .Select(q => q.c)
                    .Distinct();
            }
            else
            {
                customerDomain = _customerRepository.Table.Where(x =>
                        (filter.FranchiseeId <= 1 ||
                            (from fs in _franchiseeSalesRepository.Table where fs.FranchiseeId == filter.FranchiseeId && fs.CustomerId == x.Id select fs).Any())

                        && ((filter.FromDate == null && toDate == null) || (from fs in _franchiseeSalesRepository.Table
                                                                            where fs.Invoice != null && fs.Invoice.GeneratedOn >= filter.FromDate && fs.Invoice.GeneratedOn <= toDate && fs.CustomerId == x.Id
                                                                            select fs).Any())

                        && ((filter.AdvancedSearchBy == "Email" ? (x.CustomerEmails.Any(m => m.Email.Contains(filter.AdvancedText))) : true)
                        && (filter.AdvancedSearchBy == "City" ? x.Address.City.Name.Contains(filter.AdvancedText) : true)
                        && (filter.AdvancedSearchBy == "State" ? x.Address.State.Name.Contains(filter.AdvancedText) : true)
                        && (filter.AdvancedSearchBy == "PhoneNumber" ? x.Phone.Contains(filter.AdvancedText.Replace("-", string.Empty)) : true)
                        && (filter.AdvancedSearchBy == "Class" ? x.MarketingClass.Name.Contains(filter.AdvancedText) : true)
                        && (filter.AdvancedSearchBy == "Name" ? x.Name.Contains(filter.AdvancedText) : true)
                        && (filter.AdvancedSearchBy == "ContactPerson" ? x.ContactPerson.Contains(filter.AdvancedText) : true)
                        && (filter.AdvancedSearchBy == "ZipCode" ? x.Address.ZipCode.Contains(filter.AdvancedText) : true))

                        && (filter.DateCreated == null || (x.DateCreated >= filter.DateCreated))
                        && (endDate == null || (x.DateCreated <= endDate))).Distinct();
            }

            var customers = customerDomain.AsQueryable();
            customers = _sortingHelper.ApplySorting(customers, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Id, filter.SortingOrder);
                        break;
                    case "Name":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Name, filter.SortingOrder);
                        break;
                    case "Email":
                        customers = _sortingHelper.ApplySorting(customers, x => x.CustomerEmails.FirstOrDefault().Email, filter.SortingOrder);
                        break;
                    case "Phone":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Phone, filter.SortingOrder);
                        break;
                    case "StreetAddress":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Address.AddressLine1, filter.SortingOrder);
                        break;
                    case "City":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Address.City.Name, filter.SortingOrder);
                        break;
                    case "State":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Address.State.Name, filter.SortingOrder);
                        break;
                    case "ZipCode":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Address.ZipCode, filter.SortingOrder);
                        break;
                    case "Country":
                        customers = _sortingHelper.ApplySorting(customers, x => x.Address.Country.Name, filter.SortingOrder);
                        break;
                    case "MarketingClass":
                        customers = _sortingHelper.ApplySorting(customers, x => x.MarketingClass.Name, filter.SortingOrder);
                        break;
                    case "DateCreated":
                        customers = _sortingHelper.ApplySorting(customers, x => x.DateCreated, filter.SortingOrder);
                        break;
                    case "DateModified":
                        customers = _sortingHelper.ApplySorting(customers, x => x.DataRecorderMetaData.DateModified, filter.SortingOrder);
                        break;
                    case "ContactName":
                        customers = _sortingHelper.ApplySorting(customers, x => x.ContactPerson, filter.SortingOrder);
                        break;
                    case "TotalSales":
                        customers = _sortingHelper.ApplySorting(customers, x => x.TotalSales, filter.SortingOrder);
                        break;
                    case "AvgSales":
                        customers = _sortingHelper.ApplySorting(customers, x => x.AvgSales, filter.SortingOrder);
                        break;
                }
            }
            return customers;
        }

        public bool DownloadCustomerFile(CustomerListFilter filter, out string fileName)
        {
            fileName = string.Empty;
            var customerCollection = new List<CustomerViewModel>();
            var customers = CustomerFilterList(filter);
            //todo: implement paging to pull data from DB.

            //var customers = _customerRepository.Table.Where(x => customerIds.Contains(x.Id)).ToList();
            var cust = customers.ToList();
            List<long> customersIds = cust.Select(x => x.Id).ToList();
            var franchiseeSales = _franchiseeSalesRepository.Table.Where(x => customersIds.Contains(x.CustomerId)).ToList();
            var customerEmailApis = _customerEmailApiRepository.Table.Where(x => customersIds.Contains(x.CustomerId)).ToList();
            //prepare item collection
            foreach (var item in cust)
            {
                try
                {
                    var franchiseeSale = franchiseeSales.Where(x => x.CustomerId == item.Id && x.Invoice != null).OrderByDescending(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();                
                    var customerEmailApi = customerEmailApis.Where(x => x.CustomerId == item.Id && x.IsSynced).ToList().Any();                
                    var model = _customerFactory.CreateCustomerViewModel(item, franchiseeSale, customerEmailApi);                
                    customerCollection.Add(model);
                }
                catch(Exception ex)
                {
                    _logService.Error("Error is Customer Report Download: " + ex);
                }
            }

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/customer-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(customerCollection, fileName);
        }

        public void Save(CustomerFileUploadCreateModel model)
        {
            var customerFileUpload = _customerFactory.CreateCustomerFileUpload(model);
            var file = _fileService.SaveModel(model.File);
            customerFileUpload.FileId = file.Id;
            _customerFileUploadRepository.Save(customerFileUpload);
        }

        public bool UpdateMarketingClass(long id, long classTypeId)
        {
            var customer = _customerRepository.Get(id);
            if (customer != null)
            {
                customer.ClassTypeId = classTypeId;
                _customerRepository.Save(customer);
                return true;
            }
            return false;
        }

    }
}
