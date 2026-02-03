using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Billing.Domain;
using Core.Reports.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Organizations.Domain
{
    public class Franchisee : DomainBase
    {
        [ForeignKey("Organization")]
        public override long Id { get; set; }
        public string OwnerName { get; set; }
        public string QuickBookIdentifier { get; set; }
        public string Currency { get; set; }
        public string DisplayName { get; set; }
        public virtual Organization Organization { get; set; }

        [CascadeEntity]
        public virtual FeeProfile FeeProfile { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<FranchiseeService> FranchiseeServices { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<FranchiseeDocumentType> FranchiseeDocumentType { get; set; }
        [CascadeEntity]
        public virtual LateFee LateFee { get; set; }


        public virtual ICollection<FranchiseeInvoice> FranchiseeInvoices { get; set; }
        public virtual ICollection<SalesDataUpload> SalesDataUploads { get; set; }

        public virtual ICollection<FranchiseeSales> FranchiseeSales { get; set; }
        //  public virtual ICollection<SalesDataMailReminder> SalesDataMailReminders { get; set; }
        public virtual ICollection<FranchiseeAccountCredit> FranchiseeAccountCredit { get; set; }

        [CascadeEntity(IsCollection = true)]
        public virtual ICollection<FranchiseeNotes> FranchiseeNotes { get; set; }

        public long? BusinessId { get; set; }
        public bool IsReviewFeedbackEnabled { get; set; }
        public bool SetGeoCode { get; set; }
        public string WebLeadFranchiseeId { get; set; }

        public long? FileId { get; set; }
        public virtual ICollection<BatchUploadRecord> BatchUploadRecord { get; set; }

        public virtual ICollection<FranchiseeServiceFee> FranchiseeServiceFee { get; set; }

        [ForeignKey("FileId")]
        public virtual File File { get; set; }
        public string EIN { get; set; }
        public string LegalEntity { get; set; }
        public decimal? RenewalFee { get; set; }
        public DateTime? DateOfRenewal { get; set; }
        public string Description { get; set; }
        public string CategoryNotes { get; set; }
        public decimal? TransferFee { get; set; }
        public decimal? OriginalFranchiseeFee { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactEmail { get; set; }
        public string AccountPersonFirstName { get; set; }
        public string AccountPersonLastName { get; set; }
        public string AccountPersonEmail { get; set; }

        public string MarketingPersonFirstName { get; set; }
        public string MarketingPersonLastName { get; set; }
        public string MarketingPersonEmail { get; set; }
        public long? CategoryId { get; set; }
        public long? ReviewpushId { get; set; }
        public bool IsRoyality { get; set; }

        public string WebSite { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Lookup Category { get; set; }
        [ForeignKey("ReviewpushId")]
        public virtual ReviewPushAPILocation Reviewpush { get; set; }

        public bool IsMinRoyalityFixed { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string NotesFromCallCenter { get; set; }
        public string NotesFromOwner { get; set; }
        public decimal? Duration { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? LessDeposit { get; set; }
        public long? LanguageId { get; set; }
        [ForeignKey("LanguageId")]
        public virtual Lookup Language { get; set; }

        public string SchedulerFirstName { get; set; }
        public string SchedulerLastName { get; set; }
        public string SchedulerEmail { get; set; }
        public bool IsSEOActive { get; set; }
        public Franchisee()
        {
            FranchiseeInvoices = new Collection<FranchiseeInvoice>();
            SalesDataUploads = new Collection<SalesDataUpload>();
            BatchUploadRecord = new Collection<BatchUploadRecord>();
            FranchiseeSales = new Collection<FranchiseeSales>();
            FranchiseeAccountCredit = new Collection<FranchiseeAccountCredit>();
            FranchiseeNotes = new Collection<FranchiseeNotes>();
            IsReviewFeedbackEnabled = false;
            SetGeoCode = false;
            FranchiseeServiceFee = new Collection<FranchiseeServiceFee>();
            FranchiseeDocumentType = new Collection<FranchiseeDocumentType>();
        }
    }
}
