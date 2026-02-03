using Core.Application;
using Core.Application.Attribute;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    class PriceEstimateFileParser : IPriceEstimateFileParser
    {
        private ILogService _logService;

        public PriceEstimateFileParser(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock)
        {
            _logService = logService;
        }

        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();
        private List<PriceEstimateUploadEditModel> _priceEstimateUploadModelCollection = new List<PriceEstimateUploadEditModel>();

        public IList<PriceEstimateUploadEditModel> PrepareDomainFromDataTable(DataTable dt, bool isSuperAdmin)
        {
            PrepareHeaderIndex(dt);
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    if (isSuperAdmin)
                    {
                        ParseRow(row, "service");
                    }
                    else
                    {
                        ParseRow(row, "id");
                    }
                    
                }
                catch (Exception e1)
                {
                    _logService.Error("Error in adding row: " + e1);
                }
            }
            return _priceEstimateUploadModelCollection;
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
            if (dataInColType == null || string.IsNullOrWhiteSpace(dataInColType.ToString()))
            {
                return;
            }
            CreateModels(row);
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

        private void CreateModels(DataRow row)
        {
            PriceEstimateUploadEditModel priceEstimateModel = new PriceEstimateUploadEditModel();
            priceEstimateModel = PrepareDataModel(row, priceEstimateModel);
        }

        private PriceEstimateUploadEditModel PrepareDataModel(DataRow row, PriceEstimateUploadEditModel priceEstimateUploadEditModel)
        {
            priceEstimateUploadEditModel = CreatePriceEstimateUploadEditModel(row);
            _priceEstimateUploadModelCollection.Add(priceEstimateUploadEditModel);
            return priceEstimateUploadEditModel;
        }

        private PriceEstimateUploadEditModel CreatePriceEstimateUploadEditModel(DataRow row)
        {
            var model = new PriceEstimateUploadEditModel();
            var id = ReadValueAsStringFromRowItem(row, "id");
            model.Id = Convert.ToInt64(id);
            var service = ReadValueAsStringFromRowItem(row, "Service");
            model.Service = service;
            var serviceType = ReadValueAsStringFromRowItem(row, "ServiceType");
            model.ServiceType = serviceType;
            var note = ReadValueAsStringFromRowItem(row, "Note");
            model.Note = note;
            var materialType = ReadValueAsStringFromRowItem(row, "MaterialType");
            model.MaterialType = materialType;
            var category = ReadValueAsStringFromRowItem(row, "Category");
            model.Category = category;
            var unit = ReadValueAsStringFromRowItem(row, "Unit");
            model.Unit = unit;
            var franchiseeName = ReadValueAsStringFromRowItem(row, "FranchiseeName");
            model.FranchiseeName = franchiseeName;
            var franchiseePrice = ReadValueAsStringFromRowItem(row, "FranchiseePrice");
            model.FranchiseePrice = franchiseePrice != null ? Convert.ToDecimal(franchiseePrice.Trim('$')) : default(decimal);
            var franchiseeAdditionalPrice = ReadValueAsStringFromRowItem(row, "FranchiseeAdditionalPrice");
            model.FranchiseeAdditionalPrice = franchiseeAdditionalPrice != null ? Convert.ToDecimal(franchiseeAdditionalPrice.Trim('$')) : default(decimal);
            var bulkCorporatePrice = ReadValueAsStringFromRowItem(row, "BulkCorporatePrice");
            model.BulkCorporatePrice = bulkCorporatePrice != null ? Convert.ToDecimal(bulkCorporatePrice.Trim('$')): default(decimal);
            var bulkCorporateAdditionalPrice = ReadValueAsStringFromRowItem(row, "BulkCorporateAdditionalPrice");
            model.BulkCorporateAdditionalPrice = bulkCorporateAdditionalPrice != null ? Convert.ToDecimal(bulkCorporateAdditionalPrice.Trim('$')) : default(decimal);
            var averageFranchiseePrice = ReadValueAsStringFromRowItem(row, "AverageFranchiseePrice");
            model.AverageFranchiseePrice = averageFranchiseePrice != null ? Convert.ToDecimal(averageFranchiseePrice.Trim('$')): default(decimal);
            var maximumFranchiseePrice = ReadValueAsStringFromRowItem(row, "MaximumFranchiseePrice");
            model.MaximumFranchiseePrice = maximumFranchiseePrice != null ? Convert.ToDecimal(maximumFranchiseePrice.Trim('$')): default(decimal);
            var averageFranchiseeAdditionalPrice = ReadValueAsStringFromRowItem(row, "AverageFranchiseeAdditionalPrice");
            model.AverageFranchiseeAdditionalPrice = averageFranchiseeAdditionalPrice != null ? Convert.ToDecimal(averageFranchiseeAdditionalPrice.Trim('$')): default(decimal);
            var maximumFranchiseeAdditionalPrice = ReadValueAsStringFromRowItem(row, "MaximumFranchiseeAdditionalPrice");
            model.MaximumFranchiseeAdditionalPrice = maximumFranchiseeAdditionalPrice != null ? Convert.ToDecimal(maximumFranchiseeAdditionalPrice.Trim('$')): default(decimal);
            var maximumFranchiseePriceName = ReadValueAsStringFromRowItem(row, "MaximumFranchiseePriceName");
            model.MaximumFranchiseePriceName = maximumFranchiseePriceName;
            var isUpdated = ReadValueAsStringFromRowItem(row, "IsUpdated");
            model.IsUpdated = Convert.ToInt32(isUpdated);
            var isDeleted = ReadValueAsStringFromRowItem(row, "IsDeleted");
            model.IsDeleted = Convert.ToInt32(isDeleted);
            return model;
        }

        private bool CheckIfRowHasValidData(DataRow row)
        {
            var dataInColType = ReadValueAsStringFromRowItem(row, "id");
            var dataInColTypeNum = ReadValueAsStringFromRowItem(row, "county/municipality");

            if (string.IsNullOrWhiteSpace(dataInColType) && string.IsNullOrWhiteSpace(dataInColTypeNum))
            {
                return false;
            }
            return true;
        }
        public bool CheckForValidHeader(DataTable dt, out string message)
        {
            const string Id = "id";
            const string Service = "service";
            const string ServiceType = "servicetype";
            const string Note = "note";
            const string MaterialType = "materialtype";
            const string Category = "category";
            const string Unit = "unit";
            const string FranchiseeName = "franchiseename";
            const string FranchiseePrice = "franchiseeprice";
            const string FranchiseeAdditionalPrice = "franchiseeadditionalprice";
            //const string BulkCorporatePrice = "bulkcorporateprice";
            //const string BulkCorporateAdditionalPrice = "bulkcorporateadditionalprice";
            const string CorporatePrice = "corporateprice";
            const string CorporateAdditionalPrice = "corporateadditionalprice";
            const string AverageFranchiseePrice = "averagefranchiseeprice";
            const string MaximumFranchiseePrice = "maximumfranchiseeprice";
            const string AverageFranchiseeAdditionalPrice = "averagefranchiseeadditionalprice";
            const string MaximumFranchiseeAdditionalPrice = "maximumfranchiseeadditionalprice";
            const string MaximumFranchiseePriceName = "maximumfranchiseepricename";
            const string IsUpdated = "isupdated";
            const string IsDeleted = "isdeleted";


            var isIdPresent = false;
            var isServicePresent = false;
            var isServiceTypePresent = false;
            var isNotePresent = false;
            var isMaterialTypePresent = false;
            var isCategoryPresent = false;
            var isUnitPresent = false;
            var isFranchiseeNamePresent = false;
            var isFranchiseePricePresent = false;
            var isFranchiseeAdditionalPricePresent = false;
            //var isBulkCorporatePricePresent = false;
            //var isBulkCorporateAdditionalPricePresent = false;
            var isCorporatePricePresent = false;
            var isCorporateAdditionalPricePresent = false;
            var isAverageFranchiseePricePresent = false;
            var isMaximumFranchiseePricePresent = false;
            var isAverageFranchiseeAdditionalPricePresent = false;
            var isMaximumFranchiseeAdditionalPricePresent = false;
            var isMaximumFranchiseePriceNamePresent = false;
            var isIsDeletedPresent = false;
            var isIsUpdatedPresent = false;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var caption = dt.Columns[i].Caption;
                caption = caption.Replace(" ", string.Empty);
                if (caption.Equals(Id))
                    isIdPresent = true;
                else if (caption.Equals(Service))
                    isServicePresent = true;
                else if (caption.Equals(ServiceType))
                    isServiceTypePresent = true;
                else if (caption.Equals(Note))
                    isNotePresent = true;
                else if (caption.Equals(MaterialType))
                    isMaterialTypePresent = true;
                else if (caption.Equals(Category))
                    isCategoryPresent = true;
                else if (caption.Equals(Unit))
                    isUnitPresent = true;
                else if (caption.Equals(FranchiseeName))
                    isFranchiseeNamePresent = true;
                else if (caption.Equals(FranchiseePrice))
                    isFranchiseePricePresent = true;
                else if (caption.Equals(FranchiseeAdditionalPrice))
                    isFranchiseeAdditionalPricePresent = true;
                //else if (caption.Equals(BulkCorporatePrice))
                //    isBulkCorporatePricePresent = true;
                //else if (caption.Equals(BulkCorporateAdditionalPrice))
                //    isBulkCorporateAdditionalPricePresent = true;
                else if (caption.Equals(CorporatePrice))
                    isCorporatePricePresent = true;
                else if (caption.Equals(CorporateAdditionalPrice))
                    isCorporateAdditionalPricePresent = true;
                else if (caption.Equals(AverageFranchiseePrice))
                    isAverageFranchiseePricePresent = true;
                else if (caption.Equals(MaximumFranchiseePrice))
                    isMaximumFranchiseePricePresent = true;
                else if (caption.Equals(AverageFranchiseeAdditionalPrice))
                    isAverageFranchiseeAdditionalPricePresent = true;
                else if (caption.Equals(MaximumFranchiseeAdditionalPrice))
                    isMaximumFranchiseeAdditionalPricePresent = true;
                else if (caption.Equals(MaximumFranchiseePriceName))
                    isMaximumFranchiseePriceNamePresent = true;
                else if (caption.Equals(IsUpdated))
                    isIsUpdatedPresent = true;
                else if (caption.Equals(IsDeleted))
                    isIsDeletedPresent = true;
            }
            if (!isIdPresent)
            {
                message = "Column for 'id' is missing.";
                return false;
            }
            if (!isServicePresent)
            {
                message = "Column for 'Service' is missing.";
                return false;
            }
            if (!isServiceTypePresent)
            {
                message = "Column for 'Service Type' is missing.";
                return false;
            }
            if (!isNotePresent)
            {
                message = "Column for 'Note' is missing.";
                return false;
            }
            if (!isMaterialTypePresent)
            {
                message = "Column for 'Material Type' is missing.";
                return false;
            }
            if (!isCategoryPresent)
            {
                message = "Column for 'Category' is missing.";
                return false;
            }
            if (!isUnitPresent)
            {
                message = "Column for 'Unit' is missing.";
                return false;
            }
            if (!isFranchiseeNamePresent)
            {
                message = "Column for 'Franchisee Name' is missing.";
                return false;
            }
            if (!isFranchiseePricePresent)
            {
                message = "Column for 'Franchisee Price' is missing.";
                return false;
            }
            if (!isFranchiseeAdditionalPricePresent)
            {
                message = "Column for 'Franchisee Additional Price' is missing.";
                return false;
            }
            //if (!isBulkCorporatePricePresent)
            //{
            //    message = "Column for 'Bulk Corporate Price' is missing.";
            //    return false;
            //}
            //if (!isBulkCorporateAdditionalPricePresent)
            //{
            //    message = "Column for 'Bulk Corporate Additional Price' is missing.";
            //    return false;
            //}
            if (!isCorporatePricePresent)
            {
                message = "Column for 'Corporate Price' is missing.";
                return false;
            }
            if (!isCorporateAdditionalPricePresent)
            {
                message = "Column for 'Corporate Additional Price' is missing.";
                return false;
            }
            if (!isAverageFranchiseePricePresent)
            {
                message = "Column for 'Average Franchisee Price' is missing.";
                return false;
            }
            if (!isMaximumFranchiseePricePresent)
            {
                message = "Column for 'Maximum Franchisee Price' is missing.";
                return false;
            }
            if (!isAverageFranchiseeAdditionalPricePresent)
            {
                message = "Column for 'Average Franchisee Additional Price' is missing.";
                return false;
            }
            if (!isMaximumFranchiseeAdditionalPricePresent)
            {
                message = "Column for 'Maximum Franchisee Additional Price' is missing.";
                return false;
            }
            if (!isMaximumFranchiseePriceNamePresent)
            {
                message = "Column for 'Maximum Franchisee Price Name' is missing in County tab.";
                return false;
            }
            if (!isIsUpdatedPresent)
            {
                message = "Column for 'IsUpdated'  is missing.";
                return false;
            }
            if (!isIsDeletedPresent)
            {
                message = "Column for 'IsDeleted'  is missing.";
                return false;
            }
            message = "---";
            return true;
        }


        public bool CheckForValidHeaderForSA(DataTable dt, out string message)
        {
            //const string Id = "id";
            const string Service = "service";
            const string ServiceType = "servicetype";
            const string Note = "note";
            const string MaterialType = "materialtype";
            const string Category = "category";
            const string Unit = "unit";
            //const string FranchiseeName = "franchiseename";
            //const string FranchiseePrice = "franchiseeprice";
            //const string FranchiseeAdditionalPrice = "franchiseeadditionalprice";
            const string BulkCorporatePrice = "bulkcorporateprice";
            const string BulkCorporateAdditionalPrice = "bulkcorporateadditionalprice";
            const string AverageFranchiseePrice = "averagefranchiseeprice";
            const string MaximumFranchiseePrice = "maximumfranchiseeprice";
            const string AverageFranchiseeAdditionalPrice = "averagefranchiseeadditionalprice";
            const string MaximumFranchiseeAdditionalPrice = "maximumfranchiseeadditionalprice";
            const string MaximumFranchiseePriceName = "maximumfranchiseepricename";
            const string IsUpdated = "isupdated";
            const string IsDeleted = "isdeleted";


            //var isIdPresent = false;
            var isServicePresent = false;
            var isServiceTypePresent = false;
            var isNotePresent = false;
            var isMaterialTypePresent = false;
            var isCategoryPresent = false;
            var isUnitPresent = false;
            //var isFranchiseeNamePresent = false;
            //var isFranchiseePricePresent = false;
            //var isFranchiseeAdditionalPricePresent = false;
            var isBulkCorporatePricePresent = false;
            var isBulkCorporateAdditionalPricePresent = false;
            var isAverageFranchiseePricePresent = false;
            var isMaximumFranchiseePricePresent = false;
            var isAverageFranchiseeAdditionalPricePresent = false;
            var isMaximumFranchiseeAdditionalPricePresent = false;
            var isMaximumFranchiseePriceNamePresent = false;
            var isIsDeletedPresent = false;
            var isIsUpdatedPresent = false;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var caption = dt.Columns[i].Caption;
                caption = caption.Replace(" ", string.Empty);
                //if (caption.Equals(Id))
                //    isIdPresent = true;
                if (caption.Equals(Service))
                    isServicePresent = true;
                else if (caption.Equals(ServiceType))
                    isServiceTypePresent = true;
                else if (caption.Equals(Note))
                    isNotePresent = true;
                else if (caption.Equals(MaterialType))
                    isMaterialTypePresent = true;
                else if (caption.Equals(Category))
                    isCategoryPresent = true;
                else if (caption.Equals(Unit))
                    isUnitPresent = true;
                //else if (caption.Equals(FranchiseeName))
                //    isFranchiseeNamePresent = true;
                //else if (caption.Equals(FranchiseePrice))
                //    isFranchiseePricePresent = true;
                //else if (caption.Equals(FranchiseeAdditionalPrice))
                //    isFranchiseeAdditionalPricePresent = true;
                else if (caption.Equals(BulkCorporatePrice))
                    isBulkCorporatePricePresent = true;
                else if (caption.Equals(BulkCorporateAdditionalPrice))
                    isBulkCorporateAdditionalPricePresent = true;
                else if (caption.Equals(AverageFranchiseePrice))
                    isAverageFranchiseePricePresent = true;
                else if (caption.Equals(MaximumFranchiseePrice))
                    isMaximumFranchiseePricePresent = true;
                else if (caption.Equals(AverageFranchiseeAdditionalPrice))
                    isAverageFranchiseeAdditionalPricePresent = true;
                else if (caption.Equals(MaximumFranchiseeAdditionalPrice))
                    isMaximumFranchiseeAdditionalPricePresent = true;
                else if (caption.Equals(MaximumFranchiseePriceName))
                    isMaximumFranchiseePriceNamePresent = true;
                else if (caption.Equals(IsUpdated))
                    isIsUpdatedPresent = true;
                else if (caption.Equals(IsDeleted))
                    isIsDeletedPresent = true;
            }
            //if (!isIdPresent)
            //{
            //    message = "Column for 'id' is missing.";
            //    return false;
            //}
            if (!isServicePresent)
            {
                message = "Column for 'Service' is missing.";
                return false;
            }
            if (!isServiceTypePresent)
            {
                message = "Column for 'Service Type' is missing.";
                return false;
            }
            if (!isNotePresent)
            {
                message = "Column for 'Note' is missing.";
                return false;
            }
            if (!isMaterialTypePresent)
            {
                message = "Column for 'Material Type' is missing.";
                return false;
            }
            if (!isCategoryPresent)
            {
                message = "Column for 'Category' is missing.";
                return false;
            }
            if (!isUnitPresent)
            {
                message = "Column for 'Unit' is missing.";
                return false;
            }
            if (!isBulkCorporatePricePresent)
            {
                message = "Column for 'Bulk Corporate Price' is missing.";
                return false;
            }
            if (!isBulkCorporateAdditionalPricePresent)
            {
                message = "Column for 'Bulk Corporate Additional Price' is missing.";
                return false;
            }
            if (!isAverageFranchiseePricePresent)
            {
                message = "Column for 'Average Franchisee Price' is missing.";
                return false;
            }
            if (!isMaximumFranchiseePricePresent)
            {
                message = "Column for 'Maximum Franchisee Price' is missing.";
                return false;
            }
            if (!isAverageFranchiseeAdditionalPricePresent)
            {
                message = "Column for 'Average Franchisee Additional Price' is missing.";
                return false;
            }
            if (!isMaximumFranchiseeAdditionalPricePresent)
            {
                message = "Column for 'Maximum Franchisee Additional Price' is missing.";
                return false;
            }
            if (!isMaximumFranchiseePriceNamePresent)
            {
                message = "Column for 'Maximum Franchisee Price Name' is missing in County tab.";
                return false;
            }
            if (!isIsUpdatedPresent)
            {
                message = "Column for 'IsUpdated'  is missing.";
                return false;
            }
            if (!isIsDeletedPresent)
            {
                message = "Column for 'IsDeleted'  is missing.";
                return false;
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
    }
}
