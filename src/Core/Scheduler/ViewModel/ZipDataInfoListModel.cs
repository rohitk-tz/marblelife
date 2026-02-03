using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class ZipDataInfoListModel
    {
        public IEnumerable<ZipDataInfoViewModel> Collection { get; set; }
       public bool? canSchedule { get; set; }
    }
}
