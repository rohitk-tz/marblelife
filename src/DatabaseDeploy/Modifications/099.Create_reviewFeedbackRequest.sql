CREATE TABLE `customerfeedbackrequest` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeSalesId` BIGINT(20) NOT NULL,
  `DateSend` DATETIME NOT NULL,
  `DataPacket` TEXT NULL,
  `CustomerReviewSystemRecordId` BIGINT(20) NOT NULL,
  `IsQueued` BIT(1) NOT NULL DEFAULT b'0',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`),
  INDEX `fk_customerfeedbackrequest_FranchiseeSales_idx` (`FranchiseeSalesId` ASC) ,
  INDEX `fk_customerfeedbackrequest_customerReviewSystemRecord_idx` (`CustomerReviewSystemRecordId` ASC) ,
  CONSTRAINT `fk_customerfeedbackrequest_FranchiseeSales`
    FOREIGN KEY (`FranchiseeSalesId`)
    REFERENCES `franchiseesales` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_customerfeedbackrequest_customerReviewSystemRecord`
    FOREIGN KEY (`CustomerReviewSystemRecordId`)
    REFERENCES `customerreviewsystemrecord` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
