using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Core.Application.Attribute;
using Core.Application;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using Core.Notification.ViewModel;
using Core.Scheduler.ViewModel;
using Core.Sales.ViewModel;
using Core.Organizations.ViewModel;
using Core.Billing.ViewModel;
using Core.MarketingLead.ViewModel;
using System.IO;
using CsvHelper;
using System.Text;
using CsvHelper.TypeConversion;
using CsvHelper.Configuration;

namespace SixBar.Core.Application.Impl
{
    [DefaultImplementation]
    public class ExcelFileCreator : IExcelFileCreator
    {
        public bool CreateExcelDocument<T>(List<T> list, string xlsxFilePath)
        {
            var ds = new DataSet();
            ds.Tables.Add(ListToDataTable(list));

            return CreateExcelDocument(ds, xlsxFilePath);
        }

        #region HELPER_FUNCTIONS

        public DataTable ListToDataTable<T>(List<T> list, string tableName = "")
        {
            var dtCol = new DataTable();
            PropertyInfo[] propertyInfoCollection = typeof(T).GetProperties();

            DataTable dt = SetColumns<T>(dtCol, propertyInfoCollection, true, false);

            AddRows(list, dt);

            if (!string.IsNullOrEmpty(tableName))
                dt.TableName = tableName;

            return dt;
        }

        private static void AddRows<T>(List<T> list, DataTable dt)
        {
            foreach (T t in list)
            {
                DataRow row = dt.NewRow();
                SetValueFromInstanceInDataRow(t, typeof(T), row);
                dt.Rows.Add(row);
            }
        }
        private static void SetValueFromInstanceInDataRow<T>(T obj, Type typeOfObject, DataRow row)
        {
            PropertyInfo[] propertyCollection = typeOfObject.GetProperties();
            foreach (PropertyInfo info in propertyCollection)
            {
                Type propertyType = info.PropertyType;
                var attr = info.GetCustomAttributes(typeof(DownloadFieldAttribute), true).SingleOrDefault();

                var fieldName = info.Name;
                if (typeof(T) == typeof(SalesFunnelLocalExcelViewModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }
                else if (typeof(T) == typeof(SalesFunnelNationalExcelViewModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }
                else if (typeof(T) == typeof(FranchiseeLoanViewModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }
                else if (typeof(T) == typeof(Auditaddress))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }
                else if (typeof(T) == typeof(DownloadZipCodeModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        if (displayAttr.DisplayName != "DIRVE DIST")
                        {
                            fieldName = displayAttr.DisplayName;
                        }
                        else
                        {
                            break;

                        }
                    }
                }
                else if (typeof(T) == typeof(DownloadCountyModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }

                else if (typeof(T) == typeof(WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }
                else if (typeof(T) == typeof(FranchiseeViewModelForDownload))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }
                else if (typeof(T) == typeof(FranchiseeViewModelForDownload))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }

                else if (typeof(T) == typeof(CallDetailViewModelV2))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }

                else if (typeof(T) == typeof(DownloadAllInvoiceModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }
                else if (typeof(T) == typeof(CallDetailViewModel))
                {
                    var displayattr = info.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    DisplayNameAttribute displayAttr = displayattr as DisplayNameAttribute;

                    if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.DisplayName))
                    {
                        fieldName = displayAttr.DisplayName;
                    }
                }

                if (attr != null && (attr is DownloadFieldAttribute))
                {
                    DownloadFieldAttribute authAttr = attr as DownloadFieldAttribute;
                    if (!authAttr.Required) continue;

                    if (authAttr.IsComplexType == true && !authAttr.IsCollection)
                    {
                        //It will go into a recursive loop
                        var value = info.GetValue(obj);
                        SetValueFromInstanceInDataRow(value, propertyType, row);
                        continue;
                    }
                    if (authAttr.IsComplexType == false && authAttr.IsCollection)
                    {
                        Iscollection(obj, row, info, fieldName);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(authAttr.DisplayName))
                    {
                        fieldName = authAttr.DisplayName;
                    }
                    if (!string.IsNullOrEmpty(authAttr.CurrencyType))
                    {
                        dynamic amt = info.GetValue(obj);
                        if (typeof(T) == typeof(Auditaddress) || typeof(T) == typeof(FranchiseeLoanViewModel))
                        {
                            amt = Convert.ToDouble(amt);
                        }
                        if (amt == null) continue;
                        if (amt == -1 && typeof(T) == typeof(InvoiceViewModel)) continue;
                        amt = authAttr.CurrencyType.ToString() + Math.Round(amt, 2).ToString();
                        row[fieldName] = amt;
                        continue;
                    }

                }

                if (typeof(T) == typeof(InvoiceViewModel))
                {
                    if (!IsNullableType(info.PropertyType))
                    {
                        var value = info.GetValue(obj);
                        
                        if ((value is long) && (long)value == -1) continue;
                        if ((value is decimal) && (decimal)value == -1) continue;
                        if ((value is int) && (int)value == -1) continue;
                        if ((value is DateTime) && ((DateTime)value == default)) continue;
                        row[fieldName] = info.GetValue(obj);
                    }
                    else
                    {
                        object value = info.GetValue(obj);
                        if ((value is long) && (long)value == -1) continue;
                        if ((value is decimal) && (decimal)value == -1) continue;
                        if ((value is int) && (int)value == -1) continue;
                        if ((value is DateTime) && ((DateTime)value == default)) continue;
                        row[fieldName] = (info.GetValue(obj) ?? DBNull.Value);
                    }

                }

                if (fieldName != null)
                {
                    if (!IsNullableType(info.PropertyType))
                    {
                        var invoiceId = info.GetValue(obj);
                        if (info.GetValue(obj) == default) continue;
                        row[fieldName] = info.GetValue(obj);
                    }
                    else
                    {
                        if (info.GetValue(obj) == default) continue;
                        object value = info.GetValue(obj);
                        row[fieldName] = (info.GetValue(obj) ?? DBNull.Value);
                    }
                }
            }
        }
        private static void Iscollection<T>(T obj, DataRow row, PropertyInfo info, string fieldName)
        {
            IList oList = info.GetValue(obj) as IList;
            if (oList.Count >= 1)
            {
                string _iteam = string.Empty;
                foreach (var listItem in oList)
                {
                    _iteam = string.Join(", ", listItem);
                }
                row[fieldName] = _iteam;
            }
            else
                row[fieldName] = "";
        }
        private static DataTable SetColumns<T>(DataTable dt, PropertyInfo[] propertyInfoCollection, bool isAdded, bool isValid)
        {
            bool isExcel = false;
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

            foreach (PropertyInfo propertyInfo in propertyInfoCollection)
            {
                if (typeof(T) == typeof(SalesFunnelLocalExcelViewModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        if (displayAttr.DisplayName != "Id")
                        {
                            dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                        }
                    }
                }
                else if (typeof(T) == typeof(SalesFunnelNationalExcelViewModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        if (displayAttr.DisplayName != "Id")
                        {
                            dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                        }
                    }
                }
                else if (typeof(T) == typeof(FranchiseeLoanViewModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        if (displayAttr.DisplayName != "Id")
                        {
                            dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                        }
                    }
                }
                else if (typeof(T) == typeof(DownloadZipCodeModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        if (displayAttr.DisplayName != "DIRVE DIST")
                        {
                            dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                        }
                    }
                }
                else if (typeof(T) == typeof(DownloadCountyModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                    }
                }
                else if (typeof(T) == typeof(WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        //if (displayAttr.DisplayName != "TotalInt")
                        //{
                        dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                        //    continue;
                        //}
                    }
                }
                else if (typeof(T) == typeof(Auditaddress))
                {
                    var propertyType = propertyInfo.PropertyType;
                    var attr = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr as DisplayNameAttribute;
                    if (displayAttr != null && displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        if (displayAttr.DisplayName != "TotalInt")
                        {
                            dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                            continue;
                        }
                    }
                }
                else if (typeof(T) == typeof(WeeklyUnpaidInvoiceNotificationReportFranchiseeWiseModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (!(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                    }
                }
                else if (typeof(T) == typeof(FranchiseeViewModelForDownload))
                {
                    var propertyType = propertyInfo.PropertyType;
                    var attr = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr as DisplayNameAttribute;
                    if (displayAttr != null && displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        if (displayAttr.DisplayName != "TotalInt")
                        {
                            dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                            continue;
                        }
                    }
                }
                else if (typeof(T) == typeof(CallDetailViewModelV2))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                    }
                }
                else if (typeof(T) == typeof(DownloadZipCodeModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                    }
                }
                else if (typeof(T) == typeof(DownloadAllInvoiceModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                    }
                }
                else if (typeof(T) == typeof(CallDetailViewModel))
                {
                    var attr2 = propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), true).SingleOrDefault();
                    var displayAttr = attr2 as DisplayNameAttribute;
                    if (displayAttr != null && !(string.IsNullOrEmpty(displayAttr.DisplayName)))
                    {
                        dt.Columns.Add(new DataColumn(displayAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                    }
                }
                else
                {
                    var propertyType = propertyInfo.PropertyType;
                    var attr = propertyInfo.GetCustomAttributes(typeof(DownloadFieldAttribute), true).SingleOrDefault();

                    if (attr == null || !(attr is DownloadFieldAttribute))
                    {
                        dt.Columns.Add(new DataColumn(propertyInfo.Name, GetNullableType(propertyInfo.PropertyType)));
                        continue;
                    }

                    var authAttr = attr as DownloadFieldAttribute;

                    if (!authAttr.Required) continue;

                    if (authAttr.IsComplexType == true && authAttr.IsCollection == false)
                    {
                        PropertyInfo[] props = propertyType.GetProperties();
                        SetColumns<T>(dt, props, false, true);
                        continue;
                    }

                    if (!(string.IsNullOrEmpty(authAttr.DisplayName)))
                    {
                        dt.Columns.Add(new DataColumn(authAttr.DisplayName, GetNullableType(propertyInfo.PropertyType)));
                        continue;
                    }
                    if (!(string.IsNullOrEmpty(authAttr.CurrencyType)) || authAttr.IsCollection)
                    {

                        dt.Columns.Add(new DataColumn(propertyInfo.Name));
                        continue;
                    }
                    else
                    {
                        dt.Columns.Add(new DataColumn(propertyInfo.Name, GetNullableType(propertyInfo.PropertyType)));
                    }
                }
            }
            return dt;
        }
        private static Type GetNullableType(Type t)
        {
            Type returnType = t;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                returnType = Nullable.GetUnderlyingType(t);
            }
            return returnType;
        }
        private static bool IsNullableType(Type type)
        {
            return (type == typeof(string) ||
                    type.IsArray ||
                    (type.IsGenericType &&
                     type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        private static bool CreateExcelDocument(DataTable dt, string xlsxFilePath)
        {
            var ds = new DataSet();
            ds.Tables.Add(dt);
            bool result = CreateExcelDocumentForGivenDataSet(ds, xlsxFilePath);
            ds.Tables.Remove(dt);
            return result;
        }
        #endregion

        public bool CreateExcelDocument(DataSet ds, string excelFilename)
        {
            return CreateExcelDocumentForGivenDataSet(ds, excelFilename);
        }

        private static bool CreateExcelDocumentForGivenDataSet(DataSet ds, string excelFilename)
        {
            try
            {
                using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename, SpreadsheetDocumentType.Workbook))
                {
                    WriteExcelFile(ds, document);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet)
        {

            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new Workbook();


            spreadsheet.WorkbookPart.Workbook.Append(new BookViews(new WorkbookView()));


            var workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            var stylesheet = new Stylesheet();
            workbookStylesPart.Stylesheet = stylesheet;

            uint worksheetNumber = 1;
            foreach (DataTable dt in ds.Tables)
            {
                string workSheetId = "rId" + worksheetNumber;
                string worksheetName = dt.TableName;

                var newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new Worksheet();

                newWorksheetPart.Worksheet.AppendChild(new SheetData());


                WriteDataTableToExcelWorksheet(dt, newWorksheetPart);
                newWorksheetPart.Worksheet.Save();

                if (worksheetNumber == 1)
                    spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());

                spreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet()
                {
                    Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
                    SheetId = (uint)worksheetNumber,
                    Name = dt.TableName
                });

                worksheetNumber++;
            }

            spreadsheet.WorkbookPart.Workbook.Save();
        }


        private static void WriteDataTableToExcelWorksheet(DataTable dt, WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();

            string cellValue = "";


            int numberOfColumns = dt.Columns.Count;
            var isNumericColumn = new bool[numberOfColumns];

            var excelColumnNames = new string[numberOfColumns];
            for (int n = 0; n < numberOfColumns; n++)
                excelColumnNames[n] = GetExcelColumnName(n);


            uint rowIndex = 1;

            var headerRow = new Row { RowIndex = rowIndex };
            sheetData.Append(headerRow);

            for (int colInx = 0; colInx < numberOfColumns; colInx++)
            {
                DataColumn col = dt.Columns[colInx];
                AppendTextCell(excelColumnNames[colInx] + "1", col.ColumnName, headerRow);
                isNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32");
            }


            double cellNumericValue = 0;
            foreach (DataRow dr in dt.Rows)
            {

                ++rowIndex;
                var newExcelRow = new Row { RowIndex = rowIndex };
                sheetData.Append(newExcelRow);

                for (int colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    cellValue = dr.ItemArray[colInx].ToString();


                    if (isNumericColumn[colInx])
                    {

                        cellNumericValue = 0;
                        if (double.TryParse(cellValue, out cellNumericValue))
                        {
                            cellValue = cellNumericValue.ToString(CultureInfo.InvariantCulture);
                            AppendNumericCell(excelColumnNames[colInx] + rowIndex, cellValue, newExcelRow);
                        }
                    }
                    else
                    {

                        AppendTextCell(excelColumnNames[colInx] + rowIndex, cellValue, newExcelRow);
                    }
                }
            }
        }

        private static void AppendTextCell(string cellReference, string cellStringValue, Row excelRow)
        {

            var cell = new Cell() { CellReference = cellReference, DataType = CellValues.String };
            var cellValue = new CellValue { Text = cellStringValue };
            cell.Append(cellValue);
            excelRow.Append(cell);
        }

        private static void AppendNumericCell(string cellReference, string cellStringValue, Row excelRow)
        {
            var cell = new Cell() { CellReference = cellReference };
            var cellValue = new CellValue();
            cellValue.Text = cellStringValue;
            cell.Append(cellValue);
            excelRow.Append(cell);
        }

        private static string GetExcelColumnName(int columnIndex)
        {
            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            var firstChar = (char)('A' + (columnIndex / 26) - 1);
            var secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }


        public void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers    
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = string.Empty;
                        if (dtDataTable.Columns[i].ColumnName.Contains("Date"))
                        {
                            DateTime dateValue = DateTime.MinValue;
                            var dt = Convert.ToDateTime(dr[i].ToString());//.ToString("MM/dd/yyyy",  new CultureInfo("en-US")); ;
                            //var dt = (DateTime.TryParseExact(dr[i].ToString(), "MM/dd/yyyy", new CultureInfo("en-US"),
                  //DateTimeStyles.None, out dateValue));
                          //  value = dt;
                            //value = string.Format("{0:00}/{1:00}/{2:0000}", dt.Month, dt.Day, dt.Year);
                            var dateSplit = value.Split('-');
                            //var year = dateSplit[2];
                            //var month = dateSplit[0];
                            //var date1 = dateSplit[1];
                            //value = string.Format("{0}/{1}/{2}", month, date1, year);
                            sw.Write(value);
                        }
                        else
                        {
                            value = dr[i].ToString();
                            if (value.Contains(','))
                            {
                                value = String.Format("\"{0}\"", value);
                                sw.Write(value);
                            }
                            else
                            {
                                sw.Write(dr[i].ToString());
                            }
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }


        public void ToCSVWriter<T>(List<T> dtDataTable, string strFilePath)
        {
            //var classMap = new CSVMapData();
            using (var writer = new StreamWriter(strFilePath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                //csv.Context.RegisterClassMap(classMap);
                csv.WriteRecords(dtDataTable);
            }
        }
        public class DateOutputConverter : ITypeConverter
        {
            public object ConvertFromString(string text,
                IReaderRow row, MemberMapData memberMapData)
            {
                throw new NotImplementedException();
            }
            public string ConvertToString(
                object value,
                IWriterRow row,
                MemberMapData memberMapData)
            {
                var retval = ((DateTime)value).ToString("MM\\/dd\\/yyyy");
                return retval;
            }

        }
        public class CSVMapData : ClassMap<DownloadInvoiceModel>
        {
            public CSVMapData()
            {
                Map(m => m.ItemId);
                Map(m => m.InvoiceItem);
                //Map(m => m.StartDate).TypeConverter(new DateOutputConverter());
                Map(m => m.EndDate).TypeConverter(new DateOutputConverter());
                //Map(m => m.DueDate).TypeConverter(new DateOutputConverter());
                //Map(m => m.PaymentDate).TypeConverter(new DateOutputConverter());
                Map(m => m.PaymentMethod);
                Map(m => m.Customer);
                Map(m => m.BillToLine1);
                Map(m => m.BillToLine2);
                Map(m => m.BillToLine3);
                Map(m => m.BillToCity);
                Map(m => m.BillToState);
                Map(m => m.BillToPostalCode);
                Map(m => m.Terms);
                Map(m => m.Item);
                Map(m => m.Description);
                Map(m => m.Quantity);
                Map(m => m.TotalAmount);
                Map(m => m.Price);
                Map(m => m.AdfundRoyalty);
            }
        }
    }
}
