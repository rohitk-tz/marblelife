ALTER TABLE `franchiseesales` 
DROP FOREIGN KEY `fk_FranchiseeSales_Invoice1`;
ALTER TABLE `franchiseesales` 
CHANGE COLUMN `InvoiceId` `InvoiceId` BIGINT(20) NULL ,
ADD COLUMN `AccountCreditId` BIGINT(20) NULL AFTER `IsDeleted`,
ADD INDEX `fk_FranchiseeSales_AccountCredit1_idx` (`AccountCreditId` ASC);
ALTER TABLE `franchiseesales` 
ADD CONSTRAINT `fk_FranchiseeSales_Invoice1`
  FOREIGN KEY (`InvoiceId`)
  REFERENCES `invoice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
ADD CONSTRAINT `fk_FranchiseeSales_AccountCredit1`
  FOREIGN KEY (`AccountCreditId`)
  REFERENCES `accountcredit` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;