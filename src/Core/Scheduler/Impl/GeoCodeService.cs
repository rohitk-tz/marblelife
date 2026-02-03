using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Geo.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using Ical.Net.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class GeoCodeService : IGeoCodeService
    {
        private readonly IGeoCodeFactory _geoCodeFactory;
        private readonly IFileService _fileService;
        private readonly IRepository<GeoCodefileupload> _geoCodeFileUploadeRepository;
        private readonly IDownloadFileHelperService _downloadFileHelperService;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IRepository<County> _countyRepository;
        private readonly IRepository<ZipCode> _zipCodeRepository;
        private readonly IRepository<State> _stateRepository;
        private readonly ISortingHelper _sortingHelper;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<OrganizationRoleUserFranchisee> _organizationRoleUserFranchiseeRepository;
        private readonly IRepository<FranchiseeService> _franchiseeServiceRepository;
        public GeoCodeService(IUnitOfWork unitOfWork, IGeoCodeFactory geoCodeFactory, IFileService fileService, IDownloadFileHelperService downloadFileHelperService,
           IExcelFileCreator excelFileCreator, ISortingHelper sortingHelper)
        {
            _geoCodeFactory = geoCodeFactory;
            _fileService = fileService;
            _geoCodeFileUploadeRepository = unitOfWork.Repository<GeoCodefileupload>();
            _downloadFileHelperService = downloadFileHelperService;
            _excelFileCreator = excelFileCreator;
            _countyRepository = unitOfWork.Repository<County>();
            _zipCodeRepository = unitOfWork.Repository<ZipCode>();
            _stateRepository = unitOfWork.Repository<State>();
            _sortingHelper = sortingHelper;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _organizationRoleUserFranchiseeRepository = unitOfWork.Repository<OrganizationRoleUserFranchisee>();
            _franchiseeServiceRepository = unitOfWork.Repository<FranchiseeService>();

        }
        public void SaveFile(CustomerFileUploadCreateModel model)
        {
            var customerFileUpload = _geoCodeFactory.CreateViewModel(model);
            var file = _fileService.SaveModel(model.File);
            customerFileUpload.FileId = file.Id;
            _geoCodeFileUploadeRepository.Save(customerFileUpload);
        }

        public ZipDataUploadListModel GetZipList(ZipDataListFilter filter, int pageNumber, int pageSize)
        {
            var zipData = _geoCodeFileUploadeRepository.Table.Where(x => (filter.StatusId < 1 || x.StatusId == filter.StatusId)
                                  && (string.IsNullOrEmpty(filter.text) || ((x.Id.ToString().Equals(filter.text)))));
            zipData = _sortingHelper.ApplySorting(zipData, x => x.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        zipData = _sortingHelper.ApplySorting(zipData, x => x.Id, filter.SortingOrder);
                        break;
                    case "Status":
                        zipData = _sortingHelper.ApplySorting(zipData, x => x.Lookup.Name, filter.SortingOrder);
                        break;
                    case "UploadedOn":
                        zipData = _sortingHelper.ApplySorting(zipData, x => x.DataRecorderMetaData.DateCreated, filter.SortingOrder);
                        break;
                }
            }

            var list = zipData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new ZipDataUploadListModel()
            {
                Collection = list.Select(_geoCodeFactory.CreateViewModel).ToList(),
                PagingModel = new PagingModel(pageNumber, pageSize, zipData.Count()),
                Filter = filter
            };
        }

        public ZipDataInfoListModel GetZipInfo(ZipCodeInfoListFilter filter)
        {
            var zipCode = filter.ZipCode.TrimStart(new Char[] { '0' });
            List<ZipCode> finalCollection = new List<ZipCode>();
            var zipDatas = _zipCodeRepository.Table.Where(x => (string.IsNullOrEmpty(zipCode) || x.Zip.Equals(zipCode))
                                  && x.County != null && (filter.CountryId == null || ((x.County.CountryId == filter.CountryId)))).ToList();
            if (zipDatas.Count() > 0)
            {
                var firstData = zipDatas[0];
                for (int i = 1; i < zipDatas.Count(); i++)
                {
                    double firstDataTime = firstData.County != null ? Convert.ToDouble(firstData.County.ReachingTime) : 100;
                    double secomdDataTime = zipDatas[i].County != null ? Convert.ToDouble(zipDatas[i].County.ReachingTime) : 100;

                    if (firstDataTime < secomdDataTime)
                    {
                        continue;
                    }

                    else
                    {
                        firstData = zipDatas[i];
                    }
                }
                finalCollection.Add(firstData);
            }
            zipDatas = finalCollection;
            return new ZipDataInfoListModel()
            {
                Collection = zipDatas.Select(x => GetGeoCodeInfo(x, filter.UserId, filter.RoleId)).ToList(),
            };
        }

        public bool CreateExcelForAllFiles(ZipDataListFilter filter, out string fileName)
        {
            fileName = string.Empty;

            List<string> files = new List<string>();

            var countyList = countyFilterList(filter).ToList();
            var zipCodeList = zipCodeFilterList(filter).ToList();
            var countyListForExcel = _downloadFileHelperService.CreateDataForCounty(countyList);
            var zipCodeListForExcel = _downloadFileHelperService.CreateDataForZipCode(zipCodeList);
            var InstructionForExcel = _downloadFileHelperService.CreateDataForInstruction();

            var ds = new DataSet();
            ds.Tables.Add(_excelFileCreator.ListToDataTable(countyListForExcel, "County"));
            ds.Tables.Add(_excelFileCreator.ListToDataTable(zipCodeListForExcel, "Zip Codes"));
            ds.Tables.Add(_excelFileCreator.ListToDataTable(InstructionForExcel, "Instructions"));

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/Geo_Code-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(ds, fileName);
        }

        private IQueryable<County> countyFilterList(ZipDataListFilter filter)
        {
            var countyList = _countyRepository.Table.Select(x => x);
            if (filter.CountryId == 1)
            {
                countyList = countyList.Where(x => x.CountryId != 8);
            }
            else if (filter.CountryId == 8)
            {
                countyList = countyList.Where(x => x.CountryId == 8);
            }
            return countyList;
        }

        private IQueryable<ZipCode> zipCodeFilterList(ZipDataListFilter filter)
        {
            var zipCodeList = _zipCodeRepository.Table.Select(x => x);
            if (filter.CountryId == 1)
            {
                zipCodeList = zipCodeList.Where(x => x.County.CountryId != 8);
            }
            else if (filter.CountryId == 8)
            {
                zipCodeList = zipCodeList.Where(x => x.County.CountryId == 8);
            }
            return zipCodeList;
        }

        public ZipDataInfoViewModel GetZipInfoScheduler(long franchiseeId)
        {
            var model = new ZipDataInfoViewModel { };
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null)
                return model;

            var address = franchisee.Organization.Address.FirstOrDefault();
            if (address == null)
                return model;

            var zipCode = address.Zip != null ? address.Zip.Code : address.ZipCode;
            var stateId = address.StateId != null ? address.StateId.Value : 0;

            var stateName = address.State != null ? address.State.Name : address.StateName;

            var stateCode = string.Empty;
            var directionCode = string.Empty;
            if (stateId > 0)
            {
                var state = _stateRepository.Get(stateId);
                if (state != null)
                    stateCode = state.ShortName;
            }
            else
            {
                var state = _stateRepository.Get(x => x.Name == stateName || x.ShortName == stateName);
                if (state != null)
                    stateCode = state.ShortName;
            }

            var zipCodeDomain = _zipCodeRepository.Get(x => (x.Zip.Equals(zipCode)) && (stateCode == null || x.StateCode.Equals(stateCode)));
            if (zipCodeDomain != null && zipCodeDomain.County != null)
                directionCode = zipCodeDomain.County.DirectionCode;

            return new ZipDataInfoViewModel
            {
                DirectionCode = directionCode,
                zipcode = zipCode
            };
        }

        public GeoCodeDataInfoViewModel GetZipInfoByZipCode(string zipCode, string state, long? franchiseeId, long? countryId)
        {
            var code = string.Empty;
            var cityName = string.Empty;
            var stateName = string.Empty;
            var stateId = default(long);
            if (state != "")
            {
                var stateDomain = _stateRepository.Table.FirstOrDefault(x => x.Name.ToUpper().Equals(state.ToUpper()) 
                    && (countryId == 0 || x.CountryId == countryId));

                if (stateDomain == null)
                    stateDomain = _stateRepository.Table.FirstOrDefault(x => x.ShortName.ToUpper().Equals(state.ToUpper()) && (countryId == 0 || x.CountryId == countryId));

                if (stateDomain != null)
                {
                    var directionCode = _zipCodeRepository.Table.FirstOrDefault(x => x.Zip.Equals(zipCode)
                                        && (x.StateCode.Equals(stateDomain.ShortName) || (x.StateCode.Equals(state)))
                                        && x.County.FranchiseeId == franchiseeId
                                        && (countryId == 0 || x.County.CountryId == countryId));
                    if (directionCode != null)
                    {
                        cityName = directionCode.CityName;
                        stateName = state;
                    }

                    if (directionCode != null && directionCode.County != null)
                        code = directionCode.County.DirectionCode;

                    if (directionCode == null)
                    {
                        directionCode = _zipCodeRepository.Table.FirstOrDefault(x => x.Zip.Equals(zipCode));
                        if (directionCode != null)
                        {
                            if (directionCode.County != null)
                                code = directionCode.County.DirectionCode;
                            stateName = directionCode.StateCode;
                            cityName = directionCode.CityName;

                            var stateDomainForZipCode = _stateRepository.Table.FirstOrDefault(x => x.Name.ToUpper().Equals(stateName.ToUpper()));

                            if (stateDomainForZipCode == null)
                                stateDomainForZipCode = _stateRepository.Table.FirstOrDefault(x => x.ShortName.ToUpper().Equals(stateName.ToUpper()));
                            if (stateDomainForZipCode != null)
                            {
                                stateId = stateDomainForZipCode.Id;
                            }
                            else
                            {
                                stateId = 0;
                            }
                        }
                    }
                    //if (stateDomain != null)
                    //    stateId = stateDomain.Id;
                    //else
                    //    stateId = 0;
                }
            }
            else
            {
                var stateDomain = new State();
                var directionCode = _zipCodeRepository.Table.FirstOrDefault(x => x.Zip.Equals(zipCode) && x.County.CountryId==countryId);
                if (directionCode != null)
                    stateDomain = _stateRepository.Table.FirstOrDefault(x => x.Name.ToUpper().Equals(directionCode.StateCode.ToUpper()) && x.CountryId==countryId);
                else
                    stateDomain = null;


                if (directionCode != null)
                {
                    if (stateDomain == null)
                        stateDomain = _stateRepository.Table.FirstOrDefault(x => x.ShortName.ToUpper().Equals(directionCode.StateCode.ToUpper()) && x.CountryId == countryId);


                    code = directionCode.County != null ? directionCode.County.DirectionCode : "";
                    stateName = directionCode.StateCode;
                    cityName = directionCode.CityName;
                    var stateDomainForZipCode = _stateRepository.Table.FirstOrDefault(x => x.Name.ToUpper().Equals(stateName.ToUpper()) && x.CountryId == countryId);

                    if (stateDomainForZipCode == null)
                        stateDomainForZipCode = _stateRepository.Table.FirstOrDefault(x => x.ShortName.ToUpper().Equals(stateName.ToUpper()) && x.CountryId == countryId);
                    if (stateDomainForZipCode != null)
                    {
                        stateId = stateDomainForZipCode.Id;
                    }
                    else
                    {
                        stateId = 0;
                    }
                }
            }

            var model = new GeoCodeDataInfoViewModel()
            {
                GeoCode = code,
                CityName = cityName,
                StateName = stateName,
                StateId = stateId
            };
            return model;
        }

        private ZipDataInfoViewModel GetGeoCodeInfo(ZipCode domain, long? userId, long roleId)
        {
            string phoneNumber = string.Empty;
            var model = new ZipDataInfoViewModel { };
            if (domain.County != null)
            {
                if (domain.County.FranchiseeId != null)
                {
                    var org = _franchiseeRepository.Get(domain.County.FranchiseeId.Value);
                    model = GetFranchiseeInfo(org, userId, roleId);
                }
                else
                {
                    var franchiseeName = domain.County.FranchiseeName;
                    var org = _franchiseeRepository.Get(x => (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").StartsWith(franchiseeName.Trim().ToUpper())
                                           || (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").EndsWith(franchiseeName.Trim().ToUpper())
                                           || (x.Organization.Name.ToUpper()).Equals(franchiseeName.ToUpper())
                                           || (x.Organization.Name.Trim().ToUpper()).Replace(" ", "").Equals(franchiseeName.Trim(), StringComparison.CurrentCultureIgnoreCase));
                    model = GetFranchiseeInfo(org, userId, roleId);
                }
            }
            var franchiseeService = new List<FranchiseeService>();
            var franchiseeId = domain.County != null ? domain.County.FranchiseeId : default(long?);
            if (franchiseeId != null)
            {
                franchiseeService = _franchiseeServiceRepository.Table.Where(x => x.FranchiseeId == franchiseeId).ToList();
            }
            if (roleId == (long)RoleType.SuperAdmin)
                model.CanSchedule = true;
            return _geoCodeFactory.CreateViewModel(domain, model, franchiseeService);
        }

        private ZipDataInfoViewModel GetFranchiseeInfo(Franchisee franchisee, long? userId, long roleId)
        {
            var model = new ZipDataInfoViewModel { };
            model.FranchiseeId = franchisee != null ? franchisee.Id : (long?)null;

            model.PhoneNumber = franchisee != null ? ((franchisee.Organization.Phones != null && franchisee.Organization.Phones.Any())
                ? (franchisee.Organization.Phones.Any(x => x.TypeId == (long)PhoneType.CallCenter) ? franchisee.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.CallCenter).Select(y => y.Number).FirstOrDefault()
                : franchisee.Organization.Phones.Where(x => x.TypeId == (long)PhoneType.Office).Select(y => y.Number).FirstOrDefault()) : null) : null;

            if (userId > 0)
            {
                if (roleId == (long)RoleType.FranchiseeAdmin)
                    model.CanSchedule = franchisee != null ? _organizationRoleUserRepository.Table.Any(x => x.UserId == userId && x.RoleId == (long)RoleType.FranchiseeAdmin
                                    && x.OrganizationId == franchisee.Id && x.IsActive) : false;

                else if (roleId == (long)RoleType.FrontOfficeExecutive)
                {
                    var user = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.UserId == userId && x.RoleId == (long)RoleType.FrontOfficeExecutive
                                   && x.IsActive);
                    if (user != null)
                        model.CanSchedule = franchisee != null ? _organizationRoleUserFranchiseeRepository.Table.Any(x => x.FranchiseeId == franchisee.Id
                                         && x.OrganizationRoleUserId == user.Id && x.IsActive) : false;
                }
            }
            return model;
        }


        public bool SaveGeoCodeNotes(ZipDataUploadViewModel filter)
        {
            try
            {
                var zipData = _geoCodeFileUploadeRepository.Get(filter.Id);
                zipData.notes = filter.Notes;
                _geoCodeFileUploadeRepository.Save(zipData);
                return true;
            }
            catch (Exception e1)
            {
                return false;

            }
        }
        public bool DeleteGeoCodeRecord(long? recordid)
        {
            try
            {
                var geoCode = _geoCodeFileUploadeRepository.Get(recordid.GetValueOrDefault());
                if (geoCode != default(GeoCodefileupload))
                {
                    _geoCodeFileUploadeRepository.Delete(geoCode);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e1)
            {
                return false;

            }
        }

        public bool ReparseFile(long? recordid)
        {
            try
            {
                var geoCode = _geoCodeFileUploadeRepository.Get(recordid.GetValueOrDefault());
                if (geoCode != default(GeoCodefileupload))
                {
                    geoCode.StatusId = (long)71;
                    _geoCodeFileUploadeRepository.Save(geoCode);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e1)
            {
                return false;

            }
        }
    }
}
