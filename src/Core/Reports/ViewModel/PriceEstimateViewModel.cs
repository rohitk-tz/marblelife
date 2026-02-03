using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class PriceEstimateViewModel
    {
        public PriceEstimateViewModel()
        {
            PriceEstimateServices = new List<PriceEstimateServiceModel>();
        }
        public string Service { get; set; }
        public string ServiceType { get; set; }
        public string MaterialType { get; set; }
        public string Category { get; set; }
        public long CategoryId { get; set; }
        public string Unit { get; set; }
        public string Note { get; set; }
        public List<PriceEstimateServiceModel> PriceEstimateServices { get; set; }
        public bool HasTwoPriceColumns { get; set; }
        public bool HasPriceValues { get; set; }
        public bool IsActiveService { get; set; }
        public bool IsDisabledService { get; set; }
        public long ServiceTagId { get; set; }
        public decimal? FranchiseeCorporatePrice { get; set; }
        public decimal? FranchiseeAdditionalCorporatePrice { get; set; }
        public decimal? BulkCorporatePrice { get; set; }
        public decimal? BulkCorporateAdditionalPrice { get; set; }
        public decimal? AverageFranchiseePrice { get; set; }
        public decimal? MaximumFranchiseePrice { get; set; }
        public decimal? AverageFranchiseeAdditionalPrice { get; set; }
        public decimal? MaximumFranchiseeAdditionalPrice { get; set; }
        public string MaximumFranchiseeAdditionalPriceName { get; set; }
        public string MaximumFranchiseePriceName { get; set; }
        public decimal? FranchiseePrice { get; set; }
        public bool CategoryType { get; set; }
    }
    [NoValidatorRequired]
    public class PriceEstimateServiceModel
    {
        public long? FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public decimal? CorporatePrice { get; set; }
        public decimal? CorporateAdditionalPrice { get; set; }
        public decimal? FranchiseePrice { get; set; }
        public decimal? FranchiseeAdditionalPrice { get; set; }
        public string AlternativeSolution { get; set; }
        public bool IsFranchiseePriceZero { get; set; }
        public bool IsFranchiseeAdditionalPriceZero { get; set; }
        public bool IsActiveFranchisee { get; set; }
        public bool CategoryType { get; set; }

    }

    public class PriceEstimatePageViewModel
    {
        public PriceEstimatePageViewModel()
        {
            PriceEstimateViewModelList = new List<PriceEstimateViewModel>();
        }
        public List<PriceEstimateViewModel> PriceEstimateViewModelList { get; set; }
    }


    public class PriceEstimatePageViewModelForFA
    {
        public PriceEstimatePageViewModelForFA()
        {
            PriceEstimateViewModelForFAList = new List<PriceEstimateViewModelForFA>();
        }
        public List<PriceEstimateViewModelForFA> PriceEstimateViewModelForFAList { get; set; }
    }

    public class PriceEstimateViewModelForFA
    {
        public PriceEstimateViewModelForFA()
        {
         //   PriceEstimateServices = new List<PriceEstimateServiceModel>();
        }
        public long Id { get; set; }
        public string Service { get; set; }
        public string ServiceType { get; set; }
        public string Note { get; set; }
        public string MaterialType { get; set; }
        public string Category { get; set; }
        public long CategoryId { get; set; }
        public string Unit { get; set; }
        public string FranchiseeName { get; set; }
        //public List<PriceEstimateServiceModel> PriceEstimateServices { get; set; }
        public long ServiceTagId { get; set; }
        public decimal? FranchiseePrice { get; set; }
        public decimal? FranchiseeAdditionalPrice { get; set; }
        public decimal? BulkCorporatePrice { get; set; }
        public decimal? BulkCorporateAdditionalPrice { get; set; }
        public decimal? CorporatePrice { get; set; }
        public decimal? CorporateAdditionalPrice { get; set; }
        public decimal? AverageFranchiseePrice { get; set; }
        public decimal? MaximumFranchiseePrice { get; set; }
        public decimal? AverageFranchiseeAdditionalPrice { get; set; }
        public decimal? MaximumFranchiseeAdditionalPrice { get; set; }
        public string MaximumFranchiseeAdditionalPriceName { get; set; }
        public string MaximumFranchiseePriceName { get; set; }
    }

    public class PriceEstimateExcelViewModel
    {
        [DisplayName("Service")]
        public string Service { get; set; }
        [DisplayName("Service Type")]
        public string ServiceType { get; set; }
        [DisplayName("Material Type")]
        public string MaterialType { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
       
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Average Franchisee Price")]
        public decimal? AverageFranchiseePrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Average Franchisee Additional Price")] 
        public decimal? AverageFranchiseeAdditionalPrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Maximum Franchisee Price")] 
        public decimal? MaximumFranchiseePrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Maximum Franchisee Additional Price")] 
        public decimal? MaximumFranchiseeAdditionalPrice { get; set; }
        [DisplayName("Maximum Franchisee Price Name")] 
        public string MaximumFranchiseePriceName { get; set; }

        [DisplayName("Note")]
        public string Note { get; set; }

        [DownloadField(CurrencyType = "$")]
        [DisplayName("Bulk Corporate Price")]
        public decimal? BulkCorporatePrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Bulk Corporate Additional Price")]
        public decimal? BulkCorporateAdditionalPrice { get; set; }
        [DisplayName("Is Updated")]
        public int IsUpdated { get; set; }
        [DisplayName("Is Deleted")]
        public int IsDeleted { get; set; }
    }

    [NoValidatorRequired]
    public class PriceEstimateExcelViewModelForFA
    {
        public long Id { get; set; }
        public string Service { get; set; }
        [DisplayName("Service Type")]
        public string ServiceType { get; set; }
        [DisplayName("Material Type")]
        public string MaterialType { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        [DisplayName("Franchisee Name")]
        public string FranchiseeName { get; set; }
        [DisplayName("Note")]
        public string Note { get; set; }
        //[DownloadField(CurrencyType = "$")]
        //[DisplayName("Bulk Corporate Price")]
        //public decimal? BulkCorporatePrice { get; set; }
        //[DownloadField(CurrencyType = "$")]
        //[DisplayName("Bulk Corporate Additional Price")]
        //public decimal? BulkCorporateAdditionalPrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Corporate Price")]
        public decimal? CorporatePrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Corporate Additional Price")]
        public decimal? CorporateAdditionalPrice { get; set; }

        [DownloadField(CurrencyType = "$")]
        [DisplayName("Average Franchisee Price")]
        public decimal? AverageFranchiseePrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Average Franchisee Additional Price")]
        public decimal? AverageFranchiseeAdditionalPrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Maximum Franchisee Price")]
        public decimal? MaximumFranchiseePrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Maximum Franchisee Additional Price")]
        public decimal? MaximumFranchiseeAdditionalPrice { get; set; }
        [DisplayName("Maximum Franchisee Price Name")]
        public string MaximumFranchiseePriceName { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Franchisee Price")]
        public decimal? FranchiseePrice { get; set; }
        [DownloadField(CurrencyType = "$")]
        [DisplayName("Franchisee Additional Price")]
        public decimal? FranchiseeAdditionalPrice { get; set; }
        [DisplayName("Is Updated")]
        public int IsUpdated { get; set; }
        [DisplayName("Is Deleted")]
        public int IsDeleted { get; set; }
    }

    [NoValidatorRequired]
    public class PriceEstimateExcelUploadModel : EditModelBase
    {
        public long UserId { get; set; }
        public long RoleUserId { get; set; }
        public long Id { get; set; }
        public long FileId { get; set; }
        public long StatusId { get; set; }
        public FileModel File { get; set; }
        public string Notes { get; set; }
        public bool IsFranchiseeAdmin { get; set; }
    }

}
