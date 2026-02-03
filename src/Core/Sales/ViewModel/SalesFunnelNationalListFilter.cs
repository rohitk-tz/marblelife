using Core.Application.Attribute;
using System;
using System.Collections.Generic;


namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class SalesFunnelNationalListFilter
    {
        public DateTime? PeriodStartDate { get; set; }
        public DateTime? PeriodEndDate { get; set; }
        public long? FranchiseeId { get; set; }
        public long? Order { get; set; }
        public string PropName { get; set; }
        public long? RoleId { get; set; }
        public long? UserId { get; set; }
    }
}
