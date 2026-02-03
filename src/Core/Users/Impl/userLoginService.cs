using Core.Application;
using Core.Application.Attribute;
using Core.Organizations;
using Core.Scheduler;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using System;
using System.Linq;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class UserLoginService : IUserLoginService
    {
        private readonly IClock _clock;
        private readonly IUserLoginFactory _userLoginFactory;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<EquipmentUserDetails> _equipmentUserDetailsRepository;
        private readonly IOrganizationRoleUserInfoService _organizationRoleUserInfoService;
        private readonly IRepository<CustomerSignatureInfo> _customerSignatureInfoRepository;
        private readonly IJobService _jobService;

        public UserLoginService(IUnitOfWork unitOfWork, IClock clock, IUserLoginFactory userLoginFactory,
           IOrganizationRoleUserInfoService organizationRoleUserInfoService, IJobService jobService)
        {
            _clock = clock;
            _userLoginFactory = userLoginFactory;
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _personRepository = unitOfWork.Repository<Person>();
            _organizationRoleUserInfoService = organizationRoleUserInfoService;
            _jobService = jobService;
            _equipmentUserDetailsRepository= unitOfWork.Repository<EquipmentUserDetails>();
            _customerSignatureInfoRepository= unitOfWork.Repository<CustomerSignatureInfo>();
        }

        public UserLogin GetbyUserName(string userName)
        {
            return _userLoginRepository.Get(m => m.UserName.ToLower().Equals(userName.ToLower()));
        }

        public UserLogin GetbyUserId(long userId)
        {
            return _userLoginRepository.Get(userId);
        }

        public UserLogin UpdateforInvalidAttempt(UserLogin login)
        {
            login.LoginAttemptCount++;

            if (login.LoginAttemptCount >= UserLogin.MaxAttempts)
            {
                login.IsLocked = true;
            }

            _userLoginRepository.Save(login);

            return login;
        }

        public UserLogin UpdateforValidAttempt(UserLogin login)
        {
            login.LoginAttemptCount = 0;
            login.LastLoggedInDate = _clock.UtcNow;
            login.ResetToken = null;
            bool isEst = IsEST();
            if (isEst)
            {
                if (_clock.BrowserTimeZone == null)
                {
                    _clock.BrowserTimeZone = "330";
                }
                login.ESTOffset = Double.Parse(_clock.BrowserTimeZone);
            }
            else
            {
                if (_clock.BrowserTimeZone == null)
                {
                    _clock.BrowserTimeZone = "330";
                }
                login.EDTOffset = Double.Parse(_clock.BrowserTimeZone);
            }
            _userLoginRepository.Save(login);
            return login;
        }

        public bool IsEST()
        {
            DateTime dateNow = DateTime.Now;
            int secondSunday = 0;
            int firstSunday = 0;
            for (int d = 1; d < 31; d++)
            {
                DateTime _date = new DateTime(dateNow.Year, 3, d);
                DayOfWeek day = _date.DayOfWeek;

                if (day == DayOfWeek.Sunday)
                {
                    secondSunday = d + 7;
                    break;
                }

            }
            for (int d = 1; d < 30; d++)
            {
                DateTime _date = new DateTime(dateNow.Year, 11, d);
                DayOfWeek day = _date.DayOfWeek;

                if (day == DayOfWeek.Sunday)
                {
                    firstSunday = d;
                    break;
                }

            }
            DateTime _firstSundayNovember = new DateTime(dateNow.Year, 11, firstSunday);
            DateTime _secondSundayMarch = new DateTime(dateNow.Year, 3, secondSunday);

            if (dateNow >= _secondSundayMarch && dateNow <= _firstSundayNovember)
            {
                return false;
            }
            return true;
        }


        public bool IsUniqueUserName(string userName, long userId = 0)
        {
            return !_userLoginRepository.Table.Any(p => p.UserName.ToLower() == userName.ToLower() && (userId < 1 || userId != p.Id));
        }

        public bool IsUniqueEmailAddress(string email, long userId = 0)
        {
            if (email == null) return true;
            string upperEmail = email.ToUpper();
            var result = !_personRepository.Table.Any(p => upperEmail == p.Email.ToUpper() && (userId < 1 || userId != p.Id));
            return result;
        }

        public bool Lock(long userId, bool isLocked, out bool lockResult, out bool isEquipment)
        {
            var userLogin = GetbyUserId(userId);
            if (userLogin==null)
            {
                var orgRoleUserIdsForEqu = _organizationRoleUserInfoService.GetOrgRoleUserIdsByUserId(userId);
                var schedulesForEqu = _jobService.GetSchedulerForUserIds(orgRoleUserIdsForEqu);
                if (schedulesForEqu.Any() && !isLocked)
                {
                    lockResult = isLocked;
                    isEquipment = false;
                    return false;
                }
                lockResult = EquipmentRoleLock(userId, isLocked);
                isEquipment = true;
                if (isLocked)
                    lockResult = !isLocked;
                return true;
            }
            var orgRoleUserIds = _organizationRoleUserInfoService.GetOrgRoleUserIdsByUserId(userId);
            var orgRoleIdsByUserId = _organizationRoleUserInfoService.GetOrgRoleIdsByOrgUserId(userId);
           
            var schedules = _jobService.GetSchedulerForUserIds(orgRoleUserIds);
            if (orgRoleIdsByUserId.Contains(7) && !schedules.Any())
            {
                lockResult = EquipmentRoleLock(userId, isLocked);
            }
            if (schedules.Any() && !isLocked)
            {
                lockResult = isLocked;
                isEquipment = false;
                return false;
            }
            if (!isLocked)
            {
                userLogin.IsLocked = true;
                _userLoginRepository.Save(userLogin);
                lockResult = userLogin.IsLocked;
                isEquipment = false;
                return true;
            }
            else if (isLocked)
            {
                userLogin.IsLocked = false;
                userLogin.LoginAttemptCount = 0;
                _userLoginRepository.Save(userLogin);
                lockResult = userLogin.IsLocked;
                isEquipment = false;
                return true;
            }
            lockResult = userLogin.IsLocked;
            isEquipment = false;
            return userLogin.IsLocked;
        }

        private bool EquipmentRoleLock(long? userId, bool isLock)
        {
            var equipmentRole = _equipmentUserDetailsRepository.Table.FirstOrDefault(x => x.UserId == userId);
            if (equipmentRole == null)
                return false;

            equipmentRole.IsLock = !isLock;
            _equipmentUserDetailsRepository.Save(equipmentRole);
            return true;
        }

        public CustomerSignatureInfo GetbyCode(string userName)
        {
            return _customerSignatureInfoRepository.Get(m => m.Code.ToLower().Equals(userName.ToLower()));
        }

    }
}
