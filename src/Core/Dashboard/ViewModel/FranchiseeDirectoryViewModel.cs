using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dashboard.ViewModel
{
    public class FranchiseeDirectoryViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<object> PhoneNumbers { get; set; }
    }
}
