using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Sales.Domain
{
   public class AnnualReportType :DomainBase
    {
        public string ReportTypeName { get; set; }
        public string Description { get; set; }
    }
}
