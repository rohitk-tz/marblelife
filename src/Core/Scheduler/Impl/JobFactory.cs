using Core.Application;
using Core.Application.Attribute;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Application.ViewModel;
using Core.Geo;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Notification.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Application.Domain;
using Core.Billing.Enum;
using Core.Sales.Domain;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobFactory : IJobFactory
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAddressFactory _addressFactory;
        private readonly IClock _clock;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        public readonly IRepository<JobScheduler> _jobSchedulerRepository;
        public readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        public readonly IRepository<OrganizationRoleUserFranchisee> _organizationRoleUserFranchiseeRepository;
        public readonly IRepository<Meeting> _meetingRepository;
        public readonly IRepository<Organization> _organizationRepository;
        public readonly IRepository<Franchisee> _franchiseeRepository;
        public readonly IRepository<Person> _personRepository;
        public readonly IRepository<DataRecorderMetaData> _dataRecorderMetaDataRepository;
        public readonly IRepository<JobEstimateImage> _jobEstimateImageRepository;
        public readonly IRepository<EquipmentUserDetails> _equipmentUserDetailsRepository;
        public readonly IRepository<UserLogin> _userLogimRepository;
        public readonly IRepository<EmailTemplate> _emailTemplateRepository;
        public readonly IRepository<EstimateInvoice> _estimateInvoiceRepository;
        private readonly IRepository<EstimateInvoiceAssignee> _estimateInvoiceAssigneeRepository;
        private readonly IRepository<CustomerSignature> _customerSignatureRepository;
        private readonly IRepository<EstimateInvoiceService> _estimateInvoiceServiceRepository;
        private readonly IRepository<JobEstimate> _jobEstimateRepository;
        private readonly IRepository<Core.Application.Domain.File> _fileRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateServicesRepository;
        private readonly IRepository<JobEstimateImageCategory> _jobEstimateImageCategoryRepository;
        private readonly IRepository<BeforeAfterImages> _beforeAfterImageRepository;
        //public readonly IRepository<Application.Domain.File> _fileRepository;

        private readonly ISettings _settings;



        public JobFactory(IAddressFactory addressFactory, IClock clock, IOrganizationRoleUserInfoService organizationRoleUserInfoService, IUnitOfWork unitOfWork, ISettings settings)
        {
            _jobEstimateServicesRepository = unitOfWork.Repository<JobEstimateServices>();
            _jobEstimateImageCategoryRepository = unitOfWork.Repository<JobEstimateImageCategory>();
            _fileRepository = unitOfWork.Repository<Core.Application.Domain.File>();
            _unitOfWork = unitOfWork;
            _addressFactory = addressFactory;
            _clock = clock;
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _meetingRepository = unitOfWork.Repository<Meeting>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _organizationRoleUserFranchiseeRepository = unitOfWork.Repository<OrganizationRoleUserFranchisee>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _personRepository = unitOfWork.Repository<Person>();
            _dataRecorderMetaDataRepository = unitOfWork.Repository<DataRecorderMetaData>();
            _settings = settings;
            _jobEstimateImageRepository = unitOfWork.Repository<JobEstimateImage>();
            _equipmentUserDetailsRepository = unitOfWork.Repository<EquipmentUserDetails>();
            _userLogimRepository = unitOfWork.Repository<UserLogin>();
            _emailTemplateRepository = unitOfWork.Repository<EmailTemplate>();
            _estimateInvoiceRepository = unitOfWork.Repository<EstimateInvoice>();
            _estimateInvoiceAssigneeRepository = unitOfWork.Repository<EstimateInvoiceAssignee>();
            _customerSignatureRepository = unitOfWork.Repository<CustomerSignature>();
            _estimateInvoiceServiceRepository = unitOfWork.Repository<EstimateInvoiceService>();
            _jobEstimateRepository = unitOfWork.Repository<JobEstimate>();
            _beforeAfterImageRepository = unitOfWork.Repository<BeforeAfterImages>();
        }

        public JobScheduler CreateDomain(JobSchedulerEditModel model)
        {
            var offset = _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes;

            var domain = new JobScheduler
            {
                Id = model.Id,
                IsActive = model.IsActive,
                AssigneeId = model.AssigneeId > 0 ? model.AssigneeId : null,
                JobId = model.JobId,
                FranchiseeId = model.FranchiseeId,
                SalesRepId = model.SalesRepId > 0 ? model.SalesRepId : null,
                EstimateId = model.EstimateId > 0 ? model.EstimateId : null,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData.Id,
                ServiceTypeId = model.ServiceTypeId > 0 ? model.ServiceTypeId : null,
                IsImported = model.IsImported,
                IsNew = model.Id <= 0,
                Offset = offset,
                StartDateTimeString = model.ActualStartDateString,
                EndDateTimeString = model.ActualEndDateString,
                SchedulerStatus = model.SchedulerStatus == 0 || model.SchedulerStatus == null ? (long)ConfirmationEnum.NotResponded : model.SchedulerStatus.GetValueOrDefault()
            };
            return domain;
        }

        public Job CreateDomain(JobEditModel model)
        {
            var domain = new Job
            {
                Id = model.JobId,
                JobTypeId = model.JobTypeId,
                StatusId = model.StatusId,
                CustomerId = model.JobCustomer != null ? model.JobCustomer.CustomerId : 0,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                EstimateId = model.EstimateId > 0 ? model.EstimateId : null,
                QBInvoiceNumber = model.QBInvoiceNumber,
                GeoCode = model.GeoCode != null ? model.GeoCode.ToUpper() : null,
                IsNew = model.JobId <= 0,
                StartDateTimeString = model.ActualStartDateString,
                EndDateTimeString = model.ActualEndDateString
            };
            return domain;
        }

        public JobCustomer CreateDomain(JobCustomerEditModel model)
        {
            var address = _addressFactory.CreateDomain(model.Address);
            var domain = new JobCustomer
            {
                Id = model.CustomerId,
                AddressId = (model.AddressId > 0 && !string.IsNullOrEmpty(address.AddressLine1)) ? model.AddressId : null,
                CustomerName = model.CustomerName,
                Address = address,
                CustomerAddress = model.Address.AddressLine1,
                Email = model.Email != null ? model.Email : "",
                PhoneNumber = model.PhoneNumber,
                IsNew = model.CustomerId <= 0
            };
            return domain;
        }

        public JobResource CreateDomain(FileUploadModel model, long fileId)
        {
            var domain = new JobResource
            {
                StatusId = model.StatusId > 0 ? model.StatusId : null,
                FileId = fileId,
                JobId = model.JobId != null ? model.JobId : null,
                VacationId = model.VacationId != null ? model.VacationId : null,
                EstimateId = model.EstimateId != null ? model.EstimateId : null,
                MeetingId = model.MeetingId != null ? model.MeetingId : null,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData.Id,
                IsNew = true
            };
            return domain;
        }

        public JobViewModel CreateViewModel(JobScheduler domain, List<EstimateInvoice> estimateInvoiceList,
            List<JobScheduler> schedulerList, List<EstimateInvoiceService> serviceList,
            List<EstimateInvoiceAssignee> estimateInvoiceAssignees,
            List<EquipmentUserDetails> equipmentUserDetails, List<long?> customerSignatures, List<OrganizationRoleUser> organizationRoleUsers)
        {
            bool isLocked = false;
            bool isParent = false;
            bool isInvoiceSigned = false;
            var serviceTypeNames = string.Empty;
            var alias = domain.OrganizationRoleUser != null &&
            domain.OrganizationRoleUser.Person.FirstName != null && domain.OrganizationRoleUser.Person.FirstName.Length > 0 &&
            domain.OrganizationRoleUser.Person.LastName != null && domain.OrganizationRoleUser.Person.LastName.Length > 0 ?
            (domain.OrganizationRoleUser.Person.FirstName[0].ToString().ToUpper() +
            domain.OrganizationRoleUser.Person.LastName[0].ToString().ToUpper())
            : null;
            //var createdBy = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value);
            var createdBy = organizationRoleUsers.Where(x => x.Id == domain.DataRecorderMetaData.CreatedBy.Value).Select(x => x.Person).FirstOrDefault();
            var geoCode = domain.JobId > 0 ? GetGeoCodeForJob(domain) :
                domain.EstimateId > 0 ? GetGeoCodeForEstimate(domain) :
                GetGeoCodeForPersonal(domain);
            if (domain.MeetingID != null)
            {
                isParent = domain.Meeting.ParentId.HasValue;
            }
            if (domain.Person.UserLogin == null)
            {
                var equipmentUserDetailsDomain = equipmentUserDetails.FirstOrDefault(x => x.UserId == domain.OrganizationRoleUser.UserId);
                if (equipmentUserDetailsDomain != null)
                {
                    isLocked = equipmentUserDetailsDomain.IsLock;
                }
            }
            var estimateInvoiceAssignee = new List<EstimateInvoiceAssignee>();
            //var jobAmount = default(decimal);
            var jobAmountList = new List<JobAmount>();
            var isInvoicePresent = false;
            if (domain.EstimateId != null)
            {
                var estimateInvoice = estimateInvoiceList.FirstOrDefault(x => x.EstimateId == domain.EstimateId);
                if (estimateInvoice != null)
                {
                    isInvoicePresent = true;
                    var customerSignature = customerSignatures.FirstOrDefault(x => x == estimateInvoice.Id);
                    if (customerSignature != default(long))
                    {
                        isInvoiceSigned = true;
                    }
                    var serviceTypeNamesList = new List<string>();
                    serviceTypeNamesList = serviceList.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).Select(x => x.ServiceType).Distinct().ToList();
                    if (serviceTypeNamesList.Count > 3)
                    {
                        serviceTypeNames = string.Join(", ", serviceTypeNamesList.GetRange(0, 3)) + "....";
                    }
                    else
                    {
                        serviceTypeNames = string.Join(", ", serviceTypeNamesList);
                    }
                }

            }
            if (domain.JobId != null)
            {
                var job = domain.Job;
                var estimateScheduler = new JobScheduler();
                if (job != null)
                {
                    estimateScheduler = schedulerList.FirstOrDefault(x => x.EstimateId == job.EstimateId);
                }
                var estimateInvoiceForJob = new EstimateInvoice();
                if (estimateScheduler != null)
                {
                    estimateInvoiceForJob = estimateInvoiceList.FirstOrDefault(x => x.SchedulerId == estimateScheduler.Id);
                }
                var serviceTypeNamesList = new List<string>();
                if (estimateInvoiceForJob != default(EstimateInvoice))
                {
                    serviceTypeNamesList = serviceList.Where(x => x.EstimateInvoiceId == estimateInvoiceForJob.Id)
                        .Select(x => x.ServiceType).Distinct().ToList();
                    if (serviceTypeNamesList.Count > 3)
                    {
                        serviceTypeNames = string.Join(", ", serviceTypeNamesList.GetRange(0, 3)) + "....";
                    }
                    else
                    {
                        serviceTypeNames = string.Join(", ", serviceTypeNamesList);
                    }
                }
                estimateInvoiceAssignee = estimateInvoiceAssignees.Where(x => x.EstimateId == job.EstimateId).ToList();
                var jobEstimate = new List<EstimateInvoiceService>();
                var chooseOption = string.Empty;
                var jobAmount = new JobAmount();
                if (estimateInvoiceAssignee != null)
                {
                    var invoiceNumbers = estimateInvoiceAssignee.Select(x => x.InvoiceNumber).ToList().Distinct();
                    foreach (var number in invoiceNumbers)
                    {
                        jobAmount = new JobAmount();
                        jobAmount.JobValue = decimal.Zero;
                        jobAmount.InvoiceNumber = number.Value;
                        var assignee = estimateInvoiceAssignee.FirstOrDefault(x => x.InvoiceNumber == number);
                        jobAmount.EstimateInvoiceId = assignee.EstimateInvoiceId.Value;
                        jobEstimate = serviceList.Where(x => x.EstimateInvoiceId == assignee.EstimateInvoiceId && x.InvoiceNumber == number).ToList();
                        chooseOption = assignee.EstimateInvoice != null ? (assignee.EstimateInvoice.Option == "option1" ? "Option 1" : assignee.EstimateInvoice.Option == "option2" ? "Option 2" : "Option 3") : string.Empty;
                        if (jobEstimate.Count > 0)
                        {
                            if (chooseOption == "Option 1")
                            {
                                jobAmount.JobValue += jobEstimate.Sum(x => decimal.Parse(x.Option1));
                            }
                            else if (chooseOption == "Option 2")
                            {
                                jobAmount.JobValue += jobEstimate.Sum(x => decimal.Parse(x.Option2));
                            }
                            else if (chooseOption == "Option 3")
                            {
                                jobAmount.JobValue += jobEstimate.Sum(x => decimal.Parse(x.Option3));
                            }
                            else
                            {
                                jobAmount.JobValue += default(decimal);
                            }
                            jobAmountList.Add(jobAmount);
                        }
                    }
                }
            }
            var estimateInvoiceItem = estimateInvoiceList.Where(x => x.EstimateId == domain.EstimateId).FirstOrDefault();
            var model = new JobViewModel
            {
                Id = domain.Id,
                JobTitle = domain.Title,
                JobId = domain.JobId != null ? domain.JobId.Value : 0,
                EstimateId = domain.EstimateId != null ? domain.EstimateId.Value : 0,
                Assignee = domain.Person != null ? domain.Person.Name.ToString() : null,
                Alias = alias,
                TechId = domain.AssigneeId != null ? domain.AssigneeId.Value : 0,
                ActualStartDate = (domain.StartDateTimeString),
                ActualEndDate = (domain.EndDateTimeString),
                start = (domain.StartDateTimeString),
                end = (domain.EndDateTimeString),
                Franchisee = domain.Franchisee.Organization.Name,
                JobType = domain.Job != null ? domain.Job.JobType.Name : null,
                EstimateType = domain.Estimate != null ? (domain.Estimate.MarketingClass != null ? domain.Estimate.MarketingClass.Name : null) : null,
                QBinvoiceNumber = domain.Job != null ? domain.Job.QBInvoiceNumber : null,
                SchedulerTitle = domain.Title,
                StatusId = domain.Job != null ? (domain.Job.StatusId > 0 ? domain.Job.StatusId : 0) : 0,
                Status = domain.Job != null ? (domain.Job.JobStatus != null ? domain.Job.JobStatus.Name : null) : null,
                backgroundColor = domain.OrganizationRoleUser != null ? (domain.OrganizationRoleUser.ColorCode != null ? domain.OrganizationRoleUser.ColorCode : domain.OrganizationRoleUser.ColorCodeSale) : "#FF0000",
                color = domain.OrganizationRoleUser != null ? (domain.OrganizationRoleUser.ColorCode != null ? domain.OrganizationRoleUser.ColorCode : domain.OrganizationRoleUser.ColorCodeSale) : "#FF0000",
                StatusColor = domain.Job != null ? (domain.Job.JobStatus != null ? domain.Job.JobStatus.ColorCode : "#C0C0C0") : null,
                SalesRep = domain.SalesRep != null ? (domain.SalesRep.Person.Name.ToString()) : null,
                JobCustomer = domain.Job != null ? CreateCustomerModel(domain.Job.JobCustomer) : (domain.Estimate == null ? null : CreateCustomerModel(domain.Estimate.JobCustomer)),
                GeoCode = geoCode,
                durationEditable = false,
                CreatedBy = createdBy != null ? createdBy.Email : "",
                Imported = domain.IsImported,
                IsVacation = domain.IsVacation,
                MeetingId = domain.MeetingID.GetValueOrDefault(),
                isParent = isParent,
                IsLock = domain.Person.UserLogin != null ? domain.Person.UserLogin.IsLocked : isLocked,
                SchedulerStatus = domain.JobId != null || domain.EstimateId != null ? domain.SchedulerStatus : default(long?),
                SchedulerStatusColor = GetColorCode(domain.SchedulerStatus),
                IsCustomerMailSend = domain.IsCustomerMailSend,
                Amount = isInvoicePresent ? domain.Estimate.Amount : (decimal)0.00,
                IsInvoiceSigned = isInvoiceSigned,
                JobAmount = jobAmountList,
                ServiceTypeName = serviceTypeNames == string.Empty ? null : serviceTypeNames,
                IsInvoicePresent = isInvoicePresent,
                EstimatedAmount = domain.Estimate != null ? domain.Estimate.Amount : (decimal)0.00,
                InvoiceAmount = estimateInvoiceItem != null ? estimateInvoiceItem.PriceOfService : (float)0.00
            };
            model.title = model.JobCustomer == null ? model.SchedulerTitle : model.JobCustomer.CustomerName;
            if (model.backgroundColor == null)
            {
                model.backgroundColor = "#C0C0C0";
            }
            model.title = model.JobCustomer == null ? model.SchedulerTitle : model.JobCustomer.CustomerName;
            if (model.IsVacation && ((model.start.ToString("HH:mm") == "00:00" && model.end.ToString("HH:mm") == "23:59")))
            {
                model.start = model.start.Date.ToUniversalTime();
                model.end = model.end.Date.AddDays(1).ToUniversalTime();
            }
            else if ((model.end - model.start).TotalDays > 1 || (model.start.ToString("HH:mm") == "08:00" && model.end.ToString("HH:mm") == "17:00"))
            {
                model.start = model.start.Date;
                model.end = model.end.Date.AddDays(1);
            }
            model.allDay = (model.start.ToString("HH:mm") == "00:00" && model.end.ToString("HH:mm") == "00:00") ? true : false;
            if (model.IsVacation && model.allDay)
            {
                model.ActualStartDate = model.start.Date.ToUniversalTime();
                model.ActualEndDate = model.end.Date.AddMinutes(-1).ToUniversalTime();
            }
            return model;
        }



        public JobViewModel CreateViewModelForListView(JobScheduler domain, List<EquipmentUserDetails> equipmentUserDetails)
        {
            bool isLocked = false;
            bool isParent = false;
            var alias = domain.OrganizationRoleUser != null &&
            domain.OrganizationRoleUser.Person.FirstName != null && domain.OrganizationRoleUser.Person.FirstName.Length > 0 &&
            domain.OrganizationRoleUser.Person.LastName != null && domain.OrganizationRoleUser.Person.LastName.Length > 0 ?
            (domain.OrganizationRoleUser.Person.FirstName[0].ToString().ToUpper() +
            domain.OrganizationRoleUser.Person.LastName[0].ToString().ToUpper())
            : null;
            if (domain.MeetingID != null)
            {
                isParent = domain.Meeting.ParentId.HasValue;
            }
            if (domain.Person.UserLogin == null)
            {
                var equipmentUserDetailsDomain = equipmentUserDetails.FirstOrDefault(x => x.UserId == domain.OrganizationRoleUser.UserId);
                if (equipmentUserDetailsDomain != null)
                {
                    isLocked = equipmentUserDetailsDomain.IsLock;
                }
            }
            var NotesForEstimate2 = string.Empty;
            if (domain.Estimate != null && domain.Estimate.JobNote != null && domain.Estimate.JobNote.Count() > 0)
            {
                var NotesForEstimate1 = domain.Estimate.JobNote.Where(x => x.EstimateId != null && domain.EstimateId != null && x.EstimateId == domain.EstimateId).FirstOrDefault();
                NotesForEstimate2 = NotesForEstimate1.Note != null && NotesForEstimate1.Note != "" && NotesForEstimate1.Note != string.Empty ? NotesForEstimate1.Note : string.Empty;
            }
            var NotesForJob2 = string.Empty;
            if (domain.Job != null && domain.Job.JobNote != null && domain.Job.JobNote.Count() > 0)
            {
                var NotesForJob1 = domain.Job.JobNote.Where(x => x.JobId != null && domain.JobId != null && x.JobId == domain.JobId).FirstOrDefault();
                NotesForJob2 = NotesForJob1.Note != null && NotesForJob1.Note != "" && NotesForJob1.Note != string.Empty ? NotesForJob1.Note : string.Empty;
            }
            var isEstimateInvoicePresent = _estimateInvoiceRepository.Table.Any(x => x.EstimateId == domain.EstimateId);
            var estimateInvoice = _estimateInvoiceRepository.Table.Where(x => x.EstimateId == domain.EstimateId).Select(y => y.PriceOfService).FirstOrDefault();
            var model = new JobViewModel
            {
                Id = domain.Id,
                JobTitle = domain.Title,
                JobId = domain.JobId != null ? domain.JobId.Value : 0,
                EstimateId = domain.EstimateId != null ? domain.EstimateId.Value : 0,
                Assignee = domain.Person != null ? domain.Person.Name.ToString() : null,
                Alias = alias,
                TechId = domain.AssigneeId != null ? domain.AssigneeId.Value : 0,
                ActualStartDate = (domain.StartDateTimeString),
                ActualEndDate = (domain.EndDateTimeString),
                start = (domain.StartDateTimeString),
                end = (domain.EndDateTimeString),
                Franchisee = domain.Franchisee.Organization.Name,
                JobType = domain.Job != null ? domain.Job.JobType.Name : null,
                EstimateType = domain.Estimate != null ? (domain.Estimate.MarketingClass != null ? domain.Estimate.MarketingClass.Name : null) : null,
                QBinvoiceNumber = domain.Job != null ? domain.Job.QBInvoiceNumber : null,
                SchedulerTitle = domain.Title,
                StatusId = domain.Job != null ? (domain.Job.StatusId > 0 ? domain.Job.StatusId : 0) : 0,
                Status = domain.Job != null ? (domain.Job.JobStatus != null ? domain.Job.JobStatus.Name : null) : null,
                SalesRep = domain.SalesRep != null ? (domain.SalesRep.Person.Name.ToString()) : null,
                JobCustomer = domain.Job != null ? CreateCustomerModel(domain.Job.JobCustomer) : (domain.Estimate == null ? null : CreateCustomerModel(domain.Estimate.JobCustomer)),
                durationEditable = false,
                Imported = domain.IsImported,
                IsVacation = domain.IsVacation,
                MeetingId = domain.MeetingID.GetValueOrDefault(),
                isParent = isParent,
                IsLock = domain.Person.UserLogin != null ? domain.Person.UserLogin.IsLocked : isLocked,
                EstimatedValue = domain.Estimate != null ? domain.Estimate.Amount : 0,
                InvoiceValue = domain.Estimate != null ? estimateInvoice : 0,
                JobTotal = domain.Job != null && domain.Job.JobEstimate != null ? domain.Job.JobEstimate.Amount : 0,
                NotesForEstimate = NotesForEstimate2,
                NotesForJob = NotesForJob2,
                IsInvoicePresent = isEstimateInvoicePresent
            };
            model.title = model.JobCustomer == null ? model.SchedulerTitle : model.JobCustomer.CustomerName;
            if (model.backgroundColor == null)
            {
                model.backgroundColor = "#C0C0C0";
            }
            model.title = model.JobCustomer == null ? model.SchedulerTitle : model.JobCustomer.CustomerName;
            if (model.IsVacation && ((model.start.ToString("HH:mm") == "00:00" && model.end.ToString("HH:mm") == "23:59")))
            {
                model.start = model.start.Date.ToUniversalTime();
                model.end = model.end.Date.AddDays(1).ToUniversalTime();
            }
            else if ((model.end - model.start).TotalDays > 1 || (model.start.ToString("HH:mm") == "08:00" && model.end.ToString("HH:mm") == "17:00"))
            {
                model.start = model.start.Date;
                model.end = model.end.Date.AddDays(1);
            }
            model.allDay = (model.start.ToString("HH:mm") == "00:00" && model.end.ToString("HH:mm") == "00:00") ? true : false;
            if (model.IsVacation && model.allDay)
            {
                model.ActualStartDate = model.start.Date.ToUniversalTime();
                model.ActualEndDate = model.end.Date.AddMinutes(-1).ToUniversalTime();
            }
            return model;
        }


        private string GetGeoCodeForPersonal(JobScheduler domain)
        {
            if (domain.MeetingID != null || domain.MeetingID > 0)
                return "Meeting";
            else
                return "Personal";
        }
        private string GetGeoCodeForEstimate(JobScheduler domain)
        {
            if (domain.Estimate == null || domain.Estimate.GeoCode == null)
                return "Estimate";
            else
                return "Estimate - " + domain.Estimate.GeoCode;
        }

        private string GetGeoCodeForJob(JobScheduler domain)
        {
            if (domain.Job == null || domain.Job.GeoCode == null)
                return "Job";
            else
                return "Job - " + domain.Job.GeoCode;
        }

        public JobEditModel CreateEditModel(Job job, JobScheduler jobscheduler, List<EstimateInvoiceAssignee> estimateInvoiceAssignee = null)
        {
            var estimateInvoiceId = default(long?);
            var isInvoicePresent = false;
            var schedulerStatus = default(long);
            var schedulerId = default(long?);
            var estimateId = (jobscheduler != null && jobscheduler.Job != null) ? jobscheduler.Job.EstimateId : default(long?);
            if (estimateInvoiceAssignee == null)
            {
                estimateInvoiceAssignee = new List<EstimateInvoiceAssignee>();
            }
            var qBInvoiceNumber = "";
            var allAssigneeList = new List<JobScheduler>();
            allAssigneeList = job.JobScheduler.ToList();
            if (jobscheduler != default(JobScheduler))
            {
                qBInvoiceNumber = jobscheduler.QBInvoiceNumber;
            }
            var jobEstimateScheduler = new JobScheduler();
            var allAssignee = job.JobScheduler;
            if (allAssignee.Count() > 0)
            {
                allAssignee = allAssignee.Where(x => x.Person == null || x.Person.UserLogin.IsLocked == false).ToList();
            }
            var jobNotes = job.JobNote.OrderByDescending(x => x.Id);
            if (job.EstimateId != null)
            {
                jobEstimateScheduler = _jobSchedulerRepository.Table.Where(x => x.EstimateId == job.EstimateId).FirstOrDefault();
            }
            var techIds = allAssignee != null
                    ? allAssignee.Where(x => x.IsActive).Select(y => y.AssigneeId.Value).Distinct().ToList()
                    : new List<long>();
            if (jobscheduler == null)
            {
                jobscheduler = job.JobScheduler.FirstOrDefault(x => x.JobId == job.Id);
                if (jobscheduler != null)
                    schedulerStatus = jobscheduler.SchedulerStatus;
            }
            else
            {
                schedulerStatus = jobscheduler.SchedulerStatus;

            }
            if (estimateId != null)
            {

                isInvoicePresent = _estimateInvoiceRepository.Table.Any(x => x.EstimateId == estimateId);
                var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.EstimateId == estimateId);
                var jobScheduler = _jobSchedulerRepository.Table.FirstOrDefault(x => x.EstimateId == estimateId);
                schedulerId = jobScheduler.Id;
                estimateInvoiceId = estimateInvoice != null ? estimateInvoice.Id : default(long?);

            }
            var model = new JobEditModel
            {
                Id = jobscheduler.Id,
                JobId = job.Id,
                JobCustomer = CreateCustomerModel(job.JobCustomer),
                JobTypeId = job.JobTypeId,
                jobType = job.JobType.Name,
                JobSchedulerList = allAssignee.Where(y => y.IsActive).Select(x => CreateJobShedulerModel(x, estimateInvoiceAssignee.Where(x1 => x1.AssigneeId == x.AssigneeId).ToList())),
                QBInvoiceNumber = qBInvoiceNumber,
                Status = job.JobStatus.Name,
                StatusColor = job.JobStatus.ColorCode,
                StatusId = job.StatusId,
                StartDate = (job.StartDate),
                EndDate = (job.EndDate),
                TechIds = techIds,
                Description = job.Description,
                JobAssigneeIds = techIds,
                EstimateId = job.EstimateId,
                EstimateInvoiceId = estimateInvoiceId,
                Notes = jobNotes != null ? jobNotes.Select(x => CreateNoteViewModel(x)) : null,
                IsEstimateDeleted = job.JobEstimate == null ? true : false,
                GeoCode = job.GeoCode != null ? job.GeoCode.ToUpper() : null,
                ActualStartDateString = job.StartDateTimeString,
                ActualEndDateString = job.EndDateTimeString,
                EstimateSchedulerId = jobEstimateScheduler != default(JobScheduler) ? jobEstimateScheduler.Id : default(long?),
                SchedulerStatus = schedulerStatus,
                SchedulerStatusName = jobscheduler.Lookup.Name == "Confirmed" ? "Job " + jobscheduler.Lookup.Name : jobscheduler.Lookup.Name,
                SchedulerStatusColor = GetColorCode(jobscheduler.SchedulerStatus),
                IsActive = schedulerStatus == 218 ? true : false,
                IsRepeat = jobscheduler.IsRepeat,
                InvoiceId = jobscheduler.InvoiceId != null ? jobscheduler.InvoiceId : null,
                IsInvoicePresent = isInvoicePresent,
                IsDataToBeUpdateForAllJobs = true,
                IsCustomerMailSend = jobscheduler.IsCustomerMailSend,
                IsInvoiceRequired = jobscheduler.IsInvoiceRequired,
                CreatedOn = jobscheduler.DataRecorderMetaData.DateCreated
            };

            var info = allAssignee.FirstOrDefault();
            if (info == null)
            {
                info = allAssigneeList.FirstOrDefault();
            }
            if (info != null)
            {
                var createdBy = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(info.DataRecorderMetaData.CreatedBy.Value);
                model.FranchiseeId = info.FranchiseeId;
                model.Franchisee = info.Franchisee.Organization.Name;
                model.SalesRepId = info.SalesRepId;
                model.SalesRep = info.SalesRep != null ? info.SalesRep.Person.Name.ToString() : null;
                model.Title = info.Title;
                model.CreatedBy = createdBy.Email;
                model.SetGeoCode = info.Franchisee.SetGeoCode;
                model.DataRecorderMetaData = info.DataRecorderMetaData;
            }
            return model;
        }

        public JobCustomerEditModel CreateCustomerModel(JobCustomer domain)
        {
            var model = new JobCustomerEditModel
            {
                CustomerId = domain.Id,
                AddressId = domain.AddressId,
                Address = _addressFactory.CreateEditModel(domain.Address),
                CustomerName = domain.CustomerName,
                Email = domain.Email,
                PhoneNumber = domain.PhoneNumber,
                FullAddress = GetFullAddress(domain.Address),
            };
            return model;
        }

        public JobResourceEditModel CreateResouceModel(JobResource domain)
        {
            var createdBy = domain.DataRecorderMetaData != null ?
                _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value)
                : null;
            var model = new JobResourceEditModel
            {
                Id = domain.Id,
                Caption = domain.File.Caption,
                Size = domain.File.Size,
                RelativeLocation = (domain.File.RelativeLocation + "\\" + domain.File.Name).ToUrl(),
                FileId = domain.FileId,
                JobId = domain.JobId != null ? domain.JobId : null,
                EstimateId = domain.EstimateId != null ? domain.EstimateId : null,
                StatusId = domain.StatusId != null ? domain.StatusId : null,
                Status = domain.JobStatus != null ? domain.JobStatus.Name : null,
                FileName = domain.File.Name,
                FileType = domain.File.MimeType,
                CreatedOn = domain.DataRecorderMetaData != null ? domain.DataRecorderMetaData.DateCreated : (DateTime?)null,
                CreatedBy = createdBy != null ? createdBy.Email : null,
            };
            return model;
        }

        public JobEstimateCategoryViewModel CreatePairingModel(JobEstimateImageCategory domain, JobEstimateCategoryViewModel imageParentChild)
        {
            if (imageParentChild != null && imageParentChild.ImagePairs != null)
            {
                imageParentChild.ImagePairs = imageParentChild.ImagePairs.Where(x => x.AfterImages.DataRecorderMetaData != null).OrderByDescending(x => x.AfterImages.DataRecorderMetaData.DateCreated).ToList();
            }
            var model = new JobEstimateCategoryViewModel
            {
                Id = domain != null ? domain.Id : 0,
                MarketingClassId = domain != null ? domain.MarkertingClassId : default(long?),
                ImagePairs = imageParentChild != null ? imageParentChild.ImagePairs : null,
                InvoiceImages = imageParentChild != null ? imageParentChild.InvoiceImages : null,
                SliderImages = imageParentChild != null ? imageParentChild.SliderImages : null,
                IsChanged = false,

            };
            return model;
        }

        public JobEstimateServiceViewModel CreatePairingModel(JobEstimateServices domain)
        {
            var createdBy = domain.DataRecorderMetaData != null ?
               _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value)
               : null;
            var model = new JobEstimateServiceViewModel
            {
                Id = domain.Id,
                SurfaceType = domain.SurfaceType,
                FinishMaterial = domain.FinishMaterial,
                SurfaceColor = domain.SurfaceColor,
                //ImageUrl = domain.File != null ? (domain.File.RelativeLocation + "\\" + domain.File.Name).ToUrl() : null,
                SurfaceMaterial = domain.SurfaceMaterial,
                CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                EmailId = createdBy.Email,
                UploadDateTime = _clock.ToLocal(domain.DataRecorderMetaData.DateCreated),
                //TypeId = domain.TypeId,
                //FileId = domain.File != null ? domain.File.Id : default(long),
                ServiceTypeId = domain.ServiceTypeId,
                BuildingLocation = domain.BuildingLocation,
                CategoryId = domain.CategoryId,

                //Text = domain.TypeId != null ? (domain.TypeId == (long?)BeforeAfterImagesType.Before) ? "Before" : (domain.TypeId == (long?)BeforeAfterImagesType.During)
                //                  ? "During" : (domain.TypeId == (long?)BeforeAfterImagesType.After) ? "After" : "" : ""

            };
            return model;
        }
        public JobEstimateServiceViewModel CreateImageModel(JobEstimateImage domain)
        {
            var model = new JobEstimateServiceViewModel
            {
                Id = domain.Id,
                TypeId = domain.TypeId,
                FileId = domain.File != null ? domain.File.Id : default(long),
                Text = domain.TypeId != null ? (domain.TypeId == (long?)BeforeAfterImagesType.Before) ? "Before" : (domain.TypeId == (long?)BeforeAfterImagesType.During)
                                  ? "During" : (domain.TypeId == (long?)BeforeAfterImagesType.After) ? "After" : "" : ""

            };
            return model;
        }

        public JobSchedulerEditModel CreateJobShedulerModel(JobScheduler domain, List<EstimateInvoiceAssignee> estimateInvoiceAssignees)
        {
            var invoiceNames = estimateInvoiceAssignees.Where(x => x.SchedulerId == domain.Id).Select(x => x.Label).ToList();
            var invoiceIds = estimateInvoiceAssignees.Where(x => x.SchedulerId == domain.Id).Select(x => x.InvoiceNumber.Value).ToList();
            var invoiceNumbers = estimateInvoiceAssignees.Where(x => x.SchedulerId == domain.Id).Select(x => new InvoiceNumbersEditModel()
            {
                Id = x.InvoiceNumber.Value,
                Label = x.Label
            }).ToList();
            var model = new JobSchedulerEditModel
            {
                Id = domain.Id,
                JobId = domain.JobId != null ? domain.JobId.Value : 0,
                EstimateId = domain.EstimateId != null ? domain.EstimateId.Value : 0,
                Title = domain.Title,
                DataRecorderMetaData = domain.DataRecorderMetaData,
                AssigneeId = domain.AssigneeId != null ? domain.AssigneeId.Value : 0,
                AssigneeName = domain.Person != null ? domain.Person.Name.ToString() : null,
                IsActive = domain.IsActive,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate,
                FranchiseeId = domain.FranchiseeId,
                IsImported = domain.IsImported,
                SalesRepId = domain.SalesRepId,
                PersonId = domain.PersonId,
                ActualEndDateString = domain.EndDateTimeString,
                ActualStartDateString = domain.StartDateTimeString,
                SchedulerStatus = domain.SchedulerStatus,
                InvoiceNames = String.Join(", ", invoiceNames),
                InvoiceNumbers = invoiceIds,
                InvoiceNumber = invoiceNumbers
            };
            return model;
        }

        public JobEstimateEditModel CreateEstimateModel(JobEstimate domain, JobScheduler scheduler = default(JobScheduler))
        {
            var estimateNote = domain != null ? domain.JobNote.OrderByDescending(x => x.Id) : null;
            var createdBy = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(scheduler.DataRecorderMetaData.CreatedBy.Value);
            var franchisee = _organizationRepository.Get(scheduler.FranchiseeId);
            var isInvoicePresent = _estimateInvoiceRepository.Table.Any(x => x.SchedulerId == scheduler.Id);
            var estimateInvoice = _estimateInvoiceRepository.Table.Where(x => x.EstimateId == domain.Id).FirstOrDefault();
            var model = new JobEstimateEditModel
            {
                Id = domain != null ? domain.Id : scheduler.EstimateId.GetValueOrDefault(),
                SchedulerId = scheduler.Id,
                Hours = domain != null ? domain.EstimateHour : scheduler.Estimate.EstimateHour,
                Amount = domain != null ? domain.Amount : scheduler.Estimate.Amount,
                Franchisee = scheduler.Franchisee != null ? scheduler.Franchisee.Organization.Name : franchisee.Name,
                CustomerId = domain != null ? domain.CustomerId : scheduler.Estimate.JobCustomer.Id,
                Description = domain != null ? domain.Description : "",
                FranchiseeId = scheduler.FranchiseeId,
                SalesRepId = scheduler.SalesRepId != null ? scheduler.SalesRepId : null,
                SalesRep = scheduler.SalesRepId != null ? scheduler.SalesRep.Person.Name.ToString() : null,
                JobCustomer = domain != null ? CreateCustomerModel(domain.JobCustomer) : CreateCustomerModel(scheduler.Estimate.JobCustomer),
                StartDate = (scheduler.StartDate),
                EndDate = (scheduler.EndDate),
                Title = scheduler.Title,
                CreatedBy = createdBy.Email,
                IsImported = scheduler.IsImported,
                DataRecorderMetaData = scheduler.DataRecorderMetaData,
                Notes = estimateNote != null ? estimateNote.Select(x => CreateNoteViewModel(x)) : null,
                JobList = domain != null && domain.Jobs.Count() > 0 ? domain.Jobs.Select(x => CreateEditModel(x, default(JobScheduler))) : null,
                JobTypeId = domain != null ? domain.TypeId : null,
                GeoCode = domain != null ? domain.GeoCode : null,
                JobType = domain != null && domain.MarketingClass != null ? domain.MarketingClass.Name : null,
                ActualEndDateString = scheduler.EndDateTimeString,
                ActualStartDateString = scheduler.StartDateTimeString,
                SchedulerStatus = scheduler.SchedulerStatus,
                SchedulerStatusName = scheduler.Lookup.Name == "Confirmed" ? "Estimate " + scheduler.Lookup.Name : scheduler.Lookup.Name,
                SchedulerStatusColor = GetColorCode(scheduler.SchedulerStatus),
                IsActive = scheduler.SchedulerStatus == 218 ? true : false,
                IsInvoicePresent = isInvoicePresent,
                EstimatedAmount = estimateInvoice != null ? estimateInvoice.PriceOfService : 0,
                CreatedOn = scheduler.DataRecorderMetaData.DateCreated
            };
            return model;
        }

        public JobEstimate CreateDomain(JobEstimateEditModel model)
        {
            var domain = new JobEstimate
            {
                Id = model.Id,
                EstimateHour = model.Hours,
                CustomerId = model.CustomerId,
                Amount = model.Amount,
                Description = model.Description,
                TypeId = model.JobTypeId,
                GeoCode = model.GeoCode,
                IsNew = model.Id <= 0,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                EndDateTimeString = model.ActualEndDateString,
                StartDateTimeString = model.ActualStartDateString

            };
            return domain;
        }

        public JobNote CreateDomain(SchedulerNoteModel model)
        {
            var domain = new JobNote
            {
                //MeetingId = model.MeetingId != null ? model.MeetingId : null,
                EstimateId = model.EstimateId != null ? model.EstimateId : null,
                JobId = model.JobId != null ? model.JobId : null,
                VacationId = model.VacationId != null ? model.VacationId : null,
                Note = model.Note,
                IsNew = true,
                StatusId = model.StatusId != null ? model.StatusId : null,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData.Id,
            };
            return domain;
        }

        private SchedulerNoteModel CreateNoteViewModel(JobNote Domain)
        {
            var createdBy = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(Domain.DataRecorderMetaData.CreatedBy.Value);
            var model = new SchedulerNoteModel
            {
                Note = Domain.Note,
                JobId = Domain.JobId,
                EstimateId = Domain.EstimateId,
                StatusId = Domain.StatusId,
                CreatedOn = Domain.DataRecorderMetaData.DateModified != null ? Domain.DataRecorderMetaData.DateModified.GetValueOrDefault() : Domain.DataRecorderMetaData.DateCreated,
                CreatedBy = createdBy.Email,
                Status = Domain.JobStatus != null ? Domain.JobStatus.Name : null,
                Id = Domain.Id
            };
            return model;
        }
        public JobViewModel CreateViewModel(Holiday domain, Franchisee franchisee)
        {
            string colorCode = "#FF0000";
            if (domain.Description == "convention")
            {
                colorCode = "#26327e";
            }
            var createdBy = domain.DataRecorderMetaData != null ?
                        _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value)
                        : null;
            var model = new JobViewModel
            {
                Id = domain.Id,
                JobTitle = domain.Title,
                title = domain.Title,
                start = domain.StartDate,
                end = domain.EndDate,
                ActualStartDate = domain.StartDate,
                ActualEndDate = domain.EndDate,
                Franchisee = franchisee.Organization.Name,
                SchedulerTitle = domain.Title,
                backgroundColor = colorCode,
                color = colorCode,
                durationEditable = false,
                CreatedBy = createdBy != null ? createdBy.Email : null,
                Imported = false,
                CanSchedule = domain.canSchedule
            };

            if ((model.end - model.start).TotalDays > 1 || (model.start.ToString("HH:mm") == "08:00" && model.end.ToString("HH:mm") == "17:00"))
            {
                model.start = model.start.Date;
                model.end = model.end.Date.AddDays(1);
            }
            var daystart = model.start.ToString("HH:mm");
            var dayEnd = model.end.ToString("HH:mm");
            model.allDay = true;
            model.IsHoliday = true;
            return model;
        }

        public JobEstimateEditModel CreateVacationModel(JobScheduler domain)
        {
            var vacationNotes = domain.VacationNote.OrderByDescending(x => x.Id);
            var createdBy = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value);
            var model = new JobEstimateEditModel
            {
                Id = domain.Id,
                FranchiseeId = domain.FranchiseeId,
                Franchisee = domain.Franchisee.Organization.Name,
                Title = domain.Title,
                StartDate = (domain.StartDate),
                EndDate = (domain.EndDate),
                IsImported = domain.IsImported,
                CreatedBy = createdBy.Email,
                AssigneeId = domain.AssigneeId.Value,
                IsVacation = domain.IsVacation,
                UserId = domain.OrganizationRoleUser.UserId,
                Assignee = domain.Person != null ? domain.Person.Name.ToString() : null,
                DataRecorderMetaData = domain.DataRecorderMetaData,
                Notes = vacationNotes != null ? vacationNotes.Select(x => CreateNoteViewModel(x)) : null,
                ActualEndDateString = domain.EndDateTimeString,
                ActualStartDateString = domain.StartDateTimeString
            };
            return model;
        }

        public MeetingEditModel CreateMeetingModel(JobScheduler domain)
        {
            var meeting = _meetingRepository.Table.FirstOrDefault(x => x.Id == domain.MeetingID);
            var userLoginList = _userLogimRepository.Table.Select(x => x.Id).ToList();
            var jobScheduler = _jobSchedulerRepository.Table.Where(x => x.MeetingID == domain.MeetingID && x.FranchiseeId == domain.FranchiseeId && x.IsActive && x.StartDate == domain.StartDate && x.EndDate == domain.EndDate).Select(x => x).Distinct().ToList();
            string assigneeName = "";
            var vacationNotes = domain.VacationNote.OrderByDescending(x => x.Id);
            var createdBy = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value);
            var userIds = jobScheduler.Where(x => x.OrganizationRoleUser != null && x.IsActive).Select(x => x.OrganizationRoleUser.UserId).Distinct().ToList();
            var personIds = jobScheduler.Select(x => x.Person).Distinct().ToList();
            if (personIds.Count() > 0)
            {
                if (personIds.Any(x => x.UserLogin != null))
                {
                    personIds = personIds.Where(x => x.UserLogin.IsLocked == false).ToList();
                    meeting.IsEquipment = false;
                }
                else
                {
                    var meetingPersonIds = personIds.Select(x => x.Id).ToList();
                    var equipmentPersonIds = _equipmentUserDetailsRepository.Table.Where(x => meetingPersonIds.Contains(x.UserId) && !x.IsLock).Select(x => x.UserId).ToList();
                    personIds = personIds.Where(x => equipmentPersonIds.Contains(x.Id)).ToList();
                    meeting.IsEquipment = true;
                }

            }
            foreach (var user in personIds)
            {
                if (user != null)
                    assigneeName += (user.FirstName + " " + user.MiddleName + " " + user.LastName) + ",";
                else
                    assigneeName = null;
            }
            int idx = assigneeName.LastIndexOf(',');
            assigneeName = idx > 0 ? assigneeName.Substring(0, idx) : assigneeName;


            var isEquipment = meeting.IsEquipment;
            var model = new MeetingEditModel
            {
                Id = domain.Id,
                FranchiseeId = domain.FranchiseeId,
                Franchisee = domain.Franchisee.Organization.Name,
                Title = domain.Title,
                StartDate = (domain.StartDate),
                EndDate = (domain.EndDate),
                IsImported = domain.IsImported,
                CreatedBy = createdBy.Email,
                AssigneeId = domain.AssigneeId.Value,
                //IsVacation = domain.IsVacation,
                //UserId = domain.OrganizationRoleUser.UserId,
                Assignee = assigneeName,
                DataRecorderMetaData = domain.DataRecorderMetaData,
                Notes = vacationNotes != null ? vacationNotes.Select(x => CreateNoteViewModel(x)) : null,
                JobAssigneeIds = userIds,
                MeetingId = domain.MeetingID,
                ActualStartDateString = domain.StartDateTimeString,
                ActualEndDateString = domain.EndDateTimeString,
                IsEquipment = isEquipment,
                IsUser = !isEquipment
            };
            return model;
        }

        public JobScheduler CreateSchedulerDomain(JobEstimateEditModel model)
        {
            var domain = new JobScheduler
            {
                Id = model.Id,
                AssigneeId = model.AssigneeId,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                FranchiseeId = model.FranchiseeId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsActive = true,
                IsVacation = true,
                IsNew = model.Id <= 0,
                MeetingID = model.MeetingID,
                PersonId = model.AssigneeId,
                StartDateTimeString = model.ActualStartDateString,
                EndDateTimeString = model.ActualEndDateString,
                SchedulerStatus = (long)ConfirmationEnum.NotResponded
            };
            return domain;
        }
        public JobScheduler CreateMeetingDomain(JobEstimateEditModel model)
        {
            var domain = new JobScheduler
            {
                Id = model.Id,
                AssigneeId = model.AssigneeId,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                FranchiseeId = model.FranchiseeId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsActive = true,
                IsVacation = false,
                IsNew = model.IsUpdate == true ? false : true,
                MeetingID = model.MeetingID,
                PersonId = model.PersonId,
                StartDateTimeString = model.ActualStartDateString,
                EndDateTimeString = model.ActualEndDateString,
                SchedulerStatus = (long)ConfirmationEnum.NotResponded
            };
            return domain;
        }
        public JobOccurenceEditModel CreateEditModel(JobScheduler domain)
        {
            var invoiceAssignee = _estimateInvoiceAssigneeRepository.Table.Where(x => x.AssigneeId == domain.AssigneeId.Value && x.SchedulerId == domain.Id).ToList();
            var model = new JobOccurenceEditModel
            {
                AssigneeId = domain.AssigneeId.Value,
                EndDate = (domain.EndDate),
                StartDate = (domain.StartDate),
                ParentJobId = domain.JobId != null ? domain.JobId.Value : 0,
                ParentEstimateId = domain.EstimateId != null ? domain.EstimateId.Value : 0,
                ScheduleId = domain.Id,
                ActualEndDateString = domain.EndDateTimeString,
                ActualStartDateString = domain.StartDateTimeString,
                InvoiceNumber = invoiceAssignee.Select(x => new InvoiceNumbersEditModel()
                {
                    Label = x.Label != null ? x.Label : "",
                    Id = x.InvoiceNumber.Value
                }).ToList()
            };
            return model;
        }

        public JobScheduler CreateDomain(JobOccurenceEditModel model)
        {
            var offset = _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes;
            var domain = new JobScheduler()
            {
                Id = model.ScheduleId,
                AssigneeId = model.AssigneeId,
                DataRecorderMetaData = model.DataRecorderMetaData != null ? model.DataRecorderMetaData : new Application.Domain.DataRecorderMetaData(),
                DataRecorderMetaDataId = model.DataRecorderMetaData != null ? model.DataRecorderMetaData.Id : 0,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                FranchiseeId = model.FranchiseeId,
                IsActive = true,
                IsNew = model.ScheduleId <= 0,
                JobId = model.ParentJobId,
                Title = model.Title,
                ServiceTypeId = model.ServiceTypeId,
                Offset = offset,
                StartDateTimeString = model.ActualStartDateString,
                EndDateTimeString = model.ActualEndDateString,
                SchedulerStatus = (long)ConfirmationEnum.NotResponded
            };
            return domain;
        }

        public JobScheduler CreateDomainForEstimate(JobOccurenceEditModel model)
        {
            var offset = _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes;
            var domain = new JobScheduler()
            {
                Id = model.ScheduleId,
                AssigneeId = model.AssigneeId,
                DataRecorderMetaData = model.DataRecorderMetaData != null ? model.DataRecorderMetaData : new Application.Domain.DataRecorderMetaData(),
                DataRecorderMetaDataId = model.DataRecorderMetaData != null ? model.DataRecorderMetaData.Id : 0,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                FranchiseeId = model.FranchiseeId,
                IsActive = true,
                IsNew = model.ScheduleId <= 0,
                JobId = model.ParentJobId,
                Title = model.Title,
                ServiceTypeId = model.ServiceTypeId,
                Offset = offset,
                SalesRepId = model.AssigneeId,
                EndDateTimeString = model.ActualEndDateString,
                StartDateTimeString = model.ActualStartDateString,
                SchedulerStatus = (long)ConfirmationEnum.NotResponded
            };
            return domain;
        }
        public JobSchedulerEditModel CreateModel(JobEditModel editModel)
        {
            var model = new JobSchedulerEditModel
            {
                FranchiseeId = editModel.FranchiseeId,
                SalesRepId = editModel.SalesRepId,
                JobId = editModel.JobId,
                Title = editModel.Title,
                ServiceTypeId = editModel.JobTypeId,
                IsImported = editModel.IsImported
            };
            return model;
        }
        public JobScheduler CreateDomain(VacationRepeatEditModel model)
        {
            var domain = new JobScheduler
            {
                Id = model.Id,
                IsActive = true,
                IsVacation = true,
                AssigneeId = model.AssigneeId,
                FranchiseeId = model.FranchiseeId,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData.Id,
                IsNew = model.Id <= 0,
                MeetingID = model.MeetingId,
                PersonId = model.PersonId,
                StartDateTimeString = model.ActualStartDate,
                EndDateTimeString = model.ActualEndDate,
                SchedulerStatus = (long)ConfirmationEnum.NotResponded
            };
            return domain;
        }
        public JobScheduler CreateRepearMeetingDomain(VacationRepeatEditModel model)
        {
            var domain = new JobScheduler
            {
                Id = model.Id,
                IsActive = true,
                IsVacation = false,
                AssigneeId = model.AssigneeId,
                FranchiseeId = model.FranchiseeId,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData.Id,
                IsNew = model.Id <= 0,
                MeetingID = model.MeetingId,
                PersonId = model.AssigneeId,
                EndDateTimeString = model.ActualEndDate,
                StartDateTimeString = model.ActualStartDate,
                SchedulerStatus = (long)ConfirmationEnum.NotResponded
            };
            return domain;
        }
        public Meeting CreatMeetingModel(JobEstimateEditModel model)
        {
            if (model.ParentID == 0)
            {
                model.ParentID = null;
            }
            var domain = new Meeting
            {
                Id = model.Id,
                IsNew = model.Id <= 0,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ParentId = model.ParentID,
                Title = model.Title,
                StartDateTimeString = model.ActualStartDateString,
                EndDateTimeString = model.ActualEndDateString,
                Offset = Double.Parse(_clock.BrowserTimeZone),
                IsEquipment = model.IsEquipment.GetValueOrDefault(),

            };
            return domain;
        }
        public Meeting CreatMeetingModel(VacationRepeatEditModel model)
        {

            var domain = new Meeting
            {
                Id = model.Id,
                IsNew = model.Id <= 0,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ParentId = model.ParentId,
                Title = model.Title,
                Offset = Double.Parse(_clock.BrowserTimeZone)
            };
            return domain;
        }
        public JobScheduler CreateMeetingDomainForDeleting(JobEstimateEditModel model)
        {
            var domain = new JobScheduler
            {
                Id = model.Id,
                AssigneeId = model.AssigneeId,
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                FranchiseeId = model.FranchiseeId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsActive = false,
                IsVacation = false,
                MeetingID = model.MeetingID
            };
            return domain;
        }
        public EmailViewModel CreateMeetingDomainForMail(EmailTemplate emailTemplate, List<EmailTemplate> templateList)
        {
            var emailTemplateInSpanish = templateList
        .FirstOrDefault(x =>
            x.NotificationTypeId == emailTemplate.NotificationTypeId &&
            x.LanguageId == (long)LanguageEnum.Spanish);

            //var model = modelList
            //    .FirstOrDefault(x => x == emailTemplate.NotificationTypeId);

            return new EmailViewModel
            {
                EmailTemplateId = emailTemplate.Id,
                NotificationId = emailTemplate.NotificationTypeId,

                Subject = emailTemplate.Title,
                Subjects = emailTemplate.Subject,

                Body = emailTemplate.Body,
                EnglishBody = emailTemplate.Body,
                SpanishBody = emailTemplateInSpanish != null
                    ? emailTemplateInSpanish.Body
                    : "",

                isActive = emailTemplate.isActive,
                IsSpanishPossible = emailTemplateInSpanish != null,

                IsTransPossible = !(
                    emailTemplate.Id == (long)EmailEnum.ARReport ||
                    emailTemplate.Id == (long)EmailEnum.Feedback ||
                    emailTemplate.Id == (long)EmailEnum.LocalSiteImageGallery ||
                    emailTemplate.Id == (long)EmailEnum.MonthlyCustomerList ||
                    emailTemplate.Id == (long)EmailEnum.MonthlyMailchimpReport ||
                    emailTemplate.Id == (long)EmailEnum.MonthlyReviewSystemNotification ||
                    emailTemplate.Id == (long)EmailEnum.ServiceDeposit ||
                    emailTemplate.Id == (long)EmailEnum.WeeklyLateFeeNotification ||
                    emailTemplate.Id == (long)EmailEnum.WeeklyUnpaidInvoiceNotification
                ),

                //IsPreviewAvailable = model != default(long)
                IsPreviewAvailable = false
            };
        }

        public JobEstimate CreateDomainOccurance(JobOccurenceEditModel model, JobEstimate jobEstimate)
        {
            var offset = _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes;
            var domain = new JobEstimate()
            {
                Id = 0,
                IsNew = true,
                CustomerId = jobEstimate.CustomerId,
                Description = jobEstimate.Description,
                Amount = jobEstimate.Amount,
                GeoCode = jobEstimate.GeoCode,
                EstimateHour = jobEstimate.EstimateHour,
                TypeId = jobEstimate.TypeId,
                ParentEstimateId = model.ParentJobId,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            return domain;
        }

        public JobEstimateImageCategory CreateJobEstimateCategory(JobEstimateCategoryViewModel model)
        {
            var domain = new JobEstimateImageCategory()
            {
                Id = model.Id != null ? model.Id.GetValueOrDefault() : 0,
                JobId = model.JobId,
                EstimateId = model.EstimateId,
                MarkertingClassId = model.MarketingClassId,
                IsNew = model.Id <= 0 ? true : false,
                SchedulerId = model.SchedulerId
            };
            return domain;
        }
        public JobEstimateServices CreateJobEstimatePairing(JobEstimateServiceViewModel model, long categoryId, long? typeId)
        {
            var domain = new JobEstimateServices()
            {
                Id = (model == null || model.Id == 0 || model.Id == null) ? 0 : model.Id.GetValueOrDefault(),
                CategoryId = (model != null && model.Id != 0) ? model.CategoryId : categoryId,
                //FileId = model.FileId != default(long) ? model.FileId : null,
                FinishMaterial = model != null && model.FinishMaterial != null ? model.FinishMaterial : null,
                SurfaceColor = model != null && model.SurfaceColor != null ? model.SurfaceColor : null,
                SurfaceMaterial = model != null && model.SurfaceMaterial != null ? model.SurfaceMaterial : null,
                SurfaceType = model != null && model.SurfaceType != null ? model.SurfaceType : null,
                DataRecorderMetaData = model != null && model.DataRecorderMetaData != null ? model.DataRecorderMetaData : new DataRecorderMetaData(),
                IsNew = (model == null || model.Id == 0 || model.Id == null) ? true : false,
                //TypeId = model.TypeId,
                DataRecorderMetaDataId = model != null && model.DataRecorderMetaData != null ? (long)model.DataRecorderMetaData.Id : default(long),
                ServiceTypeId = model != null && model.ServiceTypeId != null ? model.ServiceTypeId : null,
                BuildingLocation = model != null && model.BuildingLocation != null ? model.BuildingLocation : null,
                TypeId = typeId,
                IsBeforeImage = model != null ? model.IsBeforeImage : false,
                CompanyName = model != null && model.CompamyName != null ? model.CompamyName : "",
                MAIDJANITORIAL = model != null && model.MaidJanitorial != null ? model.MaidJanitorial : "",
                MaidService = model != null && model.MaidService != null ? model.MaidService : "",
                PropertyManager = model != null && model.PropertyManager != null ? model.PropertyManager : "",
                FloorNumber = model != null && model.FloorNumber != null ? model.FloorNumber : 0,
            };
            return domain;
        }

        public JobEstimateImageCategory CreateJobEstimateCategory(JobEstimateImageCategory domain, JobEstimateCategoryViewModel model)
        {
            var Editdomain = new JobEstimateImageCategory()
            {
                Id = domain.Id,
                JobId = domain.JobId,
                EstimateId = domain.EstimateId,
                MarkertingClassId = domain.MarkertingClassId,
                IsNew = false,

            };
            return Editdomain;
        }

        public JobEstimateServices CreateJobEstimatePairing(JobEstimateServiceViewModel model, long categoryId, JobEstimateServices domain)
        {
            var editDomain = new JobEstimateServices()
            {
                Id = domain.Id,
                CategoryId = categoryId,
                //FileId = model.FileId != default(long) ? model.FileId : null,
                FinishMaterial = model.FinishMaterial,
                SurfaceColor = model.SurfaceColor,
                SurfaceMaterial = model.SurfaceMaterial,
                SurfaceType = model.SurfaceType,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsNew = false,
                //TypeId = model.TypeId,
                //DataRecorderMetaDataId = domain.DataRecorderMetaData != null ? domain.DataRecorderMetaData.Id : 0,
                ServiceTypeId = model.ServiceTypeId,
                BuildingLocation = model.BuildingLocation

            };
            editDomain.DataRecorderMetaData.DateModified = DateTime.Now;
            return editDomain;
        }


        public JobEstimateImage CreateJobEstimateImageModel(JobEstimateServiceViewModel model, long serviceId, long? typeId)
        {
            var domain = new JobEstimateImage()
            {
                Id = model.Id != null ? model.Id.GetValueOrDefault() : 0,
                FileId = model.FileId,
                //FileId = model.FileId != default(long) ? model.FileId : null,
                ServiceId = serviceId,
                TypeId = typeId,
                IsNew = model.Id > 0 ? false : true
            };
            return domain;
        }
        public FileModel CreateFileModel(JobEstimateImage domain)
        {
            //var createdBy = domain.DataRecorderMetaData != null ?
            //   _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value)
            //   : null;
            var model = new FileModel
            {
                Id = domain.Id,
                RelativeLocation = domain.File != null ? (domain.File.RelativeLocation + "\\" + domain.File.Name).ToUrl() : null,

            };
            return model;
        }
        public JobEstimateImage CreateJobEstimateImageDomain(JobEstimateServiceViewModel model, long categoryId, long? typeId, long? fileId)
        {
            var domain = new JobEstimateImage()
            {
                Id = model.Id != null ? model.Id.GetValueOrDefault() : 0,
                FileId = fileId,
                IsNew = model.Id > 0 ? false : true,
                ServiceId = categoryId,
                TypeId = typeId,
                DataRecorderMetaData = model.DataRecorderMetaData,
            };
            return domain;
        }
        public JobEstimateServiceViewModel CreateServiceViewModel(JobEstimateServices domain, IEnumerable<FileModel> files, bool? isFromBefore, long? userId)
        {
            var userRole = "";
            var imageUploadedByRole = new JobEstimateImage();
            var roles = new List<string>();
            var createdBy = (domain != null && domain.DataRecorderMetaData != null) ?
               _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value)
               : null;
            if (domain != null)
                imageUploadedByRole = _jobEstimateImageRepository.IncludeMultiple(x => x.File.DataRecorderMetaData).Where(x => x.ServiceId == domain.Id).FirstOrDefault();

            var createdByRole = (domain != null && domain.DataRecorderMetaData != null && imageUploadedByRole != new JobEstimateImage() && imageUploadedByRole != null) ?
              _organizationRoleUserInfoService.GetUserIdFromOrganizationRoleUserId(imageUploadedByRole.File.DataRecorderMetaData.CreatedBy.Value)
              : null;

            if (createdByRole != null)
                roles = _organizationRoleUserRepository.Table.Where(x => x.UserId == createdByRole.UserId && x.Role != null).Select(x => x.Role.Name).Distinct().ToList();
            if (roles != null && roles.Count() >= 1)
            {
                foreach (var role in roles)
                    userRole += role + ",";
            }
            int idx = userRole != null && userRole != "" ? userRole.LastIndexOf(',') : 0;
            userRole = userRole != null && userRole != "" ? userRole.Substring(0, idx) : "";

            var beforeAfter = domain != null && domain.Id > 0 ? _beforeAfterImageRepository.Table.FirstOrDefault(x => x.ServiceId == domain.Id) : null;
            var model = new JobEstimateServiceViewModel
            {
                OriginalId = domain != null ? domain.Id : 0,
                Id = domain != null ? domain.Id : 0,
                BuildingLocation = domain != null ? domain.BuildingLocation : null,
                CategoryId = domain != null ? domain.CategoryId : null,
                PairId = domain != null ? domain.PairId : null,
                ServiceTypeId = domain != null ? domain.ServiceTypeId : null,
                SurfaceMaterial = domain != null ? domain.SurfaceMaterial : null,
                SurfaceType = domain != null ? domain.SurfaceType : null,
                FinishMaterial = domain != null ? domain.FinishMaterial : null,
                SurfaceColor = domain != null ? domain.SurfaceColor : null,
                EmailId = createdBy != null ? createdBy.Email : null,
                CreatedBy = createdBy != null ? createdBy.FirstName + " " + createdBy.LastName : null,
                DataRecorderMetaData = domain != null ? domain.DataRecorderMetaData : null,
                UploadDateTime = (domain != null && domain.DataRecorderMetaData != null) ? domain.DataRecorderMetaData.DateModified == null ? domain.DataRecorderMetaData.DateCreated : domain.DataRecorderMetaData.DateModified : default(DateTime),
                ImagesInfo = files.ToList(),
                TypeId = domain != null ? domain.TypeId : null,
                IsGroutilife = domain != null ? (domain.ServiceTypeId == (long?)ServiceTypes.Groutelife || domain.ServiceTypeId == (long?)ServiceTypes.CONCRETECOUNTERTOPS || domain.ServiceTypeId == (long?)ServiceTypes.CONCRETECOATINGS || domain.ServiceTypeId == (long?)ServiceTypes.CONCRETEOVERLAYMENTS) ? true : false : false,
                isDisable = domain != null ? isFromBefore.GetValueOrDefault() ? !domain.IsBeforeImage.GetValueOrDefault() : domain.IsBeforeImage.GetValueOrDefault() : false,
                IsBeforeImage = domain != null && domain.IsBeforeImage != null ? domain.IsBeforeImage.GetValueOrDefault() : true,
                CompamyName = domain != null ? domain.CompanyName : "",
                MaidJanitorial = domain != null ? domain.MAIDJANITORIAL : "",
                PropertyManager = domain != null ? domain.PropertyManager : "",
                MaidService = domain != null ? domain.MaidService : "",
                Designation = createdByRole != null ? userRole : "",
                FloorNumber = domain != null ? domain.FloorNumber : 0,
                IsFromEstimate = domain != null && domain.JobEstimateImageCategory.EstimateId != null && domain.JobEstimateImageCategory.JobId == null ? true : false,
                Css = files.Count() > 0 ? files.FirstOrDefault().css : "rotate(0)",
                ImageSavedByUser = (domain != null && domain.DataRecorderMetaData != null) ? domain.DataRecorderMetaData.CreatedBy == userId ? true : false : false,
                IsBestPairMarkedImage = beforeAfter != null && beforeAfter.IsBestImage == true ? true : false,
                IsAddTpLocalGalleryImage = beforeAfter != null && beforeAfter.IsAddToLocalGallery == true ? true : false
            };
            return model;
        }


        public FileModel CreateServiceFileViewModel(Application.Domain.File domain, Application.Domain.File thumbNailDomain, long? userId, List<BeforeAfterImages> beforeAfterImages = null)
        {
            var jobestimateimage = _jobEstimateImageRepository.Table.Where(x => x.FileId == domain.Id).FirstOrDefault();
            var jobestimateimageBeforeAfter = _fileRepository.Table.Where(x => x.Id == domain.Id).FirstOrDefault();
            var jobestimateService = jobestimateimage != null ? _jobEstimateServicesRepository.Table.Where(x => x.Id == jobestimateimage.ServiceId).FirstOrDefault() : null;
            var jobEstimateImageCategory = jobestimateService != null ? _jobEstimateImageCategoryRepository.Table.Where(x => x.Id == jobestimateService.CategoryId).FirstOrDefault() : null;
            var estimateInvoice = jobEstimateImageCategory != null ? _estimateInvoiceRepository.Table.Where(x => x.EstimateId == jobEstimateImageCategory.EstimateId).FirstOrDefault() : null;

            var metadataid = jobestimateimage != null && jobestimateimage.DataRecorderMetaDataId != null ? jobestimateimage.DataRecorderMetaDataId : default(long);

            var datameta = metadataid != default(long) ? _dataRecorderMetaDataRepository.Get((long)metadataid) : null;
            var userId1 = datameta != null ? _organizationRoleUserRepository.Get(datameta.CreatedBy.GetValueOrDefault()) : default(OrganizationRoleUser);
            var extension = Path.GetExtension(domain.Name);
            var isIFrame = (extension == ".pdf" || extension == ".docx" || extension == ".xlsx") ? true : false;
            var datametaForimages = (jobestimateimageBeforeAfter != null && jobestimateimageBeforeAfter.DataRecorderMetaDataId != default(long)) ? _dataRecorderMetaDataRepository.Get(jobestimateimageBeforeAfter.DataRecorderMetaDataId) : null;
            var userIdForDatametaForImages = datametaForimages != null ? _organizationRoleUserRepository.Get(datametaForimages.CreatedBy.GetValueOrDefault()) : default(OrganizationRoleUser);
            var thumbFileId = beforeAfterImages != null && beforeAfterImages.Count() != 0 ? (beforeAfterImages.FirstOrDefault().CroppedImageThumbId != null ? beforeAfterImages.FirstOrDefault().CroppedImageThumbId : default(long)) : default(long);
            var cropedImageThumb = _fileRepository.Table.Where(x => x.Id == thumbFileId).FirstOrDefault();
            var croppedImageId = beforeAfterImages != null && beforeAfterImages.Count() != 0 ? (beforeAfterImages.FirstOrDefault().CroppedImageId != null ? beforeAfterImages.FirstOrDefault().CroppedImageId : default(long)) : default(long);
            var croppedImage = _fileRepository.Table.Where(x => x.Id == croppedImageId).FirstOrDefault();
            var model = new FileModel
            {
                OriginalId = domain.Id,
                ImageUrl = (domain.RelativeLocation + "\\" + domain.Name).ToUrl(),
                Id = domain.Id,
                Name = domain.Name,
                DataRecorderMetaData = datameta,
                CreatedBy = (datameta != null && userId1 != null) ? userId1.Person.FirstName + " " + userId1.Person.LastName : "",
                CreatedOn = (datameta != null && userId1 != null) ? datameta.DateModified != null ? _clock.ToLocal(datameta.DateModified.GetValueOrDefault()) : _clock.ToLocal(datameta.DateCreated) : default(DateTime),
                CreatedByForImage = (datametaForimages != null && userIdForDatametaForImages != null) ? userIdForDatametaForImages.Person.FirstName + " " + userIdForDatametaForImages.Person.LastName : "",
                CreatedOnForImage = (datametaForimages != null && userIdForDatametaForImages != null) ? datametaForimages.DateModified != null ? _clock.ToLocal(datametaForimages.DateModified.GetValueOrDefault()) : _clock.ToLocal(datametaForimages.DateCreated) : default(DateTime),
                UserId = userId1 != null && userId1.Person != null ? userId1.Person.Id : default(long),
                IsIFrame = isIFrame,
                IFrameUrl = domain != null ? _settings.SiteRootUrl + "/Media//" + domain.Name : null,
                Size = domain != null ? (int)domain.Size / 1000 : 0,
                FileId = domain.Id,
                UploadByRoleId = (datameta != null && userId1 != null) ? userId1.RoleId : default(long),
                css = thumbNailDomain == null ? domain.css : thumbNailDomain.css,
                Caption = domain.Caption,
                ThumbImageUrl = thumbNailDomain != null ? (thumbNailDomain.RelativeLocation + "\\" + thumbNailDomain.Name).ToUrl() : "",
                ImageSavedByUser = (jobestimateimage != null && jobestimateimage.DataRecorderMetaData != null) ? jobestimateimage.DataRecorderMetaData.CreatedBy == userId ? true : false : false,
                S3BucketImageUrl = beforeAfterImages != null && beforeAfterImages.Any() ? beforeAfterImages.FirstOrDefault().S3BucketURL : "",
                S3BucketThumbImageUrl = beforeAfterImages != null && beforeAfterImages.Any() ? beforeAfterImages.FirstOrDefault().S3BucketThumbURL != null ? beforeAfterImages.FirstOrDefault().S3BucketThumbURL : beforeAfterImages.FirstOrDefault().S3BucketURL : "",
                BeforeAfterId = beforeAfterImages != null && beforeAfterImages.Any() ? beforeAfterImages.FirstOrDefault().Id : default(long),
                IsImageCropped = beforeAfterImages != null && beforeAfterImages.Any() && beforeAfterImages.FirstOrDefault().IsImageCropped == true ? true : false,
                CroppedImageThumb = cropedImageThumb != null ? (cropedImageThumb.RelativeLocation + "\\" + cropedImageThumb.Name).ToUrl() : "",
                CroppedImageUrl = croppedImage != null ? (croppedImage.RelativeLocation + "\\" + croppedImage.Name).ToUrl() : "",
                CroppedImageFileId = beforeAfterImages != null && beforeAfterImages.Any() && beforeAfterImages.FirstOrDefault().IsImageCropped ? beforeAfterImages.FirstOrDefault().CroppedImageId : default(long),
                CroppedImageThumbFileId = beforeAfterImages != null && beforeAfterImages.Any() && beforeAfterImages.FirstOrDefault().IsImageCropped ? beforeAfterImages.FirstOrDefault().CroppedImageThumbId : default(long)
            };
            return model;
        }
        public JobEstimateImage CreateJobEstimateImageEditDomain(JobEstimateServiceViewModel model, long categoryId, long? typeId, FileModel file)
        {

            //var fileDomain = _fileRepository.Get(file.FileId.GetValueOrDefault());
            var modelId = default(long);
            if (file != null && model.ImagesInfo.Count() > 0 && model.Id > 0)
            {
                var id = model.ImagesInfo[0].Id;
                var jobEstimateDomain = _jobEstimateImageRepository.Table.Where(x => id != default && x.ServiceId == id).FirstOrDefault();
                if (jobEstimateDomain != null)
                    modelId = jobEstimateDomain.Id;
            }
            if (modelId == 0 && model.Id > 0)
            {
                modelId = _jobEstimateImageRepository.Table.Where(x => x.ServiceId == model.OriginalId).Select(x => x.Id).FirstOrDefault();
            }
            var domain = new JobEstimateImage()
            {
                Id = modelId != 0 ? modelId : model.Id != null ? model.Id.GetValueOrDefault() : default(long),
                FileId = file.FileId != null ? file.FileId : file.Id,
                IsNew = file.Id > 0 ? false : true,
                ServiceId = categoryId,
                TypeId = typeId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData != null ? model.DataRecorderMetaData.Id : 0,
                ThumbFileId = file.ThumbFileId != null ? file.ThumbFileId : null
                //Base64ImageUrl = fileDomain != null ? GetBase64String(fileDomain) : ""
            };
            return domain;
        }

        public BeforeAfterImageMailAudit CreateBeforeAfterImageMailDomain(JobScheduler jobSchdeuler, long? notificationQueueId, long? fileId, long? franchiseeId,
                      BeforeAfterImageSendMailViewModel model = null)
        {
            var domain = new BeforeAfterImageMailAudit
            {
                BeforeAfterCategoryIdAfterImageId = model != null ? model.AfterImages.Id : null,
                BeforeAfterCategoryIdBeforeImageId = model != null ? model.BeforeImages.Id : null,
                SchedulerId = jobSchdeuler.Id,
                FranchiseeId = franchiseeId,
                IsNew = true,
                FileId = fileId,
                CreatedOn = DateTime.Now,
                NotificationQueueId = notificationQueueId,
                Id = 0
            };
            return domain;
        }

        public FranchiseInfoModel CreateViewModel(Address domain)
        {
            var model = new FranchiseInfoModel()
            {
                CountryId = domain.CountryId,
                CountryName = domain.Country != null ? domain.Country.Name : "",
                //StateId = domain.StateId,
                //State = domain.StateName
            };
            return model;
        }

        private string GetColorCode(long statusName)
        {
            switch (statusName)
            {
                case 216:
                    {
                        return "blue";
                    }
                case 217:
                    {
                        return "red";
                    }
                case 218:
                    {
                        return "green";
                    }
                default:
                    {
                        return "green";
                    }
            }

        }

        public CustomerInfoModel CreateViewModelForCustomer(Address domain)
        {
            var model = new CustomerInfoModel()
            {
                CountryId = domain.CountryId,
                CountryName = domain.Country != null ? domain.Country.Name : "",
                StateId = domain.StateId,
                State = domain.StateName != null ? domain.StateName : domain.State.Name,
                AddressLine1 = domain.AddressLine1,
                AddressLine2 = domain.AddressLine2,
                ZipCode = domain.Zip != null ? domain.Zip.Code : domain.ZipCode,
                CityId = domain.CityId,
                City = domain.CityName != null ? domain.CityName : domain.City.Name
            };
            return model;
        }

        public BeforeAfterViewModel CreateBeforeAfterViewModel(JobEstimateImage jobEstimareAfter, JobEstimateServices jobEstimateService,
            List<JobEstimateImage> jobEstimateImageList, List<MarkbeforeAfterImagesHistry> markbeforeAfterImagesHistryList, int index,
            List<OrganizationRoleUser> orgRoleUser)
        {
            if (jobEstimateService == null)
            {
                return default;
            }
            var modifiedDate = jobEstimareAfter.BestFitMarkDateTime;
            var personName = "";
            var beforeImage = jobEstimateImageList.FirstOrDefault(x => x.ServiceId == jobEstimateService.Id);
            var beforeMarkBeforeAfterImage = markbeforeAfterImagesHistryList.FirstOrDefault(x => x.ServiceId == jobEstimateService.Id);
            var franchiseeName = jobEstimateService.JobEstimateImageCategory!=null? jobEstimateService.JobEstimateImageCategory.JobScheduler.Franchisee.Organization.Name:"";
            if (beforeMarkBeforeAfterImage != null)
            {
                var orgRolUserDomain = orgRoleUser.FirstOrDefault(x => x.Id == beforeMarkBeforeAfterImage.DataRecorderMetaData.CreatedBy);
                personName = orgRolUserDomain != null ? orgRolUserDomain.Person.FirstName + " " + orgRolUserDomain.Person.LastName : "";
            }
            if (beforeMarkBeforeAfterImage == null)
            {
                beforeMarkBeforeAfterImage = markbeforeAfterImagesHistryList.FirstOrDefault(x => x.ServiceId == jobEstimareAfter.ServiceId);
                if (beforeMarkBeforeAfterImage != null)
                {
                    var orgRolUserDomain = orgRoleUser.FirstOrDefault(x => x.UserId == beforeMarkBeforeAfterImage.DataRecorderMetaData.CreatedBy);
                    personName = orgRolUserDomain != null ? orgRolUserDomain.Person.FirstName + " " + orgRolUserDomain.Person.LastName : "";
                }
            }

            var markbeforeAfterImagesHistryDomain = markbeforeAfterImagesHistryList.FirstOrDefault(x => x.ServiceId == jobEstimareAfter.ServiceId && x.BestTypeId == (long)BeforeAfterPairType.REVIEWMARKETINGTYPE);

            var viewModel = new BeforeAfterViewModel()
            {
                Id = 0,
                BeforeCss = beforeImage != null ? beforeImage.File.css : "rotate(0)",
                AfterCss = jobEstimareAfter != null ? jobEstimareAfter.File.css : "rotate(0)",
                RelactiveLocationAfterImage = jobEstimareAfter != null ? GetBase64String(jobEstimareAfter.File) : "",
                RelactiveLocationBeforeImage = beforeImage != null ? GetBase64String(beforeImage.File) : "",

                //RelactiveLocationAfterImage = jobEstimareAfter != null ? (jobEstimareAfter.File.RelativeLocation + "\\" + jobEstimareAfter.File.Name).ToUrl() : "",
                //RelactiveLocationBeforeImage = beforeImage != null ? (beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name).ToUrl() : "",
                AfterImageUploadedOn = jobEstimareAfter != null && jobEstimareAfter.DataRecorderMetaData != null ? jobEstimareAfter.DataRecorderMetaData.DateModified != null ? jobEstimareAfter.DataRecorderMetaData.DateModified : jobEstimareAfter.DataRecorderMetaData.DateCreated : default(DateTime?),
                BeforeImageUploadedOn = beforeImage != null && beforeImage.DataRecorderMetaData != null ? beforeImage.DataRecorderMetaData.DateModified != null ? jobEstimareAfter.DataRecorderMetaData.DateModified : jobEstimareAfter.DataRecorderMetaData.DateCreated : default(DateTime?),
                IsSelected = markbeforeAfterImagesHistryDomain != null ? true : false,
                SelectedBy = personName,
                BeforeServiceId = jobEstimateService.Id,
                AfterServiceId = jobEstimareAfter.ServiceId,
                FrachiseeName = franchiseeName,
                ModifiedDate = modifiedDate,
                Index = index
            };

            return viewModel;
        }

        private string GetFullAddress(Address address)
        {
            string fullAddress = "";
            if (address.AddressLine1 != "" && address.AddressLine1 != null)
            {
                fullAddress += address.AddressLine1 + " ,";
            }
            if (address.AddressLine2 != "" && address.AddressLine2 != null)
            {
                fullAddress += address.AddressLine2 + " ,";
            }
            if (address.City != null)
            {
                fullAddress += address.City.Name + " ,";
            }
            else
            {
                fullAddress += address.CityName + " ,";
            }
            if (address.State != null)
            {
                fullAddress += address.State.Name + " ,";
            }
            else
            {
                fullAddress += address.StateName + " ,";
            }
            if (address.Country != null)
            {
                fullAddress += address.Country.Name + " ,";
            }
            var lastIndex = fullAddress.LastIndexOf(',');
            fullAddress = fullAddress.Substring(0, lastIndex);
            return fullAddress;
        }

        public BeforeAfterForImageViewModel CreateNeforeAfterViewModel(JobEstimateServices jobEstimateBeforeCategory,
            JobEstimateServices jobEstimateAfterCateogy, List<JobEstimateImage> jobEstimateImagess, JobEstimateServices jobEstimateBeforesExteriorImages)
        {
            var jobEstimateImageBuildingExterior = jobEstimateImagess.FirstOrDefault(x => x.TypeId == (long?)(BeforeAfterImagesType.ExteriorBuilding));
            var linkUrl = "";
            var service = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory : jobEstimateAfterCateogy;
            var jobEstimateAfterImage = jobEstimateImagess.FirstOrDefault(x => jobEstimateAfterCateogy != null && x.ServiceId == jobEstimateAfterCateogy.Id);
            var jobEstimateBeforeImage = jobEstimateImagess.FirstOrDefault(x => jobEstimateBeforeCategory != null && x.ServiceId == jobEstimateBeforeCategory.Id);
            var scheduler = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.JobEstimateImageCategory.JobScheduler : jobEstimateAfterCateogy.JobEstimateImageCategory.JobScheduler;
            if (scheduler.JobId != null)
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + scheduler.JobId + "/edit/" + scheduler.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + scheduler.EstimateId + "/manage/" + scheduler.Id;
            }

            var isNonResistianCLass = scheduler.Job != null ? (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.FLOORING) || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                 (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                 || scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.UNCLASSIFIED) :
                 (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.FLOORING) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                 (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                 || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.UNCLASSIFIED);

            var marketingClass = scheduler.Job != null ? scheduler.Job.JobType.Name : scheduler.Estimate.MarketingClass.Name;
            var imageViewModel = new BeforeAfterForImageViewModel()
            {
                AfterCss = (jobEstimateAfterImage != null && jobEstimateAfterImage.File != null) ? jobEstimateAfterImage.File.css : "rotate(0)",
                BeforeCss = (jobEstimateAfterImage != null && jobEstimateBeforeImage.File != null) ? jobEstimateBeforeImage.File.css : "rotate(0)",
                RelactiveLocationAfterImageUrl = jobEstimateAfterImage != null ? GetBase64String(jobEstimateAfterImage.File) : "/Content/images/no_image_thumb.gif",
                RelactiveLocationBeforeImageUrl = jobEstimateBeforeImage != null ? GetBase64String(jobEstimateBeforeImage.File) : "/Content/images/no_image_thumb.gif",
                BeforeServiceId = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.JobEstimateServices.Id : default(long?),
                AfterServiceId = jobEstimateAfterImage != null ? jobEstimateAfterImage.JobEstimateServices.Id : default(long?),
                IsBestPicture = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.IsBestImage : false,
                IsAddToLocalGallery = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.IsAddToLocalGallery : jobEstimateAfterImage != null ? jobEstimateAfterImage.IsAddToLocalGallery : false,
                AfterImageId = jobEstimateAfterImage != null ? jobEstimateAfterImage.Id : default(long?),
                BeforeImageId = jobEstimateBeforeImage != null ? jobEstimateBeforeImage.Id : default(long?),
                ServicesType = service != null && service.ServiceType != null ? service.ServiceType.Name : "",
                SurfaceColor = service != null ? service.SurfaceColor : "",
                SurfaceType = service != null ? service.SurfaceType : "",
                Description = service != null ? service.JobEstimateImageCategory.JobScheduler.Title : "",
                SchedulerUrl = linkUrl,
                IsJob = scheduler.JobId != null ? true : false,
                Title = scheduler.Title,
                JobId = scheduler.JobId,
                EstimateId = scheduler.EstimateId,
                RelactiveLocationExteriorImageUrl = jobEstimateImageBuildingExterior != null ? GetBase64String(jobEstimateImageBuildingExterior.File) : "/Content/images/no_image_thumb.gif",
                IsComercialClass = isNonResistianCLass,
                MarketingClass = marketingClass,
                OrderNo = isNonResistianCLass ? 1 : 100,
                SchedulerNames = scheduler.Job != null ? "J" + scheduler.JobId : "E" + scheduler.EstimateId,
                JobEstimateId = scheduler.Job != null ? scheduler.JobId : scheduler.EstimateId,
                CustomerName = scheduler.Job != null ? scheduler.Job.JobCustomer.CustomerName : scheduler.Estimate.JobCustomer.CustomerName,
                RelactiveLocationAfterImageUrlThumb = jobEstimateAfterImage != null && jobEstimateAfterImage.ThumbFileId != null ? (jobEstimateAfterImage.ThumbFile.RelativeLocation + "\\" + jobEstimateAfterImage.ThumbFile.Name).ToUrl() : "/Content/images/no_image_thumb.gif",
                RelactiveLocationBeforeImageUrlThumb = jobEstimateBeforeImage != null && jobEstimateBeforeImage.ThumbFileId != null ? (jobEstimateBeforeImage.ThumbFile.RelativeLocation + "\\" + jobEstimateBeforeImage.ThumbFile.Name).ToUrl() : "/Content/images/no_image_thumb.gif",
            };
            if (imageViewModel.RelactiveLocationAfterImageUrl == "" || imageViewModel.RelactiveLocationBeforeImageUrl == "")
            {
                imageViewModel.IsImageEmpty = true;
            }
            else
            {
                imageViewModel.IsImageEmpty = false;
            }
            return imageViewModel;
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


        public BeforeAfterForImageViewModel CreateBeforeAfterViewModel(BeforeAfterImages jobEstimateBeforeCategory,
            BeforeAfterImages jobEstimateAfterCateogy, BeforeAfterImages jobEstimateBeforesExteriorImages, List<JobEstimateServices> jobEstimateService)
        {
            var beforeService = jobEstimateBeforeCategory != null ? jobEstimateService.Where(x => x.Id == jobEstimateBeforeCategory.ServiceId).FirstOrDefault() : null;
            var afterService = jobEstimateAfterCateogy != null ? jobEstimateService.Where(x => x.Id == jobEstimateAfterCateogy.ServiceId).FirstOrDefault() : null;
            var beforeServiceId = beforeService != null ? beforeService.Id : default(long?);
            var afterServiceId = afterService != null ? afterService.Id : default(long?);
            string linkUrl = "";
            var scheduler = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.JobScheduler : jobEstimateAfterCateogy.JobScheduler;
            if (scheduler.JobId != null)
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + scheduler.JobId + "/edit/" + scheduler.Id;
            }
            else
            {
                linkUrl = _settings.SiteRootUrl + "/#/scheduler/" + scheduler.EstimateId + "/manage/" + scheduler.Id;
            }

            var isNonResistianCLass = scheduler.Job != null ? (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.FLOORING) || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                 (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                 || scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT || (scheduler.Job.JobTypeId == (long)NonResidentalClassEnum.UNCLASSIFIED) :
                 (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.FLOORING) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.HOMEMANAGEMENT) ||
                 (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.INTERIORDESIGN) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIAL)
                 || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.RESIDENTIALPROPERTYMGMT) || (scheduler.Estimate.TypeId == (long)NonResidentalClassEnum.UNCLASSIFIED);

            var marketingClass = scheduler.Job != null ? scheduler.Job.JobType.Name : scheduler.Estimate.MarketingClass.Name;
            var imageViewModel = new BeforeAfterForImageViewModel()
            {
                AfterCss = (jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.File != null) ? jobEstimateAfterCateogy.File.css : "rotate(0)",
                BeforeCss = (jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.File != null) ? jobEstimateBeforeCategory.File.css : "rotate(0)",
                //RelactiveLocationAfterImageUrl = jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.File != null ? "" : "/Content/images/no_image_thumb.gif",
                //RelactiveLocationBeforeImageUrl = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.File != null ? "" : "/Content/images/no_image_thumb.gif",

                RelactiveLocationAfterImageUrl = jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.File != null ? (jobEstimateAfterCateogy.File.RelativeLocation + "\\" + jobEstimateAfterCateogy.File.Name).ToUrl() : "/Content/images/no_image_thumb.gif",
                RelactiveLocationBeforeImageUrl = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.File != null ? (jobEstimateBeforeCategory.File.RelativeLocation + "\\" + jobEstimateBeforeCategory.File.Name).ToUrl() : "/Content/images/no_image_thumb.gif",

                BeforeServiceId = beforeServiceId,
                AfterServiceId = afterServiceId,
                IsBestPicture = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.IsBestImage : false,
                IsAddToLocalGallery = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.IsAddToLocalGallery : jobEstimateAfterCateogy != null ? jobEstimateAfterCateogy.IsAddToLocalGallery : false,
                AfterImageId = jobEstimateAfterCateogy != null ? jobEstimateAfterCateogy.Id : default(long?),
                BeforeImageId = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.Id : default(long?),
                ServicesType = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.ServiceType != null ? jobEstimateBeforeCategory.ServiceType.Name : "",
                SurfaceColor = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.SurfaceColor : "",
                SurfaceType = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.SurfaceType : "",
                Description = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.JobScheduler.Title : "",
                SchedulerUrl = linkUrl,
                IsJob = scheduler.JobId != null ? true : false,
                Title = scheduler.Title,
                JobId = scheduler.JobId,
                EstimateId = scheduler.EstimateId,
                RelactiveLocationExteriorImageUrl = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? "" : "/Content/images/no_image_thumb.gif",
                IsComercialClass = isNonResistianCLass,
                MarketingClass = marketingClass,
                OrderNo = isNonResistianCLass ? 1 : 100,
                SchedulerNames = scheduler.Job != null ? "J" + scheduler.JobId : "E" + scheduler.EstimateId,
                JobEstimateId = scheduler.Job != null ? scheduler.JobId : scheduler.EstimateId,
                CustomerName = scheduler.Job != null ? scheduler.Job.JobCustomer.CustomerName : scheduler.Estimate.JobCustomer.CustomerName,
                ToBeGroupedById = scheduler.Estimate != null ? scheduler.EstimateId : scheduler.JobId,
                BeforeImageFileId = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.File != null ? jobEstimateBeforeCategory.File.Id : default(long?),
                AfterImageFileId = jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.File != null ? jobEstimateAfterCateogy.File.Id : default(long?),
                ExteriorImageFileId = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? jobEstimateBeforesExteriorImages.File.Id : default(long?),
                RelactiveLocationAfterImageUrlThumb = jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.ThumbFileId != null ? (jobEstimateAfterCateogy.ThumbFile.RelativeLocation + "\\" + jobEstimateAfterCateogy.ThumbFile.Name).ToUrl() : "/Content/images/no_image_thumb.gif",
                RelactiveLocationBeforeImageUrlThumb = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.ThumbFileId != null ? (jobEstimateBeforeCategory.ThumbFile.RelativeLocation + "\\" + jobEstimateBeforeCategory.ThumbFile.Name).ToUrl() : "/Content/images/no_image_thumb.gif",
                S3BucketAfterImageUrlThumb= jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.ThumbFileId != null ? jobEstimateAfterCateogy.S3BucketThumbURL: "/Content/images/no_image_thumb.gif",
                S3BucketBeforeImageUrlThumb= jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.ThumbFileId != null ? (jobEstimateBeforeCategory.S3BucketThumbURL) : "/Content/images/no_image_thumb.gif",
                S3BucketExteriorImageUrlThumb = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? jobEstimateBeforesExteriorImages.S3BucketURL : "/Content/images/no_image_thumb.gif",
            };


            if (imageViewModel.RelactiveLocationAfterImageUrl == "")
            {
                imageViewModel.RelactiveLocationAfterImageUrl = jobEstimateAfterCateogy.ImageUrl != "" ? jobEstimateAfterCateogy.ImageUrl : "";
            }
            if (imageViewModel.RelactiveLocationAfterImageUrl == "")
            {
                imageViewModel.RelactiveLocationBeforeImageUrl = jobEstimateBeforeCategory.ImageUrl != "" ? jobEstimateBeforeCategory.ImageUrl : "";
            }
            if (imageViewModel.RelactiveLocationAfterImageUrl == "/Content/images/no_image_thumb.gif" && imageViewModel.RelactiveLocationBeforeImageUrl == "/Content/images/no_image_thumb.gif")
            {
                imageViewModel.IsImageEmpty = true;
            }
            else
            {
                imageViewModel.IsImageEmpty = false;
            }
            return imageViewModel;
        }


        public JobEstimateImageCategory CreateJobEstimateCategory(ShiftJobEstimateViewModel model)
        {
            var domain = new JobEstimateImageCategory()
            {
                Id = model.Id != null ? model.Id.GetValueOrDefault() : 0,
                JobId = model.JobId,
                EstimateId = model.EstimateId,
                MarkertingClassId = model.MarketingClassId,
                IsNew = model.Id <= 0 ? true : false,
                SchedulerId = model.SchedulerId
            };
            return domain;
        }

        public JobEstimateImage CreateJobEstimateImageDomain(ShiftJobEstimateViewModel model, long categoryId, long? typeId, long? fileId)
        {
            if(fileId != null)
            {
                //var beforeAfterImageData = _beforeAfterImageRepository.Table.FirstOrDefault(x => x.EstimateId == model.EstimateId && x.SchedulerId == model.SchedulerId && x.FileId == fileId);
                var beforeAfterImageData = _beforeAfterImageRepository.Table.FirstOrDefault(x => x.FileId == fileId);
                if (beforeAfterImageData != null)
                {
                    fileId = beforeAfterImageData.IsImageCropped == true ? beforeAfterImageData.CroppedImageId : fileId;
                }
            }
            var domain = new JobEstimateImage()
            {
                Id = model.Id != null ? model.Id.GetValueOrDefault() : 0,
                FileId = fileId,
                IsNew = model.Id > 0 ? false : true,
                ServiceId = categoryId,
                TypeId = typeId,
                DataRecorderMetaData = model.DataRecorderMetaData
            };
            return domain;
        }

        private string GetBase64FromFile(BeforeAfterImages beforeAfterImages)
        {
            var base64Url = "";
            if (beforeAfterImages.ThumbFileId != null)
            {
                var fileDomain = _fileRepository.Get(beforeAfterImages.ThumbFileId.GetValueOrDefault());
                base64Url = GetBase64String(fileDomain);
            }
            else if (beforeAfterImages.FileId != null)
            {
                var fileDomain = _fileRepository.Get(beforeAfterImages.FileId.GetValueOrDefault());
                base64Url = GetBase64String(fileDomain);
            }
            return base64Url;
        }
    }
}
