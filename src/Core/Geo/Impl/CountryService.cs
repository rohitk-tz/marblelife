using System;
using System.Collections.Generic;
using Core.Geo.ViewModel;
using Core.Application;
using Core.Geo.Domain;
using System.Linq;
using Core.Application.Attribute;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    public class CountryService : ICountryService
    {
        private readonly IRepository<Country> _countryRepository;
        public CountryService(IUnitOfWork unitOfWork)
        {
            _countryRepository = unitOfWork.Repository<Country>(); ;
        }
        public IEnumerable<CountryViewModel> GetCountries()
        {
            return _countryRepository.Table.Select(CreateViewModel).ToList();
        }

        public string GetCountryCurrencyByCountryId(long countryId)
        {
            return _countryRepository.Get(countryId).CurrencyCode;
            
        }

        private CountryViewModel CreateViewModel(Country domain)
        {
            return new CountryViewModel()
            {
                Id = domain.Id,
                Name = domain.Name,
                ShortName = domain.ShortName,
                CurrencyCode=domain.CurrencyCode
            };
        }
    }
}
