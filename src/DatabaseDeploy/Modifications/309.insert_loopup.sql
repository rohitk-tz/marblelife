INSERT INTO `lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('25', 'BeforeAfterJob', b'0');


INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('203', '25', 'Before', 'Before', '9', b'1', b'0');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('204', '25', 'After', 'After', '9', b'1', b'0');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('205', '25', 'During', 'During', '9', b'1', b'0');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('206', '25', 'ExteriorBuilding', 'ExteriorBuilding', '9', b'1', b'0');
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('207', '25', 'Invoice', 'InvoiceImages', '9', b'1', b'0');
