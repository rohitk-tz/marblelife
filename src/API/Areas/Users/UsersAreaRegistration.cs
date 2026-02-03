using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace BeautyShoppe.Api.Areas.Users
{
    public class UsersAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Users";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute("Users_Identity", "users/identity/{sessionId}",
                new { Controller = "Login", Action = "Identity" }
            );
            context.Routes.MapHttpRoute("Users_Logout", "user/logout",
                new { Controller = "Login", Action = "Logout" }
            );
            context.Routes.MapHttpRoute(
                "Users_Post",
                "Users",
                new { Controller = "User", Action = "Post" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute(
               "Users_default_controller",
               "Users/{id}",
               new { Controller = "User", id = UrlParameter.Optional }
            );
            context.Routes.MapHttpRoute("Users_Edit", "users/{id}/{franchiseeId}",
               new { Controller = "User", Action = "Get" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute("Users_Email", "users/{id}/email/{email}/verify",
               new { Controller = "User", Action = "IsUniqueEmail" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute("Users_UserName", "users/{id}/userName/{userName}/verify",
              new { Controller = "User", Action = "IsUniqueUserName" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute("Users_Lock", "Users/{id}/Lock",
                new { Controller = "User", Action = "Lock" }
            );
            context.Routes.MapHttpRoute("Users_default", "Users/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
            );
            context.Routes.MapHttpRoute("manage_account", "manage/{userId}/account",
              new { Controller = "User", Action = "ManageAccount" }, 
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Users_getUrl", "Users/User/getImageUrl",
                new { Controller = "User", Action = "ImageUrl" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute( "Users_saveEquipmentRole", "Users/User/postForEquipment",
                new { Controller = "User", Action = "PostForEquipment" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Users_editEquipmentRole", "Users/User/userEdit",
                new { Controller = "User", Action = "UserEdit" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Users_DefaultCalendarView", "Users/User/saveDefaultView/{deafaultView}/save",
               new { Controller = "User", Action = "SchedulerDefaultView" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Users_GetDefaultCalendarView", "Users/User/getDefaultView",
              new { Controller = "User", Action = "GetDefaultView" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Users_Customer_login", "user/login/customer",
                new { Controller = "Login", Action = "CustomerLogin" }
            );
            context.Routes.MapHttpRoute("Users_GetEmailSignatures", "Users/User/getUserSignature",
              new { Controller = "User", Action = "GetUserSignature" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("Users_SaveEmailSignatures", "Users/User/saveUserSignature",
              new { Controller = "User", Action = "SaveUserSignature" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
        }
    }
}