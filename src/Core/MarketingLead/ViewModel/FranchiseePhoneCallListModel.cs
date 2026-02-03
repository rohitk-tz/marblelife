using Core.Application.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MarketingLead.ViewModel
{
    public class FranchiseePhoneCallListModel
    {
        public List<FranchiseePhoneCallViewModel> Collection { get; set; }
        public PhoneCallFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }

        public FranchiseePhoneCallListModel()
        {
            Collection = new List<FranchiseePhoneCallViewModel>();
        }
    }

    public class FranchiseePhoneCallBulkListModel
    {
        public List<PhoneCallViewModel> Collection { get; set; }
        public PhoneCallFilter Filter { get; set; }
        public PagingModel PagingModel { get; set; }

        public FranchiseePhoneCallBulkListModel()
        {
            Collection = new List<PhoneCallViewModel>();
        }
    }

    public class AutomationBackUpCallListModel
    {
        public List<AutomationBackUpCallViewModel> Collection { get; set; }
        public List<AutomationClassMarketingLead> NoCallMatch { get; set; }
        public PhoneCallFilter Filter { get; set; }
        public long? TotalCount { get; set; }

        public AutomationBackUpCallListModel()
        {
            Collection = new List<AutomationBackUpCallViewModel>();
        }
    }

    public class AutomationBackUpCallViewModel
    {
        public long? FranchiseeId { get; set; }
        public string FranchiseeName { get; set; }
        public AutomationBackUpCallModel AutomationBackUpCallModel { get; set; }
    }

    public class AutomationBackUpCallModel
    {
        public AutomationBackUpCallModel()
        {
            ListOfNumberOfIVRCalls = new List<AutomationClassMarketingLead>();
            ListOfNumberOfNoCalls = new List<AutomationClassMarketingLead>();
            ListOfCallToKim = new List<AutomationClassMarketingLead>();
        }
        public decimal? IVRCalls { get; set; }
        public decimal? CallsToKim { get; set; }
        public decimal? CallsToTeresa { get; set; }
        public decimal? NumberOfNoCalls { get; set; }
        public List<AutomationClassMarketingLead> ListOfCallToKim { get; set; }
        public List<AutomationClassMarketingLead> ListOfCallToTeresa { get; set; }
        public List<AutomationClassMarketingLead> ListOfNumberOfNoCalls { get; set; }
        public List<AutomationClassMarketingLead> ListOfNumberOfIVRCalls { get; set; }
        public bool IsExpand { get; set; }
        public bool IsTeresaCallClick { get; set; }
        public bool IsKimCallClick { get; set; }
        public bool IsIVRCallClick { get; set; }
        public bool IsNoCallClick { get; set; }

    }
    public class AutomationClassMarketingLead
    {
        public long Id { get; set; }
        public bool DataFromNewAPI { get; set; }
        public bool DataFromInvoca { get; set; }
        public DateTime DateOfCall { get; set; }
        public string Ani { get; set; }
        public string Dnis { get; set; }
        public string TransferToNumber { get; set; }
        public string TransferType { get; set; }
        public string PhoneLabel { get; set; }
        public string CallType { get; set; }
        public int CallDuration { get; set; }
        public string Franchisee { get; set; }
        public string Tag { get; set; }
        public long? InvoiceId { get; set; }
        public long CallTypeId { get; set; }
        public string Route { get; set; }
        public string Destination { get; set; }
        public string ZipCode { get; set; }
        public string CallerId { get; set; }
        public string CallFlowState { get; set; }
    }

    public class AutomationBackUpCallFranchiseeModel
    {
        public double? ChargesOfCalls { get; set; }
        public long? CallCount { get; set; }
        public long? CallCountOriginal { get; set; }
        public long? TotalCount { get; set; }
        public List<AutomationClassMarketingLead> ListOfCallIVR{ get; set; }
    }

}
