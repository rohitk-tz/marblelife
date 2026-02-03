using Core.Application.Attribute;
using System;
using System.ComponentModel;

namespace Core.Organizations.ViewModel
{
    [NoValidatorRequired]
    public class FranchiseeViewModel : OrganizationViewModel
    {
        [DisplayName("Owner Name")]
        public string OwnerName { get; set; }
        [DisplayName("Quick Book Identifier")]
        public string QuickBookIdentifier { get; set; }
        [DisplayName("Sales Report Status")]
        public string SalesReportStatus { get; set; }
        [DisplayName("Currency")]
        public string Currency { get; set; }
        [DisplayName("Text")]
        public string Text { get; set; }
        [DisplayName("Notes Created On")]
        public DateTime? NotesCreatedOn { get; set; }
        [DisplayName("Account Credit")]
        public decimal? AccountCredit { get; set; }
        [DisplayName("Business Id")]
        public long? BusinessId { get; set; }
        [DisplayName("Deactivation Note")]
        public string DeactivationNote { get; set; }

        public long? FranchiseeDurationCount { get; set; }
        public decimal? Duration { get; set; }
        public string NoteFromCallCenter { get; set; }
        public string NoteFromOwner { get; set; }
    }
}
