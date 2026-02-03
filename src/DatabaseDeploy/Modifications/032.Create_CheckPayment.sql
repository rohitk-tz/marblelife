CREATE TABLE `CheckPayment` (
  `Id` BIGINT(20) NOT NULL ,
  `CheckId` BIGINT(20) NOT NULL,
  `Amount` DECIMAL(10,2) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)  COMMENT '',
  INDEX `fk_CheckPayment_Check_idx` (`CheckId` ASC) ,
  CONSTRAINT `fk_CheckPayment_Payment`
    FOREIGN KEY (`Id`)
    REFERENCES `Payment` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_CheckPayment_Check`
    FOREIGN KEY (`CheckId`)
    REFERENCES `Check` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
