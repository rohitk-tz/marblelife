using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    public class SelectedScheduler
    {
        public long? JobId { get; set; }
        public string Description { get; set; }
        public long? EstimateId { get; set; }
        public string BeforeImageUrl { get; set; }
        public string AfterImageUrl { get; set; }
    }
}
