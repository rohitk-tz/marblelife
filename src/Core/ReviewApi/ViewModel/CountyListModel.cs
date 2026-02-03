using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
    public  class CountyListModel
    {
        public string Status { get; set; }
        public List<CountyViewModel> Info { get; set; }
    }

    public class CountyViewModel
    {
        public string TaazaaFranchisee { get; set; }
        public string TaazaaFranchiseeEmail { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string TransferableNumber { get; set; }
        public string County { get; set; }
    }
}
