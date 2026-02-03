using Core.Application.ViewModel;
using Core.Geo.ViewModel;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;

namespace Core.Scheduler.ViewModel
{
   public class CountyCreateEditModel : EditModelBase
    {
        public long Id { get; set; }
        public long IsUpdated { get; set; }
        public string CountryName { get; set; }
        public string CountyName { get; set; }
        public string StateCode { get; set; }
        public string FranchiseeName { get; set; }
        public string TerritoryCode { get; set; }
        public string DirectionCode { get; set; }
        public string DirectionFromOffice { get; set; }
        public string ReachingTime { get; set; }
        public decimal? Population { get; set; }
        public string Status { get; set; }
        //public string FranchiseMLD { get; set; }
        public string ContractedTerritory { get; set; }
        public string CoveringLessThan3Hours { get; set; }
        public string DayTrip { get; set; }
        public string UnCovered { get; set; }
        public string StateCountryCode { get; set; }
        public long IsDeleted { get; set; }
    }
}
