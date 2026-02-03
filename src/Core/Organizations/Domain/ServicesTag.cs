using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class ServicesTag : DomainBase
    {
        public long ServiceTypeId { get; set; }
        public long CategoryId { get; set; }
        public string MaterialType { get; set; }
        public string Service { get; set; }
        public string Notes { get; set; }
        public long? NotesSavedBy { get; set; }
        public bool IsActive { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Lookup Category { get; set; }

        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }
    }
}
