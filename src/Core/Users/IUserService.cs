using Core.Organizations;
using Core.Organizations.ViewModels;
using Core.Users.Domain;
using Core.Users.Enum;
using Core.Users.ViewModels;
using System.Collections.Generic;

namespace Core.Users
{
    public interface IUserService
    {
        UserEditModel Get(long userId, long franchiseeId=default(long));
        void Save(UserEditModel userEditModel);
        Person Save(FranchiseeEditModel franchiseeEditModel, Organization organization);
        void Delete(long userId);
        UserListModel GetUsers(UserListFilter filter, int pageNumber, int pageSize);
        UserViewModel GetUserDetails(long userId);
        IEnumerable<UserViewModel> GetUsersList(long organizationId, RoleType roleType);
        bool ManageAccount(long userId, long[] franchiseeIds);
        bool CheckSchedulerAssignment(UserEditModel model);
        bool VerifyOpsMgrRole(UserEditModel model);
        string GetImageUrl();

        void Save(UserEquipmentEditModel userEquipmentEditModel);
        bool UpdatingEquipmentUserDetails(Person person);
        bool EquipmentRoleLock(long? userId, bool isLock);
        string GetUserName(long? userId);
        bool SchedulerDefaultView(long? orgId, string defaultView);
        string GetDefaultView(long? orgId);
        UserSignatureListEditModel GetUserSignature(long? orgId, long? userId);
        bool SaveUserSignature(UserSignatureListSaveModel model, long? orgId, long? userId);
    }
}
