using Core.Application;
using Core.Application.Attribute;
using Core.Geo;
using Core.Geo.ViewModel;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class CustomerFileParser : ICustomerFileParser
    {
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();
        private List<CustomerCreateEditModel> _customerModelCollection = new List<CustomerCreateEditModel>();
        private IStateService _stateService;
        private List<MarketingClass> _marketingClasses;
        private List<SubClassMarketingClass> _subMarketingClasses;
        public CustomerFileParser(IUnitOfWork unitOfWork, IStateService stateService)
        {
            _stateService = stateService;

            _marketingClasses = unitOfWork.Repository<MarketingClass>().Table.ToList().Select(x =>
            {
                x.Name = x.Name.ToUpper().Replace(" ", "");
                return x;
            }).ToList();

            _subMarketingClasses = unitOfWork.Repository<SubClassMarketingClass>().Table.ToList().Select(x =>
            {
                x.Name = x.Name.ToUpper().Replace(" ", "");
                return x;
            }).ToList();
        }

        public IList<CustomerCreateEditModel> PrepareDomainFromDataTable(DataTable dt)
        {
            PrepareHeaderIndex(dt);

            foreach (DataRow row in dt.Rows)
            {
                ParseRow(row);
            }

            return _customerModelCollection;
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
            var dataInColType = ReadValueAsStringFromRowItem(row, "customerid");
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
        private long ReadValueAsLongFromRowItem(DataRow row, string key)
        {
            if (_headersDictionary.ContainsKey(key) == false) return 0;

            var value = row.ItemArray[_headersDictionary[key]].ToString().Trim();

            if (!string.IsNullOrEmpty(value))
                return Convert.ToInt64(value);

            return 0;
        }

        private void CreateModels(DataRow row)
        {
            CustomerCreateEditModel customerModel = new CustomerCreateEditModel();
            var classData = ReadValueAsStringFromRowItem(row, "marketingclass");
            long marketingClassId;
            long? subMarketingClassId;
            GetMarketingClass(classData, out marketingClassId);
            GetSubMarketingClassAndServiceType(classData, out subMarketingClassId);
            customerModel = PrepareDataModel(row, customerModel, marketingClassId, subMarketingClassId);
        }

        private CustomerCreateEditModel PrepareDataModel(DataRow row, CustomerCreateEditModel customerModel, long marketingClassId, long? subMarketingClassId)
        {
            customerModel = CreateCustomerModel(row);

            customerModel.MarketingClassId = marketingClassId;
            customerModel.SubMarketingClassId = subMarketingClassId;
            _customerModelCollection.Add(customerModel);

            return customerModel;
        }

        private void GetMarketingClass(string classData, out long marketingClassId)
        {
            marketingClassId = (long)MarketingClassType.Residential;

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
            long retClassId = -1;

            foreach (var item in data)
            {
                retClassId = GetMarketingClass(item);
                if (retClassId > -1)
                {
                    marketingClassId = retClassId;
                    break;
                }
            }
        }

        private void GetSubMarketingClassAndServiceType(string classData, out long? subMarketingClassId)
        {
            subMarketingClassId = default(long?);

            var dataToParse = string.Empty;
            if (!string.IsNullOrWhiteSpace(classData))
            {
                dataToParse = classData;
            }

            if (string.IsNullOrWhiteSpace(dataToParse))
            {
                return;
            }

            //Specifically Handle Sales Tax
            if (dataToParse.IndexOf("Sales-Tax", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                dataToParse = Regex.Replace(dataToParse, "SALES-TAX", "SALESTAX", RegexOptions.IgnoreCase);
            }

            var data = dataToParse.Split(new[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);

            const string mldClass = "0MLD";
            if (dataToParse.ToUpper().Contains(mldClass.ToUpper()))
                data = dataToParse.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            long retClassId = -1;
            var itemToExclude = string.Empty;

            foreach (var item in data)
            {
                retClassId = GetSubMarketingClass(item);
                if (retClassId > -1)
                {
                    subMarketingClassId = retClassId;
                    itemToExclude = item;
                    break;
                }
            }

        }
        private long GetMarketingClass(string value)
        {
            var valUpper = !string.IsNullOrWhiteSpace(value) ? value.ToUpper().Trim().Replace(" ", "") : string.Empty;
            var mclass = _marketingClasses.FirstOrDefault(x => x.Name == valUpper || DoesAliasContainsString(x.Alias, valUpper));
            if (mclass == null) return -1;
            return mclass.Id;
        }

        private long GetSubMarketingClass(string value)
        {
            var valUpper = !string.IsNullOrWhiteSpace(value) ? value.ToUpper().Trim().Replace(" ", "") : string.Empty;
            var mclass = _subMarketingClasses.FirstOrDefault(x => x.Name == valUpper || DoesAliasContainsString(x.Alias, valUpper));
            if (mclass == null) return -1;
            return mclass.Id;
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

        private CustomerCreateEditModel CreateCustomerModel(DataRow row)
        {
            var model = new CustomerCreateEditModel();
            model.Id = ReadValueAsLongFromRowItem(row, "customerid");
            model.Name = ReadValueAsStringFromRowItem(row, "name");

            model.QbInvoiceId = ReadValueAsStringFromRowItem(row, "qbinvoiceid");

            //if col name is Name Contact
            //if (model.Name == null)
            //    model.Name = ReadValueAsStringFromRowItem(row, "name contact");

            var phone = ReadValueAsStringFromRowItem(row, "phonenumber");
            model.Phone = ParsePhone(phone);

            var lastInvoiceId = ReadValueAsStringFromRowItem(row, "lastinvoiceid");
            model.LastInvoiceId = Convert.ToInt64(lastInvoiceId);

            var isUpdated = ReadValueAsStringFromRowItem(row, "status");
            model.Status = Convert.ToInt32(isUpdated);

            var contactPerson = ReadValueAsStringFromRowItem(row, "contactperson");
            model.ContactPerson = contactPerson;

            var emailColumnValue = ReadValueAsStringFromRowItem(row, "email");

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

            var line1 = ReadValueAsStringFromRowItem(row, "addressline1");
            var line2 = ReadValueAsStringFromRowItem(row, "addressline2");
            var city = ReadValueAsStringFromRowItem(row, "city");
            var stateName = ReadValueAsStringFromRowItem(row, "state");
            var zipCode = ReadValueAsStringFromRowItem(row, "zipcode");

            if (string.IsNullOrWhiteSpace(line1) && string.IsNullOrWhiteSpace(line2) && string.IsNullOrWhiteSpace(city)
              && string.IsNullOrWhiteSpace(stateName) && string.IsNullOrWhiteSpace(zipCode))
            {
                model.Address = null;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(stateName) && !string.IsNullOrWhiteSpace(city) && city.LastIndexOf(" ") > 0)
                {
                    var subStr = city.Substring(city.LastIndexOf(" ") + 1);
                    if (subStr.Length == 2)
                    {
                        stateName = subStr;
                    }
                }

                var stateId = _stateService.GetStateIdByName(stateName);

                model.Address = new AddressEditModel()
                {
                    AddressLine1 = string.IsNullOrWhiteSpace(line1) ? "" : line1,
                    AddressLine2 = line2,
                    City = city,
                    ZipCode = zipCode,
                    StateId = stateId,
                    State= stateName
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
    }
}
