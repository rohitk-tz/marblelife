using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
   public class MarketingLeadCallDetailV4 : DomainBase
    {
        public long? MarketingLeadCallDetailId { get; set; }
        [ForeignKey("MarketingLeadCallDetailId")]
        public virtual MarketingLeadCallDetail MarketingLeadCallDetail { get; set; }
        public string Sid { get; set; }
        public string MissedCall_CallMetrics { get; set; }
        public string CallActivities { get; set; }
        public string Channel_Attribution { get; set; }
        public string Status_Attribution { get; set; }
        public string Rank_Attribution { get; set; }
        public string Pid_Attribution { get; set; }
        public string Bid_Attribution { get; set; }
        public string DocumentTitle_FirstTouch { get; set; }
        public string DocumentUrl_FirstTouch { get; set; }
        public string DocumentPath_FirstTouch { get; set; }
        public string DocumentTimeStamp_FirstTouch { get; set; }
        public string DocumentTitle_LastTouch { get; set; }
        public string DocumentUrl_LastTouch { get; set; }
        public string DocumentPath_LastTouch { get; set; }
        public string DocumentTimeStamp_LastTouch { get; set; }
        public string IPAddress_VisitorData { get; set; }
        public string Device_VisitorData { get; set; }
        public string Browser_VisitorData { get; set; }
        public string BrowserVersion_VisitorData { get; set; }
        public string Os_VisitorData { get; set; }
        public string OsVersion_VisitorData { get; set; }
        public string SearchTerm_VisitorData { get; set; }
        public string ActivityValue_VisitorData { get; set; }
        public string ActivityTypeId_VisitorData { get; set; }
        public string ActivityKeyword_VisitorData { get; set; }
        public string ActivityTag_VisitorData { get; set; }
        public string Campaign_VisitorData { get; set; }
        public string Platform_VisitorData { get; set; }
        public string SourceGuard_VisitorData { get; set; }
        public string VisitorLogUrl_VisitorData { get; set; }
        public string GoogleUaClientId_VisitorData { get; set; }
        public string GClid_VisitorData { get; set; }

    }
}
