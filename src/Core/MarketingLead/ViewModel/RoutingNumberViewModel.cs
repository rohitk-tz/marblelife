using Core.Application.Attribute;

namespace Core.MarketingLead.ViewModel
{
    public class RoutingNumberViewModel
    {
        public long Id { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneLabel { get; set; }
        public string Franchisee { get; set; }

        [DownloadField(Required = false)]
        public string FranchiseeId { get; set; }

        public string Tag { get; set; }

        [DownloadField(Required = false)]
        public string TagId { get; set; } 
    }
}
