using Core.Billing.Domain;
using Core.Organizations.Domain;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.MarketingLead.Domain
{
    public class WebLead : DomainBase
    {
        public long WebLeadId { get; set; } 
        public long WebLeadFranchiseeId { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string ProvinceName { get; set; }
        public string County { get; set; } 
        public string ZipCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string StreetAddress { get; set; }
        public string SuiteNumber { get; set; }
        public string City { get; set; }
        public string PropertyType { get; set; }
        public string SurfaceType { get; set; }
        public string ServiceDesc { get; set; }
        public string Contact { get; set; }
        public bool? AddEmail { get; set; }
        public string URL { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FEmail { get; set; }
        public long? FranchiseeId { get; set; }

        [ForeignKey("FranchiseeId")]
        public virtual Franchisee Franchisee { get; set; }

        public long? InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }
        public string FullName { 
            get
            {
                return string.Concat(Firstname, " ", LastName);
            }
        }
    }
}
