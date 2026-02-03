using Core.Application;
using Core.Application.Attribute;
using Core.Application.Enum;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Review.Domain;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class CreateMergeRecordForApiService : ICreateMergeRecordForApiService
    {
        private readonly IRepository<CustomerEmailAPIRecord> _customerEmailAPIRecordRepository;
        private readonly IRepository<PartialPaymentEmailApiRecord> _partialcustomerEmailAPIRecordRepository;
        private readonly ILogService _logService;
        private readonly ICustomerEmailAPIRecordFactory _customerEmailAPIRecordFactory;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private readonly IClock _clock;

        public CreateMergeRecordForApiService(IUnitOfWork unitOfWork, ISettings settings, ILogService logService, ICustomerEmailAPIRecordFactory customerEmailAPIRecordFactory, IClock clock)
        {
            _customerEmailAPIRecordRepository = unitOfWork.Repository<CustomerEmailAPIRecord>();
            _logService = logService;
            _customerEmailAPIRecordFactory = customerEmailAPIRecordFactory;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _partialcustomerEmailAPIRecordRepository = unitOfWork.Repository<PartialPaymentEmailApiRecord>();
            _clock = clock;
        }
        public void AddMergeField()
        {
            try
            {
                if (!_settings.GetMergeField)
                {
                    _logService.Info(string.Format("Merging Field Job for Mail Chimp is Turned Off - {0}  ", _clock.UtcNow));
                    return;
                }

                _logService.Info(string.Format("starting Merging Field Job Service for - ", _clock.UtcNow));
                mergeLinkForPartialPayment();
                mergeLink();
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Exception :", ex.StackTrace));
            }
        }
        private void mergeFranchisee()
        {
            var apiKey = _settings.EmailApiKey;
            var regionCode = _settings.RegionCode;
            var listId = _settings.EmailAPIListId;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(regionCode) || string.IsNullOrEmpty(listId))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            var model = new EmailApiMergeFieldModel { };
            const string Franchisee = "Franchisee";
            const string Type = "text";
            const string Tag = "Franchisee";
            model.name = Franchisee;
            model.type = Type;
            model.tag = Tag;

            string url = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/merge-fields", regionCode, listId);

            var jsonString = new JavaScriptSerializer().Serialize(model);
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + apiKey);
                result = client.UploadString(url, "POST", jsonString);
            }


            var response = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(result);

            if (!string.IsNullOrEmpty(response.detail) && !string.IsNullOrEmpty(response.title))
                _logService.Info(string.Format("error accured while creating Merge Field {1} For List : {2}, ERROR : {3}", Franchisee, response.title, listId, response.detail));


        }
        private void mergeLink()
        {
            var apiKey = _settings.EmailApiKey;
            var regionCode = _settings.RegionCode;
            var listId = _settings.EmailAPIListId;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(regionCode) || string.IsNullOrEmpty(listId))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            var model = new EmailApiMergeFieldModel { };
            const string Franchisee = "ReviewLink";
            const string Type = "text";
            const string Tag = "ReviewLink";
            model.name = Franchisee;
            model.type = Type;
            model.tag = Tag;

            string url = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/merge-fields", regionCode, listId);

            var jsonString = new JavaScriptSerializer().Serialize(model);
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + apiKey);
                result = client.UploadString(url, "POST", jsonString);
            }

            var response = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(result);

            if (!string.IsNullOrEmpty(response.detail) && !string.IsNullOrEmpty(response.title))
                _logService.Info(string.Format("error accured while creating Merge Field {1} For List : {2}, ERROR : {3}", Franchisee, response.title, listId, response.detail));
        }
        private void mergeLinkForPartialPayment()
        {
            var apiKey = _settings.EmailApiKey;
            var regionCode = _settings.RegionCode;
            var listId = _settings.EmailAPIListIdForPartialPayment;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(regionCode) || string.IsNullOrEmpty(listId))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            var model = new EmailApiMergeFieldModel { };
            const string Franchisee = "ReviewLink";
            const string Type = "text";
            const string Tag = "ReviewLink";
            model.name = Franchisee;
            model.type = Type;
            model.tag = Tag;

            string url = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/merge-fields", regionCode, listId);

            var jsonString = new JavaScriptSerializer().Serialize(model);
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + apiKey);
                result = client.UploadString(url, "POST", jsonString);
            }

            var response = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(result);

            if (!string.IsNullOrEmpty(response.detail) && !string.IsNullOrEmpty(response.title))
                _logService.Info(string.Format("error accured while creating Merge Field {1} For List : {2}, ERROR : {3}", Franchisee, response.title, listId, response.detail));
        }
    }

}
