  CREATE TABLE `calldetaildata` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NULL DEFAULT NULL,
  `PhoneLabel` VARCHAR(45) NOT NULL,
  `StartDate` DATE NOT NULL,
  `EndDate` DATE NOT NULL,
  `Count` Int NOT NULL,
  `IsWeekly` BIT(1)  NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,  
  PRIMARY KEY (`Id`));

ALTER TABLE `calldetaildata` 
ADD INDEX `fk_callDetaildata_franchisee_idx` (`FranchiseeId` ASC),
ADD INDEX `fk_callDetaildata_dataRecordermetadata_idx` (`DataRecorderMetaDataId` ASC);
ALTER TABLE `calldetaildata` 
ADD CONSTRAINT `fk_callDetaildata_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_callDetaildata_dataRecordermetadata`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  CREATE TABLE `webleaddata` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NULL DEFAULT NULL,
  `Url` VARCHAR(128) NOT NULL,
  `StartDate` DATE NOT NULL,
  `EndDate` DATE NOT NULL,
  `Count` Int NOT NULL,
  `IsWeekly` BIT(1)  NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,  
  PRIMARY KEY (`Id`));

ALTER TABLE `webleaddata` 
ADD INDEX `fk_webLeadData_Franchisee_idx` (`FranchiseeId` ASC),
ADD INDEX `fk_webLeaddata_dataRecorderMeatadata_idx` (`DataRecorderMetaDataId` ASC);
ALTER TABLE `webleaddata` 
ADD CONSTRAINT `fk_webLeadData_Franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_webLeaddata_dataRecorderMeatadata`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
