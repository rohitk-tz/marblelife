using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace Core.Application.Impl
{
    public static class PriceEstimateExcelFileParser
    {
        public static DataTable ReadExcelZip(string path, string sheetId)
        {
            DataTable dt = new DataTable();
            Dictionary<string, int> headerDictionary = new Dictionary<string, int>();

            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path, false))
            {
                int startingIndex = -1;
                int startingRow;
                Worksheet selectedWorkSheet = null;
                string relationshipId = sheetId;
                WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                
                if (workSheet == null)
                {
                    throw new Exception("File doesnot contain the sheet with required headers");
                }
                if (CheckSheetHasRequiredColumns(workSheet, spreadsheetDocument, out startingIndex, out startingRow))
                {
                    selectedWorkSheet = workSheet;
                }
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                int colIndex = 0;
                foreach (Cell cell in rows.ElementAt(0))
                {
                    var cellValue = GetCellValue(spreadsheetDocument, cell);
                    if (!string.IsNullOrWhiteSpace(cellValue)) cellValue = cellValue.ToLower().Trim();
                    headerDictionary.Add(CellReference(cell), colIndex++);
                    dt.Columns.Add(cellValue);
                }

                int index = 0;
                foreach (Row row in rows) //this will also include  header row...
                {
                    if (index <= 0)
                    {
                        index++;
                        continue;
                    }

                    DataRow tempRow = dt.NewRow();
                    var cellCollection = row.Descendants<Cell>();

                    var validDataCount = 0;

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var cell = cellCollection.ElementAtOrDefault(i);
                        if (cell == null) continue;

                        var cellValue = GetCellValue(spreadsheetDocument, cell);
                        if (!string.IsNullOrWhiteSpace(cellValue)) validDataCount++;

                        if (headerDictionary.ContainsKey(CellReference(cell)))
                        {
                            var cellIndex = headerDictionary[CellReference(cell)];
                            tempRow[cellIndex] = cellValue;
                        }
                    }

                    if (validDataCount > 0)
                        dt.Rows.Add(tempRow);
                }
            }
            return dt;
        }
        private static bool CheckSheetHasRequiredColumns(Worksheet workSheet, SpreadsheetDocument document, out int startingIndex, out int startingRow)
        {
            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();

            startingRow = -1;
            startingIndex = -1;

            int rowIndex = 0;
            foreach (Row row in rows)
            {
                var headers = new List<string>();
                int index = 0;
                startingIndex = -1;
                foreach (Cell cell in row)
                {
                    var x = GetCellValue(document, cell);
                    if (!string.IsNullOrWhiteSpace(x))
                    {
                        x = x.ToLower();
                        if (x != "")
                        {
                            headers.Add(x);
                            if (x.ToLower() == "id")
                            {
                                startingIndex = index;
                            }
                        }
                    }

                    index++;
                }

                if (headers.Count() > 0)
                {
                    if (headers.FirstOrDefault(x => x.ToLower() == "id") != null && headers.FirstOrDefault(x => x.ToLower() == "isupdated") != null)
                    {
                        startingRow = rowIndex;
                        return true;
                    }
                }

                rowIndex++;
            }

            return false;
        }
        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            if (cell == null) return null;
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value = (cell.CellValue != null) ? cell.CellValue.InnerXml : "";

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;
            }

            return value;
        }
        private static string CellReference(Cell cell)
        {
            var val = cell.CellReference.Value;
            var text = "";

            for (int i = 0; i < val.Length; i++)
            {
                var c = val[i];
                var asciiVal = (int)c;
                if ((asciiVal >= 65 && asciiVal < 91) || (asciiVal >= 97 && asciiVal <= 122))
                {
                    text += c.ToString();
                }
            }

            return text;
        }
        public static List<string> GetSheetIds(string path)
        {
            int a = 0;
            List<string> sheetIds = new List<string>();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path, false))
            {
                IEnumerable<Sheet> sheets = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                foreach (var sheet in sheets)
                {
                    sheetIds.Add(sheet.Id);
                }
            }
            return sheetIds;
        }
    }
}
