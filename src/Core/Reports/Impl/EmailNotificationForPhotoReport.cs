using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Notification.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Reports.Enum;
using Core.Reports.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Users.Domain;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class EmailNotificationForPhotoReport : IEmailNotificationForPhotoReport
    {
        private readonly ILogService _logService;
        private IUnitOfWork _unitOfWork;
        private ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        IPdfFileService _pdfFileService;
        private readonly INotificationModelFactory _notificationModelFactory;
        private readonly INotificationService _notificationService;
        private readonly IRepository<DataRecorderMetaData> _dataRecorderMetaDataRepository;
        private readonly IRepository<JobEstimate> _jobEstimateRepository;
        private readonly IRepository<JobCustomer> _jobCustomerRepository;
        private readonly IRepository<CustomerJobEstimate> _customerJobEstimateRepository;
        public EmailNotificationForPhotoReport(IUnitOfWork unitOfWork, ISettings settings, IClock clock, ILogService logService, IPdfFileService pdfFileService, INotificationModelFactory notificationModelFactory,
            INotificationService notificationService)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _pdfFileService = pdfFileService;
            _notificationModelFactory = notificationModelFactory;
            _notificationService = notificationService;
            _dataRecorderMetaDataRepository = unitOfWork.Repository<DataRecorderMetaData>();
            _jobEstimateRepository = unitOfWork.Repository<JobEstimate>();
            _jobCustomerRepository = unitOfWork.Repository<JobCustomer>();
            _customerJobEstimateRepository = unitOfWork.Repository<CustomerJobEstimate>();
        }


        public void SendEmailNotificationForPhotoReport()
        {
            GeneratePDF();
            SendWeeklyEmailNotificationToAlanAndNicolForLocalMarketingReview();
            //MapCustomerForJobEstimate();
            //SendPhotoReportEmailToFranchiseeOwner();
            //SendPhotoReportEmailToScheduler();
            //SendPhotoReportEmailToSalesRep();
            //SendPhotoReportEmailToTechnician();
        }

        private bool GeneratePDF() 
        {
            try
            {
                _logService.Info(string.Format("Start Photo Report Job"));
                //long? franchiseeId = 3;
                long? franchiseeId = 62;
                long? userId = null;

                GeneratePDFForFranchiseeOwner(franchiseeId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Generating Photo Report PDFs - {0}", ex.Message));
                return false;
            }
        }
        private bool SendPhotoReportEmailToFranchiseeOwner()
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Photo Report Email To Franchisee Owner - {0}", ex.Message));
                return false;
            }
        }
        private bool SendPhotoReportEmailToScheduler()
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Photo Report Email To Scheduler - {0}", ex.Message));
                return false;
            }
        }
        private bool SendPhotoReportEmailToSalesRep()
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Photo Report Email To SalesRep - {0}", ex.Message));
                return false;
            }
        }
        private bool SendPhotoReportEmailToTechnician()
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Photo Report Email To Technician - {0}", ex.Message));
                return false;
            }
        }
        private bool GeneratePDFForFranchiseeOwner(long? franchiseeId, long? userId)
        {
            try
            {
                _logService.Info(string.Format("Start Getting Data For Photo Report"));
                DateTime currentDateTime = DateTime.Now;
                DateTime currentStartingDateTimeForMonth = currentDateTime.AddDays(-12);
                DateTime currentEndDateTimeForMonth = currentDateTime;

                LocalMarketingViewModel localMarketingViewModel = new LocalMarketingViewModel();
                List<LocalMarketingViewModel> localMarketingListViewModel = new List<LocalMarketingViewModel>();
                List<BeforeAfterPersonsViewModal> beforeAfterPersonsListViewModal = new List<BeforeAfterPersonsViewModal>();
                localMarketingViewModel.StartDate = currentDateTime.AddDays(-7);
                localMarketingViewModel.EndDate = currentDateTime.AddDays(-1);

                var beforeAfter = _beforeAfterImagesRepository.Table.Where(x => (x.TypeId == (long)LookupTypes.BeforeWork) || (x.TypeId == (long)LookupTypes.DuringWork) ||
                (x.TypeId == (long)LookupTypes.AfterWork) || (x.TypeId == (long)LookupTypes.ExteriorBuilding) && (x.DataRecorderMetaData.DateCreated >= currentStartingDateTimeForMonth) && (x.DataRecorderMetaData.DateCreated <= currentEndDateTimeForMonth)).ToList();

                //var organizationRollUserList = _organizationRoleUserRepository.IncludeMultiple(x => x.Organization, x => x.Role).Where(x => x.IsActive == true && (userId == null || x.UserId == userId) && ((x.RoleId == (long)RoleType.SalesRep) || (x.RoleId == (long)RoleType.Technician))).ToList();
                var organizationRollUserSalesRepList = _organizationRoleUserRepository.IncludeMultiple(x => x.Organization, x => x.Role).Where(x => x.IsActive == true && (userId == null || x.UserId == userId) && (x.RoleId == (long)RoleType.SalesRep)).OrderBy(x => x.Person.FirstName).ToList();
                var organizationRollUserTechnicianList = _organizationRoleUserRepository.IncludeMultiple(x => x.Organization, x => x.Role).Where(x => x.IsActive == true && (userId == null || x.UserId == userId) && (x.RoleId == (long)RoleType.Technician)).OrderBy(x => x.Person.FirstName).ToList();
                var organizationRollUserList = organizationRollUserSalesRepList.Concat(organizationRollUserTechnicianList).ToList();
                var orgRoleUserIds = organizationRollUserList.Select(x => x.UserId).ToList();
                var activeUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && !x.IsLocked).Select(x => x.Id).ToList();
                organizationRollUserList = organizationRollUserList.Where(x => activeUers.Contains(x.UserId)).ToList();
                var jobSchedulerList = _jobSchedulerRepository.Table.Where(x => x.IsActive == true && x.StartDate >= currentStartingDateTimeForMonth && x.EndDate <= currentEndDateTimeForMonth).ToList();

                var ownerList = _organizationRepository.Table.Where(x => x.IsActive == true && x.Email != null && (franchiseeId == null || x.Id == franchiseeId)).ToList();
                var OwnerEmailList = ownerList.Where(x => x.Email != null).Select(z => z.Email);
                var SchedulerEmailList = ownerList.Where(x => x.Franchisee != null && x.Franchisee.SchedulerEmail != null).Select(z => z.Franchisee.SchedulerEmail);

                var destinationFolder = MediaLocationHelper.GetAttachmentMediaLocation().Path + "\\";
                var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\");
                var noImageViewPath = destinationFolder + "Templates\\" + "no_image_thumb.gif";

                var organizationRoleUserList = _organizationRoleUserRepository.Table.ToList();
                var jobEstimateData = _jobEstimateRepository.Table.Where(x => x.StartDate >= currentStartingDateTimeForMonth && x.EndDate <= currentEndDateTimeForMonth).ToList();

                foreach (var owner in ownerList)
                {
                    localMarketingViewModel.FranchiseeId = owner.Franchisee.Id;
                    localMarketingViewModel.FranchiseeName = owner.Franchisee.Organization.Name;
                    var RollUserList = organizationRollUserList.Where(x => x.OrganizationId == owner.Franchisee.Id && ((x.RoleId == (long)RoleType.SalesRep) || (x.RoleId == (long)RoleType.Technician))).ToList();
                    foreach (var user in RollUserList)
                    {
                        BeforeAfterPersonsViewModal beforeAfterPersonsViewModal = new BeforeAfterPersonsViewModal();
                        beforeAfterPersonsViewModal.PersonName = user.Person.FirstName + " " + user.Person.LastName;
                        beforeAfterPersonsViewModal.PersonId = user.Person.Id;
                        beforeAfterPersonsViewModal.RoleId = user.RoleId;
                        beforeAfterPersonsViewModal.PersonRole = user.Role != null && user.Role.Name != null ? user.Role.Name : "";
                        beforeAfterPersonsViewModal.Id = user.Id;
                        localMarketingViewModel.BeforeAfterPersonsViewModal.Add(beforeAfterPersonsViewModal);
                    }

                    foreach (var person in localMarketingViewModel.BeforeAfterPersonsViewModal)
                    {
                        var jobScedulerforPerson = jobSchedulerList.Where(x => x.FranchiseeId == owner.Franchisee.Id && x.PersonId == person.PersonId && x.AssigneeId == person.Id && x.StartDate >= localMarketingViewModel.StartDate && x.EndDate <= currentEndDateTimeForMonth && (x.EstimateId != null || x.JobId != null)).ToList();

                        List<JobEstimateForBeforeAfter> jobEstimateForBeforeAfterList = new List<JobEstimateForBeforeAfter>();
                        foreach (var jobScheduler in jobScedulerforPerson)
                        {
                            JobEstimateForBeforeAfter jobEstimateForBeforeAfter = new JobEstimateForBeforeAfter();
                            var beforeImageList = beforeAfter.Where(x => (jobScheduler.EstimateId != null ? jobScheduler.EstimateId == x.EstimateId : jobScheduler.JobId == x.JobId) && x.TypeId == (long)LookupTypes.BeforeWork && x.PairId == null).ToList();
                            //var afterImage = beforeAfter.Where(x => (jobScheduler.EstimateId != null ? jobScheduler.EstimateId == x.EstimateId : jobScheduler.JobId == x.JobId) && x.TypeId == (long)LookupTypes.AfterWork).ToList();
                            jobEstimateForBeforeAfter.BeforeAfterImageViewModal = new List<BeforeAfterImageViewModal>();
                            List<BeforeAfterImageViewModal> beforeAfterImageViewModalList = new List<BeforeAfterImageViewModal>();
                            foreach (var beforeImage in beforeImageList)
                            {

                                BeforeAfterImageViewModal beforeAfterImageViewModal = new BeforeAfterImageViewModal();
                                var afterImage = beforeAfter.FirstOrDefault(x => x.PairId == beforeImage.Id);
                                var scheduler = beforeImage != null ? beforeImage.JobScheduler : null;
                                var isNonResistianCLass = scheduler.Job != null ? (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.FLOORING) || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                                (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                                || scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.UNCLASSIFIED) :
                                (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.FLOORING) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                                (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                                || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.UNCLASSIFIED);

                                var marketingClass = scheduler.Job != null ? scheduler.Job.JobType.Name : scheduler.Estimate.MarketingClass.Name;
                                string linkUrl = "";
                                if (scheduler.JobId != null)
                                {
                                    linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + scheduler.JobId + "/edit/" + scheduler.Id;
                                }
                                else
                                {
                                    linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + scheduler.EstimateId + "/manage/" + scheduler.Id;
                                }
                                var estimateValue = default(decimal);
                                if (scheduler.EstimateId != null && jobEstimateData.Count() > 0)
                                {
                                    estimateValue = jobEstimateData.FirstOrDefault(x => x.Id == scheduler.EstimateId).Amount;
                                }


                                var beforeDatametadataId = beforeImage != null && beforeImage.File != null && beforeImage.File.DataRecorderMetaDataId != null ? beforeImage.File.DataRecorderMetaDataId : default(long);
                                var beforeImageUploadedPersonId = beforeDatametadataId != null ? _dataRecorderMetaDataRepository.Get(beforeDatametadataId) : null;
                                //var beforeImageUploadedPersonId = detaMetaDataList.FirstOrDefault(x => x.Id == beforeDatametadataId).CreatedBy;
                                var beforeImageUploadedPerson = beforeImageUploadedPersonId != null ? organizationRoleUserList.FirstOrDefault(x => x.Id == beforeImageUploadedPersonId.CreatedBy).Person : null;

                                var afterDatametadataId = afterImage != null && afterImage.File != null && afterImage.File.DataRecorderMetaDataId != null ? afterImage.File.DataRecorderMetaDataId : default(long);
                                var afterImageUploadedPersonId = afterDatametadataId != null ? _dataRecorderMetaDataRepository.Get(afterDatametadataId) : null;
                                var afterImageUploadedPerson = afterImageUploadedPersonId != null ? organizationRoleUserList.FirstOrDefault(x => x.Id == afterImageUploadedPersonId.CreatedBy).Person : null;


                                beforeAfterImageViewModal.AfterCss = afterImage != null && afterImage.File != null && afterImage.File.css != null ? afterImage.File.css : "rotate(0)";
                                beforeAfterImageViewModal.BeforeCss = beforeImage.File != null && beforeImage.File.css != null ? beforeImage.File.css : "rotate(0)";
                                beforeAfterImageViewModal.RelactiveLocationAfterImage = afterImage != null && afterImage.File != null && afterImage.File.RelativeLocation != null && afterImage.File.Name != null ? (afterImage.File.RelativeLocation + "\\" + afterImage.File.Name).ToUrl() : noImageViewPath;
                                beforeAfterImageViewModal.RelactiveLocationBeforeImage = beforeImage.File != null && beforeImage.File.RelativeLocation != null && beforeImage.File.Name != null ? (beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name).ToUrl() : noImageViewPath;
                                //beforeAfterImageViewModal.BeforeServiceId
                                //beforeAfterImageViewModal.AfterServiceId
                                beforeAfterImageViewModal.IsBestPicture = beforeImage.IsBestImage;
                                beforeAfterImageViewModal.IsAddToLocalGallery = beforeImage.IsAddToLocalGallery;
                                beforeAfterImageViewModal.AfterImageId = afterImage != null ? afterImage.Id : default(long?);
                                beforeAfterImageViewModal.BeforeImageId = beforeImage.Id;
                                beforeAfterImageViewModal.ServicesType = beforeImage.ServiceType != null ? beforeImage.ServiceType.Name : "";
                                beforeAfterImageViewModal.SurfaceColor = beforeImage.SurfaceColor != null ? beforeImage.SurfaceColor : "";
                                beforeAfterImageViewModal.SurfaceType = beforeImage.SurfaceType != null ? beforeImage.SurfaceType : "";
                                beforeAfterImageViewModal.Description = beforeImage.JobScheduler != null ? beforeImage.JobScheduler.Title : "";


                                beforeAfterImageViewModal.SchedulerUrl = linkUrl;
                                beforeAfterImageViewModal.IsJob = scheduler.JobId != null ? true : false;
                                beforeAfterImageViewModal.Title = scheduler.Title != null ? scheduler.Title : "";
                                beforeAfterImageViewModal.JobId = scheduler.JobId != null ? scheduler.JobId : null;
                                beforeAfterImageViewModal.EstimateId = scheduler.EstimateId != null ? scheduler.EstimateId : null;
                                //beforeAfterImageViewModal.RelactiveLocationExteriorImageUrl = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? "" : "/Content/images/no_image_thumb.gif";
                                beforeAfterImageViewModal.IsComercialClass = isNonResistianCLass;
                                beforeAfterImageViewModal.MarketingClass = marketingClass;
                                beforeAfterImageViewModal.OrderNo = isNonResistianCLass ? 1 : 100;
                                beforeAfterImageViewModal.SchedulerNames = scheduler.Job != null ? "J" + scheduler.JobId : "E" + scheduler.EstimateId;
                                beforeAfterImageViewModal.JobEstimateId = scheduler.Job != null ? scheduler.JobId : scheduler.EstimateId;
                                beforeAfterImageViewModal.CustomerName = scheduler.Job != null ? scheduler.Job.JobCustomer.CustomerName : scheduler.Estimate.JobCustomer.CustomerName;
                                beforeAfterImageViewModal.ToBeGroupedById = scheduler.Estimate != null ? scheduler.EstimateId : scheduler.JobId;
                                beforeAfterImageViewModal.BeforeImageFileId = beforeImage != null && beforeImage.File != null ? beforeImage.File.Id : default(long?);
                                beforeAfterImageViewModal.AfterImageFileId = afterImage != null && afterImage.File != null ? afterImage.File.Id : default(long?);
                                //beforeAfterImageViewModal.ExteriorImageFileId = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? jobEstimateBeforesExteriorImages.File.Id : default(long?);
                                beforeAfterImageViewModal.RelactiveLocationAfterImageUrlThumb = afterImage != null && afterImage.ThumbFileId != null ? (afterImage.ThumbFile.RelativeLocation + "\\" + afterImage.ThumbFile.Name).ToUrl() : noImageViewPath;
                                beforeAfterImageViewModal.RelactiveLocationBeforeImageUrlThumb = beforeImage != null && beforeImage.ThumbFileId != null ? (beforeImage.ThumbFile.RelativeLocation + "\\" + beforeImage.ThumbFile.Name).ToUrl() : noImageViewPath;
                                beforeAfterImageViewModal.S3BucketAfterImageUrlThumb = afterImage != null && afterImage.ThumbFileId != null && afterImage.S3BucketThumbURL != null ? (afterImage.S3BucketThumbURL != null ? afterImage.S3BucketThumbURL : (afterImage.S3BucketURL != null ? afterImage.S3BucketURL : noImageViewPath)) : (afterImage != null && afterImage.ThumbFileId != null && afterImage.File.RelativeLocation != null ? afterImage.File.RelativeLocation + "\\" + afterImage.File.Name : noImageViewPath);
                                beforeAfterImageViewModal.S3BucketBeforeImageUrlThumb = beforeImage != null && beforeImage.ThumbFileId != null && beforeImage.S3BucketThumbURL != null ? (beforeImage.S3BucketThumbURL != null ? beforeImage.S3BucketThumbURL : (beforeImage.S3BucketURL != null ? beforeImage.S3BucketURL : noImageViewPath)) : (beforeImage.File != null && beforeImage.File.RelativeLocation != null ? beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name : noImageViewPath);
                                //beforeAfterImageViewModal.S3BucketExteriorImageUrlThumb = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? jobEstimateBeforesExteriorImages.S3BucketURL : "/Content/images/no_image_thumb.gif";
                                beforeAfterImageViewModal.BeforeImageUploadedOn = beforeImage != null && beforeImage.File != null && beforeImage.File.DataRecorderMetaData != null && beforeImage.File.DataRecorderMetaData.DateCreated != null ? Convert.ToDateTime(_clock.ToUtc(beforeImage.File.DataRecorderMetaData.DateCreated.AddMinutes(30))) : (DateTime?)null;
                                beforeAfterImageViewModal.AfterImageUploadedOn = afterImage != null && afterImage.File != null && afterImage.File.DataRecorderMetaData != null && afterImage.File.DataRecorderMetaData.DateCreated != null ? Convert.ToDateTime(_clock.ToUtc(afterImage.File.DataRecorderMetaData.DateCreated.AddMinutes(30))) : (DateTime?)null;
                                beforeAfterImageViewModal.BeforeImageUploadedBy = beforeImageUploadedPerson != null && beforeImageUploadedPerson.FirstName != null && beforeImageUploadedPerson.LastName != null ? beforeImageUploadedPerson.FirstName + " " + beforeImageUploadedPerson.LastName : "";
                                beforeAfterImageViewModal.AfterImageUploadedBy = afterImageUploadedPerson != null && afterImageUploadedPerson.FirstName != null && afterImageUploadedPerson.LastName != null ? afterImageUploadedPerson.FirstName + " " + afterImageUploadedPerson.LastName : "";
                                beforeAfterImageViewModal.LocalMarketingUrl = _settings.SiteRootUrl + "#/scheduler/beforeAfter/franchiseeAdmin";
                                beforeAfterImageViewModal.EstimateValue = estimateValue != null ? estimateValue : default(decimal);

                                //person.BeforeAfterImageViewModal.Add(beforeAfterImageViewModal);
                                beforeAfterImageViewModalList.Add(beforeAfterImageViewModal);

                            }
                            if (beforeImageList.Count() == 0)
                            {
                                string linkUrl = "";
                                if (jobScheduler.JobId != null)
                                {
                                    linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + jobScheduler.JobId + "/edit/" + jobScheduler.Id;
                                }
                                else
                                {
                                    linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + jobScheduler.EstimateId + "/manage/" + jobScheduler.Id;
                                }
                                var estimateValue = default(decimal);
                                if (jobScheduler.EstimateId != null && jobEstimateData.Count() > 0)
                                {
                                    estimateValue = jobEstimateData.FirstOrDefault(x => x.Id == jobScheduler.EstimateId).Amount;
                                }

                                jobEstimateForBeforeAfter.SchedulerNames = jobScheduler.Job != null ? "J" + jobScheduler.JobId : "E" + jobScheduler.EstimateId;
                                jobEstimateForBeforeAfter.SchedulerUrl = linkUrl;
                                jobEstimateForBeforeAfter.S3BucketBeforeImageUrlThumb = noImageViewPath;
                                jobEstimateForBeforeAfter.S3BucketAfterImageUrlThumb = noImageViewPath;
                                jobEstimateForBeforeAfter.CustomerName = jobScheduler.Job != null ? jobScheduler.Job.JobCustomer.CustomerName : jobScheduler.Estimate.JobCustomer.CustomerName;
                                jobEstimateForBeforeAfter.Title = jobScheduler.Title;
                                jobEstimateForBeforeAfter.LocalMarketingUrl = _settings.SiteRootUrl + "#/scheduler/beforeAfter/franchiseeAdmin";
                                jobEstimateForBeforeAfter.EstimateValue = estimateValue != null ? estimateValue : default(decimal);
                            }
                            jobEstimateForBeforeAfter.BeforeAfterImageViewModal = beforeAfterImageViewModalList;
                            person.JobEstimateForBeforeAfter.Add(jobEstimateForBeforeAfter);
                        }
                    }
                    _logService.Info(string.Format("Data Get SuccessFully For Photo Report"));
                    //beforeAfterPersonsListViewModal = beforeAfterPersonsListViewModal.OrderByDescending(x => x.PersonName).ToList();
                }
                if (OwnerEmailList != null)
                {
                    foreach (var email in OwnerEmailList)
                    {
                        CovertIntoPDF(localMarketingViewModel, NotificationTypes.PhotoReportEmailToFranchiseeOwnerAndScheduler, email);
                    }
                }
                if (SchedulerEmailList != null)
                {
                    foreach (var email in SchedulerEmailList)
                    {
                        CovertIntoPDF(localMarketingViewModel, NotificationTypes.PhotoReportEmailToFranchiseeOwnerAndScheduler, email);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Generating Photo Report PDFs - {0}", ex.Message));
                return false;
            }
        }
        private bool CovertIntoPDF(LocalMarketingViewModel localMarketingViewModel, NotificationTypes notificationType, string email)
        {
            try
            {
                _logService.Info(string.Format("Start Job Converting PDF For Photo Report"));
                var templateName = "";
                var fileName = "";
                var emailId = email;
                //var emailId = "guljar.khan@taazaa.com";
                if (localMarketingViewModel != null)
                {
                    templateName = "photo-report.cshtml";
                    fileName = "Photo_Report_" + DateTime.Now.ToFileTimeUtc() + ".pdf";

                    var destinationFolder = MediaLocationHelper.GetAttachmentMediaLocation().Path + "\\";
                    var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\");
                    var viewPath = destinationFolder + "Templates\\" + templateName;
                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(localMarketingViewModel, destinationFolder, fileName, viewPath);
                    var fileDomain = GetFileModel(file);
                    var model = new BeforeAfterBestPairNotificationModel(_notificationModelFactory.CreateBaseDefault())
                    {
                        AssigneePhone = _settings.OwnerPhone,
                        StartDate = localMarketingViewModel.StartDate.ToString("MM-dd-yyyy"),
                        EndDate = localMarketingViewModel.EndDate.ToString("MM-dd-yyyy"),
                        NavigationUrl = _settings.SiteRootUrl + "#/scheduler/beforeAfter/bestFitMark",
                        PhotoManagemenrURL = _settings.FranchiseeAdminLink
                    };

                    var currentDate = _clock.UtcNow;

                    var resource = new NotificationResource { Resource = fileDomain, ResourceId = fileDomain.Id, IsNew = true };
                    _logService.Info(string.Format("Start Sending Email For Photo Report Job"));
                    _notificationService.QueueUpNotificationEmail(notificationType, model, _settings.CompanyName, _settings.ToSuperAdmin, emailId, _clock.UtcNow, null, new List<NotificationResource> { resource });
                    _unitOfWork.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error In Converting Into PDF - {0}", ex.Message));
                return false;
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

        public bool MapCustomerForJobEstimate()
        {
            try
            {
                var jobcustomerData = _jobCustomerRepository.Table.ToList();
                var customerJobEstimateData = _customerJobEstimateRepository.Table.ToList();
                List<CustomerJobEstimate> customerJobEstimateList = new List<CustomerJobEstimate>();

                jobcustomerData = jobcustomerData.Skip(customerJobEstimateData.Count()).ToList();

                foreach (var item in jobcustomerData)
                {
                    var isPresentInList = customerJobEstimateList.Any(x => x.Address == item.CustomerAddress && x.JobCustomer.Address.City == item.Address.City && x.JobCustomer.Address.State == item.Address.State && x.JobCustomer.Address.Country == item.Address.Country && x.JobCustomer.Address.ZipCode == item.Address.ZipCode);
                    var isPresentInTable = customerJobEstimateData.Any(x => x.Address == item.CustomerAddress && x.JobCustomer.Address.City == item.Address.City && x.JobCustomer.Address.State == item.Address.State && x.JobCustomer.Address.Country == item.Address.Country && x.JobCustomer.Address.ZipCode == item.Address.ZipCode);
                    
                    if (!isPresentInList && !isPresentInTable)
                    {
                        var customerJobEstimate = new CustomerJobEstimate
                        {
                            JobCustomerId = item.Id,
                            DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                            CustomerName = item.CustomerName,
                            Email = item.Email,
                            PhoneNumber = item.PhoneNumber,
                            AddressId = item.AddressId,
                            Address = item.CustomerAddress,
                            CustomerFrom = (long)CustomerComeFromCategory.JobEstimate,
                            IsActive = true,
                            IsDeleted = false,
                            IsNew = true
                        };
                        customerJobEstimateList.Add(customerJobEstimate);
                        _customerJobEstimateRepository.Save(customerJobEstimate);
                        _unitOfWork.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Map Customer For Job/Estimate - {0}", ex.Message));
                return false;
            }
        }

        public bool SendWeeklyEmailNotificationToAlanAndNicolForLocalMarketingReview()
        {
            try
            {
                WeeklyPhotoManagementModel weeklyPhotoManagementModel = new WeeklyPhotoManagementModel();

                DateTime date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                int day = (int)date.DayOfWeek;
                DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                weeklyPhotoManagementModel.StartDate = Monday.AddDays(-7).ToString("MM/dd/yyyy");
                weeklyPhotoManagementModel.EndDate = Sunday.AddDays(-6).ToString("MM/dd/yyyy");
                weeklyPhotoManagementModel.LocalMarketingURL = _settings.FranchiseeAdminLink;

                List<string> toMailList = new List<string>();
                toMailList.Add(_settings.FromEmail);
                toMailList.Add(_settings.MarketingEmail);

                foreach(var item in toMailList)
                {
                    SendMailToAlanAndNicol(weeklyPhotoManagementModel, item);
                }

                return true;
            }
            catch(Exception ex)
            {
                _logService.Info(string.Format("Error in Sending Weekly Email Notification To Alan And Nicol For Local Marketing Review - {0}", ex.Message));
                return false;
            }
        }

        public long SendMailToAlanAndNicol(WeeklyPhotoManagementModel weeklyPhotoManagementModel, string toMailItem)
        {
            var fromMail = _settings.FromEmail; ;
            var toMail = toMailItem;
            EmailForWeeklyPhotoManagementModel model = new EmailForWeeklyPhotoManagementModel(_notificationModelFactory.CreateBaseDefault());
            model.WeeklyPhotoManagementModel = weeklyPhotoManagementModel;
            _notificationService.QueueUpNotificationEmail(NotificationTypes.WeeklyNotificationOfPhotoManagement, model, _settings.CompanyName, fromMail, toMail, _clock.UtcNow, null, null);
            _unitOfWork.SaveChanges();
            return 0;
        }
    }
}