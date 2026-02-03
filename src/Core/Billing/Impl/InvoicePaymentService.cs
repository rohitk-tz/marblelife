using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class InvoicePaymentService : IInvoicePaymentService
    {
        private readonly IRepository<InvoicePayment> _invoicePaymentRepository;

        public InvoicePaymentService(IUnitOfWork unitOfWork)
        {
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
        }

        public void Save(long invoiceId, long paymentId)
        {
            var inPayment = new InvoicePayment
            {
                InvoiceId = invoiceId,
                PaymentId = paymentId,
                IsNew = true
            };

            _invoicePaymentRepository.Save(inPayment);
        }
    }
}
