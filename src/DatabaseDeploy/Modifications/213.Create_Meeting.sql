CREATE TABLE `meeting` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Title` varchar(1024) DEFAULT NULL,
  `ParentId` bigint(20) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `StartDate` datetime DEFAULT NULL,
  `EndDate` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `ParentId` (`ParentId`),
  CONSTRAINT `meeting_ibfk_1` FOREIGN KEY (`ParentId`) REFERENCES `meeting` (`Id`)
);

ALTER TABLE `jobscheduler` 
ADD CONSTRAINT `fk_jobScheduler_meeting`
  FOREIGN KEY (`MeetingId`)
  REFERENCES `meeting` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;