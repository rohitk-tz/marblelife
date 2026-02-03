using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class AccountCreditPayment : DomainBase
    {
        public long PaymentId { get; set; }
        public long FranchiseeAccountCreditId { get; set; }
        public decimal Amount { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payment { get; set; }

        [ForeignKey("FranchiseeAccountCreditId")]
        public virtual FranchiseeAccountCredit FranchiseeAccountCredit { get; set; }
    }
}
