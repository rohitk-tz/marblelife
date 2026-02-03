using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class PhoneCallEditModel
    {
        public long? Id { get; set; }
        public long? CallCount { get; set; }
        public long? ChargesForPhone { get; set; }
        public long? UserId { get; set; }
        public long? FranchiseeId { get; set; }
        public DateTime DateOfChange { get; set; }
    }
}
