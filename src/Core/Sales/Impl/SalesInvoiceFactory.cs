using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Geo;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Organizations;
using Core.Sales.Domain;
using Core.Sales.ViewModel;
using System;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class SalesInvoiceFactory : ISalesInvoiceFactory
    {
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IAddressFactory _addressFactory;

        public SalesInvoiceFactory(IFranchiseeSalesService franchiseeSalesService, IAddressFactory addressFactory)
        {
            _franchiseeSalesService = franchiseeSalesService;
            _addressFactory = addressFactory;
        }

        public SalesInvoiceViewModel CreateViewModel(InvoiceItem domain)
        {
            var franchiseeSalesInfo = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(domain.InvoiceId);
            var customerName = franchiseeSalesInfo != null && franchiseeSalesInfo.Customer != null && franchiseeSalesInfo.Customer.Name != null ? franchiseeSalesInfo.Customer.Name : "";
            var model = new SalesInvoiceViewModel
            {
                InvoiceId = domain.InvoiceId,
                Amount = domain.Amount,
                Description = domain != null && domain.Description != null ? domain.Description : "",
                InvoiceDate = domain != null && domain.Invoice != null && domain.Invoice.GeneratedOn != null ? domain.Invoice.GeneratedOn : DateTime.MinValue,
                InvoiceItemId = domain.Id,
                CustomerName = customerName,
                Address = franchiseeSalesInfo != null && franchiseeSalesInfo.Customer != null && franchiseeSalesInfo.Customer.Address != null ? _addressFactory.CreateViewModel(franchiseeSalesInfo.Customer.Address) : new AddressViewModel(),
                Service = domain != null && domain.ServiceType != null && domain.ServiceType.Name != null ? domain.ServiceType.Name : "",
                QBInvoiceNumber = franchiseeSalesInfo != null && franchiseeSalesInfo.QbInvoiceNumber != null ? franchiseeSalesInfo.QbInvoiceNumber : "",
                Franchisee = franchiseeSalesInfo != null && franchiseeSalesInfo.Franchisee != null && franchiseeSalesInfo.Franchisee.Organization != null && franchiseeSalesInfo.Franchisee.Organization.Name != null ? franchiseeSalesInfo.Franchisee.Organization.Name : "",
                MarketingClass = franchiseeSalesInfo != null && franchiseeSalesInfo.MarketingClass != null && franchiseeSalesInfo.MarketingClass.Name != null ? franchiseeSalesInfo.MarketingClass.Name : ""
            };
            return model;
        }

        public InvoiceFileUpload CreatDoamin(CustomerFileUploadCreateModel model)
        {
            return new InvoiceFileUpload()
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
            return new Address
            {
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                CountryId = model.CountryId,
                CityId = model.CityId!=0 && model.CityId != null? model.CityId:null,
                StateId = model.StateId!=0? model.StateId:default(long?),
                ZipId = model.ZipId,
                ZipCode = model.ZipCode,
                StateName = model.State,
                CityName = model.City,
                TypeId=11,
            };
        }
    }
}
