using Core.Notification.Domain;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler
{
    public interface IJobService
    {
        void Save(JobEditModel model);
        JobListModel GetJobs(JobListFilter filter, int pageNumber, int pageSize);
        bool ChangeStatus(long jobId, long statusId);
        JobEditModel Get(long jobId);
        bool IsValidQbNumber(string qbInvoice);
        bool UpdateInfo(long schedulerId, string qbInvoiceNumber, long? userId);
        bool Delete(long jobId);
        bool CheckAvailability(long jobId, long techId, DateTime startDate, DateTime endDate, bool isVacation);
        ScheduleAvailabilityFilterViewModel CheckAvailabilityList(ScheduleAvailabilityFilterList model, bool isVacation);
        bool SaveMediaFiles(FileUploadModel model);
        long SaveMediaFilesForUsers(FileUploadModel model);
        FileUploadModel GetMediaList(long id, long mediaType,long? estimateId, long? userId);
        bool SaveNotes(SchedulerNoteModel model);
        JobListModel GetHolidayList(long franchiseeId);
        ICollection<JobScheduler> GetSchedulerForUserIds(ICollection<long> userIds);
        long SetDefaultAssignee(long id, long franchiseeId, DateTime startDate, DateTime endDate, bool isJob);
        string GetCustomerAddress(long jobId, long estimateId);
        MailListModel GetMailList(MailListFilter query);
        EmailTemplate GetMailTemplate(long id);
        bool EditMailTemplate(long? id, bool isActive);
        bool CheckAvailabilityForJob(long id, long assigneeId, DateTime startDate, DateTime endDate, bool isVacation);
        IEnumerable<BeforeAfterImageModel> SaveJobEstimateMediaFiles(FileUploadModel model);
        IEnumerable<InvoiceLineImageModel> SaveInvoiceLineMediaFiles(FileUploadModel model);
        bool SaveBeforeAfterImages(JobEstimateCategoryViewModel model);

        bool BeforeAfterImageMailSend(long? id, BeforeAfterImageSendMailViewModel isActive, string templateName,long? franchiseeId);
        bool InvoiceMailSend(long? id, JobEstimateServiceViewModel model, string templateName, long? franchiseeId);
        bool IsEligibleForDeletion(BeforeAfterImageDeletionViewModel model);
        bool CheckAvailabilityForJobForSalesRep(long id, long assigneeId, DateTime startDate, DateTime endDate, bool isVacation);
        bool SaveFileForImageAttachment(long? invoiceId, FranchiseeSales franchiseeSales, Job job, long? userId, bool isFromJob,long? schedulerId);
        JobListModel GetHolidayListMonthWise(FranchiseeHolidayModel model);
        bool CheckAvailabilityForMeeting(long id, long assigneeId, DateTime startDate, DateTime endDate, JobEstimateEditModel model);
        FranchiseInfoModel GetFranchiseeInfo(long? franchiseeId);
        DragDropSchedulerEnum SaveDragDropEvent(DragDropSchedulerModel model);
        ConfirmationResponseModel ConfirmSchedule(ConfirmationModel model);
        ConfirmationResponseModel ConfirmScheduleFromUI(ConfirmationModel model);
        bool EditJobNotes(JobNoteEditModel model);
        bool DeleteNotes(long? Id);
        CustomerInfoModel GetCustomerInfo(string customerName);

        ReviewMarketingImageViewModel GetBeforeAfterImages(LocalMarketingReviewFilter filter);

        bool SaveImagesBestPair(SaveImagesBestPairFilter filter);
        bool SaveReviewMarkImage(SaveReviewImageFilter filter);
        BeforeAfterForFranchieeAdminGroupedViewModel GetBeforeAfterImagesForFranchiseeAdmin(BeforeAfterImageFilter filter);
        bool SaveBeforeAfterImagesForFranchiseeAdmin(SaveBeforeAfterImageFilter filter);
        BeforeAfterForFranchieeAdminGroupedViewModel GetBeforeAfterImagesForFranchiseeAdminV2(BeforeAfterImageFilter filter);
       bool SaveEstimateWorth(EstimateWorthModel filter);

        bool ShiftImagesToInvoiceBuildMaterial(ShiftJobEstimateViewModel model);
        bool EditEmailTemplate(MailTemplateEditModel filter);
        bool SaveInvoiceRequired(InvoiceRequiredViewModel filter);
        FileUploadModel GetInvoiceMediaList(long rowId, long mediaType, long? estimateId, long? userId);
        bool SaveImageRotation(RotationImageModel model);
        bool SaveCroppedImage(CroppedImageModel model, long userId);
        LocalMarketingReviewModel GetLocalMarketingReview(LocalMarketingReviewFilter filter);
        List<SalesRepTechnicianModel> GetSalesRepTechnician(long? franchiseeId, long LoggedUserId, long RoleId, long LoggedInFranchiseeId);
        bool MarkImageAsReviwed(BeforeAfterImagesLocalMarketingModel model);
        bool MarkImageAsBestPair(BeforeAfterImagesLocalMarketingModel model, long LoggedUserId, long RoleId, long LoggedInFranchiseeId);
        bool MarkImageAsAddToLocalGallery(BeforeAfterImagesLocalMarketingModel model);
        string RemoveWhitespace(string input);
        bool BestPairMarkedForJobEstimateImagePair(JobEstimateImagePairMarkedModel model, long LoggedUserId, long RoleId, long LoggedInFranchiseeId);
    }
}
