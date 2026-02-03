using Core.Application.Attribute;
using System;
using System.ComponentModel;

namespace Core.MarketingLead.ViewModel
{
    public class CallDetailViewModel
    {
        [DisplayName("Id")]

        public long Id { get; set; }
        [DisplayName("Data From New API")]
        public string DataFromNewAPI { get; set; }

        [DisplayName("Data From Invoca")]
        public string DataFromInvoca { get; set; }
        [DisplayName("Date/Time Of Call")]
        public DateTime DateOfCall { get; set; }
        [DisplayName("Caller ID (ani)")]
        public string Ani { get; set; }
        [DisplayName("Dialed Number (dnis)")]
        public string Dnis { get; set; }
        [DisplayName("Transfer To Number")]
        public string TransferToNumber { get; set; }
        [DisplayName("Call Transfer Type")]
        public string TransferType { get; set; }
        [DisplayName("Phone Label")]
        public string PhoneLabel { get; set; }
        [DisplayName("Call Type")]
        public string CallType { get; set; }
        [DisplayName("Call Duration(in min.)")]
        public int CallDuration { get; set; }
        [DisplayName("Valid Call")]
        public bool ValidCall { get; set; }
        [DisplayName("Franchisee(Invoca API)")]
        public string Franchisee { get; set; }

        [DisplayName("Called Franchisee")]
        public string CalledFranchiseeName { get; set; }
        [DisplayName("Tag")]
        public string Tag { get; set; }
        [DisplayName("Invoice Id")]
        public string InvoiceId { get; set; }
        [DisplayName("Call Type Id")]
        public long CallTypeId { get; set; }
        [DisplayName("Find Me List")]
        public string FindMeList { get; set; }
        [DownloadField(Required = false)]
        public string ClickDescription { get; set; }

        /// <summary>
        ///  New API 3
        /// </summary>
        [DisplayName("Call Flow Set Name")]
        public string CallFlowSetName { get; set; }
        [DisplayName("Call Flow Set Id")]
        public string CallFlowSetId { get; set; }
        [DisplayName("Call Flow Destination")]
        public string CallFlowDestination { get; set; }
        [DisplayName("Call Flow Destination Id")]
        public string CallFlowDestinationId { get; set; }
        [DisplayName("Call Flow Source")]
        public string CallFlowSource { get; set; }
        [DisplayName("Call Flow Source Id")]
        public string CallFlowSourceId { get; set; }
        [DisplayName("Call Flow Source Qualified")]
        public string CallFlowSourceQualified { get; set; }
        [DisplayName("Call Flow Repeat Source Caller")]
        public string CallFlowRepeatSourceCaller { get; set; }
        [DisplayName("Call Flow Source Cap")]
        public string CallFlowSourceCap { get; set; }
        [DisplayName("Call Flow Source Route")]
        public string CallFlowSourceRoute { get; set; }
        [DisplayName("Call Flow Source Route Id")]
        public string CallFlowSourceRouteId { get; set; }
        [DisplayName("Call Flow Source Route Qualified Id")]
        public string CallFlowSourceRouteQualified { get; set; }
        [DisplayName("Call Flow State")]
        public string CallFlowState { get; set; }
        [DisplayName("Call Route(Mapped By ZipCode)")]
        public string CallRoute { get; set; }
        [DisplayName("Call Flow Entered Zip")]
        public string CallFlowEnteredZip { get; set; }
        [DisplayName("Call Flow Reroute")]
        public string CallFlowReroute { get; set; }
        [DisplayName("Transfer To Numbers")]
        public string TransferToNumber_CallFlow { get; set; }
        [DisplayName("Transfer Type")]
        public string TransferType_CallFlow { get; set; }
        [DisplayName("Call Transfer Status")]
        public string CallTransferStatus_CallFlow { get; set; }
        [DisplayName("Ring Seconds")]
        public decimal? RingSeconds_CallFlow { get; set; }
        [DisplayName("Ring Count")]
        public decimal? RingCount_CallFlow { get; set; }
        [DisplayName("Keyword Groups")]
        public string KeywordGroups_CallAnalytics { get; set; }
        [DisplayName("Keyword Spotting Complete")]
        public string KeywordSpottingComplete_CallAnalytics { get; set; }
        [DisplayName("Transcription Status")]
        public string TranscriptionStatus_CallAnalytics { get; set; }
        [DisplayName("Call Note")]
        public string CallNote_CallAnalytics { get; set; }
        [DisplayName("Recording Url")]
        public string RecordingUrl_Recording { get; set; }
        [DisplayName("Recorded Seconds")]
        public string RecordedSeconds_Recording { get; set; }
        [DisplayName("Dialog Analytics")]
        public string DialogAnalytics_Recording { get; set; }
        [DisplayName("First Name")]
        public string FirstName_ReverseLookUp { get; set; }
        [DisplayName("Last Name")]
        public string LastName_ReverseLookUp { get; set; }
        [DisplayName("Street Line 1")]
        public string StreetLine1_ReverseLookUp { get; set; }
        [DisplayName("City")]
        public string City_ReverseLookUp { get; set; }
        [DisplayName("Postal Area")]
        public string PostalArea_ReverseLookUp { get; set; }
        [DisplayName("State Code")]
        public string StateCode_ReverseLookUp { get; set; }
        [DisplayName("Geo Lookup Attempt")]
        public string GeoLookupAttempt_ReverseLookUp { get; set; }
        [DisplayName("Geo Lookup Result")]
        public string GeoLookupResult_ReverseLookUp { get; set; }

        //////
        ///New API 4
        ///
        [DisplayName("Missed Call")]
        public string MissedCall_CallMetrics { get; set; }
        [DisplayName("Call Activities")]
        public string CallActivities { get; set; }
        [DisplayName("Channel")]
        public string Channel_Attribution { get; set; }
        [DisplayName("Status")]
        public string Status_Attribution { get; set; }
        [DisplayName("Rank")]
        public string Rank_Attribution { get; set; }
        [DisplayName("PID")]
        public string Pid_Attribution { get; set; }
        [DisplayName("BID")]
        public string Bid_Attribution { get; set; }
        [DisplayName("First Touch Document Title")]
        public string DocumentTitle_FirstTouch { get; set; }
        [DisplayName("First Touch Document URL")]
        public string DocumentUrl_FirstTouch { get; set; }
        [DisplayName("First Touch Document Path")]
        public string DocumentPath_FirstTouch { get; set; }
        [DisplayName("First Touch Document Time Stamp")]
        public string DocumentTimeStamp_FirstTouch { get; set; }
        [DisplayName("Last Touch Document Title")]
        public string DocumentTitle_LastTouch { get; set; }
        [DisplayName("Last Touch Document Url")]
        public string DocumentUrl_LastTouch { get; set; }
        [DisplayName("Last Touch Document Path")]
        public string DocumentPath_LastTouch { get; set; }
        [DisplayName("Last Touch Document Time Stamp")]
        public string DocumentTimeStamp_LastTouch { get; set; }
        [DisplayName("IP Address")]
        public string IPAddress_VisitorData { get; set; }
        [DisplayName("Device")]
        public string Device_VisitorData { get; set; }
        [DisplayName("Browser")]
        public string Browser_VisitorData { get; set; }
        [DisplayName("Browser Version")]
        public string BrowserVersion_VisitorData { get; set; }
        [DisplayName("OS")]
        public string Os_VisitorData { get; set; }
        [DisplayName("OS Version")]
        public string OsVersion_VisitorData { get; set; }
        [DisplayName("Search Term")]
        public string SearchTerm_VisitorData { get; set; }
        [DisplayName("Activity Value")]
        public string ActivityValue_VisitorData { get; set; }
        [DisplayName("Activity Type Id")]
        public string ActivityTypeId_VisitorData { get; set; }
        [DisplayName("Activity Keyword")]
        public string ActivityKeyword_VisitorData { get; set; }
        [DisplayName("Activity Tag")]
        public string ActivityTag_VisitorData { get; set; }
        [DisplayName("Platform")]
        public string Platform_VisitorData { get; set; }
        [DisplayName("Source Guard")]
        public string SourceGuard_VisitorData { get; set; }
        [DisplayName("Visitor Log Url")]
        public string VisitorLogUrl_VisitorData { get; set; }
        [DisplayName("Google Ua Client Id")]
        public string GoogleUaClientId_VisitorData { get; set; }
        [DisplayName("GCl Id")]
        public string GClid_VisitorData { get; set; }



        //////
        ///New API 5
        ///

        [DisplayName("Utm Source")]
        public string UtmSource_DefaultUtmParameters { get; set; }
        [DisplayName("Utm Medium")]
        public string UtmMedium_DefaultUtmParameters { get; set; }
        [DisplayName("Utm Campaign")]
        public string UtmCampaign_DefaultUtmParameters { get; set; }
        [DisplayName("Utm Term")]
        public string UtmTerm_DefaultUtmParameters { get; set; }
        [DisplayName("Utm Content")]
        public string UtmContent_DefaultUtmParameters { get; set; }
        [DisplayName("Vt Keyword")]
        public string VtKeyword_ValueTrackParameters { get; set; }
        [DisplayName("Vt Match Type")]
        public string VtMatchType_ValueTrackParameters { get; set; }
        [DisplayName("Vt Network")]
        public string VtNetwork_ValueTrackParameters { get; set; }
        [DisplayName("Vt Device")]
        public string VtDevice_ValueTrackParameters { get; set; }
        [DisplayName("Vt Device Model")]
        public string VtDeviceModel_ValueTrackParameters { get; set; }
        [DisplayName("Vt Creative")]
        public string VtCreative_ValueTrackParameters { get; set; }
        [DisplayName("Vt Placement")]
        public string VtPlacement_ValueTrackParameters { get; set; }
        [DisplayName("Vt Target")]
        public string VtTarget_ValueTrackParameters { get; set; }
        [DisplayName("Vt Param 1")]
        public string VtParam1_ValueTrackParameters { get; set; }
        [DisplayName("Vt Param 2")]
        public string VtParam2_ValueTrackParameters { get; set; }
        [DisplayName("Vt Random")]
        public string VtRandom_ValueTrackParameters { get; set; }
        [DisplayName("Vt Ace Id")]
        public string VtAceid_ValueTrackParameters { get; set; }
        [DisplayName("VtAd Position")]
        public string VtAdposition_ValueTrackParameters { get; set; }
        [DisplayName("Vt Product Target Id")]
        public string VtProductTargetId_ValueTrackParameters { get; set; }
        [DisplayName("VtAd Type")]
        public string VtAdType_ValueTrackParameters { get; set; }
        [DisplayName("Domain Set Name")]
        public string DomainSetName_SourceIqData { get; set; }
        [DisplayName("Domain Set Id")]
        public string DomainSetId_SourceIqData { get; set; }
        [DisplayName("Pool Name")]
        public string PoolName_SourceIqData { get; set; }
        [DisplayName("Location Name")]
        public string LocationName_SourceIqData { get; set; }
        [DisplayName("Custom Value")]
        public string CustomValue_SourceIqData { get; set; }
        [DisplayName("Custom Id")]
        public string CustomId_SourceIqData { get; set; }
        [DisplayName("Campaign")]
        public string Campaign_VisitorData { get; set; }
        [DisplayName("Campaign Id")]
        public string CampaignId_PaidSearch { get; set; }
        [DisplayName("Ad Group")]
        public string Adgroup_PaidSearch { get; set; }
        [DisplayName("Ad Group Id")]
        public string AdgroupId_PaidSearch { get; set; }
        [DisplayName("Ads")]
        public string Ads_PaidSearch { get; set; }
        [DisplayName("Ad Id")]
        public string AdId_PaidSearch { get; set; }
        [DisplayName("Key Words")]
        public string Keywords_PaidSearch { get; set; }
        [DisplayName("Keyword Id")]
        public string KeywordId_PaidSearch { get; set; }
        [DisplayName("Click Id")]
        public string ClickId_PaidSearch { get; set; }
        [DisplayName("KeyWord Match Type")]
        public string KeyWordMatchType_PaidSearch { get; set; }
        [DisplayName("CallInly Flag")]
        public string CallInlyFlag_PaidSearch { get; set; }
        [DisplayName("Type")]
        public string Type_PaidSearch { get; set; }
        [DisplayName("Call Note")]
        public string CallNote { get; set; }
        [DisplayName("Preferred Contact Number")]
        public long? PreferredContactNumber { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Company")]
        public string Company { get; set; }
        [DisplayName("Office")]
        public string Office { get; set; }
        [DisplayName("Zip Code")]
        public string ZipCode { get; set; }
        [DisplayName("Resulting Action")]
        public string ResultingAction { get; set; }
        [DisplayName("Number")]
        public string HouseNumber { get; set; }
        [DisplayName("Street")]
        public string Street { get; set; }
        [DisplayName("City")]
        public string City { get; set; }
        [DisplayName("State")]
        public string State { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("Home Owner")]
        public string HomeOwner { get; set; }
        [DisplayName("Home Market")]
        public string HomeMarket { get; set; }
    }
}
