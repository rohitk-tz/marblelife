using Core.Application;
using Core.Application.Attribute;
using Core.Organizations;
using Core.Organizations.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Billing.Impl
{
    [DefaultImplementation]
    public class CalculateLoanScheduleService : ICalculateLoanScheduleService
    {
        private readonly IFranchiseeServiceFeeService _franchiseeServiceFeeService;
        private readonly IRepository<FranchiseeLoanSchedule> _franchiseeLoanScheduleRepository;
        private readonly IRepository<FranchiseeAccountCredit> _franchiseeAccountCreditRepository;
        private readonly IFranchiseeServiceFeeFactory _franchiseeServiceFeeFactory;
        private readonly ILogService _logService;
        private readonly IUnitOfWork _unitOfWork;
        public CalculateLoanScheduleService(IUnitOfWork unitOfWork, IFranchiseeServiceFeeService franchiseeServiceFeeService, ILogService logService,
            IFranchiseeServiceFeeFactory franchiseeServiceFeeFactory)
        {
            _unitOfWork = unitOfWork;
            _franchiseeServiceFeeService = franchiseeServiceFeeService;
            _franchiseeLoanScheduleRepository = unitOfWork.Repository<FranchiseeLoanSchedule>();
            _logService = logService;
            _franchiseeServiceFeeFactory = franchiseeServiceFeeFactory;
            _franchiseeAccountCreditRepository = unitOfWork.Repository<FranchiseeAccountCredit>();
        }
        public void CalculateSchedule()
        {
            var loanScheduleList = _franchiseeLoanScheduleRepository.Table.Where(x => x.CalculateReschedule).ToList();
            if (!loanScheduleList.Any())
            {
                _logService.Info("No Loan record Found for re-scheduling");
                return;
            }

            CheckingForNotOverPaidLoan(loanScheduleList.Where(x => !x.IsOverPaid).ToList());
            //CheckingForOverPaidLoan(loanScheduleList.Where(x => x.IsOverPaid).ToList());
        }

        private void CheckingForNotOverPaidLoan(List<FranchiseeLoanSchedule> loanScheduleList)
        {
            foreach (var loanSchedule in loanScheduleList)
            {
                try
                {
                    //Reschedule loan
                    var loan = loanSchedule.FranchiseeLoan;
                    var newSchedules = _franchiseeServiceFeeService.CalculateLoanTerms(loanSchedule.LoanTerm, loan.Duration,
                        Convert.ToDouble(loanSchedule.Balance),
                        Convert.ToDouble(loan.InterestratePerAnum) * 0.01, loanSchedule.DueDate, Convert.ToDouble(loanSchedule.PayableAmount),
                        Convert.ToDouble(loanSchedule.TotalPrincipal));

                    //update schedule
                    foreach (var item in newSchedules)
                    {
                        item.LoanId = loan.Id;
                        var indbLoanSchedule = loan.FranchiseeLoanSchedule.Where(x => x.LoanId == loan.Id && x.LoanTerm == item.TermNumber).FirstOrDefault();
                        if (indbLoanSchedule != null)
                            item.Id = indbLoanSchedule.Id;
                        var schedule = _franchiseeServiceFeeFactory.CreateDomain(item);
                        schedule.IsRoyality = indbLoanSchedule.IsRoyality;
                        if (loanSchedule.Balance < 0)
                        {
                            schedule.IsPrePaid = true;
                        }
                        _franchiseeLoanScheduleRepository.Save(schedule);
                    }

                    loanSchedule.CalculateReschedule = false;
                    _franchiseeLoanScheduleRepository.Save(loanSchedule);
                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logService.Error(ex.StackTrace);
                }
            }
        }

        public void CheckingForOverPaidLoan(List<FranchiseeLoanSchedule> loanScheduleList)
        {
            var balance = default(double);
            //var loanIds = loanScheduleList.Select(x => x.LoanId).ToList();
            foreach (var loanSchedule in loanScheduleList)
            {

                try
                {
                    if (loanSchedule.LoanTerm == 1)
                    {
                        balance = (Convert.ToDouble(loanSchedule.FranchiseeLoan.Amount) - Convert.ToDouble(loanSchedule.OverPaidAmount));
                    }
                    else
                    {
                        balance = (Convert.ToDouble(loanSchedule.Balance) - Convert.ToDouble(loanSchedule.OverPaidAmount));
                    }
                    //Reschedule loan
                    var loan = loanSchedule.FranchiseeLoan;
                    var newSchedules = _franchiseeServiceFeeService.CalculateLoanTerms(loanSchedule.LoanTerm, loan.Duration,
                        balance,
                        Convert.ToDouble(loan.InterestratePerAnum) * 0.01, loanSchedule.DueDate, Convert.ToDouble(loanSchedule.PayableAmount),
                        Convert.ToDouble(loanSchedule.TotalPrincipal));
                    var accountCredited = false;
                    //update schedule
                    foreach (var item in newSchedules)
                    {
                        item.LoanId = loan.Id;
                        var indbLoanSchedule = loan.FranchiseeLoanSchedule.Where(x => x.LoanId == loan.Id && x.LoanTerm == item.TermNumber).FirstOrDefault();
                        if (indbLoanSchedule != null)
                            item.Id = indbLoanSchedule.Id;
                        var schedule = _franchiseeServiceFeeFactory.CreateDomain(item);
                        schedule.IsRoyality = indbLoanSchedule.IsRoyality;
                        if ((loanSchedule.Balance) - (loanSchedule.OverPaidAmount) < 0)
                        {
                            schedule.IsPrePaid = true;
                            //loanSchedule.FranchiseeLoan.IsCompleted = true;
                        }

                        _franchiseeLoanScheduleRepository.Save(schedule);
                        if (!accountCredited)
                        {
                            accountCredited = true;
                            AccountToBeCredited(((loanSchedule.OverPaidAmount) - (loanSchedule.Balance)), loanSchedule.FranchiseeLoan);
                        }
                    }

                    loanSchedule.CalculateReschedule = false;
                    loanSchedule.IsOverPaid = false;
                    _franchiseeLoanScheduleRepository.Save(loanSchedule);

                    _unitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    _unitOfWork.Rollback();
                    _logService.Error(ex.StackTrace);
                }
            }
        }


        private void AccountToBeCredited(decimal amountToBeCredited, FranchiseeLoan franchiseeLoan)
        {
            var franchiseeCredit = new FranchiseeAccountCredit()
            {
                Amount = amountToBeCredited,
                CreditedOn = DateTime.UtcNow,
                CreditTypeId = franchiseeLoan.IsRoyality.GetValueOrDefault() ? 162 : 161,
                Description = "Adding OverPaid Amount Of $" + amountToBeCredited + " for LoanId " + franchiseeLoan.Id + " having description " + franchiseeLoan.Description,
                FranchiseeId = franchiseeLoan.FranchiseeId,
                CurrencyExchangeRateId = franchiseeLoan.CurrencyExchangeRateId,
                IsNew = true,
                RemainingAmount = 0
            };
            _franchiseeAccountCreditRepository.Save(franchiseeCredit);
        }
    }
}
