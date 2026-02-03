using System;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    public class AccountCreditEditModel
    {
        public long Id { get; set; }

        public DateTime CreditedOn { get; set; }
        public long CustomerId { get; set; }
        public string QbInvoiceNumber { get; set; }

        public IList<AccountCreditItemEditModel> AccountCreditItems { get; set; }

        public AccountCreditEditModel()
        {
            AccountCreditItems = new List<AccountCreditItemEditModel>();
        }
    }
}
