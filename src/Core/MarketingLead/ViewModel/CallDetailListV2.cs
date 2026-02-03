using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Core.MarketingLead.ViewModel
{
    public class CallDetailListV2
    {
        public CallDetailV2 record;

    }

    public class CallDetailV2
    {
        public DateTime call_date { get; set; }
        public string set_name { get; set; }
        public string phone_label { get; set; }
        public string transfer_number { get; set; }
        public string caller_id { get; set; }
        public string entered_zip { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string street_address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zipcode { get; set; }
        public string reroute { get; set; }
        public string talk_minutes { get; set; }
        public string talk_seconds { get; set; }
        public string total_minutes { get; set; }
        public string total_seconds { get; set; }
        public string call_duration { get; set; }
        public string recording_link { get; set; }
        public string sid { get; set; }
        public string ivr_results { get; set; }
        public string source { get; set; }
        public string source_number { get; set; }
        public string destination { get; set; }
        public string call_route { get; set; }
        public string call_status { get; set; }
        public string APP_state { get; set; }
        public string repeat_source_caller { get; set; }
        public string source_cap { get; set; }
        public string call_route_qualified { get; set; }
        public string source_qualified { get; set; }

    }
}
