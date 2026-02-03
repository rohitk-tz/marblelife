using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Organizations.Domain;
using Core.Reports.Domain;
using Core.Sales.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class UpdateSalesAmountService : IUpdateSalesAmountService
    {
        private readonly ILogService _logService;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IRepository<FranchiseeSalesPayment> _franchiseeSalesPaymentRepository;
        private readonly IRepository<FranchiseeSalesInfo> _frnchiseeSalesInfoRepository;
        private readonly IFranchiseeSalesInfoReportFactory _franchiseeSalesInfoReportFactory;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<MarketingClass> _marketingClassRepository;
        private readonly IRepository<ServiceType> _serviceTypeRepository;
        public UpdateSalesAmountService(IUnitOfWork unitOfWork, IClock clock, ISettings settings, ILogService logService,
            IFranchiseeSalesInfoReportFactory franchiseeSalesInfoReportFactory)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _franchiseeSalesPaymentRepository = unitOfWork.Repository<FranchiseeSalesPayment>();
            _frnchiseeSalesInfoRepository = unitOfWork.Repository<FranchiseeSalesInfo>();
            _franchiseeSalesInfoReportFactory = franchiseeSalesInfoReportFactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _marketingClassRepository = unitOfWork.Repository<MarketingClass>();
            _serviceTypeRepository = unitOfWork.Repository<ServiceType>();
        }
        public void UpdateData()
        {
            _logService.Info("starting update data for Growth Report!");
            UpdateSalesAmount();
            _logService.Info("Stop update data for Growth Report!");
        }

        private void UpdateSalesAmount()
        {
            var currentDate = _clock.UtcNow;
            var endDate = _clock.UtcNow;
            var previousDate = endDate.AddMonths(-6);
            var startDate = new DateTime(previousDate.Year, previousDate.Month, 1);

            if (_settings.UpdateAllSalesAmount)
            {
                startDate = new DateTime(1993, 1, 1);
            }

            var dateRangeList = MonthsBetween(startDate, endDate).OrderBy(x => x.Item2);
            var franchiseeIds = _franchiseeRepository.Table.Select(x => x.Id).ToList();

            foreach (var franchiseeId in franchiseeIds)
            {
                foreach (var dateRange in dateRangeList)
                {
                    var salesDataList = _franchiseeSalesPaymentRepository.Table.Where(x => x.Payment != null && x.FranchiseeSales != null
                                                && x.Payment.Date.Year == dateRange.Item2 && x.Payment.Date.Month == dateRange.Item1
                                                && x.FranchiseeSales.FranchiseeId == franchiseeId);
                    if (!salesDataList.Any())
                        continue;
                    try
                    {
                        foreach (var classTypeId in _marketingClassRepository.Table.Select(x => x.Id).ToList())
                        {
                            var paymentlist = salesDataList.Where(x => x.FranchiseeSales.ClassTypeId == classTypeId).Select(y => y.Payment);

                            if (!paymentlist.Any())
                                continue;

                            foreach (var serviceTypeId in _serviceTypeRepository.Table.Select(x => x.Id).ToList())
                            {
                                var payments = paymentlist.Where(x => x.PaymentItems.Any(y => y.ItemId == serviceTypeId)).ToList();
                                if (!payments.Any())
                                    continue;
                                var amount = payments.Sum(x => x.Amount);

                                var amountInLocalCurrency = payments.Sum(x => x.Amount.ToLocalCurrency(x.CurrencyExchangeRate.Rate));

                                var domain = _frnchiseeSalesInfoRepository.Get(x => x.Month == dateRange.Item1 && x.Year == dateRange.Item2 & x.FranchiseeId == franchiseeId
                                                    && x.ClassTypeId == classTypeId && x.ServiceTypeId == serviceTypeId);

                                if ((domain == null && amount != 0) || (domain != null && domain.Id > 0))
                                {
                                    var franchiseeSalesInfo = _franchiseeSalesInfoReportFactory.CreateDomain(franchiseeId, dateRange.Item1, dateRange.Item2, amount,
                                        amountInLocalCurrency, currentDate, classTypeId, serviceTypeId, domain);

                                    _unitOfWork.StartTransaction();
                                    _frnchiseeSalesInfoRepository.Save(franchiseeSalesInfo);
                                    _unitOfWork.SaveChanges();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logService.Info(string.Format("Error updating sales amount : ", ex.StackTrace));
                        _unitOfWork.Rollback();
                    }
                }
            }
        }

        private static IEnumerable<Tuple<int, int>> MonthsBetween(DateTime startDate, DateTime endDate)
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

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                yield return Tuple.Create(iterator.Month, iterator.Year);
                iterator = iterator.AddMonths(1);
            }
        }
    }
}
