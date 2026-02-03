using Core.Application;
using Core.Application.Attribute;
using Core.Geo;
using Core.Organizations.Domain;
using Core.Users.Domain;
using Core.Users.Enum;
using Core.Users.ViewModels;
using System.Linq;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class UserFactory : IUserFactory
    {
        private readonly IAddressFactory _addressFactory;
        private readonly IPhoneFactory _phoneFactory;
        private readonly IClock _clock;
        private readonly IRepository<Person> _personRepository;
        private readonly IRepository<EquipmentUserDetails> _equipmentUserDetailsRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserFactory(IAddressFactory addressFactory, IPhoneFactory phoneFactory, IClock clock, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _addressFactory = addressFactory;
            _phoneFactory = phoneFactory;
            _clock = clock;
            _personRepository = unitOfWork.Repository<Person>();
            _equipmentUserDetailsRepository = unitOfWork.Repository<EquipmentUserDetails>();
        }

        public UserViewModel CreateViewModel(OrganizationRoleUser organizationRoleUser)
        {
            var userLogin = new UserLogin();
            var person = organizationRoleUser.Person;
            if (person == null)
            {
                var personDomain = _personRepository.Table.FirstOrDefault(x => x.Id == organizationRoleUser.UserId);
                person = personDomain;
            }
            if (person != null)
                userLogin = person.UserLogin;

            var userViewModel = new UserViewModel
            {
                Role = organizationRoleUser.Role.Name, //((RoleType)(organizationRoleUser.RoleId)).ToString(),
                OrganizationRoleUserId = organizationRoleUser.Id,
                UserId = organizationRoleUser.UserId,
                Email = person.Email,
                Name = person.Name,
                CreatedOn = person.DataRecorderMetaData.DateCreated,
                Address = _addressFactory.CreateViewModel(person.Addresses.FirstOrDefault()),
                PhoneNumbers = person.Phones.Select(x => new { x.Number, x.Lookup.Name }).ToList(),
                FranchiseeName = organizationRoleUser.Organization.Name,
                RoleId = organizationRoleUser.RoleId,
                IsActive = organizationRoleUser.IsActive,
                FranchiseeId = organizationRoleUser.OrganizationId
            };

            if (userLogin != null)
            {
                userViewModel.UserName = userLogin.UserName;
                userViewModel.LastLoggedIn = userLogin.LastLoggedInDate;
                userViewModel.IsLocked = userLogin.IsLocked;
            }
            var eq = _equipmentUserDetailsRepository.Table.FirstOrDefault(x => x.UserId == organizationRoleUser.UserId);
            if (eq != null)
            {
                userViewModel.IsLocked = eq.IsLock;
            }

            return userViewModel;
        }

        public SalesRep CreateDomain(OrganizationRoleUser organizationRoleUser, UserEditModel userEditModel)
        {
            if (string.IsNullOrEmpty(userEditModel.Alias))
            {
                char firstNameInitial = userEditModel.PersonEditModel.Name.FirstName[0];
                char lastNameInitial = userEditModel.PersonEditModel.Name.LastName[0];
                userEditModel.Alias = firstNameInitial + "" + lastNameInitial;
            }

            return new SalesRep
            {
                Id = organizationRoleUser.Id,
                Alias = userEditModel.Alias,
                IsNew = organizationRoleUser.IsNew == true ? true : false
            };
        }

        public UserSignatureEditModel CreateSignatureViewModel(EmailSignatures signature)
        {
            var userViewModel = new UserSignatureEditModel
            {
                UserId = signature.UserId,
                OrganizationRoleUserId = signature.OrganizationRoleUserId,
                SignatureName = signature.SignatureName,
                Signature = signature.Signature,
                IsDefault = signature.IsDefault,
                IsActive = signature.IsActive
            };
            return userViewModel;
        }

        public EmailSignatures CreateSignatureSaveModel(UserSignatureSaveModel signature, long? userId, long? orgId)
        {
            var sigantureModel = new EmailSignatures
            {
                UserId = userId,
                OrganizationRoleUserId = orgId,
                SignatureName = signature.SignatureName,
                Signature = signature.Signature,
                IsDefault = signature.IsDefault,
                IsActive = true
            };
            return sigantureModel;
        }
    }
}
