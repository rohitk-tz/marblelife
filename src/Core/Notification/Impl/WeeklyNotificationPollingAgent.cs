using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Reports;
using Core.Reports.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Core.Notification.Impl
{
    [DefaultImplementation]
    public class WeeklyNotificationPollingAgent : IWeeklyNotificationPollingAgent
    {
        private readonly ILogService _logService;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        private readonly IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly ISettings _settings;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<WeeklyNotification> _weeklyNotificationRepository;
        private IReportFactory _reportFactory;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IFileService _fileService;
        private readonly IReportService _reportService;

        public WeeklyNotificationPollingAgent(IUnitOfWork unitOfWork, ILogService logService, IClock clock, ISettings settings,
            IUserNotificationModelFactory userNotificationModelFactory, IReportFactory reportFcatory, IExcelFileCreator excelFileCreator,
            IFileService fileService, IReportService reportService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
            _clock = clock;
            _settings = settings;
            _userNotificationModelFactory = userNotificationModelFactory;
            _weeklyNotificationRepository = unitOfWork.Repository<WeeklyNotification>();
            _reportFactory = reportFcatory;
            _excelFileCreator = excelFileCreator;
            _fileService = fileService;
            _reportService = reportService;
        }

        public void CreateWeeklyNotification()
        {
            if (!ApplicationManager.Settings.SendWeeklyReminder)
            {
                _logService.Info("Weekly reminder turned off");
                return;
            }


            //Reminder for LateFee
            if (ApplicationManager.Settings.SendWeekyLateFeeNotification)
            {
                _logService.Info("Start Weekly lateFee Notification.");
                SalesDataLateFeeList();
                _logService.Info("End Weekly lateFee Notification.");
            }
            if (ApplicationManager.Settings.SendWeekyUnpaidInvoicesNotification)
            {
                _logService.Info("Start AR Report Notification.");
                ArReportList();
                _logService.Info("End AR Report Notification.");

            }
            if (ApplicationManager.Settings.SendWeekyUnpaidInvoicesNotification)
            {
                _logService.Info("Start Weekly Unpaid Invoice Notification.");
                UnpaidInvoiceList();
                _logService.Info("End Weekly Unpaid Invoice Notification.");
            }
        }
        private void ArReportList()
        {
            var currentWeekDate = _clock.UtcNow.Date;
            decimal totalAmount = 0;
            var notificationType = NotificationTypes.ArReportNotification;

            var dayOfWeek = Convert.ToInt32(currentWeekDate.DayOfWeek);
            if (dayOfWeek != _settings.WeeklyReminderDay)
            {
                currentWeekDate = GetWeekDay(currentWeekDate, _settings.WeeklyReminderDay);
            }

            var previousWeekDay = currentWeekDate.AddDays(-6);

            var weeklyNotificationForCurrentdate = _weeklyNotificationRepository.Table.Where(x => x.NotificationDate == currentWeekDate
                                       && x.NotificationTypeId == (long)notificationType);

            if (weeklyNotificationForCurrentdate.Any())
            {
                _logService.Info(string.Format("Reminder for date {0} has been send, can't send it again.", currentWeekDate));
                return;
            }

            try
            {
                _unitOfWork.StartTransaction();
                var unpaidInvoiceList = _franchiseeInvoiceRepository.Table.Where(x => x.Invoice.StatusId == (long)InvoiceStatus.Unpaid
                                             && x.Invoice.InvoiceItems.Sum(y => y.Amount) > 0).ToList();

                if (!unpaidInvoiceList.Any())
                {
                    _logService.Info(string.Format("No Unpaid Invoices for the period {0} - {1}", currentWeekDate, previousWeekDay));
                    return;
                }
                bool result;
                var franchiseeWiseInvoice = new List<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel>();
                var unpaidInvoiceAttachment = CreateAttachmentForArReport(unpaidInvoiceList, previousWeekDay, currentWeekDate,
                                               (long)NotificationTypes.WeeklyUnpaidInvoiceNotification, out result, out franchiseeWiseInvoice);


                if (result && unpaidInvoiceAttachment.Id > 0)
                {
                    totalAmount = franchiseeWiseInvoice.Sum(x => x.TotalInt);
                    _userNotificationModelFactory.CreateWeeklyNotificationForArReport(unpaidInvoiceAttachment, franchiseeWiseInvoice, previousWeekDay, currentWeekDate, notificationType, totalAmount);

                    var weeklyNotification = _reportFactory.CreateDomain(currentWeekDate, (long)notificationType);
                    _weeklyNotificationRepository.Save(weeklyNotification);
                }

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }

        private Application.Domain.File CreateAttachmentForArReport(IEnumerable<FranchiseeInvoice> franchiseeInvoices, DateTime startDate, DateTime endDate,
             long type, out bool result, out List<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> franchiseeWiseInvoice)
        {
            float onetoThirty = 0;
            float thirtyToSixty = 0;
            float sixtyToNinwty = 0;
            float morethanNinety = 0;
            string filePath, fileName;
            fileName = string.Empty;
            fileName = "list_Unpaid_invoices-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;

            var listInvoice2 = _reportService.GetArReportModel(franchiseeInvoices, startDate, endDate);

            var listInvoice = listInvoice2.OrderByDescending(x => x.TotalInt).ToList();
            franchiseeWiseInvoice = listInvoice;
            foreach (var value in franchiseeWiseInvoice)
            {
                float onetoThirtyFloat = float.Parse(value.Thirty.Split(' ')[1]);
                onetoThirty += onetoThirtyFloat;
                float thirtyToSixtyFloat = float.Parse(value.Sixty.Split(' ')[1]);
                thirtyToSixty += thirtyToSixtyFloat;
                float sixtyToNinwtyFloat = float.Parse(value.Ninety.Split(' ')[1]);
                sixtyToNinwty += sixtyToNinwtyFloat;
                float morethanNinetyFloat = float.Parse(value.moreThanNinety.Split(' ')[1]);
                morethanNinety += morethanNinetyFloat;
            }
            var model = new WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel
            {
                Franchisee = "Total",
                moreThanNinety = "$ " + morethanNinety,
                Ninety = "$ " + sixtyToNinwty,
                Sixty = "$ " + thirtyToSixty,
                Thirty = "$ " + onetoThirty,
                Total = "$ " + franchiseeWiseInvoice.Sum(x => x.TotalInt)
            };
            listInvoice.Add(model);
            result = _excelFileCreator.CreateExcelDocument(listInvoice, filePath);
            if (!result)
                throw new SystemException("Error : Creating Attachment!");
            var file = PrepareFileModel(fileName);
            var savedFile = _fileService.SaveModel(file);
            file.Id = savedFile.Id;

            return savedFile;
        }


        private void SalesDataLateFeeList()
        {
            var currentWeekDate = _clock.UtcNow.Date;
            var notificationType = NotificationTypes.WeeklyLateFeeNotification;

            var dayOfWeek = Convert.ToInt32(currentWeekDate.DayOfWeek);
            if (dayOfWeek != _settings.WeeklyReminderDay)
            {
                currentWeekDate = GetWeekDay(currentWeekDate, _settings.WeeklyReminderDay);
            }

            var previousWeekDay = currentWeekDate.AddDays(-6);

            var weeklyNotificationForCurrentdate = _weeklyNotificationRepository.Table.Where(x => x.NotificationDate == currentWeekDate
                                                    && x.NotificationTypeId == (long)notificationType);

            if (weeklyNotificationForCurrentdate.Any())
            {
                _logService.Info(string.Format("Reminder for date {0} has been send, can't send it again.", currentWeekDate));
                return;
            }

            try
            {
                _unitOfWork.StartTransaction();
                var franchiseeInvoiceList = _franchiseeInvoiceRepository.Table.Where(x => x.Invoice.InvoiceItems.Any
                                        (y => y.ItemTypeId == (long)InvoiceItemType.LateFees && y.LateFeeInvoiceItem != null
                                        && (y.LateFeeInvoiceItem.GeneratedOn != null && y.LateFeeInvoiceItem.GeneratedOn <= currentWeekDate))).ToList();

                if (!franchiseeInvoiceList.Any())
                {
                    _logService.Info(string.Format("No late Fee generated for the period {0} - {1}", currentWeekDate, previousWeekDay));
                    return;
                }
                bool result;
                var lateFeeAttachment = CreateAttachmentForLateFee(franchiseeInvoiceList, previousWeekDay, currentWeekDate,
                                        (long)NotificationTypes.WeeklyLateFeeNotification, out result);

                if (result && lateFeeAttachment.Id > 0)
                {
                    _userNotificationModelFactory.CreateWeeklyNotification(lateFeeAttachment, franchiseeInvoiceList, previousWeekDay, currentWeekDate, notificationType);

                    var weeklyNotification = _reportFactory.CreateDomain(currentWeekDate, (long)notificationType);
                    _weeklyNotificationRepository.Save(weeklyNotification);
                    _unitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }

        private Application.Domain.File CreateAttachmentForUnpaidInvoice(IEnumerable<FranchiseeInvoice> franchiseeInvoices, DateTime startDate, DateTime endDate,
             long type, out bool result)
        {
            var invoiceCollection = new List<WeeklyNotificationReportViewModel>();
            string filePath = string.Empty;
            var invoiceList = franchiseeInvoices.OrderByDescending(x => x.FranchiseeId).ThenByDescending(x => x.Invoice.GeneratedOn).ToList();

            foreach (var item in invoiceList)
            {
                var model = _reportFactory.CreateViewModelForNotification(item, startDate, endDate);
                invoiceCollection.Add(model);
            }
            var fileName = string.Empty;

            fileName = "list_Unpaid_invoices-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;

            var listInvoice = new List<WeeklyUnpaidInvoiceNotificationReportModel>();
            foreach (var invoice in invoiceCollection)
            {
                var model = CreateViewModelForNotification(invoice);
                listInvoice.Add(model);
            }
            result = _excelFileCreator.CreateExcelDocument(listInvoice, filePath);
            if (!result)
                throw new SystemException("Error : Creating Attachment!");
            var file = PrepareFileModel(fileName);
            var savedFile = _fileService.SaveModel(file);
            file.Id = savedFile.Id;
            return savedFile;
        }

        private Application.Domain.File CreateAttachmentForLateFee(IEnumerable<FranchiseeInvoice> franchiseeInvoices, DateTime startDate, DateTime endDate,
            long type, out bool result)
        {
            var invoiceCollection = new List<WeeklyNotificationReportViewModel>();
            string filePath = string.Empty;
            var invoiceList = franchiseeInvoices.OrderByDescending(x => x.FranchiseeId).ThenByDescending(x => x.Invoice.GeneratedOn).ToList();

            foreach (var item in invoiceList)
            {
                var model = _reportFactory.CreateViewModelForNotification(item, startDate, endDate);
                invoiceCollection.Add(model);
            }
            var fileName = string.Empty;

            fileName = "list_LateFee_invoices-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;
            result = _excelFileCreator.CreateExcelDocument(invoiceCollection, filePath);
            if (!result)
                throw new SystemException("Error : Creating Attachment!");
            var file = PrepareFileModel(fileName);
            var savedFile = _fileService.SaveModel(file);
            file.Id = savedFile.Id;
            return savedFile;
        }


        private FileModel PrepareFileModel(string name)
        {
            var fileModel = new FileModel();
            fileModel.Name = name;
            fileModel.Caption = name;
            fileModel.MimeType = "application/octet-binary";
            fileModel.RelativeLocation = MediaLocationHelper.GetAttachmentMediaLocation().Path;
            fileModel.Size = new FileInfo(fileModel.RelativeLocation + "/" + name).Length;
            return fileModel;
        }

        public static DateTime GetWeekDay(DateTime dt, int dayOfWeek)
        {
            int diff = Convert.ToInt32(dt.DayOfWeek) - dayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }


        private void UnpaidInvoiceList()
        {
            var currentWeekDate = _clock.UtcNow.Date;

            var notificationType = NotificationTypes.WeeklyUnpaidInvoiceNotification;

            var dayOfWeek = Convert.ToInt32(currentWeekDate.DayOfWeek);
            if (dayOfWeek != _settings.WeeklyReminderDay)
            {
                currentWeekDate = GetWeekDay(currentWeekDate, _settings.WeeklyReminderDay);
            }

            var previousWeekDay = currentWeekDate.AddDays(-6);

            var weeklyNotificationForCurrentdate = _weeklyNotificationRepository.Table.Where(x => x.NotificationDate == currentWeekDate
                                       && x.NotificationTypeId == (long)notificationType);

            if (weeklyNotificationForCurrentdate.Any())
            {
                _logService.Info(string.Format("Reminder for date {0} has been send, can't send it again.", currentWeekDate));
                return;
            }

            try
            {
                _unitOfWork.StartTransaction();
                var unpaidInvoiceList = _franchiseeInvoiceRepository.Table.Where(x => x.Invoice.StatusId == (long)InvoiceStatus.Unpaid
                                            && (x.Invoice.GeneratedOn <= currentWeekDate) && x.Invoice.InvoiceItems.Sum(y => y.Amount) > 0).ToList();

                if (!unpaidInvoiceList.Any())
                {
                    _logService.Info(string.Format("No Unpaid Invoices for the period {0} - {1}", currentWeekDate, previousWeekDay));
                    return;
                }
                bool result;
                var unpaidInvoiceAttachment = CreateAttachmentForUnpaidInvoice(unpaidInvoiceList, previousWeekDay, currentWeekDate,
                                               (long)NotificationTypes.WeeklyUnpaidInvoiceNotification, out result);

                if (result && unpaidInvoiceAttachment.Id > 0)
                {
                    _userNotificationModelFactory.CreateWeeklyNotification(unpaidInvoiceAttachment, unpaidInvoiceList, previousWeekDay, currentWeekDate, notificationType);

                    var weeklyNotification = _reportFactory.CreateDomain(currentWeekDate, (long)notificationType);
                    _weeklyNotificationRepository.Save(weeklyNotification);
                }

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Error(ex);
            }
        }
        private WeeklyUnpaidInvoiceNotificationReportModel CreateViewModelForNotification(WeeklyNotificationReportViewModel model)
        {
            var invoiceModel = new WeeklyUnpaidInvoiceNotificationReportModel
            {
                DueDate = model.DueDate,
                EndDate = model.EndDate.ToShortDateString(),
                Franchisee = model.Franchisee,
                InvoiceAmount = model.InvoiceAmount,
                InvoiceId = model.InvoiceId,
                LateFeeApplicable = model.LateFeeApplicable,
                PayableAmount = model.PayableAmount,
                StartDate = model.StartDate.ToShortDateString()
            };
            return invoiceModel;
        }
        private List<WeeklyNotificationReportViewModel> CreateDataForBatchRecord(List<WeeklyNotificationReportViewModel> model, DateTime startDateTime, DateTime endDateTime)
        {
            if (endDateTime == default(DateTime))
            {
                return model.Where(x => (x.StartDate <= startDateTime.Date && x.EndDate <= startDateTime.Date)).ToList();
            }
            else
            {
                return model.Where(x => ((x.StartDate >= startDateTime.Date && x.EndDate <= endDateTime.Date)
                                                                        || (x.StartDate <= startDateTime.Date && x.EndDate >= startDateTime.Date)
                                                                        //|| (x.StartDate <= endDateTime.Date && x.EndDate >= endDateTime.Date)
                                                                        )).ToList();
            }
        }

    }
}
