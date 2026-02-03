using Core.Application.Attribute;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobDetailsFactory : IJobDetailsFactory
    {
        public JobDetails CreateDomain(Job model)
        {
            var jobDetails = new JobDetails()
            {
                CustomerId = model.CustomerId,
                JobId = model.Id,
                EndDate = model.EndDate,
                StartDate = model.StartDate,
                EndDateTimeString = model.EndDateTimeString,
                StartDateTimeString = model.StartDateTimeString,
                EstimateId = model.EstimateId,
                GeoCode = model.GeoCode,
                JobTypeId = model.JobTypeId,
                StatusId=model.StatusId,
                Description=model.Description

            };
            return jobDetails;
        }

        public JobDetails CreateDomain(JobEstimateEditModel model)
        {
            var jobDetails = new JobDetails()
            {
                CustomerId = model.CustomerId,
                JobId = model.Id,
                EndDate = model.EndDate,
                StartDate = model.StartDate,
                EndDateTimeString = model.ActualEndDateString,
                StartDateTimeString = model.ActualStartDateString,
                EstimateId = model.Id,
                GeoCode = model.GeoCode,
                JobTypeId =null,
                StatusId = null

            };
            return jobDetails;
        }

        public JobDetails CreateDomain(JobEstimate jobEstimate)
        {
            var jobDetails = new JobDetails()
            {
                CustomerId = jobEstimate.CustomerId,
                JobId = null,
                EndDate = jobEstimate.EndDate.GetValueOrDefault(),
                StartDate = jobEstimate.StartDate.GetValueOrDefault(),
                EndDateTimeString = jobEstimate.EndDateTimeString,
                StartDateTimeString = jobEstimate.StartDateTimeString,
                EstimateId = jobEstimate.Id,
                GeoCode = jobEstimate.GeoCode,
                JobTypeId = null,
                StatusId = null,
               Id= jobEstimate.Id,
               Description=jobEstimate.Description

            };
            return jobDetails;
        }
    }
}
