using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class DebuggerLogModel
    {
        public long? FranchiseeId { get; set; }
        public long? ActionId { get; set; }
        public string Description { get; set; }
        public long? UserId { get; set; }
        public long? PageId { get; set; }
        public long? JobEstimateimageCategoryId { get; set; }
        public long? JobEstimateServiceCategoryId { get; set; }
        public long Id { get; set; }
        public long? TypeId { get; set; }
        public long? JobSchedulerId { get; set; }
    }
}
