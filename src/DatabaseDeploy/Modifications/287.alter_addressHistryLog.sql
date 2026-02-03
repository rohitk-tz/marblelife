ALTER TABLE `addresshistrylog` 
ADD COLUMN `InvoiceId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_addresshistrylog_Invoice_idx` (`InvoiceId`);
ALTER TABLE `addresshistrylog` 
ADD CONSTRAINT `fk_addresshistrylog_Invoice`
  FOREIGN KEY (`InvoiceId`)
  REFERENCES `invoice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  
  
  ALTER TABLE `addresshistrylog` 
ADD COLUMN `InvoiceDate` Datetime NULL DEFAULT NULL;

