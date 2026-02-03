using System;

namespace Core.Review.ViewModel
{
    public class FeedbackRequestModel
    {
        public long businessId { get; set; }
        public string clientId { get; set; }
        public string from { get; set; }
        public string hash { get; set; }
        public int page { get; set; }
        public string to { get; set; }
    }
}
