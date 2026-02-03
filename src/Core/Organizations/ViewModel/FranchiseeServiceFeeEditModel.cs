using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel

{
    [NoValidatorRequired]
    public class FranchiseeServiceFeeListsModel
    {
        public BalanceLeft AmountPaid { get; set; }
        public BalanceLeft Balance { get; set; }
        public bool IsRoyality { get; set; }
        public List<FranchiseeServiceFeeEditModel> Collection { get; set; }

    }
    [NoValidatorRequired]
    public class FranchiseeServiceFeeEditModel
    {
        public long FranchiseeId { get; set; }
        public long Id { get; set; }
        public bool? IsCertified { get; set; }
        public bool IsApplicable { get; set; }
        public decimal Amount { get; set; }
        public decimal? Percentage { get; set; }
        public long? FrequencyId { get; set; }
        public long TypeId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public bool AddOneTimeProject { get; set; }
        public long? InvoiceItemId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? Duration { get; set; }
        public long CurrencyExchangeRateId { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
        public bool? IsAdfund { get; set; }
        public string LoanAdjustment { get; set; }
        public ICollection<LoanScheduleViewModel> LoanSchedule { get; set; }
        public bool? IsEditable { get; set; }
        public string LoanAdjustmentId { get; set; }
        public bool? IsEditing { get; set; }
        public bool? IsRoyality { get; set; }
        public bool IsLoanCompletetd { get; set; }
        public bool? IsLoanPayed { get; set; }
        public DateTime? StartDate { get; set; }
        public bool IsRoyolity { get; set; }
        public List<LoanScheduleViewModel> LoanScheduleList { get; set; }
        public bool? IsCompleted { get; set; }
        public long? LoanTypeId { get; set; }
        public string LoanType { get; set; }
    }

    public class BalanceLeft
    {
        public decimal AmountActuallyPaid { get; set; }
        public decimal OverPaidAmount { get; set; }
        public decimal Balance { get; set; }
        public decimal CurrencyRate { get; set; }
        public string CurrencyCode { get; set; }
    }
    [NoValidatorRequired]
    public class FranchiseePrePayLoanFeeEditModel
    {
        public long Id { get; set; }
        public decimal PrePayAmount { get; set; }
        public long? LoanTypeId { get; set; }
    }
}
