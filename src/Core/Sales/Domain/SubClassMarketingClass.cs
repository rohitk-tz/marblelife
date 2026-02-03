using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Core.Sales.Domain
{
    public class SubClassMarketingClass : DomainBase
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public long? MarketingclassId { get; set; }

        [ForeignKey("MarketingclassId")]
        public virtual MasterMarketingClass MasterMarketingClass { get; set; }
    }
}
