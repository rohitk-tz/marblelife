using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Billing.Domain;
using Core.Organizations.Domain;
using Core.Sales.Domain;
using Core.Sales.Enum;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.Sales.Impl
{
    [DefaultImplementation]
    public class InvoiceItemUpdateInfoService : IInvoiceItemUpdateInfoService
    {
        private readonly ILogService _logService;
        private readonly IClock _clock;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISettings _setting;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<InvoiceItem> _invoiceItemRepository;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRepository<PaymentItem> _paymentItemRepository;
        private readonly IRepository<InvoiceFileUpload> _invoiceFileUploadRepository;
        private readonly IRepository<InvoicePayment> _invoicePaymentRepository;

        public InvoiceItemUpdateInfoService()
        {
            _unitOfWork = ApplicationManager.DependencyInjection.Resolve<IUnitOfWork>();
            _logService = ApplicationManager.DependencyInjection.Resolve<ILogService>();
            _clock = ApplicationManager.DependencyInjection.Resolve<IClock>();
            _setting = ApplicationManager.DependencyInjection.Resolve<ISettings>();

            _franchiseeSalesRepository = _unitOfWork.Repository<FranchiseeSales>();
            _invoiceItemRepository = _unitOfWork.Repository<InvoiceItem>();
            _franchiseeSalesPaymentRepository = _unitOfWork.Repository<FranchiseeSalesPayment>();
            _paymentItemRepository = _unitOfWork.Repository<PaymentItem>();
            _invoiceFileUploadRepository = _unitOfWork.Repository<InvoiceFileUpload>();
            _invoicePaymentRepository = _unitOfWork.Repository<InvoicePayment>();
        }
        public void UpdateReport()
        {
            if (!_setting.UpdateInvoiceRecord)
            {
                _logService.Debug("Service for Invoice File Parsing is Stopped");
                return;
            }

            var fileUplaod = GetFileToParse();

            if (fileUplaod == null)
            {
                _logService.Debug("No file found for InvoiceItem parsing");
                return;
            }

            fileUplaod.StatusId = (long)SalesDataUploadStatus.ParseInProgress;
            _invoiceFileUploadRepository.Save(fileUplaod);
            _unitOfWork.SaveChanges();

            DataTable data;
            IList<InvoiceInfoEditModel> collection;

            try
            {
                _unitOfWork.StartTransaction();

                var filePath = MediaLocationHelper.FilePath(fileUplaod.File.RelativeLocation, fileUplaod.File.Name).ToFullPath();
                data = FileParserHelper.ReadExcel(filePath);

                var invoiceFileParser = ApplicationManager.DependencyInjection.Resolve<IInvoiceFileParserService>();
                collection = invoiceFileParser.PrepareDomainFromDataTable(data);

                foreach (var record in collection.OrderBy(x => x.InvoiceId))
                {
                    _logService.Info(string.Format("updating Invoice# {0}", record.InvoiceId));
                    SaveModel(record);
                }
                fileUplaod.StatusId = (long)SalesDataUploadStatus.Parsed;
                _invoiceFileUploadRepository.Save(fileUplaod);
                _unitOfWork.SaveChanges();
                _logService.Info("Finished Updating marketing class and service Type!");
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Error : {0}", ex));
                fileUplaod.StatusId = (long)SalesDataUploadStatus.Failed;
                _invoiceFileUploadRepository.Save(fileUplaod);
                _unitOfWork.SaveChanges();
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
                    franchiseeSales.SubClassTypeId = model.SubClassTypeId;
                    _franchiseeSalesRepository.Save(franchiseeSales);
                }
                var invoiceItem = _invoiceItemRepository.Get(model.InvoiceItemId);
                if (invoiceItem != null && model.ServiceTypeId > 0)
                {
                    invoiceItem.ItemId = model.ServiceTypeId;

                    _invoiceItemRepository.Save(invoiceItem);

                    var fr_Payments = _invoicePaymentRepository.Table.Where(x => x.InvoiceId == model.InvoiceId && x.Payment != null
                                            && x.Payment.Amount == invoiceItem.Amount).Select(y => y.Payment).ToList();

                    if (!fr_Payments.Any())
                    {
                        fr_Payments = GetTargetSum(model.InvoiceId, invoiceItem.Amount);
                    }

                    var paymentIds = fr_Payments.Select(x => x.Id);
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

        private InvoiceFileUpload GetFileToParse()
        {
            return _invoiceFileUploadRepository.Table.Where(x => x.StatusId == (long)SalesDataUploadStatus.Uploaded)
                        .OrderBy(x => x.Id).FirstOrDefault();
        }

        private List<Payment> GetTargetSum(long invoiceId, decimal target)
        {
            var list = new List<Payment>();
            var paymentList = _invoicePaymentRepository.Table.Where(x => x.InvoiceId == invoiceId && x.Payment != null).Select(y => y.Payment).ToList();

            if (!paymentList.Any())
                return list;

            if (paymentList.Sum(p => p.Amount) == target)
            {
                foreach (var item in paymentList)
                {
                    list.Add(item);
                }
                return list;
            }

            for (var i = 0; i < paymentList.Count(); i++)
            {
                for (int k = 0; k < paymentList.Count(); k++)
                {
                    if (i != k)
                    {
                        decimal sum = paymentList[i].Amount + paymentList[k].Amount;
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
