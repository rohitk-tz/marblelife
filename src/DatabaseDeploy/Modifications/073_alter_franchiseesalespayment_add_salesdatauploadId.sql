ALTER TABLE `franchiseesalespayment` 
ADD COLUMN `SalesDataUploadId` BIGINT(20) NULL ,
ADD INDEX `fk_franchiseeSalesPayment_salesDataupload_idx` (`SalesDataUploadId` ASC);
ALTER TABLE `franchiseesalespayment` 
ADD CONSTRAINT `fk_franchiseeSalesPayment_salesDataupload`
  FOREIGN KEY (`SalesDataUploadId`)
  REFERENCES `salesdataupload` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;