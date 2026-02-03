using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class DurationApprovalListModel
    {
        public List<DurationApprovalViewModel> Collection { get; set; }
    }
    [NoValidatorRequired]
    public class DurationApprovalViewModel
    {
        public long? Id { get; set; }
        public string UserName { get; set; }
        public DateTime AddDate { get; set; }
        public decimal? Duration { get; set; }
        public string FranchiseeName { get; set; }
        public long? StatusId { get; set; }
        public string ApprovedByUserName { get; set; }
        public DateTime? AddDateTime { get; set; }
    }
}
