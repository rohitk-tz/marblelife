using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;

namespace Core.Sales
{
    public interface IRoyaltyReportFactory
    {
        IList<RoyaltyReportViewModel> CreateCollectionModel(IList<FranchiseeSales> domainCollection);

        IEnumerable<SelectListModel> GetMarketingClasses();
        IEnumerable<ServiceTypeListModel> GetServices();
        IEnumerable<SelectListModel> GetServiceTypeCategory();
    }
}
