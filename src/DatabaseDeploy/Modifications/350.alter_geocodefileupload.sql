  ALTER TABLE `geocodefileupload` 
ADD COLUMN `ParsedLogFileId` BIGINT(20) NULL,
ADD INDEX `fk_geocodefileupload_subClassTypeId_idx` (`ParsedLogFileId`);
ALTER TABLE `geocodefileupload` 
ADD CONSTRAINT `fk_geocodefileupload_subClassTypeId`
  FOREIGN KEY (`ParsedLogFileId`)
  REFERENCES `file` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;