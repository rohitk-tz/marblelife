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

namespace SixBar.Core.Application.Impl
{
    [DefaultImplementation]
    public class ExcelFileCreatorMarketingLead : IExcelFileCreatorMarketingLead
    {
        public bool CreateExcelDocument(List<CallDetailViewModel> list, string xlsxFilePath, List<string> columnsName)
        {
            var ds = new DataSet();
            ds.Tables.Add(ListToDataTable(columnsName, list));

            return CreateExcelDocument(ds, xlsxFilePath);
        }

        #region HELPER_FUNCTIONS

        public DataTable ListToDataTable(List<string> columnList, List<CallDetailViewModel> list, string tableName = "")
        {
            var dtCol = new DataTable();


            DataTable dt = SetColumns(dtCol, columnList);

            AddRows(list, dt, columnList);

            if (!string.IsNullOrEmpty(tableName))
                dt.TableName = tableName;

            return dt;
        }

        private static void AddRows(List<CallDetailViewModel> list, DataTable dt, List<string> downloadColumnsName)
        {
            foreach (var t in list)
            {
                DataRow row = dt.NewRow();
                SetValueFromInstanceInDataRow(t, downloadColumnsName, row, dt);
                dt.Rows.Add(row);
            }
        }
        private static void SetValueFromInstanceInDataRow(CallDetailViewModel model, List<string> downloadColumnsName, DataRow row, DataTable dt)
        {
            var typeofModel = model.GetType();
            var column = dt.Columns[0].ToString();
            foreach (var columnsNames in dt.Columns)
            {
                var columnsName = columnsNames.ToString();
                if (columnsName == "Id")
                {
                    row[columnsName] = model.Id;
                }
                else if (columnsName == "Date/Time Of Call")
                {
                    row[columnsName] = model.DateOfCall;
                }
                else if (columnsName == "Dialed Number (dnis)")
                {
                    //row[columnsName] = model.Dnis;
                    long dnisValue;

                    if (!string.IsNullOrWhiteSpace(model.Dnis) && long.TryParse(model.Dnis, out dnisValue))
                        row[columnsName] = dnisValue;
                    else
                        row[columnsName] = DBNull.Value;
                }
                else if (columnsName == "Caller ID (ani)")
                {
                    row[columnsName] = model.Ani;
                }
                else if (columnsName == "Transfer To Number")
                {
                    row[columnsName] = model.TransferToNumber;
                }
                else if (columnsName == "Phone Label")
                {
                    row[columnsName] = model.PhoneLabel;
                }
                else if (columnsName == "Ring Seconds")
                {
                    row[columnsName] = model.RingSeconds_CallFlow.GetValueOrDefault(); ;
                }
                else if (columnsName == "Ring Count")
                {
                    if (model.RingCount_CallFlow != null)
                        row[columnsName] = model.RingCount_CallFlow;
                    else
                        row[columnsName] = string.Empty;
                }
                else if (columnsName == "Recorded Seconds")
                {
                    row[columnsName] = model.RecordedSeconds_Recording;
                }
                else if (columnsName == "Recording Url")
                {
                    row[columnsName] = model.RecordingUrl_Recording;
                }
                else if (columnsName == "Call Duration")
                {
                    row[columnsName] = model.CallDuration;
                }
                else if (columnsName == "Call Route(Mapped By ZipCode)")
                {
                    row[columnsName] = model.CallRoute;
                }
                else if (columnsName == "Franchisee(Invoca API)")
                {
                    row[columnsName] = model.Franchisee;
                }
                else if (columnsName == "Call Flow Entered Zip")
                {
                    row[columnsName] = model.CallFlowEnteredZip;
                }
                else if (columnsName == "Preferred Contact Number")
                {
                    row[columnsName] = model.PreferredContactNumber;
                }
                else if (columnsName == "Email")
                {
                    row[columnsName] = model.Email;
                }
                else if (columnsName == "First Name")
                {
                    row[columnsName] = model.FirstName;
                }
                else if (columnsName == "Last Name")
                {
                    row[columnsName] = model.LastName;
                }
                else if (columnsName == "Company")
                {
                    row[columnsName] = model.Company;
                }
                else if (columnsName == "Office")
                {
                    row[columnsName] = model.Office;
                }
                else if (columnsName == "Zip Code")
                {
                    row[columnsName] = model.ZipCode;
                }
                else if (columnsName == "Resulting Action")
                {
                    row[columnsName] = model.ResultingAction;
                }
                else if (columnsName == "Number")
                {
                    row[columnsName] = model.HouseNumber;
                }
                else if (columnsName == "Street")
                {
                    row[columnsName] = model.Street;
                }
                else if (columnsName == "City")
                {
                    row[columnsName] = model.City;
                }
                else if (columnsName == "State")
                {
                    row[columnsName] = model.State;
                }
                else if (columnsName == "Country")
                {
                    row[columnsName] = model.Country;
                }
                else if (columnsName == "Call Note")
                {
                    row[columnsName] = model.CallNote;
                }
                else if (columnsName == "Transcription Status")
                {
                    row[columnsName] = model.TranscriptionStatus_CallAnalytics;
                }
                else if (columnsName == "Missed Call")
                {
                    row[columnsName] = model.MissedCall_CallMetrics;
                }
                else if (columnsName == "Find Me List")
                {
                    row[columnsName] = model.FindMeList;
                }
                else if (columnsName == "Keyword Groups")
                {
                    row[columnsName] = model.KeywordGroups_CallAnalytics;
                }
                else if (columnsName == "Keyword Spotting Complete")
                {
                    row[columnsName] = model.KeywordSpottingComplete_CallAnalytics;
                }
                else if (columnsName == "Campaign")
                {
                    row[columnsName] = model.Campaign_VisitorData;
                }
                else if (columnsName == "Campaign Id")
                {
                    row[columnsName] = model.CampaignId_PaidSearch;
                }
                else if (columnsName == "Ad Group")
                {
                    row[columnsName] = model.Adgroup_PaidSearch;
                }
                else if (columnsName == "Ad Group Id")
                {
                    row[columnsName] = model.AdgroupId_PaidSearch;
                }
                else if (columnsName == "Ads")
                {
                    row[columnsName] = model.Ads_PaidSearch;
                }
                else if (columnsName == "Ad Id")
                {
                    row[columnsName] = model.AdId_PaidSearch;
                }
                else if (columnsName == "Keywords")
                {
                    row[columnsName] = model.Keywords_PaidSearch;
                }
                else if (columnsName == "Keywords Id")
                {
                    row[columnsName] = model.KeywordId_PaidSearch;
                }
                else if (columnsName == "Click Id")
                {
                    row[columnsName] = model.ClickId_PaidSearch;
                }
                else if (columnsName == "Keyword Match Type")
                {
                    row[columnsName] = model.KeyWordMatchType_PaidSearch;
                }
                else if (columnsName == "CallInly Flag")
                {
                    row[columnsName] = model.CallInlyFlag_PaidSearch;
                }
                else if (columnsName == "Valid Call")
                {
                    row[columnsName] = model.ValidCall;
                }
                else if (columnsName == "Call Type")
                {
                    row[columnsName] = model.CallType;
                }
                else if (columnsName == "Tag")
                {
                    row[columnsName] = model.Tag;
                }
                else if (columnsName == "Call Flow Set Name")
                {
                    row[columnsName] = model.CallFlowSetName;
                }
                else if (columnsName == "Data From New API\t")
                {
                    row[columnsName] = model.DataFromNewAPI != null ? (model.DataFromNewAPI) == "Yes" ? "Yes" : "No" : "No";
                }
                else if (columnsName == "Call Flow Set Id")
                {
                    row[columnsName] = model.CallFlowSetId;
                }
                else if (columnsName == "Call Flow Destination")
                {
                    row[columnsName] = model.CallFlowDestination;
                }
                else if (columnsName == "Call Flow Destination Id")
                {
                    row[columnsName] = model.CallFlowDestinationId;
                }
                else if (columnsName == "Call Flow Reroute")
                {
                    row[columnsName] = model.CallFlowReroute;
                }
                else if (columnsName == "Call Transfer Type")
                {
                    row[columnsName] = model.TransferType;
                }
                else if (columnsName == "Invoice#")
                {
                    row[columnsName] = model.InvoiceId;
                }
                else if (columnsName == "Call Flow Source")
                {
                    row[columnsName] = model.CallFlowSource;
                }
                else if (columnsName == "Call Flow Source Id")
                {
                    row[columnsName] = model.CallFlowSourceId;
                }
                else if (columnsName == "Call Flow Source Qualified")
                {
                    row[columnsName] = model.CallFlowSourceQualified;
                }
                else if (columnsName == "Call Flow Repeat Source Caller")
                {
                    row[columnsName] = model.CallFlowRepeatSourceCaller;
                }
                else if (columnsName == "Call Flow Source Cap")
                {
                    row[columnsName] = model.CallFlowSourceCap;
                }
                else if (columnsName == "Call Flow Source Route")
                {
                    row[columnsName] = model.CallFlowSourceRoute;
                }
                else if (columnsName == "Call Flow Source Route Id")
                {
                    row[columnsName] = model.CallFlowSourceRouteId;
                }
                else if (columnsName == "Call Flow Source Route Qualified Id")
                {
                    row[columnsName] = model.CallFlowSourceRouteQualified;
                }
                else if (columnsName == "Call Flow State")
                {
                    row[columnsName] = model.CallFlowState;
                }
                else if (columnsName == "Transfer Type")
                {
                    row[columnsName] = model.TransferType_CallFlow;
                }
                else if (columnsName == "Call Transfer Status")
                {
                    row[columnsName] = model.CallTransferStatus_CallFlow;
                }
                else if (columnsName == "Dialog Analytics")
                {
                    row[columnsName] = model.DialogAnalytics_Recording;
                }
                else if (columnsName == "Geo Lookup Attempt")
                {
                    row[columnsName] = model.GeoLookupAttempt_ReverseLookUp;
                }
                else if (columnsName == "Geo Lookup Result")
                {
                    row[columnsName] = model.GeoLookupResult_ReverseLookUp;
                }
                else if (columnsName == "Call Activities")
                {
                    row[columnsName] = model.CallActivities;
                }
                else if (columnsName == "Channel")
                {
                    row[columnsName] = model.Channel_Attribution;
                }
                else if (columnsName == "Status")
                {
                    row[columnsName] = model.Status_Attribution;
                }
                else if (columnsName == "Rank")
                {
                    row[columnsName] = model.Rank_Attribution;
                }
                else if (columnsName == "PID")
                {
                    row[columnsName] = model.Pid_Attribution;
                }
                else if (columnsName == "BID")
                {
                    row[columnsName] = model.Bid_Attribution;
                }
                else if (columnsName == "First Touch Document Title")
                {
                    row[columnsName] = model.DocumentTitle_FirstTouch;
                }
                else if (columnsName == "First Touch Document URL")
                {
                    row[columnsName] = model.DocumentUrl_FirstTouch;
                }
                else if (columnsName == "First Touch Document Path")
                {
                    row[columnsName] = model.DocumentPath_FirstTouch;
                }
                else if (columnsName == "First Touch Document Time Stamp")
                {
                    row[columnsName] = model.DocumentTimeStamp_FirstTouch;
                }
                else if (columnsName == "Last Touch Document Title")
                {
                    row[columnsName] = model.DocumentTitle_LastTouch;
                }
                else if (columnsName == "Last Touch Document Url")
                {
                    row[columnsName] = model.DocumentUrl_LastTouch;
                }
                else if (columnsName == "Last Touch Document Path")
                {
                    row[columnsName] = model.DocumentPath_LastTouch;
                }
                else if (columnsName == "Last Touch Document Time Stamp")
                {
                    row[columnsName] = model.DocumentTimeStamp_LastTouch;
                }
                else if (columnsName == "IP Address")
                {
                    row[columnsName] = model.IPAddress_VisitorData;
                }
                else if (columnsName == "Device")
                {
                    row[columnsName] = model.Device_VisitorData;
                }
                else if (columnsName == "Browser")
                {
                    row[columnsName] = model.Browser_VisitorData;
                }
                else if (columnsName == "Browser Version")
                {
                    row[columnsName] = model.BrowserVersion_VisitorData;
                }
                else if (columnsName == "OS")
                {
                    row[columnsName] = model.Os_VisitorData;
                }
                else if (columnsName == "OS Version")
                {
                    row[columnsName] = model.OsVersion_VisitorData;
                }
                else if (columnsName == "Search Term")
                {
                    row[columnsName] = model.SearchTerm_VisitorData;
                }
                else if (columnsName == "Activity Value")
                {
                    row[columnsName] = model.ActivityValue_VisitorData;
                }
                else if (columnsName == "Activity Type Id")
                {
                    row[columnsName] = model.ActivityTypeId_VisitorData;
                }
                else if (columnsName == "Activity Keyword")
                {
                    row[columnsName] = model.ActivityKeyword_VisitorData;
                }
                else if (columnsName == "Activity Tag")
                {
                    row[columnsName] = model.ActivityTag_VisitorData;
                }
                else if (columnsName == "Platform")
                {
                    row[columnsName] = model.Platform_VisitorData;
                }
                else if (columnsName == "Source Guard")
                {
                    row[columnsName] = model.SourceGuard_VisitorData;
                }
                else if (columnsName == "Visitor Log Url")
                {
                    row[columnsName] = model.VisitorLogUrl_VisitorData;
                }
                else if (columnsName == "Google Ua Client Id")
                {
                    row[columnsName] = model.GoogleUaClientId_VisitorData;
                }
                else if (columnsName == "GCl Id")
                {
                    row[columnsName] = model.GClid_VisitorData;
                }
                else if (columnsName == "Utm Source")
                {
                    row[columnsName] = model.UtmSource_DefaultUtmParameters;
                }
                else if (columnsName == "Utm Medium")
                {
                    row[columnsName] = model.UtmMedium_DefaultUtmParameters;
                }
                else if (columnsName == "Utm Campaign")
                {
                    row[columnsName] = model.UtmCampaign_DefaultUtmParameters;
                }
                else if (columnsName == "Utm Term")
                {
                    row[columnsName] = model.UtmTerm_DefaultUtmParameters;
                }
                else if (columnsName == "Utm Content")
                {
                    row[columnsName] = model.UtmContent_DefaultUtmParameters;
                }
                else if (columnsName == "Vt Keyword")
                {
                    row[columnsName] = model.VtKeyword_ValueTrackParameters;
                }
                else if (columnsName == "Vt Match Type")
                {
                    row[columnsName] = model.VtMatchType_ValueTrackParameters;
                }
                else if (columnsName == "Vt Network")
                {
                    row[columnsName] = model.VtNetwork_ValueTrackParameters;
                }
                else if (columnsName == "Vt Device")
                {
                    row[columnsName] = model.VtDevice_ValueTrackParameters;
                }
                else if (columnsName == "Vt Device Model")
                {
                    row[columnsName] = model.VtDeviceModel_ValueTrackParameters;
                }
                else if (columnsName == "Vt Creative")
                {
                    row[columnsName] = model.VtCreative_ValueTrackParameters;
                }
                else if (columnsName == "Vt Placement")
                {
                    row[columnsName] = model.VtPlacement_ValueTrackParameters;
                }
                else if (columnsName == "Vt Target")
                {
                    row[columnsName] = model.VtTarget_ValueTrackParameters;
                }
                else if (columnsName == "Vt Param 1")
                {
                    row[columnsName] = model.VtParam1_ValueTrackParameters;
                }
                else if (columnsName == "Vt Param 2")
                {
                    row[columnsName] = model.VtParam2_ValueTrackParameters;
                }
                else if (columnsName == "Vt Random")
                {
                    row[columnsName] = model.VtRandom_ValueTrackParameters;
                }
                else if (columnsName == "Vt Ace Id")
                {
                    row[columnsName] = model.VtAceid_ValueTrackParameters;
                }
                else if (columnsName == "VtAd Position")
                {
                    row[columnsName] = model.VtAdposition_ValueTrackParameters;
                }
                else if (columnsName == "Vt Product Target Id")
                {
                    row[columnsName] = model.VtProductTargetId_ValueTrackParameters;
                }
                else if (columnsName == "VtAd Type")
                {
                    row[columnsName] = model.VtAdType_ValueTrackParameters;
                }
                else if (columnsName == "Domain Set Name")
                {
                    row[columnsName] = model.DomainSetName_SourceIqData;
                }
                else if (columnsName == "Domain Set Id")
                {
                    row[columnsName] = model.DomainSetId_SourceIqData;
                }
                else if (columnsName == "Pool Name")
                {
                    row[columnsName] = model.PoolName_SourceIqData;
                }
                else if (columnsName == "Location Name")
                {
                    row[columnsName] = model.LocationName_SourceIqData;
                }
                else if (columnsName == "Custom Value")
                {
                    row[columnsName] = model.CustomValue_SourceIqData;
                }
                else if (columnsName == "Custom Id")
                {
                    row[columnsName] = model.CustomId_SourceIqData;
                }

                else if (columnsName == "Type")
                {
                    row[columnsName] = model.Type_PaidSearch;
                }
                else if (columnsName == "Home Owner")
                {
                    row[columnsName] = model.HomeOwner;
                }
                else if (columnsName == "Home Market")
                {
                    row[columnsName] = model.HomeMarket;
                }


                //if (columnsName == "Data From Invoca")
                //{
                //    row[columnsName] = model.DataFromInvoca != null ? (model.DataFromInvoca) == "Yes" ? "Yes" : "No" : "No";
                //}

                //else if (columnsName == "Transfer To Number")
                //{
                //    row[columnsName] = model.TransferToNumber;
                //}
                //else if (columnsName == "Call Type Id")
                //{
                //    row[columnsName] = model.CallTypeId;
                //}
                //else if (columnsName == "Data From Invoca")
                //{
                //    row[columnsName] = model.DataFromInvoca;
                //}
                //else if (columnsName == "Transfer To Number")
                //{
                //    row[columnsName] = model.TransferToNumber;
                //}
                //else if (columnsName == "Recording Url")
                //{
                //    row[columnsName] = model.RecordingUrl_Recording;
                //}
                //else if (columnsName == "First Name")
                //{
                //    row[columnsName] = model.FirstName_ReverseLookUp;
                //}
                //else if (columnsName == "Last Name")
                //{
                //    row[columnsName] = model.LastName_ReverseLookUp;
                //}
                //else if (columnsName == "Street Line 1")
                //{
                //    row[columnsName] = model.StreetLine1_ReverseLookUp;
                //}
                //else if (columnsName == "City")
                //{
                //    row[columnsName] = model.City_ReverseLookUp;
                //}
                //else if (columnsName == "Postal Area")
                //{
                //    row[columnsName] = model.PostalArea_ReverseLookUp;
                //}
                //else if (columnsName == "State Code")
                //{
                //    row[columnsName] = model.StateCode_ReverseLookUp;
                //}
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
        private static DataTable SetColumns(DataTable dt, List<string> downloadColumn)
        {
            bool isExcel = false;
            var customColumnName = "";
            var typeOfObject = typeof(CallDetailViewModel);

            PropertyInfo[] propertyCollection = typeOfObject.GetProperties();
            foreach (var columnName in downloadColumn)
            {
                var dataType = typeof(String);
                customColumnName = columnName;
                var property = propertyCollection.FirstOrDefault(x => x.Name == columnName);
                if (property != null && property.CustomAttributes != null && property.CustomAttributes.Count() > 0)
                {
                    var customerAtrribute = property.CustomAttributes.ToList();
                    if (customerAtrribute.Count() > 0 && customerAtrribute[0].ConstructorArguments.Count() > 0)
                    {
                        customColumnName = customerAtrribute[0].ConstructorArguments[0].Value.ToString();
                        dataType = customerAtrribute[0].ConstructorArguments[0].ArgumentType;
                    }
                }
                if (customColumnName == "Id")
                {
                    dataType = typeof(int);
                }
                else if (customColumnName == "Date/Time Of Call")
                {
                    dataType = typeof(DateTime);
                }
                else if (customColumnName == "Ring Seconds")
                {
                    dataType = typeof(decimal);
                }
                else if (customColumnName == "Ring Count")
                {
                    dataType = typeof(string);
                }
                else if (customColumnName == "Dialed Number (dnis)")
                {
                    dataType = typeof(long);
                }

                dt.Columns.Add(new DataColumn(customColumnName, dataType));
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
    }
}
