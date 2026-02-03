using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ReviewApi.ViewModel
{
   public class CountryListModel
    {
        public string Status { get; set; }
        public long? Count { get; set; }
        public List<CountryViewModel> Info { get; set; }
    }

    public class CountryViewModel
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
