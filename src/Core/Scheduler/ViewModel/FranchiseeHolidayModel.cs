
using Core.Application.Attribute;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
  public  class FranchiseeHolidayModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? FranchiseeId { get; set; }
    }
}
