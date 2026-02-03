using Core.Application;
using Core.Application.Attribute;
using Core.Application.Extensions;
using Core.Application.Impl;
using Core.Application.ViewModel;
using Core.Geo;
using Core.Organizations;
using Core.Organizations.Domain;
using Core.Organizations.Enum;
using Core.Organizations.Impl;
using Core.Organizations.ViewModels;
using Core.Scheduler.Domain;
using Core.Scheduler.ViewModel;
using Core.Users.Domain;
using Core.Users.Enum;
using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Application.Domain;
namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class UserService : IUserService
    {
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<UserLogin> _userLoginRepository;
        private readonly IRepository<OrganizationRoleUser> _organizationRoleUserRepository;
        private readonly IUserFactory _userFactory;
        private readonly IPersonFactory _personFactory;
        private readonly IUserLoginFactory _userLoginFactory;
        private readonly IOrganizationFactory _organizationFactory;
        private readonly IUserLoginService _userLoginService;
        private readonly IPhoneService _phoneService;
        private readonly ISendLoginCredentialsService _sendLoginCredentialService;
        private readonly ISortingHelper _sortingHelper;
        private readonly IRepository<SalesRep> _salesRepRepository;
        private readonly IRepository<Franchisee> _franchiseeRepository;
        private readonly IRepository<JobScheduler> _jobSchedulerRepository;
        private readonly FranchiseeDocumentFactory _documentFactory;
        private readonly IRepository<Core.Application.Domain.File> _fileRepository;
        public readonly IClock _clock;
        private readonly IFileService _fileService;
        private ISettings _settings;
        private readonly IRepository<OrganizationRoleUserFranchisee> _organizationRoleUserFranchiseeRepository;
        private readonly IRepository<FranchiseDocument> _franchiseeDocumentRepository;
        private readonly IRepository<EquipmentUserDetails> _equipmentUserDetailsRepository;
        private readonly IRepository<EmailSignatures> _emailSignaturesRepository;
        public UserService(IUnitOfWork unitOfWork, IPersonFactory personFactory, IUserLoginFactory userLoginFactory,
            IOrganizationFactory organizationFactory, IUserFactory userFactory, IUserLoginService userLoginService,
            IPhoneService phoneService, ISendLoginCredentialsService sendLoginCredentialService, ISortingHelper sortingHelper, IClock clock, IFileService fileService, ISettings settings, FranchiseeDocumentFactory documentFactory)
        {
            _personRepository = unitOfWork.Repository<Person>();
            _userLoginRepository = unitOfWork.Repository<UserLogin>();
            _personFactory = personFactory;
            _userLoginFactory = userLoginFactory;
            _organizationFactory = organizationFactory;
            _userFactory = userFactory;
            _organizationRoleUserRepository = unitOfWork.Repository<OrganizationRoleUser>();
            _userLoginService = userLoginService;
            _phoneService = phoneService;
            _sendLoginCredentialService = sendLoginCredentialService;
            _sortingHelper = sortingHelper;
            _salesRepRepository = unitOfWork.Repository<SalesRep>();
            _franchiseeRepository = unitOfWork.Repository<Franchisee>();
            _jobSchedulerRepository = unitOfWork.Repository<JobScheduler>();
            _clock = clock;
            _fileService = fileService;
            _settings = settings;
            _organizationRoleUserFranchiseeRepository = unitOfWork.Repository<OrganizationRoleUserFranchisee>();
            _franchiseeDocumentRepository = unitOfWork.Repository<FranchiseDocument>();
            _documentFactory = documentFactory;
            _fileRepository = unitOfWork.Repository<Core.Application.Domain.File>();
            _equipmentUserDetailsRepository = unitOfWork.Repository<EquipmentUserDetails>();
            _emailSignaturesRepository = unitOfWork.Repository<EmailSignatures>();
        }

        public UserEditModel Get(long userId, long franchiseeId)
        {
            var css = "";
            var organizationRoleUsers = new List<OrganizationRoleUser>();
            organizationRoleUsers = _organizationRoleUserRepository.Fetch(x => x.UserId == userId && x.IsActive && x.OrganizationId == franchiseeId).ToList();
            if (organizationRoleUsers.Count == 0)
            {
                var model = new UserEditModel();
                model.PersonEditModel.PhoneNumbers = _phoneService.GetDefaultPhoneModel();
                return model;
            }
            var person = _personRepository.Get(userId);
            if (person.FileId != null)
            {
                var file = _fileRepository.Get(person.FileId.GetValueOrDefault());
                css = file.css;
            }
            if (franchiseeId == default(long))
            {
                organizationRoleUsers = _organizationRoleUserRepository.Fetch(x => x.UserId == userId && x.IsActive).ToList();
            }
            var isExecutive = organizationRoleUsers.Any(x => x.RoleId == (long)RoleType.FrontOfficeExecutive);
            var organizationId = organizationRoleUsers.FirstOrDefault() != null ? organizationRoleUsers.FirstOrDefault().OrganizationId : 0;
            var salesRepUser = organizationRoleUsers.Where(x => x.RoleId == (long)RoleType.SalesRep).FirstOrDefault();
            var salesRep = salesRepUser != null ? _salesRepRepository.Fetch(x => x.Id == salesRepUser.Id).FirstOrDefault() : null;
            var roleIds = organizationRoleUsers.Select(x => x.RoleId).Distinct().ToList();
            var organizationIds = organizationRoleUsers.Select(x => x.OrganizationId).ToList();
            var roleId = organizationRoleUsers.FirstOrDefault() != null ? organizationRoleUsers.FirstOrDefault().RoleId : 0;
            var colorCode = (roleIds.Any() && (roleIds.Any(x => x == (long)RoleType.Technician))
                                ? organizationRoleUsers.FirstOrDefault(x => x.ColorCode != null).ColorCode : null);
            var colorCodeSales = (roleIds.Any() && (roleIds.Any(x => x == (long)RoleType.SalesRep))
                                ? organizationRoleUsers.FirstOrDefault(x => x.ColorCodeSale != null).ColorCodeSale : null);
            var franchiseeDocument = _franchiseeDocumentRepository.Table.Where(x => x.FranchiseeId == organizationId && x.UserId == userId && x.DocumentTypeId == (long)(Organizations.Enum.DocumentType.NCA)).ToList().LastOrDefault();
            if (isExecutive)
            {
                var organizationRoleUser = _organizationRoleUserRepository.Get(x => x.RoleId == (long)RoleType.FrontOfficeExecutive
                                                && x.UserId == userId);
                if (organizationRoleUser != null)
                {
                    organizationIds = organizationRoleUser.OrganizationRoleUserFranchisee.Any() ?
                        organizationRoleUser.OrganizationRoleUserFranchisee.Where(x => x.IsActive).Select(y => y.FranchiseeId).ToList() : null;
                }
            }
            var userEditModel = new UserEditModel
            {
                PersonEditModel = _personFactory.CreateModel(person),
                UserLoginEditModel = person.UserLogin != null ? _userLoginFactory.CreateEditModel(person.UserLogin) : null,
                OrganizationId = organizationId,
                RoleId = roleId,
                Alias = salesRep != null ? salesRep.Alias : null,
                RoleIds = roleIds,
                OrganizationIds = organizationIds,
                IsExecutive = isExecutive,
                franchiseeDocument = franchiseeDocument != null ? _documentFactory.CreateDomainForUser(franchiseeDocument) : null,
                FileId = person.FileId,
            };
            if (franchiseeDocument != null)
            {
                userEditModel.FileName = franchiseeDocument.File != null ? franchiseeDocument.File.Name : "";
            }
            if (userEditModel.PersonEditModel.PhoneNumbers.Count() < 1)
                userEditModel.PersonEditModel.PhoneNumbers = _phoneService.GetDefaultPhoneModel();
            userEditModel.PersonEditModel.Color = colorCode;
            userEditModel.PersonEditModel.ColorCodeSale = colorCodeSales;
            userEditModel.Css = css;
            return userEditModel;
        }
        public UserViewModel GetUserDetails(long userId)
        {
            var organizationRoleUser = _organizationRoleUserRepository.Fetch(x => x.UserId == userId && x.IsActive).FirstOrDefault();
            return organizationRoleUser == null ? new UserViewModel() : _userFactory.CreateViewModel(organizationRoleUser);
        }
        public long SavesImage(FileUploadModel model)
        {
            foreach (var fileModel in model.FileList)
            {
                try
                {
                    if (fileModel.Id > 0)
                    {
                        var fileRepository = _fileRepository.Get(fileModel.Id);
                        fileRepository.IsNew = false;
                        fileRepository.css = model.css;
                        _fileRepository.Save(fileRepository);
                        continue;
                    }
                    var path = MediaLocationHelper.FilePath(fileModel.RelativeLocation, fileModel.Name).ToFullPath();
                    var destination = MediaLocationHelper.GetTempImageLocation();
                    var destFileName = string.Format((fileModel.Caption.Length <= 20) ? fileModel.Caption + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow)
                                        : fileModel.Caption.Substring(0, 20) + "_" + string.Format("{0:yyyy-MM-dd_hh-mm-ss}", _clock.UtcNow));
                    var fileName = _fileService.MoveFile(path, destination, destFileName, fileModel.Extension);
                    fileModel.Name = destFileName + fileModel.Extension;
                    fileModel.RelativeLocation = MediaLocationHelper.GetTempImageLocation().Path;
                    //string[] filePath = fileModel.RelativeLocation.Split("//");
                    string folderName = Path.GetFileName(fileModel.RelativeLocation);
                    fileModel.css = model.css;
                    fileModel.RelativeLocation = "\\" + folderName;
                    var file = _fileService.SaveModel(fileModel);
                    return file.Id;
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return default(long);
        }
        public void Save(UserEditModel userEditModel)
        {
            var previousPerson = default(Person);
            var isEquipmentPresent = false;
            var isLocked = false;
            if (userEditModel.PersonEditModel != null)
            {
                userEditModel.PersonEditModel.FileId = SaveUserImage(userEditModel);
            }
            if (userEditModel.UserLoginEditModel != null)
            {
                var userLogin = _userLoginRepository.Table.FirstOrDefault(x => x.Id == userEditModel.PersonEditModel.PersonId);
                if (userLogin != null)
                {
                    isLocked = userLogin.IsLocked;
                }
            }
            if (userEditModel.PersonEditModel.PersonId > 0)
            {
                isEquipmentPresent = IsEquipmentPresent(userEditModel.PersonEditModel.PersonId);
                if (isEquipmentPresent && !isLocked)
                {
                    previousPerson = _personRepository.Get(userEditModel.PersonEditModel.PersonId);
                }
            }
            if (userEditModel.PersonEditModel.PersonId <= 0 && userEditModel.RoleIds.Any(x => x == (long)RoleType.SalesRep || x == (long)RoleType.Technician))
            {
                var franchisee = _franchiseeRepository.Get(userEditModel.OrganizationId);
                if (franchisee != null)
                {
                    userEditModel.PersonEditModel.IsRecruitmentFeeApplicable = franchisee.FranchiseeServiceFee.Any(x =>
                    x.ServiceFeeTypeId == (long)ServiceFeeType.Recruiting && x.IsActive);
                }
            }
            var person = _personFactory.CreateDomain(userEditModel.PersonEditModel);
            _personRepository.Save(person);
            userEditModel.PersonEditModel.Id = person.Id;
            var personEditModel = userEditModel.PersonEditModel;
            var isLoginPresent = userEditModel.UserLoginEditModel != null ? true : false;
            if (userEditModel.UserLoginEditModel != null && (userEditModel.CreateLogin == true || userEditModel.UserLoginEditModel.ChangePassword == true))
            {
                var persitentUserLogin = userEditModel.UserLoginEditModel.Id > 0
                ? _userLoginRepository.Get(userEditModel.UserLoginEditModel.Id)
                : null;
                if (userEditModel.UserLoginEditModel.SendUserLoginViaEmail == true)
                {
                    userEditModel.UserLoginEditModel.UserName = person.Email;
                }
                var userLogin = _userLoginFactory.CreateDomain(userEditModel.UserLoginEditModel, person, persitentUserLogin);
                _userLoginRepository.Save(userLogin);
                userEditModel.UserLoginEditModel.Id = userLogin.Id;
                if (userEditModel.RoleIds.Contains(7))
                {
                    var equipmentUserDetails = UpdatingEquipmentUserDetails(person);
                }
                if (isEquipmentPresent && userEditModel.RoleIds.Contains(7) && !isLocked)
                {
                    if (previousPerson.Id != userLogin.Id)
                    {
                        EditPersonUserId(previousPerson.Id, userLogin.Id);
                        DeletePerson(previousPerson.Id);
                        UpdatingScheduler(previousPerson.Id, userLogin.Id);
                    }
                }
            }
            if (userEditModel.IsExecutive)
            {
                CreateFrontOfficeExecutiveRole(userEditModel, person);
            }
            else
            {
                var userList = _organizationRoleUserRepository.Table.Where(x => x.UserId == person.Id).ToList();
                if (userList.Any())
                {
                    ManageRoles(userList, userEditModel.RoleIds);
                }
                foreach (var roleId in userEditModel.RoleIds)
                {
                    userEditModel.RoleId = roleId;
                    var organizationRoleUser = SaveOrganizationRoleUser(userEditModel, person);
                    if (userEditModel.RoleId == (long)RoleType.SalesRep)
                    {
                        var salesRep = _userFactory.CreateDomain(organizationRoleUser, userEditModel);
                        _salesRepRepository.Save(salesRep);
                    }
                }
            }
            if (userEditModel.UserLoginEditModel != null && person.IsNew) //&& userEditModel.UserLoginEditModel.SendUserLoginViaEmail == true
            {
                if (userEditModel.RoleIds.Any(x => x == (long)RoleType.SalesRep || x == (long)RoleType.Technician) || userEditModel.IsExecutive)
                {
                    _sendLoginCredentialService.SendLoginCredentials(person, userEditModel.UserLoginEditModel.Password, true);
                }
                else
                    _sendLoginCredentialService.SendLoginCredentials(person, userEditModel.UserLoginEditModel.Password, false);
            }
        }
        private long? SaveUserImage(UserEditModel userEditModel)
        {
            if (userEditModel.FileUploadModel != null && userEditModel.FileUploadModel.FileList.Count() > 0)
            {
                userEditModel.FileUploadModel.css = userEditModel.Css;
                long fileId = SavesImage(userEditModel.FileUploadModel);
                userEditModel.PersonEditModel.FileId = fileId;
                return fileId;
            }
            else
            {
                var fileId = _personRepository.Fetch(x => x.Id == userEditModel.UserLoginEditModel.Id).Select(x => x.FileId).FirstOrDefault();
                if (userEditModel.isImageChanged == false)
                {
                    if (fileId == null)
                    {
                        return null;
                    }
                    userEditModel.PersonEditModel.FileId = fileId;
                    var file = _fileRepository.Get(fileId.GetValueOrDefault());
                    if (file.css != userEditModel.Css)
                    {
                        file.css = userEditModel.Css;
                        file.IsNew = false;
                        _fileRepository.Save(file);
                        return file.Id;
                    }
                }
                else
                {
                    userEditModel.PersonEditModel.FileId = null;
                    return null;
                }
                return userEditModel.PersonEditModel.FileId;
            }
        }
        private OrganizationRoleUser CreateFrontOfficeExecutiveRole(UserEditModel model, Person person)
        {
            var userInfo = _organizationRoleUserRepository.Table.FirstOrDefault(x => x.UserId == person.Id
                                  && x.RoleId == (long)RoleType.FrontOfficeExecutive);
            if (userInfo == null)
            {
                userInfo = _organizationFactory.CreateDomain(model, person, userInfo);
                _organizationRoleUserRepository.Save(userInfo);
            }
            ManageUsers(userInfo, model.OrganizationIds);
            foreach (var orgId in model.OrganizationIds)
            {
                var orgRoleFranchisee = _organizationRoleUserFranchiseeRepository.Get(x => x.FranchiseeId == orgId && x.OrganizationRoleUserId == userInfo.Id);
                var userFranchisee = _organizationFactory.CreateDomain(orgId, userInfo, orgRoleFranchisee);
                _organizationRoleUserFranchiseeRepository.Save(userFranchisee);
            }
            return userInfo;
        }
        private OrganizationRoleUser SaveOrganizationRoleUser(UserEditModel userEditModel, Person person)
        {
            var organizationRoleUserDomain = _organizationRoleUserRepository.Get(x => x.RoleId == userEditModel.RoleId
                                            && x.UserId == person.Id && x.OrganizationId == userEditModel.OrganizationId);

            if (organizationRoleUserDomain != null && organizationRoleUserDomain.Organization != null)
                organizationRoleUserDomain.IsActive = true;

            var organizationRoleUser = _organizationFactory.CreateDomain(userEditModel, person, organizationRoleUserDomain);
            organizationRoleUser.UserId = person.Id;

            if (organizationRoleUser.Id <= 0 && (userEditModel.RoleId == (long)RoleType.Technician))
            {
                organizationRoleUser.ColorCode = userEditModel.PersonEditModel.Color;
                organizationRoleUser.ColorCodeSale = null;
            }
            if (organizationRoleUser.Id <= 0 && (userEditModel.RoleId == (long)RoleType.SalesRep))
            {
                organizationRoleUser.ColorCodeSale = userEditModel.PersonEditModel.ColorCodeSale;
                organizationRoleUser.ColorCode = null;
            }
            if (organizationRoleUser.Id > 0 && (userEditModel.RoleId == (long)RoleType.Technician))
            {
                organizationRoleUser.ColorCode = userEditModel.PersonEditModel.Color;
                organizationRoleUser.ColorCodeSale = null;
            }
            if (organizationRoleUser.Id > 0 && (userEditModel.RoleId == (long)RoleType.SalesRep))
            {
                organizationRoleUser.ColorCodeSale = userEditModel.PersonEditModel.ColorCodeSale;
                organizationRoleUser.ColorCode = null;
            }
            _organizationRoleUserRepository.Save(organizationRoleUser);
            return organizationRoleUser;
        }
        public void ManageUsers(OrganizationRoleUser userInfo, ICollection<long> orgIds)
        {
            var assignedFranchisees = (userInfo.OrganizationRoleUserFranchisee != null && userInfo.OrganizationRoleUserFranchisee.Any())
                                            ? userInfo.OrganizationRoleUserFranchisee.ToList() : null;
            if (assignedFranchisees == null || !assignedFranchisees.Any())
                return;
            var idsToDeactivate = assignedFranchisees.Where(x => !orgIds.Contains(x.FranchiseeId)).Select(x => x.Id).ToList();
            foreach (var id in idsToDeactivate)
            {
                var userFranchisee = _organizationRoleUserFranchiseeRepository.Get(id);
                if (userFranchisee != null)
                {
                    userFranchisee.IsActive = false;
                    _organizationRoleUserFranchiseeRepository.Save(userFranchisee);
                }
            }
        }
        public void ManageRoles(ICollection<OrganizationRoleUser> list, ICollection<long> roleIds)
        {
            var idsToDeactivate = list.Where(x => !roleIds.Contains(x.RoleId)).Select(x => x.Id).ToList();
            foreach (var id in idsToDeactivate)
            {
                var user = _organizationRoleUserRepository.Get(id);
                if (user != null)
                {
                    user.IsActive = false;
                    _organizationRoleUserRepository.Save(user);
                }
            }
        }
        private string GenerateColorCode(long franchiseeId)
        {
            var color = GetRandomColor();
            var inDBColorCodes = _organizationRoleUserRepository.Table.Where(x => x.OrganizationId == franchiseeId && x.ColorCode != null).ToList();
            while (inDBColorCodes.Where(x => x.ColorCode.ToUpper().Equals(color.ToUpper())).Any())
                color = GenerateColorCode(franchiseeId);
            return color;
        }
        private string GetRandomColor()
        {
            var random = new Random();
            return string.Format("#{0:X6}", random.Next(0x1000000));
        }
        public Person Save(FranchiseeEditModel franchiseeEditModel, Organization organization)
        {
            var owner = franchiseeEditModel.OrganizationOwner;
            if (owner.OwnerId <= 0)
            {
                var userLogin = _userLoginFactory.CreateDomain(owner, franchiseeEditModel.Email);
                var person = _personFactory.CreateDomain(owner, userLogin, franchiseeEditModel.Email);
                _personRepository.Save(person);
                if (person.Id > 0)
                {
                    var organizationroleUser = _organizationFactory.CreateDomain(person, organization, franchiseeEditModel.OrganizationOwner.OwnerId);
                    _organizationRoleUserRepository.Save(organizationroleUser);
                }
                if (userLogin != null && userLogin.IsNew == true && owner.SendUserLoginViaEmail == true)
                {
                    var password = owner.Password; // getRandomPassword();
                    _sendLoginCredentialService.SendLoginCredentials(person, password, false);
                }
                return person;
            }
            else
            {
                var person = _personRepository.Get(owner.OwnerId);
                person.FirstName = owner.OwnerFirstName;
                person.LastName = owner.OwnerLastName;
                if (person.Email != franchiseeEditModel.Email)
                {
                    person.Email = franchiseeEditModel.Email;
                }
                _personRepository.Save(person);
                return person;
            }
        }
        public void Delete(long userId)
        {
            var organizationRoleUserDomain = _organizationRoleUserRepository.Fetch(x => x.UserId == userId).FirstOrDefault();
            /*
            Add Code to Delete Specific User Profile
            */
            _organizationRoleUserRepository.Delete(x => x.UserId == userId);
            _userLoginRepository.Delete(userId);
            _personRepository.Delete(userId);
        }
        public UserListModel GetUsers(UserListFilter filter, int pageNumber, int pageSize)
        {
            var usersList = _organizationRoleUserRepository.Table.Where(x => (filter.FranchiseeId <= 1 || x.OrganizationId == filter.FranchiseeId)
                    && (filter.RoleId < 1 || filter.RoleId == x.RoleId)
                    && x.IsActive
                    && (string.IsNullOrEmpty(filter.Email) || (x.Person.Email.Contains(filter.Email)))
                    && (string.IsNullOrEmpty(filter.UserName) || (x.Person.UserLogin.UserName.Contains(filter.UserName)))
                    && ((filter.StatusId == null) || (x.Person.UserLogin.IsLocked == (filter.StatusId == 1 ? false : true)))
                    && (string.IsNullOrEmpty(filter.Text)
                    || (x.Organization.Name.Contains(filter.Text))
                    || (x.Person.Addresses.FirstOrDefault()).AddressLine1.Contains(filter.Text)
                    || (x.Person.Addresses.FirstOrDefault()).AddressLine2.Contains(filter.Text) || (x.Person.Addresses.FirstOrDefault()).City.Name.Contains(filter.Text)
                    || (x.Person.Addresses.FirstOrDefault()).State.Name.Contains(filter.Text) || (x.Person.Addresses.FirstOrDefault()).Zip.Code.Contains(filter.Text))
                ).ToList();

            var users = usersList.Where(x => (string.IsNullOrEmpty(filter.Name != null ? filter.Name.ToUpper() : filter.Name)
                    || x.Person.FullNameUser.ToUpper().Contains(filter.Name != null ? filter.Name.ToUpper() : filter.Name))).AsQueryable();

            if (filter.IsFrontOfficeExecutive)
            {
                users = users.Where(u => u.RoleId == (long)RoleType.SalesRep || u.RoleId == (long)RoleType.Technician
                        || u.RoleId == (long)RoleType.OperationsManager || u.RoleId == (long)RoleType.Equipment);
            }
            users = _sortingHelper.ApplySorting(users, x => x.Person.DataRecorderMetaData.DateCreated, (long)SortingOrder.Desc);
            if (filter.SortingColumn != null)
            {
                switch (filter.SortingColumn)
                {
                    case "Id":
                        users = _sortingHelper.ApplySorting(users, x => x.Id, filter.SortingOrder);
                        break;
                    case "Name":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.FirstName.ToString(), filter.SortingOrder);
                        break;
                    case "Email":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.Email, filter.SortingOrder);
                        break;
                    case "Username":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.UserLogin.UserName, filter.SortingOrder);
                        break;
                    case "FranchiseeName":
                        users = _sortingHelper.ApplySorting(users, x => x.Organization.Name, filter.SortingOrder);
                        break;
                    case "StreetAddress":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.Addresses.FirstOrDefault().AddressLine1, filter.SortingOrder);
                        break;
                    case "City":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.Addresses.FirstOrDefault().City.Name, filter.SortingOrder);
                        break;
                    case "State":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.Addresses.FirstOrDefault().State.Name, filter.SortingOrder);
                        break;
                    case "ZipCode":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.Addresses.FirstOrDefault().ZipCode, filter.SortingOrder);
                        break;
                    case "Country":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.Addresses.FirstOrDefault().Country.Name, filter.SortingOrder);
                        break;
                    case "LastLoginAt":
                        users = _sortingHelper.ApplySorting(users, x => x.Person.UserLogin.LastLoggedInDate, filter.SortingOrder);
                        break;
                    case "Role":
                        users = _sortingHelper.ApplySorting(users, x => x.Role.Name, filter.SortingOrder);
                        break;
                }
            }
            var finalcollection = users.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new UserListModel
            {
                Collection = finalcollection.Select(_userFactory.CreateViewModel).ToList(),
                Filter = filter,
                PagingModel = new PagingModel(pageNumber, pageSize, users.Count())
            };
        }
        public IEnumerable<UserViewModel> GetUsersList(long organizationId, RoleType roleType)
        {
            var users = _organizationRoleUserRepository.Fetch(x => x.OrganizationId == organizationId && x.Role.Id == (long)roleType && x.IsActive)
                            .Select(_userFactory.CreateViewModel).ToList();
            return users;
        }
        public bool ManageAccount(long userId, long[] franchiseeIds)
        {
            var orgRoleUser = _organizationRoleUserRepository.Table.Where(x => x.UserId == userId
                                && x.IsActive && x.RoleId == (long)RoleType.FranchiseeAdmin).FirstOrDefault();
            if (orgRoleUser == null)
                return false;
            var franchiseeList = _franchiseeRepository.Table.Where(x => franchiseeIds.Contains(x.Id)).ToList();
            foreach (var item in franchiseeList)
            {
                var orgWithAccessInDB = _organizationRoleUserRepository.Get(x => x.OrganizationId == item.Organization.Id && x.UserId == userId
                                            && x.IsActive && x.RoleId == (long)RoleType.FranchiseeAdmin);
                if (orgWithAccessInDB == null)
                {
                    var orgnRoleUser = _organizationFactory.CreateDomain(orgRoleUser, item);
                    _organizationRoleUserRepository.Save(orgnRoleUser);
                }
                else
                {
                    if (orgWithAccessInDB.IsDefault == true)
                        continue;
                    if (orgWithAccessInDB.IsActive == true)
                        orgWithAccessInDB.IsActive = false;
                    else orgWithAccessInDB.IsActive = true;
                    _organizationRoleUserRepository.Save(orgWithAccessInDB);
                }
            }
            return true;
        }
        public bool CheckSchedulerAssignment(UserEditModel model)
        {
            var result = true;
            if (model == null || model.IsExecutive || model.PersonEditModel == null || model.PersonEditModel.PersonId <= 0)
                return result;
            if (model.RoleIds.Any())
            {
                var userList = _organizationRoleUserRepository.Table.Where(x => x.UserId == model.PersonEditModel.PersonId && x.IsActive).ToList();
                var idsToDeactivate = userList.Where(x => !model.RoleIds.Contains(x.RoleId)).Select(x => x.Id).ToList();
                foreach (var id in idsToDeactivate)
                {
                    var hasJobs = _jobSchedulerRepository.Table.Where(x => x.AssigneeId == id && x.EndDate >= _clock.UtcNow).Any();
                    if (hasJobs)
                        return false;
                }
            }
            return result;
        }
        public bool VerifyOpsMgrRole(UserEditModel model)
        {
            if (model.RoleIds.Any(x => x == (long)RoleType.Technician))
                return true;
            return false;
        }
        public string GetImageUrl()
        {
            return _settings.SiteRootUrl;
        }
        public void Save(UserEquipmentEditModel userEditModel)
        {
            if (userEditModel.PersonEditModel.PersonId <= 0 && userEditModel.RoleIds.Any(x => x == (long)RoleType.SalesRep || x == (long)RoleType.Technician))
            {
                var franchisee = _franchiseeRepository.Get(userEditModel.OrganizationId);
                if (franchisee != null)
                {
                    userEditModel.PersonEditModel.IsRecruitmentFeeApplicable = franchisee.FranchiseeServiceFee.Any(x =>
                    x.ServiceFeeTypeId == (long)ServiceFeeType.Recruiting && x.IsActive);
                }
            }
            var person = _personFactory.CreateDomain(userEditModel.PersonEditModel);
            _personRepository.Save(person);
            userEditModel.PersonEditModel.Id = person.Id;
            var equipmentUserDetails = UpdatingEquipmentUserDetails(person);
            var userList = _organizationRoleUserRepository.Table.Where(x => x.UserId == person.Id).ToList();
            if (userList.Any())
            {
                ManageRoles(userList, userEditModel.RoleIds);
            }

            foreach (var roleId in userEditModel.RoleIds)
            {
                userEditModel.RoleId = roleId;

                var organizationRoleUser = SaveOrganizationRoleUserForEquipment(userEditModel, person);
                foreach (var user in userList)
                {
                    UpdatingSchedulerForEquipment(user.Id, organizationRoleUser.Id);
                }
            }
        }
        private OrganizationRoleUser SaveOrganizationRoleUserForEquipment(UserEquipmentEditModel userEditModel, Person person)
        {
            var organizationRoleUserDomain = _organizationRoleUserRepository.Get(x => x.RoleId == userEditModel.RoleId
                                            && x.UserId == person.Id && x.OrganizationId == userEditModel.OrganizationId);

            var userLogin = _userLoginRepository.Table.Where(x => x.Id == person.Id).FirstOrDefault();
            if (userLogin != null)
            {
                userLogin.IsActive = false;
                _userLoginRepository.Delete(userLogin);

            }
            if (organizationRoleUserDomain != null && organizationRoleUserDomain.Organization != null)
                organizationRoleUserDomain.IsActive = true;

            var organizationRoleUser = _organizationFactory.CreateDomain(userEditModel, person, organizationRoleUserDomain);
            organizationRoleUser.UserId = person.Id;

            if (organizationRoleUser.Id <= 0 && (userEditModel.RoleId == (long)RoleType.Technician || userEditModel.RoleId == (long)RoleType.SalesRep))
            {
                //var colorCode = GenerateColorCode(organizationRoleUser.OrganizationId);
                organizationRoleUser.ColorCode = userEditModel.PersonEditModel.Color;
            }
            _organizationRoleUserRepository.Save(organizationRoleUser);
            return organizationRoleUser;
        }
        private bool IsEquipmentPresent(long userId)
        {
            return _organizationRoleUserRepository.Table.Any(x => x.UserId == userId && x.RoleId == (int)RoleType.Equipment && x.IsActive);
        }
        private bool EditPersonUserId(long olduserId, long newUserId)
        {
            var organizationRoleUser = _organizationRoleUserRepository.Get(newUserId);
            if (organizationRoleUser != null)
            {
                organizationRoleUser.UserId = olduserId;
                _organizationRoleUserRepository.Save(organizationRoleUser);
                return true;
            }
            return false;
        }
        private bool DeletePerson(long olduserId)
        {
            var person = _personRepository.Get(olduserId);
            _personRepository.Delete(person);
            return true;
        }
        private bool UpdatingScheduler(long oldUserId, long newUserId)
        {
            var jobSchedulers = _jobSchedulerRepository.Table.Where(x => x.PersonId == oldUserId && x.IsActive).ToList();
            foreach (var jobScheduler in jobSchedulers)
            {
                jobScheduler.AssigneeId = newUserId;
                _jobSchedulerRepository.Save(jobScheduler);
            }
            return true;
        }
        private bool UpdatingSchedulerForEquipment(long oldUserId, long newUserId)
        {
            var jobSchedulers = _jobSchedulerRepository.Table.Where(x => x.AssigneeId == oldUserId && x.IsActive).ToList();
            foreach (var jobScheduler in jobSchedulers)
            {
                jobScheduler.AssigneeId = newUserId;
                _jobSchedulerRepository.Save(jobScheduler);
            }
            return true;
        }
        public bool UpdatingEquipmentUserDetails(Person person)
        {
            var equipmentUserDetails = _equipmentUserDetailsRepository.Table.FirstOrDefault(x => x.UserId == person.Id && x.IsActive);
            if (equipmentUserDetails == null)
            {
                var equipmentDomain = new EquipmentUserDetails
                {
                    UserId = person.Id,
                    IsLock = false,
                    IsActive = true,
                    IsNew = true
                };
                _equipmentUserDetailsRepository.Save(equipmentDomain);
            }
            return true;
        }
        public bool EquipmentRoleLock(long? userId, bool isLock)
        {
            var equipmentRole = _equipmentUserDetailsRepository.Table.FirstOrDefault(x => x.UserId == userId);
            if (equipmentRole == null)
                return false;

            equipmentRole.IsLock = isLock;
            _equipmentUserDetailsRepository.Save(equipmentRole);
            return true;
        }
        public string GetUserName(long? userId)
        {
            return _organizationRoleUserRepository.Table.FirstOrDefault(x => x.Id == userId && x.IsActive).Person.Name.FullName;
        }
        public bool SchedulerDefaultView(long? orgId, string defaultView)
        {
            var orgUser = _organizationRoleUserRepository.Get(orgId.GetValueOrDefault());
            var person = _personRepository.Table.FirstOrDefault(x => x.Id == orgUser.UserId);
            person.CalendarPreference = defaultView;
            _personRepository.Save(person);
            return true;
        }
        public string GetDefaultView(long? orgId)
        {
            var orgUser = _organizationRoleUserRepository.Get(orgId.GetValueOrDefault());
            var person = _personRepository.Table.FirstOrDefault(x => x.Id == orgUser.UserId);
            return person.CalendarPreference;
        }
        public UserSignatureListEditModel GetUserSignature(long? orgId, long? userId)
        {
            var emailSignatures = _emailSignaturesRepository.Table.Where(x => x.OrganizationRoleUserId == orgId && x.UserId == userId && x.IsActive).ToList();
            UserSignatureListEditModel list = new UserSignatureListEditModel();
            list.UserSignatureEditModel = emailSignatures.Select(_userFactory.CreateSignatureViewModel).ToList();
            return list;
        }
        public bool SaveUserSignature(UserSignatureListSaveModel model, long? orgId, long? userId)
        {
            var emailSignatures = _emailSignaturesRepository.Table.Where(x => x.OrganizationRoleUserId == orgId && x.UserId == userId && x.IsActive).ToList();
            foreach(var signature in emailSignatures)
            {
                signature.IsActive = false;
                _emailSignaturesRepository.Save(signature);
            }
            if (model != null)
            {
                if (model.UserSignatureSaveModel.Count > 0)
                {
                    foreach (var signature in model.UserSignatureSaveModel)
                    {
                        EmailSignatures signatures = _userFactory.CreateSignatureSaveModel(signature, userId, orgId);
                        signatures.IsNew = true;
                        _emailSignaturesRepository.Save(signatures);
                    }
                }
            }
            return true;
        }
    }
}
