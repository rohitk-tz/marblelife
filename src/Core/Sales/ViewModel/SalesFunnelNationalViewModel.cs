using Core.Application.Attribute;
using System;
using System.ComponentModel;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesFunnelNationalViewModel
    {
        [DownloadField(Required = false)]
        public long? FranchiseeId { get; set; }
        [DisplayName("Franchisee Name")]
        public string FranchiseeName { get; set; }
        [DisplayName("Number of Tech Users")]
        public double TechCount { get; set; }
        [DisplayName("Number of Sales Users")]
        public double SaleCount { get; set; }
        [DisplayName("Number of Web Leads")]
        public double WebLeadCount { get; set; }
        [DisplayName("Number of Phone Leads")]
        public double PhoneLeadCount { get; set; }
        [DisplayName("Number Of Phone Leads Over 2 min")]
        public double PhoneLeadCountOverTwoMin { get; set; }
        [DisplayName("Number Of Estimates on Schedule")]
        public double EstimatesCount { get; set; }
        [DisplayName("Number Of Jobs on Schedule")]
        public double JobsCount { get; set; }
        [DisplayName("Number Of Royality Jobs on Schedule")]
        public double RoyalityJobs { get; set; }
        [DisplayName("Sales(in $)")]
        public decimal Sales { get; set; }
        [DisplayName("Ave Ticket")]
        public decimal AvgTicket { get; set; }
        [DisplayName("Sales/Tech")]
        public decimal SalesPerTech { get; set; }
        [DisplayName("Sales/(Tech* months)")]
        public decimal SalesPerTechPerMonth { get; set; }
        [DisplayName("% Phone Answered(Over 2 min)/Total Phone")]
        public double PhoneAnswerPer { get; set; }
        [DisplayName("% convert to Estimate")]
        public double EstimateConvertion { get; set; }
        [DisplayName("	% convert to Job")]
        public double JobConvertion { get; set; }
        [DisplayName("% convert to Invoice")]
        public double InvoicePer { get; set; }
        [DisplayName("Sales Close Rate")]
        public double SalesCloseRate { get; set; }
    }
}
