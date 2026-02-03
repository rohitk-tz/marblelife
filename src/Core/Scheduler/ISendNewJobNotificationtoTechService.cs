using Core.Application.Domain;
using Core.Notification.Enum;
using Core.Organizations.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler
{
   public interface ISendNewJobNotificationtoTechService
    {
        bool SendJobNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForCancelled(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForUpdation(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForCancelledForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForUpdationForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForCancelledForDeleteButton(JobScheduler jobScheduler,bool isFromEstimate);
        bool SendJobNotificationtoTechForRescheduled(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForRescheduledForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForUrgent(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        bool SendJobNotificationtoTechForUrgent(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        long? SendBeforeAfterImagestoCustomer(JobCustomer customerData, JobScheduler scheduler,File fileModel,NotificationTypes notificationId,string emailId);
        bool SendJobNotificationtoTechForMeetingPersonal(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData, NotificationTypes notificationTypes);
        long? SendingInvoiceToCustomer(EstimateInvoiceEditMailModel model, List<File> fileDomain, NotificationTypes notificationId, string emailId);
        long? SendingInvoiceToCustomerForSignedInvoices(EstimateInvoiceEditMailModel model, List<File> fileDomain, NotificationTypes notificationId, string emailId, bool isFromURL, bool mailToSalesRep);
    }
}
