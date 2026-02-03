using Core.Application.ViewModel;
using Core.Billing.Domain;
using Core.Billing.ViewModel;
using Core.MarketingLead.Domain;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Core.Organizations
{
    public interface IFranchiseeServiceFeeService
    {
        void SaveServiceFeeItem(FranchiseeInvoice franchiseeInvoice);
        void SaveOneTimeProjectFee(FranchiseeInvoice frInvoice, IEnumerable<OneTimeProjectFee> feeList);
        DeleteInvoiceResponseModel Delete(long id, long franchiseeId);
        FranchiseeServiceFeeListsModel GetOneTimeFeeList(long franchiseeId);
        void Save(FranchiseeServiceFeeEditModel model);
        FranchiseeServiceFeeListsModel GetLoanList(long franchiseeId);
        void SaveLoanInvoiceItem(FranchiseeInvoice frInvoice, FranchiseeLoanSchedule loan);
        InvoiceItemEditModel CreateModel(FranchiseeServiceFee serviceFee, FranchiseeInvoice invoice, decimal amount, decimal rate, int qty,
            DateTime? startdate = null, DateTime? endDate = null);
        long Save(InvoiceItemEditModel model, FranchiseeServiceFee serviceFee, long invoiceId);
        List<AmortPaymentSchedule> CalculateLoanTerms(int startTerm, int amortizationTerm, double currentBalance, double interestRate, DateTime payDate, double monthlyPayment,
            double cummulativePrincipal, bool isFromLastLoan = false);
        bool SaveLoanAdjustmentType(FranchiseeChangeServiceFee model);
        bool GetFranchiseeRoyality(long? franchiseeId);
        FranchiseeTeamImageViewModel GetFranchiseeTeamImage(long? franchiseeId);
        bool SaveFranchiseeTeamImage(FranchiseeTeamImageEditModel franchiseeTeamImage);
        bool DownloadFranchiseeLoan(long? franchiseeLoanId, out string fileName);
        bool SavePrePayLoan(FranchiseePrePayLoanFeeEditModel model);
        bool SaveFranchisseeNotes(FranchiseeNotesDurationViewModel model);
        DurationApprovalListModel GetDurationApprovalList(long franchiseeId,long? roleId);
        bool ChangeDurationStatus(DurationApprovalFilterModel model);
        void SavePhoneChargeFess(FranchiseeInvoice frInvoice, IEnumerable<Phonechargesfee> feeList);
        void SaveServiceFeeItemForSeo(FranchiseeInvoice franchiseeInvoice);
    }
}
