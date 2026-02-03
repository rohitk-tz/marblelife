using Core.Application.Attribute;
using System;
using System.ComponentModel;

namespace Core.MarketingLead.ViewModel
{
    public class CallDetailViewModelV2
    {
        [DisplayName("Id")]
        public long Id { get; set; }
        [DisplayName("Entered Zip Code")]
        public string EnteredZipCode { get; set; }
        [DisplayName("Call Date")]
        public DateTime DateOfCall { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Caller Id")]
        public string CallerId { get; set; }
        [DisplayName("State")]
        public string State { get; set; }
        [DisplayName("Call Status")]
        public string CallStatus { get; set; }
        [DisplayName("Transfer to Number")]
        public string TransferNumber { get; set; }
        [DisplayName("Destination")]
        public string Destination { get; set; }
        [DisplayName("Phone Label")]
        public string PhoneLabel { get; set; }

        [DisplayName("Call Route(Mapped By ZipCode)")]
        public string CallRoute { get; set; }
        [DisplayName("Call Duration")]
        public string CallDuration { get; set; }
        [DisplayName("Office")]
        public string FranchiseeName { get; set; }
        [DisplayName("Source")]
        public string Source { get; set; }
        [DisplayName("Re-Route")]
        public string Reroute { get; set; }
        [DisplayName("Source Qualified")]
        public string SourceQualified { get; set; }
        [DisplayName("Repeat Source Caller")]
        public string RepeaSourceCaller { get; set; }

        [DownloadField(Required = false)]
        public string SetName { get; set; }
        [DownloadField(Required = false)]
        public string StreetAddress { get; set; }
        [DownloadField(Required = false)]
        public string City { get; set; }
      
        [DownloadField(Required = false)]
        public string ZipCode { get; set; }
        
        [DownloadField(Required = false)]
        public string TalkMintues { get; set; }
        [DownloadField(Required = false)]
        public string TalkSeconds { get; set; }
        [DownloadField(Required = false)]
        public string TotalMintues { get; set; }
        [DownloadField(Required = false)]
        public string TotalSeconds { get; set; }
        [DownloadField(Required = false)]
        public string Sid { get; set; }

        [DownloadField(Required = false)]
        public string IvrResults { get; set; }
        
        [DownloadField(Required = false)]
        public string SourceNumber { get; set; }



        [DownloadField(Required = false)]
        public string SourceCap { get; set; }
        [DownloadField(Required = false)]
        public string CallRouteQualified { get; set; }
        
        
        [DownloadField(Required = false)]
        public string APPState { get; set; }
        [DownloadField(Required = false)]
        public string DataFromNewAPI { get; set; }
        [DownloadField(Required = false)]
        public string DataFromInvoca { get; set; }
        [DisplayName("Find Me List")]
        public string FindMeList { get; set; }

    }
}
