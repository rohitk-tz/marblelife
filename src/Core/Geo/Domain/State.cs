using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Geo.Domain
{
    public class State : DomainBase
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public long CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }
    }
}
