namespace Core.Billing.ViewModel
{
    public class PaymentItemEditModel
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public long ItemTypeId { get; set; }
        public string Item { get; set; }
    }
}
