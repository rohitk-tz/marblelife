using API.Attribute;
using System.Web.Http.Filters;

namespace API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(HttpFilterCollection filters)
        {
            filters.Add(new BasicAuthenticationAttribute());
            filters.Add(new CustomDataBinderAttribute());
            filters.Add(new DbTransactionFilterAttribute());
            filters.Add(new BasicExceptionFilterAtribute());
        }
    }
}
