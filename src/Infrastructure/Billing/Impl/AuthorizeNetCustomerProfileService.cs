using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Billing.Impl
{
    [DefaultImplementation]
    public class AuthorizeNetCustomerProfileService : IAuthorizeNetCustomerProfileService
    {

        private readonly ISettings _settings;
        private readonly ILogService _logService;
        private readonly IRepository<AuthorizeNetApiMaster> _authorizeNetApiMasterRepository;
        private string _apiLoginId;
        private string _apiTransactionKey;

        public AuthorizeNetCustomerProfileService(ISettings settings, IUnitOfWork unitOfWork, ILogService logService)
        {
            _authorizeNetApiMasterRepository = unitOfWork.Repository<AuthorizeNetApiMaster>();
            _settings = settings;
            _logService = logService;
        }

        private void SetApiKey(long accountTypeId)
        {
            var authorizeNetApi = _authorizeNetApiMasterRepository.Fetch(x => x.AccountTypeId == accountTypeId).First();
            _apiLoginId = authorizeNetApi.ApiLoginID;
            _apiTransactionKey = authorizeNetApi.ApiTransactionKey;
        }
        public createCustomerProfileResponse CreateNewProfile(long accountTypeId, string cardNumber, string cvv, string expiryDate, long payeeId, string email)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey,
            };
            var creditCard = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = expiryDate,
                cardCode = cvv,
            };


            //standard api call to retrieve response
            var cc = new paymentType { Item = creditCard };

            var paymentProfileList = new List<customerPaymentProfileType>();
            var ccPaymentProfile = new customerPaymentProfileType();
            ccPaymentProfile.payment = cc;

            paymentProfileList.Add(ccPaymentProfile);

            var customerProfile = new customerProfileType();
            customerProfile.merchantCustomerId = payeeId.ToString();
            customerProfile.email = email;
            customerProfile.paymentProfiles = paymentProfileList.ToArray();

            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };

            var controller = new createCustomerProfileController(request);
            controller.Execute();

            createCustomerProfileResponse response = controller.GetApiResponse();

            return response;
        }

        public createCustomerProfileResponse CreateECheckProfile(long accountTypeId, string accountNumber, string nameOnAccount, string routingNumber, string bankName, long payeeId, string email)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };
            var bankAccount = new bankAccountType
            {
                accountNumber = accountNumber,
                nameOnAccount = nameOnAccount,
                routingNumber = routingNumber,
                bankName = bankName
            };


            //standard api call to retrieve response
            var cc = new paymentType { Item = bankAccount };

            var paymentProfileList = new List<customerPaymentProfileType>();
            var ccPaymentProfile = new customerPaymentProfileType();
            ccPaymentProfile.payment = cc;

            paymentProfileList.Add(ccPaymentProfile);

            var customerProfile = new customerProfileType();
            customerProfile.merchantCustomerId = payeeId.ToString();
            customerProfile.email = email;
            customerProfile.paymentProfiles = paymentProfileList.ToArray();

            var request = new createCustomerProfileRequest { profile = customerProfile, validationMode = validationModeEnum.none };

            var controller = new createCustomerProfileController(request);
            controller.Execute();

            createCustomerProfileResponse response = controller.GetApiResponse();

            return response;
        }

        public createCustomerPaymentProfileResponse CreateAdditionalEChaekPaymentProfile(long accountTypeId, string customerProfileId, string accountNumber, string nameOnAccount, string routingNumber, string bankName)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };

            var eCheckDetail = new bankAccountType
            {
                accountNumber = accountNumber,
                nameOnAccount = nameOnAccount,
                routingNumber = routingNumber,
                bankName = bankName,
            };

            var creditCard = new paymentType { Item = eCheckDetail };

            var eCheckPaymentProfile = new customerPaymentProfileType();
            eCheckPaymentProfile.payment = creditCard;

            var request = new createCustomerPaymentProfileRequest
            {
                customerProfileId = customerProfileId,
                paymentProfile = eCheckPaymentProfile,
                validationMode = validationModeEnum.none,
            };

            //Prepare Request
            var controller = new createCustomerPaymentProfileController(request);
            controller.Execute();

            //Send Request to EndPoint
            createCustomerPaymentProfileResponse response = controller.GetApiResponse();

            return response;
        }

        public createCustomerPaymentProfileResponse CreateAdditionalPaymentProfile(long accountTypeId, string customerProfileId, string cardNumber, string cvv, string expiryDate)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };

            var creditCardDetail = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = expiryDate,
                cardCode = cvv
            };

            var creditCard = new paymentType { Item = creditCardDetail };

            var creditCardPaymentProfile = new customerPaymentProfileType();
            creditCardPaymentProfile.payment = creditCard;

            var request = new createCustomerPaymentProfileRequest
            {
                customerProfileId = customerProfileId,
                paymentProfile = creditCardPaymentProfile,
                validationMode = validationModeEnum.none
            };

            //Prepare Request
            var controller = new createCustomerPaymentProfileController(request);
            controller.Execute();

            //Send Request to EndPoint
            createCustomerPaymentProfileResponse response = controller.GetApiResponse();

            return response;
        }

        public deleteCustomerPaymentProfileResponse DeleteCustomerProfile(long accountTypeId, string profileId, string paymentProfileId)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };

            var request = new deleteCustomerPaymentProfileRequest
            {
                customerProfileId = profileId,
                customerPaymentProfileId = paymentProfileId
            };

            //Prepare Request
            var controller = new deleteCustomerPaymentProfileController(request);
            controller.Execute();

            deleteCustomerPaymentProfileResponse response = controller.GetApiResponse();
            return response;
        }

        public ANetApiResponse ChargeNewCard(long accountTypeId, string cardNumber, string cvv, string expiryDate, long invoiceId, decimal amount, long payeeId, string name, string franchiseeName)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };

            var creditCard = new creditCardType
            {
                cardNumber = cardNumber,
                expirationDate = expiryDate,
                cardCode = cvv
            };

            var paymentType = new paymentType { Item = creditCard };

            var duplicateWindowSetting = new settingType()
            {
                settingName = settingNameEnum.duplicateWindow.ToString(),
            };

            var order = new orderType
            {
                invoiceNumber = invoiceId.ToString()
            };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                amount = amount,
                payment = paymentType,
                order = order,
                billTo = new customerAddressType
                {
                    firstName = franchiseeName,
                    lastName = ""
                }
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            var controller = new createTransactionController(request);
            controller.Execute();

            ANetApiResponse response = controller.GetApiResponse();
            if (response == null) response = controller.GetErrorResponse();
            return response;
        }

        private AuthorizeNet.Environment GetAuthNetEnvironment()
        {
            return _settings.AuthNetTestMode ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;
        }

        public ANetApiResponse ChargeCustomerProfile(long accountTypeId, string customerProfileId, string customerPaymentProfileId, long invoiceId, decimal amount, string franchiseeName)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };

            var profileToCharge = new customerProfilePaymentType();

            profileToCharge.customerProfileId = customerProfileId;
            profileToCharge.paymentProfile = new paymentProfile { paymentProfileId = customerPaymentProfileId };
            GetCustomerInformation(customerPaymentProfileId, customerProfileId, franchiseeName);

            var duplicateWindowSetting = new settingType
            {
                settingName = settingNameEnum.duplicateWindow.ToString(),
            };

            var order = new orderType
            {
                invoiceNumber = invoiceId.ToString()
            };

            var transactionRequest = new transactionRequestType
            {

                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),
                amount = amount,
                profile = profileToCharge,
                transactionSettings = new[] { duplicateWindowSetting },
                order = order,
            };



            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            var controller = new createTransactionController(request);
            controller.Execute();

            ANetApiResponse response = controller.GetApiResponse();
            if (response == null) response = controller.GetErrorResponse();
            return response;
        }
        private void GetCustomerInformation(string customerPaymentProfileId, string customerProfileId, string frachiseeName)
        {
            var profile = new getCustomerPaymentProfileRequest
            {
                customerPaymentProfileId = customerPaymentProfileId,
                customerProfileId = customerProfileId,
            };
            var getProfile = new getCustomerPaymentProfileController(profile);
            getProfile.Execute();
            ANetApiResponse response = getProfile.GetApiResponse();
            if (response.messages.resultCode == AuthorizeNet.Api.Contracts.V1.messageTypeEnum.Ok)
            {
                UpdateCustomerInformation(customerProfileId, frachiseeName, ((AuthorizeNet.Api.Contracts.V1.getCustomerPaymentProfileResponse)response).paymentProfile);
            }
        }
        private void UpdateCustomerInformation(string customerProfileId, string frachiseeName, customerPaymentProfileMaskedType paymentProfile)
        {
            var updateAddress = new updateCustomerPaymentProfileRequest();
            var creditCardMaskedType = new creditCardMaskedType();
            var echeckMaskedType = new bankAccountMaskedType();
            var customerAddressType = new customerAddressType
            {
                firstName = frachiseeName
            };

            try
            {
                creditCardMaskedType = (creditCardMaskedType)paymentProfile.payment.Item;
                updateAddress = CreditCardTypeTransaction(creditCardMaskedType, customerProfileId, paymentProfile, customerAddressType);
            }
            catch (Exception e1)
            {
                echeckMaskedType = (bankAccountMaskedType)paymentProfile.payment.Item;
                updateAddress = ECheckTypeTransaction(echeckMaskedType, customerProfileId, paymentProfile, customerAddressType);
            }

            var updateAddressExecution = new updateCustomerPaymentProfileController(updateAddress);
            updateAddressExecution.Execute();
            ANetApiResponse Updationresponse = updateAddressExecution.GetApiResponse();
        }
        private updateCustomerPaymentProfileRequest CreditCardTypeTransaction(creditCardMaskedType creditCardMaskedType, string customerProfileId, customerPaymentProfileMaskedType paymentProfile, customerAddressType customerAddressType)
        {
            var driverLicense = new driversLicenseType
            {
                number = paymentProfile.driversLicense != null ? paymentProfile.driversLicense.number : null,
                dateOfBirth = paymentProfile.driversLicense != null ? paymentProfile.driversLicense.dateOfBirth : null,
                state = paymentProfile.driversLicense != null ? paymentProfile.driversLicense.state : null,
            };
            if (driverLicense.number == null && driverLicense.dateOfBirth == null && driverLicense.state == null)
            {
                driverLicense = null;
            }
            var creditCard = new creditCardType
            {
                cardNumber = creditCardMaskedType.cardNumber,
                expirationDate = creditCardMaskedType.expirationDate
            };
            var paymentType = new paymentType
            {
                Item = creditCard
            };
            var customerpaymentmaskedType = new customerPaymentProfileExType
            {
                billTo = customerAddressType,
                customerPaymentProfileId = paymentProfile.customerPaymentProfileId,
                customerType = paymentProfile.customerType,
                customerTypeSpecified = paymentProfile.customerTypeSpecified,
                driversLicense = driverLicense,
                payment = paymentType
            };
            var updateAddress = new updateCustomerPaymentProfileRequest
            {
                customerProfileId = customerProfileId,
                paymentProfile = customerpaymentmaskedType
            };
            return updateAddress;
        }

        private updateCustomerPaymentProfileRequest ECheckTypeTransaction(bankAccountMaskedType bankAccountMaskedType, string customerProfileId, customerPaymentProfileMaskedType paymentProfile, customerAddressType customerAddressType)
        {
            var driverLicense = new driversLicenseType
            {
                number = paymentProfile.driversLicense != null ? paymentProfile.driversLicense.number : null,
                dateOfBirth = paymentProfile.driversLicense != null ? paymentProfile.driversLicense.dateOfBirth : null,
                state = paymentProfile.driversLicense != null ? paymentProfile.driversLicense.state : null,
            };
            if (driverLicense.number == null && driverLicense.dateOfBirth == null && driverLicense.state == null)
            {
                driverLicense = null;
            }

            var echeck = new bankAccountType
            {
                accountNumber = bankAccountMaskedType.accountNumber,
                bankName = bankAccountMaskedType.bankName,
                nameOnAccount = bankAccountMaskedType.nameOnAccount,
                routingNumber = bankAccountMaskedType.routingNumber,
                accountType = (bankAccountTypeEnum)bankAccountMaskedType.accountType,
                echeckType = bankAccountMaskedType.echeckType,
                accountTypeSpecified = bankAccountMaskedType.accountTypeSpecified,
                echeckTypeSpecified = bankAccountMaskedType.echeckTypeSpecified
            };
            var paymentType = new paymentType
            {
                Item = echeck
            };
            var customerpaymentmaskedType = new customerPaymentProfileExType
            {
                billTo = customerAddressType,
                customerPaymentProfileId = paymentProfile.customerPaymentProfileId,
                customerType = paymentProfile.customerType,
                customerTypeSpecified = paymentProfile.customerTypeSpecified,
                driversLicense = driverLicense,
                payment = paymentType
            };
            var updateAddress = new updateCustomerPaymentProfileRequest
            {
                customerProfileId = customerProfileId,
                paymentProfile = customerpaymentmaskedType
            };
            return updateAddress;
        }
        public ANetApiResponse VoidPayment(long accountTypeId, string transactionId)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };


            //standard api call to retrieve response
            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.voidTransaction.ToString(),    // refund type
                refTransId = transactionId,
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            //validate
            if (response != null && response.messages.resultCode == messageTypeEnum.Ok)
            {
                if (response.transactionResponse != null)
                {
                    _logService.Info("Success, Auth Code : " + response.transactionResponse.authCode);
                }
            }
            else if (response != null)
            {
                _logService.Error("Error: " + response.messages.message[0].code + "  " + response.messages.message[0].text);
                if (response.transactionResponse != null)
                {
                    _logService.Error("Transaction Error : " + response.transactionResponse.errors[0].errorCode + " " + response.transactionResponse.errors[0].errorText);
                }
            }

            return response;
        }

        public createTransactionResponse DebitBankAccount(long accountTypeId, decimal amount, string accountNumber, string routingNumber, string name,
            string bankName, long invoiceId, string franchiseeName)
        {
            SetApiKey(accountTypeId);
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetAuthNetEnvironment();

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = _apiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = _apiTransactionKey
            };
            var order = new orderType
            {
                invoiceNumber = invoiceId.ToString()
            };
            var bankAccount = new bankAccountType
            {
                accountNumber = accountNumber,
                routingNumber = routingNumber,
                echeckType = echeckTypeEnum.WEB,   // change based on how you take the payment (web, telephone, etc)
                nameOnAccount = name,
                bankName = bankName
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = bankAccount };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // refund type
                payment = paymentType,
                amount = amount,
                order = order,
                billTo = new customerAddressType
                {
                    firstName = franchiseeName,
                    lastName = ""
                }

            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the contoller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();
            if (response == null)
            {
                ANetApiResponse errorResponse = controller.GetErrorResponse();
                if (errorResponse != null)
                {
                    var error = (errorResponse.messages != null && errorResponse.messages.message.Any()) ?
                                                                        errorResponse.messages.message[0].text : "some error accured!";
                    throw new Exception(error);
                }
            }
            return response;
        }


        public bool IfErrorHandleErrorResponse(ANetApiResponse chargeCardResponse, ProcessorResponse processorResponse, long franchiseeId, ILogService _logService)
        {
            if (chargeCardResponse != null && chargeCardResponse.messages != null &&
                chargeCardResponse.messages.resultCode == messageTypeEnum.Error && !(chargeCardResponse is createTransactionResponse))
            {
                processorResponse.Message = "Some error occured while processing your credit card";
                if (chargeCardResponse.messages.message != null)
                {
                    var messages = string.Join(" ------- ", chargeCardResponse.messages.message.Select(m => m.code + "-" + m.text).ToArray());
                    _logService.Info(string.Format("Some error accured in payment for Franchisee# {0}. Message - {1}", franchiseeId, messages));
                }
                else
                    _logService.Info(string.Format("Some error accured in payment for Franchisee# {0}",
                   franchiseeId));

                return true;
            }
            return false;
        }
    }
}
