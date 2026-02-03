using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Review.Domain
{
    public class CustomerReview
    {
        public CustomerReview()
        {
            Collection = new List<CustomerReviewForMarketing>();
        }
        public List<CustomerReviewForMarketing> Collection { get; set; }
    }
    public class CustomerReviewForMarketing
    {
        public long Id { get; set; }
        public long? FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string ContactPerson { get; set; }
        public DateTime? ResponseReceivedDate { get; set; }
        public DateTime? ResponseSyncingDate { get; set; }
        public string ResponseContent { get; set; }
        public decimal? Rating { get; set; }
        public long? Recommend { get; set; }
        public string CustomerNameFromAPI { get; set; }
        public long AuditStatusId { get; set; }
        public string AuditStatus { get; set; }
        public string from { get; set; }
    }
}
