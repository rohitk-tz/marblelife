using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.Reports
{
    public class ReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Reports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Reports_default",
                "Reports/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }
            );

            context.Routes.MapHttpRoute(
                "salesReport_download",
                "reports/download",
              new { Controller = "ServiceReport", Action = "DownloadSalesReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
                "lateFeeReport_download",
                "lateFee/report/download",
              new { Controller = "LateFeeReport", Action = "DownloadLateFeeReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
                "customer_email_chart", "reports/customerEmailReport/email/{franchiseeId}/report",
             new { Controller = "CustomerEmailReport", Action = "GetChartData" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
                "customer_review_chart", "reports/customerReviewReport/review/{franchiseeId}/report",
             new { Controller = "CustomerEmailReport", Action = "GetChartDataForReviews" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute(
                "emailReport_download",
                "customerEmail/report/download",
              new { Controller = "CustomerEmailReport", Action = "DownloadEmailReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
                "customer_review_count",
                "customerEmail/report/getReviewCounts/{franchiseeId}",
              new { Controller = "CustomerEmailReport", Action = "GetReviewCounts" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute("feedback_report", "reports/customerFeedbackReport/feedback/get/{responseId}",
             new { Controller = "CustomerFeedbackReport", Action = "GetFeedbackDetail" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute("feedback_status", "reports/customerFeedbackReport/manageCustomerFeedbackStatus/{isAccept}/action/{customerId}/action/{id}/action/{fromTable}",
             new { Controller = "CustomerFeedbackReport", Action = "ManageCustomerFeedbackStatus" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
                "feedbackReport_download",
                "customerFeedbackReport/download",
              new { Controller = "CustomerFeedbackReport", Action = "DownloadFeedbackReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
                "growthReport_download",
                "growth/report/download",
              new { Controller = "GrowthReport", Action = "DownloadGrowthReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
                "uploadReport_download",
                "upload/report/download",
              new { Controller = "UploadBatchReport", Action = "DownloadUploadReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
                "product_report_download",
                "prodcut/report/download",
              new { Controller = "ProductReport", Action = "DownloadReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
               "product_report_chart", "product/channel/report",
            new { Controller = "ProductReport", Action = "GetChartData" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute(
               "franchisee_mail_report",
               "franchisee/mail",
            new { Controller = "CustomerEmailReport", Action = "GetFranchiseeWiseMails" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute(
                "arReport",
                "ar/report",
              new { Controller = "ARReport", Action = "Get" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute(
               "mlfsReport",
               "reports/mlfsReport",
             new { Controller = "MLFSReport", Action = "GetReportForPurchase" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            
            context.Routes.MapHttpRoute(
               "mlfsReport_Sales",
               "reports/mlfsReportsales",
             new { Controller = "MLFSReport", Action = "GetReportForSales" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute(
              "mlfsReport_Configuration",
              "reports/mlfsReportConfiguration",
            new { Controller = "MLFSReport", Action = "GetMLFSConfigurationData" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute(
              "mlfsReport_Configuration_Save",
              "reports/mlfsReportSave",
            new { Controller = "MLFSReport", Action = "SaveMLFSConfigurationData" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute("Get_Price_Estimate_Collection", "priceEstimate/getPriceEstimateCollection",
                new { Controller = "PriceEstimate", Action = "GetPriceEstimateCollection" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Get_Price_Estimate", "priceEstimate/getPriceEstimate",
                new { Controller = "PriceEstimate", Action = "GetPriceEstimate" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Save_Price_Estimate", "priceEstimate/saveBulkCorporatePrice",
                new { Controller = "PriceEstimate", Action = "SaveBulkCorporatePriceEstimate" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Bulk_CorporatePrice_Estimate", "priceEstimate/bulkUpdateCorporatePrice",
                new { Controller = "PriceEstimate", Action = "BulkUpdateCorporatePrice" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Save_Price_Estimate_FranchiseeWise", "priceEstimate/savePriceEstimateFranchiseeWise",
                new { Controller = "PriceEstimate", Action = "SavePriceEstimateFranchiseeWise" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Bulk_Price_Estimate", "priceEstimate/bulkUpdatePriceEstimate",
                new { Controller = "PriceEstimate", Action = "BulkUpdatePriceEstimate" }, 
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );
            context.Routes.MapHttpRoute("Get_Price_Estimate_Collection_Franchisee", "priceEstimate/getPriceEstimateCollectionPerFranchisee",
               new { Controller = "PriceEstimate", Action = "GetPriceEstimateCollectionPerFranchisee" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Get_Shift_Charges", "priceEstimate/getShiftCharges",
               new { Controller = "PriceEstimate", Action = "GetShiftCharges" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Save_Shift_Charges", "priceEstimate/saveShiftCharges",
               new { Controller = "PriceEstimate", Action = "SaveShiftCharges" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Get_Replacement_Charges", "priceEstimate/getReplacementCharges",
               new { Controller = "PriceEstimate", Action = "GetReplacementCharges" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Save_Replacement_Charges", "priceEstimate/saveReplacementCharges",
               new { Controller = "PriceEstimate", Action = "SaveReplacementCharges" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Get_Maintenance_Charges", "priceEstimate/getMaintenanceCharges",
               new { Controller = "PriceEstimate", Action = "GetMaintenanceCharges" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Save_Maintenance_Charges", "priceEstimate/saveMaintenanceCharges",
               new { Controller = "PriceEstimate", Action = "SaveMaintenanceCharges" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Floor_Grinding_Adjustment", "priceEstimate/getFloorGrindingAdjustment",
               new { Controller = "PriceEstimate", Action = "GetFloorGrindingAdjustment" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Save_Floor_Grinding_Adjustment", "priceEstimate/saveFloorGrindingAdjustmentNote",
               new { Controller = "PriceEstimate", Action = "SaveFloorGrindingAdjustmentNote" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Get_Seo_Histry", "priceEstimate/getSeoHistry",
               new { Controller = "PriceEstimate", Action = "GetSeoHistry" },
               new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
             );
            context.Routes.MapHttpRoute("Save_Seo_Notes", "priceEstimate/saveSeoNotes",
              new { Controller = "PriceEstimate", Action = "SaveSeoNotes" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("priceEstimateData_download", "priceEstimate/download",
              new { Controller = "PriceEstimate", Action = "DownloadPriceEstimateDataFile" }, 
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute(
              "upload_priceEstimate",
              "priceEstimate/file/upload",
              new { Controller = "PriceEstimate", Action = "Upload" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute(
             "Get_priceEstimate_list",
             "priceEstimate/list",
             new { Controller = "PriceEstimate", Action = "GetList" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
         );
            context.Routes.MapHttpRoute(
             "Save_priceEstimate_notes",
             "priceEstimate/save/serviceNotes",
             new { Controller = "PriceEstimate", Action = "SaveServiceTagNotes" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute(
             "Get_priceEstimate_notes",
             "priceEstimate/get/serviceNotes",
             new { Controller = "PriceEstimate", Action = "GetServiceTagNotes" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );

        }
    }
}