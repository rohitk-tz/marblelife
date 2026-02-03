using Core.Application;
using Core.Application.Domain;
using Core.Application.ValueType;
using Core.Dashboard.Enum;
using Core.Geo.Domain;
using Core.Geo.Enum;
using Core.Geo.ViewModel;
using Core.Organizations.Enum;
using Core.Organizations.ViewModel;
using Core.Organizations.ViewModels;
using Core.Users.Enum;
using Core.Users.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using EnumServiceType = Core.Organizations.Enum.ServiceType;

namespace FranchiseeMigration
{
    public class FranchiseeFromFileCollection
    {
        static IList<State> states;
        private IRepository<State> _stateRepository;

        public FranchiseeFromFileCollection(IUnitOfWork unitOfWork)
        {
            _stateRepository = unitOfWork.Repository<State>();
        }

        public void FetchAllStataes()
        {
            states = _stateRepository.Table.ToArray();
        }

        public IList<FranchiseeDetailsModel> FranchiseesDetails()
        {
            FetchAllStataes();
            return new List<FranchiseeDetailsModel>()
            {
                CreateModel("SW Alabama", "swalabama@marblelife.com", "Marblelife21",  "Darryl" , "Proctor", "2057 Tujaques Place", "", "Pensacola" ,"32505", "AL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),
                        new Tuple<EnumServiceType, bool> (EnumServiceType.ColorSeal, true)),
                    Number(PhoneType.Office, "4848323333"), Number(PhoneType.Cell, "4844728329")),

                CreateModel("Arizona", "arizona@marblelife.com", "Marblelife22", "Jim", "Mannari", "1737 East Jackson St.", "", "Phoenix" ,"85034", "ARIZONA", null,
                    FeeProfile2(), Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4804833745"), Number(PhoneType.Fax, "4804834615") , Number(PhoneType.Cell, "4802259852")),

                CreateModel("Fresno", "Fresno@marblelife.com", "fresno@123", "Brian", "Jensen", "5502 West Mission Avenue", "Suite 101", "Fresno" ,"93772", "CALIFORNIA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Fabricators, true),new Tuple<EnumServiceType, bool>(EnumServiceType.TileInstall, true)),
                    Number(PhoneType.TollFree, "8776547688"), Number(PhoneType.Office, "5592760648") , Number(PhoneType.Fax, "5592763426"),
                     Number(PhoneType.Cell, "5599708827")),

                CreateModel("Los Angeles - North", "North-LA@marblelife.com", "Marblelife3", "Brian", "Jenson",  "1616 East Francis Street" , "Suite G", "Ontario", "91761" , "CALIFORNIA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Fabricators, true),new Tuple<EnumServiceType, bool>(EnumServiceType.TileInstall, true)),
                    Number(PhoneType.Office, "3104810606"),Number(PhoneType.Cell, "9518249388")),

                CreateModel("Sacramento", "sacramento@marblelife.com", "sacramento@123", "Brian", "Jensen", "5502 West Mission Avenue" , "Suite 101", "Fresno", "93772"   , "CALIFORNIA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Wood, true),new Tuple<EnumServiceType, bool>(EnumServiceType.TileInstall, true)),
                    Number(PhoneType.Office, "8885798786")),

                CreateModel("Los Angeles - South", "losangeles@marblelife.com", "Marblelife3", "Keith", "DeVries", "1616 East Francis Street", "Suite G",  "Ontario", "91761", "CALIFORNIA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "3104810606"),  Number(PhoneType.Fax, "9518249388")),

                CreateModel("San Diego", "sandiego@marblelife.com", "Marblelife5", "Jeff", "DeVries" , "19755 Scripts Poway Pky" , "Suite 452", "San Diego", "92131", "CALIFORNIA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "6193597898")),

                CreateModel("Orange County", "orangecounty@marblelife.com", "Marblelife24",  "Ken" , "Rogers",  "27665 Forbes Road" , "Suite 2", "Laguna Niguel", "92677", "CALIFORNIA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true), new Tuple<EnumServiceType, bool>(EnumServiceType.CarpetLife, true)),
                    Number(PhoneType.Office, "9495823277"), Number(PhoneType.Fax, "9493670633"), Number(PhoneType.Cell, "9492922897")),

                CreateModel("Connecticut", "connecticut@marblelife.com", "Marblelife13", "Dan", "Arsenault", "P.O. Box 67", "SHIP TO: 1 Checkerberry Lane", "Stevenson" , "06482", "CONNECTICUT", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true)),
                    Number(PhoneType.Office, "8774263811"), Number(PhoneType.Fax, "2034266086"), Number(PhoneType.Cell, "2037706411")),

                CreateModel("DC", "dc@marblelife.com", "dcWashington@123", "Freddy", "Castillo" ,"1330 7th st", "NW", "Washington", "20001", "DC", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Wood, true)),
                     Number(PhoneType.Cell, "2023049234")),

                CreateModel("Delaware Valley","delawarevalley1@marblelife.com", "Marblelife1", "Nick", "Danella", "579 Abbott Drive", "", "Broomall", "19008", "DE", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),
                        new Tuple<EnumServiceType, bool> (EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool> (EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool> (EnumServiceType.CleanShield, true)),
                    Number(PhoneType.Office, "4848323333"), Number(PhoneType.Cell, "4844728329")),

                 CreateModel("Tampa Bay", "tampa@marblelife.com", "Marblelife19",  "Chris", "Hudson", "4302 East 10th Av.", "Unit 205", "Tampa", "33605", "FL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.CarpetLife, true)),
                     Number(PhoneType.Office, "8136614866"), Number(PhoneType.Cell, "8138460895")),

                 CreateModel("Sarasota/Bradenton", "Bradenton@marblelife.com", "Marblelife28", "Chris", "Hudson",  "4302 East 10th Av.", "Unit 205", "Tampa",  "33605", "FL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                     Number(PhoneType.TollFree, "8005254707"), Number(PhoneType.Office, "9417566789"), Number(PhoneType.Fax, "8884512878")),

                 CreateModel("Southeast Florida", "seflorida@marblelife.com", "Marblelife9", "Angelo", "DeCapua",  "942 Clint", "Moore Road",  "Boca Raton","33487", "FL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.TileInstall, true)),
                     Number(PhoneType.Office, "9549848300"), Number(PhoneType.Office, "5613687607"), Number(PhoneType.Fax, "5613687609")
                     , Number(PhoneType.Office, "5617196803"), Number(PhoneType.Fax, "9545201093")),

                 CreateModel("Jacksonville", "neflorida@marblelife.com", "Marblelife26", "Wes", "Haigh", "9838 Old Baymeadows Rd.", "#247",  "Jacksonville", "32256", "FL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                     Number(PhoneType.Office, "9042962949"), Number(PhoneType.Cell, "9045090641"), Number(PhoneType.Cell, "9045246538")),

                 CreateModel("Orlando", "centralflorida@marblelife.com", "Marblelife25",  "Mike", "Freitag", "658 Douglas Avenue", "#1108", "Altamonte Springs", "32714", "FL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true)),
                     Number(PhoneType.Office, "4078621998"), Number(PhoneType.Fax, "4078622499"), Number(PhoneType.Cell, "4074669672")),

                  CreateModel("Northwest Florida", "nwflorida@marblelife.com", "Marblelife27", "Dave", "DeBlander", "3255 Potter Avenue", "", "Pensacola", "32514", "FL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.CarpetLife, true)),
                     Number(PhoneType.Office, "8504848500"), Number(PhoneType.Fax, "8504847337"), Number(PhoneType.Cell, "8507128711")),

                  CreateModel("Central Georgia", "centralgeorgia@marblelife.com", "Marblelife30", "Jamie", "Wheeler", "8076 Innisbrook Court", "", "Columbus", "31909", "GA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                     Number(PhoneType.Office, "7065874336"), Number(PhoneType.Cell, "7065276970")),

                   CreateModel("Atlanta", "atlanta@marblelife.com", "Marblelife17", "Bie", "Sunderland", "4235 Steve Reynolds Blvd.", "Suite B", "Norcross", "30093", "GA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true)),
                     Number(PhoneType.Office, "7707178100"), Number(PhoneType.Fax, "7707178822")),

                    CreateModel("Chicago-North", "northernchicago@marblelife.com", "Marblelife16",  "John", "Kniebusch", "1935 Brandon Court", "Suite A", "Glendale Heights", "60139", "IL", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                        , new Tuple<EnumServiceType, bool>(EnumServiceType.Wood, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Cleanair, true)),
                     Number(PhoneType.Fax, "6309298006"), Number(PhoneType.Cell, "6308547659")),

                    CreateModel("Louisville-Central", "centralkentucky@marblelife.com", "Marblelife31",  "Eddie", "Miller",  "106 Pike Street", "",  "Bromley",  "41016", "KY", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true)
                        , new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                     Number(PhoneType.Office, "5022637890"), Number(PhoneType.Fax, "5024565961")),

                    CreateModel("Cinncinati-North", "cincinnati@marblelife.com", "Marblelife32",  "Eddie", "Miller", "106 Pike Street", "",  "Bromley",  "41016", "KY", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true)
                        ,new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true)),
                     Number(PhoneType.Office, "8593449950")),

                    CreateModel("New Orleans", "neworleans@marblelife.com", "Marblelife33",  "Paul", "Stewart", "P.O. Box 8836", "", "Metairie", "70011", "LA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true)),
                     Number(PhoneType.Office, "5048338233"),  Number(PhoneType.Fax, "5048335459"),  Number(PhoneType.Cell, "5042586117")),

                    //    added By srikeerti
                              
                    CreateModel("Baltimore", "delawarevalley2@marblelife.com", "Marblelife4",  "Nick", "Danella", "579 Abbott Drive", "", "Broomall", "19008", "MD", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                     Number(PhoneType.Cell, "4848323333"),  Number(PhoneType.Office, "4844728329")),

                    CreateModel("Western Michigan", "westernmichigan@marblelife.com", "Marblelife12",  "Jerry", "Schaffer", "300 Washington Street", "", "Mason", "48854", "MI", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true), new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                     Number(PhoneType.Office, "8773526995"),  Number(PhoneType.Cell, "5175252457")),

                    // To Verify
                    CreateModel("Detroit-Southeast Michigan", "semichigan@marblelife.com", "Marblelife2",  "Kirk", "VanMeerbeeck", "31780 Eight Mile Road", "", "Farmington Hills", "48336", "MI", null,
                    FeeProfile2(), Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.MetalLife, true), new Tuple<EnumServiceType, bool>(EnumServiceType.TileInstall, true)),
                     Number(PhoneType.TollFree, "8002099596"),  Number(PhoneType.Office, "2484740400"),  Number(PhoneType.Fax, "2484748347"),  Number(PhoneType.Cell, "2488663227")),

                    CreateModel("Minneapolis-Minnesota", "minnesota@marblelife.com", "Marblelife35",  "Bernie", "Ardolf", "1202 Lyndale Avenue North", "", "Faribault", "55021", "MN", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Cell, "6122216283")),

                    CreateModel("St.Louis", "stlouis@marblelife.com", "Marblelife18",  "Jim", "Zemek", "116 May Rd", "Unit D", "Wentzville", "63385", "MO", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "8886789013"),Number(PhoneType.Cell, "6362905080")),

                    CreateModel("Greenville-South Carolina", "thecarolinas_1@marblelife.com", "Marblelife6",  "Norbert", "Sigling", "21 Garraux Street", "", "Greenville", "29609", "NC", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "8886272530"),Number(PhoneType.Cell, "8642356297")),

                    CreateModel("Jersey-South", "delawarevalley3@marblelife.com", "Marblelife39",  "Nick", "Danella", "579 Abbott Drive", "", "Broomall", "19008", "NJ", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4848323333"),Number(PhoneType.Cell, "4844728329")),

                    CreateModel("Jersey-Central", "delawarevalley4@marblelife.com", "Marblelife37" , "Nick", "Danella", "579 Abbott Drive", "", "Broomall", "19008", "NJ", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4848323333"),Number(PhoneType.Cell, "4844728329")),

                    CreateModel("Jersey-North", "delawarevalley5@marblelife.com", "Marblelife38",  "Nick", "Danella", "579 Abbott Drive", "", "Broomall", "19008", "NJ", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4848323333"),Number(PhoneType.Cell, "4844728329")),

                    CreateModel("New Mexico", "newmexico@marblelife.com", "Marblelife40",  "Jim", "Mannari", "1737 East Jackson St.", "", "Phoenix", "85034", "NM", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4804833745"),Number(PhoneType.Cell, "4804834615")),

                    CreateModel("Las Vegas", "nevada@marblelife.com", "Marblelife36",  "Jim", "Mannari", "579 Abbott Drive", "", "Broomall", "19008", "NV", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4804833745"),Number(PhoneType.Cell, "4804834615")),

                    CreateModel("New York", "newYork@marblelife.com", "Marblelife41",  "Nick", "Danella", "1737 East Jackson St.", "", "Phoenix", "85034", "NY", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4848323333"),Number(PhoneType.Cell, "4844728329")),

                    CreateModel("Cleveland", "cleveland@marblelife.com", "Marblelife43",  "Kirk", "VanMeerbeeck", "31780 Eight Mile Road", "", "Farmington Hills", "48336", "OH", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.MetalLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.TileInstall, true)),
                    Number(PhoneType.TollFree, "8002099596"),Number(PhoneType.Office, "2484740400"),Number(PhoneType.Fax, "2484748347"),Number(PhoneType.Cell, "2488663227")),

                    CreateModel("Columbus-Central OH", "centralohio@marblelife.com", "Marblelife42",  "Gary", "Allen", "P.O.Box 98", "", "Reynoldsburg", "43068", "OH", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "6148376146"),Number(PhoneType.Office, "6148376156"),Number(PhoneType.Cell, "6145788943")),

                    CreateModel("Cinncinati", "cincinnati_1@marblelife.com", "Marblelife32",  "Eddie", "Miller", "106 Pike Street", "", "Bromley", "41016", "OH", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "8593449950"),Number(PhoneType.Fax, "8593449940"),Number(PhoneType.Cell, "8593933647")),


                    CreateModel("Portland, Oregon", "portland@marblelife.com", "Marblelife44",  "Scott", "Carden", "103 NE 147th St", "", "Vancouver", "98685", "OR", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "5037588901"),Number(PhoneType.Cell, "5033196452")),

                    CreateModel("Delaware Valley", "delawarevalley@marblelife.com", "Marblelife1", "Nick", "Danella", "579 Abbott Drive", "", "Broomall", "19008", "PA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4848323333"),Number(PhoneType.Cell, "4844728329")),

                    CreateModel("Pittsburgh", "delawarevalley_1@marblelife.com", "Marblelife8", "Nick", "Danella", "579 Abbott Drive", "", "Broomall", "19008", "PA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4848323333"),Number(PhoneType.Cell, "4844728329")),

                    CreateModel("Spartansburg", "thecarolinas@marblelife.com", "spartansburg@123", "Norbert",  "Sigling", "21 Garraux Street,", "", "Greenville", "29609", "SC", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "8886272530"),Number(PhoneType.Cell, "8642356297")),

                    CreateModel("Nashville", "nashville@marblelife.com", "Marblelife46", "Dallas", "Randolph",  "6324 Murray Lane", "" ,"Brentwood", "37027", "TN", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CarpetLife, true)),
                    Number(PhoneType.Office, "6155019818"),Number(PhoneType.Fax, "6153311133"),Number(PhoneType.Cell, "6154802040")),

                    CreateModel("Houston", "houston@marblelife.com", "Marblelife15", "Howard",  "Partridge", "808 Park Two Dr.", "", "Sugar Land",  "77478", "TX", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Wood, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Cleanair, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CarpetLife, true)),
                    Number(PhoneType.Office, "7137844648"),Number(PhoneType.Cell, "8887321009")),

                    CreateModel("Dallas-North", "northtexas@marblelife.com","Marblelife14", "Sarah", "Boltz", "3425 Raider Drive", "Suite 3B", "Hurst", "76053" , "TX", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Enduracrete, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "8173185678"),Number(PhoneType.Fax, "8173545655"),Number(PhoneType.Cell, "8172533555")),

                    CreateModel("Salt Lake", "utah@marblelife.com", "saltLake@123", "Scott", "Harding", "3845 Marsha Dive", "", "Salt Lake City", "84128" , "UT", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "8019550246"),Number(PhoneType.Fax, "8019675995"),Number(PhoneType.Cell, "8019133506")),

                    CreateModel("DC", "dc_1@marblelife.com", "dcArlington@123",  "Freddy", "Castillo", "#", "", "Arlington", "20001" , "VA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                        , new Tuple<EnumServiceType, bool>(EnumServiceType.Wood, true)),
                    Number(PhoneType.Office, "2023049234")),

                    CreateModel("Seattle-Tacoma", "seattle@marblelife.com", "Marblelife47",  "Vince", "Ayers",  "3420  'C' St. N.E.", "# 309",  "Auburn",   "98002" , "WA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)
                        , new Tuple<EnumServiceType, bool>(EnumServiceType.Fabricators, true), new Tuple<EnumServiceType, bool>(EnumServiceType.MetalLife, true)
                        , new Tuple<EnumServiceType, bool>(EnumServiceType.TileInstall  , true)),
                    Number(PhoneType.Office, "2538338849"), Number(PhoneType.Fax, "2533338155"), Number(PhoneType.Cell, "2062614211")),

                    CreateModel("Calgary", "calgary@marblelife.com", "Marblelife48", "Leo", "Teo", "", "",  "", "", "CANADA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "4039187183")),

                    CreateModel("Vancouver", "vancouver@marblelife.com", "Marblelife11", "Domenic", "Papalia",  "", "", "Burnaby",  "", "CANADA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "6042989857"), Number(PhoneType.Fax, "6046925533")),

                    CreateModel("Toronto", "toronto@marblelife.com", "Marblelife49", "Joanna",  "Kozminski" ,"", "", "Brampton",    ""  , "CANADA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "6474087667")),

                    CreateModel("York", "york@marblelife.com", "Marblelife50",   "Danili", "Verdysh",   "31 Danesbury Crescent", "" ,"Brampton", ""  , "CANADA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                        new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "6474087666")),

                    CreateModel("Bahamas", "bahamas@marblelife.com", "Marblelife51", "Marvin", "Bonaby", "Bradley Sheppard Plaza", "",  "Nassau", "", "BAHAMAS", null,
                    FeeProfile2(), Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                         new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true),new Tuple<EnumServiceType, bool>(EnumServiceType.CarpetLife, true)),
                    Number(PhoneType.Office, "2426760175"), Number(PhoneType.Cell, "2424677341")),

                    CreateModel("Cayman Islands", "caymanislands@marblelife.com", "Marblelife52",   "Dean", "Wood", "472 West Bay Road",""  ,"Grand Cayman", "" , "CAYMAN ISLANDS", null,
                    FeeProfile2(), Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                             new Tuple<EnumServiceType, bool>(EnumServiceType.CarpetLife, true)),
                    Number(PhoneType.Office, "3459457007")),

                    CreateModel("Johnnesburgh-Pretoria", "gm-jbh@marblelife.com", "johnnesburgh@123",  "Vaughan",  "Wepener", "", "",   "Centurion","" , "SOUTH AFRICA", null,
                    null, Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                             new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true), new Tuple<EnumServiceType, bool>(EnumServiceType.CleanShield, true),
                             new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    Number(PhoneType.Office, "0126532505"))
                    //,

                    //CreateModel("United Arab Emirates", "uae@marblelife.com", "",   "Simon", "Askew",	"P.O. Box 212162" , "Al Wasi", "Dubai", "", "UNITED ARAB EMIRATES", null,
                    //Services( new Tuple<EnumServiceType, bool>(EnumServiceType.StoneLife, true),new Tuple<EnumServiceType, bool>(EnumServiceType.ColorSeal, true),
                    //         new Tuple<EnumServiceType, bool>(EnumServiceType.Tilelok, true),
                    //         new Tuple<EnumServiceType, bool>(EnumServiceType.Vinylguard, true)),
                    //Number(PhoneType.Office, "011-971-4-3415658"), Number(PhoneType.Fax, "011-971-4-3415662"), Number(PhoneType.Cell, "011-971-0502087878")),

            };
        }

        public static List<UserEditModel> Users(params Tuple<string, string>[] users)
        {
            return users.Select(x =>
                new UserEditModel
                {
                    CreateLogin = false,
                    PersonEditModel = new PersonEditModel
                    {
                        Name = new Name
                        {
                            FirstName = x.Item1,
                            LastName = x.Item2,
                        }
                    }
                }).ToList();
        }
        public static List<FranchiseeServiceEditModel> Services(params Tuple<EnumServiceType, bool>[] type)
        {
            return type.Select(x =>
            new FranchiseeServiceEditModel
            {
                ServiceTypeId = (long)x.Item1,
                CalculateRoyalty = x.Item2
            }).ToList();
        }

        public static PhoneEditModel Number(PhoneType type, string number)
        {
            return new PhoneEditModel
            {
                PhoneType = (int)type,
                PhoneNumber = number
            };
        }

        public static FranchiseeDetailsModel CreateModel(string name, string email, string password, string ownerFirstName, string ownerLastName,
            string addressLine1, string addressLine2, string city, string zipCode, string state, IList<UserEditModel> users, 
            FeeProfileEditModel feeModel,
            IList<FranchiseeServiceEditModel> services, params PhoneEditModel[] numbers)
        {
            if (users == null)
                users = new List<UserEditModel>();

            feeModel = feeModel == null ? FeeProfile3(): feeModel;

            return new FranchiseeDetailsModel
            {
                Franchisee = new FranchiseeEditModel
                {
                    Name = name,
                    Email = email,
                    DataRecorderMetaData = new DataRecorderMetaData(),
                    TypeId = (long)OrganizationType.Franchisee,
                    OrganizationOwner = new OrganizationOwnerEditModel
                    {
                        OwnerFirstName = ownerFirstName,
                        OwnerLastName = ownerLastName,
                        DataRecorderMetaData = new DataRecorderMetaData(),
                        SendUserLoginViaEmail = false,
                        Password = password
                    },
                    FeeProfile = feeModel,
                    Address = CreateAddressModel(addressLine1, addressLine2, city, zipCode, state),
                    PhoneNumbers = numbers.Select(x => new PhoneEditModel
                    {
                        DataRecorderMetaData = new DataRecorderMetaData(),
                        PhoneNumber = x.PhoneNumber,
                        PhoneType = x.PhoneType
                    }),
                    FranchiseeServices = services.Select(x => new FranchiseeServiceEditModel
                    {
                        CalculateRoyalty = x.CalculateRoyalty,
                        ServiceTypeId = x.ServiceTypeId,
                        IsActive = true
                    })
                },
                users = users.Select(x => new UserEditModel
                {
                    CreateLogin = x.CreateLogin,
                    PersonEditModel = x.PersonEditModel,
                }).ToList()

            };

        }

        private static IList<AddressEditModel> CreateAddressModel(string addressLine1, string addressLine2, string city, string zipCode, string state)
        {
            var stateId = states.Where(x => x.Name.ToUpper().Equals(state) || x.ShortName.ToUpper().Equals(state)).Select(y => y.Id).FirstOrDefault();
            if (string.IsNullOrEmpty(addressLine1) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(zipCode))
            {
                return new List<AddressEditModel>();
            }

            return new List<AddressEditModel>()
                {
                    new AddressEditModel
                    {
                        AddressLine1 = addressLine1,
                        AddressLine2 = addressLine2,
                        AddressType = (long)AddressType.Primary,
                        City = city,
                        State = state,
                        StateId = stateId,
                        ZipCode = zipCode
                    }
                };
        }

        private static FeeProfileEditModel FeeProfile1()
        {
            return new FeeProfileEditModel
            {
                AdFundPercentage = 2,
                MinimumRoyaltyPerMonth = 800,
                PaymentFrequencyId = (int)PaymentFrequency.Monthly,
                SalesBasedRoyalty = true,
                Slabs = new List<RoyaltyFeeSlabsEditModel>
                {
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 0,
                        MaxValue = 200000,
                        ChargePercentage = 6
                    },
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 200001,
                        MaxValue = 300000,
                        ChargePercentage = 5
                    },
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 300001,
                        ChargePercentage = 4
                    }
                }
            };
        }


        private static FeeProfileEditModel FeeProfile2()
        {
            return new FeeProfileEditModel
            {
                AdFundPercentage = 2,
                MinimumRoyaltyPerMonth = 800,
                PaymentFrequencyId = (int)PaymentFrequency.Monthly,
                SalesBasedRoyalty = true,
                Slabs = new List<RoyaltyFeeSlabsEditModel>
                {
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 0,
                        MaxValue = 500000,
                        ChargePercentage = 6
                    },
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 500001,
                        MaxValue = 1000000,
                        ChargePercentage = 5
                    },
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 1000001,
                        ChargePercentage = 4
                    }
                }
            };
        }

        private static FeeProfileEditModel FeeProfile3()
        {
            return new FeeProfileEditModel
            {
                AdFundPercentage = 2,
                MinimumRoyaltyPerMonth = 800,
                PaymentFrequencyId = (int)PaymentFrequency.Weekly,
                SalesBasedRoyalty = true,
                Slabs = new List<RoyaltyFeeSlabsEditModel>
                {
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 0,
                        MaxValue = 500000,
                        ChargePercentage = 6
                    },
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 500001,
                        MaxValue = 1000000,
                        ChargePercentage = 5
                    },
                    new RoyaltyFeeSlabsEditModel
                    {
                        MinValue = 1000001,
                        ChargePercentage = 4
                    }
                }
            };
        }
    }
}
