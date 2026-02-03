using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
   public class AccountCredit : DomainBase
    {
        public long CustomerId { get; set; }

        public string QbInvoiceNumber { get; set; }

        public DateTime CreditedOn { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<AccountCreditItem> CreditMemoItems { get; set; }
    }
}
