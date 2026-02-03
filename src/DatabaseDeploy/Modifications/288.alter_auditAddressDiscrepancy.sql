ALTER TABLE `auditaddressdiscrepancy` 
ADD COLUMN `InvoiceId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_auditaddressdiscrepancy_Invoice_idx` (`InvoiceId`);
ALTER TABLE `auditaddressdiscrepancy` 
ADD CONSTRAINT `fk_auditaddressdiscrepancy_Invoice`
  FOREIGN KEY (`InvoiceId`)
  REFERENCES `invoice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
   ALTER TABLE `auditaddressdiscrepancy` 
ADD COLUMN `InvoiceDate` Datetime NULL DEFAULT NULL;