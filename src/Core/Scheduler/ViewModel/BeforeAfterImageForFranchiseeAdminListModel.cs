using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class BeforeAfterImageForFAListModel
    {
        public BeforeAfterImageForFAListModel()
        {
            BeforeAfterGroupedViewModel = new List<BeforeAfterForFranchieeAdminGroupedViewModel>();
        }
        public int TotalCount { get; set; }
        public DateTime StartDate { get; set; }
        public long? FranchiseeId { get; set; }
        public DateTime Endate { get; set; }
        public List<BeforeAfterForFranchieeAdminGroupedViewModel> BeforeAfterGroupedViewModel { get; set; }

    }

    [NoValidatorRequired]
    public class BeforeAfterForFranchieeAdminGroupedViewModel
    {
        public BeforeAfterForFranchieeAdminGroupedViewModel()
        {
            BeforeAfterPersonViewModel = new List<BeforeAfterForPersonViewModel>();
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FrachiseeName { get; set; }
        public long? FrachiseeId { get; set; }
        public List<BeforeAfterForPersonViewModel> BeforeAfterPersonViewModel { get; set; }
        public int TotalCount { get; set; }
    }

    [NoValidatorRequired]
    public class BeforeAfterForPersonViewModel
    {
        public BeforeAfterForPersonViewModel()
        {
            BeforeAfterImageViewModel = new List<BeforeAfterForImageViewModel>();
            NonResidentalCollection = new List<NonResidentalImageViewModel>();
            ResidentalCollection = new List<BeforeAfterForImageViewModel>();
        }
        public string Message { get; set; }
        public long RoleId { get; set; }
        public string PersonName { get; set; }
        public string RoleName { get; set; }
        public long UserId { get; set; }
        public List<BeforeAfterForImageViewModel> BeforeAfterImageViewModel { get; set; }
        public List<NonResidentalImageViewModel> NonResidentalCollection { get; set; }
        //public List<BeforeAfterForImageViewModel> NonResidentalCollection { get; set; }
        public List<BeforeAfterForImageViewModel> ResidentalCollection { get; set; }
        public int TotalCount { get; set; }
        public List<CustomerForImageViewModel> CustomerImageViewModel { get; set; }
    }


    [NoValidatorRequired]
    public class CustomerForImageViewModel
    {
        public string CustomerName { get; set; }
        public string PersonName { get; set; }
        public string RoleName { get; set; }
        public List<BeforeAfterForImageViewModel> CustomerForImageModel { get; set; }
    }

    [NoValidatorRequired]
    public class BeforeAfterForImageViewModel
    {
        public string BeforeCss { get; set; }
        public string AfterCss { get; set; }
        public int Index { get; set; }
        public bool IsBestPicture { get; set; }
        public bool IsAddToLocalGallery { get; set; }
        public string Image { get; set; }
        public long? Id { get; set; }
        public bool IsSelected { get; set; }
        public string RelactiveLocationExteriorImageUrl { get; set; }
        public string RelactiveLocationExteriorImage { get; set; }
        public string RelactiveLocationAfterImageUrl { get; set; }
        public string RelactiveLocationBeforeImageUrl { get; set; }
        public long? OrderNo { get; set; }
        public string RelactiveLocationExteriorImageUrlBase64 { get; set; }
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
        public string SurfaceColor {get;set;}
        public string SurfaceType { get; set; }
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
        public long? ExteriorImageFileId { get; set; }

        public string Base64Before { get; set; }
        public string Base64After { get; set; }

        public string RelactiveLocationAfterImageUrlThumb { get; set; }
        public string RelactiveLocationBeforeImageUrlThumb { get; set; }

        public string S3BucketAfterImageUrlThumb { get; set; }
        public string S3BucketBeforeImageUrlThumb { get; set; }

        public string S3BucketExteriorImageUrlThumb { get; set; }
    }

    [NoValidatorRequired]
    public class NonResidentalImageViewModel
    {
        public string SchedulerNames { get; set; }
        public string CustomerName { get; set; }
        public  string NonResidentalImageUrl { get; set; }
       public List<BeforeAfterForImageViewModel> beforeAfterViewModel { get; set; }

    }
}