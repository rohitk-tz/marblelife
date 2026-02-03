using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations
{
    public interface IFranchiseeTechnicianMailService
    {
        InvoiceItemEditModel CreateModel(FranchiseeTechMailService mailServiceFee, FranchiseeInvoice invoice,
            DateTime? startdate = null, DateTime? endDate = null);
        long Save(InvoiceItemEditModel model, FranchiseeTechMailService mailServiceFee, long invoiceId);
    }
}
