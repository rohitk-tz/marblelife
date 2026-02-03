using Core.Organizations.Domain;
using Core.Sales.ViewModel;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using System.Collections.Generic;

namespace Core.Scheduler
{
    public interface IGeoCodeFactory
    {
        GeoCodefileupload CreateViewModel(CustomerFileUploadCreateModel model);
        ZipDataUploadViewModel CreateViewModel(GeoCodefileupload model);
        DownloadCountyModel CreateModelForCounty(County domain, string countryName);
        DownloadZipCodeModel CreateModelForZipCode(ZipCode zipCode, string countyName, string cityName);
        ZipDataInfoViewModel CreateViewModel(ZipCode domain, ZipDataInfoViewModel model,List<FranchiseeService> franchiseeService);
    }
}
