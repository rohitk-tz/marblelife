ALTER TABLE `jobscheduler` 
ADD COLUMN `ServiceTypeId` BIGINT(20) NULL DEFAULT NULL ,
ADD INDEX `fk_jobScheduler_serviceType_idx` (`ServiceTypeId` ASC) ;
ALTER TABLE `jobscheduler` 
ADD CONSTRAINT `fk_jobScheduler_serviceType`
  FOREIGN KEY (`ServiceTypeId`)
  REFERENCES `servicetype` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  INSERT INTO `jobstatus` (`Id`, `Name`, `ColorCode`, `IsDeleted`) VALUES ('6', 'Tentative', '	#9932CC', 0);
  
  ALTER TABLE `jobscheduler` 
ADD COLUMN `IsImported` BIT(1) NOT NULL DEFAULT b'0';

ALTER TABLE `jobcustomer` 
DROP FOREIGN KEY `fk_jobCustomer_address`;
ALTER TABLE `jobcustomer` 
CHANGE COLUMN `AddressId` `AddressId` BIGINT(20) NULL DEFAULT NULL  ,
ADD COLUMN `CustomerAddress` VARCHAR(1024) NULL DEFAULT NULL;
ALTER TABLE `jobcustomer` 
ADD CONSTRAINT `fk_jobCustomer_address`
  FOREIGN KEY (`AddressId`)
  REFERENCES `address` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


