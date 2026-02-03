using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
   public class TechnicianWorkOrderForInvoice : DomainBase
    {
        public long? TechnicianWorkOrderId { get; set; }
        [ForeignKey("TechnicianWorkOrderId")]
        public virtual TechnicianWorkOrder TechnicianWorkOrder { get; set; }

        public long? EstimateinvoiceId { get; set; }
        [ForeignKey("EstimateinvoiceId")]
        public virtual EstimateInvoice EstimateInvoice { get; set; }
        public bool IsActive { get; set; }

        public long? InvoiceNumber { get; set; }

    }
}
