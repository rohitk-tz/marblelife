using Core.Geo.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Domain
{
    public class SalesTaxRates : DomainBase
    {
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public decimal? CombinedDistrictRate { get; set; }
        public decimal? CombinedRate { get; set; }
        public decimal? CountryRate { get; set; }
        public decimal? DistanceSalesThreshold { get; set; }
        public bool? FreightTaxable { get; set; }
        public long? FranchiseeId { get; set; }
        public decimal? ParkingRate { get; set; }
        public decimal? ReducedRate { get; set; }
        public decimal? StandardRate { get; set; }
        public decimal? StateRate { get; set; }
        public decimal? SuperReducedRate { get; set; }

        public long? CityId { get; set; }
        public long? StateId { get; set; }
        public long? CountryId { get; set; }
        public long? ZipId { get; set; }

        [ForeignKey("CityId")]
        public virtual City Cities { get; set; }
        [ForeignKey("StateId")]
        public virtual State States { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Countries { get; set; }
        [ForeignKey("ZipId")]
        public virtual Zip Zips { get; set; }
    }
}
