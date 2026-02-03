using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.Domain
{
  public class AnnualRoyality : DomainBase
    {
        public long FranchiseeId { get; set; }
        public DateTime Date { get; set; }
        public decimal? Royality { get; set; }
        public decimal? Sales { get; set; }
        public bool? isMinRoyalityReached { get; set; }
        public decimal? MonthlyRoyality { get; set; }
        public decimal? Payment { get; set; }
    }
}
