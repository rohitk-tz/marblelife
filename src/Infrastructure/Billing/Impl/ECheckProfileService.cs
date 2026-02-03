using AuthorizeNet.Api.Contracts.V1;
using Core.Application;
using Core.Application.Attribute;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using System;
using System.Linq;

namespace Infrastructure.Billing.Impl
{
    [DefaultImplementation]
    public class ECheckProfileService : IECheckProfileService
    {
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<FranchiseePaymentProfile> _franchiseePaymentProfileRepository;
        private readonly IRepository<PaymentInstrument> _paymentInstrumentRepository;
        private readonly IAuthorizeNetCustomerProfileService _authorizeNetCustomerProfileService;
        private readonly IChargeCardPaymentService _chargeCardPaymentService;
        private readonly IChargeCardPaymentProfileFactory _chargeCardPaymentProfileFactory;
        private readonly IECheckService _eCheckService;
        private readonly ILogService _logService;

        public ECheckProfileService(IUnitOfWork unitOfWork, IChargeCardPaymentService chargeCardPaymentService, IAuthorizeNetCustomerProfileService authorizeNetCustomerProfileService,
            IChargeCardPaymentProfileFactory chargeCardPaymentProfileFactory, IECheckService eCheckService, ILogService logService)
        {
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _franchiseePaymentProfileRepository = unitOfWork.Repository<FranchiseePaymentProfile>();
            _paymentInstrumentRepository = unitOfWork.Repository<PaymentInstrument>();
            _chargeCardPaymentService = chargeCardPaymentService;
            _chargeCardPaymentProfileFactory = chargeCardPaymentProfileFactory;
            _authorizeNetCustomerProfileService = authorizeNetCustomerProfileService;
            _eCheckService = eCheckService;
            _logService = logService;
        }
        public ProcessorResponse CreateProfile(ECheckEditModel model, long franchiseeId)
        {
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            if (franchisee == null)
            {
                _logService.Error("Franchisee does not exist.");
                return new ProcessorResponse
                {
                    Message = "Franchisee does not exist.",
                    ProcessorResult = ProcessorResponseResult.Fail,
                    RawResponse = "Franchisee does not exist."
                };
            }
            ProcessorResponse responseforadfund = null;
            ProcessorResponse responseforroyality = null;
            responseforadfund = CreateProfileForAdFund(model, franchiseeId);
            if (responseforadfund != null && responseforadfund.ProcessorResult == ProcessorResponseResult.Accepted)
            {
                responseforroyality = CreateProfileForRoyality(model, franchiseeId);
            }
            if (responseforadfund == null || responseforroyality == null)
            {
                // delete profile if an error occured
                _logService.Info(string.Format("Some error accured.Start Deleting payment profile."));
                if (responseforadfund != null)
                    _authorizeNetCustomerProfileService.DeleteCustomerProfile((long)AuthorizeNetAccountType.AdFund, responseforadfund.CustomerProfileId, responseforadfund.RawResponse);
                if (responseforroyality != null)
                    _authorizeNetCustomerProfileService.DeleteCustomerProfile((long)AuthorizeNetAccountType.Royalty, responseforroyality.CustomerProfileId, responseforroyality.RawResponse);

            }
            return responseforadfund;
        }

        public ProcessorResponse CreateProfileForAdFund(ECheckEditModel model, long franchiseeId)
        {
            ProcessorResponse response = null;
            response = CreateAuthorizeNetProfileForEcheck((long)AuthorizeNetAccountType.AdFund, model, response, franchiseeId);

            if (response != null && response.ProcessorResult == ProcessorResponseResult.Accepted)
            {
                var eCheckId = SaveECheckAndPaymentInstrument((long)AuthorizeNetAccountType.AdFund, model, response, franchiseeId);
                response.InstrumentId = eCheckId;
            }
            return response;
        }

        public ProcessorResponse CreateProfileForRoyality(ECheckEditModel model, long franchiseeId)
        {
            ProcessorResponse response = null;
            response = CreateAuthorizeNetProfileForEcheck((long)AuthorizeNetAccountType.Royalty, model, response, franchiseeId);
            if (response != null && response.ProcessorResult == ProcessorResponseResult.Accepted)
            {
                var eCheckId = SaveECheckAndPaymentInstrument((long)AuthorizeNetAccountType.Royalty, model, response, franchiseeId);
                response.InstrumentId = eCheckId;
            }
            return response;
        }

        public ProcessorResponse CreateAuthorizeNetProfileForEcheck(long accountTypeId, ECheckEditModel model, ProcessorResponse processorResponse, long franchiseeId)
        {
            processorResponse = new ProcessorResponse();
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            var email = franchisee.Organization.Email;

            _logService.Info(string.Format("Start Creating Authorize.net profile for Franchisee# {0} and AccountType# {1}. ", franchisee.Id, accountTypeId));

            var franchiseePaymentProfiles = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.ProfileTypeId == accountTypeId).ToList();
            if (franchiseePaymentProfiles != null && franchiseePaymentProfiles.Count > 0)
            {
                _logService.Info(string.Format("Payment profile already exists. Start creating additional payment profile Franchisee# {0} and AccountType# {1}. ", franchisee.Id, accountTypeId));

                var franchiseePaymentProfile = franchiseePaymentProfiles.First();
                var createNewProfileResponse = _authorizeNetCustomerProfileService.CreateAdditionalEChaekPaymentProfile(accountTypeId, franchiseePaymentProfile.CMID,
                    model.AccountNumber, model.Name, model.Number, model.BankName);

                var authNetPaymentRefId = createNewProfileResponse.customerPaymentProfileId;
                processorResponse.CustomerProfileId = franchiseePaymentProfile.CMID;

                if (createNewProfileResponse != null && createNewProfileResponse.messages.resultCode == messageTypeEnum.Ok
                    && !string.IsNullOrEmpty(createNewProfileResponse.customerProfileId) && !string.IsNullOrEmpty(authNetPaymentRefId))
                {
                    _logService.Info(string.Format("Payment profile has been successfully created for Franchisee {0} and accountType# {1}, and the Reference# is {2}.",
                         franchiseeId, accountTypeId, authNetPaymentRefId));

                    processorResponse.Message = "Payment profile has been successfully created.";
                    processorResponse.RawResponse = authNetPaymentRefId;
                    processorResponse.ProcessorResult = ProcessorResponseResult.Accepted;
                }
                else
                {
                    _logService.Error(string.Format("Some error accured, {0}.", createNewProfileResponse.messages.message[0].text));
                    processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                    processorResponse.Message += createNewProfileResponse.messages.message[0].text;
                }
            }
            else
            {
                _logService.Info(string.Format("Start creating new payment profile Franchisee# {0} and AccountType# {1}. ", franchisee.Id, accountTypeId));

                var createNewProfileResponse = _authorizeNetCustomerProfileService.CreateECheckProfile(accountTypeId, model.AccountNumber, model.Name,
                    model.Number, model.BankName, franchiseeId, email);

                if (createNewProfileResponse != null && createNewProfileResponse.messages.resultCode == messageTypeEnum.Ok
                    && createNewProfileResponse.customerProfileId != "")
                {
                    try
                    {
                        var cmId = createNewProfileResponse.customerProfileId;
                        var authNetPaymentRefId = createNewProfileResponse.customerPaymentProfileIdList[0];

                        var franchiseePaymentProfile = _chargeCardPaymentProfileFactory.CreateDoamin(accountTypeId, cmId, franchisee);
                        _franchiseePaymentProfileRepository.Save(franchiseePaymentProfile);

                        _logService.Info(string.Format("profile has been successfully created for Franchisee# {0} and AccountType# {1} , the Reference# is {2}.",
                           franchisee.Id, accountTypeId, cmId));
                        processorResponse.Message = "profile has been successfully created.";
                        processorResponse.RawResponse = authNetPaymentRefId;
                        processorResponse.ProcessorResult = ProcessorResponseResult.Accepted;
                    }
                    catch (Exception ex)
                    {
                        processorResponse.Message += createNewProfileResponse.messages.message[0].text; ;
                        processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                        _logService.Error(ex);
                    }
                }
                else
                {
                    processorResponse.Message += createNewProfileResponse.messages.message[0].text;
                    processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                    _logService.Error(string.Format("Some Error accured while creating payment Profile, {0} ", createNewProfileResponse.messages.message[0].text));
                }
            }
            return processorResponse;
        }

        private long SaveECheckAndPaymentInstrument(long accountTypeId, ECheckEditModel model, ProcessorResponse response, long franchiseeId)
        {
            var franchiseePaymentProfile = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.ProfileTypeId == accountTypeId).FirstOrDefault();

            var eCheck = _eCheckService.Save(model);

            var typeId = (long)InstrumentType.ECheck;
            var paymentInstrument = _chargeCardPaymentProfileFactory.CreateDoamin(franchiseePaymentProfile.Id, typeId, response.RawResponse, eCheck.Id);
            var paymentInstrumentList = _paymentInstrumentRepository.Table.Where(x => x.IsPrimary == true && x.IsActive == true && x.FranchiseePaymentProfileId == franchiseePaymentProfile.Id).FirstOrDefault();
            if (paymentInstrumentList == null)
                paymentInstrument.IsPrimary = true;
            else
                paymentInstrument.IsPrimary = false;
            _paymentInstrumentRepository.Save(paymentInstrument);

            return eCheck.Id;
        }
    }
}
