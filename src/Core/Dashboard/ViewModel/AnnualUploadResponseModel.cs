using Core.Application.Attribute;
using System;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class AnnualUploadResponseModel
    {
        public bool IsUploaded { get; set; }
        public bool isFailed { get; set; }
        public bool LastUploadFailed { get; set; }
        public bool isRejected { get; set; }
        public DateTime NotificationStartDate { get; set; }
        public int Year { get; set; }
        public DateTime UploadStartDate { get; set; }
        public DateTime UploadEndDate { get; set; }
        public bool IsLastBatchUploaded { get; set; }
    }
}
