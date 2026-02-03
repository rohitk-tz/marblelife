using Core.Application.Attribute;
using System;

namespace Core.Review.ViewModel
{
    public class CustomerFeedbackReportViewModel
    {
        public long Id { get; set; }
        [DownloadField(Required = false)]
        public long FranchiseeId { get; set; }
        public long CustomerId { get; set; }
        public string Customer { get; set; }
        public string ContactPerson { get; set; }
        public string CustomerEmail { get; set; }
        public string Franchisee { get; set; }
        public DateTime? ResponseReceivedDate { get; set; }
        public DateTime? ResponseSyncingDate { get; set; }
        public decimal Rating { get; set; }
        [DownloadField(Required = false)]
        public long ResponseId { get; set; }
        public string ResponseContent { get; set; }
        [DownloadField(Required = false)]
        public double Recommend { get; set; }
        [DownloadField(Required = false)]
        public bool IsFromNewReviewSystem { get; set; }
        [DownloadField(Required = false)]
        public int IsFromCustomerReviewTable { get; set; }
        public string CustomerNameFromAPI { get; set; }
        public long AuditStatusId { get; set; }
        public string AuditStatus { get; set; }
        public string FromTable { get; set; }
        public string From { get; set; }
    }
}
