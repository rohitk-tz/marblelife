using Core.Application;
using Core.Organizations.Impl;
using Core.Review.Impl;
using Core.Scheduler.Impl;
using Jobs.Impl;
using Quartz;
using Quartz.Impl;
using System;

namespace Jobs
{
    public class TaskScheduler : ITaskScheduler
    {
        private IScheduler _scheduler;
        private readonly ILogService _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
        public string Name
        {
            get { return GetType().Name; }
        }
        public void Run()
        {
            if (Environment.UserInteractive)
            {
                _logService.Info("Running TaskScheduler");

            }
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();

            _scheduler = schedulerFactory.GetScheduler();
            ScheduleJobs();
            _scheduler.Start();

        }
        public void Stop()
        {
            if (Environment.UserInteractive)
            {
                Console.WriteLine("Stopping task scheduler");
            }
            _scheduler.Shutdown();
        }

        private void ScheduleJobs()
        {
            var dateTimeService = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _logService.Info("Running ScheduleJobs");
            EmailNotification(dateTimeService);
            InvoiceGeneration(dateTimeService);
            SalesDataParser(dateTimeService);
            PaymentReminder(dateTimeService);
            SalesDataUploadReminder(dateTimeService);
            InvoiceLateFeeGenerator(dateTimeService);
            CurrencyExchangeRateGenerator(dateTimeService);
            CustomerFileParser(dateTimeService);
            WeeklyNotification(dateTimeService);
            SendCustomerFeedbackRequest(dateTimeService);
            GetCusomerFeedbackResponse(dateTimeService);
            CreateEmailRecordForAPI(dateTimeService);
            CustomerFeedbackNotification(dateTimeService);
            ServicedCustomerMonthlyNotification(dateTimeService);
            SyncedEmailAPIRecordNotification(dateTimeService);
            MarketingLead(dateTimeService);
            GetRoutingNumber(dateTimeService);
            UpdateConvertedLead(dateTimeService);
            UpdateGrowthReport(dateTimeService);
            UploadReportNotification(dateTimeService);
            BatchUploadRecord(dateTimeService);
            CalendarFileParser(dateTimeService);
            AnnualAuditFileParser(dateTimeService);
            InvoiceFileParser(dateTimeService);
            MarketingLeadDataUpdateParser(dateTimeService);
            CalculateLoanSchedule(dateTimeService);
            DocumentExpiryNotificationParser(dateTimeService);
            NewJobNotification(dateTimeService);
            JobNotificationToUser(dateTimeService);
            ZipFileParser(dateTimeService);
            MergeField(dateTimeService);
            ReviewPushSynService(dateTimeService);
            ReviewPushTaazaaFranchiseeMappingServie(dateTimeService);
            MailForNonResidentalBuildingType(dateTimeService);
            AutoGenereatedMailForBestFit(dateTimeService);
            CancellationMailForTechSalesNotification(dateTimeService);
            HomeAdvisorParser(dateTimeService);
            NotificationToFA(dateTimeService);
            BeforeAfterImagesParser(dateTimeService);
            UpdationInvoice(dateTimeService);
            UpdationInvoiceIds(dateTimeService);
            AttachingInvoices(dateTimeService);
            GetttingSalexTaxData(dateTimeService);
            UploadingPriceEstimateData(dateTimeService);
            EmailNotificationOnFranchiseePriceExceed(dateTimeService);
            EmailNotificationForPayrollReport(dateTimeService);
            BeforeAfterImagesUploadwithS3Bucket(dateTimeService);
            EmailNotificationForPhotoReport(dateTimeService);
            SyncS3BucketInEvery2Min(dateTimeService);
            CalendarImagesMigrationToNewApplication(dateTimeService);
        }

        private void DocumentExpiryNotificationParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<DocumentExpiryNotification>().WithIdentity("DocumentExpiryNotification").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.DocumentExpiryNotificationCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Document expiry Notification.");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }

        private void CalculateLoanSchedule(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail loanReschedule = JobBuilder.Create<CalculateLoanSchedule>().WithIdentity("CalculateLoanSchedule").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(loanReschedule)
                                                              .WithCronSchedule(settings.LoanReScheduleCalculatorCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Loan Re-Schedule calculator.");
            _scheduler.ScheduleJob(loanReschedule, jobTrigger);
        }
        private void MarketingLeadDataUpdateParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail updateLeadData = JobBuilder.Create<UpdateMarketingLeadData>().WithIdentity("UpdateMarketingLeadData").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(updateLeadData)
                                                              .WithCronSchedule(settings.MarketingLeadDataParserServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Update Marketing Lead Data Parser");
            _scheduler.ScheduleJob(updateLeadData, jobTrigger);
        }
        private void InvoiceFileParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail invoiceFileParser = JobBuilder.Create<InvoiceFileParser>().WithIdentity("InvoiceFileParser").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(invoiceFileParser)
                                                              .WithCronSchedule(settings.SalesDataParserServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Invoice File Parser");
            _scheduler.ScheduleJob(invoiceFileParser, jobTrigger);
        }
        private void AnnualAuditFileParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail annualDataParser = JobBuilder.Create<AnnualFileParser>().WithIdentity("AnnualFileParser").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(annualDataParser)
                                                              .WithCronSchedule(settings.SalesDataParserServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Annual Audit Data Parser");
            _scheduler.ScheduleJob(annualDataParser, jobTrigger);
        }
        private void CalendarFileParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail calendarParser = JobBuilder.Create<CalendarFileParser>().WithIdentity("CalendarFileParser").Build();
            ITrigger calendarParserTrigger = TriggerBuilder.Create()
                                                              .ForJob(calendarParser)
                                                              .WithCronSchedule(settings.CalendarParseServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Calendar file Parser Service.");
            _scheduler.ScheduleJob(calendarParser, calendarParserTrigger);
        }
        private void BatchUploadRecord(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail uploadRecord = JobBuilder.Create<BatchUploadRecord>().WithIdentity("BatchUploadRecord").Build();
            ITrigger uploadRecordTrigger = TriggerBuilder.Create()
                                                              .ForJob(uploadRecord)
                                                              .WithCronSchedule(settings.BatchUploadReportCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Upload record Notification Service.");
            _scheduler.ScheduleJob(uploadRecord, uploadRecordTrigger);
        }
        private void UploadReportNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail uploadReport = JobBuilder.Create<UploadReportNotification>().WithIdentity("UploadReportNotification").Build();
            ITrigger uploadReportTrigger = TriggerBuilder.Create()
                                                              .ForJob(uploadReport)
                                                              .WithCronSchedule(settings.BatchUploadNotificationCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running UploadReport Notification Service.");
            _scheduler.ScheduleJob(uploadReport, uploadReportTrigger);
        }
        private void UpdateGrowthReport(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail updateReport = JobBuilder.Create<UpdateGrowthReport>().WithIdentity("UpdateGrowthReport").Build();
            ITrigger updateReportTrigger = TriggerBuilder.Create()
                                                              .ForJob(updateReport)
                                                              .WithCronSchedule(settings.UpdateGrowthReportServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Update GrowthReport Service.");
            _scheduler.ScheduleJob(updateReport, updateReportTrigger);
        }
        private void UpdateConvertedLead(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail webLeadService = JobBuilder.Create<UpdateConvertedLead>().WithIdentity("UpdateConvertedLead").Build();
            ITrigger webLeadServiceServiceTrigger = TriggerBuilder.Create()
                                                              .ForJob(webLeadService)
                                                              .WithCronSchedule(settings.GetUpdateLeadServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Update Lead Service.");
            _scheduler.ScheduleJob(webLeadService, webLeadServiceServiceTrigger);
        }
        private void GetRoutingNumber(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail webLeadService = JobBuilder.Create<GetRoutingNumber>().WithIdentity("GetRoutingNumber").Build();
            ITrigger webLeadServiceServiceTrigger = TriggerBuilder.Create()
                                                              .ForJob(webLeadService)
                                                              .WithCronSchedule(settings.GetRoutingNumberCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running WebLead Service.");
            _scheduler.ScheduleJob(webLeadService, webLeadServiceServiceTrigger);
        }
        private void MarketingLead(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail marketingleadService = JobBuilder.Create<MarketingLead>().WithIdentity("MarketingLead").Build();
            ITrigger marketingleadServiceServiceTrigger = TriggerBuilder.Create()
                                                              .ForJob(marketingleadService)
                                                              .WithCronSchedule(settings.GetMarketingLeadCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running MarketingLead Service.");
            _scheduler.ScheduleJob(marketingleadService, marketingleadServiceServiceTrigger);
        }
        private void SyncedEmailAPIRecordNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail notificationService = JobBuilder.Create<SyncedEmailNotification>().WithIdentity("SyncedEmailNotification").Build();
            ITrigger notificationServiceTrigger = TriggerBuilder.Create()
                                                              .ForJob(notificationService)
                                                              .WithCronSchedule(settings.SendEmailListNotificationCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running MailChimp Notification Service.");
            _scheduler.ScheduleJob(notificationService, notificationServiceTrigger);
        }
        private void ServicedCustomerMonthlyNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail notificationService = JobBuilder.Create<ServicedCustomerNotification>().WithIdentity("ServicedCustomerNotification").Build();
            ITrigger notificationServiceTrigger = TriggerBuilder.Create()
                                                              .ForJob(notificationService)
                                                              .WithCronSchedule(settings.ServicedCustomerNotificationCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Serviced Customer Notification Service.");
            _scheduler.ScheduleJob(notificationService, notificationServiceTrigger);
        }
        private void CustomerFeedbackNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail notificationService = JobBuilder.Create<CustomerFeedbackNotification>().WithIdentity("CustomerFeedbackNotification").Build();
            ITrigger notificationServiceTrigger = TriggerBuilder.Create()
                                                              .ForJob(notificationService)
                                                              .WithCronSchedule(settings.SendFeedbackNotificationReportCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Customer Review System Notification Service.");
            _scheduler.ScheduleJob(notificationService, notificationServiceTrigger);
        }
        private void CreateEmailRecordForAPI(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail createEmailRecord = JobBuilder.Create<CreateEmailRecordOnAPI>().WithIdentity("CreateEmailRecord").Build();
            ITrigger createEmailRecordTrigger = TriggerBuilder.Create()
                                                              .ForJob(createEmailRecord)
                                                              .WithCronSchedule(settings.CreateEmailRecordCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Create Email Record.");
            _scheduler.ScheduleJob(createEmailRecord, createEmailRecordTrigger);
        }
        private void GetCusomerFeedbackResponse(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail getFeedback = JobBuilder.Create<GetCustomerFeedbackResponse>().WithIdentity("GetCustomerFeedbackResponse").Build();
            ITrigger getFeedbackTrigger = TriggerBuilder.Create()
                                                              .ForJob(getFeedback)
                                                              .WithCronSchedule(settings.GetCustomerFeedbackCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Get Customer Feedback.");
            _scheduler.ScheduleJob(getFeedback, getFeedbackTrigger);
        }
        private void SendCustomerFeedbackRequest(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail sendFeedback = JobBuilder.Create<SendCustomerFeedbackRequest>().WithIdentity("SendCustomerFeedbackRequest").Build();
            ITrigger sendFeedbackTrigger = TriggerBuilder.Create()
                                                              .ForJob(sendFeedback)
                                                              .WithCronSchedule(settings.SendCustomerFeedbackRequestCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Send Customer Feedback.");
            _scheduler.ScheduleJob(sendFeedback, sendFeedbackTrigger);
        }
        private void WeeklyNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail weeklyNotification = JobBuilder.Create<WeeklyNotification>().WithIdentity("WeeklyNotification").Build();
            ITrigger weeklyNotificationTrigger = TriggerBuilder.Create()
                                                              .ForJob(weeklyNotification)
                                                              .WithCronSchedule(settings.WeeklyNotificationCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Weekly Notification");
            _scheduler.ScheduleJob(weeklyNotification, weeklyNotificationTrigger);
        }
        private void PaymentReminder(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail paymentReminder = JobBuilder.Create<PaymentReminder>().WithIdentity("PaymentReminder").Build();
            ITrigger paymentReminderTrigger = TriggerBuilder.Create()
                                                              .ForJob(paymentReminder)
                                                              .WithCronSchedule(settings.PaymentReminderCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Payment Reminder");
            _scheduler.ScheduleJob(paymentReminder, paymentReminderTrigger);
        }
        private void SalesDataUploadReminder(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail salesDataUploadReminder = JobBuilder.Create<SalesDataUploadReminder>().WithIdentity("SalesDataUploadReminder").Build();
            ITrigger salesDataUploadReminderTrigger = TriggerBuilder.Create()
                                                              .ForJob(salesDataUploadReminder)
                                                              .WithCronSchedule(settings.SalesDataUploadReminderCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running SalesData Upload Reminder");
            _scheduler.ScheduleJob(salesDataUploadReminder, salesDataUploadReminderTrigger);
        }

        private void EmailNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail emailNotification = JobBuilder.Create<EmailNotification>().WithIdentity("EmailNotification").Build();
            ITrigger emailNotificationTrigger = TriggerBuilder.Create()
                                                              .ForJob(emailNotification)
                                                              .WithCronSchedule(settings.EmailNotificationServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running EmailNotification");
            _scheduler.ScheduleJob(emailNotification, emailNotificationTrigger);
        }

        private void InvoiceGeneration(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail invoiceGenerator = JobBuilder.Create<InvoiceGenerator>().WithIdentity("InvoiceGenerator").Build();
            ITrigger invoiceGenerationTrigger = TriggerBuilder.Create()
                                                              .ForJob(invoiceGenerator)
                                                              .WithCronSchedule(settings.InvoiceGenerationServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Invoice Generator");
            _scheduler.ScheduleJob(invoiceGenerator, invoiceGenerationTrigger);
        }

        private void InvoiceLateFeeGenerator(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail invoiceLateFeeGenerator = JobBuilder.Create<InvoiceLateFeeGenerator>().WithIdentity("InvoiceLateFeeGenerator").Build();
            ITrigger invoiceLateFeeGenerationTrigger = TriggerBuilder.Create()
                                                              .ForJob(invoiceLateFeeGenerator)
                                                              .WithCronSchedule(settings.InvoiceLateFeeGeneratorCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Invoice LateFee Generator");
            _scheduler.ScheduleJob(invoiceLateFeeGenerator, invoiceLateFeeGenerationTrigger);
        }
        private void SalesDataParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail salesDataParser = JobBuilder.Create<SalesDataParser>().WithIdentity("SalesDataParser").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(salesDataParser)
                                                              .WithCronSchedule(settings.SalesDataParserServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Sales Data Parser");
            _scheduler.ScheduleJob(salesDataParser, jobTrigger);
        }
        private void CurrencyExchangeRateGenerator(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail currencyExchangeRate = JobBuilder.Create<CurrencyExchangeRateGenerator>().WithIdentity("CurrencyExchangeRate").Build();
            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(currencyExchangeRate)
                                                              .WithCronSchedule(settings.CurrencyExchangeRateGeneratorCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Currency Exchange Rate");
            _scheduler.ScheduleJob(currencyExchangeRate, jobTrigger);
        }
        private void CustomerFileParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail customerFileParser = JobBuilder.Create<CustomerFileParser>().WithIdentity("CustomerFileParser").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(customerFileParser)
                                                              .WithCronSchedule(settings.CustomerFileParserServiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Customer File Parser");
            _scheduler.ScheduleJob(customerFileParser, jobTrigger);
        }

        private void NewJobNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail newJobNotification = JobBuilder.Create<NewJobNotification>().WithIdentity("NewJobNotification").Build();
            ITrigger newJobNotificationTrigger = TriggerBuilder.Create()
                                                              .ForJob(newJobNotification)
                                                              .WithCronSchedule(settings.JobNotificationCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running NewJobNotification");
            _scheduler.ScheduleJob(newJobNotification, newJobNotificationTrigger);
        }

        private void JobNotificationToUser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail newJobNotificationForUser = JobBuilder.Create<NewJobNotificationOnDay>().WithIdentity("JobNotificationToUser").Build();
            ITrigger newJobNotificationForUserTrigger = TriggerBuilder.Create()
                                                              .ForJob(newJobNotificationForUser)
                                                              .WithCronSchedule(settings.JobNotificationToUserCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running NewJobNotificationForUser");
            _scheduler.ScheduleJob(newJobNotificationForUser, newJobNotificationForUserTrigger);
        }
        private void ZipFileParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail zipFileParserForUser = JobBuilder.Create<ZipParserNotification>().WithIdentity("ZipFileParser").Build();
            ITrigger zeoCodeTrigger = TriggerBuilder.Create()
                                                              .ForJob(zipFileParserForUser)
                                                              .WithCronSchedule(settings.GeoParserCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Zip File Parser");
            _scheduler.ScheduleJob(zipFileParserForUser, zeoCodeTrigger);

        }
        private void MergeField(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<MergeFieldNotification>().WithIdentity("AddMerge").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.MergeFieldCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Merging Field Notification.");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }
        private void ReviewPushSynService(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<ReviewPushLocationAPI>().WithIdentity("ReviewPushLocalTionAPI").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.ReviewPushLocalTionAPICronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Review Push Location API.");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }
        private void ReviewPushTaazaaFranchiseeMappingServie(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<ReviewPushGettingCustomerFeedbackParser>().WithIdentity("ReviewPushGettingCustomerFeedbackParser").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.ReviewPushGettingCustomerFeedbackCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Google API Syncing...");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }


        private void AutoGenereatedMailForBestFit(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<AutoGenereatedMailForBestFit>().WithIdentity("AutoGenereatedMailForBestFit").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.AutoGenereatedMailForBestFitCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Auto Generated Mail For Best Fit.");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }
        private void MailForNonResidentalBuildingType(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<MailForNonResidentalBuildingType>().WithIdentity("MailForNonResidentalBuildingType").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.MailForNonResidentalBuildingTypeCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Mail For Before After Images For Franchisee Admin.");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }

        private void CancellationMailForTechSalesNotification(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<CancellationMailForTechSales>().WithIdentity("CancellationMailForTechSalesNotificationType").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.CancellationMailForTechSalesNotificationCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Mail For Auto Generated Cancellation Mail For Tech/Sales");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }

        private void HomeAdvisorParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<HomeAdvisorParserNotification>().WithIdentity("HomeAdvisorNotificationType").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.HomeAdvisorCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Mail For Auto Generated Cancellation Mail For Tech/Sales");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }

        private void NotificationToFA(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail documentExpiry = JobBuilder.Create<NotificationToFANotification>().WithIdentity("NotificationToFA").Build();

            ITrigger jobTrigger = TriggerBuilder.Create()
                                                              .ForJob(documentExpiry)
                                                              .WithCronSchedule(settings.NotificationToFACronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Mail For Notification To FA");
            _scheduler.ScheduleJob(documentExpiry, jobTrigger);
        }

        private void BeforeAfterImagesParser(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail beforeAfterImageParser = JobBuilder.Create<BeforeAfterImagesNotification>().WithIdentity("beforeAfterImageParser").Build();
            ITrigger zeoCodeTrigger = TriggerBuilder.Create()
                                                              .ForJob(beforeAfterImageParser)
                                                              .WithCronSchedule(settings.BeforeAfterImageCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Zip File Parser");
            _scheduler.ScheduleJob(beforeAfterImageParser, zeoCodeTrigger);

        }

        private void UpdationInvoice(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail beforeAfterImageParser = JobBuilder.Create<UpdatingInvoiceNotification>().WithIdentity("UpdatingInvoiceParser").Build();
            ITrigger zeoCodeTrigger = TriggerBuilder.Create()
                                                              .ForJob(beforeAfterImageParser)
                                                              .WithCronSchedule(settings.UpdationInvoiceCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Update Sales Data Parser");
            _scheduler.ScheduleJob(beforeAfterImageParser, zeoCodeTrigger);

        }

        private void UpdationInvoiceIds(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail beforeAfterImageParser = JobBuilder.Create<UpdatingInvoiceIdsNotification>().WithIdentity("UpdatingInvoiceIdsParser").Build();
            ITrigger zeoCodeTrigger = TriggerBuilder.Create()
                                                              .ForJob(beforeAfterImageParser)
                                                              .WithCronSchedule(settings.UpdationInvoiceIdsCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Update Sales Data Parser");
            _scheduler.ScheduleJob(beforeAfterImageParser, zeoCodeTrigger);

        }

        private void AttachingInvoices(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail beforeAfterImageParser = JobBuilder.Create<AttachingInvoices>().WithIdentity("AttachingInvoicesParser").Build();
            ITrigger zeoCodeTrigger = TriggerBuilder.Create()
                                                              .ForJob(beforeAfterImageParser)
                                                              .WithCronSchedule(settings.AttachingInvoicesCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Attaching Invoices Parser");
            _scheduler.ScheduleJob(beforeAfterImageParser, zeoCodeTrigger);

        }

        private void GetttingSalexTaxData(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail saleTaxParser = JobBuilder.Create<SalesTaxAPI>().WithIdentity("GetttingSalexTaxData").Build();
            ITrigger zeoCodeTrigger = TriggerBuilder.Create()
                                                              .ForJob(saleTaxParser)
                                                              .WithCronSchedule(settings.SalexTaxDataCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Attaching Invoices Parser");
            _scheduler.ScheduleJob(saleTaxParser, zeoCodeTrigger);

        }

        private void UploadingPriceEstimateData(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail uploadingPriceEstimateDataParser = JobBuilder.Create<UploadPriceEstimateData>().WithIdentity("UploadingPriceEstimateData").Build();
            ITrigger uploadingPriceEstimateDataTrigger = TriggerBuilder.Create()
                                                              .ForJob(uploadingPriceEstimateDataParser)
                                                              .WithCronSchedule(settings.UploadPriceEstimateDataCronExpression,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Upload Price Estimate Parser");
            _scheduler.ScheduleJob(uploadingPriceEstimateDataParser, uploadingPriceEstimateDataTrigger);

        }

        private void EmailNotificationOnFranchiseePriceExceed(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail emailNotificationOnFranchiseePriceExceed = JobBuilder.Create<EmailNotificationOnFranchiseePriceExceed>().WithIdentity("EmailNotificationOnFranchiseePriceExceed").Build();
            ITrigger emailNotificationOnFranchiseePriceExceedTrigger = TriggerBuilder.Create()
                                                              .ForJob(emailNotificationOnFranchiseePriceExceed)
                                                              .WithCronSchedule(settings.EmailNotificationOnFranchiseePriceExceed,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Email Notification On Franchisee price Exceed Parser");
            _scheduler.ScheduleJob(emailNotificationOnFranchiseePriceExceed, emailNotificationOnFranchiseePriceExceedTrigger);

        }

        private void EmailNotificationForPayrollReport(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail emailNotificationForPayrollReport = JobBuilder.Create<EmailNotificationForPayrollReport>().WithIdentity("EmailNotificationForPayrollReport").Build();
            ITrigger emailNotificationForPayrollReportTrigger = TriggerBuilder.Create()
                                                              .ForJob(emailNotificationForPayrollReport)
                                                              .WithCronSchedule(settings.EmailNotificationForPayrollReport,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Email Notification For Payroll Report");
            _scheduler.ScheduleJob(emailNotificationForPayrollReport, emailNotificationForPayrollReportTrigger);
        }

        private void BeforeAfterImagesUploadwithS3Bucket(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail beforeAfterImagesUploadwithS3Bucket = JobBuilder.Create<BeforeAfterImagesUploadwithS3Bucket>().WithIdentity("BeforeAfterImagesUploadwithS3Bucket").Build();
            ITrigger beforeAfterImagesUploadwithS3BucketTrigger = TriggerBuilder.Create()
                                                              .ForJob(beforeAfterImagesUploadwithS3Bucket)
                                                              .WithCronSchedule(settings.BeforeAfterImagesUploadwithS3Bucket,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Before/After Images Upload With S3 Bucket");
            _scheduler.ScheduleJob(beforeAfterImagesUploadwithS3Bucket, beforeAfterImagesUploadwithS3BucketTrigger);
        }

        private void EmailNotificationForPhotoReport(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail emailNotificationForPhotoReport = JobBuilder.Create<EmailNotificationForPhotoReport>().WithIdentity("EmailNotificationForPhotoReport").Build();
            ITrigger emailNotificationForPhotoReportTrigger = TriggerBuilder.Create()
                                                              .ForJob(emailNotificationForPhotoReport)
                                                              .WithCronSchedule(settings.EmailNotificationForPhotoReport,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Email Notification For Photo Report");
            _scheduler.ScheduleJob(emailNotificationForPhotoReport, emailNotificationForPhotoReportTrigger);
        }

        private void SyncS3BucketInEvery2Min(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail syncS3bucket = JobBuilder.Create<SyncS3bucket>().WithIdentity("SyncS3BucketInEvery2Min").Build();
            ITrigger syncS3bucketTrigger = TriggerBuilder.Create()
                                                              .ForJob(syncS3bucket)
                                                              .WithCronSchedule(settings.S3BucketSyncIn2Min,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Before/After Images Upload In Every 2 Min With S3 Bucket");
            _scheduler.ScheduleJob(syncS3bucket, syncS3bucketTrigger);
        }

        private void CalendarImagesMigrationToNewApplication(IClock dateTimeService)
        {
            var settings = ApplicationManager.DependencyInjection.Resolve<ISettings>();
            IJobDetail calendarImagesMigrationSync = JobBuilder.Create<CalendarImagesMigrationSync>().WithIdentity("CalendarImagesMigrationToNewApplication").Build();
            ITrigger calendarImagesMigrationSynctTrigger = TriggerBuilder.Create()
                                                              .ForJob(calendarImagesMigrationSync)
                                                              .WithCronSchedule(settings.CalendarImagesMigration,
                                                                                x =>
                                                                                x.InTimeZone(dateTimeService.CurrentTimeZone))
                                                              .Build();
            _logService.Info("Running Calendar Images Migration");
            _scheduler.ScheduleJob(calendarImagesMigrationSync, calendarImagesMigrationSynctTrigger);
        }
    }
}
