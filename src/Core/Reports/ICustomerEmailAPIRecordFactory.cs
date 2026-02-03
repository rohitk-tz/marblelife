using Core.Notification.ViewModel;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Review.Domain;
using Core.Sales.ViewModel;

namespace Core.Reports
{
    public interface ICustomerEmailAPIRecordFactory
    {
        CustomerEmailAPIRecord CreateDomain(CustomerCreateEditModel customer, string email, long franchiseeId);
        CustomerEmailAPIRecordRequestModel CreateModel(CustomerEmailAPIRecord domain);
        CustomerEmailAPIRecord CreateDomain(CustomerEmailAPIRecordResponseModel model, CustomerEmailAPIRecord domain);
        EmailAPINotificationModel CreateNotificationModel(CustomerEmailAPIRecord domain);
        CustomerEmailAPIRecordRequestModel CreateModelForPartialPayment(PartialPaymentEmailApiRecord domain);
        PartialPaymentEmailApiRecord CreateModelForPartialPayment(CustomerEmailAPIRecordResponseModel model, PartialPaymentEmailApiRecord domain);

        PartialPaymentEmailApiRecord CreateDomain(string customerEmail, long? franchiseeId, long? invoiceId, long? customerId);
        PartialPaymentEmailApiRecord CreateDomain(string customerEmail, long? franchiseeId, long? invoiceId, long? customerId, long? statusId);
        CustomerEmailAPIRecordRequestModel CreateModelForPayment(PartialPaymentEmailApiRecord domain);
        EmailAPINotificationModel CreateNotificationModel(PartialPaymentEmailApiRecord domain);
    }
}
