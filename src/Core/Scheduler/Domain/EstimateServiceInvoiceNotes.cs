using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Sales.Domain;
using Core.Users.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class EstimateServiceInvoiceNotes : DomainBase
    {
        public long? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public string Notes { get; set; }

        public long? EstimateinvoiceId { get; set; }
        [ForeignKey("EstimateinvoiceId")]
        public virtual EstimateInvoice Estimateinvoice { get; set; }

        public long? InvoiceNumber { get; set; }
    }
}
