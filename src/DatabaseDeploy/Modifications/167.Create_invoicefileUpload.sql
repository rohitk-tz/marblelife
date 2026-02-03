CREATE TABLE `invoicefileupload` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FileId` BIGINT(20) NOT NULL,
  `StatusId` BIGINT(20) NOT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY(`Id`),
  INDEX `fk_InvoiceFileUplaod_DataRecorderMetaData_idx` (`DataRecorderMetaDataId` ASC),
  INDEX `fk_CustomerFileUpload_File_idx` (`FileId` ASC),
  INDEX `fk_InvoiceFileUpload_lookup_idx` (`StatusId` ASC),
  CONSTRAINT `fk_InvoiceFileUpload_lookup`
    FOREIGN KEY(`StatusId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_InvoiceFileUplaod_DataRecorderMetaData`
    FOREIGN KEY(`DataRecorderMetaDataId`)
    REFERENCES `datarecordermetadata` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_InvoiceFileUpload_File`
    FOREIGN KEY(`FileId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

