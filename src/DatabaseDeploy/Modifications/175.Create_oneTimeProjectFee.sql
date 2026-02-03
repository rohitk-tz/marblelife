CREATE TABLE `onetimeprojectfee` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Amount` DECIMAL(10,2) NOT NULL,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `InvoiceItemId` BIGINT(20) NULL DEFAULT NULL,
  `Description` VARCHAR(512) NULL DEFAULT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_oneTimeProjectfee_franchiseeId_idx` (`FranchiseeId` ASC),
  INDEX `fk_oneTimeProjectfee_InvoiceItem_idx` (`InvoiceItemId` ASC),
  CONSTRAINT `fk_oneTimeProjectfee_franchiseeId`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_oneTimeProjectfee_InvoiceItem`
    FOREIGN KEY (`InvoiceItemId`)
    REFERENCES `invoiceitem` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

ALTER TABLE `onetimeprojectfee` 
ADD COLUMN `DateCreated` DATETIME NOT NULL AFTER `IsDeleted`;

