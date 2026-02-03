using Core.MarketingLead.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application
{
   public interface IExcelFileCreatorMarketingLead
    {
        bool CreateExcelDocument(List<CallDetailViewModel> list, string xlsxFilePath, List<string> columnsName);
        DataTable ListToDataTable(List<string> columnList, List<CallDetailViewModel> list,  string tableName = "");
        bool CreateExcelDocument(DataSet ds, string excelFilename);
    }
}
