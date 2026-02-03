using Core.Application.Attribute;
using Core.Application.ViewModel;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class JobEstimateEditModel : EditModelBase
    {
        public string AssigneeName { get; set; }
        public string AssigneePhone { get; set; }
        public long? Estimateid { get; set; }
        public long Id { get; set; }
        public int Hours { get; set; }
        public decimal Amount { get; set; }
        public long FranchiseeId { get; set; }
        public long? PersonId { get; set; }
        public string Franchisee { get; set; }
        public string SalesRep { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ActualStartDateString { get; set; }
        public DateTime ActualEndDateString { get; set; }
        public long? SalesRepId { get; set; }
        public long AssigneeId { get; set; }
        public long CustomerId { get; set; }
        public string Description { get; set; }
        public JobCustomerEditModel JobCustomer { get; set; }
        public string Title { get; set; }
        public JobSchedulerEditModel JobScheduler { get; set; }
        public long SchedulerId { get; set; }
        public IEnumerable<JobSchedulerEditModel> EstimateSchedulerList { get; set; }
        public IEnumerable<JobEditModel> JobList { get; set; }
        public IEnumerable<SchedulerNoteModel> Notes { get; set; }
        public string CreatedBy { get; set; }
        public string Location { get; set; }
        public bool IsImported { get; set; }
        public long? JobTypeId { get; set; }
        public string GeoCode { get; set; }
        public string JobType { get; set; }
        public bool IsVacation { get; set; }
        public long? UserId { get; set; }
        public string Assignee { get; set; }
        public List<long> idList { get; set; }
        public long? ParentID { get; set; }
        public bool? IsUpdate { get; set; }
        public long? MeetingID { get; set; }
        public string jobTypeName { get; set; }
        public string dateType { get; set; }

        public bool updateValue { get; set; }
        public bool? IsEquipment { get; set; }
        public long SchedulerStatus { get; set; }
        public string SchedulerStatusName { get; set; }
        public string SchedulerStatusColor { get; set; }
        public bool IsActive { get; set; }
        public long? SchedulerIds { get; set; }
        public bool? IsDataToBeUpdateForAllJobs { get; set; }
        public bool IsNewMeeting { get; set; }
        public long? LogginUserId { get; set; }
        public decimal? Worth { get; set; }
        public decimal? LessDeposit { get; set; }
        public string MarketingClassId { get; set; }

        public long? EstimateInvoiceId { get; set; }
        public bool IsInvoiceRequired { get; set; }
        public string InvoiceReason { get; set; }
        public bool IsInvoicePresent { get; set; }
        public List<JobEstimateSignatureEditModel> JobSignature { get; set; }
        public List<JobEstimateSignatureEditModel> JobSignaturePost { get; set; }
        public List<long?> AllInvoiceNumbersSigned { get; set; }
        public List<long?> AllInvoiceNumbersSignedPost { get; set; }
        public bool IsSigned { get; set; }
        public bool IsSignedPost { get; set; }

        public ShiftChargesViewModel ShiftChargesViewModel { get; set; }

        public List<MaintenanceViewModelCharges> MaintenanceChargesList { get; set; }

        public string HasCustomSignature { get; set; }
        public string NotHasCustomSignature { get; set; }
        public string Signature { get; set; }
        public float? EstimatedAmount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public JobEstimateEditModel()
        {
            JobCustomer = new JobCustomerEditModel { };
            JobScheduler = new JobSchedulerEditModel { };
            IsImported = false;
            idList = new List<long>();
            IsInvoiceRequired = true;
            ShiftChargesViewModel = new ShiftChargesViewModel { };
        }
    }
    [NoValidatorRequired]
    public class JobEstimateSignatureEditModel
    {
        public long? SchedulerId { get; set; }
        public bool IsSigned { get; set; }
        public string Signature { get; set; }
        public List<long?> InvoiceNumber { get; set; }

        public List<long?> TypeId { get; set; }
    }

    [NoValidatorRequired]
    public class ShiftChargesViewModel
    {
        public ShiftChargesViewModel()
        {
            ShiftChargesViewValues = new List<ShiftChargesViewDropDownModel>();
        }
        public decimal? TechDayShiftPrice { get; set; }
        public decimal? CommercialRestorationShiftPrice { get; set; }
        public decimal? MaintainanceTechNightShiftPrice { get; set; }

        public List<ShiftChargesViewDropDownModel> ShiftChargesViewValues { get; set; }

    }
    [NoValidatorRequired]
    public class ShiftChargesViewDropDownModel
    {
        public string Display { get; set; }
        public string Value { get; set; }
    }
}
