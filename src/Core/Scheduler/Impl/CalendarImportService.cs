using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using System;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class CalendarImportService : ICalendarImportService
    {
        private readonly ICalendarFactory _calendarFactory;
        private readonly IFileService _fileService;
        private readonly IRepository<CalendarFileUpload> _calendarFileUploadRepository;
        private readonly IClock _clock;
        private readonly ILogService _logService;
        public CalendarImportService(IUnitOfWork unitOfWork, ICalendarFactory calendarFactory, IFileService fileService, IClock clock,
            ILogService logService)
        {
            _calendarFactory = calendarFactory;
            _fileService = fileService;
            _calendarFileUploadRepository = unitOfWork.Repository<CalendarFileUpload>();
            _clock = clock;
            _logService = logService;
        }
        public bool Save(CalendarImportModel model)
        {
            model.TypeId = (long)ScheduleType.Job;
            if (model.SalesRepId > 0)
                model.TypeId = (long)ScheduleType.Estimate;

            model.StatusId = (long)SalesDataUploadStatus.Uploaded;

            if (model.TimeZoneId <= 0)
                model.TimeZoneId = 1;

            try
            {
                var domain = _calendarFactory.CreateDomain(model);

                var id = SaveFile(model.File);
                domain.FileId = id;
                _calendarFileUploadRepository.Save(domain);
            }
            catch (Exception ex)
            {
                _logService.Error(string.Format("Error Saving Calendar File - {0}", ex.StackTrace));
                return false;
            }
            return true;
        }

        private long SaveFile(FileModel file)
        {
            var path = MediaLocationHelper.FilePath(file.RelativeLocation, file.Name).ToFullPath();
            var destination = MediaLocationHelper.GetCalendarMediaLocation();
            var destFileName = string.Format((file.Caption.Length <= 20) ? file.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                : file.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
            var FileName = _fileService.MoveFile(path, destination, destFileName, file.Extension);
            file.Name = destFileName + file.Extension;
            file.RelativeLocation = MediaLocationHelper.GetCalendarMediaLocation().Path;

            var domain = _fileService.SaveModel(file);
            return domain.Id;
        }
    }
}
