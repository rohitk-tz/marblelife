using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;

namespace Core.Sales
{
    public interface ISalesDataUploadService
    {
        void Save(SalesDataUploadCreateModel model);
        SalesDataUploadListModel GetBatchList(SalesDataListFilter filter, int pageNumber, int pageSize);
        DateTime? GetLastUploadedBatch(long franchiseeId);
        SalesDataUpload GetSalesDataUploadByFranchiseeId(long franchiseeId);
        bool Delete(long id);
        bool DoesOverlappingDatesExist(long franchiseeId, DateTime startDate, DateTime endDate);
        bool CheckValidRangeForSalesUpload(SalesDataUploadCreateModel model);
        void Update(SalesDataUploadCreateModel model);
        bool Reparse(long id);
        DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek);
        AnnualUploadValidationModel GetAnnualUploadInfo(AnnualUploadValidationModel model);
        DateTime GetLastUploadDateOfYear(DateTime lastUploadStartDate, long? paymentFrequencyId);
        void SaveAnnualUpload(SalesDataUploadCreateModel model, SalesDataUpload salesdataUpload);
        bool isValidUpload(SalesDataUploadCreateModel model);
        bool CheckForExpiringDocument(SalesDataUploadCreateModel model);
    }
}
