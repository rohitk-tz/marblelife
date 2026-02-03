ALTER TABLE `job` 
ADD COLUMN `EstimateId` BIGINT(20) NULL DEFAULT NULL ,
ADD INDEX `fk_job_jobEstimate_idx` (`EstimateId` ASC) ;
ALTER TABLE `job` 
ADD CONSTRAINT `fk_job_jobEstimate`
  FOREIGN KEY (`EstimateId`)
  REFERENCES `jobestimate` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `jobestimate` 
ADD COLUMN `Amount` DECIMAL(10,2) NOT NULL;



