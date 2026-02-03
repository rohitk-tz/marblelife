CREATE TABLE `systemauditrecord` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `InvoiceId` BIGINT(20) NOT NULL,
  `QBIdentifier` VARCHAR(45) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `FranchiseeId` BIGINT(20) NOT NULL,
  `annualUploadId` BIGINT(20) NOT NULL,
  PRIMARY KEY (`ID`),
  INDEX `fk_systemAuditRecord_invoice_idx` (`InvoiceId` ASC),
  INDEX `fk_systemAuditRecord_franchisee_idx` (`FranchiseeId` ASC),
  INDEX `fk_systemAuditRecord_annualSalesDataUpload_idx` (`annualUploadId` ASC),
  CONSTRAINT `fk_systemAuditRecord_invoice`
    FOREIGN KEY (`InvoiceId`)
    REFERENCES `invoice` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_systemAuditRecord_franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_systemAuditRecord_annualSalesDataUpload`
    FOREIGN KEY (`annualUploadId`)
    REFERENCES `annualsalesdataupload` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
