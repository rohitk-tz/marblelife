ALTER TABLE `jobnote` 
ADD INDEX `fk_jobNote_job_idx` (`JobId` ASC) ;
ALTER TABLE `jobnote` 
ADD CONSTRAINT `fk_jobNote_job`
  FOREIGN KEY (`JobId`)
  REFERENCES `job` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `jobnote` 
ADD CONSTRAINT `fk_jobNote_jobStatus`
  FOREIGN KEY (`StatusId`)
  REFERENCES `jobstatus` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  ALTER TABLE `jobnote` 
ADD INDEX `fk_jobNote_estimate_idx` (`EstimateId` ASC) ;
ALTER TABLE `jobnote` 
ADD CONSTRAINT `fk_jobNote_estimate`
  FOREIGN KEY (`EstimateId`)
  REFERENCES `jobestimate` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
