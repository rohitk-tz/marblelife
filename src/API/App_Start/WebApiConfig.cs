using Api.Areas.Application.Controller;
using Core.Application;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Http.Routing;

namespace API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();
            //


            config.Routes.MapHttpRoute("DefaultApiWithId", "{area}/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            config.Routes.MapHttpRoute("DefaultApiWithAction", "{area}/{controller}/{action}");

            config.Routes.MapHttpRoute("DefParamApiGet", "{area}/{controller}/{id}", new { action = "Get", id = RouteParameter.Optional }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.Routes.MapHttpRoute("DefaultApiGet", "{area}/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });

            config.Routes.MapHttpRoute("DefaultApiPost", "{area}/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });

            config.Routes.MapHttpRoute("DefaultApiPut", "{area}/{controller}/{id}", new { action = "Put", id = RouteParameter.Optional }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Put) });
            config.Routes.MapHttpRoute("DefaultApiDelete", "{area}/{controller}/{id}", new { action = "Delete", id = RouteParameter.Optional }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) });

            config.Routes.MapHttpRoute("DefaultApi", "{area}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            config.DependencyResolver = new DependencyResolver();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }

        public class DependencyResolver : IDependencyResolver
        {
            public void Dispose()
            {

            }

            public object GetService(Type serviceType)
            {
                return serviceType.BaseType != typeof(BaseController) && serviceType.BaseType != typeof(ApiController) ? null : ApplicationManager.DependencyInjection.Resolve(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return new List<object>();
            }

            public IDependencyScope BeginScope()
            {
                return this;
            }

        }
    }
}
