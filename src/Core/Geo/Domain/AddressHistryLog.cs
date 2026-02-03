using Core.Billing.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Users.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Geo.Domain
{
  public  class AddressHistryLog : DomainBase
    {
        public virtual long? addressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public virtual long? TypeId { get; set; }
        public virtual long? CityId { get; set; }
        public virtual long? ZipId { get; set; }
        public virtual long? StateId { get; set; }
        public virtual long CountryId { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string ZipCode { get; set; }

        public string EmailId { get; set; }
        public string phoneNumber { get; set; }

        public long? MarketingClassId { get; set; }
        public long? FranchiseeSalesId { get; set; }
        public long? AnnualSalesDataUploadId { get; set; }
        public long? InvoiceId { get; set; }
        public DateTime? invoiceDate { get; set; }

        [ForeignKey("CityId")]
        public virtual City City { get; set; }

        [ForeignKey("ZipId")]
        public virtual Zip Zip { get; set; }

        [ForeignKey("StateId")]
        public virtual State State { get; set; }

        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
        [ForeignKey("addressId")]
        public virtual Address Address { get; set; }

        [ForeignKey("FranchiseeSalesId")]
        public virtual FranchiseeSales FranchiseeSales { get; set; }

        [ForeignKey("MarketingClassId")]
        public virtual MarketingClass MarketingClass { get; set; }

        [ForeignKey("AnnualSalesDataUploadId")]
        public virtual AnnualSalesDataUpload AnnualSalesDataUpload { get; set; }
        [ForeignKey("AnnualSalesDataUploadId")]
        public virtual Invoice Invoice { get; set; }

    }
}
