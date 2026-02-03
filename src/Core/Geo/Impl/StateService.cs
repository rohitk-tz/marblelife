using Core.Application.Attribute;
using System.Collections.Generic;
using Core.Geo.Domain;
using Core.Application;
using Core.Geo.ViewModel;
using System.Linq;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    class StateService : IStateService
    {
        private readonly IRepository<State> _stateRepository;

        public StateService(IUnitOfWork unitOfWork)
        {
            _stateRepository = unitOfWork.Repository<State>();
        }

        public IEnumerable<StateViewModel> GetStates()
        {
            return _stateRepository.Table.Select(CreateViewModel).ToList();
        }
        public long GetStateIdByShortName(string shortName)
        {
            return _stateRepository.Fetch(x => x.ShortName.Equals(shortName)).Select(x => x.Id).SingleOrDefault();
        }
        public long GetStateIdByName(string name)
        {
            return _stateRepository.Fetch(x => x.Name.Equals(name)).Select(x => x.Id).SingleOrDefault();
        }
        public State GetbyStateNameandCountryId(string stateName, long countryId)
        {
            return _stateRepository.Get(x => x.Name == stateName.Trim() && x.Country.Id == countryId);
        }
        private StateViewModel CreateViewModel(State domain)
        {
            return new StateViewModel()
            {
                Id = domain.Id,
                Name = domain.Name,
                ShortName = domain.ShortName
            };
        }
        public IEnumerable<string> GetStateswithNameLike(string text)
        {
            var stateList = _stateRepository.Fetch(x => x.Name.Contains(text.Trim())).Select(x => x.Name).Distinct().ToList();
            return stateList;
        }
    }
}
