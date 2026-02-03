using Core.Sales.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
    public class CustomerSignatureInfo : DomainBase
    {
        public string Code { get; set; }

        public bool IsActive { get; set; }
        public bool IsFromUrl { get; set; }

        public long? EstimateInvoiceId { get; set; }

        [ForeignKey("EstimateInvoiceId")]
        public virtual EstimateInvoice EstimateInvoice { get; set; }

        public long? TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual Application.Domain.Lookup SignatureType { get; set; }

        public long? JobSchedulerId { get; set; }

        [ForeignKey("JobSchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }

      
    }
}
