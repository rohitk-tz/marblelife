INSERT INTO `lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('26', 'Franchisee Category', b'0');


INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('210', '26', 'FRONT OFFICE(MULTI LEVEL COVERAGE)', 'FRONTOFFICE', '1', b'1', b'0');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('211', '26', 'OFFICE PERSON', 'OFFICEPERSON', '2', b'1', b'0');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('212', '26', 'RESPOND WHEN AVAILABLE', 'RESPONDWHENAVAILABLE', '3', b'1', b'0');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('213', '26', 'RESPONDS NEXT DAY', 'RESPONDSNEXTDAY', '4', b'1', b'0');
