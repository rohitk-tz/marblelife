 ALTER TABLE `systemauditrecord` 
ADD COLUMN `AnnualReportTypeId` BIGINT(20) NULL DEFAULT 3,
ADD INDEX `fk_systemauditrecord_annualreporttype_idx` (`AnnualReportTypeId` ASC);
ALTER TABLE `systemauditrecord` 
ADD CONSTRAINT `fk_systemauditrecord_annualreporttype`
  FOREIGN KEY (`AnnualReportTypeId`)
  REFERENCES `annualreporttype` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;