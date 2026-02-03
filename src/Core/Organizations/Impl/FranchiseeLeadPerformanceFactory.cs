using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.MarketingLead.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeLeadPerformanceFactory : IFranchiseeLeadPerformanceFactory
    {
        public readonly IClock _clock;

        public FranchiseeLeadPerformanceFactory(IClock clock)
        {
            _clock= clock;
        }
        public LeadPerformanceEditModel CreateEditModel(LeadPerformanceFranchiseeDetails leadPerformanceFranchiseeDetails)
        {
            var editModel = new LeadPerformanceEditModel()
            {
                PpcSpend = leadPerformanceFranchiseeDetails != null ? leadPerformanceFranchiseeDetails.CategoryId == (int)LeadPerformanceEnum.PPCSPEND ?
                 leadPerformanceFranchiseeDetails.Amount.ToString() : "" : "50",
                SeoCost = leadPerformanceFranchiseeDetails != null ? leadPerformanceFranchiseeDetails.CategoryId == (int)LeadPerformanceEnum.SEOCOST ?
                 leadPerformanceFranchiseeDetails.Amount.ToString() : "0" : "275",
                SeoCostBillingPeriodId= leadPerformanceFranchiseeDetails!=null? leadPerformanceFranchiseeDetails.week.GetValueOrDefault():1

            };

            return editModel;
        }

        public LeadPerformanceFranchiseeDetails CreateDomain(double amount, LeadPerformanceEnum leadPerformanceEnum, long? franchiseeId)
        {
            var domain = new LeadPerformanceFranchiseeDetails()
            {
                Amount = amount,
                CategoryId = (long)leadPerformanceEnum,
                DateTime = (DateTime.UtcNow),
                Month = DateTime.UtcNow.Date.Month,
                FranchiseeId = franchiseeId.GetValueOrDefault(),
                DataRecorderMetaData = new DataRecorderMetaData(DateTime.UtcNow),
                IsActive = true
               
            };
            return domain;
        }

        public LeadPerformanceFranchiseeViewModel CreateViewModel(LeadPerformanceFranchiseeDetails franchiseeTechMailService,OrganizationRoleUser organizationRoleUser)
        {
            var viewModel = new LeadPerformanceFranchiseeViewModel()
            {
                Id = franchiseeTechMailService.Id,
                Amount = franchiseeTechMailService.Amount,
                FranchiseeId = franchiseeTechMailService.FranchiseeId,
                Month = franchiseeTechMailService.Month,
                DateTime= franchiseeTechMailService.DateTime,
                LastUpdatedOn = franchiseeTechMailService.DataRecorderMetaData.DateModified != null ?
                franchiseeTechMailService.DataRecorderMetaData.DateModified : franchiseeTechMailService.DataRecorderMetaData.DateCreated,
                UserName= organizationRoleUser!=null? organizationRoleUser.Person.Name.FullName:"",
                IsActive=franchiseeTechMailService.IsActive,
                SeoBillingPeriod= franchiseeTechMailService.week
            };
            return viewModel;
        }
    }
}
