using Core.Application.Attribute;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeNotesDurationViewModel
    {
        public long TypeId { get; set; }
        public long UserId { get; set; }
        public string Description { get; set; }
        public long FranchiseeId { get; set; }
        public decimal? Duration { get; set; }
        public long RoleId { get; set; }
    }
    [NoValidatorRequired]
    public class DurationApprovalFilterModel
    {
        public long StatusId { get; set; }
        public long Id { get; set; }
        public long UserId { get; set; }

    }
}
