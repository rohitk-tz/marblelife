
create table  `CustomerSchedulerReminderAudit`
(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`CustomerId` BIGINT(20) NOT NULL,
`CreatedOn` DATETIME NOT NULL ,
`IsDeleted` bit(1) NOT NULL DEFAULT b'0',
 PRIMARY KEY (`Id`),
 INDEX `fk_CustomerScheduleReminderAudit_Audit_idx` (`CustomerId` ASC),
 CONSTRAINT `fk_CustomerScheduleReminderAudit_Audit`
 FOREIGN KEY (`CustomerId`)
 REFERENCES `jobcustomer` (`Id`)
 ON DELETE NO ACTION
 ON UPDATE NO ACTION
)