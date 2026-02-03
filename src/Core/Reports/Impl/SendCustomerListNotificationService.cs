using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Notification.Enum;
using Core.Organizations.Domain;
using Core.Sales;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class SendCustomerListNotificationService : ISendCustomerListNotificationService
    {
        private readonly ILogService _logService;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        private readonly ICustomerFactory _customerFactory;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IFileService _fileService;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;

        public SendCustomerListNotificationService(IUnitOfWork unitOfWork, ILogService logService, IExcelFileCreator excelFileCreator, IClock clock,
            ICustomerFactory customerFactory, IFileService fileService, IUserNotificationModelFactory userNotificationModelFactory)
        {
            _logService = logService;
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _customerFactory = customerFactory;
            _unitOfWork = unitOfWork;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _fileService = fileService;
            _userNotificationModelFactory = userNotificationModelFactory;
        }
        public void CreateNotification()
        {
            if (ApplicationManager.Settings.SendCustomerListNotification == false)
            {
                _logService.Info("Monthly reminder turned off");
                return;
            }

            _logService.Info("Start Monthly CustomerList Notification.");

            SendNotification();

            _logService.Info("End Monthly CustomerList Notification.");
        }

        private void SendNotification()
        {
            bool result;
            _unitOfWork.StartTransaction();

            var date = _clock.UtcNow;
            var previousMonth = date.Month - 1;
            var startDate = new DateTime(date.Year, previousMonth, 1);
            var endDate = new DateTime(date.Year, previousMonth, DateTime.DaysInMonth(date.Year, previousMonth));

            var file = CreateCustomerListFile(startDate, endDate, out result);
            
            if (result && file.Id > 0)
            {
                _userNotificationModelFactory.CreateMonthlyNotificationModel(file, startDate, endDate, NotificationTypes.ListCustomerMonthlyNotification);
            }
            //_logService.Info("Start Monthly CustomerList Notification. For December");
            //var fileForDecember = CreateCustomerListFileForDecember(startDate, endDate, out result);
            //if (result && fileForDecember.Id > 0)
            //{
            //    _logService.Info("End Monthly CustomerList Notification. For December");
            //    var startDateForDecember = new DateTime(2019, 12, 1);
            //    var endDateForDecember = new DateTime(2019, 12, 31);
            //    _userNotificationModelFactory.CreateMonthlyNotificationModel(fileForDecember, startDateForDecember, endDateForDecember, NotificationTypes.ListCustomerMonthlyNotification);
            //}
            _unitOfWork.SaveChanges();
        }

        private Application.Domain.File CreateCustomerListFileForDecember(DateTime startDate, DateTime endDate, out bool result)
        {
            result = false;
            string filePath = string.Empty;
            startDate = new DateTime(2019, 12, 1);
            endDate = new DateTime(2019, 12, 31);
            var customerCollection = new List<CustomerViewModel>();
            IQueryable<Customer> customers = CreateCustomerList(startDate, endDate);

            var cust = customers.ToList();
            //prepare item collection
            foreach (var item in cust)
            {
                if (item.Address != null)
                {
                    var model = _customerFactory.CreateViewModel(item);
                    customerCollection.Add(model);
                }
            }
            var fileName = "customer_List-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;
            result = _excelFileCreator.CreateExcelDocument(customerCollection, filePath);
            if (!result)
                throw new SystemException("Error : Creating File Serviced Customer List!");

            var file = PrepareFileModel(fileName);
            var savedFile = _fileService.SaveModel(file);
            file.Id = savedFile.Id;
            return savedFile;
        }

        private Application.Domain.File CreateCustomerListFile(DateTime startDate, DateTime endDate, out bool result)
        {
            result = false;
            string filePath = string.Empty;

            var customerCollection = new List<CustomerViewModel>();
            IQueryable<Customer> customers = CreateCustomerList(startDate, endDate);

            var cust = customers.ToList();
            //prepare item collection
            foreach (var item in cust)
            {
                if (item.Address != null)
                {
                    var model = _customerFactory.CreateViewModel(item);
                    customerCollection.Add(model);
                }
            }
            var fileName = "customer_List-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            filePath = MediaLocationHelper.GetAttachmentMediaLocation().Path + "/" + fileName;
            result = _excelFileCreator.CreateExcelDocument(customerCollection, filePath);
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

        private IQueryable<Customer> CreateCustomerList(DateTime startDate, DateTime endDate)
        {
            var customerList = _franchiseeSalesRepository.Table.Where(x => x.InvoiceId != null && (x.Invoice.GeneratedOn >= startDate)
             && (x.Invoice.GeneratedOn <= endDate)).OrderByDescending(y => y.Invoice.GeneratedOn).Select(z => z.Customer).Distinct();

            return customerList;
        }
    }
}
