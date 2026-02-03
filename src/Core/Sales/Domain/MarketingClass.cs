using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Core.Sales.Domain
{
    public class MarketingClass : DomainBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ColorCode { get; set; }
        public string Alias { get; set; }
        public long? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Application.Domain.Lookup Category { get; set; }
        public int? NewOrderBy { get; set; }

    }
}
