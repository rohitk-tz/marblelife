using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class BeforeAfterImageListModel
    {
        public int TotalCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime Endate { get; set; }
        public List<BeforeAfterViewModel> BeforeAfterViewModel { get; set; }
        public bool IsReview { get; set; }
    }
    [NoValidatorRequired]
    public class BeforeAfterViewModel
    {
        public string AfterCss { get; set; }
        public string BeforeCss { get; set; }
        public int Index { get; set; }

        public string Image { get; set; }
        public long? Id { get; set; }
        public bool IsSelected { get; set; }
        public string FrachiseeName { get; set; }
        public string RelactiveLocationAfterImageUrl { get; set; }
        public string RelactiveLocationBeforeImageUrl { get; set; }
        public string RelactiveLocationBeforeImage { get; set; }
        public string RelactiveLocationAfterImage { get; set; }
        public string SelectedBy { get; set; }
        public DateTime? BeforeImageUploadedOn { get; set; }
        public DateTime? AfterImageUploadedOn { get; set; }
        public long? BeforeServiceId { get; set; }
        public long? AfterServiceId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    //New Review Marketing Page Model
    [NoValidatorRequired]
    public class ReviewMarketingImageViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsFranchiseeAdmin { get; set; }
        public long FranchiseeCount { get; set; }
        public string Message { get; set; }
        public ReviewMarketingImageViewModel()
        {
            ReviewMarketingFranchiseeViewModels = new List<ReviewMarketingFranchiseeViewModel>();
        }
        public List<ReviewMarketingFranchiseeViewModel> ReviewMarketingFranchiseeViewModels { get; set; }
    }
    [NoValidatorRequired]
    public class ReviewMarketingFranchiseeViewModel
    {
        public ReviewMarketingFranchiseeViewModel()
        {
            ReviewMarketingPersonViewModel = new List<ReviewMarketingPersonViewModel>();
        }
        public long FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public long PersonCount { get; set; }
        public string Message { get; set; }
        public List<ReviewMarketingPersonViewModel> ReviewMarketingPersonViewModel { get; set; }
    }
    [NoValidatorRequired]
    public class ReviewMarketingPersonViewModel
    {
        public ReviewMarketingPersonViewModel()
        {
            ReviewMarketingSchedulerViewModel = new List<ReviewMarketingSchedulerViewModel>();
        }
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string PersonName { get; set; }
        public long? PersonRoleId { get; set; }
        public string PersonRole { get; set; }
        public long SchedulerCount { get; set; }
        public string Message { get; set; }
        public bool IsActiveUser { get; set; }
        public List<ReviewMarketingSchedulerViewModel> ReviewMarketingSchedulerViewModel { get; set; }
    }
    [NoValidatorRequired]
    public class ReviewMarketingSchedulerViewModel
    {
        public ReviewMarketingSchedulerViewModel()
        {
            ReviewMarketingBeforeAfterImageViewModel = new List<ReviewMarketingBeforeAfterImageViewModel>();
        }
        public string Message { get; set; }
        public long Count { get; set; }
        public long? JobEstimateId { get; set; }
        public string S3BucketAfterImageUrlThumb { get; set; }
        public string S3BucketBeforeImageUrlThumb { get; set; }
        public string SchedulerName { get; set; }
        public string SchedulerUrl { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public bool IsComercialClass { get; set; }
        public string MarketingClass { get; set; }
        public decimal EstimateValue { get; set; }
        public List<ReviewMarketingBeforeAfterImageViewModel> ReviewMarketingBeforeAfterImageViewModel { get; set; }
    }
    [NoValidatorRequired]
    public class ReviewMarketingBeforeAfterImageViewModel
    {
        public string BeforeCss { get; set; }
        public string AfterCss { get; set; }
        public int Index { get; set; }
        public bool IsBestPicture { get; set; }
        public bool IsAddToLocalGallery { get; set; }
        public string Image { get; set; }
        public long? Id { get; set; }
        public bool IsSelected { get; set; }
        public string RelactiveLocationAfterImageUrl { get; set; }
        public string RelactiveLocationBeforeImageUrl { get; set; }
        public long? OrderNo { get; set; }
        public string RelactiveLocationAfterImageUrlBase64 { get; set; }
        public string RelactiveLocationBeforeImageUrlBase64 { get; set; }

        public string RelactiveLocationBeforeImage { get; set; }
        public string RelactiveLocationAfterImage { get; set; }
        public string SelectedBy { get; set; }
        public DateTime? BeforeImageUploadedOn { get; set; }
        public DateTime? AfterImageUploadedOn { get; set; }
        public long? BeforeServiceId { get; set; }
        public long? AfterServiceId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? BeforeImageId { get; set; }
        public long? AfterImageId { get; set; }
        public string SurfaceColor { get; set; }
        public string SurfaceType { get; set; }
        public string FinishMaterial { get; set; }
        public string SurfaceMaterial { get; set; }
        public string BuildingLocation { get; set; }
        public string ServicesType { get; set; }
        public string Description { get; set; }
        public string SchedulerUrl { get; set; }
        public bool IsJob { get; set; }
        public string Title { get; set; }
        public bool IsImageEmpty { get; set; }
        public long? JobId { get; set; }
        public long? EstimateId { get; set; }
        public bool IsComercialClass { get; set; }
        public string MarketingClass { get; set; }
        public string SchedulerNames { get; set; }
        public long? JobEstimateId { get; set; }
        public string CustomerName { get; set; }

        public long? ToBeGroupedById { get; set; }

        public long? BeforeImageFileId { get; set; }
        public long? AfterImageFileId { get; set; }

        public string Base64Before { get; set; }
        public string Base64After { get; set; }

        public string RelactiveLocationAfterImageUrlThumb { get; set; }
        public string RelactiveLocationBeforeImageUrlThumb { get; set; }

        public string S3BucketAfterImageUrlThumb { get; set; }
        public string S3BucketBeforeImageUrlThumb { get; set; }
        public string BeforeImageUploadedBy { get; set; }
        public string AfterImageUploadedBy { get; set; }
        public string LocalMarketingUrl { get; set; }
        public decimal EstimateValue { get; set; }
        public string EmptyImage { get; set; }
        public bool IsImagePairReviewed { get; set; }
        public long ImageTypeId { get; set; }
        public string BestPairMarkedBy { get; set; }
        public bool IsBeforeImageCroped { get; set; }
        public bool IsAfterImageCroped { get; set; }
    }
}
