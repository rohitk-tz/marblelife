using Core.Application;
using Core.Application.Attribute;
using Core.Sales.Enum;
using Core.Scheduler.Domain;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Application.Impl;
using Core.Application.Extensions;
using Core.Scheduler.ViewModel;
using Core.Geo.Domain;
using Core.Organizations.Domain;
using System.Text;
using System.IO;
using Core.Application.ViewModel;
using Core.Application.Exceptions;
using Core.Scheduler.Enum;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class ZipParserNotificationService : IZipParserNotificationService
    {
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IJobFactory _jobFactory;
        private readonly IRepository<GeoCodefileupload> _geoCodeFileRepository;
        private readonly IRepository<County> _countyRepository;
        private readonly IRepository<ZipCode> _zipRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<City> _cityRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private IFileService _fileService;
        private int newlyAddedZipCodes = 0;
        public ZipParserNotificationService(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock, IJobFactory jobFactory, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _fileService = fileService;
            _geoCodeFileRepository = unitOfWork.Repository<GeoCodefileupload>();
            _countyRepository = unitOfWork.Repository<County>();
            _zipRepository = unitOfWork.Repository<ZipCode>();
            _countryRepository = unitOfWork.Repository<Country>();
            _cityRepository = unitOfWork.Repository<City>();
            _stateRepository = unitOfWork.Repository<State>();
            _jobFactory = jobFactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
        }

        public void ProcessRecords()
        {
            _unitOfWork.StartTransaction();
            var zipParsingModel = new List<ZipParsingModel>();
            var currentCountyName = "";
            var currentZipCodeName = "";
            var newlyAddedCounties = 0;
            var updatedCounties = 0;
            var deletedCounties = 0;
          
            var updatedZipCodes = 0;
            var deletedZipCodes = 0;
            var countyIds = new List<long>();
            var stats = new SaveModelStats();
            stats.Logs = new StringBuilder();
            stats.LogsForCounty = new StringBuilder();
            stats.LogsForZipCode = new StringBuilder();
            var countyViewModel = new List<CountyViewModel>();
            var doublicateCountyViewModel = new List<CountyViewModel>();
            var sb = new StringBuilder();
            var sbForCounty = new StringBuilder();
            var sbForZipCode = new StringBuilder();
            _logService.Info("Starting Geo Code File Parsing");
            if (!ApplicationManager.Settings.ParseGeoCodeFile)
            {
                _logService.Info("Geo Code Notification turned off!");
                return;
            }
            var zipCodeFileList = GetiZipDataToParse();
            if (zipCodeFileList == null || !zipCodeFileList.Any())
            {
                _logService.Debug("No file found for parsing for geo parsing");
                return;
            }
            DataTable data;
            var geoCodeParsedFile = new GeoCodefileupload();
            int isCounty = 0;
            long? fileId = default(long?);
            var listFranchisee = _franchiseeRepository.Table.Where(x => x.Id > 1).ToList();
            var listCountry = _countryRepository.Table.Where(x => x.Id > 0).ToList();
            var isFailded = false;
            var countiesNameCollection = new List<string>();
            try
            {
                foreach (GeoCodefileupload file in zipCodeFileList)
                {
                    fileId = file.Id;
                    newlyAddedZipCodes = 0;
                    var countyCollection = new List<CountyCreateEditModel>();
                    var zipCollection = new List<ZipCreateEditModel>();
                    file.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
                    _geoCodeFileRepository.Save(file);
                    // _unitOfWork.SaveChanges();

                    var filePath = MediaLocationHelper.FilePath(file.File.RelativeLocation, file.File.Name).ToFullPath();
                    geoCodeParsedFile = file;
                    var sheetIds = ZipExcelFileParser.GetSheetIds(filePath);
                    string message;
                    CreateLogFile(sb, "GeoCode_" + fileId);
                    CreateLogFile(sb, "GeoCode_Counties_" + fileId);
                    CreateLogFile(sb, "GeoCode_ZipCode_" + fileId);
                    SaveZipFileUpload(geoCodeParsedFile, stats);
                    var isErrorExist = false;
                    var countiesCollection = _countyRepository.Table.ToList();
                    var zipCodesCollection = _zipRepository.Table.ToList();
                    var totalCountiesInFile = default(long?);
                    var totalCountiesToBeUpdated = default(long?);
                    var totalZipsInFile = default(long?);
                    var totalZipsToBeUpdated = default(long?);
                    var countiesList = new List<string>();
                    foreach (var sheetId in sheetIds)
                    {
                        if (isErrorExist)
                        {
                            break;
                        }
                        if (isCounty == 0)
                        {
                            ++isCounty;
                            _logService.Info("Started recording county data!");

                            data = ZipExcelFileParser.ReadExcelZip(filePath, sheetId);
                            var zipFileParser = ApplicationManager.DependencyInjection.Resolve<IZipFileParser>();
                            countyCollection = zipFileParser.PrepareDomainFromDataTableForCounty(data).ToList();
                            totalCountiesInFile = countiesCollection.Count();

                            if (!zipFileParser.CheckForValidCountyHeader(data, out message))
                            {
                                isFailded = true;
                                stats.Logs.Append(Log("Please upload correct File! " + message));
                                sb.Append(stats.Logs);
                                CreateLogFile(sb, "GeoCode_" + geoCodeParsedFile.Id);
                                MarkUploadAsFailed(file, stats);
                                isErrorExist = true;
                                break;
                            }
                            stats.LogsForCounty.Append(Log("Counties in Database Before Parsing File: " + totalCountiesInFile));
                            //  sbForCounty.Append(stats.LogsForCounty);

                            sb.Append(Log(string.Format("Parsing County Data")));
                            countyCollection = countyCollection.Where(x => x.IsUpdated == 1 || x.IsDeleted == 1).ToList();
                            totalCountiesToBeUpdated = countyCollection.Count();

                            stats.LogsForCounty.Append(Log("Counties to be Updated: " + totalCountiesToBeUpdated));
                            sbForCounty.Append(stats.LogsForCounty);

                            if (countyCollection.Count() > 0)
                                countyIds.AddRange(countyCollection.Select(x => x.Id));

                            foreach (var record in countyCollection)
                            {
                                if (!countyViewModel.Any(x => x.CountyName == record.CountyName && x.StateCode == record.StateCode))
                                {
                                    countyViewModel.Add(new CountyViewModel() { CountyName = record.CountyName, StateCode = record.StateCode });

                                }
                                else
                                {
                                    doublicateCountyViewModel.Add(new CountyViewModel() { CountyName = record.CountyName, StateCode = record.StateCode, Id = record.Id });
                                }
                                currentCountyName = record.CountyName;
                                if ((record.IsUpdated == 1 && record.Id > 0 && record.IsDeleted == 0) || (record.Id == 0 && record.IsUpdated == 1 && record.IsDeleted == 0))
                                {
                                    if (record.Id == 0)
                                    {
                                        stats.Logs.Append(Log(string.Format("Adding New County: {0}", record.CountyName)));
                                        ++newlyAddedCounties;
                                    }
                                    else
                                    {
                                        ++updatedCounties;
                                        stats.Logs.Append(Log(string.Format("Updating County: {0}", record.CountyName)));
                                    }
                                    _unitOfWork.StartTransaction();
                                    stats = SaveModel(listFranchisee, listCountry, true, stats, zipCodesCollection, record, null);
                                    _unitOfWork.SaveChanges();
                                }
                                else if ((record.IsUpdated == 0 && record.Id > 0 && record.IsDeleted == 1))
                                {
                                    ++deletedCounties;
                                    stats.Logs.Append(Log(string.Format("Deleting County: {0}", record.CountyName)));
                                    var countyDomain = countiesCollection.Where(x => x.Id == record.Id).FirstOrDefault();
                                    if (countyDomain != default(County))
                                    {
                                        _unitOfWork.StartTransaction();
                                        _countyRepository.Delete(countyDomain);
                                        sb.Append(stats.Logs);
                                        _unitOfWork.SaveChanges();
                                    }

                                }
                            }
                            stats.LogsForCounty.Append(Log("Counties newly added: " + newlyAddedCounties));
                            stats.LogsForCounty.Append(Log("Counties updated: " + updatedCounties));
                            stats.LogsForCounty.Append(Log("Counties deleted: " + deletedCounties));
                            if (doublicateCountyViewModel.Count() > 0)
                            {
                                stats.LogsForCounty.Append(Log("Total Number Of Duplicate Counties: " + doublicateCountyViewModel.Count()));
                                stats.LogsForCounty.Append(Log("Following are the  Duplicate Counties: "));
                                foreach (var countyViewModelList in doublicateCountyViewModel)
                                {
                                    stats.LogsForCounty.Append(Log("CountyName " + countyViewModelList.CountyName + " in  State " + countyViewModelList.StateCode + " having id " + countyViewModelList.Id));
                                }
                            }

                            sbForCounty.Append(stats.LogsForCounty);
                            currentCountyName = "";
                            _logService.Info("finished recording county data!");
                        }
                        else if (isCounty == 1)
                        {
                            isCounty++;
                            _logService.Info("Started recording Zip data!");
                            var index = 0;
                            data = ZipExcelFileParser.ReadExcelZip(filePath, sheetId);
                            
                            var zipFileParser = ApplicationManager.DependencyInjection.Resolve<IZipFileParser>();
                            if (!zipFileParser.CheckForValidZipHeader(data, out message))
                            {
                                // _unitOfWork.StartTransaction();
                                stats.Logs.Append(Log("Please upload correct File! " + message));
                                sb.Append(stats.Logs);
                                CreateLogFile(sb, "GeoCode_" + geoCodeParsedFile.Id);
                                isFailded = true;
                                MarkUploadAsFailed(file, stats);
                                _unitOfWork.SaveChanges();
                                isErrorExist = true;
                                break;
                            }

                            zipCollection = zipFileParser.PrepareDomainFromDataTableForZip(data).ToList();
                            countiesList = zipCollection.Select(x => x.countyName).ToList();
                            totalZipsInFile = zipCollection.Count();
                            zipCollection = zipCollection.Where(x => (countyCollection.Select(x1 => x1.CountyName).Contains(x.countyName)
                            && countyCollection.Select(x1 => x1.StateCode).Contains(x.StateCode)) || x.IsDeleted == 1).ToList();


                            totalZipsToBeUpdated = zipCollection.Count();
                            stats.LogsForZipCode.Append(Log("Zip Codes in Database Before Parsing File: " + zipCodesCollection.Count()));
                            foreach (var record in zipCollection)
                            {
                                index = index + 1;
                                currentZipCodeName = record.ZipCode;
                                if ((record.Id > 0 && record.IsDeleted == 0) || (record.Id == 0 && record.IsDeleted == 0))
                                {
                                    if (record.Id == 0)
                                    {
                                        //++newlyAddedZipCodes;
                                        stats.Logs.Append(Log(string.Format("Adding New ZipCode: {0}", record.ZipCode)));
                                    }
                                    else
                                    {
                                        ++updatedZipCodes;
                                        stats.Logs.Append(Log(string.Format("Updating ZipCode: {0}", record.ZipCode)));
                                    }
                                    //_unitOfWork.StartTransaction();
                                    stats = SaveModel(listFranchisee, listCountry, false, stats, zipCodesCollection, null, record);
                                    //_unitOfWork.SaveChanges();
                                }
                                else if ((record.Id > 0 && record.IsDeleted == 1))
                                {
                                    ++deletedZipCodes;
                                    var zipCodeDomain = zipCodesCollection.FirstOrDefault(x => x.Id == record.Id);
                                    stats.Logs.Append(Log(string.Format("Deleting ZipCode: {0}", record.ZipCode)));
                                    if (zipCodeDomain != default(ZipCode))
                                    {
                                        //    _unitOfWork.StartTransaction();
                                        _zipRepository.Delete(zipCodeDomain);
                                        sb.Append(stats.Logs);
                                        //_unitOfWork.SaveChanges();
                                    }
                                }
                            }
                            currentZipCodeName = "";
                            stats.LogsForZipCode.Append(Log("ZipCodes newly added: " + newlyAddedZipCodes));
                            stats.LogsForZipCode.Append(Log("ZipCodes updated: " + updatedZipCodes));
                            stats.LogsForZipCode.Append(Log("ZipCodes deleted: " + deletedZipCodes));
                            bool firstTime = true;
                            foreach (var county in countyViewModel)
                            {

                                if (!countiesList.Contains(county.CountyName))
                                {
                                    if (firstTime)
                                    {
                                        firstTime = false;
                                        stats.LogsForZipCode.Append(Log("Counties Present in County Tab but not in ZipCode Tab are:"));
                                    }
                                    stats.LogsForZipCode.Append(Log(county.CountyName));
                                }
                            }

                            _logService.Info("Finished recording Zip data!");

                        }
                    }

                    if (countyCollection.Count() == 0 && zipCollection.Count() == 0)
                    {
                        stats.Logs.Append(Log(string.Format("No Records Found in Zip and County Sheet")));
                    }
                    CreateLogFile(stats.LogsForCounty, "GeoCode_Counties_" + fileId);
                    CreateLogFile(stats.LogsForZipCode, "GeoCode_ZipCode_" + fileId);
                    CreateLogFile(stats.Logs, "GeoCode_" + fileId);
                    isErrorExist = false;

                    if (!isFailded)
                    {
                        isFailded = false;
                        MarkUploadAsSuccess(file);
                    }
                    if (isFailded)
                    {
                        MarkUploadAsFailed(file, stats);
                    }
                    _logService.Info("Ending Geo Code Notification");
                    _unitOfWork.SaveChanges();
                }
            }
            catch (InvalidFileUploadException ex)
            {

                geoCodeParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                geoCodeParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                //stats.sb.Append(sb);
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "GeoCode_" + fileId);
                MarkUploadAsFailed(geoCodeParsedFile, stats);
                _logService.Info(string.Format("Error in Geo Code : ", ex));
                //_unitOfWork.SaveChanges();
            }
            catch (IndexOutOfRangeException ex)
            {
                if (currentZipCodeName != "")
                    stats.Logs.Append(Log(string.Format("Exception in  ZipCode {0}", currentZipCodeName)));
                else if (currentCountyName != "")
                    stats.Logs.Append(Log(string.Format("Exception in  County {0}", currentCountyName)));

                geoCodeParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                geoCodeParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                stats.Logs.Append(sb);
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "GeoCode_" + fileId);

                MarkUploadAsFailed(geoCodeParsedFile, stats);
                _logService.Info(string.Format("Error in Geo Code : ", ex));
                //_unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                if (currentZipCodeName != "")
                    stats.Logs.Append(Log(string.Format("Exception in  ZipCode {0}", currentZipCodeName)));
                else if (currentCountyName != "")
                    stats.Logs.Append(Log(string.Format("Exception in  County {0}", currentCountyName)));

                geoCodeParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                geoCodeParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                stats.Logs.Append(sb);

                MarkUploadAsFailed(geoCodeParsedFile, stats);
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "GeoCode_" + fileId);
                _logService.Info(string.Format("Error in Geo Code : ", ex));
                // _unitOfWork.SaveChanges();
            }
            finally
            {
                _unitOfWork.SaveChanges();
            }
        }

        private void SaveZipFileUpload(GeoCodefileupload customerFileUpload, SaveModelStats stats)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                //   _unitOfWork.StartTransaction();
                var fileModel = PrepareLogFileModel("GeoCode_" + customerFileUpload.Id);
                var fileModelForCounties = PrepareLogFileModel("GeoCode_Counties_" + customerFileUpload.Id);
                var fileModelForZipCode = PrepareLogFileModel("GeoCode_ZipCode_" + customerFileUpload.Id);
                var file = _fileService.SaveModel(fileModel);
                var fileForCounties = _fileService.SaveModel(fileModelForCounties);
                var fileForZipCode = _fileService.SaveModel(fileModelForZipCode);
                customerFileUpload.ParsedLogFileId = file.Id;
                customerFileUpload.ParsedLogForCountyFileId = fileForCounties.Id;
                customerFileUpload.ParsedLogForZipFileId = fileModelForZipCode.Id;
                _geoCodeFileRepository.Save(customerFileUpload);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                stats.sb.Append(sb);
                LogException(stats.sb, ex);
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
        private SaveModelStats SaveModel(ICollection<Franchisee> listFranchisee, ICollection<Country> listCountry, bool isCounty
                            , SaveModelStats stats, List<ZipCode> zipCodeCollection, CountyCreateEditModel countyModel = null, ZipCreateEditModel zipModel = null)
        {
            StringBuilder sb = new StringBuilder();
            var countyDomain = new County();
            var zipCodeDomain = new ZipCode();

            //stats.Logs = new StringBuilder();
            string name = "";
            try
            {
                if (isCounty)
                {
                    if (countyModel.Id > 0)
                    {
                        countyDomain = _countyRepository.Get(countyModel.Id);
                        if (countyDomain == null) return stats;
                    }
                    var domain = CreateModel(listFranchisee, listCountry, countyModel);
                    name = domain.CountyName;
                    _countyRepository.Save(domain);
                    stats.CountyId = domain.Id;
                    _logService.Info("Updating County " + countyModel.CountyName);
                }
                else
                {
                    if (zipModel.Id > 0)
                    {
                        zipCodeDomain = zipCodeCollection.FirstOrDefault(x => x.Zip == zipModel.ZipCode && x.StateCode == zipModel.StateCode);
                        if (zipCodeDomain == null)
                        {
                            zipModel.Id = 0;
                        }
                        else
                        {
                            zipModel.Id = zipCodeDomain.Id;
                        }
                    }
                    else
                    {
                      
                        zipModel.Id = 0;
                    }
                    name = zipModel.ZipCode;
                    var domains = CreateModel(listFranchisee, listCountry, zipModel);
                    foreach (var domain in domains)
                    {
                        if (domain.Id == 0)
                        {
                            ++newlyAddedZipCodes;
                        }
                        name = domain.Zip;
                        _zipRepository.Save(domain);
                        stats.CountyId = domain.Id;
                        _logService.Info("Adding ZipCode " + zipModel.ZipCode);
                    }
                }
            }
            catch (Exception ex)
            {
                if (isCounty)
                {
                    sb.Append(stats.Logs);
                    stats.Logs.Append(Log(string.Format("Error in Parsing County Data {0} with error {1} with county name {2}", name, ex.StackTrace, countyModel.CountyName)));
                    LogException(sb, ex);
                    _logService.Error("Error Saving County" + countyModel.CountyName + " " + ex.StackTrace);
                }
                else
                {
                    stats.Logs.Append(Log(string.Format("Error in Parsing Zip Code Data {0} with error {1}", name, ex.StackTrace)));
                    _logService.Error("Error Saving ZipCode" + zipModel.ZipCode + " " + ex.StackTrace);
                }
            }
            return stats;
        }

        class SaveModelStats
        {
            public long CountyId;
            public StringBuilder Logs;
            public StringBuilder LogsForCounty;
            public StringBuilder LogsForZipCode;
            public StringBuilder sb;
            public StringBuilder sbForCounties;
            public StringBuilder sbForZipCode;
        }
        private IEnumerable<GeoCodefileupload> GetiZipDataToParse()
        {
            //Check if parsed and not exist in FranchiseeInvoice
            var zipDataUploadIds = _geoCodeFileRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded).ToList();
            return zipDataUploadIds;
        }

        private County CreateModel(ICollection<Franchisee> listFranchisee, ICollection<Country> listCountry, CountyCreateEditModel model)
        {
            var countryDomain = new Country();
            var countryId = listCountry.Where(x => x.Name.ToUpper().Equals(model.CountryName.ToUpper())).Select(x => x.Id).FirstOrDefault();
            var franchiseeId = !string.IsNullOrEmpty(model.FranchiseeName) ? GetFranchiseeId(listFranchisee, model.FranchiseeName) : null;
            if (countryId != default(long))
                countryDomain = _countryRepository.Get(countryId);
            return new County
            {
                ContractedTerritory = model.ContractedTerritory,
                Id = model.Id,
                IsNew = model.Id > 0 && model.IsUpdated == 1 ? false : true,
                StateCode = model.StateCode,
                Status = model.Status,
                CountryId = countryId == 0 ? default(long?) : countryId,
                CoveringLessThan3Hours = model.CoveringLessThan3Hours,
                ReachingTime = model.ReachingTime,
                DayTrip = model.DayTrip,
                DirectionCode = model.DirectionCode,
                DirectionFromOffice = model.DirectionFromOffice,
                FranchiseeName = model.FranchiseeName,
                //FranchiseMLD = model.FranchiseMLD,
                FranchiseeId = franchiseeId,
                Population = model.Population,
                StateCountryCode = model.StateCountryCode,
                TerritoryCode = model.TerritoryCode,
                UnCovered = model.UnCovered,
                CountyName = model.CountyName,
                //Country = countryDomain != default(Country) && ? countryDomain : null
            };
        }

        private long? GetFranchiseeId(ICollection<Franchisee> listFranchisee, string franchiseeName)
        {
            var franchisee = listFranchisee.FirstOrDefault(x => (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").StartsWith(franchiseeName.Trim().ToUpper())
                                          || (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").EndsWith(franchiseeName.Trim().ToUpper())
                                          || (x.Organization.Name.ToUpper()).Equals(franchiseeName.ToUpper())
                                          || (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").Equals(franchiseeName.Trim(), StringComparison.CurrentCultureIgnoreCase));
            if (franchisee != null)
                return franchisee.Id;

            return null;
        }

        private List<ZipCode> CreateModel(ICollection<Franchisee> listFranchisee, ICollection<Country> listCountry, ZipCreateEditModel model)
        {
            List<ZipCode> zipCodes = new List<ZipCode>();
            var countyString = !string.IsNullOrEmpty(model.countyName)
                ? model.countyName.Replace("-N", "").Replace("-S", "").Replace("-W", "").Replace("-E", "").Replace("-C", "") : null;


            if (model.Id == 0)
            {
                var counties = !string.IsNullOrEmpty(countyString)
                ? _countyRepository.Table.Where(x => x.CountyName.Equals(countyString.Trim()) && x.StateCode.Equals(model.StateCode)).ToList()
                : null;

                zipCodes.AddRange(GetZipCodeModel(counties, model));
            }
            else
            {
                var zipCode = _zipRepository.Table.FirstOrDefault(x => x.Zip == model.ZipCode && x.StateCode == model.StateCode);
                var counties = !string.IsNullOrEmpty(countyString)
                   ? _countyRepository.Table.Where(x => x.Id == zipCode.CountyId).ToList()
                   : null;
                if (counties.Count() == 0)
                {
                    counties = _countyRepository.Table.Where(x => x.CountyName == zipCode.CountyName && x.StateCode == zipCode.StateCode).ToList();
                }
                if (counties != null)
                    zipCodes.AddRange(GetZipCodeModel(counties, model));
            }
            return zipCodes;


        }
        private static void LogException(StringBuilder sb, Exception ex)
        {
            sb.Append(Log("Error - " + ex.Message));
            sb.Append(Log("Error - " + ex.StackTrace));
            if (ex.InnerException != null && ex.InnerException.StackTrace != null)
                sb.Append(Log("Error - " + ex.InnerException.StackTrace));
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
                file.Write(sb);
            }
        }

        private void MarkUploadAsFailed(GeoCodefileupload geoCodeFile, SaveModelStats stats)
        {
            var countiesCount = _countyRepository.Table.Count();
            var zipCodeCount = _zipRepository.Table.Count();
            geoCodeFile.CountiesCount = countiesCount;
            geoCodeFile.ZipCodeCount = zipCodeCount;
            geoCodeFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            geoCodeFile.StatusId = (long)SalesDataUploadStatus.Failed;
            _geoCodeFileRepository.Save(geoCodeFile);
            // SaveZipFileUpload(geoCodeFile, stats);
        }
        private void MarkUploadAsSuccess(GeoCodefileupload geoCodeFile)
        {
            var countiesCount = _countyRepository.Table.Count();
            var zipCodeCount = _zipRepository.Table.Count();
            geoCodeFile.CountiesCount = countiesCount;
            geoCodeFile.ZipCodeCount = zipCodeCount;
            geoCodeFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            geoCodeFile.StatusId = (long)SalesDataUploadStatus.Parsed;
            _geoCodeFileRepository.Save(geoCodeFile);
            // SaveZipFileUpload(geoCodeFile, stats);
        }
        private List<ZipCode> GetZipCodeModel(List<County> counties, ZipCreateEditModel model)
        {
            string cityName = model.CityName;
            string countyName = model.countyName;
            var countyId = default(long?);
            List<ZipCode> zipCodes = new List<ZipCode>();
            if (counties != null)
            {
                foreach (var county in counties)
                {
                    if (county != null)
                    {
                        model.countyId = county.Id;
                        countyName = county.CountyName;
                    }

                    var city = _cityRepository.Get(x => x.Name.Equals(model.CityName) && x.State.ShortName.Equals(model.StateCode));

                    if (city != null)
                    {
                        model.CityId = city.Id;
                        cityName = city.Name;
                    }


                    zipCodes.Add(new ZipCode
                    {
                        Zip = model.ZipCode,
                        Id = model.Id,
                        IsNew = model.Id > 0 ? false : true,
                        AreaCode = model.AreaCode,
                        Code = model.Code,
                        DriveTest = null,
                        Direction = model.Direction,
                        CountyId = (model.countyId == null || model.countyId <= 0) ? null : model.countyId,
                        StateCode = model.StateCode,
                        Dir = model.Dir,
                        CityId = model.CityId,
                        CityName = cityName,
                        CountyName = countyName,
                        TransferableNumber = model.FranchiseeTransferableNumber
                    });
                }
            }
            else
            {
                var city = _cityRepository.Get(x => x.Name.Equals(model.CityName) && x.State.ShortName.Equals(model.StateCode));

                if (city != null)
                {
                    model.CityId = city.Id;
                    cityName = city.Name;
                }


                zipCodes.Add(new ZipCode
                {
                    Zip = model.ZipCode,
                    Id = model.Id,
                    IsNew = model.Id > 0 ? false : true,
                    AreaCode = model.AreaCode,
                    Code = model.Code,
                    DriveTest = null,
                    Direction = model.Direction,
                    CountyId = null,
                    StateCode = model.StateCode,
                    Dir = model.Dir,
                    CityId = model.CityId,
                    CityName = cityName,
                    CountyName = "",
                    TransferableNumber = model.FranchiseeTransferableNumber
                });
            }
            return zipCodes;
        }


        public class ZipParsingModel
        {
            public ZipParsingType ParsingType { get; set; }
            public long? Id { get; set; }
            public string EntityName { get; set; }
        }
        public class CountyViewModel
        {
            public long? Id { get; set; }
            public string CountyName { get; set; }
            public string StateCode { get; set; }
        }
    }
}
