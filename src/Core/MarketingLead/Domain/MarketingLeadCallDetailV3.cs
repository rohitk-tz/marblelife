using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class MarketingLeadCallDetailV3 : DomainBase
    {
        public long? MarketingLeadCallDetailId { get; set; }
        [ForeignKey("MarketingLeadCallDetailId")]
        public virtual MarketingLeadCallDetail MarketingLeadCallDetail { get; set; }

        public string Sid { get; set; }
        public string CallflowSetName { get; set; }
        public string CallflowSetId { get; set; }
        public string CallflowDestination { get; set; }
        public string CallflowDestinationId { get; set; }
        public string CallflowSource { get; set; }
        public string CallflowSourceId { get; set; }
        public string CallflowSourceQualified { get; set; }
        public string CallflowRepeatSourceCaller { get; set; }
        public string CallflowSourceCap { get; set; }
        public string CallflowSourceRoute { get; set; }
        public string CallflowSourceRouteId { get; set; }
        public string CallflowSourceRouteQualified { get; set; }
        public string CallflowState { get; set; }
        public string CallflowEnteredZip { get; set; }
        public string CallflowReroute { get; set; }
        public string TransferToNumber { get; set; }
        public string TransferToNumber_CallFlow { get; set; }
        public string TransferType_CallFlow { get; set; }
        public string CallTransferStatus_CallFlow { get; set; }
        public decimal? RingSeconds_CallFlow { get; set; }
        public decimal? RingCount_CallFlow { get; set; }
        public string KeywordGroups_CallAnalytics { get; set; }
        public string KeywordSpottingComplete_CallAnalytics { get; set; }
        public string TranscriptionStatus_CallAnalytics { get; set; }
        public string CallNote_CallAnalytics { get; set; }
        public string RecordingUrl_Recording { get; set; }
        public string RecordedSeconds_Recording { get; set; }
        public string DialogAnalytics_Recording { get; set; }
        public string FirstName_ReverseLookUp { get; set; }
        public string LastName_ReverseLookUp { get; set; }
        public string StreetLine1_ReverseLookUp { get; set; }
        public string City_ReverseLookUp { get; set; }
        public string PostalArea_ReverseLookUp { get; set; }
        public string StateCode_ReverseLookUp { get; set; }
        public string GeoLookupAttempt_ReverseLookUp { get; set; }
        public string GeoLookupResult_ReverseLookUp { get; set; }
        public string home_owner_status_data_append { get; set; }
        public string home_market_value_data_append { get; set; }
    }

}
