using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
    public class ReviewCustomerListModel
    {
        public string Status { get; set; }
        public List<ReviewCustomerViewModel> Info { get; set; }
    }
}
