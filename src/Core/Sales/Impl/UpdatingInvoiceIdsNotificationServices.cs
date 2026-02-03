using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Geo.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using Core.Scheduler;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class UpdatingInvoiceIdsNotificationServices : IUpdatingInvoiceIdsNotificationServices
    {
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IJobFactory _jobFactory;
        private readonly IRepository<UpdateMarketingClassfileupload> _updateMarketingClassfileuploadRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        private readonly IRepository<MarketingClass> _marketingClassRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<Zip> _zipRepository;
        private IFileService _fileService;
        private readonly IRepository<InvoiceAddress> _invoiceAddressRepository;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRepository<PaymentItem> _paymentItemRepository;
        private readonly IRepository<InvoicePayment> _invoicePaymentRepository;
        private readonly IRepository<TermsAndConditionFranchisee> _termsAndConditionFranchiseeRepository;
        private readonly IRepository<TermsAndCondition> _termsAndConditionRepository;
        private readonly IRepository<Franchisee> _organizationRepository;
        private readonly IRepository<EstimateInvoiceServiceImage> _estimateInvoiceServiceImageRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<JobEstimateImageCategory> _jobEstimateImageCategory;
        private readonly IRepository<DebuggerLogs> _debuggerLogsRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateServices;
        private readonly IRepository<JobEstimateImage> _jobEstimateImage;
        private readonly IRepository<MarkbeforeAfterImagesHistry> _markbeforeAfterImagesHistryRepository;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        private readonly IRepository<Core.Application.Domain.File> _fileRepository;
        private readonly IDebuggerLog _debuggerService;
        private readonly IEstimateInvoiceServices _estimateInvoiceServices;
        private IBeforeAfterThumbNailService _beforeAfterThumbNameService;

        public UpdatingInvoiceIdsNotificationServices(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock,
            IJobFactory jobFactory, IFileService fileService, IDebuggerLog debuggerService,
            IEstimateInvoiceServices estimateInvoiceServices, IBeforeAfterThumbNailService beforeAfterThumbNameService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _fileService = fileService;
            _stateRepository = unitOfWork.Repository<State>();
            _cityRepository = unitOfWork.Repository<City>();
            _updateMarketingClassfileuploadRepository = unitOfWork.Repository<UpdateMarketingClassfileupload>();
            _jobFactory = jobFactory;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _invoiceRepository = unitOfWork.Repository<Invoice>();
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _marketingClassRepository = unitOfWork.Repository<MarketingClass>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _zipRepository = unitOfWork.Repository<Zip>();
            _invoiceAddressRepository = unitOfWork.Repository<InvoiceAddress>();
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _paymentItemRepository = unitOfWork.Repository<PaymentItem>();
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
            _termsAndConditionFranchiseeRepository = unitOfWork.Repository<TermsAndConditionFranchisee>();
            _termsAndConditionRepository = unitOfWork.Repository<TermsAndCondition>();
            _organizationRepository = unitOfWork.Repository<Franchisee>();
            _estimateInvoiceServiceImageRepository = unitOfWork.Repository<EstimateInvoiceServiceImage>();
            _jobEstimateImageCategory = unitOfWork.Repository<JobEstimateImageCategory>();
            _jobEstimateImage = unitOfWork.Repository<JobEstimateImage>();
            _debuggerLogsRepository = unitOfWork.Repository<DebuggerLogs>();
            _jobEstimateServices = unitOfWork.Repository<JobEstimateServices>();
            _markbeforeAfterImagesHistryRepository = unitOfWork.Repository<MarkbeforeAfterImagesHistry>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
            _fileRepository = unitOfWork.Repository<Core.Application.Domain.File>();
            _debuggerService = debuggerService;
            _estimateInvoiceServices = estimateInvoiceServices;
            _beforeAfterThumbNameService = beforeAfterThumbNameService;
        }
        public void UpdateInvoiceIds()
        {
            //UpdateInvoiceId();
            AddBeforeAfterImagesFromInvoice();
            insertingTermAndConditionFranchisee();
        }


        public void UpdateInvoiceId()
        {
            _unitOfWork.StartTransaction();

            _logService.Info("Starting Updating Invoice Ids Data For TN-Nashville");

            if (!ApplicationManager.Settings.ParseUpdateInvoiceFile)
            {
                _logService.Info("Invoice Ids Updation turned off For TN-Nashville!");
                return;
            }
            var date = new DateTime(2021, 5, 14).Date;
            var franchiseeSalesList = _franchiseeSalesRepository.Table.Where(x => x.FranchiseeId == 45 && x.Invoice != null && (x.Invoice.GeneratedOn < date)).ToList();

            foreach (var franchiseeSales in franchiseeSalesList)
            {
                var prefix = 2018;
                date = new DateTime(2019, 1, 2).Date;
                var date2 = new DateTime(2021, 5, 13).Date;

                if (franchiseeSales.Invoice.GeneratedOn <= date)
                {
                    prefix = 2018;
                }
                else if (franchiseeSales.Invoice.GeneratedOn > date && franchiseeSales.Invoice.GeneratedOn <= date2)
                {
                    prefix = 2020;
                }

                var invoiceDomain = _invoiceRepository.Get(franchiseeSales.InvoiceId.Value);
                var newInvoiceId = prefix + "" + invoiceDomain.Id;
                var newQbInvoiceId = prefix + "" + franchiseeSales.QbInvoiceNumber;
                var newInvoiceIdString = prefix + "_" + invoiceDomain.Id;
                var newQbInvoiceIdString = prefix + "_" + franchiseeSales.QbInvoiceNumber;
                invoiceDomain.CustomerInvoiceId = long.Parse(newInvoiceId);
                invoiceDomain.CustomerInvoiceIdString = newInvoiceIdString;

                var newQbId = prefix + "" + franchiseeSales.QbInvoiceNumber;
                var newQbIdString = prefix + "_" + franchiseeSales.QbInvoiceNumber;
                invoiceDomain.CustomerQbInvoiceId = long.Parse(newQbId);
                invoiceDomain.CustomerQbInvoiceIdString = newQbIdString;


                _invoiceRepository.Save(invoiceDomain);


                var newFranchiseeInvoiceId = prefix + "" + franchiseeSales.InvoiceId;
                newInvoiceIdString = prefix + "_" + franchiseeSales.Id;

                franchiseeSales.CustomerInvoiceId = long.Parse(newFranchiseeInvoiceId);
                franchiseeSales.CustomerInvoiceIdString = newInvoiceIdString;

                newQbId = prefix + "" + franchiseeSales.QbInvoiceNumber;
                newQbIdString = prefix + "_" + franchiseeSales.QbInvoiceNumber;
                franchiseeSales.CustomerQbInvoiceId = long.Parse(newQbId);
                franchiseeSales.CustomerQbInvoiceIdString = newQbIdString;

                _franchiseeSalesRepository.Save(franchiseeSales);
                _unitOfWork.SaveChanges();
            }
        }

        public void insertingTermAndConditionFranchisee()
        {
            _unitOfWork.StartTransaction(); ;
            var organizationList = _organizationRepository.Table.ToList();
            var termsAndConditionsTemplates = _termsAndConditionRepository.Table.ToList();
            foreach (var organization in organizationList)
            {
                var isPresent = _termsAndConditionFranchiseeRepository.Table.Any(x => x.FranchiseeId == organization.Id);

                if (!isPresent)
                {
                    _logService.Info("Adding Term And Condition For FranchiseeId: !" + organization.Id);
                    var concreteTermAndCondition = termsAndConditionsTemplates.FirstOrDefault(x => x.TyepeId == 240);
                    var termAndConditionFranchisee = new TermsAndConditionFranchisee()
                    {
                        FranchiseeId = organization.Id,
                        TermAndCondition = concreteTermAndCondition.TermAndCondition,
                        TyepeId = 240,
                        IsNew = true

                    };
                    _termsAndConditionFranchiseeRepository.Save(termAndConditionFranchisee);

                    var otherTermAndCondition = termsAndConditionsTemplates.FirstOrDefault(x => x.TyepeId == 241);
                    termAndConditionFranchisee = new TermsAndConditionFranchisee()
                    {
                        FranchiseeId = organization.Organization.Id,
                        TermAndCondition = otherTermAndCondition.TermAndCondition,
                        TyepeId = 241,
                        IsNew = true

                    };
                    _termsAndConditionFranchiseeRepository.Save(termAndConditionFranchisee);
                }
            }
            _unitOfWork.SaveChanges();
        }


        public void AddBeforeAfterImagesFromInvoice()
        {
            var logger = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            var images = _estimateInvoiceServiceImageRepository.Table.Where(x => x.IsBeforeAfter == true).OrderByDescending(x => x.Id).ToList();
            foreach (var image in images)
            {
                try
                {
                    var categoryId = default(long);
                    var pairId = default(long?);
                    var debuggerLogModelList = new List<DebuggerLogModel>();
                    var debuggerLogModel = new DebuggerLogModel();
                    string debuggerLogs = "";
                    if (image.DataRecorderMetaData.CreatedBy == 1051)
                    {
                        image.DataRecorderMetaData.CreatedBy = 1;
                    }
                    var schedulerId = image.EstimateInvoice != null ? image.EstimateInvoice.SchedulerId : 0;
                    //checking if the cateory exists.
                    var jobEstimateCategoryExists = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.EstimateId == image.EstimateId && x.SchedulerId == schedulerId);
                    var jobEstimateCategory = new JobEstimateImageCategory();

                    //create a new category for the estimate if its not there else use already existing category
                    if (jobEstimateCategoryExists == null)
                    {
                        jobEstimateCategory = new JobEstimateImageCategory()
                        {
                            EstimateId = image.EstimateId,
                            MarkertingClassId = image.Estimate != null ? image.Estimate.TypeId : 0,
                            IsNew = true,
                            SchedulerId = image.EstimateInvoice != null ? image.EstimateInvoice.SchedulerId : 0
                        };
                        _jobEstimateImageCategory.Save(jobEstimateCategory);
                        categoryId = jobEstimateCategory.Id;
                    }
                    else
                    {
                        categoryId = jobEstimateCategoryExists.Id;
                        jobEstimateCategory = jobEstimateCategoryExists;
                    }

                    //creating a model which has the images.
                    JobEstimateCategoryViewModel model = new JobEstimateCategoryViewModel();
                    model.EstimateId = image.EstimateId;
                    model.SchedulerId = jobEstimateCategory.SchedulerId;
                    ImagePairs pairs = new ImagePairs();
                    pairs.BeforeImages = CreateBeforeImageModel(image, categoryId);
                    pairs.AfterImages = CreateAfterImageModel(image, categoryId);
                    model.ImagePairs = new List<ImagePairs>();
                    model.ImagePairs.Add(pairs);
                    model.UserId = image.DataRecorderMetaData.CreatedBy;

                    //looping through each image(we will always have one here)
                    foreach (var imagePair in model.ImagePairs)
                    {
                        // Saving Before Images
                        debuggerLogModel.Description = "";
                        debuggerLogModel.ActionId = (long?)DebuggerLogType.ADDINGNEWVALUE;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;
                        debuggerLogModel = new DebuggerLogModel();
                        debuggerLogModel.JobEstimateimageCategoryId = categoryId;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;

                        //Creating jobestimateimageservice for each before image
                        var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(imagePair.BeforeImages, categoryId, imagePair.BeforeImages.TypeId != null ? imagePair.BeforeImages.TypeId : (long?)LookupTypes.BeforeWork);

                        debuggerLogModel.Description += _debuggerService.CreateDebugger(jobEstimateBeforeService, null, "Before", out debuggerLogs);

                        _jobEstimateServices.Save(jobEstimateBeforeService);

                        debuggerLogModel.JobEstimateServiceCategoryId = jobEstimateBeforeService.Id;

                        //getting pair id to use it in after image
                        pairId = jobEstimateBeforeService.Id;
                        var beforeServiceId = jobEstimateBeforeService.Id;

                        //creating jobestimateimage for each file
                        foreach (var fileId in imagePair.BeforeImages.FilesList)
                        {
                            var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(imagePair.BeforeImages, beforeServiceId, imagePair.BeforeImages.TypeId != null ? imagePair.BeforeImages.TypeId : (long?)LookupTypes.BeforeWork, fileId);
                            jobEstimateBeforeServiceImage.IsBestImage = false;
                            debuggerLogModel.Description += _debuggerService.CreateDebuggerForImage(jobEstimateBeforeServiceImage, null, "Before", out debuggerLogs);
                            _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                            var fileDomainLocal = _fileRepository.Get(jobEstimateBeforeServiceImage.FileId.GetValueOrDefault());
                            if (fileDomainLocal != null)
                            {
                                var fileModel = _beforeAfterThumbNameService.CreateImageThumb(fileDomainLocal, "");
                                imagePair.BeforeImages.ThumbFileId = fileModel.FileId;
                                jobEstimateBeforeServiceImage.ThumbFileId = fileModel.FileId;
                                jobEstimateBeforeServiceImage.IsNew = false;
                                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                                debuggerLogModel.Description += " Adding Before ThumbNail ImageUrl: " + fileModel.FullFilePath;
                            }
                            if (fileDomainLocal == null)
                            {
                                jobEstimateBeforeServiceImage.ThumbFileId = null;
                                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                            }
                            imagePair.BeforeImages.Id = jobEstimateBeforeService.Id;
                            if (fileId != null)
                            {
                                var fileDomain = _fileRepository.Get(jobEstimateBeforeServiceImage.FileId.Value);
                                debuggerLogModel.Description += " Adding Before ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                                _fileRepository.Save(fileDomain);
                            }
                        }

                        var jobEstimateCategorydomain = _jobEstimateImageCategory.IncludeMultiple(x => x.JobScheduler).FirstOrDefault(x => x.Id == debuggerLogModel.JobEstimateimageCategoryId);
                        debuggerLogModel.TypeId = (long?)LookupTypes.BeforeWork;
                        debuggerLogModel.ActionId = (long?)DebuggerLogType.ADDINGNEWVALUE;
                        debuggerLogModel.FranchiseeId = jobEstimateCategorydomain.JobScheduler.FranchiseeId;
                        if(model.UserId == null || model.UserId == 0)
                        {
                            model.UserId = 1;
                        }
                        debuggerLogModel.UserId = model.UserId;
                        debuggerLogModel.JobSchedulerId = jobEstimateCategory.JobScheduler.Id;
                        if (debuggerLogModel.Description.Length > 0)
                        {
                            debuggerLogModelList.Add(debuggerLogModel);
                        }

                        //Saving After Images
                        debuggerLogModel = new DebuggerLogModel();
                        debuggerLogModel.Description = "";
                        debuggerLogModel.ActionId = (long?)DebuggerLogType.ADDINGNEWVALUE;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;
                        debuggerLogModel.JobEstimateimageCategoryId = categoryId;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;

                        //Creating jobestimateimageservice for each after image using pair Id as the before image jobestimateimageserviceId
                        jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(imagePair.AfterImages, categoryId, imagePair.AfterImages.TypeId != null ? imagePair.AfterImages.TypeId : (long?)LookupTypes.AfterWork);
                        jobEstimateBeforeService.PairId = pairId;

                        debuggerLogModel.Description += _debuggerService.CreateDebugger(jobEstimateBeforeService, null, "After", out debuggerLogs);

                        _jobEstimateServices.Save(jobEstimateBeforeService);
                        debuggerLogModel.JobEstimateServiceCategoryId = jobEstimateBeforeService.Id;

                        var afterServiceId = jobEstimateBeforeService.Id;
                        if (imagePair.AfterImages.FilesList.Count() == 0)
                        {
                            imagePair.AfterImages.Id = afterServiceId;
                        }
                        debuggerLogModel.FranchiseeId = jobEstimateCategory.JobScheduler.FranchiseeId;
                        debuggerLogModel.TypeId = (long?)LookupTypes.AfterWork;
                        if (model.UserId == null || model.UserId == 0)
                        {
                            model.UserId = 1;
                        }
                        debuggerLogModel.UserId = model.UserId;
                        debuggerLogModel.JobSchedulerId = jobEstimateCategory.JobScheduler.Id;
                        if (debuggerLogModel.Description.Length > 0)
                        {
                            debuggerLogModelList.Add(debuggerLogModel);
                        }
                    }
                    SaveDebuggerLog(debuggerLogModelList);
                    _unitOfWork.SaveChanges();
                    model.Id = categoryId;
                    AddBeforeAfterImages(model);

                    image.IsBeforeAfter = false;
                    image.IsNew = false;
                    _estimateInvoiceServiceImageRepository.Save(image);
                }
                catch (Exception e)
                {
                    logger.Error("Starting services", e);
                }
            }

        }
        private bool SaveDebuggerLog(List<DebuggerLogModel> list)
        {
            var userId = list.Select(x => x.UserId).ToList();
            var orgRoleUserIdList = _organizationRoleUserRepository.Table.Where(x => userId.Contains(x.UserId)).ToList();
            foreach (var debuggerModel in list)
            {
                var domain = new DebuggerLogs()
                {
                    JobestimateimagecategoryId = debuggerModel.JobEstimateimageCategoryId,
                    JobestimateservicesId = debuggerModel.JobEstimateServiceCategoryId,
                    Description = debuggerModel.Description,
                    PageId = debuggerModel.PageId,
                    UserId = debuggerModel.UserId,
                    FranchiseeId = debuggerModel.FranchiseeId,
                    IsNew = debuggerModel.Id > 0 ? false : true,
                    Id = debuggerModel.Id > 0 ? debuggerModel.Id : 0,
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    ActionId = debuggerModel.ActionId,
                    TypeId = debuggerModel.TypeId,
                    JobSchedulerId = debuggerModel.JobSchedulerId
                };
                var orgRoleUserId = orgRoleUserIdList.FirstOrDefault(x => x.UserId == debuggerModel.UserId);
                if(orgRoleUserId != null)
                {
                    domain.UserId = orgRoleUserId.UserId;
                }
                else
                {
                    domain.UserId = 1;
                }
                _debuggerLogsRepository.Save(domain);
            }
            return true;
        }
        private JobEstimateServiceViewModel CreateBeforeImageModel(EstimateInvoiceServiceImage image, long categoryId)
        {
            JobEstimateServiceViewModel jobEstimateServiceViewModel = new JobEstimateServiceViewModel();
            var estimateInvoiceService = image.EstimateInvoiceService;
            if (estimateInvoiceService != null)
            {
                var locations = new List<String>();
                if (estimateInvoiceService.Location != null)
                {
                    locations = estimateInvoiceService.Location.Split(',').ToList();
                }

                var location = string.Empty;
                if (locations.Count > 0)
                {
                    location = locations[0];
                }
                else
                {
                    location = "Kitchen";
                }
                var serviceType = new ServiceType();
                serviceType = _serviceTypeRepository.Table.FirstOrDefault(x => x.Name == estimateInvoiceService.ServiceType);
                if(serviceType == null)
                {
                    serviceType = _serviceTypeRepository.Table.FirstOrDefault(x => x.Id.ToString() == estimateInvoiceService.ServiceType);
                }
                jobEstimateServiceViewModel.IsGroutilife = serviceType.Name == "GROUTLIFE" ? true : false;
                jobEstimateServiceViewModel.SurfaceMaterial = !String.IsNullOrEmpty(estimateInvoiceService.StoneType2) ? GetSurfaceMaterialForBeforeAfter(estimateInvoiceService.StoneType2) : "Stonelife:marble";
                jobEstimateServiceViewModel.SurfaceType = !String.IsNullOrEmpty(estimateInvoiceService.TypeOfService) ? estimateInvoiceService.TypeOfService : "Floor";
                jobEstimateServiceViewModel.SurfaceColor = !String.IsNullOrEmpty(estimateInvoiceService.StoneColor) ? estimateInvoiceService.StoneColor : null;
                jobEstimateServiceViewModel.FinishMaterial = !String.IsNullOrEmpty(estimateInvoiceService.StoneType) ? estimateInvoiceService.StoneType : null;
                jobEstimateServiceViewModel.FilesList.Add(image.File.Id);
                jobEstimateServiceViewModel.CategoryId = categoryId;
                jobEstimateServiceViewModel.BuildingLocation = !string.IsNullOrEmpty(location) ? location : "Kitchen";
                jobEstimateServiceViewModel.ServiceTypeId = serviceType.Id;
                jobEstimateServiceViewModel.IsBeforeImage = true;
                jobEstimateServiceViewModel.ImagesInfo = GetFileModel(image);
            }
            return jobEstimateServiceViewModel;
        }
        private JobEstimateServiceViewModel CreateAfterImageModel(EstimateInvoiceServiceImage image, long categoryId)
        {
            JobEstimateServiceViewModel jobEstimateServiceViewModel = new JobEstimateServiceViewModel();
            var estimateInvoiceService = image.EstimateInvoiceService;
            if (estimateInvoiceService != null)
            {
                var locations = new List<String>();
                if (estimateInvoiceService.Location != null)
                {
                   locations = estimateInvoiceService.Location.Split(',').ToList();
                }
               
                var location = string.Empty;
                if (locations.Count > 0)
                {
                    location = locations[0];
                }
                else
                {
                    location = "Kitchen";
                }
                var serviceType = new ServiceType();
                serviceType = _serviceTypeRepository.Table.FirstOrDefault(x => x.Name == estimateInvoiceService.ServiceType);
                if (serviceType == null)
                {
                    serviceType = _serviceTypeRepository.Table.FirstOrDefault(x => x.Id.ToString() == estimateInvoiceService.ServiceType);
                }
                jobEstimateServiceViewModel.IsGroutilife = serviceType.Name == "GROUTLIFE" ? true : false;
                jobEstimateServiceViewModel.SurfaceMaterial = !String.IsNullOrEmpty(estimateInvoiceService.StoneType2) ? GetSurfaceMaterialForBeforeAfter(estimateInvoiceService.StoneType2) : "Stonelife:marble";
                jobEstimateServiceViewModel.SurfaceType = !String.IsNullOrEmpty(estimateInvoiceService.TypeOfService) ? estimateInvoiceService.TypeOfService : "Floor";
                jobEstimateServiceViewModel.SurfaceColor = !String.IsNullOrEmpty(estimateInvoiceService.StoneColor) ? estimateInvoiceService.StoneColor : null;
                jobEstimateServiceViewModel.FinishMaterial = !String.IsNullOrEmpty(estimateInvoiceService.StoneType) ? estimateInvoiceService.StoneType : null;
                jobEstimateServiceViewModel.CategoryId = categoryId;
                jobEstimateServiceViewModel.BuildingLocation = !string.IsNullOrEmpty(location) ? location : "Kitchen";
                jobEstimateServiceViewModel.ServiceTypeId = serviceType.Id;
                jobEstimateServiceViewModel.IsBeforeImage = true;
            }
            return jobEstimateServiceViewModel;
        }

        private List<FileModel> GetFileModel(EstimateInvoiceServiceImage image)
        {
            List<FileModel> model = new List<FileModel>();
            var imageFile = image.File;
            FileModel file = new FileModel();
            file.Name = imageFile.Name;
            file.Caption = imageFile.Caption;
            file.RelativeLocation = imageFile.RelativeLocation;
            file.Size = imageFile.Size;
            file.MimeType = imageFile.MimeType;
            file.FileId = imageFile.Id;
            file.css = imageFile.css;
            file.ThumbFileId = imageFile.Id;
            model.Add(file);
            return model;
        }

        private string GetSurfaceMaterialForBeforeAfter(string surfaceMaterial)
        {
            switch (surfaceMaterial)
            {
                case "Marble":
                    return "Stonelife:marble";
                case "Granite":
                    return "stonelife:granite";
                case "Ceramic":
                    return "groutlife:ceramic (tile and grout)";
                case "Travertine":
                    return "stonelife:travertine";
                case "Terrazzo":
                    return "Enduracrete:terrazzo";
                case "Porcelain":
                    return "groutlife:porcelain";
                case "Vinyl":
                    return "vinylguard:vinyl";
                case "Wood":
                    return "wood";
                case "Slate":
                    return "Slate";
                case "Metal":
                    return "Metal";
                case "Carpet":
                    return "Carpet";
                case "Glass":
                    return "Glass";
                case "Limestone":
                    return "Limestone";
                case "Other":
                    return "Other";
                default:
                    return "Stonelife:marble";
            }
        }

        private bool AddBeforeAfterImages(JobEstimateCategoryViewModel model)
        {
            var jobEstimateBeforeImage = new JobEstimateImage();
            var jobEstimateServiceBeforeDomain = new JobEstimateServices();
            var jobEstimateAfterImage = new JobEstimateImage();
            var jobEstimateServiceAfterDomain = new JobEstimateServices();
            var jobEstimateCategory = new JobEstimateImageCategory();
            var jobEstimateCategoryDomain = _jobEstimateImageCategory.IncludeMultiple(x => x.JobScheduler).FirstOrDefault(x => x.Id == model.Id.Value);
            var personName = jobEstimateCategoryDomain.JobScheduler.Person.FirstName + " " + jobEstimateCategoryDomain.JobScheduler.Person.LastName;
            var beforeAfterImagesBeforeDomain = new BeforeAfterImages();
            var isNewData = false;
            jobEstimateCategory = _jobFactory.CreateJobEstimateCategory(model);
            foreach (var imagePair in model.ImagePairs)
            {
                jobEstimateAfterImage = new JobEstimateImage();
                jobEstimateServiceBeforeDomain = _jobFactory.CreateJobEstimatePairing(imagePair.BeforeImages, imagePair.BeforeImages.CategoryId.GetValueOrDefault(), (long?)LookupTypes.BeforeWork);
                if (imagePair.BeforeImages.ImagesInfo.Count() > 0)
                {
                    jobEstimateBeforeImage = _jobFactory.CreateJobEstimateImageEditDomain(imagePair.BeforeImages, jobEstimateServiceBeforeDomain.Id, (long?)LookupTypes.BeforeWork, imagePair.BeforeImages.ImagesInfo[0]);
                }
                var beforeAfterBeforeImages = CreateDomainModel(jobEstimateCategoryDomain, jobEstimateServiceBeforeDomain, jobEstimateBeforeImage);

                var beforeAfterBeforeImagesDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.CategoryId == jobEstimateCategoryDomain.Id && x.ServiceId == jobEstimateServiceBeforeDomain.Id);
                var beforeAfterImageCheckingForFileId = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.FileId == imagePair.BeforeImages.FileId && x.SchedulerId == jobEstimateCategoryDomain.SchedulerId);
                if (beforeAfterBeforeImagesDomain == null)
                {
                    beforeAfterBeforeImagesDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.ServiceId == jobEstimateServiceBeforeDomain.Id);
                }
                if (beforeAfterBeforeImagesDomain != null)
                {
                    beforeAfterBeforeImages.Id = beforeAfterBeforeImagesDomain.Id;
                    beforeAfterBeforeImages.IsNew = false;
                    beforeAfterBeforeImages.DataRecorderMetaDataId = beforeAfterBeforeImagesDomain.DataRecorderMetaDataId;
                    beforeAfterBeforeImages.DataRecorderMetaData = null;
                    beforeAfterBeforeImages.JobId = beforeAfterBeforeImagesDomain.JobId;
                    beforeAfterBeforeImages.EstimateId = beforeAfterBeforeImagesDomain.EstimateId;
                }
                else if (beforeAfterImageCheckingForFileId != null)
                {
                    beforeAfterBeforeImages.Id = beforeAfterImageCheckingForFileId.Id;
                    beforeAfterBeforeImages.IsNew = false;
                    beforeAfterBeforeImages.DataRecorderMetaDataId = beforeAfterImageCheckingForFileId.DataRecorderMetaDataId;
                    beforeAfterBeforeImages.DataRecorderMetaData = null;
                    beforeAfterBeforeImages.JobId = beforeAfterImageCheckingForFileId.JobId;
                    beforeAfterBeforeImages.EstimateId = beforeAfterImageCheckingForFileId.EstimateId;
                }
                if (beforeAfterBeforeImages.IsNew)
                {
                    isNewData = true;
                }
                if (beforeAfterBeforeImages.ServiceId == 0)
                {
                    beforeAfterBeforeImages.ServiceId = null;
                }
                _beforeAfterImagesRepository.Save(beforeAfterBeforeImages);
                if (isNewData)
                {
                    isNewData = false;
                }
                jobEstimateServiceAfterDomain = _jobFactory.CreateJobEstimatePairing(imagePair.AfterImages, imagePair.AfterImages.CategoryId.GetValueOrDefault(), (long?)LookupTypes.AfterWork);
                if (imagePair.AfterImages.ImagesInfo.Count() > 0)
                {
                    jobEstimateAfterImage = _jobFactory.CreateJobEstimateImageEditDomain(imagePair.AfterImages, jobEstimateServiceBeforeDomain.Id, (long?)LookupTypes.AfterWork, imagePair.AfterImages.ImagesInfo[0]);
                }
                var beforeAfterImagesAfterDomain = CreateDomainModel(jobEstimateCategoryDomain, jobEstimateServiceAfterDomain, jobEstimateAfterImage);

                beforeAfterImagesAfterDomain.PairId = beforeAfterBeforeImages.Id;
                beforeAfterBeforeImagesDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.CategoryId == jobEstimateCategoryDomain.Id && x.ServiceId == jobEstimateServiceAfterDomain.Id);
                beforeAfterImageCheckingForFileId = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.FileId == imagePair.AfterImages.FileId && x.SchedulerId == jobEstimateCategoryDomain.SchedulerId && x.ServiceId == beforeAfterImagesAfterDomain.ServiceId);
                if (beforeAfterBeforeImagesDomain == null)
                {
                    beforeAfterBeforeImagesDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.ServiceId == jobEstimateServiceAfterDomain.Id);
                }
                if (beforeAfterBeforeImagesDomain != null)
                {
                    beforeAfterImagesAfterDomain.Id = beforeAfterBeforeImagesDomain.Id;
                    beforeAfterImagesAfterDomain.IsNew = false;
                    beforeAfterImagesAfterDomain.DataRecorderMetaDataId = beforeAfterBeforeImagesDomain.DataRecorderMetaDataId;
                    beforeAfterImagesAfterDomain.DataRecorderMetaData = null;
                }
                else if (beforeAfterImageCheckingForFileId != null)
                {
                    beforeAfterImagesAfterDomain.Id = beforeAfterImageCheckingForFileId.Id;
                    beforeAfterImagesAfterDomain.IsNew = false;
                    beforeAfterImagesAfterDomain.DataRecorderMetaDataId = beforeAfterImageCheckingForFileId.DataRecorderMetaDataId;
                    beforeAfterImagesAfterDomain.DataRecorderMetaData = null;
                }
                if (beforeAfterImagesAfterDomain.ServiceId == 0)
                {
                    beforeAfterBeforeImagesDomain.ServiceTypeId = null;
                }
                if (beforeAfterImagesAfterDomain.IsNew)
                {
                    isNewData = true;
                }
                _beforeAfterImagesRepository.Save(beforeAfterImagesAfterDomain);

            }
            return false;
        }

        private BeforeAfterImages CreateDomainModel(JobEstimateImageCategory jobEstimateImageCategory, JobEstimateServices jobEstimateServices,
             JobEstimateImage jobEstimateImage)
        {
            var beforeAfterImages = new BeforeAfterImages()
            {
                MAIDJANITORIAL = jobEstimateServices != null ? jobEstimateServices.MAIDJANITORIAL : "",
                MaidService = jobEstimateServices != null ? jobEstimateServices.MaidService : "",
                AddToGalleryDateTime = jobEstimateImage != null ? jobEstimateImage.AddToGalleryDateTime : default(DateTime?),
                BuildingLocation = jobEstimateServices != null ? jobEstimateServices.BuildingLocation : "",
                FinishMaterial = jobEstimateServices != null ? jobEstimateServices.FinishMaterial : "",
                CompanyName = jobEstimateServices != null ? jobEstimateServices.CompanyName : "",
                FloorNumber = jobEstimateServices != null ? jobEstimateServices.FloorNumber : 0,
                MarkertingClassId = jobEstimateImageCategory != null ? jobEstimateImageCategory.MarkertingClassId : default(long?),
                BestFitMarkDateTime = jobEstimateImage != null ? jobEstimateImage.BestFitMarkDateTime : default(DateTime?),
                CategoryId = jobEstimateImageCategory != null ? jobEstimateImageCategory.Id : default(long?),
                EstimateId = jobEstimateImageCategory.EstimateId,
                JobId = jobEstimateImageCategory.JobId,
                SchedulerId = jobEstimateImageCategory.SchedulerId,
                FileId = jobEstimateImage != null ? jobEstimateImage.FileId : default(long?),
                IsBeforeImage = jobEstimateImage != null ? jobEstimateImage.IsBestImage : default(bool?),
                IsAddToLocalGallery = jobEstimateImage != null ? jobEstimateImage.IsAddToLocalGallery : default(bool),
                DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                ServiceId = jobEstimateServices != null ? jobEstimateServices.Id : default(long?),
                PairId = null,
                SurfaceColor = jobEstimateServices != null ? jobEstimateServices.SurfaceColor : "",
                PropertyManager = jobEstimateServices != null ? jobEstimateServices.PropertyManager : "",
                SurfaceMaterial = jobEstimateServices != null ? jobEstimateServices.SurfaceMaterial : "",
                SurfaceType = jobEstimateServices != null ? jobEstimateServices.SurfaceType : "",
                Id = 0,
                IsNew = true,
                PersonName = jobEstimateImageCategory.JobScheduler.Person.FirstName + " " + jobEstimateImageCategory.JobScheduler.Person.LastName,
                FranchiseeId = jobEstimateImageCategory.JobScheduler.FranchiseeId,
                RoleId = jobEstimateImageCategory.JobScheduler != null && jobEstimateImageCategory.JobScheduler.OrganizationRoleUser != null ? jobEstimateImageCategory.JobScheduler.OrganizationRoleUser.RoleId : default(long?),
                UserId = jobEstimateImageCategory.JobScheduler != null && jobEstimateImageCategory.JobScheduler.OrganizationRoleUser != null ? jobEstimateImageCategory.JobScheduler.OrganizationRoleUser.UserId : default(long?),
                ServiceTypeId = jobEstimateServices != null ? jobEstimateServices.ServiceTypeId : default(long?),
                TypeId = jobEstimateServices != null ? jobEstimateServices.TypeId : default(long?),
                ThumbFileId = jobEstimateImage != null ? jobEstimateImage.ThumbFileId : default(long?),

            };
            return beforeAfterImages;
        }
    }
}
