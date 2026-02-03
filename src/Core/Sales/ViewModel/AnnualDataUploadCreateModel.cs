using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired] 
    public class AnnualDataUploadCreateModel : EditModelBase
    {
        public long Id { get; set; }
        public long FranchiseeId { get; set; }
        public DateTime AnnualUploadStartDate { get; set; }
        public DateTime AnnualUploadEndDate { get; set; }
        public long StatusId { get; set; }
        public FileModel AnnualFile { get; set; }
        public FeedbackMessageModel Message { get; set; }
        public string year { get; set; }
    }
}
