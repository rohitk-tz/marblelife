using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Domain
{
    public class CustomerLog : DomainBase
    {
        public long? CustomerId { get; set; }
        public long? EstimateCustomerId { get; set; }
        public DateTime LoggedInAt { get; set; }
        public DateTime? LoggedOutAt { get; set; }
        public string SessionId { get; set; }
        public string DeviceKey { get; set; }
        public string ClientIp { get; set; }
        public string Browser { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
        [ForeignKey("EstimateCustomerId")]
        public virtual EstimateInvoiceCustomer EstimateInvoiceCustomer { get; set; }

        public long? EstimateInvoiceId { get; set; }

        [ForeignKey("EstimateInvoiceId")]
        public virtual EstimateInvoice EstimateInvoice { get; set; }

        public bool? IsPostSignature { get; set; }

        public long? TypeId { get; set; }
        public string Code { get; set; }
    }
}
