using System;
using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.Application;
using Core.Application.Attribute;
using System.Linq;
using Core.Billing.Enum;

namespace Core.Billing.Impl
{

    [DefaultImplementation]
    public class InvoiceItemService : IInvoiceItemService
    {
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IInvoiceItemFactory _invoiceitemFactory;
        private IRepository<FranchiseeInvoice> _franchiseeInvoiceRepository;

        public InvoiceItemService(IUnitOfWork unitOfWork, IInvoiceItemFactory invoiceitemFactory)
        {
            _invoiceItemRepository = unitOfWork.Repository<InvoiceItem>();
            _invoiceitemFactory = invoiceitemFactory;
            _franchiseeInvoiceRepository = unitOfWork.Repository<FranchiseeInvoice>();
        }

        public InvoiceItem Save(InvoiceItemEditModel model)
        {
            var invoiceItem = _invoiceitemFactory.CreateDomain(model);
            _invoiceItemRepository.Save(invoiceItem);
            return invoiceItem;
        }

        public decimal GetRoyaltyGeneratedForGivenMonthYear(long franchiseeId, int month, int year)
        {
            var invoiceIds = _franchiseeInvoiceRepository.Table.Where(x => x.FranchiseeId == franchiseeId && x.SalesDataUploadId != null).Select(x => x.InvoiceId);
            var query = _invoiceItemRepository.Table.Where(ii => invoiceIds.Contains(ii.InvoiceId) && ii.ItemTypeId == (long)InvoiceItemType.RoyaltyFee && ii.RoyaltyInvoiceItem != null
                                                && ii.RoyaltyInvoiceItem.StartDate.Month == month && ii.RoyaltyInvoiceItem.StartDate.Year == year).Select(ii => ii.Amount);

            if (!query.Any()) return 0;
            return query.Sum();
        }
    }
}
