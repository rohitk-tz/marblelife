 
SET SQL_SAFE_UPDATES = 0;
update job set jobTypeid = 4 where id > 0 and isdeleted = 0;
SET SQL_SAFE_UPDATES = 1;

ALTER TABLE `job` 
DROP FOREIGN KEY `fk_job_jobType`;
ALTER TABLE `job` 
ADD CONSTRAINT `fk_job_jobType`
  FOREIGN KEY (`JobTypeId`)
  REFERENCES `marketingclass` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
