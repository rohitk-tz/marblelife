using Core.Application.Attribute;

namespace Core.Geo.ViewModel
{
    public class AddressViewModel
    {
        [DownloadField(Required = false)]
        public long Id { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

        public string Country { get; set; }
        public string State { get; set; }
        [DownloadField(Required = false)]
        public long StateId { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }

        [DownloadField(Required = false)]
        public long CountryId { get; set; }

        public override string ToString()
        {
            return string.Concat(AddressLine1, (string.IsNullOrEmpty(AddressLine2) ? ", " : string.Concat(", ", AddressLine2, ", ")), City, " ", State, " - ", ZipCode);
        }

        public AddressViewModel()
        {
        }
    }
}
