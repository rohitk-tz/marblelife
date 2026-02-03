using Core.Geo.Domain;
using Core.Geo.ViewModel;
using System.Collections.Generic;

namespace Core.Geo
{
    public interface IStateService
    {
        IEnumerable<StateViewModel> GetStates();
        long GetStateIdByShortName(string shortName);
        State GetbyStateNameandCountryId(string stateName, long countryId);  
        IEnumerable<string> GetStateswithNameLike(string text);
        long GetStateIdByName(string name);
    }
}
