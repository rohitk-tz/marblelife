namespace Core.Sales.Impl
{
    public class InvoiceInfoEditModel
    {
        public long InvoiceId { get; set; }
        public long InvoiceItemId { get; set; }
        public long ServiceTypeId { get; set; }
        public long ClassTypeId { get; set; }
        public long? SubClassTypeId { get; set; }
    }
}