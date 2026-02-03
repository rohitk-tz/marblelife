CREATE TABLE `annualsalesdataupload` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `FileId` BIGINT(20) NOT NULL,
  `StatusId` BIGINT(20) NOT NULL,
  `ParsedLogFileId` BIGINT(20) NULL DEFAULT NULL,
  `NoOfFailedRecords` INT(11) NULL DEFAULT NULL,
  `NoOfParsedRecords` INT(11) NULL DEFAULT NULL,
  `TotalAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `PaidAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `CurrencyExchangeRateId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `PeriodStartDate` DATE NOT NULL,
  `PeriodEndDate` DATE NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `fk_annualSalesdataUpload_franchisee_idx` (`FranchiseeId` ASC),
  INDEX `fk_annualsalesdataupload_file1_idx` (`FileId` ASC),
  INDEX `fk_annualsalesdataupload_file2_idx` (`ParsedLogFileId` ASC),
  INDEX `fk_annualsalesdataupload_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC),
  INDEX `fk_annualsalesdataupload_lookup_idx` (`StatusId` ASC),
  CONSTRAINT `fk_annualSalesdataUpload_franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_annualsalesdataupload_file1`
    FOREIGN KEY (`FileId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_annualsalesdataupload_file2`
    FOREIGN KEY (`ParsedLogFileId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_annualsalesdataupload_datarecordermetadata`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `datarecordermetadata` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_annualsalesdataupload_lookup`
    FOREIGN KEY (`StatusId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

ALTER TABLE `annualsalesdataupload` 
ADD COLUMN `SalesDataUploadId` BIGINT(20) NOT NULL,
ADD INDEX `fk_annualsalesDataupload_salesdataupload_idx` (`SalesDataUploadId` ASC);
ALTER TABLE `annualsalesdataupload` 
ADD CONSTRAINT `fk_annualsalesDataupload_salesdataupload`
  FOREIGN KEY (`SalesDataUploadId`)
  REFERENCES `salesdataupload` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

