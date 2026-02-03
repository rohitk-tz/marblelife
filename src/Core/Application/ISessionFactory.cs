using Core.Users.Domain;
using Core.Users.ViewModels;

namespace Core.Application
{
    public interface ISessionFactory
    {
        ISessionContext BuildSession(ISessionContext sessionContext, UserLogin userLogin);
        ISessionContext BuildSession(ISessionContext sessionContext, long userId);
        UserSessionModel GetUserSessionModel(long userId);
        UserSessionModel GetActiveSessionModel(string sessionId);
        UserSessionModel GetCustomerSessionModel(long? customerId, long? estimateCustomerId, long? estimateInvoiceId,long? typeId,string code);
    }
}
