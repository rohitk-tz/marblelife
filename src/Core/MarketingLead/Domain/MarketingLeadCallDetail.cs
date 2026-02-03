using Core.Application;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{

    public class MarketingLeadCallDetail : DomainBase
    {
        public string SessionId { get; set; }
        public DateTime? DateAdded { get; set; }
        public string DialedNumber { get; set; }
        public string CallerId { get; set; }
        public long CallTypeId { get; set; }
        public string CallTransferType { get; set; }
        public string PhoneLabel { get; set; }
        public string TransferToNumber { get; set; }
        public string ClickDescription { get; set; }
        public int CallDuration { get; set; }
        public bool ValidCall { get; set; }
        public long TagId { get; set; }

        public bool IsFromNewAPI { get; set; }

        [ForeignKey("CallTypeId")]
        public virtual Lookup CallType { get; set; }

        public long? FranchiseeId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }
        public long? InvoiceId { get; set; }

        public long? CalledFranchiseeId { get; set; }

        [ForeignKey("CalledFranchiseeId")]
        public virtual Franchisee CalledFranchisee { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public bool IsFromInvoca { get; set; }

        [NotMapped]
        public DateTime TimeZoneStartTime
        {
            get
            {
                var a = DateAdded.GetValueOrDefault().AddMinutes((Convert.ToDouble(ApplicationManager.DependencyInjection.Resolve<IClock>().BrowserTimeZone)));
                return a;
            }
        }
    }
}
