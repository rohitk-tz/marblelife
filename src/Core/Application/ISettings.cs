using Core.Application.ValueType;
using System;

namespace Core.Application
{
    public interface ISettings
    {
        int PageSize { get; }
        string DefaultRoyaltyAmount { get; }
        string MediaRootPath { get; }
        string MediaRootUrl { get; }
        Dimension DimensionNormal { get; }
        Dimension DimensionSmall { get; }
        Dimension DimensionLarge { get; }


        string SiteRootUrl { get; }

        string LogoImage { get; }

        string DefaultTimeZone { get; }

        string SmtpHost { get; }

        int SmtpPort { get; }

        string SmtpUserName { get; }

        string SmtpPassword { get; }


        string CompanyName { get; }
        string RecipientEmail { get; }
        string ApplicationName { get; }
        string SchedulingAppliation { get; }
        string OwnerName { get; }
        string OwnerEmail { get; }
        string Designation { get; }
        string OwnerPhone { get; }

        string FromNumber { get; }

        string ReviewPushLocalTionAPICronExpression { get; }
        bool EnableSmsNotification { get; }

        string FromEmail { get; }
        string ToSuperAdmin { get; }
        string AWSSecreatKey { get; }
        string AWSAccessKey { get; }
        string AWSBucketName { get; }
        string AWSBucketURL { get; }
        string AWSBucketThumbURL { get; }

        #region Cron Expression

        string EmailNotificationServiceCronExpression { get; }
        string SalesDataUploadReminderCronExpression { get; }
        string InvoiceGenerationServiceCronExpression { get; }
        string SalesDataParserServiceCronExpression { get; }
        string MarketingLeadDataParserServiceCronExpression { get; }
        string InvoiceLateFeeGeneratorCronExpression { get; }
        string CurrencyExchangeRateGeneratorCronExpression { get; }
        string CustomerFileParserServiceCronExpression { get; }
        string PaymentReminderCronExpression { get; }
        string WeeklyNotificationCronExpression { get; }
        string SendCustomerFeedbackRequestCronExpression { get; }
        string GetCustomerFeedbackCronExpression { get; }
        string CreateEmailRecordCronExpression { get; }
        string SendFeedbackNotificationReportCronExpression { get; }
        string ServicedCustomerNotificationCronExpression { get; }
        string SendEmailListNotificationCronExpression { get; }
        string GetMarketingLeadCronExpression { get; }
        string GetRoutingNumberCronExpression { get; }
        string GetUpdateLeadServiceCronExpression { get; }
        string UpdateGrowthReportServiceCronExpression { get; }
        string BatchUploadReportCronExpression { get; }
        string BatchUploadNotificationCronExpression { get; }
        string CalendarParseServiceCronExpression { get; }
        string LoanReScheduleCalculatorCronExpression { get; }
        string DocumentExpiryNotificationCronExpression { get; }
        string JobNotificationCronExpression { get; }
        string JobNotificationToUserCronExpression { get; }
        string GeoParserCronExpression { get; }
        string MergeFieldCronExpression { get; }
        string EmailNotificationOnFranchiseePriceExceed { get; }
        string EmailNotificationForPayrollReport { get; }
        string BeforeAfterImagesUploadwithS3Bucket { get; }
        string EmailNotificationForPhotoReport { get; }
        string S3BucketSyncIn2Min { get; }
        string CalendarImagesMigration { get; }
        #endregion

        bool EnableSsl { get; }
        int RecordCountForDashboard { get; }

        //.......................keys for Authoriza.net....................

        bool AuthNetTestMode { get; }

        // .....................

        int PasswordLength { get; }



        int PaymentReminderDayCount { get; }
        DateTime LateFeeStartDate { get; }

        bool SendNotificationToFranchiser { get; }
        string CCToAdmin { get; }
        int WeeklyReminderDayCount { get; }
        int MonthlyReminderDayCount { get; }
        decimal NationChargePercentage { get; }
        bool ApplyDateValidation { get; }
        bool ApplyLateFee { get; }
        bool ApplyAddressAndPhoneValidation { get; }

        string CurrencyExchangeRateApi { get; }
        string CurrencyExchengeRateApiKey { get; }

        bool GetMergeField { get; }
        bool GetCurrencyExchangeRate { get; }
        string ExchangeRateAppId { get; }

        bool GetHistoricalRate { get; }

        int DefaultRoyaltyLateFeeWaitPeriod { get; }
        int DefaultSalesDataLateFeeWaitPeriod { get; }
        bool SendWeeklyReminder { get; }
        int WeeklyReminderDay { get; }

        #region Review System

        string ClientId { get; }
        string ReviewApiKey { get; }
        bool SendFeedbackEnabled { get; }
        bool GetFeedbackEnabled { get; }
        string KioskLink { get; }

        #endregion

        #region Email Record API Info
        bool CreateEmailRecord { get; }
        string EmailApiKey { get; }
        string RegionCode { get; }
        string EmailAPIListId { get; }
        string MlDisturbutionId { get; }

        string EmailAPIListIdForPartialPayment { get; }
        #endregion
        bool SendCustomerListNotification { get; }
        bool SendMonthlyNotification { get; }
        string MailChimpReportRecipients { get; }
        string CCToReviewReportRecipients { get; }
        string ListCustomerRecipients { get; }
        string UnpainInvoiceRecipients { get; }
        string AuditRecipients { get; }

        #region Marketing Leads
        string AccessKey { get; }
        string SecretKey { get; }
        string AccessKeyV2 { get; }
        string SecretKeyV2 { get; }
        bool GetRoutingNumbers { get; }
        bool GetCallDetails { get; }
        bool GetWebLeads { get; }
        string WebLeadsAPIkey { get; }
        #endregion

        bool UpdateAllSalesAmount { get; }
        int DefaultMonthCountForMarketingLeads { get; }

        string FilePath { get; }
        string CalendarFilePath { get; }
        bool FeeProfileValidation { get; }
        bool UpdateInitialBatchRecord { get; }

        #region MarketingLead
        bool GetHistoryData { get; }

        DateTime CallDetailStartDate { get; }
        DateTime CallDetailEndDate { get; }
        DateTime WebLeadStartDate { get; }
        DateTime WebLeadEndDate { get; }
        #endregion
        bool UpdateMonthly { get; }
        bool ParseCalendarFile { get; }
        DateTime CurrencyRateStartDate { get; }
        bool UpdateInvoiceRecord { get; }
        bool UpdateOldData { get; }
        decimal DefaultOneTimeProjectAmount { get; }
        DateTime ZeroInvoiceStartDate { get; }
        bool SendExpiryNotification { get; }
        string NationalUrlString { get; }

        bool NewJobNotificationToTechAndSales { get; }
        bool NewJobNotificationToClient { get; }

        int SEMIFranchiseeId { get; }
        string SEMIFromEmail { get; }
        bool SendWeekyLateFeeNotification { get; }
        bool SendWeekyUnpaidInvoicesNotification { get; }

        bool ParseGeoCodeFile { get; }
        bool IsAddressAuditEnabled { get; }

        string TemplateRootPath { get; }
        string TemplateRootPathBin { get; }
        bool IsFromQA { get; }

        string ReviewPushApiKey { get; }

        string ReviewPushTaazaaFranchiseeMappingCronExpression { get; }
        bool IsReviewPushParseAllDataOn { get; }

        string ReviewPushGettingCustomerFeedbackCronExpression { get; }
        string AutoGenereatedMailForBestFitCronExpression { get; }
        string MailForNonResidentalBuildingTypeCronExpression { get; }
        bool AutoGeneratedMailForBestFitEnabled { get; }
        bool MailForNonResidentalBuildingTypeEnabled { get; }
        string BeforeAfterRecipientsCc { get; }
        string BeforeAfterRecipientsTo { get; }
        bool IsMapFranchiseeToFranchiseePhoneWithFranchiseeId { get; }
        bool LocalSiteGalleryEnabled { get; }
        string SiteRootUrlForAPI { get; }

        string CancellationMailForTechSalesNotificationCronExpression { get; }
        string SmtpEmailApiKey { get; }
        string HomeAdvisorCronExpression { get; }
        bool HomeAdvisorParsingIsDisabled { get; }
        string CCToMarketing { get; }
        bool NotificationToFADisabled { get; }
        string NotificationToFACronExpression { get; }
        DateTime ExpiringDateForFranchisee { get; }
        bool RenewableMailDisabled { get; }
        string MarketingEmail { get; }

        string BeforeAfterImageCronExpression { get; }
        bool BeforeAfterMigrationDisabled { get; }
        bool MailForFranchiseeRPIDEnabled { get; }
        string RpIdRecipients { get; }
        bool JobWeeklyMigration { get; }
        string apiKey { get; }
        string UpdationInvoiceCronExpression { get; }
        bool ParseUpdateInvoiceFile { get; }
        string SignatureUrl { get; }

        bool BeforeAfterImageParsing { get; }
        string UpdationInvoiceIdsCronExpression { get; }

        bool ParseUpdateInvoiceIdsData { get; }
        string AttachingInvoicesCronExpression { get; }
        string SalexTaxDataCronExpression { get; }

        string WebLeadsToEmail { get; }
        string UploadPriceEstimateDataCronExpression { get; }
        string FranchiseeAdminLink { get; }
        string KeyForCustomerReviewSendToMarketingSite { get; }
        string KeyValueForCustomerReviewSendToMarketingSite { get; }
        string KeyForBefoeAfterBestPairSendToMarketingSite { get; }
        string KeyValueForBefoeAfterBestPairSendToMarketingSite { get; }
        string RedirectToJobEstimation { get; }
        string RedirectToFollowUp { get; }
        string MarketingSiteWebSocketURL { get; }
        string BulkPhotouploadURL { get; }
        string CustomerFeedbackToMarketingSiteHeaderKey { get; }
        string CustomerFeedbackToMarketingSiteHeaderValue { get; }
        string CustomerFeedbackToMarketingSiteURL { get; }
    }
}
