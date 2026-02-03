CREATE TABLE `timezoneinformation` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `TimeZone` VARCHAR(1024) NOT NULL ,
  `TimeDifference` INT NOT NULL ,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1' ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`) );
  
ALTER TABLE `timezoneinformation` 
CHANGE COLUMN `TimeDifference` `TimeDifference` DECIMAL(10,2) NOT NULL ;

ALTER TABLE `timezoneinformation` 
ADD COLUMN `CountryId` BIGINT(20) NOT NULL,
ADD INDEX `fk_timezoneinformation_country_idx` (`CountryId` ASC)  ;
ALTER TABLE `timezoneinformation` 
ADD CONSTRAINT `fk_timezoneinformation_country`
  FOREIGN KEY (`CountryId`)
  REFERENCES `country` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
INSERT INTO `timezoneinformation` (`TimeZone`, `TimeDifference`, `IsActive`, `IsDeleted`, `CountryId`) VALUES ('Eastern Standard Time (EST)', 5, 1, 0, '1');
INSERT INTO `timezoneinformation` (`TimeZone`, `TimeDifference`, `IsActive`, `IsDeleted`, `CountryId`) VALUES ('Mountain Standard Time (MST)', 7, 1, 0, '1');
INSERT INTO `timezoneinformation` (`TimeZone`, `TimeDifference`, `IsActive`, `IsDeleted`, `CountryId`) VALUES ('Central Standard Time (CST)', 6, 1, 0, '1');
INSERT INTO `timezoneinformation` (`TimeZone`, `TimeDifference`, `IsActive`, `IsDeleted`, `CountryId`) VALUES ('Pacific Standard Time (PST)', 8, 1, 0, '1');
INSERT INTO `timezoneinformation` (`TimeZone`, `TimeDifference`, `IsActive`, `IsDeleted`, `CountryId`) VALUES ('Atlantic Standard Time (AST)', 4, 1, 0, '1');
INSERT INTO `timezoneinformation` (`TimeZone`, `TimeDifference`, `IsActive`, `IsDeleted`, `CountryId`) VALUES ('South Africa Time Zone', -2, 1, 0, '5');
INSERT INTO `timezoneinformation` (`TimeZone`, `TimeDifference`, `IsActive`, `IsDeleted`, `CountryId`) VALUES ('Gulf Time Zone (UAE)', -4, 1, 0, '6');

ALTER TABLE `calendarfileupload` 
ADD COLUMN `TimeZoneId` BIGINT(20) NULL DEFAULT NULL ,
ADD INDEX `fk_calendarFileUpload_timeZoneInformation_idx` (`TimeZoneId` ASC)  ;
ALTER TABLE `calendarfileupload` 
ADD CONSTRAINT `fk_calendarFileUpload_timeZoneInformation`
  FOREIGN KEY (`TimeZoneId`)
  REFERENCES `timezoneinformation` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;  

update calendarfileupload set timezoneid = 1 where id > 0;

