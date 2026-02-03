using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class CustomerInfoModel
    {
        public string CustomerName { get; set; }
        public long? FranchiseeId { get; set; }
        public string CountryName { get; set; }
        public long? CountryId { get; set; }
        public long? StateId { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public long? CityId { get; set; }
        public string City { get; set; }
    }
}
