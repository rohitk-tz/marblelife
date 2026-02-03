
using Core.Application.Attribute;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class SaveImagesBestPairFilter
    {
        public long? BeforeServiceId { get; set; }
        public long? AfterServiceId { get; set; }
        public long? BeforeAfterBestPairType { get; set; }
    }
}
