using Core.Billing.Domain;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Sales.Domain;
using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface ISalesInvoiceFactory
    {
        SalesInvoiceViewModel CreateViewModel(InvoiceItem domain);
        InvoiceFileUpload CreatDoamin(CustomerFileUploadCreateModel model);
        Address CreateDomain(AddressEditModel model);
    }
}
