using Core.Application;
using Core.Application.Attribute;
using Core.MarketingLead;
using Core.Notification;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class NotificationToFA : INotificationToFA
    {
        private readonly ILogService _logService;
        private ISettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly INotificationModelFactory _notificationModelFactory;
        private readonly INotificationService _notificationService;
        public NotificationToFA(IUnitOfWork unitOfWork, ISettings settings, INotificationModelFactory notificationModelFactory, INotificationService notificationService)
        {
            _settings = settings;
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _unitOfWork = unitOfWork;
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _notificationModelFactory = notificationModelFactory;
            _notificationService = notificationService;
        }
        public void ProcessRecords()
        {
            var franchiseeList = _franchiseeRepository.Table.Where(x => x.Id != 1 && x.Id != 2 && !x.Organization.Name.StartsWith("0-") && x.Organization.IsActive).ToList();
            SendNotificationToFA(franchiseeList);
            RenwealeMailToFranchisee(franchiseeList);
        }

        private void SendNotificationToFA(List<Franchisee> franchiseeList)
        {
            if (!_settings.NotificationToFADisabled)
            {
                _logService.Debug("Notification To FA is disabled");
                return;
            }
            franchiseeList = franchiseeList.Where(x => x.RegistrationDate != null).ToList();
            _logService.Info("Starting Notification To FA at " + _clock.UtcNow);

            var fromMail = _settings.FromEmail;
            foreach (var franchisee in franchiseeList)
            {
                var model = new NotificationToFAModel(_notificationModelFactory.CreateBaseDefault())
                {
                    RenewableDate = DateTime.UtcNow.AddDays(21).ToString("MM-dd-yyyy"),
                    FranchiseeName = franchisee.Organization.Name,
                    FromEmail = "",
                    Email = franchisee.Organization.Email,
                    OrganizationId = franchisee.Organization.Id,
                    RenewableFees= franchisee.RenewalFee,
                    //Date=franchisee.RegistrationDate.Value.ToString("MM-dd-yyyy"),
                };
                var currentDate = _clock.UtcNow;
                //model.DateRange = new List<DateRangeViewModel>();

                var dateRangeModel = new DateRangeViewModel { };

                _notificationService.QueueUpNotificationEmail(NotificationTypes.NotificationToFA, model, _settings.CompanyName, fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
                _unitOfWork.SaveChanges();
            }
        }

        private void RenwealeMailToFranchisee(List<Franchisee> franchiseeList)
        {
            franchiseeList = franchiseeList.Where(x => x.RegistrationDate != null).ToList();
            if (!_settings.RenewableMailDisabled)
            {
                _logService.Debug("Renewable Mail To Franchisee is disabled");
                return;
            }
            _logService.Info("Starting Renewable Mail To Franchisee at " + _clock.UtcNow);

            foreach (var franchisee in franchiseeList)
            {
                var renewableDate = franchisee.DateOfRenewal.Value.AddDays(1);
                var todayDate = DateTime.UtcNow;
                var fromMail = _settings.FromEmail;
                var monthDifference = ((renewableDate.Year * 12) + renewableDate.Month) - ((todayDate.Year * 12) + todayDate.Month) + 1;
                if (monthDifference <= 10)
                {
                    var model = new NotificationToFAModel(_notificationModelFactory.CreateBaseDefault())
                    {
                        Date = DateTime.UtcNow.AddDays(28).ToString("MM-dd-yyyy"),
                        FranchiseeName = franchisee.Organization.Name,
                        FromEmail = "",
                        Email = franchisee.Organization.Email,
                        OrganizationId = franchisee.Organization.Id,
                        RenewableDate = franchisee.DateOfRenewal.Value.AddDays(1).ToString("MM-dd-yyyy"),
                        RenewableFees = franchisee.RenewalFee,
                        PersonName = franchisee.OwnerName
                    };
                    var currentDate = _clock.UtcNow;
                    var emailReciepant = _settings.AuditRecipients;
                    model.Email += ", " + emailReciepant;
                    var dateRangeModel = new DateRangeViewModel { };
                    if (monthDifference == 10)
                    {
                        _notificationService.QueueUpNotificationEmail(NotificationTypes.RenewableMailForFranchiseeBefore9Month, model, _settings.CompanyName, fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
                    }
                    else
                    {
                        _notificationService.QueueUpNotificationEmail(NotificationTypes.RenewableMailForFranchiseeBefore8Month, model, _settings.CompanyName, fromMail, model.Email, _clock.UtcNow, model.OrganizationId, null);
                    }
                    _unitOfWork.SaveChanges();
                }
            }
        }
    }
}

