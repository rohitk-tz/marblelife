using System;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.ValueType;
using Core.Billing.Domain;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Users.Domain;
using Core.Users.Enum;
using Core.Users.ViewModels;
using System.Linq;
using Core.ToDo.Domain;
using Core.ToDo.Enum;
using Core.Sales.Domain;
using Core.Scheduler.Domain;

namespace Core.Application.Impl
{
    [DefaultImplementation]
    public class SessionFactory : ISessionFactory
    {
        private readonly IRepository<ToDoFollowUpList> _toDoFollowUpRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserLog> _userLogRepository;
        private readonly IRepository<CurrencyExchangeRate> _currencyExchangeRateRepository;
        private readonly ISettings _settings;
        private readonly IClock _clock;
        private readonly IRepository<OrganizationRoleUserFranchisee> _organizationRoleUserFranchiseeRepository;
        private readonly IRepository<CustomerLog> _customerLogRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<EstimateInvoiceCustomer> _estimateInvoiceCustomerRepository;
        private readonly IRepository<CustomerSignature> _customerSignatureRepository;
        private readonly IRepository<EstimateInvoice> _estimateInvoiceRepository;
        private readonly IRepository<CustomerSignatureInfo> _customerSignatureInfoRepository;
        public SessionFactory(IUnitOfWork unitOfWork, ISettings settings, IClock clock)
        {
            _customerSignatureInfoRepository = unitOfWork.Repository<CustomerSignatureInfo>();
            _personRepository = unitOfWork.Repository<Person>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _roleRepository = unitOfWork.Repository<Role>();
            _userLogRepository = unitOfWork.Repository<UserLog>();
            _currencyExchangeRateRepository = unitOfWork.Repository<CurrencyExchangeRate>();
            _settings = settings;
            _clock = clock;
            _organizationRoleUserFranchiseeRepository = unitOfWork.Repository<OrganizationRoleUserFranchisee>();
            _toDoFollowUpRepository = unitOfWork.Repository<ToDoFollowUpList>();
            _customerLogRepository = unitOfWork.Repository<CustomerLog>();
            _customerRepository = unitOfWork.Repository<Customer>();
            _estimateInvoiceCustomerRepository = unitOfWork.Repository<EstimateInvoiceCustomer>();
            _customerSignatureRepository = unitOfWork.Repository<CustomerSignature>();
            _estimateInvoiceRepository = unitOfWork.Repository<EstimateInvoice>();
        }


        public ISessionContext BuildSession(ISessionContext sessionContext, UserLogin userLogin)
        {
            var person = _personRepository.Get(userLogin.Id);
            return BuildSession(sessionContext, person);
        }

        public ISessionContext BuildSession(ISessionContext sessionContext, long userId)
        {
            return BuildSession(sessionContext, _personRepository.Get(userId));

        }

        public UserSessionModel GetUserSessionModel(long userId)
        {

            return BuildUserSessionModel(_personRepository.Get(userId));

        }

        public UserSessionModel GetCustomerSessionModel(long? customerId, long? estimateCustomerId, long? estimateInvoiceId, long? typeId, string code)
        {

            return BuildCustomerSessionModel(customerId, estimateCustomerId, estimateInvoiceId, typeId, code);

        }

        public UserSessionModel GetActiveSessionModel(string sessionId)
        {
            var log = _userLogRepository.Get(t => t.SessionId == sessionId && t.LoggedOutAt == null);

            var customerlog = _customerLogRepository.Get(t => t.SessionId == sessionId && t.LoggedOutAt == null);

            if (customerlog != null && log == null)
            {
                return GetCustomerSessionModel(customerlog.CustomerId, customerlog.EstimateCustomerId, customerlog.EstimateInvoiceId, customerlog.TypeId, customerlog.Code);
            }

            if (log == null)
            {
                return null;
            }

            return GetUserSessionModel(log.UserId);

        }

        private UserSessionModel BuildUserSessionModel(Person person)
        {
            var toDoList = _toDoFollowUpRepository.Table.ToList();
            var todayDate = DateTime.UtcNow.Date;
            var toDoListCount = _toDoFollowUpRepository.Table.Where(x => x.Date == todayDate && x.UserId == person.Id && (x.StatusId == (long?)(ToDoStatusEnum.OPEN) || x.StatusId == (long?)(ToDoStatusEnum.INPROGRESS))).Count();
            var relativeLocation = (MediaLocationHelper.GetTempImageLocation().Path) + "\\";
            var query = from acr in _organizationRoleUserRepository.Table
                        join a in _organizationRepository.Table on acr.OrganizationId equals a.Id
                        join r in _roleRepository.Table on (long)acr.RoleId equals r.Id
                        where acr.UserId == person.Id && acr.IsActive && acr.IsDefault
                        select new UserSessionModel
                        {
                            OrganizationId = a.Id,
                            OrganizationName = a.Name,
                            RoleId = r.Id,
                            AccessOrder = r.AccessOrder,
                            LastLoginAt = person.UserLogin.LastLoggedInDate,
                            RoleAlias = r.Alias,
                            RoleName = r.Name,
                            OrganizationRoleUserId = acr.Id,
                            UserId = person.UserLogin.Id,
                            Name = new Name { FirstName = person.FirstName, LastName = person.LastName },
                            TimeZoneId = _settings.DefaultTimeZone,
                            CurrencyCode = "USD",
                            FileName = acr.Person.FileId != null ? (relativeLocation + acr.Person.File.Name) : "",
                            Css = (acr.Person != null && acr.Person.File != null) ? acr.Person.File.css : null,
                            TeamFileName = (a.Franchisee != null && a.Franchisee.File != null) ? (relativeLocation + a.Franchisee.File.Name) : "",
                            TodayToDoCount = toDoListCount
                        };


            var userSession = query.OrderBy(x => x.AccessOrder).FirstOrDefault();
            if (userSession.RoleId == (long)RoleType.FranchiseeAdmin)
            {
                var franchieeIdList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userSession.UserId).Select(x => x.OrganizationId).ToList();
                var userIdList = _organizationRoleUserRepository.Table.Where(x => franchieeIdList.Contains(x.OrganizationId)).Select(x => x.UserId).ToList();
                var totalCount = toDoList.Where(x => x.Date == todayDate && userIdList.Contains(x.UserId.Value) && (x.StatusId == (long?)(ToDoStatusEnum.OPEN) || x.StatusId == (long?)(ToDoStatusEnum.INPROGRESS))).Count();
                userSession.TodayToDoCount = totalCount;
            }
            else if (userSession.RoleId == (long)RoleType.FrontOfficeExecutive || userSession.RoleId == (long)RoleType.SuperAdmin)
            {
                var totalCount = toDoList.Where(x => x.Date == todayDate && (x.StatusId == (long?)(ToDoStatusEnum.OPEN) || x.StatusId == (long?)(ToDoStatusEnum.INPROGRESS))).Count();
                userSession.TodayToDoCount = totalCount;
            }
            if (_organizationRepository.Get(userSession.OrganizationId).Franchisee != null)
            {
                userSession.CurrencyCode = _organizationRepository.Get(userSession.OrganizationId).Franchisee.Currency;
            }
            if (userSession.RoleId == (long)RoleType.FrontOfficeExecutive)
            {
                var loggedInFranchisee = _organizationRoleUserFranchiseeRepository.Table
                    .Where(x => x.OrganizationRoleUserId == userSession.OrganizationRoleUserId && x.IsDefault).FirstOrDefault();
                if (loggedInFranchisee != null)
                {
                    userSession.TeamFileName = (loggedInFranchisee != null && loggedInFranchisee.Franchisee.File != null) ? (relativeLocation + loggedInFranchisee.Franchisee.File.Name) : "";
                    userSession.LoggedInOrganizationId = loggedInFranchisee.FranchiseeId;
                    if (_organizationRepository.Get(userSession.LoggedInOrganizationId.Value).Franchisee != null)
                        userSession.CurrencyCode = _organizationRepository.Get(userSession.LoggedInOrganizationId.Value).Franchisee.Currency;

                }

            }
            return userSession;

        }


        private UserSessionModel BuildCustomerSessionModel(long? customerId, long? estimateCustomerId, long? estimateInvoiceId, long? typeId, string code)
        {
            var customerSignatureInfo = new CustomerSignatureInfo();
            var customerSignature = new CustomerSignature();
            var customerName = "";
            var estimateInvoice = _estimateInvoiceRepository.Get(estimateInvoiceId.GetValueOrDefault());
            var schedulerId = default(long?);
            if (customerId != null)
            {
                var customerDomain = _customerRepository.Get(customerId.GetValueOrDefault());
                customerName = customerDomain.Name;
            }
            else
            {
                var customerDomain = _estimateInvoiceCustomerRepository.Get(estimateCustomerId.GetValueOrDefault());
                customerName = customerDomain.CustomerName;

            }

            var customerSignatureInfoLocal = _customerSignatureInfoRepository.Table.FirstOrDefault(x => x.TypeId == typeId && x.Code == code);
             customerSignature = _customerSignatureRepository.Table.Where(x => x.EstimateInvoiceId == estimateInvoiceId).OrderByDescending(x => x.Id).FirstOrDefault();

            if (customerSignature == null)
            {

                schedulerId = estimateInvoice.SchedulerId;
            }
            return new UserSessionModel()
            {
                CustomerId = customerId,
                CustomerName = customerName,
                EstimateInvoiceId = estimateInvoiceId.GetValueOrDefault(),
                EstimateCustomerId = estimateCustomerId.GetValueOrDefault(),
                IsSigned = customerSignature != null && customerSignature.Signature != null ? true : false,
                Signature = customerSignature != null && customerSignature.Signature != null ? customerSignature.Signature : "",
                SchedulerId = customerSignature != null && customerSignature.EstimateInvoice != null ? customerSignature.EstimateInvoice.SchedulerId : schedulerId,
                OrganizationRoleUserId = 1027,
                IsPostSignature = estimateInvoice.SchedulerId != null ? true : false,
                TypeId = typeId,
                JobOrginialSchedulerId = customerSignatureInfoLocal != null ? customerSignatureInfoLocal.JobSchedulerId : default(long?)
            };
        }
        private ISessionContext BuildSession(ISessionContext sessionContext, Person person)
        {
            sessionContext.UserSession = BuildUserSessionModel(person);

            return sessionContext;
        }
    }
}
