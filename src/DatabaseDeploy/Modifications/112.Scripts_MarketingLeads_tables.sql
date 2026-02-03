CREATE TABLE `marketingleadcalldetail` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `SessionId` VARCHAR(128) NOT NULL ,
  `DateAdded` DATETIME NOT NULL ,
  `DialedNumber` VARCHAR(45) NOT NULL ,
  `CallerId` VARCHAR(45) NOT NULL ,
  `CallTypeId` BIGINT(20) NOT NULL ,
  `CallTransferType` VARCHAR(512) NULL DEFAULT NULL ,
  `PhoneLabel` VARCHAR(512) NULL DEFAULT NULL ,
  `TransferToNumber` VARCHAR(45) NULL DEFAULT NULL ,
  `ClickDescription` VARCHAR(512) NULL DEFAULT NULL ,
  `CallDuration` INT NULL DEFAULT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_MarketingLeadCallDetail_lookup_idx` (`CallTypeId` ASC)  ,
  CONSTRAINT `fk_MarketingLeadCallDetail_lookup`
    FOREIGN KEY (`CallTypeId`)
    REFERENCES `lookup` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

 
INSERT INTO `lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('17', 'CallType', 0);

INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('140', '17', 'Inbound', 'Inbound', '1', 1, 0);
INSERT INTO `lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('141', '17', 'Outbound', 'Outbound', '2', 1, 0);


CREATE TABLE `routingnumber` (
  `Id` BIGINT NOT NULL AUTO_INCREMENT ,
  `PhoneNumber` VARCHAR(45) NOT NULL ,
  `PhoneLabel` VARCHAR(128) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`));


