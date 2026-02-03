using Core.Geo.Domain;
using Core.Geo.ViewModel;
using System.Collections.Generic;

namespace Core.Geo
{
    public interface ICityService
    {
        IEnumerable<CityViewModel> GetCities();
        IEnumerable<string> GetCitieswithNameLike(string text);
        City GetbyCityNameandStateId(string cityName, long stateId);
        City GetById(long cityId);
        long GetCityIdByName(string cityName);
    }
}
