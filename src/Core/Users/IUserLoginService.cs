using Core.Organizations.Domain;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Core.Users
{
    public interface IUserLoginService
    {
        UserLogin UpdateforInvalidAttempt(UserLogin login);
        UserLogin GetbyUserName(string userName);
        UserLogin GetbyUserId(long userId);
        UserLogin UpdateforValidAttempt(UserLogin login);
        bool IsUniqueUserName(string userName, long userId = 0);
        bool IsUniqueEmailAddress(string email, long userId = 0);
        bool Lock(long userId, bool isLocked, out bool lockResult, out bool isEquipment);
        CustomerSignatureInfo GetbyCode(string userName);
    }
}
