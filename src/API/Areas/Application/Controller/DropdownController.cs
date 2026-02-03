using System.Collections.Generic;
using System.Web.Http;
using Api.Areas.Application.ViewModel;
using Core.Application;
using Core.Application.Enum;
using API.Areas.Application.ViewModel;
using Core.Users.Enum;
using Core.Organizations;
using System.Linq;
using Core.Scheduler.ViewModel;

namespace Api.Areas.Application.Controller
{

    public class DropdownController : BaseController
    {
        private readonly IDropDownHelperService _dropDownHelperService;
        private readonly ISessionContext _sessionContext;
        private readonly IOrganizationRoleUserInfoService _orgRoleUserInfoService;

        public DropdownController(IDropDownHelperService dropDownHelperService, ISessionContext sessionContext,
            IOrganizationRoleUserInfoService orgRoleUserInfoService)
        {
            _dropDownHelperService = dropDownHelperService;
            _sessionContext = sessionContext;
            _orgRoleUserInfoService = orgRoleUserInfoService;
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetStates()
        {
            return _dropDownHelperService.GetStateItems();
        }


        [HttpGet]
        public IEnumerable<DropdownListItem> GetFranchiseeNameValuePair()
        {
            return _dropDownHelperService.GetFranchiseeNameValuePair();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetActiveFranchiseeList()
        {
            return _dropDownHelperService.GetActiveFranchiseeList();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetActiveFranchiseeListWithOut0ML()
        {
            long userId = _sessionContext.UserSession.UserId;
            long roleId = _sessionContext.UserSession.RoleId;
            return _dropDownHelperService.GetActiveFranchiseeListWithOut0ML(userId, roleId);
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetServiceTypes()
        {
            return _dropDownHelperService.GetServiceTypes();
        }
        
        [HttpGet]
        public IEnumerable<DropdownListItem> GetServiceTypesForInvoice()
        {
            return _dropDownHelperService.GetServiceTypesForInvoice();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetServiceTypesForInvoiceNew()
        {
            return _dropDownHelperService.GetServiceTypesForInvoiceNew();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetRoles()
        {
            return _dropDownHelperService.GetRoles(_sessionContext.UserSession.RoleId);
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetPhoneTypes()
        {
            return _dropDownHelperService.GetLookupItems((long)LookupTypes.Phone);
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetLastTwentyYears()
        {
            return _dropDownHelperService.GetLastTwentyYears();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetYearsForGrowthReport()
        {
            return _dropDownHelperService.GetYearsForGrowthReport();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetYears()
        {
            return _dropDownHelperService.GetYears();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetMonths()
        {
            return _dropDownHelperService.GetMonths();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetMonthNames()
        {
            return _dropDownHelperService.GetMonthNames();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetCardType()
        {
            return _dropDownHelperService.GetCardType();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetAccountType()
        {
            return _dropDownHelperService.GetAccountType();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetInstrumentType()
        {
            return _dropDownHelperService.GetInstrumentType();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetSalesDataUploadStatus()
        {
            return _dropDownHelperService.GetUploadStatus();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetInvoiceStatus()
        {
            return _dropDownHelperService.GetInvoiceStatus();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetMarketingClass()
        {
            return _dropDownHelperService.GetMarketingClass();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetLateFeeItemType()
        {
            return _dropDownHelperService.GetLateFeeItemType();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetCallType()
        {
            return _dropDownHelperService.GetCallType();
        }

        [HttpGet]
        public IEnumerable<MultiSelectListItem> GetServicesForDropdown()
        {
            return _dropDownHelperService.GetServicesForDropdown();
        }
        [HttpGet]
        public IEnumerable<MultiSelectListItem> GetClassForDropdown()
        {
            return _dropDownHelperService.GetClassForDropdown();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetYearsForBatch()
        {
            return _dropDownHelperService.GetYearsForBatch();
        }

        [HttpPost]
        public IEnumerable<MultiSelectListItem> GetAssigneeList([FromBody] JobListFilter query)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive && query.FranchiseeId <= 1)
            {
                query.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetAssigneeList(query.FranchiseeId, _sessionContext.UserSession.RoleId, _sessionContext.UserSession.UserId, query.Status, query.Role, query.IsEmpty);
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetJobStatus()
        {
            return _dropDownHelperService.GetJobStatus();
        }
        [HttpPost]
        public IEnumerable<DropdownListItem> GetRepList([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (franchiseeId <= 1 && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetRepresentativeList(franchiseeId, _sessionContext.UserSession.RoleId, _sessionContext.UserSession.UserId, true, null);
        }
        [HttpPost]
        public IEnumerable<DropdownListItem> GetSalesRepList([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (franchiseeId <= 1 && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetSalesRepList(franchiseeId, _sessionContext.UserSession.RoleId, _sessionContext.UserSession.UserId);
        }



        [HttpPost]
        public IEnumerable<MultiSelectListItem> GetTechAndSalesList([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (franchiseeId <= 1 && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetTechAndSalesList(franchiseeId);
        }
        [HttpPost]
        public IEnumerable<MultiSelectListItem> GetTechList([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (franchiseeId <= 1 && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetTechList(franchiseeId);
        }
        [HttpPost]
        public IEnumerable<DropdownListItem> GetTimeZoneList()
        {
            return _dropDownHelperService.GetTimeZoneList();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetReviewStatus()
        {
            return _dropDownHelperService.GetReviewStatus();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetFranchiseeForMissingAudit()
        {
            return _dropDownHelperService.GetFranchiseeForMissingAudit();
        }

        [HttpPost]
        public IEnumerable<DropdownListItem> GetUserList([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _dropDownHelperService.GetUserList(franchiseeId, _sessionContext.UserSession.RoleId, _sessionContext.UserSession.UserId);
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetTagList()
        {
            return _dropDownHelperService.GetTagList();
        }

        [HttpGet]
        public IEnumerable<MultiSelectListItem> GetProductChannel()
        {
            return _dropDownHelperService.GetProductChannel();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetCreditType()
        {
            return _dropDownHelperService.GetCreditType();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetRoutingNumberList()
        {
            return _dropDownHelperService.GetRoutingNumberList();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetUrlList()
        {
            return _dropDownHelperService.GetUrlList();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetRepeatFrequency()
        {
            return _dropDownHelperService.GetRepeatFrequency();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetDocumentType()
        {
            return _dropDownHelperService.GetDocumentType();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetNationalDocumentType()
        {
            return _dropDownHelperService.GetNationalDocumentType();
        }

        [HttpGet]
        public IEnumerable<MultiSelectListItem> GetPhoneLabelCategory()
        {
            return _dropDownHelperService.GetPhoneLabelCategory();
        }


        [HttpPost]
        public IEnumerable<MultiSelectListItem> GetTechListForMeeting([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (franchiseeId <= 1 && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetTechListForMeeting(franchiseeId);
        }
        [HttpPost]
        public IEnumerable<MultiSelectListItem> GetTechAndSalesListForEstimate([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (franchiseeId <= 1 && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetTechAndSalesListForEstimate(franchiseeId);
        }
        [HttpPost]
        public IEnumerable<DropdownListItem> GetDocumentTypeForFranchisee([FromBody] long franchiseeId)
        {
            return _dropDownHelperService.GetDocumentTypeForFranchisee(franchiseeId);
        }
        [HttpPost]
        public IEnumerable<DropdownListItem> GetNationalTypeForFranchisee([FromBody] long franchiseeId)
        {
            return _dropDownHelperService.GetNationalTypeForFranchisee(franchiseeId);
        }
        [HttpPost]
        public IEnumerable<DropdownListItem> GetUserListForDocument([FromBody] long franchiseeId)
        {
            return _dropDownHelperService.GetUserListForDocument(franchiseeId);
        }

        [HttpPost]
        public IEnumerable<MultiSelectListItem> GetTechListForMeetingForUser([FromBody] long franchiseeId)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (franchiseeId <= 1 && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                franchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetTechListForMeetingForUser(franchiseeId);
        }
        [HttpPost]
        public IEnumerable<MultiSelectListItem> GetAssigneeListForScheduler([FromBody] JobListFilter query)
        {
            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                query.FranchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            if (_sessionContext.UserSession.RoleId == (long)RoleType.FrontOfficeExecutive && query.FranchiseeId <= 1)
            {
                query.FranchiseeId = _sessionContext.UserSession.LoggedInOrganizationId.Value;
            }
            return _dropDownHelperService.GetAssigneeListForScheduler(query.FranchiseeId, _sessionContext.UserSession.RoleId, _sessionContext.UserSession.UserId, query.IsLock, query.Role, query.IsEmpty);
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetFranchiseeNameValuePairByRole()
        {
            long userId = _sessionContext.UserSession.UserId;
            long roleId = _sessionContext.UserSession.RoleId;
            return _dropDownHelperService.GetFranchiseeNameValuePairByRole(userId, roleId);
        }
        [HttpPost]
        public IEnumerable<DropdownListItem> GetUserListForFA([FromBody] long franchiseeId)
        {

            if (_sessionContext.UserSession.RoleId != (long)RoleType.SuperAdmin && _sessionContext.UserSession.RoleId != (long)RoleType.FrontOfficeExecutive)
            {
                if (franchiseeId == 0)
                    franchiseeId = _sessionContext.UserSession.OrganizationId;
            }
            return _dropDownHelperService.GetUserListForFA(franchiseeId, _sessionContext.UserSession.RoleId, _sessionContext.UserSession.UserId);
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetFranchiseeListWithOut0ML()
        {
            return _dropDownHelperService.GetFranchiseeListWithOut0ML();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetFranchiseeNameValuePairByRoleForFA()
        {
            long userId = _sessionContext.UserSession.UserId;
            long roleId = _sessionContext.UserSession.RoleId;
            return _dropDownHelperService.GetFranchiseeNameValuePairByRoleForFA(userId, roleId);
        }

        [HttpGet]
        public List<ServiceTypeGroupedListItem> GetServiceTypesNewOrder()
        {
            return _dropDownHelperService.GetServiceTypesNewOrder();
        }
        [HttpGet]
        public List<ServiceTypeGroupedListItem> GetMarketingClassNewOrder()
        {
            return _dropDownHelperService.GetMarketingClassNewOrder();
        }

        [HttpGet]
        public IEnumerable<DropdownListItem> GetServiceTagCategories()
        {
            return _dropDownHelperService.GetServiceTagCategories();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetListOfServices()
        {
            return _dropDownHelperService.GetListOfServices();
        }
        [HttpGet]
        public IEnumerable<DropdownListItem> GetAllServicesList()
        {
            return _dropDownHelperService.GetAllServicesList();
        }
    }
}
