ALTER TABLE `makalu`.`marketingleadcalldetail` 
CHANGE COLUMN `DateAdded` `DateAdded` DATETIME NULL ;


ALTER TABLE `makalu`.`marketingleadcalldetailv2` 
CHANGE COLUMN `SetName` `SetName` VARCHAR(128) NULL ;



ALTER TABLE `marketingleadcalldetail` 
ADD COLUMN `IsFromNewAPI` BIGINT(20) NULL DEFAULT false AFTER `IsDeleted`;

ALTER TABLE `marketingleadcalldetailv2` 
ADD COLUMN `marketingleadcalldetailId` BIGINT(20) NULL DEFAULT null AFTER `IsDeleted`;
ALTER TABLE `marketingleadcalldetailv2` 
ADD CONSTRAINT `fk_marketingleadcalldetailV2_marketingleadcalldetailId`
  FOREIGN KEY (`marketingleadcalldetailId`)
  REFERENCES `marketingleadcalldetail` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;


ALTER TABLE `makalu`.`marketingleadcalldetailv2` 
DROP FOREIGN KEY `fk_marketingleadcalldetailv2_marketingleadcalldetailId`;
ALTER TABLE `makalu`.`marketingleadcalldetailv2` 
CHANGE COLUMN `CallDate` `CallDate` DATETIME NULL ,
CHANGE COLUMN `PhoneLabel` `PhoneLabel` VARCHAR(128) NULL ,
CHANGE COLUMN `TransferNumber` `TransferNumber` VARCHAR(128) NULL ,
CHANGE COLUMN `CallerId` `CallerId` VARCHAR(128) NULL ,
CHANGE COLUMN `EnteredZipCode` `EnteredZipCode` VARCHAR(128) NULL ,
CHANGE COLUMN `FirstName` `FirstName` VARCHAR(128) NULL ,
CHANGE COLUMN `LastName` `LastName` VARCHAR(128) NULL ,
CHANGE COLUMN `StreetAddress` `StreetAddress` VARCHAR(128) NULL ,
CHANGE COLUMN `City` `City` VARCHAR(128) NULL ,
CHANGE COLUMN `State` `State` VARCHAR(128) NULL ,
CHANGE COLUMN `ZipCode` `ZipCode` VARCHAR(128) NULL ,
CHANGE COLUMN `TalkMintues` `TalkMintues` VARCHAR(128) NULL ,
CHANGE COLUMN `TalkSeconds` `TalkSeconds` VARCHAR(128) NULL ,
CHANGE COLUMN `TotalMintues` `TotalMintues` VARCHAR(128) NULL ,
CHANGE COLUMN `TotalSeconds` `TotalSeconds` VARCHAR(128) NULL ,
CHANGE COLUMN `CallDuration` `CallDuration` VARCHAR(128) NULL ,
CHANGE COLUMN `Sid` `Sid` VARCHAR(128) NULL ,
CHANGE COLUMN `IvrResults` `IvrResults` VARCHAR(128) NULL ,
CHANGE COLUMN `Source` `Source` VARCHAR(128) NULL ,
CHANGE COLUMN `SourceNumber` `SourceNumber` VARCHAR(128) NULL ,
CHANGE COLUMN `Destination` `Destination` VARCHAR(128) NULL ,
CHANGE COLUMN `CallRoute` `CallRoute` VARCHAR(128) NULL ,
CHANGE COLUMN `CallStatus` `CallStatus` VARCHAR(128) NULL ,
CHANGE COLUMN `APPState` `APPState` VARCHAR(128) NULL ,
CHANGE COLUMN `RepeaSourceCaller` `RepeaSourceCaller` VARCHAR(128) NULL ,
CHANGE COLUMN `SourceCap` `SourceCap` VARCHAR(128) NULL ,
CHANGE COLUMN `CallRouteQualified` `CallRouteQualified` VARCHAR(128) NULL ,
CHANGE COLUMN `SourceQualified` `SourceQualified` VARCHAR(128) NULL ,
CHANGE COLUMN `IsDeleted` `IsDeleted` BIT(1) NULL DEFAULT b'0' ,
CHANGE COLUMN `marketingleadcalldetailId` `marketingleadcalldetailId` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`marketingleadcalldetailv2` 
CHANGE COLUMN `Reroute` `Reroute` VARCHAR(128) NULL ;


ALTER TABLE `invoiceItem` 
ADD COLUMN `itemOriginal` varchar(300) NULL  AFTER `IsDeleted;