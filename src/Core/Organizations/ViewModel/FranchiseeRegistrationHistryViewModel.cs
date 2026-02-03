using Core.Application.Attribute;
using System;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeRegistrationHistryViewModel
    {
        public DateTime RegistrationDate { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
    }
}
