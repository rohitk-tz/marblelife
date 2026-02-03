using Core.Application.Attribute;
using System;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class LeadPerformanceFranchiseeFilter
    {
        public long? FranchiseeId { get; set; }
        public long? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? OrganizationRoleUserId { get; set; }
    }
}
