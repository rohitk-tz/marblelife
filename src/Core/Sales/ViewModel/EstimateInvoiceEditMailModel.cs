using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class EstimateInvoiceEditMailModel
    {
        public string Body { get; set; }
        public List<FileModel> FileModel { get; set; }
        public long? FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public decimal? LessDeposit { get; set; }
        public string MarketingClass { get; set; }
        public long? InvoiceCustomerId { get; set; }
        public long? NumberOfInvoices { get; set; }
        public string Option { get; set; }
        public decimal Price { get; set; }
        public decimal Balance { get; set; }
        public Franchisee Franchisee { get; set; }
        public string PhoneNumber { get; set; }
        public string CommunicationEmail { get; set; }
        public int? LessDepositPer { get; set; }
        public string Template { get; set; }
        public string EstimateNote { get; set; }
        public string InvoiceNote { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string SalesRep { get; set; }
        public string Code { get; set; }
        public string Url { get; set; }
        public string CustomerSignature { get; set; }
        public string IsSigned { get; set; }
        public List<EstimateInvoiceServiceEditMailModel> ServiceList { get; set; }
        public string SignDateTime { get; set; }
        public string ChooseOption { get; set; }
        public long? SchedulerId { get; set; }
        public string TechName { get; set; }
        public decimal Option1Total { get; set; }
        public decimal Option2Total { get; set; }
        public decimal Option3Total { get; set; }
        public string SalesRepEmail { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForFloorDiamond { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForHandDiamond { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForBrushes { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForPads { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForPolish { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForGroute { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForSealer { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForStripping { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForCoating { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForChips { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForKits { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForCleaner { get; set; }
        public List<ListViewModelForWorkOrder> WorkOrderForCareProducts { get; set; }
        public List<EstimateInvoiceDimensionTableViewModel> EstimateInvoiceDimensionTables { get; set; }
        public decimal? TotalArea { get; set; }
        public long? UserId { get; set; }
        public string FromEmail { get; set; }

        public string SignedInvoicesName { get; set; }
        public string UnsignedInvoicesName { get; set; }
        public bool AllInvoicesSigned { get; set; }

        public string CustomerPostSignature { get; set; }
        public string PostSignDateTime { get; set; }
        public string Technician { get; set; }
        public List<Application.Domain.File> FileDomainList { get; set; }

        public bool? IsFromUrl { get; set; }
        public bool? IsFromJob { get; set; }
        public long? NotesCount { get; set; }

        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public string OfficeNumber { get; set; }
        public long ToEmailId { get; set; }
        public string InvoiceNumber { get; set; }
        public string EstimateDate { get; set; }
        public string OfficeName { get; set; }
        public string OfficeAddressLine1 { get; set; }
        public string OfficeAddressLine2 { get; set; }
        public string OfficeAddressCity { get; set; }
        public string OfficeAddressState { get; set; }
        public string OfficeAddressCountry { get; set; }
        public string OfficeAddressZipCode { get; set; }
    }
    [NoValidatorRequired]
    public class EstimateInvoiceServiceEditMailModel
    {
        public long? Id { get; set; }

        public string ServiceType { get; set; }
        public string ServiceIds { get; set; }
        public string Description { get; set; }
        public string LocationName { get; set; }
        public string TypeOfSurface { get; set; }
        public string TypeOfStoneType { get; set; }
        public string TypeOfStoneColor { get; set; }
        public string TypeOfService { get; set; }
        public string typeOfStoneType3 { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public int InvoiceNumber { get; set; }

        public string Template { get; set; }
        public string Notes { get; set; }
        public List<SubItemEditModel> SubItem { get; set; }
        public string BackColor { get; set; }

        public decimal Option1Total { get; set; }
        public decimal Option2Total { get; set; }
        public decimal Option3Total { get; set; }
        public List<ListViewModel> LocationIds { get; set; }

        public double? Length { get; set; }
        public double? Width { get; set; }
        public double? Area { get; set; }
        public decimal SubItemTotalSumOption1 { get; set; }
        public decimal SubItemTotalSumOption2 { get; set; }
        public decimal SubItemTotalSumOption3 { get; set; }
       
        public long? SubItemNotesCount { get; set; }
    }
}
