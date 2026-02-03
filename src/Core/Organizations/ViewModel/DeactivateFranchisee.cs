using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
   public class DeactivateFranchisee
    {
        public long FranchiseeId { get; set; }
        public string DeactivateNote { get; set; }
    }
}
