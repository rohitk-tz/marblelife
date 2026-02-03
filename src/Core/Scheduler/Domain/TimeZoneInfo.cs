using Core.Geo.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class TimeZoneInformation : DomainBase
    {
        public string TimeZone { get; set; }
        public decimal TimeDifference { get; set; }
        public bool IsActive { get; set; }

        public long CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}
