using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
   public class FranchiseeViewModelForFranchiseeDirectoryDownload
    {
        [DisplayName("Id")]
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumbers { get; set; }


        [DisplayName("Primary Contact")]
        public string OwnerName { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
    }
}
