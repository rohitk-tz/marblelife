using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.MarketingLead
{
    public class MarketingLeadAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MarketingLead";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute("update_franchisee", "routingNumber/{id}/update",
             new { Controller = "RoutingNumber", Action = "UpdateFranchisee" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute("update_tag", "routingNumber/{id}/update/tag",
           new { Controller = "RoutingNumber", Action = "UpdateTag" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );

            context.Routes.MapHttpRoute(
                "weblead_download",
                "webLead/download",
              new { Controller = "WebLead", Action = "DownloadWebLeads" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
                "marketinglead_download",
                "marketingLead/download",
              new { Controller = "MarketingLead", Action = "DownloadMarketingLead" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute(
               "phoneLabels_download",
               "routingNumber/download",
             new { Controller = "RoutingNumber", Action = "DownloadRoutingNumber" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
             "weblead_report",
             "marketingLead/webLead/report",
          new { Controller = "WebLead", Action = "GetWebLeadReport" },
          new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );

            context.Routes.MapHttpRoute(
               "weblead_report_download",
               "webLead/report/download",
             new { Controller = "WebLead", Action = "DownloadWebLeadReport" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
             "callDetail_report",
             "marketingLead/report",
          new { Controller = "MarketingLead", Action = "GetCallDetailReport" },
          new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );

            context.Routes.MapHttpRoute(
               "callDetail_report_download",
               "marketingLead/report/download",
             new { Controller = "MarketingLead", Action = "DownloadCallDetailReport" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
                     "phoneVsWeb_report",
                     "marketingLead/graph/phone/vs/web",
                  new { Controller = "MarketingLeadGraph", Action = "GetPhoneVsWebReport" },
                  new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                 );

            context.Routes.MapHttpRoute(
                         "busVsPhone_report",
                         "marketingLead/graph/bus/vs/phone",
                      new { Controller = "MarketingLeadGraph", Action = "GetBusVsPhoneReport" },
                      new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                    );

            context.Routes.MapHttpRoute(
                        "localVsNational_report",
                        "marketingLead/graph/local/vs/national",
                     new { Controller = "MarketingLeadGraph", Action = "GetLocalVsNationalReport" },
                     new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );

            context.Routes.MapHttpRoute(
                        "spamVsPhone_report",
                        "marketingLead/graph/spam/vs/phone",
                     new { Controller = "MarketingLeadGraph", Action = "GetSpamVsPhoneReport" },
                     new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );

            context.Routes.MapHttpRoute(
                        "summary_report",
                        "marketingLead/graph/summary",
                     new { Controller = "MarketingLeadGraph", Action = "GetSummaryReport" },
                     new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );

            context.Routes.MapHttpRoute(
                   "weekly_report",
                   "marketingLead/graph/phone/weekly",
                new { Controller = "MarketingLeadGraph", Action = "GetLocalVsNationalPhoneReport" },
                new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
              );

            context.Routes.MapHttpRoute(
                        "daily_report",
                        "marketingLead/graph/phone/daily",
                     new { Controller = "MarketingLeadGraph", Action = "GetDailyPhoneReport" },
                     new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );

            context.Routes.MapHttpRoute(
                        "seasonal_report",
                        "marketingLead/graph/lead",
                     new { Controller = "MarketingLeadGraph", Action = "GetSeasonalLeadReport" },
                     new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                   );
            context.Routes.MapHttpRoute(
             "callDetail_report_raw_data",
             "marketingLead/report/rawdata",
          new { Controller = "MarketingLead", Action = "GetCallDetailReportRawData" },
          new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );

            context.Routes.MapHttpRoute(
                      "summary_report_adjusted",
                      "marketingLead/graph/summary/adjusted",
                   new { Controller = "MarketingLeadGraph", Action = "GetAdjustedSummaryReport" },
                   new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
                 );
            context.Routes.MapHttpRoute(
                "callDetail_graph_call_data",
                "marketingLead/graph/call",
              new { Controller = "MarketingLeadGraph", Action = "GetCallDataReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute(
                "localSitePerformance_graph_call_data",
                "marketingLead/graph/local/performance",
              new { Controller = "MarketingLeadGraph", Action = "GetLocalSitePerformanceReport" },
              new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("management_vs_local_report", "marketingLead/management/vs/local/report",
          new { Controller = "MarketingLeadGraph", Action = "GetManagementVsLocalReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute("management_report", "marketingLead/management/report",
          new { Controller = "MarketingLeadGraph", Action = "GetManagementReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute("performance_histry", "marketingLead/performance/histry",
             new { Controller = "LeadPerformanceFranchiseeDetails", Action = "GetPerformanceHistry" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("performance_report_National", "marketingLead/performance/report/nationalLevel",
            new { Controller = "LeadPerformanceFranchiseeDetails", Action = "GetPerformanceReportNational" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute("performance_report_Local", "marketingLead/performance/report/localLevel",
           new { Controller = "LeadPerformanceFranchiseeDetails", Action = "GetPerformanceReportLocal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute("ppr_seo_list_national", "marketingLead/seo/ppr/report/national",
          new { Controller = "LeadPerformanceFranchiseeDetails", Action = "GetSeoAndPpcNational" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }

        );
            context.Routes.MapHttpRoute("ppr_seo_list_local", "marketingLead/seo/ppr/report/local",
         new { Controller = "LeadPerformanceFranchiseeDetails", Action = "GetSeoAndPpcLocal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );

            context.Routes.MapHttpRoute("home_Advisor", "marketingLead/home/Advisor",
        new { Controller = "MarketingLead", Action = "GetHomeAdvisorReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );

            context.Routes.MapHttpRoute("leadFlow_report", "marketingLead/LeadFlow",
         new { Controller = "MarketingLead", Action = "GetForV2" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
       );

            context.Routes.MapHttpRoute(
               "marketinglead_downloadLeadFlow",
               "marketingLead/downloadLeadFlow",
             new { Controller = "MarketingLead", Action = "DownloadLeadFlow" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
               "marketingLead/getFranchiseePhoneCalls",
               "marketingLead/getFranchiseePhoneCalls",
             new { Controller = "MarketingLead", Action = "GetFranchiseePhoneCalls" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
               "marketingLead/editFranchiseePhoneCalls",
               "marketingLead/editFranchiseePhoneCalls",
             new { Controller = "MarketingLead", Action = "EditFranchiseePhoneCalls" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute(
              "marketingLead/generatePhoneCallInvoice",
              "marketingLead/generatePhoneCallInvoice",
            new { Controller = "MarketingLead", Action = "GeneratePhoneCallInvoice" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute(
               "marketingLead/editFranchiseePhoneCallsByBulk",
               "marketingLead/editFranchiseePhoneCallsByBulk",
             new { Controller = "MarketingLead", Action = "EditFranchiseePhoneCallsByBulk" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
               "marketingLead/getFranchiseePhoneCallsBulkList",
               "marketingLead/getFranchiseePhoneCallsBulkList",
             new { Controller = "MarketingLead", Action = "GetFranchiseePhoneCallsBulkList" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute(
               "marketingLead/saveFranchiseePhoneCallsByBulk",
               "marketingLead/saveFranchiseePhoneCallsByBulk",
             new { Controller = "MarketingLead", Action = "SaveFranchiseePhoneCallsByBulk" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute(
               "marketingLead/getAutomationBackUpReport",
               "marketingLead/getAutomationBackUpReport",
             new { Controller = "MarketingLead", Action = "GetAutomationBackUpReport" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute(
               "marketingLead/getFranchiseePhoneCallsListForFranchisee",
               "marketingLead/getFranchiseePhoneCallsListForFranchisee",
             new { Controller = "MarketingLead", Action = "GetFranchiseePhoneCallListForFranchisee" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute(
              "marketingLead/saveCallDetailsReportNotes",
              "marketingLead/saveCallDetailsReportNotes",
            new { Controller = "MarketingLead", Action = "SaveCallDetailsReportNotes" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute(
              "marketingLead/getCallDetailsReportNotes",
              "marketingLead/getCallDetailsReportNotes",
            new { Controller = "MarketingLead", Action = "GetCallDetailsReportNotes" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute(
              "marketingLead/getOfficeCollection",
              "marketingLead/getOfficeCollection",
            new { Controller = "MarketingLead", Action = "GetOfficeCollection" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute(
              "marketingLead/getFranchiseeNameValuePairCollection",
              "marketingLead/getFranchiseeNameValuePairCollection",
            new { Controller = "MarketingLead", Action = "GetFranchiseeNameValuePairCollection" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );
            context.Routes.MapHttpRoute(
               "marketinglead_downloadCallNotesHistoryDetails",
               "marketingLead/downloadCallNotesHistoryDetails",
             new { Controller = "MarketingLead", Action = "DownloadCallNotesHistoryDetails" },
             new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute(
              "marketingLead/EditCallDetailsReportNotes",
              "marketingLead/EditCallDetailsReportNotes",
            new { Controller = "MarketingLead", Action = "EditCallDetailsReportNotes" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
        }
    }
}