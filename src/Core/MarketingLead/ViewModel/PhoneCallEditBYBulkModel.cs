using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class PhoneCallEditByBulkModel
    {
        public long? Id { get; set; }
        public long? CallCount { get; set; }
        public long? ChargesForPhone { get; set; }
        public long? UserId { get; set; }
        public DateTime DateOfChange { get; set; }
        public bool? IsGenerateInvoice { get; set; }

        public List<long?> PhoneIdList { get; set; }
        public long? FranchiseeId { get; set; }
    }

    [NoValidatorRequired]
    public class PhoneCallEditByBulkList
    {
        public long? FranchiseeId { get; set; }
        public long? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<PhoneCallByBulkModel> Collection { get; set; }
    }

    [NoValidatorRequired]
    public class PhoneCallByBulkModel
    {
        public long Id { get; set; }
        public long CallCount { get; set; }
        public long ChargesForPhone { get; set; }
        public long? UserId { get; set; }
        public DateTime DateOfChange { get; set; }
        public bool? IsInvoiceGenerated { get; set; }
        public long? FranchiseeId { get; set; }
        public long? PhonechargesfeeId { get; set; }
    }
}
