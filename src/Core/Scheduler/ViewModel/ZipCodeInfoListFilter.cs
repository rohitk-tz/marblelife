using Core.Application.Attribute;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class ZipCodeInfoListFilter
    {
        public string ZipCode { get; set; }
        public long? CountryId { get; set; }
        public long? UserId { get; set; }
        public long RoleId { get; set; }
    }
}
