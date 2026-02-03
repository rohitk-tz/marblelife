using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.Organizations
{
    public class OrganizationsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Organizations";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute("franchisee_default", "Organizations/franchisee/{id}",
              new { Controller = "Franchisee", Action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute("franchisee_fee_profile", "Organizations/franchisee/{id}/fee/profile",
              new { Controller = "Franchisee", Action = "GetFeeProfile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute(
                  "delete_franchisee",
                  "Organizations/franchisee/{id}/delete/franchisee",
                  new { Controller = "Franchisee", Action = "DeleteFranchisee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
              );
            context.Routes.MapHttpRoute(
                 "franchisee_loginList",
                 "Organizations/franchisee/{userId}/list/franchisee",
                 new { Controller = "Franchisee", Action = "GetFranchiseeListForLogin" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
             );

            context.Routes.MapHttpRoute(
                 "franchisee_InfoList",
                 "Organizations/franchisee/{userId}/franchiseeInfo/collection",
                 new { Controller = "Franchisee", Action = "GetFranchiseeInfoCollection" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
             );

            context.Routes.MapHttpRoute("manage_Franchisee", "Organizations/franchisee/manage/account",
            new { Controller = "Franchisee", Action = "ManageFranchisee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute("franchisee_Notes", "Organizations/franchisee/{franchiseeId}/list/notes",
          new { Controller = "Franchisee", Action = "GetFranchiseeNotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
        );

            context.Routes.MapHttpRoute("deactivate_Franchisee", "Organizations/franchisee/deactivate/franchisee",
              new { Controller = "Franchisee", Action = "DeactivateFranchisee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("activate_Franchisee", "Organizations/franchisee/{franchiseeId}/activate/franchisee",
         new { Controller = "Franchisee", Action = "ActivateFranchisee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
       );

            context.Routes.MapHttpRoute("franchisee_BusinessId", "Organizations/franchisee/{id}/businessId/{businessId}/verify",
              new { Controller = "Franchisee", Action = "IsUniqueBusinessId" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute("franchisee_geoCode", "franchisee/{franchiseeId}/geo/code",
      new { Controller = "Franchisee", Action = "GetGeoCode" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                 );

            context.Routes.MapHttpRoute(
                  "delete_oneTimeProjectFee",
                  "Organizations/franchisee/{id}/delete/{typeId}/fee",
                  new { Controller = "Franchisee", Action = "DeleteFee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
              );

            context.Routes.MapHttpRoute(
                  "get_oneTimeProjectFee",
                  "Organizations/franchisee/{id}/get/otpFee",
                  new { Controller = "Franchisee", Action = "GetOneTimeProjectFee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
              );

            context.Routes.MapHttpRoute(
                 "save_oneTimeProjectFee",
                 "Organizations/franchisee/{franchiseeId}/save/serviceFee",
                 new { Controller = "Franchisee", Action = "SaveServiceFee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );

            context.Routes.MapHttpRoute(
                  "get_loan",
                  "Organizations/franchisee/{id}/get/loan",
                  new { Controller = "Franchisee", Action = "GetLoanList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
              );

            context.Routes.MapHttpRoute("Doc_upload", "Organizations/doc/upload",
               new { Controller = "FranchiseeDocument", Action = "UploadDocuments" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Doc_delete", "Organizations/doc/{id}/delete",
               new { Controller = "FranchiseeDocument", Action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) }
             );

            context.Routes.MapHttpRoute("Doc_info", "Organizations/doc/{id}/get/info",
               new { Controller = "FranchiseeDocument", Action = "GetById" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
             );
            context.Routes.MapHttpRoute("change_Franchisee_LoanAdjustment", "Organizations/save/changeServiceFee",
            new { Controller = "Franchisee", Action = "SaveChangeServiceFee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );

            context.Routes.MapHttpRoute(
                 "get_adfund_royalty",
                 "Organizations/franchisee/{franchiseeId}/royalty/adfund",
                 new { Controller = "Franchisee", Action = "GetFranchiseeRoyality" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute(
                 "franchisee_teamImage",
                 "Organizations/franchisee/{franchiseeId}/team/image",
                 new { Controller = "Franchisee", Action = "GetFranchiseeTeamImage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
             );
            context.Routes.MapHttpRoute(
                 "franchisee_Save_teamImage",
                 "Organizations/franchisee/save/team/image",
                 new { Controller = "Franchisee", Action = "SaveFranchiseeTeamImage" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("loan_export", "Organizations/loan/export/",
             new { Controller = "Franchisee", Action = "DownloadFranchiseeLoan" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("download_organization", "Organizations/download/franchisee/",
             new { Controller = "Franchisee", Action = "DownloadFranchisee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("download_organizationre_directory", "Organizations/download/franchiseDirectory/",
             new { Controller = "Franchisee", Action = "DownloadFranchiseeDirectory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute("franchisee_default_redesign", "Organizations/franchisees/redesign/",
              new { Controller = "Franchisee", Action = "GetFranchiseeList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute("download_organizationre_directory_file", "Organizations/franchisee/downloadFileFranchiseeDirectory/",
             new { Controller = "Franchisee", Action = "DownloadFileFranchiseeDirectory" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("franchisee_Deactivation_Note", "Organizations/franchisee/{franchiseeId}/list/deactivation/notes",
        new { Controller = "Franchisee", Action = "GetFranchiseeDeactivationNote" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
      );
            context.Routes.MapHttpRoute("franchisee_RPID", "Organizations/franchisee/{franchiseeId}/list/rpId",
       new { Controller = "Franchisee", Action = "GetFranchiseeRPID" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
     );
            context.Routes.MapHttpRoute("onetimeProject_Fees", "Organizations/franchisee/change/adfund/royality",
      new { Controller = "Franchisee", Action = "OneTimeProjectChangeStatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
    );
            context.Routes.MapHttpRoute("franchisee_Report", "Organizations/franchisee/document/report",
      new { Controller = "Franchisee", Action = "GetFranchiseeDocumentReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
    );
            context.Routes.MapHttpRoute("franchisee_List", "Organizations/franchisee/list/document",
     new { Controller = "Franchisee", Action = "GetFranchiseeDocumentList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
   );
            context.Routes.MapHttpRoute(
                  "save_prePay_Amount",
                  "Organizations/franchisee/prePay/Loan",
                  new { Controller = "Franchisee", Action = "SavePrePayLoan" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("franchiee_Notes", "Organizations/franchisee/notes",
    new { Controller = "Franchisee", Action = "SaveFranchisseeNotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
  );
            context.Routes.MapHttpRoute("franchisee_Duration_Approval_List", "Organizations/durationApprovalList/{franchiseeId}/list",
        new { Controller = "Franchisee", Action = "GetDurationApprovalList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
      );
            context.Routes.MapHttpRoute("change_duration_status", "Organizations/franchisee/changeStatus",
       new { Controller = "Franchisee", Action = "ChangeDurationStatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
     );
            context.Routes.MapHttpRoute("download_taxReport", "Organizations/franchisee/downloadTaxReport",
       new { Controller = "Franchisee", Action = "DownloadTaxReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
     );
            context.Routes.MapHttpRoute("seochargesstatus_Fees", "Organizations/franchisee/change/adfund/royality/SEOCharges",
      new { Controller = "Franchisee", Action = "SEOChargesChangeStatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
    );
        }
    }
}