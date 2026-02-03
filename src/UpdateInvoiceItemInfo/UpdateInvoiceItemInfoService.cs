using Core.Application;
using Core.Application.Attribute;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace UpdateInvoiceItemInfo
{
    [DefaultImplementation]
    public class UpdateInvoiceItemInfoService : IUpdateInvoiceItemInfoService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        public readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        public readonly IRepository<PaymentItem> _paymentItemRepository;
        public UpdateInvoiceItemInfoService()
        {
            _unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _setting = ApplicationManager.DependencyInjection.Resolve<ISettings>();

            _franchiseeSalesRepository = _unitOfWork.Repository<FranchiseeSales>();
            _invoiceItemRepository = _unitOfWork.Repository<InvoiceItem>();
            _franchiseeSalesPaymentRepository = _unitOfWork.Repository<FranchiseeSalesPayment>();
            _paymentItemRepository = _unitOfWork.Repository<PaymentItem>();
        }
        public void UpdateReport()
        {
            DataTable data;
            IList<InvoiceInfoEditModel> collection;
            try
            {
                var filePath = _setting.FilePath;
                var path = new Uri(filePath).LocalPath;
                data = FileParserHelper.ReadExcel(path);

                var invoiceFileParser = ApplicationManager.DependencyInjection.Resolve<InvoiceFileParser>();
                collection = invoiceFileParser.PrepareDomainFromDataTable(data);

                foreach (var record in collection)
                {
                    Console.WriteLine(string.Format("updating Invoice# {0}", record.InvoiceId));
                    SaveModel(record);
                }
                Console.WriteLine("Finished Update!");
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error : {0}", ex));
            }
        }

        private void SaveModel(InvoiceInfoEditModel model)
        {
            try
            {
                _unitOfWork.StartTransaction();

                var franchiseeSales = _franchiseeSalesRepository.Get(x => x.InvoiceId == model.InvoiceId);
                if (franchiseeSales != null && model.ClassTypeId > 0)
                {
                    franchiseeSales.ClassTypeId = model.ClassTypeId;
                    _franchiseeSalesRepository.Save(franchiseeSales);
                }
                var invoiceItem = _invoiceItemRepository.Get(model.InvoiceItemId);
                if (invoiceItem != null && model.ServiceTypeId > 0)
                {
                    invoiceItem.ItemId = model.ServiceTypeId;
                    _invoiceItemRepository.Save(invoiceItem);

                    var fr_salesPayments = _franchiseeSalesPaymentRepository.Table.Where(x => x.InvoiceId == model.InvoiceId && x.Payment != null
                                            && x.Payment.Amount == invoiceItem.Amount).ToList();

                    if (!fr_salesPayments.Any())
                    {
                        fr_salesPayments = GetTargetSum(model.InvoiceId, invoiceItem.Amount);
                    }

                    var paymentIds = fr_salesPayments.Select(x => x.PaymentId);
                    var paymentItems = _paymentItemRepository.Table.Where(x => paymentIds.Contains(x.PaymentId) && x.ItemTypeId == invoiceItem.ItemTypeId).ToList();

                    foreach (var paymentItem in paymentItems)
                    {
                        paymentItem.ItemId = model.ServiceTypeId;
                        _paymentItemRepository.Save(paymentItem);
                    }
                }
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logService.Info(string.Format("Unable to update Class/Service for InvoiceItem# {0}, {1}", model.InvoiceItemId, ex.Message));
            }
        }

        private List<FranchiseeSalesPayment> GetTargetSum(long invoiceId, decimal target)
        {
            var list = new List<FranchiseeSalesPayment>();
            var paymentList = _franchiseeSalesPaymentRepository.Table.Where(x => x.InvoiceId == invoiceId && x.Payment != null).ToArray();
            if (!paymentList.Any())
                return list;
            for (var i = 0; i < paymentList.Count(); i++)
            {
                for (int k = 0; k < paymentList.Count(); k++)
                {
                    if (i != k)
                    {
                        decimal sum = paymentList[i].Payment.Amount + paymentList[k].Payment.Amount;
                        if (sum == target)
                        {
                            list.Add(paymentList[i]);
                            list.Add(paymentList[k]);
                            return list;
                        }
                    }
                }
            }
            return list;
        }
    }
}
