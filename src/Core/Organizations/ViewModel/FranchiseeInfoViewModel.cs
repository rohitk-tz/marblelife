namespace Core.Organizations.ViewModel
{
    public class FranchiseeInfoViewModel
    {
        public long OrganizationId { get; set; }
        public long OrganizationRoleUserId { get; set; }
        public long UserId { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public string FranchiseeName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public bool IsBold { get; set; }
    }
}
