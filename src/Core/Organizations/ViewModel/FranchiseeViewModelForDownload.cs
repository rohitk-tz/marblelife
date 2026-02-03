using Core.Application.Attribute;
using Core.Geo.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeViewModelForDownload
    {
        [DisplayName("Id")]
        public long Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNumbers { get; set; }

        
        [DisplayName("Primary Contact")]
        public string OwnerName { get; set; }
        [DisplayName("Note")]
        public string Text { get; set; }
        [DisplayName("Notes Created On")]
        public DateTime? NotesCreatedOn { get; set; }
        [DisplayName("Account Credit(in $)")]
        public decimal? AccountCredit { get; set; }
        [DisplayName("Business Id")]
        public long? BusinessId { get; set; }
        [DisplayName("Deactivation Note")]
        public string DeactivationNote { get; set; }
    }
}
