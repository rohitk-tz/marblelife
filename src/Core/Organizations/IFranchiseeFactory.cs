using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using Core.Scheduler.Domain;
using Core.Users.Domain;
using System.Collections.Generic;

namespace Core.Organizations
{
    public interface IFranchiseeFactory
    {
        FranchiseeEditModel CreateEditModel(Franchisee domain, Person personDomain);

        Franchisee CreateDomain(FranchiseeEditModel model, Franchisee inDb);
        Organization CreateOrgDomain(FranchiseeEditModel model);

        FranchiseeViewModel CreateViewModel(Franchisee domain,List<FranchiseeDurationNotesHistry> durationList);
        FranchiseeViewModelForDownload CreateViewModelForDownload(Franchisee domain);
        FranchiseeViewModelForFranchiseeDirectoryDownload CreateViewModelForFranchiseeDirectoryDownload(Franchisee domain);
        FranchiseeTechMailService CreateFranchiseeTechMailDomain(FranchiseeEditModel model);
        FranchiseeRedesignViewModel CreateResignViewModel(Franchisee domain, List<long> orgIdList,long? roleId,List<FranchiseeDurationNotesHistry> durationList);
    }
}
