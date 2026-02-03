using Core.Application;
using Core.Application.Attribute;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobInfoService : IJobInfoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJobInfoFactory _jobInfoFactory;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IRepository<Job> _jobRepository;
        private readonly ISendNewJobNotificationtoTechService _sendNewJobNotificationToTechService;
        public JobInfoService(IUnitOfWork unitOfWork, IJobInfoFactory jobInfoFactory, ISendNewJobNotificationtoTechService sendNewJobNotificationToTechService)
        {
            _unitOfWork = unitOfWork;
            _jobInfoFactory = jobInfoFactory;
            _jobSchedulerRepository = _unitOfWork.Repository<JobScheduler>();
            _jobRepository = _unitOfWork.Repository<Job>();
            _sendNewJobNotificationToTechService = sendNewJobNotificationToTechService;
        }

        public JobInfoEditModel Get(long id)
        {
            var scheduler = _jobSchedulerRepository.Get(id);
            var model = _jobInfoFactory.CreateJobInfoModel(scheduler);
            return model;
        }

        public bool UpdateJobTime(JobInfoEditModel model)
        {
            var scheduler = _jobSchedulerRepository.Get(model.Id);
            if (scheduler == null)
                return false;
            scheduler.StartDate = model.Start;
            scheduler.EndDate = model.End;
            _jobSchedulerRepository.Save(scheduler);
            return true;
        }

        public bool Delete(long id)
        {
            var scheduler = _jobSchedulerRepository.Get(id);
            if (scheduler == null)
                return false;

            var jobId = scheduler.JobId;
            if (jobId == null)
                return false;

            var listSchedulers = _jobSchedulerRepository.Fetch(x => x.JobId == jobId);
            _sendNewJobNotificationToTechService.SendJobNotificationtoTechForCancelledForDeleteButton(scheduler, true);
            _jobSchedulerRepository.Delete(scheduler);
            if (listSchedulers.Count() == 1)
            {
                _jobRepository.Delete(x => x.Id == jobId);

            }
            return true;
        }
    }
}
