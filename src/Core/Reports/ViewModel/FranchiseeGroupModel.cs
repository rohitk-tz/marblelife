using System.Collections.Generic;

namespace Core.Reports.ViewModel
{

    public class FranchiseeGroupModel
    {
        public List<FranchiseeModel> FranchiseeModel { get; set; }
        public string GroupName { get; set; }
        public List<ValueModel> QuarterWiseSum { get; set; }
        public List<ValueModel> YearWiseSum { get; set; }
        
        public int Index { get; set; }
    }

    
    public class FranchiseeModel
    {
        public FranchiseeModel()
        {
            SalesPriceModel = new List<PriceAndYearModel>();
            PurchasePriceModel = new List<PriceAndYearModel>();
            YearWiseTotalSum = new List<ColorStatusClass>();
            TotalYearWiseSum = new List<decimal>();
        }
        
        public int Index { get; set; }
        public bool IsExpand { get; set; }
        public decimal TotalSumPerQuarter { get; set; }
        public ValueModel TotalSumPerFranchisee { get; set; }
        public decimal TotalSumOfServices { get; set; }
        public int FirstYear { get; set; }
        public string Status { get; set; }
        public string FranchiseeName { get; set; }
        public long? FranchiseeId { get; set; }
        public double? TotalSum { get; set; }
        public List<decimal> TotalYearWiseSum { get; set; }
        public List<ColorStatusClass> YearWiseTotalSum { get; set; }
        public List<PriceAndYearModel> SalesPriceModel { get; set; }
        public List<PriceAndYearModel> PurchasePriceModel { get; set; }
        public List<List<decimal>> ServiceWiseSum { get; set; }
        public List<decimal> YearlyServiceWiseSum { get; set; }
    }

   
    public class PriceAndYearModel
    {
        public PriceAndYearModel()
        {
            PriceModel = new List<PriceModel>();
        }
        public string BackColor { get; set; }
        public string Color { get; set; }
        public bool IsParsed { get; set; }
        public double Year { get; set; }
        public double Quarter { get; set; }
        public decimal TotalSum { get; set; }
        public List<PriceModel> PriceModel { get; set; }
    }
    public class PriceModel
    {
        public string MarketingClass { get; set; }
        public decimal Price { get; set; }
    }

    public class ValueModel
    {
        public bool IsParsed { get; set; }
        public decimal Price { get; set; }
        public string BackGroundColor { get; set; }
        public string Color { get; set; }
    }
}
