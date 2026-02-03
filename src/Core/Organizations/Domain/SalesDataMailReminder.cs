using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class SalesDataMailReminder : DomainBase    {
     
        public long FranchiseeId { get; set; }
      
        public DateTime Date { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
    }
}
