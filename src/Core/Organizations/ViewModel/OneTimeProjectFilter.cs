using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class OneTimeProjectFilter
    {
        public long FranchiseeId { get; set; }

        public bool IsInRoyality { get; set; }
        public bool IsseoInRoyality { get; set; }
    }
}
