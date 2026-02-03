ALTER TABLE `jobnote` 
ADD COLUMN `VacationId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_jobNote_jobScheduler_idx` (`VacationId` ASC);
ALTER TABLE `jobnote` 
ADD CONSTRAINT `fk_jobNote_jobScheduler`
  FOREIGN KEY (`VacationId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;

ALTER TABLE `jobresource` 
ADD COLUMN `VacationId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_jobResource_jobScheduler_idx` (`VacationId` ASC);
ALTER TABLE `jobresource` 
ADD CONSTRAINT `fk_jobResource_jobScheduler`
  FOREIGN KEY (`VacationId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
