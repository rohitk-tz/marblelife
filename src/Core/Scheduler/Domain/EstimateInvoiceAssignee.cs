using Core.Application.Domain;
using Core.Organizations.Domain;
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
    public class EstimateInvoiceAssignee : DomainBase
    {     
        public long? EstimateId { get; set; }
        public long? EstimateInvoiceId { get; set; }
        public long? SchedulerId { get; set; }
        public long? InvoiceNumber { get; set; }
        public string Label { get; set; }
        public long? AssigneeId { get; set; }
        [ForeignKey("AssigneeId")]
        public virtual OrganizationRoleUser User { get; set; }

        [ForeignKey("EstimateId")]
        public virtual JobEstimate JobEstimate { get; set; }
        [ForeignKey("EstimateInvoiceId")]
        public virtual EstimateInvoice EstimateInvoice { get; set; }
    }
}
