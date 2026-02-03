using Core.Billing.Domain;
using System;

namespace Core.Notification
{
    public interface ILateFeeNotificationService
    {
        void CreateLateFeeNotification(InvoiceItem invoiceItem, long organizationId, DateTime currentDate);
    }
}
