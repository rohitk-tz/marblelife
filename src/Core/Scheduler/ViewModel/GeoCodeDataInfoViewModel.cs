using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class GeoCodeDataInfoViewModel
    {
        public string GeoCode { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public long StateId { get; set; }
    }
}
