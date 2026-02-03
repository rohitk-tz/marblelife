using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.Payment
{
    public class PaymentAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Payment";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute(
                  "Card_disable",
                  "Payment/{instrumentIds}/manage/instrument",
                  new { Controller = "Payment", Action = "ManageInstrument" }
              );

            context.Routes.MapHttpRoute(
                  "card_delete",
                  "Payment/{instrumentIds}/delete/instrument",
                  new { Controller = "Payment", Action = "DeleteInstrument" }
              );

            context.Routes.MapHttpRoute(
                 "card_setPrimary",
                 "Payment/{instrumentIds}/{franchiseeId}/setPrimary",
                 new { Controller = "Payment", Action = "SetPrimary" }
             );

            context.Routes.MapHttpRoute(
                  "payment",
                  "Payment/{franchiseeId}/{invoiceId}/chargecard/new",
                  new { Controller = "Payment", Action = "MakePaymentByNewChargeCard" }
              );
            context.Routes.MapHttpRoute(
                  "eCheck_payment",
                  "Payment/{franchiseeId}/{invoiceId}/echeck",
                  new { Controller = "Payment", Action = "MakePaymentByECheck" }
              );
            context.Routes.MapHttpRoute(
                  "onFile_payment",
                  "Payment/{franchiseeId}/{invoiceId}/chargecard/file",
                  new { Controller = "Payment", Action = "MakePaymentOnFileChargeCard" }
              );

            context.Routes.MapHttpRoute(
                 "save_checkPayment",
                 "Payment/{franchiseeId}/{invoiceId}/check/record",
                 new { Controller = "Payment", Action = "SaveCheck" }
             );

            context.Routes.MapHttpRoute(
                  "chargeCard_Detail",
                  "Payment/{franchiseeId}/{paymentTypeId}/instrument/list",
                  new { Controller = "Payment", Action = "GetInstrumentList" }
              );

            context.Routes.MapHttpRoute(
                  "save_chargeCard",
                  "Payment/{franchiseeId}/chargecard",
                  new { Controller = "Payment", Action = "CreateChargeCardProfile" }
              );

            context.Routes.MapHttpRoute(
                  "create_echeck_profile",
                  "Payment/{franchiseeId}/echeck",
                  new { Controller = "Payment", Action = "CreateECheckProfile" }
              );
            context.Routes.MapHttpRoute(
                  "check_expiry",
                  "Payment/CheckExpiry/{month}/{year}",
                  new { Controller = "Payment", Action = "CheckExpiry" }
              );

            context.Routes.MapHttpRoute(
                "franchisee_instrumentlist",
                "Payment/{franchiseeId}/franchiseeinstrument/list",
                new { Controller = "Payment", Action = "GetFranchiseeInstrumentList" }
            );
            context.Routes.MapHttpRoute(
                 "adjust_AccountCredit",
                 "Payment/{franchiseeId}/{invoiceId}/adjust/account/Credit",
                 new { Controller = "Payment", Action = "AdjustAccountCredit" }
             );

            context.Routes.MapHttpRoute("franchisee_AccountCredit", "Payment/franchiseeAccountCredit/{franchiseeId}/account/credit",
          new { Controller = "FranchiseeAccountCredit", Action = "GetFranchiseeAccountCredit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
        );

            context.Routes.MapHttpRoute("franchisee_AccountCredit_save", "Payment/franchiseeAccountCredit/{franchiseeId}/save",
          new { Controller = "FranchiseeAccountCredit", Action = "SaveAccountCredit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
        );
            context.Routes.MapHttpRoute("accountCredit_delete", "Payment/franchiseeAccountCredit/{accountCreditId}/delete",
             new { Controller = "FranchiseeAccountCredit", Action = "DeleteAccountCredit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );
            context.Routes.MapHttpRoute("accountCredit_remove", "Payment/franchiseeAccountCredit/{accountCreditId}/remove",
             new { Controller = "FranchiseeAccountCredit", Action = "RemoveAccountCredit" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
           );

            context.Routes.MapHttpRoute("franchisee_AccountCredit_invoice", "Payment/franchiseeAccountCredit/{franchiseeId}/get/{invoiceId}",
        new { Controller = "FranchiseeAccountCredit", Action = "GetCreditForInvoice" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
      );
        }
    }
}