using Core.Application.Attribute;
using System;

namespace Core.Review.ViewModel
{
    [NoValidatorRequired]
    public class CustomerFeedbackReportFilter
    {
        public long FranchiseeId { get; set; }
        public long CustomerId { get; set; }
        public string Text { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ResponseStartDate { get; set; }
        public DateTime? ResponseEndDate { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public int? Response { get; set; }
        public int ResponseFrom { get; set; }
    }


    [NoValidatorRequired]
    public class CustomerFeedbackReportDomainModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string FranchiseeName { get; set; }
        public long FranchiseeId { get; set; }
        public long CustomerId { get; set; }
        public string Text { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ResponseStartDate { get; set; }
        public DateTime? ResponseEndDate { get; set; }
        public int ResponseFrom { get; set; }
        public long? Rating { get; set; }
        public long? Isapproved { get;set;}
    }

    [NoValidatorRequired]
    public class CustomerFeedbackReportResponseModel
    {
        public bool IsSuccess { get; set;}
        public string ErrorMessage { get; set;}
    }
}
