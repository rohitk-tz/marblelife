using System.Collections.Generic;
using Core.Application;
using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Application.Attribute;
using Core.Application.Domain;
using System.Linq;
using Core.Billing.Enum;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class FranchiseeSalesPaymentService : IFranchiseeSalesPaymentService
    {
        private readonly IRepository<AccountCreditPayment> _accountCreditPaymentRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IPaymentFactory _paymentFactory;
        private readonly IRepository<Lookup> _lookupRepository;

        public FranchiseeSalesPaymentService(IUnitOfWork unitOfWork, IPaymentFactory paymentFactory)
        {
            _paymentRepository = unitOfWork.Repository<Payment>();
            _paymentFactory = paymentFactory;
            _lookupRepository = unitOfWork.Repository<Lookup>();
            _accountCreditPaymentRepository= unitOfWork.Repository<AccountCreditPayment>();
        }
        public Payment Save(FranchiseeSalesPaymentEditModel model)
        {
            var payment = _paymentFactory.CreateDomain(model);
            _paymentRepository.Save(payment);
            return payment;
        }

        public ICollection<FranchiseeSalesPaymentEditModel> CreatePaymentModelCollection(ICollection<InvoicePayment> list)
        {
            var collection = new List<FranchiseeSalesPaymentEditModel>();
            foreach (var item in list)
            {
                var accountCreditPayments = _accountCreditPaymentRepository.Table.Where(x => x.PaymentId == item.PaymentId).ToList();

                var accountCreditEditModel = accountCreditPayments.Select(x => _paymentFactory.CreateViewModel(x)).ToList();
                string instrumentType = null;
                if (item.Payment.InstrumentTypeId >= 1)
                {
                    var lookupItem = _lookupRepository.Get(item.Payment.InstrumentTypeId);
                    if (lookupItem != null)
                        instrumentType = lookupItem.Name;
                }
                var paymentModel = _paymentFactory.CreateViewModel(item.Payment, instrumentType, accountCreditEditModel);
                collection.Add(paymentModel);
            }

            return collection;
        }

        public string GetPaymentInstrument(ICollection<InvoicePayment> list)
        {
            string instrumentType = null;
            if (list.Any())
            {
                var paymentMode = list.Where(x => x.Payment.InstrumentTypeId != (long)InstrumentType.AccountCredit).FirstOrDefault();
                if (paymentMode != null)
                {
                    var lookupItem = _lookupRepository.Get(paymentMode.Payment.InstrumentTypeId);
                    if (lookupItem != null)
                        instrumentType = lookupItem.Name;
                }
            }
            return instrumentType;
        }

        public PaymentModeDetailViewModel GetCheckDetails(ICollection<InvoicePayment> list)
        {
            var paymentDetail = new PaymentModeDetailViewModel();
            if (list.Any())
            {
                var paymentInstrument = list.Where(x => x.Payment.InstrumentTypeId != (long)InstrumentType.AccountCredit).FirstOrDefault();
                if (paymentInstrument != null)
                {
                    if (paymentInstrument.Payment.ChargeCardPayment != null && paymentInstrument.Payment.ChargeCardPayment.ChargeCard != null)
                    {
                        var chargeCardNumber = paymentInstrument.Payment.ChargeCardPayment.ChargeCard.Number;
                        paymentDetail.QBPaymentDetail = "XXXX - " + (chargeCardNumber.Length > 4 ? chargeCardNumber.Substring(chargeCardNumber.Length - 4, 4) : chargeCardNumber);
                        paymentDetail.PaymentDetail = paymentInstrument.Payment.ChargeCardPayment.ChargeCard.CardType.Name + " - " + paymentInstrument.Payment.ChargeCardPayment.ChargeCard.Number;
                    }
                    if (string.IsNullOrEmpty(paymentDetail.QBPaymentDetail) && paymentInstrument.Payment.ECheckPayment != null)
                    {
                        var echeckNumber = paymentInstrument.Payment.ECheckPayment.ECheck.RoutingNumber;
                        paymentDetail.QBPaymentDetail = "XXXX - " + (echeckNumber.Length > 4 ? echeckNumber.Substring(echeckNumber.Length - 4, 4) : echeckNumber);
                        paymentDetail.PaymentDetail = paymentInstrument.Payment.ECheckPayment.ECheck.Lookup.Name + " - " + paymentInstrument.Payment.ECheckPayment.ECheck.RoutingNumber;
                    }
                    if (string.IsNullOrEmpty(paymentDetail.QBPaymentDetail) && paymentInstrument.Payment.CheckPayment != null)
                    {
                        var checkNumber = paymentInstrument.Payment.CheckPayment.Check.CheckNumber;
                        paymentDetail.QBPaymentDetail = "XXXX - " + (checkNumber.Length > 4 ? checkNumber.Substring(checkNumber.Length - 4, 4) : checkNumber);
                        var checkType = paymentInstrument.Payment.CheckPayment.Check.Lookup != null ? paymentInstrument.Payment.CheckPayment.Check.Lookup.Name : "Check - ";
                        paymentDetail.PaymentDetail = checkType + " - " + paymentInstrument.Payment.CheckPayment.Check.CheckNumber;
                    }
                }
            }
            return paymentDetail;
        }

        public decimal GetAccountCreditAmount(ICollection<InvoicePayment> list)
        {
            if (!list.Any()) return 0;
            var accountCredit = list.Where(x => x.Payment.InstrumentTypeId == (long)InstrumentType.AccountCredit);
            if (!accountCredit.Any()) return 0;
            return accountCredit.Sum(x => x.Payment.Amount);
        }
    }
}
