using Core.Organizations.Domain;
using Core.Organizations.ViewModel;
using Core.Users.ViewModels;
using System.Collections.Generic;

namespace Core.Organizations
{
    public interface IFranchiseeDocumentFactory
    {
        FranchiseDocument CreateDomain(FranchiseeDocumentEditModel model, long? fileId);
        DocumentViewModel CreateViewModel(FranchiseDocument document);
        IEnumerable<DocumentTypeEditModel> CreateEditModelForDocument(IEnumerable<FranchiseeDocumentType> documentList);
        List<FranchiseeDocumentType> CreateDocumentDomain(IEnumerable<DocumentTypeEditModel> model, long franchiseeId);
        DocumentViewModel CreateDomain(FranchiseDocument model);
        FrabchiseeDocumentEditModel CreateDomainForUser(FranchiseDocument model);
    }
}
