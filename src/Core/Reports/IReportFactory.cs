using Core.Billing.Domain;
using Core.Notification.Domain;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Reports
{
    public interface IReportFactory
    {
        ServiceReportViewModel CreateViewModel(FranchiseeServiceClassCollection franchiseeServiceClass);
        LateFeeReportViewModel CreateViewModel(FranchiseeInvoice domain);
        WeeklyNotificationReportViewModel CreateViewModelForNotification(FranchiseeInvoice domain, DateTime startDate, DateTime endDate);
        WeeklyNotificationReportViewModel CreateViewModelForPreviousDate(FranchiseeInvoice domain, DateTime startDate);
        WeeklyNotification CreateDomain(DateTime date, long notificationTypeId);
        CustomerEmailReportViewModel CreateViewModel(EmailReportViewModel model);
        ChartViewModel CreateViewModel(EmailChartDataViewModel model);
        UploadBatchCollectionViewModel CreateViewModel(BatchUploadRecord domain);
        BatchUploadRecord CreateDomain(DateTime startDate, DateTime endDate, int waitPeriod, long franchiseeId, long? paymentFrequencyId, SalesDataUpload upload);
        ProductChannelReportViewModel CreateModel(FranchiseeServiceClassCollection franchiseeServiceClass);
        TopLeadersInfoModel CreateViewModel(TopLeadersInfoModel model, decimal amount);
        WeeklyNotificationReportViewModel CreateViewModelForNotificationForARReport(FranchiseeInvoice domain, DateTime startDate, DateTime endDate);

        EmailViewModel CreateViewModel(NotificationQueue franchiseeServiceClass);

        SeoHistryListModel CreateSeoViewModel(HoningMeasurement honingMeasurement, OrganizationRoleUser orgRoleUser, List<EstimatePriceNotes> seoPriceNoteList);
        PriceEstimateExcelViewModel CreatePriceEstimateViewModel(PriceEstimateViewModel model);
        PriceEstimateExcelViewModelForFA CreatePriceEstimateViewModelForFA(PriceEstimateViewModelForFA model);
        PriceEstimateFileUpload CreatePriceEstimateExcelUploadViewModel(PriceEstimateExcelUploadModel model);
        PriceEstimateDataUploadViewModel CreateViewModelPriceEstimateDataUpload(PriceEstimateFileUpload model);
        ChartViewModel CreateViewModel(ReviewChartDataViewModel model);
    }
}
