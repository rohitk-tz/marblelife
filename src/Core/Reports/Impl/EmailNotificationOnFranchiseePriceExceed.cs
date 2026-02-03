using Core.Application;
using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Reports.Enum;
using Core.Reports.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class EmailNotificationOnFranchiseePriceExceed : IEmailNotificationOnFranchiseePriceExceed
    {
        private IUnitOfWork _unitOfWork;
        private ILogService _logService;
        private ISettings _settings;
        private readonly IRepository<PriceEstimateServices> _priceEstimateServicesRepository;
        public readonly IRepository<ServicesTag> _servicesTagRepository;
        private readonly INotificationService _notificationService;
        private readonly INotificationModelFactory _notificationModelFactory;
        private readonly IClock _clock;
        public EmailNotificationOnFranchiseePriceExceed(IUnitOfWork unitOfWork, ILogService logService, INotificationService notificationService, INotificationModelFactory notificationModelFactory, ISettings settings)
        {
            _settings = settings;
            _unitOfWork = unitOfWork;
            _logService = logService;
            _priceEstimateServicesRepository = unitOfWork.Repository<PriceEstimateServices>();
            _servicesTagRepository = unitOfWork.Repository<ServicesTag>();
            _notificationService = notificationService;
            _notificationModelFactory = notificationModelFactory;
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
        }

        public class EmailClassForPriceEstimate
        {
            public List<EmailNotificationOnFranchiseePriceExceedViewModel> EmailNotificationOnFranchiseePriceExceedViewModels { get; set; }
            public EmailNotificationModelBase Base{get;set;}
            public EmailClassForPriceEstimate(EmailNotificationModelBase emailNotificationModelBase)
            {
                Base = emailNotificationModelBase;
            }
        }

        public class EmailNotificationOnFranchiseePriceExceedViewModel
        {
            public long? Id { get; set; }
            public string FranchiseeName { get; set; }
            public List<ListOfServicesForBulkUploadPriceViewModel> ListOfServicesForBulkUploadPrices { get; set; }
            public int Count { get; set; }

        }
        public class ListOfServicesForBulkUploadPriceViewModel
        {
            public string ListOfServicesName { get; set; }
            public decimal Price { get; set; }
            public decimal CoporatePrice { get; set; }
            public string PriceType { get; set; }
            public string HowCalculated { get; set; }
        }

        public void NotificationOnFranchiseePriceExceed()
        {
            _unitOfWork.StartTransaction();
            List<EmailNotificationOnFranchiseePriceExceedViewModel> notification = new List<EmailNotificationOnFranchiseePriceExceedViewModel>();
            var priceEstimateServicesList = GetListOfFranchiseePriceExceed();
            if (priceEstimateServicesList != null)
            {

                foreach (var item in priceEstimateServicesList)
                {
                    EmailNotificationOnFranchiseePriceExceedViewModel notificationOnPriceExceed = new EmailNotificationOnFranchiseePriceExceedViewModel();
                    var frId = (long)item.FranchiseeId;
                    notificationOnPriceExceed.Id = item.Id;
                    notificationOnPriceExceed.FranchiseeName = item.Franchisee.Organization.Name;
                    notificationOnPriceExceed.ListOfServicesForBulkUploadPrices = getServices(frId);
                    notificationOnPriceExceed.Count = notificationOnPriceExceed.ListOfServicesForBulkUploadPrices.Count() + 1;
                    notification.Add(notificationOnPriceExceed);
                }
                if(notification.Count > 0)
                {
                    SendMailToSuperAdmin(notification);
                    ChangePriceManageEstimateValues(priceEstimateServicesList);
                }
            }

        }

        public IEnumerable<PriceEstimateServices> GetListOfFranchiseePriceExceed()
        {
            var priceEstimateServices = _priceEstimateServicesRepository.Table.Where(x => x.IsFranchiseePriceExceedForEmail == true).ToList();
            return priceEstimateServices;
        }

        public List<ListOfServicesForBulkUploadPriceViewModel> getServices(long frId)
        {
            List<ListOfServicesForBulkUploadPriceViewModel> model = new List<ListOfServicesForBulkUploadPriceViewModel>();
            var listOfServicesforFranchiseepriceExceed = _priceEstimateServicesRepository.Table.Where(x => x.FranchiseeId != null && x.FranchiseeId == frId && x.IsFranchiseePriceExceedForEmail == true).ToList();
            var serviceTag = _servicesTagRepository.Table.Where(x => x.IsActive).ToList();
            foreach (var item in listOfServicesforFranchiseepriceExceed)
            {
                ListOfServicesForBulkUploadPriceViewModel listOfservice = new ListOfServicesForBulkUploadPriceViewModel();
                var type = item.ServicesTag.CategoryId;
                listOfservice.ListOfServicesName = item.ServicesTag.Service;
                listOfservice.Price = item.FranchiseePrice.GetValueOrDefault();
                listOfservice.CoporatePrice = item.CorporatePrice.GetValueOrDefault();
                if(type == (long)ServiceTagCategory.AREA)
                {
                    listOfservice.PriceType = "A";
                }
                else if (type == (long)ServiceTagCategory.TIME)
                {
                    listOfservice.PriceType = "T";
                }
                else if (type == (long)ServiceTagCategory.LINEARFT)
                {
                    listOfservice.PriceType = "L";
                }
                else if (type == (long)ServiceTagCategory.EVENT)
                {
                    listOfservice.PriceType = "E";
                }
                else if (type == (long)ServiceTagCategory.MAINTAINANCE)
                {
                    listOfservice.PriceType = "M";
                }
                else if (type == (long)ServiceTagCategory.PRODUCTPRICE)
                {
                    listOfservice.PriceType = "P";
                }
                else if (type == (long)ServiceTagCategory.TAXRATE)
                {
                    listOfservice.PriceType = "T";
                }
                listOfservice.HowCalculated = serviceTag.Where(x => x.Id == item.ServiceTagId).Select(y => y.Notes).FirstOrDefault();
                model.Add(listOfservice);
            }
            return model;
        }

        public long SendMailToSuperAdmin(List<EmailNotificationOnFranchiseePriceExceedViewModel> notification)
        {
            var fromMail = _settings.FromEmail; ;
            var toMail = _settings.ToSuperAdmin;
            EmailClassForPriceEstimate model = new EmailClassForPriceEstimate(_notificationModelFactory.CreateBaseDefault());
            model.EmailNotificationOnFranchiseePriceExceedViewModels =         notification;
            _notificationService.QueueUpNotificationEmail(NotificationTypes.NotificationToSuperAdminIfFranchiseePriceExceedsTheBulkCorporatePrice, model, _settings.CompanyName, fromMail, toMail, _clock.UtcNow, null, null);
            _unitOfWork.SaveChanges();
            return 0;
        }

        public long ChangePriceManageEstimateValues(IEnumerable<PriceEstimateServices> priceEstimateServiceModel)
        {
            foreach (var item in priceEstimateServiceModel)
            {
                if(item.IsFranchiseePriceExceedForEmail == true)
                {
                    item.IsFranchiseePriceExceedForEmail = false;
                }
                _priceEstimateServicesRepository.Save(item);
            }
            return 0;
        }

    }
}