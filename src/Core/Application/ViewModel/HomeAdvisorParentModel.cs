using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.ViewModel
{
    public class HomeAdvisorParentModel
    {
        public long? Id { get; set; }
        public string HAAccount { get; set; }

        public string FranchiseeName { get; set; }
        public string CompanyName { get; set; }
        public string SRID { get; set; }
        public DateTime SRSubmittedDate { get; set; }

        public string Task { get; set; }
        public decimal? NetLeadDollar { get; set; }
        public long? CityId { get; set; }
        public long? StateId { get; set; }
        public string ZipCode { get; set; }
        public string LeadType { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }

    }
}
