using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
   public class MediaModel
    {
        public long RowId { get; set; }
        public long? EstimateId { get; set; }
        public long MediaType { get; set; }
    }
}
