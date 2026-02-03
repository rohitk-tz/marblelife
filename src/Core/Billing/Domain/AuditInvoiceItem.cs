namespace Core.Billing.Domain
{
    public class AuditInvoiceItem : DomainBase
    {
        public long AuditInvoiceId { get; set; } 
        public long? ItemId { get; set; }

        public long ItemTypeId { get; set; }

        public string Description { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public virtual AuditInvoice AuditInvoice { get; set; }
    }
}
