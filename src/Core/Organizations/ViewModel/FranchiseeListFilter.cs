using Core.Application.Attribute;

namespace Core.Organizations.ViewModels
{
    [NoValidatorRequired]
    public class FranchiseeListFilter
    {
        public string Text { get; set; }
        public long? FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public string Email { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public long? FranchiseeStatus { get; set; }
        public bool? status { get; set; }
        public long? RoleId { get; set; }
        public long? LoggedInRoleId { get; set; }
        public long? LoggedInUserId { get; set; }
    }
}
