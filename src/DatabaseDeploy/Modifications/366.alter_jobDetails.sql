ALTER TABLE `makalu`.`jobdetails` 
DROP FOREIGN KEY `fk_jobDetails_jobStatus`;
ALTER TABLE `makalu`.`jobdetails` 
CHANGE COLUMN `StatusId` `StatusId` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`jobdetails` 
ADD CONSTRAINT `fk_jobDetails_jobStatus`
  FOREIGN KEY (`StatusId`)
  REFERENCES `makalu`.`jobstatus` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  ALTER TABLE `makalu`.`jobdetails` 
DROP FOREIGN KEY `fk_jobDetails_job`;
ALTER TABLE `makalu`.`jobdetails` 
CHANGE COLUMN `JobId` `JobId` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`jobdetails` 
ADD CONSTRAINT `fk_jobDetails_job`
  FOREIGN KEY (`JobId`)
  REFERENCES `makalu`.`job` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;



  ALTER TABLE `makalu`.`jobdetails` 
DROP FOREIGN KEY `fk_jobDetails_jobType`;
ALTER TABLE `makalu`.`jobdetails` 
CHANGE COLUMN `JobTypeId` `JobTypeId` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`jobdetails` 
ADD CONSTRAINT `fk_jobDetails_jobType`
  FOREIGN KEY (`JobTypeId`)
  REFERENCES `makalu`.`marketingclass` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
