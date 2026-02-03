using Core.Application.Attribute;
using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class JobNote : DomainBase
    {
        public long? JobId { get; set; }
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        public long? EstimateId { get; set; }
        [ForeignKey("EstimateId")]
        public virtual JobEstimate JobEstimate { get; set; }

        public long? StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual JobStatus JobStatus { get; set; }
        public string Note { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long? VacationId { get; set; }
        [ForeignKey("VacationId")]
        public virtual JobScheduler JobScheduler { get; set; }

        //public long? MeetingId { get; set; }
        //[ForeignKey("MeetingId")]
        //public virtual JobScheduler JobScheduler1 { get; set; }
    }
}
