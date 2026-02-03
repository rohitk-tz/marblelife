CREATE TABLE `holiday` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(128) NOT NULL,
  `StartDate` DATE NOT NULL,
  `EndDate` DATE NOT NULL,
  `Description` VARCHAR(512) NULL DEFAULT NULL,
  `CountryId` BIGINT(20) NULL DEFAULT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_holiday_country_idx` (`CountryId` ASC),
  CONSTRAINT `fk_holiday_country`
    FOREIGN KEY (`CountryId`)
    REFERENCES `country` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


ALTER TABLE `holiday` 
ADD COLUMN `DataRecorderMetaDataId` BIGINT(20) NULL,
ADD INDEX `fk_holiday_datarecordermetadataId_idx` (`DataRecorderMetaDataId` ASC);
ALTER TABLE `holiday` 
ADD CONSTRAINT `fk_holiday_datarecordermetadataId`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

	
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`) VALUES ('New Year’s Day', '2018-01-01', '2018-01-01');
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`) VALUES ('Christmas Eve', '2018-12-24', '2018-12-24');
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`) VALUES ('Christmas Day', '2018-12-25', '2018-12-25');
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`, `CountryId`) VALUES ('Memorial Day', '2018-05-28', '2018-05-28', '1');
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`, `CountryId`) VALUES ('Independence Day', '2018-07-04', '2018-07-04', '1');
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`, `CountryId`) VALUES ('Labor Day', '2018-09-03', '2018-09-03', '1');
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`, `CountryId`) VALUES ('Thanksgiving Day', '2018-11-22', '2018-11-22', '1');
INSERT INTO `holiday` (`Title`, `StartDate`, `EndDate`, `CountryId`) VALUES ('Thanksgiving  Day', '2018-10-08', '2018-10-08', '2');
