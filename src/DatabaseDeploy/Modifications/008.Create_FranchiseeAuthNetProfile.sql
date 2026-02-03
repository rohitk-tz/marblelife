
CREATE TABLE `FranchiseeAuthNetProfile` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `CMID` VARCHAR(128) NOT NULL ,
  `MerchantCustomerId` BIGINT(20) NOT NULL ,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL ,
  `IsDeleted` BIT(1) NULL DEFAULT b'0' ,
  `IsActive` BIT(1) NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_FranchiseeAuthNetProfile_Franchisee_idx` (`MerchantCustomerId` ASC)  ,
  CONSTRAINT `fk_FranchiseeAuthNetProfile_Franchisee`
    FOREIGN KEY (`MerchantCustomerId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
