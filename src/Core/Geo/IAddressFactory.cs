using Core.Billing.Domain;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Sales.Domain;
using Core.Scheduler.Domain;

namespace Core.Geo
{
    public interface IAddressFactory
    {
        Address CreateDomain(AddressEditModel model);
        AddressEditModel CreateEditModel(Address domain);
        AddressViewModel CreateViewModel(Address domain);
        Address CreateDomainForCustomerAddress(AddressEditModel model);
        AddressViewModel CreateViewModel(InvoiceAddress domain);
        AddressViewModel CreateViewModel(AuditFranchiseeSales domain);
        AddressViewModel CreateViewModelForFranchisee(Address domain);
    }
}
