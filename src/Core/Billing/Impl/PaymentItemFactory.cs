using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Billing.ViewModel;
using Core.Organizations.Domain;
using Core.Sales;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class PaymentItemFactory: IPaymentItemFactory
    {
        private IRepository<ServiceType> _serviceTypeRepository;
        private IMarketingClassService _margetingClassService;

        public PaymentItemFactory(IUnitOfWork unitOfWork, IMarketingClassService margetingClassService)
        {
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
            _margetingClassService = margetingClassService;
        }

        public PaymentItem CreatePaymentItemDomain(PaymentItemEditModel model)
        {
            return new PaymentItem()
            {
                ItemId = model.ItemId,
                ItemTypeId = model.ItemTypeId,
                IsNew = model.Id <= 0
            };
        }

        public PaymentItemEditModel CreatePaymentItemModel(PaymentItem domain)
        {
            string item = "";
            if (domain.ItemTypeId == (long)InvoiceItemType.Service)
            {
                var serviceType = _serviceTypeRepository.Get(domain.ItemId);
                var marketingClass = _margetingClassService.GetMarketingClassByPaymentId(domain.PaymentId);
                if (serviceType != null)
                    item = serviceType.Name;

                if(marketingClass != null && marketingClass.Length > 0)
                    item = marketingClass + ":"+ item;
            }

            return new PaymentItemEditModel()
            {
                ItemId = domain.ItemId,
                ItemTypeId = domain.ItemTypeId,
                Item = item
            };
        }
    }
}
