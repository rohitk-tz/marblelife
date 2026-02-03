using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    public class UploadBatchReportListModel
    {
        public IEnumerable<UploadBatchCollectionViewModel> Collection { get; set; } 
        public UploadReportFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; } 
    }
}
