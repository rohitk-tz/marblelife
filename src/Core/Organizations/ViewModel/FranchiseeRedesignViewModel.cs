using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Organizations.ViewModel
{
   public class FranchiseeRedesignViewModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string About { get; set; }
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
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("OfficePhone")]
        public string OFFICEPhone { get; set; }
        [DisplayName("CellPhone")]
        public string CellPhone { get; set; }
        [DisplayName("CallCenterPhone")]
        public string CallCenterPhone { get; set; }
        [DisplayName("BusinessPhone")]
        public string BusinessPhone { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("State")]
        public string State { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("ZipCode")]
        public string ZipCode { get; set; }

        public bool IsActive { get; set; }
        public long CountryId { get; set; }

        public string NameWithout0Prefix { get; set; }
        public string FirstAlphaOfState { get; set; }
        public decimal? Duration { get; set; }
        public string NotesFromCallCenter { get; set; }
        public string NotesFromOwner { get; set; }
        public bool IsAccessible { get; set; }
        public long DurationApprovalCount { get; set; }
    }
}
