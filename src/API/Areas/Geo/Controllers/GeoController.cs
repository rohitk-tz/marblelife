using Api.Areas.Application.Controller;
using Core.Geo;
using Core.Geo.ViewModel;
using System.Collections.Generic;
using System.Web.Http;

namespace API.Areas.Geo.Controllers
{
    [AllowAnonymous]
    public class GeoController : BaseController
    {
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;
        private readonly ICountryService _countryService;

        public GeoController(ICountryService countryService, IStateService stateService, ICityService cityService)
        {
            _stateService = stateService;
            _cityService = cityService;
            _countryService = countryService;
        }

        public IEnumerable<StateViewModel> GetAllStates()
        {
            return _stateService.GetStates();
        }

        public IEnumerable<CityViewModel> GetAllCities()
        {
            return _cityService.GetCities();
        }

        public IEnumerable<string> GetAllCitiesByName([FromUri]string name)
        {
            return _cityService.GetCitieswithNameLike(name);
        }
        [HttpGet]
        public IEnumerable<CountryViewModel> GetAllCountries()
        {
            return _countryService.GetCountries();
        }
        public string GetCountryCurrencyByCountryId(long countryId=1)
        {
            return _countryService.GetCountryCurrencyByCountryId(countryId);
        }
        public IEnumerable<string> GetAllStatesByName([FromUri]string name)
        {
            return _stateService.GetStateswithNameLike(name);
        }
    }
}
