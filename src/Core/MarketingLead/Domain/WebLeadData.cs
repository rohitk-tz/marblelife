using Core.Application.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class WebLeadData : DomainBase
    {
        public long? FranchiseeId { get; set; }

        public string Url { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int Count { get; set; }

        public bool IsWeekly { get; set; }

        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    }
}
