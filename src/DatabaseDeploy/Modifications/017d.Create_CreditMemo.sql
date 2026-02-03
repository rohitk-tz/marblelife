
CREATE TABLE `AccountCredit` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `CreditedOn` DATETIME NOT NULL,
  `CustomerId` BIGINT(20) NOT NULL ,
  `QbInvoiceNumber`  VARCHAR(16) NULL DEFAULT NULL ,
  `IsDeleted` BIT Not NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_AccountCredit_Customer_idx` (`CustomerId` ASC)  ,
  CONSTRAINT `fk_AccountCredit_Customer`
    FOREIGN KEY (`CustomerId`)
    REFERENCES `Customer` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
