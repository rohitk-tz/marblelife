namespace Core.Billing.ViewModel
{
    public class CurrencyExchangeRateViewModel
    {
        public string disclaimer { get; set; }
        public string license { get; set; }
        public string timestamp { get; set; }
        public CurrencyRates rates { get; set; }
    }
}
