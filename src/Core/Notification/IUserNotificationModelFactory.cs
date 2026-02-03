using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using System;
using System.Collections.Generic;

namespace Core.Notification
{
    public interface IUserNotificationModelFactory
    {
        void CreateForgetPasswordNotification(string passwordLink, Person person);
        void CreateLoginCredentialNotification(Person perosn, string password, bool includeSetupGuide);
        void CreateInvoiceDetailNotification(long organizationId, IList<FranchiseeInvoice> franchiseeInvoiceList);
        void CreatePaymentReminderNotification(IList<FranchiseeInvoice> franchiseeInvoiceList, Franchisee franchisee);
        void CreateSalesDataReminderNotification(SalesDataUpload salesData, DateTime startDate, DateTime endDate, long? paymentFrequencyId);
        void CreatePaymentConfirmationNotification(Invoice invoice, Payment payment, long organizationId);
        void CreateLateFeeReminderNotification(InvoiceItem invoiceItem, long organizationId, long lateFeeTypeId, DateTime currentDate);
        void CreateWeeklyNotification(File file, IEnumerable<FranchiseeInvoice> franchiseeInvoices, DateTime startdate, DateTime enddate, NotificationTypes type);
        ReviewAPIResponseModel SendEmailFeedbackRequest(string customerEmail, string customerName, Franchisee franchisee);
        void CreateMonthlyNotificationModel(File file, DateTime startDate, DateTime endDate, NotificationTypes notificationType,File files2=null);
        void CreateReviewSystemRecordNotification(File file, DateTime startDate, DateTime endDate, long organizationId);
        void CreateSalesUploadNotification(File file, DateTime startDate, DateTime endDate);
        void CreateAnnualUploadNotification(AnnualSalesDataUpload annualFileUpload);
        void CreateReviewActionNotification(AnnualSalesDataUpload annualFileUpload, bool isAccept);
        void CreateDocumentUploadNotification(string fileName, OrganizationRoleUser uploadedBy, Franchisee franchisee);
        void CreateDocumentExpiryNotification(FranchiseDocument doc);
        void ScheduleReminderNotification(JobScheduler franchiseeData, DateTime startDate, DateTime endDate,string encryptedData, NotificationTypes notificationTypes);
        void NewJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        void CancelJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        void UpdateJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData);

        void NewJobOrEstimateReminderNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        void CancelJobOrEstimateReminderNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        void UpdateJobOrEstimateReminderNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        void CancelJobOrEstimateReminderNotificationtoTechForDeleteButton(JobScheduler jobscheduler,string AssigneeName,string AssigneePhone,bool isFromEstimate);
        void ScheduleReminderNotificationToUser(JobScheduler franchiseeData, DateTime startDate, DateTime endDate);
        void ScheduleReminderNotificationToUserOnDay(JobScheduler schedule, DateTime startDate, DateTime endDate);

        void NewJobOrEstimateReminderNotificationtoTechForRescheduled(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        void NewJobOrEstimateReminderNotificationtoTechForRescheduledForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        void UrgentJobOrEstimateReminderNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData);
        void UrgentEstimateReminderNotificationtoTech(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData);
        void CreateWeeklyNotificationForArReport(File file, IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> franchiseeInvoicesFranchiseeWise, DateTime startDate, DateTime endDate, NotificationTypes type, decimal totalAmount);
        long? BeforeAfterImageNotificationtoCustomer(BeforeAfterImageMailViewModel franchiseeData,File fileDomain,NotificationTypes notificationId);
        void CreateLoanCompletionNotification(FranchiseeLoan loanSchedule);

        BeforeAfterBestPairViewModel CreateBeforeAfterBestPairModel(JobEstimateImage jobEstimateImageBefore, JobEstimateImage jobEstimateImageAfter,JobScheduler scheduler,MarkbeforeAfterImagesHistry markbeforeAfterImagesHistry
              );
        BeforeAfterBestPairViewModel CreateBeforeAfterPairModel(JobEstimateImage jobEstimateImageBefore,
                     JobEstimateImage jobEstimateImageAfter, JobScheduler scheduler, List<OrganizationRoleUser> organizations);
        void NewJobOrEstimateReminderNotificationtoTechForMeeting(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData,NotificationTypes notificationTypes);
        long? InvoiceCustomerNotificationtoCustomer(BeforeAfterImageMailViewModel franchiseeData, List<File> fileDomainList, NotificationTypes notificationId);
        long? InvoiceCustomerNotificationtoCustomerForSignedInvoices(BeforeAfterImageMailViewModel franchiseeData, List<File> fileDomainList, NotificationTypes notificationId);

        void SendWebLeadsNotification(NotificationTypes notificationTypes, DateTime date);

    }
}
