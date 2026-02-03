using Core.Application;
using Core.Application.Attribute;
using Core.Billing;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using System;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeTechnicianMailService : IFranchiseeTechnicianMailService
    {
        private ILogService _logService;
        private readonly IClock _clock;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInvoiceItemFactory _invoiceItemFactory;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<FranchiseeFeeEmailInvoiceItem> _franchiseeFeeEmailInvoiceItemRepository;
        public FranchiseeTechnicianMailService(IUnitOfWork unitOfWork, IClock clock, ILogService logService, IInvoiceItemFactory invoiceItemFactory)
        {
            _unitOfWork = unitOfWork;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _clock = clock;
            _logService = logService;
            _invoiceItemFactory = invoiceItemFactory;
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _franchiseeFeeEmailInvoiceItemRepository = unitOfWork.Repository<FranchiseeFeeEmailInvoiceItem>();
        }
        public InvoiceItemEditModel CreateModel(FranchiseeTechMailService mailServiceFee, FranchiseeInvoice invoice, DateTime? startdate = null, DateTime? endDate = null)
        {
            var description = "";
            if (mailServiceFee.IsGeneric)
            {
                description = "TECH EMAIL-RACKSPACE Franchisee Tech Email Fees Charged On " + _clock.UtcNow.Date.ToString("MM/dd/yyyy");
            }
            else
            {
                description = "TECH EMAIL-RACKSPACE Franchisee Tech Email Fees Charged On " + _clock.UtcNow.Date.ToString("MM/dd/yyyy") ;

            }
            var currencyExchangeRate = GetCurrencyExchangeRate(invoice.Franchisee, _clock.UtcNow);
            var model = new InvoiceItemEditModel
            {
                ItemTypeId = (long)InvoiceItemType.FranchiseeTechMail,
                InvoiceId = invoice.InvoiceId,
                Quantity = 1,
                Rate = (decimal)mailServiceFee.Amount,
                Description =  description,
                Amount = (decimal)mailServiceFee.Amount,
                CurrencyExchangeRateId = currencyExchangeRate.Id,
            };
            return model;
        }
        private CurrencyExchangeRate GetCurrencyExchangeRate(Franchisee franchisee, DateTime date)
        {
            long countryId = franchisee.Organization.Address != null ? franchisee.Organization.Address.First().CountryId : 0;

            var currencyExchangeRate = new CurrencyExchangeRate();

            currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId && x.DateTime.Year == date.Year
                                    && x.DateTime.Month == date.Month
                                    && x.DateTime.Day == date.Day).OrderByDescending(y => y.DateTime).FirstOrDefault();

            if (currencyExchangeRate == null)
                currencyExchangeRate = _currencyExchangeRateRepository.Fetch(x => x.CountryId == countryId).OrderByDescending(y => y.DateTime).First();
            return currencyExchangeRate;
        }

        public long Save(InvoiceItemEditModel model, FranchiseeTechMailService mailServiceFee, long invoiceId)
        {
            _logService.Info(string.Format("Start Adding Service Fee {0}  {1} - {2} ", mailServiceFee.Id, mailServiceFee.FranchiseeId, invoiceId));

            _unitOfWork.StartTransaction();

            var domain = _invoiceItemFactory.CreateDomain(model);

            _invoiceItemRepository.Save(domain);

            var franchiseeFeeEmailInvoice = CreateDomain(domain);
            var isPresent = _franchiseeFeeEmailInvoiceItemRepository.Get(domain.Id);
            if (isPresent == null)
            {
                franchiseeFeeEmailInvoice.IsNew = true;
            }
            else
            {
                franchiseeFeeEmailInvoice.IsNew = false;
            }
            _franchiseeFeeEmailInvoiceItemRepository.Save(franchiseeFeeEmailInvoice);

            _unitOfWork.SaveChanges();
            _logService.Info(string.Format("End Adding Late Fee {0}  {1} - {2} ", mailServiceFee.Id, mailServiceFee.FranchiseeId, invoiceId));
            return domain.Id;
        }

        private FranchiseeFeeEmailInvoiceItem CreateDomain(InvoiceItem invoiceItem)
        {
            var currentDate = DateTime.Now;
            var startDate = new DateTime(currentDate.Year, currentDate.Month, 1);
            var endDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            var franchiseeFeeEmail = new FranchiseeFeeEmailInvoiceItem()
            {
                Amount = invoiceItem.Amount,
                StartDate = startDate,
                EndDate = endDate,
                Id = invoiceItem.Id,
                Percentage = 0
            };
            return franchiseeFeeEmail;
        }
    }
}
