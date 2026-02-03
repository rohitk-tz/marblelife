ALTER TABLE `auditinvoice` 
ADD COLUMN `ReportTypeId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_auditinvoice_reportType_idx` (`ReportTypeId` ASC);
ALTER TABLE `auditinvoice` 
ADD CONSTRAINT `fk_auditinvoice_reportType_idx`
  FOREIGN KEY (`ReportTypeId`)
  REFERENCES `annualreporttype` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;



 ALTER TABLE `auditaddress` 
ADD COLUMN `email` varchar(120) NULL DEFAULT NULL,
ADD COLUMN `phone` varchar(16) NULL DEFAULT NULL


 
