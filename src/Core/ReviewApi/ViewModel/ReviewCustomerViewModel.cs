using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
   public class ReviewCustomerViewModel
    {
        public string FranchiseName { get; set; }
        public string FranchiseWebsite { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmailAddress { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public long? Rp_Id { get; set; }
        public bool IsFromQA { get; set; }
        public long? ReviewId { get; set; }
    }
}
