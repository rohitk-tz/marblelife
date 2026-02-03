using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
   public class DownloadCountyModel
    {
        [DisplayName("Id")]
        public long id { get; set; }
        [DisplayName("Country")]
        public string Country { get; set; }
        [DisplayName("County / Municipality")]
        public string CountyName { get; set; }
        [DisplayName("State / Province")]
        public string StateCode { get; set; }
        [DisplayName("TAAZAA FRANCHSE")]
        public string FranchiseeName { get; set; }
        //[DisplayName("TAAZAA FRANCHSE Transferable Number")]
        //public string FranchiseeTransferableNumber { get; set; }
        //[DisplayName("TAAZAA FRANCHSE Email")]
        //public string FranchiseeEmail { get; set; }
        [DisplayName("State/TERRITORY")]
        public string TerritoryCode { get; set; }
        [DisplayName("Direction Code")]
        public string DirectionCode { get; set; }
        [DisplayName("Direction from office")]
        public string DirectionFromOffice { get; set; }
        [DisplayName("Time (HR)")]
        public string ReachingTime { get; set; }
        [DisplayName("Population (,000)")]
        public decimal? Population { get; set; }
        [DisplayName("STATUS")]
        public string Status { get; set; }
        //[DisplayName("Franchise- MLD")]
        //public string FranchiseMLD { get; set; }
        [DisplayName("CONTRACTED TERRITORY")]
        public string ContractedTerritory { get; set; }
        [DisplayName("COVERING (<2.5 HR)")]
        public string CoveringLessThan3Hours { get; set; }
        [DisplayName("DAY TRIP (2.5 TO 4 HR)")]
        public string DayTrip { get; set; }
        [DisplayName("UNCOVERED (OVER 4 HRS)")]
        public string UnCovered { get; set; }
        [DisplayName("State-County")]
        public string StateCountryCode { get; set; }
        [DisplayName("isUpdated")]
        public int IsUpdated { get; set; }
        [DisplayName("isDeleted")]
        public int IsDeleted { get; set; }

    }



    public class DownloadInstructionModel
    {
        public string Instructions { get; set; }
    }
}
