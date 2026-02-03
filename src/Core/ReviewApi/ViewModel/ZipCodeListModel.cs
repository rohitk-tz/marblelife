using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
  public  class ZipCodeListModel
    {
        public string Status { get; set; }
        public List<ZipCodeViewModel> Info { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class ZipCodeViewModel
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
        public long? FranchiseeId { get; set; }
    }
}
