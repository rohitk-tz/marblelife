using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Domain;
using Core.Users.Domain;

namespace Core.Scheduler.Domain
{
    public class CustomerSignature : DomainBase
    {
        
        public long? EstimateCustomerId { get; set; }

        [ForeignKey("EstimateCustomerId")]
        public virtual EstimateInvoiceCustomer EstimateInvoiceCustomer { get; set; }

        public long? CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual JobCustomer Customer { get; set; }

        public string Signature { get; set; }
        public string Name { get; set; }

        public long? EstimateInvoiceId { get; set; }

        [ForeignKey("EstimateInvoiceId")]
        public virtual EstimateInvoice EstimateInvoice { get; set; }

        public DateTime? SignedDateTime { get; set; }

        public long? InvoiceNumber { get; set; }

        public long? TypeId { get; set; }

        [ForeignKey("TypeId")]
        public virtual Lookup SignatureType { get; set; }

        public long? JobSchedulerId { get; set; }

        [ForeignKey("JobSchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }

        public long? SignedById { get; set; }

        [ForeignKey("SignedById")]
        public virtual Person Person { get; set; }

        public bool? IsFromUrl { get; set; }
    }
}
