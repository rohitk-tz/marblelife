using Core.Application;
using Core.Billing;
using Core.Billing.Impl;
using Core.MarketingLead;
using Core.Notification;
using Core.Notification.Impl;
using Core.Organizations;
using Core.Reports;
using Core.Reports.ViewModel;
using Core.Review;
using Core.Sales;
using Core.Sales.Impl;
using Core.Scheduler;
using Core.Scheduler.Impl;
using DependencyInjection;
using Infrastructure.Billing;
using Jobs.Impl;
using System;
using System.Net;
using System.ServiceProcess;

namespace Jobs
{
    static class Program
    {
        static void Main(string[] args)
        {
            GC.Collect();
            DependencyRegistrar.RegisterDependencies();
            ApplicationManager.DependencyInjection.Register<IAppContextStore, AppContextStore>();
            ApplicationManager.DependencyInjection.Register<ISessionContext, WinJobSessionContext>();
            DependencyRegistrar.SetupCurrentContextWinJob();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var logger = ApplicationManager.DependencyInjection.Resolve<ILogService>();

            if (Environment.UserInteractive)
            {
                ExecuteServices(logger);
            }
            else
            {
                var servicesToRun = new ServiceBase[]
                    {
                      new Scheduler(),
                    };
                ServiceBase.Run(servicesToRun);
            }
        }

        private static void ExecuteServices(ILogService logger)
        {
            logger.Info("Starting services");
            try
            {
                //var notificationPollingAgent = ApplicationManager.DependencyInjection.Resolve<INotificationPollingAgent>();
                //notificationPollingAgent.PollForNotifications();

                //var currencyRatePollingAgent = ApplicationManager.DependencyInjection.Resolve<ICurrencyRateService>();
                //currencyRatePollingAgent.AllCurrencyRateByDate();

                //job for manage uploads- weekly monthly
                //var pollingAgent = ApplicationManager.DependencyInjection.Resolve<SalesDataParsePollingAgent>();
                //pollingAgent.ParseFile();

                //var invoiceParser = ApplicationManager.DependencyInjection.Resolve<FranchiseeInvoiceGenerationPollingAgent>();
                //invoiceParser.ProcessRecords();

                //var invoicelatefee = ApplicationManager.DependencyInjection.Resolve<Core.Billing.IInvoiceLateFeePollingAgent>();
                //invoicelatefee.LateFeeGenerator();

                //var customerFileParsePollingAgent = ApplicationManager.DependencyInjection.Resolve<ICustomerFileUploadPollingAgent>();
                //customerFileParsePollingAgent.ParseCustomerFile();

                //var paymentReminderPollingAgent = ApplicationManager.DependencyInjection.Resolve<IPaymentReminderPollingAgent>();
                //paymentReminderPollingAgent.CreateNotificationReminderForPayment();

                //var salesDataUploadPollingAgent = ApplicationManager.DependencyInjection.Resolve<ISalesDataUploadReminderPollingAgent>();
                //salesDataUploadPollingAgent.CreateNotificationReminderForSalesDataUpload();

                //var weeklyNotificationPollingAgent = ApplicationManager.DependencyInjection.Resolve<IWeeklyNotificationPollingAgent>();
                //weeklyNotificationPollingAgent.CreateWeeklyNotification();

                //var sendFeedbackRequest = ApplicationManager.DependencyInjection.Resolve<ISendFeedBackRequestPollingAgent>();
                //sendFeedbackRequest.SendFeedback();

                //var getFeedbackResponse = ApplicationManager.DependencyInjection.Resolve<IGetCustomerFeedbackService>();
                //getFeedbackResponse.GetFeedbackResponse();

                //var createEmailRecord = ApplicationManager.DependencyInjection.Resolve<ICreateEmailRecordForApiService>();
                //createEmailRecord.CreateEmailRecord();

                //var mergeRecord = ApplicationManager.DependencyInjection.Resolve<ICreateMergeRecordForApiService>();
                //mergeRecord.AddMergeField();

                //var sendServicedCustomerListNotification = ApplicationManager.DependencyInjection.Resolve<ISendCustomerListNotificationService>();
                //sendServicedCustomerListNotification.CreateNotification();

                //var reviewNotification = ApplicationManager.DependencyInjection.Resolve<IMonthlyReviewNotificationService>();
                //reviewNotification.CreateNotification();

                //var emailNotification = ApplicationManager.DependencyInjection.Resolve<IEmailAPIIntegrationNotificationService>();
                //emailNotification.CreateNotification();

                //For webLeadData
                //var marketingLeads = ApplicationManager.DependencyInjection.Resolve<IMarketingLeadsService>();
                //marketingLeads.GetMarketingLeads();

                //var getRoutingNumbers = ApplicationManager.DependencyInjection.Resolve<IGetRoutingNumberService>();
                //getRoutingNumbers.GetRoutingNumber();

                //var updateConvertedLeads = ApplicationManager.DependencyInjection.Resolve<IUpdateConvertedLeadsService>();
                //updateConvertedLeads.UpdateLeads();

                //var updateSalesAmount = ApplicationManager.DependencyInjection.Resolve<IUpdateSalesAmountService>();
                //updateSalesAmount.UpdateData();

                //var updateBatch = ApplicationManager.DependencyInjection.Resolve<IUpdateBatchUploadRecordService>();
                //updateBatch.UpdateData();

                //var uploadNotification = ApplicationManager.DependencyInjection.Resolve<ISalesDataUploadReportNotificationService>();
                //uploadNotification.CreateNotification();

                //var calendarPollingAgent = ApplicationManager.DependencyInjection.Resolve<CalendarParsePollingAgent>();
                //calendarPollingAgent.ParseCalendarFile();

                //var annualReportPollingAgent = ApplicationManager.DependencyInjection.Resolve<AnnualSalesDataParsePollingAgent>();
                //annualReportPollingAgent.ParseFile();

                //var invoiceFileParser = ApplicationManager.DependencyInjection.Resolve<InvoiceItemUpdateInfoService>();
                //invoiceFileParser.UpdateReport();

                //var updateCallDetails = ApplicationManager.DependencyInjection.Resolve<IUpdateMarketingLeadReportDataService>();
                //updateCallDetails.UpdateData();

                //var calculateLoanSchedule = ApplicationManager.DependencyInjection.Resolve<ICalculateLoanScheduleService>();
                //calculateLoanSchedule.CalculateSchedule();

                //var documentNotificationPollingAgent = ApplicationManager.DependencyInjection.Resolve<IDocumentNotificationPollingAgent>();
                //documentNotificationPollingAgent.SendExpiryNotification();

                // This job is for Sending Notification to Client One day before the job time as Email
                //var newJobemailNotificationOnSameDay = ApplicationManager.DependencyInjection.Resolve<IJobReminderNotificationService>();
                //newJobemailNotificationOnSameDay.CreateNotification();

                // This job is for Sending Notification to Tech or Sales One day before the job time as Email
                //var newJobemailNotification = ApplicationManager.DependencyInjection.Resolve<IJobReminderNotificationtoUsersService>();
                //newJobemailNotification.CreateNotification();

                //var zipParser = ApplicationManager.DependencyInjection.Resolve<IZipParserNotificationService>();
                //zipParser.ProcessRecords();

                //var reviewPushApi = ApplicationManager.DependencyInjection.Resolve<IReviewPushLocationAPI>();
                //reviewPushApi.ProcessRecord();

                //var reviewPushGettingCustomerFeedback = ApplicationManager.DependencyInjection.Resolve<IReviewPushGettingCustomerFeedback>();
                //reviewPushGettingCustomerFeedback.ProcessRecords();

                //Generate Image for LocalMarketing
                //var autoGeneratedMailForBestFit = ApplicationManager.DependencyInjection.Resolve<IAutoGenereatedMailForBestFitNotification>();
                //autoGeneratedMailForBestFit.ProcessRecords();

                //var mailForNonResidentalBuildingType = ApplicationManager.DependencyInjection.Resolve<IMailForNonResidentalBuildingTypeNotification>();
                //mailForNonResidentalBuildingType.ProcessRecords();

                //var cancellationMailNotification = ApplicationManager.DependencyInjection.Resolve<ICancellationMailForTechSalesNotification>();
                //cancellationMailNotification.ProcessRecords();

                //var homeAdvisorParser = ApplicationManager.DependencyInjection.Resolve<IHomeAdvisorParser>();
                //homeAdvisorParser.ProcessRecords();

                //var notificationTOFA = ApplicationManager.DependencyInjection.Resolve<INotificationToFA>();
                //notificationTOFA.ProcessRecords();

                //var beforeAfterImages = ApplicationManager.DependencyInjection.Resolve<IBeforeAfterImagesNotificationServices>();
                //beforeAfterImages.ProcessRecords();

                //var updateSalesData = ApplicationManager.DependencyInjection.Resolve<IUpdatingInvoiceNotificationServices>();
                //updateSalesData.ProcessRecords();

                //var UpdateInvoiceIds = ApplicationManager.DependencyInjection.Resolve<IUpdatingInvoiceIdsNotificationServices>();
                //UpdateInvoiceIds.UpdateInvoiceIds();

                //var AttachInvoices = ApplicationManager.DependencyInjection.Resolve<IAttachingInvoicesServices>();
                //AttachInvoices.AttachInvoice();

                //var salesTaxApi = ApplicationManager.DependencyInjection.Resolve<ISalesTaxAPIServices>();
                //salesTaxApi.GetSalesTaxAPI();

                //Price estimate file upload job
                //var priceEstimateParser = ApplicationManager.DependencyInjection.Resolve<IPriceEstimateParserNotificationService>();
                //priceEstimateParser.ProcessRecords();

                //Notification Email On Franchisee Price Exceed by 50%
                //var emailNotificationOnFranchiseePriceExceed = ApplicationManager.DependencyInjection.Resolve<IEmailNotificationOnFranchiseePriceExceed>();
                //emailNotificationOnFranchiseePriceExceed.NotificationOnFranchiseePriceExceed();

                //Notification Email for Payroll Report
                //var emailNotificationForPayRollReport = ApplicationManager.DependencyInjection.Resolve<IEmailNotificationForPayrollReport>();
                //emailNotificationForPayRollReport.SendEmailNotificationForPayrollReport();

                //Sync Before/After Images with S3 Bucket
                //var beforeAfterImageswithS3Bucket = ApplicationManager.DependencyInjection.Resolve<IBeforeAfterImagesUploadwithS3Bucket>();
                //beforeAfterImageswithS3Bucket.UploadBeforeAfterImageswithS3Bucket();

                //var emailNotificationForPhotoReport = ApplicationManager.DependencyInjection.Resolve<IEmailNotificationForPhotoReport>();
                //emailNotificationForPhotoReport.SendEmailNotificationForPhotoReport();

                //var S3BucketSyncInEvery2min = ApplicationManager.DependencyInjection.Resolve<IS3BucketSync>();
                //S3BucketSyncInEvery2min.S3BucketSyncInEvery2min();

                //var calendarImagesMigration = ApplicationManager.DependencyInjection.Resolve<ICalendarImagesMigration>();
                //calendarImagesMigration.CalendarImagesMigrationToNewApplication();
            }

            catch (Exception exception)
            {
                logger.Error("Starting services", exception);
            }


            logger.Info("Press enter key to terminate");
            Console.ReadLine();
        }
    }
}
