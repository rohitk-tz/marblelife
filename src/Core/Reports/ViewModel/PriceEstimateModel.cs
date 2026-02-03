using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class PriceEstimateSaveModel
    {
        public PriceEstimateSaveModel()
        {
            PriceEstimateServices = new List<PriceEstimateServiceModel>();
        }
        public List<PriceEstimateServiceModel> PriceEstimateServices { get; set; }
        public List<long> ServiceTagId { get; set; }

    }


    [NoValidatorRequired]
    public class PriceEstimateSaveBulkModel
    {
        public long? ServiceTagId { get; set; }
        public decimal? BulkCorporatePrice { get; set; }
        public decimal? BulkCorporateAdditionalPrice { get; set; }

    }

    [NoValidatorRequired]
    public class PriceEstimateSaveCorporatePriceModel
    {
        public PriceEstimateSaveCorporatePriceModel()
        {
            PriceEstimateSaveBulkModel = new List<PriceEstimateSaveBulkModel>();
        }
        public List<PriceEstimateSaveBulkModel> PriceEstimateSaveBulkModel { get; set; }

    }


    [NoValidatorRequired]
    public class PriceEstimateGetModel
    {
        public long? CategoryId { get; set; }
        public List<long?> ServiceTypeId { get; set; }
        public List<string> ListOfService { get; set; }
        public long? ServiceTagId { get; set; }
        public bool ShowAllFranchisee { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsFranchiseeAdmin { get; set; }
        public List<long?> ServiceTagSelectedIds { get; set; }
        public List<long?> SelectedFranchiseeIds { get; set; }
        public long? SelectedCategoryId { get; set; }
        public long? FranchiseeId { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
    }

    [NoValidatorRequired]
    public class PriceEstimateBulkUpdateModel
    {
        public decimal? BulkCorporatePrice { get; set; }
        public decimal? BulkCorporateAdditionalPrice { get; set; }
        public decimal? CorporatePrice { get; set; }
        public decimal? CorporateAdditionalPrice { get; set; }
        public decimal? FranchiseePrice { get; set; }
        public decimal? FranchiseeAdditionalPrice { get; set; }
        public string AlternativeSolution { get; set; }
        public List<long> ServiceTagId { get; set; }
        public List<long> FranchiseeId { get; set; }

    }

    [NoValidatorRequired]
    public class PriceEstimateServiceTagNotesModel
    {
        public string Note { get; set; }
        public long? ServiceTagId { get; set; }
        public string MaterialType { get; set; }
        public string Service { get; set; }
        public string ServiceType { get; set; }
        public string Category { get; set; }
    }
    
    [NoValidatorRequired]
    public class PriceEstimateServiceTagNotesGetModel
    {
        public long? ServiceTagId { get; set; }
        public string Note { get; set; }
    }

}
