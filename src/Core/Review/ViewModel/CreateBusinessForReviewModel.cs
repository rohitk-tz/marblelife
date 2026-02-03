namespace Core.Review.ViewModel
{
    public class CreateBusinessForReviewModel
    {
        public string businessName { get; set; }
        public string businessOwnerEmail { get; set; }
        public string city { get; set; }
        public string clientId { get; set; }
        public string country { get; set; }
        public string hash { get; set; }
        public string phone { get; set; }
        public string state { get; set; }
        public string streetAddress { get; set; }
        public string zip { get; set; }
        public string package { get; set; }
        public int businessId { get; set; }
        public int page { get; set; } 
    }
}
