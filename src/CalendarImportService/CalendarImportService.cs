using Core.Application;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Interfaces;
using Ical.Net.Interfaces.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalendarImportService
{
    public class CalendarImportService : ICalendarImportService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;

        public CalendarImportService()
        {
            _unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _setting = ApplicationManager.DependencyInjection.Resolve<ISettings>();
        }

        public void Import()
        {
            GetEvents();
        }

        private void GetEvents()
        {
            Console.WriteLine("Starting Service!");
            var filePath = _setting.CalendarFilePath;
            var path = new Uri(filePath).LocalPath;
            IICalendarCollection calendars = Calendar.LoadFromFile(path);

            var occurrences = calendars.GetOccurrences(_clock.UtcNow.AddMonths(-1), _clock.UtcNow);
            foreach (Occurrence occurrence in occurrences)
            {
                CreateJobModel(occurrence);
            }
        }

        private void CreateJobModel(Occurrence occurrence)
        {
            DateTime occurrenceStartTime = occurrence.Period.StartTime.AsSystemLocal;
            DateTime occurrenceEndTime = occurrence.Period.EndTime.AsSystemLocal;
            IRecurringComponent rc = occurrence.Source as IRecurringComponent;
            if (rc != null)
            {
                Console.WriteLine("Summary : " + rc.Summary + "," + Environment.NewLine +
                                  "Start : " + occurrenceStartTime + "," + Environment.NewLine +
                                  "End : " + occurrenceEndTime + "," + Environment.NewLine +
                                  "Children : " + rc.Children.Count() + "," + Environment.NewLine +
                                  "Class : " + rc.Class + "," + Environment.NewLine +
                                  "Column : " + rc.Column + "," + Environment.NewLine +
                                  "Comments : " + rc.Comments.Count() + "," + Environment.NewLine +
                                  "Contacts : " + rc.Contacts.Count() + "," + Environment.NewLine +
                                  "Created : " + rc.Created + "," + Environment.NewLine +
                                  "Description : " + rc.Description + "," + Environment.NewLine +
                                  "DtStamp : " + rc.DtStamp + "," + Environment.NewLine +
                                  "ExceptionDates : " + rc.ExceptionDates.Count() + "," + Environment.NewLine +
                                  "ExceptionRules : " + rc.ExceptionRules.Count() + "," + Environment.NewLine +
                                  "Group : " + rc.Group + "," + Environment.NewLine +
                                  "LastModified : " + rc.LastModified + "," + Environment.NewLine +
                                  "Line : " + rc.Line + "," + Environment.NewLine +
                                  "Name : " + rc.Name + "," + Environment.NewLine +
                                  "Organizer : " + rc.Organizer + "," + Environment.NewLine +
                                  "Parent : " + rc.Parent + "," + Environment.NewLine +
                                  "Priority : " + rc.Priority + "," + Environment.NewLine +
                                  "Properties : " + rc.Properties.Count() + "," + Environment.NewLine +
                                  "RecurrenceDates : " + rc.RecurrenceDates.Count() + "," + Environment.NewLine +
                                  "RecurrenceId : " + rc.RecurrenceId + "," + Environment.NewLine +
                                  "Sequence : " + rc.Sequence + "," + Environment.NewLine +
                                  "Uid : " + rc.Uid + "," + Environment.NewLine +
                                  "Url : " + rc.Url + "," + Environment.NewLine +
                                  "Attendees : " + rc.Attendees.Count() + "," + Environment.NewLine);
            }
        }
    }
}
