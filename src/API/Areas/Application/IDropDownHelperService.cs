using System.Collections.Generic;
using Api.Areas.Application.ViewModel;
using API.Areas.Application.ViewModel;

namespace Api.Areas.Application
{
    public interface IDropDownHelperService
    {
        IEnumerable<ServiceTypeListItem> GetServiceTypes();
        IEnumerable<ServiceTypeListItem> GetServiceTypesForInvoice();
        IEnumerable<ServiceTypeListItem> GetServiceTypesForInvoiceNew();
        IEnumerable<DropdownListItem> GetFranchiseeNameValuePair();
        IEnumerable<DropdownListItem> GetStateItems();
        IEnumerable<DropdownListItem> GetLookupItems(long lookupTypeId);
        IEnumerable<DropdownListItem> GetRoles(long roleId);

        IEnumerable<DropdownListItem> GetLastTwentyYears();
        IEnumerable<DropdownListItem> GetYearsForGrowthReport();
        IEnumerable<DropdownListItem> GetYears();
        IEnumerable<DropdownListItem> GetMonths();
        IEnumerable<DropdownListItem> GetCardType();
        IEnumerable<DropdownListItem> GetAccountType();
        IEnumerable<DropdownListItem> GetInstrumentType();

        IEnumerable<DropdownListItem> GetCountryItems();

        IEnumerable<DropdownListItem> GetUploadStatus();
        IEnumerable<DropdownListItem> GetInvoiceStatus();
        IEnumerable<DropdownListItem> GetMarketingClass();
        IEnumerable<DropdownListItem> GetLateFeeItemType();
        IEnumerable<DropdownListItem> GetActiveFranchiseeList();
        IEnumerable<DropdownListItem> GetMonthNames();
        IEnumerable<DropdownListItem> GetCallType();
        IEnumerable<MultiSelectListItem> GetServicesForDropdown();
        IEnumerable<MultiSelectListItem> GetClassForDropdown();
        IEnumerable<DropdownListItem> GetYearsForBatch();
        IEnumerable<MultiSelectListItem> GetAssigneeList(long franchiseeId, long RoleId, long userId,bool? status,string Role,long? isEmpty);
        IEnumerable<DropdownListItem> GetJobStatus();
        IEnumerable<DropdownListItem> GetSalesRepList(long franchiseeId, long RoleId, long userId);
        IEnumerable<MultiSelectListItem> GetTechList(long franchiseeId);
        IEnumerable<DropdownListItem> GetTimeZoneList();
        IEnumerable<DropdownListItem> GetReviewStatus();
        IEnumerable<DropdownListItem> GetFranchiseeForMissingAudit();
        IEnumerable<DropdownListItem> GetUserList(long franchiseeId, long roleId, long userId);
        IEnumerable<DropdownListItem> GetTagList();
        IEnumerable<MultiSelectListItem> GetProductChannel();
        IEnumerable<DropdownListItem> GetCreditType();
        IEnumerable<DropdownListItem> GetRoutingNumberList();
        IEnumerable<DropdownListItem> GetUrlList();
        IEnumerable<DropdownListItem> GetRepeatFrequency();
        IEnumerable<DropdownListItem> GetDocumentType();
        IEnumerable<DropdownListItem> GetNationalDocumentType();
        IEnumerable<MultiSelectListItem> GetPhoneLabelCategory();
        IEnumerable<DropdownListItem> GetRepresentativeList(long franchiseeId, long roleId, long userId, bool? status, string Role);
        IEnumerable<MultiSelectListItem> GetTechListForMeeting(long franchiseeId);
        IEnumerable<MultiSelectListItem> GetTechAndSalesList(long franchiseeId);
        IEnumerable<MultiSelectListItem> GetTechAndSalesListForEstimate(long franchiseeId);
        IEnumerable<DropdownListItem> GetDocumentTypeForFranchisee(long franchisee);
        IEnumerable<DropdownListItem> GetNationalTypeForFranchisee(long franchiseeId);
        IEnumerable<DropdownListItem> GetUserListForDocument(long franchiseeId);
        IEnumerable<DropdownListItem> GetActiveFranchiseeListWithOut0ML(long? userId, long roleId);
        IEnumerable<MultiSelectListItem> GetTechListForMeetingForUser(long franchiseeId);
        IEnumerable<MultiSelectListItem> GetAssigneeListForScheduler(long franchiseeId, long roleId, long userId, bool? status, string Role, long? isEmpty);
        IEnumerable<DropdownListItem> GetFranchiseeNameValuePairByRole(long? userId, long? roleId);
        IEnumerable<DropdownListItem> GetUserListForFA(long franchiseeId, long roleId, long userId);
        IEnumerable<DropdownListItem> GetFranchiseeListWithOut0ML();
        IEnumerable<DropdownListItem> GetFranchiseeNameValuePairByRoleForFA(long? userId, long? roleId);
        List<ServiceTypeGroupedListItem> GetServiceTypesNewOrder();
        List<ServiceTypeGroupedListItem> GetMarketingClassNewOrder();
        List<DropdownListItem> GetServiceTagCategories();
        List<DropdownListItem> GetListOfServices();
        IEnumerable<ServiceTypeListItem> GetAllServicesList();
    }
}