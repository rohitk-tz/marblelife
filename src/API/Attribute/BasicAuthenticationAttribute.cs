using Api.Impl;
using Api.Impl.Exceptions;
using Core.Application;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace API.Attribute
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (IsAuthenticated(actionContext)) return;

            throw new NotAuthenticatedException();
        }

        protected bool IsAuthenticated(HttpActionContext actionContext)
        {
            if (IsAnonymousRoleAllowed(actionContext))
                return true;

            if (!actionContext.Request.Headers.Contains(SessionHelper.TokenKeyName))
            {
                return false;
            }

            var token = actionContext.Request.Headers.GetValues(SessionHelper.TokenKeyName).First();

            if (string.IsNullOrEmpty(token)) return false;
            if (ApplicationManager.DependencyInjection.Resolve<ISessionContext>().UserSession == null) return false;

            return true;
        }

        private bool IsAnonymousRoleAllowed(HttpActionContext actionContext)
        {
            var attributes =
                   ((ReflectedHttpActionDescriptor)actionContext.ActionDescriptor).MethodInfo.CustomAttributes;

            if (attributes != null && attributes.Any(x => x.AttributeType == typeof(AllowAnonymousAttribute)))
                return true;

            attributes =
                   actionContext.ControllerContext.Controller.GetType().CustomAttributes;

            if (attributes != null && attributes.Any(x => x.AttributeType == typeof(AllowAnonymousAttribute)))
                return true;

            return false;
        }

        //private bool IsSubscriptionExpireAllowed(HttpActionContext actionContext)
        //{
        //    var attributes =
        //           ((ReflectedHttpActionDescriptor)actionContext.ActionDescriptor).MethodInfo.CustomAttributes;

        //    if (attributes != null && attributes.Any(x => x.AttributeType == typeof(NoSubscriptionExpireConstraintAttribute)))
        //        return true;

        //    attributes =
        //           actionContext.ControllerContext.Controller.GetType().CustomAttributes;

        //    if (attributes != null && attributes.Any(x => x.AttributeType == typeof(NoSubscriptionExpireConstraintAttribute)))
        //        return true;

        //    return false;
        //}
    }
}