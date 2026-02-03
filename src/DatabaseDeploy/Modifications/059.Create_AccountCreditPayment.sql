CREATE TABLE `AccountCreditPayment` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `PaymentId` BIGINT(20) NOT NULL ,
  `FranchiseeAccountCreditId` BIGINT(20) NOT NULL ,
  `Amount` DECIMAL(10,2) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) ,
  INDEX `FK_AccountCreditPayment_FranchiseeAccountCredit_idx` (`FranchiseeAccountCreditId` ASC) ,
  INDEX `FK_AccountCreditPayment_Payment_idx` (`PaymentId` ASC) ,
  CONSTRAINT `FK_AccountCreditPayment_Payment`
    FOREIGN KEY (`PaymentId`)
    REFERENCES `Payment` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `FK_AccountCreditPayment_FranchiseeAccountCredit`
    FOREIGN KEY (`FranchiseeAccountCreditId`)
    REFERENCES `FranchiseeAccountCredit` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


