using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Notification.Enum;
using Core.Organizations;
using Core.Scheduler;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Users.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace API.Areas.Scheduler.Controllers
{
    public class EstimateController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEstimateService _estimateService;
        private readonly ISessionContext _sessionContext;
        private readonly IJobService _jobService;
        public readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;


        public EstimateController(ISessionContext sessionContext, IEstimateService estimateService, IJobService jobService,
            IOrganizationRoleUserInfoService organizationRoleUserInfoService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sessionContext = sessionContext;
            _estimateService = estimateService;
            _jobService = jobService;
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
        }

        [HttpGet]
        public JobEstimateEditModel GetEstimate([FromUri]long id)
        {
            return _estimateService.Get(id);
        }

        [HttpPost]
        public bool SaveEstimate([FromBody]JobEstimateEditModel model)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (model.SalesRepId == null)
            {
                model.SalesRepId = _jobService.SetDefaultAssignee(model.Id, model.FranchiseeId, model.StartDate, model.EndDate, false);
                if (model.SalesRepId <= 0)
                {
                    PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Sales Rep is not available!!");
                    return false;
                }
            }
            var result = _jobService.CheckAvailabilityForJob(model.Id, model.SalesRepId.Value, model.StartDate, model.EndDate, false);
            if (!result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Sales Rep is not available!!");
                return false;
            }
            _estimateService.Save(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Estimate created/Updated successfully!");
            return true;
        }

        [HttpDelete]
        public bool Delete([FromUri]long id)
        {
            var result = _estimateService.Delete(id);
            if (result == true)
            {
                ResponseModel.SetSuccessMessage("Estimate deleted successfully.");
                return true;
            }
            else
                ResponseModel.SetErrorMessage("Can't delete Estimate!");
            return false;
        }

        [HttpGet]
        public JobEstimateEditModel GetVacationInfo(long id)
        {
            return _estimateService.GetVacationInfo(id);
        }
        [HttpGet]
        public MeetingEditModel GetMeetingInfo(long id)
        {
            return _estimateService.GetMessageInfo(id);
        }
        [HttpPost]
        public bool SaveVacation([FromBody]JobEstimateEditModel model)
        {
            model.LogginUserId = _sessionContext.UserSession.UserId;
            var oldStartDate = _jobSchedulerRepository.Table.Where(x => x.Id == model.Id).Select(x => x.StartDate).FirstOrDefault();
            var oldEndDate = _jobSchedulerRepository.Table.Where(x => x.Id == model.Id).Select(x => x.EndDate).FirstOrDefault();
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (model.AssigneeId <= 0)
                return false;
            _estimateService.SaveVacation(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Vacation created/updated successfully!");
            return true;
        }

        [HttpPost]
        public bool SaveMeeting([FromBody]JobEstimateEditModel model)
        {
            var isNew = model.SchedulerId == 0 ? true : false;
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (model.idList.Count <= 0)
                return false;

            model.LogginUserId= _sessionContext.UserSession.UserId;

            if (model.Id <= 0)
            {
                foreach (var assigneeId in model.idList)
                {
                    var result = _jobService.CheckAvailabilityForMeeting(model.Id, assigneeId, model.StartDate, model.EndDate, model);
                    if (!result)
                    {
                        PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("This user is not available on this day.Please select another user or contact admin to reschedule his current Meeting!");
                        return false;
                    }
                }
                
                model.MeetingID = _estimateService.SaveMeeting(model);
                model.IsUpdate = false;
                
                foreach (var assigneeId in model.idList)
                {
                    model.AssigneeId = assigneeId;
                    model.PersonId = assigneeId;
                    model.SchedulerId= _estimateService.SaveMeetingForUser(model);
                    model.IsVacation = false;
                    if (isNew && !model.IsEquipment.Value)
                    {
                        _estimateService.SendMailToMember(model, NotificationTypes.MeetingMailForMemebers);
                    }
                }

            }
            else
            {
                foreach (var assigneeId in model.idList)
                {
                    var result = _jobService.CheckAvailabilityForMeeting(model.Id, assigneeId, model.StartDate, model.EndDate, model);
                    if (!result)
                    {
                        PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("This user is not available on this day.Please select another user or contact admin to reschedule his current Meeting!");
                        return false;
                    }
                }
                _estimateService.EditMeetingForEquipment(model);
                _estimateService.EditMeeting(model);
                return true;
            }
            

            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Meeting created/updated successfully!");
            return true;
        }


        private bool EditMeetingWithNewValue(JobEstimateEditModel model)
        {
            var meetingUserId = _jobSchedulerRepository.Table.Where(x => x.Id == model.MeetingID).Select(x => x.MeetingID).FirstOrDefault();
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (model.idList.Count <= 0)
                return false;

            //var user = _organizationRoleUserInfoService.GetOrganizationRoleUserbyId(model.AssigneeId);
            //if (user == null)
            //    return false;
            if (model.Id <= 0)
            {
                model.MeetingID = _estimateService.SaveMeeting(model);
                model.IsUpdate = false;
            }
            else
            {
                List<long> oldList = _jobSchedulerRepository.Table.Where(x => x.MeetingID == model.MeetingID).Select(x => x.OrganizationRoleUser.UserId).ToList();
                model.idList = model.idList.Except(oldList).ToList();
                IList<long> deletedUserId = oldList.Except(model.idList).ToList();
                model.IsUpdate = false;
            }
            foreach (var AssigneeId in model.idList)
            {
                var userIds = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(AssigneeId, model.FranchiseeId);

                foreach (var id in userIds)
                {
                    var result = _jobService.CheckAvailability(model.Id, id, model.StartDate, model.EndDate, true);
                    if (!result)
                    {
                        PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("This user is not available on this day.Please select another user or contact admin to reschedule his current Meeting!");
                        return false;
                    }
                }

                foreach (var orgRoleUserId in userIds)
                {
                    model.AssigneeId = orgRoleUserId;
                    //_estimateService.SaveMeetingForUser(model);
                }
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Meeting created/updated successfully!");

            }
            return true;
        }
        [HttpDelete]

        public bool DeleteVacation([FromUri]long id)
        {
            var result = _estimateService.DeleteVacation(id);
            if (result == true)
            {
                ResponseModel.SetSuccessMessage("Personal deleted successfully.");
                return true;
            }
            else
                ResponseModel.SetErrorMessage("Can't delete Personal!");
            return false;
        }

        [HttpGet]
        public JobOccurenceListModel GetOccurenceInfo(long id)
        {
            return _estimateService.GetOccurenceInfo(id);
        }
        [HttpGet]
        public JobOccurenceListModel GetEstimateOccurenceInfo(long id)
        {
            return _estimateService.GetEstimateOccurenceInfo(id);
        }
        [HttpPost]
        public bool SaveSchedule([FromBody]JobOccurenceListModel model)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            //if (model.AssigneeId <= 0)
            //    return false;
            var result = _estimateService.CheckDuplicateAssignment(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("This user is already schedule on this time.Please select another user or contact admin to reschedule his current schedule!");
                return false;
            }
            _estimateService.SaveSchedule(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Schedule created successfully!");
            return true;
        }
        [HttpPost]
        public bool RepeatEstimate([FromBody]JobOccurenceListModel model)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            //if (model.AssigneeId <= 0)
            //    return false;
            var result = _estimateService.CheckDuplicateAssignment(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("This user is already schedule on this time.Please select another user or contact admin to reschedule his current schedule!");
                return false;
            }
            var isCurrentDeleted = _estimateService.CheckCurrentEstimateDeletion(model);
            if (isCurrentDeleted)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("Current Estimate Cant be Deleted!");
                return false;
            }
            _estimateService.SaveEstimateSchedule(model);
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Schedule created successfully!");
            return true;
        }
        [HttpPost]
        public bool RepeatVacation([FromBody]VacationRepeatEditModel model)
        {

            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (model.AssigneeId <= 0)
                return false;

            var userIds = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(model.AssigneeId, model.FranchiseeId);

            //foreach (var id in userIds)
            //{
            //    var result = _jobService.CheckAvailability(model.Id, id, model.StartDate, model.EndDate, true);
            //    if (!result)
            //    {
            //        PostResponseModel.Message = FeedbackMessageModel.CreateErrorMessage("This user has a schedule on this day.Please select another user or contact admin to reschedule his current schedule!");
            //        return false;
            //    }
            //}

            //foreach (var orgRoleUserId in userIds)
            //{
            model.AssigneeId = model.AssigneeId;
            model.PersonId = model.AssigneeId;
            _estimateService.RepeatVacation(model);
            //}
            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Vacation created/updated successfully!");
            return true;
        }

        [HttpPost]
        public bool RepeatMeeting([FromBody]VacationRepeatEditModel model)
        {
            ICollection<long> userids = null;
            //long meetingId = _estimateService.SaveMeeting(model);
            //model.MeetingId = meetingId != 0 ? meetingId : default(long?);

            var meetingInfo = _jobSchedulerRepository.Table.Where(x => x.Id == model.VacationId).FirstOrDefault();
            var meeting = default(long);
            if (meetingInfo.MeetingID != default(long?) && meetingInfo.MeetingID != default(long))
            {
                meeting = _estimateService.GetParentMeetingId(meetingInfo.MeetingID.Value);
            }
            if (meeting != 0)
            {
                model.ParentId = meeting;
            }
            long newMeetingId = _estimateService.SaveMeeting(model);
            model.MeetingId = newMeetingId;
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                model.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            //if (model.AssigneeId <= 0)
            //    return false;
            if (meetingInfo.MeetingID != 0)
            {
                List<long> userId = _estimateService.GetUserIdsByMeeting(meetingInfo.MeetingID.GetValueOrDefault());
                userId = userId.Distinct().ToList();
                foreach (var orgRoleUserId in userId)
                {
                    model.AssigneeId = orgRoleUserId;
                    _estimateService.RepeatMeeting(model);
                }
            }
            else
            {
                userids = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(model.AssigneeId, model.FranchiseeId);
                foreach (var orgRoleUserId in userids)
                {
                    model.AssigneeId = orgRoleUserId;
                    _estimateService.RepeatMeeting(model);
                }
            }

            PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Meeting created/updated successfully!");
            return true;
        }

        [HttpPost]
        public bool DeleteMeeting([FromBody]MeetingDeleteViewModel model)
        {
            var result = _estimateService.DeleteMeeting(model.Id.GetValueOrDefault(), model.TechId.GetValueOrDefault());
            if (result == true)
            {
                ResponseModel.SetSuccessMessage("Meeting deleted successfully.");
                return true;
            }
            else
                ResponseModel.SetErrorMessage("Can't delete Meeting!");
            return false;
        }
    }
}