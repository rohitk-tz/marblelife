using Core.Geo.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class ZipCode : DomainBase
    {
        public string Zip { get; set; }
        public long? CountyId { get; set; }
        public string AreaCode { get; set; }
        public string Direction { get; set; }
        public bool? DriveTest { get; set; }
        public string Code { get; set; }

        [ForeignKey("CountyId")]
        public virtual County County { get; set; }

        public string Dir { get; set; }
        public string CityName { get; set; }
        public string CountyName { get; set; }

        public long? CityId { get; set; }
        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        public string StateCode { get; set; }

        public string TransferableNumber { get; set; }
    }
}
