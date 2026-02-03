using Core.Application;
using Core.Application.Attribute;
using Core.Notification.ViewModel;
using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using System;
using System.Linq;

namespace Core.Review.Impl
{
    [DefaultImplementation]
    public class CustomerFeedbackReportFactory : ICustomerFeedbackReportFactory
    {
        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        public CustomerFeedbackReportFactory(IUnitOfWork unitOfWork)
        {
            _customerFeedbackResponseRepository = unitOfWork.Repository<CustomerFeedbackResponse>();
        }
        public CustomerFeedbackReportViewModel CreateViewModel(CustomerFeedbackRequest request)
        {

            var response = request.CustomerFeedbackResponse;
            if (request.CustomerFeedbackResponse == null)
            {
                response = _customerFeedbackResponseRepository.Get(request.CustomerReviewSystemRecordId);
            }
            var model = new CustomerFeedbackReportViewModel
            {
                Id = response.Id,
                ResponseId = response != null ? response.Id : 0,
                CustomerId = request.CustomerReviewSystemRecord.CustomerId,
                Customer = request.CustomerReviewSystemRecord.Customer.Name,
                ResponseReceivedDate = response.IsFromNewReviewSystem ? response.DateOfReview : request.DateSend,
                ResponseSyncingDate = response != null ? response.IsFromNewReviewSystem ? response.DateOfDataInDataBase : response.DateOfReview : (DateTime?)null,
                CustomerEmail = request.CustomerEmail,
                Recommend = response != null ? response.Recommend : 0,
                ResponseContent = response != null ? response.ResponseContent : null,
                Franchisee = request.CustomerReviewSystemRecord.Franchisee.Organization.Name,
                Rating = response != null && response.Recommend != default(double) ? (decimal)(response.Recommend / 2) : 0,
                ContactPerson = request.CustomerReviewSystemRecord.Customer.ContactPerson,
                IsFromNewReviewSystem = response != null ? response.IsFromNewReviewSystem : false,
                IsFromCustomerReviewTable = 0,
                AuditStatusId = response.AuditActionId,
                AuditStatus = response.AuditAction.Name,
                FromTable = "CustomerFeedbackRequest",
                From = "GatherUp"

            };
            return model;
        }

        public ReviewSystemRecordViewModel CreateModel(CustomerFeedbackRequest request)
        {
            var modelReview = new ReviewSystemRecordViewModel
            {
                Customer = request.Customer.Name,
                CustomerEmail = request.CustomerEmail,
                ModeOfRequest = request.IsSystemGenerated ? "Kiosk Link" : "Mailtropolis Reviewability",
                QBInvoiceId = request.QBInvoiceId,
                RequestDate = request.DateSend.ToShortDateString(),
                ResponseDate = request.CustomerFeedbackResponse != null ? request.CustomerFeedbackResponse.DateOfReview.ToShortDateString() : string.Empty,
                Rating = request.CustomerFeedbackResponse != null ? request.CustomerFeedbackResponse.Recommend + "/10" : null,
                Response = request.CustomerFeedbackResponse != null ? request.CustomerFeedbackResponse.ResponseContent : null,
            };
            return modelReview;
        }

        public CustomerFeedbackReportViewModel CreateViewModel(ReviewPushCustomerFeedback request)
        {

            var model = new CustomerFeedbackReportViewModel
            {
                Id = request.Id,
                ResponseId = request != null ? request.Review_Id.GetValueOrDefault() : 0,
                Customer = request.Name,
                ResponseReceivedDate = request.Rp_date,
                ResponseSyncingDate = request.Db_date,
                CustomerEmail = request.Email,
                ResponseContent = request.Review,
                Franchisee = request.Franchisee != null ? request.Franchisee.Organization.Name : "",
                Rating = request != null ? (decimal)(request.Rating) : 0,
                ContactPerson = request.Name,
                IsFromNewReviewSystem = true,
                IsFromCustomerReviewTable = 1,
                CustomerNameFromAPI = request.Name,
                AuditStatusId = request.AuditActionId,
                AuditStatus = request.AuditAction.Name,
                FromTable = "ReviewPushCustomerFeedback",
                From = "ReviewSystem"
            };
            return model;
        }


        public CustomerFeedbackReportViewModel CreateViewModel(CustomerFeedbackResponse response)
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
            var model = new CustomerFeedbackReportViewModel
            {
                Id = response.Id,
                ResponseId = response != null ? response.ReviewId.GetValueOrDefault() : 0,
                CustomerId = response.CustomerId.GetValueOrDefault(),
                Customer = response.Customer != null ? response.Customer.Name : "",
                ResponseReceivedDate = response.IsFromNewReviewSystem ? response.DateOfReview : default(DateTime?),
                ResponseSyncingDate = response != null ? response.IsFromNewReviewSystem ? response.DateOfDataInDataBase : response.DateOfReview : (DateTime?)null,
                CustomerEmail = email,
                Recommend = response != null ? response.Recommend : 0,
                ResponseContent = response != null ? response.ResponseContent : null,
                Franchisee = response.Franchisee != null ? response.Franchisee.Organization.Name : "",
                Rating = response != null && response.Recommend != default(double) ? (decimal)(response.Rating) : 0,
                ContactPerson = response.Customer != null ? response.Customer.ContactPerson : "",
                IsFromNewReviewSystem = response != null ? response.IsFromNewReviewSystem : false,
                IsFromCustomerReviewTable = 0,
                CustomerNameFromAPI = response.CustomerName,
                AuditStatusId = response.AuditActionId,
                AuditStatus = response.AuditAction.Name,
                FromTable = "CustomerFeedbackResponse",
                From = response.IsFromGoogleAPI ? "Google" : "ReviewSystem"
            };
            return model;
        }
    }
}
