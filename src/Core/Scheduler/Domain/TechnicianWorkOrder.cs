using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
   public class TechnicianWorkOrder : DomainBase
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public long? WorkOrderId { get; set; }
        [ForeignKey("WorkOrderId")]
        public virtual Lookup WorkOrder { get; set; }
    }
}
