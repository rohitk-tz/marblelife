using Core.Notification.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.ViewModel
{
    public class LocalMarketingViewModel
    {
        public LocalMarketingViewModel()
        {
            BeforeAfterPersonsViewModal = new List<BeforeAfterPersonsViewModal>();
        }

        public LocalMarketingViewModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            BeforeAfterPersonsViewModal = new List<BeforeAfterPersonsViewModal>();
            Base = emailNotificationModelBase;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FranchiseeName { get; set; }
        public long FranchiseeId { get; set; }
        public List<BeforeAfterPersonsViewModal> BeforeAfterPersonsViewModal { get; set; }
        public EmailNotificationModelBase Base { get; private set; }
    }
    public class BeforeAfterPersonsViewModal
    {
        public BeforeAfterPersonsViewModal()
        {
            JobEstimateForBeforeAfter = new List<JobEstimateForBeforeAfter>();
        }
        public long Id { get; set; }
        public string PersonName { get; set; }
        public string PersonRole { get; set; }
        public long PersonId { get; set; }
        public long RoleId { get; set; }
        public List<JobEstimateForBeforeAfter> JobEstimateForBeforeAfter { get; set; }
    }
    public class JobEstimateForBeforeAfter
    {
        public string Message { get; set; }
        public long Count { get; set; }
        public long? JobEstimateId { get; set; }
        public string S3BucketAfterImageUrlThumb { get; set; }
        public string S3BucketBeforeImageUrlThumb { get; set; }
        public string SchedulerNames { get; set; }
        public string SchedulerUrl { get; set; }
        public string CustomerName { get; set; }
        public string Title { get; set; }
        public bool IsComercialClass { get; set; }
        public string LocalMarketingUrl { get; set; }
        public decimal EstimateValue { get; set; }
        public JobEstimateForBeforeAfter()
        {
            BeforeAfterImageViewModal = new List<BeforeAfterImageViewModal>();
        }
        public List<BeforeAfterImageViewModal> BeforeAfterImageViewModal { get; set; }
    }
    public class BeforeAfterImageViewModal
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
        public string SurfaceColor { get; set; }
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
        public string BeforeImageUploadedBy { get; set; }
        public string AfterImageUploadedBy { get; set; }
        public string LocalMarketingUrl { get; set; }
        public decimal EstimateValue { get; set; }
    }

    public class EmailForWeeklyPhotoManagementModel
    {
        public WeeklyPhotoManagementModel WeeklyPhotoManagementModel { get; set; }
        public EmailNotificationModelBase Base { get; set; }
        public EmailForWeeklyPhotoManagementModel(EmailNotificationModelBase emailNotificationModelBase)
        {
            Base = emailNotificationModelBase;
        }
    }

    public class WeeklyPhotoManagementModel
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string LocalMarketingURL { get; set; }
    }
}
