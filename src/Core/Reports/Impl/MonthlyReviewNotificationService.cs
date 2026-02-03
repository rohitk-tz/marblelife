using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Review;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class MonthlyReviewNotificationService : IMonthlyReviewNotificationService
    {
        private readonly ILogService _logService;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbakcRequestRepository;
        private readonly ICustomerFeedbackReportFactory _customerFeedbackReportFactory;
        private readonly IFileService _fileService;

        public MonthlyReviewNotificationService(IUnitOfWork unitOfWork, ILogService logService, IExcelFileCreator excelFileCreator, IClock clock,
            IUserNotificationModelFactory userNotificationModelFactory, ICustomerFeedbackReportFactory customerFeedbackReportFactory, IFileService fileService)
        {
            _logService = logService;
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _unitOfWork = unitOfWork;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _userNotificationModelFactory = userNotificationModelFactory;
            _customerFeedbakcRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _customerFeedbackReportFactory = customerFeedbackReportFactory;
            _fileService = fileService;
        }
        public void CreateNotification()
        {
            if (ApplicationManager.Settings.SendMonthlyNotification == false)
            {
                _logService.Info("Monthly Notification turned off!");
                return;
            }

            _logService.Info("Start Monthly Review System List Notification.");

            //SendNotification();

            _logService.Info("End Monthly Review System List Notification.");
        }

        private void SendNotification()
        {
            var date = _clock.UtcNow;
            var previousMonth = date.Month - 1;
            var startDate = new DateTime(date.Year, previousMonth, 1);
            var endDate = new DateTime(date.Year, previousMonth, DateTime.DaysInMonth(date.Year, previousMonth));

            var franchiseeList = _franchiseeRepository.Table.Where(x => x.Organization.IsActive && x.Id > 1).ToList();

            var totalRequests = _customerFeedbakcRequestRepository.Table.Where(x => (x.DateSend >= startDate && x.DateSend <= endDate)).ToList();
            if (!totalRequests.Any())
            {
                _logService.Info(string.Format("No Requests send during {0} - {1}.", startDate, endDate));
                return;
            }

            foreach (var franchisee in franchiseeList)
            {
                bool result;

                var requestList = totalRequests.Where(x => x.FranchiseeId == franchisee.Id);
                if (!requestList.Any())
                {
                    _logService.Info(string.Format("No Requests send during {0} - {1}  for Franchisee {2}.", startDate, endDate, franchisee.Organization.Name));
                    continue;
                }
                _unitOfWork.StartTransaction();
                _logService.Info(string.Format("Pasring For  Franchisee{0} - ", franchisee.Organization.Name));

                var file = CreateReviewList(requestList, startDate, endDate, franchisee, out result);
                if (result && file.Id > 0)
                {
                    _userNotificationModelFactory.CreateReviewSystemRecordNotification(file, startDate, endDate, franchisee.Organization.Id);
                }
                _unitOfWork.SaveChanges();
            }
        }

        private Application.Domain.File CreateReviewList(IEnumerable<CustomerFeedbackRequest> requests, DateTime startDate, DateTime endDate, Franchisee franchisee, out bool result)
        {
            result = false;
            string filePath = string.Empty;

            var listReview = new List<ReviewSystemRecordViewModel>();

            foreach (var item in requests)
            {
                var model = _customerFeedbackReportFactory.CreateModel(item);
                listReview.Add(model);
            }

            var fileName = string.Format("ReviewSystem_Records-{0}", franchisee.Organization.Name) + string.Format("{0:yyyy-MM-dd}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;
            result = _excelFileCreator.CreateExcelDocument(listReview, filePath);
            if (!result)
                throw new SystemException("Error : Creating File Review System Record!");

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
