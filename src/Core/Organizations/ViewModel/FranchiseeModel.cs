using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
  public  class FranchiseeModel
    {
        public string CategoryName { get; set; }
        public List<FranchiseeRedesignViewModel> FranchiseeViewModel { get; set; }
    }
}
