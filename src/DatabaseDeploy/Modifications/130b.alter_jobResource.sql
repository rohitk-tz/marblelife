ALTER TABLE `jobresource` 
DROP FOREIGN KEY `JobResource_jobStatus`;
ALTER TABLE `jobresource` 
CHANGE COLUMN `StatusId` `StatusId` BIGINT(20) NULL;
ALTER TABLE `jobresource` 
ADD CONSTRAINT `JobResource_jobStatus`
  FOREIGN KEY (`StatusId`)
  REFERENCES `jobstatus` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
