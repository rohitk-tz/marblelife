ALTER TABLE `addresshistrylog` 
ADD COLUMN `AnnualSalesDataUploadId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_addresshistrylog_annualSalesData_idx` (`AnnualSalesDataUploadId`);
ALTER TABLE `addresshistrylog` 
ADD CONSTRAINT `fk_addresshistrylog_annualSalesData_idx`
  FOREIGN KEY (`AnnualSalesDataUploadId`)
  REFERENCES `AnnualSalesDataupload` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;