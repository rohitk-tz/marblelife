using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
  public  class ZipCreateEditModel
    {
        public long Id { get; set; }
       // public long IsUpdated { get; set; }
        public string ZipCode { get; set; }
        public string countyName { get; set; }
        public long? countyId { get; set; }
        public string AreaCode { get; set; }
        public string Direction { get; set; }
        public string DriveTest { get; set; }
        public string Code { get; set; }
        public string StateCode { get; set; }
        public string CityName { get; set; }
        public string Dir { get; set; }
        public long? CityId { get; set; }
        public long IsDeleted { get; set; }
        public string FranchiseeTransferableNumber { get; set; }
    }
}
