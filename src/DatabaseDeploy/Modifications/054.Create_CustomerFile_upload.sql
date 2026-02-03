CREATE TABLE `CustomerFileUpload` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FileId` BIGINT(20) NOT NULL,
  `StatusId` BIGINT(20) NOT NULL ,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)) ;

ALTER TABLE `CustomerFileUpload` 
ADD INDEX `fk_CustomerFileUpload_file_idx` (`FileId` ASC) ,
ADD INDEX `fk_CustomerFileUpload_DataRecorderMetaData_idx` (`DataRecorderMetaDataId` ASC),
ADD INDEX `fk_CustomerFileUpload_idx` (`StatusId` ASC) ;
ALTER TABLE `CustomerFileUpload` 
ADD CONSTRAINT `fk_CustomerFileUpload_File`
  FOREIGN KEY (`FileId`)
  REFERENCES `File` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_CustomerFileUpload_DataRecorderMetaData`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `DataRecorderMetaData` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_CustomerFileUpload`
  FOREIGN KEY (`StatusId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
