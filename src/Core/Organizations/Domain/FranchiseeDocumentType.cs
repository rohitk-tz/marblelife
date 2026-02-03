using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class FranchiseeDocumentType : DomainBase
    {
        public long FranchiseeId { get; set; }
        public long DocumentTypeId { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Organization Franchisee { get; set; }

        [ForeignKey("DocumentTypeId")]
        public virtual DocumentType DocumentType { get; set; }
        public bool IsPerpetuity { get; set; }
        public bool IsRejected { get; set; }
    }
}

