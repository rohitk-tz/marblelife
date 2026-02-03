ALTER TABLE `FranchiseeInvoice` 
DROP FOREIGN KEY `fk_FranchiseeInvoice_SalesDataUpload`;
ALTER TABLE `FranchiseeInvoice` 
CHANGE COLUMN `SalesDataUploadId` `SalesDataUploadId` BIGINT(20) NULL ;
ALTER TABLE `FranchiseeInvoice` 
ADD CONSTRAINT `fk_FranchiseeInvoice_SalesDataUpload`
  FOREIGN KEY (`SalesDataUploadId`)
  REFERENCES `SalesDataUpload` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
