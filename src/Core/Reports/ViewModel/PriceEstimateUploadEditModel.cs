using Core.Application.ViewModel;
using Core.Geo.ViewModel;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
   public class PriceEstimateUploadEditModel : EditModelBase
    {
        public long Id { get; set; }
        public long IsUpdated { get; set; }
        public string Service { get; set; }
        public string Note { get; set; }
        public string ServiceType { get; set; }
        public string MaterialType { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public string FranchiseeName { get; set; }
        public decimal? FranchiseePrice { get; set; }
        public decimal? FranchiseeAdditionalPrice { get; set; }
        public decimal? BulkCorporatePrice { get; set; }
        public decimal? BulkCorporateAdditionalPrice { get; set; }
        public decimal? AverageFranchiseePrice { get; set; }
        public decimal? MaximumFranchiseePrice { get; set; }
        public decimal? AverageFranchiseeAdditionalPrice { get; set; }
        public decimal? MaximumFranchiseeAdditionalPrice { get; set; }
        public string MaximumFranchiseeAdditionalPriceName { get; set; }
        public string MaximumFranchiseePriceName { get; set; }
        public long IsDeleted { get; set; }
        public bool IsDuplicate { get; set; }
    }
}
