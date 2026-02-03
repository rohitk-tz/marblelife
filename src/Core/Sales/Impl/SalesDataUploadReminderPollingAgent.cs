using Core.Application;
using Core.Application.Attribute;
using Core.Notification;
using Core.Notification.Domain;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Sales.Domain;
using Core.Sales.Enum;
using System;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesDataUploadReminderPollingAgent : ISalesDataUploadReminderPollingAgent
    {
        private readonly IRepository<SalesDataUpload> _salesdataUploadRepository;
        private IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<FeeProfile> _feeProfileRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<SalesDataMailReminder> _salesDataMailReminderRepository;
        private readonly IRepository<NotificationType> _notificationTypeRepository;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private ILogService _logService;
        private readonly ISalesDataUploadCreateModelValidator _salesDataUploadCreateModelValidator;
        private readonly ISalesDataUploadService _salesDataUploadService;

        public SalesDataUploadReminderPollingAgent(IUnitOfWork unitOfWork, IUserNotificationModelFactory userNotificationModelFactory, IClock clock,
            ISettings settings, ILogService logService, ISalesDataUploadCreateModelValidator salesDataUploadCreateModelValidator,
           ISalesDataUploadService salesDataUploadService)
        {
            _userNotificationModelFactory = userNotificationModelFactory;
            _salesdataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _feeProfileRepository = unitOfWork.Repository<FeeProfile>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _salesDataMailReminderRepository = unitOfWork.Repository<SalesDataMailReminder>();
            _notificationTypeRepository = unitOfWork.Repository<NotificationType>();
            _clock = clock;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _logService = logService;
            _salesDataUploadCreateModelValidator = salesDataUploadCreateModelValidator;
            _salesDataUploadService = salesDataUploadService;
        }

        public void CreateNotificationReminderForSalesDataUpload()
        {
            if (!IsNotificationQueuingEnabled())
            {
                _logService.Debug("SalesDataUpload Notification Queuing is disabled");
                return;
            }
            var franchiseeList = _franchiseeRepository.Table.Where(x => x.LateFee.SalesDataLateFee > 0 && x.Organization.IsActive).ToArray();

            if (!franchiseeList.Any())
            {
                _logService.Debug("No records found.");
                return;
            }

            foreach (var item in franchiseeList)
            {
                try
                {
                    if (_salesDataMailReminderRepository.Table.Any(x => x.FranchiseeId == item.Id && x.Date == _clock.UtcNow.Date))
                    {
                        _logService.Debug("Reminder already send.");
                        continue;
                    }

                    _unitOfWork.StartTransaction();

                    var salesDatauploadList = _salesdataUploadRepository.Table.Where(x => x.FranchiseeId == item.Id && x.IsActive).ToArray();

                    if (!salesDatauploadList.Any())
                    {
                        _logService.Debug("No records found.");
                        continue;
                    }

                    var uploadStartDate = _clock.UtcNow;
                    var uploadEndDate = uploadStartDate.AddDays(6);

                    var lastUpload = salesDatauploadList.Where(x => x.StatusId != (long)SalesDataUploadStatus.Failed)
                        .OrderByDescending(x => x.PeriodEndDate).FirstOrDefault();
                    if (lastUpload == null)
                        continue;
                    var feeProfile = _feeProfileRepository.Get(lastUpload.FranchiseeId);
                    var currentDate = _clock.UtcNow;

                    if (feeProfile == null)
                    {
                        _logService.Debug("No fee Profile found for Franchisee.");
                        continue;
                    }

                    if (feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Weekly)
                    {
                        var startDate = lastUpload.PeriodEndDate.Date.AddDays(1);
                        var endDate = startDate.AddDays(6);

                        var result = _salesDataUploadCreateModelValidator.CheckIfDatesAreValidWeek(startDate, endDate);
                        if (result)
                        {
                            uploadStartDate = startDate;
                            uploadEndDate = endDate;
                        }
                        else
                        {
                            uploadStartDate = _salesDataUploadService.StartOfWeek(startDate, DayOfWeek.Monday);
                            uploadEndDate = uploadStartDate.Date.AddDays(6);
                        }
                        if (currentDate >= uploadEndDate)
                        {
                            _userNotificationModelFactory.CreateSalesDataReminderNotification(lastUpload, uploadStartDate, uploadEndDate, feeProfile.PaymentFrequencyId);
                            var domain = CreateDomain(item);
                            _salesDataMailReminderRepository.Save(domain);
                        }
                    }

                    if (feeProfile.PaymentFrequencyId == null || feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly)
                    {
                        var startDate = lastUpload.PeriodEndDate.Date.AddDays(1);
                        var endDate = startDate.Date.AddMonths(1).AddDays(-1);

                        var result = _salesDataUploadCreateModelValidator.CheckDatesAreValidMonth(startDate, endDate);
                        if (result)
                        {
                            uploadStartDate = startDate;
                            uploadEndDate = endDate;
                        }
                        else
                        {
                            uploadStartDate = new DateTime(startDate.Year, startDate.Month, 1);
                            uploadEndDate = uploadStartDate.Date.AddMonths(1).AddDays(-1);
                        }
                        if (currentDate >= uploadEndDate)
                        {
                            _userNotificationModelFactory.CreateSalesDataReminderNotification(lastUpload, uploadStartDate, uploadEndDate, feeProfile.PaymentFrequencyId);
                            var domain = CreateDomain(item);
                            _salesDataMailReminderRepository.Save(domain);
                        }
                    }

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logService.Error(ex);
                }
            }
        }

        private SalesDataMailReminder CreateDomain(Franchisee doman)
        {
            return new SalesDataMailReminder
            {
                IsNew = true,
                FranchiseeId = doman.Id,
                Date = _clock.UtcNow.Date
            };
        }

        private bool IsNotificationQueuingEnabled()
        {
            var notificationtype = _notificationTypeRepository.Get(x => x.Id == (long)Notification.Enum.NotificationTypes.SalesDataUploadReminder);
            if (notificationtype.IsQueuingEnabled && notificationtype.IsServiceEnabled)
            {
                return true;
            }
            return false;
        }
    }
}
