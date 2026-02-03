using Core.Application.Attribute;
using Core.Organizations.ViewModel;
using Core.Geo;
using System.Linq;
using Core.Organizations.Domain;
using Core.Users.ViewModels;
using Core.Users.Domain;
using System.Collections.Generic;
using Core.Geo.Domain;
using Core.Users.Enum;

namespace Core.Organizations.Impl
{
    [DefaultImplementation]
    public class OrganizationFactory : IOrganizationFactory
    {
        private readonly IAddressFactory _addressFactory;
        private readonly IPhoneFactory _phoneFactory;

        public OrganizationFactory(IAddressFactory addressFactory, IPhoneFactory phoneFactory)
        {
            _addressFactory = addressFactory;
            _phoneFactory = phoneFactory;
        }
        public Organization CreateDomain(OrganizationEditModel model)
        {
            return new Organization
            {
                Id = model.Id,
                IsNew = model.Id <= 0,
                DataRecorderMetaData = model.DataRecorderMetaData,
                DataRecorderMetaDataId = model.DataRecorderMetaDataId,
                About = model.About,
                Name = model.Name,
                Email = model.Email,
                TypeId = model.TypeId,
                IsActive = model.IsActive,
                Address = model.Address != null ? new List<Address>(model.Address.Select(x => _addressFactory.CreateDomain(x))) : null,
                Phones = model.PhoneNumbers != null ? new List<Phone>(model.PhoneNumbers.Select(x => _phoneFactory.CreateDomain(x))) : null,
                DeactivationnNote=model.DeactivationNote
            };
        }

        public OrganizationEditModel CreateEditModel(Organization domain)
        {
            return new OrganizationEditModel
            {
                Id = domain.Id,
                About = domain.About,
                Address = domain.Address.Select(x => _addressFactory.CreateEditModel(x)).ToList(),
                DataRecorderMetaData = domain.DataRecorderMetaData,
                DataRecorderMetaDataId = domain.DataRecorderMetaDataId,
                Email = domain.Email,
                Name = domain.Name,
                TypeId = domain.TypeId,
                IsActive = domain.IsActive,
                PhoneNumbers = domain.Phones.Select(x => _phoneFactory.CreateEditModel(x)).ToList()
            };
        }

        public OrganizationRoleUser CreateDomain(UserEditModel userEditModel, Person person, OrganizationRoleUser domain)
        {
            var organizationRoleUser = new OrganizationRoleUser
            {
                Id = domain == null ? 0 : domain.Id,
                OrganizationId = userEditModel.OrganizationId,
                RoleId = userEditModel.RoleId,
                UserId = person.Id,
                IsActive = domain != null ? domain.IsActive : true,
                IsNew = !(domain != null && domain.Id > 0),
                IsDefault = domain != null ? domain.IsDefault : true,
                //ColorCode = domain != null ? domain.ColorCode : null,
                ColorCode = domain != null ? userEditModel.PersonEditModel.Color : null,
                ColorCodeSale=domain!=null ? userEditModel.PersonEditModel.ColorCodeSale : null,
            };
            return organizationRoleUser;
        }

        public OrganizationViewModel CreateViewModel(Organization domain)
        {
            return new OrganizationViewModel
            {
                Id = domain.Id,
                About = domain.About,
                Address = domain.Address.Select(x => _addressFactory.CreateEditModel(x)).FirstOrDefault(),
                Email = domain.Email,
                Name = domain.Name,
                TypeId = domain.TypeId,
                PhoneNumbers = domain.Phones.Select(x => new { x.Number, x.Lookup.Name,x.IsTransferable }).ToList()
            };
        }

        public OrganizationRoleUser CreateDomain(Person person, Organization organization, long ownerId)
        {
            return new OrganizationRoleUser
            {
                IsNew = ownerId <= 0,
                UserId = person.Id,
                RoleId = (long)RoleType.FranchiseeAdmin,
                OrganizationId = organization.Id,
                IsActive = true,
                IsDefault = true,
            };
        }

        public OrganizationRoleUser CreateDomain(OrganizationRoleUser orgRoleUser, Franchisee franchisee)
        {
            return new OrganizationRoleUser
            {
                UserId = orgRoleUser.UserId,
                RoleId = orgRoleUser.RoleId,
                OrganizationId = franchisee.Organization.Id,
                IsDefault = false,
                IsActive = true,
                IsNew = true
            };
        }

        public FranchiseeInfoViewModel CreateViewModel(OrganizationRoleUser orgRoleUser)
        {
            var isToBeBold = false;
            var franchiseeToBeBold = new List<string>() { "MI-Detroit", "PA-Philadelphia", "MI-Grand Rapids", "OH-Cleveland" };

            if (franchiseeToBeBold.Contains(orgRoleUser.Organization.Name))
            {
                isToBeBold = true;
            }
            var address = orgRoleUser.Organization.Address.FirstOrDefault();
            return new FranchiseeInfoViewModel
            {
                FranchiseeName = orgRoleUser.Organization.Name,
                IsActive = orgRoleUser.IsActive,
                IsDefault = orgRoleUser.IsDefault,
                OrganizationRoleUserId = orgRoleUser.Id,
                OrganizationId = orgRoleUser.OrganizationId,
                UserId = orgRoleUser.UserId,
                 CountryName= address!=null? address.Country.Name:"",
                 StateName=address!=null? address.State!=null ?address.State.Name:address.StateName:"",
                 IsBold= isToBeBold
            };
        }

        public FranchiseeInfoViewModel CreateViewModel(OrganizationRoleUserFranchisee orgRoleUser)
        {
            var isToBeBold = false;
            var franchiseeToBeBold = new List<string>() { "MI-Detroit", "PA-Philadelphia", "MI-Grand Rapids", "OH-Cleveland" };

            if (franchiseeToBeBold.Contains(orgRoleUser.Franchisee.Organization.Name))
            {
                isToBeBold = true;
            }
            var address = orgRoleUser.OrganizationRoleUser.Organization.Address.FirstOrDefault();
            return new FranchiseeInfoViewModel
            {
                FranchiseeName = orgRoleUser.Franchisee.Organization.Name,
                IsActive = orgRoleUser.IsActive,
                IsDefault = orgRoleUser.IsDefault,
                OrganizationRoleUserId = orgRoleUser.Id,
                OrganizationId = orgRoleUser.FranchiseeId,
                UserId = orgRoleUser.OrganizationRoleUser.UserId,
                CountryName = address != null ? address.Country != null ? address.Country.Name : "" : "",
                StateName = address != null? address.State!=null?address.State.Name:address.StateName:"",
                 IsBold=isToBeBold
            };
        }

        public OrganizationRoleUserFranchisee CreateDomain(long orgId, OrganizationRoleUser orgRoleUser, OrganizationRoleUserFranchisee inDb)
        {
            var domain = new OrganizationRoleUserFranchisee
            {
                Id = inDb != null ? inDb.Id : 0,
                FranchiseeId = orgId,
                IsNew = (inDb != null && inDb.Id > 0) ? false : true,
                IsActive = true,
                IsDefault = inDb != null ? inDb.IsDefault : true,
                OrganizationRoleUserId = orgRoleUser.Id,
            };
            return domain;
        }

        public OrganizationRoleUser CreateDomain(UserEquipmentEditModel userEditModel, Person person, OrganizationRoleUser domain)
        {
            var organizationRoleUser = new OrganizationRoleUser
            {
                Id = domain == null ? 0 : domain.Id,
                OrganizationId = userEditModel.OrganizationId,
                RoleId = userEditModel.RoleId,
                UserId = person.Id,
                IsActive = domain != null ? domain.IsActive : true,
                IsNew = !(domain != null && domain.Id > 0),
                IsDefault = domain != null ? domain.IsDefault : true,
                //ColorCode = domain != null ? domain.ColorCode : null,
                ColorCode = domain != null ? userEditModel.PersonEditModel.Color : null,
            };
            return organizationRoleUser;
        }
    }
}
