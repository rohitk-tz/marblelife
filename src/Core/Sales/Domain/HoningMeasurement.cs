
using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class HoningMeasurement : DomainBase
    {
        public string ShiftName { get; set; }
        public decimal? ShiftPrice { get; set; }
        public decimal? SeventeenBase { get; set; }
        public decimal? Area { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Sections { get; set; }
        public decimal? UGC { get; set; }
        public decimal? Thirty { get; set; }
        public decimal? Fifty { get; set; }
        public decimal? Hundred { get; set; }
        public decimal? TwoHundred { get; set; }
        public decimal? FourHundred { get; set; }
        public decimal? EightHundred { get; set; }
        public decimal? FifteenHundred { get; set; }
        public decimal? ThreeThousand { get; set; }
        public decimal? EightThousand { get; set; }
        public decimal? ElevenThousand { get; set; }
        public decimal? Caco { get; set; }
        public decimal? Ihg { get; set; }
        public decimal? Dimension { get; set; }
        public decimal? ProdutivityRate { get; set; }
        public decimal? TotalArea { get; set; }
        public decimal? TotalAreaInHour { get; set; }
        public decimal? TotalAreaInShift { get; set; }
        public string TotalMinute { get; set; }
        public decimal? TotalCost { get; set; }
        public decimal? TotalCostPerSquare { get; set; }
        public decimal? MinRestoration { get; set; }
        public decimal? StartingPointTechShiftEstimates { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsShiftPriceChanged { get; set; }

        public long? EstimateInvoiceServiceId { get; set; }
        [ForeignKey("EstimateInvoiceServiceId")]
        public virtual EstimateInvoiceService EstimateInvoiceService { get; set; }

        public long DataRecorderMetaDataId { get; set; }
        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public string RowDescription { get; set; }
    }
}

