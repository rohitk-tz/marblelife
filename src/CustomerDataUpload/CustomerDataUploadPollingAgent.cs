using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Geo;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Organizations.Domain;
using Core.Sales;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace CustomerDataUpload
{
    [DefaultImplementation]
    public class CustomerDataUploadPollingAgent : ICustomerDataUploadPollingAgent
    {
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();
        private readonly ILogService _logService;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private IUnitOfWork _unitOfWork;
        private List<ParsedFileParentModel> _parentModelCollection = new List<ParsedFileParentModel>();
        private IStateService _stateService;
        private readonly ICustomerService _customerService;
        private List<MarketingClass> _marketingClasses;
        private List<ServiceType> _serviceTypes;
        private CustomerCreateEditModel customerModel = new CustomerCreateEditModel();
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private IFileService _fileService;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<Customer> _customerRepository;

        public CustomerDataUploadPollingAgent(IUnitOfWork unitOfWork, IFileService fileService, ILogService logService, IStateService stateService,
            ICustomerService customerService)
        {
            _logService = logService;
            _stateService = stateService;
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _customerService = customerService;
            _unitOfWork = unitOfWork;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _fileService = fileService;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _customerRepository = unitOfWork.Repository<Customer>();

            _marketingClasses = unitOfWork.Repository<MarketingClass>().Table.ToList().Select(x =>
            {
                x.Name = x.Name.ToUpper().Replace(" ", "");
                return x;
            }).ToList();

            _serviceTypes = unitOfWork.Repository<ServiceType>().Table.ToList().Select(x =>
            {
                x.Name = x.Name.ToUpper().Replace(" ", "");
                x.Alias = x.Alias;
                return x;
            }).ToList();
        }
        public void ParseCustomerData()
        {
            var salesDataUpload = _salesDataUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded && x.IsActive).OrderBy(x => x.FranchiseeId).OrderBy(x => x.PeriodStartDate).FirstOrDefault();
            if (salesDataUpload == null)
            {
                _logService.Debug("No file found for parsing");
                return;
            }
            salesDataUpload.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
            _salesDataUploadRepository.Save(salesDataUpload);

            _unitOfWork.SaveChanges();

            DataTable data;
            IList<ParsedFileParentModel> collection;
            var sb = new StringBuilder();

            try
            {
                var filePath = MediaLocationHelper.FilePath(salesDataUpload.File.RelativeLocation, salesDataUpload.File.Name).ToFullPath();
                data = ExcelFileParser.ReadExcel(filePath);

                collection = PrepareDomainFromDataTable(data);
            }
            catch (Exception ex)
            {
                sb.Append(Log("Some issue occured in File Parsing. Please check the file."));
                LogException(sb, ex);

                CreateLogFile(sb, "Customer" + salesDataUpload.Id);

                salesDataUpload.StatusId = (long)SalesDataUploadStatus.Failed;
                SaveSalesDataUpload(salesDataUpload);
                return;
            }

            var parsedRecords = 0;
            var failedRecords = 0;
            decimal totalAmount = 0;
            decimal paidAmount = 0;
            var customers = new List<long>();
            var customerRepository = _customerRepository.Table.ToList();
            foreach (var record in collection)
            {
                try
                {
                    _unitOfWork.StartTransaction();

                    var stats = SaveModel(record, salesDataUpload, customerRepository);

                    totalAmount += stats.TotalAmount;
                    paidAmount += stats.PaidAmount;

                    if (!customers.Contains(stats.CustomerId)) customers.Add(stats.CustomerId);

                    sb.Append(stats.Logs);
                    parsedRecords++;

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    failedRecords++;
                    _unitOfWork.Rollback();

                    LogException(sb, ex);
                }

                UpdateSalesDataStatus(salesDataUpload, totalAmount, paidAmount, customers.Count(), failedRecords, parsedRecords);
                CreateLogFile(sb, "Sales_" + salesDataUpload.Id);

                salesDataUpload.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                SaveSalesDataUpload(salesDataUpload);
            }
        }
        private void CreateLogFile(StringBuilder sb, string fileName)
        {
            var path = MediaLocationHelper.GetMediaLocationForLogs().Path + fileName;

            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(sb.ToString());
            }
        }
        private static void LogException(StringBuilder sb, Exception ex)
        {
            sb.Append(Log("Error - " + ex.Message));
            sb.Append(Log("Error - " + ex.StackTrace));
            if (ex.InnerException != null && ex.InnerException.StackTrace != null)
                sb.Append(Log("Error - " + ex.InnerException.StackTrace));
        }
        private void SaveSalesDataUpload(SalesDataUpload salesDataUpload)
        {
            try
            {
                _unitOfWork.StartTransaction();

                var fileModel = PrepareLogFileModel("Sales_" + salesDataUpload.Id);
                var file = _fileService.SaveModel(fileModel);
                salesDataUpload.ParsedLogFileId = file.Id;
                salesDataUpload.IsActive = false;

                _salesDataUploadRepository.Save(salesDataUpload);

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();

                throw new Exception(ex.Message);
            }
        }
        private FileModel PrepareLogFileModel(string name)
        {
            var fileModel = new FileModel();
            fileModel.Name = name;
            fileModel.Caption = name;
            fileModel.MimeType = "application/text";
            fileModel.RelativeLocation = MediaLocationHelper.GetMediaLocationForLogs().Path;
            fileModel.Size = new FileInfo(fileModel.RelativeLocation + "" + name).Length;
            return fileModel;
        }

        private void UpdateSalesDataStatus(SalesDataUpload salesDataUpload, decimal totalAmount, decimal paidAmount, int noOfCust, int failedRecords, int parsedRecords)
        {
            salesDataUpload.TotalAmount = totalAmount;
            salesDataUpload.PaidAmount = paidAmount;
            salesDataUpload.NumberOfCustomers = noOfCust;
            salesDataUpload.NumberOfFailedRecords = failedRecords;
            salesDataUpload.NumberOfParsedRecords = parsedRecords;

            salesDataUpload.StatusId = (long)(failedRecords > 0 && parsedRecords == 0 ? SalesDataUploadStatus.Failed : SalesDataUploadStatus.Parsed);
        }
        private SaveModelStats SaveModel(ParsedFileParentModel record, SalesDataUpload salesDataUpload, List<Customer> customerRepository)
        {
            long franchiseeId = salesDataUpload.FranchiseeId,
                salesDataUploadId = salesDataUpload.Id;

            var stats = new SaveModelStats();
            var franchiseeSales = new FranchiseeSales { };


            stats.Logs += Log("Starting data save for Customer " + record.Customer.Name);

            decimal totalAmount = 0;
            stats.TotalAmount = totalAmount;
            long currencyExchangeRateId = 0;

            if (record.Customer != null && record.Customer.Address != null)
            {
                var countryId = salesDataUpload.Franchisee.Organization.Address.Select(x => x.CountryId).FirstOrDefault();
                if (countryId > 0)
                {
                    record.Customer.Address.CountryId = countryId;
                }
                else record.Customer.Address.CountryId = 1;

                var currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(x => x.DateTime).First();

                if (currencyExchangeRate != null)
                    currencyExchangeRateId = currencyExchangeRate.Id;
            }

            var customer = SearchCustomer(record.Customer, stats, customerRepository);

            if (customer != null)
                FillCustomerModel(record.Customer, customer);

            franchiseeSales = new FranchiseeSales
            {
                IsNew = true,
                QbInvoiceNumber = record.QbIdentifier,
                FranchiseeId = franchiseeId,
                ClassTypeId = record.MarketingClassId,
                SalesRep = record.SalesRep,
                Amount = totalAmount,
                SalesDataUpload = salesDataUpload,
                CurrencyExchangeRateId = currencyExchangeRateId,
                DataRecorderMetaData = new DataRecorderMetaData { DateCreated = DateTime.UtcNow }
            };

            franchiseeSales.SalesDataUploadId = salesDataUploadId;

            var savedCustomer = _customerService.SaveCustomer(record.Customer);
            franchiseeSales.CustomerId = savedCustomer.Id;
            _franchiseeSalesRepository.Save(franchiseeSales);
            return stats;

        }

        private static void FillCustomerModel(CustomerCreateEditModel model, Customer inDb)
        {
            model.Id = inDb.Id;
            model.DataRecorderMetaData = inDb.DataRecorderMetaData;
            //if (model.Address != null && inDb.Address != null)
            //{
            //    model.Address.Id = inDb.Address.Id;
            //}

            //if (model.CustomerEmails != null && model.CustomerEmails.Count() > 0)
            //{
            //    model.CustomerEmails = inDb.CustomerEmails;
            //}
        }
        private static string Log(string message)
        {
            return DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss:fff tt") + " \t\t " + message + "\r\n\r\n";
        }
        private Customer SearchCustomer(CustomerCreateEditModel customer, SaveModelStats stats, List<Customer> customerRepository)
        {
            var customerColl = _customerService.GetCustomerByEmail(customer.CustomerEmails.Select(x => x.Email).ToList(), customerRepository);

            if (customerColl != null)
            {
                stats.Logs += Log(string.Format("Found existing Customer by email. Found Records {0}", string.Join(", ", customerColl.Select(x => string.Format("[Id:{0}, Name: {1}", x.Id, x.Name)).ToList())));
            }
            else
            {
                customerColl = _customerService.GetCustomerByPhone(customer.Phone, customerRepository);
                if (customerColl != null)
                {
                    stats.Logs += Log(string.Format("Found existing Customer by phone. Found Records {0}", string.Join(", ", customerColl.Select(x => string.Format("[Id:{0}, Name: {1}", x.Id, x.Name)).ToList())));
                }
            }

            if (customerColl != null && customerColl.Count() > 0 && customer.Address != null)
            {
                foreach (var item in customerColl)
                {
                    if (item.Address == null) return item;
                    if (CompareAddress(item.Address, customer.Address)) return item;
                }

                stats.Logs += Log(string.Format("Address did not match for the customers found.", customer.Id, customer.Phone));
                return null;
            }
            else if (customer.Address != null)
            {
                return _customerService.GetCustomerByNameAndAddress(customer.Name, customer.Address, customerRepository);
            }

            return customerColl.FirstOrDefault();
        }
        private bool CompareAddress(Address inDb, AddressEditModel model)
        {
            if (MatchFieldsCaseInsensitive(model.AddressLine1, inDb.AddressLine1)
                && MatchFieldsCaseInsensitive(model.AddressLine2, inDb.AddressLine2)
                && (MatchFieldsCaseInsensitive(model.City, inDb.CityName) || (inDb.City != null && MatchFieldsCaseInsensitive(model.City, inDb.City.Name)))
                && (MatchFieldsCaseInsensitive(model.ZipCode, inDb.ZipCode) || (inDb.Zip != null && MatchFieldsCaseInsensitive(model.ZipCode, inDb.Zip.Code)))
                && ((inDb.State != null && MatchFieldsCaseInsensitive(model.State, inDb.State.ShortName)) || model.StateId == inDb.StateId))
                return true;

            return false;
        }

        private bool MatchFieldsCaseInsensitive(string field1, string field2)
        {
            if (string.IsNullOrWhiteSpace(field2) || string.IsNullOrWhiteSpace(field1)) return true;
            return field1.ToLower().Trim() == field2.ToLower().Trim();
        }

        public IList<ParsedFileParentModel> PrepareDomainFromDataTable(DataTable dt)
        {
            PrepareHeaderIndex(dt);

            foreach (DataRow row in dt.Rows)
            {
                ParseRow(row);
            }

            return _parentModelCollection;
        }
        private void PrepareHeaderIndex(DataTable dt)
        {
            int index = 0;
            foreach (var col in dt.Columns)
            {
                if (col != null && col.ToString().Length > 0)
                {
                    _headersDictionary.Add(col.ToString(), index);
                }
                index++;
            }
        }
        private void ParseRow(DataRow row)
        {
            var dataInColType = ReadValueAsStringFromRowItem(row, "type");
            if (dataInColType == null || string.IsNullOrWhiteSpace(dataInColType.ToString()))
            {
                return;
            }
            CreateModels(row);
        }

        private string ReadValueAsStringFromRowItem(DataRow row, string key)
        {
            if (_headersDictionary.ContainsKey(key) == false) return null;

            var value = row.ItemArray[_headersDictionary[key]].ToString().Trim();

            if (!string.IsNullOrEmpty(value))
                return value.Trim();

            return null;
        }

        private void CreateModels(DataRow row)
        {
            ParsedFileParentModel parentModel = new ParsedFileParentModel();
            var classData = ReadValueAsStringFromRowItem(row, "class");
            long marketingClassId, serviceTypeId;
            GetMarketingClassAndServiceType(classData, out marketingClassId, out serviceTypeId);
            parentModel = PrepareDataModel(row, parentModel, marketingClassId, serviceTypeId);
        }

        private ParsedFileParentModel PrepareDataModel(DataRow row, ParsedFileParentModel parentModel, long marketingClassId, long serviceTypeId)
        {
            customerModel = CreateCustomerModel(row, marketingClassId);
            parentModel.Customer = customerModel;

            parentModel.MarketingClassId = marketingClassId;
            parentModel.ServiceTypeId = serviceTypeId;

            _parentModelCollection.Add(parentModel);

            return parentModel;
        }

        private void GetMarketingClassAndServiceType(string classData, out long marketingClassId, out long serviceTypeId)
        {
            marketingClassId = (long)MarketingClassType.Residential;
            serviceTypeId = (long)ServiceTypes.Stonelife;

            var dataToParse = string.Empty;
            if (!string.IsNullOrWhiteSpace(classData))
            {
                dataToParse = classData;
            }

            if (string.IsNullOrWhiteSpace(dataToParse))
            {
                return;
            }

            var data = dataToParse.Split(new[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);
            long retClassId = -1, retServiceTypeId = -1;

            foreach (var item in data)
            {
                retClassId = GetMarketingClass(item);
                if (retClassId > -1)
                {
                    marketingClassId = retClassId;
                    break;
                }
            }

            foreach (var item in data)
            {
                retServiceTypeId = GetServiceTypeId(item);
                if (retServiceTypeId > -1)
                {
                    serviceTypeId = retServiceTypeId;
                    break;
                }
            }

        }
        private long GetMarketingClass(string value)
        {
            var valUpper = !string.IsNullOrWhiteSpace(value) ? value.ToUpper().Trim().Replace(" ", "") : string.Empty;
            var mclass = _marketingClasses.FirstOrDefault(x => x.Name == valUpper);
            if (mclass == null) return -1;
            return mclass.Id;
        }

        private long GetServiceTypeId(string str)
        {
            var valUpper = !string.IsNullOrWhiteSpace(str) ? str.ToUpper().Trim().Replace(" ", "") : string.Empty;
            var serviceType = _serviceTypes.FirstOrDefault(x => x.Name == valUpper || DoesAliasContainsString(x.Alias, valUpper));

            if (serviceType == null) return -1;
            return serviceType.Id;
        }
        private bool DoesAliasContainsString(string alias, string str)
        {
            if (alias == null) return false;

            var arr = alias.Split(',');
            var arrToCompare = arr.Select(x => x.Replace(" ", string.Empty).ToUpper());
            if (arrToCompare.Any(str.Equals))
            {
                return true;
            }

            return false;
        }

        private CustomerCreateEditModel CreateCustomerModel(DataRow row, long marketingClassId)
        {
            var model = new CustomerCreateEditModel();

            model.MarketingClassId = marketingClassId;
            model.Name = ReadValueAsStringFromRowItem(row, "name");

            //if col name is Name Contact
            if (model.Name == null)
                model.Name = ReadValueAsStringFromRowItem(row, "name contact");

            var phone = ReadValueAsStringFromRowItem(row, "name phone #");
            model.Phone = ParsePhone(phone);

            var emailColumnValue = ReadValueAsStringFromRowItem(row, "name e-mail");

            if (!string.IsNullOrEmpty(emailColumnValue))
            {
                var emails = emailColumnValue.Split(';');
                if (emails.Length == 1)
                    emails = emails[0].Split(',');

                foreach (var email in emails)
                {
                    model.CustomerEmails.Add(new CustomerEmail { Email = email });
                }
            }

            var line1 = ReadValueAsStringFromRowItem(row, "name street1");
            var line2 = ReadValueAsStringFromRowItem(row, "name street2");
            var city = ReadValueAsStringFromRowItem(row, "name city");
            var stateShortName = ReadValueAsStringFromRowItem(row, "name state");
            var zipCode = ReadValueAsStringFromRowItem(row, "name zip");


            if (string.IsNullOrWhiteSpace(line1) && string.IsNullOrWhiteSpace(line2) && string.IsNullOrWhiteSpace(city)
                && string.IsNullOrWhiteSpace(stateShortName) && string.IsNullOrWhiteSpace(zipCode))
            {
                model.Address = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(stateShortName) && !string.IsNullOrWhiteSpace(city) && city.LastIndexOf(" ") > 0)
                {
                    var subStr = city.Substring(city.LastIndexOf(" ") + 1);
                    if (subStr.Length == 2)
                    {
                        stateShortName = subStr;
                    }
                }

                var stateId = _stateService.GetStateIdByShortName(stateShortName);

                model.Address = new AddressEditModel()
                {
                    AddressLine1 = string.IsNullOrWhiteSpace(line1) ? "" : line1,
                    AddressLine2 = line2,
                    City = city,
                    ZipCode = zipCode,
                    StateId = stateId
                };
            }
            return model;
        }

        private string ParsePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return "";

            return Regex.Replace(phone, @"[^\d]", "");
        }

        class SaveModelStats
        {
            public long CustomerId;
            public decimal TotalAmount;
            public decimal PaidAmount;

            public string Logs;
        }
    }
}
