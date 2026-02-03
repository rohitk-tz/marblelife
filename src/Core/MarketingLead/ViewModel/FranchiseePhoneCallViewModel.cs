using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    public class FranchiseePhoneCallViewModel
    {
        public string FranchiseeName { get; set; }
        public PhoneCallViewModel PhoneCallViewModel { get; set; }
        public List<PhoneCallViewModel> Histry { get; set; }
        public List<PhoneCallVInvoiceiewModel> InvoiceLIst { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllFranchiseeSelected { get; set; }
        public bool IsEdit { get; set; }
        public bool IsEditCall { get; set; }
        public bool IsEditDate { get; set; }
        public long? FranchiseeId { get; set; }
        public string SalesUpdationDate { get; set; }
    }

    public class PhoneCallViewModel
    {
        public PhoneCallViewModel()
        {
            ListOfCallIVR = new List<AutomationClassMarketingLead>();
        }
        public string PersonName { get; set; }
        public double? ChargesForPhone { get; set; }
        public DateTime? DateOfChange { get; set; }
        public DateTime? DateOfChangeOld { get; set; }
        public long? CallCount { get; set; }
        public List<AutomationClassMarketingLead> ListOfCallIVR { get; set; }
        public double TotalCost { get; set; }
        public double? ChargesForPhoneOld { get; set; }
        public long? CallCountOld { get; set; }
        public string Month { get; set; }
        public long Id { get; set; }
        public bool IsEdit { get; set; }
        public bool IsEditCall { get; set; }
        public bool IsExpand { get; set; }
        public long? FranchiseeId { get; set; }
        public bool IsEditDate { get; set; }
        public string DateOfChangeString { get; set; }
        public bool IsInvoiceGenerated { get; set; }
        public bool IsInvoiceInQueue { get; set; }
        public string FranchiseeName { get; set; }
        public string InvoiceId { get; set; }
        public long? PhonechargesfeeId { get; set; }

        public string MonthForDataRecorder { get; set; }
        public bool IsInvoiceGeneratedFromAPI { get; set; }
        public DateTime CurrentDate { get; set; }
    }

    public class PhoneCallVInvoiceiewModel
    {
        public long? Id { get; set; }
        public double Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public long? InvoiceItemId { get; set; }
        public bool IsInvoiceGenerated { get; set; }
        public bool IsInvoiceInQueue { get; set; }
        public long? PhonechargesfeeId { get; set; }
    }
}
