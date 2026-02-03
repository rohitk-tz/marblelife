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

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class PriceEstimateParserNotificationService : IPriceEstimateParserNotificationService
    {
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        //private readonly IJobFactory _jobFactory;
        private readonly IRepository<PriceEstimateFileUpload> _priceEstimateFileUploadRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<PriceEstimateServices> _priceEstimateServicesRepository;
        private readonly IRepository<ServicesTag> _servicesTagRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private IFileService _fileService;
        private int newlyAddedPriceEstimates = 0;
        public PriceEstimateParserNotificationService(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _fileService = fileService;
            _priceEstimateFileUploadRepository = unitOfWork.Repository<PriceEstimateFileUpload>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _priceEstimateServicesRepository = unitOfWork.Repository<PriceEstimateServices>();
            _servicesTagRepository = unitOfWork.Repository<ServicesTag>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
        }

        class PriceEstimateSaveModelStats
        {
            public StringBuilder Logs;
            public StringBuilder sb;
            public long PriceEstimateId;
            public long ServiceTagId;
        }

        public class PriceEstimateUploadViewModel
        {
            public long? Id { get; set; }
            public string Service { get; set; }
            public string ServiceType { get; set; }
            public string MaterialType { get; set; }
            public string Category { get; set; }
            public string FranchiseeName { get; set; }
        }

        public void ProcessRecords()
        {
            ProcessRecordsForFA();
            ProcessRecordsForSA();
        }

        public void ProcessRecordsForFA()
        {
            _unitOfWork.StartTransaction();
            var stats = new PriceEstimateSaveModelStats();
            var priceEstimateIds = new List<long>();
            var priceEstimatesViewModel = new List<PriceEstimateUploadViewModel>();
            var duplicatePriceEstimatesViewModel = new List<PriceEstimateUploadViewModel>();
            stats.Logs = new StringBuilder();
            var sb = new StringBuilder();
            var updatedPriceEstimates = 0;
            var deletedPriceEstimates = 0;
            long currentIdInFile = default(long);
            _logService.Info("Starting Price Estimate Code File Parsing");
            var priceEstimateFileList = GetPriceEstimateDataToParse();
            if (priceEstimateFileList == null || !priceEstimateFileList.Any())
            {
                _logService.Debug("No file found for parsing price estimate data.");
                return;
            }
            DataTable data;
            var priceEstimateParsedFile = new PriceEstimateFileUpload();
            long? fileId = default(long?);
            var isFailed = false;
            try
            {
                foreach (PriceEstimateFileUpload file in priceEstimateFileList)
                {
                    fileId = file.Id;
                    newlyAddedPriceEstimates = 0;
                    var priceEstimateCollection = new List<PriceEstimateUploadEditModel>();
                    file.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
                    _priceEstimateFileUploadRepository.Save(file);
                    _unitOfWork.SaveChanges();
                    var filePath = MediaLocationHelper.FilePath(file.File.RelativeLocation, file.File.Name).ToFullPath();
                    priceEstimateParsedFile = file;
                    var sheetIds = PriceEstimateExcelFileParser.GetSheetIds(filePath);
                    string message;
                    CreateLogFile(sb, "PriceEstimate_" + fileId);
                    SavePriceEstimateFileUpload(priceEstimateParsedFile, stats);
                    var isErrorExist = false;
                    var totalPriceEstimatesInFile = default(long?);
                    var totalPriceEstimatesToBeUpdated = default(long?);
                    foreach (var sheetId in sheetIds)
                    {
                        if (isErrorExist)
                        {
                            break;
                        }
                        _logService.Info("Started recording Price estimate data!");
                        data = PriceEstimateExcelFileParser.ReadExcelZip(filePath, sheetId);
                        var priceEstimateFileParser = ApplicationManager.DependencyInjection.Resolve<IPriceEstimateFileParser>();
                        priceEstimateCollection = priceEstimateFileParser.PrepareDomainFromDataTable(data, false).ToList();
                        totalPriceEstimatesInFile = priceEstimateCollection.Count();
                        if (!priceEstimateFileParser.CheckForValidHeader(data, out message))
                        {
                            isFailed = true;
                            stats.Logs.Append(Log("Please upload correct File! " + message));
                            sb.Append(stats.Logs);
                            CreateLogFile(sb, "PriceEstimate_" + priceEstimateParsedFile.Id);
                            MarkUploadAsFailed(file, stats);
                            isErrorExist = true;
                            break;
                        }
                        stats.Logs.Append(Log("Total Price estimate data in Uploaded file: " + totalPriceEstimatesInFile));
                        priceEstimateCollection = priceEstimateCollection.Where(x => x.IsUpdated == 1 || x.IsDeleted == 1).ToList();
                        totalPriceEstimatesToBeUpdated = priceEstimateCollection.Count();
                        stats.Logs.Append(Log("Total Price estimate data needs to be updated is: " + totalPriceEstimatesToBeUpdated));
                        sb.Append(Log(string.Format("Parsing Price estimate data")));
                        if (priceEstimateCollection.Count() > 0)
                            priceEstimateIds.AddRange(priceEstimateCollection.Select(x => x.Id));

                        foreach (var record in priceEstimateCollection)
                        {
                            currentIdInFile = record.Id;
                            if (record.Id > 0 && priceEstimatesViewModel.Any(x => x.Id == record.Id))
                            {
                                duplicatePriceEstimatesViewModel.Add(new PriceEstimateUploadViewModel() { Service = record.Service, ServiceType = record.ServiceType, MaterialType = record.MaterialType, Category = record.Category, FranchiseeName = record.FranchiseeName, Id = record.Id });
                                record.IsDuplicate = true;
                            }
                            else if (!priceEstimatesViewModel.Any(x => x.Service == record.Service && x.ServiceType == record.ServiceType && x.MaterialType == record.MaterialType && x.Category == record.Category && x.FranchiseeName == record.FranchiseeName))
                            {
                                priceEstimatesViewModel.Add(new PriceEstimateUploadViewModel() { Id = record.Id, Service = record.Service, ServiceType = record.ServiceType, MaterialType = record.MaterialType, Category = record.Category, FranchiseeName = record.FranchiseeName });
                            }
                            else
                            {
                                duplicatePriceEstimatesViewModel.Add(new PriceEstimateUploadViewModel() { Service = record.Service, ServiceType = record.ServiceType, MaterialType = record.MaterialType, Category = record.Category, FranchiseeName = record.FranchiseeName, Id = record.Id });
                                record.IsDuplicate = true;
                            }
                            if (record.IsDuplicate)
                            {
                                break;
                            }
                            if (record.Category == "TIME")
                            {
                                if (record.BulkCorporatePrice >= 0)
                                {
                                    record.BulkCorporatePrice = null;
                                }
                                if (record.BulkCorporateAdditionalPrice >= 0)
                                {
                                    record.BulkCorporateAdditionalPrice = null;
                                }
                            }
                            if ((record.IsUpdated == 1 && record.Id > 0 && record.IsDeleted == 0) || (record.Id == 0 && record.IsUpdated == 1 && record.IsDeleted == 0))
                            {
                                if (record.Id == 0)
                                {
                                    if (record.FranchiseePrice > 0 || record.FranchiseeAdditionalPrice > 0)
                                    {
                                        if (record.FranchiseeName != null)
                                        {

                                            stats.Logs.Append(Log(string.Format("Adding New Price estimate data: Franchisee {0} Service {1}, Material {2}", record.FranchiseeName, record.Service, record.MaterialType)));
                                            stats = SaveModel(stats, priceEstimateCollection, record);
                                            ++newlyAddedPriceEstimates;
                                        }
                                        else
                                        {
                                            List<long> assignedFranchiseeIdList = _organizationRoleUserRepository.Table.Where(x => x.Id == file.DataRecorderMetaData.CreatedBy).Select(x => x.OrganizationId).ToList();
                                            List<Franchisee> frachiseeList = _franchiseeRepository.Table.Where(x => assignedFranchiseeIdList.Contains(x.Organization.Id) && x.Id != 2 && !x.Organization.Name.StartsWith("0-")).ToList();
                                            foreach (var franchisee in frachiseeList)
                                            {
                                                record.FranchiseeName = franchisee.Organization.Name;
                                                stats.Logs.Append(Log(string.Format("Adding New Price estimate data: Franchisee {0}, Service {1}, Material {2}", record.FranchiseeName, record.Service, record.MaterialType)));
                                                stats = SaveModel(stats, priceEstimateCollection, record);
                                                ++newlyAddedPriceEstimates;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        stats.Logs.Append(Log(string.Format("You have not added Price estimate data for Columns P or Q: Franchisee {0}, Service {1}, Material {2}", record.FranchiseeName, record.Service, record.MaterialType)));
                                    }
                                }
                                else
                                {
                                    var priceEstimateDomain = _priceEstimateServicesRepository.Table.Where(x => x.Id == record.Id).FirstOrDefault();
                                    if (priceEstimateDomain.FranchiseeAdditionalPrice != record.FranchiseeAdditionalPrice || priceEstimateDomain.FranchiseePrice != record.FranchiseePrice)
                                    {
                                        stats = SaveModel(stats, priceEstimateCollection, record);
                                        stats.Logs.Append(Log(string.Format("Updating Price estimate data: Franchisee {0}, Service {1}, Material {2}", priceEstimateDomain.Franchisee.Organization.Name, priceEstimateDomain.ServicesTag.Service, priceEstimateDomain.ServicesTag.MaterialType)));
                                        ++updatedPriceEstimates;
                                    }
                                    else
                                    {
                                        stats.Logs.Append(Log(string.Format("You have not updated Price estimate data for Columns P or Q: Franchisee {0}, Service {1}, Material {2}", priceEstimateDomain.Franchisee.Organization.Name, priceEstimateDomain.ServicesTag.Service, priceEstimateDomain.ServicesTag.MaterialType)));
                                    }
                                }
                                if (record.Category == "TIME")
                                {
                                    if (record.BulkCorporatePrice >= 0)
                                    {
                                        record.BulkCorporatePrice = null;
                                    }
                                    if (record.BulkCorporateAdditionalPrice >= 0)
                                    {
                                        record.BulkCorporateAdditionalPrice = null;
                                    }
                                }
                                _unitOfWork.StartTransaction();
                                _unitOfWork.SaveChanges();
                            }
                            else if ((record.IsUpdated == 0 && record.IsDeleted == 1))
                            {
                                if (record.Id > 0)
                                {
                                    var priceEstimateDomain = _priceEstimateServicesRepository.Table.Where(x => x.Id == record.Id).FirstOrDefault();
                                    stats.Logs.Append(Log(string.Format("Deleting Price estimate data: Franchisee {0}, Service {1}, Material {2}", priceEstimateDomain.Franchisee.Organization.Name, priceEstimateDomain.ServicesTag.Service, priceEstimateDomain.ServicesTag.MaterialType)));
                                    if (priceEstimateDomain != default(PriceEstimateServices))
                                    {
                                        _unitOfWork.StartTransaction();
                                        priceEstimateDomain.IsNew = false;
                                        priceEstimateDomain.FranchiseePrice = priceEstimateDomain.CorporatePrice;
                                        priceEstimateDomain.FranchiseeAdditionalPrice = priceEstimateDomain.CorporateAdditionalPrice;
                                        priceEstimateDomain.IsPriceChangedByFranchisee = false;
                                        _priceEstimateServicesRepository.Save(priceEstimateDomain);
                                        sb.Append(stats.Logs);
                                        _unitOfWork.SaveChanges();
                                    }
                                    ++deletedPriceEstimates;
                                }
                                else
                                {
                                    stats.Logs.Append(Log(string.Format("Record cannot be deleted before it's added to the system(Id=0): Franchisee {0}, Service {1}, Material {2}", record.FranchiseeName, record.Service, record.MaterialType)));
                                }
                            }
                        }
                        currentIdInFile = default(long);
                        stats.Logs.Append(Log("Price Estimates newly added: " + newlyAddedPriceEstimates));
                        stats.Logs.Append(Log("Price Estimates updated: " + updatedPriceEstimates));
                        stats.Logs.Append(Log("Price Estimates deleted: " + deletedPriceEstimates));
                        if (duplicatePriceEstimatesViewModel.Count() > 0)
                        {
                            stats.Logs.Append(Log("Total Number Of Duplicate Price Estimates: " + duplicatePriceEstimatesViewModel.Count()));
                            stats.Logs.Append(Log("Following are the  Duplicate Price Estimates: "));
                            foreach (var priceEstimate in duplicatePriceEstimatesViewModel)
                            {
                                stats.Logs.Append(Log("Service: " + priceEstimate.Service + " ServiceType: " + priceEstimate.ServiceType + " Material Type: " + priceEstimate.MaterialType + " Category: " + priceEstimate.Category + " for Franchisee: " + priceEstimate.FranchiseeName + " having Id: " + priceEstimate.Id));
                            }
                        }
                        sb.Append(stats.Logs);
                        _logService.Info("Finished recording Price Estimate data!");
                    }
                    if (priceEstimateCollection.Count() == 0)
                    {
                        stats.Logs.Append(Log(string.Format("No Records Found to Update/Add/Delete in Price Estimate Sheet (FileId : " + priceEstimateParsedFile.FileId + ")")));
                    }
                    CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                    isErrorExist = false;
                    if (!isFailed)
                    {
                        isFailed = false;
                        MarkUploadAsSuccess(file);
                    }
                    if (isFailed)
                    {
                        MarkUploadAsFailed(file, stats);
                    }
                    _logService.Info("Ending Price Estimate Data Upload Service");
                    _unitOfWork.SaveChanges();
                }
            }
            catch (InvalidFileUploadException ex)
            {
                priceEstimateParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                priceEstimateParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                MarkUploadAsFailed(priceEstimateParsedFile, stats);
                _logService.Info(string.Format("Error in Price Estimate File Upload: ", ex));
            }
            catch (IndexOutOfRangeException ex)
            {
                if (currentIdInFile != default(long))
                    stats.Logs.Append(Log(string.Format("Exception in  Price Estimate Id- {0}", currentIdInFile)));
                priceEstimateParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                priceEstimateParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                stats.Logs.Append(sb);
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                MarkUploadAsFailed(priceEstimateParsedFile, stats);
                _logService.Info(string.Format("Error in Price Estimate : ", ex));
            }
            catch (Exception ex)
            {
                if (currentIdInFile != default(long))
                    stats.Logs.Append(Log(string.Format("Exception in  Price Estimate Id- {0}", currentIdInFile)));
                priceEstimateParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                priceEstimateParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                stats.Logs.Append(sb);
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                MarkUploadAsFailed(priceEstimateParsedFile, stats);
                _logService.Info(string.Format("Error in Price Estimate: ", ex));
            }
            finally
            {
                _unitOfWork.SaveChanges();
            }
        }

        private IEnumerable<PriceEstimateFileUpload> GetPriceEstimateDataToParse()
        {
            //Check if parsed and not exist in FranchiseeInvoice
            var priceEstimateUploadIds = _priceEstimateFileUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded && x.IsFranchiseeAdmin).ToList();
            return priceEstimateUploadIds;
        }

        private IEnumerable<PriceEstimateFileUpload> GetPriceEstimateDataToParseSA()
        {
            //Check if parsed and not exist in FranchiseeInvoice
            var priceEstimateUploadIds = _priceEstimateFileUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded && !x.IsFranchiseeAdmin).ToList();
            return priceEstimateUploadIds;
        }

        private PriceEstimateSaveModelStats SaveModel(PriceEstimateSaveModelStats stats, List<PriceEstimateUploadEditModel> priceEstimateCollection, PriceEstimateUploadEditModel recordForSave)
        {
            StringBuilder sb = new StringBuilder();
            var priceEstimateDomain = new PriceEstimateServices();
            string name = "";
            long id = default(long);
            try
            {
                if (recordForSave.Id > 0)
                {
                    priceEstimateDomain = _priceEstimateServicesRepository.Get(recordForSave.Id);
                    if (priceEstimateDomain == null)
                        recordForSave.Id = 0;
                }
                var domain = CreateModel(recordForSave);
                name = domain.ServicesTag != null ? domain.ServicesTag.Service : string.Empty;
                _priceEstimateServicesRepository.Save(domain);
                id = domain.Id;
                stats.PriceEstimateId = domain.Id;
                _logService.Info("Updating Price EstimateData " + domain.Id);
            }
            catch (Exception ex)
            {
                sb.Append(stats.Logs);
                stats.Logs.Append(Log(string.Format("Error in Parsing Price Estimate data {0} with error {1} with service {2}", id, ex.StackTrace, name)));
                LogException(sb, ex);
                _logService.Error("Error in Price Estimate with Id: " + id, ex);
            }
            return stats;
        }

        private PriceEstimateServices CreateModel(PriceEstimateUploadEditModel model)
        {
            var priceEstimateServiceDomain = new PriceEstimateServices();
            if (model.Id != default(long))
            {
                priceEstimateServiceDomain = _priceEstimateServicesRepository.Get(model.Id);
                return new PriceEstimateServices
                {
                    Id = model.Id,
                    IsNew = model.Id > 0 && model.IsUpdated == 1 ? false : true,
                    FranchiseeId = priceEstimateServiceDomain.FranchiseeId,
                    FranchiseePrice = model.FranchiseePrice,
                    FranchiseeAdditionalPrice = model.FranchiseeAdditionalPrice,
                    IsPriceChangedByFranchisee = true,
                    BulkCorporateAdditionalPrice = priceEstimateServiceDomain.BulkCorporateAdditionalPrice,
                    BulkCorporatePrice = priceEstimateServiceDomain.BulkCorporatePrice,
                    IsPriceChangedByAdmin = priceEstimateServiceDomain.IsPriceChangedByAdmin,
                    CorporatePrice = priceEstimateServiceDomain.CorporatePrice,
                    CorporateAdditionalPrice = priceEstimateServiceDomain.CorporateAdditionalPrice,
                    AlternativeSolution = priceEstimateServiceDomain.AlternativeSolution,
                    ServiceTagId = priceEstimateServiceDomain.ServiceTagId,
                    ServicesTag = _servicesTagRepository.Table.Where(x => x.Id == priceEstimateServiceDomain.ServiceTagId).FirstOrDefault()
                };
            }
            else
            {
                var serviceTag = _servicesTagRepository.Table.Where(x => x.Category.Name == model.Category && x.Service == model.Service && x.ServiceType.Name == model.ServiceType && x.MaterialType == model.MaterialType && x.IsActive).FirstOrDefault();
                var franchiseeId = model.Id > 0 ? priceEstimateServiceDomain.FranchiseeId : GetFranchiseeId(model.FranchiseeName);
                var defauldService = _priceEstimateServicesRepository.TableNoTracking.FirstOrDefault(x => x.ServiceTagId == serviceTag.Id && x.FranchiseeId == franchiseeId && !x.IsPriceChangedByFranchisee);
                return new PriceEstimateServices
                {
                    Id = model.Id,
                    IsNew = model.Id > 0 && model.IsUpdated == 1 ? false : true,
                    FranchiseeId = franchiseeId,
                    FranchiseePrice = model.FranchiseePrice,
                    FranchiseeAdditionalPrice = model.FranchiseeAdditionalPrice,
                    IsPriceChangedByFranchisee = true,
                    BulkCorporatePrice = defauldService != null ? defauldService.BulkCorporatePrice : 0,
                    BulkCorporateAdditionalPrice = defauldService != null ? defauldService.BulkCorporateAdditionalPrice : 0,
                    CorporatePrice = defauldService != null ? defauldService.CorporatePrice : 0,
                    CorporateAdditionalPrice = defauldService != null ? defauldService.CorporateAdditionalPrice : 0,
                    ServiceTagId = serviceTag.Id,
                    ServicesTag = _servicesTagRepository.Table.Where(x => x.Id == serviceTag.Id).FirstOrDefault()
                };
            }
        }

        //private PriceEstimateSaveModelStats SaveModelForSA(PriceEstimateSaveModelStats stats, List<PriceEstimateUploadEditModel> priceEstimateCollection, PriceEstimateUploadEditModel recordForSave)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    var priceEstimateDomain = new PriceEstimateServices();
        //    string name = "";
        //    long id = default(long);
        //    try
        //    {
        //        if (recordForSave.Id > 0)
        //        {
        //            priceEstimateDomain = _priceEstimateServicesRepository.Get(recordForSave.Id);
        //            if (priceEstimateDomain == null)
        //                recordForSave.Id = 0;
        //        }
        //        var domain = CreateModelForSA(recordForSave);
        //        name = domain.ServicesTag != null ? domain.ServicesTag.Service : string.Empty;
        //        _priceEstimateServicesRepository.Save(domain);
        //        id = domain.Id;
        //        stats.PriceEstimateId = domain.Id;
        //        var priceestimates = _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId == domain.ServiceTagId && x.Id != domain.Id).ToList();
        //        foreach (var price in priceestimates)
        //        {
        //            if (!price.IsPriceChangedByFranchisee)
        //            {
        //                price.IsNew = false;
        //                price.CorporatePrice = recordForSave.BulkCorporatePrice;
        //                price.BulkCorporatePrice = recordForSave.BulkCorporatePrice;
        //                price.CorporateAdditionalPrice = recordForSave.BulkCorporateAdditionalPrice;
        //                price.BulkCorporateAdditionalPrice = recordForSave.BulkCorporateAdditionalPrice;
        //            }
        //            _priceEstimateServicesRepository.Save(price);
        //        }
        //        _logService.Info("Updating Price EstimateData " + domain.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        sb.Append(stats.Logs);
        //        stats.Logs.Append(Log(string.Format("Error in Parsing Price Estimate data {0} with error {1} with service {2}", id, ex.StackTrace, name)));
        //        LogException(sb, ex);
        //        _logService.Error("Error in Price Estimate with Id: " + id, ex);
        //    }
        //    return stats;
        //}


        //private PriceEstimateServices CreateModelForSA(PriceEstimateUploadEditModel model)
        //{
        //    var priceEstimateServiceDomain = new PriceEstimateServices();
        //    if (model.Id != default(long))
        //    {
        //        priceEstimateServiceDomain = _priceEstimateServicesRepository.Get(model.Id);
        //        return new PriceEstimateServices
        //        {
        //            Id = model.Id,
        //            IsNew = model.Id > 0 && model.IsUpdated == 1 ? false : true,
        //            FranchiseeId = priceEstimateServiceDomain.FranchiseeId,
        //            FranchiseePrice = priceEstimateServiceDomain.FranchiseePrice,
        //            FranchiseeAdditionalPrice = priceEstimateServiceDomain.FranchiseeAdditionalPrice,
        //            IsPriceChangedByFranchisee = true,
        //            BulkCorporateAdditionalPrice = priceEstimateServiceDomain.BulkCorporateAdditionalPrice,
        //            BulkCorporatePrice = priceEstimateServiceDomain.BulkCorporatePrice,
        //            IsPriceChangedByAdmin = priceEstimateServiceDomain.IsPriceChangedByAdmin,
        //            CorporatePrice = priceEstimateServiceDomain.CorporatePrice,
        //            CorporateAdditionalPrice = priceEstimateServiceDomain.CorporateAdditionalPrice,
        //            AlternativeSolution = priceEstimateServiceDomain.AlternativeSolution,
        //            ServiceTagId = priceEstimateServiceDomain.ServiceTagId,
        //            ServicesTag = _servicesTagRepository.Table.Where(x => x.Id == priceEstimateServiceDomain.ServiceTagId).FirstOrDefault()
        //        };
        //    }
        //    else
        //    {
        //        var serviceTag = _servicesTagRepository.Table.Where(x => x.Category.Name == model.Category && x.Service == model.Service && x.ServiceType.Name == model.ServiceType && x.MaterialType == model.MaterialType).FirstOrDefault();
        //        return new PriceEstimateServices
        //        {
        //            Id = model.Id,
        //            IsNew = model.Id > 0 && model.IsUpdated == 1 ? false : true,
        //            FranchiseeId = model.Id > 0 ? priceEstimateServiceDomain.FranchiseeId : GetFranchiseeId(model.FranchiseeName),
        //            FranchiseePrice = model.FranchiseePrice,
        //            FranchiseeAdditionalPrice = model.FranchiseeAdditionalPrice,
        //            IsPriceChangedByFranchisee = true,
        //            ServiceTagId = serviceTag.Id,
        //            ServicesTag = _servicesTagRepository.Table.Where(x => x.Id == serviceTag.Id).FirstOrDefault()
        //        };
        //    }
        //}

        private long? GetFranchiseeId(string franchiseeName)
        {
            List<Franchisee> listFranchisee = _franchiseeRepository.Table.ToList();
            //var franchisee = listFranchisee.FirstOrDefault(x => (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").StartsWith(franchiseeName.Trim().ToUpper())
            //                              || (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").EndsWith(franchiseeName.Trim().ToUpper())
            //                              || (x.Organization.Name.ToUpper()).Equals(franchiseeName.ToUpper())
            //                              || (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").Equals(franchiseeName.Trim(), StringComparison.CurrentCultureIgnoreCase));

            var franchisee = listFranchisee.FirstOrDefault(x => string.Equals(x.Organization.Name.Replace(" ", ""),franchiseeName.Replace(" ", ""),StringComparison.OrdinalIgnoreCase));
            if (franchisee != null)
                return franchisee.Id;

            return null;
        }

        #region Logs

        //saving log file Id in priceestimate upload file
        private void SavePriceEstimateFileUpload(PriceEstimateFileUpload customerFileUpload, PriceEstimateSaveModelStats stats)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                var fileModel = PrepareLogFileModel("PriceEstimate_" + customerFileUpload.Id);
                var file = _fileService.SaveModel(fileModel);
                customerFileUpload.ParsedLogFileId = file.Id;
                _priceEstimateFileUploadRepository.Save(customerFileUpload);
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
        #endregion

        #region Mark success or failed
        private void MarkUploadAsFailed(PriceEstimateFileUpload priceEstimateFile, PriceEstimateSaveModelStats stats)
        {
            priceEstimateFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            priceEstimateFile.StatusId = (long)SalesDataUploadStatus.Failed;
            _priceEstimateFileUploadRepository.Save(priceEstimateFile);
        }
        private void MarkUploadAsSuccess(PriceEstimateFileUpload priceEstimateFile)
        {
            priceEstimateFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
            priceEstimateFile.StatusId = (long)SalesDataUploadStatus.Parsed;
            _priceEstimateFileUploadRepository.Save(priceEstimateFile);
        }
        #endregion

        #region SuperAdmin
        public void ProcessRecordsForSA()
        {
            _unitOfWork.StartTransaction();
            var stats = new PriceEstimateSaveModelStats();
            var priceEstimateIds = new List<long>();
            var priceEstimatesViewModel = new List<PriceEstimateUploadViewModel>();
            var duplicatePriceEstimatesViewModel = new List<PriceEstimateUploadViewModel>();
            stats.Logs = new StringBuilder();
            var sb = new StringBuilder();
            var updatedPriceEstimates = 0;
            var deletedPriceEstimates = 0;
            long currentIdInFile = default(long);
            _logService.Info("Starting Price Estimate Code File Parsing");
            var priceEstimateFileList = GetPriceEstimateDataToParseSA();
            if (priceEstimateFileList == null || !priceEstimateFileList.Any())
            {
                _logService.Debug("No file found for parsing price estimate data.");
                return;
            }
            DataTable data;
            var priceEstimateParsedFile = new PriceEstimateFileUpload();
            long? fileId = default(long?);
            var isFailed = false;
            try
            {
                foreach (PriceEstimateFileUpload file in priceEstimateFileList)
                {
                    fileId = file.Id;
                    newlyAddedPriceEstimates = 0;
                    var priceEstimateCollection = new List<PriceEstimateUploadEditModel>();
                    file.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
                    _priceEstimateFileUploadRepository.Save(file);
                    _unitOfWork.SaveChanges();
                    var filePath = MediaLocationHelper.FilePath(file.File.RelativeLocation, file.File.Name).ToFullPath();
                    priceEstimateParsedFile = file;
                    var sheetIds = PriceEstimateExcelFileParser.GetSheetIds(filePath);
                    string message;
                    CreateLogFile(sb, "PriceEstimate_" + fileId);
                    SavePriceEstimateFileUpload(priceEstimateParsedFile, stats);
                    var isErrorExist = false;
                    var totalPriceEstimatesInFile = default(long?);
                    var totalPriceEstimatesToBeUpdated = default(long?);
                    foreach (var sheetId in sheetIds)
                    {
                        if (isErrorExist)
                        {
                            break;
                        }
                        _logService.Info("Started recording Price estimate data!");
                        data = PriceEstimateExcelFileParser.ReadExcelZip(filePath, sheetId);
                        var priceEstimateFileParser = ApplicationManager.DependencyInjection.Resolve<IPriceEstimateFileParser>();
                        priceEstimateCollection = priceEstimateFileParser.PrepareDomainFromDataTable(data, true).ToList();
                        totalPriceEstimatesInFile = priceEstimateCollection.Count();
                        if (!priceEstimateFileParser.CheckForValidHeaderForSA(data, out message))
                        {
                            isFailed = true;
                            stats.Logs.Append(Log("Please upload correct File! " + message));
                            sb.Append(stats.Logs);
                            CreateLogFile(sb, "PriceEstimate_" + priceEstimateParsedFile.Id);
                            MarkUploadAsFailed(file, stats);
                            isErrorExist = true;
                            break;
                        }
                        stats.Logs.Append(Log("Total Price estimate data in Uploaded file: " + totalPriceEstimatesInFile));
                        priceEstimateCollection = priceEstimateCollection.Where(x => x.IsUpdated == 1 || x.IsDeleted == 1).ToList();
                        totalPriceEstimatesToBeUpdated = priceEstimateCollection.Count();
                        stats.Logs.Append(Log("Total Price estimate data needs to be updated is: " + totalPriceEstimatesToBeUpdated));
                        sb.Append(Log(string.Format("Parsing Price estimate data")));
                        if (priceEstimateCollection.Count() > 0)
                            priceEstimateIds.AddRange(priceEstimateCollection.Select(x => x.Id));
                        foreach (var record in priceEstimateCollection)
                        {
                            currentIdInFile = record.Id;
                            if (record.Id > 0 && priceEstimatesViewModel.Any(x => x.Id == record.Id))
                            {
                                duplicatePriceEstimatesViewModel.Add(new PriceEstimateUploadViewModel() { Service = record.Service, ServiceType = record.ServiceType, MaterialType = record.MaterialType, Category = record.Category, Id = record.Id });
                                record.IsDuplicate = true;
                            }
                            else if (!priceEstimatesViewModel.Any(x => x.Service == record.Service && x.ServiceType == record.ServiceType && x.MaterialType == record.MaterialType && x.Category == record.Category))
                            {
                                priceEstimatesViewModel.Add(new PriceEstimateUploadViewModel() { Id = record.Id, Service = record.Service, ServiceType = record.ServiceType, MaterialType = record.MaterialType, Category = record.Category });
                            }
                            else
                            {
                                duplicatePriceEstimatesViewModel.Add(new PriceEstimateUploadViewModel() { Service = record.Service, ServiceType = record.ServiceType, MaterialType = record.MaterialType, Category = record.Category, Id = record.Id });
                                record.IsDuplicate = true;
                            }
                            if (record.IsDuplicate)
                            {
                                break;
                            }
                            if(record.Category == "TIME")
                            {
                                if(record.BulkCorporatePrice >= 0)
                                {
                                    record.BulkCorporatePrice = null;
                                }
                                if(record.BulkCorporateAdditionalPrice >= 0)
                                {
                                    record.BulkCorporateAdditionalPrice = null;
                                }
                            }
                            if (record.IsUpdated == 1 && record.IsDeleted == 0)
                            {
                                var isNotesUpdated = false;
                                var servicetag = _servicesTagRepository.Table.Where(x => x.Service == record.Service && x.MaterialType == record.MaterialType && x.Category.Name == record.Category && x.ServiceType.Name == record.ServiceType).FirstOrDefault();
                                if (servicetag != null || servicetag != default(ServicesTag))
                                {
                                    if (!string.IsNullOrEmpty(record.Note) && !string.Equals(servicetag.Notes, record.Note))
                                    {
                                        servicetag.Notes = record.Note;
                                        servicetag.NotesSavedBy = file.DataRecorderMetaData != null ? file.DataRecorderMetaData.CreatedBy : 1;
                                        servicetag.IsNew = false;
                                        _servicesTagRepository.Save(servicetag);
                                        stats.Logs.Append(Log(string.Format("Updating Notes: Service {0}, Material {1}", servicetag.Service, servicetag.MaterialType)));
                                        isNotesUpdated = true;
                                        ++updatedPriceEstimates;
                                    }
                                }
                                var priceEstimateDomainList = servicetag != default(ServicesTag) ? _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId == servicetag.Id).ToList() : new List<PriceEstimateServices>();
                                var isPriceUpdatedOrAdded = false;
                                if (priceEstimateDomainList.Count > 0)
                                {
                                    var priceEstimateDomain = priceEstimateDomainList.FirstOrDefault();
                                    if (priceEstimateDomain.BulkCorporatePrice != record.BulkCorporatePrice || priceEstimateDomain.BulkCorporateAdditionalPrice != record.BulkCorporateAdditionalPrice)
                                    {
                                        isPriceUpdatedOrAdded = true;
                                        stats = SavePriceEstimateForSuperAdmin(record, stats);
                                        stats.Logs.Append(Log(string.Format("Updating Price estimate data: Service {0}, Material {1}", priceEstimateDomain.ServicesTag.Service, priceEstimateDomain.ServicesTag.MaterialType)));
                                        if (!isNotesUpdated)
                                            ++updatedPriceEstimates;
                                    }
                                }
                                else
                                {
                                    if (record.BulkCorporatePrice > 0 || record.BulkCorporateAdditionalPrice > 0)
                                    {
                                        isPriceUpdatedOrAdded = true;
                                        stats = SavePriceEstimateForSuperAdmin(record, stats);
                                        stats.Logs.Append(Log(string.Format("Adding Price estimate data: Service {0}, Material {1}", record.Service, record.MaterialType)));
                                        if (!isNotesUpdated)
                                            ++updatedPriceEstimates;
                                    }
                                }
                                if (record.Category == "TIME")
                                {
                                    if (record.BulkCorporatePrice >= 0)
                                    {
                                        record.BulkCorporatePrice = null;
                                    }
                                    if (record.BulkCorporateAdditionalPrice >= 0)
                                    {
                                        record.BulkCorporateAdditionalPrice = null;
                                    }
                                }
                                if (!isNotesUpdated && !isPriceUpdatedOrAdded)
                                {
                                    stats.Logs.Append(Log(string.Format("You have not updated Price estimate data for Columns K, L or M: Service Type {0} Service {1}, Material {2}", servicetag.ServiceType.Name, servicetag.Service, servicetag.MaterialType)));
                                }
                                _unitOfWork.StartTransaction();
                                _unitOfWork.SaveChanges();
                            }
                            else if ((record.IsUpdated == 0 && record.IsDeleted == 1) || (record.IsUpdated == 1 && record.IsDeleted == 1))
                            {
                                if (record.BulkCorporateAdditionalPrice > 0 || record.BulkCorporatePrice > 0)
                                {
                                    var servicetag = _servicesTagRepository.Table.Where(x => x.Service == record.Service && x.MaterialType == record.MaterialType && x.Category.Name == record.Category && x.ServiceType.Name == record.ServiceType).FirstOrDefault();
                                    if (servicetag != null || servicetag != default(ServicesTag))
                                    {
                                        if (!string.IsNullOrEmpty(record.Note) && !string.Equals(servicetag.Notes, record.Note))
                                        {
                                            servicetag.Notes = record.Note;
                                            servicetag.NotesSavedBy = file.DataRecorderMetaData != null ? file.DataRecorderMetaData.CreatedBy : 1;
                                            servicetag.IsNew = false;
                                            _servicesTagRepository.Save(servicetag);
                                            stats.Logs.Append(Log(string.Format("Updating Notes: Service {0}, Material {1}", servicetag.Service, servicetag.MaterialType)));
                                        }
                                    }
                                    var priceEstimateDomainList = servicetag != default(ServicesTag) ? _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId == servicetag.Id).ToList() : new List<PriceEstimateServices>();
                                    if (priceEstimateDomainList.Count > 0)
                                    {
                                        stats.Logs.Append(Log(string.Format("Deleting Price estimate data: Service Type: {0}, Service {1}, Material {2}", priceEstimateDomainList.FirstOrDefault().ServicesTag.ServiceType.Name, priceEstimateDomainList.FirstOrDefault().ServicesTag.Service, priceEstimateDomainList.FirstOrDefault().ServicesTag.MaterialType)));
                                        foreach (var price in priceEstimateDomainList)
                                        {
                                            _unitOfWork.StartTransaction();
                                            price.IsNew = false;
                                            price.BulkCorporatePrice = null;
                                            price.BulkCorporateAdditionalPrice = null;
                                            price.CorporateAdditionalPrice = null;
                                            price.CorporatePrice = null;
                                            if (!price.IsPriceChangedByFranchisee)
                                            {
                                                price.FranchiseeAdditionalPrice = null;
                                                price.FranchiseePrice = null;
                                            }
                                            _priceEstimateServicesRepository.Save(price);
                                            _unitOfWork.SaveChanges();
                                        }
                                        ++deletedPriceEstimates;
                                    }
                                }
                                else
                                {
                                    stats.Logs.Append(Log(string.Format("No Price estimate data found for deleting(Corporate Prices are not yet added): Service Type: {0}, Service {1}, Material {2}", record.ServiceType, record.Service, record.MaterialType)));
                                }
                            }
                        }
                        currentIdInFile = default(long);
                        stats.Logs.Append(Log("Price Estimates newly added: " + newlyAddedPriceEstimates));
                        stats.Logs.Append(Log("Price Estimates updated: " + updatedPriceEstimates));
                        stats.Logs.Append(Log("Price Estimates deleted: " + deletedPriceEstimates));
                        if (duplicatePriceEstimatesViewModel.Count() > 0)
                        {
                            stats.Logs.Append(Log("Total Number Of Duplicate Price Estimates: " + duplicatePriceEstimatesViewModel.Count()));
                            stats.Logs.Append(Log("Following are the  Duplicate Price Estimates: "));
                            foreach (var priceEstimate in duplicatePriceEstimatesViewModel)
                            {
                                stats.Logs.Append(Log("Service: " + priceEstimate.Service + " ServiceType: " + priceEstimate.ServiceType + " Material Type: " + priceEstimate.MaterialType + " Category: " + priceEstimate.Category + " having Id: " + priceEstimate.Id));
                            }
                        }
                        sb.Append(stats.Logs);
                        _logService.Info("Finished recording Price Estimate data!");
                    }
                    if (priceEstimateCollection.Count() == 0)
                    {
                        stats.Logs.Append(Log(string.Format("No Records Found to Update/Add/Delete in Price Estimate Sheet (FileId : " + priceEstimateParsedFile.FileId + ")")));
                    }
                    CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                    isErrorExist = false;
                    if (!isFailed)
                    {
                        isFailed = false;
                        MarkUploadAsSuccess(file);
                    }
                    if (isFailed)
                    {
                        MarkUploadAsFailed(file, stats);
                    }
                    _logService.Info("Ending Price Estimate Data Upload Service");
                    _unitOfWork.SaveChanges();
                }
            }
            catch (InvalidFileUploadException ex)
            {
                priceEstimateParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                priceEstimateParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                MarkUploadAsFailed(priceEstimateParsedFile, stats);
                _logService.Info(string.Format("Error in Price Estimate File Upload: ", ex));
            }
            catch (IndexOutOfRangeException ex)
            {
                if (currentIdInFile != default(long))
                    stats.Logs.Append(Log(string.Format("Exception in  Price Estimate Id- {0}", currentIdInFile)));
                priceEstimateParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                priceEstimateParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                stats.Logs.Append(sb);
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                MarkUploadAsFailed(priceEstimateParsedFile, stats);
                _logService.Info(string.Format("Error in Price Estimate : ", ex));
            }
            catch (Exception ex)
            {
                if (currentIdInFile != default(long))
                    stats.Logs.Append(Log(string.Format("Exception in  Price Estimate Id- {0}", currentIdInFile)));
                priceEstimateParsedFile.DataRecorderMetaData.DateModified = DateTime.UtcNow;
                priceEstimateParsedFile.StatusId = (long)SalesDataUploadStatus.Failed;
                stats.Logs.Append(sb);
                LogException(stats.Logs, ex);
                CreateLogFile(stats.Logs, "PriceEstimate_" + fileId);
                MarkUploadAsFailed(priceEstimateParsedFile, stats);
                _logService.Info(string.Format("Error in Price Estimate: ", ex));
            }
            finally
            {
                _unitOfWork.SaveChanges();
            }
        }
        #endregion


        private PriceEstimateSaveModelStats SavePriceEstimateForSuperAdmin(PriceEstimateUploadEditModel record, PriceEstimateSaveModelStats stats)
        {
            StringBuilder sb = new StringBuilder();
            long id = default(long);
            string name = string.Empty;
            try
            {
                var servicetag = _servicesTagRepository.Table.Where(x => x.Service == record.Service && x.MaterialType == record.MaterialType && x.Category.Name == record.Category && x.ServiceType.Name == record.ServiceType).FirstOrDefault();
                stats.ServiceTagId = servicetag.Id;
                name = servicetag.Service;
                id = servicetag.Id;
                var priceEstimateDomainList = servicetag != default(ServicesTag) ? _priceEstimateServicesRepository.Table.Where(x => x.ServiceTagId == servicetag.Id).ToList() : new List<PriceEstimateServices>();
                var frachiseeList = _franchiseeRepository.Table.Where(x => !x.Organization.Name.StartsWith("0-") && x.Id != 2).ToList();
                foreach (var franchisee in frachiseeList)
                {
                    var priceEstimateService = _priceEstimateServicesRepository.Table.FirstOrDefault(x => x.ServiceTagId == servicetag.Id && x.FranchiseeId == franchisee.Id);
                    var domain = new PriceEstimateServices();
                    domain.FranchiseeId = franchisee.Id;
                    domain.BulkCorporatePrice = record.BulkCorporatePrice;
                    domain.BulkCorporateAdditionalPrice = record.BulkCorporateAdditionalPrice;
                    if (priceEstimateService != null)
                    {
                        domain.AlternativeSolution = priceEstimateService.AlternativeSolution;
                        if (priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.IsPriceChangedByFranchisee = priceEstimateService.IsPriceChangedByFranchisee;
                        }
                        if (priceEstimateService.IsPriceChangedByAdmin)
                        {
                            domain.IsPriceChangedByAdmin = priceEstimateService.IsPriceChangedByAdmin;
                        }
                        if (!priceEstimateService.IsPriceChangedByAdmin)
                        {
                            domain.CorporatePrice = record.BulkCorporatePrice;
                            domain.CorporateAdditionalPrice = record.BulkCorporateAdditionalPrice;
                        }
                        else
                        {
                            domain.CorporatePrice = priceEstimateService.CorporatePrice;
                            domain.CorporateAdditionalPrice = priceEstimateService.CorporateAdditionalPrice;
                        }
                        if (!priceEstimateService.IsPriceChangedByFranchisee)
                        {
                            domain.FranchiseePrice = domain.CorporatePrice;
                            domain.FranchiseeAdditionalPrice = domain.CorporateAdditionalPrice;
                        }
                        else
                        {
                            domain.FranchiseePrice = priceEstimateService.FranchiseePrice;
                            domain.FranchiseeAdditionalPrice = priceEstimateService.FranchiseeAdditionalPrice;
                        }
                    }
                    else
                    {
                        domain.CorporatePrice = record.BulkCorporatePrice;
                        domain.CorporateAdditionalPrice = record.BulkCorporateAdditionalPrice;
                        domain.FranchiseePrice = record.BulkCorporatePrice;
                        domain.FranchiseeAdditionalPrice = record.BulkCorporateAdditionalPrice;
                    }
                    domain.ServiceTagId = servicetag.Id;
                    domain.IsNew = priceEstimateService != null ? false : true;
                    domain.Id = priceEstimateService != null ? priceEstimateService.Id : 0;
                    _priceEstimateServicesRepository.Save(domain);
                }
            }
            catch (Exception ex)
            {
                sb.Append(stats.Logs);
                stats.Logs.Append(Log(string.Format("Error in Parsing Price Estimate data {0} with error {1} with service {2}", id, ex.StackTrace, name)));
                LogException(sb, ex);
                _logService.Error("Error in Price Estimate with Service Tag Id: " + id, ex);
            }
            return stats;
        }

    }
}
