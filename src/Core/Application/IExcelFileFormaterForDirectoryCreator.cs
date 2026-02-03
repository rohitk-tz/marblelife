using Core.Organizations.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application
{
    public interface IExcelFileFormaterForDirectoryCreator
    {
        bool CreateExcelDocument(List<FranchiseeModel> list, string xlsxFilePath);
    }
}
