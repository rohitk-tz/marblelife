using Core.Sales.ViewModel;
using Core.Scheduler.ViewModel;

namespace Core.Scheduler
{
    public interface IGeoCodeService
    {
        void SaveFile(CustomerFileUploadCreateModel model);
        ZipDataUploadListModel GetZipList(ZipDataListFilter filter, int pageNumber, int pageSize);
        bool CreateExcelForAllFiles(ZipDataListFilter filter, out string fileName);
        ZipDataInfoListModel GetZipInfo(ZipCodeInfoListFilter filter);
        ZipDataInfoViewModel GetZipInfoScheduler(long franchiseeId);
        GeoCodeDataInfoViewModel GetZipInfoByZipCode(string zipCode, string state, long? franchiseeId, long? countryId);
        bool SaveGeoCodeNotes(ZipDataUploadViewModel filter);
        bool DeleteGeoCodeRecord(long? recordid);
        bool ReparseFile(long? recordid);
    }
}
