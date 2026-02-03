using Core.Sales.Domain;

namespace Core.Reports
{
    public interface IUpdateBatchUploadRecordService
    {
        void UpdateData();
        void UpdateBatchRecord(SalesDataUpload upload);
    }
}
