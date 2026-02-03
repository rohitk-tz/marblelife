namespace Core.Reports.ViewModel
{
    public class ReviewChartDataViewModel
    {
        public int Year { get; set; }
        public int month { get; set; }
        public double TotalCustomers { get; set; }
        public double TotalCustomersWithReview { get; set; }
        public double CurrentCustomers { get; set; }
        public double CurrentCustomersWithReview { get; set; }
        public double BestCount { get; set; }
        public double BestCountWithReview { get; set; }
    }
}
