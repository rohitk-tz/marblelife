CREATE TABLE `FranchiseeAccountCredit` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `CreditedOn` DATETIME NOT NULL,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `InvoiceId` BIGINT(20) NOT NULL,
  `Description` VARCHAR(1024) NULL DEFAULT NULL,
  `Amount` DECIMAL(10,2) NOT NULL,
  `RemainingAmount` DECIMAL(10,2) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) ,
  INDEX `FK_FranchiseeAccountCredit_Franchisee_idx` (`FranchiseeId` ASC),
  INDEX `FK_FranchiseeAccountCredit_Invoice_idx` (`InvoiceId` ASC),
  CONSTRAINT `FK_FranchiseeAccountCredit_Franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_FranchiseeAccountCredit_Invoice`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `Invoice` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);