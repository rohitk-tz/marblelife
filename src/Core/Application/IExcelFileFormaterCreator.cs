using Core.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application
{
   public interface IExcelFileFormaterCreator
    {
        bool CreateExcelDocument(List<AnnualGroupedReport> list, string xlsxFilePath);
    }
}
