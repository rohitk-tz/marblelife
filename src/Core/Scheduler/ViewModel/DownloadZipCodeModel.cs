using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class DownloadZipCodeModel
    {
        [DisplayName("Id")]
        public long id { get; set; }
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; }
        [DisplayName("Area Code")]
        public string AreaCode { get; set; }
        [DisplayName("DIR")]
        public string Direction { get; set; }
       
        [DisplayName("CODE")]
        public string Code { get; set; }
        [DisplayName("County /  Municipality")]
        public string County { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("State / Prov")]
        public string StateCode { get; set; }

        //[DisplayName("Franchisee Transferable Number")]
        //public string FranchiseeTransferableNumber { get; set; }
        //[DisplayName("Franchisee Name")]
        //public string FranchiseeName { get; set; }


        //[DisplayName("DIRVE DIST")]
        //public bool DriveTest { get; set; }

        [DisplayName("isDeleted")]
        public int IsDeleted { get; set; }

    }
}
