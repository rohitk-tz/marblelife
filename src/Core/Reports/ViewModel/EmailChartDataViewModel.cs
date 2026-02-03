namespace Core.Reports.ViewModel
{
    public class EmailChartDataViewModel
    {
        public int Year { get; set; }
        public int month { get; set; }
        public double TotalCustomers { get; set; }
        public double TotalCustomersWithEmail { get; set; }
        public double CurrentCustomers { get; set; }
        public double CurrentCustomersWithEmail { get; set; }
        public double BestCount { get; set; }
        public double BestCountWithEmail { get; set; }
    }
}
