using Core.Organizations.Domain;
using System.Collections.Generic;

namespace Core.Billing
{
    public interface ICalculateLoanScheduleService
    {
        void CalculateSchedule();
        void CheckingForOverPaidLoan(List<FranchiseeLoanSchedule> loanScheduleList);
    }
}
