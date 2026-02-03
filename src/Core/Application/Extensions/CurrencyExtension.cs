using System;

namespace Core.Application.Extensions
{
    public static class CurrencyExtension
    {
        public static decimal ToDefaultCurrency(this decimal amount, decimal rate)
        {
            if (rate != 0 && amount != 0)
                return decimal.Parse((amount * rate).ToString("#.##"));
            else return amount;
        }
        public static decimal ToLocalCurrency(this decimal amount, decimal rate)
        {
            if (rate != 0 && amount != 0)
                return decimal.Parse((amount / rate).ToString("#.##"));
            else return amount;
        }
    }
}
