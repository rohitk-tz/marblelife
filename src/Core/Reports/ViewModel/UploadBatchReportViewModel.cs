using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class UploadBatchReportViewModel
    {
        public long FranchiseeId { get; set; }
        public long FeeProfileId { get; set; }
        public string FeeProfile { get; set; }
        public string Franchisee { get; set; }
        public IEnumerable<UploadBatchCollectionViewModel> RecordCollection { get; set; }   
    }
}
