using Core.Application.Attribute;


namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
   public class JobNoteEditModel
    {
        public string JobNote { get; set; }
        public long? Id { get; set; }

        public bool IsJob { get; set; }
    }
}
