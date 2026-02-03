using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    public class UploadBatchCollectionViewModel
    {
        [DownloadField(Required = false)]
        public long Id { get; set; }
        [DownloadField(Required = false)]
        public long? FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public string PaymentFrequency { get; set; }
        [DownloadField(Required = false)]
        public long FeeProfileId { get; set; }
        [DownloadField(Required = false)]
        public bool IsUploaded { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int WaitPeriod { get; set; }
        public DateTime ExpectedUploadDate { get; set; }
        public DateTime? ActualUploadDate { get; set; }
        public string UploadStatus { get; set; }
        public DateTime EndDateWithWaitTime { get; set; }
    }
}
