ALTER TABLE `job` 
CHANGE COLUMN `Description` `Description` VARCHAR(1024) NULL DEFAULT NULL;

ALTER TABLE `jobestimate` 
CHANGE COLUMN `Description` `Description` VARCHAR(1024) NULL DEFAULT NULL;

update jobScheduler set assigneeid = salesRepId where estimateid is not null;

ALTER TABLE `jobresource` 
DROP FOREIGN KEY `JobResource_file`,
DROP FOREIGN KEY `JobResource_jobStatus`,
DROP FOREIGN KEY `jobResource_job`;
ALTER TABLE `jobresource` 
ADD COLUMN `DataRecorderMetaDataId` BIGINT(20) NULL DEFAULT NULL AFTER `EstimateId`,
ADD INDEX `fk_jobResource_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC);
ALTER TABLE `jobresource` 
ADD CONSTRAINT `fk_JobResource_file`
  FOREIGN KEY (`FileId`)
  REFERENCES `file` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_JobResource_jobStatus`
  FOREIGN KEY (`StatusId`)
  REFERENCES `jobstatus` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_jobResource_job`
  FOREIGN KEY (`JobId`)
  REFERENCES `job` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_jobResource_datarecordermetadata`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
