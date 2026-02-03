using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using System;

namespace Core.Review
{
    public interface ICustomerFeedbackFactory
    {
        CreateCustomerForReviewModel CreateModel(Customer customer, ReviewAPIResponseModel responseModel, long businessId);
        GetCustomerForReviewModel CreateModel(long customerId, string clientId);
        CustomerReviewSystemRecord CreateDomain(Customer customer, Franchisee franchisee, long reviewSystemCustomerId);
        CustomerFeedbackRequest CreateDomain(long customerId, long franchiseeSalesId, DateTime date, string customerEmail, string qBInvoiceId, bool isSystemGenerated, long franchiseeId, long reviewSystemRecordId);
        FeedbackRequestModel CreateModel(string clientId, long businessId, string to, string from);
        CustomerFeedbackResponse CreateDomain(FeedbackResponseViewModel review);
        UpdateCustomerRecordViewModel CreateModel(string customerEmail, string firstName, string lastName, long businessId, string clientId, long customerId);
        CustomerFeedbackResponse CreateDomain(ReviewPushResponseViewModel review);
        ReviewPushCustomerFeedback CreateDomainForFeedBack(ReviewPushResponseViewModel review);
    }
}
