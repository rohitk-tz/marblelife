using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class MarketingLeadCallDetailV2 : DomainBase
    {
        public DateTime CallDate { get; set; }
        public string SetName { get; set; }
        public string PhoneLabel { get; set; }
        public string TransferNumber { get; set; }
        public string CallerId { get; set; }
        public string EnteredZipCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Reroute { get; set; }
        public string TalkMintues { get; set; }
        public string TalkSeconds { get; set; }
        public string TotalMintues { get; set; }
        public string TotalSeconds { get; set; }
        public string CallDuration { get; set; }
        public string Sid { get; set; }
        public string IvrResults { get; set; }
        public string Source { get; set; }
        public string SourceNumber { get; set; }
        public string Destination { get; set; }
        public string CallRoute { get; set; }
        public string CallStatus { get; set; }
        public string APPState { get; set; }
        public string RepeaSourceCaller { get; set; }
        public string SourceCap { get; set; }
        public string CallRouteQualified { get; set; }
        public string SourceQualified { get; set; }
        public string FindMeList { get; set; }
        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long? MarketingLeadCallDetailId { get; set; }
        [ForeignKey("MarketingLeadCallDetailId")]
        public virtual MarketingLeadCallDetail MarketingLeadCallDetail { get; set; }
    }
}
