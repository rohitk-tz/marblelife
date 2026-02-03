CREATE TABLE `auditinvoice` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `InvoiceId` BIGINT(20) NULL DEFAULT NULL,
  `QBInvoiceNumber` VARCHAR(45) NOT NULL,
  `GeneratedOn` DATETIME NOT NULL,
  `StatusId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`),
  INDEX `fk_auditInvoice_lookup_idx` (`StatusId` ASC),
  INDEX `fk_auditinvoice_invoice_idx` (`InvoiceId` ASC),
  CONSTRAINT `fk_auditInvoice_lookup`
    FOREIGN KEY (`StatusId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_auditinvoice_invoice`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `invoice` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `auditpayment` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Date` DATE NOT NULL ,
  `Amount` DECIMAL(10,2) NOT NULL,
  `InstrumentTypeId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`));

CREATE TABLE `auditinvoiceitem` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `AuditInvoiceId` BIGINT(20) NOT NULL,
  `ItemId` BIGINT(20) NULL DEFAULT NULL,
  `ItemTypeId` BIGINT(20) NOT NULL,
  `Description` VARCHAR(1024) NULL DEFAULT NULL,
  `Quantity` DECIMAL(8,2) NULL DEFAULT NULL,
  `Rate` DECIMAL(10,2) NOT NULL,
  `Amount` DECIMAL(10,2) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  CONSTRAINT `fk_auditInvoiceItem_auditInvoice`
    FOREIGN KEY (`AuditInvoiceId`)
    REFERENCES `auditinvoice` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `auditpaymentitem` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `PaymentId` BIGINT(20) NOT NULL,
  `ItemId` BIGINT(20) NOT NULL,
  `ItemTypeId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_auditPaymentItem_auditpayment_idx` (`PaymentId` ASC),
  CONSTRAINT `fk_auditPaymentItem_auditpayment`
    FOREIGN KEY (`PaymentId`)
    REFERENCES `auditpayment` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `auditinvoicepayment` (
  `InvoiceId` BIGINT(20) NOT NULL,
  `PaymentId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`InvoiceId`, `PaymentId`),
  INDEX `fk_auditinvoicepayment_payment_idx` (`PaymentId` ASC),
  CONSTRAINT `fk_auditinvoicepayment_invoice`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `auditinvoice` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_auditinvoicepayment_payment`
    FOREIGN KEY (`PaymentId`)
    REFERENCES `auditpayment` (`ID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

ALTER TABLE `auditinvoice` 
ADD COLUMN `annualUploadId` BIGINT(20) NOT NULL,
ADD INDEX `fk_auditinvoice_annualsalesdataUpload_idx` (`annualUploadId` ASC);
ALTER TABLE `auditinvoice` 
ADD CONSTRAINT `fk_auditinvoice_annualsalesdataUpload`
  FOREIGN KEY (`annualUploadId`)
  REFERENCES `annualsalesdataupload` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  