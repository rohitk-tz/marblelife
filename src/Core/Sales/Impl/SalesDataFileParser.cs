using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Geo;
using Core.Geo.ViewModel;
using Core.Organizations.ViewModels;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;
using Core.Application;
using Core.Organizations.Domain;


namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesDataFileParser : ISalesDataFileParser
    {
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();

        private List<string> _qbInvoiceNumbers = new List<string>();
        private List<ParsedFileParentModel> _parentModelCollection = new List<ParsedFileParentModel>();


        private CustomerCreateEditModel customerModel = new CustomerCreateEditModel();
        private InvoiceEditModel invoicemodel = new InvoiceEditModel();
        private FranchiseeSalesEditModel franchiseeSalesModel = new FranchiseeSalesEditModel();
        private InvoiceItemEditModel invoiceItemModel = new InvoiceItemEditModel();
        private FranchiseeSalesPaymentEditModel paymentModel = new FranchiseeSalesPaymentEditModel();
        private PaymentItemEditModel paymentItemModel = new PaymentItemEditModel();
        private IStateService _stateService;
        private List<MarketingClass> _marketingClasses;
        private List<SubClassMarketingClass> _subMarketingClasses;
        private List<ServiceType> _serviceTypes;

        public SalesDataFileParser(IStateService stateService, IUnitOfWork unitOfWork)
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

            _serviceTypes = unitOfWork.Repository<ServiceType>().Table.ToList().Select(x =>
            {
                x.Name = x.Name.ToUpper().Replace(" ", "");
                x.Alias = x.Alias;
                return x;
            }).ToList();
        }

        public IList<ParsedFileParentModel> PrepareDomainFromDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                ParseRow(row);
            }

            return _parentModelCollection;
        }

        public bool CheckForValidClassName(DataTable dt, out string result)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (!CheckIfRowHasValidData(row))
                {
                    continue;
                }
                var response = CheckIfRowHasValidMarketingClassData(row, out result);
                if (!response)
                {
                    return false;
                }
            }
            result = "---";
            return true;
        }

        private bool CheckIfRowHasValidMarketingClassData(DataRow row, out string result)
        {
            var classData = ReadValueAsStringFromRowItem(row, "class");
            var itemData = ReadValueAsStringFromRowItem(row, "item");
            var itemDescriptionData = ReadValueAsStringFromRowItem(row, "item description");
            var qbInvoiceNumber = ReadValueAsStringFromRowItem(row, "num");

            var dataToParse = classData;
            if (!string.IsNullOrWhiteSpace(classData))
            {
                dataToParse = classData;
            }
            else if (!string.IsNullOrWhiteSpace(itemData))
            {
                dataToParse = itemData;
            }
            else if (!string.IsNullOrWhiteSpace(itemDescriptionData))
            {
                dataToParse = itemDescriptionData;
            }

            const string salesTax = "SALESTAX";
            if (dataToParse.IndexOf("Sales-Tax", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                dataToParse = Regex.Replace(dataToParse, "SALES-TAX", salesTax, RegexOptions.IgnoreCase);
            }
            if (dataToParse.Equals(salesTax))
            {
                result = "---";
                return true;
            }

            var data = dataToParse.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            const string mldClass = "0MLD";
            if (dataToParse.ToUpper().Contains(mldClass.ToUpper()))
                data = dataToParse.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            long retClassId = -1, retSubClassId = -1, retServiceId = -1;

            if (data == null || data.Length <= 0)
            {
                result = "No Data In Class Column for QBInvoice - " + qbInvoiceNumber;
                return false;
            }

            var dataForClassName = data.Length > 0 ? data[0] : "";
            var dataForServiceName = data.Length > 1 ? data[1] : "";

            retClassId = GetMarketingClass(dataForClassName);       //Validate Marketing Class
            retServiceId = GetServiceTypeId(dataForServiceName);      //Validate Service Type
            //retSubClassId = GetSubMarketingClass(dataForClassName); //Validate Sub Marketing Class

            if (retClassId <= -1 || retServiceId <= -1)
            {
                result = "Incorrect Data In Class Column for QBInvoice - " + qbInvoiceNumber;
                return false;
            }
            result = "---";
            return true;
        }


        public bool CheckForValidHeader(DataTable dt, out string message)
        {
            var isBlankFile = CheckForBlankFile(dt);
            if (isBlankFile)
            {
                message = "File has no Valid Data.";
                return true;
            }

            const string Debit = "debit";
            const string Credit = "credit";
            const string PaidAmount = "paid amount";
            const string MarketingClass = "class";
            const string SalesPrice = "sales price";
            const string OriginalAmount = "original amount";
            const string Name = "name";
            const string NameContact = "name contact";
            const string NameSource = "source name";

            var isDebitPresent = false;
            var isCreditpresent = false;
            var isPaidAmountPresent = false;
            var isMarketingClassPresent = false;
            var isSalesPricePresent = false;
            var isOriginalAmountPresent = false;
            var isCustomerNamePresent = false;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var caption = dt.Columns[i].Caption;
                if (caption.Equals(Debit))
                    isDebitPresent = true;
                else if (caption.Equals(Credit))
                    isCreditpresent = true;
                else if (caption.Equals(PaidAmount))
                    isPaidAmountPresent = true;
                else if (caption.Equals(MarketingClass))
                    isMarketingClassPresent = true;
                else if (caption.Equals(SalesPrice))
                    isSalesPricePresent = true;
                else if (caption.Equals(OriginalAmount))
                    isOriginalAmountPresent = true;
                else if (caption.Equals(Name))
                    isCustomerNamePresent = true;
                else if (caption.Equals(NameContact))
                    isCustomerNamePresent = true;
                else if (caption.Equals(NameSource))
                    isCustomerNamePresent = true;
            }

            if (!isCustomerNamePresent)
            {
                message = "Columns for 'name' or 'name contact' or 'source name' is missing.";
                return false;
            }

            if (isCustomerNamePresent)
            {
                var invalidCount = 0;

                foreach (DataRow row in dt.Rows)
                {
                    if (!CheckIfRowHasValidData(row))
                    {
                        continue;
                    }
                    var customerName = GetCustomerName(row);
                    if (customerName == null || string.IsNullOrWhiteSpace(customerName.ToString()))
                        invalidCount++;
                }
                if (invalidCount > 0)
                {
                    message = "Data is missing for customerName in Columns 'name', 'name contact' or 'source name'";
                    return false;
                }
            }

            if (!isSalesPricePresent && !isOriginalAmountPresent)
            {
                message = "Columns for 'sales price' and 'original amount' is missing.";
                return false;
            }

            if (isSalesPricePresent && isSalesPricePresent)
            {
                var result = CheckForSalesdataAndOriginalAmountValue(dt);
                if (!result)
                {
                    message = "No data in Columns for 'sales amount' and 'original amount'";
                    return false;
                }
            }

            if (!isMarketingClassPresent)
            {
                message = "Columns for 'Class' is missing.";
                return false;
            }

            if (isMarketingClassPresent)
            {
                var result = CheckForMarketingClassValue(dt);
                if (!result)
                {
                    message = "Missing data in Column for 'Class'";
                    return false;
                }
            }

            if (!isCreditpresent && !isPaidAmountPresent)
            {
                message = "Columns for 'Debit', 'Credit' and 'Paid Amount' are missing.";
                return false;
            }

            if (isCreditpresent && isDebitPresent)
            {
                var invalidCount = 0;
                var validDataCount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    if (!CheckIfRowHasValidData(row))
                    {
                        continue;
                    }
                    var result = CheckIfDebitorCreditValuePresent(row);
                    if (!result)
                        invalidCount++;
                    else
                        validDataCount++;
                }

                if (invalidCount > validDataCount && !isPaidAmountPresent)
                {
                    message = "Columns for 'Paid Amount' is missing. And, No Data in Columns 'Debit', 'Credit'";
                    return false;
                }
            }
            else if (isPaidAmountPresent)
            {
                var invalidCount = 0;
                foreach (DataRow row in dt.Rows)
                {
                    var dataInColType = ReadValueAsStringFromRowItem(row, "type");
                    if (dataInColType == null || string.IsNullOrWhiteSpace(dataInColType.ToString()))
                    {
                        continue;
                    }

                    var amount = ReadValueAsStringFromRowItem(row, "paid amount");
                    if (amount == null || string.IsNullOrWhiteSpace(amount.ToString()))
                    {
                        invalidCount += 1;
                    }
                }

                if (invalidCount > 0)
                {
                    message = "Columns for 'Debit', 'Credit' are missing. And, No Data in Column 'Paid Amount'";
                    return false;
                }
            }

            if (!isCreditpresent && !isDebitPresent)
            {
                message = "Column for 'Credit' and 'Debit' are missing";
                return false;
            }
            if (!isCreditpresent)
            {
                message = "Column for 'Credit' is missing.";
                return false;
            }
            if (!isDebitPresent)
            {
                message = "Column for 'Debit' is missing";
                return false;
            }
            if (!isPaidAmountPresent)
            {
                message = "Column for 'Paid Amount' is missing.";
                return false;
            }
            if (isCreditpresent && isDebitPresent && isPaidAmountPresent && isCustomerNamePresent)
            {
                foreach(DataRow row in dt.Rows)
                {
                    var creditAmountStr = ReadValueAsStringFromRowItem(row, "credit");
                    var debitAmountStr = ReadValueAsStringFromRowItem(row, "debit");
                    var paidAmountStr = ReadValueAsStringFromRowItem(row, "paid amount");
                    var customerName = GetCustomerName(row);

                    decimal creditAmount = 0m;
                    decimal debitAmount = 0m;
                    decimal paidAmount = 0m;

                    decimal.TryParse(creditAmountStr, out creditAmount);
                    decimal.TryParse(debitAmountStr, out debitAmount);
                    decimal.TryParse(paidAmountStr, out paidAmount);

                    // Keep only 2 digits after decimal
                    creditAmount = Math.Round(creditAmount, 2, MidpointRounding.AwayFromZero);
                    debitAmount = Math.Round(debitAmount, 2, MidpointRounding.AwayFromZero);
                    paidAmount = Math.Round(paidAmount, 2, MidpointRounding.AwayFromZero);

                    if (!string.IsNullOrWhiteSpace(customerName))
                    {
                        // Rule 1: Negative values not allowed
                        if (debitAmount < 0 || creditAmount < 0)
                        {
                            message = "Negative values are not allowed for Debit or Credit for Customer - " + customerName;
                            return false;
                        }

                        // Rule 2: Paid = 0 → Debit and Credit must both be 0
                        if (paidAmount == 0)
                        {
                            if (debitAmount != 0 || creditAmount != 0)
                            {
                                message = "Debit and Credit must be zero when Paid Amount is zero for Customer - " + customerName;
                                return false;
                            }
                        }
                        else // Paid ≠ 0
                        {
                            // Rule 3: Exactly one of Debit or Credit must be > 0
                            if ((debitAmount > 0) == (creditAmount > 0))
                            {
                                message = "Exactly one of Debit or Credit must be greater than zero for Customer - " + customerName;
                                return false;
                            }

                            // Rule 4: Paid must equal Credit - Debit
                            if (paidAmount != (creditAmount - debitAmount))
                            {
                                message = "Paid Amount does not match Credit/Debit for Customer - " + customerName;
                                return false;
                            }
                        }
                    }
                }
            }

            message = "---";
            return true;
        }

        public bool CheckForBlankFile(DataTable dt)
        {
            var validCount = 0;
            foreach (DataRow row in dt.Rows)
            {
                var result = CheckIfRowHasValidData(row);
                if (result)
                {
                    validCount++;
                }
            }
            if (validCount > 0)
            {
                return false;
            }
            return true;
        }

        private bool CheckForSalesdataAndOriginalAmountValue(DataTable dt)
        {
            var salesPriceInvalidCount = 0;
            var salesPriceValidCount = 0;
            var originalAmountInvalidCount = 0;
            var originalAmountValidCount = 0;

            foreach (DataRow row in dt.Rows)
            {
                var salesAmount = ReadValueAsStringFromRowItem(row, "sales price");
                if (salesAmount == null || string.IsNullOrWhiteSpace(salesAmount.ToString()))
                    salesPriceInvalidCount++;
                else salesPriceValidCount++;

                var originalAmount = ReadValueAsStringFromRowItem(row, "original amount");
                if (originalAmount == null || string.IsNullOrWhiteSpace(originalAmount.ToString()))
                    originalAmountInvalidCount++;
                else originalAmountValidCount++;

            }
            if (salesPriceValidCount <= 0 && originalAmountValidCount <= 0)
                return false;
            return true;
        }

        private bool CheckForMarketingClassValue(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                if (!CheckIfRowHasValidData(row))
                {
                    continue;
                }
                var classValue = ReadValueAsStringFromRowItem(row, "class");

                if (classValue == null || string.IsNullOrWhiteSpace(classValue.ToString()))
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckIfDebitorCreditValuePresent(DataRow row)
        {
            var debitValue = ReadValueAsStringFromRowItem(row, "debit");
            var creditValue = ReadValueAsStringFromRowItem(row, "credit");

            if (creditValue == null || string.IsNullOrWhiteSpace(creditValue.ToString()))
            {
                if (debitValue == null || string.IsNullOrWhiteSpace(debitValue.ToString()))
                    return false;
            }
            return true;
        }

        public void PrepareHeaderIndex(DataTable dt)
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
            if (!CheckIfRowHasValidData(row))
            {
                return;
            }

            CreateModels(row);
        }

        private bool CheckIfRowHasValidData(DataRow row)
        {
            var dataInColType = ReadValueAsStringFromRowItem(row, "type");
            var dataInColTypeNum = ReadValueAsStringFromRowItem(row, "num");
            var dataInColTypeDate = ReadValueAsStringFromRowItem(row, "date");

            if (string.IsNullOrWhiteSpace(dataInColType) && string.IsNullOrWhiteSpace(dataInColTypeNum)
            && string.IsNullOrWhiteSpace(dataInColTypeDate))
            {
                return false;
            }
            return true;
        }

        private string ReadValueAsStringFromRowItem(DataRow row, string key)
        {
            if (_headersDictionary.ContainsKey(key) == false) return null;

            var value = row.ItemArray[_headersDictionary[key]].ToString().Trim();

            if (!string.IsNullOrEmpty(value))
                return value.Trim();

            return null;
        }

        private decimal? ReadValueAsNullableDecimal(DataRow row, string key)
        {
            if (_headersDictionary.ContainsKey(key) == false) return null;
            double result;
            var value = row.ItemArray[_headersDictionary[key]].ToString();

            if (!string.IsNullOrEmpty(value))
            {
                if (double.TryParse(value, out result))
                    return Convert.ToDecimal(result);
                else
                    return null;
            }

            return null;
        }
        private decimal ReadValueAsDecimalFromRowItem(DataRow row, string key)
        {
            if (_headersDictionary.ContainsKey(key) == false) return 0;

            var value = row.ItemArray[_headersDictionary[key]].ToString();
            double result;
            if (!string.IsNullOrEmpty(value))
            {
                if (double.TryParse(value, out result))
                    return Convert.ToDecimal(result);
                else
                    return 0;
            }

            return 0;
        }

        private int ReadValueAsIntFromRowItem(DataRow row, string key)
        {
            if (_headersDictionary.ContainsKey(key) == false) return 0;

            var value = row.ItemArray[_headersDictionary[key]].ToString();

            if (!string.IsNullOrEmpty(value))
            {
                return int.Parse(value);
            }

            return 0;
        }

        private DateTime ReadValueAsDateFromRowItem(DataRow row, string key)
        {
            if (_headersDictionary.ContainsKey(key) == false) return DateTime.Now;

            var value = row.ItemArray[_headersDictionary[key]].ToString();
            if (!string.IsNullOrEmpty(value))
            {
                double d;
                if (double.TryParse(value, out d))
                    return DateTime.FromOADate(d);

                DateTime dt;
                if (DateTime.TryParse(value, out dt))
                    return dt;
            }

            return DateTime.Now;
        }


        private void GetSubMarketingClassAndServiceType(string classData, string itemData, string itemDescriptionData, out long? subMarketingClassId)
        {
            subMarketingClassId = default(long?);

            var dataToParse = string.Empty;
            if (!string.IsNullOrWhiteSpace(classData))
            {
                dataToParse = classData;
            }
            else if (!string.IsNullOrWhiteSpace(itemData))
            {
                dataToParse = itemData;
            }
            else if (!string.IsNullOrWhiteSpace(itemDescriptionData))
            {
                dataToParse = itemDescriptionData;
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

        private void GetMarketingClassAndServiceType(string classData, string itemData, string itemDescriptionData, out long marketingClassId, out long serviceTypeId)
        {
            marketingClassId = (long)MarketingClassType.Residential;
            serviceTypeId = (long)ServiceTypes.Stonelife;

            var dataToParse = string.Empty;
            if (!string.IsNullOrWhiteSpace(classData))
            {
                dataToParse = classData;
            }
            else if (!string.IsNullOrWhiteSpace(itemData))
            {
                dataToParse = itemData;
            }
            else if (!string.IsNullOrWhiteSpace(itemDescriptionData))
            {
                dataToParse = itemDescriptionData;
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

            long retClassId = -1, retServiceTypeId = -1;
            var itemToExclude = string.Empty;

            foreach (var item in data)
            {
                retClassId = GetMarketingClass(item);
                if (retClassId > -1)
                {
                    marketingClassId = retClassId;
                    itemToExclude = item;
                    break;
                }
            }
            var dataForService = dataToParse.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var dataForServiceName = dataForService.Length > 1 ? dataForService[1] : "";

            serviceTypeId = GetServiceTypeId(dataForServiceName);

            if (serviceTypeId == -1)
            {
                serviceTypeId = retServiceTypeId;
            }

            //foreach (var item in dataForServiceName)
            //{
            //    if (!string.IsNullOrEmpty(itemToExclude) && itemToExclude.ToUpper().Equals(item.ToUpper()))
            //        continue;
            //    retServiceTypeId = GetServiceTypeId(item);
            //    if (retServiceTypeId > -1)
            //    {
            //        serviceTypeId = retServiceTypeId;
            //        break;
            //    }
            //}
        }


        private void CreateModels(DataRow row)
        {
            ParsedFileParentModel parentModel = new ParsedFileParentModel();

            var type = ReadValueAsStringFromRowItem(row, "type");
            if (string.IsNullOrWhiteSpace(type)) type = "";

            var qbInvoiceNumber = ReadValueAsStringFromRowItem(row, "num");
            parentModel.SalesRep = ReadValueAsStringFromRowItem(row, "rep");

            var invoiceDate = ReadValueAsDateFromRowItem(row, "date");
            var classData = ReadValueAsStringFromRowItem(row, "class");
            var itemData = ReadValueAsStringFromRowItem(row, "item");
            var itemDescriptionData = ReadValueAsStringFromRowItem(row, "item description");
            var itemDataOriginal = ReadValueAsStringFromRowItem(row, "item");
            long marketingClassId, serviceTypeId;
            long? subMarketingClassId;

            GetMarketingClassAndServiceType(classData, itemData, itemDescriptionData, out marketingClassId, out serviceTypeId);
            GetSubMarketingClassAndServiceType(classData, itemData, itemDescriptionData, out subMarketingClassId);
            

            if (type.Replace(" ", string.Empty).ToUpper().Contains("CREDITMEMO"))
            {
                int qbInvoicenumberIndex = _qbInvoiceNumbers.IndexOf(qbInvoiceNumber);
                parentModel = PrepareCreditMemoDataModel(row, parentModel, qbInvoiceNumber, invoiceDate, marketingClassId, qbInvoicenumberIndex);
            }
            else
            {
                parentModel = PrepareInvoiceDataModel(row, parentModel, qbInvoiceNumber, invoiceDate, marketingClassId, serviceTypeId, subMarketingClassId, itemDataOriginal);
            }
        }

        private ParsedFileParentModel PrepareInvoiceDataModel(DataRow row, ParsedFileParentModel parentModel, string qbInvoiceNumber, DateTime invoiceDate, long marketingClassId, long serviceTypeId, long? subMarketingClassId,string itemDataOriginal)
        {
            invoiceItemModel = CreateInvoiceItemModel(row, serviceTypeId, itemDataOriginal);
            paymentModel = CreatePaymentModel(row, (long)InvoiceItemType.Service, serviceTypeId);

            var checkId = qbInvoiceNumber + " - " + GetCustomerName(row);
            int qbInvoicenumberIndex = _qbInvoiceNumbers.IndexOf(checkId);
            if (qbInvoicenumberIndex < 0)
            {
                _qbInvoiceNumbers.Add(checkId);
                customerModel = CreateCustomerModel(row, marketingClassId, invoiceDate);
                invoicemodel = CreateInvoiceModel(row);

                parentModel.QbIdentifier = qbInvoiceNumber;
                parentModel.Date = invoiceDate;
                parentModel.Customer = customerModel;
                parentModel.Invoice = invoicemodel;

                if (paymentModel.Amount > 0)
                    parentModel.Invoice.Payments.Add(paymentModel);

                if (!parentModel.Invoice.InvoiceItems.Any(x => x.Description == invoiceItemModel.Description && x.Amount == invoiceItemModel.Amount))
                    parentModel.Invoice.InvoiceItems.Add(invoiceItemModel);

                parentModel.MarketingClassId = marketingClassId;
                parentModel.SubMarketingClassId = subMarketingClassId;
                parentModel.ServiceTypeId = serviceTypeId;

                _parentModelCollection.Add(parentModel);
            }
            else
            {
                parentModel = _parentModelCollection[qbInvoicenumberIndex];
                if (paymentModel.Amount > 0)
                    parentModel.Invoice.Payments.Add(paymentModel);

                if (invoiceItemModel.ItemTypeId == (long)InvoiceItemType.Discount ||
                    !parentModel.Invoice.InvoiceItems.Any(x => x.Description == invoiceItemModel.Description && x.Amount == invoiceItemModel.Amount))
                    parentModel.Invoice.InvoiceItems.Add(invoiceItemModel);
            }


            return parentModel;
        }

        private ParsedFileParentModel PrepareCreditMemoDataModel(DataRow row, ParsedFileParentModel parentModel, string qbInvoiceNumber, DateTime invoiceDate, long marketingClassId, int qbInvoicenumberIndex)
        {
            var creditMemoItemModel = CreateCreditMemoItemModel(row);

            if (qbInvoicenumberIndex < 0)
            {
                _qbInvoiceNumbers.Add(qbInvoiceNumber);
                parentModel.AccountCredit = CreateCreditMemoModel(row, qbInvoiceNumber);
                parentModel.AccountCredit.AccountCreditItems.Add(creditMemoItemModel);
                parentModel.Customer = CreateCustomerModel(row, marketingClassId, invoiceDate);

                parentModel.MarketingClassId = marketingClassId;
                parentModel.QbIdentifier = qbInvoiceNumber;
                parentModel.Date = invoiceDate;

                _parentModelCollection.Add(parentModel);
            }
            else
            {
                parentModel = _parentModelCollection[qbInvoicenumberIndex];
                if (parentModel.AccountCredit == null)
                    parentModel.AccountCredit = new AccountCreditEditModel();

                parentModel.AccountCredit.AccountCreditItems.Add(creditMemoItemModel);
            }

            return parentModel;
        }

        private AccountCreditEditModel CreateCreditMemoModel(DataRow row, string qbInvoiceNumber)
        {
            var model = new AccountCreditEditModel();
            model.CreditedOn = ReadValueAsDateFromRowItem(row, "date");
            model.QbInvoiceNumber = qbInvoiceNumber;
            return model;
        }

        private AccountCreditItemEditModel CreateCreditMemoItemModel(DataRow row)
        {
            var model = new AccountCreditItemEditModel();
            model.Description = ReadValueAsStringFromRowItem(row, "memo");
            model.Amount = ReadValueAsDecimalFromRowItem(row, "debit");
            return model;
        }
        private FranchiseeSalesPaymentEditModel CreatePaymentModel(DataRow row, long itemTypeId, long serviceTypeId)
        {
            var model = new FranchiseeSalesPaymentEditModel();
            model.Amount = ReadValueAsDecimalFromRowItem(row, "credit");

            if (model.Amount <= 0)
                model.Amount = ReadValueAsDecimalFromRowItem(row, "paid amount");

            model.Date = ReadValueAsDateFromRowItem(row, "date");

            model.PaymentItems.Add(new PaymentItemEditModel { ItemTypeId = itemTypeId, ItemId = serviceTypeId });

            model.CreditAmount = ReadValueAsDecimalFromRowItem(row, "credit");
            model.DebitAmount = ReadValueAsDecimalFromRowItem(row, "debit");
            return model;
        }

        private InvoiceEditModel CreateInvoiceModel(DataRow row)
        {
            var model = new InvoiceEditModel();
            model.GeneratedOn = ReadValueAsDateFromRowItem(row, "date");
            var creditAmount = ReadValueAsDecimalFromRowItem(row, "credit");
            var debitAmount = ReadValueAsDecimalFromRowItem(row, "debit");

            return model;
        }

        private InvoiceItemEditModel CreateInvoiceItemModel(DataRow row, long serviceTypeId, string itemDataOriginal)
        {
            var model = new InvoiceItemEditModel();
            model.ItemOriginal = itemDataOriginal;
            model.Description = ReadValueAsStringFromRowItem(row, "memo");
            if (string.IsNullOrEmpty(model.Description))
            {
                model.Description = ReadValueAsStringFromRowItem(row, "item");
            }

            model.Quantity = ReadValueAsDecimalFromRowItem(row, "qty");
            if (model.Quantity < 1) model.Quantity = 1;

            //Using TryParse for negative decimal
            var rate = ReadValueAsNullableDecimal(row, "sales price");

            if (rate != null)
                model.Rate = rate.Value;
            else
                model.Rate = 0;

            var origAmount = ReadValueAsNullableDecimal(row, "original amount");
            if (origAmount != null)
                model.Amount = ReadValueAsDecimalFromRowItem(row, "original amount");
            else if (rate != null)
            {
                model.Amount = rate.Value * model.Quantity;
            }
            else
            {
                var qbInvoice = model.Description = ReadValueAsStringFromRowItem(row, "num");
                throw new Exception(string.Format("Column 'Original Amount' is not present, and there is no data in Sales Price and Qty. against QBInvoice# {0}", qbInvoice));
            }

            model.ItemId = serviceTypeId;
            var debit = ReadValueAsDecimalFromRowItem(row, "debit");
            var credit = ReadValueAsNullableDecimal(row, "credit");

            model.CreditAmount = ReadValueAsDecimalFromRowItem(row, "credit");
            model.DebitAmount = ReadValueAsDecimalFromRowItem(row, "debit");

            var paidAmount = ReadValueAsDecimalFromRowItem(row, "paid amount");

            if ((model.Amount < 0 && (debit != 0 || paidAmount < 0)) || (debit != 0 && credit == null))
            {
                if (debit != 0)
                {
                    model.Amount = debit > 0 ? debit * -1 : debit;
                }
                else
                {
                    model.Amount = paidAmount > 0 ? paidAmount * -1 : paidAmount;
                }
                model.Rate = model.Amount;
                model.ItemTypeId = (long)InvoiceItemType.Discount;
            }
            else
            {
                model.ItemTypeId = (long)InvoiceItemType.Service;
            }

            return model;
        }

        private string ParsePhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return "";

            return Regex.Replace(phone, @"[^\d]", "");
        }

        private string GetCustomerName(DataRow row)
        {
            string name = ReadValueAsStringFromRowItem(row, "name");

            //if col name is Name Contact
            if (string.IsNullOrWhiteSpace(name))
                name = ReadValueAsStringFromRowItem(row, "name contact");

            if (string.IsNullOrWhiteSpace(name))
                name = ReadValueAsStringFromRowItem(row, "source name");

            return name;
        }

        private string GetNameContact(DataRow row)
        {
            var name = ReadValueAsStringFromRowItem(row, "name contact");

            if (string.IsNullOrWhiteSpace(name))
                name = ReadValueAsStringFromRowItem(row, "source name");

            return name;
        }

        private CustomerCreateEditModel CreateCustomerModel(DataRow row, long marketingClassId, DateTime invoiceDate)
        {
            var model = new CustomerCreateEditModel();

            model.DateCreated = invoiceDate;
            model.MarketingClassId = marketingClassId;
            model.Name = GetCustomerName(row);
            model.ReceiveNotification = true;

            var nameContact = GetNameContact(row);
            model.ContactPerson = nameContact;

            var phone = ReadValueAsStringFromRowItem(row, "name phone #");

            if (string.IsNullOrWhiteSpace(phone))
                phone = ReadValueAsStringFromRowItem(row, "name phone no");

            model.Phone = ParsePhone(phone);

            var emailColumnValue = ReadValueAsStringFromRowItem(row, "name e-mail");

            if (!string.IsNullOrEmpty(emailColumnValue))
            {
                var emails = emailColumnValue.Split(';');
                if (emails.Length == 1)
                    emails = emails[0].Split(',');

                foreach (var email in emails)
                {
                    model.CustomerEmails.Add(new CustomerEmail { Email = email, DateCreated = invoiceDate });
                }
            }

            var line1 = ReadValueAsStringFromRowItem(row, "name street1");
            var line2 = ReadValueAsStringFromRowItem(row, "name street2");
            var city = ReadValueAsStringFromRowItem(row, "name city");
            var stateShortName = ReadValueAsStringFromRowItem(row, "name state");
            var zipCode = ReadValueAsStringFromRowItem(row, "name zip");

            if (string.IsNullOrWhiteSpace(zipCode))
                zipCode = ReadValueAsStringFromRowItem(row, "name postal code");

            if (string.IsNullOrWhiteSpace(zipCode))
                zipCode = ReadValueAsStringFromRowItem(row, "name postcode");

            if (string.IsNullOrWhiteSpace(stateShortName))
                stateShortName = ReadValueAsStringFromRowItem(row, "name province");

            if (string.IsNullOrWhiteSpace(stateShortName))
                stateShortName = ReadValueAsStringFromRowItem(row, "name county");

            var stateName = !string.IsNullOrWhiteSpace(stateShortName) ? stateShortName.ToLower() : string.Empty;

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

                if (stateId <= 0)
                    stateId = _stateService.GetStateIdByName(stateName);

                model.Address = new AddressEditModel()
                {
                    AddressLine1 = string.IsNullOrWhiteSpace(line1) ? "" : line1,
                    AddressLine2 = line2,
                    City = city,
                    ZipCode = zipCode,
                    StateId = stateId,
                    State = stateShortName
                };
            }

            return model;
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
        private bool DoesSubMarketingAliasContainsString(string alias, string str)
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
            //var valUpper = !string.IsNullOrWhiteSpace(value) ? value.ToUpper().Trim().Replace(" ", "") : string.Empty;
            //var mclass = _marketingClasses.FirstOrDefault(x => x.Name == valUpper || DoesAliasContainsString(x.Alias, valUpper));
            //if (mclass == null) return -1;
            //return mclass.Id;
            if (!string.IsNullOrWhiteSpace(value))
            {
                var classUpperCase = value.ToUpper().Trim();
                var mclass = _marketingClasses.FirstOrDefault(x => x.Name == classUpperCase);
                if(mclass == null)
                {
                    return -1;
                }
                else
                {
                    return mclass.Id;
                }
            }
            else
            {
                return -1;
            }
        }

        private long GetSubMarketingClass(string value)
        {
            var valUpper = !string.IsNullOrWhiteSpace(value) ? value.ToUpper().Trim().Replace(" ", "") : string.Empty;
            var mclass = _subMarketingClasses.FirstOrDefault(x => x.Name == valUpper || DoesAliasContainsString(x.Alias, valUpper));
            if (mclass == null) return -1;
            return mclass.Id;
        }

        private long GetServiceTypeId(string str)
        {
            //var valUpper = !string.IsNullOrWhiteSpace(str) ? str.ToUpper().Trim().Replace(" ", "") : string.Empty;
            //var serviceType = _serviceTypes.FirstOrDefault(x => x.Name == valUpper || DoesAliasContainsString(x.Alias, valUpper));

            //if (serviceType == null)
            //{
            //    serviceType = GetServiceTypeIdForSalesTax(valUpper);
            //}

            //if (serviceType == null) return -1;
            //return serviceType.Id;
            if (string.IsNullOrWhiteSpace(str))
                return -1;

            var input = str.Trim();

            // If "MAINTENANCE" appears anywhere (case-insensitive), replace '-' with ':'
            if (input.IndexOf("MAINTENANCE", StringComparison.OrdinalIgnoreCase) >= 0)
                input = Regex.Replace(input, @"(?<=^MAINTENANCE)-(?=[A-Z])", ":", RegexOptions.IgnoreCase);

            var serviceName = input.Trim();

            // 1) Direct Name match (case-insensitive) and DashboardServices
            var serviceType = _serviceTypes.FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.Name) &&
                string.Equals(x.Name.Trim(), serviceName, StringComparison.OrdinalIgnoreCase) &&
                x.DashboardServices);

            // 2) MLD- prefix handling
            if (serviceType == null && serviceName.StartsWith("MLD-", StringComparison.OrdinalIgnoreCase))
            {
                var trimmed = serviceName.Substring(4).Trim();
                serviceType = _serviceTypes.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Name) &&
                    string.Equals(x.Name.Trim(), trimmed, StringComparison.OrdinalIgnoreCase) &&
                    x.SubCategoryId == 253);
            }

            // 3) Exact match among comma-separated Alias values (case-insensitive)
            if (serviceType == null)
            {
                serviceType = _serviceTypes.FirstOrDefault(x =>
                    !string.IsNullOrWhiteSpace(x.Alias) &&
                    x.Alias
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => a.Trim())
                        .Any(a => string.Equals(a, serviceName, StringComparison.OrdinalIgnoreCase)) &&
                    (x.DashboardServices || x.SubCategoryId == 253));
            }

            return serviceType?.Id ?? -1;
        }

        private ServiceType GetServiceTypeIdForSalesTax(string str)
        {
            var serviceType = _serviceTypes.Where(x => x.Id == (long)ServiceTypes.SALESTAX).FirstOrDefault(x => str.Contains(x.Name));
            return serviceType;
        }
    }
}
