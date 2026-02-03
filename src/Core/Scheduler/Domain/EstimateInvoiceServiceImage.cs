using Core.Application.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
   public class EstimateInvoiceServiceImage : DomainBase
    { 
        
        public long? FileId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public long? EstimateInvoiceServiceId { get; set; }
        [ForeignKey("EstimateInvoiceServiceId")]
        public virtual EstimateInvoiceService EstimateInvoiceService { get; set; }
        
        public long? EstimateInvoiceId { get; set; }
        [ForeignKey("EstimateInvoiceId")]
        public virtual EstimateInvoice EstimateInvoice { get; set; }
        public long? EstimateId { get; set; }
        [ForeignKey("EstimateId")]
        public virtual JobEstimate Estimate { get; set; }
        [ForeignKey("FileId")]
        public virtual File File { get; set; }
        public bool IsBeforeAfter { get; set; }
    }
}
