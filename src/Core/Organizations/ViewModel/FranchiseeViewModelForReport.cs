using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeViewModelForReport
    {
        public string FranchiseeName { get; set; }
        public long? FranchiseeId { get; set; }
    }
}
