using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class ZipDataInfoViewModel
    {
        public string FranchiseeName { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string AreaCode { get; set; }
        public string DirectionCode { get; set; }
        public long? FranchiseeId { get; set; }
        public bool CanSchedule { get; set; }
        public string zipcode { get; set; }
        public string County { get; set; }
        public string EmailId { get; set; }

        public decimal? Duration { get; set; }
        public string NotesFromCallCenter { get; set; }
        public string NotesFromOwner { get; set; }
        public string ContractedTeritory { get; set; }
        public decimal? Duratiton { get; set; }
        public List<ServiceViewModel> ServicesList { get; set; }
    }

    public class ServiceViewModel
    {
        public string ServiceName { get; set; }
        public bool IsActive { get; set; }
        public bool IsCertified { get; set; }
    }
   
}


