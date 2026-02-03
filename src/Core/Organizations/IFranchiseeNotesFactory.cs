using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;

namespace Core.Organizations
{
    public interface IFranchiseeNotesFactory
    {
        FranchiseeNotes CreateDomain(long franchiseeId, FranchiseeEditModel model);
        FranchiseeNotesViewModel CreateViewModel(FranchiseeNotes notes);
    }
}
