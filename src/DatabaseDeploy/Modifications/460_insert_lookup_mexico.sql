INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `Alias`, `IsDeleted`) VALUES ('39', 'Language', 'Language', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('249', '39', 'English', 'English', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('250', '39', 'Spanish', 'Spanish', '2', b'1', b'0');
