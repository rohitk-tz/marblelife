 CREATE TABLE `beforeAfterImageMailAudit` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `SchedulerId` bigint(20) NULL,
  `beforeAfterCategoryIdBeforeImageId` bigint(20)  NULL,
  `beforeAfterCategoryIdAfterImageId` bigint(20)  NULL,
  `FileId` bigint(20)  NULL,
  `FranchiseeId` bigint(20) NULL,
  `NotificationQueueId` bigint(20) NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `CreatedOn` DateTime NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_beforeAfterImageMailAudit_file_idx` (`FileId`),
  KEY `fk_beforeAfterImageMailAudit_jobestimateservices_idx` (`beforeAfterCategoryIdBeforeImageId`),
  KEY `fk_beforeAfterImageMailAudit_jobestimateservices1_idx` (`beforeAfterCategoryIdAfterImageId`),
  KEY `fk_beforeAfterImageMailAudit_jobScheduler_idx` (`SchedulerId`),
  KEY `fk_beforeAfterImageMailAudit_organization_idx` (`FranchiseeId`),
   KEY `fk_beforeAfterImageMailAudit_notificationQueue_idx` (`NotificationQueueId`),
  

  CONSTRAINT `fk_beforeAfterImageMailAudit_file` FOREIGN KEY (`FileId`) REFERENCES `File` (`Id`),
  CONSTRAINT `fk_beforeAfterImageMailAudit_jobestimateservices` FOREIGN KEY (`beforeAfterCategoryIdBeforeImageId`) REFERENCES `jobestimateservices` (`Id`),
  CONSTRAINT `fk_beforeAfterImageMailAudit_jobestimateservices1` FOREIGN KEY (`beforeAfterCategoryIdAfterImageId`) REFERENCES `jobestimateservices` (`Id`),
  CONSTRAINT `fk_beforeAfterImageMailAudit_jobScheduler` FOREIGN KEY (`SchedulerId`) REFERENCES `jobScheduler` (`Id`),
  CONSTRAINT `fk_beforeAfterImageMailAudit_organization` FOREIGN KEY (`FranchiseeId`) REFERENCES `Organization` (`Id`),
  CONSTRAINT `fk_beforeAfterImageMailAudit_notificationQueue` FOREIGN KEY (`NotificationQueueId`) REFERENCES `notificationqueue` (`Id`)
)