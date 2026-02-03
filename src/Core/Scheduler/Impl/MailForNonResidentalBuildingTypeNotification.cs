using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class MailForNonResidentalBuildingTypeNotification : IMailForNonResidentalBuildingTypeNotification
    {

        private readonly INotificationService _notificationService;
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<JobEstimateImageCategory> _jobEstimateImageCategoryRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateServicesRepository;
        private readonly IRepository<JobEstimateImage> _jobEstimateImageRepository;
        private readonly INotificationModelFactory _notificationModelFactory;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<MarkbeforeAfterImagesHistry> _markbeforeAfterImagesHistryRepository;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IPdfFileService _pdfFileService;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        public MailForNonResidentalBuildingTypeNotification(IUnitOfWork unitOfWork, ILogService logService, ISettings settings,
            IClock clock, INotificationModelFactory notificationModelFactory,
            IUserNotificationModelFactory userNotificationModelFactory, IPdfFileService pdfFileService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _jobSchedulerRepository = _unitOfWork.Repository<JobScheduler>();
            _jobEstimateImageCategoryRepository = _unitOfWork.Repository<JobEstimateImageCategory>();
            _jobEstimateServicesRepository = _unitOfWork.Repository<JobEstimateServices>();
            _jobEstimateImageRepository = _unitOfWork.Repository<JobEstimateImage>();
            _notificationModelFactory = notificationModelFactory;
            _userNotificationModelFactory = userNotificationModelFactory;
            _markbeforeAfterImagesHistryRepository = _unitOfWork.Repository<MarkbeforeAfterImagesHistry>();
            _pdfFileService = pdfFileService;
            _fileRepository = _unitOfWork.Repository<Application.Domain.File>();
            _notificationService = notificationService;
            _organizationRoleUserRepository = _unitOfWork.Repository<OrganizationRoleUser>();
            _organizationRepository = _unitOfWork.Repository<Organization>();
            _franchiseeRepository = _unitOfWork.Repository<Franchisee>();
        }

        public void ProcessRecords()
        {
            SendingMailTOFA();
            SendingMailToFranchisee();
        }


        private void SendingMailTOFA()
        {
            _logService.Info(string.Format("Starting Mail For Franchisee Admin For Before After Images"));
            if (!_settings.MailForNonResidentalBuildingTypeEnabled)
            {
                _logService.Debug("Mail For Franchisee Admin For Before After is disabled");
                return;
            }

            var utcDate = DateTime.Now.Date;
            var yesterdayDate = _clock.ToUtc(utcDate).AddDays(-7);
            var todayDate = _clock.ToUtc(utcDate);
            var endDate = todayDate.AddDays(-1);
            var jobIdList = new List<long?>();
            var estimateIdList = new List<long?>();

            var jobScheduleList = _jobSchedulerRepository.Table.Where(x => x.EndDate >= yesterdayDate && x.EndDate <= utcDate).ToList();
            var jobSchedulerIdList = jobScheduleList.Select(x => x.Id).ToList();
            var jobSchedulerGroupedList = jobScheduleList.GroupBy(x => x.FranchiseeId).ToList();
            var jobEstimateCategories = _jobEstimateImageCategoryRepository.Table.Where(x => x.SchedulerId != null && jobSchedulerIdList.Contains(x.SchedulerId.Value)).Select(x => x.Id).ToList();
            var jobEstimateServices = _jobEstimateServicesRepository.Table.Where(x => x.CategoryId != null && jobEstimateCategories.Contains(x.CategoryId.Value)).ToList();

            var franchiseeId = jobSchedulerGroupedList.Select(x => x.Key).ToList();
            var orgRoleUsers = _organizationRoleUserRepository.Table.Where(x => franchiseeId.Contains(x.OrganizationId) && x.IsActive);

            var jobEstimateCategoryList = _jobEstimateImageCategoryRepository.Table.Where(x => x.SchedulerId != null && jobSchedulerIdList.Contains(x.SchedulerId.Value)).ToList();
            var jobEstimateCategoryIdList = jobEstimateCategoryList.Select(x => x.Id).ToList();
            var jobEstimateServicesList = _jobEstimateServicesRepository.Table.Where(x => x.CategoryId != null && jobEstimateCategoryIdList.Contains(x.CategoryId.Value)).ToList();
            var jobEstimateServicesIdList = jobEstimateServicesList.Select(x => x.Id).ToList();
            var jobEstimateImagesList = _jobEstimateImageRepository.Table.Where(x => jobEstimateServicesIdList.Contains(x.ServiceId.Value)
            && (x.TypeId == (long?)(BeforeAfterImagesType.After) || x.TypeId == (long?)(BeforeAfterImagesType.Before) || x.TypeId == (long?)(BeforeAfterImagesType.During)
           )).ToList();

            var beforeAfterBestPairList = new BeforeAfterBestPairListModel();
            var beforeAfterBestGroupedViewData = new List<BeforeAfterBestPairGroupedViewModel>();
            //jobSchedulerGroupedList = jobSchedulerGroupedList.Where(x => x.Key ==62).ToList();

            foreach (var jobSchedulerGroupedData in jobSchedulerGroupedList)
            {

                var orgRoleUserList = orgRoleUsers.OrderByDescending(x => x.Id).Where(x => x.OrganizationId == jobSchedulerGroupedData.Key && x.RoleId == (long)RoleType.FranchiseeAdmin).ToList();
                var schedulerIdList = jobSchedulerGroupedData.Select(x => x.Id).ToList();
                var categoryIdList = jobEstimateCategoryList.Where(x => schedulerIdList.Contains(x.SchedulerId.Value)).Select(x => x.Id).ToList();
                if (categoryIdList.Count() == 0)
                {
                    continue;
                }

                var jobEstimateServicesLists = jobEstimateServicesList.Where(x => x.CategoryId != null && categoryIdList.Contains(x.CategoryId.Value)).ToList();
                var servicesIdList = jobEstimateServicesList.Where(x => x.CategoryId != null && categoryIdList.Contains(x.CategoryId.Value)).Select(x => x.Id).ToList();
                var jobEstimerImagesForFranchiseeList = jobEstimateImagesList.Where(x => servicesIdList.Contains(x.ServiceId.Value)).ToList();

                if (jobEstimerImagesForFranchiseeList.Count() == 0)
                {
                    continue;
                }
                var beforeServicesPairIdList = jobEstimateServicesLists.Where(x => x.PairId == null).Select(x => x.Id).ToList();
                var afterServicesPairIdList = jobEstimateServicesLists.Where(x => x.PairId != null).Select(x => x.Id).ToList();
                //var beforeImagePairList = jobEstimateImagesList.Where(x => beforeServicesPairIdList.Contains(x.ServiceId.Value)).ToList();
                //var afterImagePairList = jobEstimateImagesList.Where(x => afterServicesPairIdList.Contains(x.ServiceId.Value)).ToList();
                if (beforeServicesPairIdList.Count() >= afterServicesPairIdList.Count())
                {
                    beforeAfterBestPairList = new BeforeAfterBestPairListModel();
                    //foreach (var beforeServicesPairId in beforeServicesPairIdList)
                    //{
                    //    //var afterImageDomain = new JobEstimateImage();
                    //    //var beforeService = jobEstimateServicesList.FirstOrDefault(x => x.PairId == beforeServicesPairId);
                    //    //var beforeImageDomain = jobEstimateImagesList.FirstOrDefault(x => x.ServiceId == beforeServicesPairId);
                    //    //if (beforeService != null)
                    //    //    afterImageDomain = jobEstimateImagesList.FirstOrDefault(x => x.ServiceId == beforeService.Id);

                    //    //beforeAfterBestPairList.BeforeAfterBestPairViewModel.Add(_userNotificationModelFactory.CreateBeforeAfterPairModel(beforeImageDomain, afterImageDomain, beforeImageDomain != null && beforeImageDomain.JobEstimateServices != null && beforeImageDomain.JobEstimateServices.JobEstimateImageCategory != null ? beforeImageDomain.JobEstimateServices.JobEstimateImageCategory.JobScheduler : null,
                    //    //          orgRoleUsers.ToList()));

                    //}

                    //beforeAfterBestPairList.BeforeAfterBestPairViewModel =
                    //        beforeAfterBestPairList.BeforeAfterBestPairViewModel.Where(x => x.AfterImageUrl != "" || x.BeforeImageUrl != "").ToList();
                    //if (beforeAfterBestPairList.BeforeAfterBestPairViewModel.Count() == 0)
                    //{
                    //    continue;
                    //}
                    //var templateName = "before-after-pair.cshtml";
                    //var destinationFolder = MediaLocationHelper.GetAttachmentMediaLocation().Path + "\\";
                    //var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\");
                    //var viewPath = destinationFolder + "Templates\\" + templateName;
                    //var fileName = "Before_After_Pair_" + DateTime.Now.ToFileTimeUtc() + ".pdf";
                    //var file = _pdfFileService.GeneratePdfFromTemplateAndModel(beforeAfterBestPairList, destinationFolder, fileName, viewPath);

                    //var fileDomain = GetFileModel(file);
                    var organization = _organizationRepository.Table.FirstOrDefault(x => x.Id == jobSchedulerGroupedData.Key);
                    //todayDate = todayDate.AddDays(-1);
                    var model = new BeforeAfterBestPairNotificationModel(_notificationModelFactory.CreateBaseDefault())
                    {
                        DateTimes = yesterdayDate.Date.ToString("MM/dd/yyyy"),
                        AssigneePhone = _settings.OwnerPhone,
                        StartDate = yesterdayDate.ToString("MM-dd-yyyy"),
                        EndDate = endDate.ToString("MM-dd-yyyy"),
                        NavigationUrl = _settings.SiteRootUrl + "#/scheduler/beforeAfter/franchiseeAdmin",
                        FranchiseeName = organization != null ? organization.Name : "",
                        FranchiseeId = organization.Id
                    };

                    //var resource = new NotificationResource { Resource = fileDomain, ResourceId = fileDomain.Id, IsNew = true };
                    var emailIdForCC = orgRoleUserList.Select(x => x.Person.Email).ToList();
                    if (emailIdForCC.Count() > 0)
                    {

                        var reString = string.Join(",", emailIdForCC);
                        if (jobSchedulerGroupedData.Key == 62)
                        {
                            reString += "," + _settings.MarketingEmail;
                        }
                        _notificationService.QueueUpNotificationEmail(NotificationTypes.BeforeAfterImageForFA, model, _settings.CompanyName, _settings.FromEmail, reString, _clock.UtcNow, null);
                        _unitOfWork.SaveChanges();
                    }
                }
            }
        }

        private void SendingMailToFranchisee()
        {
            _logService.Info(string.Format("Starting Mail For Franchisee For RPID"));
            if (!_settings.MailForFranchiseeRPIDEnabled)
            {
                _logService.Debug("Mail For Franchisee For Franchisee For RPID is disabled");
                return;
            }
            var utcDate = DateTime.Now.Date;
            var yesterdayDate = _clock.ToUtc(utcDate).AddDays(-7);
            var todayDate = _clock.ToUtc(utcDate);
            var endDate = todayDate.AddDays(-1);
            var franchiseeWithoutRPID = _franchiseeRepository.Table.Where(x => x.ReviewpushId == null && x.Organization.IsActive && x.IsReviewFeedbackEnabled && x.Id != 1 && x.Id != 2 && !x.Organization.Name.StartsWith("0-")).ToList();
            var franchiseeIds = franchiseeWithoutRPID.Select(x => x.Id).ToList();
            foreach (var franchisee in franchiseeWithoutRPID)
            {
                var organization = _organizationRepository.Table.FirstOrDefault(x => x.Id == franchisee.Id);
                var model = new BeforeAfterBestPairNotificationModel(_notificationModelFactory.CreateBaseDefault())
                {
                    DateTimes = yesterdayDate.Date.ToString("MM/dd/yyyy"),
                    AssigneePhone = _settings.OwnerPhone,
                    StartDate = yesterdayDate.ToString("MM-dd-yyyy"),
                    EndDate = endDate.ToString("MM-dd-yyyy"),
                    NavigationUrl = _settings.SiteRootUrl + "#/scheduler/beforeAfter/franchiseeAdmin",
                    FranchiseeName = organization != null ? organization.Name : "",
                    FranchiseeId = organization.Id
                };


                var reString = _settings.RpIdRecipients;
                _notificationService.QueueUpNotificationEmail(NotificationTypes.MailToFranchiseeAdminForRPID, model, _settings.CompanyName, _settings.FromEmail, reString, _clock.UtcNow, null);
                _unitOfWork.SaveChanges();
            }
        }
        private Application.Domain.File GetFileModel(string localFileName)
        {

            var fileModel = new FileModel
            {
                Name = Path.GetFileName(localFileName),
                Caption = Path.GetFileNameWithoutExtension(localFileName),
                RelativeLocation = Path.GetDirectoryName(localFileName),
                MimeType = "application/pdf",
                Size = new FileInfo(localFileName).Length,
                Extension = Path.GetExtension(localFileName)
            };
            var file = new Application.Domain.File
            {
                Name = fileModel.Name,
                Caption = fileModel.Caption,
                RelativeLocation = fileModel.RelativeLocation,
                MimeType = fileModel.MimeType,
                Size = fileModel.Size,
                IsNew = true,
                DataRecorderMetaData = fileModel.DataRecorderMetaData
            };

            _fileRepository.Save(file);
            return file;
        }
    }
}
