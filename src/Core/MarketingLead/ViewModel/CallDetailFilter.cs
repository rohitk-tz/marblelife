using Core.Application.Attribute;
using Core.Billing.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class CallDetailFilter
    {
        public long? FranchiseeId { get; set; }
        public long CallTypeId { get; set; }
        public string Text { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? ConvertedLead { get; set; }
        public int? MappedFranchisee { get; set; }
        public long? TagId { get; set; }
        public string CategoryIds { get; set; }
        public string CallerId { get; set; }
        public long MarketingLeadId { get; set; }
        public string TransferToNumber { get; set; }
        public string Office { get; set; }
        //public string FranchiseeName { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Sort Sort { get; set; }

        public List<string> DownloadColumnList { get; set; }
    }


}
