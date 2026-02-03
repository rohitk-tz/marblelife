using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Scheduler.Domain
{
    public class JobEstimateServices : DomainBase
    {
       
        public long? ServiceTypeId { get; set; }
        public long? TypeId { get; set; }
        public long? CategoryId { get; set; }

        public long? PairId { get; set; }
        public string SurfaceColor { get; set; }
        
        public string FinishMaterial { get; set; }
        
        public string SurfaceMaterial { get; set; }
     
        public string SurfaceType { get; set; }

        public string BuildingLocation { get; set; }
        public string CompanyName { get; set; }

        public string MaidService { get; set; }
        public string PropertyManager { get; set; }
        public string MAIDJANITORIAL { get; set; }

        public long? DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public bool? IsBeforeImage { get; set; }

        [ForeignKey("CategoryId")]
        public virtual JobEstimateImageCategory JobEstimateImageCategory { get; set; }

        [ForeignKey("TypeId")]
        public virtual Lookup Lookup { get; set; }
        [ForeignKey("ServiceTypeId")]
        public virtual ServiceType ServiceType { get; set; }

        [ForeignKey("PairId")]
        public virtual JobEstimateServices JobEstimateImagePairing { get; set; }

        public int FloorNumber { get; set; }

        public bool IsFromEstimate { get; set; }
        public bool? IsFromInvoiceAttach { get; set; }
        public bool? IsInvoiceForJob { get; set; }
        public long? InvoiceNumber { get; set; }
    }
}
