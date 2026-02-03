using AuthorizeNet.Api.Contracts.V1;
using Core.Application;
using Core.Application.Attribute;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations;
using Core.Organizations.Domain;
using System.Linq;

namespace Infrastructure.Billing.Impl
{
    [DefaultImplementation]
    public class ChargeCardPaymentService : IChargeCardPaymentService
    {
        private readonly IRepository<ChargeCard> _chargeCardRepository;
        private readonly ISettings _settings;
        private readonly IAuthorizeNetCustomerProfileService _authorizeNetCustomerProfileService;
        private readonly IChargeCardFactory _chargeCardFactory;
        private readonly IChargeCardProfileService _chargeCardProfileService;
        private readonly IRepository<FranchiseePaymentProfile> _franchiseePaymentProfileRepository;
        private readonly IRepository<PaymentInstrument> _paymentInstrumentRepository;
        private readonly IRepository<ChargeCardPayment> _chargeCardPaymentRepository;
        private readonly ILogService _logService;
        private readonly IRepository<Organization> _organizationRepository;
        

        public ChargeCardPaymentService(IUnitOfWork unitOfWork, ISettings settings, IAuthorizeNetCustomerProfileService authorizeNetCustomerProfileService,
            IChargeCardFactory chargeCardFactory, IChargeCardProfileService chargeCardProfileService, ILogService logService)
        {
            _chargeCardRepository = unitOfWork.Repository<ChargeCard>();
            _settings = settings;
            _authorizeNetCustomerProfileService = authorizeNetCustomerProfileService;
            _chargeCardFactory = chargeCardFactory;
            _chargeCardProfileService = chargeCardProfileService;
            _franchiseePaymentProfileRepository = unitOfWork.Repository<FranchiseePaymentProfile>();
            _paymentInstrumentRepository = unitOfWork.Repository<PaymentInstrument>();
            _chargeCardPaymentRepository = unitOfWork.Repository<ChargeCardPayment>();
            _logService = logService;
            _organizationRepository= unitOfWork.Repository<Organization>();
        }

        public void Save(ProcessorResponse response, Payment payment)
        {
            var chargeCardPayment = _chargeCardFactory.CreateChargeCardPayment(response, payment);
            _chargeCardPaymentRepository.Save(chargeCardPayment);
        }
        public ProcessorResponse ChargeCardOnFile(InstrumentOnFileEditModel model, ProcessorResponse paymentResponse, long franchiseeId)
        {
            if (paymentResponse == null)
            {
                paymentResponse = new ProcessorResponse();
            }

            var franchiseeProfile = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.ProfileTypeId == model.ProfileTypeId).ToList();

            if (franchiseeProfile == null || franchiseeProfile.Count == 0) { paymentResponse = new ProcessorResponse(); }
            model.CustomerProfileId = franchiseeProfile.FirstOrDefault().CMID;
            long franchiseePaymentProfileId = franchiseeProfile.FirstOrDefault().Id;


            var franchiseeName = franchiseeProfile.FirstOrDefault() != default(FranchiseePaymentProfile) ? franchiseeProfile.FirstOrDefault().Franchisee.Organization.Name : "";
            var paymentProfile = _paymentInstrumentRepository.Fetch(x => x.InstrumentEntityId == model.InstrumentId && x.InstrumentTypeId == (long)InstrumentType.ChargeCard && x.FranchiseePaymentProfileId == franchiseePaymentProfileId).ToList();
            model.PaymentProfileId = paymentProfile != null ? paymentProfile.First().InstrumentProfileId : "";

            _logService.Info(string.Format("Start Payment using On File Card for FranchiseeId {0} and profileId {1}. ", franchiseeId, model.CustomerProfileId));
            var chargeFromExistingCardResponse =
                _authorizeNetCustomerProfileService.ChargeCustomerProfile(model.ProfileTypeId, model.CustomerProfileId,
                model.PaymentProfileId, model.InvoiceId, model.Amount, franchiseeName);

            if (_authorizeNetCustomerProfileService.IfErrorHandleErrorResponse(chargeFromExistingCardResponse, paymentResponse, franchiseeId, _logService)) return paymentResponse;

            var cardSuccessResponse = chargeFromExistingCardResponse as createTransactionResponse;
            if (cardSuccessResponse != null &&
                cardSuccessResponse.messages.resultCode == messageTypeEnum.Ok &&
                cardSuccessResponse.transactionResponse != null)
            {
                if (cardSuccessResponse.transactionResponse.responseCode ==
                    ((long)TransactionResponseType.Approved).ToString())
                {
                    paymentResponse.ProcessorResult = ProcessorResponseResult.Accepted;
                    paymentResponse.RawResponse = cardSuccessResponse.transactionResponse.transId;
                    paymentResponse.Message = cardSuccessResponse.transactionResponse.messages[0].description;
                    _logService.Info(string.Format("Payment accepted using On File Card for FranchiseeId {0} transactionId {1}, Response is - {2}. "
                        , franchiseeId, paymentResponse.RawResponse, paymentResponse.Message));

                    return paymentResponse;
                }

                paymentResponse.Message = "Error: " + chargeFromExistingCardResponse.messages.message[0].text;
                paymentResponse.ProcessorResult = ProcessorResponseResult.Fail;
                _logService.Error(string.Format("Some error accured while processing onFile card for FranchiseeId {0}. Response - {1}",
                   franchiseeId, paymentResponse.Message));

                if (cardSuccessResponse.transactionResponse != null &&
                    cardSuccessResponse.transactionResponse.errors != null)
                {
                    paymentResponse.Message = "Error: " +
                                              cardSuccessResponse.transactionResponse.errors[0].errorText;
                    _logService.Error(string.Format("Some error accured while processing onFile card for FranchiseeId {0}. Response - {1}",
                   franchiseeId, paymentResponse.Message));
                }

                return paymentResponse;
            }

            if (chargeFromExistingCardResponse != null)
            {
                paymentResponse.Message = "Error: " + chargeFromExistingCardResponse.messages.message[0].text;
                if (cardSuccessResponse.transactionResponse != null &&
                    cardSuccessResponse.transactionResponse.errors != null)
                {
                    paymentResponse.Message = "Error: " +
                                              cardSuccessResponse.transactionResponse.errors[0].errorText;
                    _logService.Error(string.Format("Some error accured while processing onFile card for FranchiseeId {0}. Response - {1}",
                   franchiseeId, paymentResponse.Message));
                }
            }
            else
            {
                paymentResponse.Message = "Some error occured while processing your credit card";
                _logService.Error(string.Format("Some error accured while processing onFile card for FranchiseeId {0}. Response - {1}",
                   franchiseeId, paymentResponse.Message));
            }

            paymentResponse.ProcessorResult = ProcessorResponseResult.Fail;
            return paymentResponse;
        }

        public ProcessorResponse ChargeCardPayment(ChargeCardPaymentEditModel model, long franchiseeId,
            out decimal creditCardCharge, out decimal chargedAmount)
        {
            creditCardCharge = 0;
            chargedAmount = model.Amount;
            _logService.Info(string.Format("Start ChargeCard payment for Franchisee# {0}. CVV - {1}", franchiseeId, model.ChargeCardEditModel.CVV));

            var processorResponse = new ProcessorResponse();
            var cvv = (model.ChargeCardEditModel.CVV.Length == 2) ? ("0" + model.ChargeCardEditModel.CVV) : ((model.ChargeCardEditModel.CVV.Length == 1)
                ? "00" + model.ChargeCardEditModel.CVV : model.ChargeCardEditModel.CVV);

            var month = (int.Parse(model.ChargeCardEditModel.ExpireMonth)) < 10 ? "0" + model.ChargeCardEditModel.ExpireMonth :
                model.ChargeCardEditModel.ExpireMonth;

            var expirationDate = month + "" + model.ChargeCardEditModel.ExpireYear.Substring(2);

            var organization = _organizationRepository.Get(franchiseeId);
            var chargeCardResponse =
                _authorizeNetCustomerProfileService.ChargeNewCard(model.ProfileTypeId, model.ChargeCardEditModel.Number, cvv, expirationDate,
                model.InvoiceId, model.Amount, franchiseeId, model.ChargeCardEditModel.Name, organization!=null? organization.Name:"");

            var cvvResponseCodeToMatch = (char)CvvResponseCode.SuccessfullyMatch;

            if (_settings.AuthNetTestMode)
            {
                cvvResponseCodeToMatch = (char)CvvResponseCode.NotProcessed;
            }

            if (_authorizeNetCustomerProfileService.IfErrorHandleErrorResponse(chargeCardResponse, processorResponse, franchiseeId, _logService)) return processorResponse;

            var cardSuccessResponse = chargeCardResponse as createTransactionResponse;
            bool condition = (cardSuccessResponse != null && cardSuccessResponse.messages.resultCode == messageTypeEnum.Ok &&
                              cardSuccessResponse.transactionResponse != null &&
                              cardSuccessResponse.transactionResponse.responseCode == ((long)TransactionResponseType.Approved).ToString());

            // &&cardSuccessResponse.transactionResponse.cvvResultCode == cvvResponseCodeToMatch.ToString()

            if (condition)
            {
                processorResponse.ProcessorResult = ProcessorResponseResult.Accepted;
                if (cardSuccessResponse.transactionResponse != null)
                {
                    _logService.Info(string.Format("payment acceptaed for Franchisee# {0} and the Response is {1}", franchiseeId, processorResponse.Message));
                    processorResponse.RawResponse = cardSuccessResponse.transactionResponse.transId;
                    processorResponse.Message = cardSuccessResponse.transactionResponse.messages[0].description;
                }
                return processorResponse;
            }

            if (chargeCardResponse != null)
            {
                processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                processorResponse.Message = "Error: " + chargeCardResponse.messages.message[0].text;
                _logService.Info(string.Format("Some error accured in payment for Franchisee# {0} and the Response is {1}",
                    franchiseeId, processorResponse.Message));

                if (cardSuccessResponse.transactionResponse != null && cardSuccessResponse.transactionResponse.errors != null)
                {
                    processorResponse.Message = "Error: " + cardSuccessResponse.transactionResponse.errors[0].errorText;
                    _logService.Info(string.Format("Some error accured in payment for Franchisee# {0} and the Response is {1}",
                    franchiseeId, processorResponse.Message));
                }
            }
            else
            {
                processorResponse.Message = "Some error occured while processing your credit card";
                _logService.Info(string.Format("Some error accured in payment for Franchisee# {0}",
                   franchiseeId));
            }
            return processorResponse;
        }


        public bool RollbackPayment(long accountTypeId, string transactionId)
        {
            _logService.Info(string.Format("starting payment Rollback for AccountType {0} and Transaction# {1}", accountTypeId, transactionId));
            var result = _authorizeNetCustomerProfileService.VoidPayment(accountTypeId, transactionId);
            return true;
        }
    }
}
