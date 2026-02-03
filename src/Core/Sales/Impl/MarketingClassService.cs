using System;
using Core.Application.Attribute;
using Core.Organizations;
using Core.Billing.Domain;
using Core.Application;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class MarketingClassService : IMarketingClassService
    {
        private readonly IFranchiseeSalesService _franchiseeSalesService;
        private readonly IRepository<InvoicePayment> _invoicePaymentRepo;

        public MarketingClassService(IUnitOfWork unitOfWork, IFranchiseeSalesService franchiseeSalesService)
        {
            _franchiseeSalesService = franchiseeSalesService;
            _invoicePaymentRepo = unitOfWork.Repository<InvoicePayment>();
        }
        public string GetMarketingClassByInvoiceId(long invoiceId)
        {
            var franchiseeSale = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(invoiceId);
            return franchiseeSale != null  && franchiseeSale.MarketingClass !=null? franchiseeSale.MarketingClass.Name : "";
        }
        public string GetSubMarketingClassByInvoiceId(long invoiceId)
        {
            var franchiseeSale = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(invoiceId);
            return franchiseeSale != null && franchiseeSale.SubClassTypeId!=null ? franchiseeSale.SubClassMarketingClass.MasterMarketingClass .Name+ ":"+ franchiseeSale.SubClassMarketingClass.Name : "";
        }

        public string GetMarketingClassByPaymentId(long paymentId)
        {
            var invoice = _invoicePaymentRepo.Table.Where(x => x.PaymentId == paymentId).Select(x => x.Invoice).FirstOrDefault();

            if (invoice == null) return "";

            var franchiseeSale = _franchiseeSalesService.GetFranchiseeSalesByInvoiceId(invoice.Id);

            return franchiseeSale != null && franchiseeSale.MarketingClass!=null ? franchiseeSale.MarketingClass.Name : "";
        }
    }
}
