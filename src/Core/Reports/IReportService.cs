using Core.Billing.Domain;
using Core.Notification.ViewModel;
using Core.Reports.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Reports
{
    public interface IReportService
    {
        ServiceReportListModel GetReportsForService(ServiceReportListFilter filter, int pageNumber, int pageSize);
        LateFeeReportListModel GetLateFeeReportList(LateFeeReportFilter filter, int pageNumber, int pageSize);
        bool DownloadSalesReport(ServiceReportListFilter filter, out string fileName);
        bool DownloadLateFeeReport(LateFeeReportFilter filter, out string fileName);
        TopLeadersListModel GetServiceLeaderList(TopLeadersFilter filter);
        TopLeadersListModel GetClassLeaderList(TopLeadersFilter filter);
        UploadBatchReportListModel GetBatchReport(UploadReportFilter filter, int pageNumber, int pageSize);
        bool DownloadUploadReport(UploadReportFilter filter, out string fileName);
        ARReportListModel GetARReportList(ArReportFilter filter);
        IEnumerable<WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel> GetArReportModel(IEnumerable<FranchiseeInvoice> franchiseeInvoices, DateTime startDate, DateTime endDate, ArReportFilter filter = null);
        PriceEstimatePageViewModel GetPriceEstimateList(PriceEstimateGetModel model, long userId, long roleUserId);
        PriceEstimateViewModel GetPriceEstimate(PriceEstimateGetModel model, long userId, long roleUserId);
        bool SaveBulkCorporatePriceEstimate(PriceEstimateSaveCorporatePriceModel model, long roleUserId);
        bool BulkUpdateCorporatePrice(PriceEstimateBulkUpdateModel model, long roleUserId);
        bool SavePriceEstimateFranchiseeWise(PriceEstimateSaveModel model, long roleUserId);
        bool SavePriceEstimateBulkUpdate(PriceEstimateBulkUpdateModel model, long roleUserId);
        PriceEstimatePageViewModel GetPriceEstimateCollectionPerFranchisee(PriceEstimateGetModel model, long userId, long roleUserId);
        ShiftChargesViewModel GetShiftCharges(long userId, long roleUserId);
        bool SaveShiftCharges(ShiftChargesSaveModel model, long userId, long roleUserId);
        ReplacementChargesViewModel GetReplacementCharges(long userId, long roleUserId);
        bool SaveReplacementCharges(ReplacementChargesSaveModel model, long userId, long roleUserId);
        MaintenanceChargesViewModel GetMaintenanceCharges(long userId, long roleUserId);
        bool SaveMaintenanceCharges(MaintenanceChargesSaveModel model, long userId, long roleUserId);
        bool SaveFloorGrindingAdjustmentNote(long userId, long roleUserId, FloorGrindingAdjustNoteSaveModel model);
        FloorGrindingAdjustmentViewModel GetFloorGrindingAdjustment(long userId, long roleUserId);
        SeoHistryViewModel GetSeoHistry(SeoHistryModel model);
        bool SaveSeoNotes(SeoNotesModel model);
        bool DownloadPriceEstimateDataFile(PriceEstimateGetModel filter, long userId, long roleUserId, out string fileName);
        void SaveFile(PriceEstimateExcelUploadModel model);
        PriceEstimateDataUploadListModel GetPriceEstimateUploadList(PriceEstimateDataListFilter filter, int pageNumber, int pageSize, long organisationRoleUserId);

        bool SaveServiceTagNotes(PriceEstimateServiceTagNotesModel filter, long userId);
        PriceEstimateServiceTagNotesGetModel GetServiceTagNotes(PriceEstimateServiceTagNotesGetModel filter);
    }
}
