ALTER TABLE `estimateinvoiceservice` 
ADD COLUMN `ParentId` BIGINT(20) NULL DEFAULT null AFTER `ServiceType`,
ADD INDEX `fk_estimateinvoiceservice_service_idx` (`ParentId` ASC);
ALTER TABLE `estimateinvoiceservice` 
ADD CONSTRAINT `fk_estimateinvoiceservice_service`
  FOREIGN KEY (`ParentId`)
  REFERENCES `estimateinvoiceservice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;