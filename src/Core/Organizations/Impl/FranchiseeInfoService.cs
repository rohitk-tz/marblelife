using Core.Organizations.ViewModels;
using Core.Organizations.Domain;
using Core.Application;
using Core.Application.Attribute;
using System.Linq;
using Core.Application.ViewModel;
using Core.Users.Domain;
using Core.Users;
using Core.Geo;
using Core.Users.Enum;
using Core.Organizations.ViewModel;
using Core.Application.Impl;
using Core.Sales.Domain;
using Core.Billing.Domain;
using System.Collections.Generic;
using Core.Scheduler.ViewModel;
using Core.Application.Extensions;
using System;
using System.IO;
using Core.Scheduler.Domain;
using Core.Organizations.Enum;
using Core.Geo.Domain;
using NodaTime;

using IClock = Core.Application.IClock;
using DocumentType = Core.Organizations.Domain.DocumentType;
using Core.Sales.Enum;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeInfoService : IFranchiseeInfoService
    {
        private readonly IFranchiseeLeadPerformanceFactory _franchiseeLeadPerformanceFactory;
        private readonly IFranchiseeTechnicianMailFactory _franchiseeTechnicianMailFactory;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<FranchiseeTechMailService> _franchiseeTechMailServiceRepository;
        private readonly IRepository<FranchiseeService> _franchiseeServiceRepository;
        private readonly IRepository<FeeProfile> _feeProfileRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<FranchiseDocument> _franchiseeDocumentRepository;
        private readonly IRepository<Domain.ServiceType> _serviceTypeRepository;
        private readonly IRepository<RoyaltyFeeSlabs> _royaltyFeeSlabsRepository;
        private readonly IRepository<FranchiseePaymentProfile> _franchiseePaymentProfileRepository;
        private readonly IRepository<Application.Domain.File> _fileRepository;
        private readonly IRepository<LeadPerformanceFranchiseeDetails> _leadPerformanceFranchiseeDetailsRepository;
        private readonly IRepository<Address> _addressRepository;
        private readonly IFranchiseeFactory _franchiseeFactory;
        private readonly IFranchiseeServicesFactory _franchiseeServiceFactory;
        private readonly IOrganizationFactory _organizationFactory;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly IPhoneService _phoneService;
        private readonly IFeeProfileFactory _feeProfileFactory;
        private readonly ISortingHelper _sortingHelper;
        private IUserService _userService;
        private readonly IFranchiseeDocumentFactory _franchiseeDocumentFactory;

        private readonly IRepository<SalesDataUpload> _salesDataUploadRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IFranchiseeNotesFactory _franchiseeNotesFactory;
        private readonly IRepository<FranchiseeNotes> _franchiseeNotesRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IFranchiseeServiceFeeFactory _franchiseeServiceFeeFactory;
        private readonly IRepository<FranchiseeServiceFee> _franchiseeServiceFeeRepository;
        private readonly IRepository<FranchiseeDocumentType> _franchiseeDocumentTypeRepository;
        public readonly IClock _clock;
        private readonly IFileService _fileService;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IExcelTaxDocumentFileFormaterCreator _excelTaxDocumentCreator;
        private readonly ILeadPerformanceFranchiseeDetailsService _leadPerformanceFranchiseeDetailsService;
        private readonly IRepository<Country> _countryRepository;
        private readonly IExcelFranchiseeFileFormaterCreator _excelFileFormatterCreator;

        private readonly IRepository<ReviewPushAPILocation> _reviewPushApiLocationRepository;

        private readonly IRepository<FranchiseeRegistrationHistry> _franchiseeRegistrationHistryRepository;

        private readonly IRepository<OnetimeprojectfeeAddFundRoyality> _onetimeprojectfeeAddFundRoyalityRepository;
        private readonly IRepository<DocumentType> _documentTypeRepository;
        private readonly IRepository<Perpetuitydatehistry> _perpetuitydatehistryRepository;

        private readonly IRepository<MinRoyaltyFeeSlabs> _minRoyaltyFeeSlabsRepository;
        private readonly IRepository<FranchiseeDurationNotesHistry> _franchiseeDurationNotesHistryRepository;
        private readonly IRepository<FranchiseeTechMailEmail> _franchiseeTechMailEmailRepository;
        private readonly IRepository<ShiftCharges> _shiftChargesRepository;
        private readonly IRepository<MaintenanceCharges> _maintenanceChargesRepository;
        private readonly IRepository<ReplacementCharges> _replacementChargesRepository;
        private readonly IRepository<FloorGrindingAdjustment> _floorGrindingAdjustmentRepository;
        private readonly IRepository<PriceEstimateServices> _priceEstimateServicesRepository;
        public FranchiseeInfoService(IUnitOfWork unitOfWork, IFranchiseeFactory franchiseeFactory, IOrganizationFactory organizationFactory,
            IFranchiseeServicesFactory franchiseeServiceFactory, IPersonFactory personFactory, IUserService userService,
            IOrganizationRoleUserInfoService organizationRoleUserInfoService, IPhoneService phoneService, IFeeProfileFactory feeProfileFactory,
            ISortingHelper sortingHelper, IFranchiseeNotesFactory franchiseeNotesFactory, IFranchiseeServiceFeeFactory franchiseeServiceFeeFactory,
            IFranchiseeDocumentFactory franchiseeDocumentFactory, IClock clock, IFileService fileService, IExcelFileCreator excelFileCreator,
            IFranchiseeTechnicianMailFactory franchiseeTechnicianMailFactory, IFranchiseeLeadPerformanceFactory franchiseeLeadPerformanceFactory,
            ILeadPerformanceFranchiseeDetailsService leadPerformanceFranchiseeDetailsService, IExcelFranchiseeFileFormaterCreator excelFileFormatterCreator,
            IExcelTaxDocumentFileFormaterCreator excelTaxDocumentCreator)
        {
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _franchiseeServiceRepository = unitOfWork.Repository<FranchiseeService>();
            _feeProfileRepository = unitOfWork.Repository<FeeProfile>();
            _personRepository = unitOfWork.Repository<Person>();
            _serviceTypeRepository = unitOfWork.Repository<Domain.ServiceType>();
            _salesDataUploadRepository = unitOfWork.Repository<SalesDataUpload>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _royaltyFeeSlabsRepository = unitOfWork.Repository<RoyaltyFeeSlabs>();
            _franchiseePaymentProfileRepository = unitOfWork.Repository<FranchiseePaymentProfile>();
            _franchiseeNotesRepository = unitOfWork.Repository<FranchiseeNotes>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _leadPerformanceFranchiseeDetailsRepository = unitOfWork.Repository<LeadPerformanceFranchiseeDetails>();
            _franchiseeFactory = franchiseeFactory;
            _franchiseeServiceFactory = franchiseeServiceFactory;
            _organizationFactory = organizationFactory;
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _userService = userService;
            _phoneService = phoneService;
            _feeProfileFactory = feeProfileFactory;
            _sortingHelper = sortingHelper;
            _franchiseeNotesFactory = franchiseeNotesFactory;
            _franchiseeServiceFeeFactory = franchiseeServiceFeeFactory;
            _franchiseeServiceFeeRepository = unitOfWork.Repository<FranchiseeServiceFee>();
            _franchiseeDocumentFactory = franchiseeDocumentFactory;
            _franchiseeDocumentTypeRepository = unitOfWork.Repository<FranchiseeDocumentType>();
            _fileRepository = unitOfWork.Repository<Application.Domain.File>();
            _clock = clock;
            _fileService = fileService;
            _excelFileCreator = excelFileCreator;
            _franchiseeTechnicianMailFactory = franchiseeTechnicianMailFactory;
            _franchiseeTechMailServiceRepository = unitOfWork.Repository<FranchiseeTechMailService>();
            _franchiseeLeadPerformanceFactory = franchiseeLeadPerformanceFactory;
            _leadPerformanceFranchiseeDetailsService = leadPerformanceFranchiseeDetailsService;
            _addressRepository = unitOfWork.Repository<Address>();
            _countryRepository = unitOfWork.Repository<Country>();
            _excelFileFormatterCreator = excelFileFormatterCreator;
            _reviewPushApiLocationRepository = unitOfWork.Repository<ReviewPushAPILocation>();
            _onetimeprojectfeeAddFundRoyalityRepository = unitOfWork.Repository<OnetimeprojectfeeAddFundRoyality>();
            _franchiseeDocumentRepository = unitOfWork.Repository<FranchiseDocument>();
            _franchiseeRegistrationHistryRepository = unitOfWork.Repository<FranchiseeRegistrationHistry>();
            _documentTypeRepository = unitOfWork.Repository<DocumentType>();
            _minRoyaltyFeeSlabsRepository = unitOfWork.Repository<MinRoyaltyFeeSlabs>();
            _perpetuitydatehistryRepository = unitOfWork.Repository<Perpetuitydatehistry>();
            _franchiseeDurationNotesHistryRepository = unitOfWork.Repository<FranchiseeDurationNotesHistry>();
            _excelTaxDocumentCreator = excelTaxDocumentCreator;
            _franchiseeTechMailEmailRepository = unitOfWork.Repository<FranchiseeTechMailEmail>();
            _shiftChargesRepository = unitOfWork.Repository<ShiftCharges>();
            _maintenanceChargesRepository = unitOfWork.Repository<MaintenanceCharges>();
            _replacementChargesRepository = unitOfWork.Repository<ReplacementCharges>();
            _floorGrindingAdjustmentRepository = unitOfWork.Repository<FloorGrindingAdjustment>();
            _priceEstimateServicesRepository = unitOfWork.Repository<PriceEstimateServices>();
        }

        public bool Delete(long franchiseeId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null) return false;

            var salesDataUpload = _salesDataUploadRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.IsActive).ToList();
            if (salesDataUpload.Any())
                return false;

            var feeProfile = _feeProfileRepository.Get(franchiseeId);
            if (feeProfile != null)
            {
                var royaltyFeeProflie = _royaltyFeeSlabsRepository.Fetch(x => x.RoyaltyFeeProfileId == feeProfile.Id).ToArray();
                if (royaltyFeeProflie.Any())
                    foreach (var royaltyFee in royaltyFeeProflie)
                    {
                        _royaltyFeeSlabsRepository.Delete(royaltyFee);
                    }
                _feeProfileRepository.Delete(feeProfile);
            }

            var franchiseeService = _franchiseeServiceRepository.Fetch(x => x.FranchiseeId == franchiseeId).ToArray();
            foreach (var item in franchiseeService)
            {
                _franchiseeServiceRepository.Delete(item);
            }

            var franchiseeServiceFee = _franchiseeServiceFeeRepository.Fetch(x => x.FranchiseeId == franchiseeId).ToArray();
            foreach (var serviceFee in franchiseeServiceFee)
            {
                _franchiseeServiceFeeRepository.Delete(serviceFee);
            }

            var franchiseePaymentProfile = _franchiseePaymentProfileRepository.Get(franchiseeId);
            if (franchiseePaymentProfile != null)
                _franchiseePaymentProfileRepository.Delete(franchiseePaymentProfile);

            var result = _organizationRoleUserInfoService.DeleteOruOfFranchisee(franchiseeId);
            if (result)
            {
                _franchiseeRepository.Delete(franchiseeId);
                _organizationRepository.Delete(franchiseeId);
                return true;
            }
            return false;
        }

        public FranchiseeEditModel Get(long franchiseeId)
        {
            if (franchiseeId == 0)
            {
                var editModel = new FranchiseeEditModel()
                {
                    PhoneNumbers = _phoneService.GetDefaultPhoneModel(),
                    FranchiseeServices = _franchiseeServiceFactory.CreateEditModel(null),
                    ServiceFees = _franchiseeServiceFeeFactory.CreateEditModel(null),
                    Documents = _franchiseeDocumentFactory.CreateEditModelForDocument(null),
                    FranchiseeEmailEditModel = _franchiseeTechnicianMailFactory.CreateEditModel(null),
                    LeadPerformanceEditModel = _franchiseeLeadPerformanceFactory.CreateEditModel(null),
                    Is0Franchisee = false,
                    WebSite = "",
                    ReviewRpId = default(long?),
                    IsMinRoyalityFixed = false,
                    RegistrationDate = null,
                    NotesFromOwner = "",
                    NotesFromCallCenter = "",
                    Duration = 1,
                    LessDeposit = 50,
                    LanguageId = 249,
                    franchiseeTechMailServiceEditModel = _franchiseeTechnicianMailFactory.CreateEditModel(null)

                };
                editModel.FeeProfile.MinRoyalitySlabs = CreateEmptyData();
                return editModel;
            };


            var franchisee = _franchiseeRepository.Get(franchiseeId);

            var orgModel = _organizationRepository.Get(franchiseeId);
            var orgRoleUser = _organizationRoleUserInfoService.GetPrimaryContractOrganizationRoleUserByOrganizationId(orgModel.Id);
            var personModel = (orgRoleUser != null) ? _personRepository.Get(orgRoleUser.UserId) : new Person();

            var franchiseeEditModel = _franchiseeFactory.CreateEditModel(franchisee, personModel);
            franchisee.IsMinRoyalityFixed = false;
            franchiseeEditModel.LessDeposit = franchisee.LessDeposit;


            var franchiseeTechMailService = _franchiseeTechMailServiceRepository.Table.FirstOrDefault(x => x.FranchiseeId == franchiseeId);
            var franchiseeTechMailEmail = _franchiseeTechMailEmailRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.FranchiseeId == franchiseeId);
            var franchiseeTechMailServiceEditModel = _franchiseeTechnicianMailFactory.CreateEditModel(franchiseeTechMailService);

            if (franchiseeTechMailEmail != null)
            {
                franchiseeTechMailServiceEditModel.ChargesForPhone = franchiseeTechMailEmail.ChargesForPhone;
            }
            else
            {
                franchiseeTechMailServiceEditModel.ChargesForPhone = 15;
            }
            franchiseeEditModel.IsActive = orgModel.IsActive;
            franchiseeEditModel.FranchiseeEmailEditModel = franchiseeTechMailServiceEditModel;
            franchiseeEditModel.DeactivationNote = orgModel.DeactivationnNote;
            if (franchiseeEditModel.PhoneNumbers.Count() < 1)
                franchiseeEditModel.PhoneNumbers = _phoneService.GetDefaultPhoneModel();

            var leadPerformaceDomain = _leadPerformanceFranchiseeDetailsRepository.Table.Where(x => x.FranchiseeId == franchiseeId)
                .OrderByDescending(x => x.Id).ToList();

            if (leadPerformaceDomain.Count() > 0)
            {
                var leadPerformanceDomainForCategory = leadPerformaceDomain.OrderByDescending(x => x.Id).FirstOrDefault(x => x.CategoryId == (long)LeadPerformanceEnum.PPCSPEND && x.IsActive);
                if (leadPerformanceDomainForCategory != null)
                {
                    franchiseeEditModel.LeadPerformanceEditModel.PpcSpend = GetAmounForLeadPerformance(leadPerformanceDomainForCategory);
                }
                else
                {
                    franchiseeEditModel.LeadPerformanceEditModel.PpcSpend = "295";
                }
                leadPerformanceDomainForCategory = leadPerformaceDomain.OrderByDescending(x => x.Id).FirstOrDefault(x => x.CategoryId == (long)LeadPerformanceEnum.SEOCOST);


                if (leadPerformanceDomainForCategory != null)
                {
                    franchiseeEditModel.LeadPerformanceEditModel.SeoCost = GetAmounForLeadPerformance(leadPerformanceDomainForCategory);
                    franchiseeEditModel.LeadPerformanceEditModel.SeoCostOriginal = GetAmounForLeadPerformance(leadPerformanceDomainForCategory);
                    franchiseeEditModel.LeadPerformanceEditModel.SeoCostBillingPeriodId = leadPerformanceDomainForCategory.week != null ? leadPerformanceDomainForCategory.week.GetValueOrDefault() : 1;
                    franchiseeEditModel.LeadPerformanceEditModel.SeoCostBillingPeriodIdOriginal = leadPerformanceDomainForCategory.week != null ? leadPerformanceDomainForCategory.week.GetValueOrDefault() : 1;
                }
                else
                {
                    franchiseeEditModel.LeadPerformanceEditModel.SeoCost = "0";
                }
            }
            else
            {
                franchiseeEditModel.LeadPerformanceEditModel = _franchiseeLeadPerformanceFactory.CreateEditModel(null);
            }
            franchiseeEditModel.IsMinRoyalityFixed = franchisee.IsMinRoyalityFixed;
            franchiseeEditModel.RegistrationDate = franchisee.RegistrationDate != null ? franchisee.RegistrationDate.GetValueOrDefault().AddDays(1) : default(DateTime?);

            if (franchiseeEditModel.RegistrationDate != null)
            {
                franchiseeEditModel.FeeProfile.MinimumRoyaltyPerMonth = GetMinimumRoyalityForFranchisee(franchisee.Id);
            }
            else
            {
                franchiseeEditModel.FeeProfile.MinimumRoyaltyPerMonth = 0;
            }
            franchiseeEditModel.FranchisieeHistryViewModel = GetRegistrationHistryViewModel(franchisee.Id);
            franchiseeEditModel.RenewalDate = franchiseeEditModel.RenewalDate != null ? franchiseeEditModel.RenewalDate.Value.AddDays(1) : default(DateTime?);

            var franchiseeText = _franchiseeNotesRepository.Table.OrderByDescending(x => x.Id).Where(x => x.FranchiseeId == franchiseeEditModel.Id).ToList();
            if (franchiseeText.Count > 0)
            {
                franchiseeEditModel.Text = franchiseeText.FirstOrDefault().Text;
                franchiseeEditModel.HasNotes = true;
            }

            return franchiseeEditModel;
        }

        public void Save(FranchiseeEditModel franchiseeEditModel)
        {
            if (franchiseeEditModel.FileUploadModel != null && franchiseeEditModel.FileUploadModel.FileList.Count() > 0)
            {
                franchiseeEditModel.FileUploadModel.css = franchiseeEditModel.Css;
                long fileId = SavesImage(franchiseeEditModel.FileUploadModel);
                franchiseeEditModel.FileId = fileId;
            }
            else
            {
                var fileId = _franchiseeRepository.Fetch(x => x.Id == franchiseeEditModel.Id).Select(x => x.FileId).FirstOrDefault();
                if (franchiseeEditModel.IsImageChanged == false)
                {
                    franchiseeEditModel.FileId = fileId;
                    var file = _fileRepository.Get(fileId.GetValueOrDefault());
                    if (file != null && file.css != franchiseeEditModel.Css)
                    {
                        file.css = franchiseeEditModel.Css;
                        file.IsNew = false;
                        _fileRepository.Save(file);
                    }
                }
                else
                {
                    franchiseeEditModel.FileId = null;
                }
            }
            var isNewFranchisee = false;
            var franchisee = franchiseeEditModel.Id > 0 ? _franchiseeRepository.Get(franchiseeEditModel.Id) : null;
            var organizationinDb = franchiseeEditModel.Id > 0 ? _organizationRepository.Get(franchiseeEditModel.Id) : null;
            franchisee = _franchiseeFactory.CreateDomain(franchiseeEditModel, franchisee);
            franchisee.LessDeposit = franchiseeEditModel.LessDeposit;
            franchisee.IsRoyality = franchiseeEditModel.IsRoyality;
            var organization = _franchiseeFactory.CreateOrgDomain(franchiseeEditModel);
            organization.DeactivationnNote = franchiseeEditModel.DeactivationNote;
            if (organization.Id < 1)
            {
                franchisee.IsNew = true;
                organization.IsActive = true;
                organization.DeactivationnNote = null;
                isNewFranchisee = true;
                franchisee.RegistrationNumber = franchiseeEditModel.RegistrationNumber;
                franchisee.SalesTax = Convert.ToDecimal(franchiseeEditModel.Taxrate);

            }
            else
            {
                if (franchisee.RegistrationDate.GetValueOrDefault().Date != franchiseeEditModel.RegistrationDate.GetValueOrDefault().Date)
                {
                    CreateRegistrationHistry(franchiseeEditModel);
                }

            }
            franchisee.RegistrationNumber = franchiseeEditModel.RegistrationNumber;
            franchisee.SalesTax = Convert.ToDecimal(franchiseeEditModel.Taxrate);
            franchisee.IsMinRoyalityFixed = franchiseeEditModel.IsMinRoyalityFixed;

            organization.Franchisee = franchisee;
            if (franchisee != null && organizationinDb != null)
            {
                organization.IsActive = organizationinDb.IsActive;

            }
            organization.Franchisee.RegistrationDate = franchiseeEditModel.RegistrationDate != null ? franchiseeEditModel.RegistrationDate.GetValueOrDefault().AddDays(-1) : default(DateTime?);

            _organizationRepository.Save(organization);

            var minRoyalityInDb = _minRoyaltyFeeSlabsRepository.Table.Where(x => x.FranchiseeId == franchisee.Id).ToList();
            changingMinRoyalityService(franchiseeEditModel.FeeProfile.MinRoyalitySlabs, franchisee.Id, minRoyalityInDb);

            if (isNewFranchisee)
            {
                franchiseeEditModel.Id = organization.Id;
                CreateRegistrationHistry(franchiseeEditModel);
            }
            var person = _userService.Save(franchiseeEditModel, organization);

            franchiseeEditModel.OrganizationOwner.OwnerId = person.Id;

            franchisee.Id = organization.Id;

            if (franchiseeEditModel.Text != null)
            {
                var franchiseeText = _franchiseeNotesRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.FranchiseeId == franchiseeEditModel.Id);
                if (franchiseeText != null)
                {
                    if (!franchiseeText.Text.Equals(franchiseeEditModel.Text))
                    {
                        var notes = _franchiseeNotesFactory.CreateDomain(franchisee.Id, franchiseeEditModel);
                        _franchiseeNotesRepository.Save(notes);
                    }
                }
                else
                {
                    var notes = _franchiseeNotesFactory.CreateDomain(franchisee.Id, franchiseeEditModel);
                    _franchiseeNotesRepository.Save(notes);
                }
            }

            var franchiseeServiceFee = _franchiseeServiceFeeFactory.CreateDomain(franchiseeEditModel.ServiceFees, franchisee).ToList();
            foreach (var serviceFee in franchiseeServiceFee)
            {
                serviceFee.FranchiseeId = franchisee.Id;
                _franchiseeServiceFeeRepository.Save(serviceFee);
            }
            var franchiseeDocument = _franchiseeDocumentFactory.CreateDocumentDomain(franchiseeEditModel.Documents, franchisee.Id);
            bool isNew = franchisee.FranchiseeDocumentType.Count() <= 0;
            if (franchiseeEditModel.FranchiseeEmailEditModel.isTechMailFees
                || franchiseeEditModel.FranchiseeEmailEditModel.IsGeneric)
            {
                var franchiseeTechMail = _franchiseeFactory.CreateFranchiseeTechMailDomain(franchiseeEditModel);
                franchiseeTechMail.FranchiseeId = franchisee.Id;
                franchiseeTechMail.IsNew = franchiseeEditModel.FranchiseeEmailEditModel.Id > 0 ? false : true;
                _franchiseeTechMailServiceRepository.Save(franchiseeTechMail);
            }
            else
            {
                var franchiseeTechMailDomain = _franchiseeTechMailServiceRepository.Table.FirstOrDefault(x => x.FranchiseeId == franchisee.Id);
                if (franchiseeTechMailDomain != null)
                    _franchiseeTechMailServiceRepository.Delete(franchiseeTechMailDomain);
            }

            var franchiseeTechMailServiceDomain = _franchiseeTechMailEmailRepository.Table.FirstOrDefault(x => x.FranchiseeId == franchisee.Id);
            if (franchiseeTechMailServiceDomain != null)
            {
                if (franchiseeTechMailServiceDomain.ChargesForPhone != franchiseeEditModel.FranchiseeEmailEditModel.ChargesForPhone)
                {
                    var franchiseeTechMailDomain = new FranchiseeTechMailEmail()
                    {
                        Id = 0,
                        FranchiseeId = franchisee.Id,
                        ChargesForPhone = franchiseeEditModel.FranchiseeEmailEditModel.ChargesForPhone,
                        IsNew = true,
                        UserId = franchiseeEditModel.UserId,
                        DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                        DateForCharges = _clock.ToUtc(DateTime.Now.Date).Date,
                        CallCount = 0,
                    };
                    _franchiseeTechMailEmailRepository.Save(franchiseeTechMailDomain);
                }
            }
            else
            {
                var franchiseeTechMailDomain = new FranchiseeTechMailEmail()
                {
                    Id = 0,
                    FranchiseeId = franchisee.Id,
                    ChargesForPhone = franchiseeEditModel.FranchiseeEmailEditModel.ChargesForPhone,
                    IsNew = true,
                    UserId = franchiseeEditModel.UserId,
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                    DateForCharges = DateTime.UtcNow,
                    CallCount = 0,
                };
                _franchiseeTechMailEmailRepository.Save(franchiseeTechMailDomain);
            }
            if (isNewFranchisee)
            {
                franchiseeEditModel.Id = organization.Id;
                var franchiseeReSalesDocument = franchiseeDocument.FirstOrDefault(x => x.DocumentTypeId == 11);
                if (franchiseeReSalesDocument == null)
                {
                    franchiseeReSalesDocument = franchiseeDocument.FirstOrDefault(x => x.DocumentType.Id == 11);
                }
                CreatePerpetuityHistry(franchiseeEditModel, franchiseeReSalesDocument.IsPerpetuity);
                
            }
            else
            {
                var franchiseeReSalesDocument = franchiseeDocument.FirstOrDefault(x => x.DocumentTypeId == 11);
                var fromDomain = _perpetuitydatehistryRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.FranchiseeId == franchiseeEditModel.Id);
                if (fromDomain != null)
                {
                    if (franchiseeReSalesDocument.IsPerpetuity != fromDomain.IsPerpetuity)
                    {
                        franchiseeEditModel.Id = organization.Id;
                        CreatePerpetuityHistry(franchiseeEditModel, franchiseeReSalesDocument.IsPerpetuity);
                    }
                }
                else
                {
                    CreatePerpetuityHistry(franchiseeEditModel, franchiseeReSalesDocument.IsPerpetuity);
                }
            }
            foreach (var serviceFee in franchiseeDocument)
            {
                _franchiseeDocumentTypeRepository.Save(serviceFee);
            }
            if (franchiseeEditModel.IsSEOActive != franchiseeEditModel.LeadPerformanceEditModel.IsSEOActiveOriginal || franchiseeEditModel.LeadPerformanceEditModel.SeoCostBillingPeriodId != franchiseeEditModel.LeadPerformanceEditModel.SeoCostBillingPeriodIdOriginal || franchiseeEditModel.LeadPerformanceEditModel.SeoCost != franchiseeEditModel.LeadPerformanceEditModel.SeoCostOriginal)
            {
                franchiseeEditModel.LeadPerformanceEditModel.SeoCostBillingPeriodId = franchiseeEditModel.SeoCostBillingPeriodId;
                franchiseeEditModel.LeadPerformanceEditModel.IsSEOActive = franchiseeEditModel.IsSEOActive;
                _leadPerformanceFranchiseeDetailsService.Save(franchiseeEditModel.LeadPerformanceEditModel, franchisee.Id);

                SaveFrnchiseeServiceForSEO(franchiseeEditModel.LeadPerformanceEditModel, franchisee.Id);
            }

            SaveTimeEstimateForFranchisee(franchisee.Id);
            var pricesForFranchisee = _priceEstimateServicesRepository.Table.Where(x => x.FranchiseeId == franchisee.Id).ToList();
            if (pricesForFranchisee.Count < 1)
            {
                CreatePriceEstimate(franchisee.Id);
            }
            SaveReviewURL(franchiseeEditModel);
        }

        private bool SaveFrnchiseeServiceForSEO(LeadPerformanceEditModel model, long franchiseeId)
        {

            var franchiseeServiceFeeForSeo = _franchiseeServiceFeeRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.FranchiseeId == franchiseeId && x.ServiceFeeTypeId == (long)ServiceFeeType.SEOCharges && x.IsActive);
            if (franchiseeServiceFeeForSeo != null)
            {
                franchiseeServiceFeeForSeo.IsActive = false;
                _franchiseeServiceFeeRepository.Save(franchiseeServiceFeeForSeo);

                var franchiseServiceFee = new FranchiseeServiceFee()
                {
                    FranchiseeId = franchiseeId,
                    ServiceFeeTypeId = (long)ServiceFeeType.SEOCharges,
                    Amount = Convert.ToDecimal(model.SeoCost),
                    Percentage = Convert.ToDecimal(0.00),
                    FrequencyId = model.SeoCostBillingPeriodId == 1 ? (long)PaymentFrequency.FirstWeek : (long)PaymentFrequency.SecondWeek,
                    IsActive = true,
                    IsNew = true,
                    SaveDateForSeoCost = _clock.ToUtc(DateTime.Now),
                    InvoiceDateForSeoCost = null
                };
                _franchiseeServiceFeeRepository.Save(franchiseServiceFee);
            }
            else
            {
                var franchiseServiceFee = new FranchiseeServiceFee()
                {
                    FranchiseeId = franchiseeId,
                    ServiceFeeTypeId = (long)ServiceFeeType.SEOCharges,
                    Amount = Convert.ToDecimal(model.SeoCost),
                    Percentage = Convert.ToDecimal(0.00),
                    FrequencyId = model.SeoCostBillingPeriodId == 1 ? (long)PaymentFrequency.FirstWeek : (long)PaymentFrequency.SecondWeek,
                    IsActive = true,
                    IsNew = true,
                    SaveDateForSeoCost = _clock.ToUtc(DateTime.Now),
                    InvoiceDateForSeoCost = null
                };
                _franchiseeServiceFeeRepository.Save(franchiseServiceFee);
            }
            return true;

        }
        public long SavesImage(FileUploadModel model)
        {
            foreach (var fileModel in model.FileList)
            {
                try
                {
                    if (fileModel.Id > 0)
                    {
                        var fileRepository = _fileRepository.Get(fileModel.Id);
                        fileRepository.IsNew = false;
                        fileRepository.css = model.css;
                        _fileRepository.Save(fileRepository);
                        continue;
                    }
                    var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
                    var destination = MediaLocationHelper.GetTempImageLocation();
                    var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                        : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
                    var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
                    fileModel.Name = destFileName + fileModel.Extension;

                    fileModel.RelativeLocation = MediaLocationHelper.GetTempImageLocation().Path;

                    string folderName = Path.GetFileName(fileModel.RelativeLocation);
                    fileModel.css = model.css;
                    fileModel.RelativeLocation = "\\" + folderName;
                    var file = _fileService.SaveModel(fileModel);

                    return file.Id;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return default(long);
        }
        public FranchiseeListModel GetFranchiseeCollection(FranchiseeListFilter filter, int pageNumber, int pageSize)
        {
            var franchiseeCollection = _franchiseeRepository.Table.Where(x => ((filter.FranchiseeId < 1 || filter.FranchiseeId == null
            ) || x.Id == filter.FranchiseeId)
            && (string.IsNullOrEmpty(filter.Email) || (x.Organization.Email.Contains(filter.Email)))
            && (string.IsNullOrEmpty(filter.Franchisee) || (x.Organization.Name.Contains(filter.Franchisee)))
            && (filter.status == null || (filter.status == false ? !x.Organization.IsActive : x.Organization.IsActive))
            && (string.IsNullOrEmpty(filter.Text)
            || (x.OwnerName.Contains(filter.Text))
            || (x.Organization.Address.FirstOrDefault().Country.Name).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().StateName).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().CityName).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().AddressLine1).Contains(filter.Text)
            || (x.Organization.Phones.FirstOrDefault().Number.Contains(filter.Text))
            || (x.Organization.Address.FirstOrDefault().AddressLine2).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().City.Name).Contains(filter.Text) || (x.Organization.Address.FirstOrDefault().State.Name).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().Zip.Code).Contains(filter.Text)));




            franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Name, (long)SortingOrder.Asc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Account":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Id, filter.SortingOrder);
                        break;
                    case "Name":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Name, filter.SortingOrder);
                        break;
                    case "Email":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Email, filter.SortingOrder);
                        break;
                    case "PrimaryContact":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.OwnerName, filter.SortingOrder);
                        break;
                    case "StreetAddress":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().AddressLine1, filter.SortingOrder);
                        break;
                    case "City":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().City.Name, filter.SortingOrder);
                        break;
                    case "State":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().State.Name, filter.SortingOrder);
                        break;
                    case "ZipCode":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().Zip.Code, filter.SortingOrder);
                        break;
                    case "Country":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().Country.Name, filter.SortingOrder);
                        break;
                    case "AccountCredit":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.FranchiseeAccountCredit.Sum(y => y.Amount), filter.SortingOrder);
                        break;
                    case "BusinessId":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.BusinessId, filter.SortingOrder);
                        break;
                }
            }


            var finalcollection = pageNumber != 0 ? franchiseeCollection.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList() : franchiseeCollection.ToList();

            var franchiseeDurationApproval = _franchiseeDurationNotesHistryRepository.Table.Where(x => x.StatusId == (long)AuditActionType.Pending && x.RoleId != (long)RoleType.SuperAdmin).ToList();
            return new FranchiseeListModel
            {
                Collection = finalcollection.Select(x => _franchiseeFactory.CreateViewModel(x, franchiseeDurationApproval.Where(x1 => x.Id == x1.FranchiseeId).ToList())).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, franchiseeCollection.Count())
            };
        }
        private List<FranchiseeViewModelForDownload> GetFranchiseeCollectionForDownload(FranchiseeListFilter filter)
        {
            var franchiseeCollection = _franchiseeRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.Id == filter.FranchiseeId)
            && (string.IsNullOrEmpty(filter.Email) || (x.Organization.Email.Contains(filter.Email)))
            && (string.IsNullOrEmpty(filter.Franchisee) || (x.Organization.Name.Contains(filter.Franchisee)))
            && (filter.status == null || (filter.status == false ? !x.Organization.IsActive : x.Organization.IsActive))
            && (string.IsNullOrEmpty(filter.Text)
            || (x.OwnerName.Contains(filter.Text))
            || (x.Organization.Address.FirstOrDefault().AddressLine1).Contains(filter.Text)
            || (x.Organization.Phones.FirstOrDefault().Number.Contains(filter.Text))
            || (x.Organization.Address.FirstOrDefault().AddressLine2).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().City.Name).Contains(filter.Text) || (x.Organization.Address.FirstOrDefault().State.Name).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().Zip.Code).Contains(filter.Text)));

            //if (filter.status != null)
            //{
            //    franchiseeCollection = franchiseeCollection.Where(x => x.Organization.IsActive == filter.status);
            //}

            franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Name, (long)SortingOrder.Asc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Account":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Id, filter.SortingOrder);
                        break;
                    case "Name":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Name, filter.SortingOrder);
                        break;
                    case "Email":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Email, filter.SortingOrder);
                        break;
                    case "PrimaryContact":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.OwnerName, filter.SortingOrder);
                        break;
                    case "StreetAddress":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().AddressLine1, filter.SortingOrder);
                        break;
                    case "City":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().City.Name, filter.SortingOrder);
                        break;
                    case "State":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().State.Name, filter.SortingOrder);
                        break;
                    case "ZipCode":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().Zip.Code, filter.SortingOrder);
                        break;
                    case "Country":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().Country.Name, filter.SortingOrder);
                        break;
                    case "AccountCredit":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.FranchiseeAccountCredit.Sum(y => y.Amount), filter.SortingOrder);
                        break;
                    case "BusinessId":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.BusinessId, filter.SortingOrder);
                        break;
                }
            }


            var finalcollection = franchiseeCollection.ToList();

            return finalcollection.Select(_franchiseeFactory.CreateViewModelForDownload).ToList();
        }

        public FeeProfileViewModel GetFranchiseeFeeProfile(long franchiseeId)
        {
            var feeProfile = _feeProfileRepository.Get(x => x.Franchisee.Id == franchiseeId);

            return _feeProfileFactory.CreateViewModel(feeProfile);
        }

        public IEnumerable<FranchiseeNotesViewModel> GetFranchiseeNotes(long franchiseeId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            var franchiseeNotesList = franchisee.FranchiseeNotes.Select(x => _franchiseeNotesFactory.CreateViewModel(x)).OrderByDescending(x => x.CreatedOn).ToList();
            return franchiseeNotesList;
        }

        public bool DeactivateFranchisee(long franchiseeId, string deactivationNote)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null)
                return false;
            var users = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(franchisee.Organization.Id);
            if (users.Any())
            {
                foreach (var user in users)
                {
                    user.IsActive = false;
                    _organizationRoleUserRepository.Save(user);
                    if (users.Count == 1 && (user.RoleId == (long)RoleType.FranchiseeAdmin || user.RoleId == (long)RoleType.SuperAdmin))
                    {
                        var userLogin = _userLoginRepository.Get(user.UserId);
                        userLogin.IsActive = false;
                        _userLoginRepository.Save(userLogin);
                    }
                }
            }
            franchisee.Organization.IsActive = false;
            franchisee.Organization.DeactivationnNote = deactivationNote;
            _organizationRepository.Save(franchisee.Organization);
            return true;
        }

        public bool ActivateFranchisee(long franchiseeId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null)
                return false;
            var users = _organizationRoleUserInfoService.GetOrganizationRoleUserByOrganizationId(franchisee.Organization.Id);
            if (users.Any())
            {
                foreach (var user in users)
                {
                    user.IsActive = true;
                    _organizationRoleUserRepository.Save(user);
                    if (user.RoleId == (long)RoleType.FranchiseeAdmin || user.RoleId == (long)RoleType.SuperAdmin)
                    {
                        var userLogin = _userLoginRepository.Get(user.UserId);
                        if (userLogin != null)
                        {
                            userLogin.IsActive = true;
                            _userLoginRepository.Save(userLogin);
                        }
                    }
                }
            }
            franchisee.Organization.IsActive = true;
            franchisee.Organization.DeactivationnNote = null;
            _organizationRepository.Save(franchisee.Organization);
            return true;
        }

        public bool IsUniqueBusinessId(long? businessId, long id = 0)
        {
            if (businessId == null) return true;
            var result = !_franchiseeRepository.Table.Any(p => businessId == p.BusinessId && (id < 1 || id != p.Id));
            return result;
        }

        public bool GetGeoCode(long franchiseeId)
        {
            var result = false;
            if (franchiseeId <= 0)
                result = true;
            else
            {
                var franchisee = _franchiseeRepository.Get(franchiseeId);
                if (franchisee != null)
                    result = franchisee.SetGeoCode;
            }
            return result;
        }
        public bool DownloadFranchisee(FranchiseeListFilter filter, out string fileName)
        {
            var data = GetFranchiseeCollectionForDownload(filter);

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/franchiseeLoan-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(data, fileName);
        }
        public bool DownloadFranchiseeDirectory(FranchiseeListFilter filter, out string fileName)
        {
            var data = GetFranchiseeDirectoryCollectionForDownload(filter);

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/franchiseeLoan-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(data, fileName);
            return false;
        }

        private List<FranchiseeViewModelForFranchiseeDirectoryDownload> GetFranchiseeDirectoryCollectionForDownload(FranchiseeListFilter filter)
        {
            var franchiseeCollection = _franchiseeRepository.Table.Where(x => (filter.FranchiseeId < 1 || x.Id == filter.FranchiseeId)
            && (string.IsNullOrEmpty(filter.Email) || (x.Organization.Email.Contains(filter.Email)))
            && (string.IsNullOrEmpty(filter.Franchisee) || (x.Organization.Name.Contains(filter.Franchisee)))
            && (filter.status == null || (filter.status == false ? !x.Organization.IsActive : x.Organization.IsActive))
            && (string.IsNullOrEmpty(filter.Text)
            || (x.OwnerName.Contains(filter.Text))
            || (x.Organization.Address.FirstOrDefault().AddressLine1).Contains(filter.Text)
            || (x.Organization.Phones.FirstOrDefault().Number.Contains(filter.Text))
            || (x.Organization.Address.FirstOrDefault().AddressLine2).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().City.Name).Contains(filter.Text) || (x.Organization.Address.FirstOrDefault().State.Name).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().Zip.Code).Contains(filter.Text)));

            //if (filter.status != null)
            //{
            //    franchiseeCollection = franchiseeCollection.Where(x => x.Organization.IsActive == filter.status);
            //}

            franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Name, (long)SortingOrder.Asc);

            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Account":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Id, filter.SortingOrder);
                        break;
                    case "Name":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Name, filter.SortingOrder);
                        break;
                    case "Email":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Email, filter.SortingOrder);
                        break;
                    case "PrimaryContact":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.OwnerName, filter.SortingOrder);
                        break;
                    case "StreetAddress":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().AddressLine1, filter.SortingOrder);
                        break;
                    case "City":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().City.Name, filter.SortingOrder);
                        break;
                    case "State":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().State.Name, filter.SortingOrder);
                        break;
                    case "ZipCode":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().Zip.Code, filter.SortingOrder);
                        break;
                    case "Country":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.Organization.Address.FirstOrDefault().Country.Name, filter.SortingOrder);
                        break;
                    case "AccountCredit":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.FranchiseeAccountCredit.Sum(y => y.Amount), filter.SortingOrder);
                        break;
                    case "BusinessId":
                        franchiseeCollection = _sortingHelper.ApplySorting(franchiseeCollection, x => x.BusinessId, filter.SortingOrder);
                        break;
                }
            }



            var finalcollection = franchiseeCollection.ToList();
            return finalcollection.Select(_franchiseeFactory.CreateViewModelForFranchiseeDirectoryDownload).ToList();
        }
        private string GetAmounForLeadPerformance(LeadPerformanceFranchiseeDetails leadPerformanceFranchiseeDetails)
        {
            return leadPerformanceFranchiseeDetails.Amount.ToString(); ;
        }

        public FranchiseeResignListModel GetFranchiseeResignList(FranchiseeListFilter filter)
        {
            FranchiseeModel model = new FranchiseeModel();
            List<FranchiseeModel> collection = new List<FranchiseeModel>();
            List<FranchiseeRedesignViewModel> foriegnCountryCollection = new List<FranchiseeRedesignViewModel>();

            var franchiseeCollection = _franchiseeRepository.Table.Where(x => (filter.FranchiseeId == null || filter.FranchiseeId < 1 || x.Id == filter.FranchiseeId)
           && (string.IsNullOrEmpty(filter.Email) || (x.Organization.Email.Contains(filter.Email)))
           && (string.IsNullOrEmpty(filter.Franchisee) || (x.Organization.Name.Contains(filter.Franchisee)))
           && (filter.status == null || (filter.status == false ? !x.Organization.IsActive : x.Organization.IsActive))
           && (filter.RoleId == (long?)RoleType.SuperAdmin ? (filter.status == false ? !x.Organization.IsActive : x.Organization.IsActive) : x.Organization.IsActive)
           && (string.IsNullOrEmpty(filter.Text)
           || (x.OwnerName.Contains(filter.Text))
            || (x.Organization.Address.FirstOrDefault().Country.Name).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().StateName).Contains(filter.Text)
            || (x.Organization.Address.FirstOrDefault().CityName).Contains(filter.Text)
           || (x.Organization.Address.FirstOrDefault().AddressLine1).Contains(filter.Text)
           || (x.Organization.Phones.FirstOrDefault().Number.Contains(filter.Text))
           || (x.Organization.Address.FirstOrDefault().AddressLine2).Contains(filter.Text)
           || (x.Organization.Address.FirstOrDefault().City.Name).Contains(filter.Text) || (x.Organization.Address.FirstOrDefault().State.Name).Contains(filter.Text)
           || (x.Organization.Address.FirstOrDefault().Zip.Code).Contains(filter.Text))).ToList();


            var address = _addressRepository.Table.Select(x => x.Country).ToList();
            var countryPair = address.Select(x => x.Id).Distinct().ToList();
            var franchiseeDurationApproval = _franchiseeDurationNotesHistryRepository.Table.Where(x => x.StatusId == (long)AuditActionType.Pending && x.RoleId != (long)RoleType.SuperAdmin).ToList();

            var orgUserIdDomainList = _organizationRoleUserRepository.Table.Where(x => x.UserId == filter.LoggedInUserId).ToList();
            var orgUserIdList = orgUserIdDomainList.Select(x => x.OrganizationId).ToList();

            if (filter.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                var franchiseeAdminUserIdDomainList = orgUserIdDomainList.Where(x => x.RoleId != (long)RoleType.SuperAdmin).ToList();
                //orgUserIdList= franchiseeAdminUserIdDomainList.Select(x => x.OrganizationId).ToList();
                var franchiseeAdminUserId = franchiseeAdminUserIdDomainList.Select(x => x.UserId).ToList();
                franchiseeDurationApproval = franchiseeDurationApproval.Where(x => !franchiseeAdminUserId.Contains(x.UserId)).ToList();
            }

            var franchiseeListViewModel = franchiseeCollection.Select(x => _franchiseeFactory.CreateResignViewModel(x, orgUserIdList, filter.RoleId, franchiseeDurationApproval.Where(x1 => x1.FranchiseeId == x.Organization.Id).ToList()));
            model.CategoryName = "CORPORATE OFFICES (0-PREFIX)";
            model.FranchiseeViewModel = GetFranchiseeByGroup("0", default(long), franchiseeListViewModel.ToList());
            model.FranchiseeViewModel = model.FranchiseeViewModel.OrderBy(x => x.State).ThenBy(x => x.NameWithout0Prefix).ToList();
            if (model.FranchiseeViewModel.Count() > 0)
                collection.Add(model);
            franchiseeListViewModel = franchiseeListViewModel.Where(x => !x.Name.StartsWith("0"));
            countryPair = countryPair.Distinct().ToList();
            foreach (var country in countryPair)
            {
                model = new FranchiseeModel();
                var countryDomain = _countryRepository.Get(country);
                model.CategoryName = countryDomain.Name.ToUpper();
                if (countryDomain.Name == "USA")
                {
                    model.CategoryName = "MARBLELIFE OFFICES - US";
                }
                model.FranchiseeViewModel = GetFranchiseeByGroup("", country, franchiseeListViewModel.ToList());


                if (countryDomain.Name != "USA" && countryDomain.Name.ToUpper() != "CANADA")
                {
                    model.FranchiseeViewModel = model.FranchiseeViewModel.AsEnumerable().OrderBy(x => x.Name).ToList();
                    foriegnCountryCollection.AddRange(model.FranchiseeViewModel);
                }
                else
                {
                    if (model.FranchiseeViewModel.Count() > 0)
                    {
                        var value = model.FranchiseeViewModel.OrderBy(x => x.Name).ThenBy(x => x.Name);
                        model.FranchiseeViewModel = value.ToList();
                        collection.Add(model);
                    }
                }
            }

            model.CategoryName = "FOREIGN OFFICES";
            model.FranchiseeViewModel = foriegnCountryCollection;

            if (foriegnCountryCollection.Count() > 0)
            {
                model.FranchiseeViewModel = model.FranchiseeViewModel.OrderBy(x => x.Name).ThenBy(x => x.FirstAlphaOfState).ToList();
                collection.Add(model);
            }
            return new FranchiseeResignListModel
            {
                Collection = collection,
                Filter = filter,
            };
        }

        public List<FranchiseeRedesignViewModel> GetFranchiseeByGroup(string filterBy, long countryId, List<FranchiseeRedesignViewModel> franchiseeList)
        {
            if (filterBy != "")
            {
                franchiseeList = franchiseeList.Where(x => x.Name.StartsWith(filterBy)).ToList();
                return franchiseeList.OrderBy(x => x.State).ToList(); ;
            }
            else if (countryId != default(long))
            {
                franchiseeList = franchiseeList.Where(x => x.CountryId == countryId).ToList();
                return franchiseeList.OrderBy(x => x.State).ToList();
            }
            return null;
        }

        public class CountryViewModel
        {
            public string CountryName { get; set; }
            public long CountryID { get; set; }
        }

        public bool DownloadFileFranchiseeDirectory(FranchiseeListFilter filter, out string fileName)
        {
            var data = GetFranchiseeDirectoryCollectionForDownload(filter);

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/franchiseeLoan-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(data, fileName);
        }
        public bool DownloadFileFranchiseeDirectoryRedesign(FranchiseeListFilter filter, out string fileName)
        {
            var data = GetFranchiseeResignList(filter);

            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "\\franchiseeDirectory-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileFormatterCreator.CreateExcelDocument(data.Collection, fileName);
        }
        public string GetFranchiseeDeactivationNote(long franchiseeId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            var franchiseeNotesList = franchisee.Organization != null ? franchisee.Organization.DeactivationnNote : "";
            return franchiseeNotesList;
        }

        public IEnumerable<DropdownListItem> GetFranchiseeRPID(long? franchiseeId)
        {
            var isRpValuePResent = false;
            var franchiseeRPI = new List<ReviewPushAPILocation>();

            var franhisee = _franchiseeRepository.Get(franchiseeId.GetValueOrDefault());
            if (franchiseeId != null && franchiseeId != 0)
                isRpValuePResent = franhisee.ReviewpushId != null ? true : false;
            var franchiseeHavingRPIValues = _franchiseeRepository.Table.Where(x => x.ReviewpushId != null).Select(x => x.ReviewpushId).ToList();
            if (franchiseeHavingRPIValues.Count() <= 0)
            {
                franchiseeRPI = _reviewPushApiLocationRepository.Table.ToList();
            }
            else
            {
                franchiseeRPI = _reviewPushApiLocationRepository.Table.ToList();
                //franchiseeRPI = _reviewPushApiLocationRepository.Table.Where(x => !franchiseeHavingRPIValues.Contains(x.Id)).ToList();
            }

            //if (isRpValuePResent)
            //{
            //    var rpiId = franhisee.Reviewpush;
            //    franchiseeRPI.Add(rpiId);
            //}
            var orgUserList = franchiseeRPI.Select(l => new DropdownListItem
            {
                Display = l.Name + " - " + l.Rp_ID,
                Value = l.Id.ToString(),
            });
            return orgUserList;
        }

        public bool OneTimeProjectChangeStatus(OneTimeProjectFilter filter)
        {
            var domain = _onetimeprojectfeeAddFundRoyalityRepository.Table.FirstOrDefault(x => x.FranchiseeId == filter.FranchiseeId);
            if (domain == null)
            {
                domain = new OnetimeprojectfeeAddFundRoyality()
                {
                    FranchiseeId = filter.FranchiseeId,
                    IsInRoyality = filter.IsInRoyality,
                    IsSEOInRoyalty = true,
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                    IsNew = true
                };
                _onetimeprojectfeeAddFundRoyalityRepository.Save(domain);
            }
            else
            {
                domain.IsInRoyality = filter.IsInRoyality;
                domain.IsNew = false;
                _onetimeprojectfeeAddFundRoyalityRepository.Save(domain);
            }
            return true;
        }

        public decimal GetMinimumRoyalityForFranchisee(long franchiseeId)
        {
            decimal months = 0;
            decimal minRoyality = 0;
            var minRoyalityList = _minRoyaltyFeeSlabsRepository.Table.Where(x => x.FranchiseeId == franchiseeId).ToList();
            var franchiseeDomain = _franchiseeRepository.Table.OrderBy(x => x.Id).FirstOrDefault(x => x.Id == franchiseeId);
            if (minRoyalityList != null)
            {
                var createdDate = franchiseeDomain.RegistrationDate.GetValueOrDefault();
                if (createdDate != null)
                {
                    var todayDate = DateTime.UtcNow;
                    LocalDate start = new LocalDate(createdDate.Year, createdDate.Month, createdDate.Day);
                    LocalDate end = new LocalDate(todayDate.Year, todayDate.Month, todayDate.Day);
                    Period period = Period.Between(start, end);
                    months = (period.Years * 12) + period.Months;

                    foreach (var minRoyalityDb in minRoyalityList)
                    {

                        if (minRoyalityDb.StartValue <= months && minRoyalityDb.EndValue >= months)
                        {
                            minRoyality = minRoyalityDb.MinRoyality;
                        }
                        else if (minRoyalityDb.StartValue <= months && minRoyalityDb.EndValue == null)
                        {
                            minRoyality = minRoyalityDb.MinRoyality;
                        }
                    }
                }
                else
                {
                    minRoyality = 0;
                }
            }
            return minRoyality;
        }

        private void CreateRegistrationHistry(FranchiseeEditModel franchisee)
        {

            var histryDomain = new FranchiseeRegistrationHistry()
            {
                DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                FranchiseeId = franchisee.Id,
                RegistrationDate = franchisee.RegistrationDate.GetValueOrDefault(),
                IsNew = true
            };

            _franchiseeRegistrationHistryRepository.Save(histryDomain);
        }

        private void CreatePerpetuityHistry(FranchiseeEditModel franchisee, bool? isPerpetuity)
        {

            var perpetuityhistryDomain = new Perpetuitydatehistry()
            {
                DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                FranchiseeId = franchisee.Id,
                LastDateChecked = DateTime.UtcNow,
                IsNew = true,
                IsPerpetuity = isPerpetuity
            };

            _perpetuitydatehistryRepository.Save(perpetuityhistryDomain);
        }

        private List<FranchiseeRegistrationHistryViewModel> GetRegistrationHistryViewModel(long? franchiseeId)
        {
            var franchiseeRegistrationHistryList = _franchiseeRegistrationHistryRepository.Table.Where(x => x.FranchiseeId == franchiseeId).ToList();
            var viewModel = franchiseeRegistrationHistryList.Select(x => new FranchiseeRegistrationHistryViewModel()
            {
                Date = x.DataRecorderMetaData.DateCreated,
                RegistrationDate = x.RegistrationDate,
                UserName = GetUserName(x.DataRecorderMetaData.ModifiedBy != null ? x.DataRecorderMetaData.ModifiedBy : x.DataRecorderMetaData.CreatedBy)
            }).ToList();

            return viewModel;
        }

        private string GetUserName(long? orgUserId)
        {
            if (orgUserId == null)
            {
                return "";
            }
            var orgDomain = _organizationRoleUserRepository.Get(orgUserId.GetValueOrDefault());
            return orgDomain.Person.Name.FirstName + " " + orgDomain.Person.Name.LastName;
        }
        public class DropdownListItem
        {

            public string Display { get; set; }
            public string Value { get; set; }
            public string Alias { get; set; }
            public long Id { get; set; }
        }

        public FranchiseeDocumentListModel GetFranchiseeDocumentReport(FranchiseeDocumentFilter filter)
        {
            var franchiseeViewModelList = new List<FranchiseeDocumentViewModel>();
            var franchisseeList = new List<string>();
            var columnList = new List<string>();
            var franchiseeViewModel = new FranchiseeDocumentViewModel();
            var franchiseeListNameList = new List<string>();
            var documentListIds = (new long[] { 3, 15, 16, 17, 18, 19, 5, 8, 11 }).ToList();
            var franchiseeList = _organizationRepository.Table.Where(x => !x.Name.StartsWith("0") && (x.Id != 1 && x.Id != 2) && x.IsActive
                            && (filter.FranchiseeId == 0 || filter.FranchiseeId == x.Id)).OrderBy(x => x.Id).Select(x => x.Franchisee).OrderBy(x => x.Organization.Name).ToList();
            var franchiseeIds = franchiseeList.Where(x => x != null).Select(x => x.Id).ToList();
            var franchiseeDocumentGroupedList = _franchiseeDocumentRepository.Table.Where(x => x.UploadFor == filter.UploadedOn && x.Franchisee.Organization.IsActive)
                .OrderBy(x => x.DocumentType.Order).ToList();
            documentListIds = documentListIds.Where(x => filter.DocumentTypeId == 0 || x == filter.DocumentTypeId).ToList();
            var documentsList = _documentTypeRepository.Table.Where(x => documentListIds.Contains(x.Id) && (filter.DocumentTypeId == 0 || x.Id == filter.DocumentTypeId)).OrderBy(x => x.Order).ToList();
            var documentsName = documentsList.Select(x => x.Name).ToList();
            var franchiseeNameList = franchiseeList.Where(x => x != null).Select(x => x.Organization.Name).ToList();
            var franchiseeIdList = franchiseeList.Where(x => x != null).OrderBy(x => x.Organization.Id).Select(x => x.Organization.Id).ToList();
            var inDbDocumentListIds = franchiseeDocumentGroupedList.Select(x => x.DocumentTypeId.GetValueOrDefault()).ToList();
            var exceptFranchiseeIds = documentListIds.Except(inDbDocumentListIds).ToList();

            foreach (var franchiseeId in franchiseeIdList)
            {
                franchiseeViewModel = new FranchiseeDocumentViewModel();
                var franchiseeDocumentList = franchiseeDocumentGroupedList.Where(x => (filter.DocumentTypeId == 0 || x.DocumentTypeId == filter.DocumentTypeId) && x.FranchiseeId == franchiseeId).Select(x => x).ToList();
                if (franchiseeDocumentList.Count() > 0)
                {
                    var collection = franchiseeDocumentList.OrderByDescending(x => x.Id).ToList();
                    franchiseeViewModel.FranchiseeName = collection.FirstOrDefault().Franchisee.Organization.Name;
                    foreach (var document in documentsList.Where(x => x != null))
                    {
                        if (collection.Any(x => x.DocumentTypeId == document.Id))
                        {
                            var franchiseeDocument = collection.FirstOrDefault(x => x.DocumentTypeId == document.Id);
                            columnList.Add(franchiseeDocument.DocumentType.Name);
                            franchiseeViewModel.ExpiryDate.Add(franchiseeDocument.ExpiryDate != null ? _clock.ToUtc(franchiseeDocument.ExpiryDate.GetValueOrDefault()) : default(DateTime?));
                            franchiseeViewModel.IsPresent.Add(true);
                            franchiseeViewModel.IsDeclined.Add(franchiseeDocument.IsRejected);
                        }
                        else
                        {
                            franchiseeViewModel.ExpiryDate.Add(default(DateTime?));
                            franchiseeViewModel.IsPresent.Add(false);

                            franchiseeViewModel.IsDeclined.Add(false);
                        }
                        if (document.Id == 11)
                        {
                            franchiseeViewModel.DocumentStatusViewModel = GetDocumentStatus(franchiseeId, Convert.ToInt32(filter.UploadedOn));
                        }
                        if (filter.UploadedOn == DateTime.UtcNow.Year.ToString() && document.Id == 11)
                        {
                            var franchiseeDocumentType = _franchiseeDocumentTypeRepository.Table.Where(x => x.DocumentTypeId == 11 && x.FranchiseeId == franchiseeId).OrderByDescending(x => x.Id).FirstOrDefault();
                            if (franchiseeDocumentType != null)
                            {
                                franchiseeViewModel.IsPerpetuity.Add(franchiseeDocumentType.IsPerpetuity);
                            }
                            else
                            {
                                franchiseeViewModel.IsPerpetuity.Add(false);
                            }
                        }
                        else
                        {
                            franchiseeViewModel.IsPerpetuity.Add(false);
                        }
                    }
                }
                else
                {
                    var franchisee = _organizationRepository.Get(franchiseeId);
                    franchiseeViewModel.FranchiseeName = franchisee.Name;
                    for (int i = 0; i < documentListIds.Count(); i++)
                    {
                        franchiseeViewModel.IsPresent.Add(false);
                        franchiseeViewModel.IsDeclined.Add(false);
                    }
                }

                franchiseeViewModelList.Add(franchiseeViewModel);
            }

            franchiseeViewModelList = franchiseeViewModelList.OrderBy(x => x.FranchiseeName).ToList();
            return new FranchiseeDocumentListModel()
            {
                FranchiseeViewModel = franchiseeViewModelList,
                DocumentList = documentsName,
            };
        }

        private bool changingMinRoyalityService(IEnumerable<MinRoyaltyFeeSlabsEditModel> royalitySlabs, long franchiseeId, List<MinRoyaltyFeeSlabs> minRoyaltyFeeSlabs)
        {
            foreach (var royalitySlab in royalitySlabs)
            {
                var royalitySlabDomain = new MinRoyaltyFeeSlabs
                {
                    EndValue = royalitySlab.EndValue,
                    StartValue = royalitySlab.StartValue,
                    FranchiseeId = franchiseeId,
                    IsNew = royalitySlab.Id == 0 ? true : false,
                    Id = royalitySlab.Id,
                    MinRoyality = royalitySlab.MinRoyality
                };
                _minRoyaltyFeeSlabsRepository.Save(royalitySlabDomain);
            }
            if (minRoyaltyFeeSlabs.Count() > royalitySlabs.Count())
            {
                var royalitySlabsId = royalitySlabs.Select(x => x.Id).ToList();
                var recordsTobeDeletedList = minRoyaltyFeeSlabs.Where(x => !royalitySlabsId.Contains(x.Id));
                foreach (var recordsTobeDeleted in recordsTobeDeletedList)
                {
                    _minRoyaltyFeeSlabsRepository.Delete(recordsTobeDeleted);
                }
            }
            return true;
        }

        private IEnumerable<MinRoyaltyFeeSlabsEditModel> CreateEmptyData()
        {
            var defaultValue = new List<MinRoyaltyFeeSlabsEditModel>()
            {
                new MinRoyaltyFeeSlabsEditModel()
                {
                     StartValue=6,
                     EndValue=12,
                     MinRoyality=200,
                     Id=0
                },
                new MinRoyaltyFeeSlabsEditModel()
                {
                     StartValue=13,
                     EndValue=18,
                     MinRoyality=400,
                     Id=0
                },
                new MinRoyaltyFeeSlabsEditModel()
                {
                     StartValue=19,
                     EndValue=24,
                     MinRoyality=600,
                     Id=0
                },
                new MinRoyaltyFeeSlabsEditModel()
                {
                     StartValue=25,
                     EndValue=null,
                     MinRoyality=800,
                     Id=0
                },
            };
            return defaultValue;
        }
        private List<DocumentStatusViewModel> GetDocumentStatus(long? franchiseeId, int year)
        {
            var perpetuitydatehistryViewModelList = new List<DocumentStatusViewModel>();
            var perpetuityDateHistryList = _perpetuitydatehistryRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.LastDateChecked.Year == year).ToList();
            var isRejectedDocumntList = _franchiseeDocumentRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.UploadFor == year.ToString() && x.IsRejected).ToList();
            var userIds = perpetuityDateHistryList.Count() > 0 ? perpetuityDateHistryList.Select(x => x.DataRecorderMetaData.CreatedBy).ToList() : default(List<long?>);
            if (perpetuityDateHistryList.Count() > 0)
            {
                var orgRoleUserList = _organizationRoleUserRepository.Table.Where(x => userIds.Contains(x.Id)).ToList();
                perpetuitydatehistryViewModelList = perpetuityDateHistryList.Where(x => x.FranchiseeId == franchiseeId && x.LastDateChecked.Year == year).AsEnumerable().Select(x => new DocumentStatusViewModel()
                {
                    Date = x.LastDateChecked,
                    Status = x.IsPerpetuity.GetValueOrDefault() ? "Perpetuity was On" : "Perpetuity was Off",
                    UserName = orgRoleUserList.FirstOrDefault(x1 => x1.Id == x.DataRecorderMetaData.CreatedBy).Person.FirstName + " " + orgRoleUserList.FirstOrDefault(x1 => x1.Id == x.DataRecorderMetaData.CreatedBy).Person.LastName
                }).ToList();
            }
            userIds = isRejectedDocumntList.Count() > 0 ? isRejectedDocumntList.Select(x => x.DataRecorderMetaData.CreatedBy).ToList() : default(List<long?>);
            if (isRejectedDocumntList.Count() > 0)
            {
                var orgRoleUserList = _organizationRoleUserRepository.Table.Where(x => userIds.Contains(x.Id)).ToList();
                perpetuitydatehistryViewModelList.AddRange(isRejectedDocumntList.Where(x => x.FranchiseeId == franchiseeId && x.UploadFor == year.ToString()).AsEnumerable().Select(x => new DocumentStatusViewModel()
                {
                    Date = x.DataRecorderMetaData.DateCreated,
                    Status = "Declined for " + x.UploadFor,
                    UserName = orgRoleUserList.FirstOrDefault(x1 => x1.Id == x.DataRecorderMetaData.CreatedBy).Person.FirstName + " " + orgRoleUserList.FirstOrDefault(x1 => x1.Id == x.DataRecorderMetaData.CreatedBy).Person.LastName
                }).ToList());
            }
            return perpetuitydatehistryViewModelList.OrderByDescending(x => x.Date).ToList();
        }
        public IEnumerable<DropdownListItem> GetFranchiseeDocumentList()
        {
            var documentListIds = (new long[] { 3, 15, 16, 17, 18, 19, 5, 8, 11 }).ToList();
            var documentTypeRepository = _documentTypeRepository.Table.Where(x => documentListIds.Contains(x.Id)).ToList();
            return documentTypeRepository.Where(x => x.CategoryId >= 0).Select(l => new DropdownListItem
            { Display = l.Name, Value = l.Id.ToString() }).OrderBy(y => y.Value).ToArray();
        }

        public bool DownloadTaxReport(FranchiseeDocumentFilter filter, out string fileName)
        {
            var taxDocumentReport = GetFranchiseeDocumentReport(filter);
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "\\franchiseeDirectory-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelTaxDocumentCreator.CreateExcelDocument(taxDocumentReport.FranchiseeViewModel, fileName, taxDocumentReport.DocumentList);
        }

        public bool SaveTimeEstimateForFranchisee(long franchiseeId)
        {
            var shiftChargesForFranchisee = _shiftChargesRepository.Table.Where(x => x.IsActive && x.FranchiseeId == franchiseeId).ToList();
            var maintenanceChargesForFranchisee = _maintenanceChargesRepository.Table.Where(x => x.IsActive && x.FranchiseeId == franchiseeId).ToList();
            var replacementChargesForFranchisee = _replacementChargesRepository.Table.Where(x => x.IsActive && x.FranchiseeId == franchiseeId).ToList();
            var floorGrindingAdjustmentsForFranchisee = _floorGrindingAdjustmentRepository.Table.Where(x => x.IsActive && x.FranchiseeId == franchiseeId).ToList();

            if (shiftChargesForFranchisee.Count == 0)
            {
                var shiftCharges = _shiftChargesRepository.Table.Where(x => x.IsActive && x.FranchiseeId == null).ToList();
                List<ShiftCharges> list = shiftCharges.Select(x => new ShiftCharges
                {
                    CommercialRestorationShiftPrice = x.CommercialRestorationShiftPrice,
                    TechDayShiftPrice = x.TechDayShiftPrice,
                    MaintenanceTechNightShiftPrice = x.MaintenanceTechNightShiftPrice,
                    IsNew = true,
                    IsActive = true,
                    FranchiseeId = franchiseeId,
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData()
                }).ToList();
                foreach (var item in list)
                {
                    _shiftChargesRepository.Save(item);
                }
            }
            if (maintenanceChargesForFranchisee.Count == 0)
            {
                var maintenanceCharges = _maintenanceChargesRepository.Table.Where(x => x.IsActive && x.FranchiseeId == null).ToList();
                List<MaintenanceCharges> list = maintenanceCharges.Select(x => new MaintenanceCharges
                {
                    Material = x.Material,
                    Order = x.Order,
                    UOM = x.UOM,
                    High = x.High,
                    Low = x.Low,
                    Notes = x.Notes,
                    IsNew = true,
                    IsActive = true,
                    FranchiseeId = franchiseeId,
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData()
                }).ToList();
                foreach (var item in list)
                {
                    _maintenanceChargesRepository.Save(item);
                }
            }
            if (replacementChargesForFranchisee.Count == 0)
            {
                var replacementCharges = _replacementChargesRepository.Table.Where(x => x.IsActive && x.FranchiseeId == null).ToList();
                List<ReplacementCharges> list = replacementCharges.Select(x => new ReplacementCharges
                {
                    Material = x.Material,
                    Order = x.Order,
                    CostOfInstallingTile = x.CostOfInstallingTile,
                    CostOfRemovingTile = x.CostOfRemovingTile,
                    CostOfTileMaterial = x.CostOfTileMaterial,
                    TotalReplacementCost = x.TotalReplacementCost,
                    IsNew = true,
                    IsActive = true,
                    FranchiseeId = franchiseeId,
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData()
                }).ToList();
                foreach (var item in list)
                {
                    _replacementChargesRepository.Save(item);
                }
            }
            if (floorGrindingAdjustmentsForFranchisee.Count == 0)
            {
                var floorGrindingAdjustments = _floorGrindingAdjustmentRepository.Table.Where(x => x.IsActive && x.FranchiseeId == null).ToList();
                List<FloorGrindingAdjustment> list = floorGrindingAdjustments.Select(x => new FloorGrindingAdjustment
                {
                    DiameterOfGrindingPlate = x.DiameterOfGrindingPlate,
                    AdjustmentFactor = x.AdjustmentFactor,
                    Area = x.Area,
                    IsNew = true,
                    IsActive = true,
                    FranchiseeId = franchiseeId
                }).ToList();
                foreach (var item in list)
                {
                    _floorGrindingAdjustmentRepository.Save(item);
                }
            }
            return true;
        }

        public bool CreatePriceEstimate(long franchiseeId)
        {
            var priceEstimates = _priceEstimateServicesRepository.Table.Where(x => x.FranchiseeId == 62).ToList();
            foreach(var price in priceEstimates)
            {
                var domain = new PriceEstimateServices()
                {
                    BulkCorporatePrice = price.BulkCorporatePrice,
                    BulkCorporateAdditionalPrice = price.BulkCorporateAdditionalPrice,
                    CorporatePrice = price.BulkCorporatePrice,
                    CorporateAdditionalPrice = price.BulkCorporateAdditionalPrice,
                    FranchiseePrice = price.BulkCorporatePrice,
                    FranchiseeAdditionalPrice = price.BulkCorporateAdditionalPrice,
                    FranchiseeId = franchiseeId,
                    ServiceTagId = price.ServiceTagId,
                    AlternativeSolution = price.AlternativeSolution,
                    IsNew = true
                };
                _priceEstimateServicesRepository.Save(domain);
            }
            return true;
        }

        public bool SEOChargesChangeStatus(OneTimeProjectFilter filter)
        {
            var domain = _onetimeprojectfeeAddFundRoyalityRepository.Table.FirstOrDefault(x => x.FranchiseeId == filter.FranchiseeId);
            if (domain == null)
            {
                domain = new OnetimeprojectfeeAddFundRoyality()
                {
                    FranchiseeId = filter.FranchiseeId,
                    IsInRoyality = false,
                    DataRecorderMetaData = new Application.Domain.DataRecorderMetaData(),
                    IsNew = true,
                    IsSEOInRoyalty = filter.IsseoInRoyality
                };
                _onetimeprojectfeeAddFundRoyalityRepository.Save(domain);
            }
            else
            {
                domain.IsNew = false;
                domain.IsSEOInRoyalty = filter.IsseoInRoyality;
                _onetimeprojectfeeAddFundRoyalityRepository.Save(domain);
            }
            return true;
        }

        private bool SaveReviewURL(FranchiseeEditModel franchiseeEditModel)
        {
            try
            {
                var reviewPushApiLocation = _reviewPushApiLocationRepository.Table.FirstOrDefault(x => x.Id == franchiseeEditModel.ReviewRpId);

                if(reviewPushApiLocation == null)
                {
                    var reviewPushApiLocationLastObject = _reviewPushApiLocationRepository.Table.LastOrDefault();
                    var franchisee = _franchiseeRepository.Table.FirstOrDefault(x => x.Id == franchiseeEditModel.Id);

                    var reviewPushLocation = new ReviewPushAPILocation
                    {
                        Location_Id = reviewPushApiLocationLastObject != null ? reviewPushApiLocationLastObject.Location_Id + 1 : default(long),
                        Rp_ID = reviewPushApiLocationLastObject != null ? reviewPushApiLocationLastObject.Rp_ID + 1 : default(long),
                        NewRp_ID = franchiseeEditModel?.ReviewURL,
                        Name = franchisee != null && franchisee.DisplayName != null ? franchisee.DisplayName : "",
                        IsDeleted = false,
                        IsNew = true
                    };
                    _reviewPushApiLocationRepository.Save(reviewPushLocation);

                    return true;
                }
                else
                {
                    if (reviewPushApiLocation.NewRp_ID == franchiseeEditModel.ReviewURL)
                    {
                        return true;
                    }
                    else
                    {
                        reviewPushApiLocation.NewRp_ID = franchiseeEditModel.ReviewURL;
                        reviewPushApiLocation.IsNew = false;
                        _reviewPushApiLocationRepository.Save(reviewPushApiLocation);

                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
