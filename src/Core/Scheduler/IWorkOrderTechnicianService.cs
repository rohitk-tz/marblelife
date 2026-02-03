using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler
{
   public interface IWorkOrderTechnicianService
    {
        List<TechnicianWorkOrderForInvoice> CreateTechnicianWorkOrderInvoice(EstimateInvoiceServiceEditModel invoice, List<TechnicianWorkOrderForInvoice> technicianWorkOrder, long? estimateInvoiceId);
    }
}
