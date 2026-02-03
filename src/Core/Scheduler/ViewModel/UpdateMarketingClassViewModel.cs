using Core.Application.Attribute;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Organizations.Impl.FranchiseeSalesFactory;

namespace Core.Scheduler.ViewModel
{
    [NoValidatorRequired]
    public class UpdateMarketingClassViewModel
    {
        public long Id { get; set; }
        public long InvoiceId { get; set; }
        public string CustomerName { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string ServiceName { get; set; }
        public string MarketingClass { get; set; }
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string FileId { get; set; }
        public string Description { get; set; }
        public CustomCustomerViewModel Customer { get; set; }
        public string Item { get; set; }
        public string FranchiseeName { get; set; }
        public List<UpdateMarketingClassViewModel> Histry { get; set; }
        public long LogFileId { get; set; }
    }
}
