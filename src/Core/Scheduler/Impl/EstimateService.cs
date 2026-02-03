using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Geo;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scheduler.Impl;
using Core.Users.Enum;
using Core.Users.Domain;
using System.Runtime.Remoting.Activation;
using Core.Reports.Domain;
using Core.Notification;
using Core.Notification.Enum;
using Core.Sales.Domain;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class EstimateService : IEstimateService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobFactory _jobFactory;
        private readonly IJobDetailsFactory _jobDetailsFactory;
        private readonly ISortingHelper _sortingHelper;
        private readonly IAddressFactory _aadressFactory;
        private readonly IClock _clock;
        public readonly IRepository<JobCustomer> _jobCustomerRepository;
        public readonly IRepository<JobEstimate> _jobEstimateRepository;
        public readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<JobScheduler> _jobschedulerRepository;
        public readonly IRepository<Meeting> _meetingRepository;
        public readonly IRepository<Job> _jobRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<JobDetails> _jobDetailsRepository;
        public readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly ISendNewJobNotificationtoTechService _sendNewJobNotificationToTechService;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<Phone> _phoneRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<EstimateInvoice> _estimateInvoiceRepository;
        private readonly IRepository<CustomerSignature> _customerSignatureRepository;
        private readonly IRepository<EstimateInvoiceAssignee> _estimateInvoiceAssignee;
        const string jobType = "Estimate";
        List<long> oldTechList = new List<long>();
        List<long> oldInactiveTechList = new List<long>();
        List<long> newdTechList = new List<long>();
        private readonly IUserNotificationModelFactory _userNotificationModelFactory;

        private readonly IEstimateInvoiceServices _estimateInvoiceServices;
        private readonly IRepository<ShiftCharges> _shiftChargesRepository;
        private readonly IRepository<MaintenanceCharges> _maintenanceChargesRepository;
        private readonly IEstimateInvoiceFactory _estimateInvoiceFactory;
        public EstimateService(IUnitOfWork unitOfWork, IJobFactory jobFactory, ISortingHelper sortingHelper, IAddressFactory addressFactory, IClock clock,
            IOrganizationRoleUserInfoService organizationRoleUserInfoService, ISendNewJobNotificationtoTechService sendNewJobNotificationToTechService,
            IJobDetailsFactory jobDetailsFactory, IUserNotificationModelFactory userNotificationModelFactory, IEstimateInvoiceServices estimateInvoiceServices, IEstimateInvoiceFactory estimateInvoiceFactory)
        {
            _unitOfWork = unitOfWork;
            _jobFactory = jobFactory;
            _sortingHelper = sortingHelper;
            _aadressFactory = addressFactory;
            _clock = clock;
            _jobCustomerRepository = unitOfWork.Repository<JobCustomer>();
            _jobEstimateRepository = unitOfWork.Repository<JobEstimate>();
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _meetingRepository = unitOfWork.Repository<Meeting>();
            _jobRepository = unitOfWork.Repository<Job>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _jobschedulerRepository = unitOfWork.Repository<JobScheduler>();
            _sendNewJobNotificationToTechService = sendNewJobNotificationToTechService;
            _personRepository = unitOfWork.Repository<Person>();
            _phoneRepository = unitOfWork.Repository<Phone>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _jobDetailsRepository = unitOfWork.Repository<JobDetails>();
            _jobDetailsFactory = jobDetailsFactory;
            _userNotificationModelFactory = userNotificationModelFactory;
            _estimateInvoiceRepository = unitOfWork.Repository<EstimateInvoice>();
            _customerSignatureRepository = unitOfWork.Repository<CustomerSignature>();
            _estimateInvoiceAssignee = unitOfWork.Repository<EstimateInvoiceAssignee>();
            _estimateInvoiceServices = estimateInvoiceServices;
            _shiftChargesRepository = unitOfWork.Repository<ShiftCharges>();
            _maintenanceChargesRepository = unitOfWork.Repository<MaintenanceCharges>();
            _estimateInvoiceFactory = estimateInvoiceFactory;
        }

        public JobEstimateEditModel Get(long estimateId)
        {
            if (estimateId == 0)
                return new JobEstimateEditModel { };

            else
            {
                List<long?> estimateIds = new List<long?>();
                var estimate = _jobEstimateRepository.Get(estimateId);
                var parentIds = new List<long>();
                var scheduler = _jobSchedulerRepository.Table.Where(x => x.EstimateId == estimate.Id && x.IsActive).FirstOrDefault();

                if (scheduler == null)
                    return new JobEstimateEditModel { };

                var schedulers = default(List<JobScheduler>);
                var isEstimateInvoicePresent = _estimateInvoiceRepository.Table.Any(x => x.EstimateId == estimateId);
                if (scheduler != null)
                {
                    estimateIds = (_jobEstimateRepository.Table.Where(x => x.ParentEstimateId == scheduler.EstimateId).Select(x => (long?)x.Id)).ToList(); // for Getting Child Estimate
                    parentIds = getParentEstimateIds(scheduler);
                }
                foreach (var parentId in parentIds)
                    estimateIds.Add(parentId);

                if (parentIds.Count() == 0)
                    estimateIds.Add(scheduler.EstimateId.GetValueOrDefault());

                estimateIds = estimateIds.OrderBy(x => x).ToList();
                schedulers = _jobSchedulerRepository.Table.Where(x => estimateIds.Contains(x.EstimateId) && x.IsActive).ToList();
                //var getChildEstimateIds=
                if (scheduler == null)
                    return new JobEstimateEditModel { };
                var model = _jobFactory.CreateEstimateModel(estimate, scheduler);
                model.IsInvoicePresent = isEstimateInvoicePresent;
                model.IsInvoiceRequired = scheduler.IsInvoiceRequired;
                model.InvoiceReason = scheduler.InvoiceReason;
                model.Worth = scheduler.Estimate.Amount;
                var jobEstimate = _jobDetailsRepository.Table.FirstOrDefault(x => x.EstimateId == model.Id);
                if (jobEstimate != null)
                {
                    model.Description = jobEstimate.Description;
                }

                model.EstimateSchedulerList = schedulers.Select(x => CreateJobShedulerModel(x));
                model.LessDeposit = schedulers.Count() > 0 ? schedulers.FirstOrDefault().Franchisee.LessDeposit : 50;
                model.MarketingClassId = scheduler.Estimate.MarketingClass.Id.ToString();
                var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.SchedulerId == scheduler.Id);
                model.EstimateInvoiceId = estimateInvoice != null ? estimateInvoice.Id : 0;
                if (estimateInvoice != null)
                {
                    var customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id && x.TypeId == (long)SignatureType.PRECOMPLETION).OrderByDescending(x => x.Id).GroupBy(x => x.Signature).ToList();
                    model.JobSignature = new List<JobEstimateSignatureEditModel>();
                    model.AllInvoiceNumbersSigned = new List<long?>();
                    foreach (var signature in customerSignature)
                    {
                        JobEstimateSignatureEditModel jobSignature = new JobEstimateSignatureEditModel();
                        jobSignature.IsSigned = customerSignature != null && signature.Key != null ? true : false;
                        jobSignature.Signature = customerSignature != null && signature.Key != null ? signature.Key : "";
                        jobSignature.InvoiceNumber = customerSignature != null && signature.Key != null ? signature.Select(x => x.InvoiceNumber).ToList() : new List<long?>();
                        model.AllInvoiceNumbersSigned.AddRange(jobSignature.InvoiceNumber);
                        model.JobSignature.Add(jobSignature);
                    }
                    if (model.AllInvoiceNumbersSigned.Count > 0)
                    {
                        model.IsSigned = true;
                    }

                    var customerSignaturePost = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoice.Id && x.TypeId == (long)SignatureType.POSTCOMPLETION).OrderByDescending(x => x.Id).GroupBy(x => x.Signature).ToList();
                    model.JobSignaturePost = new List<JobEstimateSignatureEditModel>();
                    model.AllInvoiceNumbersSignedPost = new List<long?>();
                    foreach (var signature in customerSignaturePost)
                    {
                        JobEstimateSignatureEditModel jobSignature = new JobEstimateSignatureEditModel();
                        jobSignature.IsSigned = customerSignature != null && signature.Key != null ? true : false;
                        jobSignature.Signature = customerSignature != null && signature.Key != null ? signature.Key : "";
                        jobSignature.InvoiceNumber = customerSignature != null && signature.Key != null ? signature.Select(x => x.InvoiceNumber).ToList() : new List<long?>();
                        model.AllInvoiceNumbersSignedPost.AddRange(jobSignature.InvoiceNumber);
                        model.JobSignaturePost.Add(jobSignature);
                    }
                    if (model.AllInvoiceNumbersSignedPost.Count > 0)
                    {
                        model.IsSignedPost = true;
                    }

                }
                var shiftCharges = _shiftChargesRepository.Table.FirstOrDefault(x => x.FranchiseeId == model.FranchiseeId && x.IsActive);
                model.IsDataToBeUpdateForAllJobs = true;
                model.IsInvoiceRequired = scheduler.IsInvoiceRequired;
                model.ShiftChargesViewModel = CreateShiftModel(shiftCharges);
                var maintenanceCharges = _maintenanceChargesRepository.Table.Where(x => x.FranchiseeId == scheduler.FranchiseeId && x.IsActive).ToList();
                model.MaintenanceChargesList = maintenanceCharges.Select(x => _estimateInvoiceFactory.CreateMaintanceChargesViewModel(x)).ToList();
                return model;
            }
        }


        private List<long> getParentEstimateIds(JobScheduler scheduler)
        {
            var jobEstimate = _jobEstimateRepository.Table.ToList();
            List<long> parentIds = new List<long>();
            long? estimateId = default(long?);
            long? parentEstimateId = default(long?);
            estimateId = scheduler.EstimateId;

            var jobWithSameEstimateIds = jobEstimate.Where(x => x.ParentEstimateId == estimateId).Select(x => x.Id).ToList();

            if (jobWithSameEstimateIds.Count() > 0)
            {
                foreach (var jobWithSameEstimateId in jobWithSameEstimateIds)
                {
                    var getParentEstimate = getEstimateParent(jobWithSameEstimateId, jobEstimate);
                    parentIds.AddRange(getParentEstimate);
                }
                if (jobWithSameEstimateIds.Count() > 0)
                {
                    parentIds.AddRange(jobWithSameEstimateIds);
                }
            }

            if (parentEstimateId == null)
            {
                parentIds.Add(estimateId.GetValueOrDefault());
            }
            while (parentEstimateId != 0)
            {
                var jobParentEstimate = jobEstimate.Where(x => x.Id == estimateId).FirstOrDefault();
                if (jobParentEstimate.ParentEstimateId != null)
                {

                    var jobSameEstimates = jobEstimate.Where(x => x.ParentEstimateId == jobParentEstimate.ParentEstimateId).ToList();

                    foreach (var jobSameEstimate in jobSameEstimates)
                    {
                        parentIds.Add(jobSameEstimate.Id);
                    }

                    parentEstimateId = jobParentEstimate.ParentEstimateId.GetValueOrDefault();
                    estimateId = jobParentEstimate.ParentEstimateId;
                    parentIds.Add(estimateId.GetValueOrDefault());
                }
                else
                {
                    break;
                }

            }
            parentIds = parentIds.Distinct().ToList();
            return parentIds;
        }

        private List<long> getEstimateParent(long? estimateId, List<JobEstimate> jobEstimate)
        {
            return jobEstimate.Where(x => x.ParentEstimateId == estimateId).Select(x => x.Id).ToList();
        }
        public JobSchedulerEditModel CreateJobShedulerModel(JobScheduler domain)
        {
            var model = new JobSchedulerEditModel
            {
                Id = domain.Id,
                JobId = domain.JobId != null ? domain.JobId.Value : 0,
                EstimateId = domain.EstimateId != null ? domain.EstimateId.Value : 0,
                Title = domain.Title,
                DataRecorderMetaData = domain.DataRecorderMetaData,
                AssigneeId = domain.AssigneeId != null ? domain.AssigneeId.Value : 0,
                AssigneeName = domain.OrganizationRoleUser != null ? domain.OrganizationRoleUser.Person.Name.ToString() : null,
                IsActive = domain.IsActive,
                StartDate = domain.StartDate,
                EndDate = domain.EndDate,
                FranchiseeId = domain.FranchiseeId,
                IsImported = domain.IsImported,
                SalesRepId = domain.SalesRepId,
                ActualEndDateString = domain.EndDateTimeString,
                ActualStartDateString = domain.StartDateTimeString,
                SchedulerStatus = domain.SchedulerStatus
            };
            return model;
        }
        bool IsUpdated(JobEstimateEditModel model, string oldAddress1, string oldAddress2, DateTime oldStartDatetime, DateTime oldEndDatetime)
        {
            if (model.JobCustomer.Address.AddressLine1 == oldAddress1 && model.JobCustomer.Address.AddressLine2 == oldAddress2 && model.StartDate == oldStartDatetime && model.EndDate == oldEndDatetime)
            {
                return false;
            }
            return true;

        }
        bool IsUpdated(JobScheduler model, DateTime oldStartDatetime, DateTime oldEndDatetime)
        {
            var estimateModel = _jobEstimateRepository.Get(model.EstimateId.GetValueOrDefault());
            if (model.StartDate == oldStartDatetime && model.EndDate == oldEndDatetime)
            {
                return false;
            }
            return true;

        }

        public void Save(JobEstimateEditModel model)
        {
            var currentDate = DateTime.Now;
            bool isUpdate = false;
            string oldAddress1 = "";
            string oldAddress2 = "";
            var scheduler = new JobScheduler();
            DateTime oldStartDatetime = default(DateTime);
            DateTime oldEndDatetime = default(DateTime);
            var isNewScheduler = true;
            bool isSales = (_organizationRoleUserRepository.Fetch(x => x.Id == model.SalesRepId && x.Role.Id == (long)RoleType.SalesRep)).Any();
            var oldDaata = _jobschedulerRepository.IncludeMultiple(x => x.Job).Where(x => x.Id == model.SchedulerId).FirstOrDefault();
            if (oldDaata != null)
            {
                oldAddress1 = oldDaata != null ? oldDaata.Estimate.JobCustomer.Address.AddressLine1 : "";
                oldAddress2 = oldDaata != null ? oldDaata.Estimate.JobCustomer.Address.AddressLine2 : "";
                oldStartDatetime = oldDaata != null ? oldDaata.StartDate : default(DateTime);
                oldEndDatetime = oldDaata != null ? oldDaata.EndDate : default(DateTime);
            }
            if (model.Id != 0)
            {
                oldInactiveTechList = _jobschedulerRepository.Table.Where(x => x.EstimateId == model.Id && !x.IsActive).Select(x => x.AssigneeId.Value).Distinct().ToList();
                oldTechList = _jobschedulerRepository.Table.Where(x => x.EstimateId == model.Id && x.IsActive && x.OrganizationRoleUser.RoleId == (long)RoleType.SalesRep).Select(x => x.AssigneeId.Value).ToList();
            }
            var customer = _jobFactory.CreateDomain(model.JobCustomer);
            _jobCustomerRepository.Save(customer);
            model.JobCustomer.CustomerId = customer.Id;
            model.CustomerId = customer.Id;

            var estimate = _jobFactory.CreateDomain(model);
            estimate.ParentEstimateId = oldDaata != null && oldDaata.Estimate != null && oldDaata.Estimate != null ? oldDaata.Estimate.ParentEstimateId : null;
            var isNewJob = estimate.Id == 0;
            _jobEstimateRepository.Save(estimate);

            if (string.IsNullOrEmpty(model.Title))
            {

                const string estimateTitle = "Estimate";
                var estimateCount = _jobSchedulerRepository.Table.Count(x => x.FranchiseeId == model.FranchiseeId && x.IsActive && x.EstimateId != null);
                model.Title = estimateTitle + "" + (estimateCount + 1);
            }

            model.JobScheduler.EstimateId = estimate.Id;
            model.JobScheduler.FranchiseeId = model.FranchiseeId;
            model.JobScheduler.SalesRepId = model.SalesRepId;
            model.JobScheduler.AssigneeId = model.SalesRepId;
            model.JobScheduler.StartDate = model.StartDate;
            model.JobScheduler.EndDate = model.EndDate;
            model.JobScheduler.Title = model.Title;
            model.JobScheduler.Id = model.SchedulerId;
            model.SchedulerIds = model.SchedulerId;
            model.JobScheduler.DataRecorderMetaData = model.DataRecorderMetaData;

            if (model.Id > 0)
            {
                isNewScheduler = false;
                model.JobScheduler.StartDate = model.StartDate;
                model.JobScheduler.EndDate = model.EndDate;
                var offset = _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes;
                model.JobScheduler.Offset = (int)offset;
                scheduler = _jobFactory.CreateDomain(model.JobScheduler);
                scheduler.SchedulerStatus = oldDaata.SchedulerStatus;
                var orgRoleUser = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == scheduler.FranchiseeId && x.Id == scheduler.AssigneeId).FirstOrDefault();
                scheduler.PersonId = orgRoleUser.UserId;
                scheduler.IsCancellationMailSend = true;
                scheduler.IsInvoiceRequired = model.IsInvoiceRequired;
                scheduler.InvoiceReason = model.InvoiceReason;
                _jobSchedulerRepository.Save(scheduler);
                model.SchedulerIds = scheduler.Id;
                model.SchedulerId = scheduler.Id;
                model.Estimateid = scheduler.EstimateId;
            }
            else
            {
                isNewScheduler = true;
                scheduler = _jobFactory.CreateDomain(model.JobScheduler);
                scheduler.IsNew = !isUpdate;
                var orgRoleUser = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == scheduler.FranchiseeId && x.Id == scheduler.AssigneeId).FirstOrDefault();
                scheduler.PersonId = orgRoleUser.UserId;
                scheduler.IsCancellationMailSend = true;
                scheduler.IsInvoiceRequired = true;
                _jobSchedulerRepository.Save(scheduler);
                model.SchedulerIds = scheduler.Id;
                model.SchedulerId = scheduler.Id;
                model.Estimateid = scheduler.EstimateId;
                model.IsInvoiceRequired = true;
            }

            SavingJobDetails(scheduler, model, model.SchedulerId);

            if (isNewScheduler)
            {
                SendMailToCustomer(scheduler.Id);
            }
            else
            {
                var isUpdated = IsUpdatedForTimeOrAssigneeIdForEstimate(oldEndDatetime, oldStartDatetime, scheduler);
                if (isUpdated)
                    SendUpdationMailToCustomer(scheduler.Id);
            }

            model.Id = scheduler.EstimateId.Value;
            DateTime currentUtcDate = _clock.ToLocal(_clock.UtcNow, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime futureUtcDate = _clock.ToLocal(_clock.UtcNow.AddDays(1), _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseCurrentUtcDate = _clock.ToLocal(model.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseFutureUtcDate = _clock.ToLocal(model.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            newdTechList = _jobschedulerRepository.Table.Where(x => x.EstimateId == model.Id && x.IsActive).Select(x => x.AssigneeId.Value).Distinct().ToList();
            List<long> addedTechList = newdTechList.Except(oldTechList).ToList();
            List<long> deletedTechList = oldTechList.Except(newdTechList).ToList();
            List<long> sameTechList = oldTechList.Intersect(newdTechList).ToList();
            List<long> rescheduledTechList = oldInactiveTechList.Intersect(addedTechList).ToList();
            addedTechList = addedTechList.Except(rescheduledTechList).ToList();

            if (isSales)
            {
                if (oldDaata != null)
                {
                    isUpdate = IsUpdated(model, oldAddress1, oldAddress2, oldStartDatetime, oldEndDatetime);
                }
                if (isNewJob == true)
                {
                    if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                    {
                        if (databaseCurrentUtcDate.Date == currentUtcDate.Date)
                        {
                            model.dateType = "TODAY";
                        }
                        else
                        {
                            model.dateType = "TOMORROW";
                        }
                        model.jobTypeName = jobType;
                        model.JobType = jobType;
                        model.Estimateid = scheduler.EstimateId.GetValueOrDefault();
                        SendingUrgentMails(model, model.SalesRepId.GetValueOrDefault());
                    }
                    else
                    {
                        model.jobTypeName = jobType;
                        model.JobType = jobType;
                        SendingNewsMails(model, model.SalesRepId.GetValueOrDefault());
                    }
                }
                else
                {

                    // For New Added Technician
                    foreach (var orgRoleUserId in addedTechList)
                    {

                        model.jobTypeName = jobType;
                        model.JobType = jobType;
                        if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                        {
                            if (databaseCurrentUtcDate.Date == currentUtcDate.Date)
                            {
                                model.dateType = "TODAY";
                            }
                            else
                            {
                                model.dateType = "TOMORROW";
                            }
                            model.jobTypeName = jobType;
                            model.JobType = jobType;
                            SendingUrgentMails(model, model.SalesRepId.GetValueOrDefault());
                        }
                        else
                        {
                            model.jobTypeName = jobType;
                            model.JobType = jobType;
                            SendingNewsMails(model, model.SalesRepId.GetValueOrDefault());
                        }

                    }
                    foreach (var items in deletedTechList)
                    {
                        model.jobTypeName = jobType;
                        model.JobType = jobType;
                        SendingCancellationMails(model, items);
                    }
                    foreach (var items in rescheduledTechList)
                    {
                        if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                        {
                            if (databaseCurrentUtcDate.Date == currentUtcDate.Date)
                            {
                                model.dateType = "TODAY";
                            }
                            else
                            {
                                model.dateType = "TOMORROW";
                            }
                            model.jobTypeName = jobType;
                            model.JobType = jobType;
                            SendingUrgentMails(model, model.SalesRepId.GetValueOrDefault());
                        }
                        else
                        {
                            model.jobTypeName = jobType;
                            model.JobType = jobType;
                            SendingRescheduledMails(model, items);
                        }

                    }
                    if (isUpdate)
                    {
                        if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                        {
                            model.JobType = "URGENT " + jobType;
                        }
                        else
                        {
                            model.JobType = jobType;
                        }
                        foreach (var items in sameTechList)
                        {
                            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
                            model.jobTypeName = jobType;
                            SendingUpdationMails(model, items);
                        }
                    }
                }
            }
            else
            {
                if (deletedTechList.Count > 0)
                {
                    long techid = deletedTechList[0];
                    var isCancelledRoleSale = _organizationRoleUserRepository.Table.Where(x => x.Id == techid && x.RoleId == (long)RoleType.SalesRep).ToList();
                    if (isCancelledRoleSale.Count > 0)
                    {
                        foreach (var items in deletedTechList)
                        {
                            model.jobTypeName = jobType;
                            model.JobType = jobType;

                            SendingCancellationMails(model, items);
                        }
                    }
                }
            }
        }
        public void SendingUpdationMails(JobEstimateEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForUpdationForEstimate(model, listSchedule);
        }
        private void SendingCancellationMails(JobEstimateEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelledForEstimate(model, listSchedule);
        }

        private void SendingRescheduledMails(JobEstimateEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForRescheduledForEstimate(model, listSchedule);
        }

        private void SendingUrgentMails(JobEstimateEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            model.jobTypeName = "Estimate";
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForUrgent(model, listSchedule);
        }
        private void SendingNewsMails(JobEstimateEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            model.jobTypeName = jobType;
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForEstimate(model, listSchedule);


        }
        public bool Delete(long estimateId)
        {
            if (estimateId > 0)
            {
                var estimateInfo = _jobSchedulerRepository.Table.Where(x => x.Id == estimateId).FirstOrDefault();
                var jobSchedulers = _jobSchedulerRepository.IncludeMultiple(x => x.Estimate).Where(x => x.Id == estimateId).ToList();
                var estimate = _jobEstimateRepository.Get(x => x.Id == estimateInfo.EstimateId);
                foreach (var scheduler in jobSchedulers)
                {
                    _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelledForDeleteButton(scheduler, true);
                    _jobSchedulerRepository.Delete(scheduler.Id);
                }
                //var jobList = estimate.Jobs;
                //if (!jobList.Any())
                //{
                //    _jobCustomerRepository.Delete(x => x.Id == estimate.CustomerId);
                //}
                //_jobEstimateRepository.Delete(estimate);
                return true;
            }
            return false;
        }

        public JobEstimateEditModel GetVacationInfo(long id)
        {
            if (id == 0)
                return new JobEstimateEditModel { };

            else
            {
                var vacation = _jobSchedulerRepository.Get(x => x.Id == id);
                if (vacation == null)
                    return new JobEstimateEditModel { };
                var model = _jobFactory.CreateVacationModel(vacation);
                return model;
            }
        }
        public MeetingEditModel GetMessageInfo(long id)
        {
            if (id == 0)
                return new MeetingEditModel { };

            else
            {
                var vacation = _jobSchedulerRepository.Get(x => x.Id == id);

                if (vacation == null)
                    return new MeetingEditModel { };

                var model = _jobFactory.CreateMeetingModel(vacation);
                return model;
            }
        }
        public long SaveMeeting(JobEstimateEditModel model)
        {
            var isNew = model.SchedulerId == 0 ? true : false;
            var scheduler = _jobFactory.CreatMeetingModel(model);
            _meetingRepository.Save(scheduler);


            return scheduler.Id;
        }

        public void SendMailToMember(JobEstimateEditModel model, NotificationTypes notificationTypes)
        {
            var listSchedule = (_organizationRoleUserRepository.IncludeMultiple(x => x.Organization, x => x.Organization.Franchisee, x => x.Person.Phones)
                .Where(x => x.Id == model.LogginUserId)).FirstOrDefault();
            _sendNewJobNotificationToTechService.SendJobNotificationtoTechForMeetingPersonal(model, listSchedule, notificationTypes);
        }
        public void SaveVacation(JobEstimateEditModel model)
        {
            if (model.Title == null)
            {
                var count = _jobSchedulerRepository.Table.Where(x => x.IsVacation && x.IsActive).Count();
                model.Title = "Vacation" + count;
            }
            var personId = model.AssigneeId;
            var assigneeId = _organizationRoleUserRepository.Table.Where(x => x.UserId == model.AssigneeId && model.FranchiseeId == model.FranchiseeId).Select(x => x.Id).FirstOrDefault();
            model.AssigneeId = assigneeId;
            var scheduler = _jobFactory.CreateSchedulerDomain(model);
            scheduler.PersonId = personId;
            _jobSchedulerRepository.Save(scheduler);
            model.IsVacation = true;
            model.SchedulerId = scheduler.Id;
            if (model.PersonId == null)
            {
                model.PersonId = personId;
            }
            SendMailToMember(model, NotificationTypes.PersonalMailForMemebers);
        }


        public bool DeleteVacation(long id)
        {
            if (id > 0)
            {
                var scheduler = _jobSchedulerRepository.Get(id);

                if (scheduler == null)
                    return false;

                var user = _organizationRoleUserInfoService.GetOrganizationRoleUserbyId(scheduler.AssigneeId.Value);
                if (user == null)
                    return false;

                var userIds = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(user.UserId, scheduler.FranchiseeId);
                var listScheduler = _jobSchedulerRepository.Table.Where(x => userIds.Contains(x.AssigneeId.Value) && x.Id != scheduler.Id
                                        && x.StartDate == scheduler.StartDate && x.EndDate == scheduler.EndDate && x.FranchiseeId == scheduler.FranchiseeId
                                        && x.IsVacation).ToList();
                foreach (var item in listScheduler)
                {
                    _jobSchedulerRepository.Delete(item);
                }
                _jobSchedulerRepository.Delete(scheduler);
                return true;
            }
            return false;
        }

        private class JobOccuranceAssigneeModel
        {
            public long AssigneeId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public long? SchedulerId { get; set; }
            public long? EstimateId { get; set; }
        }
        public JobOccurenceListModel GetOccurenceInfo(long id)
        {
            var Job = _jobRepository.Get(id);
            var schedulerList = _jobSchedulerRepository.Table.Where(j => j.Job != null && j.JobId == id && j.IsActive).ToList();

            return new JobOccurenceListModel
            {
                Collection = schedulerList.Select(_jobFactory.CreateEditModel),
                ParentJobId = id,
                FranchiseeId = schedulerList.FirstOrDefault() != null ? schedulerList.FirstOrDefault().FranchiseeId : 0
            };
        }

        public JobOccurenceListModel GetEstimateOccurenceInfo(long id)
        {
            var Job = _jobRepository.Get(id);

            var schedulerList = _jobSchedulerRepository.Table.Where(j => j.Estimate != null && j.EstimateId == id && j.IsActive).FirstOrDefault();
            var parentEstimateIds = _jobEstimateRepository.Table.Where(x => x.ParentEstimateId == id).Select(x => x.Id).ToList();
            var estimateIds = getParentEstimateIds(schedulerList);
            if (estimateIds.Count <= 0)
            {
                estimateIds.Add(id);
            }
            foreach (var parentEstimateId in parentEstimateIds)
                estimateIds.Add(parentEstimateId);
            var schedulerLists = _jobSchedulerRepository.Table.Where(j => j.Estimate != null && j.EstimateId.HasValue && estimateIds.Contains(j.EstimateId.Value) && j.IsActive).AsEnumerable().ToList();
            return new JobOccurenceListModel
            {
                Collection = schedulerLists.Select(_jobFactory.CreateEditModel),
                ParentEstimateId = id,
                FranchiseeId = schedulerLists.FirstOrDefault() != null ? schedulerLists.FirstOrDefault().FranchiseeId : 0
            };
        }

        public bool SaveSchedule(JobOccurenceListModel model)
        {
            var isDeleted = false;
            var salesRepid = default(long?);
            var isValueChanged = default(bool);
            var occuranceJobId = model.ParentJobId;
            var oldSchedulerData = _jobschedulerRepository.Table.Where(x => x.JobId == occuranceJobId && x.IsActive).ToList();
            var repeatedSchedulerDetails = _jobDetailsRepository.Table.FirstOrDefault(x => x.SchedulerId == model.SchedulerId);
            salesRepid = oldSchedulerData.Select(x => x.SalesRepId).FirstOrDefault();
            var schedulerIdRepeated = default(long?);
            var oldSchedulerDataCopy = oldSchedulerData.Select(scheduler => new JobScheduler
            {
                AssigneeId = scheduler.AssigneeId,
                StartDate = scheduler.StartDate,
                EndDate = scheduler.EndDate,
                Id = scheduler.Id,
                JobId = scheduler.JobId,
                StartDateTimeString = scheduler.StartDateTimeString,
                EndDateTimeString = scheduler.EndDateTimeString,

            }).ToList();

            oldInactiveTechList = _jobschedulerRepository.Table.Where(x => x.JobId == occuranceJobId && !x.IsActive).Select(x => x.AssigneeId.Value).Distinct().ToList();
            newdTechList = model.Collection.Select(x => x.AssigneeId).ToList();

            var oldSchedulerDateAndTime = new List<JobOccuranceAssigneeModel>();
            oldTechList = _jobschedulerRepository.Table.Where(x => x.JobId == occuranceJobId && x.IsActive).Select(x => x.AssigneeId.Value).ToList();

            List<long> addedTechList = newdTechList.Except(oldTechList).ToList();
            List<long> deletedTechList = oldTechList.Except(newdTechList).ToList();
            List<long> sameTechList = oldTechList.Intersect(newdTechList).ToList();
            List<long> rescheduledTechList = oldInactiveTechList.Intersect(addedTechList).ToList();
            addedTechList = addedTechList.Except(rescheduledTechList).ToList();

            if (model.Collection.Count() <= 0)
                return true;

            var job = _jobRepository.Get(model.ParentJobId);
            if (job == null || job.JobScheduler == null || !job.JobScheduler.Any())
                return false;

            var schedulesToDelete = new List<JobOccurenceEditModel>();

            var jobScheduler = job.JobScheduler.Where(x => x.IsActive).ToList();
            var set = getOldSchedulerDateTime(jobScheduler, out oldSchedulerDateAndTime);

            var inDbSchedules = _jobSchedulerRepository.Table.Where(j => j.Job != null && j.JobId == model.ParentJobId && j.IsActive).ToList();
            var inDbOccurance = inDbSchedules;
            var newOccurance = model.Collection.ToList();
            var scheduleInfo = job.JobScheduler.FirstOrDefault();
            long jobId = scheduleInfo.JobId.Value;
            long? serviceTypeId = scheduleInfo.ServiceTypeId != null ? scheduleInfo.ServiceTypeId.Value : (long?)null;
            var metaData = scheduleInfo.DataRecorderMetaData;


            isValueChanged = IsValueChanged(oldSchedulerData, model.Collection.ToList(), out schedulerIdRepeated);


            foreach (var schedule in inDbSchedules)
            {
                var jobEditModel = _jobFactory.CreateEditModel(job, default(JobScheduler));
                if (!model.Collection.Where(c => c.ScheduleId == schedule.Id).Any())
                {
                    long assigneeId = schedule.AssigneeId.GetValueOrDefault();
                    var index = deletedTechList.IndexOf(assigneeId);
                    if (index >= 0)
                    {
                        deletedTechList.RemoveAt(index);
                    }
                    var job2 = _jobRepository.Get(model.ParentJobId);
                    var jobScheduler2 = job2 != null && job2.JobScheduler != null ? job2.JobScheduler.Where(x => x.AssigneeId == assigneeId && x.IsActive && x.JobId == job2.Id).FirstOrDefault() : null;
                    if (jobScheduler2 != null)
                    {
                        jobEditModel.ActualStartDateString = schedule.StartDateTimeString;
                        jobEditModel.ActualEndDateString = schedule.EndDateTimeString;
                    }
                    isDeleted = true;
                    jobEditModel.SchedulerId = schedule.Id;
                    jobEditModel.JobId = schedule.JobId.GetValueOrDefault();
                    SendingCancellationJobMails(jobEditModel, assigneeId);
                    var person = _organizationRoleUserRepository.Table.Where(x => x.Id == schedule.AssigneeId && x.OrganizationId == schedule.FranchiseeId).FirstOrDefault();
                    schedule.PersonId = person.UserId;
                    schedule.IsActive = false;
                    schedule.IsCancellationMailSend = true;
                    _jobSchedulerRepository.Save(schedule);
                }

                var newScheduler = model.Collection.FirstOrDefault(x => x.ScheduleId == schedule.Id);
                if (newScheduler != null)
                {
                    if (newScheduler.AssigneeId != schedule.AssigneeId)
                    {
                        isDeleted = true;
                        jobEditModel.JobId = schedule.JobId.GetValueOrDefault();
                        jobEditModel.SchedulerId = schedule.Id;
                        SendingCancellationJobMails(jobEditModel, schedule.AssigneeId.GetValueOrDefault());
                    }
                }
            }

            foreach (var item in model.Collection)
            {
                var isNew = false;
                if (item.ScheduleId > 0)
                {
                    scheduleInfo = inDbSchedules.Where(x => x.Id == item.ScheduleId).FirstOrDefault();
                    item.ScheduleId = scheduleInfo.Id;
                }
                if (scheduleInfo == null)
                    scheduleInfo = job.JobScheduler.FirstOrDefault();
                if (scheduleInfo != null)
                {
                    item.FranchiseeId = scheduleInfo.FranchiseeId;
                    item.Title = scheduleInfo.Title;
                    item.DataRecorderMetaData = scheduleInfo.DataRecorderMetaData;
                    item.ParentJobId = jobId;
                    item.ServiceTypeId = serviceTypeId != null ? serviceTypeId.Value : (long?)null;
                    item.DataRecorderMetaData = item.DataRecorderMetaData != null ? item.DataRecorderMetaData : metaData;
                }

                if (item.ScheduleId <= 0)
                {
                    isNew = true;

                    var jobDomain = _jobRepository.Get(item.ParentJobId);
                    if (jobDomain != null)
                    {
                        var estimateInvoiceDomain = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.EstimateId == jobDomain.EstimateId);

                        if (estimateInvoiceDomain != null)
                        {
                            estimateInvoiceDomain.IsInvoiceChanged = true;
                            _estimateInvoiceRepository.Save(estimateInvoiceDomain);

                        }
                    }
                }
                var domain = _jobFactory.CreateDomain(item);
                var person = _organizationRoleUserRepository.Table.Where(x => x.Id == item.AssigneeId && x.OrganizationId == item.FranchiseeId).FirstOrDefault();
                if (salesRepid == 0)
                {
                    salesRepid = null;
                }
                domain.SalesRepId = salesRepid;
                domain.PersonId = person.UserId;
                if (item.ScheduleId > 0)
                {
                    domain.ParentJobId = scheduleInfo != null ? scheduleInfo.ParentJobId : default(long?);
                    domain.IsRepeat = scheduleInfo != null ? scheduleInfo.IsRepeat : default(bool);
                }
                else
                {
                    domain.IsCustomerAvailable = true;
                    //domain.IsInvoiceRequired = true;
                }
                domain.IsCancellationMailSend = true;
                domain.IsJobConverted = true;

                _jobSchedulerRepository.Save(domain);

                if (isNew)
                {
                    var description = repeatedSchedulerDetails != null ? repeatedSchedulerDetails.Description : job.Description;

                    SavingJobDetails(domain, description);

                    var jobscheduler = _jobRepository.Table.FirstOrDefault(x => x.Id == domain.JobId);
                    var jobestimate = _jobschedulerRepository.Table.FirstOrDefault(x => x.EstimateId == jobscheduler.EstimateId);
                    if (jobscheduler.EstimateId != null)
                    {
                        SaveAssigneeInfo(item, domain.Id, domain.JobId.Value, jobscheduler.EstimateId.Value);
                        SaveJobEstimateService(jobestimate.Id, item.InvoiceNumber.Select(x => x.Id).ToList(), domain.JobId.Value, domain.Id);
                    }
                }
                else
                {
                    var oldSchedulerDataForNewCheck = oldSchedulerDataCopy.FirstOrDefault(x => x.Id == domain.Id);

                    var jobscheduler = _jobRepository.Table.FirstOrDefault(x => x.Id == domain.JobId);
                    var jobestimate = _jobschedulerRepository.Table.FirstOrDefault(x => x.EstimateId == jobscheduler.EstimateId);
                    if (jobscheduler.EstimateId != null)
                    {
                        SaveAssigneeInfo(item, item.ScheduleId, domain.JobId.Value, jobscheduler.EstimateId.Value);
                        SaveJobEstimateService(jobestimate.Id, item.InvoiceNumber.Select(x => x.Id).ToList(), domain.JobId.Value, item.ScheduleId);
                    }
                    if (item.AssigneeId != oldSchedulerDataForNewCheck.AssigneeId && !rescheduledTechList.Contains(item.ScheduleId))
                    {
                        item.ScheduleId = 0;
                    }
                }

            }



            if (deletedTechList.Count() > 0)
            {
                var deletedJob = _jobRepository.Get(model.ParentJobId);
                var jobEditModel = _jobFactory.CreateEditModel(deletedJob, default(JobScheduler));
                jobEditModel.JobId = deletedJob.Id;
                //jobEditModel.SchedulerId = item.Id;
                sendMailToDeleteTech(deletedTechList, model.Collection, jobEditModel, oldSchedulerDataCopy);
            }
            if (model.Collection.Any(x => x.ScheduleId == 0))
            {
                var jobEditModel = _jobFactory.CreateEditModel(job, default(JobScheduler));
                sendMailToNewTech(addedTechList, model.Collection, jobEditModel);
            }

            if (rescheduledTechList.Count() > 0)
            {
                var jobEditModel = _jobFactory.CreateEditModel(job, default(JobScheduler));
                sendMailToResheculeTech(rescheduledTechList, model.Collection, jobEditModel);
            }
            if (sameTechList.Count() > 0)
            {
                var job2 = _jobRepository.Get(model.ParentJobId);
                var newJobScheduler = job2 != null && job2.JobScheduler != null ? job2.JobScheduler.Where(x => x.IsActive).ToList() : null;
                if (newJobScheduler != null)
                {
                    newJobScheduler = newJobScheduler.Where(x => sameTechList.Contains(x.AssigneeId.GetValueOrDefault())).ToList();
                    var isUpdated = isScheduleChanged(oldSchedulerDataCopy, model.Collection.ToList());
                }
            }

            var schedulerUpdatedList = _jobschedulerRepository.Table.Where(x => x.JobId == jobId && x.IsActive).ToList();
            var schedulerId = default(long?);
            var isJobForSpanChanges = IsUpdatedForTimeOrAssigneeId(oldSchedulerDataCopy, schedulerUpdatedList, out schedulerId);
            if (isJobForSpanChanges)
            {
                SendUpdationMailToCustomer(schedulerId);
            }
            return true;
        }

        private void SaveJobEstimateService(long schedulerId, List<long?> invoiceNumbers, long jobId, long scheduleJobId)
        {
            _estimateInvoiceServices.AddInvoiceToEstimate(schedulerId, true, invoiceNumbers, jobId, scheduleJobId);
        }

        private void SaveAssigneeInfo(JobOccurenceEditModel model, long schedulerIdForJob, long jobId, long estimateId)
        {
            model.ParentJobId = jobId;
            var assignees = _estimateInvoiceAssignee.Table.Where(x => x.SchedulerId == schedulerIdForJob && x.EstimateId == estimateId).ToList();
            foreach (var assignee in assignees)
            {
                _estimateInvoiceAssignee.Delete(assignee);
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

        private bool getOldSchedulerDateTime(List<JobScheduler> jobScheduler, out List<JobOccuranceAssigneeModel> jobOccurance)
        {
            jobOccurance = new List<JobOccuranceAssigneeModel>();
            var assigneeIds = jobScheduler.Where(x => x.IsActive).Select(x => x.AssigneeId).ToList();
            var jobOccuranceObj = new JobOccuranceAssigneeModel();
            foreach (var assigneeId in assigneeIds)
            {
                var datetimeObject = jobScheduler.Where(x => x.AssigneeId == assigneeId && x.IsActive).FirstOrDefault();
                jobOccuranceObj = new JobOccuranceAssigneeModel()
                {
                    AssigneeId = assigneeId.GetValueOrDefault(),
                    StartDate = datetimeObject.StartDateTimeString,
                    EndDate = datetimeObject.EndDateTimeString,
                    SchedulerId = datetimeObject.Id,
                    EstimateId = datetimeObject.EstimateId
                };
                jobOccurance.Add(jobOccuranceObj);
            }
            return true;
        }


        private bool isScheduleChanged(List<JobScheduler> oldSchedulerValue, List<JobOccurenceEditModel> newSchedulerValue)
        {
            foreach (var scheduler in oldSchedulerValue)
            {
                var newScheduler = newSchedulerValue.FirstOrDefault(x => x.ScheduleId == scheduler.Id);
                if (newScheduler == null) continue;
                if (newScheduler.AssigneeId == scheduler.AssigneeId && newScheduler.ActualEndDateString == scheduler.EndDateTimeString
                    && newScheduler.ActualStartDateString == scheduler.StartDateTimeString)
                {
                    continue;
                }
                else
                {
                    var job2 = _jobRepository.Get(scheduler.JobId.GetValueOrDefault());

                    var jobEditModel = _jobFactory.CreateEditModel(job2, default(JobScheduler));
                    jobEditModel.ActualStartDateString = scheduler.StartDateTimeString;
                    jobEditModel.ActualEndDateString = scheduler.EndDateTimeString;
                    jobEditModel.jobType = "Job";
                    SendingJobUpdationMails(jobEditModel, scheduler.AssigneeId.GetValueOrDefault());
                }
            }
            return false;
        }
        public bool SaveEstimateSchedule(JobOccurenceListModel model)
        {
            var schedulerIdRepeated = default(long?);
            if (model.Collection.Count() <= 0)
                return true;

            var estimate = _jobEstimateRepository.Get(model.ParentEstimateId);

            if (estimate == null)
                return false;

            var schedulesToDelete = new List<JobOccurenceEditModel>();

            var scheduleInfo = _jobSchedulerRepository.Table.Where(x => x.EstimateId == model.ParentEstimateId).FirstOrDefault();
            var estimateWorth = scheduleInfo.EstimateWorth;
            var estimateIds = getParentEstimateIds(scheduleInfo);
            var inDbScheduler = _jobschedulerRepository.Table.Where(x => estimateIds.Contains(x.EstimateId.Value) && x.IsActive).ToList();
            var oldSchedulerDataCopy = inDbScheduler.Select(scheduler => new JobScheduler
            {
                AssigneeId = scheduler.AssigneeId,
                StartDate = scheduler.StartDate,
                EndDate = scheduler.EndDate,
                Id = scheduler.Id,
                JobId = scheduler.JobId,
                StartDateTimeString = scheduler.StartDateTimeString,
                EndDateTimeString = scheduler.EndDateTimeString
            }).ToList();

            if (estimateIds.Count <= 0)
            {
                var inDbChildEstimates = _jobEstimateRepository.Table.Where(x => x.ParentEstimateId == model.ParentEstimateId).ToList();
                foreach (var inDbChildEstimate in inDbChildEstimates)
                    estimateIds.Add(inDbChildEstimate.Id);
                estimateIds.Add(model.ParentEstimateId);
            }
            var oldSchedulerDateAndTime = new List<JobOccuranceAssigneeModel>();
            estimateIds = estimateIds.OrderBy(x => x).ToList();
            var inDbSchedules = _jobSchedulerRepository.Table.Where(j => j.Estimate != null && j.EstimateId.HasValue && estimateIds.Distinct().Contains(j.EstimateId.Value)).ToList();
            var set = getOldSchedulerDateTime(inDbSchedules, out oldSchedulerDateAndTime);
            long estimateId = scheduleInfo.EstimateId.Value;
            long? serviceTypeId = scheduleInfo.ServiceTypeId != null ? scheduleInfo.ServiceTypeId.Value : (long?)null;
            var metaData = scheduleInfo.DataRecorderMetaData;

            foreach (var schedule in inDbSchedules)
            {
                if (!model.Collection.Where(c => c.ScheduleId == schedule.Id).Any())
                {
                    schedule.IsActive = false;
                    schedule.IsCancellationMailSend = true;
                    _jobSchedulerRepository.Save(schedule);
                    var jobEstimate = _jobFactory.CreateEstimateModel(schedule.Estimate.JobEstimates, schedule);
                    if (schedule != null && schedule.OrganizationRoleUser != null && schedule.OrganizationRoleUser.RoleId != (long)RoleType.Technician)
                    {
                        jobEstimate.Estimateid = schedule.EstimateId;
                        jobEstimate.SchedulerId = schedule.Id;
                        SendingCancellationMails(jobEstimate, schedule.AssigneeId.GetValueOrDefault());
                    }

                }
                var newScheduler = model.Collection.FirstOrDefault(x => x.ScheduleId == schedule.Id);
                if (newScheduler != null)
                {

                    if (newScheduler.AssigneeId != schedule.AssigneeId)
                    {
                        var jobEstimate = _jobFactory.CreateEstimateModel(schedule.Estimate.JobEstimates, schedule);
                        jobEstimate.Estimateid = schedule.EstimateId;
                        jobEstimate.SchedulerId = schedule.Id;
                        SendingCancellationMails(jobEstimate, schedule.AssigneeId.GetValueOrDefault());
                    }
                }
            }

            foreach (var item in model.Collection)
            {

                var oldData = new JobSchedulerEditModel();
                if (item.ScheduleId > 0)
                {
                    var oldData2 = _jobschedulerRepository.Table.Where(x => x.EstimateId == item.ParentEstimateId && x.IsActive).FirstOrDefault();


                    oldData.StartDate = oldData2.StartDate;
                    oldData.EndDate = oldData2.EndDate;
                    oldData.AddressLine1 = oldData2.Estimate.JobCustomer.Address.AddressLine1;
                    oldData.AddressLine2 = oldData2.Estimate.JobCustomer.Address.AddressLine2;
                    scheduleInfo = inDbSchedules.Where(x => x.Id == item.ScheduleId && x.IsActive).FirstOrDefault();
                    item.ScheduleId = scheduleInfo.Id;

                }
                if (scheduleInfo == null)
                {
                    //scheduleInfo = job.JobScheduler.FirstOrDefault();
                    scheduleInfo = _jobSchedulerRepository.Table.Where(x => x.EstimateId == model.ParentEstimateId && x.IsActive).FirstOrDefault();
                }
                if (scheduleInfo != null)
                {
                    item.FranchiseeId = scheduleInfo.FranchiseeId;
                    item.Title = scheduleInfo.Title;
                    item.DataRecorderMetaData = scheduleInfo.DataRecorderMetaData;
                    item.ParentJobId = estimateId;
                    item.ServiceTypeId = serviceTypeId != null ? serviceTypeId.Value : (long?)null;
                    item.DataRecorderMetaData = item.DataRecorderMetaData != null ? item.DataRecorderMetaData : metaData;

                }
                var person = _organizationRoleUserRepository.Table.Where(x => x.Id == item.AssigneeId && x.OrganizationId == item.FranchiseeId).FirstOrDefault();
                var domain = _jobFactory.CreateDomainForEstimate(item);
                if (person != null)
                {
                    domain.PersonId = person.UserId;
                }
                domain.EstimateId = item.ParentEstimateId;
                domain.JobId = null;
                var isNew = false;
                if (item.ScheduleId <= 0)
                {
                    var domain2 = _jobFactory.CreateDomainOccurance(item, estimate);
                    _jobEstimateRepository.Save(domain2);
                    domain.EstimateId = domain2.Id;
                    domain.IsActive = true;
                    isNew = true;
                }
                domain.IsCancellationMailSend = true;
                _jobSchedulerRepository.Save(domain);
                SavingJobDetails(domain);
                if (isNew)
                {
                    model.SchedulerId = domain.Id;
                }
                SendingRepeativeMails(model, item, oldData, domain, oldSchedulerDateAndTime, inDbSchedules);
            }


            var isValueChanged = IsValueChanged(inDbSchedules, model.Collection.ToList(), out schedulerIdRepeated);
            estimateIds = getParentEstimateIds(scheduleInfo);
            var schedulesList = _jobSchedulerRepository.Table.Where(j => estimateIds.Distinct().Contains(j.EstimateId.Value) && j.IsActive).ToList();
            var schedulerId = default(long?);
            var isValueUpdated = IsUpdatedForTimeOrAssigneeId(oldSchedulerDataCopy, schedulesList, out schedulerId);
            if (isValueUpdated)
            {
                SendUpdationMailToCustomer(schedulerId);
            }
            return true;
        }

        private void SendingRepeativeMails(JobOccurenceListModel model, JobOccurenceEditModel editModel, JobSchedulerEditModel oldData, JobScheduler customerData, List<JobOccuranceAssigneeModel> oldSchedulerDateAndTime, List<JobScheduler> inDbSchedules)
        {
            var jobScheduler = _jobschedulerRepository.IncludeMultiple(x => x.Franchisee, x => x.Estimate.JobEstimates, x => x.SalesRep).Where(x => x.Id == customerData.Id).FirstOrDefault();
            var jobEstimate = _jobEstimateRepository.Get(customerData.EstimateId.GetValueOrDefault());
            var jobEstimateEditModel = _jobFactory.CreateEstimateModel(jobEstimate, jobScheduler);
            DateTime currentUtcDate = _clock.ToLocal(_clock.UtcNow, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime futureUtcDate = _clock.ToLocal(_clock.UtcNow.AddDays(1), _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseCurrentUtcDate = _clock.ToLocal(customerData.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseFutureUtcDate = _clock.ToLocal(customerData.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            bool isSales = (_organizationRoleUserRepository.Fetch(x => x.Id == editModel.AssigneeId && x.Role.Id == (long)RoleType.SalesRep)).Any();
            bool isUpdate = default(bool);
            bool isReScheduled = false;
            if (isSales)
            {
                if (editModel.ScheduleId > 0 && oldSchedulerDateAndTime.Any(x => x.EstimateId == editModel.ParentEstimateId && x.AssigneeId != editModel.AssigneeId))
                {
                    isReScheduled = true;
                    var schedule = inDbSchedules.Where(x => x.EstimateId == editModel.ParentEstimateId).FirstOrDefault();
                    var jobEstimate2 = _jobFactory.CreateEstimateModel(schedule.Estimate.JobEstimates, schedule);
                    var assineeIdOld = oldSchedulerDateAndTime.Where(x => x.EstimateId == editModel.ParentEstimateId && x.SchedulerId == editModel.ScheduleId).Select(x => x.AssigneeId).FirstOrDefault();
                    if (schedule != null && schedule.OrganizationRoleUser != null && schedule.OrganizationRoleUser.RoleId != (long)RoleType.Technician)
                    {
                        jobEstimate2.ActualStartDateString = oldSchedulerDateAndTime.Where(x => x.EstimateId == editModel.ParentEstimateId && x.SchedulerId == editModel.ScheduleId).Select(x => x.StartDate).FirstOrDefault();
                        jobEstimate2.Estimateid = schedule.EstimateId;
                        jobEstimate2.SchedulerId = editModel.ScheduleId;
                        SendingCancellationMails(jobEstimate2, assineeIdOld);

                    }
                    if (editModel.ScheduleId > 0)
                    {
                        if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                        {
                            if (databaseCurrentUtcDate.Date == currentUtcDate.Date)
                            {
                                jobEstimateEditModel.dateType = "TODAY";
                            }

                            else
                            {
                                jobEstimateEditModel.dateType = "TOMORROW";
                            }

                            jobEstimateEditModel.jobTypeName = jobType;
                            jobEstimateEditModel.JobType = jobType;
                            jobEstimate2.Estimateid = schedule.EstimateId;
                            jobEstimate2.SchedulerId = editModel.ScheduleId;
                            SendingUrgentMails(jobEstimateEditModel, jobEstimateEditModel.SalesRepId.GetValueOrDefault());
                        }
                        else
                        {
                            jobEstimateEditModel.jobTypeName = jobType;
                            jobEstimateEditModel.JobType = jobType;
                            jobEstimate2.Estimateid = schedule.EstimateId;
                            jobEstimate2.SchedulerId = editModel.ScheduleId;
                            SendingNewsMails(jobEstimateEditModel, jobEstimateEditModel.SalesRepId.GetValueOrDefault());
                        }
                    }
                }
                var scheduler = _jobschedulerRepository.Get(customerData.Id);
                if (editModel.ScheduleId > 0)
                {
                    DateTime oldStartDatetime = oldData.StartDate;
                    DateTime oldEndDatetime = oldData.EndDate;
                    string addressLine1 = oldData.AddressLine1;
                    string addressLine2 = oldData.AddressLine2;
                    isUpdate = IsUpdated(customerData, oldStartDatetime, oldEndDatetime);

                    if (isUpdate && !isReScheduled)
                    {
                        jobEstimateEditModel.Estimateid = scheduler.EstimateId;
                        jobEstimateEditModel.SchedulerId = scheduler.Id;
                        SendingUpdationMails(jobEstimateEditModel, jobEstimateEditModel.SalesRepId.GetValueOrDefault());
                    }

                }
                if (editModel.ScheduleId <= 0)
                {

                    if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                    {
                        if (databaseCurrentUtcDate.Date == currentUtcDate.Date)
                        {
                            jobEstimateEditModel.dateType = "TODAY";
                        }

                        else
                        {
                            jobEstimateEditModel.dateType = "TOMORROW";
                        }

                        jobEstimateEditModel.jobTypeName = jobType;
                        jobEstimateEditModel.JobType = jobType;
                        SendingUrgentMails(jobEstimateEditModel, jobEstimateEditModel.SalesRepId.GetValueOrDefault());
                    }
                    else
                    {
                        jobEstimateEditModel.jobTypeName = jobType;
                        jobEstimateEditModel.JobType = jobType;
                        jobEstimateEditModel.SchedulerId = model.SchedulerId;
                        jobEstimateEditModel.Estimateid = scheduler.EstimateId;
                        SendingNewsMails(jobEstimateEditModel, jobEstimateEditModel.SalesRepId.GetValueOrDefault());
                    }
                }
            }
        }
        public bool CheckDuplicateAssignment(JobOccurenceListModel model)
        {
            var assigneeList = model.Collection;
            var result = assigneeList.GroupBy(al => new { al.AssigneeId, al.StartDate, al.EndDate }).Any(c => c.Count() > 1);
            return result;
        }

        public void RepeatVacation(VacationRepeatEditModel model)
        {
            var startDateTimeStrings = default(DateTime);
            var endDateTimeStrings = default(DateTime);
            var vacationInfo = _jobSchedulerRepository.Get(x => x.IsVacation && x.Id == model.VacationId);
            if (vacationInfo == null)
                return;

            var interval = (vacationInfo.EndDate - vacationInfo.StartDate).TotalMinutes;
            model.Title = vacationInfo.Title;
            if (model.RepeatFrequency > 0 && model.EndDate != null)
            {
                var endOfInterval = model.EndDate;
                if (model.RepeatFrequency == (long)RepeatFrequency.Custom)
                    Save(model);


                if (model.RepeatFrequency == (long)RepeatFrequency.Daily)
                {
                    startDateTimeStrings = vacationInfo.StartDateTimeString;
                    endDateTimeStrings = vacationInfo.EndDateTimeString;
                    int daysDiff = ((TimeSpan)(endOfInterval - vacationInfo.StartDate)).Days;
                    if (daysDiff > 0)
                    {
                        startDateTimeStrings = vacationInfo.StartDateTimeString;
                        endDateTimeStrings = vacationInfo.EndDateTimeString;
                        for (DateTime i = vacationInfo.StartDate.AddDays(1); i <= endOfInterval; i = i.AddDays(1))
                        {
                            var time = _clock.ToLocal(i.AddDays(-1)).AddDays(1);

                            startDateTimeStrings = startDateTimeStrings.AddDays(1);
                            endDateTimeStrings = endDateTimeStrings.AddDays(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            Save(model);
                        }
                    }
                    else
                    {
                        startDateTimeStrings = vacationInfo.StartDateTimeString;
                        endDateTimeStrings = vacationInfo.EndDateTimeString;
                        for (DateTime i = vacationInfo.StartDate.AddDays(-1); i >= endOfInterval; i = i.AddDays(-1))
                        {
                            var time = _clock.ToLocal(i.AddDays(-1)).AddDays(1);

                            startDateTimeStrings = startDateTimeStrings.AddDays(1);
                            endDateTimeStrings = endDateTimeStrings.AddDays(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = (i);
                            model.EndDate = i.AddMinutes(interval);
                            Save(model);
                            model.EndDate = i.AddMinutes(-interval);
                        }
                    }
                }

                if (model.RepeatFrequency == (long)RepeatFrequency.Weekly)
                {
                    startDateTimeStrings = vacationInfo.StartDateTimeString;
                    endDateTimeStrings = vacationInfo.EndDateTimeString;
                    int daysDiff = ((TimeSpan)(endOfInterval - vacationInfo.StartDate)).Days;
                    if (daysDiff > 0)
                    {
                        for (DateTime i = vacationInfo.StartDate.AddDays(7); i <= endOfInterval; i = i.AddDays(7))
                        {
                            var time = _clock.ToLocal(i.AddDays(-7)).AddDays(7);

                            startDateTimeStrings = startDateTimeStrings.AddDays(7);
                            endDateTimeStrings = endDateTimeStrings.AddDays(7);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            Save(model);
                        }
                    }
                    else
                    {
                        startDateTimeStrings = vacationInfo.StartDateTimeString;
                        endDateTimeStrings = vacationInfo.EndDateTimeString;
                        for (DateTime i = vacationInfo.StartDate.AddDays(-7); i >= endOfInterval; i = i.AddDays(-7))
                        {

                            var time = _clock.ToLocal(i.AddDays(-7)).AddDays(7);

                            startDateTimeStrings = startDateTimeStrings.AddDays(7);
                            endDateTimeStrings = endDateTimeStrings.AddDays(7);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            Save(model);
                            model.EndDate = i.AddMinutes(-interval);
                        }
                    }
                }

                if (model.RepeatFrequency == (long)RepeatFrequency.Monthly)
                {
                    int daysDiff = ((TimeSpan)(endOfInterval - vacationInfo.StartDate)).Days;
                    if (daysDiff > 0)
                    {
                        startDateTimeStrings = vacationInfo.StartDateTimeString;
                        endDateTimeStrings = vacationInfo.EndDateTimeString;
                        for (DateTime i = vacationInfo.StartDate.AddMonths(1); i <= endOfInterval; i = i.AddMonths(1))
                        {
                            var time = _clock.ToLocal(i.AddMonths(-1)).AddMonths(1);

                            startDateTimeStrings = startDateTimeStrings.AddMonths(1);
                            endDateTimeStrings = endDateTimeStrings.AddMonths(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            if (vacationInfo.StartDate.Month == 2 && vacationInfo.StartDate.Day == 28)
                            {
                                var lastDay = DateTime.DaysInMonth(i.Year, i.Month);
                                i = new DateTime(i.Year, i.Month, lastDay, tim2e.Hours, tim2e.Minutes, tim2e.Seconds);

                            }
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            Save(model);

                        }
                    }
                    else
                    {
                        startDateTimeStrings = vacationInfo.StartDateTimeString;
                        endDateTimeStrings = vacationInfo.EndDateTimeString;
                        for (DateTime i = vacationInfo.StartDate.AddMonths(-1); i >= endOfInterval; i = i.AddMonths(-1))
                        {
                            var time = _clock.ToLocal(i.AddMonths(-1)).AddMonths(1);

                            startDateTimeStrings = startDateTimeStrings.AddMonths(1);
                            endDateTimeStrings = endDateTimeStrings.AddMonths(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            if (vacationInfo.StartDate.Month == 2 && vacationInfo.StartDate.Day == 28)
                            {
                                var lastDay = DateTime.DaysInMonth(i.Year, i.Month);
                                i = new DateTime(i.Year, i.Month, lastDay, tim2e.Hours, tim2e.Minutes, tim2e.Seconds);

                            }
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            Save(model);
                            model.EndDate = i.AddMinutes(-interval);
                        }
                    }
                }

            }
        }
        public void RepeatMeeting(VacationRepeatEditModel model)
        {
            var startDateTimeStrings = default(DateTime);
            var endDateTimeStrings = default(DateTime);
            var meetingInfo = _jobSchedulerRepository.Get(x => !x.IsVacation && x.Id == model.VacationId);
            if (meetingInfo == null)
                return;

            var interval = (meetingInfo.EndDate - meetingInfo.StartDate).TotalMinutes;
            model.Title = meetingInfo.Title;
            if (model.RepeatFrequency > 0 && model.EndDate != null)
            {
                var endOfInterval = model.EndDate;
                if (model.RepeatFrequency == (long)RepeatFrequency.Custom)
                {
                    RepearMeeting(model);
                }


                if (model.RepeatFrequency == (long)RepeatFrequency.Daily)
                {
                    int daysDiff = ((TimeSpan)(endOfInterval - meetingInfo.StartDate)).Days;
                    if (daysDiff > 0)
                    {
                        startDateTimeStrings = meetingInfo.StartDateTimeString;
                        endDateTimeStrings = meetingInfo.EndDateTimeString;

                        for (DateTime i = meetingInfo.StartDate.AddDays(1); i <= endOfInterval; i = i.AddDays(1))
                        {

                            var time = _clock.ToLocal(i.AddDays(-1)).AddDays(1);

                            startDateTimeStrings = startDateTimeStrings.AddDays(1);
                            endDateTimeStrings = endDateTimeStrings.AddDays(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = (i);
                            model.EndDate = i.AddMinutes(interval);
                            RepearMeeting(model);
                        }
                    }
                    else
                    {
                        startDateTimeStrings = meetingInfo.StartDateTimeString;
                        endDateTimeStrings = meetingInfo.EndDateTimeString;
                        for (DateTime i = meetingInfo.StartDate.AddDays(-1); i >= endOfInterval; i = i.AddDays(-1))
                        {
                            var time = _clock.ToLocal(i.AddDays(-1)).AddDays(1);

                            startDateTimeStrings = startDateTimeStrings.AddDays(1);
                            endDateTimeStrings = endDateTimeStrings.AddDays(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = (i);
                            model.EndDate = i.AddMinutes(interval);
                            RepearMeeting(model);
                            model.EndDate = i.AddMinutes(-interval);
                        }
                    }
                }
                if (model.RepeatFrequency == (long)RepeatFrequency.Weekly)
                {
                    int daysDiff = ((TimeSpan)(endOfInterval - meetingInfo.StartDate)).Days;
                    if (daysDiff > 0)
                    {
                        startDateTimeStrings = meetingInfo.StartDateTimeString;
                        endDateTimeStrings = meetingInfo.EndDateTimeString;
                        for (DateTime i = meetingInfo.StartDate.AddDays(7); i <= endOfInterval; i = i.AddDays(7))
                        {
                            var time = _clock.ToLocal(i.AddDays(-7)).AddDays(7);

                            startDateTimeStrings = startDateTimeStrings.AddDays(7);
                            endDateTimeStrings = endDateTimeStrings.AddDays(7);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            RepearMeeting(model);
                        }
                    }
                    else
                    {
                        startDateTimeStrings = meetingInfo.StartDateTimeString;
                        endDateTimeStrings = meetingInfo.EndDateTimeString;
                        for (DateTime i = meetingInfo.StartDate.AddDays(-7); i >= endOfInterval; i = i.AddDays(-7))
                        {
                            var time = _clock.ToLocal(i.AddDays(-7)).AddDays(7);

                            startDateTimeStrings = startDateTimeStrings.AddDays(7);
                            endDateTimeStrings = endDateTimeStrings.AddDays(7);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            RepearMeeting(model);
                            model.EndDate = i.AddMinutes(-interval);
                        }
                    }
                }
                if (model.RepeatFrequency == (long)RepeatFrequency.Monthly)
                {
                    int daysDiff = ((TimeSpan)(endOfInterval - meetingInfo.StartDate)).Days;
                    if (daysDiff > 0)
                    {
                        startDateTimeStrings = meetingInfo.StartDateTimeString;
                        endDateTimeStrings = meetingInfo.EndDateTimeString;
                        for (DateTime i = meetingInfo.StartDate.AddMonths(1); i <= endOfInterval; i = i.AddMonths(1))
                        {

                            var time = _clock.ToLocal(i.AddMonths(-1)).AddMonths(1);

                            startDateTimeStrings = startDateTimeStrings.AddMonths(1);
                            endDateTimeStrings = endDateTimeStrings.AddMonths(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            if (meetingInfo.StartDate.Month == 2 && meetingInfo.StartDate.Day == 28)
                            {
                                var lastDay = DateTime.DaysInMonth(i.Year, i.Month);
                                i = new DateTime(i.Year, i.Month, lastDay, tim2e.Hours, tim2e.Minutes, tim2e.Seconds);

                            }

                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            RepearMeeting(model);

                        }

                    }
                    else
                    {
                        startDateTimeStrings = meetingInfo.StartDateTimeString;
                        endDateTimeStrings = meetingInfo.EndDateTimeString;
                        for (DateTime i = meetingInfo.StartDate.AddMonths(-1); i >= endOfInterval; i = i.AddMonths(-1))
                        {
                            var time = _clock.ToLocal(i.AddMonths(-1)).AddMonths(1);

                            startDateTimeStrings = startDateTimeStrings.AddMonths(1);
                            endDateTimeStrings = endDateTimeStrings.AddMonths(1);
                            model.ActualStartDate = startDateTimeStrings;
                            model.ActualEndDate = endDateTimeStrings;
                            var time2 = model.ActualStartDate.TimeOfDay;
                            var time3 = model.ActualEndDate.TimeOfDay;
                            model.ActualStartDate = model.ActualStartDate.Date + new TimeSpan(time2.Ticks);
                            model.ActualEndDate = model.ActualEndDate.Date + new TimeSpan(time3.Ticks);

                            time = _clock.ToUtc(time);
                            var tim2e = time.TimeOfDay;
                            i = i.Date + new TimeSpan(tim2e.Ticks);
                            if (meetingInfo.StartDate.Month == 2 && meetingInfo.StartDate.Day == 28)
                            {
                                var lastDay = DateTime.DaysInMonth(i.Year, i.Month);
                                i = new DateTime(i.Year, i.Month, lastDay, tim2e.Hours, tim2e.Minutes, tim2e.Seconds);

                            }
                            model.StartDate = i;
                            model.EndDate = i.AddMinutes(interval);
                            RepearMeeting(model);
                            model.EndDate = i.AddMinutes(-interval);

                        }
                    }
                }
            }
        }

        public void RepeatMeetingForActualTime(VacationRepeatEditModel model)
        {

        }
        public long SaveMeeting(VacationRepeatEditModel model)
        {
            var jobInfo = _jobSchedulerRepository.Get(x => x.Id == model.VacationId);

            if (jobInfo != null)
            {
                var scheduler = _jobFactory.CreatMeetingModel(model);
                scheduler.ParentId = model.ParentId;
                scheduler.Title = jobInfo.Title;
                scheduler.StartDate = jobInfo.StartDate;
                scheduler.EndDate = jobInfo.EndDate;
                scheduler.StartDateTimeString = jobInfo.StartDateTimeString;
                scheduler.EndDateTimeString = jobInfo.EndDateTimeString;
                scheduler.Offset = Double.Parse(_clock.BrowserTimeZone);
                _meetingRepository.Save(scheduler);
                return scheduler.Id;
            }
            return 0;
        }

        private void Save(VacationRepeatEditModel model)
        {
            var userId = _organizationRoleUserRepository.Table.Where(x => x.UserId == model.AssigneeId && x.OrganizationId == model.FranchiseeId).Select(x => x.Id).FirstOrDefault();
            var domain = _jobFactory.CreateDomain(model);
            domain.AssigneeId = userId;
            domain.IsCancellationMailSend = true;
            _jobSchedulerRepository.Save(domain);
            _unitOfWork.SaveChanges();
        }
        private void RepearMeeting(VacationRepeatEditModel model)
        {
            var person = model.AssigneeId != null ? _organizationRoleUserRepository.Get(model.AssigneeId) : null;

            var domain = _jobFactory.CreateRepearMeetingDomain(model);
            domain.PersonId = person != null ? person.UserId : default(long?);
            domain.IsCancellationMailSend = true;
            _jobSchedulerRepository.Save(domain);
            _unitOfWork.SaveChanges();
        }
        public List<long> GetUserIdsByMeeting(long meetingId)
        {
            var parentId = _meetingRepository.Table.Where(x => x.Id == meetingId).Select(x => x.ParentId).FirstOrDefault();
            if (parentId != null)
            {
                var id = _jobSchedulerRepository.Table.Where(x => x.MeetingID == parentId && x.IsActive).Select(y => y.AssigneeId.Value).Distinct().ToList();
                return id;
            }
            else
            {
                var id = _jobSchedulerRepository.Table.Where(x => x.MeetingID == meetingId && x.IsActive).Select(y => y.AssigneeId.Value).Distinct().ToList();
                return id;
            }
        }

        public long GetParentMeetingId(long meetingId)
        {
            var parentid = _meetingRepository.Table.Where(x => x.Id == meetingId).Select(x => x.ParentId).FirstOrDefault();
            if (parentid == null)
            {
                return meetingId;
            }
            return parentid.Value;
        }

        public bool DeleteMeeting(long id, long techId)
        {
            if (id > 0)
            {
                var meetingId = default(long?);
                var parentMeetingId = default(long?);
                var jobSchedulerItem = _jobSchedulerRepository.IncludeMultiple(x => x.Meeting, x => x.Meeting.Parent).FirstOrDefault(x => x.Id == id && x.Meeting != null);
                //var jobSchedulerItem = _jobSchedulerRepository.Get(id);
                if (jobSchedulerItem != null)
                {
                    if (jobSchedulerItem.Meeting.Parent != null)
                    {
                        meetingId = jobSchedulerItem.Meeting.Parent.Id;
                        parentMeetingId = jobSchedulerItem.Meeting.Parent.Id;
                    }
                    else if (jobSchedulerItem.Meeting != null)
                    {
                        meetingId = jobSchedulerItem.Meeting.Id;
                    }

                    if (parentMeetingId == default(long?))
                    {
                        var allMeetingToDelete = new List<long>();
                        var childMeetings = _meetingRepository.Table.Where(x => x.ParentId == meetingId).Select(x => x.Id).ToList();
                        allMeetingToDelete.AddRange(childMeetings);
                        allMeetingToDelete.Add(meetingId.GetValueOrDefault());

                        var schedulers = _jobSchedulerRepository.IncludeMultiple(x => x.Meeting).Where(x => x.MeetingID != null && allMeetingToDelete.Contains(x.MeetingID.Value)).ToList();
                        foreach (var jobScheduler in schedulers)
                        {
                            if (jobScheduler.Meeting != null)
                            {
                                _meetingRepository.Delete(jobScheduler.Meeting);
                            }
                            _jobSchedulerRepository.Delete(jobScheduler);
                        }
                    }
                    else
                    {
                        var schedulers = _jobSchedulerRepository.IncludeMultiple(x => x.Meeting).Where(x => x.Id == id && x.AssigneeId == techId && x.IsActive).ToList();
                        var franchiseeId = schedulers.FirstOrDefault().FranchiseeId;
                        foreach (var jobScheduler in schedulers)
                        {
                            var assigneeId = jobScheduler.AssigneeId.GetValueOrDefault();
                            if (jobScheduler.Meeting != null)
                            {
                                var orgInfo = _organizationRoleUserRepository.Get(assigneeId);
                                var userIds = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(orgInfo.UserId, franchiseeId);
                                //foreach (var userId in userIds)
                                //{
                                //var jobSchedulerForUser = _jobschedulerRepository.Table.Where(x => x.MeetingID == jobSchedulerItem.MeetingID && x.PersonId == jobScheduler.PersonId && x.IsActive).FirstOrDefault();
                                //jobScheduler.AssigneeId = userId;
                                _jobSchedulerRepository.Delete(jobScheduler);
                                //}
                            }

                        }
                    }
                    return true;
                }

            }
            return false;
        }
        public long SaveMeetingForUser(JobEstimateEditModel model)
        {
            var isNew = model.SchedulerId == 0 ? true : false;
            var assigneeId = _organizationRoleUserRepository.Table.Where(x => x.UserId == model.AssigneeId && x.OrganizationId == model.FranchiseeId && x.IsActive).Select(x => x.Id).FirstOrDefault();
            model.AssigneeId = assigneeId;
            var scheduler = _jobFactory.CreateMeetingDomain(model);
            scheduler.IsCancellationMailSend = true;
            model.Id = scheduler.Id;
            _jobSchedulerRepository.Save(scheduler);
            return scheduler.Id;

        }
        public void SaveMeetingForUserById(JobScheduler model)
        {
            model.IsCancellationMailSend = true;
            //var scheduler = _jobFactory.CreateMeetingDomain(model);
            _jobSchedulerRepository.Save(model);
        }
        public bool EditMeeting(JobEstimateEditModel model)
        {
            var jobScheduler = _jobSchedulerRepository.Get(model.Id);
            var oldStartDateTime = _jobSchedulerRepository.Table.Where(x => x.MeetingID == model.MeetingID && x.StartDate == jobScheduler.StartDate && x.EndDate == jobScheduler.EndDate).Select(x => x.StartDate).FirstOrDefault();
            var oldTitle = _jobSchedulerRepository.Table.Where(x => x.MeetingID == model.MeetingID && x.StartDate == jobScheduler.StartDate && x.EndDate == jobScheduler.EndDate).Select(x => x.Title).FirstOrDefault();
            var oldEndDateTime = _jobSchedulerRepository.Table.Where(x => x.MeetingID == model.MeetingID && x.StartDate == jobScheduler.StartDate && x.EndDate == jobScheduler.EndDate).Select(x => x.EndDate).FirstOrDefault();
            var currentDay = _clock.UtcNow.AddDays(-1);
            var existingUserIds = _jobSchedulerRepository.Table.Where(x => x.MeetingID == model.MeetingID && x.IsActive && x.StartDate == jobScheduler.StartDate && x.EndDate == jobScheduler.EndDate)
                                                        .Select(x => x.OrganizationRoleUser.UserId).Distinct().ToList();
            var allUserIds = model.idList;
            var newUserIds = allUserIds.Where(p => existingUserIds.All(p2 => p2 != p)).ToList();
            var updatingAssets = allUserIds.Where(p => existingUserIds.Any(p2 => p2 == p)).ToList();
            var toDeleteCoDriverIds = existingUserIds.Where(p => allUserIds.All(p2 => p2 != p)).ToList();
            var meetings = new List<JobScheduler>();
            var parentId = default(long?);
            // for adding new users
            if (newUserIds.Count > 0)
            {
                var childParentIds = new List<long>();
                foreach (var userid in newUserIds)
                {
                    var userIds = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(userid, model.FranchiseeId);
                    var meeting = _jobschedulerRepository.Table.Where(x => (x.MeetingID == model.MeetingID && x.StartDate == jobScheduler.StartDate && x.EndDate == jobScheduler.EndDate && x.IsActive)).FirstOrDefault();
                    var orgRoleUserId = userIds.FirstOrDefault();
                    meeting.AssigneeId = orgRoleUserId;
                    meeting.IsNew = true;
                    meeting.StartDate = model.StartDate;
                    meeting.EndDate = model.EndDate;
                    meeting.Title = model.Title;
                    meeting.PersonId = userid;
                    SaveMeetingForUserById(meeting);
                }
            }
            if (toDeleteCoDriverIds.Count > 0)
            {
                meetings = _jobschedulerRepository.Table.AsEnumerable().Where(x => (x.MeetingID == model.MeetingID && x.StartDate == jobScheduler.StartDate && x.EndDate == jobScheduler.EndDate && x.IsActive)).ToList();
                foreach (var userid in toDeleteCoDriverIds)
                {
                    var userIds = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(userid, model.FranchiseeId);
                    DeleteMeetingById(model.MeetingID.GetValueOrDefault(), userIds.FirstOrDefault(), meetings.FirstOrDefault());
                }
            }
            if (oldTitle != model.Title)
            {
                meetings = _jobschedulerRepository.Table.AsEnumerable().Where(x => (x.MeetingID == model.MeetingID && x.IsActive && x.StartDate == model.StartDate && x.EndDate == model.EndDate)).ToList();
                foreach (var meeting in meetings)
                {
                    meeting.Title = model.Title;
                    meeting.IsNew = false;
                    meeting.IsCancellationMailSend = true;
                    _jobSchedulerRepository.Save(meeting);
                }
            }
            if (oldStartDateTime != model.StartDate || oldEndDateTime != model.EndDate)
            {
                var childParentIds = new List<long>();
                parentId = _meetingRepository.Table.Where(x => x.Id == model.MeetingID).Select(x => x.ParentId).FirstOrDefault();
                var job = _jobschedulerRepository.Table.AsEnumerable().Where(x => (x.Id == model.Id && x.IsActive)).FirstOrDefault();
                meetings = _jobschedulerRepository.Table.AsEnumerable().Where(x => (x.MeetingID == model.MeetingID && x.IsActive && x.StartDate == job.StartDate && x.EndDate == job.EndDate)).ToList();
                foreach (var meetingInfo in meetings)
                {
                    var userIds = _organizationRoleUserInfoService.GetOrgUserIdsByOrgUserId(meetingInfo.AssigneeId.GetValueOrDefault(), meetingInfo.OrganizationRoleUser.Organization.Id);
                    TimeSpan timeOfDayForStart = model.StartDate.TimeOfDay;
                    TimeSpan timeOfDayForEnd = model.EndDate.TimeOfDay;
                    TimeSpan startTimeSpan = new TimeSpan(0, timeOfDayForStart.Hours, timeOfDayForStart.Minutes, timeOfDayForStart.Seconds, timeOfDayForStart.Milliseconds);
                    TimeSpan endTimeSpan = new TimeSpan(0, timeOfDayForEnd.Hours, timeOfDayForEnd.Minutes, timeOfDayForEnd.Seconds, timeOfDayForEnd.Milliseconds);
                    meetingInfo.StartDate = model.StartDate.Date + startTimeSpan;
                    meetingInfo.EndDate = model.EndDate.Date + endTimeSpan;
                    meetingInfo.StartDateTimeString = model.ActualStartDateString;
                    meetingInfo.EndDateTimeString = model.ActualEndDateString;
                    meetingInfo.Title = model.Title;
                    meetingInfo.IsNew = false;
                    meetingInfo.IsCancellationMailSend = true;
                    _jobSchedulerRepository.Save(meetingInfo);
                }
            }
            return true;
        }

        public void DeleteMeetingById(long meetingId, long assigneeId, JobScheduler jobScheduler)
        {
            var domain = _jobSchedulerRepository.Table.Where(x => x.MeetingID == meetingId && x.AssigneeId == assigneeId && x.IsActive && x.StartDate == jobScheduler.StartDate && x.EndDate == jobScheduler.EndDate).FirstOrDefault();
            if (domain != null)
            {
                domain.IsActive = false;
                domain.IsCancellationMailSend = true;
                _jobSchedulerRepository.Save(domain);
            }
        }

        private void sendMailToNewTech(List<long> addedTechList, IEnumerable<JobOccurenceEditModel> jobOccurance, JobEditModel model)
        {

            DateTime currentUtcDate = _clock.ToLocal(_clock.UtcNow, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime futureUtcDate = _clock.ToLocal(_clock.UtcNow.AddDays(1), _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseCurrentUtcDate = _clock.ToLocal(model.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            DateTime databaseFutureUtcDate = _clock.ToLocal(model.StartDate, _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes);
            var addedNewTechList = jobOccurance.Where(x => x.ScheduleId == 0).ToList();
            foreach (var jobScheduler in addedNewTechList)
            {
                if (databaseCurrentUtcDate.Date >= currentUtcDate.Date && databaseFutureUtcDate.Date <= futureUtcDate.Date)
                {
                    if (databaseCurrentUtcDate.Date == currentUtcDate)
                    {
                        model.dateType = "TODAY";
                    }
                    else
                    {
                        model.dateType = "TOMORROW";
                    }
                    var userId = _organizationRoleUserRepository.Table.Where(x => x.Id == jobScheduler.AssigneeId && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                    var isLocked = _userLoginRepository.Table.Where(x => x.Id == userId).Select(x => x.IsLocked).FirstOrDefault();
                    if (!isLocked)
                    {

                        if (jobScheduler != null)
                        {
                            model.ActualStartDateString = jobScheduler.ActualStartDateString;
                            model.ActualEndDateString = jobScheduler.ActualEndDateString;
                        }
                        model.jobTypeName = "Job";
                        SendingUrgentJobMails(model, jobScheduler.AssigneeId);
                    }
                }
                else
                {
                    var userId = _organizationRoleUserRepository.Table.Where(x => x.Id == jobScheduler.AssigneeId && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                    var isLocked = _userLoginRepository.Table.Where(x => x.Id == userId).Select(x => x.IsLocked).FirstOrDefault();
                    if (!isLocked)
                    {
                        if (jobScheduler != null)
                        {
                            model.StartDate = jobScheduler.ActualStartDateString;
                            model.EndDate = jobScheduler.ActualEndDateString;
                        }
                        model.jobTypeName = "Job";
                        SendingNewsJobMails(model, jobScheduler.AssigneeId);
                    }
                }
            }
        }

        private void sendMailToResheculeTech(List<long> reScheduleTectList, IEnumerable<JobOccurenceEditModel> jobOccurance, JobEditModel model)
        {
            foreach (var items in reScheduleTectList)
            {
                var userId = _organizationRoleUserRepository.Table.Where(x => x.Id == items && x.OrganizationId == model.FranchiseeId).Select(x => x.UserId).FirstOrDefault();
                var isLocked = _userLoginRepository.Table.Where(x => x.Id == userId).Select(x => x.IsLocked).FirstOrDefault();
                //var jobScheduler = jobOccurance.Where(x => x.AssigneeId == items).FirstOrDefault();
                var jobScheduler = _jobschedulerRepository.Table.Where(x => x.AssigneeId == items && x.JobId == model.JobId && !x.IsActive).FirstOrDefault();
                if (jobScheduler != null)
                {
                    model.StartDate = jobScheduler.StartDateTimeString;
                    model.EndDate = jobScheduler.EndDateTimeString;
                }
                if (!isLocked)
                {
                    model.jobTypeName = "Job";
                    SendingRescheduledJobMails(model, items);
                }
            }
        }

        private void sendMailToDeleteTech(List<long> deletedTechList, IEnumerable<JobOccurenceEditModel> jobOccurance, JobEditModel jobEditModel, List<JobScheduler> jobTiming)
        {
            foreach (var assigneeId in deletedTechList)
            {
                if (jobTiming.Count() > 0 && jobTiming.Any(x => x.AssigneeId == assigneeId))
                {
                    jobEditModel.ActualStartDateString = jobTiming.Where(x => x.AssigneeId == assigneeId).Select(x => x.StartDateTimeString).FirstOrDefault();
                    jobEditModel.ActualEndDateString = jobTiming.Where(x => x.AssigneeId == assigneeId).Select(x => x.EndDateTimeString).FirstOrDefault();
                    jobEditModel.SchedulerId = jobTiming.Where(x => x.AssigneeId == assigneeId).Select(x => x.Id).FirstOrDefault();
                }
                var currentDate = DateTime.Now.Date;
                var startDateToBeCompared = jobEditModel.StartDate.Date;
                var endDateToBeCompared = jobEditModel.EndDate.Date;
                if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                    SendingCancellationJobMails(jobEditModel, assigneeId);
            }
        }

        public void SendingJobUpdationMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items && x.IsActive)).FirstOrDefault();
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.ActualStartDateString.Date;
            var endDateToBeCompared = model.ActualEndDateString.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForUpdation(model, listSchedule);
        }
        private void SendingCancellationJobMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelled(model, listSchedule);
        }

        private void SendingRescheduledJobMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForRescheduled(model, listSchedule);
        }

        private void SendingUrgentJobMails(JobEditModel model, long items)
        {

            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            model.jobTypeName = "Job";
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTechForUrgent(model, listSchedule);
        }
        private void SendingNewsJobMails(JobEditModel model, long items)
        {
            var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == items)).FirstOrDefault();
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = model.StartDate.Date;
            var endDateToBeCompared = model.EndDate.Date;
            model.jobTypeName = "Job";
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _sendNewJobNotificationToTechService.SendJobNotificationtoTech(model, listSchedule);

        }

        public bool CheckCurrentEstimateDeletion(JobOccurenceListModel model)
        {
            var assigneeList = model.Collection;
            var scheduleInfo = _jobSchedulerRepository.Table.Where(x => x.EstimateId == model.ParentEstimateId).FirstOrDefault();
            var estimateIds = getParentEstimateIds(scheduleInfo);
            var parentEstimateId = assigneeList.Select(x => x.ParentEstimateId).ToList();
            if (!parentEstimateId.Contains(model.ParentEstimateId))
            {
                return true;
            }
            return false;
        }

        public bool EditMeetingForEquipment(JobEstimateEditModel model)
        {
            var scheduler = _jobSchedulerRepository.Get(model.Id);
            if (scheduler != null)
            {
                var meeting = _meetingRepository.Get(scheduler.MeetingID.GetValueOrDefault());
                if (meeting != null)
                {
                    meeting.IsEquipment = model.IsEquipment.GetValueOrDefault();
                    _meetingRepository.Save(meeting);
                }
                return true;
            }
            return false;

        }

        private void SavingJobDetails(JobScheduler scheduler, JobEstimateEditModel model, long? schedulerId)
        {
            var jobScheduler = _jobschedulerRepository.Table.FirstOrDefault(x => x.Id == schedulerId);
            var jobEstimateList = new List<JobEstimate>();
            var jobEstimateDifferenceList = new List<JobEstimate>();
            var jobDetails = _jobDetailsRepository.Table.FirstOrDefault(x => x.EstimateId == scheduler.EstimateId);
            if (jobDetails != null)
            {
                if (!model.IsDataToBeUpdateForAllJobs.GetValueOrDefault())
                {
                    if (scheduler.Id == model.SchedulerId)
                    {
                        jobDetails.Description = model.Description;
                        _jobDetailsRepository.Save(jobDetails);

                    }
                }
                else
                {
                    var jobEstimateDomain = _jobEstimateRepository.Table.FirstOrDefault(x => x.ParentEstimateId == model.Id || x.Id == model.Id);

                    if (jobEstimateDomain != null)
                    {
                        if (jobEstimateDomain.ParentEstimateId == null)
                        {
                            jobEstimateList = _jobEstimateRepository.Table.Where(x => x.ParentEstimateId == jobEstimateDomain.Id || x.Id == jobEstimateDomain.Id).ToList();

                        }
                        else
                        {
                            jobEstimateList = _jobEstimateRepository.Table.Where(x => x.ParentEstimateId == jobEstimateDomain.ParentEstimateId || x.Id == jobEstimateDomain.ParentEstimateId).ToList();

                        }
                    }
                    if (jobEstimateList.Count() > 0)
                    {
                        foreach (var jobEstimate in jobEstimateList)
                        {
                            var jobDetailsDomainList = _jobEstimateRepository.Table.Where(x => x.ParentEstimateId == jobEstimate.Id || x.Id == jobEstimate.Id).ToList();
                            var jobEstimateLocalDifferenceList = GetDifferectList(jobDetailsDomainList, jobEstimateList);
                            if (jobEstimateLocalDifferenceList.Count() > 0)
                            {
                                jobEstimateDifferenceList.AddRange(jobEstimateLocalDifferenceList);
                            }

                            var jobDetailsDomain = _jobDetailsRepository.Table.FirstOrDefault(x => x.EstimateId == jobEstimate.Id);
                            if (jobEstimate.Id == jobScheduler.EstimateId)
                            {
                                jobDetailsDomain.Description = model.Description;
                            }
                            else
                            {
                                jobDetailsDomain.Description += " " + model.Description;
                            }
                            _jobDetailsRepository.Save(jobDetailsDomain);
                        }
                        //if (jobEstimateDifferenceList.Count() > 0)
                        //{
                        //    foreach (var jobEstimate in jobEstimateDifferenceList)
                        //    {
                        //        var jobDetailsDomain = _jobDetailsRepository.Table.FirstOrDefault(x => x.EstimateId == jobEstimate.Id);
                        //        jobDetailsDomain.Description += " " + model.Description;
                        //        _jobDetailsRepository.Save(jobDetailsDomain);
                        //    }
                        //}
                    }
                }

            }
            else
            {
                var doamin = _jobDetailsFactory.CreateDomain(model);
                doamin.Description = model.Description;
                doamin.SchedulerId = scheduler.Id;
                doamin.EstimateId = scheduler.EstimateId;
                doamin.IsNew = true;
                doamin.JobId = scheduler.JobId;
                _jobDetailsRepository.Save(doamin);
            }
        }

        private void SavingJobDetails(JobScheduler scheduler, string description)
        {
            var jobDetails = _jobDetailsRepository.Table.FirstOrDefault(x => x.EstimateId == scheduler.JobId);
            if (jobDetails == null)
            {
                var doamin = _jobDetailsFactory.CreateDomain(scheduler.Job);
                if (description != null)
                {
                    doamin.Description = description;
                }
                doamin.SchedulerId = scheduler.Id;
                doamin.IsNew = true;
                _jobDetailsRepository.Save(doamin);
            }
        }

        private void SavingJobDetails(JobScheduler scheduler)
        {
            var jobDetails = _jobDetailsRepository.Table.FirstOrDefault(x => x.EstimateId == scheduler.EstimateId);
            if (jobDetails == null)
            {
                var estimateDomain = _jobEstimateRepository.Table.FirstOrDefault(x => x.Id == scheduler.EstimateId);
                var doamin = _jobDetailsFactory.CreateDomain(estimateDomain);
                doamin.SchedulerId = scheduler.Id;
                doamin.IsNew = true;
                _jobDetailsRepository.Save(doamin);
            }
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
            if (schedulerId == null)
            {
                return;
            }
            var scheduler = _jobschedulerRepository.IncludeMultiple(x => x.Franchisee).FirstOrDefault(x => x.Id == schedulerId);
            var currentDate = DateTime.Now.Date;
            var startDateToBeCompared = scheduler.StartDateTimeString.Date;
            var endDateToBeCompared = scheduler.EndDateTimeString.Date;
            var startDate = _clock.UtcNow.Date.AddDays(1);
            var endDate = _clock.UtcNow.Date.AddDays(2);
            if (currentDate <= startDateToBeCompared && currentDate <= endDateToBeCompared)
                _userNotificationModelFactory.ScheduleReminderNotification(scheduler, startDate, endDate, "", NotificationTypes.UpdateCustomerMail);
        }

        private bool IsValueChanged(List<JobScheduler> oldSchedulerValue, List<JobOccurenceEditModel> newSchedulerValue, out long? schedulerIdRepeated)
        {
            if (oldSchedulerValue.Count() != newSchedulerValue.Count())
            {
                schedulerIdRepeated = default(long);
                return true;
            }
            foreach (var scheduler in oldSchedulerValue)
            {
                var newScheduler = newSchedulerValue.FirstOrDefault(x => x.ScheduleId == scheduler.Id);
                if (newScheduler == null) continue;
                if (newScheduler.AssigneeId == scheduler.AssigneeId && newScheduler.EndDate == scheduler.EndDate
                    && newScheduler.StartDate == scheduler.StartDate)
                {
                    continue;
                }
                else
                {
                    schedulerIdRepeated = scheduler.Id;
                    return true;
                }
            }
            schedulerIdRepeated = default(long?);
            return false;
        }

        private bool IsUpdatedForTimeOrAssigneeId(List<JobScheduler> oldModel, List<JobScheduler> schedulerList, out long? schedulerId)
        {
            schedulerId = default(long?);
            var isUpdated = false;
            var todayDate = DateTime.UtcNow;
            var tomorrowDate = DateTime.UtcNow.AddDays(1);
            var schedulerNextDayList = schedulerList.Where(x => x.StartDate >= todayDate && x.StartDate <= tomorrowDate).ToList();

            if (schedulerNextDayList.Count() == 0)
            {
                var idList = schedulerList.Select(x => x.Id).ToList();
                var schedulerDeleted = oldModel.Where(x => !idList.Contains(x.Id)).ToList();
                foreach (var scheduler in schedulerDeleted)
                {
                    if (scheduler.StartDate >= todayDate && scheduler.EndDate <= tomorrowDate)
                    {
                        schedulerId = scheduler.Id;
                        isUpdated = true;
                        return true;
                    }
                    else
                        continue;
                }
                return false;
            }
            else if (schedulerList.Count() >= oldModel.Count())
            {
                foreach (var schedulerNextDay in schedulerNextDayList)
                {
                    var oldModelScheduler = oldModel.FirstOrDefault(x => x.Id == schedulerNextDay.Id);
                    if (oldModelScheduler == null)
                    {
                        if (schedulerNextDay.StartDate >= todayDate && schedulerNextDay.EndDate <= tomorrowDate)
                        {
                            schedulerId = schedulerNextDay.Id;
                            isUpdated = true;
                            return true;
                        }
                        else
                            continue;
                    }
                    else
                    {
                        if (schedulerNextDay.StartDateTimeString == oldModelScheduler.StartDateTimeString && schedulerNextDay.EndDateTimeString == oldModelScheduler.EndDateTimeString
                            && schedulerNextDay.AssigneeId == oldModelScheduler.AssigneeId)
                        {
                            continue;
                        }
                        else
                        {
                            schedulerId = schedulerNextDay.Id;
                            isUpdated = true;
                            return true;
                        }

                    }
                }
            }
            else if (schedulerList.Count() < oldModel.Count())
            {
                var idList = schedulerList.Select(x => x.Id).ToList();
                var schedulerDeleted = oldModel.Where(x => !idList.Contains(x.Id)).ToList();
                foreach (var scheduler in schedulerDeleted)
                {
                    if (scheduler.StartDate >= todayDate && scheduler.EndDate <= tomorrowDate)
                    {
                        isUpdated = true;
                        return true;
                    }
                    else
                        continue;
                }
            }
            return isUpdated;
        }


        private List<JobEstimate> GetDifferectList(List<JobEstimate> jobDetailsDomainList, List<JobEstimate> jobEstimateList)
        {
            var jobEstimateDifferenceList = new List<JobEstimate>();

            if (jobDetailsDomainList.Count() > 0)
            {
                if (jobDetailsDomainList.Count() > jobDetailsDomainList.Count())
                {
                    jobEstimateDifferenceList = jobDetailsDomainList.Except(jobEstimateList).ToList();
                }
                else
                {
                    jobEstimateDifferenceList = jobEstimateList.Except(jobDetailsDomainList).ToList();
                }
                //jobEstimateList.AddRange(jobEstimateDifferenceList);
            }
            return jobEstimateDifferenceList;
        }
        private bool IsUpdatedForTimeOrAssigneeIdForEstimate(DateTime oldEndDate, DateTime oldStartDate, JobScheduler scheduler)
        {
            var todayDate = DateTime.UtcNow;
            var tomorrowDate = DateTime.UtcNow.AddDays(1);
            if (scheduler.StartDate >= todayDate && scheduler.StartDate <= tomorrowDate)
            {
                if (scheduler.StartDate == oldStartDate && scheduler.EndDate == oldEndDate)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }


        public ShiftChargesViewModel CreateShiftModel(ShiftCharges shiftCharges)
        {
            var model = new ShiftChargesViewModel
            {
                CommercialRestorationShiftPrice = shiftCharges.CommercialRestorationShiftPrice,
                MaintainanceTechNightShiftPrice = shiftCharges.MaintenanceTechNightShiftPrice,
                TechDayShiftPrice = shiftCharges.TechDayShiftPrice
            };
            var shiftChargesViewDropDownModel = new ShiftChargesViewDropDownModel();
            shiftChargesViewDropDownModel.Display = "TECH DAY-SHIFT";
            shiftChargesViewDropDownModel.Value = "TECH DAY-SHIFT";
            model.ShiftChargesViewValues.Add(shiftChargesViewDropDownModel);

            shiftChargesViewDropDownModel = new ShiftChargesViewDropDownModel();
            shiftChargesViewDropDownModel.Display = "COMMERCIAL RESTORATION SHIFT";
            shiftChargesViewDropDownModel.Value = "COMMERCIAL RESTORATION SHIFT";
            model.ShiftChargesViewValues.Add(shiftChargesViewDropDownModel);

            shiftChargesViewDropDownModel = new ShiftChargesViewDropDownModel();
            shiftChargesViewDropDownModel.Display = "MAINTENANCE TECH-NIGHT SHIFT";
            shiftChargesViewDropDownModel.Value = "MAINTENANCE TECH-NIGHT SHIFT";
            model.ShiftChargesViewValues.Add(shiftChargesViewDropDownModel);
            return model;
        }
    }

}
