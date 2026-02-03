CREATE TABLE `techandsalesschedulerreminder` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CustomerId` bigint(20) NOT NULL,
  `CreatedOn` datetime NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `JobSchedulerId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_UserScheduleReminderAudit_Audit_idx` (`CustomerId`),
  KEY `fk_JobSchedulerIdss_idx` (`JobSchedulerId`),
  CONSTRAINT `fk_UserScheduleReminderAudit_Audit` FOREIGN KEY (`CustomerId`) REFERENCES `jobcustomer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_JobSchedulerIdss` FOREIGN KEY (`JobSchedulerId`) REFERENCES `jobscheduler` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
)