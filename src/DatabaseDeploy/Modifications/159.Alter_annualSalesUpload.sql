ALTER TABLE `annualsalesdataupload` 
DROP FOREIGN KEY `fk_annualsalesDataupload_salesdataupload`;
ALTER TABLE `annualsalesdataupload` 
CHANGE COLUMN `SalesDataUploadId` `SalesDataUploadId` BIGINT(20) NULL DEFAULT NULL ;
ALTER TABLE `annualsalesdataupload` 
ADD CONSTRAINT `fk_annualsalesDataupload_salesdataupload`
  FOREIGN KEY (`SalesDataUploadId`)
  REFERENCES `salesdataupload` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
