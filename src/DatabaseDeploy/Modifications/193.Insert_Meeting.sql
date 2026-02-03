ALTER TABLE `jobscheduler` 
ADD COLUMN `MeetingId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_jobScheduler_meeting_idx` (`MeetingId` ASC);
