using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Billing.Domain
{
    public class PaymentInstrument : DomainBase
    {
        [ForeignKey("FranchiseePaymentProfileId")]
        public virtual FranchiseePaymentProfile FranchiseePaymentProfile { get; set; }

        public long InstrumentTypeId { get; set; }
        public long FranchiseePaymentProfileId { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsActive { get; set; }

        public long InstrumentEntityId { get; set; }

        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }

        public string InstrumentProfileId { get; set; }

        [ForeignKey("InstrumentTypeId")]
        public virtual Lookup InstrumentType { get; set; }
    }
}
