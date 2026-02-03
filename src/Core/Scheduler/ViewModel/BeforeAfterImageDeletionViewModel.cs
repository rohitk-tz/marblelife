using Core.Application.Attribute;
using Core.Application.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class BeforeAfterImageDeletionViewModel
    {
       public  DateTime UploadDateTime { get; set; }
       public long? UserId { get; set; }
        public long? RoleId { get; set; }
        public long? LoggedInUserId { get; set; }
    }
}
