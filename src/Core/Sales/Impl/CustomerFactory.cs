using Core.Sales.Domain;
using Core.Sales.ViewModel;
using Core.Geo;
using Core.Application.Attribute;
using Core.Application;
using Core.Organizations;
using System.Linq;
using System.Collections.Generic;
using Core.Reports;
using System;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Organizations.Domain;
using Core.Reports.Domain;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class CustomerFactory : ICustomerFactory
    {
        private readonly IAddressFactory _addressFactory;
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IEmailFactory _emailFactory;
        private readonly ICustomerEmailReportService _customerEmailReportService;

        public CustomerFactory(IUnitOfWork unitOfWork, IAddressFactory addressFactory, IFranchiseeSalesService franchiseeSalesService, IEmailFactory emailFactory,
            ICustomerEmailReportService customerEmailReportService)
        {
            _addressFactory = addressFactory;
            _franchiseeSalesService = franchiseeSalesService;
            _emailFactory = emailFactory;
            _customerEmailReportService = customerEmailReportService;
        }

        public Customer CreateDomain(CustomerCreateEditModel model, Customer domain)
        {
            if (domain != null)
            {
                foreach (var inDbCustomerEmail in domain.CustomerEmails)
                {
                    model.CustomerEmails.Add(inDbCustomerEmail);
                }
                model.DateCreated = domain.DateCreated != null ? domain.DateCreated : model.DateCreated;
                model.ReceiveNotification = domain.ReceiveNotification;
                model.DataRecorderMetaData.Id = domain.DataRecorderMetaData.Id;
                model.DataRecorderMetaData = domain.DataRecorderMetaData;
                model.ContactPerson = model.ContactPerson != null ? model.ContactPerson : domain.ContactPerson;
            }
            var address = model.Address == null ? null : _addressFactory.CreateDomain(model.Address);

            return new Customer()
            {
                IsNew = model.Id < 1,
                Id = model.Id,
                Name = model.Name,
                Phone = model.Phone,
                CustomerEmails = model.CustomerEmails != null ? new List<CustomerEmail>(model.CustomerEmails.Select(x => _emailFactory.CreateDomain(x, model.Id))) : null, //model.CustomerEmails,
                ContactPerson = model.ContactPerson,
                Address = address,
                AddressId = address != null ? (long?)address.Id : null,
                DataRecorderMetaDataId = model.DataRecorderMetaData != null ? model.DataRecorderMetaData.Id : 0,
                DataRecorderMetaData = model.DataRecorderMetaData,
                ClassTypeId = model.MarketingClassId.GetValueOrDefault(),
                DateCreated = model.DateCreated,
                ReceiveNotification = model.ReceiveNotification,
                TotalSales = model.TotalSales,
                NoOfSales = model.NoOfSales,
                AvgSales = (model.TotalSales != null && model.NoOfSales != null) ? model.TotalSales / model.NoOfSales : null,

            };
        }

        public Customer CreateDomain(CustomerEditModel model)
        {
            return new Customer()
            {
                IsNew = model.Id < 1,
                Id = model.Id,
                Name = model.Name,
                Phone = model.PhoneNumber,
                CustomerEmails = model.Emails != null ? new List<CustomerEmail>(model.Emails.Select(x => _emailFactory.CreateDomain(x, model.Id))) : null,
                ContactPerson = model.ContactPerson,
                AddressId = model.AddressId,
                Address = _addressFactory.CreateDomain(model.Address),
                ClassTypeId = model.MarketingClassId,
                DateCreated = model.DateCreated,
                DataRecorderMetaDataId = model.DataRecorderMetaData != null ? model.DataRecorderMetaData.Id : 0,
                DataRecorderMetaData = model.DataRecorderMetaData,
                ReceiveNotification = model.ReceiveNotification,
                NoOfSales = model.NoOfSales,
                TotalSales = model.TotalSales,
                AvgSales = (model.TotalSales != null && model.NoOfSales != null) ? model.TotalSales / model.NoOfSales : null
            };
        }

        public Customer CreateCustomerDomain(CustomerEditModel model)
        {
            return new Customer()
            {
                IsNew = model.Id < 1,
                Id = model.Id,
                Name = model.Name,
                Phone = model.PhoneNumber,
                CustomerEmails = model.Emails != null ? new List<CustomerEmail>(model.Emails.Select(x => _emailFactory.CreateDomain(x, model.Id))) : null,
                ContactPerson = model.ContactPerson,
                AddressId = model.AddressId,
                Address = _addressFactory.CreateDomainForCustomerAddress(model.Address),
                ClassTypeId = model.MarketingClassId,
                DateCreated = model.DateCreated,
                DataRecorderMetaDataId = model.DataRecorderMetaData != null ? model.DataRecorderMetaData.Id : 0,
                DataRecorderMetaData = model.DataRecorderMetaData,
                ReceiveNotification = model.ReceiveNotification,
                NoOfSales = model.NoOfSales,
                TotalSales = model.TotalSales,
                AvgSales = (model.TotalSales != null && model.NoOfSales != null) ? model.TotalSales / model.NoOfSales : null
            };
        }


        public CustomerViewModel CreateViewModel(Customer customer)
        {
            var lastInvoiceDetail = _franchiseeSalesService.GetLastInvoiceDetails(customer.Id);
            var customerLastFranchiseeDetail = _franchiseeSalesService.GetCustomerLastFranchisee(customer.Id);
            bool isSynced = _customerEmailReportService.IsCustomerSyncedToEmailAPI(customer.Id);
            long totalNumberOfInvoices = _franchiseeSalesService.GetTotalNumberOfInvoices(customer.Id);
            // decimal customerTotalSales = _franchiseeSalesService.GetSalesOfCustomer(customer.Id);

            var customerViewModel = new CustomerViewModel
            {
                CustomerId = customer.Id,
                Emails = customer.CustomerEmails.Select(x => x.Email).ToList(),
                Email = string.Join(",", customer.CustomerEmails.Select(e => e.Email)),
                Name = customer.Name,
                Address = _addressFactory.CreateViewModel(customer.Address),
                PhoneNumber = customer.Phone,
                LastInvoiceId = (lastInvoiceDetail != null && lastInvoiceDetail.InvoiceId != null) ? lastInvoiceDetail.InvoiceId.Value : 0,
                Amount = lastInvoiceDetail != null ? lastInvoiceDetail.Amount : 0,
                MarketingClass = customer.MarketingClass != null ? customer.MarketingClass.Name : lastInvoiceDetail.MarketingClass.Name,
                ClassTypeId = customer.MarketingClass != null ? customer.MarketingClass.Id.ToString() : lastInvoiceDetail.MarketingClass.Id.ToString(),
                CurrencyRate = lastInvoiceDetail != null ? lastInvoiceDetail.CurrencyExchangeRate.Rate : 1,
                DateCreated = customer.DateCreated,
                DateModified = customer.DataRecorderMetaData.DateModified,
                FranchiseeName = customerLastFranchiseeDetail != null ? customerLastFranchiseeDetail.Franchisee.Organization.Name : null,
                CurrencyCode = customerLastFranchiseeDetail != null ? customerLastFranchiseeDetail.Franchisee.Currency : "USD",
                ContactPerson = customer.ContactPerson,
                IsSynced = isSynced,
                LastServicedDate = lastInvoiceDetail != null ? lastInvoiceDetail.Invoice.GeneratedOn : (DateTime?)null,
                TotalSales = customer.TotalSales != null ? customer.TotalSales : 0,
                NoOfSales = customer.NoOfSales,
                AvgSales = customer.AvgSales != null ? customer.AvgSales : 0,
                TotalNumberOfInvoices = totalNumberOfInvoices
            };
            return customerViewModel;
        }

        public CustomerFileUpload CreateCustomerFileUpload(CustomerFileUploadCreateModel model)
        {
            return new CustomerFileUpload()
            {
                Id = model.Id,
                FileId = model.FileId,
                StatusId = model.StatusId,
                DataRecorderMetaData = model.DataRecorderMetaData,
                IsNew = model.Id <= 0
            };
        }
        public Address CreateDomain(AddressEditModel model)
        {
            return new Address()
            {
                IsNew = true,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                CityId = model.CityId,
                StateId = model.StateId,
                CountryId = model.CountryId,
                ZipId = model.ZipId,
                ZipCode = model.ZipCode,
                CityName = model.City,
                StateName = model.State,
                TypeId = model.AddressType,
            };
        }


        public CustomerViewModel CreateCustomerViewModel(Customer customer, FranchiseeSales lastInvoiceDetail, bool isSynced)
        {
            try
            {
                var customerEmails = customer.CustomerEmails != null
                        ? customer.CustomerEmails.Select(x => x.Email).ToList()
                        : new List<string>();

                var firstCustomerEmail = customer.CustomerEmails != null
                                        ? string.Join(",", customer.CustomerEmails.Select(e => e.Email))
                                        : string.Empty;

                var marketingClassName = (customer.MarketingClass != null)
                                            ? customer.MarketingClass.Name
                                            : (lastInvoiceDetail != null &&
                                               lastInvoiceDetail.MarketingClass != null
                                                    ? lastInvoiceDetail.MarketingClass.Name
                                                    : null);

                var marketingClassId = (customer.MarketingClass != null)
                                            ? customer.MarketingClass.Id.ToString()
                                            : (lastInvoiceDetail != null &&
                                               lastInvoiceDetail.MarketingClass != null
                                                    ? lastInvoiceDetail.MarketingClass.Id.ToString()
                                                    : null);

                var currencyRate = (lastInvoiceDetail != null &&
                                    lastInvoiceDetail.CurrencyExchangeRate != null)
                                        ? lastInvoiceDetail.CurrencyExchangeRate.Rate
                                        : 1;

                var franchiseeName = (lastInvoiceDetail != null &&
                                      lastInvoiceDetail.Franchisee != null &&
                                      lastInvoiceDetail.Franchisee.Organization != null)
                                        ? lastInvoiceDetail.Franchisee.Organization.Name
                                        : null;

                var currencyCode = (lastInvoiceDetail != null &&
                                    lastInvoiceDetail.Franchisee != null)
                                        ? lastInvoiceDetail.Franchisee.Currency
                                        : "USD";

                var lastServicedDate = (lastInvoiceDetail != null &&
                                        lastInvoiceDetail.Invoice != null)
                                            ? lastInvoiceDetail.Invoice.GeneratedOn
                                            : (DateTime?)null;

                var qbInvoice = (lastInvoiceDetail != null) ? lastInvoiceDetail.QbInvoiceNumber : null;

                var lastInvoiceId = (lastInvoiceDetail != null && lastInvoiceDetail.InvoiceId.HasValue)
                                        ? lastInvoiceDetail.InvoiceId.Value
                                        : 0;

                var lastAmount = (lastInvoiceDetail != null) ? lastInvoiceDetail.Amount : 0;

                long totalNumberOfInvoices = 0;
                if (customer != null)
                {
                    totalNumberOfInvoices = _franchiseeSalesService.GetTotalNumberOfInvoices(customer.Id);
                }
                

                var customerViewModel = new CustomerViewModel
                {
                    CustomerId = customer != null ? customer.Id : 0,
                    Emails = customerEmails,
                    Email = firstCustomerEmail,
                    Name = customer != null ? customer.Name : null,
                    Address = customer != null ? _addressFactory.CreateViewModel(customer.Address) : null,
                    PhoneNumber = customer != null ? customer.Phone : null,
                    LastInvoiceId = lastInvoiceId,
                    Amount = lastAmount,
                    MarketingClass = marketingClassName,
                    ClassTypeId = marketingClassId,
                    CurrencyRate = currencyRate,
                    DateCreated = customer != null ? customer.DateCreated : DateTime.MinValue,
                    DateModified = (customer != null && customer.DataRecorderMetaData != null)
                                        ? customer.DataRecorderMetaData.DateModified
                                        : (DateTime?)null,
                    FranchiseeName = franchiseeName,
                    CurrencyCode = currencyCode,
                    ContactPerson = customer != null ? customer.ContactPerson : null,
                    IsSynced = isSynced,
                    LastServicedDate = lastServicedDate,
                    TotalSales = customer.TotalSales.HasValue ? customer.TotalSales.Value : 0,
                    NoOfSales = customer.NoOfSales.HasValue ? customer.NoOfSales.Value : 0,
                    AvgSales = customer.AvgSales.HasValue ? customer.AvgSales.Value : 0,
                    status = 0,
                    QbInvoiceId = qbInvoice,
                    TotalNumberOfInvoices = totalNumberOfInvoices
                };
                return customerViewModel;
            }
            catch (Exception ex)
            {
                return new CustomerViewModel();
            }
        }
    }
}
