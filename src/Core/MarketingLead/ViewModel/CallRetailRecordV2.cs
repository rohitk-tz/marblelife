using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    public class CallRetailRecordV2
    {

        public CallRetailRecordV2()
        {
            call_details = new CallRetailRecordV3();
            Attribution = new Attribution();
        }

        [XmlElement("sid")]
        public string sid { get; set; }

        [XmlElement("acct_id")]
        public string acct_id { get; set; }
        [XmlElement("call_details")]
        public CallRetailRecordV3 call_details { get; set; }

        [XmlElement("attribution")]
        public Attribution Attribution { get; set; }

        [XmlElement("call_analytics")]
        public CallAnalytics call_analytics { get; set; }
    }


    public class AdvancedRouting
    {
        [XmlElement("leadflow")]
        public LeadFlow leadflow { get; set; }
        [XmlElement("callflow")]
        public Callflow callflow { get; set; }
    }
    public class ReverseLookUp
    {
        [XmlElement("first_name")]
        public string first_name { get; set; }
        [XmlElement("last_name")]
        public string last_name { get; set; }
        [XmlElement("current_address")]
        public CurrentAddress current_address { get; set; }
        [XmlElement("geo_lookup_attempt")]
        public string geo_lookup_attempt { get; set; }
        [XmlElement("geo_lookup_result")]
        public string geo_lookup_result { get; set; }
    }

    public class LeadFlow
    {
        [XmlElement("leadflow_set_name")]
        public string leadflow_set_name { get; set; }
        [XmlElement("leadflow_set_id")]
        public string leadflow_set_id { get; set; }
        [XmlElement("leadflow_advertiser")]
        public string leadflow_advertiser { get; set; }
        [XmlElement("leadflow_advertiser_id")]
        public string leadflow_advertiser_id { get; set; }
        [XmlElement("leadflow_affiliate")]
        public string leadflow_affiliate { get; set; }
        [XmlElement("leadflow_affiliate_id")]
        public string leadflow_affiliate_id { get; set; }
        [XmlElement("leadflow_affiliate_price")]
        public string leadflow_affiliate_price { get; set; }
        [XmlElement("leadflow_affiliate_qualified")]
        public string leadflow_affiliate_qualified { get; set; }
        [XmlElement("leadflow_repeat_affiliate_caller")]
        public string leadflow_repeat_affiliate_caller { get; set; }
        [XmlElement("leadflow_offer")]
        public string leadflow_offer { get; set; }
        [XmlElement("leadflow_offer_id")]
        public string leadflow_offer_id { get; set; }
        [XmlElement("leadflow_offer_price")]
        public string leadflow_offer_price { get; set; }
        [XmlElement("leadflow_offer_qualified")]
        public string leadflow_offer_qualified { get; set; }
        [XmlElement("leadflow_state")]
        public string leadflow_state { get; set; }
        [XmlElement("leadflow_entered_zip")]
        public string leadflow_entered_zip { get; set; }
        [XmlElement("leadflow_reroute")]
        public string leadflow_reroute { get; set; }
    }

    public class Callflow
    {
        [XmlElement("callflow_set_name")]
        public string callflow_set_name { get; set; }
        [XmlElement("callflow_set_id")]
        public string callflow_set_id { get; set; }
        [XmlElement("callflow_destination")]
        public string callflow_destination { get; set; }
        [XmlElement("callflow_destination_id")]
        public string callflow_destination_id { get; set; }
        [XmlElement("callflow_source")]
        public string callflow_source { get; set; }
        [XmlElement("callflow_source_id")]
        public string callflow_source_id { get; set; }
        [XmlElement("callflow_source_qualified")]
        public string callflow_source_qualified { get; set; }
        [XmlElement("callflow_repeat_source_caller")]
        public string callflow_repeat_source_caller { get; set; }
        [XmlElement("callflow_source_cap")]
        public string callflow_source_cap { get; set; }
        [XmlElement("callflow_call_route")]
        public string callflow_call_route { get; set; }
        [XmlElement("callflow_call_route_id")]
        public string callflow_call_route_id { get; set; }
        [XmlElement("callflow_call_route_qualified")]
        public string callflow_call_route_qualified { get; set; }
        [XmlElement("callflow_state")]
        public string callflow_state { get; set; }
        [XmlElement("callflow_entered_zip")]
        public string callflow_entered_zip { get; set; }
        [XmlElement("callflow_reroute")]
        public string callflow_reroute { get; set; }
    }

    public class Attribution
    {
        [XmlElement("channel")]
        public string Channel { get; set; }
        [XmlElement("status")]
        public string Status { get; set; }
        [XmlElement("rank")]
        public string Rank { get; set; }
        [XmlElement("pid")]
        public string Pid { get; set; }
        [XmlElement("bid")]
        public string Bid { get; set; }
        [XmlElement("first_touch")]
        public FirstTouch first_touch { get; set; }
        [XmlElement("last_touch")]
        public LastTouch last_touch { get; set; }
        [XmlElement("visitor_data")]
        public VisitorData visitor_data { get; set; }

        [XmlElement("valuetrack_parameters")]
        public ValueTrackParameters valuetrack_parameters { get; set; }
        [XmlElement("source_iq_data")]
        public SourceIqData source_iq_data { get; set; }

        [XmlElement("paid_search")]
        public PaidSearch paid_search { get; set; }

    }
}
