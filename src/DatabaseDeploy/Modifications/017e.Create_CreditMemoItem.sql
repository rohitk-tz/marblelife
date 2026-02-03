
CREATE TABLE `AccountCreditItem` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `AccountCreditId` BIGINT(20) NOT NULL ,
  `Description` VARCHAR(1024) NULL DEFAULT NULL ,
  `Amount`  Decimal(10, 2) NOT Null ,
  `IsDeleted` BIT Not NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_AccountCreditItem_AccountCredit_idx` (`AccountCreditId` ASC)  ,
  CONSTRAINT `fk_AccountCreditItem_AccountCredit`
    FOREIGN KEY (`AccountCreditId`)
    REFERENCES `AccountCredit` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
