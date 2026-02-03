using Core.Application.Attribute;
using Core.Billing.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class CallDetailNotesFilter
    {
        public long? FranchiseeId { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CallerId { get; set; }
        public string CountryId { get; set; }
        public long? TagId { get; set; }
        public string Text { get; set; }
        public long? UserId { get; set; }
        public string ZipCode { get; set; }
        public string Office { get; set; }
        public string ResultingAction { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Sort Sort { get; set; }
    }
}
