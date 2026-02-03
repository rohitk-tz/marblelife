using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class MeetingDeleteViewModel
    {
        public long? Id { get; set; }
        public long? TechId { get; set; }
    }
}
