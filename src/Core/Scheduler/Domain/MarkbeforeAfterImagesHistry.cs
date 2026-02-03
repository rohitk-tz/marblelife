
using Core.Application.Attribute;
using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class MarkbeforeAfterImagesHistry : DomainBase
    {
        public long? ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public virtual JobEstimateServices JobEstimateServices { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [CascadeEntity]
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public long? BestTypeId { get; set; }
        [ForeignKey("BestTypeId")]
        public virtual Lookup BestType { get; set; }

        public long? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Lookup Category { get; set; }

        public bool IsFromJobEstimate { get; set; }

    }
}
