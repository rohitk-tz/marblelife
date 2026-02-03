using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    public class AnnualGroupedReport
    {
        public long ReportTypeId { get; set; }
        public long FranchiseeId { get; set; }
        public string ReportTypeDescription { get; set; }
        public List<Auditaddress> GroupedCollection { get; set; }
        public decimal TotalSum { get; set; }
        public string CurrencyCode { get; set; }

        public decimal CurrencyRate { get; set; }

    }
}
