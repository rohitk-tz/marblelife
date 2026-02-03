using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Geo;
using Core.Geo.ViewModel;
using Core.Organizations.Domain;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using Core.Scheduler.Enum;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using Ical.Net;
using Ical.Net.Interfaces;
using Ical.Net.Interfaces.Components;
using Ical.Net.Serialization.iCalendar.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class CalendarParsePollingAgent : ICalendarParsePollingAgent
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IFileService _fileService;
        private readonly IRepository<CalendarFileUpload> _calendarFileUploadRepository;
        private readonly ISettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Person> _personRepository;
        private readonly IJobService _jobService;
        private readonly IAddressFactory _addressFactory;
        private readonly ICityService _cityService;
        private readonly IStateService _stateService;
        private readonly IZipService _zipService;
        private readonly IEstimateService _estimateService;
        private readonly IRepository<DataRecorderMetaData> _dataRecorderMetaDataRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<TimeZoneInformation> _timeZoneInformationRepository;
        public CalendarParsePollingAgent(IUnitOfWork unitOfWork, ILogService logService, IClock clock, IFileService fileService,
            ISettings settings, IJobService jobService, IAddressFactory addressFactory, ICityService cityService, IStateService stateService,
            IZipService zipService, IEstimateService estimateService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _clock = clock;
            _fileService = fileService;
            _settings = settings;
            _calendarFileUploadRepository = unitOfWork.Repository<CalendarFileUpload>();
            _personRepository = unitOfWork.Repository<Person>();
            _jobService = jobService;
            _addressFactory = addressFactory;
            _cityService = cityService;
            _stateService = stateService;
            _zipService = zipService;
            _estimateService = estimateService;
            _dataRecorderMetaDataRepository = unitOfWork.Repository<DataRecorderMetaData>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _timeZoneInformationRepository = unitOfWork.Repository<TimeZoneInformation>();
        }

        public void ParseCalendarFile()
        {
            if (!_settings.ParseCalendarFile)
            {
                _logService.Info("Calendar parsing Service is Stopped!");
                return;
            }
            var calendarFile = GetFileToParse();

            if (calendarFile == null)
            {
                _logService.Info("No file found for parsing");
                return;
            }

            var sb = new StringBuilder();
            try
            {
                var model = new CalendarDataModel { };
                model.TypeId = calendarFile.TypeId;

                calendarFile.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
                _calendarFileUploadRepository.Save(calendarFile);

                _unitOfWork.SaveChanges();

                var filePath = MediaLocationHelper.FilePath(calendarFile.File.RelativeLocation, calendarFile.File.Name);
                var path = new Uri(filePath).LocalPath;
                IICalendarCollection collection = Calendar.LoadFromFile(path);

                if (collection == null || collection.Count <= 0)
                {
                    sb.Append(Log("No Data In File!"));
                    CreateLogFile(sb, "Calendar_" + calendarFile.Id);
                    calendarFile.StatusId = (long)SalesDataUploadStatus.Failed;
                    return;
                }

                model = GetProperties(collection, model);

                var wc = new WebClient();
                var result = wc.DownloadString(path);

                using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    var serializer = new CalendarSerializer();
                    var calendars = (IICalendarCollection)serializer.Deserialize(memoryStream, Encoding.UTF8);
                    CreateModel(calendars, calendarFile, model, sb);
                }
            }
            catch (Exception ex)
            {
                sb.Append(Log("Some issue occured in File Parsing. Please check the file." + ex.StackTrace));

                CreateLogFile(sb, "Calendar_" + calendarFile.Id);
                calendarFile.StatusId = (long)SalesDataUploadStatus.Failed;
                SaveCalendarFile(calendarFile);
                return;
            }
        }

        private void SaveCalendarFile(CalendarFileUpload calendarFile)
        {
            try
            {
                _unitOfWork.StartTransaction();
                var fileModel = PrepareLogFileModel("Calendar_" + calendarFile.Id);
                var file = _fileService.SaveModel(fileModel);
                calendarFile.LogFileId = file.Id;

                _calendarFileUploadRepository.Save(calendarFile);

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                throw new Exception(ex.Message);
            }
        }

        private FileModel PrepareLogFileModel(string name)
        {
            var fileModel = new FileModel();
            fileModel.Name = name;
            fileModel.Caption = name;
            fileModel.MimeType = "application/text";
            fileModel.RelativeLocation = MediaLocationHelper.GetMediaLocationForLogs().Path.ToRelativePath();
            fileModel.Size = new FileInfo(MediaLocationHelper.GetMediaLocationForLogs().Path + "" + name).Length;
            return fileModel;
        }

        private void CreateModel(IICalendarCollection calendars, CalendarFileUpload calendarFile, CalendarDataModel model, StringBuilder sb)
        {
            
            var totalRecords = calendars.SelectMany(x => x.Events).Count();
            var failedRecords = 0;
            var savedRecords = 0;
            var currentDate = _clock.UtcNow;
            var dateToCompare = currentDate.AddMonths(-3);
            int timeDifference = -5;

            var timeZone = _timeZoneInformationRepository.Get(calendarFile.TimeZoneId);
            if (timeZone != null)
                timeDifference = Convert.ToInt32(timeZone.TimeDifference);

            foreach (var ev in calendars.SelectMany(e => e.Events))
            {
                var customerModel = CreateCustomerModel(ev, calendarFile.FranchiseeId, model, sb);

                if (model.TypeId == (long)ScheduleType.Estimate)
                {
                    sb.Append(Log("Start Creating Estimate Model"));
                    var estimate = CreateEstimateModel(ev, timeDifference);
                    if (estimate.StartDate < dateToCompare)
                    {
                        sb.Append(Log("skipping record  - (" + estimate.StartDate + " - " + estimate.EndDate + ") as it's before the compariosn date - " + dateToCompare));
                        continue;
                    }
                    estimate.JobCustomer = customerModel;
                    estimate.FranchiseeId = calendarFile.FranchiseeId;
                    estimate.SalesRepId = calendarFile.AssigneeId;
                    try
                    {
                        _estimateService.Save(estimate);
                        if (estimate.JobScheduler != null && estimate.JobScheduler.EstimateId > 0)
                        {
                            var metaData = CreateDataRecorderMetaData(ev);
                            UpdateDataRecorderMetaData(metaData, estimate.DataRecorderMetaData);
                        }
                        savedRecords++;
                    }
                    catch (Exception ex)
                    {
                        sb.Append(Log("Error:" + ex.StackTrace));
                        _logService.Info(string.Format("Error saving record : {0}", estimate.JobCustomer.CustomerName + ex.StackTrace));
                        failedRecords++;
                        continue;
                    }
                }
                else
                {
                    var job = CreateJobModel(ev, timeDifference);
                    if (job.StartDate < dateToCompare)
                    {
                        sb.Append(Log("skipping record for - " + job.Title + "(" + job.StartDate + " - " + job.EndDate + ") as it's before the compariosn date - "
                            + dateToCompare));
                        continue;
                    }
                    job.JobCustomer = customerModel;
                    job.FranchiseeId = calendarFile.FranchiseeId;
                    job.AssigneeId = calendarFile.AssigneeId;
                    job.TechIds.Add(calendarFile.AssigneeId);
                    try
                    {
                        _jobService.Save(job);
                        if (job.JobId > 0)
                        {
                            var metaData = CreateDataRecorderMetaData(ev);
                            UpdateDataRecorderMetaData(metaData, job.DataRecorderMetaData);
                        }
                        savedRecords++;
                    }
                    catch (Exception ex)
                    {
                        sb.Append(Log("Error:" + ex.StackTrace));
                        _logService.Info(string.Format("Error saving record : {0}", job.JobCustomer.CustomerName + ex.StackTrace));
                        failedRecords++;
                        continue;
                    }
                }
            }

            calendarFile.SavedRecords = savedRecords;
            calendarFile.FailedRecords = failedRecords;
            calendarFile.TotalRecords = totalRecords;
            calendarFile.StatusId = (long)SalesDataUploadStatus.Parsed;
            CreateLogFile(sb, "Calendar_" + calendarFile.Id);
            SaveCalendarFile(calendarFile);
            _unitOfWork.SaveChanges();
        }

        private void UpdateDataRecorderMetaData(DataRecorderMetaData model, DataRecorderMetaData domain)
        {
            domain.CreatedBy = model.CreatedBy;
            domain.DateCreated = model.DateCreated;
            domain.DateModified = model.DateModified;
            _dataRecorderMetaDataRepository.Save(domain);
        }

        private JobEstimateEditModel CreateEstimateModel(IEvent ev, int timeDifference)
        {
            string defaultTitle = "Default Title";
            var estimateModel = new JobEstimateEditModel { };
            estimateModel.JobTypeId = (long)MarketingClassType.Residential;

            estimateModel.StartDate = ev.Start.HasDate ? ev.Start.Value : _clock.UtcNow;
            estimateModel.EndDate = ev.End.HasDate ? ev.End.Value : _clock.UtcNow;
            if (!ev.End.HasTime)
            {
                estimateModel.EndDate = ev.End.Value.AddHours(23).AddMinutes(59);
            }

            estimateModel.StartDate = estimateModel.StartDate.AddHours(timeDifference);
            estimateModel.EndDate = estimateModel.EndDate.AddHours(timeDifference);
            estimateModel.Description = ev.Description;
            estimateModel.Title = string.IsNullOrEmpty(ev.Summary) ? defaultTitle : ev.Summary;
            estimateModel.CreatedBy = ev.Organizer != null ? ev.Organizer.CommonName : null;
            estimateModel.Location = ev.Location;
            estimateModel.DataRecorderMetaData = CreateDataRecorderMetaData(ev);
            estimateModel.IsImported = true;
            return estimateModel;
        }

        private JobEditModel CreateJobModel(IEvent ev, int timeDifference)
        {
            var jobModel = new JobEditModel { };
            jobModel.TechIds = new List<long>();
            string defaultTitle = "Default Title";

            var status = ev.Status;
            if (status.ToString().Equals(CalendarStatusType.Confirmed.ToString()))
                jobModel.StatusId = (long)JobStatusType.Assigned;
            else if (status.ToString().Equals(CalendarStatusType.Tentative.ToString()))
                jobModel.StatusId = (long)JobStatusType.Tentative;

            jobModel.JobTypeId = (long)MarketingClassType.Residential;

            jobModel.StartDate = ev.Start.HasDate ? ev.Start.Value : _clock.UtcNow;
            jobModel.EndDate = ev.End.HasDate ? ev.End.Value : _clock.UtcNow;
            if (!ev.End.HasTime)
            {
                jobModel.EndDate = ev.End.Value.AddHours(23).AddMinutes(59);
            }
            jobModel.StartDate = jobModel.StartDate.AddHours(timeDifference);
            jobModel.EndDate = jobModel.EndDate.AddHours(timeDifference);
            jobModel.Description = ev.Description;
            jobModel.Title = string.IsNullOrEmpty(ev.Summary) ? defaultTitle : ev.Summary;
            jobModel.CreatedBy = ev.Organizer != null ? ev.Organizer.CommonName : null;
            jobModel.Location = ev.Location;
            jobModel.IsImported = true;
            return jobModel;
        }

        private JobCustomerEditModel CreateCustomerModel(IEvent ev, long franchiseeId, CalendarDataModel model, StringBuilder sb)
        {
            string customerName = "Default Customer Info";
            string email = "defaultEmail@email.com";
            var address = CreateAddressModel(ev, franchiseeId, sb);

            var name = model.CalendarName;
            if (string.IsNullOrEmpty(model.CalendarName))
                name = ev.Summary != null ? ev.Summary : customerName.ToString();

            var customerModel = new JobCustomerEditModel
            {
                CustomerName = name,
                Email = string.IsNullOrEmpty(model.Email) ? email : model.Email,
                Address = address,
            };
            return customerModel;
        }

        private AddressEditModel CreateAddressModel(IEvent ev, long franchiseeId, StringBuilder sb)
        {
            var addressEditModel = new AddressEditModel { };

            if (string.IsNullOrEmpty(ev.Location))
            {
                sb.Append(Log("location field is empty, getting Frachisee -  " + franchiseeId + " ,address as customer address!"));
                addressEditModel = GetFranchiseeAddress(franchiseeId);
                return addressEditModel;
            }

            var url = "https://maps.googleapis.com/maps/api/geocode/xml";
            var apiKey = "AIzaSyDpLxw_-S2SCLNs7Lq6lSI_tkQ_AfSJXPg";
            string requestUri = string.Format(url + "?address={0}", ev.Location + "&key=" + apiKey);

            var result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/xml");
                try
                {
                    result = client.DownloadString(requestUri);
                    GeoCodeResponse list = new GeoCodeResponse();
                    XmlSerializer serializer = new XmlSerializer(typeof(GeoCodeResponse));
                    using (TextReader reader = new StringReader(result))
                    {
                        list = (GeoCodeResponse)serializer.Deserialize(reader);
                    }
                    if (list != null && list.Status == "OK" && list.AddressGeoCodeModel != null)
                    {
                        addressEditModel = FillModel(list.AddressGeoCodeModel);
                    }
                    else
                    {
                        sb.Append(Log("Error Getting the Address, saving Franchisee address as customer Address!"));
                        addressEditModel = GetFranchiseeAddress(franchiseeId);
                    }
                }
                catch (WebException ex)
                {
                    sb.Append(Log("Error getting Customer Location from Google GeoCode," + ex.StackTrace));
                    _logService.Info(string.Format("Web Exception :", ex.Message));
                }
            }
            return addressEditModel;
        }

        private AddressEditModel GetFranchiseeAddress(long franchiseeId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            var franchiseeAddress = franchisee.Organization.Address.FirstOrDefault();
            return _addressFactory.CreateEditModel(franchiseeAddress);
        }

        private AddressEditModel FillModel(AddressGeoCodeModel res)
        {
            var addressEditModel = new AddressEditModel { };
            const string streetNumber = "street_number";
            const string route = "route";
            const string neighborhood = "neighborhood";
            const string locality = "locality";
            const string political = "political";
            const string administrativeAreaLevel_1 = "administrative_area_level_1";
            const string administrativeAreaLevel_2 = "administrative_area_level_2";
            const string country = "country";
            const string postalCode = "postal_code";
            string defaultAddress = "Default address Line 1";

            foreach (var item in res.AddressComponents)
            {
                switch (item.Type)
                {
                    case streetNumber:
                        addressEditModel.AddressLine1 = item.LongName;
                        break;
                    case route:
                        addressEditModel.AddressLine1 = string.IsNullOrEmpty(addressEditModel.AddressLine1)
                                                        ? addressEditModel.AddressLine1 + item.LongName : addressEditModel.AddressLine1 + " " + item.LongName;
                        break;
                    case neighborhood:
                        addressEditModel.AddressLine1 = string.IsNullOrEmpty(addressEditModel.AddressLine1)
                                                        ? addressEditModel.AddressLine1 + item.LongName : addressEditModel.AddressLine1 + " " + item.LongName;
                        break;
                    case locality:
                        addressEditModel.City = item.LongName;
                        break;
                    case political:
                        addressEditModel.City = item.LongName;
                        break;
                    case administrativeAreaLevel_2:
                        addressEditModel.City = item.LongName;
                        break;
                    case administrativeAreaLevel_1:
                        addressEditModel.State = item.LongName;
                        break;
                    case postalCode:
                        addressEditModel.ZipCode = item.LongName;
                        break;
                    case country:
                        addressEditModel.Country = item.LongName;
                        break;
                    default:
                        addressEditModel.AddressLine2 = item.LongName;
                        break;
                };
            }
            if (string.IsNullOrEmpty(addressEditModel.AddressLine1))
                addressEditModel.AddressLine1 = defaultAddress.ToString();
            return addressEditModel;
        }

        private DataRecorderMetaData CreateDataRecorderMetaData(IEvent ev)
        {
            var createdBy = GetCreatedBy(ev);
            var data = new DataRecorderMetaData
            {
                CreatedBy = createdBy,
                DateCreated = ev.Created == null ? ev.Created.AsUtc : _clock.UtcNow,
                DateModified = ev.LastModified.AsUtc,
                IsNew = true
            };
            return data;
        }

        private long GetCreatedBy(IEvent ev)
        {
            if (ev.Organizer == null || ev.Organizer.CommonName == null)
                return 1;
            var id = _personRepository.Table.Where(x => x.Email.ToUpper().Equals(ev.Organizer.CommonName.ToUpper())).Select(y => y.Id).FirstOrDefault();
            if (id <= 0)
                return 1;
            return id;
        }

        private CalendarDataModel GetProperties(IICalendarCollection calendars, CalendarDataModel model)
        {
            var properties = calendars.Count() > 0 ? calendars[0].Properties : null;
            if (properties.ContainsKey("X-WR-CALNAME"))
            {
                var name = properties["X-WR-CALNAME"].Value;
                model.CalendarName = name != null ? name.ToString() : null;
            }
            if (properties.ContainsKey("X-WR-CALDESC"))
            {
                var email = properties["X-WR-CALDESC"].Value;
                model.Email = email != null ? email.ToString() : null;
            }
            return model;
        }

        private CalendarFileUpload GetFileToParse()
        {
            return _calendarFileUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded)
                             .OrderBy(x => x.FranchiseeId).OrderBy(x => x.Id).FirstOrDefault();
        }

        private static string Log(string message)
        {
            return DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss:fff tt") + " \t\t " + message + "\r\n\r\n";
        }

        private void CreateLogFile(StringBuilder sb, string fileName)
        {
            var path = MediaLocationHelper.GetMediaLocationForLogs().Path + fileName;

            using (StreamWriter file = new StreamWriter(path))
            {
                file.Write(sb.ToString());
            }
        }

    }
}
