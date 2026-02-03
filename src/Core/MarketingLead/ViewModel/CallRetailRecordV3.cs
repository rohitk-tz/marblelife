using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    [XmlRoot("call_details")]
    public class CallRetailRecordV3
    {

        [XmlElement("start_time")]
        public string start_time { get; set; }
        [XmlElement("sid")]
        public string SessionId { get; set; }
        [XmlElement("call_type")]
        public string call_type { get; set; }
        [XmlElement("click_description")]
        public string click_description { get; set; }
        [XmlElement("dialed_number")]
        public string dialed_number { get; set; }

        [XmlElement("call_duration_rounded_minutes")]
        public decimal call_duration_rounded_minutes { get; set; }
        [XmlElement("transfer_type")]
        public string transfer_type { get; set; }
        [XmlElement("transfer_to_number")]
        public string transfer_to_number { get; set; }
        [XmlElement("phone_label")]
        public string phone_label { get; set; }
        [XmlElement("caller_id")]
        public string caller_id { get; set; }

        [XmlElement("advanced_routing")]
        public AdvancedRouting advanced_routing { get; set; }
        [XmlElement("call_transfer")]
        public CallTransfer call_transfer { get; set; }

        [XmlElement("call_analytics")]
        public CallAnalytics call_analytics { get; set; }

        [XmlElement("reverse_lookup")]
        public List<ReverseLookUp> reverse_lookup { get; set; }
        [XmlElement("call_metrics")]
        public CallMetrics call_metrics { get; set; }
        
        [XmlElement("call_activities")]
        public List<string> call_activities { get; set; }

    }

    public class CallMetrics
    {
        [XmlElement("missed_call")]
        public string MissedCall { get; set; }
    }
    public class CallAnlytics
    {
        [XmlElement("transcription_status")]
        public string transcription_status { get; set; }
    }

    public class CallTransfer
    {
        [XmlElement("transfer_to_number")]
        public string transfer_to_number { get; set; }
        [XmlElement("transfer_type")]
        public string transfer_type { get; set; }
        [XmlElement("call_transfer_status")]
        public string call_transfer_status { get; set; }
        [XmlElement("ring_seconds")]
        public decimal ring_seconds { get; set; }
        [XmlElement("ring_count")]
        public decimal ring_count { get; set; }

    }


    public class CurrentAddress
    {
        [XmlElement("street_line_1")]
        public string street_line_1 { get; set; }
        [XmlElement("city")]
        public string city { get; set; }
        [XmlElement("postal_code")]
        public string postal_code { get; set; }
        [XmlElement("state_code")]
        public string state_code { get; set; }
    }

    public class ValueTrackParameters
    {
        [XmlElement("vt_keyword")]
        public string vt_keyword { get; set; }
        [XmlElement("vt_matchtype")]
        public string vt_matchtype { get; set; }
        [XmlElement("vt_network")]
        public string vt_network { get; set; }
        [XmlElement("vt_device")]
        public string vt_device { get; set; }
        [XmlElement("vt_devicemodel")]
        public string vt_devicemodel { get; set; }
        [XmlElement("vt_creative")]
        public string vt_creative { get; set; }
        [XmlElement("vt_placement")]
        public string vt_placement { get; set; }
        [XmlElement("vt_target")]
        public string vt_target { get; set; }
        [XmlElement("vt_param1")]
        public string vt_param1 { get; set; }
        [XmlElement("vt_param2")]
        public string vt_param2 { get; set; }
        [XmlElement("vt_random")]
        public string vt_random { get; set; }
        [XmlElement("vt_aceid")]
        public string vt_aceid { get; set; }
        [XmlElement("vt_adposition")]
        public string vt_adposition { get; set; }
        [XmlElement("vt_producttargetid")]
        public string vt_producttargetid { get; set; }
        [XmlElement("vt_adtype")]
        public string vt_adtype { get; set; }
    }

    public class SourceIqData
    {
        [XmlElement("domain_set_name")]
        public string domain_set_name { get; set; }
        [XmlElement("domain_set_id")]
        public string domain_set_id { get; set; }
        [XmlElement("pool_name")]
        public string pool_name { get; set; }
        [XmlElement("location_name")]
        public string location_name { get; set; }
        [XmlElement("custom_value")]
        public string custom_value { get; set; }
        [XmlElement("custom_id")]
        public string custom_id { get; set; }
    }


    public class PaidSearch
    {
        [XmlElement("campaign")]
        public string Campaign { get; set; }
        [XmlElement("campaign_id")]
        public string campaign_id { get; set; }
        [XmlElement("adgroup")]
        public string Adgroup { get; set; }
        [XmlElement("adgroup_id")]
        public string adgroup_id { get; set; }
        //[XmlElement("ads")]
        //public string Ads { get; set; }
        [XmlElement("ad_id")]
        public string ad_id { get; set; }
        //[XmlElement("keywords")]
        //public List<string> Keywords { get; set; }
        [XmlElement("keyword_id")]
        public string keyword_id { get; set; }
        [XmlElement("click_id")]
        public string click_id { get; set; }
        //[XmlElement("keyword_match_type")]
        //public  List<string>keyword_match_type { get; set; }
        [XmlElement("call_only_flag")]
        public string call_only_flag { get; set; }
        [XmlElement("type")]
        public string Type { get; set; }

    }

    public class CallAnalytics
    {
        [XmlElement("keyword_groups")]
        public List<string> keyword_groups { get; set; }

        [XmlElement("keyword_spotting_complete")]
        public string keyword_spotting_complete { get; set; }
        [XmlElement("transcription_status")]
        public string transcription_status { get; set; }
        [XmlElement("call_note")]
        public string call_note { get; set; }
        [XmlElement("recording")]
        public Recording recording { get; set; }
        [XmlElement("dialog_analytics")]
        public List<string> dialog_analytics { get; set; }
    }

    public class Recording
    {
        [XmlElement("recording_url")]
        public string recording_url { get; set; }
        [XmlElement("recorded_seconds")]
        public int recorded_seconds { get; set; }
    }

    public class FirstTouch
    {
        [XmlElement("document_title")]
        public string document_title { get; set; }
        [XmlElement("document_url")]
        public string document_url { get; set; }
        [XmlElement("document_path")]
        public string document_path { get; set; }
        [XmlElement("document_timestamp")]
        public string document_timestamp { get; set; }
    }

    public class LastTouch
    {
        [XmlElement("document_title")]
        public string document_title { get; set; }
        [XmlElement("document_url")]
        public string document_url { get; set; }
        [XmlElement("document_path")]
        public string document_path { get; set; }
        [XmlElement("document_timestamp")]
        public string document_timestamp { get; set; }
    }

    public class VisitorData
    {
        [XmlElement("ip_address")]
        public string ip_address { get; set; }
        [XmlElement("device")]
        public string Device { get; set; }
        [XmlElement("browser")]
        public string Browser { get; set; }
        [XmlElement("browser_version")]
        public string browser_version { get; set; }
        [XmlElement("os")]
        public string Os { get; set; }
        [XmlElement("os_version")]
        public string os_version { get; set; }
        [XmlElement("search_term")]
        public string search_term { get; set; }
        [XmlElement("referrer")]
        public string Referrer { get; set; }
        [XmlElement("activity_value")]
        public string activity_value { get; set; }
        [XmlElement("activity_type_id")]
        public string activity_type_id { get; set; }
        [XmlElement("activity_keyword")]
        public string activity_keyword { get; set; }
        [XmlElement("activity_tag")]
        public string activity_tag { get; set; }
        [XmlElement("campaign")]
        public string campaign { get; set; }
        [XmlElement("platform")]
        public string platform { get; set; }
        [XmlElement("sourceguard")]
        public string sourceguard { get; set; }
        [XmlElement("visitor_log_url")]
        public string visitor_log_url { get; set; }
        [XmlElement("google_ua_client_id")]
        public string google_ua_client_id { get; set; }
        [XmlElement("gclid")]
        public string gclid { get; set; }

        [XmlElement("default_utm_parameters")]
        public DefaultUtmParameters default_utm_parameters { get; set; }
    }

    public class DefaultUtmParameters
    {
        [XmlElement("utm_source")]
        public string utm_source { get; set; }
        [XmlElement("utm_medium")]
        public string utm_medium { get; set; }
        [XmlElement("utm_campaign")]
        public string utm_campaign { get; set; }
        [XmlElement("utm_term")]
        public string utm_term { get; set; }
        [XmlElement("utm_content")]
        public string utm_content { get; set; }
    }
}
