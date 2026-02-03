using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class TimeEstimateViewModel
    {
        public TimeEstimateViewModel()
        {
        }
    }
    #region ShiftCharges
    [NoValidatorRequired]
    public class ShiftChargesViewModel
    {
        public long? FranchiseeId { get; set; }
        public decimal? TechDayShiftPrice { get; set; }
        public decimal? AverageTechDayShiftPrice { get; set; }
        public decimal? MaximumTechDayShiftPrice { get; set; }
        public decimal? CommercialRestorationShiftPrice { get; set; }
        public decimal? AverageCommercialRestorationShiftPrice { get; set; }
        public decimal? MaximumCommercialRestorationShiftPrice { get; set; }
        public decimal? MaintenanceTechNightShiftPrice { get; set; }
        public decimal? AverageMaintenanceTechNightShiftPrice { get; set; }
        public decimal? MaximumMaintenanceTechNightShiftPrice { get; set; }
        public string FranchiseeNameForMaximumTechDayShiftPrice { get; set; }
        public string FranchiseeNameForMaximumCommercialRestorationShiftPrice { get; set; }
        public string FranchiseeNameForMaximumMaintenanceTechNightShiftPrice { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }

        public decimal? TechDayShiftPriceCorporate { get; set; }
        public decimal? CommercialRestorationShiftPriceCorporate { get; set; }
        public decimal? MaintenanceTechNightShiftPriceCorporate { get; set; }

    }

    [NoValidatorRequired]
    public class ShiftChargesSaveModel
    {
        public decimal? TechDayShiftPrice { get; set; }
        public decimal? CommercialRestorationShiftPrice { get; set; }
        public decimal? MaintenanceTechNightShiftPrice { get; set; }

    }
    #endregion

    #region ReplacementCharges
    [NoValidatorRequired]
    public class ReplacementChargesModel
    {
        public string Material { get; set; }
        public decimal? CostOfRemovingTile { get; set; }
        public decimal? AverageCostOfRemovingTile { get; set; }
        public decimal? MaximumCostOfRemovingTile { get; set; }
        public string FranchiseeNameForMaximumCostOfRemovingTile { get; set; }
        public decimal? CorporateCostOfRemovingTile { get; set; }
        public decimal? CostOfInstallingTile { get; set; }
        public decimal? AverageCostOfInstallingTile { get; set; }
        public decimal? MaximumCostOfInstallingTile { get; set; }
        public string FranchiseeNameForMaximumCostOfInstallingTile { get; set; }
        public decimal? CorporateCostOfInstallingTile { get; set; }
        public decimal? CostOfTileMaterial { get; set; }
        public decimal? AverageCostOfTileMaterial { get; set; }
        public decimal? MaximumCostOfTileMaterial { get; set; }
        public string FranchiseeNameForMaximumCostOfTileMaterial { get; set; }
        public decimal? CorporateCostOfTileMaterial { get; set; }
        public decimal? TotalReplacementCost { get; set; }
        public decimal? AverageTotalReplacementCost { get; set; }
        public decimal? MaximumTotalReplacementCost { get; set; }
        public string FranchiseeNameForMaximumTotalReplacementCost { get; set; }
        public decimal? CorporateTotalReplacementCost { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    [NoValidatorRequired]
    public class ReplacementChargesViewModel
    {
        public ReplacementChargesViewModel()
        {
            ReplacementChargesList = new List<ReplacementChargesModel>();
        }

        public List<ReplacementChargesModel> ReplacementChargesList { get; set; }
    }
    [NoValidatorRequired]
    public class ReplacementChargesSaveModel
    {
        public ReplacementChargesSaveModel()
        {
            ReplacementChargesList = new List<ReplacementChargesModel>();
        }

        public List<ReplacementChargesModel> ReplacementChargesList { get; set; }
    }
    #endregion ReplacementCharges


    #region MaintenanceCharges
    [NoValidatorRequired]
    public class MaintenanceChargesModel
    {
        public string Material { get; set; }
        public decimal? High { get; set; }
        public decimal? AverageHigh { get; set; }
        public decimal? MaximumHigh { get; set; }
        public decimal? CorporateHigh { get; set; }
        public decimal? Low { get; set; }
        public decimal? AverageLow { get; set; }
        public decimal? MaximumLow { get; set; }
        public decimal? CorporateLow { get; set; }
        public string UOM { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public string FranchiseeNameForMaximumHigh { get; set; }
        public string FranchiseeNameForMaximumLow { get; set; }
        public string Notes { get; set; }
    }
    [NoValidatorRequired]
    public class MaintenanceChargesViewModel
    {
        public MaintenanceChargesViewModel()
        {
            MaintenanceChargesList = new List<MaintenanceChargesModel>();
        }

        public List<MaintenanceChargesModel> MaintenanceChargesList { get; set; }
    }
    [NoValidatorRequired]
    public class MaintenanceChargesSaveModel
    {
        public MaintenanceChargesSaveModel()
        {
            MaintenanceChargesList = new List<MaintenanceChargesModel>();
        }

        public List<MaintenanceChargesModel> MaintenanceChargesList { get; set; }
    }
    #endregion MaintenanceCharges

    #region FloorGrindingAdjustment
    [NoValidatorRequired]
    public class FloorGrindingAdjustmentModel
    {
        public string DiameterOfGrindingPlate { get; set; }
        public decimal? Area { get; set; }
        public decimal? AdjustmentFactor { get; set; }
        public bool IsPriceChangedByFranchisee { get; set; }
        public bool IsActive { get; set; }
    }
    [NoValidatorRequired]
    public class FloorGrindingAdjustmentViewModel
    {
        public FloorGrindingAdjustmentViewModel()
        {
            FloorGrindingAdjustmentList = new List<FloorGrindingAdjustmentModel>();
        }

        public List<FloorGrindingAdjustmentModel> FloorGrindingAdjustmentList { get; set; }

        public string Note { get; set; }
    }

    [NoValidatorRequired]

    public class FloorGrindingAdjustNoteSaveModel
    {
        public string Note { get; set; }
    }


    [NoValidatorRequired]

    public class SeoHistryModel
    {
        public string Text { get; set; }

        public int pageNumber { get; set; }
        public int pageSize { get; set; }
    }


    [NoValidatorRequired]
    public class SeoHistryViewModel
    {
        public SeoHistryViewModel()
        {
            SeoHistryListModel = new List<SeoHistryListModel>();
        }

        public List<SeoHistryListModel> SeoHistryListModel { get; set; }

        public string Note { get; set; }
        public int Count { get; set; }
    }

    [NoValidatorRequired]
    public class SeoHistryListModel
    {

        public string FranchiseeName { get; set; }
        public decimal? Price { get; set; }
        public string SchedulerUrl { get; set; }
        public string UserName { get; set; }
        public string AddedOn { get; set; }
        public long SchedulerId { get; set; }
        public long EstimateId { get; set; }
        public long HoiningMeasurementId { get; set; }
        public string Notes { get; set; }

        public string NotesAddedBy { get; set; }

    }


    [NoValidatorRequired]

    public class SeoNotesModel
    {
        public string Notes { get; set; }
        public long? HoiningMeasurementId { get; set; }
    }
    #endregion MaintenanceCharges
}
