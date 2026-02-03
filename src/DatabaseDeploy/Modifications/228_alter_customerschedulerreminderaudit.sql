ALTER TABLE `customerschedulerreminderaudit` 
ADD COLUMN `JobId` BIGINT(20) NULL DEFAULT NULL ,
ADD INDEX `fk_job_jobid_mail_idx` (`JobId` ASC) ;
ALTER TABLE `customerschedulerreminderaudit` 
ADD CONSTRAINT `fk_job_jobid_mail`
  FOREIGN KEY (`JobId`)
  REFERENCES `job` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
