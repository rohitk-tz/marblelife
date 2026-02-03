using AuthorizeNet.Api.Contracts.V1;
using Core.Application;
using Core.Application.Attribute;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Billing.Impl
{
    [DefaultImplementation]
    public class ChargeCardProfileService : IChargeCardProfileService
    {
        private readonly IRepository<ECheck> _eCheckRepository;
        private readonly IRepository<ChargeCard> _chargeCardRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<PaymentInstrument> _paymentInstrumentRepository;
        private readonly IRepository<FranchiseePaymentProfile> _franchiseePaymentProfileRepository;
        private readonly IAuthorizeNetCustomerProfileService _authorizeNetCustomerProfileService;
        private readonly IChargeCardPaymentProfileFactory _chargeCardPaymentProfileFactory;
        private readonly IChargeCardService _chargeCardService;
        private readonly ILogService _logService;

        public ChargeCardProfileService(IUnitOfWork unitOfWork, IAuthorizeNetCustomerProfileService authorizeNetCustomerProfileService,
             IChargeCardPaymentProfileFactory chargeCardPaymentProfileFactory, IChargeCardService chargeCardService, ILogService logService)
        {
            _eCheckRepository = unitOfWork.Repository<ECheck>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _authorizeNetCustomerProfileService = authorizeNetCustomerProfileService;
            _franchiseePaymentProfileRepository = unitOfWork.Repository<FranchiseePaymentProfile>();
            _paymentInstrumentRepository = unitOfWork.Repository<PaymentInstrument>();
            _chargeCardPaymentProfileFactory = chargeCardPaymentProfileFactory;
            _chargeCardService = chargeCardService;
            _chargeCardRepository = unitOfWork.Repository<ChargeCard>();
            _logService = logService;
        }

        public List<PaymentInstrumentViewModel> GetInstrumentList(long franchiseeId, long paymentTypeId)
        {
            var paymentInstrumnets = new List<PaymentInstrumentViewModel>();

            var franchiseePaymentProfile = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.ProfileTypeId == paymentTypeId).ToList();
            if (paymentTypeId == 0) { franchiseePaymentProfile = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId).ToList(); }
            if (franchiseePaymentProfile == null && franchiseePaymentProfile.Count == 0)
            {
                return paymentInstrumnets;
            }

            var franchiseePaymentProfiles = (from pi in _paymentInstrumentRepository.Table.ToList()
                                             join fp in franchiseePaymentProfile on pi.FranchiseePaymentProfileId equals fp.Id
                                             select pi).ToList();

            if (franchiseePaymentProfiles == null || !franchiseePaymentProfiles.Any())
            {
                return paymentInstrumnets;
            }

            foreach (var paymentProfile in franchiseePaymentProfiles)
            {
                if (paymentProfile.InstrumentTypeId == (long)InstrumentType.ChargeCard)
                {
                    var chargeCardId = paymentProfile.InstrumentEntityId;
                    var chargeCard = _chargeCardRepository.Get(chargeCardId);

                    if (chargeCard == null) continue;

                    var card = new PaymentInstrumentViewModel
                    {
                        Number = chargeCard.Number,
                        InstrumentId = chargeCardId,
                        Name = chargeCard.NameOnCard,
                        ExpirationDate = chargeCard.ExpiryMonth + "/" + chargeCard.ExpiryYear,
                        CardType = chargeCard.CardType.Name,
                        IsActive = paymentProfile.IsActive,
                        IsPrimary = paymentProfile.IsPrimary,
                        TypeId = paymentProfile.InstrumentTypeId,
                        PaymentInstrumentId = paymentProfile.Id
                    };
                    paymentInstrumnets.Add(card);
                }
                else if (paymentProfile.InstrumentTypeId == (long)InstrumentType.ECheck)
                {
                    var eCheckId = paymentProfile.InstrumentEntityId;
                    var eCheck = _eCheckRepository.Get(eCheckId);
                    if (eCheck == null) continue;

                    var card = new PaymentInstrumentViewModel
                    {
                        Number = eCheck.RoutingNumber,
                        InstrumentId = eCheckId,
                        Name = eCheck.Name,
                        ExpirationDate = "N/A",
                        CardType = eCheck.Lookup.Name,
                        TypeId = paymentProfile.InstrumentTypeId,
                        AccountNumber = eCheck.AccountNumber,
                        AccountTypeId = eCheck.Lookup.Id,
                        BankName = eCheck.BankName,
                        PaymentInstrumentId = paymentProfile.Id,
                        IsActive = paymentProfile.IsActive,
                        IsPrimary = paymentProfile.IsPrimary
                    };
                    paymentInstrumnets.Add(card);
                }
            }
            return paymentInstrumnets;
        }
        public ProcessorResponse CreateProfile(ChargeCardEditModel model, long franchiseeId)
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
            var expiryMonth = Convert.ToInt32(model.ExpireMonth); var expiryYear = Convert.ToInt32(model.ExpireYear);
            CheckExpiryDate(expiryMonth, expiryYear);
            ProcessorResponse responseforadfund = null;
            ProcessorResponse responseforroyality = null;
            responseforadfund = CreateProfileForAdFund(model, franchiseeId);
            if (responseforadfund != null && responseforadfund.ProcessorResult == ProcessorResponseResult.Accepted)
            {
                responseforroyality = CreateProfileForRoyality(model, franchiseeId);
            }
            if ((responseforadfund == null || responseforroyality == null))
            {
                // delete profile if an error occured
                _logService.Error(string.Format("Some error accured.Start Deleting payment profile."));
                if (responseforadfund != null)
                    _authorizeNetCustomerProfileService.DeleteCustomerProfile((long)AuthorizeNetAccountType.AdFund, responseforadfund.CustomerProfileId, responseforadfund.RawResponse);
                if (responseforroyality != null)
                    _authorizeNetCustomerProfileService.DeleteCustomerProfile((long)AuthorizeNetAccountType.Royalty, responseforroyality.CustomerProfileId, responseforroyality.RawResponse);
            }
            return responseforadfund;
        }
        private ProcessorResponse CreateProfileForAdFund(ChargeCardEditModel model, long franchiseeId)
        {
            ProcessorResponse response = null;
            response = CreateAuthorizeNetProfile((long)AuthorizeNetAccountType.AdFund, model, response, franchiseeId);

            if (response != null && response.ProcessorResult == ProcessorResponseResult.Accepted)
            {
                var chargeCardId = SaveChargeCardAndInstrument((long)AuthorizeNetAccountType.AdFund, model, response, franchiseeId);
                response.InstrumentId = chargeCardId;
            }
            return response;
        }
        private ProcessorResponse CreateProfileForRoyality(ChargeCardEditModel model, long franchiseeId)
        {
            ProcessorResponse response = null;
            response = CreateAuthorizeNetProfile((long)AuthorizeNetAccountType.Royalty, model, response, franchiseeId);

            if (response != null && response.ProcessorResult == ProcessorResponseResult.Accepted)
            {
                var chargeCardId = SaveChargeCardAndInstrument((long)AuthorizeNetAccountType.Royalty, model, response, franchiseeId);
                response.InstrumentId = chargeCardId;
            }
            return response;
        }
        public ProcessorResponse CreateAuthorizeNetProfile(long accountTypeId, ChargeCardEditModel model, ProcessorResponse processorResponse, long franchiseeId)
        {
            processorResponse = new ProcessorResponse();
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            var email = franchisee.Organization.Email;

            _logService.Info(string.Format("Start Creating Authorize.net profile for Franchisee# {0} and AccountType# {1} ", franchisee.Id, accountTypeId));

            var franchiseePaymentProfiles = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId
                                            && x.ProfileTypeId == accountTypeId).ToList();

            if (franchiseePaymentProfiles != null && franchiseePaymentProfiles.Count > 0)
            {
                _logService.Info(string.Format("Payment profile already exists. Start creating additional payment profile Franchisee# {0} and AccountType# {1} ", franchisee.Id, accountTypeId));

                var franchiseePaymentProfile = franchiseePaymentProfiles.First();
                var cvv = (model.CVV.Length == 2) ? ("0" + model.CVV) : ((model.CVV.Length == 1) ? "00" + model.CVV : model.CVV);

                var month = (int.Parse(model.ExpireMonth)) < 10 ?
                    "0" + model.ExpireMonth : model.ExpireMonth;

                var expirationDate = month + "" + model.ExpireYear.Substring(2);

                var createNewProfileResponse = _authorizeNetCustomerProfileService.CreateAdditionalPaymentProfile(accountTypeId, franchiseePaymentProfile.CMID,
                    model.Number, cvv, expirationDate);

                var authNetPaymentRefId = createNewProfileResponse.customerPaymentProfileId;

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
                    processorResponse.Message = createNewProfileResponse.messages.message[0].text;
                    processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                }
            }
            else
            {
                var cvv = (model.CVV.Length == 2) ? ("0" + model.CVV) : ((model.CVV.Length == 1) ? "00" + model.CVV : model.CVV);

                var month = (int.Parse(model.ExpireMonth)) < 10 ? "0" + model.ExpireMonth : model.ExpireMonth;

                var expirationDate = month + "" + model.ExpireYear.Substring(2);

                _logService.Info(string.Format("Start creating new payment profile Franchisee# {0} and AccountType# {1}. ", franchisee.Id, accountTypeId));

                var createNewProfileResponse = _authorizeNetCustomerProfileService.CreateNewProfile(accountTypeId, model.Number, cvv, expirationDate, franchiseeId, email);

                if (createNewProfileResponse != null && createNewProfileResponse.messages.resultCode == messageTypeEnum.Ok
                        && createNewProfileResponse.customerProfileId != "" && createNewProfileResponse.customerProfileId != null)
                {
                    try
                    {
                        var cmId = createNewProfileResponse.customerProfileId;
                        var authNetPaymentRefId = createNewProfileResponse.customerPaymentProfileIdList[0];

                        var franchiseePaymentProfile = _chargeCardPaymentProfileFactory.CreateDoamin(accountTypeId, cmId, franchisee);
                        _franchiseePaymentProfileRepository.Save(franchiseePaymentProfile);

                        processorResponse.CustomerProfileId = cmId;
                        _logService.Info(string.Format("profile has been successfully created for Franchisee# {0} and AccountType# {1} , the Reference# is {2}.",
                            franchisee.Id, accountTypeId, cmId));
                        processorResponse.Message = "Profile has been successfully created";
                        processorResponse.RawResponse = authNetPaymentRefId;
                        processorResponse.ProcessorResult = ProcessorResponseResult.Accepted;
                    }
                    catch (Exception ex)
                    {
                        processorResponse.Message += createNewProfileResponse.messages.message[0].text;
                        processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                        _logService.Error(ex);
                    }
                }
                else
                {
                    _logService.Error(string.Format("Some error accured, {0}.", createNewProfileResponse.messages.message[0].text));
                    processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                    processorResponse.Message += createNewProfileResponse.messages.message[0].text;
                }
            }
            return processorResponse;
        }
        public bool ManageInstrument(string instrumentIds, bool isActive)
        {
            if (instrumentIds != null && instrumentIds.Contains(","))
            {
                var instrumentIdList = instrumentIds.Split(',').Select(long.Parse).ToList();

                var paymentInstruments = (from pi in _paymentInstrumentRepository.Table join i in instrumentIdList on pi.Id equals i select pi).ToList();
                if (paymentInstruments.Any(x => x.IsPrimary == true))
                    return false;
                if (paymentInstruments != null)
                {
                    foreach (var item in paymentInstruments)
                    {
                        item.IsActive = isActive == true ? false : true;
                        _paymentInstrumentRepository.Save(item);
                    }
                    return true;
                }
            }
            return false;
        }
        public bool DeleteInstrument(string instrumentIds)
        {
            if (instrumentIds != null && instrumentIds.Contains(","))
            {
                var instrumentIdList = instrumentIds.Split(',').Select(long.Parse).ToList();
                var paymentInstruments = (from pi in _paymentInstrumentRepository.Table join i in instrumentIdList on pi.Id equals i select pi).ToList();
                if (paymentInstruments == null || paymentInstruments.Count == 0)
                {
                    return false;
                }
                if (paymentInstruments.Any(x => x.IsPrimary == true))
                {
                    foreach (var item in paymentInstruments)
                    {
                        var otherPaymentInstruments = _paymentInstrumentRepository.Table.Where(x => x.Id != item.Id && x.FranchiseePaymentProfileId == item.FranchiseePaymentProfileId).
                                               OrderByDescending(x => x.DataRecorderMetaData.DateCreated).ToArray();
                        if (otherPaymentInstruments.Any())
                        {
                            var card = otherPaymentInstruments.FirstOrDefault();
                            if (card == null) return false;
                            SetPrimary(card.Id, item.FranchiseePaymentProfileId);
                        }
                        else return false;
                    }
                }
                bool result = false;
                foreach (var paymentInstrument in paymentInstruments)
                {
                    result = false;
                    var franchiseePaymentProfile = _franchiseePaymentProfileRepository.Get(x => x.Id == paymentInstrument.FranchiseePaymentProfileId);

                    var response = _authorizeNetCustomerProfileService.DeleteCustomerProfile(paymentInstrument.FranchiseePaymentProfile.ProfileTypeId, franchiseePaymentProfile.CMID.ToString(), paymentInstrument.InstrumentProfileId.ToString());

                    if (response != null && response.messages.resultCode == messageTypeEnum.Ok && response.messages.message != null)
                    {
                        _paymentInstrumentRepository.Delete(paymentInstrument.Id);
                        result = true;
                    }
                }
                return result;
            }
            return false;
        }
        public ProcessorResponse SetPrimary(long franchiseeId, long paymentInstrumentId)
        {
            var processorResponse = new ProcessorResponse();
            var paymentInstrumentList = _paymentInstrumentRepository.Table.Where(x => x.FranchiseePaymentProfileId == franchiseeId).ToArray();

            if (paymentInstrumentList.Any())
            {
                var paymentInstrument = paymentInstrumentList.Where(x => x.Id == paymentInstrumentId).FirstOrDefault();
                if (paymentInstrument == null)
                {
                    processorResponse.Message = "error :";
                }
                if (paymentInstrument.IsActive == false)
                {
                    processorResponse.Message = "Can't set Primary. Selected Payment Instrument is Suspended.";
                    processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                    return processorResponse;
                }
                else
                {
                    if (paymentInstrument.InstrumentTypeId == (long)InstrumentType.ChargeCard)
                    {
                        var chargeCard = _chargeCardRepository.Get(x => x.Id == paymentInstrument.InstrumentEntityId);
                        bool result = CheckExpiryDate(chargeCard.ExpiryMonth, chargeCard.ExpiryYear);
                        if (result == false)
                        {
                            processorResponse.Message = "Can't set Primary. Card is Expired.";
                            processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                            return processorResponse;
                        }
                    }
                    paymentInstrument.IsPrimary = true;
                    var existingPrimaryinDb = _paymentInstrumentRepository.Table.Where(x => x.IsPrimary == true && x.FranchiseePaymentProfileId == franchiseeId).FirstOrDefault();
                    if (existingPrimaryinDb != null)
                    {
                        existingPrimaryinDb.IsPrimary = false;
                        _paymentInstrumentRepository.Save(existingPrimaryinDb);
                    }
                    _paymentInstrumentRepository.Save(paymentInstrument);
                    processorResponse.Message = "Payment Instrument has been set as Primary.";
                    return processorResponse;
                }
            }
            return processorResponse;
        }
        public ProcessorResponse SetPrimary(long franchiseeId, string paymentInstrumentIds)
        {
            var processorResponse = new ProcessorResponse();
            if (paymentInstrumentIds != null && paymentInstrumentIds.Contains(","))
            {
                var instrumentIdList = paymentInstrumentIds.Split(',').Select(long.Parse).ToList();
                var paymentInstruments = (from pi in _paymentInstrumentRepository.Table join i in instrumentIdList on pi.Id equals i select pi).ToList();
                if (paymentInstruments == null || paymentInstruments.Count == 0)
                {
                    processorResponse.Message = "error :";
                }
                if (paymentInstruments != null)
                {

                    if (paymentInstruments.Any(x => x.IsActive == false))
                    {
                        processorResponse.Message = "Can't set Primary. Selected Payment Instrument is Suspended.";
                        processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                        return processorResponse;
                    }
                    else
                    {
                        if (paymentInstruments.Any(x => x.InstrumentTypeId == (long)InstrumentType.ChargeCard))
                        {
                            foreach (var paymentInstrument in paymentInstruments)
                            {
                                var chargeCard = _chargeCardRepository.Get(x => x.Id == paymentInstrument.InstrumentEntityId);
                                bool result = CheckExpiryDate(chargeCard.ExpiryMonth, chargeCard.ExpiryYear);
                                if (result == false)
                                {
                                    processorResponse.Message = "Can't set Primary. Card is Expired.";
                                    processorResponse.ProcessorResult = ProcessorResponseResult.Fail;
                                    return processorResponse;
                                }
                            }
                        }
                        foreach (var paymentInstrument in paymentInstruments)
                        {
                            paymentInstrument.IsPrimary = true;
                            var existingPrimaryinDb = _paymentInstrumentRepository.Table.Where(x => x.IsPrimary == true && x.FranchiseePaymentProfileId == paymentInstrument.FranchiseePaymentProfileId).FirstOrDefault();
                            if (existingPrimaryinDb != null)
                            {
                                existingPrimaryinDb.IsPrimary = false;
                                _paymentInstrumentRepository.Save(existingPrimaryinDb);
                            }
                            _paymentInstrumentRepository.Save(paymentInstrument);

                        }
                        processorResponse.Message = "Payment Instrument has been set as Primary.";
                        return processorResponse;

                    }
                }
            }
            return processorResponse;
        }
        public bool CheckExpiryDate(int expireMonth, int expireYear)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;
            if (Convert.ToInt32(expireMonth) < currentMonth && Convert.ToInt32(expireYear) <= currentYear)
                return false;
            else
                return true;
        }
        private long SaveChargeCardAndInstrument(long accountTypeId, ChargeCardEditModel model, ProcessorResponse response, long franchiseeId)
        {
            _logService.Info(string.Format("Start saving Payment Instrument for Franchisee# {0} and AccountType# {1}", franchiseeId, accountTypeId));
            var franchiseePaymentProfile = _franchiseePaymentProfileRepository.Fetch(x => x.FranchiseeId == franchiseeId && x.ProfileTypeId == accountTypeId).FirstOrDefault();
            var chargeCard = _chargeCardService.Save(model);
            var instrumentTypeId = (long)InstrumentType.ChargeCard;
            var paymentInstrument = _chargeCardPaymentProfileFactory.CreateDoamin(franchiseePaymentProfile.Id, instrumentTypeId, response.RawResponse, chargeCard.Id);
            var paymentInstrumentList = _paymentInstrumentRepository.Table.Where(x => x.IsPrimary == true && x.IsActive == true && x.FranchiseePaymentProfileId == franchiseePaymentProfile.Id).FirstOrDefault();
            if (paymentInstrumentList == null)
                paymentInstrument.IsPrimary = true;
            else
                paymentInstrument.IsPrimary = false;
            _paymentInstrumentRepository.Save(paymentInstrument);
            _logService.Info("end saving Payment Instrument.");
            return chargeCard.Id;
        }
        public List<FranchiseePaymentInstrumentViewModel> GetFranchiseeInstrumentList(long franchiseeId)
        {
            var paymentInstrumnets = new List<PaymentInstrumentViewModel>();
            var franchiseePaymentInstrumentList = new List<FranchiseePaymentInstrumentViewModel>();
            var franchiseePaymentProfileList = GetInstrumentList(franchiseeId, 0);

            if (franchiseePaymentProfileList == null && franchiseePaymentProfileList.Count == 0)
            {
                return franchiseePaymentInstrumentList;
            }
            var paymentinstrumentCollection = franchiseePaymentProfileList.GroupBy(x => new
            {
                x.AccountNumber,
                x.AccountTypeId,
                x.Name,
                x.BankName,
                x.Number,
                x.CardType,
                x.ExpirationDate,
                x.IsActive,
                x.IsPrimary
            }).ToList();

            foreach (var item in paymentinstrumentCollection)
            {
                var model = new FranchiseePaymentInstrumentViewModel();
                model.Number = item.Key.Number;
                model.Name = item.Key.Name;
                model.ExpirationDate = item.Key.ExpirationDate;
                model.CardType = item.Key.CardType;
                model.IsActive = item.Key.IsActive;
                model.IsPrimary = item.Key.IsPrimary;
                model.AccountNumber = item.Key.AccountNumber;
                model.BankName = item.Key.BankName;
                model.AccountTypeId = item.Key.AccountTypeId;
                // model.TypeId = item.Key.TypeId;              
                model.InstrumentIds = String.Join(",", item.Select(x => x.PaymentInstrumentId).ToList());
                franchiseePaymentInstrumentList.Add(model);
            }

            return franchiseePaymentInstrumentList;
        }
        private static string Log(string message)
        {
            return DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm:ss:fff tt") + " \t\t " + message + "\r\n\r\n";
        }

    }
}
