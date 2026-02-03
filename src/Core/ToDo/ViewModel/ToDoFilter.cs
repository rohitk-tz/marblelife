using Core.Application.Attribute;
using System;

namespace Core.ToDo.ViewModel
{
    [NoValidatorRequired]
    public  class ToDoFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long? FranchiseeId { get; set; }
        public long? OrgUserId { get; set; }
        public long? UserId { get; set; }
        public long? RoleId { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public bool? IsFranchiseeLevel { get; set; }
        public long? LoggedInFranchiseeId { get; set; }
        public long? StatusId { get; set; }
    }
}
