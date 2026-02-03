using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class EstimateInvoiceServices : IEstimateInvoiceServices
    {
        private readonly List<string> fileNameList = new List<string>();
        private readonly ISendNewJobNotificationtoTechService _sendNewJobNotificationToTechService;
        private readonly IWorkOrderTechnicianService _workOrderTechnicianService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<EstimateInvoice> _estimateInvoiceRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<EstimateInvoiceCustomer> _estimateInvoiceCustomerRepository;
        private readonly IRepository<EstimateInvoiceService> _estimateInvoiceServiceRepository;
        private readonly IRepository<EstimateInvoiceServiceDescription> _estimateInvoiceServiceDescriptionRepository;
        private readonly IEstimateInvoiceFactory _estimateInvoiceFactory;
        private readonly IRepository<MarketingClass> _marketingClassRepository;
        private readonly IPdfFileService _pdfFileService;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IRepository<TermsAndConditionFranchisee> _termsAndConditionFranchiseeRepository;
        private readonly IRepository<EstimateServiceInvoiceNotes> _estimateServiceInvoiceNotesRepository;
        private readonly IRepository<CustomerSignatureInfo> _customerSignatureInfoRepository;
        private readonly IRepository<CustomerSignature> _customerSignatureRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<TechnicianWorkOrder> _technicianWorkOrderRepository;
        private readonly IRepository<TechnicianWorkOrderForInvoice> _technicianWorkOrderInvoiceRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateServices;
        private readonly IRepository<JobEstimateImage> _jobEstimateImage;
        private readonly IRepository<JobEstimateImageCategory> _jobEstimateImageCategory;
        private readonly IRepository<EstimateInvoiceDimension> _estimateInvoiceDimensionRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<EstimateInvoiceAssignee> _estimateInvoiceAssigneeRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<Lookup> _lookupRepository;
        private ISettings _settings;
        private readonly IClock _clock;
        private readonly IJobFactory _jobFactory;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IFileService _fileService;
        private readonly IRepository<HoningMeasurement> _honingMeasurementRepository;
        private readonly IRepository<HoningMeasurementDefault> _honingMeasurementDefaultRepository;
        private readonly IRepository<MaintenanceCharges> _maintenanceChargesRepository;
        private readonly IRepository<ServicesTag> _servicesTagRepository;
        private ILogService _logService;
        private readonly IRepository<JobEstimate> _jobEstimateRepository;
        private readonly IRepository<EstimateInvoiceServiceImage> _estimateInvoiceServiceImageRepository;
        private readonly IRepository<JobCustomer> _jobCustomerRepository;
        public EstimateInvoiceServices(IUnitOfWork unitOfWork, IEstimateInvoiceFactory estimateInvoiceFactory, IPdfFileService pdfFileService,
             ISendNewJobNotificationtoTechService sendNewJobNotificationToTechService, ISettings settings, IClock clock, IWorkOrderTechnicianService workOrderTechnicianService, IJobFactory jobFactory, IFileService fileService, ILogService logService)
        {
            _logService = logService;
            _jobFactory = jobFactory;
            _clock = clock;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _estimateInvoiceRepository = unitOfWork.Repository<EstimateInvoice>();
            _estimateInvoiceCustomerRepository = unitOfWork.Repository<EstimateInvoiceCustomer>();
            _estimateInvoiceServiceRepository = unitOfWork.Repository<EstimateInvoiceService>();
            _estimateInvoiceFactory = estimateInvoiceFactory;
            _estimateInvoiceServiceDescriptionRepository = unitOfWork.Repository<EstimateInvoiceServiceDescription>();
            _marketingClassRepository = unitOfWork.Repository<MarketingClass>();
            _pdfFileService = pdfFileService;
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _sendNewJobNotificationToTechService = sendNewJobNotificationToTechService;
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _termsAndConditionFranchiseeRepository = unitOfWork.Repository<TermsAndConditionFranchisee>();
            _estimateServiceInvoiceNotesRepository = unitOfWork.Repository<EstimateServiceInvoiceNotes>();
            _customerSignatureInfoRepository = unitOfWork.Repository<CustomerSignatureInfo>();
            _customerSignatureRepository = unitOfWork.Repository<CustomerSignature>();
            _jobRepository = unitOfWork.Repository<Job>();
            _technicianWorkOrderRepository = unitOfWork.Repository<TechnicianWorkOrder>();
            _technicianWorkOrderInvoiceRepository = unitOfWork.Repository<TechnicianWorkOrderForInvoice>();
            _workOrderTechnicianService = workOrderTechnicianService;
            _jobEstimateServices = unitOfWork.Repository<JobEstimateServices>();
            _jobEstimateImage = unitOfWork.Repository<JobEstimateImage>();
            _jobEstimateImageCategory = unitOfWork.Repository<JobEstimateImageCategory>();
            _estimateInvoiceDimensionRepository = unitOfWork.Repository<EstimateInvoiceDimension>();
            _estimateInvoiceAssigneeRepository = unitOfWork.Repository<EstimateInvoiceAssignee>();
            _emailTemplateRepository = unitOfWork.Repository<EmailTemplate>();
            _personRepository = unitOfWork.Repository<Person>();
            _fileService = fileService;
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _honingMeasurementRepository = unitOfWork.Repository<HoningMeasurement>();
            _maintenanceChargesRepository = unitOfWork.Repository<MaintenanceCharges>();
            _servicesTagRepository = unitOfWork.Repository<ServicesTag>();
            _jobEstimateRepository = unitOfWork.Repository<JobEstimate>();
            _estimateInvoiceServiceImageRepository = unitOfWork.Repository<EstimateInvoiceServiceImage>();
            _honingMeasurementDefaultRepository = unitOfWork.Repository<HoningMeasurementDefault>();
            _jobCustomerRepository = unitOfWork.Repository<JobCustomer>();
        }

        public EstimateInvoiceViewModel GetEstimateInvoice(InvoiceEstimateFilterModel model, long? userId, long? roleId)
        {
            if (model.Id == 0)
            {

                var estimateInvoiceViewModel1 = new EstimateInvoiceViewModel();
                var emailTemplete1 = _emailTemplateRepository.Table.FirstOrDefault(x => x.NotificationTypeId == ((long?)NotificationTypes.MailToCustomerForInvoice));
                estimateInvoiceViewModel1.MailBody = emailTemplete1.Body;
                estimateInvoiceViewModel1.Title = emailTemplete1.Title;
                var jobScheduler1 = _jobSchedulerRepository.Get(model.SchedulerId.GetValueOrDefault());
                if (jobScheduler1 != null)
                {
                    if(jobScheduler1.FranchiseeId == 62 || jobScheduler1.FranchiseeId == 38)
                    {
                        estimateInvoiceViewModel1.CcEmail = jobScheduler1.Person.Email == "" ? _settings.SEMIFromEmail : _settings.SEMIFromEmail + ", " + jobScheduler1.Person.Email + ", " + jobScheduler1.Franchisee.SchedulerEmail + ", " + jobScheduler1.Franchisee.Organization.Email;
                    }
                    else
                    {
                        estimateInvoiceViewModel1.CcEmail = jobScheduler1.Person.Email + ", " + jobScheduler1.Franchisee.SchedulerEmail + ", " + jobScheduler1.Franchisee.Organization.Email;
                    }
                    
                }
                estimateInvoiceViewModel1.Id = 0;
                return estimateInvoiceViewModel1;

            }
            var estimateInvoiceViewModel = new EstimateInvoiceViewModel();
            var estimateInvoiceServiceViewModel = new EstimateInvoiceServiceViewModel();
            var estimateInvoiceServiceViewModelList = new List<EstimateInvoiceServiceViewModel>();
            var estimateInvoice = _estimateInvoiceRepository.Get(model.Id.GetValueOrDefault());
            var estimateInvoiceServiceDescriptionList = _estimateInvoiceServiceDescriptionRepository.Table.ToList();
            var estimateInvoiceServiceDescriptionViewModel = CreateServiceDescriptionViewModel(estimateInvoiceServiceDescriptionList);
            var estimateInvoiceService = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.Id == model.Id).ToList();
            var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateInvoiceId == model.Id).ToList();
            var jobScheduler = _jobSchedulerRepository.Get(estimateInvoice.SchedulerId.GetValueOrDefault());
            if (jobScheduler != null)
            {
                if(jobScheduler.FranchiseeId == 62 || jobScheduler.FranchiseeId == 38)
                {
                    var CcEmailString = jobScheduler.Person.Email == "" ? _settings.SEMIFromEmail : _settings.SEMIFromEmail + "," + jobScheduler.Person.Email + "," + jobScheduler.Franchisee.SchedulerEmail + "," + jobScheduler.Franchisee.Organization.Email;
                    var ccList = CcEmailString.Split(new[] { "," }, StringSplitOptions.None).ToList();
                    ccList = ccList.Distinct().ToList();
                    var ccCount = 0;
                    foreach (var cc in ccList)
                    {
                        if (ccCount == 0)
                        {
                            estimateInvoiceViewModel.CcEmail = cc;
                        }
                        else
                        {
                            estimateInvoiceViewModel.CcEmail = estimateInvoiceViewModel.CcEmail + "," + cc;
                        }
                        ccCount += 1;
                    }
                }
                else
                {
                    var CcEmailString = jobScheduler.Person.Email + "," + jobScheduler.Franchisee.SchedulerEmail + "," + jobScheduler.Franchisee.Organization.Email;
                    var ccList = CcEmailString.Split(new[] { "," }, StringSplitOptions.None).ToList();
                    ccList = ccList.Distinct().ToList();
                    var ccCount = 0;
                    foreach (var cc in ccList)
                    {
                        if (ccCount == 0)
                        {
                            estimateInvoiceViewModel.CcEmail = cc;
                        }
                        else
                        {
                            estimateInvoiceViewModel.CcEmail = estimateInvoiceViewModel.CcEmail + "," + cc;
                        }
                        ccCount += 1;
                    }
                }
            }
            estimateInvoiceViewModel.Id = estimateInvoice.Id;
            estimateInvoiceViewModel.Option = estimateInvoice.Option;
            estimateInvoiceViewModel.EstimateId = estimateInvoice.EstimateId;
            estimateInvoiceViewModel.SchedulerId = estimateInvoice.SchedulerId;
            estimateInvoiceViewModel.EstimateInvoiceId = estimateInvoice.Id;
            var parentEstimateId = estimateInvoiceService.Select(x => x.Id).ToList();
            var subItem = _estimateInvoiceServiceRepository.Table.Where(x => parentEstimateId.Contains(x.Id)).ToList();
            estimateInvoiceService = estimateInvoiceService.Where(x => x.ParentId == null).ToList();
            var honingMeausrementList = _honingMeasurementRepository.Table.Where(x => x.IsActive == true).ToList();
            var honingMeausrementDefaultList = _honingMeasurementDefaultRepository.Table.Where(x => x.IsActive == true).ToList();
            estimateInvoiceViewModel.ServiceList = estimateInvoiceService.Select(x => _estimateInvoiceFactory.CreateViewModel(x, estimateInvoiceServiceDescriptionViewModel, estimateInvoice, subItem.Where(x1 => x1.ParentId == x.Id).ToList(), estimateInvoiceServiceMeasurements, estimateInvoiceServiceAssignee, honingMeausrementList.FirstOrDefault(x1 => x1.EstimateInvoiceServiceId == x.Id), honingMeausrementList, honingMeausrementDefaultList)).ToList();
            estimateInvoiceViewModel.LessDeposit = estimateInvoice.Franchisee != null ? estimateInvoice.Franchisee.LessDeposit : 50;
            estimateInvoiceViewModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
            estimateInvoiceViewModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
            estimateInvoiceViewModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
            estimateInvoiceViewModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
            estimateInvoiceViewModel.PhoneNumber1 = estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1;
            estimateInvoiceViewModel.PhoneNumber2 = estimateInvoice.EstimateInvoiceCustomer.PhoneNumber2;
            estimateInvoiceViewModel.NumberOfInvoices = estimateInvoice.NumberOfInvoices;
            var marketingClass = _marketingClassRepository.Get(estimateInvoice.ClassTypeId);
            estimateInvoiceViewModel.MarketingClass = marketingClass.Name;
            estimateInvoiceViewModel.Id = estimateInvoice.Id;
            estimateInvoiceViewModel.InvoiceCount = estimateInvoice.NumberOfInvoices.ToString();
            estimateInvoiceViewModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : "";
            estimateInvoiceViewModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : "";
            estimateInvoiceViewModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : "";
            estimateInvoiceViewModel.InvoiceNotesList = GetInvoiceNotes(estimateInvoice);
            estimateInvoiceViewModel.Notes = estimateInvoice.Notes;
            var emailTemplete = _emailTemplateRepository.Table.FirstOrDefault(x => x.NotificationTypeId == ((long?)NotificationTypes.MailToCustomerForInvoice));
            estimateInvoiceViewModel.MailBody = emailTemplete.Body;
            estimateInvoiceViewModel.Title = emailTemplete.Title;
            estimateInvoiceViewModel.IsCustomerAvailable = estimateInvoice.IsCustomerAvailable;
            return estimateInvoiceViewModel;
        }

        private List<EstimateInvoiceServiceDescriptionViewModel> CreateServiceDescriptionViewModel(List<EstimateInvoiceServiceDescription> estimateInvoiceServiceDescriptionList)
        {
            var estimateInvoiceServiceDescriptionViewModel = new EstimateInvoiceServiceDescriptionViewModel();
            var estimateInvoiceServiceDescriptionViewModelList = new List<EstimateInvoiceServiceDescriptionViewModel>();
            foreach (var estimateInvoiceServiceDescription in estimateInvoiceServiceDescriptionList)
            {
                estimateInvoiceServiceDescriptionViewModel.Description = estimateInvoiceServiceDescription.Description;
                estimateInvoiceServiceDescriptionViewModel.ServiceType = estimateInvoiceServiceDescription.ServiceType;
                estimateInvoiceServiceDescriptionViewModelList.Add(estimateInvoiceServiceDescriptionViewModel);
            }
            return estimateInvoiceServiceDescriptionViewModelList;
        }

        public bool SaveEstimateInvoice(EstimateInvoiceEditModel model)
        {
            var technicianWorkOrder = _technicianWorkOrderRepository.IncludeMultiple(x => x.WorkOrder).ToList();

            var estimate = _jobEstimateRepository.Table.FirstOrDefault(x => x.Id == model.EstimateId);
            if (estimate != null || estimate.Id != 0)
            {
                var marketingClass = _marketingClassRepository.Table.Where(x => x.Name == model.MarketingClass).FirstOrDefault();
                estimate.TypeId = marketingClass.Id;
                estimate.IsNew = false;
                _jobEstimateRepository.Save(estimate);
            }
            if (model.SchedulerId != null || model.SchedulerId != 0)
            {
                var scheduler = _jobSchedulerRepository.Get(model.SchedulerId.Value);
                scheduler.Estimate.Amount = Convert.ToDecimal(model.Price);
                _jobSchedulerRepository.Save(scheduler);
            }
            if (model.Id == 0)
            {
                var estimateInvoiceCustomer = _estimateInvoiceFactory.CreateDomainForCustomer(model);
                _estimateInvoiceCustomerRepository.Save(estimateInvoiceCustomer);

                var estimateInvoice = _estimateInvoiceFactory.CreateDomain(model);
                var marketingClass = _marketingClassRepository.Table.FirstOrDefault(x => x.Name == model.MarketingClass);
                estimateInvoice.ClassTypeId = marketingClass.Id;
                estimateInvoice.InvoiceCustomerId = estimateInvoiceCustomer.Id;
                if (estimateInvoice.Id == 0)
                {
                    estimateInvoice.IsCustomerAvailable = true;
                }
                estimateInvoice.IsInvoiceChanged = true;
                _estimateInvoiceRepository.Save(estimateInvoice);
                model.EstimateInvoiceId = estimateInvoice.Id;

                var technicianWorkOrderForInvoice = technicianWorkOrder.Select(x => _estimateInvoiceFactory.CreateDomain(x, estimateInvoice.Id)).ToList();

                var createTechnicianWorkOrderInvoiceDomain = technicianWorkOrderForInvoice;
                var estimateInvoiceServiceList = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
                var index = 0;
                model.ServiceList = model.ServiceList.OrderBy(x => x.InvoiceNumber).ToList();


                var previousInvoiceNumber = 0;
                var invoiceNumberDone = new List<long>();
                foreach (var service in model.ServiceList)
                {

                    var estimateInvoiceService = _estimateInvoiceFactory.CreateDomain(service);

                    estimateInvoiceService.EstimateInvoiceId = estimateInvoice.Id;
                    _estimateInvoiceServiceRepository.Save(estimateInvoiceService);
                    service.Id = estimateInvoiceService.Id;
                    var estimateInvoiceListWithoutParent = estimateInvoiceServiceList.Where(x => x.ParentId != null).ToList();
                    if (service.HoningMeasurementList != null)
                    {
                        SaveMeasurementForHoingAndPolishing(estimateInvoiceService.Id, service.HoningMeasurementList);
                    }
                    SaveInvoiceImages(estimateInvoiceService.Id, service.ImageList, estimate.Id, estimateInvoiceService.EstimateInvoiceId, model.UserId);
                    if (index == 0)
                    {
                        DeletingWorkOrderInvoice(estimateInvoice.Id);
                    }
                    DeletingMeasurementsInvoice(estimateInvoiceService.Id);
                    var measureTypeId = GetMeasurementType(estimateInvoiceService);
                    var unitTypeId = GetUnitType(measureTypeId);
                    AddMeasurements(service.Measurements, estimateInvoiceService.Id, unitTypeId);
                    if (invoiceNumberDone.Contains(service.InvoiceNumber))
                    {
                        createTechnicianWorkOrderInvoiceDomain = _workOrderTechnicianService.CreateTechnicianWorkOrderInvoice(service, technicianWorkOrderForInvoice, estimateInvoice.Id);
                    }
                    else
                    {
                        if (index != 0)
                        {
                            AddingWorkOrderInvoice(createTechnicianWorkOrderInvoiceDomain, previousInvoiceNumber);
                        }
                        technicianWorkOrderForInvoice = technicianWorkOrder.Select(x => _estimateInvoiceFactory.CreateDomain(x, estimateInvoice.Id)).ToList();
                        createTechnicianWorkOrderInvoiceDomain = _workOrderTechnicianService.CreateTechnicianWorkOrderInvoice(service, technicianWorkOrderForInvoice, estimateInvoice.Id);
                    }
                    SaveSubItem(service.SubItem, service, estimateInvoiceListWithoutParent, estimateInvoiceService.EstimateInvoiceId, createTechnicianWorkOrderInvoiceDomain, model.EstimateId, model.UserId);
                    ++index;
                    invoiceNumberDone.Add(service.InvoiceNumber);
                    previousInvoiceNumber = service.InvoiceNumber;
                    if (index == model.ServiceList.Count())
                    {
                        AddingWorkOrderInvoice(createTechnicianWorkOrderInvoiceDomain, previousInvoiceNumber);
                    }
                }

            }
            else
            {
                var serviceIdAlreadyPresentList = model.ServiceList.Where(x => x.Id > 0).Select(x => x.Id.Value).ToList();
                foreach (var serviceList in model.ServiceList)
                {
                    var subItemId = serviceList.SubItem.Where(x => x.Id != null && x.Id != 0).Select(x => x.Id.Value).ToList();
                    serviceIdAlreadyPresentList.AddRange(subItemId);
                }
                var estimateinvoice = _estimateInvoiceRepository.Get(model.Id.Value);

                var technicianWorkOrderForInvoice = technicianWorkOrder.Select(x => _estimateInvoiceFactory.CreateDomain(x, estimateinvoice.Id)).ToList();
                var createTechnicianWorkOrderInvoiceDomain = technicianWorkOrderForInvoice;

                var alreadyCreatedServices = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateinvoice.Id)
                                                                .Select(x => x.Id).ToList();
                var estimateinvoiceCustomer = _estimateInvoiceCustomerRepository.Get(estimateinvoice.InvoiceCustomerId.Value);

                var estimateInvoiceCustomerDomain = _estimateInvoiceFactory.CreateDomainForCustomer(model);
                estimateInvoiceCustomerDomain.Id = estimateinvoiceCustomer.Id;
                estimateInvoiceCustomerDomain.DataRecorderMetaDataId = estimateinvoiceCustomer.DataRecorderMetaDataId;
                _estimateInvoiceCustomerRepository.Save(estimateInvoiceCustomerDomain);


                var estimateInvoice = _estimateInvoiceFactory.CreateDomain(model);
                estimateInvoice.Id = estimateinvoice.Id;
                var marketingClass = _marketingClassRepository.Table.FirstOrDefault(x => x.Name == model.MarketingClass);
                estimateInvoice.ClassTypeId = marketingClass.Id;
                estimateInvoice.InvoiceCustomerId = estimateinvoiceCustomer.Id;
                estimateInvoice.DataRecorderMetaDataId = estimateinvoice.DataRecorderMetaDataId;
                estimateInvoice.IsInvoiceChanged = true;
                _estimateInvoiceRepository.Save(estimateInvoice);
                model.EstimateInvoiceId = estimateInvoice.Id;
                var estimateInvoiceServiceList = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
                var index = 0;
                model.ServiceList = model.ServiceList.OrderBy(x => x.InvoiceNumber).ToList();
                var invoiceNumberDone = new List<long>();
                var previousInvoiceNumber = 0;
                foreach (var service in model.ServiceList)
                {
                    var estimateInvoiceService = _estimateInvoiceFactory.CreateDomain(service);
                    if (!estimateInvoiceService.IsNew)
                    {
                        var domain = _estimateInvoiceServiceRepository.Get(service.Id.Value);
                        estimateInvoiceService.Id = domain.Id;
                        estimateInvoiceService.DataRecorderMetaDataId = domain.DataRecorderMetaDataId;
                    }
                    estimateInvoiceService.EstimateInvoiceId = estimateinvoice.Id;

                    _estimateInvoiceServiceRepository.Save(estimateInvoiceService);
                    service.Id = estimateInvoiceService.Id;

                    var estimateInvoiceListWithoutParent = estimateInvoiceServiceList.Where(x => x.ParentId != null).ToList();
                    if (service.HoningMeasurementList != null)
                    {
                        SaveMeasurementForHoingAndPolishing(estimateInvoiceService.Id, service.HoningMeasurementList);
                    }
                    SaveInvoiceImages(estimateInvoiceService.Id, service.ImageList, estimate.Id, estimateInvoiceService.EstimateInvoiceId, model.UserId);
                    if (index == 0)
                    {

                        DeletingWorkOrderInvoice(estimateInvoice.Id);
                    }

                    if (invoiceNumberDone.Contains(service.InvoiceNumber))
                    {
                        createTechnicianWorkOrderInvoiceDomain = _workOrderTechnicianService.CreateTechnicianWorkOrderInvoice(service, technicianWorkOrderForInvoice, estimateInvoice.Id);
                    }
                    else
                    {
                        if (index != 0)
                        {
                            AddingWorkOrderInvoice(createTechnicianWorkOrderInvoiceDomain, previousInvoiceNumber);
                        }
                        technicianWorkOrderForInvoice = technicianWorkOrder.Select(x => _estimateInvoiceFactory.CreateDomain(x, estimateInvoice.Id)).ToList();
                        createTechnicianWorkOrderInvoiceDomain = _workOrderTechnicianService.CreateTechnicianWorkOrderInvoice(service, technicianWorkOrderForInvoice, estimateInvoice.Id);
                    }

                    DeletingMeasurementsInvoice(estimateInvoiceService.Id);
                    var measureTypeId = GetMeasurementType(estimateInvoiceService);
                    var unitTypeId = GetUnitType(measureTypeId);
                    AddMeasurements(service.Measurements, estimateInvoiceService.Id, unitTypeId);

                    SaveSubItem(service.SubItem, service, estimateInvoiceListWithoutParent, estimateInvoiceService.EstimateInvoiceId, createTechnicianWorkOrderInvoiceDomain, model.EstimateId, model.UserId);
                    ++index;

                    invoiceNumberDone.Add(service.InvoiceNumber);
                    previousInvoiceNumber = service.InvoiceNumber;
                    if (index == model.ServiceList.Count())
                    {
                        AddingWorkOrderInvoice(createTechnicianWorkOrderInvoiceDomain, previousInvoiceNumber);
                    }
                }
                if (alreadyCreatedServices.Count() != serviceIdAlreadyPresentList.Count())
                {
                    var idsToBeDeleted = alreadyCreatedServices.Except(serviceIdAlreadyPresentList).ToList();

                    foreach (var id in idsToBeDeleted)
                    {
                        var toBeDeletedService = _estimateInvoiceServiceRepository.Table.FirstOrDefault(x => x.Id == id);
                        if (toBeDeletedService != null)
                            _estimateInvoiceServiceRepository.Delete(toBeDeletedService);
                    }
                }
            }
            SaveEstimateInvoiceNotes(model);
            _unitOfWork.SaveChanges();
            return true;
        }

        public int SendMailToCustomer(long? schedulerId, List<int> serviceInvoice, string templateName, long? userId, SelectInvoicesViewModel model)
        {
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            try
            {
                var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);

                if (estimateInvoice == null)
                {
                    return -1;
                }
                var id = Convert.ToInt64(model.ToEmailId);
                if (id != null && id != 0)
                {
                    var estimateCustomer = _estimateInvoiceCustomerRepository.Table.FirstOrDefault(x => x.Id == id);
                    model.Email = model.Email.Replace(" ", "");
                    estimateCustomer.Email = model.Email;
                    model.CCEmail = model.CCEmail.Replace(" ", "");
                    estimateCustomer.CCEmail = model.CCEmail;
                    _estimateInvoiceCustomerRepository.Save(estimateCustomer);
                    _unitOfWork.SaveChanges();
                }
                

                var jobCustomer = _jobCustomerRepository.Table.FirstOrDefault(x => x.Id == model.CustomerId && x.Email != null);
                if(jobCustomer != null)
                {
                    jobCustomer.Email = model.Email;
                }

                var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
                var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();

                var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();

                if (serviceInvoice != null && serviceInvoice.Count() > 0)
                {
                    estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => serviceInvoice.Contains(x.Key)).ToList();
                }

                var destinationFolder = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + templateName);

                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == jobScheduler.EstimateId).Select(x => x.Id).ToList();
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.Body = model.Body;
                estimateInvoiceEditModel.FileModel = model.FileModel;
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";

                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : " ";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : " ";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : " ";
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                estimateInvoiceEditModel.SchedulerId = estimateInvoice.SchedulerId;
                estimateInvoiceEditModel.ToEmailId = id;

                int index = 1;
                var fileDomain = new List<Application.Domain.File>();
                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                if (estimateInvoiceServicesGroupedData.Count() == 0)
                {
                    return -1;
                }
                var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id && x.TypeId == model.TypeId).OrderByDescending(x => x.Id);

                var code = GetCode();
                estimateInvoiceEditModel.Code = code;
                estimateInvoiceEditModel.Url = _settings.SignatureUrl;
                estimateInvoiceEditModel.OfficeNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.IsTransferable).Number != null ? 
                   FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.IsTransferable).Number) : null;
                if(estimateInvoiceEditModel.OfficeNumber == null)
                {
                    estimateInvoiceEditModel.OfficeNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == 1).Number;
                }
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();
                if (estimateInvoiceServicesGroupedData.Count() == customerSignature.Count())
                {
                    estimateInvoiceEditModel.IsSigned = "none";
                }
                else
                {
                    estimateInvoiceEditModel.IsSigned = "block";
                }

                estimateInvoiceEditModel.OfficeName = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Name != null ? estimateInvoice.Franchisee.Organization.Name : "";
                estimateInvoiceEditModel.OfficeAddressLine1 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1 : "";
                estimateInvoiceEditModel.OfficeAddressLine2 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2 : "";
                estimateInvoiceEditModel.OfficeAddressCity = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName : "";
                estimateInvoiceEditModel.OfficeAddressState = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName : "";
                estimateInvoiceEditModel.OfficeAddressCountry = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country) != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name : ""; ;
                estimateInvoiceEditModel.OfficeAddressZipCode = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode : ""; ;

                estimateInvoiceEditModel.EstimateDate = estimateInvoice.JobScheduler.StartDateTimeString.ToString();
                var invoiceNumberCount = 1;
                var invoiceItemCount = estimateInvoiceServicesGroupedData.Count();

                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    estimateInvoiceEditModel.Option1Total = default(decimal);
                    estimateInvoiceEditModel.Option2Total = default(decimal);
                    estimateInvoiceEditModel.Option3Total = default(decimal);

                    estimateInvoiceEditModel.ServiceList = new List<EstimateInvoiceServiceEditMailModel>();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    var list = estimateInvoiceServicesLocal.ToList();

                    if (customerSignature != null && customerSignature.Count() > 0)
                    {
                        var customerSignatureForInvoice = customerSignature.FirstOrDefault(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                        if (customerSignatureForInvoice != null)
                        {
                            estimateInvoiceEditModel.CustomerSignature = customerSignatureForInvoice.Signature;
                            estimateInvoiceEditModel.SignDateTime = customerSignatureForInvoice.SignedDateTime != null && customerSignatureForInvoice.SignedDateTime != default(DateTime?)
                                                                                            ? _clock.ToLocal(customerSignatureForInvoice.SignedDateTime.GetValueOrDefault()).ToString("MM/dd/yyyy") : "";
                        }
                        else
                        {
                            estimateInvoiceEditModel.CustomerSignature = "";
                            estimateInvoiceEditModel.SignDateTime = "";
                        }
                    }
                    else
                    {
                        estimateInvoiceEditModel.CustomerSignature = "";
                        estimateInvoiceEditModel.SignDateTime = "";
                    }

                    if (customerSignature.Count() == 0)
                    {
                        estimateInvoiceEditModel.IsSigned = "block";
                    }

                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);

                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option1Total));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option2Total));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option3Total));
                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";
                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";
                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoiceNotes != null && estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);
                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    var fileName = fileFullName + ".pdf";

                    if (invoiceItemCount == 1)
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id.ToString();
                    }
                    else
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id + "-" + invoiceNumberCount;
                        invoiceNumberCount += 1;
                    }

                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    fileDomain.Add(GetFileModel(file));

                    index += 1;
                }
                foreach (var file in model.FileModel)
                {
                    fileDomain.Add(SaveFile(file));
                }

                jobScheduler.IsCustomerMailSend = true;
                _jobSchedulerRepository.Save(jobScheduler);

                if (!model.IsFromJob.Value)
                {
                    SendingInvoiceToCustomer(estimateInvoiceEditModel, fileDomain, NotificationTypes.MailToCustomerForInvoice, estimateInvoiceEditModel.Email, model.LoggedInUserId);
                    SaveCustomerSignatureInfo(estimateInvoice, code, model.IsFromJob, model.JobSchedulerId);
                }
                else
                {
                    SendingInvoiceToCustomer(estimateInvoiceEditModel, fileDomain, NotificationTypes.MailToCustomerForPostJobCompletion, estimateInvoiceEditModel.Email, model.LoggedInUserId);
                    SaveCustomerSignatureInfo(estimateInvoice, code, model.IsFromJob, model.JobSchedulerId);
                }

                return 1;
            }
            catch (Exception e1)
            {
                var a = e1.InnerException;
                return 0;
            }

        }

        public long? SendingInvoiceToCustomer(EstimateInvoiceEditMailModel model, List<Application.Domain.File> fileDomain, NotificationTypes notificationId, string emailId, long? userId)
        {
            model.UserId = userId;
            var notificaitonTypeId = default(long?);
            var person = _personRepository.Get(userId.GetValueOrDefault());
            if (person != null)
            {
                model.FromEmail = person.Email;
            }
            else
            {
                model.FromEmail = _settings.MarketingEmail;
            }
            notificaitonTypeId = _sendNewJobNotificationToTechService.SendingInvoiceToCustomer(model, fileDomain, notificationId, emailId);
            return notificaitonTypeId;
        }

        public long? SendingInvoiceToCustomerForSignedInvoices(EstimateInvoiceEditMailModel model, List<Application.Domain.File> fileDomain, NotificationTypes notificationId, string emailId, long? userId, bool isFromURL, bool mailToSalesRep)
        {
            model.UserId = userId;
            var person = _personRepository.Get(userId.GetValueOrDefault());
            if (person != null)
            {
                model.FromEmail = person.Email;
            }
            else
            {
                model.FromEmail = _settings.MarketingEmail;
            }
            var notificaitonId = _sendNewJobNotificationToTechService.SendingInvoiceToCustomerForSignedInvoices(model, fileDomain, notificationId, emailId, isFromURL, mailToSalesRep);
            return notificaitonId;
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
            _ = file.DataRecorderMetaData.CreatedBy == null ? file.DataRecorderMetaData.CreatedBy = 1027 : file.DataRecorderMetaData.CreatedBy = file.DataRecorderMetaData.CreatedBy;
            //file.DataRecorderMetaData.IsNew = true;
            _fileRepository.Save(file);
            return file;
        }


        private Application.Domain.File GetFileModelForModel(FileModel fileModel)
        {

            var file = new Application.Domain.File
            {
                Name = fileModel.Caption,
                Caption = fileModel.Caption,
                RelativeLocation = MediaLocationHelper.GetACustomerInvoiceLocation().Path,
                MimeType = fileModel.MimeType,
                Size = fileModel.Size,
                IsNew = true,
                DataRecorderMetaData = fileModel.DataRecorderMetaData
            };
            _ = file.DataRecorderMetaData.CreatedBy == null ? file.DataRecorderMetaData.CreatedBy = 1027 : file.DataRecorderMetaData.CreatedBy = file.DataRecorderMetaData.CreatedBy;
            _fileRepository.Save(file);
            return file;
        }

        public string UploadInvoicesZipFile(long schedulerId, List<int> serviceInvoice)
        {
            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);
            if (estimateInvoice == null)
            {
                return "";
            }
            var technicianOrderData = CreateWorkOrderViewModel(estimateInvoice.Id);
            var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();
            estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => serviceInvoice.Contains(x.Key)).ToList();
            estimateInvoiceEditModel.WorkOrderForFloorDiamond = technicianOrderData.Where(x => x.CategoryName == "FLOOR DIAMOND").ToList();
            estimateInvoiceEditModel.WorkOrderForHandDiamond = technicianOrderData.Where(x => x.CategoryName == "HAND DIAMOND").ToList();
            estimateInvoiceEditModel.WorkOrderForBrushes = technicianOrderData.Where(x => x.CategoryName == "BRUSHES").ToList();
            estimateInvoiceEditModel.WorkOrderForPads = technicianOrderData.Where(x => x.CategoryName == "PADS").ToList();
            estimateInvoiceEditModel.WorkOrderForPolish = technicianOrderData.Where(x => x.CategoryName == "POLISH").ToList();
            estimateInvoiceEditModel.WorkOrderForGroute = technicianOrderData.Where(x => x.CategoryName == "GROUT").ToList();
            estimateInvoiceEditModel.WorkOrderForSealer = technicianOrderData.Where(x => x.CategoryName == "SEALER").ToList();
            estimateInvoiceEditModel.WorkOrderForStripping = technicianOrderData.Where(x => x.CategoryName == "STRIPPING").ToList();
            estimateInvoiceEditModel.WorkOrderForCoating = technicianOrderData.Where(x => x.CategoryName == "COATING").ToList();
            estimateInvoiceEditModel.WorkOrderForChips = technicianOrderData.Where(x => x.CategoryName == "CHIPS").ToList();
            estimateInvoiceEditModel.WorkOrderForKits = technicianOrderData.Where(x => x.CategoryName == "KITS").ToList();
            estimateInvoiceEditModel.WorkOrderForCleaner = technicianOrderData.Where(x => x.CategoryName == "CLEANER").ToList();
            estimateInvoiceEditModel.WorkOrderForCareProducts = technicianOrderData.Where(x => x.CategoryName == "CARE PRODUCTS").ToList();
            if (estimateInvoiceServicesGroupedData.Count() == 0)
            {
                return "";
            }
            try
            {
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";

                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = (estimateInvoice.Option1 != "" && estimateInvoice.Option1 != null) ? estimateInvoice.Option1 : "";
                estimateInvoiceEditModel.Option2 = (estimateInvoice.Option2 != "" && estimateInvoice.Option2 != null) ? estimateInvoice.Option2 : "";
                estimateInvoiceEditModel.Option3 = (estimateInvoice.Option3 != "" && estimateInvoice.Option3 != null) ? estimateInvoice.Option3 : "";
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).Select(x => x.Id).ToList();
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();

                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                int index = 1;
                var fileDomain = new List<Application.Domain.File>();
                var destinationFolder = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + "cutomer_invoice_with_work.cshtml");
                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();

                estimateInvoiceEditModel.OfficeName = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Name != null ? estimateInvoice.Franchisee.Organization.Name : "";
                estimateInvoiceEditModel.OfficeAddressLine1 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1 : "";
                estimateInvoiceEditModel.OfficeAddressLine2 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2 : "";
                estimateInvoiceEditModel.OfficeAddressCity = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName : "";
                estimateInvoiceEditModel.OfficeAddressState = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName : "";
                estimateInvoiceEditModel.OfficeAddressCountry = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country) != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name : ""; ;
                estimateInvoiceEditModel.OfficeAddressZipCode = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode : ""; ;

                estimateInvoiceEditModel.EstimateDate = estimateInvoice.JobScheduler.StartDateTimeString.ToString();
                var invoiceNumberCount = 1;
                var invoiceItemCount = estimateInvoiceServicesGroupedData.Count();
                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {

                    estimateInvoiceEditModel.Option1Total = 0;
                    estimateInvoiceEditModel.Option2Total = 0;
                    estimateInvoiceEditModel.Option3Total = 0;
                    estimateInvoiceEditModel.TotalArea = 0;
                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);
                    estimateInvoiceEditModel.WorkOrderForFloorDiamond = technicianOrderData.Where(x => x.CategoryName == "FLOOR DIAMOND" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForHandDiamond = technicianOrderData.Where(x => x.CategoryName == "HAND DIAMOND" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForBrushes = technicianOrderData.Where(x => x.CategoryName == "BRUSHES" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForPads = technicianOrderData.Where(x => x.CategoryName == "PADS" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForPolish = technicianOrderData.Where(x => x.CategoryName == "POLISH" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForGroute = technicianOrderData.Where(x => x.CategoryName == "GROUT" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForSealer = technicianOrderData.Where(x => x.CategoryName == "SEALER" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForStripping = technicianOrderData.Where(x => x.CategoryName == "STRIPPING" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForCoating = technicianOrderData.Where(x => x.CategoryName == "COATING" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForChips = technicianOrderData.Where(x => x.CategoryName == "CHIPS" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForKits = technicianOrderData.Where(x => x.CategoryName == "KITS" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForCleaner = technicianOrderData.Where(x => x.CategoryName == "CLEANER" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.WorkOrderForCareProducts = technicianOrderData.Where(x => x.CategoryName == "CARE PRODUCTS" && x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    var list = estimateInvoiceServicesLocal.ToList();
                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    estimateInvoiceEditModel.TotalArea = GetTotalArea(estimateInvoiceEditModel.EstimateInvoiceDimensionTables); //measurements.Sum(x => x.Area);
                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));
                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption1.ToString()));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption2.ToString()));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption3.ToString()));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);

                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";
                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";
                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";
                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }
                    var firstName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var LastName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), false);
                    fileFullName = fileFullName.Replace(".", "");
                    fileFullName = fileFullName.Replace(":", "");
                    var fileName = fileFullName + ".pdf";

                    var signatureViewModel = GetSignature(estimateInvoice.Id, estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.CustomerSignature = signatureViewModel.PreSignature;
                    estimateInvoiceEditModel.SignDateTime = signatureViewModel.PreSignatureDate;
                    estimateInvoiceEditModel.CustomerPostSignature = signatureViewModel.PostSignature;
                    estimateInvoiceEditModel.PostSignDateTime = signatureViewModel.PostSignatureDate;
                    estimateInvoiceEditModel.Technician = signatureViewModel.Technician;

                    if (invoiceItemCount == 1)
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id.ToString();
                    }
                    else
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id + "-" + invoiceNumberCount;
                        invoiceNumberCount += 1;
                    }

                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    fileDomain.Add(GetFileModel(file));
                    index += 1;

                }
                using (var zip = new ZipFile())
                {
                    foreach (var domain in fileDomain)
                    {
                        var file = _fileRepository.Get(domain.Id);
                        if (file != null)
                        {
                            string filePath = file.RelativeLocation + @"\" + file.Name;
                            zip.AddFile(filePath, "");
                        }
                    }
                    var fileName2 = "InvoiceCustomerDownload-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                    var fileName = "InvoiceCustomerDownload-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".zip";
                    var rootPath = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "/" + fileName;
                    zip.Save(rootPath);
                    return fileName2;
                }
            }
            catch (Exception e1)
            {
                _logService.Error(e1 + "   " + e1.InnerException);
                var a = e1.InnerException;
                return "";
            }
        }

        private List<EstimateInvoiceDimensionTableViewModel> GetMeasurements(List<EstimateInvoiceDimension> dimensionList, List<EstimateInvoiceService> invoiceService)
        {
            var estimateInvoiceDimensionTableViewModel = new EstimateInvoiceDimensionTableViewModel();
            var estimateInvoiceDimensionTableViewModelList = new List<EstimateInvoiceDimensionTableViewModel>();
            var estimateInvoiceDimensionEditModelList = new List<EstimateInvoiceDimensionViewModel>();
            var estimateInvoiceDimensionEditModel = new EstimateInvoiceDimensionViewModel();
            var x1 = 0;
            foreach (EstimateInvoiceService service in invoiceService)
            {
                estimateInvoiceDimensionEditModel = new EstimateInvoiceDimensionViewModel();
                estimateInvoiceDimensionTableViewModel = new EstimateInvoiceDimensionTableViewModel();
                x1++;
                estimateInvoiceDimensionTableViewModel.InvoiceLine = x1;
                var typeId = GetMeasurementType(service);
                var unitTypeId = GetUnitType(typeId);
                if (dimensionList.Any(x => x.EstimateInvoiceServiceId == service.Id))
                {
                    var dimension = dimensionList.Where(x => x.EstimateInvoiceServiceId == service.Id).ToList();
                    estimateInvoiceDimensionTableViewModel.DimensionList = dimension.Select(x => _estimateInvoiceFactory.CreateEstimateInvoiceDimensionViewModel(x, unitTypeId)).ToList();
                }
                else
                {
                    estimateInvoiceDimensionTableViewModel.DimensionList = new List<EstimateInvoiceDimensionViewModel>();
                }
                estimateInvoiceDimensionTableViewModelList.Add(estimateInvoiceDimensionTableViewModel);
            }

            return estimateInvoiceDimensionTableViewModelList;
        }

        private decimal GetTotalArea(List<EstimateInvoiceDimensionTableViewModel> model)
        {
            decimal totalArea = 0;
            if (model.Count == 0)
            {
                return totalArea;
            }
            foreach (var invoiceDimension in model)
            {
                if (invoiceDimension.DimensionList.Count > 0)
                {
                    foreach (var dimension in invoiceDimension.DimensionList)
                    {
                        if(dimension.Area.HasValue)
                            totalArea += dimension.Area.Value;
                    }
                }
            }
            return totalArea;
        }

        private bool SaveSubItem(List<SubItemEditModel> subItem, EstimateInvoiceServiceEditModel service, List<EstimateInvoiceService> estimateInvoiceServices, long estimateInvoiceNumber, List<TechnicianWorkOrderForInvoice> technicianWorkOrderForInvoice, long? estimateId, long? userId)
        {
            var childServices = estimateInvoiceServices.Where(x => x.ParentId == service.Id).ToList();
            var estimateInvoiceServicesChild = childServices.Select(x => _estimateInvoiceFactory.CreateViewModelForSubItem(x, null, null)).ToList();
            var differences = subItem.Except(estimateInvoiceServicesChild).ToList();

            foreach (var subItemDifference in differences)
            {
                var estimateInvoiceService = _estimateInvoiceFactory.CreateDomainModel(service, subItemDifference);
                estimateInvoiceService.EstimateInvoiceId = estimateInvoiceNumber;
                estimateInvoiceService.ServiceTagId = GetMeasurementType(estimateInvoiceService);

                if (estimateInvoiceService.Id > 0)
                {
                    var estimateInvoiceServiceDomain = _estimateInvoiceServiceRepository.Get(estimateInvoiceService.Id);
                    if (estimateInvoiceServiceDomain != null)
                    {
                        estimateInvoiceService.DataRecorderMetaDataId = estimateInvoiceServiceDomain.DataRecorderMetaDataId;
                        _estimateInvoiceServiceRepository.Save(estimateInvoiceService);
                    }
                }
                else
                {
                    _estimateInvoiceServiceRepository.Save(estimateInvoiceService);
                }
                if (subItemDifference.Measurements.Count() > 0)
                {
                    DeletingMeasurementsInvoice(estimateInvoiceService.Id);
                    AddMeasurements(subItemDifference.Measurements.ToList(), subItemDifference.Id.GetValueOrDefault() != 0 ? subItemDifference.Id.GetValueOrDefault() : estimateInvoiceService.Id, GetMeasurementType(estimateInvoiceService));
                }

                if (subItemDifference.HoningMeasurementList.Count() > 0)
                {
                    SaveMeasurementForHoingAndPolishing(estimateInvoiceService.Id, subItemDifference.HoningMeasurementList);
                }
                SaveInvoiceImages(estimateInvoiceService.Id, subItemDifference.ImageList, estimateId, estimateInvoiceService.EstimateInvoiceId, userId);
            }

            var estimateInvoiceServicesChildId = estimateInvoiceServicesChild.Select(x => x.Id).ToList();
            var subItemid = subItem.Where(x => x.Id != 0).Select(x => x.Id).ToList();
            var deletedItems = estimateInvoiceServicesChildId.Except(subItemid).ToList();

            foreach (var deletedItem in deletedItems)
            {

                var estimateInvoiceService = _estimateInvoiceServiceRepository.Get(deletedItem.GetValueOrDefault());
                DeletingMeasurementsInvoice(deletedItem.GetValueOrDefault());
                _estimateInvoiceServiceRepository.Delete(estimateInvoiceService);
            }

            return true;
        }

        private List<ListViewModelForNotes> GetInvoiceNotes(EstimateInvoice estimateInvoice)
        {
            var es = _estimateServiceInvoiceNotesRepository.Table.Where(x => x.EstimateinvoiceId == estimateInvoice.Id).ToList();
            var invoiceNotesList = es.Select(x => new ListViewModelForNotes()
            {
                Id = x.Id,
                Notes = x.Notes,
                InvoiceNumber = x.InvoiceNumber
            }).ToList();

            return invoiceNotesList;
        }

        private bool SaveEstimateInvoiceNotes(EstimateInvoiceEditModel model)
        {
            try
            {
                var alreadySavedNotesId = _estimateServiceInvoiceNotesRepository.Table.Where(x => x.EstimateinvoiceId == model.EstimateInvoiceId).Select(x => x.Id).ToList();
                var newlySavedId = model.InvoiceNotesList.Select(x => x.Id.Value).ToList();
                var idToBeDeleted = alreadySavedNotesId.Except(newlySavedId).ToList();
                foreach (var invoiceNotes in model.InvoiceNotesList)
                {
                    if (invoiceNotes.Id == 0)
                    {
                        var invoiceNotesDomain = new EstimateServiceInvoiceNotes()
                        {
                            DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                            EstimateinvoiceId = model.EstimateInvoiceId,
                            UserId = model.UserId,
                            Notes = invoiceNotes.Notes,
                            IsNew = true,
                            InvoiceNumber = invoiceNotes.InvoiceNumber

                        };
                        _estimateServiceInvoiceNotesRepository.Save(invoiceNotesDomain);
                    }
                    else
                    {
                        var invoiceNotesDomain = _estimateServiceInvoiceNotesRepository.Get(invoiceNotes.Id.Value);
                        invoiceNotesDomain.Notes = invoiceNotes.Notes;
                        invoiceNotesDomain.InvoiceNumber = invoiceNotes.InvoiceNumber;
                        _estimateServiceInvoiceNotesRepository.Save(invoiceNotesDomain);
                    }
                }
                foreach (var id in idToBeDeleted)
                {
                    var invoiceNotesDomain = _estimateServiceInvoiceNotesRepository.Get(id);
                    _estimateServiceInvoiceNotesRepository.Delete(invoiceNotesDomain);
                }
                return true;
            }
            catch (Exception e1)
            {
                var a = e1.InnerException;
                return false;
            }
        }

        private string GetFileNameName(string customerName, EstimateInvoiceService invoiceService, bool isCustomerInvoice)
        {
            customerName = RemoveInvalidChars(customerName);
            var customerSplittedName = customerName.Split(' ');
            var locationJoined = "";
            var locationSplittedName = invoiceService.Location.Split(',');
            if (locationSplittedName.Length > 2)
            {
                locationSplittedName[0] = locationSplittedName[0].Replace(" ", "");
                locationSplittedName[1] = locationSplittedName[1].Replace(" ", "");
                locationJoined = locationSplittedName[0] + "_" + locationSplittedName[1];
            }
            else
            {
                var locationSplittedNameLocal = new List<string>();
                foreach (var locationName in locationSplittedName)
                {
                    locationSplittedNameLocal.Add(locationName.Replace(" ", ""));
                }
                locationJoined = String.Join("_", locationSplittedNameLocal);
            }

            var serviceType = invoiceService != null ? invoiceService.ServiceType : "";
            var fileName = "";
            foreach (var name in customerSplittedName.Reverse())
            {
                fileName += name + "_";
            }


            if (serviceType == "CONCRETE-COATINGS" || serviceType == "ENDURACRETE")
            {
                if (isCustomerInvoice)
                    fileName = fileName + "_" + "CustomerConcreteOrder";
                else
                    fileName = fileName + "_" + "InternalConcreteOrder";
            }
            else
            {
                if (isCustomerInvoice)
                    fileName = fileName + "_" + "CustomerOrder";
                else
                    fileName = fileName + "_" + "InternalOrder";
            }
            if (!string.IsNullOrEmpty(locationJoined))
                fileName = fileName + "_" + locationJoined;
            var count = fileNameList.Where(s => s.StartsWith(fileName)).Count();
            if (!fileNameList.Contains(fileName))
                fileNameList.Add(fileName);
            else
            {
                fileName = fileName + "_" + count;
                fileNameList.Add(fileName);
            }
            return fileName;
        }

        private string FormatPhoneNumber(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            value = new System.Text.RegularExpressions.Regex(@"\D")
                .Replace(value, string.Empty);
            value = value.TrimStart('1');
            if (value.Length == 7)
                return Convert.ToInt64(value).ToString("###-####");
            if (value.Length == 10)
                return Convert.ToInt64(value).ToString("###-###-####");
            if (value.Length > 10)
                return Convert.ToInt64(value)
                    .ToString("###-###-#### " + new String('#', (value.Length - 10)));
            return value;
        }

        private bool SaveCustomerSignatureInfo(EstimateInvoice estimateInvoice, string code, bool? isFromJob, long? jobSchedulerId)
        {
            var customerSignatureInfo = new CustomerSignatureInfo()
            {
                Code = code,
                EstimateInvoiceId = estimateInvoice.Id,
                IsNew = true,
                IsActive = true,
                TypeId = isFromJob.GetValueOrDefault() ? (long?)SignatureType.POSTCOMPLETION : (long?)SignatureType.PRECOMPLETION,
                JobSchedulerId = jobSchedulerId

            };
            _customerSignatureInfoRepository.Save(customerSignatureInfo);
            return true;
        }

        public bool SaveCustomerSignature(CustomersignatureViewModel model)
        {
            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.Id == model.EstimateInvoiceId);

            var schedulerId = default(long?);

            foreach (var invoicenumber in model.InvoiceNumbers)
            {

                var customerSignatureAlready = _customerSignatureRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.EstimateInvoiceId == model.EstimateInvoiceId && x.InvoiceNumber == invoicenumber && x.TypeId == model.TypeId);
                if (customerSignatureAlready != null)
                    _customerSignatureRepository.Delete(customerSignatureAlready);


                if (model.JobOrginialSchedulerId == null)
                {
                    var estimateInvoiceAssignee = _estimateInvoiceAssigneeRepository.Table.FirstOrDefault(x => x.EstimateInvoiceId == model.EstimateInvoiceId && x.InvoiceNumber == invoicenumber);
                    if (estimateInvoiceAssignee != null)
                        schedulerId = estimateInvoiceAssignee.SchedulerId;
                }
                else
                {
                    schedulerId = model.JobOrginialSchedulerId;
                }
                if (model.TypeId == (long?)SignatureType.PRECOMPLETION)
                {
                    schedulerId = model.JobSchedulerId;
                }
                var customerSignature = new CustomerSignature()
                {
                    CustomerId = model.CustomerId,
                    EstimateCustomerId = estimateInvoice != null ? estimateInvoice.InvoiceCustomerId : null,
                    IsNew = true,
                    EstimateInvoiceId = model.EstimateInvoiceId,
                    Signature = model.Signature,
                    Name = model.Name,
                    SignedDateTime = _clock.ToUtc(_clock.UtcNow),
                    InvoiceNumber = invoicenumber,
                    TypeId = model.TypeId,
                    JobSchedulerId = schedulerId,
                    IsFromUrl = model.IsFromURL,
                    SignedById = !model.IsFromURL ? model.UserId : null
                };
                _customerSignatureRepository.Save(customerSignature);

            }


            if (model.TypeId == (long?)SignatureType.PRECOMPLETION)
            {
                SendMailToCustomerForSignedInvoices(model.JobSchedulerId != null ? model.JobSchedulerId : model.SchedulerId, "cutomer_invoice.cshtml", model.UserId.Value, model.IsFromURL, model.IsFromJob, model.JobOrginialSchedulerId);
            }
            else
            {
                SendMailToCustomerForSignedInvoicesPost(model.SchedulerId, "cutomer_invoice.cshtml", model.UserId.Value, model.IsFromURL, model.IsFromJob, model.JobOrginialSchedulerId, model);
            }

            estimateInvoice.IsInvoiceChanged = true;
            _estimateInvoiceRepository.Save(estimateInvoice);
            return true;
        }

        public string UploadSignedInvoicesZipFile(JobInvoiceDownloadViewModel model)
        {
            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == model.SchedulerId);
            if (estimateInvoice == null)
            {
                return "";
            }
            var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).OrderByDescending(x => x.Id);
            var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();
            if (estimateInvoiceServicesGroupedData.Count() == 0)
            {
                return "";
            }
            var invoicesSigned = customerSignature.Select(x => x.InvoiceNumber).ToList();

            try
            {
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";

                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : "";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : "";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : "";
                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).Select(x => x.Id).ToList();
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                int index = 1;
                var fileDomain = new List<Application.Domain.File>();
                var destinationFolder = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + "cutomer_invoice.cshtml");
                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                if (model.InvoiceNumbers.Count > 0)
                {
                    estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => model.InvoiceNumbers.Contains(x.Key)).ToList();
                }
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();

                estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => invoicesSigned.Contains(x.Key)).ToList();

                estimateInvoiceEditModel.OfficeName = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Name != null ? estimateInvoice.Franchisee.Organization.Name : "";
                estimateInvoiceEditModel.OfficeAddressLine1 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1 : "";
                estimateInvoiceEditModel.OfficeAddressLine2 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2 : "";
                estimateInvoiceEditModel.OfficeAddressCity = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName : "";
                estimateInvoiceEditModel.OfficeAddressState = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName : "";
                estimateInvoiceEditModel.OfficeAddressCountry = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country) != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name : ""; ;
                estimateInvoiceEditModel.OfficeAddressZipCode = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode : ""; ;

                estimateInvoiceEditModel.EstimateDate = estimateInvoice.JobScheduler.StartDateTimeString.ToString();
                var invoiceNumberCount = 1;
                var invoiceItemCount = estimateInvoiceServicesGroupedData.Count();

                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    estimateInvoiceEditModel.Option1Total = 0;
                    estimateInvoiceEditModel.Option2Total = 0;
                    estimateInvoiceEditModel.Option3Total = 0;

                    if (customerSignature != null && customerSignature.Count() > 0)
                    {
                        var customerSignatureForInvoice = customerSignature.FirstOrDefault(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                        if (customerSignatureForInvoice != null)
                        {
                            estimateInvoiceEditModel.CustomerSignature = customerSignatureForInvoice.Signature;
                            estimateInvoiceEditModel.SignDateTime = customerSignatureForInvoice.SignedDateTime != null && customerSignatureForInvoice.SignedDateTime != default(DateTime?)
                                                                                            ? _clock.ToLocal(customerSignatureForInvoice.SignedDateTime.GetValueOrDefault()).ToString("MM/dd/yyyy") : "";
                        }
                    }

                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);

                    var list = estimateInvoiceServicesLocal.ToList();
                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    estimateInvoiceEditModel.LessDepositPer = (int)estimateInvoice.Franchisee.LessDeposit;
                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";
                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));
                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption1.ToString()));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption2.ToString()));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption3.ToString()));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);

                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";
                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";
                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }
                    var firstName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var LastName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    fileFullName = fileFullName.Replace(".", "");
                    fileFullName = fileFullName.Replace(":", "");
                    var fileName = fileFullName + ".pdf";

                    if (invoiceItemCount == 1)
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id.ToString();
                    }
                    else
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id + "-" + invoiceNumberCount;
                        invoiceNumberCount += 1;
                    }
                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    fileDomain.Add(GetFileModel(file));
                    index += 1;

                }
                using (var zip = new ZipFile())
                {
                    foreach (var domain in fileDomain)
                    {
                        var file = _fileRepository.Get(domain.Id);
                        if (file != null)
                        {
                            string filePath = file.RelativeLocation + @"\" + file.Name;
                            zip.AddFile(filePath, "");
                        }
                    }
                    var fileName2 = "InvoiceCustomerDownload-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                    var fileName = "InvoiceCustomerDownload-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".zip";
                    var rootPath = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "/" + fileName;
                    zip.Save(rootPath);

                    return fileName2;
                }
            }
            catch (Exception e1)
            {
                var a = e1.InnerException;
                return "";
            }
        }

        public string UploadInvoicesCustomerZipFile(long schedulerId, List<int> serviceInvoice)
        {
            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);
            if (estimateInvoice == null)
            {
                return "";
            }
            var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();
            estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => serviceInvoice.Contains(x.Key)).ToList();
            if (estimateInvoiceServicesGroupedData.Count() == 0)
            {
                return "";
            }
            try
            {
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.CustomerName = RemoveInvalidChars(estimateInvoiceEditModel.CustomerName);
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";

                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : " ";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : " ";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : " ";
                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).Select(x => x.Id).ToList();
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                int index = 1;
                var fileDomain = new List<Application.Domain.File>();
                var destinationFolder = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + "cutomer_invoice.cshtml");
                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();

                estimateInvoiceEditModel.OfficeName = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Name != null ? estimateInvoice.Franchisee.Organization.Name : "";
                estimateInvoiceEditModel.OfficeAddressLine1 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1 : "";
                estimateInvoiceEditModel.OfficeAddressLine2 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2 : "";
                estimateInvoiceEditModel.OfficeAddressCity = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName : "";
                estimateInvoiceEditModel.OfficeAddressState = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName : "";
                estimateInvoiceEditModel.OfficeAddressCountry = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country) != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name : ""; ;
                estimateInvoiceEditModel.OfficeAddressZipCode = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode : ""; ;

                estimateInvoiceEditModel.EstimateDate = estimateInvoice.JobScheduler.StartDateTimeString.ToString();
                var invoiceNumberCount = 1;
                var invoiceItemCount = estimateInvoiceServicesGroupedData.Count();

                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    estimateInvoiceEditModel.Option1Total = 0;
                    estimateInvoiceEditModel.Option2Total = 0;
                    estimateInvoiceEditModel.Option3Total = 0;

                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);

                    var list = estimateInvoiceServicesLocal.ToList();
                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    estimateInvoiceEditModel.LessDepositPer = (int)estimateInvoice.Franchisee.LessDeposit;

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));


                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption1.ToString()));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption2.ToString()));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption3.ToString()));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);

                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));


                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";

                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";

                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";

                    estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : " ";
                    estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : " ";
                    estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : " ";

                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }

                    var firstName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var LastName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    fileFullName = fileFullName.Replace(".", "");
                    fileFullName = fileFullName.Replace(":", "");
                    var fileName = fileFullName + ".pdf";

                    var signatureViewModel = GetSignature(estimateInvoice.Id, estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.CustomerSignature = signatureViewModel.PreSignature;
                    estimateInvoiceEditModel.SignDateTime = signatureViewModel.PreSignatureDate;
                    estimateInvoiceEditModel.CustomerPostSignature = signatureViewModel.PostSignature;
                    estimateInvoiceEditModel.PostSignDateTime = signatureViewModel.PostSignatureDate;
                    estimateInvoiceEditModel.Technician = signatureViewModel.Technician;

                    if (invoiceItemCount == 1)
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id.ToString();
                    }
                    else
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id + "-" + invoiceNumberCount;
                        invoiceNumberCount += 1;
                    }

                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    fileDomain.Add(GetFileModel(file));
                    index += 1;

                }
                using (var zip = new ZipFile())
                {
                    foreach (var domain in fileDomain)
                    {
                        var file = _fileRepository.Get(domain.Id);
                        if (file != null)
                        {
                            string filePath = file.RelativeLocation + @"\" + file.Name;
                            zip.AddFile(filePath, "");
                        }
                    }
                    var fileName2 = "InvoiceCustomerDownload-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now);
                    var fileName = "InvoiceCustomerDownload-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".zip";
                    var rootPath = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "/" + fileName;
                    zip.Save(rootPath);

                    return fileName2;
                }
            }
            catch (Exception e1)
            {
                _logService.Error(e1 + "   " + e1.InnerException);
                var a = e1.InnerException;
                return "";
            }
        }

        private string GetTechnicianName(List<EstimateInvoiceAssignee> assigneeList)
        {
            string techName = "";
            List<string> techNames = new List<string>();
            foreach (var jobScheduler in assigneeList)
            {
                techNames.Add(jobScheduler.User.Person != null ? jobScheduler.User.Person.FirstName + " " + jobScheduler.User.Person.LastName : "");
            }
            if (techNames.Count == 0)
            {
                techName = "-";
            }
            techName = string.Join(", ", techNames);
            return techName;
        }

        private bool AddingWorkOrderInvoice(List<TechnicianWorkOrderForInvoice> domainList, long? invoiceNumber)
        {
            foreach (var domain in domainList)
            {
                domain.IsNew = true;
                domain.InvoiceNumber = invoiceNumber;
                _technicianWorkOrderInvoiceRepository.Save(domain);
                _unitOfWork.SaveChanges();
            }
            return true;
        }

        private bool DeletingWorkOrderInvoice(long estimateInvoiceId)
        {
            var domainList = _technicianWorkOrderInvoiceRepository.Table.Where(x => x.EstimateinvoiceId == estimateInvoiceId).ToList();
            foreach (var domain in domainList)
            {
                domain.IsNew = false;
                _technicianWorkOrderInvoiceRepository.Delete(domain);
            }
            return true;
        }

        private List<ListViewModelForWorkOrder> CreateWorkOrderViewModel(long estimateInvoiceId)
        {
            var workOrderTechnician = _technicianWorkOrderInvoiceRepository.Table.Where(x => x.EstimateinvoiceId == estimateInvoiceId).ToList();
            var workOrderTechnicianViewModel = workOrderTechnician.Select(x => _estimateInvoiceFactory.CreateViewModel(x)).ToList();
            return workOrderTechnicianViewModel;
        }

        public bool AddInvoiceToEstimate(long schedulerId, bool isInvoiceForJob, List<long?> invoiceNumbers = null, long? jobId = null, long? scheduleJobId = null)
        {
            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);
            var sliderDomain = new JobEstimateServiceViewModel();
            if (estimateInvoice == null)
            {
                return false;
            }

            var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
            var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).OrderByDescending(x => x.Id);
            var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();
            if (estimateInvoiceServicesGroupedData.Count() == 0)
            {
                return false;
            }
            try
            {
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";

                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : "";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : "";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : "";
                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).Select(x => x.Id).ToList();
                if (jobId != null)
                {
                    jobForEstimateList = jobForEstimateList.Where(x => x.Equals(jobId)).ToList();
                }
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                int index = 1;
                var fileDomain = new List<Application.Domain.File>();
                var invoiceMapping = new List<FileMappedToInvoice>();
                var destinationFolder = MediaLocationHelper.GetDocumentImageLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + "cutomer_invoice.cshtml");
                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                if (invoiceNumbers != null)
                {
                    estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => invoiceNumbers.Contains(x.Key)).ToList();

                }
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();
                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    estimateInvoiceEditModel.Option1Total = 0;
                    estimateInvoiceEditModel.Option2Total = 0;
                    estimateInvoiceEditModel.Option3Total = 0;
                    var list = estimateInvoiceServicesLocal.ToList();
                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);
                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    estimateInvoiceEditModel.LessDepositPer = (int)estimateInvoice.Franchisee.LessDeposit;
                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption1.ToString()));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption2.ToString()));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.SubItemTotalSumOption3.ToString()));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);

                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";

                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";

                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }

                    var firstName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var LastName = estimateInvoiceEditModel.CustomerName.Split(' ')[0];
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    fileFullName = fileFullName.Replace(".", "");
                    fileFullName = fileFullName.Replace(":", "");
                    var fileName = fileFullName + ".pdf";

                    var signatureViewModel = GetSignature(estimateInvoice.Id, estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.CustomerSignature = signatureViewModel.PreSignature;
                    estimateInvoiceEditModel.SignDateTime = signatureViewModel.PreSignatureDate;
                    estimateInvoiceEditModel.CustomerPostSignature = signatureViewModel.PostSignature;
                    estimateInvoiceEditModel.PostSignDateTime = signatureViewModel.PostSignatureDate;
                    estimateInvoiceEditModel.Technician = signatureViewModel.Technician;

                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    var fileModel = GetFileModel(file);
                    fileDomain.Add(fileModel);
                    var invoiceMap = new FileMappedToInvoice()
                    {
                        File = fileModel.Id,
                        InvoiceNumber = estimateInvoiceServicesLocal.Key
                    };
                    invoiceMapping.Add(invoiceMap);
                    foreach (var assignee in assignees)
                    {
                        assignee.Label = fileName;
                        assignee.IsNew = false;
                        _estimateInvoiceAssigneeRepository.Save(assignee);
                    }
                    index += 1;
                }
                var categoryId = default(long?);
                var jobEstimateImageCategory = _jobEstimateImageCategory.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();
                var jobEstimateImageCategoryDomain = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);
                if (jobEstimateImageCategoryDomain == null)
                {
                    var estimateCategory = new JobEstimateImageCategory()
                    {
                        EstimateId = jobScheduler.EstimateId,
                        SchedulerId = jobScheduler.Id,
                        JobId = jobScheduler.JobId,
                        IsNew = true
                    };
                    _jobEstimateImageCategory.Save(estimateCategory);
                    categoryId = estimateCategory.Id;
                }
                else
                {
                    categoryId = jobEstimateImageCategoryDomain.Id;
                }
                var filesId = fileDomain.Select(x => (long?)x.Id).ToList();
                sliderDomain = new JobEstimateServiceViewModel()
                {
                    FilesList = fileDomain.Select(x => (long?)x.Id).ToList(),
                    RowId = estimateInvoice.Id,
                };
                SaveBuildingImages(sliderDomain, categoryId.GetValueOrDefault(), isInvoiceForJob, jobScheduler.Id, invoiceMapping, scheduleJobId);
                //ChangeInvoicesForJob(estimateInvoice.Id);
                return true;
            }
            catch (Exception e1)
            {
                var a = e1.InnerException;
                return false;
            }
        }

        private void SaveBuildingImages(JobEstimateServiceViewModel sliderDomain, long categoryId, bool isInvoiceForJob, long schedulerId, List<FileMappedToInvoice> fileMappedToInvoices, long? jobSchedulerId = null)
        {
            var inDbServicesIds = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long)BeforeAfterImagesType.Invoice && x.IsFromInvoiceAttach.Value && x.IsInvoiceForJob == isInvoiceForJob && (x.JobEstimateImageCategory.SchedulerId == schedulerId) && !x.IsFromEstimate).Select(x => x.Id).ToList();
            foreach (var fileIdsDelete in inDbServicesIds)
            {
                var jobEstimateImage = _jobEstimateServices.Get(fileIdsDelete);
                _jobEstimateServices.Delete(jobEstimateImage);
            }

            if (isInvoiceForJob)
            {
                var servicesIdsForJob = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long)BeforeAfterImagesType.Invoice && !x.IsFromEstimate && (x.JobEstimateImageCategory.SchedulerId == jobSchedulerId)).Select(x => x.Id).ToList();
                foreach (var fileIdsDelete in servicesIdsForJob)
                {
                    var jobEstimateImage = _jobEstimateServices.Get(fileIdsDelete);
                    _jobEstimateServices.Delete(jobEstimateImage);
                }

            }
            var estimateServices = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == sliderDomain.RowId).ToList();
            foreach (var fileId in sliderDomain.FilesList)
            {
                var map = fileMappedToInvoices.FirstOrDefault(x => x.File == fileId);
                var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, categoryId, (long?)LookupTypes.InvoiceImages);
                jobEstimateBeforeService.IsFromInvoiceAttach = true;
                jobEstimateBeforeService.IsInvoiceForJob = isInvoiceForJob;
                jobEstimateBeforeService.InvoiceNumber = map.InvoiceNumber;
                jobEstimateBeforeService.IsNew = true;
                _jobEstimateServices.Save(jobEstimateBeforeService);
                var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(sliderDomain, buildingExteriorServiceId, (long?)LookupTypes.InvoiceImages, fileId);
                jobEstimateBeforeServiceImage.IsNew = true;
                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                var services = estimateServices.Where(x => x.InvoiceNumber == map.InvoiceNumber).ToList();
                foreach (var service in services)
                {
                    service.InvoiceImageId = jobEstimateBeforeServiceImage.Id;
                    service.IsNew = false;
                    _estimateInvoiceServiceRepository.Save(service);
                }
            }
        }

        private bool DeletingMeasurementsInvoice(long estimateInvoiceServiceId)
        {
            var estimateInvoiceMeasurementList = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceServiceId == estimateInvoiceServiceId).ToList();
            foreach (var estimateInvoiceMeasurement in estimateInvoiceMeasurementList)
            {
                estimateInvoiceMeasurement.IsNew = false;
                _estimateInvoiceDimensionRepository.Delete(estimateInvoiceMeasurement);
            }
            return true;
        }

        private bool AddMeasurements(List<EstimateInvoiceDimensionEditModel> dimensionList, long estimateInvoiceServiceId, long? unitTypeId)
        {
            foreach (var dimension in dimensionList)
            {
                var unitType = default(long?);
                var estimateInvoiceServiceMeasurementDomain = _estimateInvoiceFactory.CreateDomain(dimension, estimateInvoiceServiceId);
                var serviceTypeDomain = _lookupRepository.Get(unitTypeId.GetValueOrDefault());

                if (serviceTypeDomain != null)
                {
                    var unitDomain = _lookupRepository.Table.FirstOrDefault(x => x.Alias == serviceTypeDomain.Name);
                    if (unitDomain == null)
                    {
                        unitDomain = _lookupRepository.Table.FirstOrDefault(x => x.Alias == serviceTypeDomain.Alias);
                    }
                    unitType = unitDomain.Id;
                }

                estimateInvoiceServiceMeasurementDomain.IncrementedPrice = dimension.IncrementedPrice;
                estimateInvoiceServiceMeasurementDomain.SetPrice = dimension.SetPrice;
                estimateInvoiceServiceMeasurementDomain.UnitTypeId = unitType;
                estimateInvoiceServiceMeasurementDomain.IsNew = true;
                _estimateInvoiceDimensionRepository.Save(estimateInvoiceServiceMeasurementDomain);
            }
            return true;
        }

        private bool AddMeasurements(List<EstimateInvoiceDimensionViewModel> dimensionList, long estimateInvoiceServiceId, long? unitTypeId)
        {
            foreach (var dimension in dimensionList)
            {
                var unitType = default(long?);
                var estimateInvoiceServiceMeasurementDomain = _estimateInvoiceFactory.CreateDomain(dimension, estimateInvoiceServiceId);
                var serviceTypeDomain = _lookupRepository.Get(unitTypeId.GetValueOrDefault());
                if (serviceTypeDomain != null)
                {
                    var unitDomain = _lookupRepository.Table.FirstOrDefault(x => x.Alias == serviceTypeDomain.Name);
                    if (unitDomain != null)
                        unitType = unitDomain.Id;
                    else
                    {
                        if (serviceTypeDomain.Name == "LINEAR FT")
                        {
                            unitDomain = _lookupRepository.Table.FirstOrDefault(x => x.Alias == "LINEARFT");
                            unitType = unitDomain.Id;
                        }
                    }
                }
                estimateInvoiceServiceMeasurementDomain.UnitTypeId = unitType;
                estimateInvoiceServiceMeasurementDomain.IsNew = true;
                estimateInvoiceServiceMeasurementDomain.SetPrice = dimension.SetPrice;
                estimateInvoiceServiceMeasurementDomain.IncrementedPrice = dimension.IncrementedPrice;
                _estimateInvoiceDimensionRepository.Save(estimateInvoiceServiceMeasurementDomain);
            }
            return true;
        }

        private Core.Application.Domain.File SaveFile(FileModel model)
        {
            var path = MediaLocationHelper.FilePath(model.RelativeLocation, model.Name).ToFullPath();
            var destination = MediaLocationHelper.GetACustomerInvoiceLocation();
            var destFileName = string.Format((model.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)));
            //var destFileName = model.Caption;
            var fileName = _fileService.MoveFile(path, destination, destFileName, model.Extension);
            model.Name = destFileName + model.Extension;
            model.RelativeLocation = destination.Path;
            model.Caption = destFileName;
            var file = _fileService.SaveModel(model);
            return file;
        }

        public int SendMailToCustomerForSignedInvoices(long? schedulerId, string templateName, long? userId, bool isFromUrl, bool? IsFromJob, long? jobSchedulerId)
        {
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            try
            {
                var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);

                if (estimateInvoice == null)
                {
                    return -1;
                }

                var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
                var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();

                var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();

                var destinationFolder = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + templateName);

                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == jobScheduler.EstimateId).Select(x => x.Id).ToList();
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";

                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : " ";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : " ";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : " ";
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                estimateInvoiceEditModel.SchedulerId = estimateInvoice.SchedulerId;

                int index = 1;
                var fileDomain = new List<Application.Domain.File>();

                var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).OrderByDescending(x => x.Id);
                var invoicesSigned = customerSignature.Select(x => x.InvoiceNumber).ToList();
                Random _random = new Random();
                var code = GetCode();
                estimateInvoiceEditModel.Code = code;
                estimateInvoiceEditModel.Url = _settings.SignatureUrl;
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();

                List<string> signedInvoicesName = new List<string>();
                List<string> unsignedInvoicesName = new List<string>();

                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    if (invoicesSigned.Contains(estimateInvoiceServicesLocal.Key))
                    {
                        signedInvoicesName.Add("Invoice_" + estimateInvoiceServicesLocal.Key);
                    }
                    else
                    {
                        unsignedInvoicesName.Add("Invoice_" + estimateInvoiceServicesLocal.Key);
                    }
                }
                if (signedInvoicesName.Count > 0)
                {
                    estimateInvoiceEditModel.SignedInvoicesName = string.Join(", ", signedInvoicesName);
                }
                else
                {
                    estimateInvoiceEditModel.SignedInvoicesName = string.Empty;
                }
                if (unsignedInvoicesName.Count > 0)
                {
                    estimateInvoiceEditModel.UnsignedInvoicesName = string.Join(" ,", unsignedInvoicesName);
                }
                else
                {
                    estimateInvoiceEditModel.UnsignedInvoicesName = string.Empty;
                }
                if (unsignedInvoicesName.Count == 0)
                {
                    estimateInvoiceEditModel.AllInvoicesSigned = true;
                }
                else
                {
                    estimateInvoiceEditModel.AllInvoicesSigned = false;
                }

                estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => invoicesSigned.Contains(x.Key)).ToList();

                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                if (estimateInvoiceServicesGroupedData.Count() == 0)
                {
                    return -1;
                }
                estimateInvoiceEditModel.IsSigned = "block";

                estimateInvoiceEditModel.OfficeName = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Name != null ? estimateInvoice.Franchisee.Organization.Name : "";
                estimateInvoiceEditModel.OfficeAddressLine1 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1 : "";
                estimateInvoiceEditModel.OfficeAddressLine2 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2 : "";
                estimateInvoiceEditModel.OfficeAddressCity = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName : "";
                estimateInvoiceEditModel.OfficeAddressState = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName : "";
                estimateInvoiceEditModel.OfficeAddressCountry = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country) != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name : "";
                estimateInvoiceEditModel.OfficeAddressZipCode = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode : "";

                estimateInvoiceEditModel.EstimateDate = estimateInvoice.JobScheduler.StartDateTimeString.ToString();
                var invoiceNumberCount = 1;
                var invoiceItemCount = estimateInvoiceServicesGroupedData.Count();
                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    estimateInvoiceEditModel.ServiceList = new List<EstimateInvoiceServiceEditMailModel>();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    var list = estimateInvoiceServicesLocal.ToList();

                    if (customerSignature != null && customerSignature.Count() > 0)
                    {
                        var customerSignatureForInvoice = customerSignature.FirstOrDefault(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                        if (customerSignatureForInvoice != null)
                        {
                            estimateInvoiceEditModel.CustomerSignature = customerSignatureForInvoice.Signature;
                            estimateInvoiceEditModel.SignDateTime = customerSignatureForInvoice.SignedDateTime != null && customerSignatureForInvoice.SignedDateTime != default(DateTime?)
                                                                                            ? _clock.ToLocal(customerSignatureForInvoice.SignedDateTime.GetValueOrDefault()).ToString("MM/dd/yyyy") : "";
                        }
                    }

                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);

                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option1Total));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option2Total));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option3Total));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);


                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";
                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";
                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoiceNotes != null && estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";
                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    var fileName = fileFullName + ".pdf";

                    if (invoiceItemCount == 1)
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id.ToString();
                    }
                    else
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id + "-" + invoiceNumberCount;
                        invoiceNumberCount += 1;
                    }
                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    fileDomain.Add(GetFileModel(file));

                    index += 1;
                }
                jobScheduler.IsCustomerMailSend = true;
                _jobSchedulerRepository.Save(jobScheduler);

                SendingInvoiceToCustomerForSignedInvoices(estimateInvoiceEditModel, fileDomain, NotificationTypes.MailToCustomerForSignedInvoice, estimateInvoiceEditModel.Email, userId, isFromUrl, false);
                SendingInvoiceToCustomerForSignedInvoices(estimateInvoiceEditModel, fileDomain, NotificationTypes.MailToSalesRepForSignedInvoice, estimateInvoiceEditModel.SalesRepEmail, userId, isFromUrl, true);
                if (unsignedInvoicesName.Count > 0)
                {
                    SaveCustomerSignatureInfo(estimateInvoice, code, IsFromJob, jobSchedulerId);
                }

                return 1;
            }
            catch (Exception e1)
            {
                var a = e1.InnerException;
                return 0;
            }

        }

        public int SendMailToCustomerForSignedInvoicesPost(long? schedulerId, string templateName, long? userId, bool isFromUrl, bool? IsFromJob, long? jobSchedulerId, CustomersignatureViewModel model)
        {
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            try
            {
                var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);

                if (estimateInvoice == null)
                {
                    return -1;
                }

                var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
                var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();

                var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();

                var destinationFolder = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + templateName);

                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == jobScheduler.EstimateId).Select(x => x.Id).ToList();
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";
                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : " ";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : " ";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : " ";
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                estimateInvoiceEditModel.SchedulerId = estimateInvoice.SchedulerId;

                int index = 1;
                var fileDomain = new List<Application.Domain.File>();

                var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id && x.TypeId == model.TypeId).OrderByDescending(x => x.Id);
                var invoicesSigned = customerSignature.Select(x => x.InvoiceNumber).ToList();

                var code = GetCode();
                estimateInvoiceEditModel.Code = code;
                estimateInvoiceEditModel.Url = _settings.SignatureUrl;
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId).ToList();

                List<string> signedInvoicesName = new List<string>();
                List<string> unsignedInvoicesName = new List<string>();

                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    if (invoicesSigned.Contains(estimateInvoiceServicesLocal.Key))
                    {
                        signedInvoicesName.Add("Invoice_" + estimateInvoiceServicesLocal.Key);
                    }
                    else
                    {
                        unsignedInvoicesName.Add("Invoice_" + estimateInvoiceServicesLocal.Key);
                    }
                }
                if (signedInvoicesName.Count > 0)
                {
                    estimateInvoiceEditModel.SignedInvoicesName = string.Join(" ,", signedInvoicesName);
                }
                else
                {
                    estimateInvoiceEditModel.SignedInvoicesName = string.Empty;
                }
                if (unsignedInvoicesName.Count > 0)
                {
                    estimateInvoiceEditModel.UnsignedInvoicesName = string.Join(" ,", unsignedInvoicesName);
                }
                else
                {
                    estimateInvoiceEditModel.UnsignedInvoicesName = string.Empty;
                }
                if (unsignedInvoicesName.Count == 0)
                {
                    estimateInvoiceEditModel.AllInvoicesSigned = true;
                }
                else
                {
                    estimateInvoiceEditModel.AllInvoicesSigned = false;
                }

                estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => invoicesSigned.Contains(x.Key)).ToList();

                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                if (estimateInvoiceServicesGroupedData.Count() == 0)
                {
                    return -1;
                }
                estimateInvoiceEditModel.IsSigned = "block";

                estimateInvoiceEditModel.OfficeName = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Name != null ? estimateInvoice.Franchisee.Organization.Name : "";
                estimateInvoiceEditModel.OfficeAddressLine1 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine1 : "";
                estimateInvoiceEditModel.OfficeAddressLine2 = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().AddressLine2 : "";
                estimateInvoiceEditModel.OfficeAddressCity = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().CityName : "";
                estimateInvoiceEditModel.OfficeAddressState = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().StateName : "";
                estimateInvoiceEditModel.OfficeAddressCountry = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country) != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().Country.Name : "";
                estimateInvoiceEditModel.OfficeAddressZipCode = estimateInvoice != null && estimateInvoice.Franchisee != null && estimateInvoice.Franchisee.Organization != null && estimateInvoice.Franchisee.Organization.Address != null && (estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode) != null ? estimateInvoice.Franchisee.Organization.Address.FirstOrDefault().ZipCode : "";

                estimateInvoiceEditModel.EstimateDate = estimateInvoice.JobScheduler.StartDateTimeString.ToString();
                var invoiceNumberCount = 1;
                var invoiceItemCount = estimateInvoiceServicesGroupedData.Count();
                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    estimateInvoiceEditModel.ServiceList = new List<EstimateInvoiceServiceEditMailModel>();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    var list = estimateInvoiceServicesLocal.ToList();

                    var signatureViewModel = GetSignature(estimateInvoice.Id, estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.CustomerSignature = signatureViewModel.PreSignature;
                    estimateInvoiceEditModel.SignDateTime = signatureViewModel.PreSignatureDate;
                    estimateInvoiceEditModel.CustomerPostSignature = signatureViewModel.PostSignature;
                    estimateInvoiceEditModel.PostSignDateTime = signatureViewModel.PostSignatureDate;
                    estimateInvoiceEditModel.Technician = signatureViewModel.Technician;

                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);

                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option1Total));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option2Total));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option3Total));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);

                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";
                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";
                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoiceNotes != null && estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";
                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    var fileName = fileFullName + ".pdf";

                    if (invoiceItemCount == 1)
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id.ToString();
                    }
                    else
                    {
                        estimateInvoiceEditModel.InvoiceNumber = estimateInvoice.Id + "-" + invoiceNumberCount;
                        invoiceNumberCount += 1;
                    }
                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    fileDomain.Add(GetFileModel(file));

                    index += 1;
                }
                jobScheduler.IsCustomerMailSend = true;
                _jobSchedulerRepository.Save(jobScheduler);

                estimateInvoiceEditModel.IsFromJob = model.IsFromJob;
                estimateInvoiceEditModel.IsFromUrl = model.IsFromURL;

                SendingInvoiceToCustomerForSignedInvoices(estimateInvoiceEditModel, fileDomain, NotificationTypes.PostJobFeedbackToCustomer, estimateInvoiceEditModel.Email, userId, isFromUrl, false);
                SendingInvoiceToCustomerForSignedInvoices(estimateInvoiceEditModel, fileDomain, NotificationTypes.PostJobFeedbackToSalesRep, estimateInvoiceEditModel.SalesRepEmail, userId, isFromUrl, true);
                if (unsignedInvoicesName.Count > 0)
                {
                    SaveCustomerSignatureInfo(estimateInvoice, code, IsFromJob, jobSchedulerId);
                }

                return 1;
            }
            catch (Exception e1)
            {
                var a = e1.InnerException;
                return 0;
            }

        }


        private SignatureViewModel GetSignature(long? estimateInvoiceId, long? invoiceNumber)
        {
            var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoiceId).OrderByDescending(x => x.Id).ToList();

            SignatureViewModel signatureViewModel = new SignatureViewModel();
            if (customerSignature != null && customerSignature.Count() > 0)
            {
                var customerSignaturePreSigned = customerSignature.Where(x => x.TypeId == (long)SignatureType.PRECOMPLETION).OrderByDescending(x => x.Id).ToList();
                var customerSignatureForInvoice = customerSignaturePreSigned.FirstOrDefault(x => x.InvoiceNumber == invoiceNumber);
                if (customerSignaturePreSigned != null && customerSignatureForInvoice != null)
                {
                    signatureViewModel.PreSignature = customerSignatureForInvoice.Signature;
                    signatureViewModel.IsSigned = "none";
                    signatureViewModel.PreSignatureDate = customerSignatureForInvoice.SignedDateTime != null && customerSignatureForInvoice.SignedDateTime != default(DateTime?)
                                                                                    ? _clock.ToLocal(customerSignatureForInvoice.SignedDateTime.GetValueOrDefault()).ToString("MM/dd/yyyy") : "";

                }
                else
                {
                    if (customerSignaturePreSigned == null)
                    {
                        signatureViewModel.IsSigned = "block";
                    }
                }

                var customerSignaturePostSigned = customerSignature.Where(x => x.TypeId == (long)SignatureType.POSTCOMPLETION).OrderByDescending(x => x.Id).ToList();
                customerSignatureForInvoice = customerSignaturePostSigned.FirstOrDefault(x => x.InvoiceNumber == invoiceNumber);
                if (customerSignaturePostSigned != null && customerSignatureForInvoice != null)
                {
                    signatureViewModel.PostSignature = customerSignatureForInvoice.Signature;
                    signatureViewModel.IsSigned = "none";
                    signatureViewModel.PostSignatureDate = customerSignatureForInvoice.SignedDateTime != null && customerSignatureForInvoice.SignedDateTime != default(DateTime?)
                                                                                    ? _clock.ToLocal(customerSignatureForInvoice.SignedDateTime.GetValueOrDefault()).ToString("MM/dd/yyyy") : "";
                    signatureViewModel.Technician = customerSignatureForInvoice != null && customerSignatureForInvoice.JobScheduler != null ? customerSignatureForInvoice.JobScheduler.Person.FirstName + " " + customerSignatureForInvoice.JobScheduler.Person.LastName : " ";
                }
            }
            if (customerSignature.Count() == 0)
            {
                signatureViewModel.IsSigned = "block";
            }

            return signatureViewModel;
        }
        public bool SendFeedBackMailToCustomer(SelectInvoicesViewModel model)
        {
            throw new NotImplementedException();
        }

        public bool CustomerIsAvailableOrNot(SelectInvoicesViewModel model)
        {
            var estimateInvoice = _estimateInvoiceRepository.Get(model.EstimateInvoiceId.GetValueOrDefault());
            estimateInvoice.IsCustomerAvailable = model.IsCustomerAvailable;
            _estimateInvoiceRepository.Save(estimateInvoice);

            var jobScheduler = _jobSchedulerRepository.Get(model.JobSchedulerId.GetValueOrDefault());
            jobScheduler.IsCustomerAvailable = model.IsCustomerAvailable.GetValueOrDefault();
            _jobSchedulerRepository.Save(jobScheduler);
            _unitOfWork.SaveChanges();

            if (!model.IsCustomerAvailable.GetValueOrDefault())
            {
                var estimateInvoiceEditMailModel = SendMailToCustomerForCustomerAvailability(model, "cutomer_invoice.cshtml", NotificationTypes.PostJobFeedbackToCustomer);

                estimateInvoiceEditMailModel.SchedulerId = model.JobSchedulerId;
                SendMailToCustomerForCustomerAvailabilityWithFileDomain(model, "cutomer_invoice.cshtml", NotificationTypes.PostJobFeedbackToAdmin, estimateInvoiceEditMailModel, estimateInvoiceEditMailModel.FileDomainList);

            }

            return true;
        }


        private EstimateInvoiceEditMailModel SendMailToCustomerForCustomerAvailability(SelectInvoicesViewModel model, string templateName, NotificationTypes notificationId)
        {
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            try
            {
                var estimateInvoice = _estimateInvoiceRepository.Get(model.EstimateInvoiceId.GetValueOrDefault());

                if (estimateInvoice == null)
                {
                    return default(EstimateInvoiceEditMailModel);
                }

                var estimateInvoiceServicesMaster = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList();
                var estimateInvoiceServices = estimateInvoiceServicesMaster.Where(x => x.ParentId == null).ToList();

                var estimateInvoiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id && x.AssigneeId == model.UserId).ToList();
                var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();

                var destinationFolder = MediaLocationHelper.GetACustomerInvoiceLocation().Path + "\\";
                var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + templateName);

                var jobScheduler = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler : null;
                var jobForEstimateList = _jobRepository.Table.Where(x => x.EstimateId == jobScheduler.EstimateId).Select(x => x.Id).ToList();
                var jobSchedulerForEstimateList = _jobSchedulerRepository.Table.Where(x => jobForEstimateList.Contains(x.JobId.Value)).ToList();
                estimateInvoiceEditModel.Body = model.Body;
                estimateInvoiceEditModel.FileModel = model.FileModel;
                estimateInvoiceEditModel.CustomerName = estimateInvoice.EstimateInvoiceCustomer.CustomerName;
                estimateInvoiceEditModel.Address = estimateInvoice.EstimateInvoiceCustomer.StreetAddress;
                estimateInvoiceEditModel.PhoneNumber1 = FormatPhoneNumber(estimateInvoice.EstimateInvoiceCustomer.PhoneNumber1);
                estimateInvoiceEditModel.Email = estimateInvoice.EstimateInvoiceCustomer.Email;
                estimateInvoiceEditModel.SalesRepEmail = estimateInvoice.JobScheduler != null ? estimateInvoice.JobScheduler.Person.Email : "";

                estimateInvoiceEditModel.City = estimateInvoice.EstimateInvoiceCustomer.CityName;
                estimateInvoiceEditModel.FranchiseeName = estimateInvoice.Franchisee.Organization.Name;
                estimateInvoiceEditModel.FranchiseeId = estimateInvoice.Franchisee.Id;
                estimateInvoiceEditModel.Franchisee = estimateInvoice.Franchisee;
                estimateInvoiceEditModel.Option1 = estimateInvoice.Option1 != null ? estimateInvoice.Option1 : " ";
                estimateInvoiceEditModel.Option2 = estimateInvoice.Option2 != null ? estimateInvoice.Option2 : " ";
                estimateInvoiceEditModel.Option3 = estimateInvoice.Option3 != null ? estimateInvoice.Option3 : " ";
                estimateInvoiceEditModel.SalesRep = jobScheduler != null ? jobScheduler.Person.Name.FirstName + " " + jobScheduler.Person.Name.LastName : "";
                estimateInvoiceEditModel.ChooseOption = estimateInvoice.Option == "option1" ? "Option 1" : estimateInvoice.Option == "option2" ? "Option 2" : "Option 3";
                estimateInvoiceEditModel.SchedulerId = estimateInvoice.SchedulerId;

                int index = 1;
                var fileDomain = new List<Application.Domain.File>();
                var fileDomainForReturn = new List<Application.Domain.File>();
                var templateList = _termsAndConditionFranchiseeRepository.Table.Where(x => x.FranchiseeId == estimateInvoice.FranchiseeId).ToList();
                if (estimateInvoiceServicesGroupedData.Count() == 0)
                {
                    return default(EstimateInvoiceEditMailModel);
                }
                var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id && x.TypeId == model.TypeId).OrderByDescending(x => x.Id);

                var code = GetCode();
                estimateInvoiceEditModel.Code = code;
                estimateInvoiceEditModel.Url = _settings.SignatureUrl;
                var estimateInvoiceServiceMeasurements = _estimateInvoiceDimensionRepository.Table.Where(x => x.EstimateInvoiceService.EstimateInvoice != null && x.EstimateInvoiceService.EstimateInvoice.SchedulerId == jobScheduler.Id).ToList();
                var estimateInvoiceServiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId == estimateInvoice.EstimateId && x.AssigneeId == model.UserId).ToList();
                if (estimateInvoiceServicesGroupedData.Count() == customerSignature.Count())
                {
                    estimateInvoiceEditModel.IsSigned = "none";
                }
                else
                {
                    estimateInvoiceEditModel.IsSigned = "block";
                }
                var invoiceNumbersList = estimateInvoiceServiceAssignee.Select(x => x.InvoiceNumber).ToList();
                estimateInvoiceServicesGroupedData = estimateInvoiceServicesGroupedData.Where(x => invoiceNumbersList.Contains(x.Key)).ToList();
                foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
                {
                    estimateInvoiceEditModel.ServiceList = new List<EstimateInvoiceServiceEditMailModel>();
                    var measurements = estimateInvoiceServiceMeasurements.Where(x => x.EstimateInvoiceService.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.EstimateInvoiceDimensionTables = GetMeasurements(measurements, estimateInvoiceServicesLocal.ToList());
                    var list = estimateInvoiceServicesLocal.ToList();


                    if (customerSignature.Count() == 0)
                    {
                        estimateInvoiceEditModel.IsSigned = "block";
                    }

                    var assignees = estimateInvoiceServiceAssignee.Where(x => x.InvoiceNumber == estimateInvoiceServicesLocal.Key).ToList();
                    estimateInvoiceEditModel.TechName = GetTechnicianName(assignees);

                    var signatureViewModel = GetSignature(estimateInvoice.Id, estimateInvoiceServicesLocal.Key);

                    estimateInvoiceEditModel.CustomerSignature = signatureViewModel.PreSignature;
                    estimateInvoiceEditModel.SignDateTime = signatureViewModel.PreSignatureDate;
                    estimateInvoiceEditModel.CustomerPostSignature = signatureViewModel.PostSignature;
                    estimateInvoiceEditModel.PostSignDateTime = signatureViewModel.PostSignatureDate;
                    estimateInvoiceEditModel.Technician = signatureViewModel.Technician;

                    estimateInvoiceEditModel.ServiceList = list.Select(x => _estimateInvoiceFactory.CreateMailViewModel(x, estimateInvoiceServicesMaster.Where(x1 => x1.ParentId == x.Id).ToList(), templateList)).ToList();

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option1));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option2));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => decimal.Parse(x.Option3));

                    estimateInvoiceEditModel.Option1Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option1Total));
                    estimateInvoiceEditModel.Option2Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option2Total));
                    estimateInvoiceEditModel.Option3Total += estimateInvoiceEditModel.ServiceList.Sum(x => (x.Option3Total));
                    estimateInvoiceEditModel.NotesCount = estimateInvoiceEditModel.ServiceList.Where(x => !string.IsNullOrEmpty(x.Notes)).Count() + estimateInvoiceEditModel.ServiceList.Sum(x => x.SubItemNotesCount);

                    var price = estimateInvoice.Option == "option1" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option1Total, 2)) : estimateInvoice.Option == "option2" ? String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option2Total, 2)) : String.Format("{0:0.00}", Math.Round((double)estimateInvoiceEditModel.Option3Total, 2));
                    estimateInvoiceEditModel.Price = decimal.Parse(price);
                    estimateInvoiceEditModel.LessDepositPer = (int)(decimal.Round(estimateInvoice.Franchisee.LessDeposit.GetValueOrDefault(), 2));
                    estimateInvoiceEditModel.LessDeposit = decimal.Parse(String.Format("{0:0.00}", Math.Round(((double)(estimateInvoiceEditModel.Price * estimateInvoice.Franchisee.LessDeposit)) / 100D, 2)));
                    estimateInvoiceEditModel.Balance = decimal.Parse(String.Format("{0:0.00}", Math.Round((double)(estimateInvoiceEditModel.Price - estimateInvoiceEditModel.LessDeposit), 2)));

                    estimateInvoiceEditModel.PhoneNumber = estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)) != null ?
                        FormatPhoneNumber(estimateInvoice.Franchisee.Organization.Phones.FirstOrDefault(x => x.TypeId == (long)(PhoneType.Office)).Number) : "";
                    estimateInvoiceEditModel.Template = estimateInvoiceEditModel.ServiceList.Count() > 0 ? estimateInvoiceEditModel.ServiceList.FirstOrDefault().Template : "";
                    var estimateInvoiceNotes = _estimateServiceInvoiceNotesRepository.Table.FirstOrDefault(x => x.EstimateinvoiceId == estimateInvoice.Id && x.InvoiceNumber == estimateInvoiceServicesLocal.Key);
                    estimateInvoiceEditModel.EstimateNote = (estimateInvoiceNotes != null && estimateInvoice.Notes != "" && estimateInvoice.Notes != null) ? estimateInvoice.Notes : "";
                    estimateInvoiceEditModel.InvoiceNote = (estimateInvoiceNotes != null && estimateInvoiceNotes.Notes != "" && estimateInvoiceNotes.Notes != null) ? estimateInvoiceNotes.Notes : "";
                    if (estimateInvoice.Franchisee.SchedulerEmail != null)
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.SchedulerEmail;
                    }
                    else
                    {
                        estimateInvoiceEditModel.CommunicationEmail = estimateInvoice.Franchisee.ContactEmail;
                    }
                    var fileFullName = GetFileNameName(estimateInvoiceEditModel.CustomerName, estimateInvoiceServicesLocal.FirstOrDefault(), true);
                    var fileName = fileFullName + ".pdf";
                    var file = _pdfFileService.GeneratePdfFromTemplateAndModel(estimateInvoiceEditModel, destinationFolder, fileName, viewPath);
                    fileDomain.Add(GetFileModel(file));
                    fileDomainForReturn.Add(GetFileModel(file));

                    index += 1;
                }
                foreach (var file in model.FileModel)
                {
                    fileDomain.Add(SaveFile(file));
                }

                jobScheduler.IsCustomerMailSend = true;
                _jobSchedulerRepository.Save(jobScheduler);

                var invoiceNumbers = estimateInvoiceAssignee.Select(x => x.InvoiceNumber).ToList();
                var estimateInvoiceServicesMasterLocal = estimateInvoiceServicesMaster.Where(x => invoiceNumbers.Contains(x.InvoiceNumber)).ToList();
                var estimateInvoiceEditMailModel = GetSignedAndUnSignedInvoices(estimateInvoiceServicesMasterLocal, customerSignature.ToList());

                estimateInvoiceEditModel.SignedInvoicesName = estimateInvoiceEditMailModel.SignedInvoicesName;
                estimateInvoiceEditModel.UnsignedInvoicesName = estimateInvoiceEditMailModel.UnsignedInvoicesName;
                estimateInvoiceEditModel.AllInvoicesSigned = estimateInvoiceEditMailModel.AllInvoicesSigned;

                SendingInvoiceToCustomer(estimateInvoiceEditModel, fileDomain, notificationId, estimateInvoiceEditModel.Email, model.UserId);
                SaveCustomerSignatureInfo(estimateInvoice, code, true, model.JobSchedulerId);
                estimateInvoiceEditModel.FileDomainList = fileDomainForReturn;
                return estimateInvoiceEditModel;
            }
            catch (Exception e1)
            {
                var a = e1.InnerException;
                return default(EstimateInvoiceEditMailModel);
            }
        }

        private void SendMailToCustomerForCustomerAvailabilityWithFileDomain(SelectInvoicesViewModel model, string templateName, NotificationTypes notificationId, EstimateInvoiceEditMailModel estimateInvoiceEditModel, List<Application.Domain.File> fileDomain)
        {
            SendingInvoiceToCustomer(estimateInvoiceEditModel, fileDomain, notificationId, estimateInvoiceEditModel.Email, model.UserId);
        }

        private EstimateInvoiceEditMailModel GetSignedAndUnSignedInvoices(List<EstimateInvoiceService> estimateInvoiceServices, List<CustomerSignature> customerSignature)
        {
            var estimateInvoiceEditModel = new EstimateInvoiceEditMailModel();
            var invoicesSigned = customerSignature.Select(x => x.InvoiceNumber).ToList();
            var estimateInvoiceServicesGroupedData = estimateInvoiceServices.GroupBy(x => x.InvoiceNumber).ToList();

            List<string> signedInvoicesName = new List<string>();
            List<string> unsignedInvoicesName = new List<string>();

            foreach (var estimateInvoiceServicesLocal in estimateInvoiceServicesGroupedData)
            {
                if (invoicesSigned.Contains(estimateInvoiceServicesLocal.Key))
                {
                    signedInvoicesName.Add("Invoice_" + estimateInvoiceServicesLocal.Key);
                }
                else
                {
                    unsignedInvoicesName.Add("Invoice_" + estimateInvoiceServicesLocal.Key);
                }
            }

            if (signedInvoicesName.Count > 0)
            {
                estimateInvoiceEditModel.SignedInvoicesName = string.Join(", ", signedInvoicesName);
            }
            else
            {
                estimateInvoiceEditModel.SignedInvoicesName = string.Empty;
            }
            if (unsignedInvoicesName.Count > 0)
            {
                estimateInvoiceEditModel.UnsignedInvoicesName = string.Join(" ,", unsignedInvoicesName);
            }
            else
            {
                estimateInvoiceEditModel.UnsignedInvoicesName = string.Empty;
            }
            if (unsignedInvoicesName.Count == 0)
            {
                estimateInvoiceEditModel.AllInvoicesSigned = true;
            }
            else
            {
                estimateInvoiceEditModel.AllInvoicesSigned = false;
            }
            return estimateInvoiceEditModel;
        }


        private long? GetMeasurementType(EstimateInvoiceService service)
        {
            var splittedServices = service.ServiceName.Split(',');
            var servicestag = _servicesTagRepository.Table.Where(x => x.ServiceType.Name == service.ServiceType && x.MaterialType == service.StoneType2 && splittedServices.Contains(x.Service)).FirstOrDefault();
            if ((service.ServiceType == "COUNTERLIFE" && service.StoneType2 == "Engineered Stone") && (splittedServices.Contains("Engineered Stone-Polish") || splittedServices.Contains("Engineered Stone-Scratch Removal")))
            {
                return (long)MeasurementEnum.TIME;
            }
            else if (servicestag != null)
                return servicestag.CategoryId;
            else
                return (long)MeasurementEnum.AREA;
        }


        private long? GetUnitType(long? unitTypeId)
        {
            if (unitTypeId == (long?)MeasurementEnum.AREA)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == MeasurementEnum.AREA.ToString()).Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.EVENT)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == MeasurementEnum.EVENT.ToString()).Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.LINEARFT)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == MeasurementEnum.LINEARFT.ToString()).Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.MAINTENANCE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == MeasurementEnum.MAINTENANCE.ToString()).Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.PRODUCTPRICE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == MeasurementEnum.PRODUCTPRICE.ToString()).Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.TAXRATE)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == MeasurementEnum.TAXRATE.ToString()).Id;
            }
            else if (unitTypeId == (long?)MeasurementEnum.TIME)
            {
                return _lookupRepository.Table.FirstOrDefault(x => x.Alias == MeasurementEnum.TIME.ToString()).Id;
            }
            return null;
        }

        private string GetCode()
        {
            Random _random = new Random();
            var finalCode = "";

            for (var a = 1; a <= 10; a++)
            {
                var code = _random.Next(0, 99999).ToString("D5");
                var isCodePresent = _customerSignatureInfoRepository.Table.Any(x => x.Code == code);
                if (!isCodePresent)
                {
                    finalCode = code;
                    break;
                }
                else
                {
                    continue;
                }
            }
            return finalCode;
        }

        private bool SaveMeasurementForHoingAndPolishing(long? serviceId, List<HoningMeasurementViewModel> honingMeasurementViewModelList)
        {
            var honingMeasurementList = _honingMeasurementRepository.Table.OrderByDescending(x => x.Id).Where(x => x.EstimateInvoiceServiceId == serviceId).ToList();
            var honingMeasurementDefaultList = _honingMeasurementDefaultRepository.Table.OrderByDescending(x => x.Id).Where(x => x.EstimateInvoiceServiceId == serviceId).ToList();
            var isShiftPriceChanged = false;
            foreach (var honingMeasurement in honingMeasurementList)
            {
                if (honingMeasurement != null)
                {
                    honingMeasurement.IsActive = false;
                    _honingMeasurementRepository.Save(honingMeasurement);
                }
            }
            foreach (var honingMeasurement in honingMeasurementDefaultList)
            {
                if (honingMeasurement != null)
                {
                    honingMeasurement.IsActive = false;
                    _honingMeasurementDefaultRepository.Save(honingMeasurement);
                }
            }
            if (honingMeasurementList.Count > 0)
            {
                var lastHoningMeasurement = honingMeasurementList.LastOrDefault();
                if (lastHoningMeasurement.ShiftPrice == honingMeasurementViewModelList.LastOrDefault().ShiftPrice)
                {
                    isShiftPriceChanged = false;
                }
                else
                {
                    isShiftPriceChanged = true;
                }
            }
            var count = 0;
            foreach (var honingMeasurementViewModel in honingMeasurementViewModelList)
            {
                var honingMeasurementDomain = new HoningMeasurement()
                {
                    SeventeenBase = honingMeasurementViewModel.SeventeenBase,
                    TotalArea = honingMeasurementViewModel.TotalArea,
                    Area = honingMeasurementViewModel.Area,
                    Length = honingMeasurementViewModel.Length,
                    Width = honingMeasurementViewModel.Width,
                    Caco = honingMeasurementViewModel.Caco,
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    Dimension = Convert.ToDecimal(honingMeasurementViewModel.Dimension),
                    EightHundred = honingMeasurementViewModel.EightHundred,
                    ElevenThousand = honingMeasurementViewModel.ElevenThousand,
                    Fifty = honingMeasurementViewModel.Fifty,
                    FourHundred = honingMeasurementViewModel.FourHundred,
                    Hundred = honingMeasurementViewModel.Hundred,
                    Ihg = honingMeasurementViewModel.Ihg,
                    IsActive = true,
                    MinRestoration = honingMeasurementViewModel.MinRestoration,
                    ProdutivityRate = honingMeasurementViewModel.ProdutivityRate,
                    Sections = honingMeasurementViewModel.Sections,
                    Thirty = honingMeasurementViewModel.Thirty,
                    TotalAreaInHour = honingMeasurementViewModel.TotalAreaInHour,
                    TotalAreaInShift = honingMeasurementViewModel.TotalAreaInShift,
                    TotalCostPerSquare = honingMeasurementViewModel.TotalCostPerSquare,
                    TotalCost = honingMeasurementViewModel.TotalCost,
                    TotalMinute = honingMeasurementViewModel.TotalMinute,
                    TwoHundred = honingMeasurementViewModel.TwoHundred,
                    UGC = honingMeasurementViewModel.UGC,
                    IsNew = true,
                    EstimateInvoiceServiceId = serviceId,
                    ShiftPrice = honingMeasurementViewModel.ShiftPrice,
                    ShiftName = honingMeasurementViewModel.ShiftName,
                    ThreeThousand = honingMeasurementViewModel.ThreeThousand,
                    FifteenHundred = honingMeasurementViewModel.FifteenHundred,
                    EightThousand = honingMeasurementViewModel.EightThousand,
                    StartingPointTechShiftEstimates = honingMeasurementViewModel.StartingPointTechShiftEstimates,
                    RowDescription = honingMeasurementViewModel.RowDescription,
                    IsShiftPriceChanged = count == (honingMeasurementViewModelList.Count - 1) ? isShiftPriceChanged : false
                };
                _honingMeasurementRepository.Save(honingMeasurementDomain);
                count++;
                var honingMeasurementDefaultDomain = new HoningMeasurementDefault()
                {
                    SectionsDefault = honingMeasurementViewModel.Sections,
                    UGCDefault = honingMeasurementViewModel.UGCOriginal,
                    ThirtyDefault = honingMeasurementViewModel.ThirtyOriginal,
                    FiftyDefault = honingMeasurementViewModel.FiftyOriginal,
                    HundredDefault = honingMeasurementViewModel.HundredOriginal,
                    TwoHundredDefault = honingMeasurementViewModel.TwoHundredOriginal,
                    FourHundredDefault = honingMeasurementViewModel.FourHundredOriginal,
                    EightHundredDefault = honingMeasurementViewModel.EightHundredOriginal,
                    FifteenHundredDefault = honingMeasurementViewModel.FifteenHundredOriginal,
                    ThreeThousandDefault = honingMeasurementViewModel.ThreeThousandOriginal,
                    EightThousandDefault = honingMeasurementViewModel.EightThousandOriginal,
                    ElevenThousandDefault = honingMeasurementViewModel.ElevenThousandOriginal,
                    CacoDefault = honingMeasurementViewModel.CacoOriginal,
                    IhgDefault = honingMeasurementViewModel.IhgOriginal,
                    DimensionDefault = Convert.ToDecimal(honingMeasurementViewModel.Dimension),
                    ProdutivityRateDefault = honingMeasurementViewModel.ProdutivityRateOriginal,
                    TotalMinuteDefault = honingMeasurementViewModel.TotalMinuteOriginal,
                    SeventeenBaseDefault = honingMeasurementViewModel.SeventeenBaseOriginal,
                    TotalAreaDefault = honingMeasurementViewModel.TotalAreaOriginal,
                    TotalAreaInHourDefault = honingMeasurementViewModel.TotalAreaInHourOriginal,
                    TotalAreaInShiftDefault = honingMeasurementViewModel.TotalAreaInShiftOriginal,
                    TotalCostDefault = honingMeasurementViewModel.TotalCostOriginal,
                    TotalCostPerSquareDefault = honingMeasurementViewModel.TotalCostPerSquareOriginal,
                    IsActive = true,
                    MinRestorationDefault = honingMeasurementViewModel.MinRestorationOriginal,
                    IsNew = true,
                    EstimateInvoiceServiceId = serviceId,
                    HoningMeasurementId = honingMeasurementDomain.Id
                };
                _honingMeasurementDefaultRepository.Save(honingMeasurementDefaultDomain);
            }
            return true;
        }

        public EstimateInvoiceServiceGetServiceResultModel GetServiceTypeId(EstimateInvoiceServiceGetServiceModel model)
        {
            var resultModel = new EstimateInvoiceServiceGetServiceResultModel();
            var serviceId = default(long?);
            var splittedServices = model.ServiceName.Split(',');
            var isBundle = model.ServiceName.StartsWith("Bundle");
            if (!isBundle)
            {
                var servicestag = _servicesTagRepository.Table.Where(x => x.ServiceType.Name == model.ServiceType && x.MaterialType == model.TypeOfStoneType2 && splittedServices.Contains(x.Service)).FirstOrDefault();
                if ((model.ServiceType == "COUNTERLIFE" && model.TypeOfStoneType2 == "Engineered Stone") && (splittedServices.Contains("Engineered Stone-Polish") || splittedServices.Contains("Engineered Stone-Scratch Removal")))
                {
                    serviceId = (long)MeasurementEnum.TIME;
                }
                else if (servicestag != null)
                    serviceId = servicestag.CategoryId;
                else
                    serviceId = (long)MeasurementEnum.AREA;
            }
            else
            {
                var servicestag = _servicesTagRepository.Table.Where(x => x.ServiceType.Name == model.ServiceType && x.MaterialType == model.TypeOfStoneType2 && splittedServices.Contains(x.Service)).FirstOrDefault();
                if (servicestag != null)
                    serviceId = servicestag.CategoryId;
                else
                    serviceId = (long)MeasurementEnum.TIME;
            }

            resultModel.ServiceId = serviceId;
            return resultModel;
        }

        private bool SaveInvoiceImages(long? serviceId, List<EstimateInvoiceImageEditModel> estimateInvoiceImage, long? estimateId, long estimateInvoiceId, long? userId)
        {
            var uploadedImages = _estimateInvoiceServiceImageRepository.Table.Where(x => x.EstimateInvoiceServiceId == serviceId).ToList();

            var alreadyUploadedFiles = uploadedImages.Select(x => x.FileId).ToList();
            var alreadyUploadedFilesMathcing = estimateInvoiceImage.Where(x => alreadyUploadedFiles.Contains(x.FileId)).Select(x => x.FileId).ToList();
            if (uploadedImages.Count > 0)
            {
                foreach (var image in uploadedImages)
                {
                    if (!alreadyUploadedFilesMathcing.Contains(image.FileId))
                    {
                        _estimateInvoiceServiceImageRepository.Delete(image);
                    }
                }
            }
            foreach (var image in estimateInvoiceImage)
            {
                if (!alreadyUploadedFilesMathcing.Contains(image.FileId))
                {
                    EstimateInvoiceServiceImage serviceImage = new EstimateInvoiceServiceImage();
                    serviceImage.EstimateInvoiceServiceId = serviceId;
                    serviceImage.EstimateId = estimateId;
                    serviceImage.EstimateInvoiceId = estimateInvoiceId;
                    serviceImage.FileId = image.FileId;
                    serviceImage.IsBeforeAfter = true;
                    serviceImage.IsNew = true;
                    serviceImage.DataRecorderMetaData = new DataRecorderMetaData(userId.GetValueOrDefault());
                    _estimateInvoiceServiceImageRepository.Save(serviceImage);
                }
            }
            return true;
        }
        public string RemoveInvalidChars(string filename)
        {
            return string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
