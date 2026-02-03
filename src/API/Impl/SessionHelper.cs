using System;
using System.Web;
using Core.Application;
using Core.Sales.Domain;
using Core.Users;
using Core.Users.Domain;
using Core.Users.ViewModels;

namespace Api.Impl
{
    public static class SessionHelper
    {
        public const string TokenKeyName = "Token";
        public const string TimeZoneOffsetKeyName = "Timezoneoffset";
        public const string TimeZoneNameKeyName = "TimeZoneName";

        public static string CreateNewSession(HttpRequestBase request, UserLogin userLogin)
        {
            var sessionId = Guid.NewGuid().ToString();
            string userAgent = request.UserAgent;
            string os = GetOS(userAgent);

            ApplicationManager.DependencyInjection.Resolve<IUserLogService>().SaveLoginSession(userLogin.Id, sessionId, request.UserHostAddress, DateTime.UtcNow,
                request.Browser.Browser + " " + request.Browser.Version, os, userAgent);
            ApplicationManager.DependencyInjection.Resolve<ISessionFactory>().BuildSession(ApplicationManager.DependencyInjection.Resolve<ISessionContext>(), userLogin);
            return sessionId;
        }

        public static UserSessionModel SetSessionModel(string token)
        {
            var model = ApplicationManager.DependencyInjection.Resolve<ISessionFactory>().GetActiveSessionModel(token);
            if (model == null)
            {
                return null;
            }

            var sessionContext = ApplicationManager.DependencyInjection.Resolve<ISessionContext>();
            sessionContext.Token = token;
            sessionContext.UserSession = model;

            return model;
        }

        public static void DestroySession(string token)
        {
            ApplicationManager.DependencyInjection.Resolve<IUserLogService>().EndLoggedinSession(token);
        }

        public static void SetClientTimeZone()
        {
            var clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            var clientoffset = HttpContext.Current.Request.Headers.Get(TimeZoneOffsetKeyName);
            var name = HttpContext.Current.Request.Headers.Get(TimeZoneNameKeyName);
            clock.BrowserTimeZone = clientoffset;
            //if (!string.IsNullOrEmpty(name))
            //    clock.SetTimeZoneInfo(name);
            //else
                clock.SetTimeZoneInfo(string.IsNullOrEmpty(clientoffset) ? 0 : Convert.ToInt32(clientoffset));
        }

        public static string CreateNewSessionForCustomer(HttpRequestBase request, long customerId, long estimateCustomerId, long estimateInvoiceId,long? typeId,string code)
        {
            var sessionId = Guid.NewGuid().ToString();
            ApplicationManager.DependencyInjection.Resolve<IUserLogService>().SaveLoginCustomerSession(customerId, estimateCustomerId, estimateInvoiceId, sessionId, request.UserHostAddress, DateTime.UtcNow,
                typeId, code, request.Browser.Browser + " " + request.Browser.Version);
            return sessionId;
        }

        private static string GetOS(string ua)
        {
            if (ua.Contains("Windows NT 10.0")) return "Windows 10";
            if (ua.Contains("Windows NT 6.1")) return "Windows 7";
            if (ua.Contains("Mac OS X")) return "macOS";
            if (ua.Contains("Android")) return "Android";
            if (ua.Contains("iPhone")) return "iOS";
            return "Unknown";
        }
    }
}