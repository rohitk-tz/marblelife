using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Sales;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class SalesDataUploadReportNotificationService : ISalesDataUploadReportNotificationService
    {
        private readonly ILogService _logService;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<BatchUploadRecord> _batchUploadRecordRepository;
        private readonly IReportFactory _reportFactory;
        private readonly IFileService _fileService;
        private readonly IDownloadFileHelperService _downloadFileHelperService;
        public SalesDataUploadReportNotificationService(IUnitOfWork unitOfWork, ILogService logService, IExcelFileCreator excelFileCreator, IClock clock,
            IUserNotificationModelFactory userNotificationModelFactory, IReportFactory reportFactory, IFileService fileService, IDownloadFileHelperService downloadFileHelperService)
        {
            _logService = logService;
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _unitOfWork = unitOfWork;
            _userNotificationModelFactory = userNotificationModelFactory;
            _batchUploadRecordRepository = unitOfWork.Repository<BatchUploadRecord>();
            _reportFactory = reportFactory;
            _fileService = fileService;
            _downloadFileHelperService = downloadFileHelperService;
        }
        public void CreateNotification()
        {
            if (ApplicationManager.Settings.SendMonthlyNotification == false)
            {
                _logService.Info("Monthly Notification turned off!");
                return;
            }

            _logService.Info("Start Monthly SalesData Upload Report Notification.");

            CreateNotificationModel();

            _logService.Info("End Monthly SalesData Upload Report Notification.");
        }

        private void CreateNotificationModel()
        {
            bool result;
            var date = _clock.UtcNow;
            var previousMonth = date.AddMonths(-1);
            var startDate = new DateTime(previousMonth.Year, previousMonth.Month, 1);
            var endDate = new DateTime(previousMonth.Year, previousMonth.Month, DateTime.DaysInMonth(date.Year, previousMonth.Month));
            var file = CreateFile(startDate, endDate, out result);
            if (result && file.Id > 0)
            {
                _unitOfWork.StartTransaction();
                _userNotificationModelFactory.CreateSalesUploadNotification(file, startDate, endDate);
                _unitOfWork.SaveChanges();
            }
        }

        private Application.Domain.File CreateFile(DateTime startDate, DateTime endDate, out bool result)
        {
            result = false;
            string filePath = string.Empty;
            var ds = new DataSet();
            var collection = new List<UploadBatchCollectionViewModel>();
            var last30DaysModel = new UploadBatchCollectionViewModel();
            var last31to60DaysModel = new UploadBatchCollectionViewModel();
            var oldestModel = new UploadBatchCollectionViewModel();
            DateTime currentUtc = _clock.UtcNow.AddDays(-30);
            DateTime oldUtc = _clock.UtcNow;
            var last30Days = _downloadFileHelperService.CreateDataForBatchRecord(currentUtc, oldUtc);
            foreach (var item in last30Days)
            {
                last30DaysModel = _reportFactory.CreateViewModel(item);
                collection.Add(last30DaysModel);
            }
            ds.Tables.Add(_excelFileCreator.ListToDataTable(collection, "Last 30 Days"));
            collection.Clear();
            currentUtc = currentUtc.AddDays(-30);
            oldUtc = oldUtc.AddDays(-31);
            var last31To60Days = _downloadFileHelperService.CreateDataForBatchRecord(currentUtc, oldUtc);
            foreach (var item in last31To60Days)
            {
                last31to60DaysModel = _reportFactory.CreateViewModel(item);
                collection.Add(last31to60DaysModel);
            }
            ds.Tables.Add(_excelFileCreator.ListToDataTable(collection, "31-60 Days"));
            collection.Clear();
            currentUtc = currentUtc.AddDays(-1);
            oldUtc = default(DateTime);

            var oldest = _downloadFileHelperService.CreateDataForBatchRecord(currentUtc, oldUtc);
            foreach (var item in oldest)
            {
                oldestModel = _reportFactory.CreateViewModel(item);
                collection.Add(oldestModel);
            }
            ds.Tables.Add(_excelFileCreator.ListToDataTable(collection, "More Than 60 days"));
          
            //changes here so that only email is send to those who hasnt uploaded any data till not
            //IQueryable<BatchUploadRecord> list = _batchUploadRecordRepository.Table.Where(x => (x.Franchisee.Organization.IsActive) && (!x.IsCorrectUploaded) && (!x.UploadedOn.HasValue && x.UploadedOn == null));

            //var cust = list.ToList();
            //prepare item collection
            //foreach (var item in cust)
            //{
            //    var model = _reportFactory.CreateViewModel(item);
            //    collection.Add(model);
            //}
            var fileName = "missingUpload_List-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;
            result = _excelFileCreator.CreateExcelDocument(ds, filePath);
            if (!result)
                throw new SystemException("Error : Creating File Serviced Customer List!");

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
