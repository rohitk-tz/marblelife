using Core.Application.ViewModel;
using Core.Organizations.Domain;
using Core.Organizations.ViewModels;
using System.Collections.Generic;

namespace Core.Organizations
{
    public interface IFranchiseeServicesFactory
    {
        IEnumerable<FranchiseeService> CreateDomainCollection(IEnumerable<FranchiseeServiceEditModel> model, Franchisee franchisee);
        IEnumerable<FranchiseeServiceEditModel> CreateEditModel(IEnumerable<FranchiseeService> services);
    }
}
