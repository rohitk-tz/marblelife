using Core.Application;
using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class CancellationMailForTechSalesNotification : ICancellationMailForTechSalesNotification
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobInfoFactory _jobInfoFactory;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly ISendNewJobNotificationtoTechService _sendNewJobNotificationToTechService;
        private readonly IJobFactory _jobFactory;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        public CancellationMailForTechSalesNotification(IUnitOfWork unitOfWork, IJobInfoFactory jobInfoFactory,
            ISendNewJobNotificationtoTechService sendNewJobNotificationToTechService, IJobFactory jobFactory)
        {
            _unitOfWork = unitOfWork;
            _jobInfoFactory = jobInfoFactory;
            _jobSchedulerRepository = _unitOfWork.Repository<JobScheduler>();
            _sendNewJobNotificationToTechService = sendNewJobNotificationToTechService;
            _organizationRoleUserRepository= _unitOfWork.Repository<OrganizationRoleUser>();
            _jobFactory = jobFactory;
        }
        public void ProcessRecords()
        {
            var schedulerList = _jobSchedulerRepository.IncludeMultiple(x=>x.Estimate).Where(x => !x.IsCancellationMailSend && x.MeetingID==null && !x.IsVacation).ToList();
            foreach(var scheduler in schedulerList)
            {
                if (scheduler.JobId != null)
                {
                    var jobEditModel = _jobFactory.CreateEditModel(scheduler.Job, scheduler);
                    var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == scheduler.AssigneeId)).FirstOrDefault();
                    jobEditModel.SchedulerId = scheduler.Id;
                    jobEditModel.JobId = scheduler.JobId.GetValueOrDefault();
                    _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelled(jobEditModel, listSchedule);
                    
                }
                else
                {
                    var jobEstimateModel = _jobFactory.CreateEstimateModel(scheduler.Estimate.JobEstimates, scheduler);
                    var listSchedule = (_organizationRoleUserRepository.Table.Where(x => x.Id == scheduler.AssigneeId)).FirstOrDefault();
                    jobEstimateModel.SchedulerId = scheduler.Id;
                    jobEstimateModel.Estimateid = scheduler.EstimateId.GetValueOrDefault();
                    _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelledForEstimate(jobEstimateModel, listSchedule);
                }

                scheduler.IsCancellationMailSend = true;
                _jobSchedulerRepository.Save(scheduler);
                _unitOfWork.SaveChanges();
            }
        }
    }
}
