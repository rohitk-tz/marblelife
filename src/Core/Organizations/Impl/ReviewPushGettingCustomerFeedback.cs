using Core.Application;
using Core.Application.Attribute;
using Core.MarketingLead.Domain;
using Core.Organizations.ViewModel;
using Core.Review.Domain;
using Core.Sales.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class ReviewPushGettingCustomerFeedback : IReviewPushGettingCustomerFeedback
    {
        private ILogService _logService;
        private ISettings _settings;
        private IClock _clock;
        private IUnitOfWork _unitOfWork;
        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbackRequestRepository;
        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        private readonly IRepository<FranchsieeGoogleReviewUrlAPI> _franchsieeGoogleReviewUrlAPIRepository;
        public ReviewPushGettingCustomerFeedback(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _settings = settings;
            _clock = clock;
            _customerFeedbackRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _franchsieeGoogleReviewUrlAPIRepository = unitOfWork.Repository<FranchsieeGoogleReviewUrlAPI>();
            _customerFeedbackResponseRepository = unitOfWork.Repository<CustomerFeedbackResponse>();
        }
        public void ProcessRecords()
        {
            //getttingCustomerReviews();
            gettingGoogleAPIReviews();


        }

        private void getttingCustomerReviews()
        {

            _logService.Info(string.Format("Starting Customer Response From Review Push"));
            var apiKey = _settings.ReviewPushApiKey;

            if (string.IsNullOrEmpty(apiKey))
            {
                _logService.Info("Invalid Api Key Information!");
                return;
            }
            var result = GetCustomerFeedbackfromAPI(apiKey);
            //var response = (new JavaScriptSerializer()).Deserialize<ReviewPushCustomerFeedbackListModel>(result);
            var response = JsonConvert.DeserializeObject<ReviewPushCustomerFeedbackListModel>(result);
            if (response == null)
            {
                return;
            }
            var responseData = response.info;
            SaveCustomerResponse(responseData);
            _logService.Info(string.Format("Ending Customer Response From Review Push"));

        }
        private string GetCustomerFeedbackfromAPI(string apiKey)
        {
            string url = string.Format("https://marblelife.com/ziplocator/API/getReviewResponse/status/1/token/{0}", apiKey);

            var result = string.Empty;
            using (var client = new WebClient())
            {
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Franchisee With RPID Exception :", ex.Message));
                }
            }
            return result;
        }




        private bool SaveCustomerResponse(List<ReviewPushCustomerFeedbackViewModel> info)
        {
            foreach (var reviewPushCustomerFeedback in info)
            {
                try
                {
                    _unitOfWork.StartTransaction();
                    if (reviewPushCustomerFeedback.Review_id == null) continue;
                    if (Convert.ToBoolean(reviewPushCustomerFeedback.FromQA) != _settings.IsFromQA) continue;
                    var customerResponse = _customerFeedbackRequestRepository.Get(reviewPushCustomerFeedback.Review_id.GetValueOrDefault());
                    if (customerResponse != null)
                    {
                        customerResponse.DataPacket = reviewPushCustomerFeedback.Response;
                        customerResponse.IsQueued = false;
                        _customerFeedbackRequestRepository.Save(customerResponse);
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Getting Customer Response For Id {0}  With  Exception :{1}", reviewPushCustomerFeedback.Review_id, ex.Message));
                }
            }
            return true;
        }

        private void gettingGoogleAPIReviews()
        {
            var feedbacksFromGoogleReview = _customerFeedbackResponseRepository.Table.Where(x => x.IsFromGoogleAPI).ToList();
            _logService.Info(string.Format("Starting Customer Response From Google API"));
            var franchsieeGoogleReviewUrlAPIList = _franchsieeGoogleReviewUrlAPIRepository.Table.Where(x => x.BrightLocalLink != "").ToList();

            foreach (var franchsieeGoogleReviewUrlAPI in franchsieeGoogleReviewUrlAPIList)
            {
                _logService.Info(string.Format("Starting Customer Response From  Google API For Franchisee {0} ", franchsieeGoogleReviewUrlAPI.FranchiseeId));
                var review = GetCustomerFeedbackfromGoogleAPI(franchsieeGoogleReviewUrlAPI.BrightLocalLink);
                var response = JsonConvert.DeserializeObject<ReviewPushCustomerGoogleFeedbackListModel>(review);
                if (response.issuccess)
                {
                    SaveCustomerGoogleResponse(response.results, franchsieeGoogleReviewUrlAPI.FranchiseeId, feedbacksFromGoogleReview);
                }
            }
        }

        private bool SaveCustomerGoogleResponse(List<ReviewPushCustomerGoogleFeedbackViewModel> reviewList, long franchiseeId, List<CustomerFeedbackResponse> list)
        {
            foreach (var review in reviewList)
            {
                var isReviewPresent = list.Any(x => x.ResponseContent == review.reviewBody && x.CustomerName == review.author);
                if (isReviewPresent)
                {
                    continue;
                }
                var customerReviewResponse = new CustomerFeedbackResponse()
                {
                    DateOfReview = _clock.ToUtc(review.datePublished),
                    CustomerName = review.author,
                    FranchiseeId = franchiseeId,
                    DateOfDataInDataBase = _clock.ToUtc(DateTime.Now),
                    FeedbackId = null,
                    IsFromGoogleAPI = true,
                    IsFromNewReviewSystem = true,
                    IsFromSystemReviewSystem = false,
                    AuditActionId = (long)AuditActionType.Pending,
                    Rating = long.Parse(review.ratingValue),
                    Recommend = review.ratingValue != null ? long.Parse(review.ratingValue) + 5 : 0,
                    IsNew = true,
                    Url = "",
                    ResponseContent = review.reviewBody,

                };
                _customerFeedbackResponseRepository.Save(customerReviewResponse);
                _unitOfWork.SaveChanges();
            }
            return true;
        }

        private string GetCustomerFeedbackfromGoogleAPI(string apiUrl)
        {
            string url = apiUrl;

            var result = string.Empty;
            using (var client = new WebClient())
            {
                try
                {
                    result = client.DownloadString(url);
                }
                catch (WebException ex)
                {
                    _logService.Info(string.Format("Franchisee With RPID Exception :", ex.Message));
                }
            }
            return result;
        }
    }
}
