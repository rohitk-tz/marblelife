namespace Core.Geo.Domain
{
    public class Country: DomainBase
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string CurrencyCode { get; set;}
        public bool IsDefault { get; set; }
    }
}
