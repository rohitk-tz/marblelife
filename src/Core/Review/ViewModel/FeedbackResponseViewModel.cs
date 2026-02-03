namespace Core.Review.ViewModel
{
    public class FeedbackResponseViewModel
    {
        public int rating { get; set; }
        public int recommend { get; set; }
        public string dateOfReview { get; set; }
        public bool showReview { get; set; }
        public string authorEmail { get; set; }
        public string authorName { get; set; }
        public string body { get; set; }
        public long customId { get; set; }
        public long FeedbackId { get; set; }
    }
}
