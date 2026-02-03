using System;

namespace Core.Dashboard.ViewModel
{
    public class DocumentSummaryViewModel
    {
        public string DocumentName { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool isExpiring { get; set; }
        public long documentTypeId { get; set; }
    }
}
