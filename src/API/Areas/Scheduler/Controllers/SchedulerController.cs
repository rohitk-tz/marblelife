using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Notification.Domain;
using Core.Organizations;
using Core.Scheduler;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using Core.Users;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Areas.Scheduler.Controllers
{

    public class SchedulerController : BaseController
    {
        public WkHtmltoPdfSwitches Switches { get; set; }
        private readonly IJobService _jobService;
        private readonly IUserService _userService;
        private readonly ISessionContext _sessionContext;
        private readonly IOrganizationRoleUserInfoService _orgRoleUserInfoService;
        private readonly IEstimateService _estimateService;
        
        public SchedulerController(ISessionContext sessionContext, IJobService jobService, IOrganizationRoleUserInfoService orgRoleUserInfoService,
            IEstimateService estimateService, IInvoiceService invoiceService, IUserService userService)
        {
            _sessionContext = sessionContext;
            _jobService = jobService;
            _orgRoleUserInfoService = orgRoleUserInfoService;
            _estimateService = estimateService;
            _userService = userService;

        }

        [HttpGet]
        public JobEditModel Get(long id)
        {

            return _jobService.Get(id);
        }

        [HttpPost]
        public bool SaveJob([FromBody] JobEditModel model)
        {
            string techNames = "";
            bool isNotAvailable = false;
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (model.TechIds == null || model.TechIds.Count() <= 0)
            {
                long techId = _jobService.SetDefaultAssignee(model.JobId, model.FranchiseeId, model.StartDate, model.EndDate, true);
                if (techId <= 0)
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Technician(s) are not available!!");
                    return false;
                }
                else
                    model.TechIds.Add(techId);
            }

            if (model.TechIds != null)
            {
                foreach (var item in model.TechIds)
                {
                    var result = _jobService.CheckAvailabilityForJob(model.JobId, item, model.StartDate, model.EndDate, false);
                    if (!result)
                    {
                        techNames += _userService.GetUserName(item) + " , ";
                        isNotAvailable = true;
                    }
                }
            }

            if (isNotAvailable)
            {
                var index = techNames.LastIndexOf(",");
                techNames = techNames.Substring(0, index);
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage(techNames + " Technician(s) are not available!!");
                isNotAvailable = false;
                return false;

            }
            if (model.JobOccurence != null && model.JobOccurence.Collection.Count() > 0)
            {
                var response = _estimateService.CheckDuplicateAssignment(model.JobOccurence);
                if (response)
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Unable to assigne a duplicate user for the same time!");
                    return false;
                }
            }
            model.LoggedInUserId = _sessionContext.UserSession.UserId;
            _jobService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Job created/Updated successfully! Invoice(s) will be Attached to Job(s) within few minutes!!");
            return true;
        }

        [HttpPost]
        public JobListModel GetJobList([FromBody] JobListFilter query)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive && query.FranchiseeId <= 1)
            {
                query.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.Technician)
            {
                query.TechId = _sessionContext.UserSession.UserId;
                query.ResourceIds = null;
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.SalesRep || _sessionContext.UserSession.RoleId == (long)RoleType.OperationsManager)
            {
                query.TechId = 0;
                if (query.ResourceIds == null || query.ResourceIds.Count() <= 0)
                    query.ResourceIds = _orgRoleUserInfoService.GetOrgRoleUserIdsByRole(_sessionContext.UserSession.UserId, _sessionContext.UserSession.OrganizationId);
                query.PersonId = _sessionContext.UserSession.UserId;
            }
            query.LoggedInUserOrgId= _sessionContext.UserSession.OrganizationRoleUserId;
            var x = _jobService.GetJobs(query, query.PageNumber, query.PageSize);
            return x;
        }

        [HttpPost]
        public bool ChangeStatus(long jobId, [FromBody] long statusId)
        {
            var result = _jobService.ChangeStatus(jobId, statusId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Job Status has been changed!");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Unable to change Job Status!");
                return false;
            }
        }

        [HttpGet]
        public bool IsValidQbNumber(string qbInvoice)
        {
            return _jobService.IsValidQbNumber(qbInvoice);
        }

        [HttpDelete]
        public bool Delete([FromUri] long id)
        {
            var result = _jobService.Delete(id);
            if (result == true)
            {
                ResponseModel.SetSuccessMessage("Job deleted successfully.");
                return true;
            }
            else
                ResponseModel.SetErrorMessage("Can't delete Job!");
            return false;
        }

        [HttpPost]
        public bool UpdateJobInfo(long jobId, [FromBody] string qbInvoiceNumber)
        {
            var userId = _sessionContext.UserSession.UserId;
            var result = _jobService.UpdateInfo(jobId, qbInvoiceNumber, userId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Qb Invoice updated successfully!");
                return true;
            }
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Unable to update QbInvoiceNumber!");
            return false;
        }

        [HttpPost]
        public bool CheckAvailability(ScheduleAvailabilityFilter model)
        {
            return _jobService.CheckAvailability(model.JobId, model.AssigneeId, model.StartDate, model.EndDate, false);
        }

        [HttpPost]
        public bool UploadMedia([FromBody] FileUploadModel model)
        {
            if ((model.JobId == null || model.FileList == null) && model.EstimateId <= 0)
                return false;
            var result = _jobService.SaveMediaFiles(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Media Uploaded!");
            }
            return result;
        }
        [HttpPost]
        public long UploadMediaForUser([FromBody] FileUploadModel model)
        {
            if ((model.JobId == null || model.FileList == null) && model.EstimateId <= 0)
                return default(long);
            var result = _jobService.SaveMediaFilesForUsers(model);
            if (result != default(long))
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Media Uploaded!");
            }
            return result;
        }

        [HttpPost]
        public FileUploadModel GetMediaList( [FromBody] MediaModel model)
        {
            var userId = _sessionContext.UserSession.OrganizationRoleUserId;
            return _jobService.GetMediaList(model.RowId, model.MediaType,model.EstimateId, userId);
        }

        [HttpPost]
        public bool SaveNotes([FromBody] SchedulerNoteModel model)
        {
            if (string.IsNullOrEmpty(model.Note))
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Can't add Blank Note!");
                return false;
            }
            if (model.JobId <= 0 && model.VacationId <= 0 && model.EstimateId <= 0 && model.MeetingId <= 0)
                return false;

            if (model.MeetingId != null)
            {
                model.VacationId = model.MeetingId;
            }
            var result = _jobService.SaveNotes(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Note Added!");
            }
            return result;
        }

        [HttpGet]
        public JobListModel GetHolidayList([FromUri] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _jobService.GetHolidayList(franchiseeId);
        }

        [HttpPost]
        public string GetCustomerAddress([FromUri] long jobId, [FromBody] long estimeId)
        {
            return _jobService.GetCustomerAddress(jobId, estimeId); ;
        }
        [HttpPost]
        public MailListModel GetMailList([FromBody] MailListFilter query)
        {
            return _jobService.GetMailList(query);
        }
        [HttpPost]
        public EmailTemplate GetMailTemplate([FromUri] long id)
        {
            return _jobService.GetMailTemplate(id);
        }
        [HttpPost]
        public bool EditMailTemplate([FromBody] MailListFilter query)
        {
            var editEmail = _jobService.EditMailTemplate(query.Id, query.isActive);
            if (editEmail)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [HttpPost]
        public bool SaveJobEstimateImages([FromBody] JobEstimateCategoryViewModel query)
        {
            if (query.UserId == default(long?))
            {
                query.UserId = _sessionContext.UserSession.UserId;
            }
            var editEmail = _jobService.SaveBeforeAfterImages(query);
            if (editEmail)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Media Uploaded Successfully!");
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public IEnumerable<BeforeAfterImageModel> UploadMediaBeforeAfter([FromBody] FileUploadModel model)
        {
            if ((model.JobId == null || model.FileList == null) && model.EstimateId <= 0)
                return null;
            var result = _jobService.SaveJobEstimateMediaFiles(model);
            if (result.Count() > 0)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Media Uploaded!");
            }
            return result;
        }

        [HttpPost]
        public bool BeforeAfterImageSendMail([FromBody] BeforeAfterImageSendMailViewModel model, [FromUri] long? id)
        {
            long? franchiseeId = default(long?);
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
            }
            else
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            var result = _jobService.BeforeAfterImageMailSend(id, model, "addin-alarm-history-list.excel.cshtml", franchiseeId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Mail Sent Successfully!");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Error in Sending Mail!");
                return false;
            }
        }
        [HttpPost]
        public bool InvoiceSendMail([FromBody] JobEstimateServiceViewModel model, [FromUri] long? id)
        {
            long? franchiseeId = default(long?);
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            else
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId;
            }
            var result = _jobService.InvoiceMailSend(id, model, "invoice-print-customer.cshtml", franchiseeId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Mail Send Successfully!");
                return true;
            }
            else
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Error in Sending Mail!");
                return false;
            }
        }

        [HttpPost]
        public bool IsEligibleForDeletion([FromBody] BeforeAfterImageDeletionViewModel model)
        {
            model.RoleId = _sessionContext.UserSession.RoleId;
            model.LoggedInUserId = _sessionContext.UserSession.UserId;
            var result = _jobService.IsEligibleForDeletion(model);
            return result;
        }


        [HttpPost]
        public JobListModel GetHolidayListMonthWise([FromBody] FranchiseeHolidayModel model)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _jobService.GetHolidayListMonthWise(model);
        }


        [HttpGet]
        public FranchiseInfoModel GetFranchiseeInfo([FromUri] long? franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _jobService.GetFranchiseeInfo(franchiseeId);
        }

        [HttpPost]
        public DragDropSchedulerEnum SaveDragDropEvent([FromBody] DragDropSchedulerModel model)
        {
            // return null;
            return _jobService.SaveDragDropEvent(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public ConfirmationResponseModel ConfirmSchedule([FromBody] ConfirmationModel model)
        {
            // return null;
            return _jobService.ConfirmSchedule(model);
        }
        [HttpPost]
        public ConfirmationResponseModel ConfirmScheduleFromUI([FromBody] ConfirmationModel model)
        {
            // return null;
            return _jobService.ConfirmScheduleFromUI(model);
        }


        [HttpPost]
        public bool EditJobNotes([FromBody] JobNoteEditModel model)
        {
            _jobService.EditJobNotes(model);
            if (model.IsJob)
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Job Edit successfully!");
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Estimate Edit successfully!");
            return true;
        }
        [HttpPost]
        public bool DeleteJobNotes([FromUri] long Id)
        {
            _jobService.DeleteNotes(Id);
            return true;
        }


        [HttpGet]
        public CustomerInfoModel GetCustomerInfo([FromUri] string customerName)
        {
            return _jobService.GetCustomerInfo(customerName);
        }
        [HttpPost]
        //public BeforeAfterImageListModel GetBeforeAfterImages([FromBody] BeforeAfterImageFilter filter)
        //{
        //    return _jobService.GetBeforeAfterImages(filter);
        //}
        public ReviewMarketingImageViewModel GetBeforeAfterImages([FromBody] LocalMarketingReviewFilter filter)
        {
            filter.LoggedUserId = _sessionContext.UserSession.UserId;
            filter.RoleId = _sessionContext.UserSession.RoleId;
            filter.LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            return _jobService.GetBeforeAfterImages(filter);
        }

        [HttpPost]
        public bool SaveImagesBestPair([FromBody] SaveImagesBestPairFilter filter)
        {
            return _jobService.SaveImagesBestPair(filter);
        }

        [HttpPost]
        public bool SaveReviewMarkImage([FromBody] SaveReviewImageFilter filter)
        {
            return _jobService.SaveReviewMarkImage(filter);
        }


        [HttpPost]
        public BeforeAfterForFranchieeAdminGroupedViewModel GetBeforeAfterImagesForFranchiseeAdmin([FromBody] BeforeAfterImageFilter filter)
        {
            filter.LoggedUserId= _sessionContext.UserSession.UserId;
            filter.RoleId = _sessionContext.UserSession.RoleId;
            filter.LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            return _jobService.GetBeforeAfterImagesForFranchiseeAdminV2(filter);
        }

        [HttpPost]
        public bool SaveBeforeAfterImagesForFranchiseeAdmin([FromBody] SaveBeforeAfterImageFilter filter)
        {
            return _jobService.SaveBeforeAfterImagesForFranchiseeAdmin(filter);
        }
        [HttpPost]
        public bool SaveEstimateWorth([FromBody] EstimateWorthModel filter)
        {
            return _jobService.SaveEstimateWorth(filter);
        }
        [HttpPost]
        public bool ShiftImagesToInvoiceBuildMaterial([FromBody] ShiftJobEstimateViewModel filter)
        {
            return _jobService.ShiftImagesToInvoiceBuildMaterial(filter);
        }
        [HttpPost]
        public bool EditEmailTemplate([FromBody] MailTemplateEditModel filter)
        {
            return _jobService.EditEmailTemplate(filter);
        }

        [HttpPost]
        public bool SaveInvoiceRequired([FromBody] InvoiceRequiredViewModel filter)
        {
            return _jobService.SaveInvoiceRequired(filter);
        }


        [HttpPost]
        public FileUploadModel GetInvoiceMediaList([FromBody] MediaModel model)
        {
            var userId = _sessionContext.UserSession.OrganizationRoleUserId;
            return _jobService.GetInvoiceMediaList(model.RowId, model.MediaType, model.EstimateId, userId);
        }


        [HttpPost]
        public ScheduleAvailabilityFilterViewModel CheckAvailabilityList(ScheduleAvailabilityFilterList model)
        {
            return _jobService.CheckAvailabilityList(model, false);
        }

        [HttpPost]
        public IEnumerable<InvoiceLineImageModel> InvoiceLineUpload([FromBody] FileUploadModel model)
        {
            if ((model.ServiceId == null || model.FileList == null) && model.EstimateId <= 0)
                return null;
            var result = _jobService.SaveInvoiceLineMediaFiles(model);
            if (result.Count() > 0)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Media Uploaded!");
            }
            return result;
        }

        [HttpPost]
        public bool SaveRotation([FromBody] RotationImageModel model)
        {
            var result = _jobService.SaveImageRotation(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Media Rotated Successfully!");
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public bool SaveCroppedImage([FromBody] CroppedImageModel model)
        {
            var userId = _sessionContext.UserSession.OrganizationRoleUserId;
            var result = _jobService.SaveCroppedImage(model, userId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Media Saved Successfully!");
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public LocalMarketingReviewModel GetLocalMarketingReview([FromBody] LocalMarketingReviewFilter filter)
        {
            filter.LoggedUserId = _sessionContext.UserSession.UserId;
            filter.RoleId = _sessionContext.UserSession.RoleId;
            filter.LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            return _jobService.GetLocalMarketingReview(filter);
        }

        [HttpGet]
        public List<SalesRepTechnicianModel> GetSalesRepTechnicianList([FromUri] long? franchiseeId)
        {
            var LoggedUserId = _sessionContext.UserSession.UserId;
            var RoleId = _sessionContext.UserSession.RoleId;
            var LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            return _jobService.GetSalesRepTechnician(franchiseeId, LoggedUserId, RoleId, LoggedInFranchiseeId);
        }

        [HttpPost]
        public bool MarkImageAsReviwed([FromBody] BeforeAfterImagesLocalMarketingModel model)
        {
            var result = _jobService.MarkImageAsReviwed(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Image Pair Marked As Reviewed Successfully!");
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public bool MarkImageAsBestPair([FromBody] BeforeAfterImagesLocalMarketingModel model)
        {
            var LoggedUserId = _sessionContext.UserSession.UserId;
            var RoleId = _sessionContext.UserSession.RoleId;
            var LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            var result = _jobService.MarkImageAsBestPair(model, LoggedUserId, RoleId, LoggedInFranchiseeId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Image Pair Marked As Best Pair Successfully!");
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public bool MarkImageAsAddToLocalGallery([FromBody] BeforeAfterImagesLocalMarketingModel model)
        {
            var result = _jobService.MarkImageAsAddToLocalGallery(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Image Pair Marked As Add To Local Site Gallery Successfully!");
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public bool BestPairMarkedForJobEstimateImagePair([FromBody] JobEstimateImagePairMarkedModel model)
        {
            var LoggedUserId = _sessionContext.UserSession.UserId;
            var RoleId = _sessionContext.UserSession.RoleId;
            var LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            var result = _jobService.BestPairMarkedForJobEstimateImagePair(model, LoggedUserId, RoleId, LoggedInFranchiseeId);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Image Pair Marked As Best Pair Successfully!");
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}