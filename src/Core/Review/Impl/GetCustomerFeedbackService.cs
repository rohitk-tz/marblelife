using Core.Application;
using Core.Application.Attribute;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Review.Impl
{
    [DefaultImplementation]
    public class GetCustomerFeedbackService : IGetCustomerFeedbackService
    {
        private readonly IClock _clock;
        private readonly ISettings _settings;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerFeedbackService _customerReviewService;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly ICustomerFeedbackFactory _customerFeedbackFactory;
        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbcakRequestRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerEmail> _customerEmailRepository;
        private readonly IRepository<ReviewPushCustomerFeedback> _reviewPushCustomerFeedbackRepository;
        private readonly IRepository<ReviewPushAPILocation> _reviewPushAPILocationRepository;
        public GetCustomerFeedbackService(IClock clock, IUnitOfWork unitOfWork, ISettings settings, ILogService logService, ICustomerFeedbackService customerReviewService,
            ICustomerFeedbackFactory customerFeedbackFactory)
        {
            _clock = clock;
            _settings = settings;
            _logService = logService;
            _unitOfWork = unitOfWork;
            _customerReviewService = customerReviewService;
            _customerFeedbackFactory = customerFeedbackFactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _customerFeedbackResponseRepository = unitOfWork.Repository<CustomerFeedbackResponse>();
            _customerFeedbcakRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _customerEmailRepository = unitOfWork.Repository<CustomerEmail>();
            _reviewPushCustomerFeedbackRepository = unitOfWork.Repository<ReviewPushCustomerFeedback>();
            _reviewPushAPILocationRepository = unitOfWork.Repository<ReviewPushAPILocation>();
            _organizationRepository = unitOfWork.Repository<Organization>();
        }
        public void GetFeedbackResponse()
        {
            var getFeedbackEnabled = _settings.GetFeedbackEnabled;
            var isAllDataParsingOn = _settings.IsReviewPushParseAllDataOn;
            if (!getFeedbackEnabled)
            {
                _logService.Info("Feedback Response is Disabled.");
                return;
            }

            var clientId = _settings.ClientId;
            var apiKey = _settings.ReviewApiKey;

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(apiKey))
            {
                _logService.Info("Please Provide Valid ClientId And API key.");
                return;
            }

            var to = _clock.UtcNow.AddDays(1);
            var from = _clock.UtcNow.AddMonths(-3);


            var franchiseeList = _franchiseeRepository.Table.Where(x => x.Organization.IsActive && x.IsReviewFeedbackEnabled).ToList();


            if (isAllDataParsingOn)
            {
                SendRequestToApiForFeedbackForAllData();
            }
            else
            {
                SendRequestToApiForFeedback(clientId, default(long), from, to);
            }
        }

        private void SendRequestToApiForFeedback(string clientId, long businessId, DateTime from, DateTime to)
        {
            var feedbackRequest = _customerFeedbcakRequestRepository.Table.Where(x => !x.IsQueued && x.ResponseId == null && !x.IsSystemGenerated).ToList();

            if (!feedbackRequest.Any())
                return;

            var toDate = _clock.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
            var fromDate = _clock.UtcNow.AddMonths(-4).ToString("yyyy-MM-dd");

            var response = _customerReviewService.GetFeedback(clientId, businessId, fromDate, toDate);
            if (response != null && response.result == "SUCCESS")
            {
                SavingData(response.info.ToList());
                _unitOfWork.SaveChanges();
            }
            else if (response == null && response.info.Count() == 0)
                _logService.Info(string.Format("No Response data Found for {0} - {1}:", toDate, fromDate));
            else
                _logService.Info(string.Format("Error: {0}", response.result));
        }

        private void SendRequestToApiForFeedbackForAllData()
        {
            var feedbackRequest = _customerFeedbcakRequestRepository.Table.Where(x => !x.IsQueued && x.ResponseId == null && !x.IsSystemGenerated).ToList();

            if (!feedbackRequest.Any())
                return;

            var toDate = _clock.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
            var fromDate = _clock.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");

            var response = _customerReviewService.GetFeedbackForAllData();
            SavingData(response.info.ToList());
        }

        private bool SavingData(List<ReviewPushResponseViewModel> viewModel)
        {
            var franchiseeId = default(long?);
            foreach (var model in viewModel)
            {
                try
                {
                    if (model.Taz_franchise_name == null)
                    {
                        var reviewLocationAPI = _reviewPushAPILocationRepository.Table.FirstOrDefault(x => x.Location_Id == model.Location_id);
                        if (reviewLocationAPI != null)
                        {
                            var franchiseeDomain = _franchiseeRepository.Table.FirstOrDefault(x => x.ReviewpushId == reviewLocationAPI.Id);
                            franchiseeId = franchiseeDomain != null ? franchiseeDomain.Id : default(long?);
                        }
                        else
                        {
                            franchiseeId = default(long?);
                        }
                    }
                    else
                    {
                        model.FranchiseeId = GetFranchiseeFromDataBase(model.Taz_franchise_name);
                    }
                    var isFromQA = _settings.IsFromQA;
                    if (isFromQA)
                    {
                        var isContainMail = model.Url.Contains("mailto:");
                        if (isContainMail)
                        {
                            var urlSplit = model.Url.Split(new string[] { "mailto:" }, System.StringSplitOptions.RemoveEmptyEntries);
                            if (urlSplit[0].Contains("taazaa.com"))
                                model.Email = urlSplit[0];
                            else
                            {
                                model.Email = urlSplit[0] + "@yopmail.com";
                            }
                        }
                    }
                    else
                    {
                        var isContainMail = model.Url.Contains("mailto:");
                        if (isContainMail)
                        {
                            var urlSplit = model.Url.Split(new string[] { "mailto:" }, System.StringSplitOptions.RemoveEmptyEntries);
                            model.Email = urlSplit[0];
                        }
                    }
                    _unitOfWork.StartTransaction();
                    var isFromDatabase = IsFromDataBaseOrNot(model);
                    if (isFromDatabase)
                    {
                        var isAlreadyPresent = _customerFeedbackResponseRepository.Table.Any(x => x.ReviewId == model.Id);
                        if (!isAlreadyPresent)
                        {
                            var customerDomain = _customerEmailRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Email == (model.Email));
                            var feedbackResponse = _customerFeedbackFactory.CreateDomain(model);
                            if (customerDomain != null)
                                feedbackResponse.CustomerId = customerDomain.CustomerId;
                            else
                                feedbackResponse.CustomerId = default(long?);
                            feedbackResponse.IsFromNewReviewSystem = true;
                            if (model.Taz_franchise_name == null)
                            {
                                feedbackResponse.FranchiseeId = franchiseeId;
                                if (franchiseeId == null)
                                {
                                    var reviewPushList = _franchiseeRepository.Table.Where(x => x.ReviewpushId == model.Id).ToList();
                                    if (reviewPushList.Count() == 1)
                                    {
                                        feedbackResponse.FranchiseeId = reviewPushList[0].Id;
                                    }
                                }
                            }
                            else
                            {
                                feedbackResponse.FranchiseeId = GetFranchiseeFromDataBase(model.Taz_franchise_name);
                            }

                            feedbackResponse.CustomerName = model.Name;
                            _customerFeedbackResponseRepository.Save(feedbackResponse);
                            model.Rp_date = model.Rp_date.GetValueOrDefault();
                            var custonerFeedBackrequest = _customerFeedbcakRequestRepository.Table.Where(x => (x.CustomerEmail.ToLower().Equals(model.Email.ToLower())) && x.DateSend <= model.Rp_date.Value
                          && x.ResponseId == null).OrderByDescending(x => x.DateSend).ToList().FirstOrDefault();
                            if (custonerFeedBackrequest != null)
                            {
                                custonerFeedBackrequest.ResponseId = feedbackResponse.Id;
                                _customerFeedbcakRequestRepository.Save(custonerFeedBackrequest);
                            }
                        }
                        else
                        {

                            var customerFeedBackData = _customerFeedbackResponseRepository.Table.FirstOrDefault(x => x.ReviewId == model.Id);
                            var customerDomain = _customerEmailRepository.Table.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Email == (model.Email));
                            if (customerDomain != null)
                                customerFeedBackData.CustomerId = customerDomain.CustomerId;
                            else
                                customerFeedBackData.CustomerId = default(long?);
                            if (model.Taz_franchise_name == null)
                            {
                                customerFeedBackData.FranchiseeId = franchiseeId;
                            }
                            else
                            {
                                customerFeedBackData.FranchiseeId = GetFranchiseeFromDataBase(model.Taz_franchise_name);
                            }
                            //customerFeedBackData.FranchiseeId = franchiseeId;
                            customerFeedBackData.CustomerName = model.Name;
                            _customerFeedbackResponseRepository.Save(customerFeedBackData);
                        }
                    }
                    else
                    {
                        var isAlreadyPresent = _reviewPushCustomerFeedbackRepository.Table.Any(x => x.Review_Id == model.Id);
                        var reviewPushList = _franchiseeRepository.Table.Where(x => x.ReviewpushId == model.Id).ToList();
                        if (!isAlreadyPresent)
                        {
                            if (reviewPushList.Count() > 1)
                            {
                                franchiseeId = null;
                            }
                            var feedbackResponse = _customerFeedbackFactory.CreateDomainForFeedBack(model);
                            feedbackResponse.FranchiseeId = franchiseeId;
                            _reviewPushCustomerFeedbackRepository.Save(feedbackResponse);
                        }
                        else
                        {
                            if (reviewPushList.Count() > 1)
                            {
                                franchiseeId = null;
                            }
                            else
                            {
                                if (reviewPushList.Count() != 0)
                                {
                                    franchiseeId = reviewPushList[0].Id;
                                }
                                else
                                    franchiseeId = null;
                            }
                            if (model.Taz_franchise_name != null)
                            {
                                franchiseeId = GetFranchiseeFromDataBase(model.Taz_franchise_name);
                            }
                            var customerFeedBackData = _reviewPushCustomerFeedbackRepository.Table.FirstOrDefault(x => x.Review_Id == model.Id);
                            customerFeedBackData.FranchiseeId = franchiseeId;
                            _reviewPushCustomerFeedbackRepository.Save(customerFeedBackData);
                        }
                    }
                    _unitOfWork.SaveChanges();
                }
                catch (Exception e1)
                {
                    _logService.Info((string.Format("Exception in Customer {0} exception {1}", model.Name, e1.InnerException)));
                }

            }
            return true;
        }

        private long? GetFranchiseeFromDataBase(string taazaaFranchiseeName)
        {
            var organizationDomain = _organizationRepository.Table.FirstOrDefault(x => x.Name == taazaaFranchiseeName);
            var franchiseeId = default(long?);
            if (organizationDomain == null)
            {
                franchiseeId = default(long?);
            }
            else
            {
                franchiseeId = organizationDomain.Id;
            }
            return franchiseeId;

        }
        private bool IsFromDataBaseOrNot(ReviewPushResponseViewModel model)
        {
            try
            {
                var email = "";
                var isContainMail = model.Url.Contains("mailto:");
                var isFromQA = _settings.IsFromQA;
                if (isContainMail)
                {

                    var urlSplit = model.Url.Split(new string[] { "mailto:" }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (urlSplit.Length > 0)
                    {
                        email = urlSplit[0];
                    }
                    if (isFromQA)
                    {
                        if (urlSplit.Length > 0 && !urlSplit[0].Contains("taazaa.com"))
                        {
                            var splittedText = email.Split('@')[0];
                            email = splittedText + "@yopmail.com";
                            model.Email = splittedText + "@yopmail.com";
                        }
                        else
                        {
                            if (urlSplit[0].Contains("taazaa.com"))
                            {
                                email = urlSplit[0];
                            }
                            else
                                model.Email = "";
                        }
                    }
                    var customerEmailPresent = _customerEmailRepository.Table.Any(x => x.Email == (email));
                    return customerEmailPresent;
                }
                return false;
            }
            catch (Exception e1)
            {

            }
            return false;
        }

    }
}
