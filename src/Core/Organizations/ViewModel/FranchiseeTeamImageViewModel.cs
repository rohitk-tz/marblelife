using Core.Application.Attribute;
using System;
using System.Collections.Generic;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeTeamImageViewModel
    {
        public string FranchiseeTeamImage { get; set; }
        public long? FileId { get; set; }
        public string ImageName { get; set; }
        public string FranchiseeName { get; set; }
    }
}
