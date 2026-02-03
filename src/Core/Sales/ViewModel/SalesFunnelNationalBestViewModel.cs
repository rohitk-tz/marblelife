using Core.Application.Attribute;
using System;
using System.Collections.Generic;


namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesFunnelNationalBestViewModel
    {
        public string TechCountFranchisee { get; set; }
        public string SaleCountFranchisee { get; set; }
        public string WebLeadCountFranchisee { get; set; }
        public string PhoneLeadCountFranchisee { get; set; }
        public string PhoneLeadCountOverTwoMinCountFranchisee { get; set; }
        public string EstimatesCountFranchisee { get; set; }
        public string JobsCountFranchisee { get; set; }
        public string RoyalityJobsCountFranchisee { get; set; }

        public string SalesCountFranchisee { get; set; }
        public string AvgTicketCountFranchisee { get; set; }
        public string SalesPerTechCountFranchisee { get; set; }
        public string SalesPerTechPerMonthCountFranchisee { get; set; }
        public string PhoneAnswerPerCountFranchisee { get; set; }
        public string EstimateConvertionCountFranchisee { get; set; }
        public string JobConvertionCountFranchisee { get; set; }
        public string SalesCloseRateCountFranchisee { get; set; }
        public string InvoicePerCountFranchisee { get; set; }
        public string MissedCallsCountFranchisee { get; set; }
        public string LostEstimateCountFranchisee { get; set; }
        public string LostJobsCountFranchisee { get; set; }
        public string TotalJobsCountFranchisee { get; set; }
        public string TotalCallsCountFranchisee { get; set; }
    }
}
