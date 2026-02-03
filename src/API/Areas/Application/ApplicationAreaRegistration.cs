using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace Api.Areas.Application
{
    public class ApplicationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Application";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute(
                "Application_default",
                "Application/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
            );
            context.Routes.MapHttpRoute(
         "Application_default_DownloadZipFile",
         "Application/File/DownloadZipFile/{fileName}",
       new { Controller = "File", Action = "DownloadZipFile" },
       new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
     );
        }
    }
}