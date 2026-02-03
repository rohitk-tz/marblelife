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
    public class CreateEmailRecordForApiService : ICreateEmailRecordForApiService
    {
        private readonly IRepository<CustomerEmailAPIRecord> _customerEmailAPIRecordRepository;
        private readonly IRepository<PartialPaymentEmailApiRecord> _partialcustomerEmailAPIRecordRepository;
        private readonly ILogService _logService;
        private readonly ICustomerEmailAPIRecordFactory _customerEmailAPIRecordFactory;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;

        public CreateEmailRecordForApiService(IUnitOfWork unitOfWork, ISettings settings, ILogService logService, ICustomerEmailAPIRecordFactory customerEmailAPIRecordFactory)
        {
            _customerEmailAPIRecordRepository = unitOfWork.Repository<CustomerEmailAPIRecord>();
            _logService = logService;
            _customerEmailAPIRecordFactory = customerEmailAPIRecordFactory;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _partialcustomerEmailAPIRecordRepository = unitOfWork.Repository<PartialPaymentEmailApiRecord>();
        }

        public void CreateEmailRecord()
        {
            _logService.Info(string.Format("Create Email Record Job Starting"));
            var requestEnabled = _settings.CreateEmailRecord;
            if (!requestEnabled)
            {
                _logService.Info("Can't create Email record On API, as request is disabled!");
                return;
            }

            //var emailList = _customerEmailAPIRecordRepository.Table.Where(x => !(x.IsSynced) && !(x.IsFailed)).ToList();

            //foreach (var item in emailList)
            //{
            //    SendRequestToApi(item);
            //}

            var emailList2 = _partialcustomerEmailAPIRecordRepository.Table.Where(x => !(x.IsSynced) && !(x.IsFailed) && x.statusId == (long)LookupTypes.PartialPayment).ToList();

            foreach (var item in emailList2)
            {
                SendRequestToApiForPartialPayment(item);
            }
            var depositList = _partialcustomerEmailAPIRecordRepository.Table.Where(x => !(x.IsSynced) && !(x.IsFailed) && x.statusId == (long)LookupTypes.Paid).ToList();

            foreach (var item in depositList)
            {
                SendRequestToApiForFullPayment(item);
            }
        }

        private void SendRequestToApiForFullPayment(PartialPaymentEmailApiRecord record)
        {
            try
            {
                var apiKey = _settings.EmailApiKey;
                var regionCode = _settings.RegionCode;
                var listId = _settings.EmailAPIListId;

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(regionCode) || string.IsNullOrEmpty(listId))
                {
                    _logService.Info("Invalid Api Key Information for Deposit Customer!");
                    return;
                }

                var model = _customerEmailAPIRecordFactory.CreateModelForPayment(record);

                string url = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/members", regionCode, listId);

                var jsonString = new JavaScriptSerializer().Serialize(model);
                string result = string.Empty;
                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + apiKey);

                    try
                    {
                        result = client.UploadString(url, "POST", jsonString);
                    }
                    catch (WebException ex)
                    {
                        _logService.Info(string.Format("Web Exception for Partial payment Customer :", ex.Message));
                        var stream = ex.Response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var responseText = sr.ReadToEnd();
                        var responseerror = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(responseText);
                        responseerror.IsFailed = true;

                        _unitOfWork.StartTransaction();

                        var emailRecord = _customerEmailAPIRecordFactory.CreateModelForPartialPayment(responseerror, record);
                        _partialcustomerEmailAPIRecordRepository.Save(emailRecord);

                        _unitOfWork.SaveChanges();

                        if (!string.IsNullOrEmpty(responseerror.detail) && !string.IsNullOrEmpty(responseerror.title))
                            _logService.Info(string.Format("error accured while creating Full payment customer Email record for {0}, ERROR : {1},{2}", record.Customer.Name, responseerror.title, responseerror.detail));
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Exception :", ex.Message));
                        return;
                    }
                }
                var response = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(result);
                try
                {
                    _unitOfWork.StartTransaction();

                    var emailRecord = _customerEmailAPIRecordFactory.CreateModelForPartialPayment(response, record);
                    _partialcustomerEmailAPIRecordRepository.Save(emailRecord);

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Exception :", ex.Message));
                    _unitOfWork.Rollback();
                }
            }
            catch(Exception ex)
            {
                _logService.Info(string.Format("Error in Parsing Full  Payment with  InvoiceId {0} Exception {1} :", record.InvoiceId, ex.Message));
                _unitOfWork.Rollback();
            }
        }
        private void SendRequestToApi(CustomerEmailAPIRecord record)
        {
            var apiKey = _settings.EmailApiKey;
            var regionCode = _settings.RegionCode;
            var listId = _settings.EmailAPIListId;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(regionCode) || string.IsNullOrEmpty(listId))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }

            var model = _customerEmailAPIRecordFactory.CreateModel(record);

            string url = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/members", regionCode, listId);

            var jsonString = new JavaScriptSerializer().Serialize(model);
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + apiKey);

                try
                {
                    result = client.UploadString(url, "POST", jsonString);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Web Exception :", ex.Message));
                    var stream = ex.Response.GetResponseStream();
                    var sr = new StreamReader(stream);
                    var responseText = sr.ReadToEnd();
                    var responseerror = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(responseText);
                    responseerror.IsFailed = true;

                    _unitOfWork.StartTransaction();

                    var emailRecord = _customerEmailAPIRecordFactory.CreateDomain(responseerror, record);
                    _customerEmailAPIRecordRepository.Save(emailRecord);

                    _unitOfWork.SaveChanges();

                    if (!string.IsNullOrEmpty(responseerror.detail) && !string.IsNullOrEmpty(responseerror.title))
                        _logService.Info(string.Format("error accured while creating customer Email record for {0}, ERROR : {1},{2}", record.Customer.Name, responseerror.title, responseerror.detail));
                    return;
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Exception :", ex.Message));
                    return;
                }
            }
            var response = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(result);
            try
            {
                _unitOfWork.StartTransaction();

                var emailRecord = _customerEmailAPIRecordFactory.CreateDomain(response, record);
                _customerEmailAPIRecordRepository.Save(emailRecord);

                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Exception :", ex.Message));
                _unitOfWork.Rollback();
            }
        }

        private void SendRequestToApiForPartialPayment(PartialPaymentEmailApiRecord record)
        {
            try
            {
                var apiKey = _settings.EmailApiKey;
                var regionCode = _settings.RegionCode;
                var listId = _settings.EmailAPIListIdForPartialPayment;

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(regionCode) || string.IsNullOrEmpty(listId))
                {
                    _logService.Info("Invalid Api Key Information for Partial payment Customer!");
                    return;
                }

                var model = _customerEmailAPIRecordFactory.CreateModelForPartialPayment(record);

                string url = string.Format("https://{0}.api.mailchimp.com/3.0/lists/{1}/members", regionCode, listId);

                var jsonString = new JavaScriptSerializer().Serialize(model);
                string result = string.Empty;
                using (var client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.Headers.Add(HttpRequestHeader.Authorization, "Basic " + apiKey);

                    try
                    {
                        result = client.UploadString(url, "POST", jsonString);
                    }
                    catch (WebException ex)
                    {
                        _logService.Info(string.Format("Web Exception for Partial payment Customer :", ex.Message));
                        var stream = ex.Response.GetResponseStream();
                        var sr = new StreamReader(stream);
                        var responseText = sr.ReadToEnd();
                        var responseerror = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(responseText);
                        responseerror.IsFailed = true;

                        _unitOfWork.StartTransaction();

                        var emailRecord = _customerEmailAPIRecordFactory.CreateModelForPartialPayment(responseerror, record);
                        _partialcustomerEmailAPIRecordRepository.Save(emailRecord);

                        _unitOfWork.SaveChanges();

                        if (!string.IsNullOrEmpty(responseerror.detail) && !string.IsNullOrEmpty(responseerror.title))
                            _logService.Info(string.Format("error accured while creating Partial payment customer Email record for {0}, ERROR : {1},{2}", record.Customer.Name, responseerror.title, responseerror.detail));
                        return;
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Exception :", ex.Message));
                        return;
                    }
                }
                var response = (new JavaScriptSerializer()).Deserialize<CustomerEmailAPIRecordResponseModel>(result);
                try
                {
                    _unitOfWork.StartTransaction();

                    var emailRecord = _customerEmailAPIRecordFactory.CreateModelForPartialPayment(response, record);
                    _partialcustomerEmailAPIRecordRepository.Save(emailRecord);

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logService.Info(string.Format("Exception :", ex.Message));
                    _unitOfWork.Rollback();
                }
            }
            catch(Exception ex)
            {
                _logService.Info(string.Format("Error in Parsing Partial Payment with  InvoiceId {0} Exception {1} :", record.InvoiceId, ex.Message));
                _unitOfWork.Rollback();
            }
        }

        public void AddMergeField()
        {
            var apiKey = _settings.EmailApiKey;
            var regionCode = _settings.RegionCode;
            var listId = _settings.EmailAPIListId;

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(regionCode) || string.IsNullOrEmpty(listId))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }
            try
            {
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
            catch (Exception ex)
            {
                _logService.Info(string.Format("Exception :", ex.StackTrace));
            }
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
