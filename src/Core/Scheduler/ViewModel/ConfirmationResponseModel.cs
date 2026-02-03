using Core.Application.Attribute;
using Core.Scheduler.Enum;
using System;
namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class ConfirmationResponseModel
    {
        public ConfirmationEnum ConfirmationEnum { get; set; }
        public string CustomerName { get; set; }
        public string StartDateTime { get; set; }
        public DateTime EndDateTIme { get; set; }
        public string SchedulerType { get; set; }

    }
}
