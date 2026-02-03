using Api.Impl;
using API.DependencyInjection;
using Core.Application;
using DependencyInjection;
using FluentValidation.WebApi;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace API
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ApiDependencyRegistrar.RegisterDepndencies();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalConfiguration.Configuration.Filters);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;

            GlobalConfiguration.Configuration.Services.Add(typeof(System.Web.Http.Validation.ModelValidatorProvider), new FluentValidationModelValidatorProvider(new ValidatorFactory()));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        protected void Application_BeginRequest()
        {
            HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");
            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                HttpContext.Current.Response.AddHeader("Cache-Control", "no-cache");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, DELETE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, token, timezoneoffset");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }

            SetupSessionContext(HttpContext.Current.Request);
            SetLocalizationBasedRegistration();

            //LogIncomingRequest();
        }
        protected void SetLocalizationBasedRegistration()
        {

            SessionHelper.SetClientTimeZone(); 
        }


        protected void Application_EndRequest()
        {
            ApplicationManager.DependencyInjection.Resolve<IAppContextStore>().ClearStorage();
        }

        private void SetupSessionContext(HttpRequest request)
        {
            var token = request.Headers.Get(SessionHelper.TokenKeyName);
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            SessionHelper.SetSessionModel(token);
        }
        
        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            ApplicationManager.DependencyInjection.Resolve<ILogService>().Error("Application Exception", exception);
        }

        private void LogIncomingRequest()
        {
            var url = HttpContext.Current.Request.Url;
            var token = HttpContext.Current.Request.Headers.Get(SessionHelper.TokenKeyName);
            var userAgent = HttpContext.Current.Request.UserAgent;

            string message = string.Format("Url: {0}, Token: {1}, UserAgent: {2}", url, token, userAgent);
            ApplicationManager.DependencyInjection.Resolve<ILogService>().Info(message);
        }
    }
}
