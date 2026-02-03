using Core.Sales.Domain;
using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface ISalesDataUploadFactory
    {
        SalesDataUpload CreateDomain(SalesDataUploadCreateModel model);
        SalesDataUploadCreateModel CreateModel(SalesDataUpload domain);
        SalesDataUploadViewModel CreateListModel(SalesDataUpload domain);
        AnnualSalesDataUpload CreateAnnualUploadDomain(SalesDataUploadCreateModel model, SalesDataUpload upload);
        SalesDataUploadViewModel CreateListModel(AnnualSalesDataUpload domain);
       
    }
}
