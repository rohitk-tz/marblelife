using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.Domain
{
   public class MasterMarketingClass : DomainBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ColorCode { get; set; }
    }
}
