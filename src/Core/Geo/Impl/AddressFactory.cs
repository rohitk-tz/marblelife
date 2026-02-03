using Core.Application;
using Core.Application.Attribute;
using Core.Application.Exceptions;
using Core.Billing.Domain;
using Core.Geo.Domain;
using Core.Geo.ViewModel;
using Core.Sales.Domain;
using Core.Scheduler.Domain;
using System.Linq;
using System.Text.RegularExpressions;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    public class AddressFactory : IAddressFactory
    {
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly IZipService _zipService;
        private readonly IRepository<Country> _countryRepository;


        public AddressFactory(IStateService stateService, ICityService cityService, IZipService zipService, IUnitOfWork unitOfWork)
        {
            _stateService = stateService;
            _cityService = cityService;
            _zipService = zipService;

            _countryRepository = unitOfWork.Repository<Country>();
        }

        public Address CreateDomain(AddressEditModel model)
        {
            var state = _stateService.GetbyStateNameandCountryId(model.State, model.CountryId);
            long stateId = state != null ? state.Id : (model.StateId > 0 ? model.StateId : 0);
            var city = _cityService.GetbyCityNameandStateId(model.City, stateId);
            var zip = _zipService.GetbyZipCode(model.ZipCode);
            //if (string.IsNullOrEmpty(model.AddressLine1))
            //{
            //    throw new InvalidAddressException("Address line 1 is mandatory.");
            //}
            //if (string.IsNullOrEmpty(model.City))
            //{
            //    throw new InvalidAddressException("City is mandatory.");
            //}
            //if (model.CountryId < 1)
            //{
            //    throw new InvalidAddressException(Localization.Validations.Geo.CountryNotProvided);
            //}

            //if (model.StateId < 1)
            //{
            //    throw new InvalidAddressException(Localization.Validations.Geo.StateNotProvided);
            //}

            //if (city == null && model.City == null)
            //{
            //    throw new InvalidAddressException(string.Format(Localization.Validations.Geo.InvalidCity, model.City));
            //}

            //var validZipCode = _zipService.IsUSZipCode(model.ZipCode);
            //if (validZipCode == false)
            //{
            //    throw new InvalidAddressException(string.Format(Localization.Validations.Geo.InvalidZip, model.ZipCode));
            //}

            var country = _countryRepository.Get(model.CountryId);
            var address = new Address(model.AddressLine1, model.AddressLine2, state, city, zip, model.StateId, model.CountryId, model.State, model.City, model.ZipCode)
            {
                Id = model.Id,
                IsNew = model.Id < 1,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                StateId = model.StateId > 0 ? (long?)model.StateId : null,
                TypeId = model.AddressType,
                CountryId = country.Id,
                CityId = city != null ? city.Id : (long?)null,
                CityName = city != null ? city.Name : model.City,
                StateName = state != null ? state.Name : model.State,
                ZipCode = zip != null ? zip.Code : model.ZipCode,
            };

            address.State = state;
            address.City = city;
            address.Zip = zip;
            address.Country = country;

            return address;

        }

        public Address CreateDomainForCustomerAddress(AddressEditModel model)
        {
            var state = _stateService.GetbyStateNameandCountryId(model.State, model.CountryId);
            long stateId = state != null ? state.Id : (model.StateId > 0 ? model.StateId : 0);
            var city = _cityService.GetbyCityNameandStateId(model.City, stateId);
            var zip = _zipService.GetbyZipCode(model.ZipCode);

            var country = _countryRepository.Get(model.CountryId);
            var address = new Address(model.AddressLine1, model.AddressLine2, state, city, zip, model.StateId, model.CountryId, model.State, model.City, model.ZipCode)
            {
                Id = model.Id,
                IsNew = model.Id < 1,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                StateId = model.StateId > 0 ? (long?)model.StateId : null,
                TypeId = model.AddressType,
                CountryId = country.Id,
                CityId = city != null ? city.Id : (long?)null,
                CityName = city != null ? city.Name : model.City,
                StateName = state != null ? state.Name : model.State,
                ZipCode = zip != null ? zip.Code : model.ZipCode,
                ZipId = model.ZipId
            };

            address.State = state;
            address.City = city;
            address.Zip = zip == null && model.ZipId != 0 ? _zipService.GetbyZipId(model.ZipId.GetValueOrDefault()) : null;
            address.Country = country;

            return address;

        }

        public AddressEditModel CreateEditModel(Address domain)
        {
            if (domain == null) return null;

            return new AddressEditModel(domain.TypeId)
            {
                Id = domain.Id,
                AddressLine1 = string.IsNullOrEmpty(domain.AddressLine1) ? "" : domain.AddressLine1,
                AddressLine2 = domain.AddressLine2,
                City = domain.City != null ? domain.City.Name : domain.CityName,
                State = domain.State != null ? domain.State.Name : domain.StateName,
                // StateId = domain.State.Id,
                // State = domain.State.ShortName,
                CountryId = domain.Country.Id,
                Country = domain.Country.Name,
                ZipCode = domain.Zip != null ? domain.Zip.Code : domain.ZipCode,
                AddressType = domain.TypeId
            };
        }

        public AddressViewModel CreateViewModel(Address domain)
        {
            if (domain == null)
                return new AddressViewModel
                {
                    AddressLine1 = " ",
                    AddressLine2 = " ",
                    City = " ",
                    State = " ",
                    Country = " ",
                    ZipCode = " ",
                    CountryId = 0
                };

            return new AddressViewModel
            {
                AddressLine1 = domain.AddressLine1 ?? " ",
                AddressLine2 = domain.AddressLine2 ?? " ",
                City = (domain.City != null && domain.City.Name != null) ? domain.City.Name : domain.CityName ?? " ",
                State = (domain.State != null && domain.State.Name != null) ? domain.State.Name : domain.StateName ?? " ",
                Country = (domain.Country != null && domain.Country.Name != null) ? domain.Country.Name : " ",
                ZipCode = (domain.Zip != null && domain.Zip.Code != null) ? domain.Zip.Code : domain.ZipCode ?? " ",
                CountryId = domain.CountryId
            };
        }
        public AddressViewModel CreateViewModel(InvoiceAddress domain)
        {
            if (domain == null) return new AddressViewModel();

            return new AddressViewModel
            {
                AddressLine1 = domain.AddressLine1 != null ? domain.AddressLine1 : null,
                AddressLine2 = (domain.AddressLine2 != "") ? domain.AddressLine2 : null,
                City = domain.City != null ? domain.City.Name : domain.CityName,
                State = domain.State != null ? domain.State.Name : domain.StateName,
                Country = domain.Country.Name,
                ZipCode = domain.Zip != null ? domain.Zip.Code : domain.ZipCode,
                CountryId = domain.CountryId.GetValueOrDefault(),
            };
        }
        
        public AddressViewModel CreateViewModel(AuditFranchiseeSales franchiseeSales)
        {
            if (franchiseeSales == null) return new AddressViewModel();
            if (franchiseeSales != null)
            {
                if (franchiseeSales.Customer == null && franchiseeSales.AuditCustomer == null)
                {
                    return new AddressViewModel();
                }

            }
            return new AddressViewModel
            {
                AddressLine1 = franchiseeSales.Customer != null ? franchiseeSales.Customer.Address.AddressLine1 : franchiseeSales.AuditCustomer.AuditAddress != null ? franchiseeSales.AuditCustomer.AuditAddress.AddressLine1 : "",
                AddressLine2 = franchiseeSales.Customer != null ? franchiseeSales.Customer.Address.AddressLine2 : franchiseeSales.AuditCustomer.AuditAddress != null ? franchiseeSales.AuditCustomer.AuditAddress.AddressLine2 : "",
                City = franchiseeSales.Customer != null ? franchiseeSales.Customer.Address.City != null ? franchiseeSales.Customer.Address.City.Name : franchiseeSales.Customer.Address.CityName
                                       : franchiseeSales.AuditCustomer.AuditAddress != null ? franchiseeSales.AuditCustomer.AuditAddress.City != null? franchiseeSales.AuditCustomer.AuditAddress.City.Name: franchiseeSales.AuditCustomer.AuditAddress.CityName : "",
                State = franchiseeSales.Customer != null ? franchiseeSales.Customer.Address.State != null ? franchiseeSales.Customer.Address.State.Name : franchiseeSales.Customer.Address.StateName
                                       : franchiseeSales.AuditCustomer.AuditAddress != null ? franchiseeSales.AuditCustomer.AuditAddress.State != null ? franchiseeSales.AuditCustomer.AuditAddress.State.Name : franchiseeSales.AuditCustomer.AuditAddress.StateName : "",
                Country = franchiseeSales.Customer != null ? franchiseeSales.Customer.Address.Country != null ? franchiseeSales.Customer.Address.Country.Name : ""
                                       : franchiseeSales.AuditCustomer.AuditAddress != null ? franchiseeSales.AuditCustomer.AuditAddress.Country != null ? franchiseeSales.AuditCustomer.AuditAddress.Country.Name : "" : "",
                ZipCode = franchiseeSales.Customer != null ? franchiseeSales.Customer.Address.Zip != null ? franchiseeSales.Customer.Address.Zip.Code : franchiseeSales.Customer.Address.ZipCode
                                       : franchiseeSales.AuditCustomer.AuditAddress != null ? franchiseeSales.AuditCustomer.AuditAddress.Zip != null ? franchiseeSales.AuditCustomer.AuditAddress.Zip.Code : franchiseeSales.AuditCustomer.AuditAddress.ZipCode : "",
                CountryId = franchiseeSales.Customer != null ? franchiseeSales.Customer.Address.Country != null ? franchiseeSales.Customer.Address.Country.Id : default(long): franchiseeSales.AuditCustomer.AuditAddress.CountryId
            };
        }
        public AddressViewModel CreateViewModelForFranchisee(Address domain)
        {
            if (domain == null) return new AddressViewModel();

            return new AddressViewModel
            {
                AddressLine1 = domain.AddressLine1 != null ? domain.AddressLine1 : null,
                AddressLine2 = (domain.AddressLine2 != "") ? domain.AddressLine2 : null,
                City = domain.City != null ? domain.City.Name : domain.CityName,
                State = domain.State != null ? domain.State.Name : domain.StateName,
                Country = domain.Country.Name,
                ZipCode = domain.Zip != null ? domain.Zip.Code : domain.ZipCode,
                CountryId = domain.CountryId,
            };
        }

    }
}
