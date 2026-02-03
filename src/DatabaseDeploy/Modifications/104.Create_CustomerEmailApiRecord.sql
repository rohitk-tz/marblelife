CREATE TABLE `customeremailapirecord` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `CustomerId` BIGINT(20) NOT NULL ,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `CustomerEmail` VARCHAR(512) NOT NULL ,
  `apiCustomerId` VARCHAR(512) NULL DEFAULT NULL ,
  `apiListId` VARCHAR(512) NULL DEFAULT NULL ,
  `apiEmailId` VARCHAR(512) NULL DEFAULT NULL ,
  `ErrorResponse` TEXT NULL DEFAULT NULL ,
  `status` VARCHAR(218) NULL DEFAULT NULL ,  
  `DateCreated` DATETIME NULL DEFAULT NULL ,
  `IsSynced` BIT(1) NOT NULL DEFAULT b'0' ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`ID`)  ,
  INDEX `fk_customerEmailapiRecord_customer_idx` (`CustomerId` ASC)  ,
  INDEX `fk_customerEmailApiRecord_Franchisee_idx` (`FranchiseeId` ASC),
  CONSTRAINT `fk_customerEmailapiRecord_customer`
    FOREIGN KEY (`CustomerId`)
    REFERENCES `customer` (`Id`),
 CONSTRAINT `fk_customerEmailApiRecord_Franchisee`
	FOREIGN KEY (`FranchiseeId`)
	REFERENCES `makalu`.`franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
