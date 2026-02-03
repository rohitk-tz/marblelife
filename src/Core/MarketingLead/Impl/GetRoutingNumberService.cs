using Core.Application;
using Core.Application.Attribute;
using Core.MarketingLead.Domain;
using Core.MarketingLead.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;

namespace Core.MarketingLead.Impl
{
    [DefaultImplementation]
    public class GetRoutingNumberService : IGetRoutingNumberService
    {
        private readonly ILogService _logService;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<RoutingNumber> _routingNumberRepository;
        private readonly IMarketingLeadsFactory _marketingLeadsFactory;
        private IClock _clock;

        public GetRoutingNumberService(IUnitOfWork unitOfWork, ISettings settings, ILogService logService, IClock clock, IMarketingLeadsFactory marketingLeadsFactory)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _routingNumberRepository = unitOfWork.Repository<RoutingNumber>();
            _marketingLeadsFactory = marketingLeadsFactory;
            _clock = clock;
        }
        public void GetRoutingNumber()
        {
            if (_settings.GetRoutingNumbers)
            {
                _logService.Info(string.Format("Getting Routing Numbers - {0}", _clock.UtcNow));
                GetRoutingPhoneNumbers();
            }
        }

        private void GetRoutingPhoneNumbers()
        {
            var access_key = _settings.AccessKey;
            var secret_key = _settings.SecretKey;

            if (string.IsNullOrEmpty(access_key) || string.IsNullOrEmpty(secret_key))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            var result = GetRoutingNumberFromAPI(access_key, secret_key);

            RoutingNumberList list = new RoutingNumberList();
            XmlSerializer serializer = new XmlSerializer(typeof(RoutingNumberList));
            using (TextReader reader = new StringReader(result))
            {
                list = (RoutingNumberList)serializer.Deserialize(reader);
            }
            if (list == null || list.record.Count < 1)
            {
                _logService.Info(string.Format("No Data Found!"));
                return;
            }

            foreach (var item in list.record)
            {
                if (!string.IsNullOrEmpty(item.PhoneNumber) && !string.IsNullOrEmpty(item.PhoneLabel))
                {
                    try
                    {
                        var routingNumber = _routingNumberRepository.Table.Where(x => x.PhoneNumber.Equals(item.PhoneNumber) && x.PhoneLabel.Trim().ToLower().Equals(item.PhoneLabel.Trim().ToLower())).FirstOrDefault();
                        if (routingNumber != null)
                            continue;

                        var domain = _marketingLeadsFactory.CreateDomain(item.PhoneNumber, item.PhoneLabel);
                        _unitOfWork.StartTransaction();
                        _routingNumberRepository.Save(domain);
                        _unitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _unitOfWork.Rollback();
                        _logService.Info(string.Format("Error saving Routing Number - {0}", ex.Message));
                        continue;
                    }
                }
            }
        }

        private string GetRoutingNumberFromAPI(string access_key, string secret_key)
        {
            string url = string.Format("https://secure.dialogtech.com/ibp_api.php?access_key={0}&secret_access_key={1}&action=routing.numbers&format=xml", access_key, secret_key);

            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/xml");
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Web Exception :", ex.Message));
                }
            }
            return result;
        }
    }
}
