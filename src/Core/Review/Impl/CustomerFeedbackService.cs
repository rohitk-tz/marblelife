using Core.Application;
using Core.Application.Attribute;
using Core.Notification;
using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using Core.Sales.Enum;
using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace Core.Review.Impl
{
    [DefaultImplementation]
    public class CustomerFeedbackService : ICustomerFeedbackService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private readonly IRepository<Customer> _customerRepository;
        private readonly ICustomerFeedbackFactory _customerFeedbackFactory;
        private readonly IRepository<CustomerReviewSystemRecord> _customerReviewSystemRecordRepository;
        private readonly IRepository<CustomerFeedbackRequest> _reviewFeedbackRequestRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly ICustomerFeedbackAPIRecordService _customerFeedbackAPIRecordService;

        public CustomerFeedbackService(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, ICustomerFeedbackFactory customerFeedbackFactory,
            IClock clock, ICustomerFeedbackAPIRecordService customerFeedbackAPIRecordService)
        {
            _logService = logService;
            _clock = clock;
            _settings = settings;
            _customerRepository = unitOfWork.Repository<Customer>();
            _customerFeedbackFactory = customerFeedbackFactory;
            _customerReviewSystemRecordRepository = unitOfWork.Repository<CustomerReviewSystemRecord>();
            _reviewFeedbackRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _customerFeedbackAPIRecordService = customerFeedbackAPIRecordService;
        }

        public ReviewAPIResponseModel TriggerEmail(Customer customer, CustomerCreateEditModel customerModel, long franchiseeId, string qBinvoiceId, string customerEmail, long marketingClassId)
        {
            var response = new ReviewAPIResponseModel { };
            var franchisee = _franchiseeRepository.Get(franchiseeId);
            var currentDate = _clock.UtcNow;


            if (!franchisee.IsReviewFeedbackEnabled)
            {
                _logService.Info("Feedback request for the Franchisee is turned off!");
                response.errorMessage = "Feedback request for the Franchisee" + franchisee.Organization.Name + " is turned off!";
                response.errorCode = 101;
                return response;
            }

            var clientId = _settings.ClientId;
            var apiKey = _settings.ReviewApiKey;


            var reviewSystemInfo = _customerFeedbackFactory.CreateDomain(customer, franchisee, default(long));
            _customerReviewSystemRecordRepository.Save(reviewSystemInfo);
            response.ReviewSystemRecordId = reviewSystemInfo.Id;
            return response;
        }

        private ReviewAPIResponseModel CreateCustomerRecord(Customer customer, CustomerCreateEditModel customerModel, Franchisee franchisee, string clientId,
            long marketingClassId, string customerEmail)
        {
            var result = new ReviewAPIResponseModel { };
            if (marketingClassId != (long)MarketingClassType.Residential)
            {
                result = ValidateCustomerName(customerModel.ContactPerson);
            }
            else
                result = ValidateCustomerName(customer.Name, customerModel.ContactPerson);

            //if (!result.IsvalidName)
            //{
            //    //trigger email From System
            //    var emailResponse = _customerFeedbackAPIRecordService.SendEmailFeedbackRequest(customerEmail, customerModel.Name, franchisee);
            //    return emailResponse;
            //}
            var createResponse = CreateCustomer(customer, result, franchisee.BusinessId.Value, clientId);
            if (createResponse.errorCode == 0 && createResponse.CustomerId > 0)
            {
                //save customer Review System Info
                var reviewSystemInfo = _customerFeedbackFactory.CreateDomain(customer, franchisee, createResponse.CustomerId);
                _customerReviewSystemRecordRepository.Save(reviewSystemInfo);
                createResponse.ReviewSystemRecordId = reviewSystemInfo.Id;
            }
            else
                _logService.Info(string.Format("Response :", createResponse.errorMessage));
            return createResponse;
        }


        private ReviewAPIResponseModel UpdateCustomerRecord(CustomerReviewSystemRecord reviewSystemRecord, string clientId, CustomerCreateEditModel customerModel,
            string customerEmail, Franchisee franchisee)
        {
            var validName = true;

            var getResponse = GetCustomer(reviewSystemRecord.ReviewSystemCustomerId, clientId);
            if (!string.IsNullOrEmpty(customerModel.ContactPerson))
            {
                var nameResponse = ValidateCustomerName(customerModel.ContactPerson);
                if (nameResponse.IsvalidName)
                {
                    var reviewSystemName = (getResponse.firstName.ToLower() + getResponse.lastName.ToLower()).Trim();
                    reviewSystemName = reviewSystemName.Replace(" ", "");
                    var fullname = (nameResponse.firstName.ToLower() + nameResponse.lastName.ToLower()).Trim();
                    fullname = fullname.Replace(" ", "");
                    if (!reviewSystemName.Equals(fullname))
                    {
                        validName = false;
                        getResponse.firstName = nameResponse.firstName;
                        getResponse.lastName = nameResponse.lastName;
                    }
                }
            }

            if (!getResponse.email.ToLower().Equals(customerEmail.ToLower()) || !validName)
            {
                //Update CustomerProfile
                var updateResponse = UpdateCustomer(getResponse.CustomerId, getResponse.firstName, getResponse.lastName, customerEmail, franchisee.BusinessId.Value, clientId);
                updateResponse.ReviewSystemRecordId = reviewSystemRecord.Id;
                return updateResponse;
            }
            else
            {
                getResponse.CustomerId = reviewSystemRecord.Id;
                getResponse.ReviewSystemRecordId = reviewSystemRecord.Id;
                return getResponse;
            }
        }

        private ReviewAPIResponseModel ValidateCustomerName(string name, string contactName = null)
        {
            var response = new ReviewAPIResponseModel { };
            response.IsvalidName = true;
            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(contactName))
            {
                response.errorMessage = "Can't Create customer for review system without Name";
                response.IsvalidName = false;
                return response;
            }
            var nameToCompare = name;
            if (!string.IsNullOrEmpty(contactName))
                nameToCompare = contactName;

            var firstLastNamePair = nameToCompare.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            response.lastName = firstLastNamePair[0];
            response.firstName = string.Empty;
            foreach (var item in firstLastNamePair.Skip(1))
            {
                response.firstName += item + " ";
            }

            if (string.IsNullOrEmpty(response.firstName) || string.IsNullOrEmpty(response.lastName))
            {
                firstLastNamePair = nameToCompare.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                response.firstName = firstLastNamePair[0];
                response.lastName = string.Empty;
                foreach (var item in firstLastNamePair.Skip(1))
                {
                    response.lastName += item + " ";
                }
                if (string.IsNullOrEmpty(response.firstName) || string.IsNullOrEmpty(response.lastName))
                {
                    response.errorMessage = string.Format("Can't create customer without full Name for {0}", nameToCompare);
                    response.IsvalidName = false;
                    return response;
                }
                return response;
            }
            return response;
        }

        private ReviewAPIResponseModel UpdateCustomer(long customerId, string firstName, string lastName, string customerEmail, long businessId, string clientId)
        {
            var model = _customerFeedbackFactory.CreateModel(customerEmail, firstName, lastName, businessId, clientId, customerId);
            model.clientId = clientId;

            string[] collection = new string[] { "businessId" +  model.businessId, "clientId" + clientId,  "customerEmail" + model.customerEmail,
                                    "customerFirstName" + model.customerFirstName, "customerId" + model.customerId,
                                    "customerLastName" + model.customerLastName };

            var hash = GenerateHash(collection);
            model.hash = hash;

            //const string UrlPattern = "https://getfivestars.com/api/customer/update";
            const string UrlPattern = "https://app.gatherup.com/api/customer/update";
            var jsonString = new JavaScriptSerializer().Serialize(model);
            string url = string.Format(UrlPattern);

            var response = SendRequestToApi(url, jsonString);
            return response;
        }

        private ReviewAPIResponseModel CreateCustomer(Customer customer, ReviewAPIResponseModel responseModel, long businessId, string clientId)
        {
            var model = _customerFeedbackFactory.CreateModel(customer, responseModel, businessId);
            model.clientId = clientId;

            string[] collection = new string[] { "businessId" +  model.businessId, "clientId" + clientId,  "customerEmail" + model.customerEmail,  "customerFirstName"
                                 + model.customerFirstName, "customerLastName" + model.customerLastName, "customerPhone" + model.customerPhone, "sendFeedbackRequest" + model.sendFeedbackRequest,
                                "communicationPreference" + model.communicationPreference, "customerCustomId" + model.customerCustomId};

            var hash = GenerateHash(collection);
            model.hash = hash;

            //const string UrlPattern = "https://getfivestars.com/api/customer/create";
            const string UrlPattern = "https://app.gatherup.com/api/customer/create";
            var jsonString = new JavaScriptSerializer().Serialize(model);
            string url = string.Format(UrlPattern);

            var response = SendRequestToApi(url, jsonString);
            return response;
        }

        public ReviewAPIResponseModel GetCustomer(long customerId, string clientId)
        {
            var model = _customerFeedbackFactory.CreateModel(customerId, clientId);
            string[] collection = new string[] { "clientId" + model.clientId, "customerId" + model.customerId };

            var hash = GenerateHash(collection);
            model.hash = hash;

            //const string UrlPattern = "https://getfivestars.com/api/customer/get";
            const string UrlPattern = "https://app.gatherup.com/api/customer/get";
            var jsonString = new JavaScriptSerializer().Serialize(model);
            string url = string.Format(UrlPattern);

            var response = SendRequestToApi(url, jsonString);
            response.CustomerId = customerId;
            return response;
        }

        public ReviewAPIResponseModel SendFeedbackRequest(long customerId, string clientId)
        {
            var model = _customerFeedbackFactory.CreateModel(customerId, clientId);
            string[] collection = new string[] { "clientId" + model.clientId, "customerId" + model.customerId };

            var hash = GenerateHash(collection);
            model.hash = hash;

            //const string UrlPattern = "https://getfivestars.com/api/customer/feedback/send";
            const string UrlPattern = "https://app.gatherup.com/api/customer/feedback/send";

            var jsonString = new JavaScriptSerializer().Serialize(model);
            string url = string.Format(UrlPattern);

            var response = SendRequestToApi(url, jsonString);
            response.DataPacket = jsonString;
            return response;
        }

        private ReviewAPIResponseModel SendRequestToApi(string url, string jsonString)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                result = client.UploadString(url, "POST", jsonString);
            }
            var response = (new JavaScriptSerializer()).Deserialize<ReviewAPIResponseModel>(result);
            return response;
        }

        private string GenerateHash(string[] collection)
        {
            var apiKey = _settings.ReviewApiKey;

            string paramText = string.Join("", collection.OrderBy(x => x));

            var hash = Sha256_hash(apiKey + paramText);
            return hash;
        }

        public static string Sha256_hash(string value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                return string.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }

        //public ReviewAPIResponseModel CreateBusiness(string clientId)
        //{
        //    var model = new CreateBusinessForReviewModel
        //    {
        //        businessName = "Marblelife Test Business 2",
        //        businessOwnerEmail = "srikeertisingh@yopmail.com",
        //        clientId = clientId,
        //        city = "Haiku",
        //        country = "United States",
        //        phone = "9999999999",
        //        state = "HI",
        //        streetAddress = "4150 Hana Hwy",
        //        zip = "96708",
        //        package = "basic"
        //    };

        //    string[] collection = new string[] { "businessName" +  model.businessName, "businessOwnerEmail" + model.businessOwnerEmail, "clientId"
        //                                       + model.clientId, "city" + model.city,  "country" + model.country, "phone" + model.phone, "state"
        //                                       + model.state, "streetAddress"+ model.streetAddress,"zip" + model.zip, "package" + model.package};

        //    var hash = GenerateHash(collection);
        //    model.hash = hash;

        //    const string urlPattern = "https://getfivestars.com/api/business/create";
        //    var jsonString = new JavaScriptSerializer().Serialize(model);
        //    string url = string.Format(urlPattern);

        //    var response = SendRequestToApi(url, jsonString);
        //    return response;
        //}

        //public ReviewAPIResponseModel GetBusinessInformation(int businessId, string clientId)
        //{
        //    var model = new GetBusinessReviewModel 
        //    {
        //       // businessId = businessId,
        //        clientId = clientId
        //    };

        //    string[] collection = new string[] { "businessId" + model.businessId, "clientId" + model.clientId };

        //    var hash = GenerateHash(collection);
        //    model.hash = hash;

        //    const string urlPattern = "https://getfivestars.com/api/business/get";
        //    var jsonString = new JavaScriptSerializer().Serialize(model);
        //    string url = string.Format(urlPattern);

        //    var response = SendRequestToApi(url, jsonString);
        //    return response;
        //}

        //public ReviewAPIResponseModel GetListOfBusiness(string clientId)
        //{
        //    var model = new GetBusinessReviewModel
        //    {
        //        clientId = clientId,
        //        page = 1
        //    };

        //    string[] collection = new string[] { "clientId" + model.clientId, "page" + model.page };

        //    var hash = GenerateHash(collection);
        //    model.hash = hash;

        //    const string urlPattern = "https://getfivestars.com/api/businesses/get";
        //    var jsonString = new JavaScriptSerializer().Serialize(model);
        //    string url = string.Format(urlPattern);

        //    var response = SendRequestToApi(url, jsonString);
        //    return response;
        //}

        public ReviewPushResponseListModel GetFeedback(string clientId, long businessId, string from, string to)
        {
            var apiKey = _settings.ReviewPushApiKey;

            //string urlPattern = "https://getfivestars.com/api/feedbacks/get";
            string urlPattern = string.Format("https://marblelife.com/ziplocator/API/getreviews/sdate/{0}/edate/{1}/token/{2}", from,to, apiKey);

            string url = string.Format(urlPattern);

            var response = SendRequestToApiForFeedback(url);
            return response;
        }

        public ReviewPushResponseListModel GetFeedbackForAllData()
        {
            var response = new ReviewPushResponseListModel();
            var apiKey = _settings.ReviewPushApiKey;
            string url = string.Format("https://marblelife.com/ziplocator/API/getreviews/token/{0}", apiKey);

            using (var client = new WebClient())
            {
                try
                {
                    var result = client.DownloadString(url);
                    response = (new JavaScriptSerializer()).Deserialize<ReviewPushResponseListModel>(result);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Franchisee With RPID Exception :", ex.Message));
                }
            }
            return response;
        }

        private ReviewPushResponseListModel SendRequestToApiForFeedback(string url)
        {
            string result = string.Empty;
            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                result = client.DownloadString(url);
            }
            var response = (new JavaScriptSerializer()).Deserialize<ReviewPushResponseListModel>(result);
            return response;
        }

        private FeedbackResponseListModel ParseJson(FeedbackResponseListModel response, string result)
        {
            const string Rating = "rating";
            const string Recommend = "recommend";
            const string DateOfReview = "dateOfReview";
            const string ShowReview = "showReview";
            const string Body = "body";
            const string CustomId = "customId";
            const string AuthorEmail = "authorEmail";
            const string FeedbackId = "feedbackId";

            response.reviews = new List<FeedbackResponseViewModel>();

            var reviews = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(result);

            for (int i = 1; i <= response.count; i++)
            {
                var matchString = CustomId + i.ToString();
                var customerId = reviews.FirstOrDefault(t => t.Key.Contains(matchString));
                if (customerId.Key == null || customerId.Value == null)
                    break;
                else
                {
                    var ratingValue = Convert.ToInt16(reviews[Rating + i.ToString()]);
                    var recommendValue = Convert.ToInt16(reviews[Recommend + i.ToString()]);
                    var dateOfReviewValue = reviews[DateOfReview + i.ToString()];
                    bool showReviewValue = Convert.ToInt16(reviews[ShowReview + i.ToString()]) == 1 ? true : false;
                    var bodyValue = reviews[Body + i.ToString()];
                    var authorEmailValue = reviews[AuthorEmail + i.ToString()];
                    var feedbackIdValue = Convert.ToInt64(reviews[FeedbackId + i.ToString()]);

                    var model = new FeedbackResponseViewModel
                    {
                        customId = Convert.ToInt64(customerId.Value),
                        rating = ratingValue,
                        recommend = recommendValue,
                        body = bodyValue,
                        authorEmail = authorEmailValue,
                        dateOfReview = dateOfReviewValue,
                        showReview = showReviewValue,
                        FeedbackId = feedbackIdValue
                    };
                    response.reviews.Add(model);
                }
            }
            return response;
        }
    }
}
