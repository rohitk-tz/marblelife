using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class ManageFranchiseeAccountModel
    {
        public long FranchiseeId { get; set; }
        public long UserId { get; set; }
        public long CurrentFranchiseeId { get; set; }
    }
}
