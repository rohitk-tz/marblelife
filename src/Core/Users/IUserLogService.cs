using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users
{
    public interface IUserLogService
    {

        bool IsTokenValid(string token);
        long GetUserId(string token);

        void SaveLoginSession(long userId, string sessionId, string clientIp,
            DateTime loginDateTime, string browser = "", string os = "", string userAgent = "");

        void EndLoggedinSession(string token);
        bool IsFirstTimeLogin(long userId);
        void SaveLoginCustomerSession(long userId, long customerId, long? estimateInvoiceId, string sessionId, string clientIp,
             DateTime loginDateTime, long? typeId,string code, string browser = "");
    }
}
