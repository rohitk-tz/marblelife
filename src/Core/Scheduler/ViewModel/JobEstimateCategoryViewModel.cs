using System.Collections.Generic;
using Core.Application.Attribute;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobEstimateCategoryViewModel
    {
        public JobEstimateCategoryViewModel()
        {
            SliderImages = new JobEstimateServiceViewModel();
            InvoiceImages = new JobEstimateServiceViewModel();
        }
        public bool? IsFromBeforeAfterPane { get; set; }
        public long? Id { get; set; }
        public long? MarketingClassId { get; set; }
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        public long? SchedulerId { get; set; }
        public bool? IsChanged { get; set; }

        public long? UserId { get; set; }
       
        public List<ImagePairs> ImagePairs { get; set; }

        public JobEstimateServiceViewModel SliderImages { get; set; }
        public JobEstimateServiceViewModel InvoiceImages { get; set; }
    }
    [NoValidatorRequired]
    public class ImagePairs
    {
        public bool IsSelectedFinishMaterialProperty { get; set; }
        public bool isSelectedSurfaceColorProperty { get; set; }
        public bool isSelectedCompanyNameProperty { get; set; }
        public bool isSelectedMaidServiceProperty { get; set; }
        public bool isSelectedPropertyManagerProperty { get; set; }
        public bool isSelectedMaidJanitorialProperty { get; set; }
        public bool IsSelectedServiceTypeProperty { get; set; }
        public bool IsBestPicture { get; set; }
        public bool IsAddToLocalGallery { get; set; }

        public bool? IsChanged { get; set; }
        public ImagePairs()
        {
            BeforeImages = new JobEstimateServiceViewModel();
            AfterImages = new JobEstimateServiceViewModel();
         
        }
        public JobEstimateServiceViewModel BeforeImages { get; set; }
        public JobEstimateServiceViewModel AfterImages { get; set; }
        
    }
    [NoValidatorRequired]
    public class RotationImageModel
    {
        public RotationImageModel()
        {

        }
        public long? FileId { get; set; }
        public string Css { get; set; }
    }
    [NoValidatorRequired]
    public class CroppedImageModel
    {
        public CroppedImageModel()
        {

        }
        public long? FileId { get; set; }
        public long? BeforeAfterId { get; set; }
        public string Base64 { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
    }
}

