using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
    public class NewJobEstimateModel
    {
        public string FullName { get; set; }
        public string FranchiseeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TechName { get; set; }
        public string CustomerAddress { get; set; }
        public string TechImageUrl { get; set; }
        public string Email { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine1 { get; set; }
        public string Eamail { get; set; }
        public long OrganizationId { get; set; }
        public long? FileId { get; set; }
        public string PhoneNumber { get; set; }
        public string FileName { get; set; }
        public string jobTitle { get; set; }
        public string jobType { get; set; }

    }
}
