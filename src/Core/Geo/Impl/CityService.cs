using Core.Application;
using Core.Application.Attribute;
using Core.Geo.Domain;
using System.Collections.Generic;
using Core.Geo.ViewModel;
using System.Linq;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    public class CityService : ICityService
    {
        private readonly IRepository<City> _cityRepository;
        public CityService(IUnitOfWork unitOfWork)
        {
            _cityRepository = unitOfWork.Repository<City>();
        }
        public IEnumerable<string> GetCitieswithNameLike(string text)
        {
            var cityList = _cityRepository.Fetch(x => x.Name.Contains(text.Trim())).Select(x => x.Name).Distinct().ToList();
            return cityList;
        }

        public City GetbyCityNameandStateId(string cityName, long stateId)
        {
            return _cityRepository.Get(x => x.Name == cityName.Trim() && x.State.Id == stateId);
        }

        public City GetById(long cityId)
        {
            return _cityRepository.Get(cityId);
        }

        public IEnumerable<CityViewModel> GetCities()
        {
            return _cityRepository.Table.Select(CreateCityViewModel).ToList();
        }

        private CityViewModel CreateCityViewModel(City domain)
        {
            return new CityViewModel()
            {
                Id = domain.Id,
                Name = domain.Name
            };
        }

        public long GetCityIdByName(string cityName)
        {
            var city= _cityRepository.Table.FirstOrDefault(x => x.Name == cityName.Trim());
            if (city != null)
            {
                return city.Id;
            }
            else
            {
                return default;
            }
        }
    }
}
