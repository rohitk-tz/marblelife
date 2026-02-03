using Core.Billing.Domain;
using Core.Organizations.Domain;

namespace Core.Notification
{
    public interface IPaymentReminderPollingAgent
    {
        void CreateNotificationReminderForPayment();
        void CreatePaymentConfirmationNotification(Invoice invoice, Payment payment, long organizationId);
        void CreateLoanCompletionNotification(FranchiseeLoan loanSchedule);
    }
}
