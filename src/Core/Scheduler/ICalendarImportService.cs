using Core.Scheduler.ViewModel;

namespace Core.Scheduler
{
    public interface ICalendarImportService
    {
        bool Save(CalendarImportModel model);
    }
}
