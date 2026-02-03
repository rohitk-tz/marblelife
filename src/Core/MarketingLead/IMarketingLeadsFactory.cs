using Core.Application.ViewModel;
using Core.MarketingLead.Domain;
using Core.MarketingLead.ViewModel;
using Core.Scheduler.Domain;
using System;
using System.Collections.Generic;

namespace Core.MarketingLead
{
    public interface IMarketingLeadsFactory 
    {
        MarketingLeadCallDetailViewModel CreateModel(CallRetailRecord record, long callTypeId);
        MarketingLeadCallDetail CreateDomain(MarketingLeadCallDetailViewModel model);
        RoutingNumber CreateDomain(string phoneNumber, string phoneLabel);
        WebLead CreateDomain(WebLeadViewModel model);
        CallDetailViewModel CreateViewModel(MarketingLeadCallDetail domain);
        RoutingNumberViewModel CreateViewModel(RoutingNumber domain);
        WebLeadInfoModel CreateViewModel(WebLead domain);
        MarketingLeadChartViewModel CreateViewModel(MarketingLeadChartViewModel viewModel,int? reportType);
        HomeAdvisorParentModel CreateViewModelForHomeAdvisor(HomeAdvisor domain);
        MarketingLeadCallDetailV2 CreateModel(CallDetailV2 record);
        CallDetailViewModelV2 CreateViewModel(MarketingLeadCallDetailV2 domain);
        MarketingLeadCallDetailViewModel CreateModelForNewApi(CallRetailRecordV3 record,long callType);
        MarketingLeadCallDetailV2 CreateModelForNewAPI(CallRetailRecordV3 record, string sid,DateTime dateTime);
        PhoneCallViewModel CreatePhoneViewModel(FranchiseeTechMailEmail domain, Phonechargesfee phonechargesfee);
        PhoneCallVInvoiceiewModel CreatePhoneInvoiceViewModel(Phonechargesfee phonechargesfee);
        MarketingLeadCallDetailV3 CreateModelForNewAPI3(CallRetailRecordV3 record, string sid, DateTime callDate);
        MarketingLeadCallDetailV4 CreateModelForNewAPI4(CallRetailRecordV2 record, string sid, DateTime callDate);
        MarketingLeadCallDetailV5 CreateModelForNewAPI5(CallRetailRecordV2 record, string sid, DateTime callDate);
        CallDetailViewModel CreateNewViewModel(MarketingLeadCallDetail domain, MarketingLeadCallDetailV3 domain3,
            MarketingLeadCallDetailV4 domain4, MarketingLeadCallDetailV5 domain5, MarketingLeadCallDetailV2 domain2 , List<CallDetailsReportNotes> callNoteList);

        MarketingLeadCallDetailViewModel CreateModelForInvoca(InvocaCallDetails record, long callTypeId);

        MarketingLeadCallDetailV2 CreateModelForInvocaAPI(InvocaCallDetails record, string sid, string route);
        MarketingLeadCallDetailV3 CreateModelForInvocaAPI3(InvocaCallDetails record, string sid);
        MarketingLeadCallDetailV4 CreateModelForInvocaAPI4(InvocaCallDetails record, string sid);
        MarketingLeadCallDetailV5 CreateModelForInvocaAPI5(InvocaCallDetails record, string sid);
    }
}
