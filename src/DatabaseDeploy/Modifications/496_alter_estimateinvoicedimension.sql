Alter table estimateinvoicedimension
Add column `SetPrice` bigint(20) NULL default Null;

Alter table estimateinvoicedimension
Add column `IncrementedPrice` bigint(20) NULL default Null;


Alter table estimateInvoiceService
Add column `ServiceTagId` bigint(20) NULL default Null,
ADD INDEX `fk_estimateInvoiceService_lookup_idx` (`ServiceTagId` ASC);
ALTER TABLE `estimateInvoiceService` 
ADD CONSTRAINT `fk_estimateInvoiceService_lookup`
  FOREIGN KEY (`ServiceTagId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  Alter table estimateinvoicedimension
Add column `UnitTypeId` bigint(20) NULL default 291,
ADD INDEX `fk_estimateinvoicedimension_lookup_idx` (`UnitTypeId` ASC);
ALTER TABLE `estimateinvoicedimension` 
ADD CONSTRAINT `fk_estimateinvoicedimension_lookup`
  FOREIGN KEY (`UnitTypeId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  UPDATE `makalu`.`ServiceType` SET `NewOrderBy` = '10' WHERE (`Id` = '2');
UPDATE `makalu`.`ServiceType` SET `NewOrderBy` = '9' WHERE (`Id` = '43');

UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '10' WHERE (`Id` = '2');
UPDATE `makalu`.`servicetype` SET `Description` = 'Concrete Staining', `NewOrderBy` = '9' WHERE (`Id` = '43');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '19' WHERE (`Id` = '19');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '20' WHERE (`Id` = '17');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '21' WHERE (`Id` = '4');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '22' WHERE (`Id` = '40');



UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '19' WHERE (`Id` = '1');
UPDATE `makalu`.`servicetype` SET `NewOrderBy` = '18' WHERE (`Id` = '19');