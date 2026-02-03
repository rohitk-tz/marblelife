using Core.Application.Attribute;
using System;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class DocumentListFilter
    {
        public string Text { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string IsImportant { get; set; }
        public long FranchiseeId { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public bool ShowToUser { get; set; }
        public long? CategoryId { get; set; }
        public long? UserId { get; set; }
        public bool isSaleTech { get; set; }
        public long? loggedinUser { get; set; }
        public long? DocumentTypeId { get;set; }
    }
}
