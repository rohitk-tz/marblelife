
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
    public class FranchiseeTechMailService : DomainBase
    {
        public long FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public double Amount { get; set; }
        public long TechCount { get; set; }
        public bool IsGeneric { get; set; }
        public long MultiplicationFactor { get; set; }

    }
}
