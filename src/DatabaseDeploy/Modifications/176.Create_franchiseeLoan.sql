CREATE TABLE `franchiseeloan` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Amount` DECIMAL(10,2) NOT NULL,
  `Duration` INT NOT NULL,
  `InterestratePerAnum` DECIMAL(10,2) NOT NULL,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `Description` VARCHAR(512) NULL DEFAULT NULL,
  `DateCreated` DATETIME NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_franchiseeLoan_franchisee_idx` (`FranchiseeId` ASC),
  CONSTRAINT `fk_franchiseeLoan_franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `franchiseeloanschedule` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `LoanId` BIGINT(20) NOT NULL,
  `DueDate` DATE NOT NULL,
  `Balance` DECIMAL(10,2) NOT NULL,
  `Interest` DECIMAL(10,2) NOT NULL,
  `Principal` DECIMAL(10,2) NOT NULL,
  `PayableAmount` DECIMAL(10,2) NOT NULL,
  `PaidAmount` DECIMAL(10,2) NULL DEFAULT NULL,
  `IsPrePaid` BIT(1) NOT NULL DEFAULT b'0',
  `InvoiceItemId` BIGINT(20) NULL DEFAULT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_franchiseeLoanSchedule_franchiseeLoan_idx` (`LoanId` ASC),
  INDEX `fk_franchiseeLoanSchedule_invoiceItem_idx` (`InvoiceItemId` ASC),
    CONSTRAINT `fk_franchiseeLoanSchedule_franchiseeLoan`
    FOREIGN KEY (`LoanId`)
    REFERENCES `franchiseeloan` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseeloanschedule_invoiceItem`
    FOREIGN KEY (`InvoiceItemId`)
    REFERENCES `invoiceitem` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
