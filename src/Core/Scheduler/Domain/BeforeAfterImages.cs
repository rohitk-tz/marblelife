using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Users.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Scheduler.Domain
{
   public class BeforeAfterImages : DomainBase
    {
        public long? ServiceTypeId { get; set; }
        public long? TypeId { get; set; }
        public long? CategoryId { get; set; }

        public long? PairId { get; set; }
        public string SurfaceColor { get; set; }

        public string FinishMaterial { get; set; }

        public string SurfaceMaterial { get; set; }

        public string SurfaceType { get; set; }

        public long? DataRecorderMetaDataId { get; set; }

        public string BuildingLocation { get; set; }
        public string CompanyName { get; set; }

        public string MaidService { get; set; }
        public string PropertyManager { get; set; }
        public string MAIDJANITORIAL { get; set; }
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
        public long? SchedulerId { get; set; }
        public long? MarkertingClassId { get; set; }
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        [ForeignKey("JobId")]
        public virtual JobScheduler JobSchedulerJobId { get; set; }
        [ForeignKey("EstimateId")]
        public virtual JobScheduler JobSchedulerEstimateId { get; set; }

        [ForeignKey("MarkertingClassId")]
        public virtual MarketingClass MarketingClass { get; set; }
        [ForeignKey("SchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }

        public long? ServiceId { get; set; }

        public bool IsBestImage { get; set; }

        public DateTime? BestFitMarkDateTime { get; set; }
        public bool IsAddToLocalGallery { get; set; }

        public DateTime? AddToGalleryDateTime { get; set; }
        public long? FileId { get; set; }


        [ForeignKey("ServiceId")]
        public virtual JobEstimateServices JobEstimateServices { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }

        public long? FranchiseeId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public string PersonName { get; set; }

        public long? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Person Person { get; set; }
        public long? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        public string ImageUrl { get; set; }

        public long? ThumbFileId { get; set; }

        [ForeignKey("ThumbFileId")]
        public virtual File ThumbFile { get; set; }

        public string BaseUrl { get; set; }
        public string S3BucketURL { get; set; }
        public string S3BucketThumbURL { get; set; }
        public bool IsImageCropped { get; set; }
        public long? CroppedImageId { get; set; }
        public long? CroppedImageThumbId { get; set; }
        public bool IsImagePairReviewed { get; set; }
        public long? BestPairMarkedBy { get; set; }

        [ForeignKey("BestPairMarkedBy")]
        public virtual Person Person1 { get; set; }
        public bool IsImageUpdated { get; set; }
        public bool IsImageMigrateToCalendar { get; set; }
    }
}
