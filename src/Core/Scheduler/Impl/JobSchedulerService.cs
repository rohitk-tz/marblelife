using Core.Application;
using Core.Application.Attribute;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class JobSchedulerService : IJobSchedulerService
    {
        private readonly IJobFactory _jobFactory;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        public JobSchedulerService(IJobFactory jobFactory, IUnitOfWork unitOfWork, IClock clock)
        {
            _jobFactory = jobFactory;
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _unitOfWork = unitOfWork;
            _clock = clock;
        }
        public void ChangeSchedulerDateTime(JobEditModel model, DatetimeModel dateTimeModel, List<OldSchedulerModel> oldSchedulerList)
        {

            var schedulerList = _jobSchedulerRepository.Table.Where(x => x.JobId == model.JobId && x.IsActive).OrderBy(x => x.Id).ToList();
            var schedulerDomain = schedulerList.FirstOrDefault(x => x.Id == model.Id);
            var parentSchedulerIds = schedulerList.Where(x => x.ParentJobId != null).Select(x => x.ParentJobId).ToList();

            if (parentSchedulerIds.Count() > 0)
            {
                schedulerList = schedulerList.Where(x => parentSchedulerIds.Contains(x.Id) || parentSchedulerIds.Contains(x.ParentJobId)).ToList();
            }
            else
            {
                if (model.IsRepeat)
                    schedulerList = schedulerList.Where(x => x.ParentJobId == schedulerDomain.Id || x.Id == schedulerDomain.Id).ToList();

                else
                {
                    schedulerList = schedulerList.Where(x => x.ParentJobId == null && x.IsRepeat).ToList();
                }
            }
            if (schedulerList.Count() == 0)
            {
                return;
            }
            var daysDiff = (dateTimeModel.EndDate - dateTimeModel.StartDate).TotalDays;
            var hoursDiff = (dateTimeModel.EndDate - dateTimeModel.StartDate).TotalHours;
            var liveDaysDiff = (model.ActualEndDateString - model.ActualStartDateString).TotalDays;
            var liveHoursDiff = (model.ActualEndDateString - model.ActualStartDateString).TotalHours;

            if (liveDaysDiff == daysDiff)
            {
                var isPresent = default(bool);
                var oldStartDate = default(DateTime);
                var oldEndDate = default(DateTime);
                double startDateTimeGap = 0;
                double endDateTimeGap = 0;
                foreach (var scheduler in schedulerList)
                {

                    if (oldStartDate != default(DateTime))
                    {
                        isPresent = oldSchedulerList.Any(x => x.Id == scheduler.Id);
                        if (isPresent == false) continue;
                        startDateTimeGap = (oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).StartDate - oldStartDate).TotalDays;
                        endDateTimeGap = (oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).EndDate - oldEndDate).TotalDays;
                    }

                    SetDateTimeOfScheduler(scheduler, model.ActualStartDateString, model.ActualEndDateString,
                        oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id), startDateTimeGap, endDateTimeGap);

                    isPresent = oldSchedulerList.Any(x => x.Id == scheduler.Id);
                    if (isPresent == false) continue;
                    oldStartDate = oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).StartDate;
                    oldEndDate = oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).EndDate;
                }
            }

            else if (liveDaysDiff < 1)
            {
                foreach (var scheduler in schedulerList)
                {
                    var isNotNull= oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id);
                    if (isNotNull == null) continue;
                    var oldStartDate = oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).StartDate;
                    var oldEndDate = oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).EndDate;
                    scheduler.StartDateTimeString = SettingDateTime(model.ActualStartDateString, oldStartDate, 0, default);
                    scheduler.EndDateTimeString = SettingDateTime(model.ActualEndDateString, oldEndDate, 0, default);
                    scheduler.StartDate = _clock.ToUtc(scheduler.StartDateTimeString);
                    scheduler.EndDate = _clock.ToUtc(scheduler.EndDateTimeString);
                    _jobSchedulerRepository.Save(scheduler);
                }
            }
            else if (liveDaysDiff > 1)
            {
                foreach (var scheduler in schedulerList)
                {
                    if (oldSchedulerList.Any(x => x.Id == scheduler.Id))
                    {
                        var oldStartDate = oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).StartDate;
                        var oldEndDate = oldSchedulerList.FirstOrDefault(x => x.Id == scheduler.Id).EndDate;
                        scheduler.StartDateTimeString = SettingDateTime(model.ActualStartDateString, oldStartDate, 0, default);
                        scheduler.EndDateTimeString = SettingDateTime(model.ActualEndDateString, oldEndDate, 0, default);
                        scheduler.StartDate = _clock.ToUtc(scheduler.StartDateTimeString);
                        scheduler.EndDate = _clock.ToUtc(scheduler.EndDateTimeString);
                        _jobSchedulerRepository.Save(scheduler);
                    }
                }
            }
        }

        private void SetDateTimeOfScheduler(JobScheduler scheduler, DateTime startDate, DateTime endDate,
            OldSchedulerModel oldSchedulerModel, double startDateTimeGap, double endDateTimeGap)
        {
            if (oldSchedulerModel==null) return;
            scheduler.StartDateTimeString = SettingDateTime(startDate, oldSchedulerModel.StartDate,
                startDateTimeGap, SchedulerDateTime.StartDate);

            scheduler.StartDate = _clock.ToUtc(scheduler.StartDateTimeString);
            var daysDiff = (oldSchedulerModel.EndDate - oldSchedulerModel.StartDate).TotalDays;
            scheduler.EndDateTimeString = scheduler.StartDateTimeString.AddDays(daysDiff);
            scheduler.EndDate = _clock.ToUtc(scheduler.EndDateTimeString);
            _jobSchedulerRepository.Save(scheduler);


            SchedulerDateTime.StartDate = scheduler.StartDateTimeString;
        }

        private DateTime SettingDateTime(DateTime newSchdeulerDateTime, DateTime oldSchedulerDateTime,
            double dateTimeGap, DateTime lastSchedulerDateTime)
        {
            if (dateTimeGap == 0)
            {
                var oldTime = newSchdeulerDateTime.ToString("hh:mm tt");
                var newDate = newSchdeulerDateTime.ToShortDateString();
                var newDateTime = newDate + ' ' + oldTime;
                return DateTime.Parse(newDateTime);

            }
            else
            {
                return lastSchedulerDateTime.AddDays(dateTimeGap);
            }
        }

        private static class SchedulerDateTime
        {
            public static DateTime StartDate { get; set; }
        }
    }
}
