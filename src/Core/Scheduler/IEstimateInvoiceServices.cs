using Core.Sales.ViewModel;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler
{
   public interface IEstimateInvoiceServices
    {
        EstimateInvoiceViewModel GetEstimateInvoice(InvoiceEstimateFilterModel model,long? userId, long? roleId);
        bool SaveEstimateInvoice(EstimateInvoiceEditModel model);
        int SendMailToCustomer(long? schedulerId, List<int> serviceInvoice, string page, long? userId, SelectInvoicesViewModel model);
        string UploadInvoicesZipFile(long schedulerId, List<int> serviceInvoice);
        bool SaveCustomerSignature(CustomersignatureViewModel model);
        string UploadSignedInvoicesZipFile(JobInvoiceDownloadViewModel model);
        string UploadInvoicesCustomerZipFile(long schedulerId, List<int> serviceInvoice);
        bool AddInvoiceToEstimate(long schedulerId, bool isInvoiceForJob, List<long?> invoiceNumbers = null, long? jobId = null, long? scheduleJobId = null);
        bool SendFeedBackMailToCustomer(SelectInvoicesViewModel model);
        bool CustomerIsAvailableOrNot(SelectInvoicesViewModel model);
        EstimateInvoiceServiceGetServiceResultModel GetServiceTypeId(EstimateInvoiceServiceGetServiceModel model);
    }
}
