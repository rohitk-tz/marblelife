using Core.Application.Attribute;
using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class JobResource : DomainBase
    {
        public long? JobId { get; set; }
        public long? EstimateId { get; set; } 
        public long? StatusId { get; set; }
        public long FileId { get; set; }

        [ForeignKey("StatusId")]
        public virtual JobStatus JobStatus { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long? VacationId { get; set; }
        public long? MeetingId { get; set; }

    }
}
