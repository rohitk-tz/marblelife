using Core.Application.Attribute;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class ConfirmationModel
    {
        public string EncryptedData { get; set; }
        public long? SchedulerId { get; set; }
        public bool Status { get; set; }
    }
}
