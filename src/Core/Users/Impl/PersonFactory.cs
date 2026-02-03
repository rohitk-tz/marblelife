using Core.Application;
using Core.Application.Attribute;
using Core.Application.Domain;
using Core.Application.Extensions;
using Core.Geo;
using Core.Geo.Domain;
using Core.Users.Domain;
using Core.Users.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Core.Users.Impl
{
    [DefaultImplementation]
    public class PersonFactory : IPersonFactory
    {
        private readonly IAddressFactory _addressFactory;
        private readonly IPhoneFactory _phoneFactory;
        //private readonly IClock _clock;

        public PersonFactory(IAddressFactory addressFactory, IPhoneFactory phoneFactory, IClock clock)
        {
            _addressFactory = addressFactory;
            _phoneFactory = phoneFactory;
            //_clock = clock;
        }

        public PersonEditModel CreateModel(Person domain)
        {
            return new PersonEditModel
            {
                PersonId = domain.Id,
                Name = domain.Name,
                PhoneNumbers = domain.Phones.Select(x => _phoneFactory.CreateEditModel(x)).ToList(),
                Address = domain.Addresses.Select(x => _addressFactory.CreateEditModel(x)).ToList(),
                Email = domain.Email,
                DataRecorderMetaData = domain.DataRecorderMetaData,
                IsRecruitmentFeeApplicable = domain.IsRecruitmentFeeApplicable,
                FileName=domain.File!=null?(domain.File.RelativeLocation+"\\"+domain.File.Name).ToFullPath() : ""
            };
        }

        public Person CreateDomain(PersonEditModel model)
        {
            return new Person
            {
                Id = model.PersonId,
                IsNew = model.PersonId <= 0,
                Name = model.Name,
                FirstName = model.Name.FirstName,
                LastName = model.Name.LastName,
                MiddleName = model.Name.MiddleName,
                Email = model.Email,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData.Id,
                IsRecruitmentFeeApplicable = model.IsRecruitmentFeeApplicable,
                Addresses = model.Address != null ? new List<Address>(model.Address.Select(x => _addressFactory.CreateDomain(x))) : null,
                Phones = model.PhoneNumbers != null ? new List<Phone>(model.PhoneNumbers.Select(x => _phoneFactory.CreateDomain(x))) : null,
                FileId = model.FileId
            };
        }

        public Person CreateDomain(OrganizationOwnerEditModel organizationOwner, UserLogin userLogin, string email)
        {
            return new Person
            {
                Id = userLogin.Id,
                FirstName = organizationOwner.OwnerFirstName,
                LastName = organizationOwner.OwnerLastName,
                Email = email,
                DataRecorderMetaDataId = organizationOwner.DataRecorderMetaData.Id,
                DataRecorderMetaData = organizationOwner.DataRecorderMetaData,
                IsNew = organizationOwner.OwnerId <= 0,
                IsRecruitmentFeeApplicable = organizationOwner.IsRecruitmentFeeApplicable,
                UserLogin = userLogin,
            };
        }

        public Person CreateDomain(PersonEquipmentEditModel model)
        {
            return new Person
            {
                Id = model.PersonId,
                IsNew = model.PersonId <= 0,
                Name = model.Name,
                FirstName = model.Name.FirstName,
                LastName = model.Name.LastName,
                MiddleName = model.Name.MiddleName,
                Email = model.Email,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaData.Id,
                IsRecruitmentFeeApplicable = model.IsRecruitmentFeeApplicable,
                Addresses = model.Address != null ? new List<Address>(model.Address.Select(x => _addressFactory.CreateDomain(x))) : null,
                FileId = model.FileId
            };
        }
    }
}
