using System;

namespace Core.Reports.ViewModel
{
    public class ProductChartViewModel
    {
        public DateTime Date { get; set; }
        public decimal Mld { get; set; }
        public decimal Franchisesales { get; set; }    
        public decimal Jet { get; set; }
        public decimal Walmart { get; set; }
        public decimal Amazon { get; set; }

        public decimal Amazonprime { get; set; }

        public decimal Amazoncanada { get; set; }
        public decimal Hardware { get; set; }
        public decimal Retail { get; set; }
        public decimal Testing { get; set; }
        public decimal Mldwarehouse { get; set; }
        public decimal Government { get; set; }
        public decimal Hotel { get; set; }
       public decimal Admin { get; set; }
        public decimal Total { get; set; }
        public decimal TotalOfAll { get; set; }
        public decimal OTHER { get; set; }
    }
}
