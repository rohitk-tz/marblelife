using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Geo.Domain
{
    public class Zip : DomainBase
    {
        public string Code { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public virtual IList<City> Cities { get; set; }
    }
}
