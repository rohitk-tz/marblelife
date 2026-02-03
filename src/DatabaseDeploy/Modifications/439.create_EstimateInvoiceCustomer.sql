CREATE TABLE `EstimateInvoiceCustomer` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `StreetAddress`  varchar(1024) DEFAULT NULL,
  `CityName` varchar(100) NOT NULL,
  `StateName` varchar(100) NOT NULL,
`Email` varchar(100) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_EstimateInvoice_datarecordermetadata_idx` (`DataRecorderMetaDataId`),
  CONSTRAINT `fk_EstimateInvoice_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;



ALTER TABLE `makalu`.`estimateinvoicecustomer` 
ADD COLUMN `PhoneNumber1` VARCHAR(100) NULL AFTER `Email`;

ALTER TABLE `makalu`.`estimateinvoicecustomer` 
ADD COLUMN `PhoneNumber2` VARCHAR(100) NULL AFTER `PhoneNumber1`;

ALTER TABLE `makalu`.`estimateinvoicecustomer` 
ADD COLUMN `CustomerName` VARCHAR(100) NULL AFTER `PhoneNumber2`;


ALTER TABLE `makalu`.`estimateinvoicecustomer` 
CHANGE COLUMN `PhoneNumber1` `PhoneNumber1` VARCHAR(100) NULL ,
CHANGE COLUMN `PhoneNumber2` `PhoneNumber2` VARCHAR(100) NULL ;



;
