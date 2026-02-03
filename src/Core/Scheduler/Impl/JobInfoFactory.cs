using Core.Application;
using Core.Application.Attribute;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobInfoFactory : IJobInfoFactory
    {
        private readonly IClock _clock;
        private readonly IJobFactory _jobFactory;
        public JobInfoFactory(IClock clock, IJobFactory jobFactory)
        {
            _clock = clock;
            _jobFactory = jobFactory;
        }
        public JobInfoEditModel CreateJobInfoModel(JobScheduler scheduler)
        {
            var job = scheduler.Job;
            var model = new JobInfoEditModel
            {
                Id = scheduler.Id,
                JobId = job != null ? scheduler.JobId : null,
                AssigneeId = scheduler.OrganizationRoleUser != null ? scheduler.AssigneeId : null,
                Assignee = scheduler.OrganizationRoleUser != null ? scheduler.OrganizationRoleUser.Person.Name.ToString() : null,
                CustomerClass = (job != null && job.JobType != null) ? job.JobType.Name : null,
                Description = job != null ? job.Description : null,
                End = _clock.ToLocal(scheduler.EndDate),
                Start = _clock.ToLocal(scheduler.StartDate),
                Franchisee = scheduler.Franchisee.Organization.Name,
                FranchiseeId = scheduler.FranchiseeId,
                GeoCode = job != null ? job.GeoCode : null,
                JobEnd = job != null ? _clock.ToLocal(job.EndDate) : (DateTime?)null,
                JobStart = job != null ? _clock.ToLocal(job.StartDate) : (DateTime?)null,
                SalesRep = scheduler.SalesRep != null ? scheduler.SalesRep.Person.Name.ToString() : null,
                Title = scheduler.Title,
                JobCustomer = _jobFactory.CreateCustomerModel(scheduler.Job.JobCustomer),
                IsCustomerMailSend = scheduler.IsCustomerMailSend

            };
            return model;
        }
    }
}
