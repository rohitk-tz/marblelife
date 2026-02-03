using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Core.Organizations.Domain
{
   public class LoanAdjustmentAudit : DomainBase
    {
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }
        public long LoanId { get; set; }
        [ForeignKey("LoanId")]
        public virtual FranchiseeLoan FranchiseeLoan { get; set; }

        public bool BeforeLoanAdjustment { get; set; }
        public bool AfterLoanAdjustment { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
