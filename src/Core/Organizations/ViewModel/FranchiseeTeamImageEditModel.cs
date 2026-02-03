using Core.Application.Attribute;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
   public class FranchiseeTeamImageEditModel
    {
        public FileUploadModel FileUploadModel { get; set; }
        public long? FranchiseeId { get; set; }
    }
}
