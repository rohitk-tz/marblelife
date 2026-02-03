using Core.Application.Attribute;
using System;
using System.ComponentModel;


namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesFunnelLocalExcelViewModel
    {

        [DisplayName("Month")]
        public string Month { get; set; }
        [DownloadField(Required = false)]
        public long? FranchiseeId { get; set; }
        [DisplayName("Franchisee Name")]
        public string FranchiseeName { get; set; }
        [DisplayName("(A)" + " " + "Number of Tech Users")]
        public string TechCount { get; set; }
        [DisplayName("(B)" + " " + "Number of Sales Users")]
        public string SaleCount { get; set; }
        [DisplayName("(C)" + " " + "Number of Web Leads")]
        public string WebLeadCount { get; set; }
        [DisplayName("(D)" + " " + "Number of Phone Leads")]
        public string PhoneLeadCount { get; set; }
        [DisplayName("(E)" + " " + "Number Of Phone Leads Over 2 min")]
        public string PhoneLeadCountOverTwoMin { get; set; }
        [DisplayName("(F)" + " " + "Number Of Estimates on Schedule")]
        public string EstimatesCount { get; set; }
        [DisplayName("(G)" + " " + "Number Of Jobs on Schedule")]
        public string JobsCount { get; set; }
        [DisplayName("(H)" + " " + "Number Of Royalty Jobs on Schedule")]
        public string RoyalityJobs { get; set; }
        [DisplayName("(I)" + " " + "Sales(in $)")]
        public string Sales { get; set; }
        [DisplayName("(J)" + " " + "Ave Ticket" + " " + "(I/H)")]
        public string AvgTicket { get; set; }
        [DisplayName("(K)" + " " + "Sales/Tech" + " " + "(I/A)")]
        public string SalesPerTech { get; set; }
        [DisplayName("(L)" + " " + "Sales/(Tech* months)" + " " + "(K/(months))")]
        public string SalesPerTechPerMonth { get; set; }
        [DisplayName("(M)" + " " + "% Phone Answered(Over 2 min)/Total Phone" + " " + "(E/D)")]
        public string PhoneAnswerPer { get; set; }
        [DisplayName("(N)" + " " + "% convert to Estimate" + " " + "(F/E)")]
        public string EstimateConvertion { get; set; }
        [DisplayName("(O)" + " " + "% convert to Job" + " " + "(G/F)")]
        public string JobConvertion { get; set; }
        [DisplayName("(P)" + " " + "% convert to Invoice" + " " + "(H/F)")]
        public string InvoicePer { get; set; }
        [DisplayName("(Q)" + " " + "Sales Close Rate" + " " + "(H(C+D))")]
        public string SalesCloseRate { get; set; }
        [DisplayName("(R)" + " " + "Missed Calls" + " " + "(D-E)")]
        public string MissedCalls { get; set; }
        [DisplayName("(S)Lost Estimate" + " " + "(E-F)")]
        public string LostEstimate { get; set; }
        [DisplayName("(T)" + " " + "Lost Jobs" + " " + "(F-G)")]
        public string LostJobs { get; set; }
        [DisplayName("(U)" + " " + "Total Jobs" + " " + "(H)")]
        public string TotalJobs { get; set; }
        [DisplayName("(V)" + " " + "Total Calls" + " " + "(R+S+T+U)")]
        public string TotalCalls { get; set; }
    }
}
