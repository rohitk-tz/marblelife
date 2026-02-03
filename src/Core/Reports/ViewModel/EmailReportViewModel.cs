namespace Core.Reports.ViewModel
{
    public class EmailReportViewModel
    {
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public double CurrentCustomers { get; set; }
        public double PreviousCustomers { get; set; }
        public double CurrentCustomerWithEmail { get; set; }
        public double PreviousCustomerWithEmail { get; set; }

    }
}
