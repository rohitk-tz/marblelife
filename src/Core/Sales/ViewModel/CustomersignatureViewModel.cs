using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class CustomersignatureViewModel
    {
        public long? EstimateInvoiceId { get; set; }
        public long? CustomerId { get; set; }
        public string Signature { get; set; }
        public long? JobSchedulerId { get; set; }
        public long? SchedulerId { get; set; }
        public string Name { get; set; }
        public List<long> InvoiceNumbers { get; set; }
        public long? UserId { get; set; }
        public bool? IsFromJob { get; set; }
        public long? JobOrginialSchedulerId { get; set; }

        public bool IsFromURL { get; set; }
        public long? TypeId { get; set; }
    }
}
