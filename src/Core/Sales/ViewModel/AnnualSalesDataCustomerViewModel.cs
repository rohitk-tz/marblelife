using Core.Application.Attribute;
using System;

namespace Core.Sales.ViewModel
{
    [NoValidatorRequired]
    public class AnnualSalesDataCustomerViewModel
    {
        public long Id { get; set; }
        public string oldMarketingClass { get; set; }
        public string newMarketingClass { get; set; }
        public string oldAddress { get; set; }
        public string newAddress { get; set; }
        public bool isDiscrepancyAddress { get; set; }
        public string customerName { get; set; }
        public string franchiseeName { get; set; }
        public string invoiceId { get; set; }
        public string qbInvoiceId { get; set; }
        public DateTime? invoiceDate { get; set; }
        public string oldemailId { get; set; }
        public string oldphoneNumber { get; set; }
        public string newemailId { get; set; }
        public string newphoneNumber { get; set; }
        public string oldAddressLine1 { get; set; }
        public string newAddressLine1 { get; set; }
        public string newAddressLine2 { get; set; }
        public string oldAddressLine2 { get; set; }
        public string oldCity { get; set; }
        public string newCity { get; set; }
        public string oldState { get; set; }
        public string newState { get; set; }
        public string newZip { get; set; }
        public string oldZip { get; set; }
        public string newCountry { get; set; }
        public string oldCountry { get; set; }
        public bool canAddressEditable { get; set; }
        public string statusId { get; set; }
        public bool isStateChanged { get; set; }

    }
}
