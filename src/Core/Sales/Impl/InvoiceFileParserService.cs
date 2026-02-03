using Core.Application;
using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class InvoiceFileParserService : IInvoiceFileParserService
    {
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();
        private List<InvoiceInfoEditModel> _invoiceModelCollection = new List<InvoiceInfoEditModel>();
        private List<MarketingClass> _marketingClasses;
        private List<ServiceType> _serviceTypes;
        private List<SubClassMarketingClass> _subMarketingClasses;
        public InvoiceFileParserService(IUnitOfWork unitOfWork)
        {
            _marketingClasses = unitOfWork.Repository<MarketingClass>().Table.ToList().Select(x =>
            {
                x.Name = x.Name.ToUpper().Replace(" ", "");
                return x;
            }).ToList();

            _serviceTypes = unitOfWork.Repository<ServiceType>().Table.ToList().Select(x =>
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
        public IList<InvoiceInfoEditModel> PrepareDomainFromDataTable(DataTable dt)
        {
            PrepareHeaderIndex(dt);

            foreach (DataRow row in dt.Rows)
            {
                ParseRow(row);
            }

            return _invoiceModelCollection;
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
            var dataInColType = ReadValueAsLongFromRowItem(row, "invoiceid");
            var dataInInvoiceItemCol = ReadValueAsLongFromRowItem(row, "invoiceitemid");
            if ((dataInColType <= 0 || string.IsNullOrWhiteSpace(dataInColType.ToString()) || (dataInInvoiceItemCol <= 0 || string.IsNullOrWhiteSpace(dataInInvoiceItemCol.ToString()))))
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
            InvoiceInfoEditModel invoiceModel = new InvoiceInfoEditModel();
            long marketingClassId, serviceTypeId;
            long? subMarketingClassId;
            var invoiceId = ReadValueAsLongFromRowItem(row, "invoiceid");
            var invoiceItemId = ReadValueAsLongFromRowItem(row, "invoiceitemid");
            var serviceData = ReadValueAsStringFromRowItem(row, "newservice");
            var classData = ReadValueAsStringFromRowItem(row, "newclass");

            if (invoiceId > 0 && invoiceItemId > 0)
            {
                invoiceModel.InvoiceId = invoiceId;
                invoiceModel.InvoiceItemId = invoiceItemId;

                if (!(string.IsNullOrEmpty(classData)))
                {
                    GetMarketingClass(classData, out marketingClassId);
                    GetSubMarketingClassAndServiceType(classData, out subMarketingClassId);
                    if (marketingClassId > 0)
                        invoiceModel.ClassTypeId = marketingClassId;
                    if (subMarketingClassId > 0)
                        invoiceModel.SubClassTypeId = subMarketingClassId;
                }
                if (!string.IsNullOrEmpty(serviceData))
                {
                    GetServiceType(serviceData, classData, out serviceTypeId);
                    if (serviceTypeId > 0)
                        invoiceModel.ServiceTypeId = serviceTypeId;
                }
                if (invoiceModel.ServiceTypeId > 0 || invoiceModel.ClassTypeId > 0)
                    _invoiceModelCollection.Add(invoiceModel);
            }
        }

        private void GetSubMarketingClassAndServiceType(string classData, out long? marketingClassId)
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
                retClassId = GetSubMarketingClass(item);
                if (retClassId > -1)
                {
                    marketingClassId = retClassId;
                    break;
                }
            }
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
        private void GetServiceType(string serviceData, string classData, out long serviceTypeId)
        {
            serviceTypeId = (long)Billing.Enum.ServiceTypes.Stonelife;

            var dataToParse = string.Empty;
            if (!string.IsNullOrWhiteSpace(serviceData))
            {
                dataToParse = serviceData;
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
            else
            {
                if (dataToParse == "CONCRETE-COUNTERTOPS")
                    dataToParse = Regex.Replace(dataToParse, " CONCRETE-COUNTERTOPS", "CONCRETECOUNTERTOPS", RegexOptions.IgnoreCase);
                else if (dataToParse == "CONCRETE-COATINGS")
                    dataToParse = Regex.Replace(dataToParse, "CONCRETE-COATINGS", "CONCRETECOATINGS", RegexOptions.IgnoreCase);
                else if (dataToParse == "CONCRETE-OVERLAYMENTS")
                    dataToParse = Regex.Replace(dataToParse, "CONCRETE-OVERLAYMENTS", "CONCRETEOVERLAYMENTS", RegexOptions.IgnoreCase);
                else if (dataToParse == "CONCRETE-REPAIR")
                    dataToParse = Regex.Replace(dataToParse, "CONCRETE-REPAIR", "CONCRETEREPAIR", RegexOptions.IgnoreCase);
                else if (dataToParse == "MLFS-FRANCHISESALES")
                    dataToParse = Regex.Replace(dataToParse, "MLFS-FRANCHISESALES", "MLFSFRANCHISESALES", RegexOptions.IgnoreCase);
                else if (dataToParse == "BI-MONTHLY")
                    dataToParse = Regex.Replace(dataToParse, "BI-MONTHLY", "BIMONTHLY", RegexOptions.IgnoreCase);
            }
            var data = dataToParse.Split(new[] { ':', '-' }, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrEmpty(classData))
            {
                const string mldClass = "0MLD";
                if (classData.ToUpper().Contains(mldClass.ToUpper()))
                    data = dataToParse.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
            }

            long retServiceTypeId = -1;
            var itemToExclude = string.Empty;

            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(itemToExclude) && itemToExclude.ToUpper().Equals(item.ToUpper()))
                    continue;
                retServiceTypeId = GetServiceTypeId(item);
                if (retServiceTypeId > -1)
                {
                    serviceTypeId = retServiceTypeId;
                    break;
                }
            }
        }

        private long GetServiceTypeId(string str)
        {
            var valUpper = !string.IsNullOrWhiteSpace(str) ? str.ToUpper().Trim().Replace(" ", "") : string.Empty;
            var serviceType = _serviceTypes.FirstOrDefault(x => x.Name == valUpper || DoesAliasContainsString(x.Alias, valUpper));

            if (serviceType == null)
            {
                serviceType = GetServiceTypeIdForSalesTax(valUpper);
            }

            if (serviceType == null) return -1;
            return serviceType.Id;
        }

        private ServiceType GetServiceTypeIdForSalesTax(string str)
        {
            var serviceType = _serviceTypes.Where(x => x.Id == (long)Billing.Enum.ServiceTypes.SALESTAX).FirstOrDefault(x => str.Contains(x.Name));
            return serviceType;
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
    }
}
