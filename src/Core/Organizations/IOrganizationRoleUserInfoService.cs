using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Users.Domain;
using System.Collections.Generic;

namespace Core.Organizations
{
    public interface IOrganizationRoleUserInfoService
    {
        Person GetPersonByOrganziationRoleUserId(long organizationRoleUserId);
        Organization GetOrganizationByOrganziationRoleUserId(long organizationRoleUserId);
        OrganizationRoleUser GetOrganizationRoleUserbyId(long organizationRoleUserId);
        OrganizationRoleUser GetOrganizationRoleUserbyEmail(string email);
        OrganizationRoleUser GetPrimaryContractOrganizationRoleUserByOrganizationId(long organizationId);
        OrganizationRoleUser GetOrganizationRoleUserbyUserId(long userId);
        IList<OrganizationRoleUser> GetOrganizationRoleUserByOrganizationId(long organizationId);
        bool DeleteOruOfFranchisee(long franchiseeId);
        FranchiseeInfoListModel GetFranchiseeListForLogin(long userId, long roleId);
        bool ManageFranchisee(ManageFranchiseeAccountModel model);
        FranchiseeInfoListModel GetFranchiseeInfoCollection(long userId);
        Organization GetOrganizationByOrganizationId(long orgId);
        ICollection<long> GetOrgRoleUserIdsByRole(long userId, long orgId);
        ICollection<long> GetOrgUserIdsByOrgUserId(long orgRoleId, long orgId);
        ICollection<long> GetOrgRoleUserIdsByUserId(long userId);
        Role GetRoleName(long roleId);
        bool ManageFrontOfficeFranchisee(ManageFranchiseeAccountModel model);
        ICollection<long> GetOrgUserIdsByOrgUserIdBySalesAndTech(long userId, long orgId);
        OrganizationRoleUser GetUserIdFromOrganizationRoleUserId(long organizationRoleUserId);
        ICollection<long> GetOrgUserIdsByOrgUserIdForMeeting(long userId, long orgId);
        ICollection<long> GetOrgRoleIdsByOrgUserId(long userId);
    }
}
