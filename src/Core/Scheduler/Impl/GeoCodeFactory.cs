using Core.Application.Attribute;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    public class GeoCodeFactory : IGeoCodeFactory
    {
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        public GeoCodeFactory(IOrganizationRoleUserInfoService organizationRoleUserInfoService)
        {
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
        }

        public GeoCodefileupload CreateViewModel(CustomerFileUploadCreateModel model)
        {
            return new GeoCodefileupload
            {
                Id = model.Id,
                FileId = model.FileId,
                StatusId = model.StatusId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsNew = model.Id <= 0,
                notes = model.notes
            };
        }

        public ZipDataUploadViewModel CreateViewModel(GeoCodefileupload model)
        {
            var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(model.DataRecorderMetaData.CreatedBy.Value);
            var uploadedBy = userInfo.Email;
            return new ZipDataUploadViewModel
            {
                Id = model.Id,
                UploadedOn = model.DataRecorderMetaData.DateCreated,
                StatusId = model.StatusId,
                Status = model.Lookup.Name,
                UploadedBy = uploadedBy,
                IsEditable = false,
                Notes = model.notes,
                TempNotes = model.notes,
                FileId = model.FileId,
                LogFileId = model.ParsedLogFileId.GetValueOrDefault(),
                LogForCountyFileId = model.ParsedLogForCountyFileId.GetValueOrDefault(),
                LogForZipFileId = model.ParsedLogForZipFileId.GetValueOrDefault(),
                CountiesCount = model.CountiesCount!=0? model.CountiesCount.ToString():"",
                ZipCodeCount = model.ZipCodeCount != 0 ? model.ZipCodeCount.ToString() : "",
            };
        }

        public DownloadCountyModel CreateModelForCounty(County county, string countryName)
        {
            return new DownloadCountyModel
            {
                id = county.Id,
                IsUpdated = 0,
                IsDeleted = 0,
                ContractedTerritory = county.ContractedTerritory,
                CoveringLessThan3Hours = county.CoveringLessThan3Hours,
                CountyName = county.CountyName,
                DayTrip = county.DayTrip,
                DirectionCode = county.DirectionCode,
                DirectionFromOffice = county.DirectionFromOffice,
                FranchiseeName = county.FranchiseeName,
                //FranchiseMLD = county.FranchiseMLD,
                Population = county.Population,
                ReachingTime = county.ReachingTime,
                StateCode = county.StateCode,
                StateCountryCode = county.StateCountryCode,
                Status = county.Status,
                TerritoryCode = county.TerritoryCode,
                UnCovered = county.UnCovered,
                Country = countryName
            };
        }

        public DownloadZipCodeModel CreateModelForZipCode(ZipCode zipCode, string countyName, string cityName)
        {
            return new DownloadZipCodeModel
            {
                id = zipCode.Id,
                AreaCode = zipCode.AreaCode,
                Code = zipCode.Code,
                Direction = zipCode.Dir,
                //DriveTest = zipCode.DriveTest.GetValueOrDefault(),
                IsDeleted = 0,
                ZipCode = zipCode.Zip,
                County = countyName,
                City = cityName,
                StateCode = zipCode.StateCode,
                //FranchiseeTransferableNumber = zipCode.TransferableNumber
            };
        }

        public ZipDataInfoViewModel CreateViewModel(ZipCode domain, ZipDataInfoViewModel model, List<FranchiseeService> franchiseeService)
        {
            var phoneNumberFirst = "";
            var phoneNumberList = domain.County != null && domain.County.Franchisee != null ?
                domain.County.Franchisee.Organization.Phones != null ? domain.County.Franchisee.Organization.Phones : default : default;
            if (phoneNumberList != null)
            {
                foreach (var phoneNumber in phoneNumberList)
                {
                    if (phoneNumber.IsTransferable)
                    {
                        phoneNumberFirst = phoneNumber.Number;
                        break;
                    }
                }
            }
            var marketingPersonEmail = (domain.County != null && domain.County.Franchisee != null) ? domain.County.Franchisee.MarketingPersonEmail != null ?
domain.County.Franchisee.MarketingPersonEmail : domain.County.Franchisee.Organization.Email : "";
            return new ZipDataInfoViewModel
            {
                AreaCode = domain.AreaCode,
                City = domain.City != null ? domain.City.Name : domain.CityName,
                Country = domain.County != null && domain.County.Country != null ? domain.County.Country.Name : "N/A",
                DirectionCode = domain.County != null ? domain.County.DirectionCode : "(No Direction Code Assigned)",
                State = domain.StateCode,
                FranchiseeName = domain.County != null ? domain.County.FranchiseeName : "(Not Assigned Yet)",
                //PhoneNumber = model.PhoneNumber,
                PhoneNumber = phoneNumberFirst,
                FranchiseeId = model.FranchiseeId,
                CanSchedule = model.CanSchedule,
                County = domain.County != null ? domain.County.CountyName : "",
                EmailId = marketingPersonEmail,
                ContractedTeritory = (domain.County != null && (domain.County.ContractedTerritory != "" && domain.County.ContractedTerritory != null)) ? domain.County.ContractedTerritory : "N/A",
                ServicesList = franchiseeService.OrderByDescending(x=>x.IsCertified).Select(x => CreateServiceModel(x)).ToList(),
                Duration = domain.County != null && domain.County.Franchisee != null ? domain.County.Franchisee.Duration : default(long?),
                NotesFromCallCenter = domain.County != null && domain.County.Franchisee != null ? domain.County.Franchisee.NotesFromCallCenter : "",
                NotesFromOwner = domain.County != null && domain.County.Franchisee != null ? domain.County.Franchisee.NotesFromOwner : "",
                Duratiton = domain.County != null && domain.County.Franchisee != null ? domain.County.Franchisee.Duration : 1
            };
        }
        private ServiceViewModel CreateServiceModel(FranchiseeService franchiseeService)
        {
            return new ServiceViewModel()
            {
                IsActive = franchiseeService.IsActive,
                IsCertified = franchiseeService.IsCertified,
                ServiceName = franchiseeService.ServiceType.Name
            };
        }
    }


}
