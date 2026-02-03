using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class EmailViewModel
    {
        public long Id { get; set; }
        public bool isActive { get; set; }
        public long EmailTemplateId { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string FranchiseeName { get; set; }
        public long? FranchiseeId { get; set; }
        public DateTime? SendDate { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientEmailCc { get; set; }
        public string EnglishBody { get; set; }
        public string SpanishBody { get; set; }
        public bool IsTransPossible { get; set; }
        public bool IsSpanishPossible { get; set; }
        public bool IsPreviewAvailable { get; set; }
        public string Subjects { get; set; }
        public long NotificationId { get; set; }
    }

}
