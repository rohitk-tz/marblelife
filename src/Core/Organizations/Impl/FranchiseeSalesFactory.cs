using Core.Organizations.Domain;
using Core.Organizations.ViewModels;
using Core.Application.Attribute;
using Core.Organizations.ViewModel;
using System.Linq;
using Core.Geo;
using Core.Application;
using Core.Review.Domain;
using Core.Billing.Domain;
using Core.Scheduler.ViewModel;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class FranchiseeSalesFactory : IFranchiseeSalesFactory
    {
        private IAddressFactory _addressFactory;
        private readonly IRepository<CustomerFeedbackRequest> _customerFeedbackRequestRepository;
        private readonly IRepository<CustomerFeedbackResponse> _customerFeedbackResponseRepository;
        private readonly IRepository<InvoiceAddress> _invoiceAddressRepository;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        public FranchiseeSalesFactory(IAddressFactory addressFactory, IUnitOfWork unitOfwork, IOrganizationRoleUserInfoService organizationRoleUserInfoService)
        {
            _addressFactory = addressFactory;
            _customerFeedbackRequestRepository = unitOfwork.Repository<CustomerFeedbackRequest>();
            _customerFeedbackResponseRepository = unitOfwork.Repository<CustomerFeedbackResponse>();
            _invoiceAddressRepository = unitOfwork.Repository<InvoiceAddress>();
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _serviceTypeRepository = unitOfwork.Repository<ServiceType>();
        }
        public FranchiseeSales CreateDomain(FranchiseeSalesEditModel model)
        {
            return new FranchiseeSales()
            {
                Id = model.Id,
                FranchiseeId = model.FranchiseeId,
                InvoiceId = model.InvoiceId,
                AccountCreditId = model.CreditMemoId,
                CustomerId = model.CustomerId,
                QbInvoiceNumber = model.QbInvoiceNumber,
                Amount = model.Amount,
                SalesDataUploadId = model.SalesDataUploadId
            };
        }

        public FranchiseeSalesEditModel CreateModel(long franchiseeId, long invoiceId, long customerId, string qbInvoiceNumber, long salesDataUploadId, decimal amount)
        {
            return new FranchiseeSalesEditModel()
            {
                FranchiseeId = franchiseeId,
                CustomerId = customerId,
                InvoiceId = invoiceId,
                QbInvoiceNumber = qbInvoiceNumber,
                SalesDataUploadId = salesDataUploadId,
                Amount = amount
            };
        }

        public FranchiseeSalesViewModel CreateViewModel(FranchiseeSales domain)
        {
            var invoiceAddress = _invoiceAddressRepository.Table.Where(x => x.InvoiceId == domain.InvoiceId).FirstOrDefault();
            var subClass = domain.SubClassTypeId != null ? domain.SubClassMarketingClass.MasterMarketingClass != null ? domain.SubClassMarketingClass.MasterMarketingClass.Name + ":" + domain.SubClassMarketingClass.Name : domain.SubClassMarketingClass.Name : null;
            
            var serviceListIds = domain.Invoice.InvoiceItems.Select(x => x.ItemId).ToList();
            var serviceCollection = string.Join(", ", _serviceTypeRepository.Table.Where(x => (x.DashboardServices || x.SubCategoryId == (long)Enum.ServiceTypeCategory.MLDAndMLFS) && serviceListIds.Contains(x.Id)).Select(x => x.Name).Distinct());
            var model = new FranchiseeSalesViewModel()
            {
                Id = domain.InvoiceId.Value,
                InvoiceId = domain.CustomerInvoiceId == 0 ? domain.InvoiceId : domain.CustomerInvoiceId,
                //Id = domain.InvoiceId.Value,
                //InvoiceId = domain.InvoiceId,
                CustomerName = domain.Customer.Name,
                InvoiceDate = domain.Invoice.GeneratedOn,
                //QbInvoiceNumber = domain.QbInvoiceNumber,
                QbInvoiceNumber = domain.CustomerQbInvoiceId > 0 ? domain.CustomerQbInvoiceId.ToString() : domain.QbInvoiceNumber,
                PaidAmount = domain.Invoice.InvoicePayments.ToList().Sum(x => x.Payment.Amount),
                TotalAmount = domain.Invoice.InvoiceItems.ToList().Sum(x => x.Amount),
                AccruedAmount = (domain.SalesDataUpload != null && domain.SalesDataUpload.AccruedAmount != null) ? domain.SalesDataUpload.AccruedAmount : 0,
                Address = invoiceAddress == null ? _addressFactory.CreateViewModel(domain.Customer.Address) : _addressFactory.CreateViewModel(invoiceAddress),
                MarketingClass = subClass == null ? domain.MarketingClass.Name : subClass,
                FranchiseeName = domain.Franchisee.Organization.Name,
                FranchiseeId = domain.Franchisee.Id,
                CurrencyCode = domain.Franchisee.Currency,
                CurrencyRate = domain.CurrencyExchangeRate.Rate,
                ServiceList = serviceCollection
            };

            var feedbackRequest = _customerFeedbackRequestRepository.Table.Where(x => x.QBInvoiceId == domain.QbInvoiceNumber
                                    && x.FranchiseeId == domain.FranchiseeId).FirstOrDefault();
            if (feedbackRequest != null)
            {
                model.IsSystemGenerated = feedbackRequest.IsSystemGenerated;
                model.IsFeedbackRequestSent = !feedbackRequest.IsQueued;
                var response = feedbackRequest.CustomerFeedbackResponse;
                model.IsFeedbackResponseReceived = response != null ? true : false;
                model.ResponseId = response != null ? response.Id : 0;
            }
            return model;
        }

        public UpdateMarketingClassViewModel CreateDomainModel(FranchiseeSales domain)
        {
            var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(domain.DataRecorderMetaData.CreatedBy.Value);
            var uploadedBy = userInfo.Email;
            return new UpdateMarketingClassViewModel()
            {
                CustomerName = domain.Customer.Name,
                //Description=domain.Invoice.inv
            };
        }

        public UpdateMarketingClassViewModel CreateDomainInvoiceModel(InvoiceItem invoiceItem, FranchiseeSales domain)
        {
            return new UpdateMarketingClassViewModel()
            {
                Id = invoiceItem.Id,
                CustomerName = domain != null && domain.Customer != null && domain.Customer.Name != null ? domain.Customer.Name : "",
                Description = invoiceItem != null && invoiceItem.Description != null ? invoiceItem.Description : "",
                Amount = invoiceItem != null && invoiceItem.Amount != null ? invoiceItem.Amount : default(long),
                GeneratedOn = invoiceItem != null && invoiceItem.Invoice != null && invoiceItem.Invoice.GeneratedOn != null ? invoiceItem.Invoice.GeneratedOn : DateTime.MinValue,
                Quantity = invoiceItem != null && invoiceItem.Quantity != null ? invoiceItem.Quantity : default(long),
                Rate = invoiceItem != null && invoiceItem.Rate != null ? invoiceItem.Rate : default(long),
                InvoiceId = invoiceItem != null && invoiceItem.InvoiceId != null ? invoiceItem.InvoiceId : default(long),
                MarketingClass = domain != null && domain.MarketingClass != null && domain.MarketingClass.Name != null ? domain.MarketingClass.Name : "",
                ServiceName = invoiceItem != null && invoiceItem.ServiceType != null && invoiceItem.ServiceType.Name != null ? invoiceItem.ServiceType.Name : "",
                Customer = CreateViewModel(domain.Customer),
                Item = invoiceItem != null && invoiceItem.ItemOriginal != null ? invoiceItem.ItemOriginal : "",
                FranchiseeName = domain.Franchisee != null && domain.Franchisee.Organization != null && domain.Franchisee.Organization.Name != null ? domain.Franchisee.Organization.Name : "",
            };
        }

        public DownloadAllInvoiceModel CreateDomainDownloadInvoiceModel(UpdateMarketingClassViewModel model)
        {
            return new DownloadAllInvoiceModel()
            {
                Id = model.Id,
                CustomerName = model.CustomerName,
                Description = model.Description,
                Amount = model.Amount,
                GeneratedOn = model.GeneratedOn,
                Quantity = model.Quantity,
                Rate = model.Rate,
                InvoiceId = model.InvoiceId,
                MarketingClass = model.MarketingClass,
                ServiceName = model.ServiceName,
                FinalClass = "",
                FinalService = "",
                NewClass = "",
                NewService = "",
                NewDescription = "",
                FinalDescription = "",
                IsChanged = 0,
                AddressLine1 = model.Customer.AddressLine1,
                AddressLine2 = model.Customer.AddressLine2,
                City = model.Customer.City,
                State = model.Customer.State,
                ZipCode = model.Customer.ZipCode,
                Item = model.Item,
                FranchiseeName = model.FranchiseeName
            };
        }

        public UpdateMarketingClassfileupload CreateViewModel(CustomerFileUploadCreateModel model)
        {
            return new UpdateMarketingClassfileupload
            {
                Id = model.Id,
                FileId = model.FileId,
                StatusId = model.StatusId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsNew = model.Id <= 0,
                notes = model.notes
            };
        }

        private CustomCustomerViewModel CreateViewModel(Customer model)
        {
            return new CustomCustomerViewModel
            {
                AddressLine1 = model != null && model.Address != null && model.Address.AddressLine1 != null ? model.Address.AddressLine1 : "",
                AddressLine2 = model != null && model.Address != null && model.Address.AddressLine2 != null ? model.Address.AddressLine2 : "",
                City = model != null && model.Address != null && model.Address.City != null && model.Address.City.Name != null ? model.Address.City.Name : "",
                State = model != null && model.Address != null && model.Address.State != null && model.Address.State.Name != null ? model.Address.State.Name : "",
                ZipCode = model != null && model.Address != null && model.Address.Zip != null && model.Address.Zip.Code  != null ? model.Address.Zip.Code : "",
            };
        }
        public ZipDataUploadViewModel CreateViewModel(UpdateMarketingClassfileupload model)
        {
            var userInfo = _organizationRoleUserInfoService.GetPersonByOrganziationRoleUserId(model.DataRecorderMetaData.CreatedBy.Value);
            var uploadedBy = userInfo.Email;
            return new ZipDataUploadViewModel
            {
                Id = model.Id,
                UploadedOn = model.DataRecorderMetaData.DateCreated,
                StatusId = model.StatusId,
                Status = model.Lookup.Name,
                UploadedBy = uploadedBy,
                IsEditable = false,
                Notes = model.notes,
                TempNotes = model.notes,
                FileId = model.FileId,
                LogFileId = model.ParsedLogFileId.GetValueOrDefault(),
            };
        }

        public class CustomCustomerViewModel
        {
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
        }
    }
}
