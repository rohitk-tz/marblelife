using Api.Areas.Application.Controller;
using Api.Impl;
using Api.Impl.Exceptions;
using Core.Application;
using Core.Application.Exceptions;
using Core.Application.ViewModel;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace API.Attribute
{
    public class BasicExceptionFilterAtribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var controller = (actionExecutedContext.ActionContext.ControllerContext.Controller as BaseController);

            if (actionExecutedContext.Exception != null)
            {
                LogError(actionExecutedContext.Exception);

                var errorCode = SetResponseErrorCodeType(actionExecutedContext.Exception);

                controller.ResponseModel.SetErrorMessage(actionExecutedContext.Exception.Message, errorCode);

                var objectContent = new ObjectContent(controller.ResponseModel.GetType(), controller.ResponseModel,
                    actionExecutedContext.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

                if (actionExecutedContext.Response == null)
                    actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

                actionExecutedContext.Response.Content = objectContent;
            }
        }

        private static void LogError(Exception exception)
        {
            if (exception is CustomBaseException)
            {
                var token = HttpContext.Current.Request.Headers.Get(SessionHelper.TokenKeyName);
                var exceptionMsg = string.Format("Url: {0} Token: {1} \n Message: {2}", HttpContext.Current.Request.Url, token, exception.Message);
                ApplicationManager.DependencyInjection.Resolve<ILogService>().Error(exceptionMsg);
            }
            else
            {
                ApplicationManager.DependencyInjection.Resolve<ILogService>().Error(exception);
            }

        }
        private static ResponseErrorCodeType SetResponseErrorCodeType(Exception exception)
        {
            var errorCode = ResponseErrorCodeType.RandomException;

            if (exception is ValidationFailureException)
            {
                errorCode = ResponseErrorCodeType.ValidationFailure;
            }
            else if (exception is NotAuthenticatedException)
            {
                errorCode = ResponseErrorCodeType.UserNotAuthenticated;
            }
            else if (exception is UserBlockedException)
            {
                errorCode = ResponseErrorCodeType.UserBlocked;
            }
            else if (exception is InvalidDataProvidedException)
            {
                errorCode = ResponseErrorCodeType.InvalidData;
            }
            return errorCode;
        }
    }
}