using Api.Areas.Application.Controller;
using Core.Application;
using Core.Billing;
using Core.Organizations;
using Core.Scheduler;
using Core.Scheduler.Domain;
using Core.ToDo;
using Core.ToDo.ViewModel;
using Core.Users;
using System.Collections.Generic;
using System.Web.Http;

namespace API.Areas.Scheduler.Controllers
{
    public class ToDoController : BaseController
    {
        private readonly IToDoService _todoService;
        private readonly ISessionContext _sessionContext;
        public ToDoController(ISessionContext sessionContext, IToDoService todoService)
        {
            _sessionContext = sessionContext;
            _todoService = todoService;
        }

        [HttpPost]
        public ToDoListModel GetToDoList([FromBody] ToDoFilter filter)
        {
            filter.LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            filter.OrgUserId = _sessionContext.UserSession.OrganizationRoleUserId;
            filter.UserId = _sessionContext.UserSession.UserId;
            filter.RoleId = _sessionContext.UserSession.RoleId;
            return _todoService.Get(filter);
        }

        [HttpPost]
        public bool SaveToDoFollowUp([FromBody] ToDoEditModel editmodel)
        {
            var LoggedInFranchiseeId = _sessionContext.UserSession.OrganizationId;
            editmodel.UserId = _sessionContext.UserSession.UserId;
            return _todoService.SaveToDoFollowUp(editmodel, LoggedInFranchiseeId);
        }

        [HttpGet]
        public ToDoEditModel GetToDoById(long? id)
        {
            return _todoService.GetToDoById(id);
        }

        [HttpGet]
        public IEnumerable<CustomerNameModel> GetCustomerList([FromUri] string text)
        {
            return _todoService.GetCustomerList(text);
        }
        [HttpGet]
        public CustomerNameModel GetCustomerInfo([FromUri] string text)
        {

            return _todoService.GetCustomerInfo(text);
        }

        [HttpGet]
        public CommentListModel GetCommentToDo(long id)
        {
            return _todoService.GetCommentInfo(id);
        }

        [HttpPost]
        public bool SaveCommentInfo([FromBody] CommentModel model)
        {
            return _todoService.SaveCommentInfo(model);
        }
        [HttpGet]
        public CommentListModel GetCommentToDoForScheduler(long id)
        {
            var userId = _sessionContext.UserSession.UserId;
            var roleId = _sessionContext.UserSession.RoleId;
            return _todoService.GetCommentToDoForScheduler(id, userId, roleId);
        }
        [HttpPost]
        public CommentListModel GetToDoListForScheduler([FromBody] UserListModel model)
        {
            model.RoleId = _sessionContext.UserSession.RoleId;
            model.UserId = _sessionContext.UserSession.OrganizationRoleUserId;
            return _todoService.GetCommentToDoForSchedulerScreen(model);
        }
        [HttpPost]
        public bool SaveToDoByStatus([FromBody] ToDoEditModel model)
        {
            return _todoService.SaveToDoByStatus(model);
        }
        [HttpPost]
        public bool DeleteToDo([FromUri] long? id)
        {
            return _todoService.DeleteToDo(id);
        }
        [HttpGet]
        public ToDoEditModel GetToDoInfo([FromUri] string text)
        {
            var userId = _sessionContext.UserSession.UserId;
            return _todoService.GetToDoInfo(text, userId);
        }
        [HttpGet]
        public IEnumerable<ToDoModel> GetToDoList([FromUri] string text)
        {
            var userId = _sessionContext.UserSession.UserId;
            return _todoService.GetToDoList(text,userId);
        }

        [HttpPost]
        public ToDoSchedulerViewModel GetSchedulerListForToDo([FromBody] ToDoSchedulerModel filter)
        {
            return _todoService.GetSchedulerForToDo(filter);
        }
    }
}