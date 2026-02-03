using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Geo.Domain;
using Core.MarketingLead.Domain;
using Core.Notification;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;

using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobReminderNotificationService : IJobReminderNotificationService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<NotificationType> _notificationTypeRepository;
        private readonly IRepository<JobScheduler> _jobschedulerRepository;
        private readonly ISettings _setting;
        private readonly IRepository<CustomerSchedulerReminderAudit> _jobReminderAuditRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<JobEstimateImageCategory> _jobEstimateImageCategoryRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateImageServiceRepository;
        private readonly IRepository<JobEstimateImage> _jobEstimateImageImageRepository;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
       
        private readonly IRepository<JobCustomer> _jobCustomerRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<File> _fileRepository;
        private readonly IRepository<CallDetailData> _callDetailDataRepository;
        private readonly IRepository<WebLead> _webLeadRepository;
        public JobReminderNotificationService(IUnitOfWork unitOfWork, ILogService logService, IClock clock,
            IUserNotificationModelFactory userNotificationModelFactory, ISettings setting)
        {

            _logService = logService;
            _clock = clock;
            _unitOfWork = unitOfWork;
            _userNotificationModelFactory = userNotificationModelFactory;
            _notificationTypeRepository = unitOfWork.Repository<NotificationType>();
            _jobschedulerRepository = unitOfWork.Repository<JobScheduler>();
            _setting = setting;
            _jobReminderAuditRepository = unitOfWork.Repository<CustomerSchedulerReminderAudit>();
            _jobSchedulerRepository = _unitOfWork.Repository<JobScheduler>();
            _jobEstimateImageCategoryRepository = _unitOfWork.Repository<JobEstimateImageCategory>();
            _jobEstimateImageServiceRepository = _unitOfWork.Repository<JobEstimateServices>();
            _jobEstimateImageImageRepository = _unitOfWork.Repository<JobEstimateImage>();
            _beforeAfterImagesRepository = _unitOfWork.Repository<BeforeAfterImages>();
            _fileRepository = _unitOfWork.Repository<File>();
            _jobCustomerRepository = _unitOfWork.Repository<JobCustomer>();
            _franchiseeSalesRepository = _unitOfWork.Repository<FranchiseeSales>();
            _salesDataUploadRepository = _unitOfWork.Repository<SalesDataUpload>();
            _webLeadRepository = _unitOfWork.Repository<WebLead>();
            _callDetailDataRepository = _unitOfWork.Repository<CallDetailData>();
            
        }

        public void CreateNotification()
        {
            MailSend();
            BeforeAfterImagesMigration();
            WebLeadsMail();
        }

        private void MailSend()
        {
            var encrypredData = "";
            if (!_setting.NewJobNotificationToClient)
            {
                _logService.Debug("New Job  Notification Queuing is disabled");
                return;
            }

            _logService.Info("New Job  Notification to User Started at- " + _clock.UtcNow);
            var startDate = _clock.UtcNow.Date.AddDays(-2);
            var endDate = _clock.UtcNow.Date.AddDays(4);

            var listScheduleAfterDoneForJobSchedulerId = _jobReminderAuditRepository.Table.Select(x => x.JobSchedulerId).Distinct().ToList();

            var listSchedule = _jobschedulerRepository.Table.Where(x => x.StartDate >= startDate && x.EndDate <= endDate && x.IsActive && !x.IsVacation
                                && (x.JobId != null || x.EstimateId != null)).OrderByDescending(x => x.Id).Distinct().ToList();

            startDate = _clock.UtcNow.Date.AddDays(2);
            endDate = _clock.UtcNow.Date.AddDays(3);

            listSchedule = listSchedule.Where(x => x.ActualStartDate >= startDate && x.ActualEndDate <= endDate).ToList();
            listSchedule = listSchedule.Where(x => (x.Job != null ? !(listScheduleAfterDoneForJobSchedulerId.Contains(x.Id)) : !(listScheduleAfterDoneForJobSchedulerId.Contains(x.Id)))).ToList();

            if (!listSchedule.Any())
            {
                _logService.Debug("No records found.");
                return;
            }
            var scheduledList = new List<JobScheduler>();

            foreach (var item in listSchedule)
            {
                if (item.JobId.HasValue && !scheduledList.Any((JobScheduler x) => x.JobId == item.JobId) && _jobReminderAuditRepository.Table.FirstOrDefault(x => x.JobId == item.JobId) == default(CustomerSchedulerReminderAudit))
                //if (item.JobId.HasValue && !scheduledList.Any((JobScheduler x) => x.Id == item.Id) && _jobReminderAuditRepository.Table.FirstOrDefault(x => x.JobSchedulerId == item.Id) == default(CustomerSchedulerReminderAudit))
                {
                    scheduledList.Add(item);
                    _logService.Debug("job id = " + item.JobId + " estimation id = " + item.EstimateId);
                }

                else if (item.EstimateId.HasValue && !scheduledList.Any((JobScheduler x) => x.EstimateId == item.EstimateId))
                {
                    scheduledList.Add(item);
                    _logService.Debug("job id = " + item.JobId + " estimation id = " + item.EstimateId);
                }
            }

            foreach (var item in scheduledList.OrderByDescending(x => x.Id))
            {
                try
                {
                    encrypredData = EncryptionHelper.Encrypt(item.Id.ToString());
                    encrypredData = EncryptionHelper.UrlEncrypt(encrypredData);
                    if (encrypredData.IndexOf('%') != -1)
                    {
                        encrypredData = encrypredData.Replace("%", "LOOP");
                    }
                    _unitOfWork.StartTransaction();
                    _userNotificationModelFactory.ScheduleReminderNotification(item, startDate, endDate, encrypredData, NotificationTypes.NewJobNotificationToUser);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logService.Error(ex);
                }
            }
        }


        private void BeforeAfterImagesMigration()
        {

            if (!_setting.BeforeAfterMigrationDisabled)
            {
                _logService.Debug("Before After Image Migration is disabled");
                return;
            }
            _logService.Info("Before After Image Migration Starts at " + _clock.UtcNow);

            var utcDate = DateTime.UtcNow.Date.AddMonths(1);
            var yesterdayDate = _clock.ToUtc(utcDate).AddMonths(-4);

            var jobSchedulerForLastTwoMonths = _jobSchedulerRepository.Table.Where(x => x.StartDate >= yesterdayDate && x.StartDate <= utcDate).ToList();
            var jobSchedulerIdList = jobSchedulerForLastTwoMonths.Select(x => x.Id).ToList();
            var jobEstimateCategory = _jobEstimateImageCategoryRepository.Table.Where(x => x.SchedulerId != null && jobSchedulerIdList.Contains(x.SchedulerId.Value)).ToList();
            var jobEstimateCategoryId = jobEstimateCategory.Select(x => x.Id).ToList();
            var jobEstimateServices = _jobEstimateImageServiceRepository.Table.Where(x => x.CategoryId != null && jobEstimateCategoryId.Contains(x.CategoryId.Value) && (x.TypeId == ((long?)LookupTypes.ExteriorBuilding) ||
            x.TypeId == ((long?)LookupTypes.BeforeWork) || x.TypeId == ((long?)LookupTypes.AfterWork))).ToList();
            var jobEstimateServiceId = jobEstimateServices.Select(x => x.Id).ToList();
            var jobEstimateImages = _jobEstimateImageImageRepository.Table.Where(x => x.ServiceId != null && jobEstimateServiceId.Contains(x.ServiceId.Value)).ToList();

            var beforeAfterImagesBeforeDomain = new BeforeAfterImages();
            //jobEstimateServices = jobEstimateServices.Where(x => x.Id == 12048).ToList();
            foreach (var jobEstimateService in jobEstimateServices.Where(x => x.PairId == null))
            {

                var jobEstimateCategoryDomain = jobEstimateCategory.FirstOrDefault(x => x.Id == jobEstimateService.CategoryId);

                var jobEstimateServiceAfterDomain = jobEstimateServices.FirstOrDefault(x => x.PairId == jobEstimateService.Id);
                var jobEstimateServiceBeforeDomain = jobEstimateService;

                var jobEstimateBeforeImage = jobEstimateImages.FirstOrDefault(x => x.ServiceId == jobEstimateService.Id);
                var jobEstimateAfterImage = jobEstimateImages.FirstOrDefault(x => jobEstimateServiceAfterDomain != null && x.ServiceId == jobEstimateServiceAfterDomain.Id);
                var personName = jobEstimateCategoryDomain.JobScheduler.Person.FirstName + " " + jobEstimateCategoryDomain.JobScheduler.Person.LastName;

                var franchiseeId = jobEstimateCategoryDomain.JobScheduler.FranchiseeId;
                beforeAfterImagesBeforeDomain = new BeforeAfterImages()
                {
                    MAIDJANITORIAL = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.MAIDJANITORIAL : "",
                    MaidService = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.MaidService : "",
                    AddToGalleryDateTime = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.AddToGalleryDateTime : default(DateTime?),
                    BuildingLocation = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.BuildingLocation : "",
                    FinishMaterial = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.FinishMaterial : "",
                    CompanyName = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.CompanyName : "",
                    FloorNumber = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.FloorNumber : 0,
                    MarkertingClassId = jobEstimateCategoryDomain != null ? jobEstimateCategoryDomain.MarkertingClassId : default(long?),
                    BestFitMarkDateTime = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.BestFitMarkDateTime : default(DateTime?),
                    CategoryId = jobEstimateCategoryDomain != null ? jobEstimateCategoryDomain.Id : default(long?),
                    EstimateId = jobEstimateCategoryDomain.EstimateId,
                    JobId = jobEstimateCategoryDomain.JobId,
                    SchedulerId = jobEstimateCategoryDomain.SchedulerId,
                    FileId = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.FileId : default(long?),
                    IsBeforeImage = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.IsBestImage : default(bool?),
                    IsAddToLocalGallery = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.IsAddToLocalGallery : default(bool),
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                    ServiceId = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.Id : default(long?),
                    PairId = null,
                    SurfaceColor = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.SurfaceColor : "",
                    PropertyManager = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.PropertyManager : "",
                    SurfaceMaterial = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.SurfaceMaterial : "",
                    SurfaceType = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.SurfaceType : "",
                    Id = 0,
                    IsNew = true,
                    PersonName = personName,
                    FranchiseeId = franchiseeId,
                    RoleId = jobEstimateCategoryDomain.JobScheduler != null && jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser != null ? jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser.RoleId : default(long?),
                    UserId = jobEstimateCategoryDomain.JobScheduler != null && jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser != null ? jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser.UserId : default(long?),
                    ServiceTypeId = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.ServiceTypeId : default(long?),
                    TypeId = jobEstimateServiceBeforeDomain != null ? jobEstimateServiceBeforeDomain.TypeId : default(long?),
                    ImageUrl= jobEstimateBeforeImage != null ? GetBase64String(jobEstimateBeforeImage.File) : "",

                };
                if (beforeAfterImagesBeforeDomain.FileId != null)
                {
                    var file = _fileRepository.Table.FirstOrDefault(x => x.Id == beforeAfterImagesBeforeDomain.FileId);
                    if (file == null)
                    {
                        beforeAfterImagesBeforeDomain.File = null;
                        beforeAfterImagesBeforeDomain.FileId = null;
                        beforeAfterImagesBeforeDomain.ImageUrl = "";
                    }
                }

                var beforeAfterBeforeImagesDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.CategoryId == jobEstimateCategoryDomain.Id && x.ServiceId == jobEstimateServiceBeforeDomain.Id);
                var beforeAfterBeforeForFileDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.JobId == jobEstimateCategoryDomain.JobId && x.FileId == beforeAfterImagesBeforeDomain.FileId && x.SchedulerId == jobEstimateCategoryDomain.SchedulerId);
                if (beforeAfterBeforeImagesDomain != null)
                {
                    beforeAfterImagesBeforeDomain.Id = beforeAfterBeforeImagesDomain.Id;
                    beforeAfterImagesBeforeDomain.IsNew = false;
                }
                else if (beforeAfterBeforeForFileDomain != null)
                {
                    beforeAfterImagesBeforeDomain.Id = beforeAfterBeforeForFileDomain.Id;
                    beforeAfterImagesBeforeDomain.IsNew = false;
                }

                _beforeAfterImagesRepository.Save(beforeAfterImagesBeforeDomain);
                if (jobEstimateServiceBeforeDomain != null)
                {
                    _unitOfWork.SaveChanges();
                }

                if (jobEstimateServiceAfterDomain == null)
                {
                    continue;
                }
                var beforeAfterImagesAfterDomain = new BeforeAfterImages()
                {
                    MAIDJANITORIAL = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.MAIDJANITORIAL : "",
                    MaidService = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.MaidService : "",
                    AddToGalleryDateTime = jobEstimateAfterImage != null ? jobEstimateAfterImage.AddToGalleryDateTime : default(DateTime?),
                    BuildingLocation = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.BuildingLocation : "",
                    FinishMaterial = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.FinishMaterial : "",
                    CompanyName = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.CompanyName : "",
                    FloorNumber = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.FloorNumber : 0,
                    MarkertingClassId = jobEstimateCategoryDomain != null ? jobEstimateCategoryDomain.MarkertingClassId : default(long?),
                    BestFitMarkDateTime = jobEstimateAfterImage != null ? jobEstimateAfterImage.BestFitMarkDateTime : default(DateTime?),
                    CategoryId = jobEstimateCategoryDomain != null ? jobEstimateCategoryDomain.Id : default(long?),
                    EstimateId = jobEstimateCategoryDomain.EstimateId,
                    JobId = jobEstimateCategoryDomain.JobId,
                    SchedulerId = jobEstimateCategoryDomain.SchedulerId,
                    FileId = jobEstimateAfterImage != null ? jobEstimateAfterImage.FileId : default(long?),
                    IsBeforeImage = jobEstimateAfterImage != null ? jobEstimateAfterImage.IsBestImage : default(bool?),
                    IsAddToLocalGallery = jobEstimateAfterImage != null ? jobEstimateAfterImage.IsAddToLocalGallery : default(bool),
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                    ServiceId = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.Id : default(long?),
                    PairId = beforeAfterImagesBeforeDomain != null && beforeAfterImagesBeforeDomain.Id != 0 ? beforeAfterImagesBeforeDomain.Id : default(long?),
                    SurfaceColor = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.SurfaceColor : "",
                    PropertyManager = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.PropertyManager : "",
                    SurfaceMaterial = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.SurfaceMaterial : "",
                    SurfaceType = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.SurfaceType : "",
                    Id = 0,
                    IsNew = true,
                    PersonName = personName,
                    FranchiseeId = franchiseeId,
                    RoleId = jobEstimateCategoryDomain.JobScheduler != null && jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser != null ? jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser.RoleId : default(long?),
                    UserId = jobEstimateCategoryDomain.JobScheduler != null && jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser != null ? jobEstimateCategoryDomain.JobScheduler.OrganizationRoleUser.UserId : default(long?),
                    ServiceTypeId = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.ServiceTypeId : default(long?),
                    TypeId = jobEstimateServiceAfterDomain != null ? jobEstimateServiceAfterDomain.TypeId : default(long?),
                    ImageUrl = jobEstimateAfterImage != null ? GetBase64String(jobEstimateAfterImage.File) : "",
                };

                if (beforeAfterImagesAfterDomain.FileId != null)
                {
                    var file = _fileRepository.Table.FirstOrDefault(x => x.Id == beforeAfterImagesAfterDomain.FileId);
                    if (file == null)
                    {
                        beforeAfterImagesAfterDomain.File = null;
                        beforeAfterImagesAfterDomain.FileId = null;
                        beforeAfterImagesAfterDomain.ImageUrl = "";
                    }
                }
                beforeAfterBeforeImagesDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.CategoryId == jobEstimateCategoryDomain.Id && x.ServiceId == jobEstimateServiceAfterDomain.Id);
                beforeAfterBeforeForFileDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.JobId == jobEstimateCategoryDomain.JobId && x.FileId == beforeAfterImagesAfterDomain.FileId && x.SchedulerId == jobEstimateCategoryDomain.SchedulerId);
                if (beforeAfterBeforeImagesDomain != null)
                {
                    beforeAfterImagesAfterDomain.Id = beforeAfterBeforeImagesDomain.Id;
                    beforeAfterImagesAfterDomain.IsNew = false;
                }
                else if (beforeAfterBeforeForFileDomain != null)
                {
                    beforeAfterImagesAfterDomain.Id = beforeAfterBeforeForFileDomain.Id;
                    beforeAfterImagesAfterDomain.IsNew = false;
                }
                _beforeAfterImagesRepository.Save(beforeAfterImagesAfterDomain);
                _unitOfWork.SaveChanges();
            }
        }


        private string GetBase64String(Application.Domain.File file)
        {

            if (file == null)
            {
                return "";
            }
            try
            {
                var filePath = (file.RelativeLocation + @"\" + file.Name).ToPath();
                byte[] imageArray = System.IO.File.ReadAllBytes(filePath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                base64ImageRepresentation = "data:image/png;base64," + base64ImageRepresentation;
                return base64ImageRepresentation;
            }
            catch (Exception e1)
            {
                return "";
            }
        }


        private void WebLeadsMail()
        {
            if (_setting.WebLeadsToEmail==string.Empty)
            {
                _logService.Debug("No Email Id found To Send Emails.");
                return;
            }
            DateTime todaysDate = DateTime.UtcNow.Date;
            DateTime previousDate = todaysDate.AddDays(-1);
            List<WebLead> webleads = _webLeadRepository.Table.Where(x=>x.CreatedDate == previousDate).ToList();
            var callDetailData = _callDetailDataRepository.Table.Where(x => x.DataRecorderMetaData.DateCreated.Date == previousDate).AsQueryable();
            if(webleads.Count == 0 || callDetailData.ToList().Count == 0)
            {
                _userNotificationModelFactory.SendWebLeadsNotification(NotificationTypes.WebLeadsMail, previousDate);
            }

        }
    }
}
