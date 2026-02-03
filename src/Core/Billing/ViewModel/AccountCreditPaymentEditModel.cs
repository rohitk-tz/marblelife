using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Billing.ViewModel
{
    public class AccountCreditPaymentEditModel
    {
        public string AccountCreditId { get; set; }
        public string Description { get; set; }
        public Decimal Amount { get; set; }
    }
}
