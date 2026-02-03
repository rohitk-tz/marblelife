using Core.Application;
using Core.Application.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Organizations.Domain;
using Core.Organizations;
using Core.Notification.Enum;
using Core.Notification;
using Core.Notification.ViewModel;
using Core.Users.Enum;
using Core.Review.Domain;
using Core.MarketingLead.Domain;
using Core.Organizations.ViewModel;
using Newtonsoft.Json;
using Core.Sales.Enum;
using System.Net;
using Core.Users.Domain;
using Core.Sales.Domain;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class EmailNotificationForPayrollReport : IEmailNotificationForPayrollReport
    {
        private readonly ILogService _logService;
        private IUnitOfWork _unitOfWork;
        private ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<Organization> _organizationsRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly INotificationService _notificationService;
        private readonly INotificationModelFactory _notificationModelFactory;

        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbackRequestRepository;
        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        private readonly IRepository<FranchsieeGoogleReviewUrlAPI> _franchsieeGoogleReviewUrlAPIRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<ReviewPushCustomerFeedback> _reviewPushCustomerFeedbackRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<AllCustomerFeedback> _allCustomerFeedbackRepository;
        public EmailNotificationForPayrollReport(IUnitOfWork unitOfWork, ISettings settings, ILogService logService, INotificationService notificationService, INotificationModelFactory notificationModelFactory)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _organizationsRepository = unitOfWork.Repository<Organization>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _notificationService = notificationService;
            _notificationModelFactory = notificationModelFactory;

            _customerFeedbackRequestRepository = unitOfWork.Repository<CustomerFeedbackRequest>();
            _franchsieeGoogleReviewUrlAPIRepository = unitOfWork.Repository<FranchsieeGoogleReviewUrlAPI>();
            _customerFeedbackResponseRepository = unitOfWork.Repository<CustomerFeedbackResponse>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _reviewPushCustomerFeedbackRepository = unitOfWork.Repository<ReviewPushCustomerFeedback>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _allCustomerFeedbackRepository = unitOfWork.Repository<AllCustomerFeedback>();
        }

        public class EmailClassForPriceEstimate
        {
            public NotificationForPayrollReportViewModel NotificationForPayrollReportViewModels { get; set; }
            public EmailNotificationModelBase Base { get; set; }
            public EmailClassForPriceEstimate(EmailNotificationModelBase emailNotificationModelBase)
            {
                Base = emailNotificationModelBase;
            }
        }

        public class NotificationForPayrollReportViewModel
        {
            public int FromDate { get; set; }
            public int ToDate { get; set; }
            public string URL { get; set; }
            public string ToEmail { get; set; }

        }
        public void SendEmailNotificationForPayrollReport()
        {
            _logService.Info(string.Format("Payroll Report Job Is Running Now"));
            gettingGoogleAPIReviews();
            var sendCustomerReviewToMarketingSite = SendCustomerReviewsToMarketingSite();

            //DeleteDuplicateCustomerFeedback();
            //SyncCustomerReviewWithNewTable();
            var CurrentDate = DateTime.Now.ToString();
            DateTime DateValue = (Convert.ToDateTime(CurrentDate.ToString()));

            var CurrentDay = DateValue.Day;
            if (CurrentDay == 9 || CurrentDay == 23)
            {
                SendEmailToFranchiseeOwner(CurrentDay);
                SendEmailToScheduler(CurrentDay);
                SendEmailToSalesRep(CurrentDay);
                SendEmailToTechnician(CurrentDay);
            }
            else
            {
                return;
            }
        }

        private bool SendEmailToFranchiseeOwner(int CurrentDay)
        {
            try
            {
                var OwnerEmailList = _organizationsRepository.Table.Where(x => x.Franchisee.Organization.Id == 62 && x.IsActive == true && x.Email != null).Select(y => y.Email).ToList();
                NotificationForPayrollReportViewModel notification = new NotificationForPayrollReportViewModel();
                foreach (var item in OwnerEmailList)
                {
                    notification.ToEmail = item;
                    notification.FromDate = CurrentDay != 0 && CurrentDay == 9 ? 24 : 10;
                    notification.ToDate = CurrentDay != 0 && CurrentDay == 9 ? 08 : 22;
                    notification.URL = _settings.FranchiseeAdminLink;
                    SendEmailToFranchiseeOwnerAndScheduler(notification);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Email To Franchisee Owner - {0}", ex.Message));
                return false;
            }
        }
        private bool SendEmailToScheduler(int CurrentDay)
        {
            try
            {
                var OwnerEmailList = _franchiseeRepository.Table.Where(x => x.Organization.Id == 62 && x.SchedulerEmail != null).Select(y => y.SchedulerEmail).ToList();
                NotificationForPayrollReportViewModel notification = new NotificationForPayrollReportViewModel();
                foreach (var item in OwnerEmailList)
                {
                    notification.ToEmail = item;
                    notification.FromDate = CurrentDay != 0 && CurrentDay == 9 ? 24 : 10;
                    notification.ToDate = CurrentDay != 0 && CurrentDay == 9 ? 08 : 22;
                    notification.URL = _settings.FranchiseeAdminLink;
                    SendEmailToFranchiseeOwnerAndScheduler(notification);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Email To Scheduler - {0}", ex.Message));
                return false;
            }
        }
        private bool SendEmailToSalesRep(int CurrentDay)
        {
            try
            {
                var OwnerEmailList = _organizationRoleUserRepository.Table.Where(x => x.Organization.Id == 62 && x.IsActive == true && x.RoleId == (long)RoleType.SalesRep && x.Person.Email != null).ToList();
                //var organizationRollUserList = _organizationRoleUserRepository.IncludeMultiple(x => x.Organization, x => x.Role).Where(x => x.IsActive == true && (userId == null || x.UserId == userId) && ((x.RoleId == (long)RoleType.SalesRep) || (x.RoleId == (long)RoleType.Technician))).ToList();
                var orgRoleUserIds = OwnerEmailList.Select(x => x.UserId).ToList();
                var activeUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && !x.IsLocked).Select(x => x.Id).ToList();
                OwnerEmailList = OwnerEmailList.Where(x => activeUers.Contains(x.UserId)).ToList();
                NotificationForPayrollReportViewModel notification = new NotificationForPayrollReportViewModel();
                foreach (var item in OwnerEmailList)
                {
                    notification.ToEmail = item.Person.Email;
                    notification.FromDate = CurrentDay != 0 && CurrentDay == 9 ? 24 : 10;
                    notification.ToDate = CurrentDay != 0 && CurrentDay == 9 ? 08 : 22;
                    notification.URL = _settings.FranchiseeAdminLink;
                    SendEmailToSalesRepAndTechnician(notification);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Email To SalesRep - {0}", ex.Message));
                return false;
            }
        }
        private bool SendEmailToTechnician(int CurrentDay)
        {
            try
            {
                var OwnerEmailList = _organizationRoleUserRepository.Table.Where(x => x.Organization.Id == 62 && x.IsActive == true && x.RoleId == (long)RoleType.Technician && x.Person.Email != null).ToList();
                //var organizationRollUserList = _organizationRoleUserRepository.IncludeMultiple(x => x.Organization, x => x.Role).Where(x => x.IsActive == true && (userId == null || x.UserId == userId) && ((x.RoleId == (long)RoleType.SalesRep) || (x.RoleId == (long)RoleType.Technician))).ToList();
                var orgRoleUserIds = OwnerEmailList.Select(x => x.UserId).ToList();
                var activeUers = _userLoginRepository.Table.Where(x => orgRoleUserIds.Contains(x.Id) && !x.IsLocked).Select(x => x.Id).ToList();
                OwnerEmailList = OwnerEmailList.Where(x => activeUers.Contains(x.UserId)).ToList();
                NotificationForPayrollReportViewModel notification = new NotificationForPayrollReportViewModel();
                foreach (var item in OwnerEmailList)
                {
                    notification.ToEmail = item.Person.Email;
                    notification.FromDate = CurrentDay != 0 && CurrentDay == 9 ? 24 : 10;
                    notification.ToDate = CurrentDay != 0 && CurrentDay == 9 ? 08 : 22;
                    notification.URL = _settings.FranchiseeAdminLink;
                    SendEmailToSalesRepAndTechnician(notification);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Send Email To Technician - {0}", ex.Message));
                return false;
            }
        }

        private long SendEmailToFranchiseeOwnerAndScheduler(NotificationForPayrollReportViewModel notification)
        {
            try
            {
                var fromMail = _settings.FromEmail;
                var toMail = notification.ToEmail;
                EmailClassForPriceEstimate model = new EmailClassForPriceEstimate(_notificationModelFactory.CreateBaseDefault());
                model.NotificationForPayrollReportViewModels = notification;
                _notificationService.QueueUpNotificationEmail(NotificationTypes.SendLinkToFranchiseeOwnerAndSchedulerForPayrollReport, model, _settings.CompanyName, fromMail, toMail, _clock.UtcNow, null, null);
                _unitOfWork.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Sending Email To FranchiseeOwner And Scheduler - {0}", ex.Message));
                return 0;
            }
        }

        private long SendEmailToSalesRepAndTechnician(NotificationForPayrollReportViewModel notification)
        {
            try
            {
                var fromMail = _settings.FromEmail;
                var toMail = notification.ToEmail;
                EmailClassForPriceEstimate model = new EmailClassForPriceEstimate(_notificationModelFactory.CreateBaseDefault());
                model.NotificationForPayrollReportViewModels = notification;
                _notificationService.QueueUpNotificationEmail(NotificationTypes.SendLinkToSalesRepAndTechnicianForPayrollReport, model, _settings.CompanyName, fromMail, toMail, _clock.UtcNow, null, null);
                _unitOfWork.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Sending Email To SalesRep And Technician - {0}", ex.Message));
                return 0;
            }
        }


        //ReviewPush
        private void gettingGoogleAPIReviews()
        {
            try
            {
                _logService.Info(string.Format("gettingGoogleAPIReviews Job Is Running Now"));
                var feedbacksFromGoogleReview = _customerFeedbackResponseRepository.Table.ToList();
                _logService.Info(string.Format("Starting Customer Response From Google API"));
                var franchsieeGoogleReviewUrlAPIList = _franchsieeGoogleReviewUrlAPIRepository.Table.Where(x => x.BrightLocalLink != "").ToList();

                foreach (var franchsieeGoogleReviewUrlAPI in franchsieeGoogleReviewUrlAPIList)
                {
                    _logService.Info(string.Format("Starting Customer Response From  Google API For Franchisee {0} ", franchsieeGoogleReviewUrlAPI.FranchiseeId));
                    var review = GetCustomerFeedbackfromGoogleAPI(franchsieeGoogleReviewUrlAPI.BrightLocalLink);
                    var response = JsonConvert.DeserializeObject<ReviewPushCustomerGoogleFeedbackListModel>(review);
                    if (response != null && response.issuccess)
                    {
                        SaveCustomerGoogleResponse(response.results, franchsieeGoogleReviewUrlAPI.FranchiseeId, feedbacksFromGoogleReview);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error In Getting The Customer Review - {0}:", ex.Message));
            }
        }

        private bool SaveCustomerGoogleResponse(List<ReviewPushCustomerGoogleFeedbackViewModel> reviewList, long franchiseeId, List<CustomerFeedbackResponse> list)
        {
            try
            {
                foreach (var review in reviewList)
                {
                    try
                    {
                        var reviewString = NormalizeString(review.reviewBody);
                        var isReviewPresent = list.Where(x => NormalizeString(x.ResponseContent) == reviewString && x.CustomerName == review.author).ToList();
                        if (isReviewPresent.Count > 0)
                        {
                            if(isReviewPresent.Count > 1)
                            {
                                isReviewPresent = isReviewPresent.Where(x => x.IsFromGoogleAPI == true).ToList();
                                isReviewPresent = isReviewPresent.OrderByDescending(x => x.Id).Take(isReviewPresent.Count() - 1).ToList();
                                foreach(var reviewfeedback in isReviewPresent)
                                {
                                    try
                                    {
                                        reviewfeedback.IsDeleted = true;
                                        reviewfeedback.IsNew = false;
                                        _customerFeedbackResponseRepository.Delete(reviewfeedback);
                                        _unitOfWork.SaveChanges();
                                    }
                                    catch(Exception ex)
                                    {
                                        _logService.Info(string.Format("Error In Delete The Duplicate Customer Review - {0}:", ex.Message));
                                    }
                                }
                            }
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
                            AuditActionId = review.ratingValue != null ? long.Parse(review.ratingValue) > 3 ? (long)AuditActionType.Approved : (long)AuditActionType.Rejected : (long)AuditActionType.Rejected,
                            Rating = review.ratingValue != null ? long.Parse(review.ratingValue) : default(decimal),
                            Recommend = review.ratingValue != null ? long.Parse(review.ratingValue) + 5 : 0,
                            IsNew = true,
                            Url = "",
                            ResponseContent = review.reviewBody,

                        };
                        _customerFeedbackResponseRepository.Save(customerReviewResponse);
                        _unitOfWork.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Error In Saving The Customer Review - {0}:", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error In Saving The Customer Review - {0}:", ex.Message));
            }
            
            return true;
        }
        public static string NormalizeString(string str)
        {
            var cleanedString = str.Trim().Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            var cleanedString1 = Regex.Replace(cleanedString, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
            var cleanedString2 = Regex.Replace(cleanedString1, @"[^\w\s]", string.Empty);
            return cleanedString2;
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

        private bool DeleteDuplicateCustomerFeedback()
        {
            try
            {
                _logService.Info(string.Format("DeleteDuplicateCustomerFeedback Job Is Running Now"));
                var feedbacksFromGoogleReview = _customerFeedbackResponseRepository.Table.OrderByDescending(x => x.Id).ToList();

                var groupdData = feedbacksFromGoogleReview.GroupBy(x => x.ResponseContent).ToList();

                foreach (var review in groupdData)
                {
                    var duplicateResult = review.GroupBy(x => x.DateOfReview).ToList();
                    foreach (var item in duplicateResult)
                    {
                        var duplicateByName = item.GroupBy(x => x.CustomerName).ToList();
                        if (duplicateByName.Count() == 1)
                        {
                            foreach (var data in duplicateByName.ToList())
                            {
                                var dupDate = data.ToList();
                                //var dupDateCount = dupDate.Count();
                                dupDate = dupDate.Take(dupDate.Count() - 1).ToList();
                                foreach (var data1 in dupDate)
                                {
                                    _customerFeedbackResponseRepository.Delete(data1.Id);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Delete Duplicate Customer Feedback - {0}", ex.Message));
                return true;
            }
        }

        public bool SendCustomerReviewsToMarketingSite()
        {
            try
            {
                var customerReview = _customerFeedbackResponseRepository.Table.Where(x => x.IsSentToMarketingWebsite == false).ToList();

                CustomerReview customerReviewCollection = new CustomerReview();

                foreach (var review in customerReview)
                {
                    if (review.ResponseContent != null && review.Rating > 3)
                    {
                        var reviewForMarketing = new CustomerReviewForMarketing()
                        {
                            Id = review.Id,
                            FranchiseeId = review.FranchiseeId,
                            FranchiseeName = review.Franchisee != null && review.Franchisee.Organization != null && review.Franchisee.Organization.Name != null ? review.Franchisee.Organization.Name : "",
                            CustomerId = review.CustomerId != null ? review.CustomerId : default(long),
                            CustomerName = review.CustomerName != null ? review.CustomerName : "",
                            CustomerEmail = "",
                            ContactPerson = "",
                            ResponseReceivedDate = review.DateOfReview,
                            ResponseSyncingDate = review.DateOfDataInDataBase,
                            ResponseContent = review.ResponseContent,
                            Rating = review.Rating,
                            Recommend = (long)review.Recommend,
                            CustomerNameFromAPI = "",
                            AuditStatusId = review.AuditActionId,
                            AuditStatus = review.AuditAction != null && review.AuditAction.Alias != null ? review.AuditAction.Alias : "",
                            from = "GoogleAPI",
                        };
                        customerReviewCollection.Collection.Add(reviewForMarketing);
                    }
                }

                var data = customerReviewCollection;

                string json = JsonConvert.SerializeObject(data);

                string apiUrl = _settings.CustomerFeedbackToMarketingSiteURL;

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string customHeaderKey = _settings.CustomerFeedbackToMarketingSiteHeaderKey;
                        string customHeaderValue = _settings.CustomerFeedbackToMarketingSiteHeaderValue;
                        client.DefaultRequestHeaders.Add(customHeaderKey, customHeaderValue);
                        if(customHeaderKey != "keytocheck")
                        {
                            return true;
                        }
                        HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                        // Send POST request synchronously
                        HttpResponseMessage response = client.PostAsync(apiUrl, content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var reviewList = customerReview.Where(x => x.Rating > 3).ToList();
                            foreach (var review in reviewList)
                            {
                                review.IsSentToMarketingWebsite = true;
                                review.IsNew = false;
                                _customerFeedbackResponseRepository.Save(review);
                                _unitOfWork.SaveChanges();
                            }
                            _logService.Info(string.Format("Customer Review Data Is Successfully Sent To Marketing Site"));
                        }
                        else
                        {
                            _logService.Info(string.Format("Error in Sending Data of Customer Review To Marketing Site"));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Some Error in Sending Customer Review To Marketing Site - {0}", ex.Message));
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Sending Customer Review To Marketing Site - {0}", ex.Message));
                return false;
            }
        }


        // Rewiew Table updated successfully!!
        public bool SyncCustomerReviewWithNewTable()
        {
            try
            {
                SyncReviewpushCustomerFeedback();
                SynCcustomerFeedbackResponse();
                _logService.Info(string.Format("Customer Review Successfully Sync with New Table!!!"));
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Syncing Customer Reviews In New Table - {0}", ex.Message));
                return true;
            }
        }

        public bool SyncReviewpushCustomerFeedback()
        {
            try
            {
                var reviewpushcustomerfeedbackData = _reviewPushCustomerFeedbackRepository.Table.ToList();
                List<AllCustomerFeedback> list = new List<AllCustomerFeedback>();
                foreach (var review in reviewpushcustomerfeedbackData)
                {
                    var feedback = new AllCustomerFeedback()
                    {
                        CustomerId = null,
                        CustomerName = review.Name,
                        CustomerEmail = review.Email,
                        ResponseReceivedDate = review.Rp_date,
                        ResponseSyncingDate = review.Db_date,
                        ResponseContent = review.Review,
                        FranchiseeId = review.FranchiseeId,
                        FranchiseeName = review.FranchiseeName,
                        Franchisee = review.Franchisee,
                        Rating = review.Rating,
                        Recommend = review.Rating != null ? review.Rating + 5 : 0,
                        ContactPerson = null,
                        CustomerNameFromAPI = null,
                        AuditStatusId = review.AuditActionId,
                        AuditAction = review.AuditAction,
                        From = "ReviewSystem",
                        FromTable = "ReviewPushCustomerFeedback",
                        ReviewPushCustomerFeedbackId = review.Id,
                        ReviewPushCustomerFeedback = review,
                        CustomerFeedbackRequestId = null,
                        CustomerFeedbackResponseId = null,
                        IsOldReview = true,
                        IsSentToMarketingWebsite = true,
                        IsEmailSent = true,
                        IsDeleted = false,
                        IsActive = true,
                        IsNew = true
                    };
                    var isPresent = list.Any(x => x.CustomerName == feedback.CustomerName && x.ResponseContent == feedback.ResponseContent && x.FranchiseeName == feedback.FranchiseeName);
                    if (isPresent)
                    {
                        continue;
                    }
                    _allCustomerFeedbackRepository.Save(feedback);
                    _unitOfWork.SaveChanges();

                    list.Add(feedback);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Syncing ReviewPushCustomerFeedback Reviews In New Table - {0}", ex.Message));
                return true;
            }
        }

        public bool SynCcustomerFeedbackResponse()
        {
            try
            {
                var customerFeedbackResponseData = _customerFeedbackResponseRepository.Table.ToList();
                List<AllCustomerFeedback> list = new List<AllCustomerFeedback>();

                foreach (var response in customerFeedbackResponseData)
                {
                    var email = "";
                    if (response.Url != null)
                    {
                        var urlSplit = response.Url.Split(new string[] { "mailto:" }, System.StringSplitOptions.RemoveEmptyEntries);
                        if (urlSplit.Length > 0)
                        {
                            email = urlSplit[0];
                        }
                    }
                    if (response.Customer != null && email == "")
                    {
                        email = response.Customer.CustomerEmails != null ? response.Customer.CustomerEmails.FirstOrDefault().Email : email;

                    }
                    var feedback = new AllCustomerFeedback()
                    {
                        CustomerId = response.CustomerId,
                        CustomerName = response.Customer != null ? response.Customer.Name : "",
                        CustomerEmail = email,
                        ResponseReceivedDate = response.IsFromNewReviewSystem ? response.DateOfReview : default(DateTime?),
                        ResponseSyncingDate = response != null ? response.IsFromNewReviewSystem ? response.DateOfDataInDataBase : response.DateOfReview : (DateTime?)null,
                        ResponseContent = response != null ? response.ResponseContent : null,
                        FranchiseeId = response.FranchiseeId,
                        FranchiseeName = response.Franchisee != null ? response.Franchisee.Organization.Name : null,
                        Franchisee = response.Franchisee,
                        Rating = response.Rating,
                        Recommend = response != null ? (long)response.Recommend : 0,
                        ContactPerson = response.Customer != null ? response.Customer.ContactPerson : null,
                        CustomerNameFromAPI = response.CustomerName,
                        AuditStatusId = response.AuditActionId,
                        From = response != null && response.IsFromGoogleAPI == false ? "ReviewSystem" : "FromGoogleAPI",
                        FromTable = "CustomerFeedbackResponse",
                        ReviewPushCustomerFeedbackId = null,
                        CustomerFeedbackRequestId = null,
                        CustomerFeedbackResponseId = response.Id,
                        CustomerFeedbackResponse = response,
                        IsOldReview = response != null && response.IsFromGoogleAPI == true ? false : true,
                        IsSentToMarketingWebsite = true,
                        IsEmailSent = true,
                        IsDeleted = false,
                        IsActive = true,
                        IsNew = true
                    };
                    var isPresent = list.Any(x => x.CustomerName == feedback.CustomerName && x.ResponseContent == feedback.ResponseContent && x.FranchiseeName == feedback.FranchiseeName && x.ResponseSyncingDate == feedback.ResponseSyncingDate && x.CustomerEmail == feedback.CustomerEmail);
                    if (isPresent)
                    {
                        continue;
                    }
                    _allCustomerFeedbackRepository.Save(feedback);
                    _unitOfWork.SaveChanges();

                    list.Add(feedback);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error in Syncing CustomerFeedbackResponse Reviews In New Table - {0}", ex.Message));
                return true;
            }
        }
    }
}
