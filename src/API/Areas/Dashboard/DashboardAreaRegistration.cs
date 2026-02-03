using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.Dashboard
{
    public class DashboardAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Dashboard";
            }
        }
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute(
               "chart_class",
               "dashboard/revenue/{franchiseeId}",
            new { Controller = "Dashboard", Action = "GetRevenue" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
                           "chart_service",
                           "dashboard/service/revenue/{franchiseeId}",
                        new { Controller = "Dashboard", Action = "GetRevenueBasedOnService" },
                        new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                       );
            context.Routes.MapHttpRoute(
                "SalesRep_Leaderboard",
                "dashboard/leaderboard/{franchiseeId}/salesrep",
             new { Controller = "Dashboard", Action = "GetSalesRepLeaderBoard" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
                "sales_Summary",
                "dashboard/franchisee/{franchiseeId}/sales/summary",
             new { Controller = "Dashboard", Action = "GetSalesSummary" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
                "invoice_List",
                "dashboard/franchisee/{franchiseeId}/invoice/list",
             new { Controller = "Dashboard", Action = "GetRecentInvoices" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
                "franchisee_directory",
                "dashboard/franchisee/Directory",
             new { Controller = "Dashboard", Action = "GetFranchiseeDirectoryList" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
                "Franchisee_Leaderboard",
                "dashboard/leaderboard/{franchiseeId}/franchisee",
             new { Controller = "Dashboard", Action = "GetFranchiseeLeaderboard" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute(
                "customer_count",
                "dashboard/customer/count/{franchiseeId}",
             new { Controller = "Dashboard", Action = "GetCustomerCount" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
                "pending_upload",
                "dashboard/franchisee/{franchiseeId}/pending/upload/list",
             new { Controller = "Dashboard", Action = "GetPendingUploadRange" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
                "unpaid_invoice_List",
                "dashboard/franchisee/{franchiseeId}/unpaid/invoice/list",
             new { Controller = "Dashboard", Action = "GetUnpaidInvoices" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
                "annual_upload_response",
                "dashboard/franchisee/{franchiseeId}/annual/upload/response",
             new { Controller = "Dashboard", Action = "GetAnnualUploadResponse" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
                "document_Summary",
                "dashboard/franchisee/{franchiseeId}/documents",
             new { Controller = "Dashboard", Action = "GetDocumentSummary" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
                "document_Pending",
                "dashboard/franchisee/{franchiseeId}/documents/pending",
             new { Controller = "Dashboard", Action = "GetPendingDocument" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
                "franchisee_directory_superAdmin",
                "dashboard/franchisee/Directory/{franchiseeName}/superAdmin",
             new { Controller = "Dashboard", Action = "GetFranchiseeDirectoryListForSuperAdmin" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute(
                "franchisee_directory_redirection",
                "dashboard/franchisee/RedirectionToBulkPhotoUpload",
             new { Controller = "Dashboard", Action = "RedirectionToBulkPhotoUpload" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
        }
    }
}