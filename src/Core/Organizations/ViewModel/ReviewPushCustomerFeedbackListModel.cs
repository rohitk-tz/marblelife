using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
  public  class ReviewPushCustomerFeedbackListModel
    {
        public string result { get; set; }
        public List<ReviewPushCustomerFeedbackViewModel> info { get; set; }
    }


    public class ReviewPushCustomerGoogleFeedbackListModel
    {
        public bool issuccess { get; set; }
       
        public List<ReviewPushCustomerGoogleFeedbackViewModel> results { get; set; }
    }
}
