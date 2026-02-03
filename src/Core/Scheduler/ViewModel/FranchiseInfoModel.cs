using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
   public class FranchiseInfoModel
    {
        public long? FranchiseeId { get; set; }
        public string CountryName { get; set; }
        public long? CountryId { get; set; }
        public long? StateId { get; set; }
        public string State { get; set; }
    }
}
