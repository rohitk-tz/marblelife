CREATE TABLE `batchuploadrecord` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `PaymentFrequencyId` BIGINT(20) NULL,
  `WaitPeriod` INT(11) NULL,
  `StartDate` DATE NOT NULL,
  `EndDate` DATE NOT NULL,
  `ExpectedUploadDate` DATE NOT NULL,
  `UploadedOn` DATE NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_batchUploadRecord_Franchisee_idx` (`FranchiseeId` ASC),
  INDEX `fk_batchUploadRecord_loookup_idx` (`PaymentFrequencyId` ASC),
  CONSTRAINT `fk_batchUploadRecord_Franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_batchUploadRecord_loookup`
    FOREIGN KEY (`PaymentFrequencyId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
