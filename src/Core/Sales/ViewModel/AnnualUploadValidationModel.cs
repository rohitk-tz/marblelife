using Core.Application.Attribute;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AnnualUploadValidationModel
    {
        public long? PaymentFrequencyId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public DateTime UploadStartDate { get; set; }
        public DateTime UploadEndDate { get; set; }
        public bool IsAnnualUpload { get; set; }
        public long FranchiseeId { get; set; }

        public string AnnualUploadYears { get; set; }
    }
}
