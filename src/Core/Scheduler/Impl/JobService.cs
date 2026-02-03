using API.Impl;
using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Enum;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing;
using Core.Billing.ViewModel;
using Core.Geo;
using Core.Geo.Domain;
using Core.Notification;
using Core.Notification.Domain;
using Core.Notification.Enum;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using Microsoft.SqlServer;
using Ical.Net.ExtensionMethods;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobService : IJobService
    {
        private IBeforeAfterThumbNailService _beforeAfterThumbNameService;
        System.Drawing.Image imgBef;
        System.Drawing.Image _imgR;
        System.Drawing.Image _img2;
        private readonly IRepository<ReviewMarketingImageLastDateHistry> _reviewMarketingImageLastDateHistryRepository;
        private readonly IJobSchedulerService _jobSchedulerService;
        private readonly IJobFactory _jobFactory;
        private readonly IJobDetailsFactory _jobDetailsFactory;
        private readonly IAddressFactory _aadressFactory;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<JobCustomer> _jobCustomerRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly IRepository<JobEstimate> _jobEstimateRepository;
        private readonly IRepository<JobScheduler> _jobschedulerRepository;
        private readonly IRepository<JobResource> _jobResourceRepository;
        private readonly IRepository<JobNote> _jobNoteRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<Holiday> _holidayRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Phone> _phoneRepository;
        private readonly IRepository<NotificationEmail> _notificationEmailRepository;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<DataRecorderMetaData> _dataRecorderMetaDataRepository;
        private readonly IRepository<JobEstimateServices> _jobEstimateServices;
        private readonly IRepository<EstimateInvoiceAssignee> _estimateInvoiceAssignee;
        private readonly IRepository<JobEstimateImageCategory> _jobEstimateImageCategory;
        private readonly IRepository<JobEstimateImage> _jobEstimateImage;
        private readonly IRepository<CustomerSchedulerReminderAudit> _jobReminderAuditRepository;
        private readonly IRepository<EquipmentUserDetails> _equipmentUserDetailsRepository;
        private readonly IRepository<JobDetails> _jobDetailsRepository;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IRepository<BeforeAfterImageMailAudit> _beforeAfterImageMailAuditRepository;
        private readonly IRepository<EstimateInvoice> _estimateInvoiceRepository;
        private readonly ISettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService _logService;
        private readonly IEstimateService _estimateService;
        private readonly ISendNewJobNotificationtoTechService _sendNewJobNotificationToTechService;
        private readonly IClock _clock;
        private readonly IFileService _fileService;
        private readonly IBeforeAfterImageService _beforeAfterImageService;
        private readonly ISortingHelper _sortingHelper;
        private readonly IPdfFileService _pdfFileService;
        private IInvoiceService _invoiceService;
        public long? jobschedulerId;
        private readonly IDebuggerLog _debuggerService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<MarkbeforeAfterImagesHistry> _markbeforeAfterImagesHistryRepository;
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        List<long> oldTechList = new List<long>();
        List<long> oldInactiveTechList = new List<long>();
        const string jobType = "Job";
        List<long> newdTechList = new List<long>();
        private readonly IRepository<BeforeAfterImages> _beforeAfterImagesRepository;
        private ILogService logger = ApplicationManager.DependencyInjection.Resolve<ILogService>();
        private readonly IRepository<DebuggerLogs> _debuggerLogsRepository;
        private readonly IEstimateInvoiceServices _estimateInvoiceServices;
        private readonly IRepository<EstimateInvoiceService> _estimateInvoiceServiceRepository;
        private readonly IRepository<EstimateInvoiceAssignee> _estimateInvoiceAssigneeRepository;
        private readonly IRepository<CustomerSignature> _customerSignatureRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        public JobService(IUnitOfWork unitOfWork, IJobFactory jobFactory, ISortingHelper sortingHelper, IFileService fileService, IEstimateService estimateService,
            IAddressFactory addressFactory, IClock clock, ISendNewJobNotificationtoTechService sendNewJobNotificationToTechService, ILogService logService,
           IPdfFileService pdfFileService, ISettings settings, IInvoiceService invoiceService
            , IJobDetailsFactory jobDetailsFactory, IJobSchedulerService jobSchedulerService,
            IUserNotificationModelFactory userNotificationModelFactory, IBeforeAfterImageService beforeAfterImageService,
            IDebuggerLog debuggerService, IEstimateInvoiceServices estimateInvoiceServices, IBeforeAfterThumbNailService beforeAfterThumbNameService)
        {
            _unitOfWork = unitOfWork;
            _sendNewJobNotificationToTechService = sendNewJobNotificationToTechService;
            _logService = logService;
            _sortingHelper = sortingHelper;
            _fileService = fileService;
            _clock = clock;
            _estimateService = estimateService;
            _pdfFileService = pdfFileService;
            _jobFactory = jobFactory;
            _aadressFactory = addressFactory;
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _jobCustomerRepository = unitOfWork.Repository<JobCustomer>();
            _jobRepository = unitOfWork.Repository<Job>();
            _jobEstimateRepository = unitOfWork.Repository<JobEstimate>();
            _estimateInvoiceRepository = unitOfWork.Repository<EstimateInvoice>();
            _jobResourceRepository = unitOfWork.Repository<JobResource>();
            _jobNoteRepository = unitOfWork.Repository<JobNote>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _holidayRepository = unitOfWork.Repository<Holiday>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _jobschedulerRepository = unitOfWork.Repository<JobScheduler>();
            _personRepository = unitOfWork.Repository<Person>();
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _phoneRepository = unitOfWork.Repository<Phone>();
            _notificationEmailRepository = unitOfWork.Repository<NotificationEmail>();
            _emailTemplateRepository = unitOfWork.Repository<EmailTemplate>();
            _dataRecorderMetaDataRepository = unitOfWork.Repository<DataRecorderMetaData>();
            _jobEstimateServices = unitOfWork.Repository<JobEstimateServices>();
            _estimateInvoiceAssignee = unitOfWork.Repository<EstimateInvoiceAssignee>();
            _jobEstimateImageCategory = unitOfWork.Repository<JobEstimateImageCategory>();
            _jobEstimateImage = unitOfWork.Repository<JobEstimateImage>();
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _beforeAfterImageMailAuditRepository = unitOfWork.Repository<BeforeAfterImageMailAudit>();
            _settings = settings;
            _invoiceService = invoiceService;
            _equipmentUserDetailsRepository = unitOfWork.Repository<EquipmentUserDetails>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _stateRepository = unitOfWork.Repository<State>();
            _jobReminderAuditRepository = unitOfWork.Repository<CustomerSchedulerReminderAudit>();
            _jobDetailsRepository = unitOfWork.Repository<JobDetails>();
            _jobDetailsFactory = jobDetailsFactory;
            _jobSchedulerService = jobSchedulerService;
            _userNotificationModelFactory = userNotificationModelFactory;
            _customerRepository = unitOfWork.Repository<Customer>();
            _markbeforeAfterImagesHistryRepository = unitOfWork.Repository<MarkbeforeAfterImagesHistry>();
            _reviewMarketingImageLastDateHistryRepository = unitOfWork.Repository<ReviewMarketingImageLastDateHistry>();
            _beforeAfterImageService = beforeAfterImageService;
            _beforeAfterImagesRepository = unitOfWork.Repository<BeforeAfterImages>();
            _debuggerLogsRepository = unitOfWork.Repository<DebuggerLogs>();
            _debuggerService = debuggerService;
            _estimateInvoiceServices = estimateInvoiceServices;
            _customerSignatureRepository = unitOfWork.Repository<CustomerSignature>();
            _beforeAfterThumbNameService = beforeAfterThumbNameService;
            _estimateInvoiceServiceRepository = unitOfWork.Repository<EstimateInvoiceService>();
            _estimateInvoiceAssigneeRepository = unitOfWork.Repository<EstimateInvoiceAssignee>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
        }


        public JobListModel GetJobs(JobListFilter filter, int pageNumber, int pageSize)
        {
            var equipmentUserDetailsDomainList = _equipmentUserDetailsRepository.Table.Where(x => x.UserId != default(long)).ToList();
            var orgUserId = _organizationRoleUserRepository.Get(filter.LoggedInUserOrgId);
            if (!filter.IsFromScheduler)
                filter.StartDate = null;
            IQueryable<JobScheduler> jobs = JobFilterList(filter, equipmentUserDetailsDomainList);
            var finalcollection = jobs.ToList();
            var collectionFinal = new List<JobViewModel>();
            var totalSum = decimal.Zero;
            if (filter.IsFromScheduler)
            {
                pageSize = jobs.Count();
                var organizationRoleUserList = _organizationRoleUserRepository.Table.Where(x => x.Id != default(long)).ToList();
                var schedulerList = _jobschedulerRepository.Table.Where(x => x.EstimateId != null && x.FranchiseeId == filter.FranchiseeId).ToList();
                var schedulerListIds = schedulerList.Select(x => x.Id).ToList();
                var estimateInvoiceList = _estimateInvoiceRepository.Table.Where(x => x.EstimateId != null && x.FranchiseeId == filter.FranchiseeId).ToList();
                var estimateInvoiceListIds = estimateInvoiceList.Select(x => x.Id).ToList();
                var serviceList = _estimateInvoiceServiceRepository.Table.Where(x => estimateInvoiceListIds.Contains(x.EstimateInvoiceId)).ToList();
                var estimateInvoiceAssignees = _estimateInvoiceAssigneeRepository.Table.Where(x => x.EstimateId != null).ToList();
                var customerSignatureList = _customerSignatureRepository.Table.Where(x => estimateInvoiceListIds.Contains(x.EstimateInvoiceId.Value)).Select(x => x.EstimateInvoiceId).ToList();
                collectionFinal = finalcollection.Select(x => _jobFactory.CreateViewModel(x, estimateInvoiceList,
                schedulerList, serviceList, estimateInvoiceAssignees, equipmentUserDetailsDomainList, customerSignatureList, organizationRoleUserList)).ToList();


                var jb = collectionFinal.Where(x => filter.StartDateForCal <= x.ActualStartDate.Date && x.ActualStartDate.Date <= filter.EndDateForCal).ToList();
                var jobsWithInvoice = jb.Where(x => x.JobAmount.Count > 0).Select(x => x.JobAmount).ToList();
                var alreadyExistingInvoices = new List<JobAmountDuplicacyCheck>();
                foreach (var jobsAmt in jobsWithInvoice)
                {
                    foreach (var joba in jobsAmt)
                    {
                        var xjob = alreadyExistingInvoices.Where(x => x.InvoiceNumber == joba.InvoiceNumber && x.EstimateInvoiceId == joba.EstimateInvoiceId).ToList();
                        if (xjob.Count == 0)
                        {
                            totalSum += joba.JobValue.Value;
                            alreadyExistingInvoices.Add(new JobAmountDuplicacyCheck
                            {
                                InvoiceNumber = joba.InvoiceNumber,
                                EstimateInvoiceId = joba.EstimateInvoiceId
                            });
                        }

                    }
                }

                var filterForHoliday = new FranchiseeHolidayModel()
                {
                    StartDate = filter.StartDate.GetValueOrDefault(),
                    EndDate = filter.EndDate.GetValueOrDefault(),
                    FranchiseeId = filter.FranchiseeId
                };
                var holidayListCollection = GetHolidayListMonthWise(filterForHoliday).Collection;
                if (holidayListCollection != null)
                {
                    var holidayList = holidayListCollection.ToList();
                    collectionFinal.AddRange(holidayList);
                }
                collectionFinal = collectionFinal.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                finalcollection = finalcollection.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                collectionFinal = finalcollection.Select(x => _jobFactory.CreateViewModelForListView(x, equipmentUserDetailsDomainList)).ToList();
            }
            return new JobListModel
            {
                Collection = collectionFinal,
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, jobs.Count()),
                DefaultView = orgUserId.Person.CalendarPreference,
                TotalSum = decimal.Parse(string.Format("{00:00}", totalSum.ToString()))
            };
        }

        private JobScheduler CheckJobDateUpdated(JobScheduler jobScheduler)
        {
            if (jobScheduler.Job != null)
            {
                jobScheduler.StartDate = jobScheduler.Job.StartDate;
                jobScheduler.EndDate = jobScheduler.Job.EndDate;
            }
            return jobScheduler;
        }

        private IQueryable<JobScheduler> JobFilterList(JobListFilter filter, List<EquipmentUserDetails> equipmentUserDetails)
        {
            var equipmentUserDetailsList = equipmentUserDetails.Where(x => x.IsLock == true).Select(x => x.UserId).ToList();
            var resourceIds = (filter.ResourceIds == null || filter.ResourceIds.Count() <= 0) ? null : filter.ResourceIds.ToList();
            bool isMultipleResource = resourceIds != null ? true : false;

            var startDatePart = DateTime.MinValue;
            var endDatePart = DateTime.MinValue;
            if (filter.DateCreated == null)
            {
                startDatePart = DateTime.MinValue;
            }
            else
            {
                startDatePart = filter.DateCreated.GetValueOrDefault().Date;
            }
            if (filter.DateModified == null)
            {
                endDatePart = DateTime.MinValue;
            }
            else
            {
                endDatePart = filter.DateModified.GetValueOrDefault().Date;
            }



            var jobsList = _jobSchedulerRepository.IncludeMultiple(x => x.Meeting, x => x.Job, x => x.Job.JobCustomer, x => x.OrganizationRoleUser,
                x => x.SalesRep, x => x.Estimate, x => x.Estimate.JobCustomer)
                .Where(x => (filter.FranchiseeId <= 1 || x.FranchiseeId == filter.FranchiseeId)
                && x.IsActive
                && x.Franchisee != null
                && x.OrganizationRoleUser != null
                && (filter.TechId <= 0 || x.OrganizationRoleUser.UserId == filter.TechId || x.SalesRep.Id == filter.TechId)
                && (filter.RoleId == null || x.OrganizationRoleUser.RoleId == filter.RoleId)
                && (filter.Option == null || (filter.Option == 1 ? x.Job != null : (filter.Option == 0 ? x.Estimate != null : x.IsVacation)))
                && (filter.Imported == null || (filter.Imported == 1 ? x.IsImported : !x.IsImported))
                && (string.IsNullOrEmpty(filter.Text)
                || (x.Job.JobCustomer.CustomerName.Contains(filter.Text))
                || x.Job.JobCustomer.Email.Contains(filter.Text)
                || x.Estimate.JobCustomer.Email.Contains(filter.Text)
                || x.Job.QBInvoiceNumber.Equals(filter.Text)
                || x.Title.Contains(filter.Text)
                || x.SalesRep.Person.FirstName.Contains(filter.Text)
                || x.SalesRep.Person.LastName.Contains(filter.Text)
                 || x.Job.JobCustomer.PhoneNumber.Contains(filter.Text)
                 || x.Estimate.JobCustomer.PhoneNumber.Contains(filter.Text))
                 && (string.IsNullOrEmpty(filter.CustomerName)
                || (x.Job.JobCustomer.CustomerName.Contains(filter.CustomerName))
                || (x.Estimate.JobCustomer.CustomerName.StartsWith(filter.CustomerName)))
                && (string.IsNullOrEmpty(filter.FirstName)
                || (x.SalesRep.Person.FirstName.Contains(filter.FirstName)))
                && (string.IsNullOrEmpty(filter.LastName)
                || (x.SalesRep.Person.LastName.Contains(filter.LastName)))
                && (string.IsNullOrEmpty(filter.Email)
                || (x.Job.JobCustomer.Email.Contains(filter.Email))
                || (x.Estimate.JobCustomer.Email.Contains(filter.Email)))
                && (string.IsNullOrEmpty(filter.PhoneNumber)
                || (x.Job.JobCustomer.PhoneNumber.Contains(filter.PhoneNumber))
                || (x.Estimate.JobCustomer.PhoneNumber.Contains(filter.PhoneNumber)))
                && (string.IsNullOrEmpty(filter.Street)
                || (x.Estimate.JobCustomer.Address.AddressLine1.Contains(filter.Street))
                || (x.Estimate.JobCustomer.Address.AddressLine2.Contains(filter.Street)))
                && (string.IsNullOrEmpty(filter.City)
                || (x.Job.JobCustomer.Address.CityName.Contains(filter.City))
                || (x.Estimate.JobCustomer.Address.CityName.Contains(filter.City)))
                && (string.IsNullOrEmpty(filter.State)
                || (x.Job.JobCustomer.Address.StateName.Contains(filter.State))
                || (x.Estimate.JobCustomer.Address.StateName.Contains(filter.State)))
                && (string.IsNullOrEmpty(filter.Country)
                || (x.Job.JobCustomer.Address.Country.Name.Contains(filter.Country))
                || (x.Estimate.JobCustomer.Address.Country.Name.Contains(filter.Country)))
                && (string.IsNullOrEmpty(filter.ZipCode)
                || (x.Job.JobCustomer.Address.ZipCode.Contains(filter.ZipCode))
                || (x.Estimate.JobCustomer.Address.ZipCode.Contains(filter.ZipCode)))
                 && ((filter.StartDate == null || filter.EndDate == null)
                 || (x.StartDate <= filter.EndDate && x.EndDate >= filter.EndDate)
                 || (x.StartDate >= filter.StartDate && x.EndDate <= filter.EndDate))
                && (filter.JobTypeId <= 0 || x.Job.JobTypeId == filter.JobTypeId)).ToList();

            var jobs = jobsList.Where(x => ((startDatePart == DateTime.MinValue && endDatePart == DateTime.MinValue)
             || (x.ActualStartDateInLocal <= endDatePart && x.ActualEndDateInLocal >= endDatePart)
             || (x.ActualStartDateInLocal >= startDatePart && x.ActualEndDateInLocal <= endDatePart))).AsQueryable();

            if (!filter.IsLock.GetValueOrDefault())
            {
                jobs = jobs.Where(x => (x.OrganizationRoleUser.RoleId == (long)RoleType.Equipment)
                || x.OrganizationRoleUser.Person.UserLogin.IsLocked == filter.IsLock);
            }
            if (isMultipleResource)
            {
                var resourceId = resourceIds.FirstOrDefault();
                if (filter.ResourceIds.Count() > 0)
                {
                    jobs = jobs.Where(x => resourceIds.Contains(x.AssigneeId.Value));
                }
                else
                {
                    jobs = jobs.Where(x => x.PersonId == filter.PersonId);
                }
            }
            if (equipmentUserDetailsList.Count() > 0 && !filter.IsLock.GetValueOrDefault())
            {
                jobs = jobs.Where(x => !equipmentUserDetailsList.Contains(x.OrganizationRoleUser.UserId));
            }
            jobs = _sortingHelper.ApplySorting(jobs, x => x.Id, (long)SortingOrder.Desc);
            if (filter.PropName != null)
            {
                switch (filter.PropName)
                {
                    case "Id":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Id, filter.Order);
                        break;
                    case "Name":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Job.JobCustomer.CustomerName, filter.Order);
                        break;
                    case "Email":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Job.JobCustomer.Email, filter.Order);
                        break;
                    case "Title":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Title, filter.Order);
                        break;
                    case "Country":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Job.JobCustomer.Address.Country.Name, filter.Order);
                        break;
                    case "JobType":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Job.JobType.Name, filter.Order);
                        break;
                    case "StartDate":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.StartDate, filter.Order);
                        break;
                    case "EndDate":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.EndDate, filter.Order);
                        break;
                    case "SalesRep":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.SalesRep.Person.FirstName, filter.Order);
                        break;
                    case "QBInvoiceNumber":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Job.QBInvoiceNumber, filter.Order);
                        break;
                    case "Franchisee":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Franchisee.Organization.Name, filter.Order);
                        break;
                    case "PhoneNumber":
                        jobs = _sortingHelper.ApplySorting(jobs, x => x.Job.JobCustomer.PhoneNumber, filter.Order);
                        break;
                }
            }
            return jobs;
        }
        bool IsUpdated(JobEditModel model, string oldAddress1, string oldAddress2, string oldPhoneNumber, DateTime oldStartDatetime, DateTime oldEndDatetime)
        {
            if (model.JobCustomer.Address.AddressLine1 == oldAddress1 && model.JobCustomer.Address.AddressLine2 == oldAddress2 && model.JobCustomer.PhoneNumber == oldPhoneNumber
                && model.StartDate == oldStartDatetime && model.EndDate == oldEndDatetime)
            {
                return false;
            }
            return true;

        }

        private bool IsUpdatedForTimeOrAssigneeId(List<OldSchedulerModel> oldModel, long? jobId)
        {
            var todayDate = DateTime.UtcNow.Date;
            var isUpdate = false;
            var tomorrowDate = DateTime.UtcNow.AddDays(2).Date;
            var schedulerList = _jobschedulerRepository.Table.Where(x => x.JobId == jobId && x.IsActive).ToList();
            var schedulerNextDayList = schedulerList.Where(x => x.StartDate >= todayDate && x.StartDate <= tomorrowDate).ToList();
            if (schedulerNextDayList.Count() == 0)
                return false;
            else
            {
                foreach (var schedulerNextDay in schedulerNextDayList)
                {

                    var oldModelScheduler = oldModel.FirstOrDefault(x => x.Id == schedulerNextDay.Id);
                    if (oldModelScheduler == null)
                    {
                        if (schedulerNextDay.StartDate >= todayDate && schedulerNextDay.StartDate <= tomorrowDate)
                        {
                            isUpdate = true;
                            return true;
                        }
                        else
                            continue;
                    }
                    else
                    {
                        if (schedulerNextDay.StartDateTimeString == oldModelScheduler.StartDate && schedulerNextDay.EndDateTimeString == oldModelScheduler.EndDate
                            && schedulerNextDay.AssigneeId == oldModelScheduler.AssigneeId)
                        {
                            continue;
                        }
                        else
                        {
                            isUpdate = true;
                            return true;
                        }

                    }
                }
            }

            return isUpdate;
        }

        bool IsTimeUpdated(JobEditModel model, DateTime oldStartDatetime, DateTime oldEndDatetime)
        {
            if (model.StartDate == oldStartDatetime && model.EndDate == oldEndDatetime)
            {
                return false;
            }


            return true;

        }
        public void Save(JobEditModel model)
        {
            var currentDate = DateTime.Now;
            var isFromPast = (currentDate > model.StartDate || currentDate > model.EndDate) ? true : false;
            DatetimeModel datetimeModel = new DatetimeModel();
            List<OldSchedulerModel> oldSchedulerModel = new List<OldSchedulerModel>();
            var oldSchedulerList = _jobschedulerRepository.Table.Where(x => x.JobId == model.JobId && x.IsActive).Distinct().ToList();
            if (oldSchedulerList.Count() > 0)
            {
                foreach (var oldScheduler in oldSchedulerList)
                {
                    oldSchedulerModel.Add(GetOldScheduleModel(oldScheduler));
                }
            }
            var jobDomain = _jobRepository.Table.FirstOrDefault(x => x.Id == model.JobId);
            if (jobDomain != null)
            {
                datetimeModel.StartDate = jobDomain.StartDateTimeString;
                datetimeModel.EndDate = jobDomain.EndDateTimeString;
            }
            var schedulerIdForMail = default(long?);
            bool isUpdate = false;
            bool isTimeUpdate = false;
            string oldAddress1 = "";
            string oldAddress2 = "";
            long? oldAssigneeId = default(long?);
            string oldPhoneNumber = "";
            bool? isJobRepeat = false;
            var schedulerId = default(long?);
            DateTime oldStartDatetime = default(DateTime);
            DateTime oldEndDatetime = default(DateTime);
            oldTechList = oldSchedulerList.Select(x => x.AssigneeId.Value).Distinct().ToList();
            oldInactiveTechList = _jobschedulerRepository.Table.Where(x => x.JobId == model.JobId && !x.IsActive).Select(x => x.AssigneeId.Value).Distinct().ToList();
            newdTechList = model.TechIds.Distinct().ToList();
            List<long> addedTechList1 = newdTechList.Except(oldTechList).ToList();
            List<long> deletedTechList1 = oldTechList.Except(newdTechList).ToList();
            List<long> sameTechList1 = oldTechList.Intersect(newdTechList).ToList();
            List<long> rescheduledTechList1 = oldInactiveTechList.Intersect(addedTechList1).ToList();
            addedTechList1 = addedTechList1.Except(rescheduledTechList1).ToList();

            List<long?> schedulerLists = new List<long?>();
            List<long?> schedulerListsForImage = new List<long?>();
            var jobOccurance = model.JobOccurence != null ? model.JobOccurence.Collection : null;

            var oldDaata = _jobschedulerRepository.Table.FirstOrDefault(x => x.JobId == model.JobId && x.Id == model.Id);
            if (oldDaata != null)
            {
                oldAddress1 = oldDaata != null ? oldDaata.Job.JobCustomer.Address.AddressLine1 : "";
                oldAddress2 = oldDaata != null ? oldDaata.Job.JobCustomer.Address.AddressLine2 : "";
                oldStartDatetime = oldDaata != null ? oldDaata.StartDate : default(DateTime);
                oldEndDatetime = oldDaata != null ? oldDaata.EndDate : default(DateTime);
                oldPhoneNumber = oldDaata != null ? oldDaata.Job.JobCustomer.PhoneNumber : "";
                oldAssigneeId = oldDaata != null ? oldDaata.AssigneeId : default(long);
            }
            if (model.JobId != 0)
            {
                oldTechList = _jobschedulerRepository.Table.Where(x => x.JobId == model.JobId && x.IsActive).Select(x => x.AssigneeId.Value).Distinct().ToList();
                oldInactiveTechList = _jobschedulerRepository.Table.Where(x => x.JobId == model.JobId && !x.IsActive).Select(x => x.AssigneeId.Value).Distinct().ToList();
            }


            if (model.StatusId < (long)JobStatusType.InProgress)
            {
                model.StatusId = (long)JobStatusType.Assigned;
            }
            var customer = _jobFactory.CreateDomain(model.JobCustomer);
            _jobCustomerRepository.Save(customer);
            model.JobCustomer.CustomerId = customer.Id;

            var job = _jobFactory.CreateDomain(model);
            var isNewJob = job.Id == 0;
            job.CustomerId = model.JobCustomer.CustomerId;
            if (!model.IsDataToBeUpdateForAllJobs)
            {
                jobDomain = _jobRepository.Get(model.JobId);
                if (jobDomain != null)
                {
                    job.Description = jobDomain.Description;
                }
            }
            _jobRepository.Save(job);
            model.JobId = job.Id;
            if (string.IsNullOrEmpty(model.Title))
            {
                const string jobTitle = jobType;
                var jobCount = _jobSchedulerRepository.Table.Count(x => x.IsActive && x.FranchiseeId == model.FranchiseeId && x.JobId != null);
                model.Title = jobTitle + "" + (jobCount + 1);
            }
            if (model.JobOccurence != null && model.JobOccurence.Collection.Count() > 0)
            {

                SaveScheduleInfo(model);
                var jobestimate = _jobschedulerRepository.Table.Where(x => x.EstimateId == model.EstimateId).FirstOrDefault();
                foreach (var scheduler in model.JobOccurence.Collection)
                {
                    schedulerListsForImage.Add(scheduler.ScheduleId);
                    if (model.EstimateId != null)
                    {
                        SaveAssigneeInfo(scheduler, scheduler.ScheduleId, model.JobId, model.EstimateId.Value);
                        //SaveJobEstimateService(jobestimate.Id, scheduler.InvoiceNumber.Select(x => x.Id).ToList(), model.JobId, scheduler.ScheduleId);
                    }
                }
            }
            else
            {
                var list = model.JobSchedulerList;
                if (list != null)
                {
                    isJobRepeat = true;
                }
                if (model.TechIds != null)
                {
                    list = ManageJobAssignee(model, list).ToList();
                }
                int a = 0;
                schedulerLists = new List<long?>();
                foreach (var item in list)
                {

                    var schedulerModel = _jobFactory.CreateModel(model);

                    item.Title = schedulerModel.Title;
                    item.SalesRepId = schedulerModel.SalesRepId;
                    item.ServiceTypeId = model.JobTypeId;
                    var person = _organizationRoleUserRepository.Table.Where(x => x.Id == item.AssigneeId && x.OrganizationId == model.FranchiseeId).FirstOrDefault();

                    var scheduler = _jobFactory.CreateDomain(item);
                    scheduler.PersonId = person != null ? person.UserId : default(long?);
                    var offset = _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes;
                    if (item.DataRecorderMetaData.CreatedBy == null)
                    {
                        scheduler.DataRecorderMetaData = model.DataRecorderMetaData;
                    }
                    if (item.Id > 0)
                    {
                        var jobSchedulerDomain = _jobschedulerRepository.Table.FirstOrDefault(x => x.Id == item.Id);
                        scheduler.ParentJobId = jobSchedulerDomain.ParentJobId;
                        scheduler.IsRepeat = jobSchedulerDomain.IsRepeat;
                    }
                    else
                    {
                        scheduler.ParentJobId = schedulerId;
                        scheduler.IsRepeat = true;
                        scheduler.IsCustomerAvailable = true;
                    }
                    if (item.IsFromId && item.Id <= 0)
                    {
                        scheduler.ParentJobId = schedulerId;
                    }
                    scheduler.Offset = offset;

                    if (!scheduler.IsRepeat && model.Id > 0)
                    {
                        var oldDateTime = oldSchedulerList.FirstOrDefault(x => x.Id == item.Id);
                        scheduler.StartDate = oldDateTime.StartDate;
                        scheduler.EndDate = oldDateTime.EndDate;
                        scheduler.StartDateTimeString = oldDateTime.StartDateTimeString;
                        scheduler.EndDateTimeString = oldDateTime.EndDateTimeString;
                    }
                    scheduler.IsCancellationMailSend = true;
                    scheduler.IsJobConverted = true;
                    _jobSchedulerRepository.Save(scheduler);

                    jobschedulerId = model.JobId;
                    schedulerListsForImage.Add(scheduler.Id);
                    if (a == 0)
                        schedulerId = scheduler.Id;
                    a = ++a;

                    var jobestimate = _jobschedulerRepository.Table.Where(x => x.EstimateId == model.EstimateId).FirstOrDefault();
                    if (!schedulerLists.Contains(schedulerId))
                    {
                        schedulerLists.Add(schedulerId);
                        if (model.EstimateId != null)
                            SaveAssigneeInfoForEditJob(item, schedulerId.Value, model.JobId, model.EstimateId.GetValueOrDefault());
                        //SaveJobEstimateService(jobestimate.Id, item.InvoiceNumber.Select(x => x.Id).ToList(), model.JobId, schedulerId.Value);

                    }
                    else
                    {

                        schedulerLists.Add(scheduler.Id);
                        if (model.EstimateId != null)
                        {
                            SaveAssigneeInfoForEditJob(item, scheduler.Id, model.JobId, model.EstimateId.GetValueOrDefault());
                            //SaveJobEstimateService(jobestimate.Id, item.InvoiceNumber.Select(x => x.Id).ToList(), model.JobId, schedulerId.Value);
                        }
                    }
                }
            }

            SavingJobDetails(model, schedulerLists, model.Id);
            ChangingJobDetails(model);
            if (datetimeModel.StartDate != default(DateTime))
            {
                _jobSchedulerService.ChangeSchedulerDateTime(model, datetimeModel, oldSchedulerModel);
            }
            var assignedbyId = model.DataRecorderMetaData.CreatedBy;
            var assignedbyInfo = _jobschedulerRepository.Table.Where(x => x.JobId == model.JobId).Select(x => x.OrganizationRoleUser.Organization.Id).FirstOrDefault();
            var assignedByPhone = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.OrganizationId == assignedbyInfo);
            var personId = _personRepository.Table.Where(x => x.Id == assignedByPhone.UserId).Select(x => x.Id).FirstOrDefault();
            model.AssigneePhone = assignedByPhone.Organization.Phones.FirstOrDefault().Number;
            model.AssigneeName = assignedByPhone.Person.FirstName + " " + assignedByPhone.Person.MiddleName + " " + assignedByPhone.Person.LastName;
            DateTime currentUtcDate = _clock.ToLocal(_clock.UtcNow, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime futureUtcDate = _clock.ToLocal(_clock.UtcNow.AddDays(1), _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseCurrentUtcDate = _clock.ToLocal(model.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseFutureUtcDate = _clock.ToLocal(model.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            string address = model.JobCustomer.Address.AddressLine1 + " " + model.JobCustomer.Address.AddressLine2 + ", " + model.JobCustomer.Address.City + " " + model.JobCustomer.Address.ZipCode;
            var startDate = new DateTime();
            var endDate = new DateTime();
            model.Id = schedulerId;
            if (!isJobRepeat.Value)
            {
                foreach (var schedulerList in schedulerListsForImage)
                {
                    if (model.EstimateId != null)
                    {
                        // CheckingForBeforeAndAfterImages(model.EstimateId, model.JobId, schedulerList, model.LoggedInUserId, model.JobTypeId);

                    }
                }
            }
            if (oldDaata != null)
            {
                isUpdate = IsUpdated(model, oldAddress1, oldAddress2, oldPhoneNumber, oldStartDatetime, oldEndDatetime);
            }

            if (isNewJob == true)
            {
                if (schedulerLists.Count() == 0)
                {
                    schedulerIdForMail = model.JobOccurence.Collection.FirstOrDefault().ScheduleId;
                }
                else
                {
                    schedulerIdForMail = schedulerLists.FirstOrDefault();
                }
                SendMailToCustomer(schedulerIdForMail);
                if (jobOccurance != null)
                {
                    foreach (var items in jobOccurance.Select(x => x.AssigneeId).Distinct())
                    {
                        var jobSchedulers = jobOccurance != null ? jobOccurance.ToList().Where(x => x.AssigneeId == items) : null;
                        if (jobSchedulers != null)
                        {
                            foreach (var jobScheduler in jobSchedulers)
                            {
                                if (jobScheduler == null)
                                {
                                    startDate = model.ActualStartDateString;
                                    endDate = model.ActualEndDateString;
                                }
                                else
                                {
                                    startDate = jobScheduler.ActualStartDateString;
                                    endDate = jobScheduler.ActualEndDateString;
                                }
                                DateTime databaseCurrentUtcDate2 = _clock.ToLocal(startDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
                                if (databaseCurrentUtcDate2.Date >= currentUtcDate.Date && databaseCurrentUtcDate2.Date <= futureUtcDate.Date)
                                {
                                    if (databaseCurrentUtcDate2.Date == currentUtcDate.Date)
                                    {
                                        model.dateType = "TODAY";
                                    }
                                    else
                                    {
                                        model.dateType = "TOMORROW";
                                    }
                                    var userId = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                                    var isLocked = _userLoginRepository.Table.Where(x => x.Id == userId).Select(x => x.IsLocked).FirstOrDefault();
                                    if (!isLocked)
                                    {
                                        if (jobScheduler != null)
                                        {
                                            model.StartDate = jobScheduler.ActualStartDateString;
                                            model.EndDate = jobScheduler.ActualEndDateString;
                                        }
                                        model.jobTypeName = jobType;

                                        SendingUrgentMails(model, items);
                                    }

                                }
                                else
                                {
                                    var userId2 = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                                    var isLocked2 = _userLoginRepository.Table.Where(x => x.Id == userId2).Select(x => x.IsLocked).FirstOrDefault();
                                    if (!isLocked2)
                                    {
                                        if (jobScheduler != null)
                                        {
                                            model.StartDate = jobScheduler.ActualStartDateString;
                                            model.EndDate = jobScheduler.ActualEndDateString;
                                        }
                                        model.jobTypeName = jobType;
                                        if (model.Id == null)
                                        {
                                            model.Id = jobScheduler.ScheduleId;
                                        }
                                        SendingNewsMails(model, items);
                                    }
                                }
                            }
                        }
                        else
                        {
                            startDate = model.ActualStartDateString;
                            endDate = model.ActualEndDateString;
                            DateTime databaseCurrentUtcDate2 = _clock.ToLocal(startDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
                            if (databaseCurrentUtcDate2.Date >= currentUtcDate.Date && databaseCurrentUtcDate2.Date <= futureUtcDate.Date)
                            {
                                if (databaseCurrentUtcDate.Date == currentUtcDate.Date)
                                {
                                    model.dateType = "TODAY";
                                }
                                else
                                {
                                    model.dateType = "TOMORROW";
                                }
                                var userId = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                                var isLocked = _userLoginRepository.Table.Where(x => x.Id == userId).Select(x => x.IsLocked).FirstOrDefault();
                                if (!isLocked)
                                {
                                    model.jobTypeName = jobType;
                                    SendingUrgentMails(model, items);
                                }

                            }
                            else
                            {
                                var userId2 = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                                var isLocked2 = _userLoginRepository.Table.Where(x => x.Id == userId2).Select(x => x.IsLocked).FirstOrDefault();
                                if (!isLocked2)
                                {
                                    model.jobTypeName = jobType;
                                    SendingNewsMails(model, items);
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var items in model.TechIds)
                    {
                        startDate = model.ActualStartDateString;
                        endDate = model.ActualEndDateString;
                        DateTime databaseCurrentUtcDate2 = _clock.ToLocal(startDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
                        if (startDate.Date >= currentUtcDate.Date && startDate.Date <= futureUtcDate.Date)
                        {
                            if (databaseCurrentUtcDate.Date == currentUtcDate.Date)
                            {
                                model.dateType = "TODAY";
                            }
                            else
                            {
                                model.dateType = "TOMORROW";
                            }
                            var userId = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                            var isLocked = _userLoginRepository.Table.Where(x => x.Id == userId).Select(x => x.IsLocked).FirstOrDefault();
                            if (!isLocked)
                            {
                                model.StartDate = model.ActualStartDateString;
                                model.EndDate = model.ActualEndDateString;
                                model.jobTypeName = jobType;
                                SendingUrgentMails(model, items);
                            }

                        }
                        else
                        {
                            var userId2 = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                            var isLocked2 = _userLoginRepository.Table.Where(x => x.Id == userId2).Select(x => x.IsLocked).FirstOrDefault();
                            if (!isLocked2)
                            {
                                model.StartDate = model.ActualStartDateString;
                                model.EndDate = model.ActualEndDateString;
                                model.jobTypeName = jobType;
                                SendingNewsMails(model, items);
                            }
                        }
                    }
                }
            }
            else
            {
                if (schedulerLists.Count() == 0)
                {
                    schedulerIdForMail = model.JobOccurence.Collection.FirstOrDefault().ScheduleId;
                }
                else
                {
                    schedulerIdForMail = schedulerLists.FirstOrDefault();
                }


                if (addedTechList1.Count() > 0)
                {
                    foreach (var assigneeId in addedTechList1)
                    {
                        SendingNewsMails(model, assigneeId);
                    }
                }
                if (deletedTechList1.Count() > 0)
                {
                    foreach (var assigneeId in deletedTechList1)
                    {
                        var jobScheduler = _jobschedulerRepository.Table.Where(x => x.JobId == model.JobId && x.AssigneeId == assigneeId && !x.IsActive).FirstOrDefault();
                        if (jobScheduler != null)
                        {
                            model.ActualStartDateString = jobScheduler.ActualStartDate;
                            model.ActualEndDateString = jobScheduler.EndDateTimeString;
                        }
                        SendingCancellationMails(model, assigneeId);
                    }
                }
                if (rescheduledTechList1.Count() > 0)
                {
                    foreach (var assigneeId in rescheduledTechList1)
                    {
                        var jobScheduler = jobOccurance != null ? jobOccurance.ToList().Where(x => x.AssigneeId == assigneeId).FirstOrDefault() : null;
                        if (jobScheduler != null)
                        {
                            model.ActualStartDateString = jobScheduler.ActualStartDateString;
                            model.ActualEndDateString = jobScheduler.ActualEndDateString;
                        }
                        SendingRescheduledMails(model, assigneeId);
                    }
                }
            }
            if (isUpdate)
            {
                newdTechList = _jobschedulerRepository.Table.Where(x => x.JobId == model.JobId && x.IsActive).Select(x => x.AssigneeId.Value).Distinct().ToList();
                List<long> addedTechList = newdTechList.Except(oldTechList).ToList();
                List<long> deletedTechList = oldTechList.Except(newdTechList).ToList();
                List<long> sameTechList = oldTechList.Intersect(newdTechList).ToList();
                List<long> rescheduledTechList = oldInactiveTechList.Intersect(addedTechList).ToList();
                if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                {
                    model.jobType = "URGENT " + jobType;
                }
                else
                {
                    model.jobType = jobType;
                }
                foreach (var items in sameTechList)
                {
                    if (jobOccurance == null && model.JobSchedulerList != null)
                    {
                        var jobScheduler2 = model.JobSchedulerList.Where(x => x.AssigneeId == items && x.IsActive).FirstOrDefault();
                        if (jobScheduler2 == null)
                        {
                            continue;
                        }
                        model.StartDate = jobScheduler2.ActualStartDateString;
                        model.EndDate = jobScheduler2.ActualEndDateString;
                    }
                    var jobScheduler = jobOccurance != null ? jobOccurance.ToList().Where(x => x.AssigneeId == items).FirstOrDefault() : null;
                    var userId = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                    var isLocked = _userLoginRepository.Table.Where(x => x.Id == userId).Select(x => x.IsLocked).FirstOrDefault();
                    if (!isLocked)
                    {
                        if (jobScheduler != null)
                        {
                            model.ActualStartDateString = jobScheduler.ActualStartDateString;
                            model.ActualEndDateString = jobScheduler.ActualEndDateString;
                        }
                        SendingUpdationMails(model, items);
                    }
                }
            }
            var isTimeSpanUpdated = IsUpdatedForTimeOrAssigneeId(oldSchedulerModel, model.JobId); ;
            if (isTimeSpanUpdated && !isNewJob)
                if (!isFromPast)
                    SendUpdationMailToCustomer(schedulerIdForMail);
        }
        private void SaveJobEstimateService(long schedulerId, List<long?> invoiceNumbers, long jobId, long scheduleJobId)
        {
            _estimateInvoiceServices.AddInvoiceToEstimate(schedulerId, true, invoiceNumbers, jobId, scheduleJobId);
        }
        private void CheckingForBeforeAndAfterImages(long? estimateId, long? jobId, long? schedulerId, long? loggedInUserId, long? marketingTypeId)
        {
            var jobEstimateImageCategoryDomain = _jobEstimateImageCategory.Table.Where(x => x.EstimateId == estimateId).FirstOrDefault();
            if (jobEstimateImageCategoryDomain != default(JobEstimateImageCategory))
            {
                var jobEstimateImageCategory = CreateImageModel(estimateId, jobId, schedulerId, marketingTypeId);
                var jobEstimateImageCategoryId = jobEstimateImageCategoryDomain.Id;

                _jobEstimateImageCategory.Save(jobEstimateImageCategory);
                _unitOfWork.SaveChanges();
                SavingJobEstimateServices(jobEstimateImageCategory, jobEstimateImageCategoryId, loggedInUserId);
            }
        }
        private void SavingJobEstimateServices(JobEstimateImageCategory jobEstimateImageCategoryDomain, long? jobEstimateImageCategoryId, long? loggedInUserId)
        {
            var jobEstimateServices = _jobEstimateServices.Table.Where(x => x.CategoryId == jobEstimateImageCategoryId).ToList();
            var paidId = default(long?);
            var jobEstimateServiceDomain = new JobEstimateServices();
            foreach (var jobEstimateService in jobEstimateServices.OrderBy(x => x.Id))
            {

                var serviceId = jobEstimateService.Id;

                if (jobEstimateService.PairId == null)
                {

                    jobEstimateServiceDomain = CreateServiceModel(jobEstimateService, loggedInUserId, jobEstimateImageCategoryDomain.Id);

                    _jobEstimateServices.Save(jobEstimateServiceDomain);
                    if (jobEstimateServiceDomain.TypeId == ((long?)BeforeAfterImagesType.Before))
                        paidId = jobEstimateServiceDomain.Id;
                }
                else
                {
                    jobEstimateServiceDomain = CreateServiceModel(jobEstimateService, loggedInUserId, jobEstimateImageCategoryDomain.Id);
                    if (jobEstimateServiceDomain.TypeId == ((long?)BeforeAfterImagesType.After))
                        jobEstimateServiceDomain.PairId = paidId;

                    _jobEstimateServices.Save(jobEstimateServiceDomain);
                    _unitOfWork.SaveChanges();
                }
                if (jobEstimateServiceDomain != default(JobEstimateServices))
                    SavingJobEstimateImages(jobEstimateServiceDomain, serviceId, loggedInUserId);

            }
        }
        private JobEstimateImageCategory CreateImageModel(long? estimateId, long? jobid, long? schedulerId, long? marketingTypeId)
        {
            var JobEstimateServicesDomain = new JobEstimateImageCategory()
            {
                JobId = jobid,
                EstimateId = estimateId,
                SchedulerId = schedulerId,
                MarkertingClassId = marketingTypeId,
                IsNew = true

            };
            return JobEstimateServicesDomain;
        }
        private JobEstimateServices CreateServiceModel(JobEstimateServices jobEstimateService, long? loggedInUserId, long? categoryId)
        {
            var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = loggedInUserId, IsNew = true };
            _dataRecorderMetaDataRepository.Save(dataRecorderMetaData);
            var JobEstimateServicesDomain = new JobEstimateServices()
            {
                BuildingLocation = jobEstimateService.BuildingLocation,
                DataRecorderMetaData = dataRecorderMetaData,
                IsBeforeImage = jobEstimateService.IsBeforeImage,
                CompanyName = jobEstimateService.CompanyName,
                IsNew = true,
                FinishMaterial = jobEstimateService.FinishMaterial,
                FloorNumber = jobEstimateService.FloorNumber,
                MaidService = jobEstimateService.MaidService,
                PropertyManager = jobEstimateService.PropertyManager,
                MAIDJANITORIAL = jobEstimateService.MAIDJANITORIAL,
                SurfaceType = jobEstimateService.SurfaceType,
                SurfaceColor = jobEstimateService.SurfaceColor,
                SurfaceMaterial = jobEstimateService.SurfaceMaterial,
                CategoryId = categoryId,
                TypeId = jobEstimateService.TypeId,
                ServiceType = jobEstimateService.ServiceType,
                PairId = jobEstimateService.PairId,
                ServiceTypeId = jobEstimateService.ServiceTypeId,
                Lookup = jobEstimateService.Lookup,
                Id = 0
            };
            return JobEstimateServicesDomain;
        }
        private void SavingJobEstimateImages(JobEstimateServices jobEstimateServicesDomain, long? jobEstimateImageServiceId, long? loggedInUserId)
        {
            var jobEstimateServicesImages = _jobEstimateImage.Table.Where(x => x.ServiceId == jobEstimateImageServiceId).ToList();
            foreach (var jobEstimateServicesImage in jobEstimateServicesImages)
            {
                jobEstimateServicesImage.ServiceId = jobEstimateServicesDomain.Id;
                jobEstimateServicesImage.DataRecorderMetaDataId = jobEstimateServicesImage.DataRecorderMetaDataId;
                jobEstimateServicesImage.DataRecorderMetaData = jobEstimateServicesImage.DataRecorderMetaData;
                jobEstimateServicesImage.IsNew = true;
                _jobEstimateImage.Save(jobEstimateServicesImage);
                _unitOfWork.SaveChanges();
            }
        }
        private void SendingUpdationMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            _sendNewJobNotificationToTechService.SendJobNotificationtoTechForUpdation(model, listSchedule);
        }
        private void SendingCancellationMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelled(model, listSchedule);
        }

        private void SendingRescheduledMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            _sendNewJobNotificationToTechService.SendJobNotificationtoTechForRescheduled(model, listSchedule);
        }

        private void SendingUrgentMails(JobEditModel model, long items)
        {

            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            model.jobTypeName = "Job";
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.ActualStartDateString.Date;
            var endDateToBeCompared = model.ActualStartDateString.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForUrgent(model, listSchedule);
        }
        private void SendingNewsMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            model.jobTypeName = "Job";
            _sendNewJobNotificationToTechService.SendJobNotificationtoTech(model, listSchedule);

        }
        private void SaveScheduleInfo(JobEditModel model)
        {
            var schedulerId = default(long?);
            var isParsed = false;
            foreach (var item in model.JobOccurence.Collection)
            {
                item.Title = model.Title;
                item.ServiceTypeId = model.JobTypeId;
                item.FranchiseeId = model.FranchiseeId;
                item.ParentJobId = model.JobId;
                item.FranchiseeId = model.FranchiseeId;
                var orgroleUser = _organizationRoleUserRepository.Get(item.AssigneeId);
                var domain = _jobFactory.CreateDomain(item);
                domain.PersonId = orgroleUser != null ? orgroleUser.UserId : default(long?);
                if (model.SalesRepId != null)
                {
                    domain.SalesRepId = model.SalesRepId.GetValueOrDefault();
                }
                if (item.DataRecorderMetaData.CreatedBy == null)
                {
                    domain.DataRecorderMetaData = model.DataRecorderMetaData;
                    domain.DataRecorderMetaDataId = model.DataRecorderMetaData.Id;
                }
                if (model.Id <= 0 || model.Id == null)
                {
                    domain.IsCustomerAvailable = true;
                    domain.IsRepeat = true;
                }

                domain.ParentJobId = schedulerId;
                domain.IsCancellationMailSend = true;
                domain.IsJobConverted = true;
                _jobSchedulerRepository.Save(domain);
                item.ScheduleId = domain.Id;
                if ((model.Id <= 0 || model.Id == null) && !isParsed)
                {
                    isParsed = true;
                    schedulerId = domain.Id;
                }
            }
        }

        private EstimateInvoiceAssignee CreateAssigneeDomain(JobOccurenceEditModel model, InvoiceNumbersEditModel invoice)
        {
            var domain = new EstimateInvoiceAssignee()
            {
                AssigneeId = model.AssigneeId,
                InvoiceNumber = invoice.Id,
                Label = invoice.Label
            };
            return domain;
        }
        private void SaveAssigneeInfo(JobOccurenceEditModel model, long schedulerIdForJob, long jobId, long estimateId)
        {
            model.ParentJobId = jobId;
            var assignedInvoices = _estimateInvoiceAssignee.Table.Where(x => x.SchedulerId == schedulerIdForJob && x.EstimateId == estimateId).ToList();
            foreach (var invoice in assignedInvoices)
            {
                _estimateInvoiceAssignee.Delete(invoice);
            }
            if (model.InvoiceNumber != null)
            {
                foreach (var invoice in model.InvoiceNumber)
                {
                    var domain = CreateAssigneeDomain(model, invoice);
                    domain.EstimateId = estimateId;
                    var jobEstimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.EstimateId == domain.EstimateId);
                    domain.EstimateInvoiceId = jobEstimateInvoice.Id;
                    domain.SchedulerId = schedulerIdForJob;
                    domain.IsNew = true;
                    _estimateInvoiceAssignee.Save(domain);
                }
            }
        }
        private EstimateInvoiceAssignee CreateAssigneeDomainForEditJob(JobSchedulerEditModel model, InvoiceNumbersEditModel invoice)
        {
            var domain = new EstimateInvoiceAssignee()
            {
                AssigneeId = model.AssigneeId,
                InvoiceNumber = invoice.Id,
                Label = invoice.Label
            };
            return domain;
        }

        private void SaveAssigneeInfoForEditJob(JobSchedulerEditModel model, long schedulerIdForJob, long jobId, long estimateId)
        {
            //model.ParentJobId = jobId;
            var assignedInvoices = _estimateInvoiceAssignee.Table.Where(x => x.SchedulerId == schedulerIdForJob && x.EstimateId == estimateId).ToList();
            foreach (var invoice in assignedInvoices)
            {
                _estimateInvoiceAssignee.Delete(invoice);
            }
            if (model.InvoiceNumber != null)
            {
                foreach (var invoice in model.InvoiceNumber)
                {
                    var domain = CreateAssigneeDomainForEditJob(model, invoice);
                    domain.EstimateId = estimateId;
                    var jobEstimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.EstimateId == domain.EstimateId);
                    domain.EstimateInvoiceId = jobEstimateInvoice.Id;
                    domain.SchedulerId = schedulerIdForJob;
                    domain.IsNew = true;
                    _estimateInvoiceAssignee.Save(domain);
                }
            }
            else
            {
                var estimateInvoice = _estimateInvoiceRepository.Table.Where(x => x.EstimateId == estimateId).FirstOrDefault();
                var invoiceServices = estimateInvoice != null ? _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).ToList() : new List<EstimateInvoiceService>();
                var invoiceNumber = invoiceServices.Select(x => x.InvoiceNumber).Distinct().ToList();
                foreach (var invoice in invoiceNumber)
                {
                    var service = invoiceServices.Where(x => x.InvoiceNumber == invoice).FirstOrDefault();
                    var domain = new EstimateInvoiceAssignee();
                    domain.AssigneeId = model.AssigneeId;
                    domain.InvoiceNumber = invoice;
                    domain.Label = GetFileNameName(estimateInvoice.EstimateInvoiceCustomer.CustomerName, service);
                    domain.EstimateId = estimateId;
                    var jobEstimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.EstimateId == domain.EstimateId);
                    domain.EstimateInvoiceId = jobEstimateInvoice.Id;
                    domain.SchedulerId = schedulerIdForJob;
                    domain.IsNew = true;
                    _estimateInvoiceAssignee.Save(domain);
                }
            }
        }

        private string GetFileNameName(string customerName, EstimateInvoiceService invoiceService)
        {
            List<string> fileNameList = new List<string>();
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
                fileName = fileName + "_" + "InternalConcreteOrder";
            }
            else
            {
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


        //private IEnumerable<JobResourceEditModel> ManageMediaFiles(JobEditModel model)
        //{
        //    var mediaFiles = model.Resource;
        //    return mediaFiles;
        //}

        private IEnumerable<JobSchedulerEditModel> ManageJobAssignee(JobEditModel model, IEnumerable<JobSchedulerEditModel> list)
        {
            var techList = list != null ? list.ToList() : new List<JobSchedulerEditModel>();
            var techIds = model.TechIds.ToList();
            var inDbAssignee = model.JobSchedulerList != null ? model.JobSchedulerList.ToList() : null;
            if (inDbAssignee == null)
            {
                foreach (var item in model.TechIds)
                {
                    var schedulerModel = CreateSchedulerModel(model, item, false, 0); ;
                    techList.Add(schedulerModel);
                }
            }
            else
            {
                var inDbIds = model.JobAssigneeIds;
                var assigneeToDeactivate = inDbIds.Where(x => !model.TechIds.Contains(x));
                foreach (var item in assigneeToDeactivate)
                {
                    var assignee = techList.Where(x => x.AssigneeId == item).FirstOrDefault();
                    if (assignee != null)
                    {
                        assignee.IsActive = false;
                    }
                }

                foreach (var item in techIds)
                {
                    var assignee = techList.Where(x => x.AssigneeId == item).FirstOrDefault();
                    if (assignee == null)
                    {
                        var newAssignee = CreateSchedulerModel(model, item, true, 0);
                        techList.Add(newAssignee);
                    }
                    else
                    {
                        if (!assignee.IsActive)
                            assignee.IsActive = true;
                    }

                }
            }
            return techList;
        }
        private JobSchedulerEditModel CreateSchedulerModelForReactivation(JobScheduler model, long techId, long id = 0)
        {
            var schedulerModel = new JobSchedulerEditModel
            {
                AssigneeId = techId,
                JobId = model.JobId,
                IsActive = true,
                Title = model.Title,
                FranchiseeId = model.FranchiseeId,
                Id = id,
                SalesRepId = model.SalesRepId > 0 ? model.SalesRepId : null,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DataRecorderMetaData = model.DataRecorderMetaData,
            };
            return schedulerModel;
        }
        private JobSchedulerEditModel CreateSchedulerModel(JobEditModel model, long techId, bool isFromIds, long id = 0)
        {
            var schedulerModel = new JobSchedulerEditModel
            {
                AssigneeId = techId,
                JobId = model.JobId,
                IsActive = true,
                Title = model.Title,
                FranchiseeId = model.FranchiseeId,
                Id = id,
                SalesRepId = model.SalesRepId > 0 ? model.SalesRepId : null,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                DataRecorderMetaData = model.DataRecorderMetaData,
                ActualEndDateString = model.ActualEndDateString,
                ActualStartDateString = model.ActualStartDateString,
                IsFromId = isFromIds
            };
            return schedulerModel;
        }

        public bool SaveMediaFiles(FileUploadModel model)
        {
            foreach (var fileModel in model.FileList)
            {
                try
                {
                    if (fileModel.Id > 0)
                        continue;
                    var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
                    var destination = MediaLocationHelper.GetJobMediaLocation();
                    var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                        : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
                    var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
                    fileModel.Name = destFileName + fileModel.Extension;
                    fileModel.RelativeLocation = MediaLocationHelper.GetJobMediaLocation().Path;
                    var file = _fileService.SaveModel(fileModel);

                    var jobResource = _jobFactory.CreateDomain(model, file.Id);
                    _jobResourceRepository.Save(jobResource);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return true;
        }

        public long SaveMediaFilesForUsers(FileUploadModel model)
        {
            foreach (var fileModel in model.FileList)
            {
                try
                {
                    if (fileModel.Id > 0)
                        continue;
                    var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
                    var destination = MediaLocationHelper.GetTempImageLocation();
                    var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                        : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
                    var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
                    fileModel.Name = destFileName + fileModel.Extension;
                    fileModel.RelativeLocation = MediaLocationHelper.GetJobMediaLocation().Path;
                    var file = _fileService.SaveModel(fileModel);
                    return file.Id;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return default(long);
        }

        public bool ChangeStatus(long jobId, long statusId)
        {
            if (jobId <= 0 || statusId <= 0)
                return false;
            var job = _jobRepository.Get(jobId);

            if (statusId >= (long)JobStatusType.Assigned && !job.JobScheduler.Any())
                return false;

            if (job != null)
            {
                job.StatusId = statusId;
                _jobRepository.Save(job);
                return true;
            }
            return false;
        }

        public JobEditModel Get(long jobSchedulerIdId)
        {

            if (jobSchedulerIdId == 0)
                return new JobEditModel { };

            else
            {
                var jobSchedulerDomain = _jobschedulerRepository.Get(jobSchedulerIdId);
                var jobId = jobSchedulerDomain.JobId.GetValueOrDefault();
                var job = _jobRepository.Get(jobId);
                var estimateInvoiceAssigneeDomain = _estimateInvoiceAssignee.Table.Where(x => x.EstimateId == job.EstimateId).ToList();
                if (job == null)
                    return new JobEditModel { };
                var model = _jobFactory.CreateEditModel(job, jobSchedulerDomain, estimateInvoiceAssigneeDomain);
                model.Worth = jobSchedulerDomain.EstimateWorth;
                model.IsHavingMoreThanOneDay = model.JobSchedulerList.Count() > 1 ? true : false;
                var jobDetail = _jobDetailsRepository.Table.FirstOrDefault(x => x.SchedulerId == jobSchedulerIdId);
                model.LessDeposit = jobSchedulerDomain != null ? jobSchedulerDomain.Franchisee.LessDeposit : 50;
                if (jobDetail != null)
                {
                    model.Description = jobDetail.Description;
                }

                var estimateScheduler = _jobschedulerRepository.Table.FirstOrDefault(x => x.EstimateId == job.EstimateId);
                var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == estimateScheduler.Id);

                if (estimateInvoice != null)
                {
                    var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id).OrderByDescending(x => x.Id).ToList();
                    var customerSignaturePre = customerSignature.Where(x => x.TypeId == (long)SignatureType.PRECOMPLETION).GroupBy(x => x.Signature).ToList();
                    var customerSignaturePost = customerSignature.Where(x => x.TypeId == (long)SignatureType.POSTCOMPLETION).GroupBy(x => x.Signature).ToList();
                    model.JobSignaturePre = new List<JobEstimateSignatureEditModel>();
                    model.JobSignaturePost = new List<JobEstimateSignatureEditModel>();
                    model.AllInvoiceNumbersSignedPre = new List<long?>();
                    foreach (var signature in customerSignaturePre)
                    {
                        JobEstimateSignatureEditModel jobSignaturePre = new JobEstimateSignatureEditModel();
                        jobSignaturePre.IsSigned = customerSignature != null && signature.Key != null ? true : false;
                        jobSignaturePre.Signature = customerSignature != null && signature.Key != null ? signature.Key : "";
                        jobSignaturePre.InvoiceNumber = customerSignature != null && signature.Key != null ? signature.Select(x => x.InvoiceNumber).ToList() : new List<long?>();
                        jobSignaturePre.TypeId = customerSignature != null && signature != null ? signature.Select(x => x.TypeId).ToList() : new List<long?>();
                        model.AllInvoiceNumbersSignedPre.AddRange(jobSignaturePre.InvoiceNumber);
                        model.JobSignaturePre.Add(jobSignaturePre);
                    }

                    model.AllInvoiceNumbersSignedPost = new List<long?>();
                    foreach (var signature in customerSignaturePost)
                    {
                        JobEstimateSignatureEditModel jobSignaturePost = new JobEstimateSignatureEditModel();
                        jobSignaturePost.IsSigned = customerSignature != null && signature.Key != null ? true : false;
                        jobSignaturePost.Signature = customerSignature != null && signature.Key != null ? signature.Key : "";
                        jobSignaturePost.InvoiceNumber = customerSignature != null && signature.Key != null ? signature.Select(x => x.InvoiceNumber).ToList() : new List<long?>();
                        jobSignaturePost.TypeId = customerSignature != null && signature != null ? signature.Select(x => x.TypeId).ToList() : new List<long?>();
                        jobSignaturePost.SchedulerId = signature != null ? signature.Select(x => x.JobSchedulerId).FirstOrDefault() : default(long?);
                        model.AllInvoiceNumbersSignedPost.AddRange(jobSignaturePost.InvoiceNumber);
                        model.JobSignaturePost.Add(jobSignaturePost);
                    }
                    model.JobSignaturePost = model.JobSignaturePost.Where(x => x.SchedulerId == jobSchedulerDomain.Id).ToList();
                    if (model.AllInvoiceNumbersSignedPre.Count > 0)
                    {
                        model.IsSigned = true;
                    }
                    model.AllInvoiceNumbersSignedForEstimate = new List<long?>();
                    var schedulerForJob = model.JobSchedulerList.FirstOrDefault(x => x.Id == jobSchedulerIdId);
                    var customerSignatureForJob = customerSignature.Where(x => x.EstimateInvoiceId == estimateInvoice.Id && x.TypeId == (long)SignatureType.PRECOMPLETION && schedulerForJob.InvoiceNumbers.Contains((long)x.InvoiceNumber)).OrderByDescending(x => x.Id).GroupBy(x => x.Signature).ToList();

                    foreach (var signature in customerSignatureForJob)
                    {
                        var InvoiceNumber = customerSignature != null && signature.Key != null ? signature.Select(x => x.InvoiceNumber).ToList() : new List<long?>();
                        model.AllInvoiceNumbersSignedForEstimate.AddRange(InvoiceNumber);
                        model.Assignee = estimateScheduler.Person.FirstName + " " + estimateScheduler.Person.LastName;
                    }

                    var emailTemplate = _emailTemplateRepository.Table.Where(x => x.NotificationTypeId == ((long?)NotificationTypes.PostJobFeedbackToCustomer) || x.NotificationTypeId == ((long?)NotificationTypes.PostJobFeedbackToAdmin)).ToList();
                    model.MailBody = emailTemplate.FirstOrDefault(x => x.NotificationTypeId == (long?)NotificationTypes.PostJobFeedbackToCustomer).Body;
                    model.MailBodyToAdmin = emailTemplate.FirstOrDefault(x => x.NotificationTypeId == (long?)NotificationTypes.PostJobFeedbackToCustomer).Body;
                    model.IsCustomerAvailable = jobSchedulerDomain.IsCustomerAvailable;
                }
                return model;
            }
        }

        public bool IsValidQbNumber(string qbInvoice)
        {
            if (qbInvoice == null) return true;
            var result = _franchiseeSalesRepository.Table.Where(x => x.QbInvoiceNumber.ToLower().Equals(qbInvoice.ToLower())).FirstOrDefault();
            if (result != null)
                return true;
            return false;
        }

        public bool UpdateInfo(long schedulerId, string qbInvoiceNumber, long? userId)
        {
            var jobSchedulerDomain = _jobschedulerRepository.IncludeMultiple(y => y.Job.JobCustomer).Where(x => x.Id == schedulerId).FirstOrDefault();

            if (jobSchedulerDomain != default(JobScheduler))
            {
                var jobId = jobSchedulerDomain.JobId.GetValueOrDefault();
                var job = _jobRepository.Get(jobId);
                if (job == null)
                    return false;

                job.QBInvoiceNumber = (qbInvoiceNumber != null || qbInvoiceNumber != "") ? qbInvoiceNumber : null;
                _jobRepository.Save(job);

                jobSchedulerDomain.QBInvoiceNumber = qbInvoiceNumber;
                jobSchedulerDomain.IsCancellationMailSend = true;
                _jobschedulerRepository.Save(jobSchedulerDomain);

                var franchiseeInfo = _jobschedulerRepository.Table.Where(x => x.JobId == job.Id && x.Id == schedulerId).FirstOrDefault();
                if (franchiseeInfo != null)
                {
                    var franchiseeSales = _franchiseeSalesRepository.Table.FirstOrDefault(x => x.QbInvoiceNumber == qbInvoiceNumber && x.FranchiseeId == franchiseeInfo.FranchiseeId);

                    if (franchiseeSales != null && job.StatusId == (long?)JobStatusEnum.Completed)
                    {
                        SaveFileForImageAttachment(franchiseeSales.InvoiceId, franchiseeSales, job, userId, false, schedulerId);
                    }
                    //else
                    //{
                    //    var categoryId = _jobEstimateImageCategory.Table.Where(x => x.JobId == job.Id).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
                    //    var jobEstimateServices = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long?)LookupTypes.InvoiceImages).FirstOrDefault();
                    //    if (jobEstimateServices != null)
                    //        _jobEstimateServices.Delete(jobEstimateServices);
                    //}
                }
            }
            return true;
        }
        public bool SaveFileForImageAttachment(long? invoiceId, FranchiseeSales franchiseeSales, Job job, long? userId, bool isFromJob, long? schedulerId)
        {
            var invoice = _invoiceService.InvoiceDetails(invoiceId.GetValueOrDefault());
            var invoiceDetailsForAttactmentViewModel = CreateViewModel(invoice);
            var fileId = AttachInvoiceWithJob(invoiceDetailsForAttactmentViewModel, "invoice-job-attacktment.cshtml", franchiseeSales.FranchiseeId, isFromJob);
            if (fileId != null)
            {
                var categoryId = _jobEstimateImageCategory.Table.Where(x => x.SchedulerId == schedulerId).OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();
                if (categoryId == 0)
                {

                    var jobEstimateCategoryModel = new JobEstimateCategoryViewModel()
                    {
                        Id = 0,
                        JobId = job.Id,
                        EstimateId = null,
                        MarketingClassId = franchiseeSales.ClassTypeId,
                        SchedulerId = schedulerId
                    };
                    var jobEstimateCategory = _jobFactory.CreateJobEstimateCategory(jobEstimateCategoryModel);
                    _jobEstimateImageCategory.Save(jobEstimateCategory);
                    categoryId = jobEstimateCategory.Id;
                    SaveAttachInvoiceWithJob(fileId, categoryId, userId, false);
                    return true;
                }
                SaveAttachInvoiceWithJob(fileId, categoryId, userId, true);
                return true;
            }
            return true;
        }
        public bool SaveAttachInvoiceWithJob(long? fileId, long categoryId, long? userId, bool isPresent)
        {
            try
            {
                var serviceId = default(long?);
                if (!isPresent)
                {
                    var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = userId, IsNew = true };
                    _dataRecorderMetaDataRepository.Save(dataRecorderMetaData);
                    var JobEstimateServiceViewModel = new JobEstimateServiceViewModel { DataRecorderMetaData = dataRecorderMetaData, Id = 0 };
                    try
                    {
                        var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, categoryId, (long?)LookupTypes.InvoiceImages);
                        _jobEstimateServices.Save(jobEstimateBeforeService);
                        var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                        var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(JobEstimateServiceViewModel, buildingExteriorServiceId, (long?)LookupTypes.InvoiceImages, fileId);
                        _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                        return true;
                    }
                    catch (Exception e1)
                    {
                        return false;
                    }
                }
                else
                {
                    var jobEstimateServices = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long?)LookupTypes.InvoiceImages).FirstOrDefault();
                    if (jobEstimateServices == null)
                    {
                        var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, categoryId, (long?)LookupTypes.InvoiceImages);
                        _jobEstimateServices.Save(jobEstimateBeforeService);
                        serviceId = jobEstimateBeforeService.Id;
                    }
                    else
                    {
                        serviceId = jobEstimateServices.Id;
                    }

                    var jobEstimateBeforeServiceImage = _jobEstimateImage.Table.Where(x => x.ServiceId == serviceId).FirstOrDefault();
                    if (jobEstimateBeforeServiceImage != null)
                    {
                        jobEstimateBeforeServiceImage.FileId = fileId;
                        jobEstimateBeforeServiceImage.IsNew = false;
                        _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                    }
                    else
                    {
                        var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = userId, IsNew = true };
                        _dataRecorderMetaDataRepository.Save(dataRecorderMetaData);
                        var jobEstimateImage = new JobEstimateImage { ServiceId = serviceId, FileId = fileId, IsNew = true, DataRecorderMetaData = dataRecorderMetaData, TypeId = (long?)LookupTypes.InvoiceImages };
                        _jobEstimateImage.Save(jobEstimateImage);
                    }
                }
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }
        private InvoiceDetailsForAttactmentViewModel CreateViewModel(InvoiceDetailsViewModel invoiceViewModel)
        {
            string emailId = "";
            string franchiseePhone = "";
            foreach (var email in invoiceViewModel.CustomerEmails)
            {
                emailId = email.email + " ,";
            }
            foreach (var phone in invoiceViewModel.PhoneNumbers)
            {
                franchiseePhone = phone.PhoneNumber + " ,";
            }
            return new InvoiceDetailsForAttactmentViewModel()
            {
                Address = invoiceViewModel.Address != null ? invoiceViewModel.Address.AddressLine1 + " , " + invoiceViewModel.Address.AddressLine2 + " , " +
                                                        invoiceViewModel.Address.Country + " ," + invoiceViewModel.Address.State + " , " + invoiceViewModel.Address.City + " , " + invoiceViewModel.Address.ZipCode : "",
                Payments = invoiceViewModel.Payments,
                InvoiceItems = invoiceViewModel.InvoiceItems,
                AnnualUploadId = invoiceViewModel.AnnualUploadId,
                AuditInvoiceId = invoiceViewModel.AuditInvoiceId,
                ContactPerson = invoiceViewModel.ContactPerson,
                CurrencyCode = invoiceViewModel.CurrencyCode,
                CurrentLoanAmount = invoiceViewModel.CurrentLoanAmount,
                Customer = invoiceViewModel.Customer,
                CustomerEmails = emailId,
                DueDate = invoiceViewModel.DueDate.ToShortDateString(),
                Email = invoiceViewModel.Email,
                FranchiseeAddress = invoiceViewModel.FranchiseeAddress != null ? invoiceViewModel.FranchiseeAddress.AddressLine1 + " , " + invoiceViewModel.FranchiseeAddress.AddressLine2 + " , " +
                                                        invoiceViewModel.FranchiseeAddress.Country + " ," + invoiceViewModel.FranchiseeAddress.State + " , " + invoiceViewModel.FranchiseeAddress.City + " , " + invoiceViewModel.FranchiseeAddress.ZipCode : "",
                FranchiseeName = invoiceViewModel.FranchiseeName,
                GeneratedOn = invoiceViewModel.GeneratedOn.ToShortDateString(),
                GrandTotal = invoiceViewModel.GrandTotal,
                InvoiceDate = invoiceViewModel.InvoiceDate.HasValue ? invoiceViewModel.InvoiceDate.Value.ToShortDateString() : "",
                InvoiceId = invoiceViewModel.InvoiceId,
                LoanAmount = invoiceViewModel.LoanAmount,
                PhoneNumber = invoiceViewModel.PhoneNumber,
                QBInvoiceNumber = invoiceViewModel.QBInvoiceNumber,
                RemainingLoanAmount = invoiceViewModel.RemainingLoanAmount,
                SalesAmount = invoiceViewModel.SalesAmount,
                TotalPayment = invoiceViewModel.TotalPayment,
                UploadEndDate = invoiceViewModel.UploadEndDate != null ? invoiceViewModel.UploadEndDate.ToShortDateString() : "",
                ReportName = invoiceViewModel.ReportName,
                TotalAmount = invoiceViewModel.TotalAmount,
                StatusId = invoiceViewModel.StatusId,
                ReportId = invoiceViewModel.ReportId,
                PhoneNumbers = franchiseePhone,
                FranchiseePhoneNumbers = invoiceViewModel.FranchiseePhone
            };
        }

        public bool Delete(long jobId)
        {
            if (jobId > 0)
            {
                var job = _jobRepository.Get(jobId);
                var estimate = job.JobEstimate;

                var jobSchedulers = _jobSchedulerRepository.Table.Where(x => x.JobId == jobId && x.IsActive).ToList();
                foreach (var scheduler in jobSchedulers)
                {
                    //var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == scheduler)).FirstOrDefault();
                    //model.jobTypeName = jobType;
                    _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelledForDeleteButton(scheduler, true);
                    _jobSchedulerRepository.Delete(scheduler.Id);
                }
                if (estimate == null)
                {
                    _jobCustomerRepository.Delete(x => x.Id == job.CustomerId);
                }
                _jobRepository.Delete(job);
                return true;
            }
            return false;
        }

        public bool CheckAvailability(long id, long assigneeId, DateTime startDate, DateTime endDate, bool isVacation)
        {
            var orgRoleUser = _organizationRoleUserRepository.Get(assigneeId);
            var startDateToCompare = startDate.AddMinutes(-14);
            var assigneeInfo = new List<JobScheduler>();
            if (isVacation)
            {
                assigneeInfo = _jobSchedulerRepository.Table.Where(x => assigneeId == x.AssigneeId && (id <= 0 || x.Id != id)
                                     && x.IsActive).ToList();
            }
            else
            {
                //assigneeInfo = _jobSchedulerRepository.Table.Where(x => assigneeId == x.AssigneeId && (id <= 0 || (x.JobId != id && x.EstimateId != id && x.MeetingID != id))
                //                    && x.IsActive).ToList();
                if (orgRoleUser != null)
                {
                    if (id == 0)
                    {
                        var assigneeInfo1 = _jobSchedulerRepository.Table.AsEnumerable().Where(x => (x.PersonId == orgRoleUser.UserId) && x.IsActive && (x.ActualStartDate >= startDateToCompare || x.ActualEndDate <= endDate));
                        assigneeInfo = assigneeInfo1.ToList();
                    }
                    else
                    {
                        assigneeInfo = _jobSchedulerRepository.Table.Where(x => (x.PersonId == orgRoleUser.UserId) && (id <= 0 || (x.JobId != id && x.EstimateId != id && x.MeetingID != id))
                                       && x.IsActive).ToList();
                    }
                }
                else
                {
                    assigneeInfo = _jobSchedulerRepository.Table.Where(x => assigneeId == x.AssigneeId && (id <= 0 || (x.JobId != id && x.EstimateId != id && x.MeetingID != id))
                                        && x.IsActive).ToList();
                }
            }

            if (!assigneeInfo.Any())
                return true;

            assigneeInfo = assigneeInfo.Where(x => (startDateToCompare <= x.StartDateTimeString && endDate >= x.StartDateTimeString)
                        || (startDateToCompare <= x.EndDateTimeString && endDate >= x.EndDateTimeString)
                        || (startDateToCompare >= x.StartDateTimeString && endDate <= x.EndDateTimeString)).ToList();

            if (!assigneeInfo.Any())
                return true;
            return false;
        }

        public bool CheckAvailabilityForJob(long id, long assigneeId, DateTime startDate, DateTime endDate, bool isVacation)
        {
            var orgnRoleUser = _organizationRoleUserRepository.Get(assigneeId);

            var startDateToCompare = startDate.AddMinutes(-14);
            var assigneeInfo = new List<JobScheduler>();

            assigneeInfo = _jobSchedulerRepository.Table.Where(x => orgnRoleUser.UserId == x.PersonId && (id <= 0 || (x.JobId != id && x.EstimateId != id && x.MeetingID != id))
                                && x.IsActive).ToList();

            if (!assigneeInfo.Any())
                return true;

            assigneeInfo = assigneeInfo.Where(x => (startDateToCompare <= x.StartDate && endDate >= x.StartDate)
                        || (startDateToCompare <= x.EndDate && endDate >= x.EndDate)
                        || (startDateToCompare >= x.StartDate && endDate <= x.EndDate)).ToList();

            if (!assigneeInfo.Any())
                return true;
            return false;
        }

        public bool CheckAvailabilityForJobForSalesRep(long id, long assigneeId, DateTime startDate, DateTime endDate, bool isVacation)
        {
            var orgnRoleUser = _organizationRoleUserRepository.Get(assigneeId);

            var startDateToCompare = startDate.AddMinutes(-14);
            var assigneeInfo = new List<JobScheduler>();

            assigneeInfo = _jobSchedulerRepository.Table.Where(x => orgnRoleUser.UserId == x.PersonId && (id <= 0 || (x.JobId != id && x.EstimateId != id && x.MeetingID != id))
                                && x.IsActive).ToList();

            if (!assigneeInfo.Any())
                return true;

            assigneeInfo = assigneeInfo.Where(x => (startDateToCompare <= x.StartDate && endDate >= x.StartDate)
                        || (startDateToCompare <= x.EndDate && endDate >= x.EndDate)
                        || (startDateToCompare >= x.StartDate && endDate <= x.EndDate)).ToList();

            if (!assigneeInfo.Any())
                return true;
            return false;
        }
        public FileUploadModel GetMediaList(long rowId, long mediaType, long? estimateId, long? userId)
        {
            var jobSchedulerDomain = default(JobScheduler);
            var id = default(long?);
            var model = new FileUploadModel { };
            model.Resources = new List<JobResourceEditModel>();
            var mediaList = new List<JobResource>();
            var list = new JobEstimateCategoryViewModel();
            var imageClassViewModel = new JobEstimateImageCategory();
            var jobEstimateServices = new List<JobEstimateServices>();
            var imageClassViewModels = new List<JobEstimateServiceViewModel>();
            var schedulerId = default(long?);

            jobSchedulerDomain = _jobschedulerRepository.Table.Where(x => x.Id == rowId).FirstOrDefault();
            if (jobSchedulerDomain == default(JobScheduler))
            {
                jobSchedulerDomain = _jobschedulerRepository.Table.Where(x => x.Id == rowId).FirstOrDefault();
            }
            if (jobSchedulerDomain != default(JobScheduler))
            {
                if (mediaType == (long)ScheduleType.Job)
                {
                    id = jobSchedulerDomain.JobId;
                }
                else if (mediaType == (long)ScheduleType.Estimate)
                {
                    id = jobSchedulerDomain.EstimateId;
                }
                else if (mediaType == (long)ScheduleType.Vacation)
                {
                    id = jobSchedulerDomain.Id;
                }
                else if (mediaType == (long)ScheduleType.Meeting)
                {
                    id = jobSchedulerDomain.MeetingID;
                }
            }

            if (id <= 0)
                return new FileUploadModel { };

            if (mediaType == (long)ScheduleType.Job)
            {
                var parentEstimateId = default(long?);

                if (estimateId != null)
                {
                    var jobEstimate = _jobEstimateRepository.Table.FirstOrDefault(x => x.Id == estimateId);
                    if (jobEstimate == null)
                    {
                        parentEstimateId = jobEstimate.ParentEstimateId;
                    }
                }

                var categoryList = _jobEstimateImageCategory.Table.Where(x => (estimateId != null && x.EstimateId == estimateId && x.SchedulerId != jobschedulerId)
                || (x.SchedulerId == jobSchedulerDomain.Id) && ((parentEstimateId == null || x.EstimateId == parentEstimateId))
                || (estimateId == null && (x.JobId == jobSchedulerDomain.JobId))).Select(x => x.Id).Distinct().ToList();


                imageClassViewModel = _jobEstimateImageCategory.Table.Where(x => x.SchedulerId == rowId).FirstOrDefault();

                categoryList = categoryList.Distinct().ToList();
                var imageClassViewModelForEstimate = _jobEstimateImageCategory.Table.Where(x => x.SchedulerId != rowId
                  && x.EstimateId == jobSchedulerDomain.Job.EstimateId && x.JobId == null).Select(x => x.Id).ToList();
                schedulerId = imageClassViewModel != null ? imageClassViewModel.SchedulerId.GetValueOrDefault() : default(long?);

                if (categoryList.Count() > 0)
                {
                    jobEstimateServices = _jobEstimateServices.Table.Where(x => x.CategoryId.HasValue && categoryList.Contains(x.CategoryId.Value) && ((x.TypeId != (long)LookupTypes.InvoiceImages) && ((x.InvoiceNumber == null)))).ToList();
                    var jobEstimateServicesForInvoices = _jobEstimateServices.Table.Where(x => x.CategoryId.HasValue && categoryList.Contains(x.CategoryId.Value) && (x.TypeId == (long)LookupTypes.InvoiceImages &&
                    (x.IsFromInvoiceAttach == true && x.IsInvoiceForJob == true) && (x.JobEstimateImageCategory.SchedulerId == rowId))).ToList();
                    var jobEstimateServicesForOtherInvoiceUpload = _jobEstimateServices.Table.Where(x => x.CategoryId.HasValue && categoryList.Contains(x.CategoryId.Value) && (x.TypeId == (long)LookupTypes.InvoiceImages &&
                    (x.InvoiceNumber == null))).ToList();
                    jobEstimateServices.AddRange(jobEstimateServicesForInvoices);
                    jobEstimateServices.AddRange(jobEstimateServicesForOtherInvoiceUpload);
                }
            }
            else if (mediaType == (long)ScheduleType.Estimate)
            {
                var jobEstimate = new JobEstimate();
                if (id != null)
                {
                    jobEstimate = _jobEstimateRepository.Get(id.Value);
                }
                if (jobEstimate.ParentEstimateId == null)
                {
                    imageClassViewModel = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.EstimateId == id);
                }
                else
                {
                    imageClassViewModel = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.EstimateId == id);
                }
                schedulerId = imageClassViewModel != null ? imageClassViewModel.SchedulerId.GetValueOrDefault() : default(long?);
                if (imageClassViewModel != null && imageClassViewModel != null)
                {
                    jobEstimateServices = _jobEstimateServices.Table.Where(x => x.CategoryId == imageClassViewModel.Id && (!x.IsInvoiceForJob.Value || x.IsInvoiceForJob == null)).ToList();
                }
            }
            else if (mediaType == (long)ScheduleType.Vacation)
            {
                mediaList = _jobResourceRepository.Table.Where(x => x.VacationId == rowId).ToList();
                model.Resources = mediaList.Select(x => _jobFactory.CreateResouceModel(x)).ToList();
            }
            else if (mediaType == (long)ScheduleType.Meeting)
            {
                mediaList = _jobResourceRepository.Table.Where(x => x.MeetingId == rowId).ToList();
                model.Resources = mediaList.Select(x => _jobFactory.CreateResouceModel(x)).ToList();
            }

            var beforeDuringInvoiceSliderImages = jobEstimateServices.Where(x => (x.TypeId == (long)LookupTypes.BeforeWork) || (x.TypeId == (long)LookupTypes.ExteriorBuilding) || (x.TypeId == (long)LookupTypes.AfterWork)
                                              || (x.TypeId == (long)LookupTypes.InvoiceImages) || (x.TypeId == (long)LookupTypes.DuringWork)).ToList();

            var parentChildJobUdring = jobEstimateServices.Select(x => x.Id).ToList();

            list = JobEstimateImageModel(beforeDuringInvoiceSliderImages, parentChildJobUdring, id, mediaType, schedulerId, userId);
            model.JobId = mediaType == (long)ScheduleType.Job ? id : default(long);
            model.EstimateId = mediaType == (long)ScheduleType.Estimate ? id : default(long);

            model.ImageList = _jobFactory.CreatePairingModel(imageClassViewModel, list);
            return model;
        }

        public JobEstimateCategoryViewModel JobEstimateImageModel(List<JobEstimateServices> jobEstimateImageServices, List<long> jobEstimateServiceGrouped, long? jobEstimateId, long? mediaType, long? schedulerId, long? userId)
        {
            JobEstimateCategoryViewModel jobEstimateCategoryViewModel = new JobEstimateCategoryViewModel();

            List<ImagePairs> jobEstimateCategoryImagePairs = new List<ImagePairs>();
            List<ImagePairs> invoiceImagePairs = new List<ImagePairs>();
            var filesBeforeIds = new List<Application.Domain.File>();
            var filesAfterIds = new List<Application.Domain.File>();

            var thumbFilesBeforeIds = new List<JobEstimateImage>();
            var thumbFilesAfterIds = new List<JobEstimateImage>();

            JobEstimateServiceViewModel jobEstimateBeforeServiceViewModel = new JobEstimateServiceViewModel();
            JobEstimateServiceViewModel jobEstimateAfterServiceViewModel = new JobEstimateServiceViewModel();
            JobEstimateServiceViewModel jobEstimateSliderImagesServiceViewModel = new JobEstimateServiceViewModel();
            JobEstimateServiceViewModel jobEstimateInvoiceImagesServiceViewModel = new JobEstimateServiceViewModel();

            var jobEstimateServices = _jobEstimateServices.IncludeMultiple(y => y.DataRecorderMetaData).Where(x => jobEstimateServiceGrouped.Contains(x.Id)).ToList();
            var jobEstimateImages = _jobEstimateImage.Table.Where(x => x.ServiceId.HasValue && jobEstimateServiceGrouped.Contains(x.ServiceId.Value)).ToList();
            int beforeAfterIndex = 0;

            var servicesId = jobEstimateImageServices.Select(x => x.Id).ToList();
            var beforeAfterImages = _beforeAfterImagesRepository.Table.Where(x => servicesId.Contains(x.ServiceId.Value));
            foreach (var jobEstimateImageService in jobEstimateImageServices.Where(x => (x.TypeId == (long)LookupTypes.BeforeWork) || x.TypeId == (long)LookupTypes.DuringWork && x.DataRecorderMetaData != null).OrderByDescending(x => x.DataRecorderMetaData.DateCreated))
            {
                var isBestPicture = false;
                var isAddetoLocalSite = false;
                ++beforeAfterIndex;
                ImagePairs jobEstimateCategoryImagePair = new ImagePairs();
                var beforeImageJobFileViewModel = new List<FileModel>();
                var afterImageJobFileViewModel = new List<FileModel>();

                var beforeDuringJobModel = jobEstimateServices.Where(x => x.Id == jobEstimateImageService.Id).FirstOrDefault();
                var afterJobModel = jobEstimateServices.Where(x => x.PairId == beforeDuringJobModel.Id).FirstOrDefault();
                if (beforeDuringJobModel != null)
                {
                    filesBeforeIds = jobEstimateImages.Where(x => x.ServiceId == beforeDuringJobModel.Id && (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.DuringWork)).Select(x => x.File).Distinct().ToList();
                    thumbFilesBeforeIds = jobEstimateImages.Where(x => x.ServiceId == beforeDuringJobModel.Id && (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.DuringWork)).Distinct().ToList();
                    var jobEstimateImageDomain = jobEstimateImages.FirstOrDefault(x => x.ServiceId == beforeDuringJobModel.Id && (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.DuringWork));
                    if (jobEstimateImageDomain != null)
                    {
                        isBestPicture = jobEstimateImageDomain.IsBestImage;
                        isAddetoLocalSite = jobEstimateImageDomain.IsAddToLocalGallery;
                    }
                }
                if (afterJobModel != null)
                {
                    filesAfterIds = jobEstimateImages.Where(x => afterJobModel != null && x.ServiceId == afterJobModel.Id && x.TypeId == (long?)LookupTypes.AfterWork).Select(x => x.File).Distinct().ToList();

                    thumbFilesAfterIds = jobEstimateImages.Where(x => afterJobModel != null && x.ServiceId == afterJobModel.Id && x.TypeId == (long?)LookupTypes.AfterWork).Distinct().ToList();
                    var jobEstimateImageDomain = jobEstimateImages.FirstOrDefault(x => afterJobModel != null && x.ServiceId == afterJobModel.Id && x.TypeId == (long?)LookupTypes.AfterWork);
                    if (jobEstimateImageDomain != null)
                    {
                        isBestPicture = jobEstimateImageDomain.IsBestImage;
                        isAddetoLocalSite = jobEstimateImageDomain.IsAddToLocalGallery;
                    }
                }

                thumbFilesAfterIds = thumbFilesAfterIds.Where(x => x != null).ToList();
                thumbFilesBeforeIds = thumbFilesBeforeIds.Where(x => x != null).ToList();

                filesAfterIds = filesAfterIds.Where(x => x != null).ToList();
                filesBeforeIds = filesBeforeIds.Where(x => x != null).ToList();


                var sc = beforeAfterImages.Where(x => x.ServiceId == jobEstimateImageService.Id && (x.TypeId == (long)LookupTypes.BeforeWork || x.TypeId == (long)LookupTypes.DuringWork));
                foreach (var filesBeforeId in filesBeforeIds)
                {
                    var beforeImage = thumbFilesBeforeIds.FirstOrDefault(x1 => x1.FileId.GetValueOrDefault() == filesBeforeId.Id && x1.ThumbFileId != null);
                    if (beforeImage != null)
                        beforeImageJobFileViewModel.Add(_jobFactory.CreateServiceFileViewModel(filesBeforeId, beforeImage.ThumbFile, userId, sc.ToList()));
                }

                foreach (var filesAfterId in filesAfterIds)
                {
                    var aftterImage = thumbFilesAfterIds.FirstOrDefault(x1 => x1.FileId.GetValueOrDefault() == filesAfterId.Id && x1.ThumbFileId != null);
                    if (aftterImage != null)
                        afterImageJobFileViewModel.Add(_jobFactory.CreateServiceFileViewModel(filesAfterId, aftterImage.ThumbFile, userId, beforeAfterImages.Where(x => x.ServiceId == afterJobModel.Id && x.TypeId == (long)LookupTypes.AfterWork).ToList()));
                }
                var beforeImageJobViewModel = _jobFactory.CreateServiceViewModel(beforeDuringJobModel, beforeImageJobFileViewModel, true, userId);
                var afterImageJobViewModel = _jobFactory.CreateServiceViewModel(afterJobModel, afterImageJobFileViewModel, false, userId);

                if (beforeDuringJobModel.JobEstimateImageCategory.SchedulerId == schedulerId)
                {
                    beforeImageJobViewModel.IsFromCurrentScheduler = true;
                    afterImageJobViewModel.IsFromCurrentScheduler = true;
                }
                else
                {
                    beforeImageJobViewModel.IsFromCurrentScheduler = false;
                    afterImageJobViewModel.IsFromCurrentScheduler = false;
                }
                afterImageJobViewModel.SelectedIndex = beforeAfterIndex;
                beforeImageJobViewModel.SelectedIndex = beforeAfterIndex;
                afterImageJobViewModel.ImageIndex = beforeAfterIndex;
                beforeImageJobViewModel.ImageIndex = beforeAfterIndex;
                afterImageJobViewModel.OriginalImageIndex = beforeAfterIndex;
                beforeImageJobViewModel.OriginalImageIndex = beforeAfterIndex;

                jobEstimateCategoryImagePair.AfterImages = afterImageJobViewModel;
                jobEstimateCategoryImagePair.BeforeImages = beforeImageJobViewModel;
                jobEstimateCategoryImagePair.IsChanged = false;
                jobEstimateCategoryImagePair.IsSelectedFinishMaterialProperty = false;
                jobEstimateCategoryImagePair.isSelectedCompanyNameProperty = false;
                jobEstimateCategoryImagePair.isSelectedMaidJanitorialProperty = false;
                jobEstimateCategoryImagePair.isSelectedMaidServiceProperty = false;
                jobEstimateCategoryImagePair.isSelectedPropertyManagerProperty = false;
                jobEstimateCategoryImagePair.isSelectedSurfaceColorProperty = false;
                jobEstimateCategoryImagePair.IsSelectedServiceTypeProperty = false;
                jobEstimateCategoryImagePair.IsBestPicture = beforeImageJobViewModel.IsBestPairMarkedImage;
                jobEstimateCategoryImagePair.IsAddToLocalGallery = beforeImageJobViewModel.IsAddTpLocalGalleryImage;
                jobEstimateCategoryImagePairs.Add(jobEstimateCategoryImagePair);
            }
            if (jobEstimateImageServices.Where(x => x.TypeId == (long)LookupTypes.InvoiceImages).Count() > 0)
            {
                var invoicesImages = jobEstimateImageServices.Where(x => x.TypeId == (long)LookupTypes.InvoiceImages).Select(x => x.Id).ToList();
                var filesInvoiceIds = jobEstimateImages.Where(x => x.ServiceId.HasValue && invoicesImages.Contains(x.ServiceId.Value)).Select(x => x.File).ToList();
                var invoiceImageJobFileViewModel = filesInvoiceIds.Select(x => _jobFactory.CreateServiceFileViewModel(x, null, userId));
                var invoiceImageJobViewModel = _jobFactory.CreateServiceViewModel(null, invoiceImageJobFileViewModel, false, userId);

                jobEstimateInvoiceImagesServiceViewModel = invoiceImageJobViewModel;
            }

            if (jobEstimateImageServices.Where(x => x.TypeId == (long)LookupTypes.ExteriorBuilding).Count() > 0)
            {
                var buildingImages = jobEstimateImageServices.Where(x => x.TypeId == (long)LookupTypes.ExteriorBuilding).Select(x => x.Id).ToList();
                var filesBuildingIds = jobEstimateImages.Where(x => x.ServiceId.HasValue && buildingImages.Contains(x.ServiceId.Value)).Select(x => x.File).ToList();
                var exteriorBuildingImageJobFileViewModel = filesBuildingIds.Select(x => _jobFactory.CreateServiceFileViewModel(x, null, userId, beforeAfterImages.Where(y => y.FileId == x.Id && y.TypeId == (long)LookupTypes.ExteriorBuilding).ToList()));
                var exteriorBuildingImageJobViewModel = _jobFactory.CreateServiceViewModel(null, exteriorBuildingImageJobFileViewModel, false, userId);
                jobEstimateSliderImagesServiceViewModel = exteriorBuildingImageJobViewModel;
            }
            var jobEstimateCategoryModel = new JobEstimateCategoryViewModel()
            {
                ImagePairs = jobEstimateCategoryImagePairs,
                EstimateId = mediaType == (long)ScheduleType.Estimate ? jobEstimateId : null,
                JobId = mediaType == (long)ScheduleType.Job ? jobEstimateId : null,
                SchedulerId = schedulerId,
                InvoiceImages = jobEstimateInvoiceImagesServiceViewModel,
                SliderImages = jobEstimateSliderImagesServiceViewModel
            };
            return jobEstimateCategoryModel;
        }
        public bool SaveNotes(SchedulerNoteModel model)
        {
            var note = _jobFactory.CreateDomain(model);
            _jobNoteRepository.Save(note);
            return true;
        }

        public JobListModel GetHolidayList(long franchiseeId)
        {
            var list = new JobListModel { };
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null)
                return list;

            var address = franchisee.Organization.Address.FirstOrDefault();
            if (address == null)
                return list;

            var countryId = address.CountryId;
            var holidayList = _holidayRepository.Table.Where(h => h.CountryId == null || h.CountryId == countryId).ToList();
            list.Collection = holidayList.Select(x => _jobFactory.CreateViewModel(x, franchisee));
            return list;
        }

        public ICollection<JobScheduler> GetSchedulerForUserIds(ICollection<long> userIds)
        {
            return _jobSchedulerRepository.Table.Where(x => userIds.Contains(x.AssigneeId.Value)
                            && x.EndDate > _clock.UtcNow && x.IsActive && !x.IsVacation).ToList();
        }

        public long SetDefaultAssignee(long id, long franchiseeId, DateTime startDate, DateTime endDate, bool isJob)
        {
            var assigneeList = _organizationRoleUserRepository.Fetch(x => x.IsActive && x.OrganizationId == franchiseeId
                                    && (x.Person.UserLogin != null && x.Person.UserLogin.IsActive && !x.Person.UserLogin.IsLocked));
            if (!assigneeList.Any())
                return 0;
            if (isJob)
                assigneeList = assigneeList.Where(x => x.RoleId == (long)RoleType.Technician);
            else
                assigneeList = assigneeList.Where(x => x.RoleId == (long)RoleType.SalesRep);

            if (!assigneeList.Any())
                return 0;

            foreach (var assignee in assigneeList)
            {
                var isAvailable = CheckAvailability(id, assignee.Id, startDate, endDate, false);
                if (!isAvailable)
                    continue;
                else
                    return assignee.Id;
            }
            return 0;
        }

        public string GetCustomerAddress(long jobId, long estimateId)
        {
            string address = "";
            if (jobId != default(long))
                address = _jobschedulerRepository.Table.Where(x => x.JobId == jobId).Select(x => x.Job.JobCustomer.CustomerAddress + ", " + x.Job.JobCustomer.Address.AddressLine2 + ", " + x.Job.JobCustomer.Address.CityName + ", " + x.Job.JobCustomer.Address.StateName + " " + x.Job.JobCustomer.Address.ZipCode + ", " + x.Job.JobCustomer.Address.Country.Name).FirstOrDefault();
            else
                address = _jobschedulerRepository.Table.Where(x => x.EstimateId == estimateId).Select(x => x.Estimate.JobCustomer.CustomerAddress + ", " + x.Job.JobCustomer.Address.AddressLine2 + ", " + x.Estimate.JobCustomer.Address.CityName + ", " + x.Estimate.JobCustomer.Address.StateName + " " + x.Estimate.JobCustomer.Address.ZipCode + ", " + x.Estimate.JobCustomer.Address.Country.Name).FirstOrDefault();

            //address = address.Replace(' ', '+');
            //address = address.Replace(',', '+');
            return address;
        }
        public MailListModel GetMailList(MailListFilter filter)
        {
            _logService.Info("GetMailList is Running 1");
            // 1. English templates
            var englishTemplates = _emailTemplateRepository.Table
                .Where(x =>
                    x.LanguageId == (long)LanguageEnum.English &&
                    x.IsRequired)
                .OrderByDescending(x => x.Id)
                .ToList();

            _logService.Info("GetMailList is Running 2");
            // 2. Spanish templates
            var spanishTemplates = _emailTemplateRepository.Table
                .Where(x =>
                    x.LanguageId == (long)LanguageEnum.Spanish &&
                    x.IsRequired)
                .ToList();

            _logService.Info("GetMailList is Running 3");
            // 3. Notification email ids
            //var notificationEmailIds = _notificationEmailRepository.Table
            //    .Where(x => x.EmailTemplateId != default(long))
            //    .Select(x => x.EmailTemplateId)
            //    .Distinct()
            //    .ToList();

            // 4. FINAL COLLECTION (PASS LIST — NOT DICTIONARY)
            //var collection = englishTemplates
            //    .Select(template =>
            //        _jobFactory.CreateMeetingDomainForMail(
            //            template,
            //            spanishTemplates,        
            //            notificationEmailIds))
            //    .ToList();

            _logService.Info("GetMailList is Running 4");
            var collection = englishTemplates
                .Select(template =>
                    _jobFactory.CreateMeetingDomainForMail(
                        template,
                        spanishTemplates))
                .ToList();

            _logService.Info("GetMailList is Running 5");
            return new MailListModel
            {
                Collection = collection,
                PagingModel = new PagingModel(
                    filter.PageNumber,
                    filter.PageSize,
                    englishTemplates.Count)
            };
        }
        public EmailTemplate GetMailTemplate(long id)
        {
            var mailTemplate = _emailTemplateRepository.Table.Where(x => x.NotificationTypeId == id).FirstOrDefault();
            var body = _notificationEmailRepository.Table.Where(x => x.EmailTemplateId == id).OrderByDescending(x => x.Id).Select(x => x.Body).FirstOrDefault();

            body = Regex.Replace(body, "<img.*?>", string.Empty);
            body = Regex.Replace(body, "<a.*?>", string.Empty);
            body = Regex.Replace(body, "<a href.*?>", string.Empty);
            mailTemplate.Body = body;
            return mailTemplate;
        }
        public bool EditMailTemplate(long? id, bool isActive)
        {
            var mailTemplate = _emailTemplateRepository.Get(id.GetValueOrDefault());
            mailTemplate.isActive = isActive;
            mailTemplate.IsNew = false;
            _emailTemplateRepository.Save(mailTemplate);
            return true;
        }

        public bool SaveBeforeAfterImages(JobEstimateCategoryViewModel model)
        {
            var categoryId = default(long);
            var pairId = default(long?);
            var debuggerLogModelList = new List<DebuggerLogModel>();
            var debuggerLogModel = new DebuggerLogModel();
            string debuggerLogs = "";

            if (model.Id <= 0)
            {
                var jobEstimateCategory = _jobFactory.CreateJobEstimateCategory(model);

                _jobEstimateImageCategory.Save(jobEstimateCategory);
                categoryId = jobEstimateCategory.Id;

                if (model.IsFromBeforeAfterPane.GetValueOrDefault())
                {
                    foreach (var imagePair in model.ImagePairs)
                    {
                        debuggerLogModel.Description = "";
                        debuggerLogModel.ActionId = (long?)DebuggerLogType.ADDINGNEWVALUE;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;
                        debuggerLogModel = new DebuggerLogModel();
                        debuggerLogModel.JobEstimateimageCategoryId = categoryId;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;

                        if (imagePair.BeforeImages.FilesList.Count() == 0 && imagePair.BeforeImages.ImagesInfo.Count() > 0)
                        {
                            imagePair.BeforeImages.FilesList.AddRange(imagePair.BeforeImages.ImagesInfo.Select(x => x.FileId));
                        }
                        if (imagePair.AfterImages.FilesList.Count() == 0 && imagePair.AfterImages.ImagesInfo.Count() > 0)
                        {
                            imagePair.AfterImages.FilesList.AddRange(imagePair.AfterImages.ImagesInfo.Select(x => x.FileId));
                        }

                        var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(imagePair.BeforeImages, categoryId, imagePair.BeforeImages.TypeId != null ? imagePair.BeforeImages.TypeId : (long?)LookupTypes.BeforeWork);


                        debuggerLogModel.Description += _debuggerService.CreateDebugger(jobEstimateBeforeService, null, "Before", out debuggerLogs);


                        _jobEstimateServices.Save(jobEstimateBeforeService);

                        debuggerLogModel.JobEstimateServiceCategoryId = jobEstimateBeforeService.Id;

                        //imagePair.BeforeImages.Id = jobEstimateBeforeService.Id;

                        pairId = jobEstimateBeforeService.Id;
                        var beforeServiceId = jobEstimateBeforeService.Id;


                        foreach (var fileId in imagePair.BeforeImages.FilesList)
                        {
                            var jobEstimateImageDomain = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == imagePair.BeforeImages.Id);

                            var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(imagePair.BeforeImages, beforeServiceId, imagePair.BeforeImages.TypeId != null ? imagePair.BeforeImages.TypeId : (long?)LookupTypes.BeforeWork, fileId);
                            jobEstimateBeforeServiceImage.IsBestImage = imagePair.IsBestPicture;

                            jobEstimateBeforeServiceImage = AddOrDeleteBestBeforeImages(imagePair, jobEstimateBeforeService, jobEstimateBeforeServiceImage, true);

                            if (imagePair.AfterImages.ImagesInfo.Count() > 1)
                            {
                                if (imagePair.AfterImages.ImagesInfo.FirstOrDefault().Id > 0)
                                {
                                    jobEstimateBeforeServiceImage.Id = imagePair.AfterImages.ImagesInfo.FirstOrDefault().Id;
                                    jobEstimateBeforeServiceImage.IsNew = false;
                                }
                                else
                                {
                                    jobEstimateBeforeServiceImage.Id = 0;
                                    jobEstimateBeforeServiceImage.IsNew = true;
                                }
                            }
                            if (!jobEstimateBeforeService.IsNew)
                            {
                                var id = GetIdFromDb(jobEstimateBeforeServiceImage);

                                if (id != jobEstimateBeforeService.Id && id != default(long?))
                                {
                                    jobEstimateBeforeServiceImage.Id = id.GetValueOrDefault();
                                }
                                else
                                {
                                    jobEstimateBeforeServiceImage.Id = 0;
                                    jobEstimateBeforeServiceImage.IsNew = true;
                                }
                            }
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
                                fileDomain.css = imagePair.BeforeImages.Css;
                                debuggerLogModel.Description += " Adding Before ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                                _fileRepository.Save(fileDomain);
                            }
                        }


                        if (imagePair.BeforeImages.FilesList.Count() == 0)
                        {
                            var beforeAfterImagesLocal = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == beforeServiceId);
                            if (beforeAfterImagesLocal != null)
                            {
                                _jobEstimateImage.Delete(beforeAfterImagesLocal);
                            }
                        }
                        var jobEstimateCategorydomain = _jobEstimateImageCategory.IncludeMultiple(x => x.JobScheduler).FirstOrDefault(x => x.Id == debuggerLogModel.JobEstimateimageCategoryId);
                        debuggerLogModel.TypeId = (long?)LookupTypes.BeforeWork;
                        debuggerLogModel.ActionId = (long?)DebuggerLogType.ADDINGNEWVALUE;
                        debuggerLogModel.FranchiseeId = jobEstimateCategorydomain.JobScheduler.FranchiseeId;
                        debuggerLogModel.UserId = model.UserId;
                        debuggerLogModel.JobSchedulerId = jobEstimateCategory.JobScheduler.Id;
                        // Saving Before Images
                        if (debuggerLogModel.Description.Length > 0)
                        {
                            debuggerLogModelList.Add(debuggerLogModel);
                        }
                        // Saving After Images
                        debuggerLogModel = new DebuggerLogModel();
                        debuggerLogModel.Description = "";
                        debuggerLogModel.ActionId = (long?)DebuggerLogType.ADDINGNEWVALUE;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;

                        debuggerLogModel.JobEstimateimageCategoryId = categoryId;
                        debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;


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
                        foreach (var fileId in imagePair.AfterImages.FilesList)
                        {

                            var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(imagePair.AfterImages, afterServiceId, imagePair.AfterImages.TypeId != null ? imagePair.AfterImages.TypeId : (long?)LookupTypes.AfterWork, fileId);
                            jobEstimateBeforeServiceImage.IsBestImage = imagePair.IsBestPicture;

                            jobEstimateBeforeServiceImage = AddOrDeleteBestBeforeImages(imagePair, jobEstimateBeforeService, jobEstimateBeforeServiceImage, true);
                            if (imagePair.IsBestPicture)
                            {
                                jobEstimateBeforeServiceImage.BestFitMarkDateTime = DateTime.UtcNow;
                                AddorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK, true);
                            }

                            else
                            {
                                jobEstimateBeforeServiceImage.BestFitMarkDateTime = null;
                                DeleteorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK);
                            }

                            if (imagePair.IsAddToLocalGallery)
                            {
                                jobEstimateBeforeServiceImage.IsAddToLocalGallery = true;
                                if (jobEstimateBeforeServiceImage.AddToGalleryDateTime == default(DateTime?))
                                {
                                    jobEstimateBeforeServiceImage.AddToGalleryDateTime = DateTime.UtcNow;
                                }
                                AddorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY, true);
                            }
                            else
                            {
                                jobEstimateBeforeServiceImage.IsAddToLocalGallery = false;
                                jobEstimateBeforeServiceImage.AddToGalleryDateTime = null;
                                DeleteorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY);
                            }

                            if (imagePair.AfterImages.ImagesInfo.Count() > 0)
                            {
                                if (imagePair.AfterImages.ImagesInfo.FirstOrDefault().Id > 0)
                                {
                                    jobEstimateBeforeServiceImage.Id = imagePair.AfterImages.ImagesInfo.FirstOrDefault().Id;
                                    jobEstimateBeforeServiceImage.IsNew = false;
                                }
                                else
                                {
                                    jobEstimateBeforeServiceImage.Id = 0;
                                    jobEstimateBeforeServiceImage.IsNew = true;
                                }
                            }

                            if (!jobEstimateBeforeService.IsNew)
                            {
                                var id = GetIdFromDb(jobEstimateBeforeServiceImage);

                                if (id != jobEstimateBeforeService.Id && id != default(long?))
                                {
                                    jobEstimateBeforeServiceImage.Id = id.GetValueOrDefault();
                                }
                                else
                                {
                                    jobEstimateBeforeServiceImage.Id = 0;
                                    jobEstimateBeforeServiceImage.IsNew = true;
                                }
                            }
                            debuggerLogModel.Description += _debuggerService.CreateDebuggerForImage(jobEstimateBeforeServiceImage, null, "After", out debuggerLogs);
                            _jobEstimateImage.Save(jobEstimateBeforeServiceImage);

                            var fileDomainLocal = _fileRepository.Get(jobEstimateBeforeServiceImage.FileId.GetValueOrDefault());
                            if (fileDomainLocal != null)
                            {
                                var fileModel = _beforeAfterThumbNameService.CreateImageThumb(fileDomainLocal, imagePair.AfterImages.Css);
                                imagePair.AfterImages.ThumbFileId = fileModel.FileId;
                                jobEstimateBeforeServiceImage.ThumbFileId = fileModel.FileId;
                                jobEstimateBeforeServiceImage.IsNew = false;
                                debuggerLogModel.Description += " Adding After ThumbNail ImageUrl: " + fileModel.FullFilePath;
                                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                            }
                            if (fileDomainLocal == null)
                            {
                                jobEstimateBeforeServiceImage.ThumbFileId = null;
                                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                            }
                            imagePair.AfterImages.Id = afterServiceId;

                            if (fileId != null)
                            {
                                var fileDomain = _fileRepository.Get(jobEstimateBeforeServiceImage.FileId.Value);
                                //debuggerLogModel.Description += " Adding After ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                                fileDomain.css = imagePair.AfterImages.Css;
                                _fileRepository.Save(fileDomain);
                            }
                        }

                        if (imagePair.AfterImages.FilesList.Count() == 0)
                        {
                            var beforeAfterImagesLocal = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == afterServiceId);
                            if (beforeAfterImagesLocal != null)
                            {
                                _jobEstimateImage.Delete(beforeAfterImagesLocal);
                            }
                        }

                        debuggerLogModel.FranchiseeId = jobEstimateCategory.JobScheduler.FranchiseeId;
                        debuggerLogModel.TypeId = (long?)LookupTypes.AfterWork;
                        debuggerLogModel.UserId = model.UserId;
                        debuggerLogModel.JobSchedulerId = jobEstimateCategory.JobScheduler.Id;
                        if (debuggerLogModel.Description.Length > 0)
                        {
                            debuggerLogModelList.Add(debuggerLogModel);
                        }
                    }

                    SaveDebuggerLog(debuggerLogModelList);
                }
                SaveBuildingImages(model.SliderImages, categoryId);
                SaveInvoiceImages(model.InvoiceImages, categoryId);

            }
            else
            {
                var domain = _jobEstimateImageCategory.Get(model.Id.GetValueOrDefault());
                var editDomain = _jobFactory.CreateJobEstimateCategory(domain, model);
                categoryId = editDomain.Id;

                if (model.IsFromBeforeAfterPane.GetValueOrDefault())
                {
                    DeleteBeforeAfterImages(model.ImagePairs, model.Id);
                    if (model.ImagePairs.Count() != 0)
                    {
                        if (editDomain.SchedulerId == null)
                        {
                            editDomain.SchedulerId = model.SchedulerId;
                        }

                        _jobEstimateImageCategory.Save(editDomain);

                        var editEstimateServiceInDb = _jobEstimateServices.Table.Where(x => x.CategoryId == editDomain.Id).ToList();
                        var editServicesIds = editEstimateServiceInDb.Select(x => x.Id).ToList();
                        var editEstimateImageInDb = _jobEstimateImage.Table.Where(x => x.ServiceId.HasValue
                                                      && editServicesIds.Contains(x.ServiceId.Value)).ToList();

                        var beforeImagesIds = model.ImagePairs.Select(x1 => x1.BeforeImages.Id).ToList();
                        beforeImagesIds.AddRange(model.ImagePairs.Select(x1 => x1.AfterImages.Id).ToList());
                        var jobEstimateList = _jobEstimateImage.Table.Where(x => beforeImagesIds.Contains(x.ServiceId)).ToList();
                        var jobImagesFromModel = model.ImagePairs.Select(x => x.AfterImages).ToList();
                        jobImagesFromModel.AddRange(model.ImagePairs.Select(x => x.BeforeImages).ToList());
                        var deletedImageIds = GetImageDeletedIds(jobImagesFromModel, model.ImagePairs);


                        foreach (var beforeImagesList in model.ImagePairs)
                        {

                            debuggerLogModel = new DebuggerLogModel();
                            debuggerLogModel.ActionId = (long?)DebuggerLogType.EDITINGVALUE;
                            debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;
                            debuggerLogModel.JobEstimateimageCategoryId = categoryId;
                            debuggerLogModel.Description = "";


                            var beforeId = default(long?);
                            var afterId = default(long?);

                            var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(beforeImagesList.BeforeImages, categoryId, beforeImagesList.BeforeImages.TypeId != null ? beforeImagesList.BeforeImages.TypeId : (long?)LookupTypes.BeforeWork);
                            debuggerLogModel.Description += _debuggerService.CreateDebugger(jobEstimateBeforeService, editEstimateServiceInDb, "Before", out debuggerLogs);

                            _jobEstimateServices.Save(jobEstimateBeforeService);

                            beforeImagesList.BeforeImages.Id = jobEstimateBeforeService.Id;
                            var beforeServiceId = jobEstimateBeforeService.Id;
                            pairId = beforeServiceId;

                            foreach (var fileModel in beforeImagesList.BeforeImages.ImagesInfo)
                            {

                                var id = fileModel.Id;
                                var jobEstimateImageList = _jobEstimateImage.Table.Where(x => x.ServiceId == beforeServiceId).ToList();
                                var isPresent = _jobEstimateImage.Table.Any(x => id != null && (x.ServiceId == id || x.ServiceId == beforeImagesList.BeforeImages.OriginalId));
                                var jobEstimateImage = jobEstimateList.FirstOrDefault(x => x.ServiceId == fileModel.Id);
                                var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageEditDomain(beforeImagesList.BeforeImages, beforeServiceId, beforeImagesList.BeforeImages.TypeId != null ? beforeImagesList.BeforeImages.TypeId : (long?)LookupTypes.BeforeWork, fileModel);
                                if (jobEstimateImage == null)
                                {
                                    jobEstimateBeforeServiceImage.IsNew = true;
                                    jobEstimateBeforeServiceImage.Id = 0;
                                    isPresent = false;
                                }
                                if (jobEstimateBeforeServiceImage.Id == 0)
                                    jobEstimateBeforeServiceImage.IsNew = true;


                                if (jobEstimateBeforeServiceImage.Id > 0)
                                {
                                    jobEstimateBeforeServiceImage.IsNew = false;
                                }

                                if (!isPresent)
                                {
                                    jobEstimateBeforeServiceImage.IsNew = true;
                                    jobEstimateBeforeServiceImage.Id = 0;
                                }

                                if (jobEstimateBeforeServiceImage.IsBestImage != beforeImagesList.IsBestPicture)
                                {
                                    if (beforeImagesList.BeforeImages.FileId != null)
                                    {
                                        var fileDomain = _fileRepository.Get(beforeImagesList.BeforeImages.FileId.Value);
                                        //debuggerLogModel.Description = "Adding To Best Picture Before Image having File Path-" + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                                    }
                                }

                                jobEstimateBeforeServiceImage.IsBestImage = beforeImagesList.IsBestPicture;
                                var isNew = jobEstimateBeforeServiceImage.IsNew;
                                jobEstimateBeforeServiceImage = AddOrDeleteBestBeforeImages(beforeImagesList, jobEstimateBeforeService, jobEstimateBeforeServiceImage, true);

                                debuggerLogModel.Description += _debuggerService.CreateDebuggerForImage(jobEstimateBeforeServiceImage, jobEstimateImageList, "Before", out debuggerLogs);
                                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);

                                var fileDomainLocal = _fileRepository.Get(jobEstimateBeforeServiceImage.FileId.GetValueOrDefault());
                                if (fileDomainLocal != null && id == 0)
                                {

                                    //var fileModelThumb = _beforeAfterThumbNameService.CreateImageThumb(fileDomainLocal, beforeImagesList.BeforeImages.Css);
                                    var fileModelThumb = _beforeAfterThumbNameService.CreateImageThumb(fileDomainLocal, fileDomainLocal.css);

                                    jobEstimateBeforeServiceImage.ThumbFileId = fileModelThumb.FileId;
                                    beforeImagesList.BeforeImages.ThumbFileId = fileModelThumb.FileId;
                                    fileModel.ThumbFileId = fileModelThumb.FileId;

                                    jobEstimateBeforeServiceImage.IsNew = false;
                                    _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                                    debuggerLogModel.Description += " Adding Before ThumbNail ImageUrl: " + fileModelThumb.FullFilePath;
                                }

                                if (fileDomainLocal == null)
                                {
                                    jobEstimateBeforeServiceImage.ThumbFileId = null;
                                    _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                                }
                                debuggerLogModel.JobEstimateServiceCategoryId = jobEstimateBeforeServiceImage.Id;
                                if (fileModel.Id != null)
                                {
                                    var fileDomain = _fileRepository.Get(jobEstimateBeforeServiceImage.FileId.Value);
                                    //if (jobEstimateBeforeServiceImage.IsNew)
                                    //{
                                    //    debuggerLogModel.Description += " Adding Before ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                                    //}
                                    if (fileDomain != null)
                                        fileDomain.css = beforeImagesList.BeforeImages.Css;
                                    _fileRepository.Save(fileDomain);
                                }
                                debuggerLogModel.TypeId = (long?)LookupTypes.BeforeWork;
                                debuggerLogModel.ActionId = (long?)DebuggerLogType.EDITINGVALUE;
                                debuggerLogModel.FranchiseeId = domain.JobScheduler.FranchiseeId;
                                debuggerLogModel.UserId = model.UserId;
                                debuggerLogModel.JobSchedulerId = domain.JobScheduler.Id;
                                // Saving Before Images
                                if (debuggerLogModel.Description.Length > 0)
                                {
                                    debuggerLogModelList.Add(debuggerLogModel);
                                }

                            }


                            if (beforeImagesList.BeforeImages.ImagesInfo.Count() == 0)
                            {
                                var beforeAfterImagesLocal = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == beforeServiceId);
                                if (beforeAfterImagesLocal != null)
                                {
                                    _jobEstimateImage.Delete(beforeAfterImagesLocal);
                                }
                            }
                            // Saving After Images
                            debuggerLogModel = new DebuggerLogModel();
                            debuggerLogModel.Description = "";
                            debuggerLogModel.ActionId = (long?)DebuggerLogType.EDITINGVALUE;
                            debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;

                            debuggerLogModel.JobEstimateimageCategoryId = categoryId;
                            debuggerLogModel.PageId = (long?)PageType.BEFOREAFTERPAGE;

                            var jobEstimateAfterService = _jobFactory.CreateJobEstimatePairing(beforeImagesList.AfterImages, categoryId, (long?)LookupTypes.AfterWork);
                            jobEstimateAfterService.PairId = pairId;
                            debuggerLogModel.Description += _debuggerService.CreateDebugger(jobEstimateAfterService, editEstimateServiceInDb, "After", out debuggerLogs);

                            _jobEstimateServices.Save(jobEstimateAfterService);
                            beforeImagesList.AfterImages.Id = jobEstimateAfterService.Id;
                            var afterServiceId = jobEstimateAfterService.Id;
                            foreach (var file in beforeImagesList.AfterImages.ImagesInfo)
                            {
                                var jobEstimateImageList = _jobEstimateImage.Table.Where(x => x.ServiceId == afterServiceId).ToList();
                                var id = file.Id;
                                var isPresent = _jobEstimateImage.Table.Any(x => id != null && (x.FileId == id || x.ServiceId == afterServiceId));
                                var jobEstimateImage = jobEstimateList.FirstOrDefault(x => x.ServiceId == file.Id);
                                var jobEstimateAfterServiceImage = _jobFactory.CreateJobEstimateImageEditDomain(beforeImagesList.AfterImages, afterServiceId, (long?)LookupTypes.AfterWork, file);
                                if (jobEstimateImage == null)
                                {
                                    jobEstimateAfterServiceImage.IsNew = true;
                                    jobEstimateAfterServiceImage.Id = 0;
                                    isPresent = false;
                                }

                                if (jobEstimateAfterServiceImage.Id == 0)
                                    jobEstimateAfterServiceImage.IsNew = true;

                                if (jobEstimateAfterServiceImage.Id > 0)
                                {
                                    jobEstimateAfterServiceImage.IsNew = false;
                                }
                                if (!isPresent)
                                {
                                    jobEstimateAfterServiceImage.IsNew = true;
                                    jobEstimateAfterServiceImage.Id = 0;
                                }
                                var isNew = jobEstimateAfterServiceImage.IsNew;
                                jobEstimateAfterServiceImage.IsBestImage = beforeImagesList.IsBestPicture;

                                if (jobEstimateAfterServiceImage.IsBestImage != beforeImagesList.IsBestPicture)
                                {
                                    var fileDomain = _fileRepository.Get(beforeImagesList.BeforeImages.FileId.Value);
                                    //debuggerLogModel.Description = "Adding To Best Picture After Image having File Path-" + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                                }

                                jobEstimateAfterServiceImage = AddOrDeleteBestBeforeImages(beforeImagesList, jobEstimateBeforeService, jobEstimateAfterServiceImage, true);


                                debuggerLogModel.Description += _debuggerService.CreateDebuggerForImage(jobEstimateAfterServiceImage, jobEstimateImageList, "After", out debuggerLogs);
                                _jobEstimateImage.Save(jobEstimateAfterServiceImage);

                                var fileDomainLocal = _fileRepository.Get(jobEstimateAfterServiceImage.FileId.GetValueOrDefault());
                                if (fileDomainLocal != null && id == 0)
                                {

                                    var fileModel = _beforeAfterThumbNameService.CreateImageThumb(fileDomainLocal, beforeImagesList.AfterImages.Css);
                                    beforeImagesList.AfterImages.ThumbFileId = fileModel.FileId;
                                    jobEstimateAfterServiceImage.ThumbFileId = fileModel.FileId;
                                    file.ThumbFileId = fileModel.FileId;
                                    jobEstimateAfterServiceImage.IsNew = false;

                                    _jobEstimateImage.Save(jobEstimateAfterServiceImage);
                                    debuggerLogModel.Description += " Adding After ThumbNail ImageUrl: " + fileModel.FullFilePath;
                                }
                                if (fileDomainLocal == null)
                                {
                                    jobEstimateAfterServiceImage.ThumbFileId = null;
                                    _jobEstimateImage.Save(jobEstimateAfterServiceImage);
                                }
                                debuggerLogModel.JobEstimateServiceCategoryId = jobEstimateAfterServiceImage.Id;
                                if (file.Id != null)
                                {

                                    var fileDomain = _fileRepository.Get(jobEstimateAfterServiceImage.FileId.Value);

                                    if (isNew)
                                    {
                                        //debuggerLogModel.Description += " Adding After ImageUrl: " + fileDomain.RelativeLocation + "\\" + fileDomain.Name;
                                    }
                                    fileDomain.css = beforeImagesList.AfterImages.Css;
                                    _fileRepository.Save(fileDomain);
                                }

                                debuggerLogModel.TypeId = (long?)LookupTypes.AfterWork;
                                debuggerLogModel.ActionId = (long?)DebuggerLogType.EDITINGVALUE;
                                debuggerLogModel.FranchiseeId = domain.JobScheduler.FranchiseeId;
                                debuggerLogModel.UserId = model.UserId;
                                debuggerLogModel.JobSchedulerId = domain.JobScheduler.Id;
                                if (debuggerLogModel.Description.Length > 0)
                                {
                                    debuggerLogModelList.Add(debuggerLogModel);
                                }
                            }

                            if (beforeImagesList.AfterImages.ImagesInfo.Count() == 0)
                            {
                                var beforeAfterImagesLocal = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == afterServiceId);
                                if (beforeAfterImagesLocal != null)
                                {
                                    _jobEstimateImage.Delete(beforeAfterImagesLocal);
                                }
                            }

                            if (beforeImagesList.AfterImages.ImagesInfo.Count() <= 0)
                            {
                                DeleteImagePair(beforeImagesList.BeforeImages.Id, false);
                            }
                            if (beforeImagesList.BeforeImages.ImagesInfo.Count() <= 0)
                            {
                                DeleteImagePair(beforeImagesList.BeforeImages.Id, true);
                            }
                            SaveDebuggerLog(debuggerLogModelList);
                        }
                    }
                }
                SaveBuildingImages(model.SliderImages, categoryId);
                SaveInvoiceImages(model.InvoiceImages, categoryId);
            }
            _unitOfWork.SaveChanges();
            model.Id = categoryId;

            AddBeforeAfterImages(model);
            AddExteriorImageImages(model.SliderImages, categoryId);

            return true;
        }
        private void DeleteBeforeAfterImages(List<ImagePairs> sliderDomain, long? categoryId)
        {
            var inDbJobEstimateImageCategory = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId &&
            (x.TypeId == ((long)BeforeAfterImagesType.Before) || x.TypeId == ((long)BeforeAfterImagesType.After) || x.TypeId == ((long)BeforeAfterImagesType.During))).Select(x => (long?)x.Id).ToList();
            var fromUiJobEstimateBeforeImageCategory = sliderDomain.Where(x => x.BeforeImages.Id != 0).Select(x => x.BeforeImages.Id).ToList();
            var fromUiJobEstimateAfterImageCategory = sliderDomain.Where(x => x.AfterImages.Id != 0).Select(x => x.AfterImages.Id).ToList();
            fromUiJobEstimateBeforeImageCategory = fromUiJobEstimateBeforeImageCategory.Union(fromUiJobEstimateAfterImageCategory).ToList();
            var deletedCategoryIds = inDbJobEstimateImageCategory.Except(fromUiJobEstimateBeforeImageCategory);
            foreach (var deletedCategoryId in deletedCategoryIds)
            {
                var jobEstimateService = _jobEstimateServices.Get(deletedCategoryId.GetValueOrDefault());
                if (jobEstimateService != null)
                    _jobEstimateServices.Delete(jobEstimateService);
            }
        }
        private void SaveBuildingImages(JobEstimateServiceViewModel sliderDomain, long categoryId)
        {
            //var categoryList = GetCategoryIdForImage(categoryId);
            var inDbServicesIds = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long)BeforeAfterImagesType.ExteriorBuilding).Select(x => x.Id).ToList();
            var inDbImageIds = _jobEstimateImage.Table.Where(x => x.ServiceId.HasValue && inDbServicesIds.Contains(x.ServiceId.Value)).Select(x => x.FileId).ToList();
            var newImageIds = sliderDomain.ImagesInfo.Select(x => (long?)x.Id).ToList();
            var fileIdsDeleteds = inDbImageIds.Except(newImageIds);
            var fileIdsTobeDeleted = _jobEstimateImage.Table.Where(x => fileIdsDeleteds.Contains(x.FileId)).ToList();
            foreach (var fileIdsDelete in fileIdsDeleteds)
            {
                var jobEstimateImage = fileIdsTobeDeleted.Where(x => x.FileId == fileIdsDelete && x.TypeId == (long)BeforeAfterImagesType.ExteriorBuilding).FirstOrDefault();
                _jobEstimateImage.Delete(jobEstimateImage);
            }
            foreach (var fileId in sliderDomain.FilesList)
            {

                var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, categoryId, (long?)LookupTypes.ExteriorBuilding);
                _jobEstimateServices.Save(jobEstimateBeforeService);
                var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(sliderDomain, buildingExteriorServiceId, (long?)LookupTypes.ExteriorBuilding, fileId);


                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
            }
        }

        private List<long> GetCategoryIdForImage(long? categoryId)
        {
            var jobEstimateServiceDomain = _jobEstimateImageCategory.Get(categoryId.GetValueOrDefault());
            var jobEstimateServiceList = _jobEstimateImageCategory.Table.Where(x => x.EstimateId == jobEstimateServiceDomain.EstimateId).ToList();
            return jobEstimateServiceList.Select(x => x.Id).ToList();
        }
        private void SaveInvoiceImages(JobEstimateServiceViewModel sliderDomain, long categoryId)
        {
            var inDbServicesIds = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && (x.TypeId == (long)BeforeAfterImagesType.Invoice || x.TypeId == (long)LookupTypes.InvoiceImages) && (x.IsFromInvoiceAttach.Value || x.IsInvoiceForJob.Value)).Select(x => x.Id).ToList();
            var inDbImageIds = _jobEstimateImage.Table.Where(x => x.ServiceId.HasValue && inDbServicesIds.Contains(x.ServiceId.Value)).Select(x => x.FileId).ToList();
            var newImageIds = sliderDomain.ImagesInfo.Select(x => (long?)x.Id).ToList();
            var fileIdsDeleteds = inDbImageIds.Except(newImageIds);
            var fileIdsTobeDeleted = _jobEstimateImage.Table.Where(x => fileIdsDeleteds.Contains(x.FileId)).ToList();
            foreach (var fileIdsDelete in fileIdsDeleteds)
            {
                var jobEstimateImage = fileIdsTobeDeleted.FirstOrDefault(x => x.FileId == fileIdsDelete && (x.TypeId == (long)BeforeAfterImagesType.Invoice || x.TypeId == (long)LookupTypes.InvoiceImages));
                if (jobEstimateImage != null)
                    _jobEstimateImage.Delete(jobEstimateImage);
            }
            foreach (var fileId in sliderDomain.FilesList)
            {
                var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, categoryId, (long?)LookupTypes.InvoiceImages);
                jobEstimateBeforeService.IsFromEstimate = sliderDomain.IsFromEstimate;
                if (sliderDomain.IsFromEstimate)
                {
                    jobEstimateBeforeService.IsInvoiceForJob = false;
                    jobEstimateBeforeService.IsFromInvoiceAttach = true;
                }
                else
                {
                    jobEstimateBeforeService.IsInvoiceForJob = true;
                    jobEstimateBeforeService.IsFromInvoiceAttach = false;
                }
                _jobEstimateServices.Save(jobEstimateBeforeService);
                var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(sliderDomain, buildingExteriorServiceId, (long?)LookupTypes.InvoiceImages, fileId);
                _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
            }
        }
        private void DuringAfterBeforeImageDeletion(List<long?> inDbValues, List<long?> changedValue, bool isFromBefore)
        {
            var deletedFilesIds = new List<long?>();

            if (inDbValues.Count() > changedValue.Count())
            {
                deletedFilesIds = inDbValues.Except(changedValue).ToList();
            }
            else
            {
                deletedFilesIds = changedValue.Except(inDbValues).ToList();
            }
            foreach (var deletedFilesId in deletedFilesIds)
            {
                var inDbFileIdRow = _jobEstimateImage.Table.Where(x => x.FileId == deletedFilesId).FirstOrDefault();
                _jobEstimateImage.Delete(inDbFileIdRow);
            }
        }
        public IEnumerable<BeforeAfterImageModel> SaveJobEstimateMediaFiles(FileUploadModel model)
        {
            long? fileId = default(long?);
            List<BeforeAfterImageModel> list = new List<BeforeAfterImageModel>();
            foreach (var fileModel in model.FileList)
            {
                try
                {
                    //if (fileModel.Id > 0)
                    //    continue;


                    var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
                    var destination = MediaLocationHelper.GetDocumentImageLocation();
                    var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                        : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
                    var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
                    fileModel.Name = destFileName + fileModel.Extension;

                    fileModel.RelativeLocation = MediaLocationHelper.GetDocumentImageLocation().Path;

                    //using (Image imgPhoto = Image.FromFile(fileName))
                    //{
                    //    resizeImage(imgPhoto, new Size(10, 10));
                    //}

                    if (fileModel.Extension != ".pdf" && fileModel.Extension != ".docx" && fileModel.Extension != ".doc" && fileModel.Extension != ".xlsx")
                    {
                        //byte[] photoBytes = System.IO.File.ReadAllBytes(fileName);
                        //// Format is automatically detected though can be changed.
                        //using (ImageFactory imageFactory = new ImageFactory(true))
                        //{
                        //    ResizeLayer layer = new ResizeLayer(new Size(500, 0), ResizeMode.Max);
                        //    imageFactory.Load(fileName).Resize(layer).Quality(100).Save(fileName);
                        //}

                        //imgBef = System.Drawing.Image.FromFile(fileName);
                    }
                    var file = _fileService.SaveModel(fileModel);
                    fileId = file.Id;
                    var jobResource = _jobFactory.CreateDomain(model, file.Id);
                    var isIFrame = (fileModel.Extension == ".pdf" || fileModel.Extension == ".docx" || fileModel.Extension == ".xlsx") ? true : false;
                    _jobResourceRepository.Save(jobResource);
                    //Imager.PerformImageResizeAndPutOnCanvas(file.RelativeLocation, file.Name, 600, 400, destFileName  + ".jpg");


                    var a = new BeforeAfterImageModel
                    {
                        FileId = jobResource.FileId,
                        RelativeLocation = (fileModel.RelativeLocation + "\\" + fileModel.Name).ToUrl(),
                        IsIFrame = isIFrame,
                        IFrameLocation = isIFrame ? _settings.SiteRootUrl + "/Media//" + fileModel.Name : ""
                        //IFrameLocation = isIFrame ? destination.Path + "\\" + fileModel.Name : ""
                    };
                    list.Add(a);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return list;
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        private byte[] imgToByteArray(Image img)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                img.Save(mStream, img.RawFormat);
                return mStream.ToArray();
            }
        }
        public void PdfToImage(FileUploadModel model, FileModel fileModel, out List<BeforeAfterImageModel> list)
        {
            list = new List<BeforeAfterImageModel>();
        }

        public BeforeAfterImageModel ExcelToImage(FileUploadModel model, FileModel fileModel, out List<BeforeAfterImageModel> list)
        {
            list = new List<BeforeAfterImageModel>();
            long? fileId = default(long?);
            var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
            var destination = MediaLocationHelper.GetJobMediaLocation();
            var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));

            var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
            fileModel.Name = destFileName + fileModel.Extension;
            fileModel.RelativeLocation = MediaLocationHelper.GetJobMediaLocation().Path;
            var file = _fileService.SaveModel(fileModel);

            var excelFilePath = MediaLocationHelper.GetJobMediaLocation().Path + "//" + fileModel.Name;
            Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
            workbook.LoadFromFile(excelFilePath);
            Spire.Xls.Worksheet sheet = workbook.Worksheets[0];

            var destinationImageFileName = destFileName + ".jpeg";
            var destinationImageFilePath = MediaLocationHelper.GetJobMediaLocation().Path + "//" + destinationImageFileName;
            sheet.SaveToImage(destinationImageFilePath, ImageFormat.Jpeg);

            fileModel.FileReferenceId = file.Id;
            fileModel.Id = 0;
            fileModel.Name = destinationImageFileName;
            fileModel.Caption = destFileName;
            var imageFileModel = _fileService.SaveModel(fileModel);
            fileId = file.Id;
            var jobResource = _jobFactory.CreateDomain(model, imageFileModel.Id);
            _jobResourceRepository.Save(jobResource);
            var a = new BeforeAfterImageModel
            {
                FileId = jobResource.FileId,
                RelativeLocation = (fileModel.RelativeLocation + "\\" + fileModel.Name).ToUrl()
            };

            return a;

        }

        public bool BeforeAfterImageMailSend(long? id, BeforeAfterImageSendMailViewModel model, string templateName, long? franchiseeId)
        {
            var scheduler = _jobschedulerRepository.Get(id.GetValueOrDefault());
            var jobCustomer = scheduler != null && scheduler.Job != null ? scheduler.Job.JobCustomer : scheduler != null && scheduler.Estimate != null ? scheduler.Estimate.JobCustomer : null;
            var emailId = jobCustomer.Email;
            string customerName = jobCustomer != null ? jobCustomer.CustomerName : "";
            var fileName = "BeforeAfterImage_" + DateTime.Now.ToFileTimeUtc() + ".pdf";
            var destinationFolder = MediaLocationHelper.GetAttachmentMediaLocation().Path + "\\";
            var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + templateName);
            var file = _pdfFileService.GeneratePdfFromTemplateAndModel(model, destinationFolder, fileName, viewPath);
            var fileDomain = GetFileModel(file);
            try
            {
                var notificationQueueId = SendingBeforeAfterImagesToCustomer(jobCustomer, scheduler, fileDomain, NotificationTypes.BeforeAfterImages, jobCustomer.Email);
                if (notificationQueueId > 0)
                {
                    var beforeAfterImagesAudit = _jobFactory.CreateBeforeAfterImageMailDomain(scheduler, notificationQueueId, fileDomain.Id, franchiseeId, model);
                    _beforeAfterImageMailAuditRepository.Save(beforeAfterImagesAudit);
                    _logService.Info("notificationQueueId: " + notificationQueueId);
                }

                if (model.MailTo != "")
                {
                    var splittedEmail = model.MailTo.Split(',');
                    foreach (var email in splittedEmail)
                    {
                        SendingBeforeAfterImagesToCustomer(jobCustomer, scheduler, fileDomain, NotificationTypes.BeforeAfterImages, email);

                    }
                }
                else
                {
                    return false;
                }
                jobCustomer.Email = emailId;
                return true;
            }
            catch (Exception e1)
            {
                return false;
            }
        }

        public bool InvoiceMailSend(long? id, JobEstimateServiceViewModel model, string templateName, long? franchiseeId)
        {
            var scheduler = _jobschedulerRepository.Get(id.GetValueOrDefault());
            var jobCustomer = scheduler != null && scheduler.Job != null ? scheduler.Job.JobCustomer : scheduler.Estimate != null ? scheduler.Estimate.JobCustomer : null;
            string customerName = jobCustomer != null ? jobCustomer.CustomerName : "";
            var fileName = "InvoiceImage_" + DateTime.Now.ToFileTimeUtc() + ".pdf";
            var destinationFolder = MediaLocationHelper.GetAttachmentMediaLocation().Path + "\\";
            var viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + templateName);
            var file = _pdfFileService.GeneratePdfFromTemplateAndModel(model, destinationFolder, fileName, viewPath);
            var fileDomain = GetFileModel(file);
            try
            {
                var notificationQueueId = SendingBeforeAfterImagesToCustomer(jobCustomer, scheduler, fileDomain, NotificationTypes.InvoiceImages, jobCustomer.Email);
                if (notificationQueueId > 0)
                {
                    var beforeAfterImagesAudit = _jobFactory.CreateBeforeAfterImageMailDomain(scheduler, notificationQueueId, fileDomain.Id, franchiseeId, null);
                    _beforeAfterImageMailAuditRepository.Save(beforeAfterImagesAudit);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e1)
            {
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

        public long? SendingBeforeAfterImagesToCustomer(JobCustomer model, JobScheduler scheduler, Application.Domain.File fileDomain, NotificationTypes notificationId, string emailId)
        {
            var notificaitonId = _sendNewJobNotificationToTechService.SendBeforeAfterImagestoCustomer(model, scheduler, fileDomain, notificationId, emailId);
            return notificaitonId;
        }

        public bool IsEligibleForDeletion(BeforeAfterImageDeletionViewModel model)
        {
            var uploadedByUserId = _organizationRoleUserRepository.Table.Where(x => x.UserId == model.UserId).Select(x => x.RoleId).FirstOrDefault();

            if (uploadedByUserId == (long?)RoleType.SuperAdmin || uploadedByUserId == (long?)RoleType.FrontOfficeExecutive)
            {
                return false;
            }
            else
            {
                if (model.RoleId == (long?)RoleType.FranchiseeAdmin && (DateTime.Now - model.UploadDateTime).TotalDays <= 7)
                {
                    return true;
                }
                else if ((model.RoleId == (long?)RoleType.SalesRep || model.RoleId == (long?)RoleType.Technician) && ((_clock.ToUtc(DateTime.Now) - model.UploadDateTime).TotalDays <= 1))
                {
                    if (uploadedByUserId == (long?)RoleType.FranchiseeAdmin || uploadedByUserId == (long?)RoleType.SuperAdmin || uploadedByUserId == (long?)RoleType.FrontOfficeExecutive)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public long? AttachInvoiceWithJob(InvoiceDetailsForAttactmentViewModel model, string templateName, long? franchiseeId, bool isFromJob)
        {
            var fileName = "Invoice_" + DateTime.Now.ToFileTimeUtc() + ".pdf";
            var viewPath = "";
            var destinationFolder = MediaLocationHelper.GetDocumentImageLocation().Path + "\\";
            if (!isFromJob)
            {
                viewPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Templates\" + templateName);
            }
            else
            {
                viewPath = _settings.TemplateRootPath + "\\" + "invoice-job-attacktment.cshtml";
            }
            var file = _pdfFileService.GeneratePdfFromTemplateAndModel(model, destinationFolder, fileName, viewPath);
            var fileDomain = GetFileModel(file);
            try
            {
                return fileDomain.Id;
            }
            catch (Exception e1)
            {
                return null;
            }
        }

        public JobListModel GetHolidayListMonthWise(FranchiseeHolidayModel model)
        {
            var list = new JobListModel { };
            var franchisee = _franchiseeRepository.Get(model.FranchiseeId.GetValueOrDefault());
            if (franchisee == null)
                return list;

            var address = franchisee.Organization.Address.FirstOrDefault();
            if (address == null)
                return list;
            var startDate = new DateTime(model.EndDate.Year, model.EndDate.Month, 1).AddMonths(-1);
            var endDate = startDate.AddMonths(1);
            var countryId = address.CountryId;
            var holidayList = _holidayRepository.Table.Where(h => h.CountryId != null && h.CountryId == countryId
            && (h.StartDate >= startDate && h.EndDate < endDate)).ToList();
            if (holidayList.Count == 0)
            {
                holidayList = _holidayRepository.Table.Where(h => h.CountryId == null || h.CountryId == countryId
                                && (h.StartDate >= startDate && h.EndDate < endDate)).ToList();
            }
            list.Collection = holidayList.Select(x => _jobFactory.CreateViewModel(x, franchisee));
            return list;
        }

        public bool CheckAvailabilityForMeeting(long id, long assigneeId, DateTime startDate, DateTime endDate, JobEstimateEditModel model)
        {
            var orgRoleUser = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.UserId == assigneeId);
            var startDateToCompare = startDate.AddMinutes(-14);
            var assigneeInfo = new List<JobScheduler>();
            //assigneeInfo = _jobSchedulerRepository.Table.Where(x => assigneeId == x.AssigneeId && (id <= 0 || (x.JobId != id && x.EstimateId != id && x.MeetingID != id))
            //                    && x.IsActive).ToList();
            if (orgRoleUser != null)
            {
                if (id == 0)
                {
                    assigneeInfo = _jobSchedulerRepository.Table.Where(x => (x.PersonId == orgRoleUser.UserId) && x.IsActive && (x.StartDate >= startDateToCompare || x.EndDate <= endDate)).ToList();
                }
                else
                {
                    assigneeInfo = _jobSchedulerRepository.Table.Where(x => (x.PersonId == orgRoleUser.UserId) && (x.MeetingID != null) && x.MeetingID != model.MeetingID && (x.StartDate >= startDateToCompare || x.EndDate <= endDate) && x.Id != id
                                   && x.IsActive).ToList();
                }
            }
            else
            {
                assigneeInfo = _jobSchedulerRepository.Table.Where(x => assigneeId == x.AssigneeId && (id <= 0 || (x.MeetingID != id && x.EstimateId != id && x.JobId != id))
                                    && x.IsActive).ToList();
            }

            if (!assigneeInfo.Any())
                return true;

            assigneeInfo = assigneeInfo.Where(x => (startDateToCompare <= x.StartDate && endDate >= x.StartDate)
                        || (startDateToCompare <= x.EndDate && endDate >= x.EndDate)
                        || (startDateToCompare >= x.StartDate && endDate <= x.EndDate)).ToList();

            if (!assigneeInfo.Any())
                return true;
            return false;
        }

        public FranchiseInfoModel GetFranchiseeInfo(long? franchiseeId)
        {
            var organizationDomain = _organizationRepository.Table.FirstOrDefault(x => x.Id == franchiseeId);
            var address = organizationDomain.Address != null ? organizationDomain.Address.FirstOrDefault() : null;
            if (address != null)
            {
                if (address.StateId == null)
                {
                    address.StateId = _stateRepository.Table.Where(x => x.Name == address.StateName || x.ShortName == address.StateName).Select(x => x.Id).FirstOrDefault();
                }
                var franchiseInfoModel = _jobFactory.CreateViewModel(address);
                return franchiseInfoModel;
            }
            return null;
        }

        public DragDropSchedulerEnum SaveDragDropEvent(DragDropSchedulerModel model)
        {
            var id = default(long?);
            var isAvailable = true;
            var endDate = default(DateTime?);
            var startDate = default(DateTime?);
            var scheduler = _jobSchedulerRepository.Table.FirstOrDefault(x => x.Id == model.Id);
            id = scheduler.JobId != null ? scheduler.JobId : scheduler.EstimateId;
            if (id == default(long?))
            {
                id = scheduler.MeetingID;
            }
            if (scheduler != null)
            {
                startDate = scheduler.StartDate;
                endDate = scheduler.EndDate;
                startDate = startDate.GetValueOrDefault().AddDays(model.Days.GetValueOrDefault()).
                    AddMilliseconds(model.Seconds.GetValueOrDefault());
                endDate = endDate.GetValueOrDefault().AddDays(model.Days.GetValueOrDefault()).
                    AddMilliseconds(model.Seconds.GetValueOrDefault());
                if (id != null || id != 0)
                {
                    isAvailable = CheckAvailabilityForJob(id.GetValueOrDefault(), scheduler.AssigneeId.GetValueOrDefault(),
                        startDate.GetValueOrDefault(), endDate.GetValueOrDefault(), false);
                }
                if (!isAvailable)
                {
                    return DragDropSchedulerEnum.AlreadyAssigned;
                }
                else
                {
                    scheduler.StartDateTimeString = scheduler.StartDateTimeString.AddDays(model.Days.GetValueOrDefault()).
                         AddMilliseconds(model.Seconds.GetValueOrDefault());
                    scheduler.EndDateTimeString = scheduler.EndDateTimeString.AddDays(model.Days.GetValueOrDefault()).
                         AddMilliseconds(model.Seconds.GetValueOrDefault());
                    scheduler.StartDate = startDate.GetValueOrDefault();
                    scheduler.EndDate = endDate.GetValueOrDefault();
                    scheduler.IsCancellationMailSend = true;
                    _jobschedulerRepository.Save(scheduler);


                    DeleteJobSchedulerAudit(scheduler);
                    if (scheduler.JobId != null)
                    {
                        SendingJobMails(scheduler);
                    }
                    bool isSales = (_organizationRoleUserRepository.Fetch(x => x.Id == scheduler.AssigneeId && x.Role.Id == (long)RoleType.SalesRep)).Any();

                    if (isSales && scheduler.EstimateId != null)
                    {
                        SendingEstimateMails(scheduler);
                    }
                    return DragDropSchedulerEnum.Changed;
                }
            }

            return default;
        }

        private void SendingJobMails(JobScheduler scheduler)
        {
            var jobEditModel = _jobFactory.CreateEditModel(scheduler.Job, default(JobScheduler));
            jobEditModel.StartDate = scheduler.StartDate;
            jobEditModel.EndDate = scheduler.EndDate;
            jobEditModel.ActualEndDateString = scheduler.EndDateTimeString;
            jobEditModel.ActualStartDateString = scheduler.StartDateTimeString;
            _estimateService.SendingJobUpdationMails(jobEditModel, scheduler.AssigneeId.GetValueOrDefault());
        }
        private void SendingEstimateMails(JobScheduler scheduler)
        {
            var jobEstimateEditModel = _jobFactory.CreateEstimateModel(scheduler.Job != null ? scheduler.Job.JobEstimate : scheduler.Estimate, scheduler);
            _estimateService.SendingUpdationMails(jobEstimateEditModel, jobEstimateEditModel.SalesRepId.GetValueOrDefault());
        }

        public ConfirmationResponseModel ConfirmSchedule(ConfirmationModel model)
        {
            var isFromConformation = true;
            model.EncryptedData = model.EncryptedData.Replace("LOOP", "%");
            var decruptedData = "";
            var schedulerList = new List<JobScheduler>();
            var confirmationResponseModel = new ConfirmationResponseModel();
            if (model.SchedulerId == null || model.SchedulerId == 0)
            {
                try
                {
                    decruptedData = EncryptionHelper.UrlDecrypt(model.EncryptedData);
                    decruptedData = EncryptionHelper.Decrypt(decruptedData);
                }
                catch (Exception e1)
                {
                    decruptedData = EncryptionHelper.Decrypt(model.EncryptedData);
                }

                model.SchedulerId = long.Parse(decruptedData);
            }
            var schdeulerDomain = _jobschedulerRepository.Get(model.SchedulerId.GetValueOrDefault());

            if (schdeulerDomain == null)
            {
                confirmationResponseModel.ConfirmationEnum = ConfirmationEnum.InvalidId;
                return confirmationResponseModel;
            }
            if (schdeulerDomain.StartDate <= DateTime.Now)
            {
                return GetConfirmationReponseModel(schdeulerDomain, ConfirmationEnum.PastScheduler);
            }
            if (schdeulerDomain.SchedulerStatus == (long)ConfirmationEnum.Confirmed && model.Status == true)
            {
                return GetConfirmationReponseModel(schdeulerDomain, ConfirmationEnum.AlreadyConfirmed);
            }
            if (schdeulerDomain.Job != null)
            {
                schedulerList.Add(schdeulerDomain);
                var jobSchedulerList = schdeulerDomain.Job.JobScheduler;
                if (jobSchedulerList.Count() > 0)
                {
                    var startDate = _clock.ToLocal(schdeulerDomain.StartDate, schdeulerDomain.Offset.GetValueOrDefault(), 1).ToLongDateString();
                    var endDate = _clock.ToLocal(schdeulerDomain.EndDate, schdeulerDomain.Offset.GetValueOrDefault(), 1).ToLongDateString();
                    foreach (var jobScheduler in jobSchedulerList)
                    {
                        var schedulerStartDate = _clock.ToLocal(jobScheduler.StartDate, jobScheduler.Offset.GetValueOrDefault(), 1).ToLongDateString();
                        var schedulerEndDate = _clock.ToLocal(jobScheduler.EndDate, jobScheduler.Offset.GetValueOrDefault(), 1).ToLongDateString();
                        if (startDate == schedulerStartDate)
                        {
                            schedulerList.Add(jobScheduler);
                        }
                    }
                }
            }
            else
            {
                schedulerList.Add(schdeulerDomain);
            }
            foreach (var scheduler in schedulerList)
            {
                if (model.Status == true)
                {
                    isFromConformation = true;
                    scheduler.SchedulerStatus = (long)ConfirmationEnum.Confirmed;
                    scheduler.IsCancellationMailSend = true;
                }
                else
                {
                    isFromConformation = false;
                    scheduler.SchedulerStatus = (long)ConfirmationEnum.NotConfirmed;
                    scheduler.IsCancellationMailSend = false;
                }
                _jobSchedulerRepository.Save(scheduler);
            }
            if (!isFromConformation)
            {
                return GetConfirmationReponseModel(schdeulerDomain, ConfirmationEnum.NotConfirmed);
            }
            else
            {
                return GetConfirmationReponseModel(schdeulerDomain, ConfirmationEnum.Confirmed);
            }


        }

        private ConfirmationResponseModel GetConfirmationReponseModel(JobScheduler schdeulerDomain, ConfirmationEnum status)
        {
            ConfirmationResponseModel confirmationResponseModel = new ConfirmationResponseModel();
            confirmationResponseModel.ConfirmationEnum = status;
            confirmationResponseModel.CustomerName = schdeulerDomain.JobId != null ? schdeulerDomain.Job.JobCustomer.CustomerName : (schdeulerDomain.Estimate.JobCustomer.CustomerName);
            confirmationResponseModel.StartDateTime = schdeulerDomain.StartDateTimeString.ToString("d");
            confirmationResponseModel.EndDateTIme = schdeulerDomain.EndDateTimeString;
            confirmationResponseModel.SchedulerType = schdeulerDomain.JobId != null ? "Job" : "Estimate";
            return confirmationResponseModel;
        }

        public ConfirmationResponseModel ConfirmScheduleFromUI(ConfirmationModel model)
        {
            var decruptedData = "";
            var confirmationResponseModel = new ConfirmationResponseModel();
            if (model.SchedulerId == null || model.SchedulerId == 0)
            {
                try
                {
                    decruptedData = EncryptionHelper.UrlDecrypt(model.EncryptedData);
                    decruptedData = EncryptionHelper.Decrypt(decruptedData);
                }
                catch (Exception e1)
                {
                    decruptedData = EncryptionHelper.Decrypt(model.EncryptedData);
                }

                model.SchedulerId = long.Parse(decruptedData);
            }
            var schdeulerDomain = _jobschedulerRepository.Get(model.SchedulerId.GetValueOrDefault());
            if (schdeulerDomain == null)
            {
                confirmationResponseModel.ConfirmationEnum = ConfirmationEnum.InvalidId;
                return confirmationResponseModel;
            }
            if (schdeulerDomain.StartDate <= DateTime.Now)
            {
                return GetConfirmationReponseModel(schdeulerDomain, ConfirmationEnum.PastScheduler);
            }
            if (schdeulerDomain.SchedulerStatus == (long)ConfirmationEnum.Confirmed)
            {
                schdeulerDomain.SchedulerStatus = (long)ConfirmationEnum.NotConfirmed;
                schdeulerDomain.IsCancellationMailSend = true;
                _jobSchedulerRepository.Save(schdeulerDomain);
                return GetConfirmationReponseModel(schdeulerDomain, ConfirmationEnum.NotConfirmed);
            }
            schdeulerDomain.SchedulerStatus = (long)ConfirmationEnum.Confirmed;
            schdeulerDomain.IsCancellationMailSend = true;
            _jobSchedulerRepository.Save(schdeulerDomain);


            return GetConfirmationReponseModel(schdeulerDomain, ConfirmationEnum.Confirmed);

        }
        private void ChangingJobDetails(JobEditModel model)
        {
            if (model.IsDataToBeUpdateForAllJobs)
            {
                var todayDate = _clock.ToUtc(DateTime.Now).Date;
                var jobList = _jobDetailsRepository.Table.Where(x => x.JobId == model.JobId).ToList();
                foreach (var job in jobList)
                {

                    if (job.SchedulerId == model.Id)
                    {
                        job.Description = model.Description;
                        _jobDetailsRepository.Save(job);
                    }
                    else
                    {
                        //job.Description += " " + model.Description;
                        job.Description = model.Description;
                        _jobDetailsRepository.Save(job);
                    }
                }
            }
            else
            {
                var jobDetails = _jobDetailsRepository.Table.FirstOrDefault(x => x.SchedulerId == model.Id);
                if (jobDetails != default)
                {
                    jobDetails.Description = model.Description;
                    _jobDetailsRepository.Save(jobDetails);
                }

            }
        }

        private void SavingJobDetails(JobEditModel model, List<long?> schedulerIds, long? schedulerId)
        {
            if (model.JobOccurence != null && model.JobOccurence.Collection.Count() > 0)
            {
                foreach (var scheduler in model.JobOccurence.Collection)
                {
                    SaveJobDetails(scheduler.ScheduleId, schedulerId.GetValueOrDefault(), model);
                }
            }
            else
            {
                foreach (var scheduler in schedulerIds)
                {
                    SaveJobDetails(scheduler.GetValueOrDefault(), schedulerId.GetValueOrDefault(), model);
                }

            }
        }
        private void DeleteJobSchedulerAudit(JobScheduler scheduler)
        {
            var schedulerAuditDomain = _jobReminderAuditRepository.Table.FirstOrDefault(x => x.JobSchedulerId == scheduler.Id);
            if (schedulerAuditDomain != null)
            {
                _jobReminderAuditRepository.Delete(schedulerAuditDomain);
            }
        }

        private void SaveJobDetails(long schedulerId, long jobSchedulerId, JobEditModel model)
        {
            var jobDetail = _jobDetailsRepository.Table.FirstOrDefault(x => x.SchedulerId == schedulerId);
            if (jobDetail == null)
            {
                var job = _jobRepository.Get(model.JobId);
                var jobDetailDomain = _jobDetailsFactory.CreateDomain(job);
                jobDetailDomain.SchedulerId = schedulerId;
                jobDetailDomain.IsNew = true;
                //if (model.IsDataToBeUpdateForAllJobs)
                //    jobDetailDomain.Description += " " + model.Description;
                //else
                //{
                //        jobDetailDomain.Description = job.Description;
                //}

                _jobDetailsRepository.Save(jobDetailDomain);
            }
            else
            {
                //if (model.IsDataToBeUpdateForAllJobs)
                //{
                //    jobDetail.Description += " " + model.Description;
                //}
                //else
                //{
                //    if (jobSchedulerId == schedulerId)
                //    {
                //        jobDetail.Description = model.Description;
                //    }
                //}
                //_jobDetailsRepository.Save(jobDetail);
            }
        }

        private OldSchedulerModel GetOldScheduleModel(JobScheduler jobScheduler)
        {
            return new OldSchedulerModel()
            {
                EndDate = jobScheduler.EndDateTimeString,
                StartDate = jobScheduler.StartDateTimeString,
                Id = jobScheduler.Id,
                AssigneeId = jobScheduler.AssigneeId,
                StartDateUtc = jobScheduler.StartDate,
                EndDateUtc = jobScheduler.EndDate
            };
        }

        private void ChangingJobScheduler(long jobSchedulerID, DateTime startDate, DateTime endDate,
            DateTime startDateString, DateTime endDateString)
        {
            var jobScheduler = _jobschedulerRepository.Get(jobSchedulerID);
            jobScheduler.StartDate = startDate;
            jobScheduler.EndDate = endDate;
            jobScheduler.StartDateTimeString = startDateString;
            jobScheduler.EndDateTimeString = endDateString;
            jobScheduler.IsNew = false;
            jobScheduler.IsCancellationMailSend = true;
            _jobschedulerRepository.Save(jobScheduler);
        }

        public bool EditJobNotes(JobNoteEditModel model)
        {
            var jobNote = _jobNoteRepository.Get(model.Id.GetValueOrDefault());
            jobNote.Note = model.JobNote;
            jobNote.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            _jobNoteRepository.Save(jobNote);
            return true;
        }

        public bool DeleteNotes(long? Id)
        {
            var jobNote = _jobNoteRepository.Get(Id.GetValueOrDefault());
            _jobNoteRepository.Delete(jobNote);
            return true;
        }

        private void SendMailToCustomer(long? schedulerId)
        {

            var scheduler = _jobschedulerRepository.IncludeMultiple(x => x.Franchisee).FirstOrDefault(x => x.Id == schedulerId);
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = scheduler.StartDateTimeString.Date;
            var endDateToBeCompared = scheduler.EndDateTimeString.Date;
            var startDate = _clock.UtcNow.Date.AddDays(1);
            var endDate = _clock.UtcNow.Date.AddDays(2);
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _userNotificationModelFactory.ScheduleReminderNotification(scheduler, startDate, endDate, "", NotificationTypes.NewCustomerMail);
        }

        private void SendUpdationMailToCustomer(long? schedulerId)
        {
            var scheduler = _jobschedulerRepository.IncludeMultiple(x => x.Franchisee).FirstOrDefault(x => x.Id == schedulerId);
            var startDate = _clock.UtcNow.Date.AddDays(1);
            var endDate = _clock.UtcNow.Date.AddDays(2);
            _userNotificationModelFactory.ScheduleReminderNotification(scheduler, startDate, endDate, "", NotificationTypes.UpdateCustomerMail);
        }


        public CustomerInfoModel GetCustomerInfo(string customerName)
        {
            var splittedName = "";
            var split = customerName.Split(' ');
            if (split.Length > 1)
            {
                splittedName = split[1] + ", " + split[0];
            }
            else
            {
                splittedName = customerName;
            }
            var customerDomain = _customerRepository.Table.FirstOrDefault(x => x.ContactPerson.Contains(splittedName) || x.Name.Contains(splittedName));
            if (customerDomain == null)
            {
                customerDomain = _customerRepository.Table.FirstOrDefault(x => x.ContactPerson.Contains(customerName) || x.Name.Contains(customerName));
            }

            if (customerDomain == null) return default;
            var address = customerDomain.Address != null ? customerDomain.Address : null;
            if (address != null)
            {
                if (address.StateId == null)
                {
                    address.StateId = _stateRepository.Table.Where(x => x.Name == address.StateName || x.ShortName == address.StateName).Select(x => x.Id).FirstOrDefault();
                }
                var customerInfoModel = _jobFactory.CreateViewModelForCustomer(address);
                customerInfoModel.CustomerName = customerDomain.Name;
                customerInfoModel.PhoneNumber = customerDomain.Phone;
                customerInfoModel.Email = customerDomain.CustomerEmails.Count() > 0 ? customerDomain.CustomerEmails.FirstOrDefault().Email : "";
                return customerInfoModel;
            }
            return null;
        }

        private BeforeAfterModel CreateBeforeAfterModel(JobEstimateImage domain)
        {
            return new BeforeAfterModel
            {
                ServiceId = domain.ServiceId,
                Id = domain.Id
            };
        }


        private bool GetImageDeletedIds(List<JobEstimateServiceViewModel> imagesFromModelListFromDb, List<ImagePairs> imagePairs)
        {
            var modelPresentInBothModelAndDb = new List<JobEstimateServiceViewModel>();
            foreach (var imagePair in imagePairs)
            {
                var afterFileId = imagePair.AfterImages.ImagesInfo.Select(x => x.FileId);
                var beforeFileId = imagePair.BeforeImages.ImagesInfo.Select(x => x.FileId);
                var jobEstimateImageBeforeModelFromDb = imagesFromModelListFromDb.FirstOrDefault(x => x.Id == imagePair.BeforeImages.Id);
                var jobEstimateImageAfterModelFromDb = imagesFromModelListFromDb.FirstOrDefault(x => x.Id == imagePair.AfterImages.Id);

                if (jobEstimateImageBeforeModelFromDb != null)
                {
                    if (jobEstimateImageBeforeModelFromDb != beforeFileId)
                    {
                        var jobEstimateImage = _jobEstimateImage.Table.FirstOrDefault(x => x.Id == jobEstimateImageBeforeModelFromDb.Id);
                        //_jobEstimateImage.Delete(jobEstimateImage);
                    }
                }

                if (jobEstimateImageAfterModelFromDb != null)
                {
                    if (jobEstimateImageAfterModelFromDb != afterFileId)
                    {
                        var jobEstimateImage = _jobEstimateImage.Table.FirstOrDefault(x => x.Id == jobEstimateImageAfterModelFromDb.Id);
                        //_jobEstimateImage.Delete(jobEstimateImage);
                    }
                }
            }
            return true;
        }
        private bool DeleteImagePair(long? pairId, bool isFromBefore)
        {
            if (!isFromBefore)
            {
                var jobEstimateService = _jobEstimateServices.Table.FirstOrDefault(x => x.PairId == pairId);
                var jobImage = jobEstimateService != null ? _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == jobEstimateService.Id) : null;
                if (jobImage != null)
                {
                    _jobEstimateImage.Delete(jobImage);
                }
            }
            else
            {
                var jobEstimateService = _jobEstimateServices.Table.FirstOrDefault(x => x.Id == pairId);
                var jobImage = jobEstimateService != null ? _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == jobEstimateService.Id) : null;
                if (jobImage != null)
                {
                    _jobEstimateImage.Delete(jobImage);
                }
            }
            return true;
        }

        private bool AddorCheckForBeforeAfterBestPicture(long? serviceId, BeforeAfterPairType beforeAfterPairType, BeforeAfterBestPairType beforeAfterBestPairType, bool isFromJobEstimate)
        {
            try
            {
                var markBeforeAfterImageHistoryDomain = _markbeforeAfterImagesHistryRepository.Table.FirstOrDefault(x => x.ServiceId == serviceId && x.BestTypeId == (long)beforeAfterPairType);
                if (markBeforeAfterImageHistoryDomain == null)
                {
                    var domain = new MarkbeforeAfterImagesHistry
                    {
                        ServiceId = serviceId,
                        DataRecorderMetaData = new DataRecorderMetaData(),
                        IsNew = true,
                        BestTypeId = (long)beforeAfterPairType,
                        CategoryId = (long)beforeAfterBestPairType,
                        IsFromJobEstimate = isFromJobEstimate
                    };
                    _markbeforeAfterImagesHistryRepository.Save(domain);
                }
                else if (markBeforeAfterImageHistoryDomain != null && BeforeAfterPairType.REVIEWMARKETINGTYPE == beforeAfterPairType)
                {
                    DeleteorCheckForBeforeAfterBestPicture(serviceId, beforeAfterPairType, beforeAfterBestPairType);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool DeleteorCheckForBeforeAfterBestPicture(long? serviceId, BeforeAfterPairType beforeAfterPairType, BeforeAfterBestPairType beforeAfterBestPairType)
        {
            try
            {
                var markBeforeAfterImageHistoryDomain = _markbeforeAfterImagesHistryRepository.Table.FirstOrDefault(x => x.ServiceId == serviceId && x.BestTypeId == (long)beforeAfterPairType && x.CategoryId == (long)beforeAfterBestPairType);
                if (markBeforeAfterImageHistoryDomain != null)
                {
                    _markbeforeAfterImagesHistryRepository.Delete(markBeforeAfterImageHistoryDomain);
                }

                var jobEstimateImage = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == serviceId);
                if (jobEstimateImage != null)
                {
                    jobEstimateImage.IsAddToLocalGallery = false;
                    jobEstimateImage.AddToGalleryDateTime = null;
                    _jobEstimateImage.Save(jobEstimateImage);


                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ReviewMarketingImageViewModel GetBeforeAfterImages(LocalMarketingReviewFilter filter)
        {
            //var listModel = new BeforeAfterImageListModel();
            //var startDate = default(DateTime?);
            //var isReview = false;
            //var isFromNull = false;
            //if (filter.StartDate == null)
            //{
            //    //DateTime date = new DateTime(2019, 12, 1);
            //    DateTime date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            //    int day = (int)date.DayOfWeek;
            //    DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
            //    DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
            //    filter.StartDate = Monday.AddDays(-7);
            //    filter.EndDate = Sunday.AddDays(-6);
            //    isFromNull = true;
            //}
            //else
            //{
            //    DateTime date = new DateTime(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month,
            //        filter.StartDate.GetValueOrDefault().Day);
            //    int day = (int)date.DayOfWeek;
            //    if (day == 0)
            //    {
            //        filter.StartDate = filter.StartDate.GetValueOrDefault().AddDays(1).Date;
            //        filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(6).Date;
            //    }
            //    else
            //    {
            //        DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
            //        DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
            //        filter.StartDate = Monday;
            //        filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(6);
            //    }
            //}

            //var jobEstimateImages = _jobEstimateImage.Table.Where(x => ((x.BestFitMarkDateTime >= filter.StartDate && x.BestFitMarkDateTime <= filter.EndDate)
            //  || (x.AddToGalleryDateTime >= filter.StartDate && x.AddToGalleryDateTime <= filter.EndDate))
            //  && (filter.SurfaceMaterial == "" || x.JobEstimateServices.SurfaceMaterial == filter.SurfaceMaterial)
            //  && (filter.FinishMaterial == "" || x.JobEstimateServices.FinishMaterial == filter.FinishMaterial)
            //  && (filter.BuildingType == "" || x.JobEstimateServices.BuildingLocation == filter.BuildingType)
            //  && (filter.ServiceTypeId == 0 || x.JobEstimateServices.ServiceTypeId == filter.ServiceTypeId)
            //  && (filter.SurfaceColor == "" || x.JobEstimateServices.SurfaceColor == filter.SurfaceColor)
            //  && (filter.ManagementCompany == "" || x.JobEstimateServices.CompanyName == filter.ManagementCompany)
            //  && (filter.MaidService == "" || x.JobEstimateServices.MaidService == filter.MaidService)
            //  && (filter.SurfaceTypeId == null || x.JobEstimateServices.SurfaceType == filter.SurfaceTypeId)
            //  && (filter.BuildingLocation == "" || x.JobEstimateServices.BuildingLocation == filter.BuildingLocation)
            //  && ((filter.MarketingClassId == 0) || (x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Job != null ? x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Job.JobTypeId == filter.MarketingClassId : x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.Estimate.TypeId == filter.MarketingClassId))
            //  && (filter.FranchiseeId == 0 || x.JobEstimateServices.JobEstimateImageCategory.JobScheduler.FranchiseeId == filter.FranchiseeId)).ToList();

            //jobEstimateImages = jobEstimateImages.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
            //var jobEstimateServicesId = jobEstimateImages.Select(x => x.ServiceId).ToList();
            //var jobEstimateServiceList = _jobEstimateServices.Table.Where(x => jobEstimateServicesId.Contains(x.Id) && x.PairId != null).ToList();
            //var jobEstimateServicesIdWithPair = jobEstimateServiceList.Where(x => jobEstimateServicesId.Contains(x.Id)).Select(x1 => x1.Id).ToList();
            //jobEstimateImages = jobEstimateImages.Where(x => jobEstimateServicesIdWithPair.Contains(x.ServiceId.Value)).OrderByDescending(x => x.BestFitMarkDateTime).ToList();

            //var jobEstimateImagesList = _jobEstimateImage.Table.Where(x => jobEstimateServicesId.Contains(x.ServiceId)).ToList();
            //var markbeforeAfterImagesHistryList = _markbeforeAfterImagesHistryRepository.Table.Where(x => jobEstimateServicesId.Contains(x.ServiceId)).ToList();
            //var jobEstimateServicesList = _jobEstimateServices.Table.Where(x => jobEstimateServicesId.Contains(x.Id)).ToList();
            //int index = 0;
            //var orgnRoleUserList = _organizationRoleUserRepository.Table.ToList();
            //var beforeAfterViewModel = jobEstimateImages.Select(x => _jobFactory.CreateBeforeAfterViewModel(x, GetNonPairService(x.ServiceId, jobEstimateServicesList), jobEstimateImagesList, markbeforeAfterImagesHistryList, ++index, orgnRoleUserList)).ToList();

            //if (filter.StartDate != null)
            //{
            //    startDate = filter.StartDate;

            //}
            //if (isFromNull)
            //{
            //    isFromNull = false;
            //    filter.EndDate = filter.EndDate.GetValueOrDefault().AddDays(-1);
            //}

            //var reviewMarketingImageDomain = _reviewMarketingImageLastDateHistryRepository.Table.FirstOrDefault(x => x.StartDate == filter.StartDate && x.EndDate == filter.EndDate);

            //if (reviewMarketingImageDomain != null)
            //{
            //    isReview = reviewMarketingImageDomain.IsReview.GetValueOrDefault();
            //}

            //return new BeforeAfterImageListModel()
            //{
            //    StartDate = filter.StartDate == null ? startDate.GetValueOrDefault().Date : filter.StartDate.Value.Date,
            //    Endate = filter.StartDate == null ? DateTime.Now.Date : filter.EndDate.Value.Date,
            //    BeforeAfterViewModel = beforeAfterViewModel.Distinct().ToList(),
            //    TotalCount = jobEstimateImages.Count(),
            //    IsReview = isReview
            //};
            ReviewMarketingImageViewModel reviewMarketingImageViewModel = new ReviewMarketingImageViewModel();
            try
            {
                if (!filter.IsDateFilter)
                {
                    if (filter.StartDate == null)
                    {
                        DateTime date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                        int day = (int)date.DayOfWeek;
                        DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                        DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                        filter.StartDate = Monday.AddDays(-7);
                        filter.EndDate = Sunday.AddDays(-7);
                    }
                    else
                    {
                        DateTime date = new DateTime(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month,
                            filter.StartDate.GetValueOrDefault().Day);
                        int day = (int)date.DayOfWeek;
                        if (day == 0)
                        {
                            filter.StartDate = filter.StartDate.GetValueOrDefault().AddDays(1).Date;
                            filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(6).Date;
                        }
                        else
                        {
                            DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                            DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                            filter.StartDate = Monday;
                            filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(7);
                        }
                    }
                }

                if (filter.FranchiseeId == null)
                {

                    if (filter.RoleId == (long?)RoleType.SuperAdmin || filter.RoleId == (long?)RoleType.FrontOfficeExecutive)
                    {
                        filter.FranchiseeId = null;
                    }
                    else
                    {
                        filter.FranchiseeId = filter.LoggedInFranchiseeId;
                    }
                }
                var fileList = _fileRepository.Table.ToList();
                var SalesRepList = _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.RoleId == (long)RoleType.SalesRep).OrderBy(y => y.Person.FirstName).ToList();
                var TechnicianList = _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.RoleId == (long)RoleType.Technician).OrderBy(y => y.Person.FirstName).ToList();
                var organizationRollUserList = SalesRepList.Concat(TechnicianList).ToList();

                //var orgRoleUserIds = organizationRollUserList.Select(x => x.UserId).ToList();
                //var activeUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && !x.IsLocked).Select(x => x.Id).ToList();
                //organizationRollUserList = organizationRollUserList.Where(x => activeUers.Contains(x.UserId)).ToList();

                var orgRoleUserIds = organizationRollUserList.Select(x => x.UserId).ToList();
                var inActiveUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && x.IsLocked).Select(x => x.Id).ToList();

                var jobEstimateData = _jobEstimateRepository.Table.Where(x => x.StartDate >= filter.StartDate && x.EndDate <= filter.EndDate).ToList();
                var beforeAfter = GetBeforeAfterImagesListForReviewMarketing(filter);

                var jobSchedulerList = GetJobSchedulerList(filter, beforeAfter);

                var franchiseeList = GetFranchiseeList(filter, organizationRollUserList, jobSchedulerList);

                reviewMarketingImageViewModel.StartDate = filter.StartDate;
                reviewMarketingImageViewModel.EndDate = filter.EndDate;
                reviewMarketingImageViewModel.IsFranchiseeAdmin = filter.RoleId == (long?)RoleType.FranchiseeAdmin ? true : false;
                foreach (var franchisee in franchiseeList)
                {
                    ReviewMarketingFranchiseeViewModel reviewMarketingFranchiseeViewModel = new ReviewMarketingFranchiseeViewModel();
                    reviewMarketingFranchiseeViewModel.FranchiseeId = franchisee.Id;
                    reviewMarketingFranchiseeViewModel.FranchiseeName = franchisee.Organization.Name;
                    reviewMarketingImageViewModel.ReviewMarketingFranchiseeViewModels.Add(reviewMarketingFranchiseeViewModel);
                }

                foreach (var franchisee in reviewMarketingImageViewModel.ReviewMarketingFranchiseeViewModels)
                {

                    var users = GetUsersListForReviewMarketing(filter, organizationRollUserList, franchisee);
                    if (users.Count() == 0)
                    {
                        franchisee.PersonCount = 0;
                        franchisee.Message = "No SalesRep/Technician Available In This Franchisee.";
                    }
                    else
                    {
                        franchisee.PersonCount = users.Count();
                        franchisee.Message = "SalesRep/Technician Are Available In This Franchisee.";

                        foreach (var user in users)
                        {
                            var isActiveUser = inActiveUers.Any(x => x == user.UserId);
                            ReviewMarketingPersonViewModel person = new ReviewMarketingPersonViewModel();
                            person.Id = user.Id;
                            person.PersonId = user.Person.Id;
                            person.PersonName = user.Person.FirstName + " " + user.Person.LastName;
                            person.PersonRoleId = user.RoleId;
                            person.PersonRole = user.Role != null && user.Role.Name != null ? user.Role.Name : "";
                            person.IsActiveUser = isActiveUser;
                            franchisee.ReviewMarketingPersonViewModel.Add(person);
                        }
                    }
                }

                foreach (var item in reviewMarketingImageViewModel.ReviewMarketingFranchiseeViewModels)
                {
                    var name = item.FranchiseeName;
                    foreach (var person in item.ReviewMarketingPersonViewModel)
                    {
                        var jobScedulerforPerson = jobSchedulerList.Where(x => x.FranchiseeId == item.FranchiseeId && x.PersonId == person.PersonId && x.AssigneeId == person.Id && (x.EstimateId != null || x.JobId != null)).ToList();
                        if (jobScedulerforPerson.Count() == 0)
                        {
                            person.SchedulerCount = 0;
                            person.Message = "No Job/Estimate Assige To This Person";
                        }
                        else
                        {
                            person.SchedulerCount = jobScedulerforPerson.Count();
                            person.Message = "Job/Estimate Assiged To This Person";
                            foreach (var jobScheduler in jobScedulerforPerson)
                            {
                                ReviewMarketingSchedulerViewModel reviewMarketingSchedulerViewModel = new ReviewMarketingSchedulerViewModel();
                                var beforeImageList = beforeAfter.Where(x => (jobScheduler.EstimateId != null ? jobScheduler.EstimateId == x.EstimateId : jobScheduler.JobId == x.JobId) && x.TypeId == (long)LookupTypes.BeforeWork && x.PairId == null).ToList();
                                var exteriorImageList = beforeAfter.Where(x => (jobScheduler.EstimateId != null ? jobScheduler.EstimateId == x.EstimateId : jobScheduler.JobId == x.JobId) && x.TypeId == (long)LookupTypes.ExteriorBuilding && x.PairId == null).ToList();
                                if (exteriorImageList.Count() > 0)
                                {
                                    beforeImageList = exteriorImageList.Concat(beforeImageList).ToList();
                                }
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
                                    var data = jobEstimateData.FirstOrDefault(x => x.Id == jobScheduler.EstimateId);
                                    if (data != null)
                                    {
                                        estimateValue = jobEstimateData.FirstOrDefault(x => x.Id == jobScheduler.EstimateId).Amount;
                                    }
                                    else
                                    {
                                        estimateValue = default(decimal);
                                    }
                                }
                                reviewMarketingSchedulerViewModel.SchedulerName = jobScheduler.Job != null ? "J" + jobScheduler.JobId : "E" + jobScheduler.EstimateId;
                                reviewMarketingSchedulerViewModel.EstimateValue = estimateValue != null ? estimateValue : default(decimal);
                                reviewMarketingSchedulerViewModel.SchedulerUrl = linkUrl;
                                reviewMarketingSchedulerViewModel.Title = jobScheduler.Title;
                                reviewMarketingSchedulerViewModel.CustomerName = jobScheduler.Job != null && jobScheduler.Job.JobCustomer != null && jobScheduler.Job.JobCustomer.CustomerName != null ? jobScheduler.Job.JobCustomer.CustomerName : (jobScheduler != null && jobScheduler.Estimate != null && jobScheduler.Estimate.JobCustomer != null && jobScheduler.Estimate.JobCustomer.CustomerName != null ? jobScheduler.Estimate.JobCustomer.CustomerName : "");
                                //schedulerListLocalMarketingModel.MarketingClass = marketingClass;
                                person.ReviewMarketingSchedulerViewModel.Add(reviewMarketingSchedulerViewModel);

                                if (beforeImageList.Count() == 0)
                                {
                                    reviewMarketingSchedulerViewModel.Count = 0;
                                    reviewMarketingSchedulerViewModel.Message = "No Before/After Images Uploaded For This Scheduler.";
                                    ReviewMarketingBeforeAfterImageViewModel reviewMarketingBeforeAfterImageViewModel = new ReviewMarketingBeforeAfterImageViewModel();
                                    reviewMarketingBeforeAfterImageViewModel.S3BucketAfterImageUrlThumb = "";
                                    reviewMarketingBeforeAfterImageViewModel.S3BucketBeforeImageUrlThumb = "";
                                    reviewMarketingBeforeAfterImageViewModel.RelactiveLocationAfterImageUrlThumb = "";
                                    reviewMarketingBeforeAfterImageViewModel.RelactiveLocationBeforeImageUrlThumb = "";
                                    reviewMarketingBeforeAfterImageViewModel.RelactiveLocationAfterImage = "";
                                    reviewMarketingBeforeAfterImageViewModel.RelactiveLocationBeforeImage = "";
                                    reviewMarketingBeforeAfterImageViewModel.EmptyImage = "/Content/images/no_image_thumb.gif";
                                    reviewMarketingSchedulerViewModel.ReviewMarketingBeforeAfterImageViewModel.Add(reviewMarketingBeforeAfterImageViewModel);
                                }
                                else
                                {
                                    reviewMarketingSchedulerViewModel.Count = beforeImageList.Count();
                                    reviewMarketingSchedulerViewModel.Message = "Before/After Images Uploaded For This Scheduler.";
                                    foreach (var beforeImage in beforeImageList)
                                    {
                                        if (beforeImage.TypeId == (long)LookupTypes.ExteriorBuilding)
                                        {
                                            ReviewMarketingBeforeAfterImageViewModel reviewMarketingBeforeAfterImageViewModel = new ReviewMarketingBeforeAfterImageViewModel();
                                            var scheduler = beforeImage != null ? beforeImage.JobScheduler : null;

                                            reviewMarketingBeforeAfterImageViewModel.ImageTypeId = (long)LookupTypes.ExteriorBuilding;
                                            reviewMarketingBeforeAfterImageViewModel.S3BucketBeforeImageUrlThumb = beforeImage != null ? (beforeImage.S3BucketThumbURL != null ? beforeImage.S3BucketThumbURL : (beforeImage.S3BucketURL != null ? beforeImage.S3BucketURL : "")) : "";
                                            reviewMarketingBeforeAfterImageViewModel.RelactiveLocationBeforeImageUrlThumb = beforeImage != null && beforeImage.ThumbFileId != null ? (beforeImage.ThumbFile.RelativeLocation + "\\" + beforeImage.ThumbFile.Name).ToUrl() : "";
                                            reviewMarketingBeforeAfterImageViewModel.RelactiveLocationBeforeImage = beforeImage != null && beforeImage.File != null && beforeImage.File.RelativeLocation != null && beforeImage.File.Name != null ? (beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name).ToUrl() : "";
                                            reviewMarketingBeforeAfterImageViewModel.EmptyImage = "/Content/images/no_image_thumb.gif";
                                            reviewMarketingBeforeAfterImageViewModel.IsImagePairReviewed = beforeImage != null && beforeImage.IsImagePairReviewed != false ? true : false;

                                            reviewMarketingSchedulerViewModel.ReviewMarketingBeforeAfterImageViewModel.Add(reviewMarketingBeforeAfterImageViewModel);
                                        }
                                        else
                                        {
                                            ReviewMarketingBeforeAfterImageViewModel reviewMarketingBeforeAfterImageViewModel = new ReviewMarketingBeforeAfterImageViewModel();
                                            var afterImage = beforeAfter.FirstOrDefault(x => x.PairId == beforeImage.Id);
                                            var scheduler = beforeImage != null ? beforeImage.JobScheduler : null;
                                            reviewMarketingBeforeAfterImageViewModel.ImageTypeId = (long)LookupTypes.BeforeWork;
                                            reviewMarketingBeforeAfterImageViewModel.BeforeImageId = beforeImage != null ? beforeImage.Id : default(long);
                                            reviewMarketingBeforeAfterImageViewModel.AfterImageId = afterImage != null ? afterImage.Id : default(long);
                                            reviewMarketingBeforeAfterImageViewModel.AfterCss = afterImage != null && afterImage.File != null && afterImage.File.css != null ? afterImage.File.css : "rotate(0)";
                                            reviewMarketingBeforeAfterImageViewModel.BeforeCss = beforeImage.File != null && beforeImage.File.css != null ? beforeImage.File.css : "rotate(0)";
                                            reviewMarketingBeforeAfterImageViewModel.IsBestPicture = beforeImage.IsBestImage;
                                            reviewMarketingBeforeAfterImageViewModel.IsAddToLocalGallery = beforeImage.IsAddToLocalGallery;
                                            reviewMarketingBeforeAfterImageViewModel.ServicesType = beforeImage.ServiceType != null ? beforeImage.ServiceType.Name : "";
                                            reviewMarketingBeforeAfterImageViewModel.SurfaceColor = beforeImage.SurfaceColor != null ? beforeImage.SurfaceColor : "";
                                            reviewMarketingBeforeAfterImageViewModel.SurfaceType = beforeImage.SurfaceType != null ? beforeImage.SurfaceType : "";
                                            reviewMarketingBeforeAfterImageViewModel.FinishMaterial = beforeImage.FinishMaterial != null ? beforeImage.FinishMaterial : "";
                                            reviewMarketingBeforeAfterImageViewModel.SurfaceMaterial = beforeImage.SurfaceMaterial != null ? beforeImage.SurfaceMaterial : "";
                                            reviewMarketingBeforeAfterImageViewModel.BuildingLocation = beforeImage.BuildingLocation != null ? beforeImage.BuildingLocation : "";
                                            reviewMarketingBeforeAfterImageViewModel.Title = scheduler != null && scheduler.Title != null ? scheduler.Title : "";
                                            reviewMarketingBeforeAfterImageViewModel.IsBeforeImageCroped = beforeImage != null ? (beforeImage.CroppedImageThumbId != null ? (beforeImage.CroppedImageId != null ? true : false) : false) : false;
                                            reviewMarketingBeforeAfterImageViewModel.IsAfterImageCroped = afterImage != null ? (afterImage.CroppedImageThumbId != null ? (afterImage.CroppedImageId != null ? true : false) : false) : false;
                                            reviewMarketingBeforeAfterImageViewModel.S3BucketAfterImageUrlThumb = afterImage != null ? (afterImage.S3BucketThumbURL != null ? afterImage.S3BucketThumbURL : (afterImage.S3BucketURL != null ? afterImage.S3BucketURL : "")) : "";
                                            reviewMarketingBeforeAfterImageViewModel.S3BucketBeforeImageUrlThumb = beforeImage != null ? (beforeImage.S3BucketThumbURL != null ? beforeImage.S3BucketThumbURL : (beforeImage.S3BucketURL != null ? beforeImage.S3BucketURL : "")) : "";
                                            reviewMarketingBeforeAfterImageViewModel.RelactiveLocationAfterImageUrlThumb = afterImage != null && afterImage.ThumbFileId != null ? (afterImage.ThumbFile.RelativeLocation + "\\" + afterImage.ThumbFile.Name).ToUrl() : "";
                                            reviewMarketingBeforeAfterImageViewModel.RelactiveLocationBeforeImageUrlThumb = beforeImage != null && beforeImage.ThumbFileId != null ? (beforeImage.ThumbFile.RelativeLocation + "\\" + beforeImage.ThumbFile.Name).ToUrl() : "";
                                            if (reviewMarketingBeforeAfterImageViewModel.IsBeforeImageCroped && fileList.Count() > 0)
                                            {
                                                var file = beforeImage.CroppedImageThumbId != null ? fileList.FirstOrDefault(x => x.Id == beforeImage.CroppedImageThumbId) : fileList.FirstOrDefault(x => x.Id == beforeImage.CroppedImageId);
                                                reviewMarketingBeforeAfterImageViewModel.RelactiveLocationBeforeImageUrlThumb = file != null ? (file.RelativeLocation + "\\" + file.Name).ToUrl() : "";
                                            }
                                            if (reviewMarketingBeforeAfterImageViewModel.IsAfterImageCroped && fileList.Count() > 0)
                                            {
                                                var file = afterImage.CroppedImageThumbId != null ? fileList.FirstOrDefault(x => x.Id == afterImage.CroppedImageThumbId) : fileList.FirstOrDefault(x => x.Id == afterImage.CroppedImageId);
                                                reviewMarketingBeforeAfterImageViewModel.RelactiveLocationAfterImageUrlThumb = file != null ? (file.RelativeLocation + "\\" + file.Name).ToUrl() : "";
                                            }
                                            reviewMarketingBeforeAfterImageViewModel.RelactiveLocationAfterImage = afterImage != null && afterImage.File != null && afterImage.File.RelativeLocation != null && afterImage.File.Name != null ? (afterImage.File.RelativeLocation + "\\" + afterImage.File.Name).ToUrl() : "";
                                            reviewMarketingBeforeAfterImageViewModel.RelactiveLocationBeforeImage = beforeImage != null && beforeImage.File != null && beforeImage.File.RelativeLocation != null && beforeImage.File.Name != null ? (beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name).ToUrl() : "";
                                            reviewMarketingBeforeAfterImageViewModel.EmptyImage = "/Content/images/no_image_thumb.gif";
                                            reviewMarketingBeforeAfterImageViewModel.IsImagePairReviewed = beforeImage != null && beforeImage.IsImagePairReviewed != false ? true : afterImage != null && afterImage.IsImagePairReviewed != false ? true : false;
                                            reviewMarketingBeforeAfterImageViewModel.BeforeImageFileId = beforeImage != null && beforeImage.FileId != null ? beforeImage.FileId : null;
                                            reviewMarketingBeforeAfterImageViewModel.AfterImageFileId = afterImage != null && afterImage.FileId != null ? afterImage.FileId : null;
                                            reviewMarketingBeforeAfterImageViewModel.BestPairMarkedBy = beforeImage != null && beforeImage.BestPairMarkedBy != null ? beforeImage.Person1.FirstName + " " + beforeImage.Person1.LastName : "";
                                            reviewMarketingSchedulerViewModel.ReviewMarketingBeforeAfterImageViewModel.Add(reviewMarketingBeforeAfterImageViewModel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if(reviewMarketingImageViewModel.ReviewMarketingFranchiseeViewModels.Count() > 0)
                {
                    foreach (var item1 in reviewMarketingImageViewModel.ReviewMarketingFranchiseeViewModels)
                    {
                        foreach(var person in item1.ReviewMarketingPersonViewModel)
                        {
                            person.ReviewMarketingSchedulerViewModel.RemoveAll(x => x.Message == "No Before/After Images Uploaded For This Scheduler.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return reviewMarketingImageViewModel;
        }

        private JobEstimateServices GetNonPairService(long? serviceId, List<JobEstimateServices> list)
        {
            var serviceDomain = list.FirstOrDefault(x => x.Id == serviceId);
            var serviceWithoutPair = list.FirstOrDefault(x => x.Id == serviceDomain.PairId);
            return serviceWithoutPair;
        }

        public bool SaveImagesBestPair(SaveImagesBestPairFilter filter)
        {
            AddorCheckForBeforeAfterBestPicture(filter.BeforeServiceId, BeforeAfterPairType.REVIEWMARKETINGTYPE, (BeforeAfterBestPairType)filter.BeforeAfterBestPairType, false);
            AddorCheckForBeforeAfterBestPicture(filter.AfterServiceId, BeforeAfterPairType.REVIEWMARKETINGTYPE, (BeforeAfterBestPairType)filter.BeforeAfterBestPairType, false);
            return true;
        }


        private long? GetIdFromDb(JobEstimateImage jobEstimateImage)
        {
            var serviceImage = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == jobEstimateImage.ServiceId);
            if (serviceImage != null)
            {
                return serviceImage.Id;
            }
            else
            {
                return default(long?);
            }
        }


        private JobEstimateImage AddOrDeleteBestBeforeImages(ImagePairs imagePair, JobEstimateServices jobEstimateBeforeService, JobEstimateImage jobEstimateBeforeServiceImage, bool isFromJobEstimate)
        {
            if (imagePair.IsBestPicture)
            {
                jobEstimateBeforeServiceImage.BestFitMarkDateTime = DateTime.UtcNow;
                AddorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK, isFromJobEstimate);
            }

            else
            {
                jobEstimateBeforeServiceImage.BestFitMarkDateTime = null;
                DeleteorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK);
            }

            if (imagePair.IsAddToLocalGallery)
            {
                jobEstimateBeforeServiceImage.IsAddToLocalGallery = true;
                if (jobEstimateBeforeServiceImage.AddToGalleryDateTime == default(DateTime?))
                {
                    jobEstimateBeforeServiceImage.AddToGalleryDateTime = DateTime.UtcNow;
                }
                AddorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.REVIEWMARKETINGTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY, isFromJobEstimate);
            }
            else
            {
                jobEstimateBeforeServiceImage.IsAddToLocalGallery = false;
                jobEstimateBeforeServiceImage.AddToGalleryDateTime = null;
                DeleteorCheckForBeforeAfterBestPicture(jobEstimateBeforeService.Id, BeforeAfterPairType.REVIEWMARKETINGTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY);
            }
            return jobEstimateBeforeServiceImage;
        }

        public bool SaveReviewMarkImage(SaveReviewImageFilter filter)
        {
            var reviewHistoryImageLastDateHistory = _reviewMarketingImageLastDateHistryRepository.Table.
                              FirstOrDefault(x => x.StartDate == filter.StartDate && x.EndDate == filter.EndDate);

            if (reviewHistoryImageLastDateHistory == null)
            {
                var reviewHistoryImageLastDateHistoryDomain = new ReviewMarketingImageLastDateHistry()
                {
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    EndDate = filter.EndDate,
                    StartDate = filter.StartDate,
                    IsNew = true,
                    IsReview = filter.IsReview
                };
                _reviewMarketingImageLastDateHistryRepository.Save(reviewHistoryImageLastDateHistoryDomain);
            }
            else
            {
                reviewHistoryImageLastDateHistory.IsReview = filter.IsReview;
                _reviewMarketingImageLastDateHistryRepository.Save(reviewHistoryImageLastDateHistory);
            }
            return true;
        }

        public BeforeAfterForFranchieeAdminGroupedViewModel GetBeforeAfterImagesForFranchiseeAdmin(BeforeAfterImageFilter filter)
        {
            logger.Info("Starting Service " + _clock.UtcNow);
            var today = DateTime.UtcNow;
            var schedulerListForSelectedUsers = new List<long>();
            var franchiseeList = _organizationRoleUserRepository.Table.Where(x => x.UserId == filter.LoggedUserId).Select(x => x.Organization).ToList();
            if (filter.RoleId == (long?)(RoleType.SuperAdmin) || filter.RoleId == (long?)(RoleType.FrontOfficeExecutive))
            {
                franchiseeList = _organizationRoleUserRepository.Table.Where(x => !x.Organization.Name.StartsWith("0- ") &&
                x.Organization.Id != 1 && x.Organization.Id != 2).Select(x => x.Organization).ToList();
            }

            var isFilterUsed = (filter.SurfaceColor != "" || filter.SurfaceMaterial != "" || filter.FinishMaterial != "" ||
                filter.BuildingType != "" || filter.ServiceTypeId != 0 || filter.ManagementCompany != "" || filter.MaidService != ""
                || filter.SurfaceTypeId != null || filter.BuildingLocation != "") ? true : false;


            var isFromNull = false;

            if (filter.StartDate == null)
            {
                //DateTime date = new DateTime(2019, 12, 1);
                DateTime date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                int day = (int)date.DayOfWeek;
                DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                filter.StartDate = Monday.AddDays(-7);
                filter.EndDate = Sunday.AddDays(-6);
                isFromNull = true;
            }
            else
            {
                DateTime date = new DateTime(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month,
                    filter.StartDate.GetValueOrDefault().Day);
                int day = (int)date.DayOfWeek;
                if (day == 0)
                {
                    filter.StartDate = filter.StartDate.GetValueOrDefault().AddDays(1).Date;
                    filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(6).Date;
                }
                else
                {
                    DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                    DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                    filter.StartDate = Monday;
                    filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(7);
                }
            }

            var beforeAfterImageForFAListModel = new BeforeAfterImageForFAListModel();

            var beforeAfterForFranchieeAdminGroupedViewModel = new BeforeAfterForFranchieeAdminGroupedViewModel();
            List<BeforeAfterForFranchieeAdminGroupedViewModel> beforeAfterForFranchieeAdminGroupedViewModelList
                                                     = new List<BeforeAfterForFranchieeAdminGroupedViewModel>();

            BeforeAfterForPersonViewModel beforeAfterForPersonViewModel = new BeforeAfterForPersonViewModel();
            List<BeforeAfterForPersonViewModel> beforeAfterForPersonViewModelList = new List<BeforeAfterForPersonViewModel>();

            BeforeAfterForImageViewModel beforeAfterForImageViewModel = new BeforeAfterForImageViewModel();
            List<BeforeAfterForImageViewModel> BeforeAfterForImageViewModelList = new List<BeforeAfterForImageViewModel>();

            if (filter.FranchiseeId == null || filter.FranchiseeId == 0)
            {

                if (filter.RoleId == (long?)RoleType.SuperAdmin || filter.RoleId == (long?)RoleType.FrontOfficeExecutive)
                {
                    filter.FranchiseeId = 62;
                }
                else
                {
                    filter.FranchiseeId = franchiseeList.Where(x => x.Id == filter.LoggedInFranchiseeId).FirstOrDefault().Id;
                }
            }

            franchiseeList = franchiseeList.Where(x => x.Id == filter.FranchiseeId).ToList();
            beforeAfterImageForFAListModel.FranchiseeId = filter.FranchiseeId;
            beforeAfterForFranchieeAdminGroupedViewModel.FrachiseeName = franchiseeList.FirstOrDefault().Name;
            var userIdList = _organizationRoleUserRepository.Table.Where(x => x.IsActive && (filter.UserId == 0 || filter.UserId == null || filter.UserId == x.UserId) && x.OrganizationId == filter.FranchiseeId
            && (x.RoleId == (long)RoleType.SalesRep || x.RoleId == (long)RoleType.Technician)).ToList();
            if (userIdList.Count() == 0)
            {
                userIdList = _organizationRoleUserRepository.Table.Where(x => x.IsActive && (filter.UserId == x.UserId) && x.OrganizationId == filter.FranchiseeId
            ).ToList();
            }
            if (filter.RoleId == ((long?)RoleType.SalesRep) || filter.RoleId == ((long?)RoleType.Technician) ||
                filter.RoleId == ((long?)RoleType.OperationsManager))
            {
                userIdList = userIdList.Where(x => x.UserId == filter.LoggedUserId.GetValueOrDefault()).ToList();
            }
            var schedulerList = _jobschedulerRepository.Table.Where(x => x.FranchiseeId == filter.FranchiseeId &&
            (x.StartDate >= filter.StartDate && x.StartDate <= filter.EndDate)).ToList();

            logger.Info("Starting Creating Person Model " + _clock.UtcNow);

            var personModel = userIdList.Select(x => new PersonViewModel()
            {
                PersonName = x.Person.FirstName + " " + x.Person.LastName,
                UserId = x.UserId,
                Id = x.Id,
                RoleId = x.RoleId,
                Role = x.Role
            }).Distinct().ToList();

            logger.Info("Ending Creating Person Model " + _clock.UtcNow);

            var userIdListForUser = personModel.Select(x => x.Id).ToList();

            var schedulerListForSelectedFranchisee = schedulerList;

            var schedulerIdListForSelectedFranchisee = schedulerListForSelectedFranchisee.Select(x => x.Id).ToList();
            var jobEstimateCategoryIds = _jobEstimateImageCategory.Table.Where(x => schedulerIdListForSelectedFranchisee.Contains(x.SchedulerId.Value)
               ).Select(x => x.Id).ToList();

            logger.Info("Ending Creating Person Model " + _clock.UtcNow);

            var jobEstimateServicesList = _jobEstimateServices.Table.Where(x => (jobEstimateCategoryIds.Contains(x.CategoryId.Value))
            && (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.AfterWork
            || x.TypeId == (long?)LookupTypes.DuringWork || x.TypeId == (long?)LookupTypes.ExteriorBuilding)
             && (filter.SurfaceColor == "" || filter.SurfaceColor == x.SurfaceColor)
             && (filter.MarketingClassId == 0 || (x.JobEstimateImageCategory.MarkertingClassId == filter.MarketingClassId))
              && (filter.SurfaceMaterial == "" || x.SurfaceMaterial == filter.SurfaceMaterial)
              && (filter.FinishMaterial == "" || x.FinishMaterial == filter.FinishMaterial)
              && (filter.BuildingType == "" || x.BuildingLocation == filter.BuildingType)
              && (filter.ServiceTypeId == 0 || x.ServiceTypeId == filter.ServiceTypeId)
              && (filter.ManagementCompany == "" || x.CompanyName == filter.ManagementCompany)
              && (filter.MaidService == "" || x.MaidService == filter.MaidService)
              && (filter.SurfaceTypeId == null || x.SurfaceType == filter.SurfaceTypeId)
              && (filter.BuildingLocation == "" || x.BuildingLocation == filter.BuildingLocation) || x.TypeId == (long?)LookupTypes.ExteriorBuilding).ToList();

            var jobEstimateServiceIds = jobEstimateServicesList.Select(x => x.Id).ToList();

            var jobEstimateServiceAssigneeList = jobEstimateServicesList.Select(x => x.JobEstimateImageCategory.JobScheduler);
            var personModelGrouped = schedulerListForSelectedFranchisee.AsEnumerable().GroupBy(x => x.AssigneeId).Distinct();


            var personIds = personModel.Select(x => x.Id);
            var jobIdsForParticularFranchisee = schedulerListForSelectedFranchisee.Where(x => x.JobId != null).Select(x => x.JobId).ToList();
            var allJobIds = schedulerListForSelectedFranchisee.Where(x => personIds.Contains(x.AssigneeId) && x.JobId != null).ToList();
            var allJobs = _jobRepository.Table.Where(x => jobIdsForParticularFranchisee.Contains(x.Id)).ToList();
            var jobEstimateImagesList = _jobEstimateImage.Table.Where(x => jobEstimateServiceIds.Contains(x.ServiceId.Value)).ToList();
            foreach (var person in personModel)
            {
                schedulerListForSelectedUsers = new List<long>();
                beforeAfterForPersonViewModel.PersonName = person.PersonName;
                beforeAfterForPersonViewModel.RoleName = person.Role.Name;

                if (person.RoleId == (long?)RoleType.SalesRep)
                {
                    schedulerListForSelectedFranchisee = schedulerListForSelectedFranchisee.Where(x => x.JobId == null).ToList();
                }

                var jobIds = allJobIds.Where(x => x.AssigneeId == person.Id).Select(x => x.JobId).Distinct().ToList();
                var esimtateIds = schedulerListForSelectedFranchisee.Where(x => x.AssigneeId == person.Id && x.EstimateId != null).Select(x => x.EstimateId).Distinct().ToList();
                var estimateIds = allJobs.Where(x => jobIds.Contains(x.Id)).Select(x => x.EstimateId).ToList();
                estimateIds = estimateIds.Where(x => x != null).ToList();
                var jobsIds = allJobs.Where(x => estimateIds.Contains(x.EstimateId)).Select(x => x.Id).ToList();
                esimtateIds.AddRange(estimateIds);


                foreach (var jobsId in jobsIds)
                {
                    jobIds.Add((long?)jobsId);
                }
                var scheduulerIds = schedulerListForSelectedFranchisee.Where(x => jobIds.Contains(x.JobId) || esimtateIds.Contains(x.EstimateId) & x.MeetingID == null && !x.IsVacation).Distinct().Select(x => x.Id).ToList();

                schedulerListForSelectedUsers.AddRange(scheduulerIds);
                schedulerListForSelectedUsers = schedulerListForSelectedUsers.Distinct().OrderByDescending(x => x).ToList();

                if (schedulerListForSelectedUsers.Count() > 0)
                {
                    var beforeAfterImagePerson = _beforeAfterImageService.GetBeforeAfterImagesForFranchiseeAdmin(schedulerListForSelectedUsers,
                        filter.StartDate.GetValueOrDefault(), filter.EndDate.GetValueOrDefault(), filter, schedulerList, jobEstimateServicesList, jobEstimateImagesList);


                    beforeAfterImagePerson = beforeAfterImagePerson.Where(x => x.AfterServiceId != default(long?) && x.BeforeServiceId != default(long?)).OrderByDescending(x => x.JobEstimateId).ToList();


                    beforeAfterForPersonViewModel.BeforeAfterImageViewModel = beforeAfterImagePerson;
                    //beforeAfterForPersonViewModel.NonResidentalCollection = (beforeAfterImagePerson.Where(x => x.OrderNo == 100).ToList());
                    beforeAfterForPersonViewModel.NonResidentalCollection = CreateNonResidentalViewModel(beforeAfterImagePerson.Where(x => x.OrderNo == 100).ToList());
                    beforeAfterForPersonViewModel.ResidentalCollection = beforeAfterImagePerson.Where(x => x.OrderNo == 1).ToList();
                    beforeAfterForPersonViewModel.TotalCount = beforeAfterImagePerson.Count();
                    beforeAfterForPersonViewModelList.Add(beforeAfterForPersonViewModel);

                    beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel = beforeAfterForPersonViewModelList;

                    beforeAfterForPersonViewModel = new BeforeAfterForPersonViewModel();
                }

                else
                {
                    beforeAfterForPersonViewModel.BeforeAfterImageViewModel = new List<BeforeAfterForImageViewModel>();
                    beforeAfterForPersonViewModel.NonResidentalCollection = new List<NonResidentalImageViewModel>();
                    beforeAfterForPersonViewModel.ResidentalCollection = new List<BeforeAfterForImageViewModel>();
                    beforeAfterForPersonViewModel.TotalCount = 0;
                    beforeAfterForPersonViewModelList.Add(beforeAfterForPersonViewModel);

                    beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel = beforeAfterForPersonViewModelList.Distinct().ToList();
                    beforeAfterForPersonViewModel = new BeforeAfterForPersonViewModel();
                }
                schedulerListForSelectedFranchisee = schedulerList;
                schedulerIdListForSelectedFranchisee = schedulerListForSelectedFranchisee.Select(x => x.Id).ToList();
            }
            beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel = beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel.OrderByDescending(x => x.BeforeAfterImageViewModel.Count()).ToList();
            beforeAfterForFranchieeAdminGroupedViewModel.TotalCount = personModel.Count();
            beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel =
            beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

            if (isFromNull)
            {
                isFromNull = false;
                filter.EndDate = filter.EndDate.GetValueOrDefault().AddDays(-1);
            }
            beforeAfterImageForFAListModel.BeforeAfterGroupedViewModel.Add(beforeAfterForFranchieeAdminGroupedViewModel);
            beforeAfterForFranchieeAdminGroupedViewModel.FrachiseeId = filter.FranchiseeId;
            beforeAfterForFranchieeAdminGroupedViewModel.StartDate = filter.StartDate;

            beforeAfterForFranchieeAdminGroupedViewModel.EndDate = filter.EndDate;
            return beforeAfterForFranchieeAdminGroupedViewModel;
        }


        public bool SaveBeforeAfterImagesForFranchiseeAdmin(SaveBeforeAfterImageFilter filter)
        {
            if (filter.IsAddToGalary)
            {
                var afterService = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == filter.AfterServiceId);
                afterService.IsAddToLocalGallery = filter.IsSelected;
                if (filter.IsSelected)
                {
                    afterService.IsAddToLocalGallery = true;
                    afterService.AddToGalleryDateTime = DateTime.UtcNow;
                    AddorCheckForBeforeAfterBestPicture(filter.AfterServiceId, BeforeAfterPairType.REVIEWMARKETINGTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY, false);

                }
                else
                {
                    afterService.IsAddToLocalGallery = false;
                    afterService.AddToGalleryDateTime = null;
                    DeleteorCheckForBeforeAfterBestPicture(filter.AfterServiceId, BeforeAfterPairType.REVIEWMARKETINGTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY);
                }

                _jobEstimateImage.Save(afterService);
                AddingBestPairForBeforeAfterImages(afterService);
                var beforeService = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == filter.BeforeServiceId);
                beforeService.IsAddToLocalGallery = filter.IsSelected;
                if (filter.IsSelected)
                {
                    afterService.IsAddToLocalGallery = true;
                    beforeService.AddToGalleryDateTime = DateTime.UtcNow;
                    AddorCheckForBeforeAfterBestPicture(filter.BeforeServiceId, BeforeAfterPairType.REVIEWMARKETINGTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY, false);

                }
                else
                {
                    afterService.IsAddToLocalGallery = false;
                    beforeService.AddToGalleryDateTime = null;
                    DeleteorCheckForBeforeAfterBestPicture(filter.BeforeServiceId, BeforeAfterPairType.REVIEWMARKETINGTYPE, BeforeAfterBestPairType.ADDTOLOCALGALLERY);
                }
                _jobEstimateImage.Save(beforeService);
                AddingBestPairForBeforeAfterImages(beforeService);
            }
            else
            {
                var afterService = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == filter.AfterServiceId);
                afterService.IsBestImage = filter.IsSelected;
                if (filter.IsSelected)
                {
                    afterService.IsBestImage = true;
                    afterService.BestFitMarkDateTime = DateTime.UtcNow;
                    AddorCheckForBeforeAfterBestPicture(filter.AfterServiceId, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK, false);

                }
                else
                {
                    afterService.IsBestImage = false;
                    afterService.BestFitMarkDateTime = null;
                    DeleteorCheckForBeforeAfterBestPicture(filter.AfterServiceId, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK);
                }
                _jobEstimateImage.Save(afterService);
                AddingBestPairForBeforeAfterImages(afterService);
                var beforeService = _jobEstimateImage.Table.FirstOrDefault(x => x.ServiceId == filter.BeforeServiceId);
                beforeService.IsBestImage = filter.IsSelected;
                if (filter.IsSelected)
                {
                    beforeService.BestFitMarkDateTime = DateTime.UtcNow;
                    AddorCheckForBeforeAfterBestPicture(filter.BeforeServiceId, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK, false);
                }
                else
                {
                    beforeService.BestFitMarkDateTime = null;
                    DeleteorCheckForBeforeAfterBestPicture(filter.BeforeServiceId, BeforeAfterPairType.BESTPAIRTYPE, BeforeAfterBestPairType.BESTPAIRMARK);
                }
                _jobEstimateImage.Save(beforeService);
                AddingBestPairForBeforeAfterImages(beforeService);
            }

            return true;
        }

        public static void PerformImageResizeAndPutOnCanvas(string pFilePath, string pFileName, int pWidth, int pHeight, string pOutputFileName)
        {
            System.Drawing.Image imgBef;
            imgBef = System.Drawing.Image.FromFile(pFilePath + pFileName);


            System.Drawing.Image _imgR;
            _imgR = Imager.Resize(imgBef, pWidth, pHeight, true);


            System.Drawing.Image _img2;
            _img2 = Imager.PutOnCanvas(_imgR, pWidth, pHeight, System.Drawing.Color.White);

            //Save JPEG  
            Imager.SaveJpeg(pFilePath + pOutputFileName, _img2);

        }


        private bool AddExteriorImageImages(JobEstimateServiceViewModel model, long categoryId)
        {
            var addedIdInDb = new List<long>();
            var jobEstimateCategoryDomain = _jobEstimateImageCategory.IncludeMultiple(x => x.JobScheduler).FirstOrDefault(x => x.Id == categoryId);
            var jobEstimateServicesIds = _jobEstimateServices.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long?)LookupTypes.ExteriorBuilding).Select(x => x.Id).ToList();
            var jobEstimateImageList = _jobEstimateImage.Table.Where(x => jobEstimateServicesIds.Contains(x.ServiceId.Value)).ToList();
            var alreadyPresentFilesIds = _beforeAfterImagesRepository.Table.Where(x => x.CategoryId == categoryId && x.TypeId == (long?)LookupTypes.ExteriorBuilding).Select(x => x.FileId).ToList();
            var newImageIds = model.ImagesInfo.Select(x => (long?)x.Id).ToList();
            var fileIdsDeleteds = alreadyPresentFilesIds.Except(newImageIds).ToList();
            var beforeAfterImagesToBeDeleted = _beforeAfterImagesRepository.Table.Where(x => fileIdsDeleteds.Contains(x.FileId) && x.CategoryId == categoryId).ToList();
            foreach (var fileId in model.FilesList)
            {
                var jobEstimateService = jobEstimateImageList.FirstOrDefault(x => x.FileId == fileId);
                var isNewData = false;
                var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, categoryId, (long?)LookupTypes.ExteriorBuilding);
                var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(model, buildingExteriorServiceId, (long?)LookupTypes.ExteriorBuilding, fileId);
                var beforeAfterBeforeImages = CreateDomainModel(jobEstimateCategoryDomain, jobEstimateBeforeService, jobEstimateBeforeServiceImage);
                beforeAfterBeforeImages.ServiceId = jobEstimateService.ServiceId;
                var beforeAfterBeforeImagesDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.CategoryId == jobEstimateCategoryDomain.Id && x.ServiceId == beforeAfterBeforeImages.ServiceId);
                if (beforeAfterBeforeImagesDomain != null)
                {
                    beforeAfterBeforeImages.Id = beforeAfterBeforeImagesDomain.Id;
                    beforeAfterBeforeImages.IsNew = false;
                    beforeAfterBeforeImages.DataRecorderMetaDataId = beforeAfterBeforeImagesDomain.DataRecorderMetaDataId;
                    beforeAfterBeforeImages.DataRecorderMetaData = null;
                    beforeAfterBeforeImages.S3BucketThumbURL = null;
                    beforeAfterBeforeImages.S3BucketURL = null;
                }
                if (beforeAfterBeforeImages.IsNew)
                {
                    isNewData = true;
                }
                _beforeAfterImagesRepository.Save(beforeAfterBeforeImages);
                if (isNewData)
                {
                    addedIdInDb.Add(beforeAfterBeforeImages.Id);
                    isNewData = false;
                }
            }

            foreach (var fileIdsDeleted in fileIdsDeleteds)
            {
                var beforeAfterImageToBeDeleted = beforeAfterImagesToBeDeleted.FirstOrDefault(x => x.FileId == fileIdsDeleted);
                _beforeAfterImagesRepository.Delete(beforeAfterImageToBeDeleted);
            }
            return true;
        }
        private bool AddBeforeAfterImages(JobEstimateCategoryViewModel model)
        {
            var alreadyPresentInDb = new List<long>();
            var alreadyPresentInDbs = new List<long>();
            var jobEstimateBeforeImage = new JobEstimateImage();
            var jobEstimateServiceBeforeDomain = new JobEstimateServices();
            var jobEstimateAfterImage = new JobEstimateImage();
            var jobEstimateServiceAfterDomain = new JobEstimateServices();
            var jobEstimateCategory = new JobEstimateImageCategory();
            var jobEstimateCategoryDomain = _jobEstimateImageCategory.IncludeMultiple(x => x.JobScheduler).FirstOrDefault(x => x.Id == model.Id.Value);
            var personName = jobEstimateCategoryDomain.JobScheduler.Person.FirstName + " " + jobEstimateCategoryDomain.JobScheduler.Person.LastName;
            var beforeAfterImagesBeforeDomain = new BeforeAfterImages();
            jobEstimateCategory = _jobFactory.CreateJobEstimateCategory(model);
            if (jobEstimateCategory.EstimateId != null)
            {
                alreadyPresentInDb = _beforeAfterImagesRepository.Table.Where(x => (x.EstimateId == jobEstimateCategory.EstimateId) && (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.AfterWork)).Select(x => x.Id).ToList();
            }
            else if (jobEstimateCategory.JobId != null)
            {
                alreadyPresentInDbs = _beforeAfterImagesRepository.Table.Where(x => (x.JobId == jobEstimateCategory.JobId) && (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.AfterWork)).Select(x => x.Id).ToList();

            }
            alreadyPresentInDb.AddRange(alreadyPresentInDbs);
            var addedIdInDb = new List<long?>();
            var isNewData = false;
            foreach (var imagePair in model.ImagePairs)
            {


                jobEstimateAfterImage = new JobEstimateImage();
                jobEstimateBeforeImage = new JobEstimateImage();
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
                    beforeAfterBeforeImages.S3BucketThumbURL = null;
                    beforeAfterBeforeImages.S3BucketURL = null;
                    beforeAfterBeforeImages.IsImageCropped = beforeAfterBeforeImagesDomain.IsImageCropped;
                    beforeAfterBeforeImages.CroppedImageId = beforeAfterBeforeImagesDomain.CroppedImageId;
                    beforeAfterBeforeImages.CroppedImageThumbId = beforeAfterBeforeImagesDomain.CroppedImageThumbId;
                    beforeAfterBeforeImages.IsBestImage = beforeAfterBeforeImagesDomain.IsBestImage;
                    beforeAfterBeforeImages.IsAddToLocalGallery = beforeAfterBeforeImagesDomain.IsAddToLocalGallery;
                }
                else if (beforeAfterImageCheckingForFileId != null)
                {
                    beforeAfterBeforeImages.Id = beforeAfterImageCheckingForFileId.Id;
                    beforeAfterBeforeImages.IsNew = false;
                    beforeAfterBeforeImages.DataRecorderMetaDataId = beforeAfterImageCheckingForFileId.DataRecorderMetaDataId;
                    beforeAfterBeforeImages.DataRecorderMetaData = null;
                    beforeAfterBeforeImages.JobId = beforeAfterImageCheckingForFileId.JobId;
                    beforeAfterBeforeImages.EstimateId = beforeAfterImageCheckingForFileId.EstimateId;
                    beforeAfterBeforeImages.IsImageCropped = beforeAfterImageCheckingForFileId.IsImageCropped;
                    beforeAfterBeforeImages.CroppedImageId = beforeAfterImageCheckingForFileId.CroppedImageId;
                    beforeAfterBeforeImages.CroppedImageThumbId = beforeAfterImageCheckingForFileId.CroppedImageThumbId;
                    beforeAfterBeforeImages.IsBestImage = beforeAfterImageCheckingForFileId.IsBestImage;
                    beforeAfterBeforeImages.IsAddToLocalGallery = beforeAfterImageCheckingForFileId.IsAddToLocalGallery;
                }

                if (beforeAfterBeforeImages.IsNew)
                {
                    isNewData = true;
                }
                if (beforeAfterBeforeImages.ServiceId == 0)
                {
                    beforeAfterBeforeImages.ServiceId = null;
                }
                beforeAfterBeforeImages.S3BucketURL = null;
                beforeAfterBeforeImages.S3BucketThumbURL = null;
                beforeAfterBeforeImages.IsImageUpdated = false;
                beforeAfterBeforeImages.IsImageMigrateToCalendar = false;
                _beforeAfterImagesRepository.Save(beforeAfterBeforeImages);
                addedIdInDb.Add(beforeAfterBeforeImages.Id);
                if (isNewData)
                {
                    isNewData = false;
                    alreadyPresentInDb.Add(beforeAfterBeforeImages.Id);
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
                    //beforeAfterImagesAfterDomain.JobId = beforeAfterBeforeImagesDomain.JobId;
                    //beforeAfterImagesAfterDomain.EstimateId = beforeAfterBeforeImagesDomain.EstimateId;
                    beforeAfterImagesAfterDomain.IsImageCropped = beforeAfterBeforeImagesDomain.IsImageCropped;
                    beforeAfterImagesAfterDomain.CroppedImageId = beforeAfterBeforeImagesDomain.CroppedImageId;
                    beforeAfterImagesAfterDomain.CroppedImageThumbId = beforeAfterBeforeImagesDomain.CroppedImageThumbId;
                    beforeAfterImagesAfterDomain.IsBestImage = beforeAfterBeforeImagesDomain.IsBestImage;
                    beforeAfterImagesAfterDomain.IsAddToLocalGallery = beforeAfterBeforeImagesDomain.IsAddToLocalGallery;

                }
                else if (beforeAfterImageCheckingForFileId != null)
                {
                    beforeAfterImagesAfterDomain.Id = beforeAfterImageCheckingForFileId.Id;
                    beforeAfterImagesAfterDomain.IsNew = false;
                    beforeAfterImagesAfterDomain.DataRecorderMetaDataId = beforeAfterImageCheckingForFileId.DataRecorderMetaDataId;
                    beforeAfterImagesAfterDomain.DataRecorderMetaData = null;
                    //beforeAfterImagesAfterDomain.JobId = beforeAfterImageCheckingForFileId.JobId;
                    //beforeAfterImagesAfterDomain.EstimateId = beforeAfterImageCheckingForFileId.EstimateId;
                    beforeAfterImagesAfterDomain.IsImageCropped = beforeAfterImageCheckingForFileId.IsImageCropped;
                    beforeAfterImagesAfterDomain.CroppedImageId = beforeAfterImageCheckingForFileId.CroppedImageId;
                    beforeAfterImagesAfterDomain.CroppedImageThumbId = beforeAfterImageCheckingForFileId.CroppedImageThumbId;
                    beforeAfterImagesAfterDomain.IsBestImage = beforeAfterImageCheckingForFileId.IsBestImage;
                    beforeAfterImagesAfterDomain.IsAddToLocalGallery = beforeAfterImageCheckingForFileId.IsAddToLocalGallery;
                }
                if (beforeAfterImagesAfterDomain.ServiceId == 0)
                {
                    beforeAfterBeforeImagesDomain.ServiceTypeId = null;
                }
                if (beforeAfterImagesAfterDomain.IsNew)
                {
                    isNewData = true;
                }
                beforeAfterBeforeImages.S3BucketURL = null;
                beforeAfterBeforeImages.S3BucketThumbURL = null;
                beforeAfterBeforeImages.IsImageUpdated = false;
                beforeAfterBeforeImages.IsImageMigrateToCalendar = false;
                _beforeAfterImagesRepository.Save(beforeAfterImagesAfterDomain);
                addedIdInDb.Add(beforeAfterImagesAfterDomain.Id);
                if (isNewData)
                {
                    isNewData = false;
                    alreadyPresentInDb.Add(beforeAfterBeforeImages.Id);
                }
            }

            if (addedIdInDb.Count() > alreadyPresentInDb.Count() && alreadyPresentInDb.Count() > 0)
            {
                foreach (var addedId in addedIdInDb)
                {
                    var toBeDeletedIds = !alreadyPresentInDb.Contains(addedId.Value);
                    if (toBeDeletedIds)
                    {
                        var beforeAfterDomain = _beforeAfterImagesRepository.Get(addedId.Value);
                        _beforeAfterImagesRepository.Delete(beforeAfterDomain);
                    }
                }
            }
            else if (alreadyPresentInDb.Count() > 0)
            {
                foreach (var addedId in alreadyPresentInDb)
                {
                    var toBeDeletedIds = !addedIdInDb.Contains(addedId);
                    if (toBeDeletedIds)
                    {
                        var beforeAfterDomain = _beforeAfterImagesRepository.Get(addedId);
                        _beforeAfterImagesRepository.Delete(beforeAfterDomain);
                    }
                }
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

        public BeforeAfterForFranchieeAdminGroupedViewModel GetBeforeAfterImagesForFranchiseeAdminV2(BeforeAfterImageFilter filter)
        {
            logger.Info("Starting Services " + _clock.UtcNow);
            BeforeAfterForPersonViewModel beforeAfterForPersonViewModel = new BeforeAfterForPersonViewModel();
            List<BeforeAfterForPersonViewModel> beforeAfterForPersonViewModelList = new List<BeforeAfterForPersonViewModel>();

            var beforeAfterForFranchieeAdminGroupedViewModel = new BeforeAfterForFranchieeAdminGroupedViewModel();
            var beforeAfterForFranchieeAdminGroupedViewModelList
                                                     = new List<BeforeAfterForFranchieeAdminGroupedViewModel>();

            if (filter.StartDate == null)
            {
                //DateTime date = new DateTime(2019, 12, 1);
                DateTime date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                int day = (int)date.DayOfWeek;
                DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                filter.StartDate = Monday.AddDays(-7);
                filter.EndDate = Sunday.AddDays(-6);
            }
            else
            {
                DateTime date = new DateTime(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month,
                    filter.StartDate.GetValueOrDefault().Day);
                int day = (int)date.DayOfWeek;
                if (day == 0)
                {
                    filter.StartDate = filter.StartDate.GetValueOrDefault().AddDays(1).Date;
                    filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(6).Date;
                }
                else
                {
                    DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                    DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                    filter.StartDate = Monday;
                    filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(7);
                }
            }

            if (filter.FranchiseeId == null || filter.FranchiseeId == 0)
            {

                if (filter.RoleId == (long?)RoleType.SuperAdmin || filter.RoleId == (long?)RoleType.FrontOfficeExecutive)
                {
                    filter.FranchiseeId = 62;
                }
                else
                {
                    filter.FranchiseeId = filter.LoggedInFranchiseeId;
                }
            }


            logger.Info("Starting Getting Value from Before After Images " + _clock.UtcNow);

            var beforeAfterImagesList = _beforeAfterImagesRepository.Table.Where(x =>
             (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.AfterWork
            || x.TypeId == (long?)LookupTypes.DuringWork || x.TypeId == (long?)LookupTypes.ExteriorBuilding)
             && (filter.SurfaceColor == "" || filter.SurfaceColor == x.SurfaceColor)
             && (filter.FranchiseeId == null || filter.FranchiseeId == x.FranchiseeId)
             && (filter.MarketingClassId == 0 || (x.MarkertingClassId == filter.MarketingClassId))
              && (filter.SurfaceMaterial == "" || x.SurfaceMaterial == filter.SurfaceMaterial)
              && (filter.FinishMaterial == "" || x.FinishMaterial == filter.FinishMaterial)
              && (filter.BuildingType == "" || x.BuildingLocation == filter.BuildingType)
              && (filter.ServiceTypeId == 0 || x.ServiceTypeId == filter.ServiceTypeId)
              && (filter.ManagementCompany == "" || x.CompanyName == filter.ManagementCompany)
              && (filter.MaidService == "" || x.MaidService == filter.MaidService)
              && (filter.SurfaceTypeId == null || x.SurfaceType == filter.SurfaceTypeId)
              && (filter.BuildingLocation == "" || x.BuildingLocation == filter.BuildingLocation)
              || x.TypeId == (long?)LookupTypes.ExteriorBuilding).ToList();

            //  beforeAfterImagesList = beforeAfterImagesList.Where(x => x.JobEstimateServices != null).ToList();

            logger.Info("Ending Getting Value from Before After Images " + _clock.UtcNow);

            logger.Info("Starting Creating Person Data " + _clock.UtcNow);
            var personModel = GetGroupedPersonData(filter);
            logger.Info("Ending Creating Person Data " + _clock.UtcNow);

            var beforeAfterForImageViewModel = new List<BeforeAfterForImageViewModel>();
            var userIdList = new List<PersonUploadViewModel>();
            //personModel = personModel.OrderByDescending(x => x.RoleId).ToList();
            var utcStartDate = _clock.ToUtc(filter.StartDate.Value);
            var utcEndDate = _clock.ToUtc(filter.EndDate.Value);
            var jobSchedulerList = _jobschedulerRepository.Table.Where(x => x.StartDate >= utcStartDate && x.EndDate <= utcEndDate
                             && x.FranchiseeId == filter.FranchiseeId && (x.JobId != null || x.EstimateId != null)).ToList();

            var jobSchedulerListForFranchisee = _jobschedulerRepository.Table.Where(x => x.IsActive && x.FranchiseeId == filter.FranchiseeId && (x.JobId != null || x.EstimateId != null)).ToList();
            var jobestimateServicesList = _jobEstimateServices.Table.Where(x => x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.AfterWork).ToList();
            logger.Info("Entering Person Model Loop " + _clock.UtcNow);
            foreach (var person in personModel)
            {
                logger.Info("Entering Person Model Loop For Person " + person.PersonName);
                logger.Info("Entering Person Model Loop For Person at " + _clock.UtcNow);

                var beforeAfterImagesListBeforeAfter = beforeAfterImagesList.Where(x => (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.AfterWork)).ToList();

                var jobSchedulerListForParticularPerson = jobSchedulerList.Where(x => x.AssigneeId == person.Id && x.IsActive).ToList();

                var jobid = jobSchedulerListForParticularPerson.Where(x => x.JobId != null).Select(x => x.JobId).ToList();
                var estimateId = jobSchedulerListForParticularPerson.Where(x => x.EstimateId != null).Select(x => x.EstimateId).ToList();
                var convertedEstimateId = jobSchedulerListForParticularPerson.Where(x => x.JobId != null && x.Job.EstimateId != null).Select(x => x.Job.EstimateId).ToList();
                estimateId.AddRange(convertedEstimateId);
                estimateId = estimateId.Distinct().ToList();

                var jobSchedulerIdForFranchisee = jobSchedulerListForFranchisee.
                                                              Where(x => jobid.Contains(x.JobId) || estimateId.Contains(x.EstimateId)).Distinct().Select(x => x.Id).ToList();

                var jobSchedulerIdForFranchiseeConverted = jobSchedulerListForFranchisee.
                                                              Where(x => x.Job != null && x.Job.EstimateId != null && estimateId.Contains(x.Job.EstimateId)).Distinct().Select(x => x.Id).ToList();

                var jobSchedulerIdList = jobSchedulerListForParticularPerson.Select(x => x.Id).ToList();

                jobSchedulerIdList.AddRange(jobSchedulerIdForFranchisee);
                jobSchedulerIdList.AddRange(jobSchedulerIdForFranchiseeConverted);

                var jobSchedulerIdListInBeforeAfter = beforeAfterImagesListBeforeAfter.Select(x => x.SchedulerId.Value).Distinct().ToList();

                var beforeAfterImagesListLocal = beforeAfterImagesList.Where(x => jobSchedulerIdList.Contains(x.SchedulerId.Value)).ToList();
                jobSchedulerIdList = jobSchedulerIdList.Except(jobSchedulerIdListInBeforeAfter).ToList();

                beforeAfterForPersonViewModel = new BeforeAfterForPersonViewModel();

                var beforeAfterImagesListForPerson = beforeAfterImagesListLocal.Where(x => (x.TypeId == (long?)LookupTypes.BeforeWork || x.TypeId == (long?)LookupTypes.AfterWork) && x.PairId == null);
                beforeAfterImagesListForPerson = beforeAfterImagesListForPerson.Where(x => jobSchedulerIdListInBeforeAfter.Contains(x.SchedulerId.Value)).ToList();
                beforeAfterForPersonViewModel.PersonName = person.PersonName;
                beforeAfterForPersonViewModel.RoleName = person.Role.Name;
                beforeAfterForPersonViewModel.UserId = person.UserId;

                if (person.RoleId == (long?)RoleType.SalesRep)
                {
                    beforeAfterImagesListForPerson = beforeAfterImagesListForPerson.Where(x => x.EstimateId != null).ToList();
                }

                var jobEstimateBeforeCategory = beforeAfterImagesListForPerson.Where(x => x.PairId == null).ToList();

                logger.Info("Creating View Model For Person at " + _clock.UtcNow);
                beforeAfterImagesListForPerson = beforeAfterImagesListForPerson.Distinct().ToList();
                var serviceIdList = beforeAfterImagesListForPerson.Select(x => x.ServiceTypeId).ToList();
                var jobestimateServices = jobestimateServicesList.Where(x => serviceIdList.Contains(x.ServiceTypeId)).ToList();
                var beforeAfterImagePerson = beforeAfterImagesListForPerson.Select(x => _jobFactory.CreateBeforeAfterViewModel(x,
                      beforeAfterImagesListLocal.FirstOrDefault(x1 => x1.PairId == x.Id), beforeAfterImagesListLocal.
                      FirstOrDefault(x1 => x1.CategoryId == x.CategoryId && x1.TypeId == (long?)LookupTypes.ExteriorBuilding), jobestimateServices)).ToList();
                logger.Info("Ending View Model For Person at " + _clock.UtcNow);


                if (jobSchedulerIdList.Count() > 0)
                {
                    var beforeAfterForImageViewModelForScheduler = new BeforeAfterForImageViewModel();
                    var beforeAfterForImageViewModelList = new List<BeforeAfterForImageViewModel>();
                    foreach (var jobSchedulerId in jobSchedulerIdList)
                    {
                        var jobScheduler = jobSchedulerListForFranchisee.FirstOrDefault(x => x.Id == jobSchedulerId);
                        if (jobScheduler.AssigneeId != person.Id)
                        {
                            continue;
                        }
                        if (jobScheduler.EstimateId != null)
                        {
                            var isAnyImagePresent = beforeAfterImagesList.Any(x => x.EstimateId == jobScheduler.EstimateId);
                            if (isAnyImagePresent && jobScheduler.EstimateId != null)
                            {
                                continue;
                            }
                        }
                        else
                        {
                            var isAnyImagePresent = beforeAfterImagesList.Any(x => x.JobId == jobScheduler.JobId);
                            if (isAnyImagePresent && jobScheduler.JobId != null)
                            {
                                continue;
                            }
                        }
                        beforeAfterForImageViewModelForScheduler = new BeforeAfterForImageViewModel();
                        var exteriorImage = beforeAfterImagesListLocal.FirstOrDefault(x => x.TypeId == (long?)LookupTypes.ExteriorBuilding && x.SchedulerId == jobSchedulerId);

                        beforeAfterForImageViewModelForScheduler = CreateEmptyModel(null, null, exteriorImage, jobScheduler);
                        beforeAfterImagePerson.Add(beforeAfterForImageViewModelForScheduler);
                    }
                }


                beforeAfterImagePerson = beforeAfterImagePerson.Distinct().ToList();
                beforeAfterImagePerson = beforeAfterImagePerson.OrderByDescending(x => x.JobEstimateId).ToList();
                beforeAfterImagePerson = beforeAfterImagePerson.Distinct().ToList();
                beforeAfterImagePerson = beforeAfterImagePerson.Where(x => x.AfterServiceId != default(long?) && x.BeforeServiceId != default(long?)).ToList();

                beforeAfterForPersonViewModel.BeforeAfterImageViewModel = beforeAfterImagePerson;
                beforeAfterForPersonViewModel.CustomerImageViewModel = GetCustomerGroupedData(beforeAfterForPersonViewModel.BeforeAfterImageViewModel, person);


                logger.Info("Creating Non Residental View Model For Person at " + _clock.UtcNow);
                beforeAfterForPersonViewModel.NonResidentalCollection = CreateNonResidentalViewModel(beforeAfterImagePerson.Where(x => x.OrderNo == 100).ToList());
                logger.Info("Ending Creating Non Residental View Model For Person at " + _clock.UtcNow);
                beforeAfterForPersonViewModel.ResidentalCollection = beforeAfterImagePerson.Where(x => x.OrderNo == 1).ToList();
                beforeAfterForPersonViewModel.TotalCount = beforeAfterImagePerson.Count();

                beforeAfterForPersonViewModelList.Add(beforeAfterForPersonViewModel);
                beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel = beforeAfterForPersonViewModelList.Distinct().ToList();
            }
            beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel = beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel.OrderByDescending(x => x.BeforeAfterImageViewModel.Count()).ToList();
            beforeAfterForFranchieeAdminGroupedViewModel.TotalCount = personModel.Count();
            beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel =
            beforeAfterForFranchieeAdminGroupedViewModel.BeforeAfterPersonViewModel.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

            beforeAfterForFranchieeAdminGroupedViewModel.FrachiseeId = filter.FranchiseeId;
            beforeAfterForFranchieeAdminGroupedViewModel.StartDate = filter.StartDate;
            beforeAfterForFranchieeAdminGroupedViewModel.FrachiseeName = jobSchedulerListForFranchisee.Count() > 0 && jobSchedulerListForFranchisee.FirstOrDefault().Franchisee != null
                ? jobSchedulerListForFranchisee.FirstOrDefault().Franchisee.Organization.Name : "";
            beforeAfterForFranchieeAdminGroupedViewModel.EndDate = filter.EndDate;
            logger.Info("Ending The Request " + _clock.UtcNow);
            return beforeAfterForFranchieeAdminGroupedViewModel;
        }


        private List<PersonViewModel> GetGroupedPersonData(BeforeAfterImageFilter filter)
        {
            var userIdList = _organizationRoleUserRepository.Table.Where(x => x.IsActive && (filter.UserId == 0 || filter.UserId == null || filter.UserId == x.UserId) && x.OrganizationId == filter.FranchiseeId
           && (x.RoleId == (long)RoleType.SalesRep || x.RoleId == (long)RoleType.Technician)).ToList();
            if (userIdList.Count() == 0)
            {
                userIdList = _organizationRoleUserRepository.Table.Where(x => x.IsActive && (filter.UserId == x.UserId) && x.OrganizationId == filter.FranchiseeId
            ).ToList();
            }
            var userIdLists = userIdList.Select(x => x.UserId).ToList();
            var userLoginList = _userLoginRepository.Table.Where(x => userIdLists.Contains(x.Id)).ToList();
            var unlockedUsers = userLoginList.Where(x => !x.IsLocked).Select(x => x.Id);
            userIdList = userIdList.Where(x => unlockedUsers.Contains(x.UserId)).ToList();
            if (filter.RoleId == ((long?)RoleType.SalesRep) || filter.RoleId == ((long?)RoleType.Technician) ||
                filter.RoleId == ((long?)RoleType.OperationsManager))
            {
                userIdList = userIdList.Where(x => x.UserId == filter.LoggedUserId.GetValueOrDefault()).ToList();
            }

            var personModel = userIdList.Select(x => new PersonViewModel()
            {
                PersonName = x.Person.FirstName + " " + x.Person.LastName,
                UserId = x.UserId,
                Id = x.Id,
                RoleId = x.RoleId,
                Role = x.Role
            }).Distinct().ToList();

            return personModel;
        }

        private bool AddingBestPairForBeforeAfterImages(JobEstimateImage jobEstimateImage)
        {

            var beforeAfterImageDomain = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.ServiceId == jobEstimateImage.ServiceId && x.FileId == jobEstimateImage.FileId);
            if (beforeAfterImageDomain != null)
            {
                beforeAfterImageDomain.IsAddToLocalGallery = jobEstimateImage.IsAddToLocalGallery;
                beforeAfterImageDomain.IsBestImage = jobEstimateImage.IsBestImage;
                beforeAfterImageDomain.AddToGalleryDateTime = jobEstimateImage.AddToGalleryDateTime;
                beforeAfterImageDomain.BestFitMarkDateTime = jobEstimateImage.BestFitMarkDateTime;
                beforeAfterImageDomain.S3BucketURL = null;
                beforeAfterImageDomain.S3BucketThumbURL = null;
                _beforeAfterImagesRepository.Save(beforeAfterImageDomain);
            }
            else
            {
                return false;
            }
            return true;
        }

        private List<NonResidentalImageViewModel> CreateNonResidentalViewModel(List<BeforeAfterForImageViewModel> imageViewModel)
        {
            var nonResidentalImageViewModel = new NonResidentalImageViewModel();
            var nonResidentalImageViewModelList = new List<NonResidentalImageViewModel>();
            var groupedDataNonResidentalImages = imageViewModel.GroupBy(x => x.ToBeGroupedById).ToList();

            foreach (var groupedData in groupedDataNonResidentalImages)
            {
            }
            var groupedNonResidental = groupedDataNonResidentalImages.Select(x => new NonResidentalImageViewModel()
            {
                NonResidentalImageUrl = x.ToList().Select(x1 => x1.RelactiveLocationExteriorImageUrl).FirstOrDefault(),
                beforeAfterViewModel = x.ToList(),
                CustomerName = x.ToList().Select(x1 => x1.CustomerName).FirstOrDefault(),
                SchedulerNames = x.ToList().Select(x1 => x1.SchedulerNames).FirstOrDefault(),


            }).ToList();

            return groupedNonResidental;
        }

        private BeforeAfterForImageViewModel CreateEmptyModel(BeforeAfterImages jobEstimateBeforeCategory,
                BeforeAfterImages jobEstimateAfterCateogy, BeforeAfterImages jobEstimateBeforesExteriorImages
            , JobScheduler scheduler)
        {
            string linkUrl = "";
            if (scheduler == null)
            {
                return default(BeforeAfterForImageViewModel);
            }
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
                RelactiveLocationAfterImageUrl = jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.File != null ? GetBase64String(jobEstimateAfterCateogy.File) : "/Content/images/no_image_thumb.gif",
                RelactiveLocationBeforeImageUrl = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.File != null ? GetBase64String(jobEstimateBeforeCategory.File) : "/Content/images/no_image_thumb.gif",
                BeforeServiceId = jobEstimateBeforeCategory != null ? jobEstimateBeforeCategory.ServiceId : default(long?),
                AfterServiceId = jobEstimateAfterCateogy != null ? jobEstimateAfterCateogy.ServiceId : default(long?),
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
                RelactiveLocationExteriorImageUrl = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? GetBase64String(jobEstimateBeforesExteriorImages.File) : "/Content/images/no_image_thumb.gif",
                IsComercialClass = isNonResistianCLass,
                MarketingClass = marketingClass,
                OrderNo = isNonResistianCLass ? 1 : 100,
                SchedulerNames = scheduler.Job != null ? "J" + scheduler.JobId : "E" + scheduler.EstimateId,
                JobEstimateId = scheduler.Job != null ? scheduler.JobId : scheduler.EstimateId,
                CustomerName = scheduler.Job != null ? scheduler.Job.JobCustomer.CustomerName : scheduler.Estimate.JobCustomer.CustomerName,
                ToBeGroupedById = scheduler.Estimate != null ? scheduler.EstimateId : scheduler.JobId,
                RelactiveLocationAfterImageUrlThumb = jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.ThumbFileId != null ? (jobEstimateAfterCateogy.ThumbFile.RelativeLocation + "\\" + jobEstimateAfterCateogy.ThumbFile.Name).ToUrl() : "/Content/images/no_image_thumb.gif",
                RelactiveLocationBeforeImageUrlThumb = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.ThumbFileId != null ? (jobEstimateBeforeCategory.ThumbFile.RelativeLocation + "\\" + jobEstimateBeforeCategory.ThumbFile.Name).ToUrl() : "/Content/images/no_image_thumb.gif",


                S3BucketAfterImageUrlThumb = jobEstimateAfterCateogy != null && jobEstimateAfterCateogy.ThumbFileId != null ? (jobEstimateAfterCateogy.S3BucketThumbURL) : "/Content/images/no_image_thumb.gif",
                S3BucketBeforeImageUrlThumb = jobEstimateBeforeCategory != null && jobEstimateBeforeCategory.ThumbFileId != null ? (jobEstimateBeforeCategory.S3BucketThumbURL) : "/Content/images/no_image_thumb.gif",
                S3BucketExteriorImageUrlThumb = jobEstimateBeforesExteriorImages != null && jobEstimateBeforesExteriorImages.File != null ? jobEstimateBeforesExteriorImages.S3BucketURL : "/Content/images/no_image_thumb.gif",
            };

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

        public bool SaveEstimateWorth(EstimateWorthModel filter)
        {
            var jobScheduler = _jobschedulerRepository.Get(filter.JobSchedulerId.Value);
            jobScheduler.EstimateWorth = filter.Worth;
            jobScheduler.Estimate.Amount = filter.Worth.Value;
            _jobschedulerRepository.Save(jobScheduler);
            return true;

        }

        private List<CustomerForImageViewModel> GetCustomerGroupedData(List<BeforeAfterForImageViewModel> list, PersonViewModel person)
        {
            var groupedData = list.GroupBy(x => x.CustomerName);
            var customerForImageViewModelList = new List<CustomerForImageViewModel>();
            var customerForImageViewModel = new CustomerForImageViewModel();

            foreach (var groupData in groupedData)
            {
                customerForImageViewModel = new CustomerForImageViewModel();
                customerForImageViewModel.CustomerName = groupData.Key;
                customerForImageViewModel.PersonName = person.PersonName;
                customerForImageViewModel.RoleName = person.Role.Name;
                customerForImageViewModel.CustomerForImageModel = groupData.ToList();
                customerForImageViewModelList.Add(customerForImageViewModel);

            }
            return customerForImageViewModelList;
        }

        public bool ShiftImagesToInvoiceBuildMaterial(ShiftJobEstimateViewModel model)
        {
            var jobEstimateCategoryDomain = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.SchedulerId == model.SchedulerId);
            if (jobEstimateCategoryDomain == null)
            {
                jobEstimateCategoryDomain = _jobFactory.CreateJobEstimateCategory(model);
                _jobEstimateImageCategory.Save(jobEstimateCategoryDomain);
            }
            if (model.ShiftId == 121)
            {
                foreach (var fileId in model.FileIds)
                {

                    var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, jobEstimateCategoryDomain.Id, (long?)LookupTypes.ExteriorBuilding);
                    _jobEstimateServices.Save(jobEstimateBeforeService);
                    var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                    var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(model, buildingExteriorServiceId, (long?)LookupTypes.ExteriorBuilding, fileId);
                    _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                }
                return true;
            }
            else
            {
                foreach (var fileId in model.FileIds)
                {

                    var jobEstimateBeforeService = _jobFactory.CreateJobEstimatePairing(null, jobEstimateCategoryDomain.Id, (long?)LookupTypes.InvoiceImages);
                    _jobEstimateServices.Save(jobEstimateBeforeService);
                    var buildingExteriorServiceId = jobEstimateBeforeService.Id;
                    var jobEstimateBeforeServiceImage = _jobFactory.CreateJobEstimateImageDomain(model, buildingExteriorServiceId, (long?)LookupTypes.InvoiceImages, fileId);
                    _jobEstimateImage.Save(jobEstimateBeforeServiceImage);
                }
                return true;
            }
        }

        public bool EditEmailTemplate(MailTemplateEditModel filter)
        {
            filter.Body = filter.Body.Replace("&nbsp;&nbsp;&nbsp;", "   ");
            filter.Body = filter.Body.Replace("&nbsp;&nbsp;", "  ");
            filter.Body = filter.Body.Replace("&nbsp;", " ");
            var emailTemplate = _emailTemplateRepository.Table.FirstOrDefault(x => x.NotificationTypeId == filter.EmailTemplateId && x.LanguageId == filter.LanguageId);

            if (emailTemplate != null)
            {
                emailTemplate.Body = filter.Body;
                _emailTemplateRepository.Save(emailTemplate);
            }
            else
            {
                emailTemplate = _emailTemplateRepository.Table.FirstOrDefault(x => x.Id == filter.EmailTemplateId);
                if (emailTemplate != null)
                {
                    var emailTemplateDomain = new EmailTemplate()
                    {
                        Description = emailTemplate.Description,
                        Subject = filter.Subject,
                        Body = filter.Body,
                        LanguageId = (long)filter.LanguageId,
                        NotificationTypeId = emailTemplate.NotificationTypeId,
                        IsNew = true,
                        Title = emailTemplate.Title,
                        isActive = true
                    };
                    _emailTemplateRepository.Save(emailTemplateDomain);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private bool SaveDebuggerLog(List<DebuggerLogModel> list)
        {
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
                _debuggerLogsRepository.Save(domain);
            }
            return true;
        }

        private string GettingChangesInServices(JobEstimateServiceViewModel fromUi, JobEstimateServices fromDb)
        {
            var description = "";
            if (fromUi.BuildingLocation != fromDb.BuildingLocation)
            {
                description += "Changes in Building Location from " + fromDb.BuildingLocation + "to " + fromUi.BuildingLocation;
            }

            if (fromUi.CompamyName != fromDb.CompanyName)
            {
                description += "Changes in Company Name from " + fromDb.CompanyName + "to " + fromUi.CompamyName;
            }

            if (fromUi.FinishMaterial != fromDb.FinishMaterial)
            {
                description += "Changes in Finsish Material from " + fromDb.FinishMaterial + "to " + fromUi.FinishMaterial;
            }

            if (fromUi.FloorNumber != fromDb.FloorNumber)
            {
                description += "Changes in Floor Number from " + fromDb.FloorNumber + "to " + fromUi.FloorNumber;
            }

            if (fromUi.MaidJanitorial != fromDb.MAIDJANITORIAL)
            {
                description += "Changes in Maid Janitorial Number from " + fromDb.MAIDJANITORIAL + "to " + fromUi.MaidJanitorial;
            }

            if (fromUi.MaidService != fromDb.MaidService)
            {
                description += "Changes in Maid Service Number from " + fromDb.MaidService + "to " + fromUi.MaidService;
            }

            if (fromUi.PropertyManager != fromDb.PropertyManager)
            {
                description += "Changes in Property Manager Number from " + fromDb.PropertyManager + "to " + fromUi.PropertyManager;
            }

            if (fromUi.SurfaceColor != fromDb.SurfaceColor)
            {
                description += "Changes in Surface Color Number from " + fromDb.SurfaceColor + "to " + fromUi.SurfaceColor;
            }

            if (fromUi.SurfaceMaterial != fromDb.SurfaceMaterial)
            {
                description += "Changes in Surface Material Number from " + fromDb.SurfaceMaterial + "to " + fromUi.SurfaceMaterial;
            }

            if (fromUi.SurfaceType != fromDb.SurfaceType)
            {
                description += "Changes in Surface Type  Number from " + fromDb.SurfaceType + "to " + fromUi.SurfaceType;
            }


            return description;
        }

        public bool SaveInvoiceRequired(InvoiceRequiredViewModel filter)
        {
            var jobScheduler = _jobschedulerRepository.Table.FirstOrDefault(x => x.Id == filter.SchedulerId);
            if (jobScheduler != null)
            {
                jobScheduler.IsInvoiceRequired = filter.IsInvoiceRequired;
                jobScheduler.InvoiceReason = filter.InvoiceReason;
                _jobschedulerRepository.Save(jobScheduler);
                return true;
            }
            else
            {
                return false;
            }
        }


        public FileUploadModel GetInvoiceMediaList(long rowId, long mediaType, long? estimateId, long? userId)
        {
            var jobSchedulerDomain = default(JobScheduler);
            var id = default(long?);
            var model = new FileUploadModel { };
            model.Resources = new List<JobResourceEditModel>();
            var mediaList = new List<JobResource>();
            var list = new JobEstimateCategoryViewModel();
            var imageClassViewModel = new JobEstimateImageCategory();
            var jobEstimateServices = new List<JobEstimateServices>();
            var imageClassViewModels = new List<JobEstimateServiceViewModel>();
            var schedulerId = default(long?);

            jobSchedulerDomain = _jobschedulerRepository.Table.FirstOrDefault(x => x.Id == rowId);
            if (jobSchedulerDomain == default(JobScheduler))
            {
                jobSchedulerDomain = _jobschedulerRepository.Table.FirstOrDefault(x => x.Id == rowId);
            }
            if (jobSchedulerDomain != default(JobScheduler))
            {
                if (mediaType == (long)ScheduleType.Estimate)
                {
                    id = jobSchedulerDomain.EstimateId;
                }
            }

            if (id <= 0)
                return new FileUploadModel { };

            if (mediaType == (long)ScheduleType.Estimate)
            {
                var jobEstimate = _jobEstimateRepository.Get(id.Value);
                if (jobEstimate.ParentEstimateId == null)
                {
                    imageClassViewModel = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.EstimateId == id);
                }
                else
                {
                    //imageClassViewModel = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.EstimateId == id || x.EstimateId == jobEstimate.ParentEstimateId);
                    imageClassViewModel = _jobEstimateImageCategory.Table.FirstOrDefault(x => x.EstimateId == id);
                }
                schedulerId = imageClassViewModel != null ? imageClassViewModel.SchedulerId.GetValueOrDefault() : default(long?);
                if (imageClassViewModel != null && imageClassViewModel != null)
                {
                    jobEstimateServices = _jobEstimateServices.Table.Where(x => x.CategoryId == imageClassViewModel.Id).ToList();
                }
            }

            var beforeDuringInvoiceSliderImages = jobEstimateServices.Where(x => ((x.TypeId == (long)LookupTypes.InvoiceImages))).ToList();

            var parentChildJobUdring = jobEstimateServices.Select(x => x.Id).ToList();

            list = JobEstimateImageMediaModel(beforeDuringInvoiceSliderImages, parentChildJobUdring, id, mediaType, schedulerId, userId);
            model.JobId = mediaType == (long)ScheduleType.Job ? id : default(long);
            model.EstimateId = mediaType == (long)ScheduleType.Estimate ? id : default(long);

            model.ImageList = _jobFactory.CreatePairingModel(imageClassViewModel, list);
            return model;
        }


        private JobEstimateCategoryViewModel JobEstimateImageMediaModel(List<JobEstimateServices> jobEstimateImageServices, List<long> jobEstimateServiceGrouped, long? jobEstimateId, long? mediaType, long? schedulerId, long? userId)
        {
            JobEstimateCategoryViewModel jobEstimateCategoryViewModel = new JobEstimateCategoryViewModel();

            List<ImagePairs> jobEstimateCategoryImagePairs = new List<ImagePairs>();
            List<ImagePairs> invoiceImagePairs = new List<ImagePairs>();
            var filesBeforeIds = new List<Application.Domain.File>();
            var filesAfterIds = new List<Application.Domain.File>();

            JobEstimateServiceViewModel jobEstimateInvoiceImagesServiceViewModel = new JobEstimateServiceViewModel();

            var jobEstimateServices = _jobEstimateServices.Table.Where(x => jobEstimateServiceGrouped.Contains(x.Id)).ToList();
            var jobEstimateImages = _jobEstimateImage.Table.Where(x => x.ServiceId.HasValue && jobEstimateServiceGrouped.Contains(x.ServiceId.Value)).ToList();
            int beforeAfterIndex = 0;

            if (jobEstimateImageServices.Where(x => x.TypeId == (long)LookupTypes.InvoiceImages).Count() > 0)
            {
                var invoicesImages = jobEstimateImageServices.Where(x => x.TypeId == (long)LookupTypes.InvoiceImages).Select(x => x.Id).ToList();
                var filesInvoiceIds = jobEstimateImages.Where(x => x.ServiceId.HasValue && invoicesImages.Contains(x.ServiceId.Value)).Select(x => x.File).ToList();
                var invoiceImageJobFileViewModel = filesInvoiceIds.Select(x => _jobFactory.CreateServiceFileViewModel(x, null, userId));
                var invoiceImageJobViewModel = _jobFactory.CreateServiceViewModel(null, invoiceImageJobFileViewModel, false, null);

                jobEstimateInvoiceImagesServiceViewModel = invoiceImageJobViewModel;
            }

            var jobEstimateCategoryModel = new JobEstimateCategoryViewModel()
            {
                EstimateId = mediaType == (long)ScheduleType.Estimate ? jobEstimateId : null,
                JobId = mediaType == (long)ScheduleType.Job ? jobEstimateId : null,
                SchedulerId = schedulerId,
                InvoiceImages = jobEstimateInvoiceImagesServiceViewModel,
            };
            return jobEstimateCategoryModel;
        }


        public ScheduleAvailabilityFilterViewModel CheckAvailabilityList(ScheduleAvailabilityFilterList model, bool isVacation)
        {
            //int assigneeNotAvailable = 0;
            ScheduleAvailabilityFilterViewModel scheduleAvailabilityFilterViewModel = new ScheduleAvailabilityFilterViewModel();
            List<string> assigneeNotAvailableName = new List<string>();
            foreach (var assignee in model.ScheduleAvailabilityFilter)
            {
                var orgRoleUser = _organizationRoleUserRepository.Get(assignee.AssigneeId);
                var startDateToCompare = assignee.StartDate.AddMinutes(-14);
                var endDateToCompare = assignee.EndDate.AddMinutes(14);
                var assigneeInfo = new List<JobScheduler>();
                if (isVacation)
                {
                    assigneeInfo = _jobSchedulerRepository.Table.Where(x => assignee.AssigneeId == x.AssigneeId && (assignee.JobId <= 0 || x.Id != assignee.JobId)
                                         && x.IsActive).ToList();
                }
                else
                {
                    if (orgRoleUser != null)
                    {
                        assigneeInfo = _jobSchedulerRepository.Table.Where(x => (x.PersonId == orgRoleUser.UserId) && (assignee.JobId <= 0 || (x.JobId != assignee.JobId && x.EstimateId != assignee.JobId && x.MeetingID != assignee.JobId))
                                           && x.IsActive).ToList();
                    }
                    else
                    {
                        assigneeInfo = _jobSchedulerRepository.Table.Where(x => assignee.AssigneeId == x.AssigneeId && (assignee.JobId <= 0 || (x.JobId != assignee.JobId && x.EstimateId != assignee.JobId && x.MeetingID != assignee.JobId))
                                            && x.IsActive).ToList();
                    }
                }
                assigneeInfo = assigneeInfo.Where(x => (startDateToCompare <= x.StartDate && assignee.EndDate >= x.StartDate)
                            || (startDateToCompare <= x.EndDate && assignee.EndDate >= x.EndDate)
                            || (startDateToCompare >= x.StartDate && assignee.EndDate <= x.EndDate)
                            || (endDateToCompare >= x.StartDate && assignee.EndDate <= x.EndDate)
                            || (endDateToCompare <= x.EndDate && assignee.EndDate >= x.EndDate)).ToList();

                if (assigneeInfo.Any())
                {
                    assigneeNotAvailableName.Add(orgRoleUser.Person.FirstName);
                }
            }
            if (assigneeNotAvailableName.Count > 0)
            {
                scheduleAvailabilityFilterViewModel.IsAvailable = false;
                scheduleAvailabilityFilterViewModel.AssigneeNames = String.Join(", ", assigneeNotAvailableName.Distinct());
            }
            else
            {
                scheduleAvailabilityFilterViewModel.IsAvailable = true;
            }
            return scheduleAvailabilityFilterViewModel;
        }


        public IEnumerable<InvoiceLineImageModel> SaveInvoiceLineMediaFiles(FileUploadModel model)
        {
            long? fileId = default(long?);
            List<InvoiceLineImageModel> list = new List<InvoiceLineImageModel>();
            List<BeforeAfterImageModel> listBeforeAfter = new List<BeforeAfterImageModel>();
            foreach (var fileModel in model.FileList)
            {
                var count = 1;
                _logService.Debug("File upload started: " + fileModel.Caption + "Count file: " + count);
                try
                {
                    if (fileModel.FileId == null)
                    {
                        var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
                        var destination = MediaLocationHelper.GetDocumentImageLocation();
                        var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                            : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
                        var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
                        fileModel.Name = destFileName + fileModel.Extension;
                        fileModel.RelativeLocation = MediaLocationHelper.GetDocumentImageLocation().Path;
                        var file = _fileService.SaveModel(fileModel);
                        fileId = file.Id;

                        var jobResource = _jobFactory.CreateDomain(model, file.Id);
                        _jobResourceRepository.Save(jobResource);

                        var a = new InvoiceLineImageModel
                        {
                            FileId = fileId,
                            RelativeLocation = (fileModel.RelativeLocation + "\\" + fileModel.Name).ToUrl()
                        };
                        list.Add(a);

                        var isIFrame = (fileModel.Extension == ".pdf" || fileModel.Extension == ".docx" || fileModel.Extension == ".xlsx") ? true : false;

                        var b = new BeforeAfterImageModel
                        {
                            FileId = jobResource.FileId,
                            RelativeLocation = (fileModel.RelativeLocation + "\\" + fileModel.Name).ToUrl(),
                            IsIFrame = isIFrame,
                            IFrameLocation = isIFrame ? _settings.SiteRootUrl + "/Media//" + fileModel.Name : ""
                        };
                        listBeforeAfter.Add(b);
                    }
                }
                catch (Exception ex)
                {
                    _logService.Error(ex + "   " + ex.InnerException);
                    continue;
                }
                count++;
            }
            _unitOfWork.SaveChanges();
            return list;
        }
        public bool SaveImageRotation(RotationImageModel model)
        {
            var jobEstimateImageThumbFileId = _jobEstimateImage.Table.FirstOrDefault(x => x.FileId == model.FileId).ThumbFileId;
            var fileData = _fileRepository.Table.Where(x => x.Id == jobEstimateImageThumbFileId || x.Id == model.FileId).ToList();
            foreach (var data in fileData)
            {
                data.css = model.Css;
                _fileRepository.Save(data);
            }
            var result = true;
            return result;
        }
        public bool SaveCroppedImage(CroppedImageModel model, long userId)
        {
            try
            {
                var beforeAfterImagesFileId = _beforeAfterImagesRepository.IncludeMultiple(y => y.File).FirstOrDefault(x => x.Id == model.BeforeAfterId);
                if (beforeAfterImagesFileId != null && beforeAfterImagesFileId.FileId != null)
                {
                    var NameImage = beforeAfterImagesFileId.File.Name.Split('.');
                    var fileNmae = NameImage[0] + "_Crop";
                    var fileData = beforeAfterImagesFileId.File.RelativeLocation + "\\" + fileNmae + "." + NameImage[1];
                    System.IO.File.WriteAllBytes(@fileData, Convert.FromBase64String(model.Base64));


                    //var dataRecorderMetaData = new DataRecorderMetaData { DateCreated = _clock.UtcNow, CreatedBy = userId, IsNew = true };
                    var fileItem = new Application.Domain.File()
                    {
                        Name = fileNmae + "." + NameImage[1],
                        Caption = beforeAfterImagesFileId.File.Caption,
                        Size = model.Size,
                        RelativeLocation = beforeAfterImagesFileId.File.RelativeLocation,
                        MimeType = model.Type,
                        DataRecorderMetaData = new DataRecorderMetaData(),
                        IsDeleted = false,
                        css = beforeAfterImagesFileId.File.css,
                        IsFileToBeDeleted = beforeAfterImagesFileId.File.IsFileToBeDeleted,
                        IsNew = true
                    };
                    _fileRepository.Save(fileItem);
                    _unitOfWork.SaveChanges();

                    beforeAfterImagesFileId.IsImageCropped = true;
                    beforeAfterImagesFileId.CroppedImageId = fileItem.Id;
                    _beforeAfterImagesRepository.Save(beforeAfterImagesFileId);
                    _unitOfWork.SaveChanges();

                    var fileDomainLocal = _fileRepository.Get(beforeAfterImagesFileId.CroppedImageId.GetValueOrDefault());
                    //var fileDomainName = fileDomainLocal.Name.Split('.');
                    //fileDomainLocal.Name = fileDomainName[0] + "Thumb." + fileDomainName[1];
                    long? thumbFileId = null;
                    if (fileDomainLocal != null)
                    {
                        var fileModel = _beforeAfterThumbNameService.CreateImageThumb(fileDomainLocal, "");
                        thumbFileId = fileModel.FileId;
                    }
                    if (thumbFileId != null)
                    {
                        beforeAfterImagesFileId.CroppedImageThumbId = thumbFileId;
                        _beforeAfterImagesRepository.Save(beforeAfterImagesFileId);
                        _unitOfWork.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public LocalMarketingReviewModel GetLocalMarketingReview(LocalMarketingReviewFilter filter)
        {
            LocalMarketingReviewModel localMarketingReviewModel = new LocalMarketingReviewModel();
            try
            {
                if (!filter.IsDateFilter)
                {
                    if (filter.StartDate == null)
                    {
                        DateTime date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                        int day = (int)date.DayOfWeek;
                        DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                        DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                        filter.StartDate = Monday.AddDays(-7);
                        filter.EndDate = Sunday.AddDays(-7);
                    }
                    else
                    {
                        DateTime date = new DateTime(filter.StartDate.GetValueOrDefault().Year, filter.StartDate.GetValueOrDefault().Month,
                            filter.StartDate.GetValueOrDefault().Day);
                        int day = (int)date.DayOfWeek;
                        if (day == 0)
                        {
                            filter.StartDate = filter.StartDate.GetValueOrDefault().AddDays(1).Date;
                            filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(6).Date;
                        }
                        else
                        {
                            DateTime Monday = date.AddDays((-1) * (day == 0 ? 6 : day - 1));
                            DateTime Sunday = date.AddDays((1) * (day == 0 ? day : 7 - day));
                            filter.StartDate = Monday;
                            filter.EndDate = filter.StartDate.GetValueOrDefault().AddDays(7);
                        }
                    }
                }

                if (filter.FranchiseeId == null)
                {

                    if (filter.RoleId == (long?)RoleType.SuperAdmin || filter.RoleId == (long?)RoleType.FrontOfficeExecutive)
                    {
                        filter.FranchiseeId = null;
                    }
                    else
                    {
                        filter.FranchiseeId = filter.LoggedInFranchiseeId;
                    }
                }
                var fileList = _fileRepository.Table.ToList();
                var SalesRepList = _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.RoleId == (long)RoleType.SalesRep).OrderBy(y => y.Person.FirstName).ToList();
                var TechnicianList = _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.RoleId == (long)RoleType.Technician).OrderBy(y => y.Person.FirstName).ToList();
                var organizationRollUserList = SalesRepList.Concat(TechnicianList).ToList();

                //var orgRoleUserIds = organizationRollUserList.Select(x => x.UserId).ToList();
                //var activeUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && !x.IsLocked).Select(x => x.Id).ToList();
                //organizationRollUserList = organizationRollUserList.Where(x => activeUers.Contains(x.UserId)).ToList();

                var orgRoleUserIds = organizationRollUserList.Select(x => x.UserId).ToList();
                var inActiveUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && x.IsLocked).Select(x => x.Id).ToList();

                var jobEstimateData = _jobEstimateRepository.Table.Where(x => x.StartDate >= filter.StartDate && x.EndDate <= filter.EndDate).ToList();
                var beforeAfter = GetBeforeAfterImagesList(filter);

                var jobSchedulerList = GetJobSchedulerList(filter, beforeAfter);

                var franchiseeList = GetFranchiseeList(filter, organizationRollUserList, jobSchedulerList);

                localMarketingReviewModel.StartDate = filter.StartDate;
                localMarketingReviewModel.EndDate = filter.EndDate;
                localMarketingReviewModel.IsFranchiseeAdmin = filter.RoleId == (long?)RoleType.FranchiseeAdmin ? true : false;
                foreach (var franchisee in franchiseeList)
                {
                    FranchiseeListLocalMarketingModel franchiseeListLocalMarketingModel = new FranchiseeListLocalMarketingModel();
                    franchiseeListLocalMarketingModel.FranchiseeId = franchisee.Id;
                    franchiseeListLocalMarketingModel.FranchiseeName = franchisee.Organization.Name;
                    localMarketingReviewModel.FranchiseeListLocalMarketingModel.Add(franchiseeListLocalMarketingModel);
                }

                foreach (var franchisee in localMarketingReviewModel.FranchiseeListLocalMarketingModel)
                {

                    var users = GetUsersList(filter, organizationRollUserList, franchisee);
                    if (users.Count() == 0)
                    {
                        franchisee.PersonCount = 0;
                        franchisee.Message = "No SalesRep/Technician Available In This Franchisee.";
                    }
                    else
                    {
                        franchisee.PersonCount = users.Count();
                        franchisee.Message = "SalesRep/Technician Are Available In This Franchisee.";

                        foreach (var user in users)
                        {
                            var isActiveUser = inActiveUers.Any(x => x == user.UserId);
                            PersonListLocalMarketingModel person = new PersonListLocalMarketingModel();
                            person.Id = user.Id;
                            person.PersonId = user.Person.Id;
                            person.PersonName = user.Person.FirstName + " " + user.Person.LastName;
                            person.PersonRoleId = user.RoleId;
                            person.PersonRole = user.Role != null && user.Role.Name != null ? user.Role.Name : "";
                            person.IsActiveUser = isActiveUser;
                            franchisee.PersonListLocalMarketingModel.Add(person);
                        }
                    }
                }
                foreach (var item in localMarketingReviewModel.FranchiseeListLocalMarketingModel)
                {
                    var name = item.FranchiseeName;
                    foreach (var person in item.PersonListLocalMarketingModel)
                    {
                        var jobScedulerforPerson = jobSchedulerList.Where(x => x.FranchiseeId == item.FranchiseeId && x.PersonId == person.PersonId && x.AssigneeId == person.Id && (x.EstimateId != null || x.JobId != null)).ToList();
                        if (jobScedulerforPerson.Count() == 0)
                        {
                            person.SchedulerCount = 0;
                            person.Message = "No Job/Estimate Assige To This Person";
                        }
                        else
                        {
                            person.SchedulerCount = jobScedulerforPerson.Count();
                            person.Message = "Job/Estimate Assiged To This Person";
                            foreach (var jobScheduler in jobScedulerforPerson)
                            {
                                SchedulerListLocalMarketingModel schedulerListLocalMarketingModel = new SchedulerListLocalMarketingModel();
                                //var beforeImageList = beforeAfter.Where(x => (jobScheduler.EstimateId != null ? jobScheduler.EstimateId == x.EstimateId : jobScheduler.JobId == x.JobId) && x.TypeId == (long)LookupTypes.BeforeWork && x.PairId == null).ToList();
                                var beforeImageList = beforeAfter.Where(x => (jobScheduler.EstimateId != null ? jobScheduler.EstimateId == x.EstimateId : (jobScheduler.Job.EstimateId != null ? jobScheduler.Job.EstimateId == x.EstimateId : jobScheduler.JobId == x.JobId)) && x.TypeId == (long)LookupTypes.BeforeWork && x.PairId == null).ToList();
                                var exteriorImageList = beforeAfter.Where(x => (jobScheduler.EstimateId != null ? jobScheduler.EstimateId == x.EstimateId : jobScheduler.JobId == x.JobId) && x.TypeId == (long)LookupTypes.ExteriorBuilding && x.PairId == null).ToList();
                                if (exteriorImageList.Count() > 0)
                                {
                                    beforeImageList = exteriorImageList.Concat(beforeImageList).ToList();
                                }
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
                                    var data = jobEstimateData.FirstOrDefault(x => x.Id == jobScheduler.EstimateId);
                                    if (data != null)
                                    {
                                        estimateValue = jobEstimateData.FirstOrDefault(x => x.Id == jobScheduler.EstimateId).Amount;
                                    }
                                    else
                                    {
                                        estimateValue = default(decimal);
                                    }
                                }
                                var isEstimate = jobScheduler.Job != null ? false : true;
                                //var marketingClass = isEstimate == true ? jobEstimateData.FirstOrDefault(x => x.Id == jobScheduler.EstimateId).MarketingClass.Name : "";
                                schedulerListLocalMarketingModel.SchedulerName = jobScheduler.Job != null ? "J" + jobScheduler.JobId : "E" + jobScheduler.EstimateId;
                                schedulerListLocalMarketingModel.EstimateValue = estimateValue != null ? estimateValue : default(decimal);
                                schedulerListLocalMarketingModel.SchedulerUrl = linkUrl;
                                schedulerListLocalMarketingModel.Title = jobScheduler.Title;
                                schedulerListLocalMarketingModel.CustomerName = jobScheduler.Job != null && jobScheduler.Job.JobCustomer != null && jobScheduler.Job.JobCustomer.CustomerName != null ? jobScheduler.Job.JobCustomer.CustomerName : (jobScheduler != null && jobScheduler.Estimate != null && jobScheduler.Estimate.JobCustomer != null && jobScheduler.Estimate.JobCustomer.CustomerName != null ? jobScheduler.Estimate.JobCustomer.CustomerName : "");
                                //schedulerListLocalMarketingModel.MarketingClass = marketingClass;
                                schedulerListLocalMarketingModel.MarketingClass = "";
                                person.SchedulerListLocalMarketingModel.Add(schedulerListLocalMarketingModel);

                                if (beforeImageList.Count() == 0)
                                {
                                    schedulerListLocalMarketingModel.Count = 0;
                                    schedulerListLocalMarketingModel.Message = "No Before/After Images Uploaded For This Scheduler.";
                                    BeforeAfterImagesLocalMarketingModel beforeAfterImagesLocalMarketingModel = new BeforeAfterImagesLocalMarketingModel();
                                    beforeAfterImagesLocalMarketingModel.S3BucketAfterImageUrlThumb = "";
                                    beforeAfterImagesLocalMarketingModel.S3BucketBeforeImageUrlThumb = "";
                                    beforeAfterImagesLocalMarketingModel.RelactiveLocationAfterImageUrlThumb = "";
                                    beforeAfterImagesLocalMarketingModel.RelactiveLocationBeforeImageUrlThumb = "";
                                    beforeAfterImagesLocalMarketingModel.RelactiveLocationAfterImage = "";
                                    beforeAfterImagesLocalMarketingModel.RelactiveLocationBeforeImage = "";
                                    beforeAfterImagesLocalMarketingModel.EmptyImage = "/Content/images/no_image_thumb.gif";
                                    schedulerListLocalMarketingModel.BeforeAfterImagesLocalMarketingModel.Add(beforeAfterImagesLocalMarketingModel);
                                }
                                else
                                {
                                    schedulerListLocalMarketingModel.Count = beforeImageList.Count();
                                    schedulerListLocalMarketingModel.Message = "Before/After Images Uploaded For This Scheduler.";
                                    foreach (var beforeImage in beforeImageList)
                                    {
                                        if (beforeImage.TypeId == (long)LookupTypes.ExteriorBuilding)
                                        {
                                            BeforeAfterImagesLocalMarketingModel beforeAfterImagesLocalMarketingModel = new BeforeAfterImagesLocalMarketingModel();
                                            var scheduler = beforeImage != null ? beforeImage.JobScheduler : null;

                                            beforeAfterImagesLocalMarketingModel.ImageTypeId = (long)LookupTypes.ExteriorBuilding;
                                            beforeAfterImagesLocalMarketingModel.ExteriorImageId = beforeImage != null ? beforeImage.Id : default(long);
                                            beforeAfterImagesLocalMarketingModel.ExteriorImageCss = beforeImage.File != null && beforeImage.File.css != null ? beforeImage.File.css : "rotate(0)";
                                            beforeAfterImagesLocalMarketingModel.ExteriorImageTitle = scheduler != null && scheduler.Title != null ? scheduler.Title : "";
                                            beforeAfterImagesLocalMarketingModel.S3BucketBeforeImageUrlThumb = beforeImage != null ? (beforeImage.S3BucketThumbURL != null ? beforeImage.S3BucketThumbURL : (beforeImage.S3BucketURL != null ? beforeImage.S3BucketURL : "")) : "";
                                            beforeAfterImagesLocalMarketingModel.RelactiveLocationBeforeImageUrlThumb = beforeImage != null && beforeImage.ThumbFileId != null ? (beforeImage.ThumbFile.RelativeLocation + "\\" + beforeImage.ThumbFile.Name).ToUrl() : "";
                                            beforeAfterImagesLocalMarketingModel.RelactiveLocationBeforeImage = beforeImage != null && beforeImage.File != null && beforeImage.File.RelativeLocation != null && beforeImage.File.Name != null ? (beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name).ToUrl() : "";
                                            beforeAfterImagesLocalMarketingModel.EmptyImage = "/Content/images/no_image_thumb.gif";
                                            beforeAfterImagesLocalMarketingModel.ExteriorImageFileId = beforeImage != null && beforeImage.FileId != null ? beforeImage.FileId : null;
                                            beforeAfterImagesLocalMarketingModel.IsImagePairReviewed = beforeImage != null && beforeImage.IsImagePairReviewed != false ? true : false;

                                            schedulerListLocalMarketingModel.BeforeAfterImagesLocalMarketingModel.Add(beforeAfterImagesLocalMarketingModel);
                                        }
                                        else
                                        {
                                            BeforeAfterImagesLocalMarketingModel beforeAfterImagesLocalMarketingModel = new BeforeAfterImagesLocalMarketingModel();
                                            var afterImage = beforeAfter.FirstOrDefault(x => x.PairId == beforeImage.Id);
                                            var scheduler = beforeImage != null ? beforeImage.JobScheduler : null;
                                            beforeAfterImagesLocalMarketingModel.ImageTypeId = (long)LookupTypes.BeforeWork;
                                            beforeAfterImagesLocalMarketingModel.BeforeImageId = beforeImage != null ? beforeImage.Id : default(long);
                                            beforeAfterImagesLocalMarketingModel.AfterImageId = afterImage != null ? afterImage.Id : default(long);
                                            beforeAfterImagesLocalMarketingModel.AfterCss = afterImage != null && afterImage.File != null && afterImage.File.css != null ? afterImage.File.css : "rotate(0)";
                                            beforeAfterImagesLocalMarketingModel.BeforeCss = beforeImage.File != null && beforeImage.File.css != null ? beforeImage.File.css : "rotate(0)";
                                            beforeAfterImagesLocalMarketingModel.IsBestPicture = beforeImage.IsBestImage;
                                            beforeAfterImagesLocalMarketingModel.IsAddToLocalGallery = beforeImage.IsAddToLocalGallery;
                                            beforeAfterImagesLocalMarketingModel.ServicesType = beforeImage.ServiceType != null ? beforeImage.ServiceType.Name : "";
                                            beforeAfterImagesLocalMarketingModel.SurfaceColor = beforeImage.SurfaceColor != null ? beforeImage.SurfaceColor : "";
                                            beforeAfterImagesLocalMarketingModel.SurfaceType = beforeImage.SurfaceType != null ? beforeImage.SurfaceType : "";
                                            beforeAfterImagesLocalMarketingModel.FinishMaterial = beforeImage.FinishMaterial != null ? beforeImage.FinishMaterial : "";
                                            beforeAfterImagesLocalMarketingModel.SurfaceMaterial = beforeImage.SurfaceMaterial != null ? beforeImage.SurfaceMaterial : "";
                                            beforeAfterImagesLocalMarketingModel.BuildingLocation = beforeImage.BuildingLocation != null ? beforeImage.BuildingLocation : "";
                                            beforeAfterImagesLocalMarketingModel.Title = scheduler != null && scheduler.Title != null ? scheduler.Title : "";
                                            //beforeAfterImagesLocalMarketingModel.CustomerName = scheduler != null && scheduler.Job != null && scheduler.Job.JobCustomer != null && scheduler.Job.JobCustomer.CustomerName != null ? scheduler.Job.JobCustomer.CustomerName : scheduler != null && scheduler.Estimate != null && scheduler.Estimate.JobCustomer != null && scheduler.Estimate.JobCustomer.CustomerName != null ? scheduler.Estimate.JobCustomer.CustomerName : "";
                                            //beforeAfterImagesLocalMarketingModel.S3BucketAfterImageUrlThumb = afterImage != null && afterImage.ThumbFileId != null && afterImage.S3BucketThumbURL != null ? (afterImage.S3BucketThumbURL != null ? afterImage.S3BucketThumbURL : (afterImage.S3BucketURL != null ? afterImage.S3BucketURL : "/Content/images/no_image_thumb.gif")) : (afterImage != null && afterImage.ThumbFileId != null && afterImage.File.RelativeLocation != null ? afterImage.File.RelativeLocation + "\\" + afterImage.File.Name : "/Content/images/no_image_thumb.gif");
                                            //beforeAfterImagesLocalMarketingModel.S3BucketBeforeImageUrlThumb = beforeImage != null && beforeImage.ThumbFileId != null && beforeImage.S3BucketThumbURL != null ? (beforeImage.S3BucketThumbURL != null ? beforeImage.S3BucketThumbURL : (beforeImage.S3BucketURL != null ? beforeImage.S3BucketURL : "/Content/images/no_image_thumb.gif")) : (beforeImage.File != null && beforeImage.File.RelativeLocation != null ? beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name : "/Content/images/no_image_thumb.gif");
                                            beforeAfterImagesLocalMarketingModel.IsBeforeImageCroped = beforeImage != null ? (beforeImage.CroppedImageThumbId != null ? (beforeImage.CroppedImageId != null ? true : false) : false) : false;
                                            beforeAfterImagesLocalMarketingModel.IsAfterImageCroped = afterImage != null ? (afterImage.CroppedImageThumbId != null ? (afterImage.CroppedImageId != null ? true : false) : false) : false;
                                            
                                            beforeAfterImagesLocalMarketingModel.S3BucketAfterImageUrlThumb = afterImage != null ? (afterImage.S3BucketThumbURL != null ? afterImage.S3BucketThumbURL : (afterImage.S3BucketURL != null ? afterImage.S3BucketURL : "")) : "";
                                            beforeAfterImagesLocalMarketingModel.S3BucketBeforeImageUrlThumb = beforeImage != null ? (beforeImage.S3BucketThumbURL != null ? beforeImage.S3BucketThumbURL : (beforeImage.S3BucketURL != null ? beforeImage.S3BucketURL : "")) : "";
                                            beforeAfterImagesLocalMarketingModel.RelactiveLocationAfterImageUrlThumb = afterImage != null && afterImage.ThumbFileId != null ? (afterImage.ThumbFile.RelativeLocation + "\\" + afterImage.ThumbFile.Name).ToUrl() : "";
                                            beforeAfterImagesLocalMarketingModel.RelactiveLocationBeforeImageUrlThumb = beforeImage != null && beforeImage.ThumbFileId != null ? (beforeImage.ThumbFile.RelativeLocation + "\\" + beforeImage.ThumbFile.Name).ToUrl() : "";
                                            if (beforeAfterImagesLocalMarketingModel.IsBeforeImageCroped && fileList.Count() > 0)
                                            {
                                                var file = beforeImage.CroppedImageThumbId != null ? fileList.FirstOrDefault(x => x.Id == beforeImage.CroppedImageThumbId) : fileList.FirstOrDefault(x => x.Id == beforeImage.CroppedImageId);
                                                beforeAfterImagesLocalMarketingModel.RelactiveLocationBeforeImageUrlThumb = file != null ? (file.RelativeLocation + "\\" + file.Name).ToUrl() : "";
                                            }
                                            if (beforeAfterImagesLocalMarketingModel.IsAfterImageCroped && fileList.Count() > 0)
                                            {
                                                var file = afterImage.CroppedImageThumbId != null ? fileList.FirstOrDefault(x => x.Id == afterImage.CroppedImageThumbId) : fileList.FirstOrDefault(x => x.Id == afterImage.CroppedImageId);
                                                beforeAfterImagesLocalMarketingModel.RelactiveLocationAfterImageUrlThumb = file != null ? (file.RelativeLocation + "\\" + file.Name).ToUrl() : "";
                                            }

                                            beforeAfterImagesLocalMarketingModel.RelactiveLocationAfterImage = afterImage != null && afterImage.File != null && afterImage.File.RelativeLocation != null && afterImage.File.Name != null ? (afterImage.File.RelativeLocation + "\\" + afterImage.File.Name).ToUrl() : "";
                                            beforeAfterImagesLocalMarketingModel.RelactiveLocationBeforeImage = beforeImage != null && beforeImage.File != null && beforeImage.File.RelativeLocation != null && beforeImage.File.Name != null ? (beforeImage.File.RelativeLocation + "\\" + beforeImage.File.Name).ToUrl() : "";
                                            beforeAfterImagesLocalMarketingModel.EmptyImage = "/Content/images/no_image_thumb.gif";
                                            beforeAfterImagesLocalMarketingModel.IsImagePairReviewed = beforeImage != null && beforeImage.IsImagePairReviewed != false ? true : afterImage != null && afterImage.IsImagePairReviewed != false ? true : false;
                                            beforeAfterImagesLocalMarketingModel.BeforeImageFileId = beforeImage != null && beforeImage.FileId != null ? beforeImage.FileId : null;
                                            beforeAfterImagesLocalMarketingModel.AfterImageFileId = afterImage != null && afterImage.FileId != null ? afterImage.FileId : null;
                                            schedulerListLocalMarketingModel.BeforeAfterImagesLocalMarketingModel.Add(beforeAfterImagesLocalMarketingModel);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (filter.IsFilter)
                {
                    foreach (var franchiseeItem in localMarketingReviewModel.FranchiseeListLocalMarketingModel)
                    {
                        franchiseeItem.PersonListLocalMarketingModel.RemoveAll(x => x.SchedulerCount == 0);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return localMarketingReviewModel;
        }

        private List<BeforeAfterImages> GetBeforeAfterImagesList(LocalMarketingReviewFilter filter)
        {
            var serviceTypeId = default(long);
            var isImageReviewed = default(bool);
            if (filter.IsImagePairReviewed != "0")
            {
                if (filter.IsImagePairReviewed == "1")
                {
                    isImageReviewed = true;
                }
                else if (filter.IsImagePairReviewed == "2")
                {
                    isImageReviewed = false;
                }
                else
                {
                    isImageReviewed = false;
                }

            }
            if (filter.ServiceTypeId != "")
            {
                serviceTypeId = long.Parse(filter.ServiceTypeId);
            }
            var model = _beforeAfterImagesRepository.Table.Where(x => ((x.TypeId == (long)LookupTypes.BeforeWork) || (x.TypeId == (long)LookupTypes.DuringWork) ||
                                (x.TypeId == (long)LookupTypes.AfterWork) || (x.TypeId == (long)LookupTypes.ExteriorBuilding))
                                //&& (x.DataRecorderMetaData.DateCreated >= filter.StartDate) 
                                //&& (x.DataRecorderMetaData.DateCreated <= filter.EndDate)
                                && (filter.SurfaceTypeId == "" || x.SurfaceType.Contains(filter.SurfaceTypeId))
                                && (filter.SurfaceMaterial == "" || x.SurfaceMaterial.Contains(filter.SurfaceMaterial))
                                && (filter.SurfaceColor == "" || x.SurfaceColor.Contains(filter.SurfaceColor))
                                && (filter.FinishMaterial == "" || x.FinishMaterial.Contains(filter.FinishMaterial))
                                && (filter.BuildingLocation == "" || x.BuildingLocation.Contains(filter.BuildingLocation))
                                && (filter.ServiceTypeId == "" || x.ServiceTypeId == serviceTypeId)
                                && (filter.IsImagePairReviewed == "0" || x.IsImagePairReviewed == isImageReviewed)
                                && (filter.MarketingClassId == 0 || filter.MarketingClassId == null || x.MarkertingClassId == filter.MarketingClassId)
                                ).OrderByDescending(z => z.Id).ToList();
            return model;
        }

        private List<JobScheduler> GetJobSchedulerList(LocalMarketingReviewFilter filter, List<BeforeAfterImages> beforeAfter)
        {
            if (filter.StartDate != null)
            {
                filter.StartDate = _clock.ToUtc(filter.StartDate.Value);
            }
            if (filter.EndDate != null)
            {
                filter.EndDate = _clock.ToUtc(filter.EndDate.Value);
            }
            var toDate = filter.EndDate.HasValue ? filter.EndDate.Value.AddTicks(-1).AddDays(1) : (DateTime?)null;
            if (filter.IsFilter)
            {
                var schedulerIds = beforeAfter.Select(x => x.SchedulerId).ToList();
                var jobSchedulerList = _jobschedulerRepository.Table.Where(x => schedulerIds.Contains(x.Id) && x.StartDate >= filter.StartDate && x.EndDate <= toDate).ToList();
                return jobSchedulerList;
            }
            else
            {
                var jobSchedulerList = _jobschedulerRepository.Table.Where(x => x.Id > 0 && x.FranchiseeId > 0 && x.IsActive == true && x.StartDate >= filter.StartDate && x.EndDate <= toDate).ToList();
                return jobSchedulerList;
            }

        }

        private List<Franchisee> GetFranchiseeList(LocalMarketingReviewFilter filter, List<OrganizationRoleUser> organizationRollUserList, List<JobScheduler> jobScheduler)
        {
            var filterUserId = default(long);
            if (filter.UserId != null)
            {
                filterUserId = long.Parse(filter.UserId);
                filter.FranchiseeId = organizationRollUserList.FirstOrDefault(x => x.UserId == filterUserId).OrganizationId;
            }
            if (filter.FranchiseeId == null)
            {
                var franchiseeList = _franchiseeRepository.Table.Where(x => x.Id != 2 && x.Organization.IsActive == true).ToList();
                return franchiseeList;
            }
            else if (filter.FranchiseeId != null)
            {
                var franchiseeList = _franchiseeRepository.Table.Where(x => x.Id != 2 && x.Id == filter.FranchiseeId && x.Organization.IsActive == true).ToList();
                return franchiseeList;
            }
            else
            {
                var franchiseeList = _franchiseeRepository.Table.Where(x => x.Id != 2 && x.Organization.IsActive == true).ToList();
                return franchiseeList;
            }
        }

        private List<OrganizationRoleUser> GetUsersList(LocalMarketingReviewFilter filter, List<OrganizationRoleUser> organizationRollUserList, FranchiseeListLocalMarketingModel franchisee)
        {
            var filterUserId = default(long);
            if (filter.UserId != null)
            {
                filterUserId = long.Parse(filter.UserId);
                filter.FranchiseeId = organizationRollUserList.FirstOrDefault(x => x.UserId == filterUserId).OrganizationId;
            }
            var model = filter.FranchiseeId != 0 && filter.FranchiseeId != null && filterUserId != 0 ? organizationRollUserList.Where(x => x.OrganizationId == filter.FranchiseeId && x.UserId == filterUserId).ToList() : organizationRollUserList.Where(x => x.OrganizationId == franchisee.FranchiseeId).ToList();
            return model;
        }

        public List<SalesRepTechnicianModel> GetSalesRepTechnician(long? franchiseeId, long LoggedUserId, long RoleId, long LoggedInFranchiseeId)
        {
            if (franchiseeId == null)
            {

                if (RoleId == (long?)RoleType.SuperAdmin || RoleId == (long?)RoleType.FrontOfficeExecutive)
                {
                    franchiseeId = null;
                }
                else
                {
                    franchiseeId = LoggedInFranchiseeId;
                }
            }
            var SalesRepList = franchiseeId != null ? _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.OrganizationId == franchiseeId && x.RoleId == (long)RoleType.SalesRep).OrderBy(y => y.Person.FirstName).ToList() : _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.RoleId == (long)RoleType.SalesRep).OrderBy(y => y.Person.FirstName).ToList();
            var TechnicianList = franchiseeId != null ? _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.OrganizationId == franchiseeId && x.RoleId == (long)RoleType.Technician).OrderBy(y => y.Person.FirstName).ToList() : _organizationRoleUserRepository.Table.Where(x => x.IsActive == true && x.RoleId == (long)RoleType.Technician).OrderBy(y => y.Person.FirstName).ToList();
            var organizationRollUserList = SalesRepList.Concat(TechnicianList).ToList();
            var orgRoleUserIds = organizationRollUserList.Select(x => x.UserId).ToList();
            var activeUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && !x.IsLocked).Select(x => x.Id).ToList();
            organizationRollUserList = organizationRollUserList.Where(x => activeUers.Contains(x.UserId)).ToList();
            List<SalesRepTechnicianModel> model = new List<SalesRepTechnicianModel>();
            foreach (var item in organizationRollUserList)
            {
                SalesRepTechnicianModel salesRepTechnician = new SalesRepTechnicianModel();
                salesRepTechnician.Display = item.Person.FirstName + " " + item.Person.LastName + "(" + item.Role.Name + ")";
                salesRepTechnician.Id = item.Person.Id;
                salesRepTechnician.Role = item.Role.Name;
                salesRepTechnician.Value = item.Person.Id.ToString();
                model.Add(salesRepTechnician);
            }
            return model;
        }

        public bool MarkImageAsReviwed(BeforeAfterImagesLocalMarketingModel model)
        {
            if (model.BeforeImageId != null)
            {
                var beforeAfterImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.BeforeImageId);
                beforeAfterImageModel.IsImagePairReviewed = model.IsImagePairReviewed;

                _beforeAfterImagesRepository.Save(beforeAfterImageModel);
                _unitOfWork.SaveChanges();
                return true;
            }
            else if (model.AfterImageId != null)
            {
                var beforeAfterImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.AfterImageId);
                beforeAfterImageModel.IsImagePairReviewed = model.IsImagePairReviewed;

                _beforeAfterImagesRepository.Save(beforeAfterImageModel);
                _unitOfWork.SaveChanges();
                return true;
            }
            else if (model.ExteriorImageFileId != null)
            {
                var exteriorImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.ExteriorImageId);
                exteriorImageModel.IsImagePairReviewed = model.IsImagePairReviewed;

                _beforeAfterImagesRepository.Save(exteriorImageModel);
                _unitOfWork.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }


        }

        public bool MarkImageAsBestPair(BeforeAfterImagesLocalMarketingModel model, long LoggedUserId, long RoleId, long LoggedInFranchiseeId)
        {
            if (model.BeforeImageId != null || model.AfterImageId != null)
            {
                var person = _personRepository.Table.FirstOrDefault(x => x.Id == LoggedUserId);
                var beforeImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.BeforeImageId);
                var AfterImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.AfterImageId);
                if (beforeImageModel != null)
                {
                    beforeImageModel.IsBestImage = model.IsBestPicture;
                    beforeImageModel.BestPairMarkedBy = LoggedUserId;
                    beforeImageModel.Person1 = person;

                    _beforeAfterImagesRepository.Save(beforeImageModel);
                    _unitOfWork.SaveChanges();
                }
                if (AfterImageModel != null)
                {
                    AfterImageModel.IsBestImage = model.IsBestPicture;
                    AfterImageModel.BestPairMarkedBy = LoggedUserId;
                    AfterImageModel.Person1 = person;

                    _beforeAfterImagesRepository.Save(AfterImageModel);
                    _unitOfWork.SaveChanges();
                }
                //var response = SendBestPairToMarketingSite(model);
                //if (response)
                //{
                //    _logService.Info(string.Format("Best Pair Marked Image Pair Successfully Send To Marketing Site: EstimateId - {0}", model.EstimateId));
                //}
                //else
                //{
                //    _logService.Info(string.Format("Error In Sending Best Pair Marked Image Pair To Marketing Site: EstimateId - {0}", model.EstimateId));
                //}
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MarkImageAsAddToLocalGallery(BeforeAfterImagesLocalMarketingModel model)
        {
            if (model.BeforeImageId != null || model.AfterImageId != null)
            {
                var beforeImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.BeforeImageId);
                var AfterImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.AfterImageId);
                if (beforeImageModel != null)
                {
                    beforeImageModel.IsAddToLocalGallery = model.IsAddToLocalGallery;

                    _beforeAfterImagesRepository.Save(beforeImageModel);
                    _unitOfWork.SaveChanges();
                }
                if (AfterImageModel != null)
                {
                    AfterImageModel.IsAddToLocalGallery = model.IsAddToLocalGallery;

                    _beforeAfterImagesRepository.Save(AfterImageModel);
                    _unitOfWork.SaveChanges();
                }
                var response = SendBestPairToMarketingSite(model);
                if (response)
                {
                    _logService.Info(string.Format("Best Pair Marked Image Pair Successfully Send To Marketing Site: EstimateId - {0}", model.EstimateId));
                }
                else
                {
                    _logService.Info(string.Format("Error In Sending Best Pair Marked Image Pair To Marketing Site: EstimateId - {0}", model.EstimateId));
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SendBestPairToMarketingSite(BeforeAfterImagesLocalMarketingModel model)
        {
            if (model.BeforeImageId != null && model.AfterImageId != null)
            {
                var beforeImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.BeforeImageId);
                var afterImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.AfterImageId);
                var isEstimate = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.Id == model.BeforeImageId).EstimateId;

                var exteriorImageModel = isEstimate != null ? _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.EstimateId == beforeImageModel.EstimateId && x.TypeId == (long)LookupTypes.ExteriorBuilding) : _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.JobId == beforeImageModel.JobId && x.TypeId == (long)LookupTypes.ExteriorBuilding);

                var franchiseeName = _organizationRepository.Table.FirstOrDefault(x => x.Id == beforeImageModel.FranchiseeId).Name;
                var serviceType = _serviceTypeRepository.Table.FirstOrDefault(x => x.Id == beforeImageModel.ServiceTypeId).Name;

                BestPairImageSendToMarketingSiteModel marketingSiteModel = new BestPairImageSendToMarketingSiteModel();

                var bestPairImage = new BestPairImageForMarketingSiteModel()
                {
                    Id = beforeImageModel != null && beforeImageModel.EstimateId != null ? beforeImageModel.EstimateId : beforeImageModel.JobId,
                    FranchiseeId = beforeImageModel.FranchiseeId,
                    FranchiseeName = franchiseeName,
                    BeforeImageId = beforeImageModel.Id,
                    BeforeImageS3BucketURL = beforeImageModel != null && beforeImageModel.S3BucketURL != null ? RemoveWhitespace(beforeImageModel.S3BucketURL) : "",
                    BeforeImageS3BucketThumbURL = beforeImageModel != null && beforeImageModel.S3BucketThumbURL != null ? RemoveWhitespace(beforeImageModel.S3BucketThumbURL) : "",
                    CroppedBeforeImageS3BucketURL = "",
                    BeforeImageCSS = beforeImageModel.File != null && beforeImageModel.File.css!= null ? beforeImageModel.File.css : "rotate(0)",
                    AfterImageId = afterImageModel.Id,
                    AfterImageS3BucketURL = afterImageModel != null && afterImageModel.S3BucketURL != null ? RemoveWhitespace(afterImageModel.S3BucketURL) : "",
                    AfterImageS3BucketThumbURL = afterImageModel != null && afterImageModel.S3BucketThumbURL != null ? RemoveWhitespace(afterImageModel.S3BucketThumbURL) : "",
                    CroppedAfterImageS3BucketURL = "",
                    AfterImageCSS = afterImageModel.File!= null && afterImageModel.File.css != null ? afterImageModel.File.css : "rotate(0)",
                    SurfaceMaterial = beforeImageModel.SurfaceMaterial,
                    ServiceType = serviceType,
                    SurfaceType = beforeImageModel.SurfaceType,
                    SurfaceColor = beforeImageModel.SurfaceColor,
                    SurfaceFinish = beforeImageModel.FinishMaterial,
                    BuildingLocation = beforeImageModel.BuildingLocation,
                    MarketingClass = beforeImageModel != null && beforeImageModel.MarketingClass != null && beforeImageModel.MarketingClass.Name != null ? beforeImageModel.MarketingClass.Name : "",
                    IsBestPair = model.IsAddToLocalGallery,
                    IsSpecialPair = beforeImageModel.IsAddToLocalGallery,
                    FromSource = "MIMS"
                };
                marketingSiteModel.Collection.Add(bestPairImage);
                if (exteriorImageModel != null)
                {
                    foreach (var image in marketingSiteModel.Collection)
                    {
                        ExteriorImagesModelForMarketingSite exterior = new ExteriorImagesModelForMarketingSite();

                        exterior.Id = exteriorImageModel != null && exteriorImageModel.EstimateId != null ? exteriorImageModel.EstimateId : exteriorImageModel.JobId;
                        exterior.ExteriorImageId = exteriorImageModel.Id;
                        exterior.ExteriorImageS3BucketURL = exteriorImageModel != null && exteriorImageModel.S3BucketURL != null ? RemoveWhitespace(exteriorImageModel.S3BucketURL) : "";
                        exterior.ExteriorThumbImageS3BucketURL = exteriorImageModel != null && exteriorImageModel.S3BucketThumbURL != null ? RemoveWhitespace(exteriorImageModel.S3BucketThumbURL) : "";
                        exterior.CroppedExteriorImageS3BucketURL = "";
                        exterior.ExteriorImageCSS = exteriorImageModel.File != null && exteriorImageModel.File.css != null ? exteriorImageModel.File.css : "rotate(0)";
                        image.ExteriorImageList.Add(exterior);
                    }
                }

                string json = JsonConvert.SerializeObject(marketingSiteModel);

                // URL to post to
                //string apiUrl = "https://new.marblelife.com/wp-json/api/v1/ml_ba_images";
                string apiUrl = _settings.MarketingSiteWebSocketURL;

                using (HttpClient client = new HttpClient())
                {
                    // Custom header key-value pair
                    string customHeaderKey = _settings.KeyForBefoeAfterBestPairSendToMarketingSite;
                    //string customHeaderValue = _settings.KeyValueForBefoeAfterBestPairSendToMarketingSite;
                    string customHeaderValue = "1a83d4430ba90415560a3b627c50b920";

                    // Add custom header
                    client.DefaultRequestHeaders.Add(customHeaderKey, customHeaderValue);

                    // Convert JSON string to HttpContent
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Send POST request synchronously
                    HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;

                    // Check if request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private List<BeforeAfterImages> GetBeforeAfterImagesListForReviewMarketing(LocalMarketingReviewFilter filter)
        {
            var serviceTypeId = default(long);
            
            if (filter.ServiceTypeId != "")
            {
                serviceTypeId = long.Parse(filter.ServiceTypeId);
            }
            var model = _beforeAfterImagesRepository.Table.Where(x => ((x.TypeId == (long)LookupTypes.BeforeWork) || (x.TypeId == (long)LookupTypes.DuringWork) ||
                                (x.TypeId == (long)LookupTypes.AfterWork)) && x.IsBestImage
                                //&& (x.DataRecorderMetaData.DateCreated >= filter.StartDate) 
                                //&& (x.DataRecorderMetaData.DateCreated <= filter.EndDate)
                                && (filter.SurfaceType == "" || x.SurfaceType.Contains(filter.SurfaceType))
                                && (filter.SurfaceMaterial == "" || x.SurfaceMaterial.Contains(filter.SurfaceMaterial))
                                && (filter.SurfaceColor == "" || x.SurfaceColor.Contains(filter.SurfaceColor))
                                && (filter.FinishMaterial == "" || x.FinishMaterial.Contains(filter.FinishMaterial))
                                && (filter.BuildingLocation == "" || x.BuildingLocation.Contains(filter.BuildingLocation))
                                && (filter.ServiceTypeId == "" || x.ServiceTypeId == serviceTypeId)
                                && (filter.MarketingClassId == 0 || filter.MarketingClassId == null || x.MarkertingClassId == filter.MarketingClassId)
                                && (filter.PendingToAddInLocalSiteGallery == true || x.IsAddToLocalGallery == filter.PendingToAddInLocalSiteGallery)
                                ).OrderByDescending(z => z.Id).ToList();
            return model;
        }

        private List<OrganizationRoleUser> GetUsersListForReviewMarketing(LocalMarketingReviewFilter filter, List<OrganizationRoleUser> organizationRollUserList, ReviewMarketingFranchiseeViewModel franchisee)
        {
            var filterUserId = default(long);
            if (filter.UserId != null)
            {
                filterUserId = long.Parse(filter.UserId);
                filter.FranchiseeId = organizationRollUserList.FirstOrDefault(x => x.UserId == filterUserId).OrganizationId;
            }
            var model = filter.FranchiseeId != 0 && filter.FranchiseeId != null && filterUserId != 0 ? organizationRollUserList.Where(x => x.OrganizationId == filter.FranchiseeId && x.UserId == filterUserId).ToList() : organizationRollUserList.Where(x => x.OrganizationId == franchisee.FranchiseeId).ToList();
            return model;
        }

        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public bool BestPairMarkedForJobEstimateImagePair(JobEstimateImagePairMarkedModel model, long LoggedUserId, long RoleId, long LoggedInFranchiseeId)
        {
            if (model.BeforeImageFileId != null || model.AfterImageFileId != null)
            {
                var person = _personRepository.Table.FirstOrDefault(x => x.Id == LoggedUserId);
                var beforeImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.FileId == model.BeforeImageFileId && x.CategoryId == model.BeforeImageCategoryId && x.ServiceId == model.BeforeImageServiceId);
                var AfterImageModel = _beforeAfterImagesRepository.Table.FirstOrDefault(x => x.FileId == model.AfterImageFileId && x.CategoryId == model.AfterImageCategoryId && x.ServiceId == model.AfterImageServiceId);
                if (beforeImageModel != null)
                {
                    beforeImageModel.IsBestImage = model.IsBestPairMarked;
                    beforeImageModel.IsAddToLocalGallery = model.IsAddToLocalSiteGallery;
                    beforeImageModel.BestPairMarkedBy = LoggedUserId;
                    beforeImageModel.Person1 = person;

                    _beforeAfterImagesRepository.Save(beforeImageModel);
                    _unitOfWork.SaveChanges();
                }
                if (AfterImageModel != null)
                {
                    AfterImageModel.IsBestImage = model.IsBestPairMarked;
                    AfterImageModel.IsAddToLocalGallery = model.IsAddToLocalSiteGallery;
                    AfterImageModel.BestPairMarkedBy = LoggedUserId;
                    AfterImageModel.Person1 = person;

                    _beforeAfterImagesRepository.Save(AfterImageModel);
                    _unitOfWork.SaveChanges();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

public class DatetimeModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class BeforeAfterModel
{
    public long? ServiceId { get; set; }
    public long? Id { get; set; }
}
public class OldSchedulerModel
{
    public long? Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public long? AssigneeId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
}

public class PersonViewModel
{
    public string PersonName { get; set; }
    public long UserId { get; set; }
    public long? Id { get; set; }
    public long? RoleId { get; set; }
    public Role Role { get; set; }
}

public class PersonUploadViewModel
{
    public long? UserId { get; set; }
    public bool? IsHavingImage { get; set; }
}

