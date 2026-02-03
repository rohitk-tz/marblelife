CREATE TABLE `FranchiseeSalesPayment` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeSalesId` BIGINT(20) NOT NULL,
  `InvoiceId` BIGINT(20) NOT NULL,
  `PaymentId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`) ,
INDEX `fk_FranchiseeSalesPayment_Invoice_idx` (`InvoiceId`),
INDEX `fk_FranchiseeSalesPayment_Payment_idx` (`PaymentId`),
INDEX `fk_FranchiseeSalesPayment_FranchiseeSalesId_idx` (`FranchiseeSalesId`),
CONSTRAINT `fk_franchiseesalespayment_Invoice`
  FOREIGN KEY (`InvoiceId`)
  REFERENCES `Invoice` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
CONSTRAINT `fk_FranchiseeSalesPayment_Payment`
  FOREIGN KEY (`PaymentId`)
  REFERENCES `Payment` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION,
CONSTRAINT `fk_FranchiseeSalesPayment_FranchiseeSalesId`
  FOREIGN KEY (`FranchiseeSalesId`)
  REFERENCES `FranchiseeSales` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION);

