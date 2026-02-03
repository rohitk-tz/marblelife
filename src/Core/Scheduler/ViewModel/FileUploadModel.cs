using Core.Application.Attribute;
using Core.Application.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class FileUploadModel : EditModelBase
    {
        
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        public long? MeetingId { get; set; }
        public long? VacationId { get; set; }
        public long? StatusId { get; set; }
        public long? ServiceId { get; set; }
        public IEnumerable<FileModel> FileList { get; set; }
        public IEnumerable<JobResourceEditModel> Resources { get; set; }

        public JobEstimateCategoryViewModel ImageList { get; set; }

        public long? UserId { get; set; }

        public string css { get; set; }
    }
}
