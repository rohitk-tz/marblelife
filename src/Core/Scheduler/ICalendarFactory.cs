using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;

namespace Core.Scheduler
{
    public interface ICalendarFactory
    {
        CalendarFileUpload CreateDomain(CalendarImportModel model);
    }
}
