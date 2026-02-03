using Core.Application;
using Core.Application.Attribute;
using Core.Application.Impl;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Reports.Domain;
using Core.Sales.Domain;
using System;
using System.Linq;

namespace Core.Reports.Impl
{
    [DefaultImplementation]
    public class UpdateBatchUploadRecordService : IUpdateBatchUploadRecordService
    {
        private readonly ILogService _logService;
        private readonly ISettings _settings;
        private IUnitOfWork _unitOfWork;
        private readonly IClock _clock;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<BatchUploadRecord> _batchUploadReportRepository;
        private readonly IReportFactory _reportFactory;

        public UpdateBatchUploadRecordService(IUnitOfWork unitOfWork, IClock clock, ISettings settings, ILogService logService, IReportFactory reportFactory)
        {
            _logService = logService;
            _settings = settings;
            _unitOfWork = unitOfWork;
            _clock = clock;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _batchUploadReportRepository = unitOfWork.Repository<BatchUploadRecord>();
            _reportFactory = reportFactory;
        }
        public void UpdateData()
        {
            _logService.Info("starting update data for Batch upload Report!");
            UpdateBatchRecord();
            _logService.Info("Stop update data for Batch upload Report!");
        }

        private void UpdateBatchRecord()
        {
            var currentDate = _clock.UtcNow;
            var startDate = currentDate.AddDays(-1);
            var endDate = currentDate;

            if (_settings.UpdateInitialBatchRecord)
            {
                startDate = new DateTime(currentDate.Year, 1, 1);
                endDate = currentDate;
                UpdateAllData(startDate, endDate);
            }
            else
            {
                UpdateData(startDate, endDate);
            }
        }

        public void UpdateData(DateTime startDate, DateTime endDate)
        {
            var franchiseeList = _franchiseeRepository.Table.Where(x => x.SalesDataUploads.Any() && x.Organization.IsActive).ToList();
            try
            {
                foreach (var franchisee in franchiseeList)
                {
                    var waitPeriod = _settings.DefaultSalesDataLateFeeWaitPeriod;
                    if (franchisee.LateFee != null && franchisee.LateFee.SalesDataWaitPeriodInDays > 0)
                        waitPeriod = franchisee.LateFee.SalesDataWaitPeriodInDays;
                    var paymentFrequencyId = franchisee.FeeProfile != null ? franchisee.FeeProfile.PaymentFrequencyId : null;

                    if (paymentFrequencyId == (long)PaymentFrequency.Monthly || paymentFrequencyId == null)
                    {
                        var monthStartDate = new DateTime(startDate.Year, startDate.Month, 1);
                        var MonthEndDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                        var upload = franchisee.SalesDataUploads.Where(x => x.PeriodEndDate.Date == MonthEndDate.Date && x.PeriodStartDate.Date == monthStartDate.Date)
                            .OrderBy(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();
                        var indbUpload = franchisee.BatchUploadRecord.Where(x => x.StartDate.Date == monthStartDate.Date && x.EndDate.Date == monthStartDate.Date)
                            .OrderBy(y => y.UploadedOn).FirstOrDefault();
                        SaveUpload(monthStartDate, MonthEndDate, waitPeriod, franchisee.Id, paymentFrequencyId, indbUpload, upload);
                    }

                    else if (paymentFrequencyId == (long)PaymentFrequency.Weekly)
                    {
                        var weekStart = DateRangeHelperService.GetFirstDayOfWeek(startDate);
                        var weekEnd = weekStart.AddDays(6);
                        var upload = franchisee.SalesDataUploads.Where(x => x.PeriodStartDate.Date == weekStart.Date && x.PeriodEndDate.Date == weekEnd.Date)
                                .OrderBy(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();
                        var inDBRecord = franchisee.BatchUploadRecord.Where(x => x.StartDate.Date == weekStart.Date && x.EndDate.Date == weekEnd.Date)
                            .OrderBy(y => y.UploadedOn).FirstOrDefault();
                        SaveUpload(weekStart, weekEnd, waitPeriod, franchisee.Id, paymentFrequencyId, inDBRecord, upload);
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("error : {0}", ex.Message));
            }
        }

        public void UpdateAllData(DateTime startDate, DateTime endDate)
        {
            var franchiseeList = _franchiseeRepository.Table.Where(x => x.SalesDataUploads.Any() && x.Organization.IsActive).ToList();
            try
            {
                foreach (var franchisee in franchiseeList)
                {
                    var waitPeriod = _settings.DefaultSalesDataLateFeeWaitPeriod;
                    if (franchisee.LateFee != null && franchisee.LateFee.SalesDataWaitPeriodInDays > 0)
                        waitPeriod = franchisee.LateFee.SalesDataWaitPeriodInDays;
                    var uploadList = franchisee.SalesDataUploads.Where(x => x.PeriodEndDate.Date >= startDate.Date).ToList();
                    var paymentFrequencyId = franchisee.FeeProfile != null ? franchisee.FeeProfile.PaymentFrequencyId : null;

                    if (paymentFrequencyId == (long)PaymentFrequency.Monthly || paymentFrequencyId == null)
                    {
                        var MonthlyUploadRange = DateRangeHelperService.MonthsStartDate(startDate, endDate);
                        foreach (var range in MonthlyUploadRange)
                        {
                            var startOfPreMonth = range.Item1;
                            var endDateOfPreMonth = startOfPreMonth.AddMonths(1).AddDays(-1);
                            var upload = uploadList.Where(x => x.PeriodStartDate.Date == startOfPreMonth.Date && x.PeriodEndDate.Date == endDateOfPreMonth
                                          ).OrderBy(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();
                            var inDBRecord = franchisee.BatchUploadRecord.Where(x => x.StartDate.Date == startOfPreMonth && x.EndDate.Date == endDateOfPreMonth)
                                .OrderBy(y => y.UploadedOn).FirstOrDefault();
                            SaveUpload(startOfPreMonth, endDateOfPreMonth, waitPeriod, franchisee.Id, paymentFrequencyId, inDBRecord, upload);
                        }
                    }
                    else if (paymentFrequencyId == (long)PaymentFrequency.Weekly)
                    {
                        var weekRange = DateRangeHelperService.DayOfWeekCollection(startDate, endDate);
                        foreach (var range in weekRange)
                        {
                            var endDateOfWeek = range;
                            var startDayOfWeek = endDateOfWeek.AddDays(-6);
                            var upload = uploadList.Where(x => x.PeriodStartDate.Date == startDayOfWeek.Date && x.PeriodEndDate.Date == endDateOfWeek.Date)
                                .OrderBy(y => y.DataRecorderMetaData.DateCreated).FirstOrDefault();
                            var inDBRecord = franchisee.BatchUploadRecord.Where(x => x.StartDate.Date == startDayOfWeek.Date && x.EndDate.Date == endDateOfWeek.Date)
                                .OrderBy(y => y.UploadedOn).FirstOrDefault();
                            SaveUpload(startDayOfWeek, endDateOfWeek, waitPeriod, franchisee.Id, paymentFrequencyId, inDBRecord, upload);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("error : {0}", ex.Message));
            }
        }

        public void UpdateBatchRecord(SalesDataUpload upload)
        {
            var inDBUpload = _batchUploadReportRepository.Get(x => x.FranchiseeId == upload.FranchiseeId && x.StartDate == upload.PeriodStartDate.Date && x.EndDate == upload.PeriodEndDate.Date);
            var daysCount = upload.Franchisee.LateFee != null ? (upload.Franchisee.LateFee.SalesDataWaitPeriodInDays >= 0 ? upload.Franchisee.LateFee.SalesDataWaitPeriodInDays : _settings.DefaultSalesDataLateFeeWaitPeriod) : _settings.DefaultSalesDataLateFeeWaitPeriod;
            long? paymentFrequencyId = upload.Franchisee.FeeProfile != null ? upload.Franchisee.FeeProfile.PaymentFrequencyId : null;
            SaveUpload(upload.PeriodStartDate, upload.PeriodEndDate, daysCount, upload.FranchiseeId, paymentFrequencyId, inDBUpload, upload);
        }

        public void SaveUpload(DateTime startDate, DateTime endDate, int waitPeriod, long franchiseeId, long? paymentFrequencyId, BatchUploadRecord inDBRecord, SalesDataUpload upload)
        {
            try
            {
                _unitOfWork.StartTransaction();

                if (inDBRecord == null)
                {
                    var domain = _reportFactory.CreateDomain(startDate, endDate, waitPeriod, franchiseeId, paymentFrequencyId, upload);
                    if (domain.ExpectedUploadDate <= _clock.UtcNow)
                        _batchUploadReportRepository.Save(domain);
                }
                else if (inDBRecord.UploadedOn == null && upload != null)
                {
                    var uploadDate = upload.DataRecorderMetaData.DateCreated.Date;
                    inDBRecord.UploadedOn = uploadDate;
                    var isCorrectUploaded = inDBRecord.ExpectedUploadDate.Date >= uploadDate;
                    inDBRecord.IsCorrectUploaded = isCorrectUploaded;
                    _batchUploadReportRepository.Save(inDBRecord);
                }
                _unitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                _logService.Info(string.Format("Exception : {0}", ex.Message));
            }
        }
    }
}
