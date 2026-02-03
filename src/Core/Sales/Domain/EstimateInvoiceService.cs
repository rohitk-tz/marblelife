using Core.Application.Domain;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.Domain
{
    public class EstimateInvoiceService : DomainBase
    {
        public string ServiceName { get; set; }
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public string ServiceType { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string TypeOfService { get; set; }
        public string StoneType { get; set; }
        public string StoneColor { get; set; }
        public string Price { get; set; }
        public long EstimateInvoiceId { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Notes { get; set; }
        public string PriceNotes { get; set; }
        public string StoneType2 { get; set; }
        public int InvoiceNumber { get; set; }
        public bool IsCross { get; set; }
        public long? ParentId { get; set; }
        public bool IsBundle { get; set; }
        public bool IsActive { get; set; }
        public string BundleName { get; set; }
        public bool IsMainBundle { get; set; }
        public string Alias { get; set; }

        [ForeignKey("EstimateInvoiceId")]
        public virtual EstimateInvoice EstimateInvoice { get; set; }

        [ForeignKey("ParentId")]
        public virtual EstimateInvoiceService EstimateInvoiceServiceDomain { get; set; }

        public long? InvoiceImageId { get; set; }

        [ForeignKey("InvoiceImageId")]
        public virtual JobEstimateImage JobEstimateImage { get; set; }

        public long? ServiceTagId { get; set; }
        [ForeignKey("ServiceTagId")]
        public virtual Lookup ServiceTag { get; set; }
    }
}
