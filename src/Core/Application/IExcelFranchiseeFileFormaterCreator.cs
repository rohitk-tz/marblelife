using Core.Organizations.ViewModel;
using System;
using System.Collections.Generic;

namespace Core.Application
{
   public interface IExcelFranchiseeFileFormaterCreator
    {
        bool CreateExcelDocument(List<FranchiseeModel> list, string xlsxFilePath);
    }
}
