using System;
using Core.Sales.ViewModel;
using Core.Application.Attribute;
using FluentValidation;
using Core.Localization.Validations;
using Core.Application.Impl;
using Core.Organizations;
using Core.Organizations.Enum;
using Core.Application.ViewModel;
using Core.Application;
using Core.Users.Enum;

namespace Core.Sales.Impl
{
    [DefaultImplementation(Interface = typeof(IValidator<SalesDataUploadCreateModel>))]
    public class SalesDataUploadCreateModelValidator : AbstractValidator<SalesDataUploadCreateModel>, ISalesDataUploadCreateModelValidator
    {
        //private readonly ISalesDataUploadService _salesDataUploadService;
        //private readonly IFranchiseeInfoService _franchiseeService;
        private readonly ISettings _settings;
        private readonly ISessionContext _sessionContext;

        public SalesDataUploadCreateModelValidator(ISettings settings, ISessionContext sessionContext)
        {
            _sessionContext = sessionContext;
            RuleFor(x => x.FranchiseeId).NotNull().WithMessage(Shared.Required).NotEmpty().WithMessage(Shared.Required);
            RuleFor(x => x.File).NotNull().WithMessage(Shared.Required);

            When(x => (x.IsUpdate != true && _settings.FeeProfileValidation), () =>
              {
                  RuleFor(x => x.PeriodStartDate).NotNull().WithMessage(Shared.Required).NotEmpty().WithMessage(Shared.Required).Must((x, y) =>
                {
                    var salesDataUploadService = ApplicationManager.DependencyInjection.Resolve<ISalesDataUploadService>();
                    return !salesDataUploadService.DoesOverlappingDatesExist(x.FranchiseeId, x.PeriodStartDate, x.PeriodEndDate);
                }).WithMessage("An upload with overlapping Dates already exist.");
              });

            RuleFor(x => x.PeriodEndDate).NotNull().WithMessage(Shared.Required).GreaterThan(x => x.PeriodStartDate).WithMessage("StartDate should not greater than EndDate");
            _settings = settings;
        }

        public bool ValidateDates(SalesDataUploadCreateModel model)
        {
            var salesDataUploadService = ApplicationManager.DependencyInjection.Resolve<ISalesDataUploadService>();
            var franchiseeService = ApplicationManager.DependencyInjection.Resolve<IFranchiseeInfoService>();

            var feeProfile = franchiseeService.GetFranchiseeFeeProfile(model.FranchiseeId);
            //var lastUploadedOn = salesDataUploadService.GetLastUploadedBatch(model.FranchiseeId);
            var result = true;

            //if (lastUploadedOn != null)
            //{
            //    result = (model.PeriodStartDate.Date - lastUploadedOn.Value).TotalDays == 1;
            //    if (result == false)
            //    {
            //        model.Message = FeedbackMessageModel.CreateErrorMessage("Invalid Dates");
            //        return false;
            //    }
            //}
            if (_settings.ApplyDateValidation == true)
            {
                if (feeProfile.PaymentFrequencyId == null || feeProfile.PaymentFrequencyId == (long)PaymentFrequency.Monthly)
                {
                    result = CheckDatesAreValidMonth(model.PeriodStartDate, model.PeriodEndDate);
                    if (result == false)
                        model.Message = FeedbackMessageModel.CreateErrorMessage("Invalid Month Dates");
                }
                else
                {
                    result = CheckIfDatesAreValidWeek(model.PeriodStartDate, model.PeriodEndDate);
                    if (result == false)
                        model.Message = FeedbackMessageModel.CreateErrorMessage("Invalid Week Dates");
                }
            }
            return result;
        }
        public bool CheckIfDatesAreValidWeek(DateTime startDate, DateTime endDate)
        {
            var firstDayOfweek = DayOfWeek.Monday;
            var lastDayOfWeek = DayOfWeek.Sunday;

            if (firstDayOfweek != startDate.DayOfWeek)
                return false;
            else if (lastDayOfWeek != endDate.DayOfWeek)
                return false;
            else if ((endDate - startDate).TotalDays != 6)
            {
                return false;
            }

            return true;
        }

        public bool CheckDatesAreValidMonth(DateTime startDate, DateTime endDate)
        {
            var clock = new Clock();
            var startMonth = startDate.Month;
            var endMonth = endDate.Month;
            var firstDay = clock.FirstDayOfMonth(startDate);
            var lastDay = clock.LastDayOfMonth(startDate);
            if (startMonth == endMonth)
            {
                if (firstDay.Day != startDate.Day || lastDay.Day != endDate.Day)
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
