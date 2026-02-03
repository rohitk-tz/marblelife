using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class MailTemplateEditModel
    {
        public long? NotificationId { get; set; }
        public string Body { get; set; }
        public long? LanguageId { get; set; }
        public long? EmailTemplateId { get; set; }
        public string Subject { get; set; }
    }

    [NoValidatorRequired]
    public class InvoiceRequiredViewModel
    {
        public long? SchedulerId { get; set; }
        public bool IsInvoiceRequired { get; set; }
        public string InvoiceReason { get; set; }
    }
}
