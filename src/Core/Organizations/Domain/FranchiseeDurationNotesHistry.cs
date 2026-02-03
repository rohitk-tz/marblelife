using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Users.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
  public  class FranchiseeDurationNotesHistry : DomainBase
    {
        public string Description { get; set; }
        public long FranchiseeId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }
        public long StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Lookup Status { get; set; }
        public long TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual Lookup Type { get; set; }
        public decimal? Duration { get; set; }
        public long RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public long? ApprovedById { get; set; }
        [ForeignKey("ApprovedById")]
        public virtual Person ApprovedBy { get; set; }
    }
}
