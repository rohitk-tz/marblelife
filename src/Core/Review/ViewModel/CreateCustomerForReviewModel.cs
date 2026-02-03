namespace Core.Review.ViewModel
{
    public class CreateCustomerForReviewModel
    {
        public long businessId { get; set; }
        public string clientId { get; set; }
        public string customerEmail { get; set; }
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public string customerPhone { get; set; }
        public string hash { get; set; }
        public int sendFeedbackRequest { get; set; }
        public string communicationPreference { get; set; }
        public long customerCustomId { get; set; }
    }
}
