using Core.Application.Attribute;
using Core.Application.ValueType;
using System;
using System.Configuration;
using System.Globalization;

namespace Core.Application.Impl
{
    [DefaultImplementation]
    public class Settings : ISettings
    {
        public int PageSize
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]); }
        }

        public string DefaultRoyaltyAmount
        {
            get { return ConfigurationManager.AppSettings["DefaultRoyaltyAmount"]; }
        }
        public string MediaRootPath
        {
            get { return ConfigurationManager.AppSettings["MediaRootPath"]; }
        }
        public string MediaRootUrl
        {
            get { return ConfigurationManager.AppSettings["MediaRootUrl"]; }
        }

        public Dimension DimensionNormal
        {
            get
            {
                return Dimension("Thumb.Normal");
            }
        }
        public Dimension DimensionSmall
        {
            get { return Dimension("Thumb.Small"); }
        }
        public Dimension DimensionLarge
        {
            get { return Dimension("Thumb.Large"); }
        }

        static int ConvertInt32(string value, int returnValueOnNullOrEmpty = 0)
        {
            if (!string.IsNullOrEmpty(value))
            {
                int valueToReturn;
                if (int.TryParse(value, out valueToReturn)) return valueToReturn;
            }

            return returnValueOnNullOrEmpty;
        }

        private static Dimension Dimension(string thumbSize)
        {
            var configDimention = ConfigurationManager.AppSettings[thumbSize];
            var arrDimention = configDimention.Split('x');
            return new Dimension
            {
                Width = ConvertInt32(arrDimention[0]),
                Height = ConvertInt32(arrDimention[1])
            };
        }
        public string SiteRootUrl
        {
            get { return ConfigurationManager.AppSettings["SiteRootUrl"]; }
        }

        public string LogoImage { get { return ConfigurationManager.AppSettings["LogoImageUrl"]; } }

        public string DefaultTimeZone { get { return ConfigurationManager.AppSettings["DefaultTimeZone"]; } }
        public string SmtpHost { get { return ConfigurationManager.AppSettings["SmtpHost"]; } }
        public int SmtpPort { get { return Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]); } }
        public string SmtpUserName { get { return ConfigurationManager.AppSettings["SmtpUserName"]; } }
        public string SmtpPassword { get { return ConfigurationManager.AppSettings["SmtpPassword"]; } }

        public string CompanyName { get { return ConfigurationManager.AppSettings["CompanyName"]; } }
        public string RecipientEmail { get { return ConfigurationManager.AppSettings["RecipientEmail"]; } }
        public string ApplicationName { get { return ConfigurationManager.AppSettings["ApplicationName"]; } }
        public string SchedulingAppliation { get { return ConfigurationManager.AppSettings["SchedulingAppliation"]; } }
        public string OwnerName { get { return ConfigurationManager.AppSettings["OwnerName"]; } }
        public string OwnerEmail { get { return ConfigurationManager.AppSettings["OwnerEmail"]; } }
        public string Designation { get { return ConfigurationManager.AppSettings["Designation"]; } }
        public string OwnerPhone { get { return ConfigurationManager.AppSettings["OwnerPhone"]; } }

        public string FromNumber { get { return ConfigurationManager.AppSettings["FromNumber"]; } }
        public bool EnableSmsNotification { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSmsNotification"]); } }

        public string FromEmail { get { return ConfigurationManager.AppSettings["FromEmail"]; } }
        public string FranchiseeAdminLink { get { return (ConfigurationManager.AppSettings["FranchiseeAdminLink"]).ToString(); } }

        #region Cron Expression

        public string EmailNotificationServiceCronExpression { get { return ConfigurationManager.AppSettings["EmailNotificationServiceCronExpression"]; } }
        public string InvoiceGenerationServiceCronExpression { get { return ConfigurationManager.AppSettings["InvoiceGenerationServiceCronExpression"]; } }
        public string SalesDataParserServiceCronExpression { get { return ConfigurationManager.AppSettings["SalesDataParserServiceCronExpression"]; } }
        public string MarketingLeadDataParserServiceCronExpression { get { return ConfigurationManager.AppSettings["MarketingLeadDataParserServiceCronExpression"]; } }
        public string InvoiceLateFeeGeneratorCronExpression { get { return ConfigurationManager.AppSettings["InvoiceLateFeeGeneratorCronExpression"]; } }
        public string CurrencyExchangeRateGeneratorCronExpression { get { return ConfigurationManager.AppSettings["CurrencyExchangeRateGeneratorCronExpression"]; } }
        public string PaymentReminderCronExpression { get { return ConfigurationManager.AppSettings["PaymentReminderCronExpression"]; } }
        public string CustomerFileParserServiceCronExpression { get { return ConfigurationManager.AppSettings["CustomerFileParserServiceCronExpression"]; } }
        public string SalesDataUploadReminderCronExpression { get { return ConfigurationManager.AppSettings["SalesDataUploadReminderCronExpression"]; } }
        public string WeeklyNotificationCronExpression { get { return ConfigurationManager.AppSettings["WeeklyNotificationCronExpression"]; } }
        public string SendCustomerFeedbackRequestCronExpression { get { return ConfigurationManager.AppSettings["SendCustomerFeedbackRequestCronExpression"]; } }
        public string GetCustomerFeedbackCronExpression { get { return ConfigurationManager.AppSettings["GetCustomerFeedbackCronExpression"]; } }
        public string CreateEmailRecordCronExpression { get { return ConfigurationManager.AppSettings["CreateEmailRecordCronExpression"]; } }
        public string SendFeedbackNotificationReportCronExpression { get { return ConfigurationManager.AppSettings["SendFeedbackNotificationReportCronExpression"]; } }
        public string ServicedCustomerNotificationCronExpression { get { return ConfigurationManager.AppSettings["ServicedCustomerNotificationCronExpression"]; } }
        public string SendEmailListNotificationCronExpression { get { return ConfigurationManager.AppSettings["SendEmailListNotificationCronExpression"]; } }
        public string GetMarketingLeadCronExpression { get { return ConfigurationManager.AppSettings["GetMarketingLeadCronExpression"]; } }
        public string GetRoutingNumberCronExpression { get { return ConfigurationManager.AppSettings["GetRoutingNumberCronExpression"]; } }
        public string GetUpdateLeadServiceCronExpression { get { return ConfigurationManager.AppSettings["GetUpdateLeadServiceCronExpression"]; } }
        public string UpdateGrowthReportServiceCronExpression { get { return ConfigurationManager.AppSettings["UpdateGrowthReportServiceCronExpression"]; } }
        public string BatchUploadReportCronExpression { get { return ConfigurationManager.AppSettings["BatchUploadReportCronExpression"]; } }
        public string BatchUploadNotificationCronExpression { get { return ConfigurationManager.AppSettings["BatchUploadNotificationCronExpression"]; } }
        public string CalendarParseServiceCronExpression { get { return ConfigurationManager.AppSettings["CalendarParseServiceCronExpression"]; } }
        public string LoanReScheduleCalculatorCronExpression { get { return ConfigurationManager.AppSettings["LoanReScheduleCalculatorCronExpression"]; } }
        public string DocumentExpiryNotificationCronExpression { get { return ConfigurationManager.AppSettings["DocumentExpiryNotificationCronExpression"]; } }
        public string JobNotificationCronExpression { get { return ConfigurationManager.AppSettings["JobNotificationCronExpression"]; } }
        public string JobNotificationToUserCronExpression { get { return ConfigurationManager.AppSettings["JobNotificationToUserCronExpression"]; } }
        public string GeoParserCronExpression { get { return ConfigurationManager.AppSettings["GeoParserCronExpression"]; } }

        public string MergeFieldCronExpression { get { return ConfigurationManager.AppSettings["MergeFieldCronExpression"]; } }
        public string EmailNotificationOnFranchiseePriceExceed { get { return ConfigurationManager.AppSettings["EmailNotificationOnFranchiseePriceExceedCronExpression"]; } }
        public string ToSuperAdmin { get { return ConfigurationManager.AppSettings["EmailNotificationOnFranchiseePriceExceed"]; } }

        public string ReviewPushLocalTionAPICronExpression { get { return ConfigurationManager.AppSettings["ReviewPushLocalTionAPICronExpression"]; } }
        public string EmailNotificationForPayrollReport { get { return ConfigurationManager.AppSettings["EmailNotificationForPayrollReportCronExpression"]; } }
        public string BeforeAfterImagesUploadwithS3Bucket { get { return ConfigurationManager.AppSettings["BeforeAfterImagesUploadwithS3BucketCronExpression"]; } }
        public string EmailNotificationForPhotoReport { get { return ConfigurationManager.AppSettings["EmailNotificationForPhotoReportCronExpression"]; } }
        public string S3BucketSyncIn2Min { get { return ConfigurationManager.AppSettings["S3BucketSyncIn2MinCronExpression"]; } }
        public string CalendarImagesMigration { get { return ConfigurationManager.AppSettings["CalendarImagesMigrationCronExpression"]; } }
        #endregion

        public bool EnableSsl { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]); } }
        public int RecordCountForDashboard
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["RecordCountForDashboard"]); }
        }

        public bool AuthNetTestMode { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["AuthNetTestMode"]); } }

        public int PasswordLength
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["PasswordLength"]); }
        }

        public int PaymentReminderDayCount
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["PaymentReminderDayCount"]); }
        }
        public DateTime LateFeeStartDate
        {
            get { return DateTime.ParseExact(ConfigurationManager.AppSettings["LateFeeStartDate"], "dd/MM/yyyy", CultureInfo.InvariantCulture); }
        }
        public bool SendNotificationToFranchiser { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendNotificationToFranchiser"]); } }
        public string CCToAdmin { get { return (ConfigurationManager.AppSettings["CCToAdmin"]).ToString(); } }

        public string MailChimpReportRecipients { get { return (ConfigurationManager.AppSettings["MailChimpReportRecipients"]).ToString(); } }
        public string CCToReviewReportRecipients { get { return (ConfigurationManager.AppSettings["CCToReviewReportRecipients"]).ToString(); } }
        public string ListCustomerRecipients { get { return (ConfigurationManager.AppSettings["ListCustomerRecipients"]).ToString(); } }
        public string UnpainInvoiceRecipients { get { return (ConfigurationManager.AppSettings["UnpainInvoiceRecipients"]).ToString(); } }
        public string AuditRecipients { get { return (ConfigurationManager.AppSettings["AuditRecipients"]).ToString(); } }

        public int WeeklyReminderDayCount
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["WeeklyReminderDayCount"]); }
        }
        public int MonthlyReminderDayCount
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["MonthlyReminderDayCount"]); }
        }
        public decimal NationChargePercentage
        {
            get { return Convert.ToDecimal(ConfigurationManager.AppSettings["NationChargePercentage"]); }
        }
        public bool ApplyDateValidation { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ApplyDateValidation"]); } }
        public bool ApplyLateFee { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ApplyLateFee"]); } }
        public bool ApplyAddressAndPhoneValidation { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ApplyAddressAndPhoneValidation"]); } }

        public string CurrencyExchangeRateApi { get { return (ConfigurationManager.AppSettings["CurrencyExchangeRateApi"]).ToString(); } }
        public string CurrencyExchengeRateApiKey { get { return (ConfigurationManager.AppSettings["CurrencyExchengeRateApiKey"]).ToString(); } }


        public bool GetMergeField { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetMergeField"]); } }
        public bool GetCurrencyExchangeRate { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetCurrencyExchangeRate"]); } }
        public string ExchangeRateAppId { get { return (ConfigurationManager.AppSettings["ExchangeRateAppId"]).ToString(); } }

        public bool GetHistoricalRate { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetHistoricalRate"]); } }

        public int DefaultRoyaltyLateFeeWaitPeriod { get { return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultRoyaltyLateFeeWaitPeriod"]); } }
        public int DefaultSalesDataLateFeeWaitPeriod { get { return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultSalesDataLateFeeWaitPeriod"]); } }
        public bool SendWeeklyReminder { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendWeeklyReminder"]); } }

        public int WeeklyReminderDay { get { return Convert.ToInt32(ConfigurationManager.AppSettings["WeeklyReminderDay"]); } }

        #region Review System 

        public string ClientId { get { return (ConfigurationManager.AppSettings["ClientId"]).ToString(); } }
        public string ReviewApiKey { get { return (ConfigurationManager.AppSettings["ReviewApiKey"]).ToString(); } }
        public bool SendFeedbackEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendFeedbackEnabled"]); } }
        public bool GetFeedbackEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetFeedbackEnabled"]); } }
        public string KioskLink { get { return (ConfigurationManager.AppSettings["KioskLink"]).ToString(); } }

        #endregion

        #region EmailRecord API Info

        public bool CreateEmailRecord { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["CreateEmailRecord"]); } }
        public string EmailApiKey { get { return (ConfigurationManager.AppSettings["EmailApiKey"]).ToString(); } }
        public string RegionCode { get { return (ConfigurationManager.AppSettings["RegionCode"]).ToString(); } }
        public string EmailAPIListId { get { return (ConfigurationManager.AppSettings["EmailAPIListId"]).ToString(); } }
        public string EmailAPIListIdForPartialPayment { get { return (ConfigurationManager.AppSettings["EmailAPIListIdForPartialPayment"]).ToString(); } }
        public string MlDisturbutionId { get { return (ConfigurationManager.AppSettings["MlDisturbutionId"]).ToString(); } }

        #endregion
        public bool SendCustomerListNotification { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendCustomerListNotification"]); } }
        public bool SendMonthlyNotification { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendMonthlyNotification"]); } }

        #region Marketing Lead API Info

        public string AccessKey { get { return (ConfigurationManager.AppSettings["AccessKey"]).ToString(); } }
        public string SecretKey { get { return (ConfigurationManager.AppSettings["SecretKey"]).ToString(); } }
        public bool GetRoutingNumbers { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetRoutingNumbers"]); } }
        public bool GetCallDetails { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetCallDetails"]); } }
        public bool GetWebLeads { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetWebLeads"]); } }
        public string WebLeadsAPIkey { get { return (ConfigurationManager.AppSettings["WebLeadsAPIkey"]).ToString(); } }

        #endregion

        public bool UpdateAllSalesAmount { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateAllSalesAmount"]); } }
        public int DefaultMonthCountForMarketingLeads { get { return Convert.ToInt32(ConfigurationManager.AppSettings["DefaultMonthCountForMarketingLeads"]); } }
        public string FilePath { get { return (ConfigurationManager.AppSettings["FilePath"]).ToString(); } }
        public string CalendarFilePath { get { return (ConfigurationManager.AppSettings["CalendarFilePath"]).ToString(); } }

        public bool FeeProfileValidation { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["FeeProfileValidation"]); } }
        public bool UpdateInitialBatchRecord { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateInitialBatchRecord"]); } }

        #region MarketingLead
        public bool GetHistoryData { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["GetHistoryData"]); } }
        public DateTime CallDetailStartDate
        {
            get { return DateTime.ParseExact(ConfigurationManager.AppSettings["CallDetailStartDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture); }
        }
        public DateTime CallDetailEndDate
        {
            get { return DateTime.ParseExact(ConfigurationManager.AppSettings["CallDetailEndDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture); }
        }
        public DateTime WebLeadStartDate
        {
            get { return DateTime.ParseExact(ConfigurationManager.AppSettings["WebLeadStartDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture); }
        }
        public DateTime WebLeadEndDate
        {
            get { return DateTime.ParseExact(ConfigurationManager.AppSettings["WebLeadEndDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture); }
        }
        #endregion

        public bool UpdateMonthly
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateMonthly"]); }
        }

        public bool ParseCalendarFile { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ParseCalendarFile"]); } }

        public DateTime CurrencyRateStartDate
        {
            get { return DateTime.ParseExact(ConfigurationManager.AppSettings["CurrencyRateStartDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture); }
        }

        public bool UpdateInvoiceRecord { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateInvoiceRecord"]); } }

        public bool UpdateOldData { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateOldData"]); } }

        public decimal DefaultOneTimeProjectAmount { get { return Convert.ToDecimal(ConfigurationManager.AppSettings["DefaultOneTimeProjectAmount"]); } }
        public DateTime ZeroInvoiceStartDate { get { return DateTime.ParseExact(ConfigurationManager.AppSettings["ZeroInvoiceStartDate"], "MM/dd/yyyy", CultureInfo.InvariantCulture); } }
        public bool SendExpiryNotification { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendExpiryNotification"]); } }
        public string NationalUrlString { get { return (ConfigurationManager.AppSettings["NationalUrlString"]).ToString(); } }
        public bool NewJobNotificationToClient { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["NewJobNotificationToClient"]); } }
        public bool NewJobNotificationToTechAndSales { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["NewJobNotificationToTechAndSales"]); } }
        public int SEMIFranchiseeId { get { return Convert.ToInt32(ConfigurationManager.AppSettings["SEMIFranchiseeId"]); } }
        public string SEMIFromEmail { get { return (ConfigurationManager.AppSettings["SEMIFromEmail"]).ToString(); } }

        public bool SendWeekyLateFeeNotification { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendWeekyLateFeeNotification"]); } }
        public bool SendWeekyUnpaidInvoicesNotification { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["SendWeekyUnpaidInvoicesNotification"]); } }

        public bool ParseGeoCodeFile { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ParseGeoCodeFile"]); } }

        public bool IsAddressAuditEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsAddressAuditEnabled"]); } }

        public string TemplateRootPath { get { return (ConfigurationManager.AppSettings["TemplateRootPath"]).ToString(); } }

        public string TemplateRootPathBin { get { return (ConfigurationManager.AppSettings["TemplateRootPathBin"]).ToString(); } }

        public bool IsFromQA { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsFromQA"]); } }

        public string ReviewPushApiKey { get { return (ConfigurationManager.AppSettings["ReviewPushApiKey"]).ToString(); } }

        public string ReviewPushTaazaaFranchiseeMappingCronExpression { get { return (ConfigurationManager.AppSettings["ReviewPushTaazaaFranchiseeMappingCronExpression"]).ToString(); } }

        public bool IsReviewPushParseAllDataOn { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsReviewPushParseAllDataOn"]); } }

        public string ReviewPushGettingCustomerFeedbackCronExpression { get { return (ConfigurationManager.AppSettings["ReviewPushGettingCustomerFeedbackCronExpression"]).ToString(); } }


        public string MailForNonResidentalBuildingTypeCronExpression { get { return (ConfigurationManager.AppSettings["MailForNonResidentalBuildingTypeCronExpression"]).ToString(); } }

        public string AutoGenereatedMailForBestFitCronExpression { get { return (ConfigurationManager.AppSettings["AutoGenereatedMailForBestFitCronExpression"]).ToString(); } }

        public bool AutoGeneratedMailForBestFitEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["AutoGeneratedMailForBestFitEnabled"]); } }

        public bool MailForNonResidentalBuildingTypeEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["MailForNonResidentalBuildingTypeEnabled"]); } }

        public string BeforeAfterRecipientsCc { get { return (ConfigurationManager.AppSettings["BeforeAfterRecipientsCc"]).ToString(); } }
        public string BeforeAfterRecipientsTo { get { return (ConfigurationManager.AppSettings["BeforeAfterRecipientsTo"]).ToString(); } }

        public bool LocalSiteGalleryEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["LocalSiteGalleryEnabled"]); } }
        public string SiteRootUrlForAPI { get { return (ConfigurationManager.AppSettings["SiteRootUrlForAPI"]).ToString(); } }

        public string CancellationMailForTechSalesNotificationCronExpression { get { return (ConfigurationManager.AppSettings["CancellationMailForTechSalesNotificationCronExpression"]).ToString(); } }

        public bool IsMapFranchiseeToFranchiseePhoneWithFranchiseeId { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["IsMapFranchiseeToFranchiseePhoneWithFranchiseeId"]); } }

        public string SmtpEmailApiKey { get { return (ConfigurationManager.AppSettings["SmtpEmailApiKey"]).ToString(); } }

        public string HomeAdvisorCronExpression { get { return (ConfigurationManager.AppSettings["HomeAdvisorCronExpression"]).ToString(); } }

        public bool HomeAdvisorParsingIsDisabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["HomeAdvisorParsingIsDisabled"]); } }

        public string CCToMarketing
        {
            get { return (ConfigurationManager.AppSettings["CCToMarketing"]); }
        }

        public bool NotificationToFADisabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["NotificationToFADisabled"]); } }
        public string NotificationToFACronExpression { get { return (ConfigurationManager.AppSettings["NotificationToFACronExpression"]).ToString(); } }

        public DateTime ExpiringDateForFranchisee { get { return DateTime.ParseExact(ConfigurationManager.AppSettings["ExpiringDateForFranchisee"], "MM/dd/yyyy", CultureInfo.InvariantCulture); } }

        public bool RenewableMailDisabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["RenewableMailDisabled"]); } }

        public string MarketingEmail { get { return (ConfigurationManager.AppSettings["MarketingEmail"]).ToString(); } }

        public string BeforeAfterImageCronExpression { get { return (ConfigurationManager.AppSettings["BeforeAfterImageCronExpression"]).ToString(); } }

        public bool BeforeAfterMigrationDisabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["BeforeAfterMigrationDisabled"]); } }

        public bool MailForFranchiseeRPIDEnabled { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["MailForFranchiseeRPIDEnabled"]); } }

        public string RpIdRecipients
        {
            get
            {
                return (ConfigurationManager.AppSettings["RpIdRecipients"]).ToString();
            }
        }

        public bool JobWeeklyMigration { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["JobWeeklyMigration"]); } }

        public string AccessKeyV2 { get { return (ConfigurationManager.AppSettings["AccessKeyV2"]).ToString(); } }
        public string SecretKeyV2 { get { return (ConfigurationManager.AppSettings["SecretKeyV2"]).ToString(); } }
        public string apiKey { get { return (ConfigurationManager.AppSettings["apiKey"]).ToString(); } }

        public string UpdationInvoiceCronExpression { get { return (ConfigurationManager.AppSettings["UpdationInvoiceCronExpression"]).ToString(); } }

        public bool ParseUpdateInvoiceFile { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ParseUpdateInvoiceFile"]); } }

        public string SignatureUrl { get { return (ConfigurationManager.AppSettings["SignatureUrl"]).ToString(); } }

        public bool BeforeAfterImageParsing { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["BeforeAfterImageParsing"]); } }

        public string UpdationInvoiceIdsCronExpression { get { return (ConfigurationManager.AppSettings["UpdationInvoiceIdsCronExpression"]).ToString(); } }

        public bool ParseUpdateInvoiceIdsData { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["ParseUpdateInvoiceIdsData"]); } }

        public string AttachingInvoicesCronExpression { get { return (ConfigurationManager.AppSettings["AttachingInvoicesCronExpression"]).ToString(); } }
        public string WebLeadsToEmail { get { return (ConfigurationManager.AppSettings["WebLeadsToEmail"]).ToString(); } }

        public string SalexTaxDataCronExpression { get { return (ConfigurationManager.AppSettings["SalexTaxDataCronExpression"]).ToString(); } }
        public string UploadPriceEstimateDataCronExpression { get { return (ConfigurationManager.AppSettings["UploadPriceEstimateDataCronExpression"]).ToString(); } }
        public string AWSSecreatKey { get { return (ConfigurationManager.AppSettings["AWSSecreatKey"]).ToString(); } }
        public string AWSAccessKey { get { return (ConfigurationManager.AppSettings["AWSAccessKey"]).ToString(); } }
        public string AWSBucketName { get { return (ConfigurationManager.AppSettings["AWSBucketName"]).ToString(); } }
        public string AWSBucketURL { get { return (ConfigurationManager.AppSettings["AWSBucketURL"]).ToString(); } }
        public string AWSBucketThumbURL { get { return (ConfigurationManager.AppSettings["AWSBucketThumbURL"]).ToString(); } }
        public string KeyForCustomerReviewSendToMarketingSite { get { return (ConfigurationManager.AppSettings["KeyForCustomerReviewSendToMarketingSite"]).ToString(); } }
        public string KeyValueForCustomerReviewSendToMarketingSite { get { return (ConfigurationManager.AppSettings["KeyValueForCustomerReviewSendToMarketingSite"]).ToString(); } }
        public string KeyForBefoeAfterBestPairSendToMarketingSite { get { return (ConfigurationManager.AppSettings["KeyForBefoeAfterBestPairSendToMarketingSite"]).ToString(); } }
        public string KeyValueForBefoeAfterBestPairSendToMarketingSite { get { return (ConfigurationManager.AppSettings["KeyValueForBefoeAfterBestPairSendToMarketingSite"]).ToString(); } }
        public string RedirectToJobEstimation { get { return (ConfigurationManager.AppSettings["RedirectToJobEstimation"]).ToString(); } }
        public string RedirectToFollowUp { get { return (ConfigurationManager.AppSettings["RedirectToFollowUp"]).ToString(); } }
        public string MarketingSiteWebSocketURL { get { return (ConfigurationManager.AppSettings["MarketingSiteWebSocketURL"]).ToString(); } }
        public string BulkPhotouploadURL { get { return (ConfigurationManager.AppSettings["BulkPhotouploadURL"]).ToString(); } }
        public string CustomerFeedbackToMarketingSiteHeaderKey { get { return (ConfigurationManager.AppSettings["CustomerFeedbackToMarketingSiteHeaderKey"]).ToString(); } }
        public string CustomerFeedbackToMarketingSiteHeaderValue { get { return (ConfigurationManager.AppSettings["CustomerFeedbackToMarketingSiteHeaderValue"]).ToString(); } }
        public string CustomerFeedbackToMarketingSiteURL { get { return (ConfigurationManager.AppSettings["CustomerFeedbackToMarketingSiteURL"]).ToString(); } }
    }
}
