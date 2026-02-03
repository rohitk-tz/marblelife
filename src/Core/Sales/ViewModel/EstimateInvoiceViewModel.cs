using Core.Application.Attribute;
using Core.Application.ValueType;
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.ViewModel
{
    public class EstimateInvoiceViewModel
    {
        public EstimateInvoiceViewModel()
        {
            InvoiceNotesList = new List<ListViewModelForNotes>();
            ServiceList = new List<EstimateInvoiceServiceViewModel>();

        }
        public long? Id { get; set; }
        public string MailBody { get; set; }
        public string Title { get; set; }
        public long? CustomerId { get; set; }
        public long? EstimateId { get; set; }
        public long? SchedulerId { get; set; }
        public long? FranchiseeId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Email { get; set; }
        public string CcEmail { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public decimal? LessDeposit { get; set; }
        public string InvoiceCount { get; set; }
        public long? InvoiceCustomerId { get; set; }
        public long? NumberOfInvoices { get; set; }
        public long? EstimateInvoiceId { get; set; }
        public string MarketingClass { get; set; }
        public string Option { get; set; }
        public string Notes { get; set; }

        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }

        //public List<MaintenanceViewModelCharges> MaintenanceChargesList { get; set; }
        public List<ListViewModelForNotes> InvoiceNotesList { get; set; }
        //public List<string> InvoiceNotesList { get; set; }
        public List<EstimateInvoiceServiceViewModel> ServiceList { get; set; }

        public bool? IsCustomerAvailable { get; set; }
    }

    [NoValidatorRequired]
    public class MaintenanceViewModelCharges
    {
        public string Material { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }
    }
    [NoValidatorRequired]
    public class EstimateInvoiceServiceViewModel
    {
        public EstimateInvoiceServiceViewModel()
        {
            HoningMeasurementList = new List<HoningMeasurementViewModel>();
        }
        public bool? IsAlias { get; set; }
        public string Alias { get; set; }
        public long? Id { get; set; }
        public List<ListViewModel> LocationIds { get; set; }
        public string ServiceType { get; set; }
        public List<ListViewModel> ServiceIds { get; set; }
        public string Description { get; set; }
        public string LocationName { get; set; }
        public string TypeOfSurface { get; set; }
        public string TypeOfStoneType { get; set; }
        public string TypeOfStoneColor { get; set; }
        public string TypeOfService { get; set; }
        public float LessDeposit { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string InvoiceNumber { get; set; }
        public string OldInvoiceNumber { get; set; }
        public string Notes { get; set; }
        public string PriceNotes { get; set; }
        public string TypeOfStoneType2 { get; set; }
        public bool IsExpand { get; set; }
        public List<SubItemEditModel> SubItem { get; set; }
        public bool? IsCross { get; set; }
        public string BorderColor { get; set; }
        public bool? IsBundle { get; set; }
        public bool? IsActive { get; set; }
        public string BundleName { get; set; }
        public bool? IsMainBundle { get; set; }
        public List<EstimateInvoiceDimensionViewModel> Measurements { get; set; }
        public string AssigneeName { get; set; }
        public long? ServiceTypeId { get; set; }
        public long? UnitTypeId { get; set; }
        public string UnitType { get; set; }
        public HoningMeasurementViewModel HoningMeasurement { get; set; }
        public List<HoningMeasurementViewModel> HoningMeasurementList { get; set; }
        public List<EstimateInvoiceServiceImageModel> ImageList { get; set; }
    }

    [NoValidatorRequired]
    public class EstimateInvoiceServiceDescriptionViewModel
    {
        public string ServiceType { get; set; }
        public string Description { get; set; }
    }
    [NoValidatorRequired]
    public class ListViewModel
    {
        public string Id { get; set; }
        public string Notes { get; set; }
    }
    [NoValidatorRequired]
    public class ListViewModelForNotes
    {
        public long? Id { get; set; }
        public string Notes { get; set; }
        public long? InvoiceNumber { get; set; }
    }

    [NoValidatorRequired]
    public class ListViewModelForWorkOrder
    {
        public string CategoryName { get; set; }
        public string Name { get; set; }
        public bool IsPresent { get; set; }
        public long? InvoiceNumber { get; set; }
    }
    [NoValidatorRequired]
    public class EstimateInvoiceDimensionViewModel
    {
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Area { get; set; }
        public decimal? AreaTime { get; set; }
        public string Description { get; set; }
        public long? UnitId { get; set; }
        public string UnitType { get; set; }
        public decimal? SetPrice { get; set; }
        public decimal? IncrementedPrice { get; set; }
        public bool IsSaved { get; set; }
        public string Dimension { get; set; }
    }
    [NoValidatorRequired]
    public class EstimateInvoiceDimensionTableViewModel
    {
        public int InvoiceLine { get; set; }
        public List<EstimateInvoiceDimensionViewModel> DimensionList { get; set; }
    }

    [NoValidatorRequired]
    public class HoningMeasurementViewModel
    {
        public decimal? SeventeenBase { get; set; }
        public decimal? TotalArea { get; set; }
        public string ShiftName { get; set; }
        public decimal? ShiftPrice { get; set; }
        public decimal? Area { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Sections { get; set; }
        public decimal? UGC { get; set; }
        public decimal? Thirty { get; set; }
        public decimal? Fifty { get; set; }
        public decimal? Hundred { get; set; }
        public decimal? TwoHundred { get; set; }
        public decimal? FourHundred { get; set; }
        public decimal? EightHundred { get; set; }
        public decimal? FifteenHundred { get; set; }
        public decimal? ThreeThousand { get; set; }
        public decimal? EightThousand { get; set; }
        public decimal? ElevenThousand { get; set; }
        public decimal? Caco { get; set; }
        public decimal? Ihg { get; set; }
        public string Dimension { get; set; }
        public decimal? ProdutivityRate { get; set; }
        public decimal? TotalAreaInHour { get; set; }
        public decimal? TotalAreaInShift { get; set; }
        public string TotalMinute { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? TotalCostPerSquare { get; set; }
        public decimal? MinRestoration { get; set; }
        public decimal? StartingPointTechShiftEstimates { get; set; }
        public string RowDescription { get; set; }
        public bool IsSaved { get; set; }
        #region Original
        public decimal? SectionsOriginal { get; set; }
        public decimal? UGCOriginal { get; set; }
        public decimal? ThirtyOriginal { get; set; }
        public decimal? FiftyOriginal { get; set; }
        public decimal? HundredOriginal { get; set; }
        public decimal? TwoHundredOriginal { get; set; }
        public decimal? FourHundredOriginal { get; set; }
        public decimal? EightHundredOriginal { get; set; }
        public decimal? FifteenHundredOriginal { get; set; }
        public decimal? ThreeThousandOriginal { get; set; }
        public decimal? EightThousandOriginal { get; set; }
        public decimal? ElevenThousandOriginal { get; set; }
        public decimal? CacoOriginal { get; set; }
        public decimal? IhgOriginal { get; set; }
        public decimal? ProdutivityRateOriginal { get; set; }
        public string TotalMinuteOriginal { get; set; }
        public decimal? TotalAreaInHourOriginal { get; set; }
        public decimal? TotalAreaInShiftOriginal { get; set; }
        public decimal? SeventeenBaseOriginal { get; set; }
        public decimal? TotalAreaOriginal { get; set; }
        public decimal? TotalCostOriginal { get; set; }
        public decimal? TotalCostPerSquareOriginal { get; set; }
        public decimal? MinRestorationOriginal { get; set; }
        public bool HasOriginalValues { get; set; }
        #endregion

    }
    [NoValidatorRequired]
    public class EstimateInvoiceServiceGetServiceModel
    {
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string TypeOfStoneType2 { get; set; }
    }

    [NoValidatorRequired]
    public class EstimateInvoiceServiceGetServiceResultModel
    {
        public long? ServiceId { get; set; }
    }
    [NoValidatorRequired]
    public class EstimateInvoiceServiceImageModel
    {
        public string RelativeLocation { get; set; }
        public string Caption { get; set; }
        public bool? IsUploadedImage { get; set; }
        public long? FileId { get; set; }
    }

}
