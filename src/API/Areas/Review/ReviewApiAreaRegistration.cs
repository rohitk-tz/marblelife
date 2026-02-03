using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Mvc;

namespace API.Areas.ReviewApi
{
    public class ReviewApiAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Review";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.Routes.MapHttpRoute(
              "reviewApi_customerList",
              "Review/ReviewSystem/getCustomerList/sdate/{startDate}/edate/{endDate}",
            new { Controller = "ReviewSystem", Action = "GetReviewCustomerList" },
            new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
          );
            context.Routes.MapHttpRoute(
             "reviewApi_customerList_api",
             "Review/ReviewSystem/SaveReviewPushResponse",
           new { Controller = "ReviewSystem", Action = "SaveReviewPushResponse" },
           new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) }
         );
            context.Routes.MapHttpRoute(
                        "reviewApi_zipCodeSearch_api",
                        "Review/ReviewSystem/SearchZipCode/zipCode/{zipCode}",
                      new { Controller = "ReviewSystem", Action = "SearchZipCode" },
                      new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                    );
            context.Routes.MapHttpRoute(
                       "reviewApi_searchFranchisee_api",
                       "Review/ReviewSystem/SearchFranchisee/{countryName}",
                     new { Controller = "ReviewSystem", Action = "SearchFranchisee" },
                     new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                   );
            context.Routes.MapHttpRoute(
                      "reviewApi_getCoutryList_api",
                      "Review/ReviewSystem/GetCountryList",
                    new { Controller = "ReviewSystem", Action = "GetAllCountries" },
                    new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) }
                  );
        }
    }
}