using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.ViewModel
{
    public class TechnicianWorkOrderForInvoiceList
    {
        public List<TechnicianWorkOrderForInvoiceViewModel> TechnicianWorkOrderList { get; set; }
    }
    public class TechnicianWorkOrderForInvoiceViewModel
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
