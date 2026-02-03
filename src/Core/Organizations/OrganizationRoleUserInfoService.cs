using Core.Application;
using Core.Application.Attribute;
using Core.Dashboard.Enum;
using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using System.Collections.Generic;
using System.Linq;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class OrganizationRoleUserInfoService : IOrganizationRoleUserInfoService
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IRepository<Organization> _organizationRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IOrganizationFactory _organizationFactory;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<Role> _role;
        private readonly IRepository<OrganizationRoleUserFranchisee> _organizationRoleUserFranchiseeRepository;

        public OrganizationRoleUserInfoService(IUnitOfWork unitOfWork, IOrganizationFactory organizationFactory)
        {
            _personRepository = unitOfWork.Repository<Person>();
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _organizationRepository = unitOfWork.Repository<Organization>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _organizationFactory = organizationFactory;
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _role = unitOfWork.Repository<Role>();
            _organizationRoleUserFranchiseeRepository = unitOfWork.Repository<OrganizationRoleUserFranchisee>();
        }

        public Role GetRoleName(long organizationRoleId)
        {
            return _role.Get(GetRoleIdFromOrganizationRoleUserId(organizationRoleId).RoleId);
        }

        public OrganizationRoleUser GetRoleIdFromOrganizationRoleUserId(long organizationRoleUserId)
        {
            return _organizationRoleUserRepository.Get((organizationRoleUserId));
        }
        public OrganizationRoleUser GetUserIdFromOrganizationRoleUserId(long organizationRoleUserId)
        {
            return _organizationRoleUserRepository.Get((organizationRoleUserId));
        }
        public Person GetPersonByOrganziationRoleUserId(long organizationRoleUserId)
        {
            return _personRepository.Get(GetOrganizationRoleUserbyId(organizationRoleUserId).UserId);
        }

        public Organization GetOrganizationByOrganziationRoleUserId(long organizationRoleUserId)
        {
            return _organizationRepository.Get(GetOrganizationRoleUserbyId(organizationRoleUserId).OrganizationId);
        }

        public OrganizationRoleUser GetOrganizationRoleUserbyId(long organizationRoleUserId)
        {
            return _organizationRoleUserRepository.Get(organizationRoleUserId);
        }

        public OrganizationRoleUser GetOrganizationRoleUserbyEmail(string email)
        {
            var person = _personRepository.Get(p => p.Email != null && p.Email == email);
            return _organizationRoleUserRepository.Get(o => o.UserId == person.Id);
        }

        public OrganizationRoleUser GetPrimaryContractOrganizationRoleUserByOrganizationId(long organizationId)
        {
            return _organizationRoleUserRepository.Fetch(x => x.OrganizationId == organizationId).FirstOrDefault();
        }

        public OrganizationRoleUser GetOrganizationRoleUserbyUserId(long userId)
        {
            return _organizationRoleUserRepository.Fetch(x => x.UserId == userId).FirstOrDefault();
        }
        public IList<OrganizationRoleUser> GetOrganizationRoleUserByOrganizationId(long organizationId)
        {
            return _organizationRoleUserRepository.Fetch(x => x.OrganizationId == organizationId).ToArray();
        }

        public bool DeleteOruOfFranchisee(long franchiseeId)
        {
            var organizationRoleUser = GetOrganizationRoleUserByOrganizationId(franchiseeId);
            if (organizationRoleUser.Any())
            {
                foreach (var record in organizationRoleUser)
                {
                    var person = _personRepository.Fetch(x => x.Id == record.UserId).FirstOrDefault();
                    _personRepository.Delete(person);

                    var userLogin = _userLoginRepository.Fetch(x => x.Id == record.UserId).FirstOrDefault();
                    if (userLogin != null)
                        _userLoginRepository.Delete(userLogin);
                }
                return true;
            }
            return false;
        }

        public FranchiseeInfoListModel GetFranchiseeListForLogin(long userId, long roleId)
        {
            var orgRoleUserList = new List<OrganizationRoleUser>();
            var orgRoleUserForFrontOfficeList = new List<OrganizationRoleUserFranchisee>();

            orgRoleUserList = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId && x.IsActive
                                && (x.RoleId == (long)RoleType.FranchiseeAdmin)).ToList();

            if (roleId == (long)RoleType.FrontOfficeExecutive)
            {
                var orgRoleUser = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.UserId == userId && x.IsActive
                                && (x.RoleId == (long)RoleType.FrontOfficeExecutive) && x.Organization.IsActive) ;
                if (orgRoleUser != null)
                {
                    orgRoleUserForFrontOfficeList = orgRoleUser.OrganizationRoleUserFranchisee.Where(x => x.IsActive).ToList();
                    orgRoleUserForFrontOfficeList = orgRoleUserForFrontOfficeList.Where(x => x.Franchisee.Organization.IsActive).ToList();
                }
            }

            var model = new FranchiseeInfoListModel { };
            if (roleId == (long)RoleType.FrontOfficeExecutive)
            {
                model.Collection = orgRoleUserForFrontOfficeList.Select(_organizationFactory.CreateViewModel).ToList();
                model.Collection =  GetFranchiseeDirectoryListForSuperAdmin(model.Collection.ToList());
            }
            else
            {
                model.Collection = orgRoleUserList.Select(_organizationFactory.CreateViewModel).ToList();
                model.Collection = GetFranchiseeDirectoryListForSuperAdmin(model.Collection.ToList());
            };

            return model;
        }

        public bool ManageFranchisee(ManageFranchiseeAccountModel model)
        {
            var currentLoginOrg = _organizationRoleUserRepository.Get(x => x.OrganizationId == model.CurrentFranchiseeId
                                && x.UserId == model.UserId && (x.RoleId == (long)RoleType.FranchiseeAdmin) && x.IsActive);
            var orgToLogin = _organizationRoleUserRepository.Get(x => x.OrganizationId == model.FranchiseeId && x.UserId == model.UserId
                            && x.IsActive && (x.RoleId == (long)RoleType.FranchiseeAdmin));
            if (orgToLogin == null || currentLoginOrg == null)
                return false;

            currentLoginOrg.IsDefault = false;
            orgToLogin.IsDefault = true;
            _organizationRoleUserRepository.Save(currentLoginOrg);
            _organizationRoleUserRepository.Save(orgToLogin);

            return true;
        }

        public bool ManageFrontOfficeFranchisee(ManageFranchiseeAccountModel model)
        {
            var orgRoleUser = _organizationRoleUserRepository.Get(x => x.UserId == model.UserId
                            && (x.RoleId == (long)RoleType.FrontOfficeExecutive) && x.IsActive);


            if (orgRoleUser == null)
                return false;

            var loggedInOrg = orgRoleUser.OrganizationRoleUserFranchisee.FirstOrDefault(x => x.FranchiseeId == model.CurrentFranchiseeId);
            var orgToLogin = orgRoleUser.OrganizationRoleUserFranchisee.FirstOrDefault(x => x.FranchiseeId == model.FranchiseeId);

            if (loggedInOrg == null)
                return false;

            var listDefaultOrg = orgRoleUser.OrganizationRoleUserFranchisee.Where(x => x.OrganizationRoleUserId == orgRoleUser.Id
                                && x.IsActive && x.IsDefault).ToList();

            foreach (var org in listDefaultOrg)
            {
                org.IsDefault = false;
                _organizationRoleUserFranchiseeRepository.Save(org);
            }

            loggedInOrg.IsDefault = false;
            orgToLogin.IsDefault = true;
            _organizationRoleUserFranchiseeRepository.Save(loggedInOrg);
            _organizationRoleUserFranchiseeRepository.Save(orgToLogin);

            return true;
        }

        public FranchiseeInfoListModel GetFranchiseeInfoCollection(long userId)
        {
            var franchiseeCollection = _franchiseeRepository.Table.Where(x => x.Organization.IsActive).ToArray();

            var orgRoleUserCollection = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId && x.IsActive
                                        && x.RoleId == (long)RoleType.FranchiseeAdmin).ToArray();

            var list = (from franchisee in franchiseeCollection
                        join orgroleUser in orgRoleUserCollection on franchisee.Id equals orgroleUser.OrganizationId
                        into org
                        from orgroleUser in org.DefaultIfEmpty()
                        select new FranchiseeInfoViewModel
                        {
                            FranchiseeName = franchisee.Organization.Name,
                            IsActive = orgroleUser != null ? orgroleUser.IsActive : false,
                            IsDefault = orgroleUser != null ? orgroleUser.IsDefault : false,
                            OrganizationId = franchisee.Organization.Id,
                            OrganizationRoleUserId = orgroleUser != null ? orgroleUser.Id : 0,
                            UserId = orgroleUser != null ? orgroleUser.UserId : 0
                        }).OrderBy(x=>x.FranchiseeName);

            return new FranchiseeInfoListModel
            {
                Collection = list
            };
        }

        public Organization GetOrganizationByOrganizationId(long orgId)
        {
            return _organizationRepository.Get(orgId);
        }

        public ICollection<long> GetOrgRoleUserIdsByRole(long userId, long orgId)
        {
            var ids = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == orgId && x.IsActive
                        && (x.RoleId == (long)RoleType.Technician || x.UserId == userId)).Select(y => y.Id).Distinct().ToList();
            return ids;
        }

        public ICollection<long> GetOrgUserIdsByOrgUserId(long userId, long orgId)
        {
            var ids = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == orgId && x.IsActive
                    && x.UserId == userId).Select(y => y.Id).Distinct().ToList();
            return ids;
        }

        public ICollection<long> GetOrgRoleUserIdsByUserId(long userId)
        {
            var ids = _organizationRoleUserRepository.Table.Where(x => (x.UserId == userId)).Select(y => y.Id).Distinct().ToList();
            return ids;
        }
        public ICollection<long> GetOrgUserIdsByOrgUserIdBySalesAndTech(long userId, long orgId)
        {
            var ids = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == orgId && x.IsActive
                   && (x.RoleId == (long)RoleType.Technician || x.RoleId == (long)RoleType.SalesRep) && x.UserId == userId).Select(y => y.Id).Distinct().ToList();
            return ids;
        }
        public ICollection<long> GetOrgUserIdsByOrgUserIdForMeeting(long userId, long orgId)
        {
            var ids = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == orgId && x.IsActive
                       && x.UserId == userId).Select(y => y.Id).Distinct().ToList();
            return ids;
        }

        public ICollection<long> GetOrgRoleIdsByOrgUserId(long userId)
        {
            var ids = _organizationRoleUserRepository.Table.Where(x => x.IsActive
                    && x.UserId == userId).Select(y => y.RoleId).Distinct().ToList();
            return ids;
        }

        public List<FranchiseeInfoViewModel> GetFranchiseeDirectoryListForSuperAdmin(List<FranchiseeInfoViewModel> franchiseeInfoViewModel)
        {
            var organizationFinalsList = new List<FranchiseeInfoViewModel>();
            string[] sortingOrder = { "USA", "Canada", "South Africa", "Cayman Islands", "Bahamas", "United Arab Emirates" };
            var groupedOrganization = franchiseeInfoViewModel.GroupBy(x => x.CountryName);
            foreach (var countryName in sortingOrder)
            {
                var localGroupedOrganization = groupedOrganization.Where(x => x.Key == countryName).Select(x => x).ToList();
                if (localGroupedOrganization.Count() > 0)
                {
                    var localGroupedOrganization2 = localGroupedOrganization[0].ToList();
                    if (localGroupedOrganization2.Any(x => x.CountryName.StartsWith("0-")))
                    {
                        var zerofranchiseeNames = localGroupedOrganization2.Where(x => x.CountryName.StartsWith("0-")).OrderBy(x => x.CountryName).ToList();
                        var withoutZerofranchiseeNames = localGroupedOrganization2.Where(x => !x.CountryName.StartsWith("0-")).OrderBy(x => x.CountryName).ToList();
                        localGroupedOrganization2 = new List<FranchiseeInfoViewModel>();
                        localGroupedOrganization2.AddRange(withoutZerofranchiseeNames);
                        localGroupedOrganization2.AddRange(zerofranchiseeNames);
                        organizationFinalsList.AddRange(localGroupedOrganization2);
                    }
                    else
                    {
                        organizationFinalsList.AddRange(localGroupedOrganization2.OrderBy(x => x.FranchiseeName));
                    }
                }
            }
            return organizationFinalsList;
        }
    }
}