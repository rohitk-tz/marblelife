ALTER TABLE `jobscheduler` 
ADD COLUMN `IsRepeat` BIGINT(20) NULL DEFAULT b'0' AFTER `schedulerStatus`;

SET SQL_SAFE_UPDATES = 0;
update `jobscheduler`  set isRepeat=b'1' where `ParentJobId` is not null;


SET SQL_SAFE_UPDATES = 1;



