using Core.Sales.Domain;

namespace Core.Notification
{
    public interface IAnnualAuditNotificationService
    {
        void CreateAnnualUploadNotification(AnnualSalesDataUpload annualFileUpload);
        void CreateReviewActionNotification(AnnualSalesDataUpload annualFileUpload, bool isAccept);
    }
}
