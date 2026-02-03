using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    public class InvocaCallDetails
    {
        [XmlElement("advertiser_id_from_network")]
        public string advertiser_id_from_network { get; set; }

        [XmlElement("complete_call_id")]
        public string complete_call_id { get; set; }

        [XmlElement("start_time_local")]
        public string start_time_local { get; set; }

        [XmlElement("start_time_xml")]
        public string start_time_xml { get; set; }

        [XmlElement("mobile")]
        public string mobile { get; set; }

        [XmlElement("duration")]
        public decimal duration { get; set; }

        [XmlElement("call_source_description")]
        public string call_source_description { get; set; }

        [XmlElement("calling_phone_number")]
        public string calling_phone_number { get; set; }

        [XmlElement("promo_line_description")]
        public string promo_line_description { get; set; }

        [XmlElement("destination_phone_number")]
        public string destination_phone_number { get; set; }

        [XmlElement("transfer_from_type")]
        public string transfer_from_type { get; set; }

        [XmlElement("call_result_description_detail")]
        public string call_result_description_detail { get; set; }

        [XmlElement("connect_duration")]
        public decimal connect_duration { get; set; }

        [XmlElement("first_name_data_append")]
        public string first_name_data_append { get; set; }

        [XmlElement("last_name_data_append")]
        public string last_name_data_append { get; set; }

        [XmlElement("address_full_street_data_append")]
        public string address_full_street_data_append { get; set; }

        [XmlElement("address_city_data_append")]
        public string address_city_data_append { get; set; }

        [XmlElement("address_zip_data_append")]
        public string address_zip_data_append { get; set; }

        [XmlElement("address_state_data_append")]
        public string address_state_data_append { get; set; }

        [XmlElement("signal_name")]
        public string signal_name { get; set; }

        [XmlElement("advertiser_campaign_name")]
        public string advertiser_campaign_name { get; set; }

        [XmlElement("advertiser_campaign_id_from_network")]
        public string advertiser_campaign_id_from_network { get; set; }

        [XmlElement("advertiser_name")]
        public string advertiser_name { get; set; }

        //[XmlElement("advertiser_id_from_network")]
        //public string advertiser_id_from_network { get; set; }

        [XmlElement("affiliate_name")]
        public string affiliate_name { get; set; }

        [XmlElement("affiliate_id_from_network")]
        public string affiliate_id_from_network { get; set; }

        [XmlElement("affiliate_payout_localized")]
        public string affiliate_payout_localized { get; set; }

        [XmlElement("affiliate_call_volume_ranking")]
        public string affiliate_call_volume_ranking { get; set; }

        [XmlElement("repeat_calling_phone_number")]
        public string repeat_calling_phone_number { get; set; }

        //[XmlElement("advertiser_campaign_name")]
        //public string advertiser_campaign_name { get; set; }

        //[XmlElement("advertiser_campaign_id_from_network")]
        //public string advertiser_campaign_id_from_network { get; set; }

        [XmlElement("matching_affiliate_payout_policies")]
        public string matching_affiliate_payout_policies { get; set; }

        //[XmlElement("call_result_description_detail")]
        //public string call_result_description_detail { get; set; }

        [XmlElement("region")]
        public string region { get; set; }

        //[XmlElement("address_zip_data_append")]
        //public string address_zip_data_append { get; set; }

        //[XmlElement("repeat_calling_phone_number")]
        //public string repeat_calling_phone_number { get; set; }

        //[XmlElement("advertiser_campaign_name")]
        //public string advertiser_campaign_name { get; set; }

        //[XmlElement("advertiser_campaign_id_from_network")]
        //public string advertiser_campaign_id_from_network { get; set; }

        //[XmlElement("advertiser_name")]
        //public string advertiser_name { get; set; }

        //[XmlElement("advertiser_id_from_network")]
        //public string advertiser_id_from_network { get; set; }

        //[XmlElement("affiliate_name")]
        //public string affiliate_name { get; set; }

        //[XmlElement("affiliate_id_from_network")]
        //public string affiliate_id_from_network { get; set; }

        //[XmlElement("affiliate_payout_localized")]
        //public string affiliate_payout_localized { get; set; }

        //[XmlElement("repeat_calling_phone_number")]
        //public string repeat_calling_phone_number { get; set; }

        //[XmlElement("callflow_source_cap")]
        //public string callflow_source_cap { get; set; }

        //[XmlElement("callflow_call_route")]
        //public string callflow_call_route { get; set; }

        //[XmlElement("callflow_call_route_id")]
        //public string callflow_call_route_id { get; set; }

        //[XmlElement("callflow_call_route_qualified")]
        //public string callflow_call_route_qualified { get; set; }

        //[XmlElement("address_state_data_append")]
        //public string address_state_data_append { get; set; }

        //[XmlElement("address_zip_data_append")]
        //public string address_zip_data_append { get; set; }

        //[XmlElement("repeat_calling_phone_number")]
        //public string repeat_calling_phone_number { get; set; }

        //[XmlElement("promo_line_description")]
        //public string promo_line_description { get; set; }

        //[XmlElement("status")]
        //public string Status { get; set; }

        //[XmlElement("rank")]
        //public string Rank { get; set; }

        //[XmlElement("pid")]
        //public string Pid { get; set; }

        [XmlElement("invoca_id")]
        public string invoca_id { get; set; }

        [XmlElement("landing_title")]
        public string landing_title { get; set; }
        [XmlElement("landing_page")]
        public string landing_page { get; set; }
        [XmlElement("calling_path")]
        public string calling_path { get; set; }

        //[XmlElement("document_timestamp")]
        //public string document_timestamp { get; set; }

        [XmlElement("calling_page")]
        public string calling_page { get; set; }

        //[XmlElement("ip_address")]
        //public string ip_address { get; set; }

        //[XmlElement("device")]
        //public string Device { get; set; }

        //[XmlElement("browser")]
        //public string Browser { get; set; }

        //[XmlElement("browser_version")]
        //public string browser_version { get; set; }

        //[XmlElement("os")]
        //public string Os { get; set; }

        //[XmlElement("os_version")]
        //public string os_version { get; set; }

        //[XmlElement("search_term")]
        //public string search_term { get; set; }

        [XmlElement("dynamic_number_pool_referrer_search_type")]
        public string dynamic_number_pool_referrer_search_type { get; set; }

        //[XmlElement("promo_line_description")]
        //public string promo_line_description { get; set; }

        [XmlElement("dynamic_number_pool_id")]
        public string dynamic_number_pool_id { get; set; }

        //[XmlElement("activity_keyword")]
        //public string activity_keyword { get; set; }

        //[XmlElement("activity_tag")]
        //public string activity_tag { get; set; }

        [XmlElement("dynamic_number_pool_referrer_referrer_campaign")]
        public string dynamic_number_pool_referrer_referrer_campaign { get; set; }

        //[XmlElement("platform")]
        //public string platform { get; set; }

        [XmlElement("media_type")]
        public string media_type { get; set; }

        //[XmlElement("visitor_log_url")]
        //public string visitor_log_url { get; set; }

        [XmlElement("g_cid")]
        public string g_cid { get; set; }

        //[XmlElement("gclid")]
        //public string gclid { get; set; }

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

        //[XmlElement("vt_keyword")]
        //public string vt_keyword { get; set; }
        //[XmlElement("vt_matchtype")]
        //public string vt_matchtype { get; set; }
        //[XmlElement("vt_network")]
        //public string vt_network { get; set; }
        //[XmlElement("vt_device")]
        //public string vt_device { get; set; }
        //[XmlElement("vt_devicemodel")]
        //public string vt_devicemodel { get; set; }
        //[XmlElement("vt_creative")]
        //public string vt_creative { get; set; }
        //[XmlElement("vt_placement")]
        //public string vt_placement { get; set; }
        //[XmlElement("vt_target")]
        //public string vt_target { get; set; }
        //[XmlElement("vt_param1")]
        //public string vt_param1 { get; set; }
        //[XmlElement("vt_param2")]
        //public string vt_param2 { get; set; }
        //[XmlElement("vt_random")]
        //public string vt_random { get; set; }
        //[XmlElement("vt_aceid")]
        //public string vt_aceid { get; set; }
        //[XmlElement("vt_adposition")]
        //public string vt_adposition { get; set; }
        //[XmlElement("vt_producttargetid")]
        //public string vt_producttargetid { get; set; }
        //[XmlElement("vt_adtype")]
        //public string vt_adtype { get; set; }

        //[XmlElement("domain_set_name")]
        //public string domain_set_name { get; set; }
        //[XmlElement("domain_set_id")]
        //public string domain_set_id { get; set; }

        //[XmlElement("promo_line_description")]
        //public string promo_line_description { get; set; }
        //[XmlElement("location_name")]
        //public string location_name { get; set; }
        //[XmlElement("custom_value")]
        //public string custom_value { get; set; }
        //[XmlElement("custom_id")]
        //public string custom_id { get; set; }

        //[XmlElement("dynamic_number_pool_referrer_referrer_campaign")]
        //public string dynamic_number_pool_referrer_referrer_campaign { get; set; }

        [XmlElement("dynamic_number_pool_referrer_referrer_campaign_id")]
        public string dynamic_number_pool_referrer_referrer_campaign_id { get; set; }

        [XmlElement("dynamic_number_pool_referrer_ad_group")]
        public string dynamic_number_pool_referrer_ad_group { get; set; }

        //[XmlElement("dynamic_number_pool_referrer_ad_group")]
        //public string dynamic_number_pool_referrer_ad_group { get; set; }

        //[XmlElement("ads")]
        //public string Ads { get; set; }

        [XmlElement("dynamic_number_pool_referrer_ad_id")]
        public string dynamic_number_pool_referrer_ad_id { get; set; }

        [XmlElement("dynamic_number_pool_referrer_search_keywords")]
        public string dynamic_number_pool_referrer_search_keywords { get; set; }

        [XmlElement("dynamic_number_pool_referrer_search_keywords_id")]
        public string dynamic_number_pool_referrer_search_keywords_id { get; set; }
        
        [XmlElement("dynamic_number_pool_referrer_keyword_match_type")]
        public string dynamic_number_pool_referrer_keyword_match_type { get; set; }




        [XmlElement("gclid")]
        public string gclid { get; set; }

        //[XmlElement("call_only_flag")]
        //public string call_only_flag { get; set; }

        //[XmlElement("type")]
        //public string Type { get; set; }

        //[XmlElement("keyword_groups")]
        //public List<string> keyword_groups { get; set; }

        //[XmlElement("keyword_spotting_complete")]
        //public string keyword_spotting_complete { get; set; }
        //[XmlElement("transcription_status")]
        //public string transcription_status { get; set; }
        [XmlElement("notes")]
        public string notes { get; set; }
        [XmlElement("recording")]
        public string recording { get; set; }
        //[XmlElement("recorded_seconds")]
        //public int recorded_seconds { get; set; }
        //[XmlElement("dialog_analytics")]
        //public List<string> dialog_analytics { get; set; }

        [XmlElement("hangup_cause")]
        public string hangup_cause { get; set; }

        [XmlElement("Answered By Agent")]
        public bool Answered_By_Agent {get; set;}

        [XmlElement("caller_zip")]
        public string caller_zip { get; set; }

        [XmlElement("verified_zip")]
        public string verified_zip { get; set; }
        [XmlElement("office")]
        public string office { get; set; }

        [XmlElement("find_me_list")]
        public string find_me_list { get; set; }

        public string SessionId { get; set; }
        [XmlElement("home_owner_status_data_append")]
        public string home_owner_status_data_append { get; set; }
        [XmlElement("home_market_value_data_append")]
        public string home_market_value_data_append { get; set; }
    }
}
