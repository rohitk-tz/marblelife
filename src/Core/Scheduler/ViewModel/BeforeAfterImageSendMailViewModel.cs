using Core.Application.Attribute;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class BeforeAfterImageSendMailViewModel
    {
        public string MailTo { get; set; }
        public JobEstimateServiceViewModel BeforeImages { get; set; }
        public JobEstimateServiceViewModel AfterImages { get; set; }
    }
}
