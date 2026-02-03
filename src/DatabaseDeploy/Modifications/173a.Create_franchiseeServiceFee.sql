CREATE TABLE `franchiseeServiceFee` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `ServiceFeeTypeId` BIGINT(20) NOT NULL,
  `Amount` DECIMAL(10,2) NOT NULL,
  `Percentage` DECIMAL(10,2) NULL DEFAULT NULL,
  `FrequencyId` BIGINT(20) NULL DEFAULT NULL,
  `IsActive` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY(`Id`),
  INDEX `fk_franchiseeServiceFee_Franchisee_idx` (`FranchiseeId` ASC),
  INDEX `fk_franchiseeServiceFee_franchiseeServiceFeeTypeId_idx` (`ServiceFeeTypeId` ASC),
  INDEX `fk_franchiseeServiceFee_Frequency_idx` (`FrequencyId` ASC),
  CONSTRAINT `fk_franchiseeServiceFee_Franchisee`
    FOREIGN KEY(`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseeServiceFee_ServiceFeeTypeId`
    FOREIGN KEY(`ServiceFeeTypeId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseeServiceFee_Frequency`
	FOREIGN KEY(`FrequencyId`)
    REFERENCES `lookup` (`Id`)
	ON DELETE NO ACTION
    ON UPDATE NO ACTION);

CREATE TABLE `servicefeeinvoiceitem` (
  `Id` BIGINT(20) NOT NULL,
  `StartDate` DATE NOT NULL,
  `EndDate` DATE NOT NULL,
  `ServiceFeeTypeId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY(`Id`),
  INDEX `fk_serviceFeeInvoiceItem_Lookup1_idx` (`ServiceFeeTypeId` ASC),
  CONSTRAINT `fk_serviceFeeInvoiceItem_InvoiceItem`
    FOREIGN KEY(`Id`)
    REFERENCES `invoiceitem` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_serviceFeeInvoiceItem_Lookup1`
    FOREIGN KEY(`ServiceFeeTypeId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
