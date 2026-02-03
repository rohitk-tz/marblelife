using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    [NoValidatorRequired]
    public class PhoneCallInvoiceEditModel
    {
        public long? Id { get; set; }
        public long? UserId { get; set; }
    }
}
