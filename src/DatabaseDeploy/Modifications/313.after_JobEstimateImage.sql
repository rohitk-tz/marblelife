ALTER TABLE `jobestimateimage` 
ADD COLUMN `DataRecorderMetaDataId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_jobestimateimage_dataRecorderMetaData_idx` (`DataRecorderMetaDataId`);
ALTER TABLE `jobestimateimage` 
ADD CONSTRAINT `fk_jobestimateimage_dataRecorderMetaData`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `DataRecorderMetaData` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `jobestimateservices` 
ADD COLUMN `IsBeforeImage` BIT(1) NULL DEFAULT NULL;
