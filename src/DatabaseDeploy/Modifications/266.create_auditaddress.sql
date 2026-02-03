CREATE TABLE `auditaddressdiscrepancy` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseesalesId` BIGINT(20) NOT NULL ,
  `AddressLine1` varchar(100) NOT NULL ,
  `AddressLine2` varchar(100) NULL ,
  `ZipCode` varchar(100) NULL ,
  `City` varchar(100) NULL ,
  `State` varchar(100) NULL ,
  `IsDeleted` BIT Not NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_auditaddressdiscrepancy_franchiseesales_idx` (`franchiseesalesId` ASC),
  CONSTRAINT `fk_auditaddressdiscrepancy_Franchiseesales`
    FOREIGN KEY (`franchiseesalesId`)
    REFERENCES `franchiseesales` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
    

	  
  
    ALTER TABLE `auditaddressdiscrepancy`
ADD COLUMN `Country` VARCHAR(100) NULL DEFAULT NULL AFTER `IsDeleted`;

ALTER TABLE `auditaddressdiscrepancy`
ADD COLUMN `isUpdated` BIT(1) NULL DEFAULT b'0' AFTER `Country`;

ALTER TABLE `auditaddressdiscrepancy` ADD COLUMN `email` VARCHAR(45) NULL DEFAULT NULL AFTER `isUpdated`;

ALTER TABLE `auditaddressdiscrepancy`
ADD COLUMN `annualsalesdatauploadId` BIGINT(20) NULL,
ADD INDEX `fk_fauditaddress_lookup1_idx` (`annualsalesdatauploadId`);
ALTER TABLE `auditaddressdiscrepancy`
ADD CONSTRAINT `fk_auditaddressdiscrepancy_lookup1`
  FOREIGN KEY (`annualsalesdatauploadId`)
  REFERENCES `annualsalesdataupload` (`id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


  