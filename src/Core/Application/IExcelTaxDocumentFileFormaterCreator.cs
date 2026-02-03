using Core.Organizations.ViewModel;
using System.Collections.Generic;

namespace Core.Application
{
    public interface IExcelTaxDocumentFileFormaterCreator
    {
        bool CreateExcelDocument(List<FranchiseeDocumentViewModel> list, string xlsxFilePath, List<string> columnList);
    }
}
