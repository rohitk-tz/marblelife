using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobResourceEditModel : EditModelBase
    {
        public long Id { get; set; }
        public long? FileId { get; set; } 
        public long? JobId { get; set; }
        public long? EstimateId { get; set; } 
        public long? StatusId { get; set; }
        public string Status { get; set; } 
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Caption { get; set; }
        public decimal Size { get; set; } 
        public string RelativeLocation { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    } 
}
