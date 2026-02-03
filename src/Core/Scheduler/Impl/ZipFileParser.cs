using Core.Application;
using Core.Application.Attribute;
using Core.Scheduler.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Scheduler.Impl
{
    [DefaultImplementation]
    class ZipFileParser : IZipFileParser
    {
        private ILogService _logService;

        public ZipFileParser(IUnitOfWork unitOfWork, ILogService logService, ISettings settings, IClock clock, IJobFactory jobFactory)
        {
            _logService = logService;
        }

        object directionCode = default(object);
        object zipCode = default(object);
        private Dictionary<string, int> _headersDictionary = new Dictionary<string, int>();
        private List<CountyCreateEditModel> _countyModelCollection = new List<CountyCreateEditModel>();
        private List<ZipCreateEditModel> _zipModelCollection = new List<ZipCreateEditModel>();

        public IList<CountyCreateEditModel> PrepareDomainFromDataTableForCounty(DataTable dt)
        {
            PrepareHeaderIndex(dt);
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    directionCode = row.ItemArray[7];
                    ParseRow(row, "id", true);
                }
                catch (Exception e1)
                {
                    _logService.Error("Error in adding DirectionCode " + directionCode.ToString(), e1);
                }
            }
            return _countyModelCollection;
        }

        public IList<ZipCreateEditModel> PrepareDomainFromDataTableForZip(DataTable dt)
        {
            PrepareHeaderIndex(dt);
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    zipCode = row.ItemArray[2];
                    ParseRow(row, "id", false);
                }
                catch (Exception e1)
                {
                    _logService.Error("Error in reading ZipCode " + zipCode.ToString(), e1);
                }
            }
            return _zipModelCollection;
        }
        private void PrepareHeaderIndex(DataTable dt)
        {
            int index = 0;
            foreach (var col in dt.Columns)
            {
                if (col != null && col.ToString().Length > 0)
                {
                    _headersDictionary.Add(col.ToString(), index);
                }
                index++;
            }
        }

        private void ParseRow(DataRow row, string value, bool isCounty)
        {
            var dataInColType = ReadValueAsStringFromRowItem(row, value);
            if (dataInColType == null || string.IsNullOrWhiteSpace(dataInColType.ToString()))
            {
                return;
            }

            CreateModels(row, isCounty);
        }

        private string ReadValueAsStringFromRowItem(DataRow row, string key)
        {
            key = key.ToLower();
            if (_headersDictionary.ContainsKey(key) == false) return null;
            var value = row.ItemArray[_headersDictionary[key]].ToString().Trim();

            if (!string.IsNullOrEmpty(value))
                return value.Trim();

            return null;
        }

        private void CreateModels(DataRow row, bool isCounty)
        {
            if (isCounty)
            {
                CountyCreateEditModel customerModel = new CountyCreateEditModel();
                customerModel = PrepareDataModel(row, customerModel);
            }
            else
            {
                ZipCreateEditModel zipModel = new ZipCreateEditModel();
                zipModel = PrepareDataModel(row, zipModel);
            }
        }

        private CountyCreateEditModel PrepareDataModel(DataRow row, CountyCreateEditModel countyModel)
        {
            countyModel = CreateCountyModel(row);
            _countyModelCollection.Add(countyModel);
            return countyModel;
        }

        private ZipCreateEditModel PrepareDataModel(DataRow row, ZipCreateEditModel zipModel)
        {
            zipModel = CreateCountyModelForZip(row);
            _zipModelCollection.Add(zipModel);
            return zipModel;
        }

        private CountyCreateEditModel CreateCountyModel(DataRow row)
        {
            var model = new CountyCreateEditModel();
            var id = ReadValueAsStringFromRowItem(row, "id");
            model.Id = Convert.ToInt64(id);
            var countryName = ReadValueAsStringFromRowItem(row, "Country");
            model.CountryName = countryName;
            var isUpdated = ReadValueAsStringFromRowItem(row, "IsUpdated");
            model.IsUpdated = Convert.ToInt32(isUpdated);
            var stateCode = ReadValueAsStringFromRowItem(row, "State / Province");
            model.StateCode = stateCode;
            var countyName = ReadValueAsStringFromRowItem(row, "County / Municipality");
            model.CountyName = countyName != null ? countyName.ToUpper() : null;
            var stateCountryCode = ReadValueAsStringFromRowItem(row, "State-County");
            model.StateCountryCode = stateCountryCode;
            var franchisee = ReadValueAsStringFromRowItem(row, "TAAZAA FRANCHSE");
            model.FranchiseeName = franchisee;
            var directionCode = ReadValueAsStringFromRowItem(row, "Direction Code");
            model.DirectionCode = directionCode;
            var territoryCode = ReadValueAsStringFromRowItem(row, "State/TERRITORY");
            model.TerritoryCode = territoryCode;
            var directionFromOffice = ReadValueAsStringFromRowItem(row, "Direction from office");
            model.DirectionFromOffice = directionFromOffice;
            var time = ReadValueAsStringFromRowItem(row, "Time (HR)");
            model.ReachingTime = time;
            var population = ReadValueAsStringFromRowItem(row, "Population (,000)");
            model.Population = Convert.ToDecimal(population);
            var contractedTerritory = ReadValueAsStringFromRowItem(row, "CONTRACTED TERRITORY");
            model.ContractedTerritory = contractedTerritory;
            var coveringLessThan3Hours = ReadValueAsStringFromRowItem(row, "COVERING (<2.5 HR)");
            model.CoveringLessThan3Hours = coveringLessThan3Hours;
            var dayTrip = ReadValueAsStringFromRowItem(row, "DAY TRIP (2.5 TO 4 HR)");
            model.DayTrip = dayTrip;
            var unCovered = ReadValueAsStringFromRowItem(row, "UNCOVERED (OVER 4 HRS)");
            model.UnCovered = unCovered;
            var status = ReadValueAsStringFromRowItem(row, "STATUS");
            model.Status = (status);
            var franchiseeMLD = ReadValueAsStringFromRowItem(row, "Franchise- MLD");
            //model.FranchiseMLD = franchisee;
            var isDeleted = ReadValueAsStringFromRowItem(row, "IsDeleted");
            model.IsDeleted = Convert.ToInt32(isDeleted);
            return model;
        }
        private ZipCreateEditModel CreateCountyModelForZip(DataRow row)
        {
            var model = new ZipCreateEditModel();
            var id = ReadValueAsStringFromRowItem(row, "Id");
            model.Id = Convert.ToInt64(id);
            //var isUpdated = ReadValueAsStringFromRowItem(row, "IsUpdated");
            //model.IsUpdated = Convert.ToInt32(isUpdated);
            var zipCode = ReadValueAsStringFromRowItem(row, "Zip Code");
            model.ZipCode = zipCode;
            var stateCode = ReadValueAsStringFromRowItem(row, "State / Prov");
            model.StateCode = stateCode;
            var city = ReadValueAsStringFromRowItem(row, "City");
            model.CityName = city;
            var countyName = ReadValueAsStringFromRowItem(row, "County /  Municipality");
            model.countyName = countyName != null ? countyName.ToUpper() : null;
            var areaCode = ReadValueAsStringFromRowItem(row, "Area Code");
            if (areaCode == null)
            {
                areaCode = "";
            }
            model.AreaCode = areaCode;
            var dir = ReadValueAsStringFromRowItem(row, "DIR");
            model.Dir = dir;
            model.DriveTest = "";

            var code = ReadValueAsStringFromRowItem(row, "CODE");
            model.Code = code;
            var isDeleted = ReadValueAsStringFromRowItem(row, "isDeleted");
            model.IsDeleted = Convert.ToInt32(isDeleted);
            var transferableNumber = ReadValueAsStringFromRowItem(row, "franchisee transferable number");
            model.FranchiseeTransferableNumber = transferableNumber;
            return model;
        }

        private bool CheckIfRowHasValidData(DataRow row)
        {
            var dataInColType = ReadValueAsStringFromRowItem(row, "id");
            var dataInColTypeNum = ReadValueAsStringFromRowItem(row, "county/municipality");

            if (string.IsNullOrWhiteSpace(dataInColType) && string.IsNullOrWhiteSpace(dataInColTypeNum))
            {
                return false;
            }
            return true;
        }
        public bool CheckForValidCountyHeader(DataTable dt, out string message)
        {
            //var isBlankFile = CheckForBlankFile(dt);
            //if (isBlankFile)
            //{
            //    message = "File has no Valid Data.";
            //    return true;
            //}

            const string Id = "id";
            const string Country = "country";
            const string StateProvince = "state/province";

            const string County = "county/municipality";
            const string TaazaaFranchisee = "taazaafranchse";

            const string TAAZAAFRANCHSEEmail = "taazaafranchseemail";
            const string DirectionFromOffice = "directionfromoffice";
            const string Population = "population(,000)";
            //const string FranchiseMLD = "franchise- mld";
            const string CONTRACTEDTERRITORY = "contractedterritory";
            const string UNCOVERED = "uncovered(over4hrs)";
            const string IsDeleted = "isdeleted";

            const string DayTrip = "daytrip(2.5to4hr)";
            const string StateCountry = "state-country";
            const string TransferableNumber = "taazaafranchsetransferablenumber";
            const string Covering = "covering(<2.5hr)";
            const string Status = "status";
            const string Time = "time(hr)";

            const string StateTeritory = "state/territory";
            const string DirectionCode = "directioncode";
            const string FracnhiseeMLD = "franchise-mld";
            const string StateCounty = "state-county";




            var isIdPresent = false;
            var isCountrypresent = false;
            var isCountypresent = false;
            var isStateProvincePresent = false;
            var isTaazaaFranchiseePresent = false;
            var isTAAZAAFRANCHSEEmailPresent = false;
            var isDirectionFromOfficePresent = false;
            var isPopulationPresent = false;
            var isFranchiseMLDPresent = false;
            var isCONTRACTEDTERRITORYPresent = false;
            var isUNCOVEREDPresent = false;
            var isIsDeletedPresent = false;

            var isDayTripPresent = false;
            var isStateCountryPresent = false;
            var isTransferableNumberPresent = false;
            var isCoveringPresent = false;
            var isStatusPresent = false;
            var isTimePresent = false;

            var isStateTeritoryPresent = false;
            var isDirectionCodePresent = false;
            var isFracnhiseeMLDPresent = false;
            var isStateCountyPresent = false;
            for (int i = 0; i < dt.Columns.Count; i++)
            {

                var caption = dt.Columns[i].Caption;
                caption = caption.Replace(" ", string.Empty);
                if (caption.Equals(Id))
                    isIdPresent = true;

                else if (caption.Equals(Country))
                    isCountrypresent = true;
                else if (caption.Equals(County))
                    isCountypresent = true;
                else if (caption.Equals(StateProvince))
                    isStateProvincePresent = true;
                else if (caption.Equals(TaazaaFranchisee))
                    isTaazaaFranchiseePresent = true;
                else if (caption.Equals(TAAZAAFRANCHSEEmail))
                    isTAAZAAFRANCHSEEmailPresent = true;
                else if (caption.Equals(DirectionFromOffice))
                    isDirectionFromOfficePresent = true;
                else if (caption.Equals(Population))
                    isPopulationPresent = true;
                //else if (caption.Equals(FranchiseMLD))
                //    isFranchiseMLDPresent = true;
                else if (caption.Equals(UNCOVERED))
                    isUNCOVEREDPresent = true;
                else if (caption.Equals(IsDeleted))
                    isIsDeletedPresent = true;
                else if (caption.Equals(CONTRACTEDTERRITORY))
                    isCONTRACTEDTERRITORYPresent = true;
                else if (caption.Equals(DayTrip))
                    isDayTripPresent = true;
                else if (caption.Equals(StateCountry))
                    isStateCountryPresent = true;
                else if (caption.Equals(TransferableNumber))
                    isTransferableNumberPresent = true;
                else if (caption.Equals(Covering))
                    isCoveringPresent = true;
                else if (caption.Equals(Status))
                    isStatusPresent = true;
                else if (caption.Equals(Time))
                    isTimePresent = true;
                else if (caption.Equals(StateTeritory))
                    isStateTeritoryPresent = true;
                else if (caption.Equals(DirectionCode))
                    isDirectionCodePresent = true;
                else if (caption.Equals(FracnhiseeMLD))
                    isFracnhiseeMLDPresent = true;
                else if (caption.Equals(StateCounty))
                    isStateCountyPresent = true;
            }



            if (!isIdPresent)
            {
                message = "Columns for 'id'  is missing in County tab.";
                return false;
            }
            if (!isCountrypresent)
            {
                message = "Columns for 'Country'  is missing in County tab.";
                return false;
            }
            if (!isStateProvincePresent)
            {
                message = "Columns for 'State / Province'  is missing in County tab.";
                return false;
            }
            if (!isCountypresent)
            {
                message = "Columns for 'County'  is missing in County tab.";
                return false;
            }
            if (!isTaazaaFranchiseePresent)
            {
                message = "Columns for 'Taazaa Franchisee'  is missing in County tab.";
                return false;
            }
            //if (!isTAAZAAFRANCHSEEmailPresent)
            //{
            //    message = "Columns for 'Taazaa Franchisee Email'  is missing.";
            //    return false;
            //}
            if (!isDirectionFromOfficePresent)
            {
                message = "Columns for 'Direction From Office'  is missing in County tab.";
                return false;
            }
            if (!isPopulationPresent)
            {
                message = "Columns for 'Population'  is missing in County tab.";
                return false;
            }
            //if (!isFranchiseMLDPresent)
            //{
            //    message = "Columns for 'Franchisee MLD'  is missing.";
            //    return false;
            //}
            if (!isCONTRACTEDTERRITORYPresent)
            {
                message = "Columns for 'Contracted Territory'  is missing in County tab.";
                return false;
            }
            if (!isUNCOVEREDPresent)
            {
                message = "Columns for 'Uncovered'  is missing in County tab.";
                return false;
            }


            if (!isIsDeletedPresent)
            {
                message = "Columns for 'IsDeleted'  is missing in County tab.";
                return false;
            }

            if (!isDayTripPresent)
            {
                message = "Columns for 'Day Trip'  is missing in County tab.";
                return false;
            }

            //if (!isStateCountryPresent)
            //{
            //    message = "Columns for 'State-County'  is missing.";
            //    return false;
            //}

            if (!isCoveringPresent)
            {
                message = "Columns for 'COVERING (<2.5 HR)'  is missing in County tab.";
                return false;
            }
            //if (!isTransferableNumberPresent)
            //{
            //    message = "Columns for 'TAAZAA FRANCHSE Transferable Number'  is missing.";
            //    return false;
            //}



            if (!isStatusPresent)
            {
                message = "Columns for 'Status'  is missing in County tab.";
                return false;
            }
            if (!isTimePresent)
            {
                message = "Columns for 'Time(HR)'  is missing in County tab.";
                return false;
            }

            if (!isStateTeritoryPresent)
            {
                message = "Columns for 'State/TERRITORY'  is missing in County tab.";
                return false;
            }
            if (!isDirectionCodePresent)
            {
                message = "Columns for 'Direction Code'  is missing in County tab.";
                return false;
            }

            //if (!isFracnhiseeMLDPresent)
            //{
            //    message = "Columns for 'Franchisee MLD'  is missing.";
            //    return false;
            //}
            if (!isStateCountyPresent)
            {
                message = "Columns for 'State-County'  is missing in County tab.";
                return false;
            }
            message = "---";
            return true;
        }


        public bool CheckForValidZipHeader(DataTable dt, out string message)
        {
            //var isBlankFile = CheckForBlankFile(dt);
            //if (isBlankFile)
            //{
            //    message = "File has no Valid Data.";
            //    return true;
            //}

            const string Id = "id";
            const string ZipCode = "zipcode";
            const string County = "county/municipality";
            const string IsDeleted = "isdeleted";
            const string Code = "code";
            const string Dir = "dir";
            const string City = "city";
            const string AreaCode = "areacode";
            const string StateProv = "state/prov";

            var isIdPresent = false;
            var isCodePresent = false;
            var isZipCodePresent = false;
            var isCountyPresent = false;
            var isDeletedPresent = false;
            var isDirPresent = false;
            var isCityPresent = false;
            var isAreaCodePresent = false;
            var isStateProvePresent = false;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var caption = dt.Columns[i].Caption.ToLower();
                caption = caption.Replace(" ", string.Empty);
                if (caption.Equals(Id))
                    isIdPresent = true;
                else if (caption.Equals(County))
                    isCountyPresent = true;
                else if (caption.Equals(IsDeleted))
                    isDeletedPresent = true;
                else if (caption.Equals(ZipCode))
                    isZipCodePresent = true;
                else if (caption.Equals(Code))
                    isCodePresent = true;
                else if (caption.Equals(Dir))
                    isDirPresent = true;
                else if (caption.Equals(City))
                    isCityPresent = true;
                else if (caption.Equals(AreaCode))
                    isAreaCodePresent = true;
                else if (caption.Equals(StateProv))
                    isStateProvePresent = true;
            }

            if (!isStateProvePresent)
            {
                message = "Column for 'State/Prov'  is missing in Zip Code tab.";
                return false;
            }
            if (!isAreaCodePresent)
            {
                message = "Column for 'Area Code'  is missing in Zip Code tab.";
                return false;
            }
            if (!isCityPresent)
            {
                message = "Column for 'City'  is missing in Zip Code tab.";
                return false;
            }
            if (!isDirPresent)
            {
                message = "Column for 'Dir'  is missing in Zip Code tab.";
                return false;
            }
            if (!isIdPresent)
            {
                message = "Column for 'id'  is missing in Zip Code tab.";
                return false;
            }
            if (!isCodePresent)
            {
                message = "Column for 'Code'  is missing in Zip Code tab.";
                return false;
            }
            if (!isCountyPresent)
            {
                message = "Column for 'County'  is missing in Zip Code tab.";
                return false;
            }
            if (!isDeletedPresent)
            {
                message = "Column for 'IsDeleted'  is missing in Zip Code tab.";
                return false;
            }
            if (!isZipCodePresent)
            {
                message = "Column for 'Zip Code'  is missing in Zip Code tab.";
                return false;
            }
            message = "---";
            return true;
        }

        public bool CheckForBlankFile(DataTable dt)
        {
            var validCount = 0;
            foreach (DataRow row in dt.Rows)
            {
                var result = CheckIfRowHasValidData(row);
                if (result)
                {
                    validCount++;
                }
            }
            if (validCount > 0)
            {
                return false;
            }
            return true;
        }
    }
}
