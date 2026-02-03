using Core.Billing.Domain;
using Core.Billing.ViewModel;
using System.Collections.Generic;

namespace Core.Notification
{
    public interface IInvoiceNotificationService
    {
        void CreateInvoiceDetailNotification(IList<FranchiseeInvoice> franchiseeInvoiceList, long franchiseeId);
    }
}
