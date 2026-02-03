using Core.Application.Attribute;
using System.Collections.Generic;

namespace Core.Reports.ViewModel
{
    [NoValidatorRequired]
    public class ServiceReportViewModel
    {
        public long FranchiseeId { get; set; }
        public string Franchisee { get; set; }
        public string MarketingClass { get; set; }
        public string Service { get; set; }

        [DownloadField(Required = false)]
        public long? ServiceTypeId { get; set; }

        [DownloadField(Required = false)]
        public long ClassTypeId { get; set; }

        [DownloadField(CurrencyType = "$")]
        public decimal TotalSales { get; set; }
        public string PrimaryContact { get; set; }       
        
        public string FranchiseeEmail { get; set; }

        [DownloadField(Required = false)]
        public IEnumerable<object> PhoneNumbers { get; set; }

        public string PhoneNumber { get; set; }
    }
}
