using Core.Application.Attribute;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class CalendarFactory : ICalendarFactory
    {
        public CalendarFileUpload CreateDomain(CalendarImportModel model)
        {
            var assigneeId = model.TypeId == (long)ScheduleType.Job ? model.TechId.Value : model.SalesRepId.Value;
            var domain = new CalendarFileUpload
            {
                Id = model.Id,
                AssigneeId = assigneeId,
                TypeId = model.TypeId,
                FileId = model.FileId,
                FranchiseeId = model.FranchiseeId,
                FailedRecords = model.FailedRecords,
                SavedRecords = model.SavedRecords,
                TotalRecords = model.TotalRecords,
                StatusId = model.StatusId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                TimeZoneId = model.TimeZoneId,
                IsNew = model.Id <= 0
            };
            return domain;
        }
    }
}
