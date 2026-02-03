ALTER TABLE `jobscheduler` 
ADD COLUMN `ParentJobId` BIGINT(20) NULL DEFAULT null AFTER `schedulerStatus`;