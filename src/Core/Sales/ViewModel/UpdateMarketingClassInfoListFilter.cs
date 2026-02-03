using Core.Application.Attribute;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class UpdateMarketingClassInfoListFilter
    {
        public int Year { get; set; }
        public long? UserId { get; set; }
        public long RoleId { get; set; }
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public bool IsFromDownload { get; set; }
    }

}
