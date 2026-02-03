using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Geo.Domain;
using System;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Infrastructure.Billing.Impl
{
    [DefaultImplementation]
    public class CurrencyRateService : ICurrencyRateService
    {
        // private readonly string _defaultCurrencyCode = "USD";
        private readonly ILogService _logService;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;
        public CurrencyRateService(IUnitOfWork unitOfWork, ILogService logService, IClock clock, ISettings setting)
        {
            _unitOfWork = unitOfWork;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _countryRepository = unitOfWork.Repository<Country>();
            _logService = logService;
            _clock = clock;
            _setting = setting;
        }


        public void AllCurrencyRateByDate()
        {
            if (!_setting.GetCurrencyExchangeRate)
            {
                _logService.Info(string.Format("Get CurrencyExchangeRate turned Off for - {0}  ", _clock.UtcNow));
                return;
            }

            _logService.Info(string.Format("starting CurrencyExchange rate Service for - ", _clock.UtcNow));

            var countryList = _countryRepository.Table.ToList();
            foreach (var item in countryList.Where(x => !x.IsDefault).ToList())
            {
                try
                {
                    _unitOfWork.StartTransaction();

                    var date = _clock.UtcNow.Date;
                    var currentDate = _clock.UtcNow.Date;
                    var getHistoricRate = _setting.GetHistoricalRate;
                    if (getHistoricRate)
                    {
                        date = _setting.CurrencyRateStartDate;
                    }

                    for (DateTime d = date; d <= currentDate; d = d.AddDays(1))
                    {
                        decimal exchangeRate = GetCurrencyExchnageRateFromApi(item.CurrencyCode, d);
                        if (exchangeRate != 0)
                        {
                            var currencyExchangeRatedomain = CreateDomain(item.Id, exchangeRate, d);
                            if (item.Id == 4)
                            {
                                currencyExchangeRatedomain.Rate =Convert.ToDecimal(0.84);
                            }
                            _currencyExchangeRateRepository.Save(currencyExchangeRatedomain);
                        }
                        _unitOfWork.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    _logService.Error("Exception - in save Currency Exchange Rate of  " + item.Name, e);
                }
            }
        }

        private decimal GetCurrencyExchnageRateFromApi(string currencyCode, DateTime date)
        {
            try
            {
                var CurrencyDate = date.Date.ToString("yyyy-MM-dd");
                _logService.Info("Get Currency Exchange Rate from API for " + CurrencyDate.ToString() + " and CurrencyCode - " + currencyCode);
                var appId = _setting.ExchangeRateAppId;

                string urlPattern = _setting.CurrencyExchangeRateApi + CurrencyDate + ".json?app_id=" + appId + "&symbols=" + currencyCode;
                string url = string.Format(urlPattern, currencyCode.ToUpper());

                string response = new WebClient().DownloadString(url);

                var responseModel = (new JavaScriptSerializer()).Deserialize<CurrencyExchangeRateViewModel>(response);
                decimal exchangeRate = 0;
                if (responseModel.rates != null)
                {
                    switch (currencyCode)
                    {
                        case "CAD":
                            exchangeRate = 1 / responseModel.rates.CAD;
                            break;
                        case "BSD":
                            exchangeRate = 1 / responseModel.rates.BSD;
                            break;
                        case "AED":
                            exchangeRate = 1 / responseModel.rates.AED;
                            break;
                        case "KYD":
                            exchangeRate = 1 / responseModel.rates.KYD;
                            break;
                        case "ZAR":
                            exchangeRate = 1 / responseModel.rates.ZAR;
                            break;
                        case "MXN":
                            exchangeRate = 1 / responseModel.rates.MXN;
                            break;

                    }
                }

                exchangeRate = Convert.ToDecimal(Math.Round(exchangeRate, 4));
                return exchangeRate;
            }
            catch (Exception e)
            {
                _logService.Error("Exception - in save Currency Exchange Rate of  " + _clock.UtcNow.ToString(), e);
                return 0;
            }
        }

        private CurrencyExchangeRate CreateDomain(long countryId, decimal exchangeRate, DateTime date)
        {
            return new CurrencyExchangeRate
            {
                IsNew = true,
                CountryId = countryId,
                Rate = exchangeRate,
                DateTime = date
            };

        }
    }
}
