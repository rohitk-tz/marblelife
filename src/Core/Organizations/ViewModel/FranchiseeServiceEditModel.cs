using Core.Application.Attribute;

namespace Core.Organizations.ViewModels
{
    [NoValidatorRequired]
    public class FranchiseeServiceEditModel
    {
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public long ServiceTypeId { get; set; }
        public bool CalculateRoyalty { get; set; }
        public long CategoryId { get; set; }
        public bool IsActive { get; set; }
        public bool IsCertified { get; set; }
    }
}
