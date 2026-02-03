using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class CalendarFileUpload : DomainBase
    {
        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long FileId { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        public long? LogFileId { get; set; }

        public int FailedRecords { get; set; }
        public int SavedRecords { get; set; }
        public int TotalRecords { get; set; } 

        public long AssigneeId { get; set; }
        [ForeignKey("AssigneeId")]
        public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }

        public long TypeId { get; set; }
        public long StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Lookup Lookup { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long TimeZoneId { get; set; }
        [ForeignKey("TimeZoneId")]
        public virtual TimeZoneInformation TimeZoneInformation { get; set; } 
    }
}
