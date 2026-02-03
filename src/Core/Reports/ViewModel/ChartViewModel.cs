using System;

namespace Core.Reports.ViewModel
{
    public class ChartViewModel
    {
        public DateTime Date { get; set; }
        public decimal Current { get; set; }
        public decimal Best { get; set; }
        public decimal Total { get; set; }

    }
}
