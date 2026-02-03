using Core.Application.Attribute;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class MailListFilter
    {
        public long? Id { get; set; }
        public bool isActive { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long? Order { get; set; }
        public string PropName { get; set; }
        public long? FranchiseeId { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string Text { get; set; }
        public string EmailFrom { get; set; }
        public string EmailTo { get; set; }
        public string EmailCc { get; set; }
    }
}
