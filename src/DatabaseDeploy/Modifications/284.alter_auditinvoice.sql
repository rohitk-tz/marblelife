ALTER TABLE `AuditAddressDiscrepancy` 
ADD COLUMN `MarketingClassId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_AuditAddressDiscrepancy_marketingClass_idx` (`MarketingClassId` ASC);
ALTER TABLE `AuditAddressDiscrepancy` 
ADD CONSTRAINT `fk_AuditAddressDiscrepancy_marketingClass`
  FOREIGN KEY (`MarketingClassId`)
  REFERENCES `marketingclass` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  
  
  ALTER TABLE `addressHistrylog` 
ADD COLUMN `MarketingClassId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_AuditHistrylog_marketingClass_idx` (`MarketingClassId` ASC);
ALTER TABLE `addressHistrylog` 
ADD CONSTRAINT `fk_AuditHistrylog_marketingClass`
  FOREIGN KEY (`MarketingClassId`)
  REFERENCES `marketingclass` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;