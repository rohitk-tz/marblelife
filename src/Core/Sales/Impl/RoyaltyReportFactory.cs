using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Organizations.Domain;
using Core.Sales.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System;
using Core.Application.ViewModel;
using Core.Application;
using Core.Sales.Domain;
using Core.Application.Domain;
using Core.Application.Enum;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class RoyaltyReportFactory : IRoyaltyReportFactory
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoyaltyReportFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IList<RoyaltyReportViewModel> CreateCollectionModel(IList<FranchiseeSales> domainCollection)
        {
            var collection = new List<RoyaltyReportViewModel>();
            var classIds = domainCollection.Select(x => x.ClassTypeId).Distinct();

            foreach (var item in classIds)
            {
                var classSales = domainCollection.Where(x => x.ClassTypeId == item && x.Invoice != null && x.Invoice.InvoiceItems.Any()).ToArray();
                var invoiceItems = classSales.SelectMany(cs => cs.Invoice.InvoiceItems.Where(x => x.ItemTypeId == (long)InvoiceItemType.Service)).ToArray();

                collection.Add(new RoyaltyReportViewModel
                {
                    ClassId = item,
                    Total = invoiceItems.Select(x => x.Amount).Sum(),   //  .ToList().Sum(x => x.Amount),
                    ServiceAmountViewModel = CreateServiceViewModelCollection(invoiceItems)
                });
            }
            return collection;
        }

        private IEnumerable<ServiceAmountViewModel> CreateServiceViewModelCollection(InvoiceItem[] invoiceItems)
        {
            var collection = new List<ServiceAmountViewModel>();
            var serviceIds = invoiceItems.Where(x => x.ItemId != null).Select(x => x.ItemId.Value).Distinct();

            foreach (var item in serviceIds)
            {
                collection.Add(new ServiceAmountViewModel
                {
                    ServiceId = item,
                    Amount = invoiceItems.Where(x => x.ItemId == item).Sum(m => m.Amount)
                });
            }
            return collection;
        }

        public IEnumerable<ServiceTypeListModel> GetServices()
        {
            var serviceTypes = _unitOfWork.Repository<ServiceType>();
            return serviceTypes.Table.Select(s => new ServiceTypeListModel
            {
                Display = s.Name,
                Value = s.Id,
                TypeId = s.CategoryId
            }).OrderBy(x => x.TypeId).ToArray();
        }
        public IEnumerable<SelectListModel> GetMarketingClasses()
        {
            var classes = _unitOfWork.Repository<MarketingClass>();
            return classes.Table.Select(m => new SelectListModel
            {
                Display = m.Name,
                Value = m.Id,
            }).ToArray();
        }

        public IEnumerable<SelectListModel> GetServiceTypeCategory()
        {
            var lookupRepository = _unitOfWork.Repository<Lookup>();
            return lookupRepository.Fetch(x => x.LookupTypeId == (long)LookupTypes.ServiceTypeCategory && x.IsActive).OrderBy(x => x.RelativeOrder)
                .Select(s => new SelectListModel { Display = s.Name, Value = s.Id }).ToArray();
        }

    }
}
