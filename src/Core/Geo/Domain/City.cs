using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Geo.Domain
{
    public class City : DomainBase
    {
        public  string Name { get; set; }
        public  long StateId { get; set; }
        [ForeignKey("StateId")]
        public virtual  State State { get; set; }
        public virtual IList<Zip> Zips { get; set; }
        public City()
        {
            Zips = new List<Zip>();
        }
    }
}
