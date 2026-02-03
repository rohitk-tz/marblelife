using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dashboard.ViewModel
{
    [NoValidatorRequired]
    public class RecentInvoiceListModel
    {
        public IEnumerable<RecentInvoiceViewModel> Collection { get; set; }
    }
}

