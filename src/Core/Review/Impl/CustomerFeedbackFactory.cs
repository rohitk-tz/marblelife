using Core.Application.Attribute;
using Core.Organizations.Domain;
using Core.Review.Domain;
using Core.Review.ViewModel;
using Core.Sales.Domain;
using System;
using System.Linq;

namespace Core.Review.Impl
{
    [DefaultImplementation]
    public class CustomerFeedbackFactory : ICustomerFeedbackFactory
    {
        public CreateCustomerForReviewModel CreateModel(Customer customer, ReviewAPIResponseModel responseModel, long businessId)
        {
            var email = customer.CustomerEmails.FirstOrDefault();

            var model = new CreateCustomerForReviewModel
            {
                businessId = businessId,
                customerCustomId = customer.Id,
                communicationPreference = "email",
                customerEmail = email.Email,
                customerFirstName = responseModel.firstName,
                customerLastName = responseModel.lastName,
                customerPhone = customer.Phone,
                sendFeedbackRequest = 0,
            };
            return model;
        }

        public GetCustomerForReviewModel CreateModel(long customerId, string clientId)
        {
            var model = new GetCustomerForReviewModel
            {
                clientId = clientId,
                customerId = customerId
            };
            return model;
        }

        public CustomerReviewSystemRecord CreateDomain(Customer customer, Franchisee franchisee, long reviewSystemCustomerId)
        {

            var domain = new CustomerReviewSystemRecord
            {
                BusinessId = franchisee.BusinessId != null ? franchisee.BusinessId : null,
                CustomerId = customer.Id,
                FranchiseeId = franchisee.Id,
                ReviewSystemCustomerId = reviewSystemCustomerId,
                IsNew = true,
            };
            return domain;
        }



        public CustomerFeedbackRequest CreateDomain(long customerId, long franchiseeSalesId, DateTime date, string customerEmail, string qBInvoiceId, bool isSystemTGenerated, long franchiseeId, long reviewSystemRecordId = 0)
        {
            var domain = new CustomerFeedbackRequest
            {
                FranchiseeSalesId = franchiseeSalesId,
                CustomerReviewSystemRecordId = reviewSystemRecordId,
                DateSend = date,
                CustomerEmail = customerEmail,
                QBInvoiceId = qBInvoiceId,
                IsQueued = true,
                IsSystemGenerated = isSystemTGenerated,
                FranchiseeId = franchiseeId,
                CustomerId = customerId,
                IsNew = true
            };
            return domain;
        }

        public FeedbackRequestModel CreateModel(string clientId, long businessId, string from, string to)
        {
            var model = new FeedbackRequestModel
            {
                businessId = businessId,
                clientId = clientId,
                from = from,
                page = 1,
                to = to,

            };
            return model;
        }

        public CustomerFeedbackResponse CreateDomain(FeedbackResponseViewModel review)
        {
            var dateString = review.dateOfReview.Split('T');
            var date = dateString[0].ToString();
            var reviewDate = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

            var domain = new CustomerFeedbackResponse
            {
                DateOfReview = reviewDate,
                Rating = review.rating,
                ResponseContent = review.body,
                CustomerId = review.customId,
                Recommend = review.recommend,
                ShowReview = review.showReview,
                FeedbackId = review.FeedbackId,
                IsNew = true
            };
            return domain;
        }

        public UpdateCustomerRecordViewModel CreateModel(string customerEmail, string firstName, string lastName, long businessId, string clientId, long customerId)
        {
            var model = new UpdateCustomerRecordViewModel
            {
                businessId = businessId,
                clientId = clientId,
                customerEmail = customerEmail,
                customerId = customerId,
                customerFirstName = firstName,
                customerLastName = lastName
            };
            return model;
        }

        public CustomerFeedbackResponse CreateDomain(ReviewPushResponseViewModel review)
        {

            var domain = new CustomerFeedbackResponse
            {
                DateOfReview = review.Rp_date.GetValueOrDefault(),
                Rating = long.Parse(review.Rating),
                ResponseContent = review.Review,
                CustomerId = review.CustomerId.GetValueOrDefault(),
                Recommend = review.Rating != null ? long.Parse(review.Rating) + 5 : 0,
                ShowReview = true,
                FeedbackId = review.Id.GetValueOrDefault(),
                IsNew = true,
                DateOfDataInDataBase = review.Db_date.GetValueOrDefault(),
                Url = review.Url,
                ReviewId = review.Id.GetValueOrDefault(),
                IsFromNewReviewSystem = true

            };
            return domain;
        }


        public ReviewPushCustomerFeedback CreateDomainForFeedBack(ReviewPushResponseViewModel review)
        {

            var domain = new ReviewPushCustomerFeedback
            {
                Rp_date = review.Rp_date,
                Rating = long.Parse(review.Rating),
                Review = review.Review,
                Review_Id = review.Id.GetValueOrDefault(),
                IsNew = true,
                Db_date = review.Db_date.GetValueOrDefault(),
                Url = review.Url,
                FranchiseeName = review.Franchise_name,
                Name = review.Name,
                Location_Id = review.Location_id,
                Rp_ID = review.Customer_id,
                Email = review.Email
            };
            return domain;
        }

    }
}
