using AuthorizeNet.Api.Contracts.V1;
using Core.Application;
using Core.Application.Attribute;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using System;
using System.Linq;

namespace Infrastructure.Billing.Impl
{
    [DefaultImplementation]
    public class ECheckPaymentService : IECheckPaymentService
    {
        private readonly IAuthorizeNetCustomerProfileService _authorizeNetCustomerProfileService;
        private readonly ISettings _settings;
        private readonly IECheckProfileService _eCheckProfileService;
        private readonly IRepository<FranchiseePaymentProfile> _franchiseePaymentProfileRepository;
        private readonly IRepository<PaymentInstrument> _paymentInstrumentRepository;
        private readonly IECheckFactory _eCheckFactory;
        private readonly IRepository<ECheck> _eCheckRepository;
        private readonly IRepository<ECheckPayment> _eCheckPaymentRepository;
        private readonly ILogService _logService;
        private readonly IChargeCardPaymentService _chargeCardPaymentService;
        private readonly IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;
        public ECheckPaymentService(IUnitOfWork unitOfWork, ISettings settings, IAuthorizeNetCustomerProfileService authorizeNetCustomerProfileService,
            IECheckProfileService eCheckProfileService, IECheckFactory eCheckFactory, ILogService logService, IChargeCardPaymentService chargeCardPaymentService)
        {
            _authorizeNetCustomerProfileService = authorizeNetCustomerProfileService;
            _settings = settings;
            _eCheckProfileService = eCheckProfileService;
            _franchiseePaymentProfileRepository = unitOfWork.Repository<FranchiseePaymentProfile>();
            _paymentInstrumentRepository = unitOfWork.Repository<PaymentInstrument>();
            _eCheckFactory = eCheckFactory;
            _eCheckRepository = unitOfWork.Repository<ECheck>();
            _eCheckPaymentRepository = unitOfWork.Repository<ECheckPayment>();
            _logService = logService;
            _chargeCardPaymentService = chargeCardPaymentService;
            _franchiseeInvoiceRepository= unitOfWork.Repository<FranchiseeInvoice>();
        }

        public void Save(ECheckEditModel model, Payment payment)
        {
            var eCheckPayment = _eCheckFactory.CreateECheckPayment(model, payment);
            _eCheckPaymentRepository.Save(eCheckPayment);
        }

        public ProcessorResponse MakeECheckPayment(ECheckEditModel model, long franchiseeId)
        {
            var paymentResponse = new ProcessorResponse();
            if (model.InstrumentTypeId == (long)InstrumentType.ECheckOnFile)
            {
                _logService.Info(string.Format("Start eCheck payment for Franchisee# {0}", franchiseeId));

                var franchiseeProfile = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.ProfileTypeId == model.ProfileTypeId).ToList();
                if (franchiseeProfile == null || franchiseeProfile.Count == 0) { paymentResponse = new ProcessorResponse(); }
                long franchiseePaymentProfileId = franchiseeProfile.FirstOrDefault().Id;
                var paymentProfiles = _paymentInstrumentRepository.Fetch(x => x.InstrumentEntityId == model.InstrumentId && x.InstrumentTypeId == (long)InstrumentType.ECheck && x.FranchiseePaymentProfileId == franchiseePaymentProfileId).ToList();

                var franchiseeName = franchiseeProfile.FirstOrDefault() != default(FranchiseePaymentProfile) ? franchiseeProfile.FirstOrDefault().Franchisee.Organization.Name : "";

                var paymentProfile = paymentProfiles != null ? paymentProfiles.First() : null;
                if (paymentProfile == null) { paymentResponse = new ProcessorResponse(); }
                var response = _authorizeNetCustomerProfileService.ChargeCustomerProfile(model.ProfileTypeId, franchiseeProfile.First().CMID, paymentProfile.InstrumentProfileId,
                                 model.InvoiceId, model.Amount, franchiseeName);

                if (_authorizeNetCustomerProfileService.IfErrorHandleErrorResponse(response, paymentResponse, franchiseeId, _logService)) return paymentResponse;

                var echeckResponse = response as createTransactionResponse;
                if (response != null && response.messages.resultCode == messageTypeEnum.Ok && echeckResponse.transactionResponse != null)
                {
                    if (echeckResponse.transactionResponse.responseCode == ((long)TransactionResponseType.Approved).ToString())
                    {
                        paymentResponse.ProcessorResult = ProcessorResponseResult.Accepted;
                        paymentResponse.RawResponse = echeckResponse.transactionResponse.transId;
                        paymentResponse.Message = echeckResponse.transactionResponse.messages[0].description;
                        paymentResponse.InstrumentId = model.InstrumentId;
                        _logService.Info(string.Format("eCheck payment completed for Franchisee# {0} and transaction# {1}, Response is - {2}",
                            franchiseeId, paymentResponse.RawResponse, paymentResponse.Message));
                        return paymentResponse;
                    }

                    paymentResponse.Message = response.messages.message[0].text;
                    paymentResponse.ProcessorResult = ProcessorResponseResult.Fail;
                    _logService.Error(string.Format("Some error occured while processing your ECheck. {0}", paymentResponse.Message));

                    if (echeckResponse.transactionResponse != null && echeckResponse.transactionResponse.errors != null)
                    {
                        paymentResponse.Message = echeckResponse.transactionResponse.errors[0].errorText;
                        _logService.Error(string.Format("Some error occured while processing your ECheck. {0}", paymentResponse.Message));
                    }
                    return paymentResponse;
                }
                else if (response != null && response.messages.resultCode == messageTypeEnum.Error)
                {
                    paymentResponse.Message = response.messages.message[0].text;
                    paymentResponse.ProcessorResult = ProcessorResponseResult.Fail;
                    _logService.Error(string.Format("Some error occured while processing your ECheck. {0}", paymentResponse.Message));
                }
                else
                {
                    paymentResponse.Message = "Some error occured while processing your ECheck";
                    _logService.Error("Some error occured while processing your ECheck.");
                }
                return paymentResponse;
            }
            else if (model.SaveOnFile == true)
            {
                if (!ECheckPayment(model, paymentResponse)) return paymentResponse;
                var result = _eCheckProfileService.CreateProfile(model, franchiseeId);
                paymentResponse.InstrumentId = result.InstrumentId;
                if (result.ProcessorResult == ProcessorResponseResult.Fail && paymentResponse.ProcessorResult == ProcessorResponseResult.Accepted
                    && paymentResponse.RawResponse != null)
                {
                    _logService.Error("Some error accured, start rollback payment.");
                    _chargeCardPaymentService.RollbackPayment(model.ProfileTypeId, paymentResponse.RawResponse);
                }
                return result;
            }
            else
            {
                if (!ECheckPayment(model, paymentResponse)) return paymentResponse;
                try
                {
                    var eCheck = _eCheckFactory.CreateDomain(model);
                    _eCheckRepository.Save(eCheck);
                    paymentResponse.InstrumentId = eCheck.Id;
                }
                catch (Exception ex)
                {
                    _logService.Error("Some error accured, start rollback payment.");
                    if (paymentResponse.RawResponse != null && paymentResponse.ProcessorResult == ProcessorResponseResult.Accepted)
                        _chargeCardPaymentService.RollbackPayment(model.ProfileTypeId, paymentResponse.RawResponse);
                    throw ex;
                }
                return paymentResponse;
            }
        }

        private bool ECheckPayment(ECheckEditModel model, ProcessorResponse paymentResponse)
        {
            model.ProcessorResponse = new ProcessorResponse();
            var franchisee = _franchiseeInvoiceRepository.Table.Where(x => x.InvoiceId == model.InvoiceId).FirstOrDefault();
            var franchiseeName = franchisee != default(FranchiseeInvoice) ? franchisee.Franchisee.Organization.Name : "";

            var response = _authorizeNetCustomerProfileService.DebitBankAccount(model.ProfileTypeId, model.Amount, model.AccountNumber, model.Number,
                model.Name, model.BankName, model.InvoiceId, franchiseeName);
            bool condition = (response != null && response.messages.resultCode == messageTypeEnum.Ok && response.transactionResponse != null
                && response.transactionResponse.errors == null);

            if (condition)
            {
                if (response != null)
                {
                    paymentResponse.RawResponse = response.transactionResponse.transId;
                    paymentResponse.Message = response.transactionResponse.messages[0].description;
                    paymentResponse.ProcessorResult = ProcessorResponseResult.Accepted;
                    _logService.Info(string.Format("eCheck payment completed for transaction# {0}, Response is - {1}",
                             paymentResponse.RawResponse, paymentResponse.Message));
                }
                return true;
            }

            if (response != null)
            {
                paymentResponse.ProcessorResult = ProcessorResponseResult.Fail;
                paymentResponse.Message = "Error: " + response.transactionResponse.errors[0].errorText;
                _logService.Error(string.Format(paymentResponse.Message));
            }
            else
            {
                paymentResponse.Message = "Some error occured while processing your ECheck";
                _logService.Error("Some error occured while processing your ECheck");
            }
            return false;
        }
    }
}
