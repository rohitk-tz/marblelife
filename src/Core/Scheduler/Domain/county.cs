using Core.Geo.Domain;
using Core.Organizations.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Scheduler.Domain
{
    public class County: DomainBase 
    {
        public string CountyName { get; set; }
        public long? CountryId { get; set; }
        public string StateCode { get; set; }
        public string FranchiseeName { get; set;
        }
        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public string TerritoryCode { get; set; }
        public string DirectionCode { get; set; }
        public string DirectionFromOffice { get; set; }
        public string ReachingTime { get; set; }
        public decimal? Population { get; set; }
        public string Status { get; set; }
        public string FranchiseMLD { get; set; }
        public string ContractedTerritory { get; set; }
        public string CoveringLessThan3Hours { get; set; }
        public string DayTrip { get; set; }
        public string UnCovered { get; set; }
        public string StateCountryCode { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}
