using Core.Application.Attribute;
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
    public class JobEstimateImage : DomainBase
    {
        public long? ServiceId { get; set; }
        public long? TypeId { get; set; }

        public bool IsBestImage { get; set; }

        public DateTime? BestFitMarkDateTime { get; set; }
        public bool IsAddToLocalGallery { get; set; }

        public DateTime? AddToGalleryDateTime { get; set; }
        public long? FileId { get; set; }


        [ForeignKey("ServiceId")]
        public virtual JobEstimateServices JobEstimateServices { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }


        [ForeignKey("TypeId")]
        public virtual Lookup Lookup { get; set; }

        public long? DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }



        public long? ThumbFileId { get; set; }

        [ForeignKey("ThumbFileId")]
        public virtual File ThumbFile { get; set; }

        public string BaseUrl { get; set; }
    }
}
