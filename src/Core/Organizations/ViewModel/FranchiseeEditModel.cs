using Core.Organizations.ViewModel;
using System.Collections.Generic;
using Core.Users.ViewModels;
using System;
using Core.Scheduler.ViewModel;

namespace Core.Organizations.ViewModels
{
    public class FranchiseeEditModel : OrganizationEditModel
    {
        public long? UserId { get; set; }
        public string QuickBookIdentifier { get; set; }
        public string Currency { get; set; }
        public IEnumerable<FranchiseeServiceEditModel> FranchiseeServices { get; set; }
        public FranchiseeEmailEditModel FranchiseeEmailEditModel { get; set; }
        public FeeProfileEditModel FeeProfile { get; set; }
        public MinRoyaltyFeeSlabsEditModel MinRoyalityFeeProfile { get; set; }
        public LateFeeEditModel LateFee { get; set; }
        public ICollection<FranchiseeServiceFeeEditModel> ServiceFees { get; set; }
        public IEnumerable<DocumentTypeEditModel> Documents { get; set; }
        public OrganizationOwnerEditModel OrganizationOwner { get; set; }
        public FranchiseeEmailEditModel franchiseeTechMailServiceEditModel { get; set; }
        public long? BusinessId { get; set; }
        public string Text { get; set; }
        public bool IsReviewFeedbackEnabled { get; set; }
        public string DisplayName { get; set; }
        public bool SetGeoCode { get; set; }
        public FileUploadModel FileUploadModel { get; set; }
        public string DeactivationNote { get; set; }
        public decimal? RenewalFee { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string Description { get; set; }
        public string EIN { get; set; }
        public string LegalEntity { get; set; }
        public decimal? TransferFee { get; set; }
        public decimal? OriginalFee { get; set; }
        public bool IsActive { get; set; }
        public bool IsImageChanged { get; set; }

        public bool IsRoyality { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactEmail { get; set; }

        public string AccountPersonFirstName { get; set; }
        public string AccountPersonLastName { get; set; }
        public string AccountPersonEmail { get; set; }

        public string MarketingPersonFirstName { get; set; }
        public string MarketingPersonLastName { get; set; }
        public string MarketingPersonEmail { get; set; }

        public string SchedulerFirstName { get; set; }
        public string SchedulerLastName { get; set; }
        public string SchedulerEmail { get; set; }
        public long? FileId { get; set; }
        public string Css { get; set; }
        public string FileName { get; set; }
        public string CategoryNotes { get; set; }
        public long? CategoryId { get; set; }

        public bool? Is0Franchisee { get; set; }
        public string WebSite { get; set; }
        public LeadPerformanceEditModel LeadPerformanceEditModel { get; set; }
        public long? ReviewRpId { get; set; }
        public bool IsMinRoyalityFixed { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string NotesFromCallCenter { get; set; }
        public string NotesFromOwner { get; set; }
        public decimal? Duration { get; set; }
        public List<FranchiseeRegistrationHistryViewModel> FranchisieeHistryViewModel { get; set; }

        public string RegistrationNumber { get; set; }
        public decimal Taxrate { get; set; }
        public decimal? LessDeposit { get; set; }
        public long? LanguageId { get; set; }
        public bool IsSEOActive { get; set; }
        public bool IsSEOActiveOriginal { get; set; }
        public long SeoCostBillingPeriodId { get; set; }
        public bool HasNotes { get; set; }
        public int IsSEOInRoyalty { get; set; }
        public string ReviewURL { get; set; }
        public FranchiseeEditModel()
        {
            FranchiseeServices = new List<FranchiseeServiceEditModel>();
            FeeProfile = new FeeProfileEditModel();
            MinRoyalityFeeProfile = new MinRoyaltyFeeSlabsEditModel();
            LateFee = new LateFeeEditModel();
            OrganizationOwner = new OrganizationOwnerEditModel();
            SetGeoCode = true;
            ServiceFees = new List<FranchiseeServiceFeeEditModel>();
            Documents = new List<DocumentTypeEditModel>();
            FranchiseeEmailEditModel = new FranchiseeEmailEditModel();
            LeadPerformanceEditModel = new LeadPerformanceEditModel();
            FranchisieeHistryViewModel = new List<FranchiseeRegistrationHistryViewModel>();
            franchiseeTechMailServiceEditModel = new FranchiseeEmailEditModel();
        }
    }
}