ALTER TABLE `jobresource` 
ADD COLUMN `MeetingId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_resoruce_meetingid_idx` (`MeetingId` ASC);
