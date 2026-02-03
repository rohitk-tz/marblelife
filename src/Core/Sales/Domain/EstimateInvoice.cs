using Core.Application.Domain;
using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Sales.Domain
{
  public  class EstimateInvoice : DomainBase
    {
        public long DataRecorderMetaDataId { get; set; }

        [ForeignKey("DataRecorderMetaDataId")]
        public virtual DataRecorderMetaData DataRecorderMetaData { get; set; }
    
        public long? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }
    
        public long? InvoiceCustomerId { get; set; }
        [ForeignKey("InvoiceCustomerId")]
        public virtual EstimateInvoiceCustomer EstimateInvoiceCustomer { get; set; }
   
        public float PriceOfService { get; set; }
        
        public float LessDeposit { get; set; }
   
        public long ClassTypeId { get; set; }
        [ForeignKey("ClassTypeId")]
        public virtual MarketingClass MarketingClass { get; set; }
       
        
        public long? FranchiseeId { get; set; }
        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        public long? EstimateId { get; set; }

        [ForeignKey("EstimateId")]
        public virtual JobEstimate JobEstimate { get; set; }
    
        public long? SchedulerId { get; set; }
        [ForeignKey("SchedulerId")]
        public virtual JobScheduler JobScheduler { get; set; }

        public long? NumberOfInvoices { get; set; }
        public string Option { get; set; }
        public string Notes { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public bool IsInvoiceChanged { get; set; }
        public bool? IsCustomerAvailable { get; set; }

        public bool IsInvoiceParsing { get; set; }
    }
}
