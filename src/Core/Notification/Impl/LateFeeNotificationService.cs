using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using System;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class LateFeeNotificationService : ILateFeeNotificationService
    {
        private IUserNotificationModelFactory _userNotificationModelFactory;
        public LateFeeNotificationService(IUserNotificationModelFactory userNotificationModelFactory, IClock clock,
            ISettings settings)
        {
            _userNotificationModelFactory = userNotificationModelFactory;
        }
        public void CreateLateFeeNotification(InvoiceItem invoiceItem, long organizationId, DateTime currentDate)
        {
            if (invoiceItem.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.Royalty)
            {
                var lateFeeTypeId = (long)LateFeeType.Royalty;
                _userNotificationModelFactory.CreateLateFeeReminderNotification(invoiceItem, organizationId, lateFeeTypeId, currentDate);
            }
            if (invoiceItem.LateFeeInvoiceItem.LateFeeTypeId == (long)LateFeeType.SalesData)
            {
                var lateFeeTypeId = (long)LateFeeType.SalesData;
                _userNotificationModelFactory.CreateLateFeeReminderNotification(invoiceItem, organizationId, lateFeeTypeId, currentDate);
            }
        }
    }
}
