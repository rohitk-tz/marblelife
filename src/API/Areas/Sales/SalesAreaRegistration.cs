using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.Sales
{
    public class SalesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Sales";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute("last_uploaded_batch", "Sales/batch/last/uploaded/{franchiseeId}",
              new { Controller = "Batch", Action = "GetLastUploadedBatch" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute("sales_royalty_report", "sales/royalty/report/{salesDataUploadId}",
              new { Controller = "Sales", Action = "GetRoyaltyReport" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute("invoice_adfund_download", "invoice/downloadAdfundInvoice",
              new { Controller = "Invoice", Action = "DownloadAdfundInvoiceFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            
            context.Routes.MapHttpRoute("invoice_royality_download", "invoice/downloadRoyalityInvoice",
              new { Controller = "Invoice", Action = "DownloadRoyalityInvoiceFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("invoiceList_download", "invoice/download/list",
              new { Controller = "Invoice", Action = "DownloadInvoiceListFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("invoice_download_all", "invoice/download/all",
              new { Controller = "Invoice", Action = "DownloadAllInvoiceFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("invoiceList_download_all", "invoice/download/list/all",
              new { Controller = "Invoice", Action = "DownloadInvoiceListAllFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("invoiceItem_delete", "invoice/invoiceItem/{invoiceItemId}/delete",
              new { Controller = "Invoice", Action = "DeleteInvoiceItem" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute("salesData_download", "sales/download",
              new { Controller = "Sales", Action = "DownloadSalesDataFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("customer_download", "customer/download",
              new { Controller = "Customer", Action = "DownloadCustomerFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("sales_batch_delete", "sales/batch/delete/{id}",
              new { Controller = "Batch", Action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) }
            );

            context.Routes.MapHttpRoute("sales_batch_reparse", "sales/batch/reparse/{id}",
              new { Controller = "Batch", Action = "Reparse" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
            );

            context.Routes.MapHttpRoute("account_credit_list", "sales/account/credit",
             new { Controller = "AccountCredit", Action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute("franchisee_account_credit_list", "sales/account/list/credit",
             new { Controller = "AccountCredit", Action = "GetAccountCreditList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute("customer_save", "customer/save",
              new { Controller = "Customer", Action = "SaveCustomer" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("sales_batch_update", "sales/batch/update",
             new { Controller = "Batch", Action = "Update" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute("customer_list_upload", "sales/batch/customer/upload",
             new { Controller = "Batch", Action = "UploadCustomerList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute("customer_update_class", "customer/{id}/update",
              new { Controller = "Customer", Action = "UpdateMarketingClass" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("invoiceList_downloaded", "invoice/downloaded/list",
              new { Controller = "Invoice", Action = "GetDownloadedInvoiceList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("invoice_markDownloaded", "invoice/mark/downloaded",
              new { Controller = "Invoice", Action = "MarkAsDownloaded" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("invoice_saveReconciliationNotes", "invoice/SaveReconciliationNotes",
              new { Controller = "Invoice", Action = "SaveReconciliationNotes" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );

            context.Routes.MapHttpRoute("sales_batch_annual", "sales/batch/verify",
            new { Controller = "Batch", Action = "GetAnnualUplodInfo" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute("annula_Sales", "sales/annualBatch/detail",
             new { Controller = "AnnualBatch", Action = "GetAnnualSales" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute("delete_sales", "sales/annualBatch/delete/{id}",
             new { Controller = "AnnualBatch", Action = "Delete" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Delete) }
           );

            context.Routes.MapHttpRoute("batch_action", "sales/annualBatch/{isAccept}/action/{batchId}",
            new { Controller = "AnnualBatch", Action = "ManageBatch" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute("audit_record", "sales/annualBatch/audit/records",
           new { Controller = "AnnualBatch", Action = "GetAnnualAuditRecord" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );

            context.Routes.MapHttpRoute("audit_Detail", "sales/annualBatch/{invoiceId}/compare/{auditInvoiceId}",
          new { Controller = "AnnualBatch", Action = "GetInvoiceDetails" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );

            context.Routes.MapHttpRoute("annualsData_download", "sales/annualBatch/download",
             new { Controller = "AnnualBatch", Action = "DownloadAnnualDataFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute("annual_upload", "sales/annualBatch/upload",
            new { Controller = "AnnualBatch", Action = "UploadAnnualFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );

            context.Routes.MapHttpRoute("invoiceData_download", "sales/salesInvoice/download",
             new { Controller = "SalesInvoice", Action = "Download" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );

            context.Routes.MapHttpRoute("annula_Sales_Address", "sales/annualBatch/detail/address",
             new { Controller = "AnnualBatch", Action = "GetAnnualSalesAddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("annula_Sales_Customer_Address", "sales/annualBatch/detail/address/customer",
            new { Controller = "AnnualBatch", Action = "GetAnnualCustomerAddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute("annula_Sales_Customer_Address_Updation", "sales/annualBatch/update/customer/adress",
            new { Controller = "AnnualBatch", Action = "UpdateCustomerAddress" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
          );
            context.Routes.MapHttpRoute("audit_record_reparse", "sales/annualBatch/audit/reparse/{id}",
        new { Controller = "AnnualBatch", Action = "ReparseAnnualAudit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
      );
            context.Routes.MapHttpRoute("sales_funnel_national", "sales/funnel/national",
       new { Controller = "SalesFunnel", Action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
     );
            context.Routes.MapHttpRoute("sales_funnel_national_download", "sales/funnel/national/download",
      new { Controller = "SalesFunnel", Action = "DownloadFunelNationalFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
    );
            context.Routes.MapHttpRoute("sales_funnel_local", "sales/funnel/local",
       new { Controller = "SalesFunnel", Action = "GetSalesFunnelLocal" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
     );
            context.Routes.MapHttpRoute("sales_funnel_local_graph", "sales/funnel/local/graph",
      new { Controller = "SalesFunnel", Action = "GetChartData" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
    );
            context.Routes.MapHttpRoute("sales_funnel_local_download", "sales/funnel/local/download",
     new { Controller = "SalesFunnel", Action = "DownloadFunelLocalFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
   );
            context.Routes.MapHttpRoute("update_sales_data", "sales/updateSalesData",
    new { Controller = "Sales", Action = "UpdateSalesData" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
  );
            context.Routes.MapHttpRoute("invoiceDetails_download_all", "sales/updateSalesData/download",
              new { Controller = "Sales", Action = "DownloadInvoiceFile" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
            );
            context.Routes.MapHttpRoute("invoiceDetails_upload_all", "sales/updateSalesData/upload",
             new { Controller = "Sales", Action = "Upload" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
           );
            context.Routes.MapHttpRoute("invoice_Updation_List", "sales/getInvoiceParseList",
           new { Controller = "Sales", Action = "getInvoiceParseList" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
         );
        }
    }
}