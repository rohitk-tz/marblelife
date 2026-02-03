using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Notification;
using Core.Notification.Enum;
using Core.Organizations.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class SendNewJobNotificationtoTechService : ISendNewJobNotificationtoTechService
    {

        private readonly IUserNotificationModelFactory _userNotificationModelFactory;
        private readonly IRepository<JobScheduler> _jobschedulerRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Meeting> _meetingRepository;
        private ISettings _settings;
        public SendNewJobNotificationtoTechService(IUserNotificationModelFactory userNotificationModelFactory, IUnitOfWork unitOfWork, ISettings settings)
        {
            _unitOfWork = unitOfWork;
            _userNotificationModelFactory = userNotificationModelFactory;
            _jobschedulerRepository = unitOfWork.Repository<JobScheduler>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _personRepository = unitOfWork.Repository<Person>();
            _meetingRepository = unitOfWork.Repository<Meeting>();
            _settings = settings;
        }

        public bool SendJobNotificationtoTech(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.StartDate.Date;
            var endDate = franchiseeData.EndDate.Date;
            var assigneeId = _jobschedulerRepository.Table.Where(x => x.JobId == franchiseeData.JobId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Job";
            franchiseeData.jobType = "Job";
            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.NewJobOrEstimateReminderNotificationtoTech(franchiseeData, organizationData);
            return true;
        }
        public bool SendJobNotificationtoTechForCancelledForDeleteButton(JobScheduler jobScheduler, bool isFromEstimate)
        {

            var assigneeId = _jobschedulerRepository.Table.Where(x => x.JobId == jobScheduler.JobId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            string AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            string AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            //franchiseeData.jobTypeName = "a Job";
            //franchiseeData.jobType = "Job";
            _userNotificationModelFactory.CancelJobOrEstimateReminderNotificationtoTechForDeleteButton(jobScheduler, AssigneePhone, AssigneeName, isFromEstimate);
            return true;
        }
        public bool SendJobNotificationtoTechForCancelled(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {

            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;
            var assigneeId = _jobschedulerRepository.Table.Where(x => x.JobId == franchiseeData.JobId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Job";
            franchiseeData.jobType = "Job";
            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.CancelJobOrEstimateReminderNotificationtoTech(franchiseeData, organizationData);
            return true;
        }

        public bool SendJobNotificationtoTechForRescheduled(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;
            var assigneeId = _jobschedulerRepository.Table.Where(x => x.JobId == franchiseeData.JobId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Job";
            franchiseeData.jobType = "Job";

            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.NewJobOrEstimateReminderNotificationtoTechForRescheduled(franchiseeData, organizationData);
            return true;
        }
        public bool SendJobNotificationtoTechForRescheduledForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;
            var assigneeId = _jobschedulerRepository.Table.Where(x => x.EstimateId == franchiseeData.Id).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Estimate";
            franchiseeData.JobType = "Estimate";
            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.NewJobOrEstimateReminderNotificationtoTechForRescheduledForEstimate(franchiseeData, organizationData);
            return true;
        }
        public bool SendJobNotificationtoTechForUpdation(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;

            var assigneeId = _jobschedulerRepository.Table.Where(x => x.JobId == franchiseeData.JobId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Job";
            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.UpdateJobOrEstimateReminderNotificationtoTech(franchiseeData, organizationData);
            return true;
        }

        public bool SendJobNotificationtoTechForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;

            var assigneeId = _jobschedulerRepository.Table.Where(x => x.EstimateId == franchiseeData.Id).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Estimate";
            franchiseeData.JobType = "Estimate";
            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.NewJobOrEstimateReminderNotificationtoTechForEstimate(franchiseeData, organizationData);
            return true;
        }

        public bool SendJobNotificationtoTechForCancelledForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;

            var assigneeId = _jobschedulerRepository.Table.Where(x => x.EstimateId == franchiseeData.Id).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Estimate";
            franchiseeData.JobType = "Estimate";
            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.CancelJobOrEstimateReminderNotificationtoTechForEstimate(franchiseeData, organizationData);
            return true;
        }
        public bool SendJobNotificationtoTechForUpdationForEstimate(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;

            var assigneeId = _jobschedulerRepository.Table.Where(x => x.EstimateId == franchiseeData.Id).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Estimate";
            franchiseeData.JobType = "Estimate";

            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.UpdateJobOrEstimateReminderNotificationtoTechForEstimate(franchiseeData, organizationData);
            return true;
        }
        public bool SendJobNotificationtoTechForUrgent(JobEditModel franchiseeData, OrganizationRoleUser organizationData)
        {

            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;
            var assigneeId = _jobschedulerRepository.Table.Where(x => x.JobId == franchiseeData.JobId).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "JOB";
            franchiseeData.jobType = "JOB";
            if (currentDate <= startDate && currentDate <= endDate)
            {
                _userNotificationModelFactory.UrgentJobOrEstimateReminderNotificationtoTech(franchiseeData, organizationData);
            }
            return true;
        }
        public bool SendJobNotificationtoTechForUrgent(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;

            var assigneeId = _jobschedulerRepository.Table.Where(x => x.EstimateId == franchiseeData.Id).Select(x => (x.DataRecorderMetaData.ModifiedBy == null ? x.DataRecorderMetaData.CreatedBy : x.DataRecorderMetaData.ModifiedBy)).FirstOrDefault();
            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == assigneeId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName;
            franchiseeData.jobTypeName = "Estimate";
            franchiseeData.JobType = "Estimate";
            if (currentDate <= startDate && currentDate <= endDate)
                _userNotificationModelFactory.UrgentEstimateReminderNotificationtoTech(franchiseeData, organizationData);
            return true;
        }


        public long? SendBeforeAfterImagestoCustomer(JobCustomer customerData, JobScheduler scheduler, File fileDomain, NotificationTypes notificationId, string emailId)
        {
            var beforeAfterImageMailViewModel = new BeforeAfterImageMailViewModel
            {
                CustomerName = customerData != null ? customerData.CustomerName : "",
                Description = scheduler != null && scheduler.Job != null ? scheduler.Job.Description : "",
                EndDate = scheduler != null ? scheduler.EndDate : default(DateTime?),
                StartDate = scheduler != null ? scheduler.StartDate : default(DateTime?),
                EmailId = emailId != null ? emailId : "",
                CustomerId = customerData != null ? customerData.Id : default(long?),
                FranchiseeId = scheduler != null && scheduler.Franchisee != null ? scheduler.Franchisee.Id : default(long?),
                FromMail = scheduler != null && scheduler.OrganizationRoleUser != null && scheduler.OrganizationRoleUser.Organization != null ? scheduler.OrganizationRoleUser.Organization.Email : "",
                FranchiseeName = scheduler != null && scheduler.OrganizationRoleUser != null && scheduler.OrganizationRoleUser.Organization != null ? scheduler.OrganizationRoleUser.Organization.Name : "",
            };
            var notificationIds = _userNotificationModelFactory.BeforeAfterImageNotificationtoCustomer(beforeAfterImageMailViewModel, fileDomain, notificationId);
            return notificationIds;
        }

        public bool SendJobNotificationtoTechForMeetingPersonal(JobEstimateEditModel franchiseeData, OrganizationRoleUser organizationData, NotificationTypes notificationTypes)
        {
            var currentDate = DateTime.Now.Date;
            var startDate = franchiseeData.ActualStartDateString.Date;
            var endDate = franchiseeData.ActualEndDateString.Date;

            var assigneeData = (_organizationRoleUserRepository.Table.Where(x => x.Id == franchiseeData.LogginUserId)).FirstOrDefault();
            franchiseeData.AssigneePhone = assigneeData != null && assigneeData.Person.Phones.Count != 0 ? assigneeData.Person.Phones.FirstOrDefault().Number : "";
            franchiseeData.AssigneeName = assigneeData != null ? assigneeData.Person.FirstName + " " + assigneeData.Person.MiddleName + " " + assigneeData.Person.LastName : "";
            _userNotificationModelFactory.NewJobOrEstimateReminderNotificationtoTechForMeeting(franchiseeData, organizationData, notificationTypes);
            return true;
        }


        public long? SendingInvoiceToCustomer(EstimateInvoiceEditMailModel model, List<File> fileDomain, NotificationTypes notificationId, string emailId)
        {
            var jobScheduler = _jobschedulerRepository.Get(model.SchedulerId.GetValueOrDefault());
            var ccEmail = "";
            if (jobScheduler != null)
            {
                ccEmail = jobScheduler.Person.Email;
            }
            var beforeAfterImageMailViewModel = new BeforeAfterImageMailViewModel
            {
                CustomerName = model != null ? model.CustomerName : "",
                FranchiseeName = model.FranchiseeName,
                EmailId = model.Email,
                FromMail = model.FromEmail,
                Code = model.Code,
                Url = model.Url,
                OfficeNumber = model.OfficeNumber,
                ToEmailId = model.ToEmailId,
                IsSigned = model.IsSigned,
                CcEmail = ccEmail,
                SchedulerEmail = jobScheduler.Franchisee.SchedulerEmail,
                //FromEmail = model.FromEmail,
                Body = model.Body,
                FileModel = model.FileModel,
                SchedulerId = jobScheduler != null ? jobScheduler.Id : default(long?),
                JobId = jobScheduler != null ? jobScheduler.JobId : default(long?),
                SchedulerUrl = _settings.SiteRootUrl + "/#/scheduler/" + jobScheduler.JobId + "/edit/" + jobScheduler.Id,
                Name = jobScheduler != null ? jobScheduler.Person.FirstName + " " + jobScheduler.Person.LastName : "",
                FranchiseeId = jobScheduler != null ? jobScheduler.FranchiseeId : default(long?),
            };
            var notificationIds = _userNotificationModelFactory.InvoiceCustomerNotificationtoCustomer(beforeAfterImageMailViewModel, fileDomain, notificationId);
            return notificationIds;
        }


        public long? SendingInvoiceToCustomerForSignedInvoices(EstimateInvoiceEditMailModel model, List<File> fileDomain, NotificationTypes notificationId, string emailId, bool isFromURL, bool mailToSalesRep)
        {
            var jobScheduler = _jobschedulerRepository.Get(model.SchedulerId.GetValueOrDefault());
            var ccEmail = "";
            if (jobScheduler != null)
            {
                ccEmail = jobScheduler.Person.Email;
            }
            var beforeAfterImageMailViewModel = new BeforeAfterImageMailViewModel
            {
                CustomerName = model != null ? model.CustomerName : "",
                FranchiseeName = model.FranchiseeName,
                EmailId = mailToSalesRep ? model.SalesRepEmail : model.Email,
                FromMail = model.FromEmail,
                Code = model.Code,
                Url = model.Url,
                IsSigned = model.IsSigned,
                CcEmail = ccEmail,
                SchedulerEmail = jobScheduler.Franchisee.SchedulerEmail,
                Body = model.Body,
                FileModel = model.FileModel,
                SignedInvoicesName = model.SignedInvoicesName,
                UnsignedInvoicesName = model.UnsignedInvoicesName,
                AllInvoicesSigned = model.AllInvoicesSigned,
                MailToSalesRep = mailToSalesRep,
                SalesRepName = model.SalesRep,
                IsFromURL = isFromURL,
                IsFromJob = model.IsFromJob,
                DoneFrom= isFromURL? "E-signature Link" : "Marblelife Application"

            };
            var notificationIds = _userNotificationModelFactory.InvoiceCustomerNotificationtoCustomerForSignedInvoices(beforeAfterImageMailViewModel, fileDomain, notificationId);
            return notificationIds;
        }
    }
}
