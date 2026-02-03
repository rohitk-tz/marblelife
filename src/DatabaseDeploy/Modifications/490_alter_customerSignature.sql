INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('45', 'Customer Signature', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('289', '45', 'BEFORECOMPLETITION', 'BEFORE COMPLETITION', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('290', '45', 'AFTERCOMPLETITION', 'AFTER COMPLETITION', '2', b'1', b'0');

ALTER TABLE `customersignature` 
ADD COLUMN `TypeId` BIGINT(20) NULL DEFAULT 290 AFTER `IsDeleted`,
ADD INDEX `fk_customersignature_lookup_idx` (`TypeId` ASC);
ALTER TABLE `customersignature` 
ADD CONSTRAINT `fk_customersignature_lookup`
  FOREIGN KEY (`TypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

  ALTER TABLE `customersignatureinfo` 
ADD COLUMN `TypeId` BIGINT(20) NULL DEFAULT 290 AFTER `IsDeleted`,
ADD INDEX `fk_customersignatureinfo_lookup_idx` (`TypeId` ASC);
ALTER TABLE `customersignature` 
ADD CONSTRAINT `fk_customersignatureinfo_lookup`
  FOREIGN KEY (`TypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `customersignature` 
ADD COLUMN `JobSchedulerId` BIGINT(20) NULL DEFAULT null AFTER `IsDeleted`,
ADD INDEX `fk_customersignature_scheduler_idx` (`JobSchedulerId` ASC);
ALTER TABLE `customersignature` 
ADD CONSTRAINT `fk_customersignature_scheduler`
  FOREIGN KEY (`JobSchedulerId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  
  ALTER TABLE `customersignatureinfo` 
ADD COLUMN `JobSchedulerId` BIGINT(20) NULL DEFAULT null AFTER `IsDeleted`,
ADD INDEX `fk_customersignatureinfo_scheduler_idx` (`JobSchedulerId` ASC);
ALTER TABLE `customersignature` 
ADD CONSTRAINT `fk_customersignatureinfo_scheduler`
  FOREIGN KEY (`JobSchedulerId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `customersignature` 
ADD COLUMN `IsFromUrl` bit(1) default false;

ALTER TABLE `customersignatureInfo` 
ADD COLUMN `IsFromUrl` bit(1) default false;


ALTER TABLE `customersignature` 
ADD COLUMN `SignedById` BIGINT(20) NULL DEFAULT null AFTER `IsDeleted`,
ADD INDEX `fk_customersignature_person_idx` (`TypeId` ASC);
ALTER TABLE `customersignature` 
ADD CONSTRAINT `fk_customersignature_person`
  FOREIGN KEY (`SignedById`)
  REFERENCES `person` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
