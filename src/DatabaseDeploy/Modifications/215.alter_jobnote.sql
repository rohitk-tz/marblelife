ALTER TABLE `jobnote` 
ADD COLUMN `MeetingId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_meetingid_idx` (`MeetingId` ASC);
ALTER TABLE `jobnote` 
ADD CONSTRAINT `fk_meetingid`
  FOREIGN KEY (`MeetingId`)
  REFERENCES `jobscheduler` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;