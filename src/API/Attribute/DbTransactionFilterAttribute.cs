using Api.Areas.Application.Controller;
using Core.Application;
using System.Web.Http.Filters;

namespace API.Attribute
{
    public class DbTransactionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null) return;

            var controller = (actionExecutedContext.ActionContext.ControllerContext.Controller as BaseController);

            ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>().SaveChanges();
        }
    }
}