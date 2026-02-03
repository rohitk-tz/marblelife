using Core.Application.Attribute;

namespace Core.Users.ViewModels
{
    [NoValidatorRequired]
    public class UserListFilter
    {
        public bool? IsActive { get; set; }
        public string Text { get; set; }
        public long FranchiseeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public long RoleId { get; set; }
        public string SortingColumn { get; set; }
        public long? SortingOrder { get; set; }
        public bool IsFrontOfficeExecutive { get; set; }
        public long? StatusId { get; set; }
    }
}
