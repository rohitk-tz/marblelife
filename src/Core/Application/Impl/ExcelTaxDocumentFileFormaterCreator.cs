using Core.Application.Attribute;
using Core.Organizations.ViewModel;
using Core.Sales.ViewModel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;

namespace Core.Application.Impl
{

    [DefaultImplementation]
    public class ExcelTaxDocumentFileFormaterCreator : IExcelTaxDocumentFileFormaterCreator
    {
        public IDictionary<int, CellValues> getColumnType()
        {
            IDictionary<int, CellValues> dict = new Dictionary<int, CellValues>();
            dict.Add(1, CellValues.SharedString);
            dict.Add(2, CellValues.SharedString);
            dict.Add(3, CellValues.SharedString);
            dict.Add(4, CellValues.SharedString);
            dict.Add(5, CellValues.SharedString);
            dict.Add(6, CellValues.SharedString);
            dict.Add(7, CellValues.SharedString);
            dict.Add(8, CellValues.SharedString);
            dict.Add(9, CellValues.SharedString);
            dict.Add(10, CellValues.SharedString);
            dict.Add(11, CellValues.SharedString);
            dict.Add(12, CellValues.SharedString);
            return dict;
        }

        public List<String> Values()
        {
            return new List<string>
            {
                "Name",
                "Email",
                "PrimaryContact",
                "StreetAddress",
                "State",
                "City",
                "Zip",
                "Country",
                "OfficeNumber",
                "CellNumber",
                "CallCenter",
                "BusinessDirectory"
            };
        }
        public bool CreateExcelDocument(List<FranchiseeDocumentViewModel> list, string xlsxFilePath, List<string> columnList)
        {
            columnList.Insert(0, "Franchisee Name");
            int lastIndex = 0;
            var dt = new DataTable();
            dt = ListToDataTable(columnList, xlsxFilePath);
            DataTable ds = AddCoumns(columnList, dt);

            ds = AddRows(list, ds, columnList);
            return CreateExcelDocuments(ds, xlsxFilePath);
        }
        public bool CreateExcelDocuments(DataTable dt, string excelFilename)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilename, SpreadsheetDocumentType.Workbook))
            {
                return CreateParts(document, excelFilename, dt);
            }
        }
        public DataTable AddCoumns(List<string> listElements, DataTable dt)
        {
            dt = SetColumns(listElements);
            return dt;
        }
        public DataTable AddRows(List<FranchiseeDocumentViewModel> listElements, DataTable dt, List<string> columnList)
        {
            foreach (var listGroupedElement in listElements)
            {
                DataRow _row = dt.NewRow();
                
                dt.Rows.Add(_row);
                int index = 0;
                _row[columnList[0]] = listGroupedElement.FranchiseeName;
                foreach (var value in listGroupedElement.IsPresent)
                {
                    if (value)
                    {
                        if (!listGroupedElement.IsDeclined[index] && !listGroupedElement.IsPerpetuity[index])
                        {
                            _row[columnList[index+1]] = Convert.ToDateTime(listGroupedElement.ExpiryDate[index]).ToShortDateString();
                        }
                        else if (listGroupedElement.IsDeclined[index] && !listGroupedElement.IsPerpetuity[index])
                        {
                            _row[columnList[index+1]] = "Declined Document";
                        }
                        else if (listGroupedElement.IsPerpetuity[index])
                        {
                            _row[columnList[index+1]] = "Perpetuity was On";
                        }
                    }
                    else
                    {
                        if (listGroupedElement.IsPerpetuity.Count() == 0)
                        {
                            _row[columnList[index+1]] = "x";
                        }
                       else  if (!listGroupedElement.IsPerpetuity[index])
                        {
                            _row[columnList[index+1]] = "x";
                        }
                        else if (listGroupedElement.IsPerpetuity[index])
                        {
                            _row[columnList[index+1]] = "Perpetuity was On";
                        }
                    }
                    ++index;
                }
                //if (isAnnualCell)
                //{
                //    DataRow _row4 = dt.NewRow();
                //    dt.Rows.Add(_row4);
                //}
                //if (listGroupedElement.FranchiseeViewModel.Count() == 0 && !isAnnualCell)
                //{
                //    DataRow _row3 = dt.NewRow();
                //    _row3[0] = "No Record Found";
                //    dt.Rows.Add(_row3);

                //}
                //else
                //{
                //    if (!isAnnualCell)
                //    {
                //        foreach (var listElement in listGroupedElement.FranchiseeViewModel)
                //        {
                //            DataRow _row2 = dt.NewRow();
                //            _row2[columnList[0]] = listElement.Name;
                //           _row2[columnList[1]] = listElement.Email;
                //            _row2[columnList[2]] = listElement.OwnerName;
                //            _row2[columnList[3]] = listElement.Address;
                //            _row2[columnList[4]] = listElement.State;
                //            _row2[columnList[5]] = listElement.City;
                //            _row2[columnList[6]] = listElement.ZipCode;
                //            _row2[columnList[7]] = listElement.Country;
                //            _row2[columnList[8]] = listElement.OFFICEPhone;
                //            _row2[columnList[9]] = listElement.CellPhone;
                //            _row2[columnList[10]] = listElement.CallCenterPhone;
                //            _row2[columnList[11]] = listElement.BusinessPhone;
                //           dt.Rows.Add(_row2);
                //        }
                //    }
                //}

            }
            return dt;
        }
        public DataTable ListToDataTable(List<string> list, string tableName = "")
        {
            DataTable dt = SetColumns(list);
            return dt;
        }
        private DataTable SetColumns(List<string> list)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < list.Count(); i++)
            {
                dt.Columns.Add(list[i]);
            }
            return dt;
        }




        private bool CreateParts(SpreadsheetDocument document, string excelFilename, DataTable dt)
        {
            WorkbookPart workbookPart1 = document.AddWorkbookPart();
            GenerateWorkbookPart1Content(workbookPart1);

            WorkbookStylesPart workbookStylesPart1 = workbookPart1.AddNewPart<WorkbookStylesPart>("rId1");
            GenerateWorkbookStylesPart1Content(workbookStylesPart1);

            WorksheetPart worksheetPart1 = workbookPart1.AddNewPart<WorksheetPart>("rId2");
            GenerateWorksheetPart1Content(worksheetPart1, dt);

            SharedStringTablePart sharedStringTablePart1 = workbookPart1.AddNewPart<SharedStringTablePart>("rId3");
            CustomizedCode(worksheetPart1, sharedStringTablePart1, dt);
            //GenerateSharedStringTablePart1Content(sharedStringTablePart1);

            ExtendedFilePropertiesPart extendedFilePropertiesPart1 = document.AddNewPart<ExtendedFilePropertiesPart>("rId3");
            GenerateExtendedFilePropertiesPart1Content(extendedFilePropertiesPart1);

            SetPackageProperties(document);
            return true;
        }
        private void SetPackageProperties(OpenXmlPackage document)
        {
            document.PackageProperties.Creator = "";
            document.PackageProperties.Title = "";
            document.PackageProperties.Subject = "";
            document.PackageProperties.Description = "";
            document.PackageProperties.Revision = "2";
            document.PackageProperties.Created = System.Xml.XmlConvert.ToDateTime("2019-03-06T03:09:24Z", System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
            document.PackageProperties.Modified = System.Xml.XmlConvert.ToDateTime("2019-03-06T03:34:34Z", System.Xml.XmlDateTimeSerializationMode.RoundtripKind);
            document.PackageProperties.LastModifiedBy = "";
            document.PackageProperties.Language = "en-US";
        }
        private void GenerateExtendedFilePropertiesPart1Content(ExtendedFilePropertiesPart extendedFilePropertiesPart1)
        {
            Ap.Properties properties1 = new Ap.Properties();
            properties1.AddNamespaceDeclaration("vt", "http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes");
            Ap.Template template1 = new Ap.Template();
            template1.Text = "";
            Ap.TotalTime totalTime1 = new Ap.TotalTime();
            totalTime1.Text = "5";
            Ap.Application application1 = new Ap.Application();
            application1.Text = "LibreOffice/6.1.4.2$Windows_X86_64 LibreOffice_project/9d0f32d1f0b509096fd65e0d4bec26ddd1938fd3";

            properties1.Append(template1);
            properties1.Append(totalTime1);
            properties1.Append(application1);

            extendedFilePropertiesPart1.Properties = properties1;
        }
        private void CustomizedCode(WorksheetPart worksheetPart1, SharedStringTablePart sharedStringTablePart1, DataTable dt)
        {
            Worksheet worksheet1 = new Worksheet();
            SharedStringTable sharedStringTable1 = new SharedStringTable() { Count = (UInt32Value)8U, UniqueCount = (UInt32Value)8U };
            SheetViews sheetViews1 = new SheetViews();
            Columns columns1 = GetColumns(dt.Columns.Count);
            Pane pane1 = new Pane() { VerticalSplit = 1D, TopLeftCell = "A2", ActivePane = PaneValues.BottomLeft, State = PaneStateValues.Frozen };
            SheetView sheetView1 = new SheetView() { ShowFormulas = false, ShowGridLines = true, ShowRowColHeaders = true, ShowZeros = true, RightToLeft = false, TabSelected = true, ShowOutlineSymbols = true, DefaultGridColor = true, View = SheetViewValues.Normal, TopLeftCell = "A1", ColorId = (UInt32Value)64U, ZoomScale = (UInt32Value)100U, ZoomScaleNormal = (UInt32Value)100U, ZoomScalePageLayoutView = (UInt32Value)100U, WorkbookViewId = (UInt32Value)0U };
            Selection selection1 = new Selection() { Pane = PaneValues.TopLeft, ActiveCell = "A1", ActiveCellId = (UInt32Value)0U, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "A1" } };
            Selection selection2 = new Selection() { Pane = PaneValues.BottomLeft, ActiveCell = "D11", ActiveCellId = (UInt32Value)0U, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "D11" } };

            sheetView1.Append(pane1);
            sheetView1.Append(selection1);
            sheetView1.Append(selection2);

            sheetViews1.Append(sheetView1);


            worksheet1.Append(sheetViews1);
            //SheetData sheetData2 = AppendColumns(dt);
            SheetData sheetData1 = AppendRows(dt);
            List<SharedStringItem> sharedStringItem1 = SetHeader(dt);
            List<SharedStringItem> sharedStringItem2 = GenerateSharedStringTable(dt, sharedStringTablePart1);
            sharedStringItem1.AddRange(sharedStringItem2);
            sharedStringTable1.Append(sharedStringItem1);

            worksheet1.Append(columns1);
            worksheet1.Append(sheetData1);
            worksheetPart1.Worksheet = worksheet1;
            sharedStringTablePart1.SharedStringTable = (sharedStringTable1);
        }
        private List<SharedStringItem> GenerateSharedStringTable(DataTable dt, SharedStringTablePart sharedStringTablePart1)
        {
            int rowIndexForPassing = 0;
            List<SharedStringItem> listofShared = new List<SharedStringItem>();
            foreach (var rows in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    CellValues value = getDataType(i);
                    if (value == CellValues.SharedString)
                    {
                        SharedStringItem sharedStringItem1 = new SharedStringItem();
                        Text text1 = new Text() { Space = SpaceProcessingModeValues.Preserve };
                        text1.Text = dt.Rows[rowIndexForPassing][i].ToString();
                        sharedStringItem1.Append(text1);
                        listofShared.Add(sharedStringItem1);
                    }
                }
                ++rowIndexForPassing;
            }
            return listofShared;
        }
        private CellValues getDataType(int columnIndex)
        {
            var columnNames = getColumnType();
            return columnNames[++columnIndex];
        }
        private List<SharedStringItem> SetHeader(DataTable dt)
        {
            int rowIndexForPassing = 0;
            List<SharedStringItem> listOfColumns = new List<SharedStringItem>();
            foreach (DataColumn column in dt.Columns)
            {
                SharedStringItem sharedStringItem1 = new SharedStringItem();
                Text text1 = new Text() { Space = SpaceProcessingModeValues.Preserve };
                text1.Text = column.ColumnName;
                sharedStringItem1.Append(text1);
                listOfColumns.Add(sharedStringItem1);
            }
            ++rowIndexForPassing;
            return listOfColumns;
        }
        private Columns GetColumns(int totalColumns)
        {
            Columns columns1 = new Columns();
            for (int a = 0; a < totalColumns; a++)
            {
                //string columnName = "column" + a;
                Column columnName = new Column() { Min = (UInt32Value)1U, Max = (UInt32Value)1U, Width = 40.52D, Style = (UInt32Value)1U, Hidden = false, CustomWidth = true, OutlineLevel = 0, Collapsed = false };
                columns1.Append(columnName);
            }
            return columns1;
        }
        private SheetData AppendRows(DataTable dt)
        {
            int cellValue = 0;
            uint rowIndex = 1;
            int rowIndexForPassing = 0;

            int isHeader = 0;
            UInt32Value cellStyleIndex = 1U;
            SheetData sheetData1 = new SheetData();
            Row row = new Row() { RowIndex = (UInt32Value)rowIndex, StyleIndex = (UInt32Value)1U, CustomFormat = false, Height = 12.8D, Hidden = false, CustomHeight = false, OutlineLevel = 0, Collapsed = false };
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                string cellName = getCellName(j) + rowIndex;
                CellValues value = getDataType(j);
                Cell cell1 = new Cell() { CellReference = cellName, StyleIndex = (UInt32Value)cellStyleIndex, DataType = CellValues.SharedString };
                CellValue cellValue1 = new CellValue();
                cellValue1.Text = cellValue.ToString();
                ++cellValue;
                cell1.Append(cellValue1);
                row.Append(cell1);
            }
            sheetData1.Append(row);
            rowIndex++;
            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                Row row1;
                var cell = dt.Rows[i][0];
                string cellValues = cell.ToString();
                var isAnnualCell = cell.ToString().Contains("ANNUAL SALES DATA:") ? true : false;
                if (isAnnualCell)
                {
                    row1 = new Row() { RowIndex = (UInt32Value)rowIndex, CustomFormat = true, Height = 32.8D, Hidden = false, CustomHeight = true, OutlineLevel = 0, Collapsed = false };
                    cellStyleIndex = 6U;
                    //continue;

                }
                else if (cell == "CORPORATE OFFICES (0-PREFIX)" || cell == "MARBLELIFE OFFICES - US"
                    || cellValues == "CANADA" || cell == "FOREIGN OFFICES" || cellValues == "Canada")
                {
                    row1 = new Row() { RowIndex = (UInt32Value)rowIndex, CustomFormat = true, Height = 32.8D, Hidden = false, CustomHeight = true, OutlineLevel = 0, Collapsed = false };
                    cellStyleIndex = 6U;
                }
                else
                {

                    row1 = new Row() { RowIndex = (UInt32Value)rowIndex, CustomFormat = true, Height = 12.8D, Hidden = false, CustomHeight = true, OutlineLevel = 0, Collapsed = false };
                    cellStyleIndex = 1U;
                }

                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    string cellName = getCellName(j) + rowIndex;
                    CellValues value = getDataType(j);
                    Cell cell1 = new Cell() { CellReference = cellName, StyleIndex = (UInt32Value)cellStyleIndex, DataType = value };
                    CellValue cellValue1 = new CellValue();
                    if (value == CellValues.Number)
                    {
                        cellValue1.Text = dt.Rows[rowIndexForPassing][j].ToString();
                    }
                    else if (value == CellValues.SharedString)
                    {
                        cellValue1.Text = cellValue.ToString();
                        ++cellValue;
                    }
                    else if (value == CellValues.Date)
                    {
                        cellValue1.Text = dt.Rows[rowIndexForPassing][j].ToString();
                    }
                    cell1.Append(cellValue1);
                    row1.Append(cell1);
                }
                ++rowIndex;
                sheetData1.Append(row1);
                ++rowIndexForPassing;

            }
            return sheetData1;
        }



        private string getCellName(int columnIndex)
        {

            if (columnIndex < 26)
                return ((char)('A' + columnIndex)).ToString();

            var firstChar = (char)('A' + (columnIndex / 26) - 1);
            var secondChar = (char)('A' + (columnIndex % 26));

            return string.Format("{0}{1}", firstChar, secondChar);
        }
        // Generates content of workbookPart1.



        private void GenerateWorkbookPart1Content(WorkbookPart workbookPart1)
        {
            Workbook workbook1 = new Workbook();
            //workbook1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");

            FileVersion fileVersion1 = new FileVersion() { ApplicationName = "Calc" };

            WorkbookProperties workbookProperties1 = new WorkbookProperties() { Date1904 = false, ShowObjects = ObjectDisplayValues.All, BackupFile = false };
            WorkbookProtection workbookProtection1 = new WorkbookProtection();

            BookViews bookViews1 = new BookViews();
            WorkbookView workbookView1 = new WorkbookView() { ShowHorizontalScroll = true, ShowVerticalScroll = true, ShowSheetTabs = true, XWindow = 0, YWindow = 0, WindowWidth = (UInt32Value)16384U, WindowHeight = (UInt32Value)8192U, TabRatio = (UInt32Value)500U, FirstSheet = (UInt32Value)0U, ActiveTab = (UInt32Value)0U };

            bookViews1.Append(workbookView1);

            Sheets sheets1 = new Sheets();
            Sheet sheet1 = new Sheet() { Name = "Sheet", SheetId = (UInt32Value)1U, State = SheetStateValues.Visible, Id = "rId2" };

            sheets1.Append(sheet1);
            //CalculationProperties calculationProperties1 = new CalculationProperties() { ReferenceMode = ReferenceModeValues.A1, Iterate = false, IterateCount = (UInt32Value)100U, IterateDelta = 0.001D };

            WorkbookExtensionList workbookExtensionList1 = new WorkbookExtensionList();

            WorkbookExtension workbookExtension1 = new WorkbookExtension() { Uri = "{7626C862-2A13-11E5-B345-FEFF819CDC9F}" };
            workbookExtension1.AddNamespaceDeclaration("loext", "http://schemas.libreoffice.org/");
            workbookExtension1.AddNamespaceDeclaration("x", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");

            OpenXmlUnknownElement openXmlUnknownElement1 = OpenXmlUnknownElement.CreateOpenXmlUnknownElement("<loext:extCalcPr stringRefSyntax=\"CalcA1\" xmlns:loext=\"http://schemas.libreoffice.org/\" />");

            workbookExtension1.Append(openXmlUnknownElement1);

            workbookExtensionList1.Append(workbookExtension1);

            workbook1.Append(fileVersion1);
            workbook1.Append(workbookProperties1);
            workbook1.Append(workbookProtection1);
            workbook1.Append(bookViews1);
            workbook1.Append(sheets1);
            //workbook1.Append(calculationProperties1);
            workbook1.Append(workbookExtensionList1);

            workbookPart1.Workbook = workbook1;
        }

        // Generates content of workbookStylesPart1.
        private void GenerateWorkbookStylesPart1Content(WorkbookStylesPart workbookStylesPart1)
        {
            Stylesheet stylesheet1 = new Stylesheet();
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            NumberingFormats numberingFormats1 = new NumberingFormats() { Count = (UInt32Value)3U };
            NumberingFormat numberingFormat1 = new NumberingFormat() { NumberFormatId = (UInt32Value)164U, FormatCode = "General" };
            NumberingFormat numberingFormat2 = new NumberingFormat() { NumberFormatId = (UInt32Value)165U, FormatCode = "_(\\$* #,##0.00_);_(\\$* \\(#,##0.00\\);_(\\$* \\-??_);_(@_)" };
            NumberingFormat numberingFormat3 = new NumberingFormat() { NumberFormatId = (UInt32Value)166U, FormatCode = "\\$#,##0.00_);[RED]\"($\"#,##0.00\\)" };

            numberingFormats1.Append(numberingFormat1);
            numberingFormats1.Append(numberingFormat2);
            numberingFormats1.Append(numberingFormat3);

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)7U };

            Font font1 = new Font();
            FontSize fontSize1 = new FontSize() { Val = 10D };
            FontName fontName1 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };

            font1.Append(fontSize1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);

            Font font2 = new Font();
            FontSize fontSize2 = new FontSize() { Val = 10D };
            FontName fontName2 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 0 };

            font2.Append(fontSize2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);

            Font font3 = new Font();
            FontSize fontSize3 = new FontSize() { Val = 10D };
            FontName fontName3 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering3 = new FontFamilyNumbering() { Val = 0 };

            font3.Append(fontSize3);
            font3.Append(fontName3);
            font3.Append(fontFamilyNumbering3);

            Font font4 = new Font();
            FontSize fontSize4 = new FontSize() { Val = 10D };
            FontName fontName4 = new FontName() { Val = "Arial" };
            FontFamilyNumbering fontFamilyNumbering4 = new FontFamilyNumbering() { Val = 0 };

            font4.Append(fontSize4);
            font4.Append(fontName4);
            font4.Append(fontFamilyNumbering4);

            Font font5 = new Font();
            FontSize fontSize5 = new FontSize() { Val = 11D };
            Color color1 = new Color() { Rgb = "FF000000" };
            FontName fontName5 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering5 = new FontFamilyNumbering() { Val = 2 };
            FontCharSet fontCharSet1 = new FontCharSet() { Val = 1 };

            font5.Append(fontSize5);
            font5.Append(color1);
            font5.Append(fontName5);
            font5.Append(fontFamilyNumbering5);
            font5.Append(fontCharSet1);

            Font font6 = new Font();
            Bold bold1 = new Bold() { Val = true };
            FontSize fontSize6 = new FontSize() { Val = 18D };
            Color color2 = new Color() { Rgb = "FF000000" };
            FontName fontName6 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering6 = new FontFamilyNumbering() { Val = 2 };
            FontCharSet fontCharSet2 = new FontCharSet() { Val = 1 };

            font6.Append(bold1);
            font6.Append(fontSize6);
            font6.Append(color2);
            font6.Append(fontName6);
            font6.Append(fontFamilyNumbering6);
            font6.Append(fontCharSet2);

            Font font7 = new Font();
            FontSize fontSize7 = new FontSize() { Val = 11D };
            Color color3 = new Color() { Rgb = "FFFF0000" };
            FontName fontName7 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering7 = new FontFamilyNumbering() { Val = 2 };
            FontCharSet fontCharSet3 = new FontCharSet() { Val = 1 };

            font7.Append(fontSize7);
            font7.Append(color3);
            font7.Append(fontName7);
            font7.Append(fontFamilyNumbering7);
            font7.Append(fontCharSet3);

            fonts1.Append(font1);
            fonts1.Append(font2);
            fonts1.Append(font3);
            fonts1.Append(font4);
            fonts1.Append(font5);
            fonts1.Append(font6);
            fonts1.Append(font7);

            Fills fills1 = new Fills() { Count = (UInt32Value)4U };

            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };

            fill1.Append(patternFill1);

            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };

            fill2.Append(patternFill2);

            Fill fill3 = new Fill();

            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFFFF00" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Rgb = "FFFFFF00" };

            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);

            fill3.Append(patternFill3);

            Fill fill4 = new Fill();

            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Rgb = "FF00B0F0" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Rgb = "FF33CCCC" };

            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);

            fill4.Append(patternFill4);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);

            Borders borders1 = new Borders() { Count = (UInt32Value)1U };

            Border border1 = new Border() { DiagonalUp = false, DiagonalDown = false };
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)20U };

            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment1 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection1 = new Protection() { Locked = true, Hidden = false };

            cellFormat1.Append(alignment1);
            cellFormat1.Append(protection1);
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)2U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat6 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat7 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat8 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat9 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat10 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat11 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat12 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat13 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat14 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat15 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat16 = new CellFormat() { NumberFormatId = (UInt32Value)43U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat17 = new CellFormat() { NumberFormatId = (UInt32Value)41U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };

            CellFormat cellFormat18 = new CellFormat() { NumberFormatId = (UInt32Value)165U, FontId = (UInt32Value)4U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = true, ApplyProtection = false };
            Alignment alignment2 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };

            cellFormat18.Append(alignment2);
            CellFormat cellFormat19 = new CellFormat() { NumberFormatId = (UInt32Value)42U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            CellFormat cellFormat20 = new CellFormat() { NumberFormatId = (UInt32Value)9U, FontId = (UInt32Value)1U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };

            cellStyleFormats1.Append(cellFormat1);
            cellStyleFormats1.Append(cellFormat2);
            cellStyleFormats1.Append(cellFormat3);
            cellStyleFormats1.Append(cellFormat4);
            cellStyleFormats1.Append(cellFormat5);
            cellStyleFormats1.Append(cellFormat6);
            cellStyleFormats1.Append(cellFormat7);
            cellStyleFormats1.Append(cellFormat8);
            cellStyleFormats1.Append(cellFormat9);
            cellStyleFormats1.Append(cellFormat10);
            cellStyleFormats1.Append(cellFormat11);
            cellStyleFormats1.Append(cellFormat12);
            cellStyleFormats1.Append(cellFormat13);
            cellStyleFormats1.Append(cellFormat14);
            cellStyleFormats1.Append(cellFormat15);
            cellStyleFormats1.Append(cellFormat16);
            cellStyleFormats1.Append(cellFormat17);
            cellStyleFormats1.Append(cellFormat18);
            cellStyleFormats1.Append(cellFormat19);
            cellStyleFormats1.Append(cellFormat20);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)12U };

            CellFormat cellFormat21 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = false, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            Alignment alignment3 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection2 = new Protection() { Locked = true, Hidden = false };

            cellFormat21.Append(alignment3);
            cellFormat21.Append(protection2);

            CellFormat cellFormat22 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            Alignment alignment4 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection3 = new Protection() { Locked = true, Hidden = false };

            cellFormat22.Append(alignment4);
            cellFormat22.Append(protection3);

            CellFormat cellFormat23 = new CellFormat() { NumberFormatId = (UInt32Value)165U, FontId = (UInt32Value)4U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)17U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment5 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = true, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection4 = new Protection() { Locked = true, Hidden = false };

            cellFormat23.Append(alignment5);
            cellFormat23.Append(protection4);

            CellFormat cellFormat24 = new CellFormat() { NumberFormatId = (UInt32Value)165U, FontId = (UInt32Value)4U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)17U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment6 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection5 = new Protection() { Locked = true, Hidden = false };

            cellFormat24.Append(alignment6);
            cellFormat24.Append(protection5);

            CellFormat cellFormat25 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)4U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = true, ApplyProtection = false };
            Alignment alignment7 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = true, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection6 = new Protection() { Locked = true, Hidden = false };

            cellFormat25.Append(alignment7);
            cellFormat25.Append(protection6);

            CellFormat cellFormat26 = new CellFormat() { NumberFormatId = (UInt32Value)165U, FontId = (UInt32Value)4U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)17U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment8 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = true, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection7 = new Protection() { Locked = true, Hidden = false };

            cellFormat26.Append(alignment8);
            cellFormat26.Append(protection7);

            CellFormat cellFormat27 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)5U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            Alignment alignment9 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection8 = new Protection() { Locked = true, Hidden = false };

            cellFormat27.Append(alignment9);
            cellFormat27.Append(protection8);

            CellFormat cellFormat28 = new CellFormat() { NumberFormatId = (UInt32Value)165U, FontId = (UInt32Value)5U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)17U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment10 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection9 = new Protection() { Locked = true, Hidden = false };

            cellFormat28.Append(alignment10);
            cellFormat28.Append(protection9);

            CellFormat cellFormat29 = new CellFormat() { NumberFormatId = (UInt32Value)165U, FontId = (UInt32Value)5U, FillId = (UInt32Value)3U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)17U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment11 = new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection10 = new Protection() { Locked = true, Hidden = false };

            cellFormat29.Append(alignment11);
            cellFormat29.Append(protection10);

            CellFormat cellFormat30 = new CellFormat() { NumberFormatId = (UInt32Value)166U, FontId = (UInt32Value)4U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)17U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment12 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection11 = new Protection() { Locked = true, Hidden = false };

            cellFormat30.Append(alignment12);
            cellFormat30.Append(protection11);

            CellFormat cellFormat31 = new CellFormat() { NumberFormatId = (UInt32Value)164U, FontId = (UInt32Value)6U, FillId = (UInt32Value)2U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFont = true, ApplyBorder = false, ApplyAlignment = false, ApplyProtection = false };
            Alignment alignment13 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection12 = new Protection() { Locked = true, Hidden = false };

            cellFormat31.Append(alignment13);
            cellFormat31.Append(protection12);

            CellFormat cellFormat32 = new CellFormat() { NumberFormatId = (UInt32Value)165U, FontId = (UInt32Value)4U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U, FormatId = (UInt32Value)17U, ApplyFont = true, ApplyBorder = true, ApplyAlignment = true, ApplyProtection = true };
            Alignment alignment14 = new Alignment() { Horizontal = HorizontalAlignmentValues.General, Vertical = VerticalAlignmentValues.Bottom, TextRotation = (UInt32Value)0U, WrapText = false, Indent = (UInt32Value)0U, ShrinkToFit = false };
            Protection protection13 = new Protection() { Locked = true, Hidden = false };

            cellFormat32.Append(alignment14);
            cellFormat32.Append(protection13);

            cellFormats1.Append(cellFormat21);
            cellFormats1.Append(cellFormat22);
            cellFormats1.Append(cellFormat23);
            cellFormats1.Append(cellFormat24);
            cellFormats1.Append(cellFormat25);
            cellFormats1.Append(cellFormat26);
            cellFormats1.Append(cellFormat27);
            cellFormats1.Append(cellFormat28);
            cellFormats1.Append(cellFormat29);
            cellFormats1.Append(cellFormat30);
            cellFormats1.Append(cellFormat31);
            cellFormats1.Append(cellFormat32);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)6U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
            CellStyle cellStyle2 = new CellStyle() { Name = "Comma", FormatId = (UInt32Value)15U, BuiltinId = (UInt32Value)3U };
            CellStyle cellStyle3 = new CellStyle() { Name = "Comma [0]", FormatId = (UInt32Value)16U, BuiltinId = (UInt32Value)6U };
            CellStyle cellStyle4 = new CellStyle() { Name = "Currency", FormatId = (UInt32Value)17U, BuiltinId = (UInt32Value)4U };
            CellStyle cellStyle5 = new CellStyle() { Name = "Currency [0]", FormatId = (UInt32Value)18U, BuiltinId = (UInt32Value)7U };
            CellStyle cellStyle6 = new CellStyle() { Name = "Percent", FormatId = (UInt32Value)19U, BuiltinId = (UInt32Value)5U };

            cellStyles1.Append(cellStyle1);
            cellStyles1.Append(cellStyle2);
            cellStyles1.Append(cellStyle3);
            cellStyles1.Append(cellStyle4);
            cellStyles1.Append(cellStyle5);
            cellStyles1.Append(cellStyle6);

            Colors colors1 = new Colors();

            IndexedColors indexedColors1 = new IndexedColors();
            RgbColor rgbColor1 = new RgbColor() { Rgb = "FF000000" };
            RgbColor rgbColor2 = new RgbColor() { Rgb = "FFFFFFFF" };
            RgbColor rgbColor3 = new RgbColor() { Rgb = "FFFF0000" };
            RgbColor rgbColor4 = new RgbColor() { Rgb = "FF00FF00" };
            RgbColor rgbColor5 = new RgbColor() { Rgb = "FF0000FF" };
            RgbColor rgbColor6 = new RgbColor() { Rgb = "FFFFFF00" };
            RgbColor rgbColor7 = new RgbColor() { Rgb = "FFFF00FF" };
            RgbColor rgbColor8 = new RgbColor() { Rgb = "FF00FFFF" };
            RgbColor rgbColor9 = new RgbColor() { Rgb = "FF800000" };
            RgbColor rgbColor10 = new RgbColor() { Rgb = "FF008000" };
            RgbColor rgbColor11 = new RgbColor() { Rgb = "FF000080" };
            RgbColor rgbColor12 = new RgbColor() { Rgb = "FF808000" };
            RgbColor rgbColor13 = new RgbColor() { Rgb = "FF800080" };
            RgbColor rgbColor14 = new RgbColor() { Rgb = "FF008080" };
            RgbColor rgbColor15 = new RgbColor() { Rgb = "FFC0C0C0" };
            RgbColor rgbColor16 = new RgbColor() { Rgb = "FF808080" };
            RgbColor rgbColor17 = new RgbColor() { Rgb = "FF9999FF" };
            RgbColor rgbColor18 = new RgbColor() { Rgb = "FF993366" };
            RgbColor rgbColor19 = new RgbColor() { Rgb = "FFFFFFCC" };
            RgbColor rgbColor20 = new RgbColor() { Rgb = "FFCCFFFF" };
            RgbColor rgbColor21 = new RgbColor() { Rgb = "FF660066" };
            RgbColor rgbColor22 = new RgbColor() { Rgb = "FFFF8080" };
            RgbColor rgbColor23 = new RgbColor() { Rgb = "FF0066CC" };
            RgbColor rgbColor24 = new RgbColor() { Rgb = "FFCCCCFF" };
            RgbColor rgbColor25 = new RgbColor() { Rgb = "FF000080" };
            RgbColor rgbColor26 = new RgbColor() { Rgb = "FFFF00FF" };
            RgbColor rgbColor27 = new RgbColor() { Rgb = "FFFFFF00" };
            RgbColor rgbColor28 = new RgbColor() { Rgb = "FF00FFFF" };
            RgbColor rgbColor29 = new RgbColor() { Rgb = "FF800080" };
            RgbColor rgbColor30 = new RgbColor() { Rgb = "FF800000" };
            RgbColor rgbColor31 = new RgbColor() { Rgb = "FF008080" };
            RgbColor rgbColor32 = new RgbColor() { Rgb = "FF0000FF" };
            RgbColor rgbColor33 = new RgbColor() { Rgb = "FF00B0F0" };
            RgbColor rgbColor34 = new RgbColor() { Rgb = "FFCCFFFF" };
            RgbColor rgbColor35 = new RgbColor() { Rgb = "FFCCFFCC" };
            RgbColor rgbColor36 = new RgbColor() { Rgb = "FFFFFF99" };
            RgbColor rgbColor37 = new RgbColor() { Rgb = "FF99CCFF" };
            RgbColor rgbColor38 = new RgbColor() { Rgb = "FFFF99CC" };
            RgbColor rgbColor39 = new RgbColor() { Rgb = "FFCC99FF" };
            RgbColor rgbColor40 = new RgbColor() { Rgb = "FFFFCC99" };
            RgbColor rgbColor41 = new RgbColor() { Rgb = "FF3366FF" };
            RgbColor rgbColor42 = new RgbColor() { Rgb = "FF33CCCC" };
            RgbColor rgbColor43 = new RgbColor() { Rgb = "FF99CC00" };
            RgbColor rgbColor44 = new RgbColor() { Rgb = "FFFFCC00" };
            RgbColor rgbColor45 = new RgbColor() { Rgb = "FFFF9900" };
            RgbColor rgbColor46 = new RgbColor() { Rgb = "FFFF6600" };
            RgbColor rgbColor47 = new RgbColor() { Rgb = "FF666699" };
            RgbColor rgbColor48 = new RgbColor() { Rgb = "FF969696" };
            RgbColor rgbColor49 = new RgbColor() { Rgb = "FF003366" };
            RgbColor rgbColor50 = new RgbColor() { Rgb = "FF339966" };
            RgbColor rgbColor51 = new RgbColor() { Rgb = "FF003300" };
            RgbColor rgbColor52 = new RgbColor() { Rgb = "FF333300" };
            RgbColor rgbColor53 = new RgbColor() { Rgb = "FF993300" };
            RgbColor rgbColor54 = new RgbColor() { Rgb = "FF993366" };
            RgbColor rgbColor55 = new RgbColor() { Rgb = "FF333399" };
            RgbColor rgbColor56 = new RgbColor() { Rgb = "FF333333" };

            indexedColors1.Append(rgbColor1);
            indexedColors1.Append(rgbColor2);
            indexedColors1.Append(rgbColor3);
            indexedColors1.Append(rgbColor4);
            indexedColors1.Append(rgbColor5);
            indexedColors1.Append(rgbColor6);
            indexedColors1.Append(rgbColor7);
            indexedColors1.Append(rgbColor8);
            indexedColors1.Append(rgbColor9);
            indexedColors1.Append(rgbColor10);
            indexedColors1.Append(rgbColor11);
            indexedColors1.Append(rgbColor12);
            indexedColors1.Append(rgbColor13);
            indexedColors1.Append(rgbColor14);
            indexedColors1.Append(rgbColor15);
            indexedColors1.Append(rgbColor16);
            indexedColors1.Append(rgbColor17);
            indexedColors1.Append(rgbColor18);
            indexedColors1.Append(rgbColor19);
            indexedColors1.Append(rgbColor20);
            indexedColors1.Append(rgbColor21);
            indexedColors1.Append(rgbColor22);
            indexedColors1.Append(rgbColor23);
            indexedColors1.Append(rgbColor24);
            indexedColors1.Append(rgbColor25);
            indexedColors1.Append(rgbColor26);
            indexedColors1.Append(rgbColor27);
            indexedColors1.Append(rgbColor28);
            indexedColors1.Append(rgbColor29);
            indexedColors1.Append(rgbColor30);
            indexedColors1.Append(rgbColor31);
            indexedColors1.Append(rgbColor32);
            indexedColors1.Append(rgbColor33);
            indexedColors1.Append(rgbColor34);
            indexedColors1.Append(rgbColor35);
            indexedColors1.Append(rgbColor36);
            indexedColors1.Append(rgbColor37);
            indexedColors1.Append(rgbColor38);
            indexedColors1.Append(rgbColor39);
            indexedColors1.Append(rgbColor40);
            indexedColors1.Append(rgbColor41);
            indexedColors1.Append(rgbColor42);
            indexedColors1.Append(rgbColor43);
            indexedColors1.Append(rgbColor44);
            indexedColors1.Append(rgbColor45);
            indexedColors1.Append(rgbColor46);
            indexedColors1.Append(rgbColor47);
            indexedColors1.Append(rgbColor48);
            indexedColors1.Append(rgbColor49);
            indexedColors1.Append(rgbColor50);
            indexedColors1.Append(rgbColor51);
            indexedColors1.Append(rgbColor52);
            indexedColors1.Append(rgbColor53);
            indexedColors1.Append(rgbColor54);
            indexedColors1.Append(rgbColor55);
            indexedColors1.Append(rgbColor56);

            colors1.Append(indexedColors1);

            stylesheet1.Append(numberingFormats1);
            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(colors1);

            workbookStylesPart1.Stylesheet = stylesheet1;
        }


        // Generates content of worksheetPart1.
        private void GenerateWorksheetPart1Content(WorksheetPart worksheetPart1, DataTable dt)
        {
            Worksheet worksheet1 = new Worksheet();
            worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetProperties sheetProperties1 = new SheetProperties() { FilterMode = false };
            PageSetupProperties pageSetupProperties1 = new PageSetupProperties() { FitToPage = false };

            sheetProperties1.Append(pageSetupProperties1);
            SheetDimension sheetDimension1 = new SheetDimension() { Reference = "A1:P4" };

            SheetViews sheetViews1 = new SheetViews();
            Columns columns1 = GetColumns(dt.Columns.Count);
            SheetView sheetView1 = new SheetView() { ShowFormulas = false, ShowGridLines = true, ShowRowColHeaders = true, ShowZeros = true, RightToLeft = true, TabSelected = true, ShowOutlineSymbols = true, DefaultGridColor = true, View = SheetViewValues.Normal, TopLeftCell = "A1", ColorId = (UInt32Value)64U, ZoomScale = (UInt32Value)100U, ZoomScaleNormal = (UInt32Value)100U, ZoomScalePageLayoutView = (UInt32Value)100U, WorkbookViewId = (UInt32Value)0U };
            Pane pane1 = new Pane() { HorizontalSplit = 0D, VerticalSplit = 1D, TopLeftCell = "A2", ActivePane = PaneValues.BottomLeft, State = PaneStateValues.Frozen };
            Selection selection1 = new Selection() { Pane = PaneValues.TopLeft, ActiveCell = "A2", ActiveCellId = (UInt32Value)0U, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "A1" } };
            Selection selection2 = new Selection() { Pane = PaneValues.BottomLeft, ActiveCell = "D11", ActiveCellId = (UInt32Value)0U, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "D11" } };

            sheetView1.Append(pane1);
            sheetView1.Append(selection1);
            sheetView1.Append(selection2);

            sheetViews1.Append(sheetView1);
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 22.8D, ZeroHeight = false, OutlineLevelRow = 0, OutlineLevelColumn = 0, DefaultColumnWidth = 30D };

            PrintOptions printOptions1 = new PrintOptions() { HorizontalCentered = false, VerticalCentered = false, Headings = false, GridLines = false, GridLinesSet = true };
            PageMargins pageMargins1 = new PageMargins() { Left = 0.7875D, Right = 0.7875D, Top = 1.05277777777778D, Bottom = 1.05277777777778D, Header = 0.7875D, Footer = 0.7875D };
            PageSetup pageSetup1 = new PageSetup() { PaperSize = (UInt32Value)1U, Scale = (UInt32Value)100U, FirstPageNumber = (UInt32Value)1U, FitToWidth = (UInt32Value)1U, FitToHeight = (UInt32Value)1U, PageOrder = PageOrderValues.DownThenOver, Orientation = OrientationValues.Portrait, BlackAndWhite = false, Draft = false, CellComments = CellCommentsValues.None, UseFirstPageNumber = true, HorizontalDpi = (UInt32Value)300U, VerticalDpi = (UInt32Value)300U, Copies = (UInt32Value)1U };

            HeaderFooter headerFooter1 = new HeaderFooter() { DifferentOddEven = false, DifferentFirst = false };
            OddHeader oddHeader1 = new OddHeader();
            oddHeader1.Text = "&C&\"Times New Roman,Regular\"&12&A";
            OddFooter oddFooter1 = new OddFooter();
            oddFooter1.Text = "&C&\"Times New Roman,Regular\"&12Page &P";

            headerFooter1.Append(oddHeader1);
            headerFooter1.Append(oddFooter1);
            worksheet1.Append(sheetProperties1);
            worksheet1.Append(columns1);
            worksheet1.Append(sheetDimension1);
            worksheet1.Append(sheetViews1);
            worksheet1.Append(sheetFormatProperties1);

            worksheet1.Append(printOptions1);
            worksheet1.Append(pageMargins1);
            worksheet1.Append(pageSetup1);
            worksheet1.Append(headerFooter1);

            worksheetPart1.Worksheet = worksheet1;

        }
    }
}
