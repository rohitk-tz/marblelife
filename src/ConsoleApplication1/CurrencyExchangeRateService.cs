using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Geo.Domain;
using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.IO;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    [DefaultImplementation]
    public class CurrencyExchangeRateService : ICurrencyExchangeRateService
    {
        private readonly string _defaultCurrencyCode = "USD";
        private readonly ILogService _logService;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly IRepository<Country> _countryRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;

        public CurrencyExchangeRateService(IUnitOfWork unitOfWork, ILogService logService, IClock clock, ISettings setting)
        {
            _unitOfWork = unitOfWork;
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _countryRepository = unitOfWork.Repository<Country>();
            _logService = logService;
            _clock = clock;
            _setting = setting;
        }


        public void GetAllCurrencyRate()
        {
            var countryList = _countryRepository.Table.ToList();
            foreach (var item in countryList.Where(x => !x.IsDefault).ToList())
            {
                try
                {
                    var startDate = new DateTime(2016, 01, 01);
                    var endDate = new DateTime(2016, 09, 30);

                    for (DateTime date = startDate; date <= endDate; date = date.AddMonths(1))
                    {
                        Thread.Sleep(2000);
                        var endOfTheMonth = date.AddMonths(1).AddDays(-1);
                        GetRateFromAPIAndSaveAgainstTheDate(item, endOfTheMonth);
                    }

                    startDate = new DateTime(2016, 10, 01);
                    endDate = new DateTime(2017, 01, 13);

                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        GetRateFromAPIAndSaveAgainstTheDate(item, date);
                    }
                }
                catch (Exception e)
                {
                    _logService.Error("Exception - in save Currency Exchange Rate of  " + item.Name, e);
                }
            }
        }

        private void GetRateFromAPIAndSaveAgainstTheDate(Country item, DateTime date)
        {
            decimal exchangeRate = GetCurrencyExchangeRateFromApi(item.CurrencyCode, date);

            _unitOfWork.StartTransaction();
            try
            {
                var currencyExchangeRatedomain = CreateDomain(item.Id, exchangeRate, date);
                _currencyExchangeRateRepository.Save(currencyExchangeRatedomain);
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _logService.Error("Exception - in save Currency Exchange Rate of  " + item.Name + " for date - " + date.ToShortDateString(), e);
            }
        }

        private decimal GetCurrencyExchangeRateFromApi(string currencyCode, DateTime date)
        {
            try
            {
                _logService.Error("Get Currency Exchange Rate from API  " + date.ToString() + "for" + currencyCode);
                string currencyDate = date.Year.ToString() + "-" + date.Month.ToString() + "-" + date.Day.ToString();

                var apiKey = _setting.CurrencyExchengeRateApiKey;
                string urlPattern = _setting.CurrencyExchangeRateApi + "?date=" + currencyDate + "&quote=" + currencyCode + "&api_key=" + apiKey;

                string url = string.Format(urlPattern, currencyCode.ToUpper(), _defaultCurrencyCode);

                string json = "";
                string credentialHeader = string.Format("Bearer" + apiKey);
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.ContentType = "application/json";
                HttpWebResponse webresponse = (HttpWebResponse)request.GetResponse();

                var sw = new StreamReader(webresponse.GetResponseStream(), System.Text.Encoding.ASCII);
                json = sw.ReadToEnd();
                sw.Close();

                var obj = (new JavaScriptSerializer()).Deserialize<RootObject>(json);

                var value = obj.quotes.CAD != null ? obj.quotes.CAD.ask : null;
                if (string.IsNullOrEmpty(value))
                    value = obj.quotes.AED != null ? obj.quotes.AED.ask : null;
                if (string.IsNullOrEmpty(value))
                    value = obj.quotes.BSD != null ? obj.quotes.BSD.ask : null;
                if (string.IsNullOrEmpty(value))
                    value = obj.quotes.KYD != null ? obj.quotes.KYD.ask : null;
                if (string.IsNullOrEmpty(value))
                    value = obj.quotes.ZAR != null ? obj.quotes.ZAR.ask : null;

                decimal exchangeRate = Convert.ToDecimal(Math.Round(
                     double.Parse(value, System.Globalization.CultureInfo.InvariantCulture), 4));

                exchangeRate = Math.Round(1 / exchangeRate, 4);
                return exchangeRate;
            }
            catch (Exception e)
            {
                _logService.Error("Exception - in save Currency Exchange Rate of Currency Code " + currencyCode + " for date " + date.ToString(), e);
                return 0;
            }
        }
        private class Quotes
        {
            public CAD CAD { get; set; }
            public BSD BSD { get; set; }
            public KYD KYD { get; set; }
            public ZAR ZAR { get; set; }
            public AED AED { get; set; }
        }
        private class CurrencyRate
        {
            public string ask { get; set; }
            public string bid { get; set; }
            public string date { get; set; }
        }
        private class CAD : CurrencyRate { }
        private class BSD : CurrencyRate { }
        private class KYD : CurrencyRate { }
        private class ZAR : CurrencyRate { }
        private class AED : CurrencyRate { }
        private class RootObject
        {
            public string base_currency { get; set; }
            public Quotes quotes { get; set; }
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
