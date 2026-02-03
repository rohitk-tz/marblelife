using Core.Geo.ViewModel;
using System.Collections.Generic;

namespace Core.Geo
{
    public interface ICountryService
    {
        IEnumerable<CountryViewModel> GetCountries();
        string GetCountryCurrencyByCountryId(long countryId);

    }
}
