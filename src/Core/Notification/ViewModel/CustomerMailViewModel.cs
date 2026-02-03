using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Notification.ViewModel
{
   public class CustomerMailViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string TechName { get; set; }
        public long? Id { get; set; }
    }
}
