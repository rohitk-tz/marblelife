using Core.Application.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
    public class EstimateInvoiceCustomer : DomainBase
    {
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
        public string CustomerName { get; set; }
        public string StreetAddress { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public string CCEmail { get; set; }

    }
}
