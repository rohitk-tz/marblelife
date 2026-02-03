using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;

namespace Core.Sales
{
    public interface ICustomerFactory
    {
        Customer CreateDomain(CustomerCreateEditModel model, Customer domain);
        CustomerViewModel CreateViewModel(Customer customer);
        CustomerFileUpload CreateCustomerFileUpload(CustomerFileUploadCreateModel model);
        Customer CreateDomain(CustomerEditModel model);
        Address CreateDomain(AddressEditModel model);

        Customer CreateCustomerDomain(CustomerEditModel model);
        CustomerViewModel CreateCustomerViewModel(Customer customer, FranchiseeSales lastInvoiceDetail, bool isSynced);
    }
}
