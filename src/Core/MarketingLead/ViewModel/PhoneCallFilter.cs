using Core.Application.Attribute;
using System;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
   public class PhoneCallFilter
    {
        public long? FranchiseeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? PhoneCost { get; set; }
        public long? Id { get; set; }
        public long? UserId { get; set; }
        public DateTime? Date { get; set; }
        public CallDetailFilter Filter { get; set; }
        public string PropName { get; set; }
        public long? Order { get; set; }
    }


    [NoValidatorRequired]
    public class AutomationBackUpFilter
    {
        public long? FranchiseeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string PropName { get; set; }
        public long? Order { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

