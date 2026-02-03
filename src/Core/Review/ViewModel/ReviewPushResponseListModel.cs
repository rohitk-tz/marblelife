using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Review.ViewModel
{
   public class ReviewPushResponseListModel
    {
        public string result { get; set; }
        public ICollection<ReviewPushResponseViewModel> info { get; set; }
    }
}
