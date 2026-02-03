using Api.Areas.Application.Controller;
using Api.Enum;
using Api.Impl.Exceptions;
using Core.Application;
using Core.Application.Domain;
using Core.Application.Impl;
using Core.Application.ViewModel;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace API.Attribute
{
    public class CustomDataBinderAttribute : ActionFilterAttribute
    {
        private long? _loggedInUserId;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            _loggedInUserId = GetCurrentLoggedInUserId();

            var controller = SetController(actionContext);
            SetMethodType(actionContext, controller);

            if (controller.MethodType == HttpMethodType.Get || controller.MethodType == HttpMethodType.Delete) return;

            if (actionContext.ActionArguments == null) return;

            controller.SetActionArguments(actionContext.ActionArguments);

            if (!actionContext.ModelState.IsValid)
            {
                controller.SetValidationResult(actionContext.ModelState);
                throw new ValidationFailureException();
            }

            foreach (var argument in actionContext.ActionArguments)
            {
                FillActionAttribute(argument.Value);
            }
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = SetController(actionExecutedContext.ActionContext);

            if (actionExecutedContext.Exception == null)
            {
                var objectContent = actionExecutedContext.Response.Content as ObjectContent;
                if (objectContent != null)
                {
                    var data = controller.SetData(objectContent.Value);

                    if (!data.IsFeedbackSet)
                        data.SetSuccessMessage("Request succeeded successfully.");

                    actionExecutedContext.Response.Content = new ObjectContent(controller.ResponseModel.GetType(), data,
                        actionExecutedContext.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
                }
            }
        }

        private void FillActionAttribute(object actionArgument)
        {
            if (actionArgument == null) return;

            if (actionArgument is EditModelBase)
            {
                SetDatarecorderMetaData(actionArgument as EditModelBase);
            }

            foreach (var item in actionArgument.GetType().GetProperties().Where(x => !x.PropertyType.IsValueType && !x.PropertyType.IsPrimitive && !x.PropertyType.FullName.StartsWith("System.")).ToArray())
            {
                FillActionAttribute(item.GetValue(actionArgument));
            }
        }

        private void SetDatarecorderMetaData(EditModelBase model)
        {
            if (model.DataRecorderMetaData == null || model.DataRecorderMetaData.Id < 1)
                model.DataRecorderMetaData = new DataRecorderMetaData
                {
                    CreatedBy = _loggedInUserId,
                    DateCreated = ApplicationManager.DependencyInjection.Resolve<Clock>().UtcNow
                };
            else
            {
                model.DataRecorderMetaData.DateModified = ApplicationManager.DependencyInjection.Resolve<Clock>().UtcNow;
                model.DataRecorderMetaData.ModifiedBy = _loggedInUserId;
            }
        }

        private long? GetCurrentLoggedInUserId()
        {
            var sessionContext = ApplicationManager.DependencyInjection.Resolve<ISessionContext>();
            if (sessionContext.UserSession == null) return null;
            return sessionContext.UserSession.OrganizationRoleUserId;
        }

        private void SetMethodType(HttpActionContext actionContext, BaseController controller)
        {
            var method = actionContext.Request.Method.Method.ToLower();

            switch (method)
            {
                case "get":
                    controller.SetResponseModel(HttpMethodType.Get);
                    break;
                case "delete":
                    controller.SetResponseModel(HttpMethodType.Delete);
                    break;
                case "post":
                    controller.SetPostResponseModel(HttpMethodType.Post);
                    break;
                case "put":
                    controller.SetPostResponseModel(HttpMethodType.Put);
                    break;
            }
        }

        private BaseController SetController(HttpActionContext actionContext)
        {
            var controller = (actionContext.ControllerContext.Controller as BaseController);
            if (controller == null) throw new InvalidOperationException("Invalid controller");
            return controller;
        }

    }
}