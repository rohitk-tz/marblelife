using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class BeforeAfterFileModel
    {
        public long? FileId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
