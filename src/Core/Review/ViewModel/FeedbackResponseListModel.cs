using System.Collections.Generic;

namespace Core.Review.ViewModel
{
    public class FeedbackResponseListModel
    {
        public int page { get; set; }
        public int perPage { get; set; }
        public int pages { get; set; }
        public int count { get; set; } 
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public ICollection<FeedbackResponseViewModel> reviews { get; set; }

    }
}
