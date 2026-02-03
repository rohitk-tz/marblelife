using Core.Application.Domain;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class TermsAndConditionFranchisee : DomainBase
    {
        public string TermAndCondition { get; set; }

        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long? TyepeId { get; set; }

        [ForeignKey("TyepeId")]
        public virtual Lookup Type { get; set; }
    }
}
