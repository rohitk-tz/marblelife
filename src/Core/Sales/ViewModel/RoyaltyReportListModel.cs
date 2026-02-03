using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Organizations.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class RoyaltyReportListModel
    {
        public long FranchiseeId { get; set; }
        public string FranchiseeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal YTDSales { get; set; } // Sales Prior to Start period

        public string CurrencyCode { get; set; }
        public decimal CurrencyRate { get; set; }

        public decimal TotalAmount
        {
            get
            {
                return Collection != null ? Collection.Select(c => c.ServiceAmountViewModel.Sum(s => s.Amount)).Sum() : 0;
            }

        }

        public IEnumerable<RoyaltyReportViewModel> Collection { get; set; }

        public IEnumerable<RoyaltyFeeSlabsEditModel> RoyaltyFeeSlabs { get; set; }

        public IEnumerable<SelectListModel> Classes { get; set; }
        public IEnumerable<SelectListModel> Services { get; set; }

        public IEnumerable<SelectListModel> ServiceTypeCategories { get; set; }

    }
}
