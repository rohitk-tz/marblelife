using Core.Sales.ViewModel;

namespace Core.Sales
{
    public interface IAnnualSalesDataUploadService
    {
        SalesDataUploadListModel GetBatchList(SalesDataListFilter filter, int pageNumber, int pageSize);
        AnnualAuditSalesListModel GetAnnualSalesData(SalesDataListFilter filter);
        AuditInvoiceViewModel InvoiceDetails(long invoiceId, long auditInvoiceId);
        bool Delete(long id);
        bool ManageBatch(bool isAccept, long batchId);
        AnnualAuditSalesListModel GetAnnualAuditRecord(SalesDataListFilter filter);
        bool DownloadAnnualDataFile(SalesDataListFilter filter, out string fileName);
        bool SaveUpload(AnnualDataUploadCreateModel model);
        AnnualAuditSalesListModel GetAnnualSalesDataAddress(SalesDataListFilter filter);
        AnnualSalesDataCustonerListModel GetAnnualSalesCustomerAddress(AnnualSalesDataListFiltercs filter, int pageNumber, int pageSize);
        bool isValidUpload(AnnualDataUploadCreateModel model);
        bool UpdateCustomerAddress(AnnualSalesDataCustomerViewModel filter);
        bool ReparseAnnualReport(long? Id);
        bool DownloadAnnualDataFileFormatted(SalesDataListFilter filter, out string fileName);
    }
}
