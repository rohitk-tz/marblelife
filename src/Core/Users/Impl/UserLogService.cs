using Core.Application;
using Core.Application.Attribute;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using System;
using System.Linq;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class UserLogService : IUserLogService
    {
        private readonly IRepository<UserLog> _userLogRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IClock _clock;
        private readonly IRepository<CustomerLog> _customerLogRepository;
        private readonly IRepository<EstimateInvoice> _estimateInvoiceRepository;
        private readonly IRepository<EstimateInvoiceService> _estimateInvoiceServiceRepository;

        public UserLogService(IUnitOfWork unitOfWork, IClock clock)
        {
            _userLogRepository = unitOfWork.Repository<UserLog>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _customerLogRepository = unitOfWork.Repository<CustomerLog>();
            _estimateInvoiceRepository = unitOfWork.Repository<EstimateInvoice>();
            _estimateInvoiceServiceRepository = unitOfWork.Repository<EstimateInvoiceService>();
            _clock = clock;
        }

        public bool IsTokenValid(string token)
        {
            return _userLogRepository.Get(t => t.SessionId == token && t.LoggedOutAt == null) != null;
        }

        public long GetUserId(string token)
        {
            return _userLogRepository.Get(t => t.SessionId == token).UserId;
        }


        public void SaveLoginSession(long userId, string sessionId, string clientIp,
            DateTime loginDateTime, string browser = "", string os = "", string userAgent = "")
        {
            var domain = new UserLog
            {
                LoggedInAt = loginDateTime,
                SessionId = sessionId,
                UserId = userId,
                ClientIp = clientIp,
                Browser = browser,
                OS = os,
                UserAgent = userAgent,
                IsNew = true
            };

            _userLogRepository.Save(domain);
        }

        public void EndLoggedinSession(string token)
        {
            var log = _userLogRepository.Get(t => t.SessionId == token && t.LoggedOutAt == null);

            if (log == null) return;
            var offset = _clock.CurrentTimeZone.BaseUtcOffset.TotalMinutes;
            log.LoggedOutAt = _clock.UtcNow;

            _userLogRepository.Save(log);
        }

        public bool IsFirstTimeLogin(long userId)
        {
            var logs = _userLogRepository.Fetch(x => x.UserId == userId);

            return logs.Count() < 2;
        }

        public void SaveLoginCustomerSession(long userId, long customerId, long? estimateInvoiceId, string sessionId, string clientIp,
            DateTime loginDateTime, long? typeId,string code, string browser = "" )
        {
            var estimateInvoice = _estimateInvoiceRepository.Table.FirstOrDefault(x => x.Id == estimateInvoiceId);
            var estimateInvoiceServices = _estimateInvoiceServiceRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoiceId).ToList();
            var domain = new CustomerLog
            {
                LoggedInAt = loginDateTime,
                SessionId = sessionId,
                CustomerId = userId == 0 ? default(long?) : userId,
                ClientIp = clientIp,
                Browser = browser,
                IsNew = true,
                EstimateCustomerId = customerId == 0 ? default(long?) : customerId,
                EstimateInvoiceId = estimateInvoiceId,
                IsPostSignature = estimateInvoice.SchedulerId != null ? true : false,
                TypeId = typeId,
                Code=code
            };

            _customerLogRepository.Save(domain);
        }
    }
}
