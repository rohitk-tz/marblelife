using Core.Application.Attribute;
using Core.Billing.Domain;
using System.Collections.Generic;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class InvoiceNotificationService : IInvoiceNotificationService
    {
        private IUserNotificationModelFactory _userNotificationModelFactory;

        public InvoiceNotificationService(IUserNotificationModelFactory userNotificationModelFactory)
        {
            _userNotificationModelFactory = userNotificationModelFactory;
        }
        public void CreateInvoiceDetailNotification(IList<FranchiseeInvoice> franchiseeInvoiceList, long franchiseeId)
        {
            _userNotificationModelFactory.CreateInvoiceDetailNotification(franchiseeId, franchiseeInvoiceList);
        }
    }
}
