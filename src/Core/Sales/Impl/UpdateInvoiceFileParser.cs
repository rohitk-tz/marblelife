using Core.Application;
using Core.Application.Attribute;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class UpdateInvoiceFileParser : IUpdateInvoiceFileParser
    {
        private ILogService _logService;
        object directionCode = default(object);
        object zipCode = default(object);
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();
        private List<UpdateInvoiceEditModel> _updationInvoiceCollection = new List<UpdateInvoiceEditModel>();

        private List<UpdateInvoiceEditModel> _updateInvoiceCollection = new List<UpdateInvoiceEditModel>();
        public UpdateInvoiceFileParser(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock)
        {
            _logService = logService;
        }


        public bool CheckForValidHeader(DataTable dt, out string message)
        {
            message = "";
            const string Id = "id";
            const string invoiceId = "invoiceid";
            const string customerName = "customername";
            const string generatedOn = "generatedon";

            const string serviceName = "servicename";
            const string finalService = "finalservice";
            const string newService = "newservice";

            const string marketingClass = "marketingclass";
            const string finalClass = "finalclass";
            const string newClass = "newclass";
            const string rate = "rate";
            const string quantity = "quantity";

            const string amount = "amount";
            const string description = "description";
            const string finaldescription = "finaldescription";
            const string newdescription = "newdescription";
            const string isChanged = "ischanged";


            const string addressLine1 = "addressline1";
            const string addressLine2 = "addressline2";
            const string state = "state";
            const string city = "city";
            const string zipCode = "zipCode";
            var isIdPresent = false;
            var isInvoiceIdPresent = false;
            var isCustomerNamePresent = false;
            var isGeneratedOnPresent = false;

            var isServiceNamePresent = false;
            var isNewServicePresent = false;
            var isFinalServicePresent = false;

            var isMarketingClassPresent = false;
            var isNewClassPresent = false;
            var isFinalClassPresent = false;

            var isRatePresent = false;

            var isQauntityPresent = false;

            var isAmountPresent = false;

            var isDescriptionPresent = false;
            var isnewDescriptionPresent = false;
            var isfinalDescriptionPresent = false;

            var isIsChangedPresent = false;
            var isAddressLine1Present = false;
            var isAddressLine2Present = false;
            var isCityPresent = false;
            var isStatePresent = false;
            var isZipCodePresent = false;

            for (int i = 0; i < dt.Columns.Count; i++)
            {

                var caption = dt.Columns[i].Caption;
                caption = caption.Replace(" ", string.Empty);

                if (caption.Equals(Id))
                    isIdPresent = true;

                else if (caption.Equals(invoiceId))
                    isInvoiceIdPresent = true;

                else if (caption.Equals(customerName))
                    isCustomerNamePresent = true;

                else if (caption.Equals(generatedOn))
                    isGeneratedOnPresent = true;

                else if (caption.Equals(marketingClass))
                    isMarketingClassPresent = true;

                else if (caption.Equals(newClass))
                    isNewClassPresent = true;

                else if (caption.Equals(finalClass))
                    isFinalClassPresent = true;

                else if (caption.Equals(serviceName))
                    isServiceNamePresent = true;

                else if (caption.Equals(newService))
                    isNewServicePresent = true;

                else if (caption.Equals(finalService))
                    isFinalServicePresent = true;

                else if (caption.Equals(rate))
                    isRatePresent = true;

                else if (caption.Equals(quantity))
                    isQauntityPresent = true;

                else if (caption.Equals(description))
                    isDescriptionPresent = true;

                else if (caption.Equals(newdescription))
                    isnewDescriptionPresent = true;

                else if (caption.Equals(finaldescription))
                    isfinalDescriptionPresent = true;

                else if (caption.Equals(amount))
                    isAmountPresent = true;

                else if (caption.Equals(isChanged))
                    isIsChangedPresent = true;

                else if (caption.Equals(addressLine1))
                    isAddressLine1Present = true;

                else if (caption.Equals(addressLine2))
                    isAddressLine2Present = true;
                else if (caption.Equals(city))
                    isCityPresent = true;
                else if (caption.Equals(state))
                    isStatePresent = true;
                else if (caption.Equals(zipCode))
                    isZipCodePresent = true;
            }


            if (!isIdPresent)
            {
                message = "Columns for 'id'  is missing.";
                return false;
            }

            else if (!isCustomerNamePresent)
            {
                message = "Columns for 'Customer Name'  is missing.";
                return false;
            }

            else if (!isInvoiceIdPresent)
            {
                message = "Columns for 'Invoice Id'  is missing.";
                return false;
            }
            else if (!isGeneratedOnPresent)
            {
                message = "Columns for 'Generated On'  is missing.";
                return false;
            }

            else if (!isServiceNamePresent)
            {
                message = "Columns for 'Service Name'  is missing.";
                return false;
            }

            else if (!isNewServicePresent)
            {
                message = "Columns for 'New Service'  is missing.";
                return false;
            }

            else if (!isFinalServicePresent)
            {
                message = "Columns for 'Final Service'  is missing.";
                return false;
            }

            else if (!isMarketingClassPresent)
            {
                message = "Columns for 'Marketing Class'  is missing.";
                return false;
            }

            else if (!isNewClassPresent)
            {
                message = "Columns for 'New Class'  is missing.";
                return false;
            }

            else if (!isFinalClassPresent)
            {
                message = "Columns for 'Final Class'  is missing.";
                return false;
            }

            else if (!isRatePresent)
            {
                message = "Columns for 'Rate'  is missing.";
                return false;
            }

            else if (!isQauntityPresent)
            {
                message = "Columns for 'Quantity'  is missing.";
                return false;
            }

            else if (!isAmountPresent)
            {
                message = "Columns for 'Amount'  is missing.";
                return false;
            }

            else if (!isDescriptionPresent)
            {
                message = "Columns for 'Description'  is missing.";
                return false;
            }

            else if (!isnewDescriptionPresent)
            {
                message = "Columns for 'New Description'  is missing.";
                return false;
            }

            else if (!isfinalDescriptionPresent)
            {
                message = "Columns for 'Final Description'  is missing.";
                return false;
            }

            else if (!isIsChangedPresent)
            {
                message = "Columns for 'Is Changed'  is missing.";
                return false;
            }

            else if (!isAddressLine1Present)
            {
                message = "Columns for 'Address Line 1'  is missing.";
                return false;
            }

            else if (!isAddressLine2Present)
            {
                message = "Columns for 'Address Line 2'  is missing.";
                return false;
            }

            else if (!isCityPresent)
            {
                message = "Columns for 'City'  is missing.";
                return false;
            }

            else if (!isStatePresent)
            {
                message = "Columns for 'State'  is missing.";
                return false;
            }

            return true;
        }

        public IList<UpdateInvoiceEditModel> PrepareDomainFromDataTableForUpdateInvoice(DataTable dt)
        {
            PrepareHeaderIndex(dt);
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    directionCode = row.ItemArray[1];
                    ParseRow(row, "id");
                }
                catch (Exception e1)
                {
                    _logService.Error("Error in adding DirectionCode " + directionCode.ToString(), e1);
                }
            }
            return _updateInvoiceCollection;
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

        private void ParseRow(DataRow row, string value)
        {
            var dataInColType = ReadValueAsStringFromRowItem(row, value);

            if (dataInColType == null && value == "id")
            {
                value = "0";
            }
            else if (dataInColType == null || string.IsNullOrWhiteSpace(dataInColType.ToString()))
            {
                return;
            }

            CreateModels(row);
        }

        private void CreateModels(DataRow row)
        {
            UpdateInvoiceEditModel customerModel = new UpdateInvoiceEditModel();
            customerModel = PrepareDataModel(row, customerModel);
        }

        private UpdateInvoiceEditModel PrepareDataModel(DataRow row, UpdateInvoiceEditModel countyModel)
        {
            countyModel = CreateModel(row);
            _updateInvoiceCollection.Add(countyModel);
            return countyModel;
        }

        private UpdateInvoiceEditModel CreateModel(DataRow row)
        {
            var numericValue = 0;
            var model = new UpdateInvoiceEditModel();
            var id = ReadValueAsStringFromRowItem(row, "id");
            model.Id = Convert.ToInt64(id);
            var invoiceId = ReadValueAsStringFromRowItem(row, "Invoice Id");
            model.InvoiceId = Convert.ToInt64(invoiceId);
            var customerName = ReadValueAsStringFromRowItem(row, "Customer Name");
            model.CustomerName = (customerName);

            var addressLine1 = ReadValueAsStringFromRowItem(row, "Address Line 1");
            model.AddressLine1 = (addressLine1);

            var addressLine2 = ReadValueAsStringFromRowItem(row, "Address Line 2");
            model.AddressLine2 = (addressLine2);

            var city = ReadValueAsStringFromRowItem(row, "City");
            model.City = (city);

            var state = ReadValueAsStringFromRowItem(row, "State");
            model.State = (state);

            var zipCode = ReadValueAsStringFromRowItem(row, "ZipCode");
            model.ZipCode = (zipCode);



            var generatedOn = ReadValueAsStringFromRowItem(row, "GeneratedOn");

            if (generatedOn == null)
            {
                generatedOn = ReadValueAsStringFromRowItem(row, "Generated On");
            }
            var isInt = int.TryParse(generatedOn, out numericValue);
            if (isInt == true)
            {
                //generatedOn = new DateTime(new TimeSpan(Convert.ToInt32(generatedOn), 0, 0, 0).Ticks).ToString();
                generatedOn = new DateTime().ToString();
            }
            //model.GeneratedOn = DateTime.Parse(generatedOn);

            var serviceName = ReadValueAsStringFromRowItem(row, "Service Name");
            model.ServiceName = (serviceName);
            var newService = ReadValueAsStringFromRowItem(row, "New Service");
            model.NewService = (newService);
            var finalService = ReadValueAsStringFromRowItem(row, "Final Service");
            model.FinalService = (finalService);

            var marketingClass = ReadValueAsStringFromRowItem(row, "Marketing Class");
            model.MarketingClass = (marketingClass);
            var newClass = ReadValueAsStringFromRowItem(row, "New Class");
            model.NewClass = (newClass);
            var finalClass = ReadValueAsStringFromRowItem(row, "Final Class");
            model.FinalClass = (finalClass);

            var rate = ReadValueAsStringFromRowItem(row, "Rate");
            //model.Rate = Convert.ToDecimal(rate);
            model.Rate = Convert.ToDecimal(default(Decimal));

            var quantity = ReadValueAsStringFromRowItem(row, "Quantity");
            //model.Quantity = Convert.ToDecimal(quantity);
            model.Quantity = Convert.ToDecimal(default(Decimal));

            var amount = ReadValueAsStringFromRowItem(row, "Amount");
            //model.Amount = Convert.ToDecimal(amount);
            model.Amount = Convert.ToDecimal(default(Decimal));

            var description = ReadValueAsStringFromRowItem(row, "Description");
            model.Description = description;
            var newdescription = ReadValueAsStringFromRowItem(row, "New Description");
            model.NewDescription = newdescription;
            var finaldescription = ReadValueAsStringFromRowItem(row, "Final Description");
            model.FinalDescription = finaldescription;

            var isChanged = ReadValueAsStringFromRowItem(row, "Is Changed");
            model.IsChanged = Convert.ToInt32(isChanged);

            var item = ReadValueAsStringFromRowItem(row, "Item");
            model.Item = item;
            return model;
        }

        private string ReadValueAsStringFromRowItem(DataRow row, string key)
        {
            key = key.ToLower();
            if (_headersDictionary.ContainsKey(key) == false) return null;
            var value = row.ItemArray[_headersDictionary[key]].ToString().Trim();

            if (!string.IsNullOrEmpty(value))
                return value.Trim();

            return null;
        }
    }
}
