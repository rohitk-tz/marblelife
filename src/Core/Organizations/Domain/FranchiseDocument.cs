using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseDocument : DomainBase
    {
        public long? FileId { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public string UploadFor { get; set; }
        public bool IsImportant { get; set; }
        public bool ShowToUser { get; set; }

        public long? DocumentTypeId { get; set; }
        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; }
        public long? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual OrganizationRoleUser OrganizationRoleUser { get; set; }
        public bool IsPerpetuity { get; set; }
        public bool IsRejected { get; set; }
    }
}
