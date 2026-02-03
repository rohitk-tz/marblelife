using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.ViewModel
{
    public class SalesFunnelLocalGraphViewModel
    {
        public SalesFunnelLocalGraphViewModel()
        {
        }
        public DateTime Date { get; set; }
        public decimal TotalCount { get; set; }
        public decimal LocalCount { get; set; }
        public decimal Total { get; set; }


        public string Color { get; set; }
        public decimal Local { get; set; }
        public decimal National { get; set; }
        public string DayOfWeek { get; set; }
        public string Status { get; set; }
        public string DateString { get; set; }

        public string LastYearDateString { get; set; }
        public int? LostEstimateCount { get; set; }
        public int? LostJobsCount { get; set; }
        public int? MissedCallsCount { get; set; }
        public int? TotalJobsCount { get; set; }
        public int? ConvertToEstimateCount { get; set; }
        public int? ConvertToJobCount { get; set; }
        public int? ConvertToInvoiceCount { get; set; }
        public int? SalesCloseRateCount { get; set; }
        public int PhoneAnsweredCount { get; set; }
    }
}
