using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Billing.Enum;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Reports.ViewModel;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class ProductReportService : IProductReportService
    {
        private readonly ISortingHelper _sortingHelper;
        private readonly IReportFactory _reportFactory;
        private readonly IExcelFileCreator _excelFileCreator;
        private readonly IClock _clock;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IRepository<PaymentItem> _paymentItemRepository;
        private readonly IRepository<FranchiseeSales> _franchiseeSalesRepository;
        private readonly IRepository<Organizations.Domain.ServiceType> _serviceTypeRepository;
        public readonly IRepository<InvoicePayment> _invoicePaymentRepository;

        public ProductReportService(IUnitOfWork unitOfWork, ISortingHelper sortingHelper, IClock clock, IReportFactory reportFactory,
            IExcelFileCreator excelFileCreator)
        {
            _sortingHelper = sortingHelper;
            _reportFactory = reportFactory;
            _excelFileCreator = excelFileCreator;
            _clock = clock;
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _paymentRepository = unitOfWork.Repository<Payment>();
            _paymentItemRepository = unitOfWork.Repository<PaymentItem>();
            _franchiseeSalesRepository = unitOfWork.Repository<FranchiseeSales>();
            _serviceTypeRepository = unitOfWork.Repository<Organizations.Domain.ServiceType>();
            _invoicePaymentRepository = unitOfWork.Repository<InvoicePayment>();
        }

        public ProductChannelReportListModel GetReport(ProductReportListFilter filter, int pageNumber, int pageSize)
        {
            var franchiseeServiceClassList = GetFranchiseeServiceClassList(filter);

            var finalcollection = franchiseeServiceClassList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new ProductChannelReportListModel
            {
                Collection = finalcollection.Select(_reportFactory.CreateModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, franchiseeServiceClassList.Count())
            };
        }

        private IEnumerable<FranchiseeServiceClassCollection> GetFranchiseeServiceClassList(ProductReportListFilter filter)
        {
            if (filter.PaymentDateStart == null && filter.PaymentDateEnd == null)
            {
                var clock = new Clock();
                var currentDate = _clock.UtcNow;
                const int defaultMonth = 12;
                var month = currentDate.Month != 1 ? currentDate.Month - 1 : defaultMonth;
                var year = currentDate.Month != 1 ? currentDate.Year : currentDate.Year - 1;
                var date = new DateTime(year, month, 1);
                filter.PaymentDateStart = clock.FirstDayOfMonth(date);
                filter.PaymentDateEnd = clock.LastDayOfMonth(date);
            }

            var serviceListids = filter.TypeIds != null ? filter.TypeIds.Split(',').ToArray() : null;
            var serviceTypeIds = new List<long>();
            if (serviceListids != null)
            {
                foreach (var item in serviceListids)
                {
                    serviceTypeIds.Add(Convert.ToInt64(item));
                }
            }

            var franchiseeSalespayment = (from collection in _franchiseeSalesRepository.Table
                                          join invoicePayment in _invoicePaymentRepository.Table on collection.InvoiceId equals invoicePayment.InvoiceId
                                          join payment in _paymentRepository.Table on invoicePayment.PaymentId equals payment.Id
                                          join paymentItem in _paymentItemRepository.Table on payment.Id equals paymentItem.PaymentId
                                          //join franchiseeSales in _franchiseeSalesRepository.Table on collection.FranchiseeSalesId equals franchiseeSales.Id
                                          where ((filter.FranchiseeId < 1 || collection.FranchiseeId == filter.FranchiseeId)
                                            && paymentItem.ServiceType.CategoryId == (long)ServiceTypeCategory.ProductChannel
                                            && ((paymentItem.ItemTypeId == (long)InvoiceItemType.Service || paymentItem.ItemTypeId == (long)InvoiceItemType.Discount))
                                            && serviceTypeIds.Contains(paymentItem.ItemId)
                                            && (filter.PaymentDateStart == null || (payment.Date >= filter.PaymentDateStart))
                                            && (filter.PaymentDateEnd == null || (payment.Date <= filter.PaymentDateEnd)))
                                          select new
                                          {
                                              FranchiseeId = collection.FranchiseeId,
                                              ServiceTypeId = paymentItem.ItemId,
                                              ServiceType = paymentItem.ServiceType.Name,
                                              Franchisee = collection.Franchisee,
                                              PaymentItem = paymentItem
                                          }).GroupBy(y => new { y.FranchiseeId, y.ServiceTypeId, y.Franchisee, y.ServiceType }).ToList();

            var finalResult = franchiseeSalespayment.Select(x => new FranchiseeServiceClassCollection
            {
                FranchiseeId = x.Key.FranchiseeId,
                ServiceTypeId = x.Key.ServiceTypeId,
                Franchisee = x.Key.Franchisee,
                ServiceType = x.Key.ServiceType,
                TotalSales = x.Sum(y => y.PaymentItem.Payment.Amount)
            }).OrderByDescending(x => x.TotalSales);

            return finalResult;
        }

        public bool DownloadReport(ProductReportListFilter filter, out string fileName)
        {
            fileName = string.Empty;

            var reportCollection = new List<ProductChannelReportViewModel>();
            IEnumerable<FranchiseeServiceClassCollection> reportList = GetFranchiseeServiceClassList(filter).ToList();

            //prepare item collection
            foreach (var item in reportList)
            {
                var model = _reportFactory.CreateModel(item);
                reportCollection.Add(model);
            }
            fileName = MediaLocationHelper.GetTempMediaLocation().Path + "/productReport-" + string.Format("{0:yyyy-MM-dd_hh-mm-ss-tt}", _clock.UtcNow) + ".xlsx";
            return _excelFileCreator.CreateExcelDocument(reportCollection, fileName);

        }

        public ProductReportChartListModel GenerateChartData(ProductReportListFilter filter)
        {
            var graphData = GetGraphData();
            var chartData = GetChartData(filter);
            return new ProductReportChartListModel
            {
                Graphs = graphData,
                ChartData = chartData
            };
        }

        private IEnumerable<ProductChartViewModel> GetChartData(ProductReportListFilter filter)
        {
            var list = new List<ProductChartViewModel>();
            var paymentItemList = GetProductChannelServiceList(filter);

            //if (filter.EndDate.Value.Month == _clock.UtcNow.Month)
            //    filter.EndDate = filter.EndDate.Value.Date.AddMonths(-1);

            var months = MonthsBetween(filter.StartDate.Value, filter.EndDate.Value);
            foreach (var item in months)
            {
                var paymentItemsForMonth = paymentItemList.Where(x => x.Payment.Date.Month == item.Item1 && x.Payment.Date.Year == item.Item2).ToList();

                var model = new ProductChartViewModel { };
                var daysInMonth = DateTime.DaysInMonth(item.Item2, item.Item1);
                model.Date = new DateTime(item.Item2, item.Item1, daysInMonth);
                if (paymentItemsForMonth.Count() > 0)
                {
                    model.TotalOfAll = paymentItemsForMonth.Sum(x => x.Payment.Amount);
                    //if (model.TotalOfAll == 0)
                    //{
                    //    continue;
                    //}
                    model.Amazon = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.WebAmazon).Sum(y => y.Payment.Amount);
                    model.Amazoncanada = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.WebAmazonCanada)
                        .Sum(y => y.Payment.Amount);
                    model.Amazonprime = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.WebAmazonPrime)
                        .Sum(y => y.Payment.Amount);
                    model.Franchisesales = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.WebFranchiseeSales)
                        .Sum(y => y.Payment.Amount);
                    model.Government = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.Government)
                        .Sum(y => y.Payment.Amount);
                    model.Hardware = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.Hardware)
                        .Sum(y => y.Payment.Amount);
                    model.Hotel = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.Hotel)
                        .Sum(y => y.Payment.Amount);
                    model.Jet = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.WebJet)
                        .Sum(y => y.Payment.Amount);
                    model.Mld = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.WebMld)
                        .Sum(y => y.Payment.Amount);
                    model.Mldwarehouse = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.MldWarehouse)
                        .Sum(y => y.Payment.Amount);
                    model.Retail = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.Retail)
                        .Sum(y => y.Payment.Amount);
                    model.Testing = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.Testing)
                        .Sum(y => y.Payment.Amount);
                    model.Walmart = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.WebWalmart)
                        .Sum(y => y.Payment.Amount);
                    model.Admin = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.Admin)
                        .Sum(y => y.Payment.Amount);
                    model.OTHER = paymentItemsForMonth.Where(x => x.ItemId == (long)Organizations.Enum.ServiceType.OTHER)
                       .Sum(y => y.Payment.Amount);

                    model.Total = BiggestValue(model);
                }
                //if (model.TotalOfAll != 0)
                //{
                list.Add(model);
                //}
            }
            return list;
        }

        private decimal BiggestValue(ProductChartViewModel model)
        {
            decimal maxValue = default(decimal);
            Dictionary<int, decimal> valueArrays = new Dictionary<int, decimal>();
            valueArrays.Add(0, model.Amazon);
            valueArrays.Add(1, model.Amazoncanada);
            valueArrays.Add(2, model.Amazonprime);
            valueArrays.Add(3, model.Government);
            valueArrays.Add(4, model.Hardware);
            valueArrays.Add(5, model.Hotel);
            valueArrays.Add(6, model.Jet);
            valueArrays.Add(7, model.Mld);
            valueArrays.Add(8, model.Mldwarehouse);
            valueArrays.Add(9, model.Retail);
            valueArrays.Add(10, model.Testing);
            valueArrays.Add(11, model.Walmart);
            maxValue = valueArrays.ElementAt(0).Value;
            for (int a = 1; a <= 10; a++)
            {
                if (maxValue > valueArrays.ElementAt(a).Value)
                {
                    continue;
                }
                else
                {
                    maxValue = valueArrays.ElementAt(a).Value;
                }

            }
            return maxValue;
        }

        public IEnumerable<Tuple<int, int>> MonthsBetween(DateTime startDate, DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            while (iterator <= limit)
            {
                yield return Tuple.Create(iterator.Month, iterator.Year);
                iterator = iterator.AddMonths(1);
            }
        }

        private IQueryable<PaymentItem> GetProductChannelServiceList(ProductReportListFilter filter)
        {
            var collection = _paymentItemRepository.Table.Where(x => x.Payment != null
                                                               && (filter.PaymentDateStart == null || x.Payment.Date >= filter.PaymentDateStart)
                                                               && (filter.PaymentDateEnd == null || x.Payment.Date <= filter.PaymentDateEnd)
                                                               && x.ServiceType.CategoryId == (long)ServiceTypeCategory.ProductChannel);

            return collection;
        }

        private IEnumerable<ProductGraphViewModel> GetGraphData()
        {
            var list = new List<ProductGraphViewModel>();
            var serviceList = _serviceTypeRepository.Table.Where(x => x.CategoryId == (long)ServiceTypeCategory.ProductChannel);
            foreach (var item in serviceList)
            {
                var itemName = string.Empty;
                var name = item.Name.Split(new[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (name.Length > 1)
                    itemName = name[1].Replace(" ", "");
                else
                    itemName = name[0];

                var model = new ProductGraphViewModel
                {
                    Title = item.Name + " (in $)",
                    ValueField = itemName.ToLower(),
                    LineColor = item.ColorCode,
                    BalloonText = item.Name + "(in $)" + " , [[category]]: [[value]]",
                    LineThickness = 4,
                    type = "line",
                    ValueAxis = "v1"
                };
                list.Add(model);
            }
            var model2 = new ProductGraphViewModel
            {
                Title = "Total" + " (in $)",
                ValueField = "totalOfAll",
                LineColor = "#0000HH",
                BalloonText = "Total , [[category]]: [[value]]",
                LineThickness = 0,
                type = "line",
                ValueAxis = "v2",
                Bullet = "",
            };
            var barInfo = new ProductGraphViewModel
            {
                Title = "Total" + " (in $)",
                ValueField = "totalOfAll",
                LineColor = "#0000FF",
                type = "column",
                //BalloonText = "Total , [[category]]: [[value]]",
                FillAlphas = 0.8,
                LineAlpha = 0.2,
                ColumnWidth = 0.6,
                ValueAxis = "v2",
                Bullet = "",
            };
            list.Add(barInfo);
            list.Add(model2);

            return list;
        }

    }
}
