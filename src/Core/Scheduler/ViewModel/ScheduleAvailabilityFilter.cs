using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired] 
    public class ScheduleAvailabilityFilter
    {
        public long JobId { get; set; }
        public long AssigneeId { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [NoValidatorRequired]
    public class ScheduleAvailabilityFilterList
    {
        public ScheduleAvailabilityFilterList()
        {
            ScheduleAvailabilityFilter = new List<ScheduleAvailabilityFilter>();
        }

        public List<ScheduleAvailabilityFilter> ScheduleAvailabilityFilter { get; set; }
    }

    public class ScheduleAvailabilityFilterViewModel
    {
        public bool IsAvailable { get; set; }
        public string AssigneeNames { get; set; }
    }


}
