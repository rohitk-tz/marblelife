using Core.Sales.ViewModel;
using System;

namespace Core.Sales
{
    public interface ISalesDataUploadCreateModelValidator
    {
        bool ValidateDates(SalesDataUploadCreateModel model);
        bool CheckDatesAreValidMonth(DateTime startDate, DateTime endDate);
        bool CheckIfDatesAreValidWeek(DateTime startDate, DateTime endDate);
    }
}
