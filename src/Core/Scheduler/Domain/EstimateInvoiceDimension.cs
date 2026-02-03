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
   public class EstimateInvoiceDimension : DomainBase
    {
        public long? EstimateInvoiceServiceId { get; set; }
        [ForeignKey("EstimateInvoiceServiceId")]
        public virtual EstimateInvoiceService EstimateInvoiceService { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Area { get; set; }
        public decimal? AreaTime { get; set; }
        public string Description { get; set; }

        public decimal? SetPrice { get; set; }
        public decimal? IncrementedPrice { get; set; }

        public long? UnitTypeId { get; set; }
        [ForeignKey("UnitTypeId")]
        public virtual Lookup UnitType { get; set; }
        public long? Dimension { get; set; }
    }
}
