using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.Domain
{
   public class EstimateInvoiceServiceDescription : DomainBase
    {
        public string ServiceType { get; set; }
        public string Description { get; set; }
    }
}
