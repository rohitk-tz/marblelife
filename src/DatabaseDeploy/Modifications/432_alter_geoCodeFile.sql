
ALTER TABLE `geocodefileupload` 
ADD COLUMN `ParsedLogForCountyFileId` BIGINT(20) NULL DEFAULT null AFTER `ParsedLogFileId`,
ADD INDEX `fk_geocodefileupload_file_idx` (`ParsedLogForCountyFileId` ASC);
ALTER TABLE `geocodefileupload` 
ADD CONSTRAINT `fk_geocodefileupload_file`
  FOREIGN KEY (`ParsedLogForCountyFileId`)
  REFERENCES `file` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  ALTER TABLE `geocodefileupload` 
ADD COLUMN `ParsedLogForZipFileId` BIGINT(20) NULL DEFAULT null AFTER `ParsedLogFileId`,
ADD INDEX `fk_geocodefileupload_file_1_idx` (`ParsedLogForZipFileId` ASC);
ALTER TABLE `geocodefileupload` 
ADD CONSTRAINT `fk_geocodefileupload_file_1`
  FOREIGN KEY (`ParsedLogForZipFileId`)
  REFERENCES `file` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;