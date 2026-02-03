using Core.Review.ViewModel;
using Core.Sales.Domain;
using Core.Sales.ViewModel;

namespace Core.Review
{
    public interface ICustomerFeedbackService
    {
        //ReviewAPIResponseModel
        ReviewAPIResponseModel TriggerEmail(Customer customer, CustomerCreateEditModel customerModel, long franchiseeId,
            string qBinvoiceId, string customerEmail, long marketingClassId);
        ReviewAPIResponseModel GetCustomer(long customerId, string clientId);
        ReviewAPIResponseModel SendFeedbackRequest(long customerId, string clientId);
        //ReviewAPIResponseModel CreateBusiness(string clientId);
        //ReviewAPIResponseModel GetListOfBusiness(string clientId);

        // ReviewAPIResponseModel GetBusinessInformation(int businessId, string clientId);
        ReviewPushResponseListModel GetFeedback(string clientId, long businessId, string from, string to);
        ReviewPushResponseListModel GetFeedbackForAllData();

    }
}
