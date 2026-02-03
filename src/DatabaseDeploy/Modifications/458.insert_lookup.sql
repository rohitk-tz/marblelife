INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('38', 'LoanType', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('244', '38', 'ISQFT', 'ISQFT', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('245', '38', 'Surgical Strike', 'Surgical Strike', '2', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('246', '38', 'Geofence', 'Geofence', '3', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('247', '38', 'Other', 'Other', '3', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('248', '38', 'None', 'None', '3', b'1', b'0');



ALTER TABLE `franchiseeLoan` 
ADD COLUMN `LoanTypeId` BIGINT(20) NULL DEFAULT null AFTER `IsDeleted`;
ALTER TABLE `franchiseeLoan` 
ADD CONSTRAINT `fk_franchiseeLoan_LoanTypeId`
  FOREIGN KEY (`LoanTypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;