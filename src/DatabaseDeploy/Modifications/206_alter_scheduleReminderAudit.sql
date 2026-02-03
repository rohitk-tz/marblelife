ALTER TABLE `customerschedulerreminderaudit` 
ADD COLUMN `JobSchedulerId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_JobSchedulerId_idx` (`JobSchedulerId` ASC);
ALTER TABLE `customerschedulerreminderaudit` 
ADD CONSTRAINT `fk_JobSchedulerId`
  FOREIGN KEY (`JobSchedulerId`)
  REFERENCES `JobScheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;