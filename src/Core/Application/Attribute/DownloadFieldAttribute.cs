namespace Core.Application.Attribute
{
    public class DownloadFieldAttribute : System.Attribute
    {
        public bool IsCollection { get; set; }

        public bool IsComplexType { get; set; }

        public bool Required { get; set; }
        public string DisplayName { get; set; }
        public string CurrencyType { get; set; }

        public DownloadFieldAttribute() : this(true)
        { }

        public DownloadFieldAttribute(bool isCollection, bool complexType) : this(true)
        {
            IsCollection = isCollection;
            IsComplexType = complexType;
        }

        public DownloadFieldAttribute(bool required)
        {
            Required = required;
        }

        public DownloadFieldAttribute(string displayName, string currencyType) : this(true)
        {
            DisplayName = displayName;
            CurrencyType = currencyType;
        }
    }
}
