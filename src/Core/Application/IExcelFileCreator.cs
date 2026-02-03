using System.Collections.Generic;
using System.Data;

namespace Core.Application
{
    public interface IExcelFileCreator
    {
        bool CreateExcelDocument<T>(List<T> list, string xlsxFilePath);
        DataTable ListToDataTable<T>(List<T> list, string tableName = "");
        bool CreateExcelDocument(DataSet ds, string excelFilename);

        void ToCSV(DataTable dtDataTable, string strFilePath);

        void ToCSVWriter<T>(List<T> dtDataTable, string strFilePath);
    }
}
