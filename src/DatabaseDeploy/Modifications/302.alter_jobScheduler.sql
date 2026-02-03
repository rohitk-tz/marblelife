ALTER TABLE `jobscheduler` 
ADD COLUMN `PersonId` BIGINT(20) NULL,
ADD INDEX `fk_jobScheduler_persons_idx` (`FranchiseeId`);
ALTER TABLE `jobscheduler` 
ADD CONSTRAINT `fk_jobScheduler_persons`
  FOREIGN KEY (`PersonId`)
  REFERENCES `person` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;