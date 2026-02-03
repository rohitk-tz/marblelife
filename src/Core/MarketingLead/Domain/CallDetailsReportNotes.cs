using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class CallDetailsReportNotes : DomainBase
    {
        public string CallerId { get; set; }
        public string Notes { get; set; }
        public long? PreferredContactNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Office { get; set; }
        public string ZipCode { get; set; }
        public string ResultingAction { get; set; }
        public string HouseNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsActive { get; set; }
        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public string CreatedBy { get; set; }
        public string UserRole { get; set; }
        public long? MarketingLeadId { get; set; }
        [ForeignKey("MarketingLeadId")]
        public virtual MarketingLeadCallDetailV2 MarketingLeadCallDetailV2 { get; set; }

        public long? MarketingLeadIdFromCallDetailsReport { get; set; }
        [ForeignKey("MarketingLeadId")]
        public virtual MarketingLeadCallDetailV2 MarketingLeadCallDetail { get; set; }
        public DateTime? EditTimestamp { get; set; }
        public string EmailId { get; set; }
    }

    public class CallNotesHistoryViewModel
    {
        public long? Id { get; set; }
        public DateTime AddedOn { get; set; }
        public string CallerId { get; set; }
        public string AddedBy { get; set; }
        public string CallMatched { get; set; }
        public string Role { get; set; }
        public string Office { get; set; }
        public string ResultingAction { get; set; }
        public string Note { get; set; }
        public DateTime? DateAndTimeOfCall { get; set; }
        public string DialedNumber_dnis { get; set; }
        public string TransferToNumber { get; set; }
        public string PhoneLabel { get; set; }
        public decimal? RingSeconds { get; set; }
        public decimal? RingCount { get; set; }
        public string RecordedSeconds { get; set; }
        public long CallDuration { get; set; }
        public string CallRoute_MappedByZipCode { get; set; }
        public string FranchiseeFromInvocaAPI { get; set; }
        public string CallFlowEnteredZip { get; set; }
        public long? PreferredContactNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string ZipCode { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string TranscriptionStatus { get; set; }
        public string MissedCall { get; set; }
        public string FindMeList { get; set; }
    }
}