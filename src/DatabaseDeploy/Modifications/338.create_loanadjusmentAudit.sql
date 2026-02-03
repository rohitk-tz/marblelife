CREATE TABLE `loanadjustmentAudit` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `CreatedOn` datetime NOT NULL,
  `UserId` bigint(20) DEFAULT NULL,
  `LoanId` bigint(20) DEFAULT NULL,
  `BeforeLoanAdjustment` BIT(1) DEFAULT NULL,
  `AfterLoanAdjustment` BIT(1) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  CONSTRAINT `fk_loanadjustmentAudit_user_id` FOREIGN KEY (`UserId`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_loanadjustmentAudit_franchiseeloan_id` FOREIGN KEY (`LoanId`) REFERENCES `franchiseeloan` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
  )