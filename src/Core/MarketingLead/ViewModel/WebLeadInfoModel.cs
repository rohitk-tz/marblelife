using Core.Application.Attribute;
using System;

namespace Core.MarketingLead.ViewModel
{
    public class WebLeadInfoModel
    {
        [DownloadField(Required = false)]
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string SuiteNumber { get; set; }
        public string StreetAddress { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string PropertyType { get; set; }
        public string SurfaceType { get; set; }
        public string Contact { get; set; }
        public string Franchisee { get; set; }
        public string FranchiseeEmail { get; set; }
        public string Url { get; set; }
        public string ServiceDescription { get; set; }
        [DownloadField(Required = false)]
        public long? FranchiseeId { get; set; }
        public long? InvoiceId { get; set; }
        [DownloadField(Required = false)]
        public long WebLeadId { get; set; }
    }
}
