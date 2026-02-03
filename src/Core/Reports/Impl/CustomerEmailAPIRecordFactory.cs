using Core.Application;
using Core.Application.Attribute;
using Core.Notification.ViewModel;
using Core.Reports.Domain;
using Core.Reports.ViewModel;
using Core.Review.Domain;
using Core.Sales.ViewModel;
using System;
using System.Globalization;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class CustomerEmailAPIRecordFactory : ICustomerEmailAPIRecordFactory
    {
        private readonly IClock _clock;
        private ISettings _settings;
        public CustomerEmailAPIRecordFactory(IClock clock, ISettings settings)
        {
            _clock = clock;
            _settings = settings;
        }
        public CustomerEmailAPIRecord CreateDomain(CustomerCreateEditModel customer, string email, long franchiseeId)
        {
            var domain = new CustomerEmailAPIRecord
            {
                CustomerEmail = email,
                IsSynced = false,
                Status = "subscribed",
                FranchiseeId = franchiseeId,
                IsFailed = false,
                IsNew = true
            };
            return domain;
        }

        //public CustomerEmailAPIRecord CreateDomainforPartialPayment(CustomerCreateEditModel customer, string email, long franchiseeId, long invoiceId)
        //{
        //    var domain = new PartialCustomeremailapirecord
        //    {
        //        CustomerEmail = email,
        //        IsSynced = false,
        //        Status = "subscribed",
        //        FranchiseeId = franchiseeId,
        //        IsFailed = false,
        //        IsNew = true
        //    };
        //    return domain;
        //}

        public CustomerEmailAPIRecordRequestModel CreateModelForPayment(PartialPaymentEmailApiRecord domain)
        {
            var firstName = "";
            var lastName = "";
            var firstLastNamePair = domain.Customer.Name.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (firstLastNamePair.Count() == 1)
            {
                firstName = firstLastNamePair[0];
                lastName = string.Empty;
            }
            else if (firstLastNamePair.Count() > 1)
            {
                firstName = firstLastNamePair[firstLastNamePair.Count() - 1];
                foreach (var item in firstLastNamePair.Take(firstLastNamePair.Count() - 1))
                {
                    lastName += item + " ";
                }
            }
            string link = _settings.KioskLink;
            //foreach (var item in firstLastNamePair.Take(1))
            //{
            //    lastName += item + " ";
            //}
            var url = "";
            if (!string.IsNullOrEmpty(link))
            {
                if (domain.Franchisee.Reviewpush!=null)
                url = string.Format(link + domain.Franchisee.Reviewpush.NewRp_ID);
            }

            //foreach (var item in firstLastNamePair.Skip(1))
            //{
            //    lastName += item + " ";
            //}

            var model = new CustomerEmailAPIRecordRequestModel();
            model.email_address = domain.CustomerEmail;
            model.status = "subscribed";
            model.merge_fields.FNAME = firstName;
            model.merge_fields.LNAME = lastName;
            model.merge_fields.FRANCHISEE = domain.Franchisee.Organization.Name;
            model.merge_fields.REVIEWLINK = url;
            return model;
        }

        public CustomerEmailAPIRecordRequestModel CreateModel(CustomerEmailAPIRecord domain)
        {
            var firstLastNamePair = domain.Customer.Name.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var firstName = firstLastNamePair[0];
            var lastName = string.Empty;

            foreach (var item in firstLastNamePair.Skip(1))
            {
                lastName += item + " ";
            }

            var model = new CustomerEmailAPIRecordRequestModel();
            model.email_address = domain.CustomerEmail;
            model.status = "subscribed";
            model.merge_fields.FNAME = firstName;
            model.merge_fields.LNAME = lastName;
            model.merge_fields.FRANCHISEE = domain.Franchisee.Organization.Name;
            return model;
        }


        public CustomerEmailAPIRecordRequestModel CreateModelForPartialPayment(PartialPaymentEmailApiRecord domain)
        {
            var firstName = "";
            var lastName = "";
            var firstLastNamePair = domain.Customer.Name.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (firstLastNamePair.Count() == 1)
            {
                firstName = firstLastNamePair[0];
                lastName = string.Empty;
            }
            else if (firstLastNamePair.Count() > 1)
            {
                firstName = firstLastNamePair[firstLastNamePair.Count() - 1];
                foreach (var item in firstLastNamePair.Take(firstLastNamePair.Count() - 1))
                {
                    lastName += item + " ";
                }
            }
            string link = _settings.KioskLink;
            var url = "";
            if (!string.IsNullOrEmpty(link))
            {
                if (domain.Franchisee.Reviewpush != null)
                    url = string.Format(link + domain.Franchisee.Reviewpush.NewRp_ID);
            }
            var model = new CustomerEmailAPIRecordRequestModel();
            model.email_address = domain.CustomerEmail;
            model.status = "subscribed";
            model.merge_fields.FNAME = firstName;
            model.merge_fields.LNAME = lastName;
            model.merge_fields.FRANCHISEE = domain.Franchisee.Organization.Name;
            model.merge_fields.REVIEWLINK = url;
            return model;
        }
        public CustomerEmailAPIRecord CreateDomain(CustomerEmailAPIRecordResponseModel model, CustomerEmailAPIRecord domain)
        {
            var dateString = model.timestamp_opt != null ? model.timestamp_opt.Split(new[] { 'T' }, StringSplitOptions.RemoveEmptyEntries) : null;
            DateTime dt = _clock.UtcNow;
            if (dateString != null)
            {
                var date = dateString[0];
                dt = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            var result = new CustomerEmailAPIRecord
            {
                ApiCustomerId = model.Id,
                CustomerId = domain.CustomerId,
                IsSynced = model.detail != null ? false : true,
                APIEmailId = model.unique_email_id,
                Id = domain.Id,
                CustomerEmail = domain.CustomerEmail,
                APIListId = model.list_id,
                ErrorResponse = model.detail,
                DateCreated = dt,
                Status = model.status,
                FranchiseeId = domain.FranchiseeId,
                IsFailed = model.IsFailed,
                IsNew = domain.Id <= 0
            };
            return result;
        }

        public PartialPaymentEmailApiRecord CreateModelForPartialPayment(CustomerEmailAPIRecordResponseModel model, PartialPaymentEmailApiRecord domain)
        {
            var dateString = model.timestamp_opt != null ? model.timestamp_opt.Split(new[] { 'T' }, StringSplitOptions.RemoveEmptyEntries) : null;
            DateTime dt = _clock.UtcNow;
            if (dateString != null)
            {
                var date = dateString[0];
                dt = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            var result = new PartialPaymentEmailApiRecord
            {
                ApiCustomerId = model.Id,
                CustomerId = domain.CustomerId,
                IsSynced = model.detail != null ? false : true,
                APIEmailId = model.unique_email_id,
                Id = domain.Id,
                CustomerEmail = domain.CustomerEmail,
                APIListId = model.list_id,
                ErrorResponse = model.detail,
                DateCreated = dt,
                Status = model.status,
                FranchiseeId = domain.FranchiseeId,
                IsFailed = model.IsFailed,
                IsNew = domain.Id <= 0,
                InvoiceId = domain.InvoiceId,
                statusId = domain.statusId
            };
            return result;
        }

        public EmailAPINotificationModel CreateNotificationModel(CustomerEmailAPIRecord domain)
        {
            var model = new EmailAPINotificationModel();
            model.Franchisee = domain.Franchisee.Organization.Name;
            model.Customer = domain.Customer.Name;
            model.CustomerEmail = domain.CustomerEmail;
            model.DateSynced = domain.DateCreated.Value.ToShortDateString();
            return model;
        }

        public EmailAPINotificationModel CreateNotificationModel(PartialPaymentEmailApiRecord domain)
        {
            var model = new EmailAPINotificationModel();
            model.Franchisee = domain.Franchisee.Organization.Name;
            model.Customer = domain.Customer.Name;
            model.CustomerEmail = domain.CustomerEmail;
            model.DateSynced = domain.DateCreated.Value.ToShortDateString();
            return model;
        }


        public PartialPaymentEmailApiRecord CreateDomain(string customerEmail, long? franchiseeId, long? invoiceId, long? customerId)
        {
            var partialPayment = new PartialPaymentEmailApiRecord
            {
                CustomerEmail = customerEmail,
                FranchiseeId = franchiseeId.Value,
                InvoiceId = invoiceId.Value,
                Status = "subscribed",
                IsDeleted = false,
                IsNew = true,
                CustomerId = customerId.Value,

            };
            return partialPayment;
        }
        public PartialPaymentEmailApiRecord CreateDomain(string customerEmail, long? franchiseeId, long? invoiceId, long? customerId, long? statusId)
        {
            var partialPayment = new PartialPaymentEmailApiRecord
            {
                CustomerEmail = customerEmail,
                FranchiseeId = franchiseeId.Value,
                InvoiceId = invoiceId.Value,
                Status = "subscribed",
                IsDeleted = false,
                IsNew = true,
                CustomerId = customerId.Value,
                statusId = statusId.GetValueOrDefault()
            };
            return partialPayment;
        }
    }
}
