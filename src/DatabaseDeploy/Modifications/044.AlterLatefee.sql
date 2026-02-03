ALTER TABLE `latefeeinvoiceitem` 
DROP COLUMN `InterestRate`;


CREATE TABLE `interestrateinvoiceitem` (
  `Id` BIGINT NOT NULL,
  `Percentage` DECIMAL(10,2) NOT NULL,
  `WaitPeriod` INT NOT NULL,
  `ExpectedDate` DATE NULL,
  `Amount` DECIMAL(10,2) NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  CONSTRAINT `fk_interestrateinvoiceitem_invoiceitem`
    FOREIGN KEY (`Id`)
    REFERENCES `invoiceitem` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

ALTER TABLE `latefeeinvoiceitem` 
CHANGE COLUMN `LateFee` `Amount` DECIMAL(10,2) NULL DEFAULT NULL ;
