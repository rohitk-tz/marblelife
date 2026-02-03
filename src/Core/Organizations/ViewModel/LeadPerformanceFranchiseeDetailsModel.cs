using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
   public class LeadPerformanceFranchiseeDetailsModel
    {
        public long FranchiseeId { get; set; }

        public double Amount { get; set; }
        public int Month { get; set; }
        public DateTime DateTime { get; set; }
        public long? CategoryId { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        public long MultiplicationFactor { get; set; }
    }
}
