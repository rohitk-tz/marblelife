using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class ServiceType : DomainBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public long CategoryId { get; set; }
        public string Alias { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Lookup Category { get; set; }
        public string ColorCode { get; set; }

        public int? OrderBy { get; set; }

        public long? SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual Lookup SubCategory { get; set; }
        public int? NewOrderBy { get; set; }

        public bool DashboardServices { get; set; }
    }
}
