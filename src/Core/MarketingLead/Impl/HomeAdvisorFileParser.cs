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
using Core.Geo.Domain;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class HomeAdvisorFileParser : IHomeAdvisorFileParser
    {
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();

        private List<string> _qbInvoiceNumbers = new List<string>();
        private List<HomeAdvisorParentModel> _parentModelCollection = new List<HomeAdvisorParentModel>();


        private IStateService _stateService;
        private ICityService _cityService;
        public HomeAdvisorFileParser(IStateService stateService, IUnitOfWork unitOfWork, ICityService cityService)
        {
            _stateService = stateService;
            _cityService = cityService;
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
            var dataInColTypeHaAccount = ReadValueAsStringFromRowItem(row, "ha account");
            var dataInColTypeCompanyName = ReadValueAsStringFromRowItem(row, "company name");
            var dataInColTypeSRId = ReadValueAsStringFromRowItem(row, "sr id");
            var dataInColTypeSRSubmittedDate = ReadValueAsStringFromRowItem(row, "sr submitted date");
            var dataInColTypeTask = ReadValueAsStringFromRowItem(row, "task");
            var dataInColTypeNetLead = ReadValueAsStringFromRowItem(row, "net lead $");
            var dataInColTypeCity = ReadValueAsStringFromRowItem(row, "city");
            var dataInColTypeState = ReadValueAsStringFromRowItem(row, "state");
            var dataInColTypeZipCode = ReadValueAsStringFromRowItem(row, "zip code");
            var dataInColTypeLeadType = ReadValueAsStringFromRowItem(row, "lead type");

            if (string.IsNullOrWhiteSpace(dataInColTypeHaAccount) && string.IsNullOrWhiteSpace(dataInColTypeCompanyName)
            && string.IsNullOrWhiteSpace(dataInColTypeSRId) && string.IsNullOrWhiteSpace(dataInColTypeSRId)
             && string.IsNullOrWhiteSpace(dataInColTypeSRSubmittedDate) && string.IsNullOrWhiteSpace(dataInColTypeTask)
              && string.IsNullOrWhiteSpace(dataInColTypeNetLead) && string.IsNullOrWhiteSpace(dataInColTypeCity)
               && string.IsNullOrWhiteSpace(dataInColTypeState) && string.IsNullOrWhiteSpace(dataInColTypeZipCode)
               && string.IsNullOrWhiteSpace(dataInColTypeLeadType))
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




        private void CreateModels(DataRow row)
        {
            HomeAdvisorParentModel parentModel = new HomeAdvisorParentModel();

            parentModel.HAAccount = ReadValueAsStringFromRowItem(row, "ha account");
            parentModel.CompanyName = ReadValueAsStringFromRowItem(row, "company name");
            parentModel.SRID = ReadValueAsStringFromRowItem(row, "sr id");
            parentModel.SRSubmittedDate = ReadValueAsDateFromRowItem(row, "sr submitted date");
            parentModel.NetLeadDollar = Convert.ToDecimal(ReadValueAsStringFromRowItem(row, "net lead $"));
            parentModel.CityName = ReadValueAsStringFromRowItem(row, "city");
            parentModel.StateName = ReadValueAsStringFromRowItem(row, "state");
            parentModel.ZipCode = ReadValueAsStringFromRowItem(row, "zip code");
            parentModel.LeadType = ReadValueAsStringFromRowItem(row, "lead type");
            parentModel.Task = ReadValueAsStringFromRowItem(row, "task");

            var stateId = _stateService.GetStateIdByShortName(parentModel.StateName);


            if (stateId <= 0)
                parentModel.StateId = _stateService.GetStateIdByName(parentModel.StateName);
            else
                parentModel.StateId = stateId;

            var cityId = _cityService.GetCityIdByName(parentModel.CityName);
            if (cityId > 0)
                parentModel.CityId = cityId;

            _parentModelCollection.Add(parentModel);

        }


        IList<HomeAdvisorParentModel> IHomeAdvisorFileParser.PrepareDomainFromDataTable(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                ParseRow(row);
            }

            return _parentModelCollection;
        }
    }
}
