CREATE TABLE `calendarfileupload` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `FileId` BIGINT(20) NOT NULL,
  `LogFileId` BIGINT(20) NULL DEFAULT NULL,
  `FailedRecords` INT NULL DEFAULT NULL,
  `SavedRecords` INT NULL,
  `AssigneeId` BIGINT(20) NOT NULL,
  `TypeId` BIGINT(20) NOT NULL,
  `StatusId` BIGINT(20) NOT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`) ,
  INDEX `fk_calendarFileUpload_franchisee_idx` (`FranchiseeId` ASC) ,
  INDEX `fk_calendarFileUpload_file_idx` (`FileId` ASC) ,
  INDEX `fk_calendarFileUpload_file2_idx` (`LogFileId` ASC) ,
  INDEX `fk_calendarFileUpload_lookup1_idx` (`StatusId` ASC) ,
  INDEX `fk_calendarFileUpload_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC) ,
  CONSTRAINT `fk_calendarFileUpload_franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_calendarFileUpload_file1`
    FOREIGN KEY (`FileId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_calendarFileUpload_file2`
    FOREIGN KEY (`LogFileId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_calendarFileUpload_lookup1`
    FOREIGN KEY (`StatusId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_calendarFileUpload_datarecordermetadata`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `datarecordermetadata` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


ALTER TABLE `calendarfileupload` 
ADD COLUMN `TotalRecords` INT(11) NULL DEFAULT NULL;

