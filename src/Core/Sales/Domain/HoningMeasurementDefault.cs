using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class HoningMeasurementDefault : DomainBase
    {
        public decimal? SectionsDefault { get; set; }
        public decimal? UGCDefault { get; set; }
        public decimal? ThirtyDefault { get; set; }
        public decimal? FiftyDefault { get; set; }
        public decimal? HundredDefault { get; set; }
        public decimal? TwoHundredDefault { get; set; }
        public decimal? FourHundredDefault { get; set; }
        public decimal? EightHundredDefault { get; set; }
        public decimal? FifteenHundredDefault { get; set; }
        public decimal? ThreeThousandDefault { get; set; }
        public decimal? EightThousandDefault { get; set; }
        public decimal? ElevenThousandDefault { get; set; }
        public decimal? CacoDefault { get; set; }
        public decimal? IhgDefault { get; set; }
        public decimal? DimensionDefault { get; set; }
        public decimal? ProdutivityRateDefault { get; set; }
        public decimal? TotalAreaDefault { get; set; }
        public decimal? TotalAreaInHourDefault { get; set; }
        public decimal? TotalAreaInShiftDefault { get; set; }
        public string TotalMinuteDefault { get; set; }
        public decimal? TotalCostDefault { get; set; }
        public decimal? TotalCostPerSquareDefault { get; set; }
        public decimal? MinRestorationDefault { get; set; }
        public decimal? SeventeenBaseDefault { get; set; }
        public bool? IsActive { get; set; }

        public long? EstimateInvoiceServiceId { get; set; }
        [ForeignKey("EstimateInvoiceServiceId")]
        public virtual EstimateInvoiceService EstimateInvoiceService { get; set; }

        public long? HoningMeasurementId { get; set; }
        [ForeignKey("HoningMeasurementId")]
        public virtual HoningMeasurement HoningMeasurement { get; set; }
    }
}

