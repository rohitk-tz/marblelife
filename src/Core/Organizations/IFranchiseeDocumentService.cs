using Core.Organizations.ViewModel;

namespace Core.Organizations
{
    public interface IFranchiseeDocumentService
    {
        bool SaveDocuments(FranchiseeDocumentEditModel model);
        DocumentListModel GetFranchiseeDocument(DocumentListFilter filter, int pageNumber, int pageSize);
        bool Delete(long id);
        DocumentViewModel GetFranchiseeInfoById(long? docId);
        string IsExpiryValid(FranchiseeDocumentEditModel model);
    }
}
