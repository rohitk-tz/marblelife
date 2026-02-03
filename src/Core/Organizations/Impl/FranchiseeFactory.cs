using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Geo.Domain;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using Core.Sales;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using Core.Users.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeFactory : IFranchiseeFactory
    {
        private readonly IOrganizationFactory _organizationFactory;
        private readonly IFeeProfileFactory _feeProfileFactory;
        private readonly ILateFeeFactory _latefeeFactory;
        private readonly IFranchiseeServicesFactory _franchiseeServicesFactory;
        private readonly IRoyaltyFeeSlabsFactory _royaltyFeeSlabsFactory;
        private readonly ISalesDataUploadService _salesDataUploadService;
        private readonly IFranchiseeNotesFactory _franchiseeNotesFactory;
        private readonly IFranchiseeServiceFeeFactory _franchiseeServiceFeeFactory;
        private readonly IFranchiseeDocumentFactory _franchiseeDocumentFactory;
        private readonly IRepository<FranchiseeDocumentType> _franchiseeDocumentTypeRepository;
        private readonly IRepository<MinRoyaltyFeeSlabs> _minRoyalityFeeSlabRepository;
        private readonly IRepository<OnetimeprojectfeeAddFundRoyality> _onetimeprojectfeeAddFundRoyalityRepository;
        private readonly IRepository<ReviewPushAPILocation> _reviewPushAPILocationRepository;
        public readonly IClock _clock;
        public FranchiseeFactory(IOrganizationFactory organizationFactory, IFeeProfileFactory feeProfileFactory,
            IFranchiseeServicesFactory franchiseeServicesFactory, IRoyaltyFeeSlabsFactory royaltyFeeSlabsFactory, ISalesDataUploadService salesDataUploadService
            , ILateFeeFactory latefeeFactory, IFranchiseeNotesFactory franchiseeNotesfactory, IFranchiseeServiceFeeFactory franchiseeServiceFeeFactory,
            IFranchiseeDocumentFactory franchiseeDocumentFactory, IUnitOfWork unitOfWork, IClock clock)
        {
            _organizationFactory = organizationFactory;
            _feeProfileFactory = feeProfileFactory;
            _latefeeFactory = latefeeFactory;
            _franchiseeServicesFactory = franchiseeServicesFactory;
            _royaltyFeeSlabsFactory = royaltyFeeSlabsFactory;
            _salesDataUploadService = salesDataUploadService;
            _franchiseeNotesFactory = franchiseeNotesfactory;
            _franchiseeServiceFeeFactory = franchiseeServiceFeeFactory;
            _franchiseeDocumentFactory = franchiseeDocumentFactory;
            _franchiseeDocumentTypeRepository = unitOfWork.Repository<FranchiseeDocumentType>();
            _onetimeprojectfeeAddFundRoyalityRepository = unitOfWork.Repository<OnetimeprojectfeeAddFundRoyality>();
            _clock = clock;
            _minRoyalityFeeSlabRepository = unitOfWork.Repository<MinRoyaltyFeeSlabs>();
            _reviewPushAPILocationRepository = unitOfWork.Repository<ReviewPushAPILocation>();
        }

        public Franchisee CreateDomain(FranchiseeEditModel model, Franchisee inDb)
        {
            bool isUpdate = model.Id > 0;

            var franchisee = inDb != null ? inDb : new Franchisee();
            franchisee.Currency = model.Currency;
            franchisee.OwnerName = model.OrganizationOwner.OwnerName.ToString();
            franchisee.QuickBookIdentifier = model.QuickBookIdentifier;
            franchisee.LateFee = _latefeeFactory.CreateDomain(model.LateFee, model.Id, franchisee.LateFee);
            franchisee.FeeProfile = _feeProfileFactory.CreateDomain(model.FeeProfile, model.Id, franchisee.FeeProfile);
            franchisee.BusinessId = model.BusinessId;
            franchisee.IsReviewFeedbackEnabled = model.IsReviewFeedbackEnabled;
            franchisee.DisplayName = model.DisplayName;
            franchisee.SetGeoCode = model.SetGeoCode;
            if (franchisee.FeeProfile != null)
                franchisee.FeeProfile.Franchisee = franchisee;

            franchisee.EIN = model.EIN;
            franchisee.Description = model.Description;
            franchisee.DateOfRenewal = model.RenewalDate;
            franchisee.RenewalFee = model.RenewalFee;
            franchisee.LegalEntity = model.LegalEntity;
            franchisee.TransferFee = model.TransferFee;
            franchisee.OriginalFranchiseeFee = model.OriginalFee;
            franchisee.ContactFirstName = model.ContactFirstName;
            franchisee.ContactLastName = model.ContactLastName;
            franchisee.ContactEmail = model.ContactEmail;

            franchisee.AccountPersonFirstName = model.AccountPersonFirstName;
            franchisee.AccountPersonLastName = model.AccountPersonLastName;
            franchisee.AccountPersonEmail = model.AccountPersonEmail;

            franchisee.MarketingPersonFirstName = model.MarketingPersonFirstName;
            franchisee.MarketingPersonLastName = model.MarketingPersonLastName;
            franchisee.MarketingPersonEmail = model.MarketingPersonEmail;

            franchisee.SchedulerLastName = model.SchedulerLastName;
            franchisee.SchedulerFirstName = model.SchedulerFirstName;
            franchisee.SchedulerEmail = model.SchedulerEmail;

            franchisee.IsRoyality = model.IsRoyality;
            franchisee.FileId = model.FileId;
            franchisee.CategoryId = model.CategoryId;
            franchisee.CategoryNotes = model.CategoryNotes;
            franchisee.WebSite = model.WebSite;
            franchisee.ReviewpushId = model.ReviewRpId != 0 ? model.ReviewRpId : null;
            franchisee.NotesFromCallCenter = model.NotesFromCallCenter;
            franchisee.NotesFromOwner = model.NotesFromOwner;
            franchisee.Duration = model.Duration;
            franchisee.FranchiseeServices = _franchiseeServicesFactory.CreateDomainCollection(model.FranchiseeServices, franchisee).ToList();
            franchisee.LanguageId = model.LanguageId;
            franchisee.IsSEOActive = model.IsSEOActive;
            return franchisee;
        }

        public Organization CreateOrgDomain(FranchiseeEditModel model)
        {
            return _organizationFactory.CreateDomain(model);
        }

        public FranchiseeEditModel CreateEditModel(Franchisee domain, Person personDomain)
        {
            if (domain == null) return new FranchiseeEditModel();
            var model = new FranchiseeEditModel();

            var orgModel = _organizationFactory.CreateEditModel(domain.Organization);
            model.Taxrate = domain.SalesTax.GetValueOrDefault();
            model.RegistrationNumber = domain.RegistrationNumber;
            model.Name = orgModel.Name;
            model.Email = domain.Organization.Email;
            model.About = orgModel.About;
            model.Address = orgModel.Address;
            model.PhoneNumbers = orgModel.PhoneNumbers;
            model.DataRecorderMetaData = orgModel.DataRecorderMetaData;
            model.DataRecorderMetaDataId = orgModel.DataRecorderMetaDataId;
            model.Currency = domain.Currency;
            model.Id = domain.Id;
            model.OrganizationOwner.OwnerFirstName = personDomain.FirstName;
            model.OrganizationOwner.OwnerLastName = personDomain.LastName;
            model.OrganizationOwner.OwnerId = personDomain.Id;
            model.QuickBookIdentifier = domain.QuickBookIdentifier;
            model.Currency = domain.Currency;
            model.FranchiseeServices = _franchiseeServicesFactory.CreateEditModel(domain.FranchiseeServices);
            model.Documents = _franchiseeDocumentFactory.CreateEditModelForDocument(domain.FranchiseeDocumentType);
            var minRoyalitySlab = _minRoyalityFeeSlabRepository.Table.Where(x => x.FranchiseeId == domain.Id).ToList();
            model.FeeProfile = _feeProfileFactory.CreateEditModel(domain.FeeProfile, minRoyalitySlab);
            model.LateFee = _latefeeFactory.CreateEditModel(domain.LateFee);
            model.ServiceFees = _franchiseeServiceFeeFactory.CreateEditModel(domain.FranchiseeServiceFee);
            model.BusinessId = domain.BusinessId != null ? domain.BusinessId.Value : (long?)null;
            model.IsReviewFeedbackEnabled = domain.IsReviewFeedbackEnabled;
            model.DisplayName = domain.DisplayName;
            model.SetGeoCode = domain.SetGeoCode;

            model.EIN = domain.EIN;
            model.LegalEntity = domain.LegalEntity;
            model.RenewalFee = domain.RenewalFee;
            model.TransferFee = domain.TransferFee;
            model.Description = domain.Description;
            model.RenewalDate = domain.DateOfRenewal;
            model.OriginalFee = domain.OriginalFranchiseeFee;

            model.ContactEmail = domain.ContactEmail;
            model.ContactFirstName = domain.ContactFirstName;
            model.ContactLastName = domain.ContactLastName;

            model.AccountPersonFirstName = domain.AccountPersonFirstName;
            model.AccountPersonLastName = domain.AccountPersonLastName;
            model.AccountPersonEmail = domain.AccountPersonEmail;

            model.MarketingPersonFirstName = domain.MarketingPersonFirstName;
            model.MarketingPersonLastName = domain.MarketingPersonLastName;
            model.MarketingPersonEmail = domain.MarketingPersonEmail;

            model.SchedulerFirstName = domain.SchedulerFirstName;
            model.SchedulerLastName = domain.SchedulerLastName;
            model.SchedulerEmail = domain.SchedulerEmail;

            model.IsRoyality = domain.IsRoyality;
            model.FileId = domain.File != null ? (long?)domain.File.Id : null;
            model.FileName = domain.File != null ? (domain.File.RelativeLocation + "\\" + domain.File.Name).ToFullPath() : "";
            model.CategoryId = domain.CategoryId;
            model.CategoryNotes = domain.CategoryNotes;
            model.Is0Franchisee = domain.Organization.Name.StartsWith("0") ? true : false;
            model.WebSite = domain.WebSite;
            model.ReviewRpId = domain.ReviewpushId;
            model.NotesFromCallCenter = domain.NotesFromCallCenter;
            model.NotesFromOwner = domain.NotesFromOwner;
            model.Duration = domain.Duration;
            model.LanguageId = domain.LanguageId;
            model.IsSEOActive = domain.IsSEOActive;
            model.IsSEOActiveOriginal = domain.IsSEOActive;
            model.IsSEOInRoyalty = GetSEOChargesStatus(domain);
            model.ReviewURL = GetReviewURL(domain);

            return model;
        }

        public FranchiseeViewModel CreateViewModel(Franchisee domain, List<FranchiseeDurationNotesHistry> list)
        {
            if (domain == null) return new FranchiseeViewModel();

            var model = new FranchiseeViewModel();
            var salesDataUpload = _salesDataUploadService.GetSalesDataUploadByFranchiseeId(domain.Id);
            var orgModel = _organizationFactory.CreateViewModel(domain.Organization);
            var franchiseeNotes = domain.FranchiseeNotes.OrderByDescending(x => x.DataRecorderMetaData.DateCreated).FirstOrDefault();
            var accountCredit = domain.FranchiseeAccountCredit.Select(x => x.RemainingAmount);
            // invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type).Select(x => x.Amount)

            model.Name = orgModel.Name;
            model.Email = orgModel.Email;
            model.About = orgModel.About;
            model.Address = orgModel.Address;
            model.PhoneNumbers = orgModel.PhoneNumbers;
            model.Id = domain.Id;
            model.OwnerName = domain.OwnerName;
            model.Currency = domain.Currency;
            model.QuickBookIdentifier = domain.QuickBookIdentifier;
            model.Text = franchiseeNotes != null ? franchiseeNotes.Text : null;
            model.NotesCreatedOn = franchiseeNotes != null ? franchiseeNotes.DataRecorderMetaData.DateCreated : (DateTime?)null;
            model.SalesReportStatus = salesDataUpload != null ? salesDataUpload.Lookup.Name : null;
            model.AccountCredit = accountCredit.Sum();
            model.IsActive = domain.Organization.IsActive;
            model.BusinessId = domain.BusinessId;
            model.DeactivationNote = domain.Organization.DeactivationnNote;
            model.FranchiseeDurationCount = list.Count();
            model.Duration = domain.Duration;
            model.NoteFromCallCenter = domain.NotesFromCallCenter;
            model.NoteFromOwner = domain.NotesFromOwner;
            return model;
        }
        public FranchiseeViewModelForDownload CreateViewModelForDownload(Franchisee domain)
        {
            if (domain == null) return new FranchiseeViewModelForDownload();
            string phoneNumberString = "";
            string address = "";
            var model = new FranchiseeViewModelForDownload();
            var salesDataUpload = _salesDataUploadService.GetSalesDataUploadByFranchiseeId(domain.Id);
            var orgModel = _organizationFactory.CreateViewModel(domain.Organization);
            var franchiseeNotes = domain.FranchiseeNotes.OrderByDescending(x => x.DataRecorderMetaData.DateCreated).FirstOrDefault();
            var accountCredit = domain.FranchiseeAccountCredit.Select(x => x.RemainingAmount);
            // invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type).Select(x => x.Amount)

            foreach (var phoneNumber in orgModel.PhoneNumbers)
            {
                System.Reflection.PropertyInfo number = phoneNumber.GetType().GetProperty("Number");
                System.Reflection.PropertyInfo name = phoneNumber.GetType().GetProperty("Name");
                phoneNumberString += (string)number.GetValue(phoneNumber, null) + " (" + (string)name.GetValue(phoneNumber, null) + " )" + " ,";
            }


            model.Name = orgModel.Name;
            model.Email = orgModel.Email;
            model.Address = orgModel.Address != null ? orgModel.Address.AddressLine1 + " , " + orgModel.Address.AddressLine2 + " ," + orgModel.Address.State + " ," + orgModel.Address.City : "";
            model.PhoneNumbers = phoneNumberString;
            model.Id = domain.Id;
            model.OwnerName = domain.OwnerName;
            model.Text = franchiseeNotes != null ? franchiseeNotes.Text : null;
            model.NotesCreatedOn = franchiseeNotes != null ? _clock.ToUtc(franchiseeNotes.DataRecorderMetaData.DateCreated) : (DateTime?)null;
            model.AccountCredit = accountCredit.Sum();
            model.BusinessId = domain.BusinessId;
            model.DeactivationNote = domain.Organization.DeactivationnNote;
            return model;
        }


        public FranchiseeViewModelForFranchiseeDirectoryDownload CreateViewModelForFranchiseeDirectoryDownload(Franchisee domain)
        {
            if (domain == null) return new FranchiseeViewModelForFranchiseeDirectoryDownload();
            string phoneNumberString = "";
            string address = "";
            var model = new FranchiseeViewModelForFranchiseeDirectoryDownload();
            var salesDataUpload = _salesDataUploadService.GetSalesDataUploadByFranchiseeId(domain.Id);
            var orgModel = _organizationFactory.CreateViewModel(domain.Organization);
            var franchiseeNotes = domain.FranchiseeNotes.OrderByDescending(x => x.DataRecorderMetaData.DateCreated).FirstOrDefault();
            var accountCredit = domain.FranchiseeAccountCredit.Select(x => x.RemainingAmount);
            // invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)type).Select(x => x.Amount)

            foreach (var phoneNumber in orgModel.PhoneNumbers)
            {
                System.Reflection.PropertyInfo number = phoneNumber.GetType().GetProperty("Number");
                System.Reflection.PropertyInfo name = phoneNumber.GetType().GetProperty("Name");
                phoneNumberString += (string)number.GetValue(phoneNumber, null) + " (" + (string)name.GetValue(phoneNumber, null) + " )" + " ,";
            }


            model.Name = orgModel.Name;
            model.Email = orgModel.Email;
            model.Address = orgModel.Address != null ? orgModel.Address.AddressLine1 + " , " + orgModel.Address.AddressLine2 + " ," + orgModel.Address.State + " ," + orgModel.Address.City : "";
            model.PhoneNumbers = phoneNumberString;
            model.Id = domain.Id;
            model.OwnerName = domain.OwnerName;
            model.Country = orgModel.Address != null ? orgModel.Address.Country : "";
            return model;
        }

        public FranchiseeTechMailService CreateFranchiseeTechMailDomain(FranchiseeEditModel model)
        {
            var franchiseeTechMailServicedomainModel = new FranchiseeTechMailService
            {
                Amount = model.FranchiseeEmailEditModel.Amount,
                TechCount = !model.FranchiseeEmailEditModel.IsGeneric ? model.FranchiseeEmailEditModel.TechnianCount : 0,
                IsNew = model.Id > 0 ? false : true,
                Id = model.FranchiseeEmailEditModel.Id,
                IsGeneric = model.FranchiseeEmailEditModel.IsGeneric,
                IsDeleted = false,
                MultiplicationFactor = !model.FranchiseeEmailEditModel.IsGeneric ? (long)model.FranchiseeEmailEditModel.MultiplacationFactor : 0,

            };

            return franchiseeTechMailServicedomainModel;
        }


        public FranchiseeRedesignViewModel CreateResignViewModel(Franchisee domain, List<long> orgIdList, long? roleId, List<FranchiseeDurationNotesHistry> durationList)
        {
            var address = default(Address);
            if (domain == null) return new FranchiseeRedesignViewModel();

            var nameWithout0Prefix = "";
            if (domain.Organization != null)
                address = domain.Organization.Address.FirstOrDefault();


            var model = new FranchiseeRedesignViewModel();

            if (address != default(Address) && address.State != null)
            {
                string firstAlpha = address.State.Name.Substring(0, 1);
                model.FirstAlphaOfState = firstAlpha;
            }
            else if (address != default(Address) && address.State == null)
            {
                string firstAlpha = address.StateName.Substring(0, 1);
                model.FirstAlphaOfState = firstAlpha.ToUpper();

            }
            var salesDataUpload = _salesDataUploadService.GetSalesDataUploadByFranchiseeId(domain.Id);
            var orgModel = _organizationFactory.CreateViewModel(domain.Organization);
            var franchiseeNotes = domain.FranchiseeNotes.OrderByDescending(x => x.DataRecorderMetaData.DateCreated).FirstOrDefault();
            var accountCredit = domain.FranchiseeAccountCredit.Select(x => x.RemainingAmount);

            model.Name = orgModel.Name.ToUpper();
            model.Email = orgModel.Email;
            model.About = orgModel.About;
            model.Id = domain.Id;
            model.OwnerName = domain.OwnerName;
            model.Currency = domain.Currency;
            model.QuickBookIdentifier = domain.QuickBookIdentifier;
            model.Text = franchiseeNotes != null ? franchiseeNotes.Text : null;
            model.NotesCreatedOn = franchiseeNotes != null ? franchiseeNotes.DataRecorderMetaData.DateCreated : (DateTime?)null;
            model.SalesReportStatus = salesDataUpload != null ? salesDataUpload.Lookup.Name : null;
            model.AccountCredit = accountCredit.Sum();
            model.IsActive = domain.Organization.IsActive;
            model.BusinessId = domain.BusinessId;
            model.DeactivationNote = domain.Organization.DeactivationnNote;
            model.Country = address != default(Address) ? address.Country.Name : "";
            model.State = address != default(Address) ? address.State != null ? address.State.ShortName.ToUpper() : address.StateName.ToUpper() : "";
            model.City = address != default(Address) ? address.City != null ? address.City.Name : address.CityName : "";
            model.ZipCode = address != default(Address) ? address.Zip != null ? address.Zip.Code : address.ZipCode : "";
            model.CountryId = address != default(Address) ? address.Country.Id : default(long);
            model.Address = address != default(Address) ? GetAddress(address) : "";
            model.BusinessPhone = domain.Organization != null ? GetPhones(domain.Organization.Phones.ToList(), (long)PhoneType.BusinessDirectory) : "";
            model.CallCenterPhone = domain.Organization != null ? GetPhones(domain.Organization.Phones.ToList(), (long)PhoneType.CallCenter) : "";
            model.CellPhone = domain.Organization != null ? GetPhones(domain.Organization.Phones.ToList(), (long)PhoneType.Cell) : "";
            model.OFFICEPhone = domain.Organization != null ? GetPhones(domain.Organization.Phones.ToList(), (long)PhoneType.Office) : "";
            model.NameWithout0Prefix = nameWithout0Prefix;
            model.Duration = domain.Duration.GetValueOrDefault();
            model.NotesFromCallCenter = domain.NotesFromCallCenter;
            model.NotesFromOwner = domain.NotesFromOwner;
            model.IsAccessible = orgIdList.Any(x => x == domain.Id) ? true : false;
            if (roleId == ((long?)RoleType.SuperAdmin) || roleId == ((long?)RoleType.FrontOfficeExecutive))
            {
                model.IsAccessible = true;
            }
            model.DurationApprovalCount = durationList.Count();
            return model;

        }

        private string GetAddress(Address address)
        {
            string addressString = "";
            string addressLine1 = address.AddressLine1;
            string addressLine2 = address.AddressLine2;
            if (addressLine1 != "")
                addressString += addressLine1;
            if (addressLine2 != "")
                addressString += "," + addressLine2;
            return addressString;
        }

        private string GetPhones(List<Phone> phones, long TypeId)
        {
            string phoneNumber = "";
            phones = phones.Where(x => x.TypeId == TypeId).ToList();
            foreach (var phone in phones)
            {
                phoneNumber += phone.Number + ", ";
            }
            var indexOf = phoneNumber.LastIndexOf(',');
            if (phones.Count() > 0)
                phoneNumber = phoneNumber.Substring(0, indexOf);
            return phoneNumber;
        }

        private int GetSEOChargesStatus(Franchisee domain)
        {
            var model = _onetimeprojectfeeAddFundRoyalityRepository.Table.FirstOrDefault(x => x.FranchiseeId == domain.Id);
            if(model == null)
            {
                return 1;
            }
            else
            {
                return model.IsSEOInRoyalty == true ? 2 : 1;
            }
        }

        private string GetReviewURL(Franchisee domain)
        {
            string reviewURL = "";
            var reviewPushApiLocation = _reviewPushAPILocationRepository.Table.FirstOrDefault(x => x.Id == domain.ReviewpushId);
            reviewURL = reviewPushApiLocation?.NewRp_ID != null ? reviewPushApiLocation?.NewRp_ID : "";
            return reviewURL;
        }
    }
}
