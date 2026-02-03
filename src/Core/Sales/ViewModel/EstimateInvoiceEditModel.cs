using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class EstimateInvoiceEditModel
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public long? CustomerId { get; set; }
        public long? EstimateId { get; set; }
        public long? EstimateInvoiceId { get; set; }
        public long? SchedulerId { get; set; }
        public long? FranchiseeId { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public float LessDeposit { get; set; }
        public string MarketingClass { get; set; }
        public long? InvoiceCustomerId { get; set; }
        public long? NumberOfInvoices { get; set; }
        public string Option { get; set; }
        public float Price { get; set; }
        public string Notes { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public List<ListViewModelForNotes> InvoiceNotesList { get; set; }
        public List<EstimateInvoiceServiceEditModel> ServiceList { get; set; }
        public bool? IsCustomerInvoice { get; set; }
    }
    [NoValidatorRequired]
    public class EstimateInvoiceServiceEditModel
    {
        public EstimateInvoiceServiceEditModel()
        {
            Measurements = new List<EstimateInvoiceDimensionEditModel>();
            ImageList = new List<EstimateInvoiceImageEditModel>();
            HoningMeasurementList = new List<HoningMeasurementViewModel>();
        }
        public long? Id { get; set; }
        public string Alias { get; set; }
        public string ServiceType { get; set; }
        public List<ListViewModel> ServiceIds1 { get; set; }
        public List<ListViewModel> LocationIds { get; set; }
        public string Description { get; set; }
        public List<string> LocationName1 { get; set; }
        public List<string> TypeOfSurface1 { get; set; }
        public List<string> TypeOfStoneType1 { get; set; }
        public List<string> TypeOfStoneColor1 { get; set; }
        public List<string> TypeOfService1 { get; set; }
        public List<string> typeOfStoneType3 { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public int InvoiceNumber { get; set; }
        public string Notes { get; set; }
        public string PriceNotes { get; set; }
        public List<SubItemEditModel> SubItem { get; set; }
        public bool? IsCross { get; set; }
        public bool? IsBundle { get; set; }
        public bool? IsActive { get; set; }
        public string BundleName { get; set; }
        public bool? IsMainBundle { get; set; }
        public List<EstimateInvoiceDimensionEditModel> Measurements { get; set; }
        public HoningMeasurementViewModel HoningMeasurement { get; set; }
        public List<HoningMeasurementViewModel> HoningMeasurementList { get; set; }
        public List<EstimateInvoiceImageEditModel> ImageList { get; set; }
    }

    [NoValidatorRequired]
    public class SubItemEditModel
    {
        public SubItemEditModel()
        {
            Measurements = new List<EstimateInvoiceDimensionViewModel>();
            ImageList = new List<EstimateInvoiceImageEditModel>();
            HoningMeasurementList = new List<HoningMeasurementViewModel>();
        }
        public bool? IsAlias { get; set; }
        public string Alias { get; set; }
        public long? Id { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string PriceNotes { get; set; }
        public ListViewModel ServiceIds { get; set; }

        public bool? IsCross { get; set; }
        public string BorderColor { get; set; }
        public bool? IsBundle { get; set; }
        public bool? IsActive { get; set; }
        public string BundleName { get; set; }
        public bool? IsMainBundle { get; set; }
        public long? ServiceTypeId { get; set; }
        public long? UnitTypeId { get; set; }
        public string UnitType { get; set; }
        public List<EstimateInvoiceDimensionViewModel> Measurements { get; set; }
        public HoningMeasurementViewModel HoningMeasurement { get; set; }
        public List<HoningMeasurementViewModel> HoningMeasurementList { get; set; }
        public List<EstimateInvoiceImageEditModel> ImageList { get; set; }
    }

    [NoValidatorRequired]
    public class EstimateInvoiceDimensionEditModel
    {
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Area { get; set; }
        public decimal? AreaTime { get; set; }
        public string Description { get; set; }
        public decimal? SetPrice { get; set; }
        public decimal? IncrementedPrice { get; set; }
        public long? UnitId { get; set; }
        public string UnitType { get; set; }
        public string Dimension { get; set; }
    }

    [NoValidatorRequired]
    public class InvoiceEstimateFilterModel
    {
        public long? Id { get; set; }
        public long? TypeId { get; set; }
        public long? SchedulerId { get; set; }
    }
    [NoValidatorRequired]
    public class EstimateInvoiceImageEditModel
    {
        public long? FileId { get; set; }
        public string RelativeLocation { get; set; }
        public string Caption { get; set; }
        public bool IsUploadedImage { get; set; }

    }

    public class EstimateInvoiceImageViewModel
    {
        public string Caption { get; set; }
        public bool IsUploadedImage { get; set; }
        public string RelativeLocation { get; set; }
    }
}
