using Core.Application.Attribute;
using System;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class TopLeadersFilter
    {
        public string TypeIds { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long FranchiseeId { get; set; }
        public long? LoggedInFranchiseeId { get; set; } 
    }
}
