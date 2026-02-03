using Core.Application;
using Core.Application.Attribute;
using Core.Application.Enum;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Reports.Domain;
using Core.Review.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class EmailAPIIntegrationNotificationService : IEmailAPIIntegrationNotificationService
    {
        private readonly ILogService _logService;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<CustomerEmailAPIRecord> _customerEmailAPIRecordRepository;
        private readonly ICustomerEmailAPIRecordFactory _customerEmailAPIRecordFactory;
        private readonly IFileService _fileService;
        private readonly IRepository<PartialPaymentEmailApiRecord> _partialcustomerEmailAPIRecordRepository;
        public EmailAPIIntegrationNotificationService(IUnitOfWork unitOfWork, ILogService logService, IExcelFileCreator excelFileCreator, IClock clock,
            IUserNotificationModelFactory userNotificationModelFactory, IFileService fileService, ICustomerEmailAPIRecordFactory customerEmailAPIRecordFactory)
        {
            _logService = logService;
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _unitOfWork = unitOfWork;
            _userNotificationModelFactory = userNotificationModelFactory;
            _customerEmailAPIRecordRepository = unitOfWork.Repository<CustomerEmailAPIRecord>();
            _customerEmailAPIRecordFactory = customerEmailAPIRecordFactory;
            _fileService = fileService;
            _partialcustomerEmailAPIRecordRepository = unitOfWork.Repository<PartialPaymentEmailApiRecord>();
        }
        public void CreateNotification()
        {
            if (ApplicationManager.Settings.SendMonthlyNotification == false)
            {
                _logService.Info("Email list Notification turned off!");
                return;
            }

            _logService.Info("Start Monthly MalChimp Integration List Notification.");

            SendNotification();

            _logService.Info("End Monthly MalChimp Integration List Notification.");
        }

        private void SendNotification()
        {
            bool result;
            var date = _clock.UtcNow;
            var previousMonth = date.Month - 1;
            var startDate = new DateTime(date.Year, previousMonth, 1);
            var endDate = new DateTime(date.Year, previousMonth, DateTime.DaysInMonth(date.Year, previousMonth));
           
            _unitOfWork.StartTransaction();

            var mails = _partialcustomerEmailAPIRecordRepository.Table.Where(x => (x.IsSynced) && !(x.IsFailed) && x.statusId == (long)LookupTypes.PartialPayment).ToList();
            var file = CreateMailListFile(startDate, endDate, out result, mails, "-PartialPayment-");
            mails = _partialcustomerEmailAPIRecordRepository.Table.Where(x => (x.IsSynced) && !(x.IsFailed) && x.statusId == (long)LookupTypes.Paid).ToList();
            var fullpaymentFile = CreateMailListFile(startDate, endDate, out result, mails, "-FullPayment-");

            if (result && file.Id > 0)
            {
                _userNotificationModelFactory.CreateMonthlyNotificationModel(file, startDate, endDate, NotificationTypes.MonthlyMailChimpReport, fullpaymentFile);
            }

            //if (result && file.Id > 0)
            //{
            //    _userNotificationModelFactory.CreateMonthlyNotificationModel(fullpaymentFile, startDate, endDate, NotificationTypes.MonthlyMailChimpReport);
            //}

            _unitOfWork.SaveChanges();
        }

        private Application.Domain.File CreateMailListFile(DateTime startDate, DateTime endDate, out bool result, List<PartialPaymentEmailApiRecord> mails, string fileNames)
        {
            result = false;
            string filePath = string.Empty;

            var mailList = new List<EmailAPINotificationModel>();
            var mailLists = mails.Where(x => x.IsSynced && x.DateCreated >= startDate && x.DateCreated <= endDate).ToList();

            foreach (var item in mailLists)
            {
                var model = _customerEmailAPIRecordFactory.CreateNotificationModel(item);
                mailList.Add(model);
            }

            var fileName = fileNames + string.Format("{0:yyyy-MM-dd}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;
            result = _excelFileCreator.CreateExcelDocument(mailList, filePath);
            if (!result)
                throw new SystemException("Error : Creating File Email API Record!");

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
    }
}
