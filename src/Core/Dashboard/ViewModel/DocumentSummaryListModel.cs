using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dashboard.ViewModel
{
   public class DocumentSummaryListModel
    {
        public IEnumerable<DocumentSummaryViewModel> Collection { get; set; }
    }
}
