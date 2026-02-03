using Core.Application.ViewModel;
using Core.Notification.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class MailListModel
    {
        public IEnumerable<EmailViewModel> Collection { get; set; }
        public PagingModel PagingModel { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
