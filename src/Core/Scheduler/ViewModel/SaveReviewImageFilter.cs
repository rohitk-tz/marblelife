using Core.Application.Attribute;
using System;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
   public class SaveReviewImageFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsReview { get; set; }
    }
}
