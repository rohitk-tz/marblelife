using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class MarketingLeadCallDetailV5 : DomainBase
    {
        public long? MarketingLeadCallDetailId { get; set; }
        [ForeignKey("MarketingLeadCallDetailId")]
        public virtual MarketingLeadCallDetail MarketingLeadCallDetail { get; set; }
        public string Sid { get; set; }
        public string UtmSource_DefaultUtmParameters { get; set; }
        public string UtmMedium_DefaultUtmParameters { get; set; }
        public string UtmCampaign_DefaultUtmParameters { get; set; }
        public string UtmTerm_DefaultUtmParameters { get; set; }
        public string UtmContent_DefaultUtmParameters { get; set; }
        public string VtKeyword_ValueTrackParameters { get; set; }
        public string VtMatchType_ValueTrackParameters { get; set; }
        public string VtNetwork_ValueTrackParameters { get; set; }
        public string VtDevice_ValueTrackParameters { get; set; }
        public string VtDeviceModel_ValueTrackParameters { get; set; }
        public string VtCreative_ValueTrackParameters { get; set; }
        public string VtPlacement_ValueTrackParameters { get; set; }
        public string VtTarget_ValueTrackParameters { get; set; }
        public string VtParam1_ValueTrackParameters { get; set; }
        public string VtParam2_ValueTrackParameters { get; set; }
        public string VtRandom_ValueTrackParameters { get; set; }
        public string VtAceid_ValueTrackParameters { get; set; }
        public string VtAdposition_ValueTrackParameters { get; set; }
        public string VtProductTargetId_ValueTrackParameters { get; set; }
        public string VtAdType_ValueTrackParameters { get; set; }
        public string DomainSetName_SourceIqData { get; set; }
        public string DomainSetId_SourceIqData { get; set; }
        public string PoolName_SourceIqData { get; set; }
        public string LocationName_SourceIqData { get; set; }
        public string CustomValue_SourceIqData { get; set; }
        public string CustomId_SourceIqData { get; set; }
        public string Campaign_PaidSearch { get; set; }
        public string CampaignId_PaidSearch { get; set; }
        public string Adgroup_PaidSearch { get; set; }
        public string AdgroupId_PaidSearch { get; set; }
        public string Ads_PaidSearch { get; set; }
        public string AdId_PaidSearch { get; set; }
        public string Keywords_PaidSearch { get; set; }
        public string KeywordId_PaidSearch { get; set; }
        public string ClickId_PaidSearch { get; set; }
        public string KeyWordMatchType_PaidSearch { get; set; }
        public string CallInlyFlag_PaidSearch { get; set; }
        public string Type_PaidSearch { get; set; }

    }
}
