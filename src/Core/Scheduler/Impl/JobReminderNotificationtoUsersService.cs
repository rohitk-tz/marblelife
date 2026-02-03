using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Geo.Domain;
using Core.Notification;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobReminderNotificationtoUsersService : IJobReminderNotificationtoUsersService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<NotificationType> _notificationTypeRepository;
        private readonly IRepository<JobScheduler> _jobschedulerRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<JobEstimate> _estimateRepository;
        private readonly IRepository<CustomerEmailAPIRecord> _customerEmailAPIRecordRepository;
        private readonly ISettings _setting;
        private readonly IRepository<TechAndSalesSchedulerReminder> _techAndSalesSchedulerRepository;
        public JobReminderNotificationtoUsersService(IUnitOfWork unitOfWork, ILogService logService, IClock clock,
           IUserNotificationModelFactory userNotificationModelFactory, ISettings setting)
        {
            _logService = logService;
            _clock = clock;
            _unitOfWork = unitOfWork;
            _userNotificationModelFactory = userNotificationModelFactory;
            _customerEmailAPIRecordRepository = unitOfWork.Repository<CustomerEmailAPIRecord>();
            _notificationTypeRepository = unitOfWork.Repository<NotificationType>();
            _jobschedulerRepository = unitOfWork.Repository<JobScheduler>();
            _jobRepository = unitOfWork.Repository<Job>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _personRepository = unitOfWork.Repository<Person>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _addressRepository = unitOfWork.Repository<Address>();
            _estimateRepository = unitOfWork.Repository<JobEstimate>();
            _setting = setting;
            _techAndSalesSchedulerRepository = unitOfWork.Repository<TechAndSalesSchedulerReminder>();
        }

        public void CreateNotification()
        {
            if (!_setting.NewJobNotificationToTechAndSales)
            {
                _logService.Debug("New Job  Notification  For Tech and User Queuing is disabled");
                return;
            }
            //_logService.Info("New Job  Notification  For Tech and User Service Starts At- " + _clock.UtcNow);

            //_logService.Debug("New Job  Notification  For Tech and User Queuing Starting");

            //var startDate = _clock.UtcNow.Date.AddDays(-2);
            //var endDate = _clock.UtcNow.Date.AddDays(3);
            //var listScheduleAfterDoneForJobSchedulerId = (from list in _techAndSalesSchedulerRepository.Table
            //                                              select list.JobSchedulerId).Distinct().ToList();

            //var listSchedule = _jobschedulerRepository.Table.Where(x => x.StartDate >= startDate && x.EndDate <= endDate && x.IsActive && !x.IsVacation
            //                    && (x.JobId != null || x.EstimateId != null)).OrderByDescending(x => x.Id).Distinct().ToList();

            //startDate = _clock.UtcNow.Date.AddDays(-1);
            //endDate = _clock.UtcNow.Date.AddDays(1);
            //listSchedule = listSchedule.Where(x => x.ActualStartDate >= startDate && x.ActualEndDate <= endDate).ToList();
            //listSchedule = listSchedule.Where(x => (x.Job != null ? !(listScheduleAfterDoneForJobSchedulerId.Contains(x.Id)) : !(listScheduleAfterDoneForJobSchedulerId.Contains(x.Id)))).ToList();

            //if (!listSchedule.Any())
            //{
            //    _logService.Debug("No records found.");
            //    return;
            //}
            //foreach (var item in listSchedule)
            //{
            //    try
            //    {
            //        _unitOfWork.StartTransaction();
                    //_*userNotificationModelFactory.ScheduleReminderNotificationToUserOnDay(item, startDate, endDate);*/
            //        _logService.Debug("New Job  Notification  For Tech and User Queuing Ending");
            //        _unitOfWork.SaveChanges();
            //    }
            //    catch (Exception ex)
            //    {
            //        _logService.Debug("Error in New Job  Notification  For Tech and User");
            //        _unitOfWork.Rollback();
            //        _logService.Error(ex);
            //    }
            //}

        }
    }
}
