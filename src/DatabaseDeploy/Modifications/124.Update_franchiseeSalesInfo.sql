TRUNCATE `franchiseesalesinfo`;

ALTER TABLE `franchiseesalesinfo` 
CHANGE COLUMN `Id` `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
ADD COLUMN `ClassTypeId` BIGINT(20) NOT NULL AFTER `UpdatedDate`,
ADD COLUMN `ServiceTypeId` BIGINT(20) NOT NULL AFTER `ClassTypeId`,
ADD INDEX `fk_franchiseesalesInfo_marketingClass_idx` (`ClassTypeId` ASC) ,
ADD INDEX `fk_franchiseeSalesInfo_serviceType_idx` (`ServiceTypeId` ASC) ;
ALTER TABLE `franchiseesalesinfo` 
ADD CONSTRAINT `fk_franchiseesalesInfo_marketingClass`
  FOREIGN KEY (`ClassTypeId`)
  REFERENCES `marketingclass` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_franchiseeSalesInfo_serviceType`
  FOREIGN KEY (`ServiceTypeId`)
  REFERENCES `servicetype` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;