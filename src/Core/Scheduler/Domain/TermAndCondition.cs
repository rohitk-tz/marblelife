using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class TermsAndCondition : DomainBase
    {
        public string TermAndCondition { get; set; }


        public long? TyepeId { get; set; }

        [ForeignKey("TyepeId")]
        public virtual Lookup Type { get; set; }
    }
}
