using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
  public  class FranchiseeListModel
    {
        public string Status { get; set; }
        public long? Count { get; set; }
        public List<FranchiseeViewModel> Info { get; set; }
    }

    public class FranchiseeViewModel
    {
        public string FranchiseeName { get; set; }
        public string ZipCode { get; set; }
        public string AreaCodes { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string TransferableNumber { get; set; }
        public string FranchiseeEmail { get; set; }
        public string County { get; set; }
    }
}
