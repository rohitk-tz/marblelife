using Core.Application.Attribute;
using Core.Sales.Domain;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class AnnualAuditNotificationService : IAnnualAuditNotificationService
    {
        private IUserNotificationModelFactory _userNotificationModelFactory;
        public AnnualAuditNotificationService(IUserNotificationModelFactory userNotificationModelFactory)
        {
            _userNotificationModelFactory = userNotificationModelFactory;
        }
        public void CreateAnnualUploadNotification(AnnualSalesDataUpload annualFileUpload)
        {
            _userNotificationModelFactory.CreateAnnualUploadNotification(annualFileUpload);
        }

        public void CreateReviewActionNotification(AnnualSalesDataUpload annualFileUpload, bool isAccept)
        {
            _userNotificationModelFactory.CreateReviewActionNotification(annualFileUpload, isAccept); 
        }
    }
}
