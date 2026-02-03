using Api.Areas.Application.Controller;
using Core.Application;
using Core.Application.ViewModel;
using Core.Organizations;
using Core.Scheduler;
using Core.Scheduler.ViewModel;
using System.Web.Http;

namespace API.Areas.Scheduler.Controllers
{
    public class JobController : BaseController
    {
        private readonly IJobInfoService _jobInfoService;
        private readonly ISessionContext _sessionContext;
        private readonly IOrganizationRoleUserInfoService _orgRoleUserInfoService;
        private readonly IEstimateService _estimateService;
        public JobController(ISessionContext sessionContext, IJobInfoService jobInfoService, IOrganizationRoleUserInfoService orgRoleUserInfoService,
            IEstimateService estimateService)
        {
            _sessionContext = sessionContext;
            _jobInfoService = jobInfoService;
            _orgRoleUserInfoService = orgRoleUserInfoService;
            _estimateService = estimateService;
        }

        [HttpGet]
        public JobInfoEditModel Get(long id)
        {
            return _jobInfoService.Get(id);
        }

        [HttpPut]
        public bool Put(long id, [FromBody]JobInfoEditModel model)
        {
            var result = _jobInfoService.UpdateJobTime(model);
            if (result)
            {
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Start/End Time has been updated successfully.");
            }
            else
                PostResponseModel.Message = FeedbackMessageModel.CreateSuccessMessage("Unable to Update Start/End Time");
            return result;
        }

        [HttpDelete]
        public bool Delete(long id)
        {
            var result = _jobInfoService.Delete(id);
            if (result == true)
            {
                ResponseModel.SetSuccessMessage("Job deleted successfully.");
                return true;
            }
            else
                ResponseModel.SetErrorMessage("Can't delete Job!");
            return false;
        }
    }
}