using Core.Application.Attribute;
using Core.Organizations;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesFunnelFactory : ISalesFunnelFactory
    {
        public SalesFunnelNationalExcelViewModel CreateListModel(SalesFunnelNationalViewModel domain)
        {
            return new SalesFunnelNationalExcelViewModel()
            {
                AvgTicket = domain.AvgTicket > 0 ? domain.AvgTicket.ToString("#.##") : domain.AvgTicket.ToString(),
                EstimateConvertion = domain.EstimateConvertion > 0 ? domain.EstimateConvertion.ToString("#.##") : domain.EstimateConvertion.ToString(),
                EstimatesCount = domain.EstimatesCount > 0 ? domain.EstimatesCount.ToString() : domain.EstimatesCount.ToString(),
                FranchiseeName = domain.FranchiseeName,
                InvoicePer = domain.InvoicePer > 0 ? domain.InvoicePer.ToString("#.##") : domain.InvoicePer.ToString(),
                JobConvertion = domain.JobConvertion > 0 ? domain.JobConvertion.ToString("#.##") : domain.JobConvertion.ToString(),
                JobsCount = domain.JobsCount.ToString(),
                PhoneAnswerPer = domain.PhoneAnswerPer>0? domain.PhoneAnswerPer.ToString("#.##"): domain.PhoneAnswerPer.ToString(),
                PhoneLeadCount = domain.PhoneLeadCount.ToString(),
                PhoneLeadCountOverTwoMin = domain.PhoneLeadCountOverTwoMin.ToString(),
                RoyalityJobs = domain.RoyalityJobs.ToString(),
                SaleCount = domain.SaleCount.ToString(),
                Sales = domain.Sales > 0 ? domain.Sales.ToString("#.##") : domain.Sales.ToString(),
                SalesCloseRate = domain.SalesCloseRate > 0 ? domain.SalesCloseRate.ToString("#.##") : domain.SalesCloseRate.ToString(),
                SalesPerTech = domain.SalesPerTech > 0 ? domain.SalesPerTech.ToString("#.##") : domain.SalesPerTech.ToString(),
                SalesPerTechPerMonth = domain.SalesPerTechPerMonth > 0 ? domain.SalesPerTechPerMonth.ToString("#.##") : domain.SalesPerTechPerMonth.ToString(),
                TechCount = domain.TechCount.ToString(),
                WebLeadCount = domain.WebLeadCount.ToString(),
                FranchiseeId = domain.FranchiseeId,
            };
        }

        public SalesFunnelLocalExcelViewModel CreateListModel(SalesFunnelLocalViewModel domain)
        {
            return new SalesFunnelLocalExcelViewModel()
            {
                AvgTicket = domain.AvgTicket>0? domain.AvgTicket.ToString("#.##"): domain.AvgTicket.ToString(),
                EstimateConvertion = domain.EstimateConvertion>0? domain.EstimateConvertion.ToString("#.##"): domain.EstimateConvertion.ToString(),
                EstimatesCount = domain.EstimatesCount.ToString(),
                FranchiseeName = domain.FranchiseeName,
                InvoicePer = domain.InvoicePer > 0 ? domain.InvoicePer.ToString("#.##") : domain.InvoicePer.ToString(),
                JobConvertion = domain.JobConvertion>0?domain.JobConvertion.ToString("#.##"): domain.JobConvertion.ToString(),
                JobsCount = domain.JobsCount.ToString(),
                PhoneAnswerPer = domain.PhoneAnswerPer.ToString("#.##"),
                PhoneLeadCount = domain.PhoneLeadCount.ToString("#.##"),
                PhoneLeadCountOverTwoMin = domain.PhoneLeadCountOverTwoMin.ToString("#.##"),
                RoyalityJobs = domain.RoyalityJobs > 0 ? domain.RoyalityJobs.ToString("#.##") : domain.RoyalityJobs.ToString(),
                SaleCount = domain.SaleCount.ToString(),
                Sales = domain.Sales.ToString("#.##"),
                SalesCloseRate = domain.SalesCloseRate.ToString("#.##"),
                SalesPerTech = domain.SalesPerTech.ToString("#.##"),
                SalesPerTechPerMonth = domain.SalesPerTechPerMonth.ToString("#.##"),
                TechCount = domain.TechCount.ToString(),
                WebLeadCount = domain.WebLeadCount.ToString(),
                FranchiseeId = domain.FranchiseeId,
                LostEstimate = domain.LostEstimate.ToString("#.##"),
                LostJobs = domain.LostJobs.ToString("#.##"),
                MissedCalls = domain.MissedCalls.ToString("#.##"),
                TotalCalls = domain.TotalCalls.ToString(),
                TotalJobs = domain.TotalJobs.ToString(),
                Month = domain.Month.ToString(),

            };
        }
    }
}
