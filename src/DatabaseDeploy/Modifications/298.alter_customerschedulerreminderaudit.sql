ALTER TABLE `customerschedulerreminderaudit` 
ADD COLUMN `EstimateId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_customerschedulerreminderaudit_estimate_idx` (`EstimateId` ASC);
ALTER TABLE `customerschedulerreminderaudit` 
ADD CONSTRAINT `fk_customerschedulerreminderaudit_estimate`
  FOREIGN KEY (`EstimateId`)
  REFERENCES `jobestimate` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;