using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.Scheduler.ViewModel
{
    public class SalesTaxRatesViewModel
    {
        [XmlElement("City")]
        public string City { get; set; }
        [XmlElement("State")]
        public string State { get; set; }
        [XmlElement("Country")]
        public string Country { get; set; }
        [XmlElement("ZipCode")]
        public string ZipCode { get; set; }
        [XmlElement("CombinedDistrictRate")]
        public double? CombinedDistrictRate { get; set; }
        [XmlElement("CombinedRate")]
        public double? CombinedRate { get; set; }
        [XmlElement("CountryRate")]
        public double? CountryRate { get; set; }
        [XmlElement("DistanceSalesThreshold")]
        public double? DistanceSalesThreshold { get; set; }
        [XmlElement("FreightTaxable")]
        public double FreightTaxable { get; set; }
        [XmlElement("FranchiseeId")]
        public long? FranchiseeId { get; set; }
        [XmlElement("ParkingRate")]
        public double? ParkingRate { get; set; }
        [XmlElement("ReducedRate")]
        public double? ReducedRate { get; set; }
        [XmlElement("StandardRate")]
        public double? StandardRate { get; set; }
        [XmlElement("StateRate")]
        public double? StateRate { get; set; }
        [XmlElement("SuperReducedRate")]
        public double? SuperReducedRate { get; set; }

    }
}
