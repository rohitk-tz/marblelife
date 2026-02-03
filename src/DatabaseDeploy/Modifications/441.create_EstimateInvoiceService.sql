CREATE TABLE `EstimateInvoiceService` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `ServiceName`  varchar(1024) DEFAULT NULL,
  `Description`  varchar(1024) DEFAULT NULL,
  `Location`  varchar(1024) DEFAULT NULL,
  `TypeOfService`  varchar(1024) DEFAULT NULL,
  `StoneType`  varchar(1024) DEFAULT NULL,
  `StoneColor`  varchar(1024) DEFAULT NULL,
  `Price`  varchar(200) DEFAULT NULL,
  `EstimateInvoiceId` bigint(20) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_EstimateInvoiceService_datarecordermetadata_idx` (`DataRecorderMetaDataId`),
   KEY `fk_EstimateInvoiceService_estimateInvoice_idx` (`EstimateInvoiceId`),
  CONSTRAINT `fk_EstimateInvoiceService_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
   CONSTRAINT `fk_EstimateInvoiceService_estimateInvoice` FOREIGN KEY (`EstimateInvoiceId`) REFERENCES `EstimateInvoice` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;



ALTER TABLE `makalu`.`estimateinvoiceservice` 
ADD COLUMN `Option1` DECIMAL NULL DEFAULT NULL AFTER `TypeOfService`;

ALTER TABLE `makalu`.`estimateinvoiceservice` 
ADD COLUMN `Option2` DECIMAL(10,2) NULL DEFAULT NULL AFTER `Option1`,
ADD COLUMN `Option3` DECIMAL(10,2) NULL DEFAULT NULL AFTER `Option2`,
CHANGE COLUMN `Option1` `Option1` DECIMAL(10,2) NULL DEFAULT NULL ;



ALTER TABLE `makalu`.`estimateinvoiceservice` 
ADD COLUMN `InvoiceNumber` INT NULL DEFAULT NULL AFTER `Option3`;


ALTER TABLE `makalu`.`estimateinvoiceservice` 
ADD COLUMN `ServiceType` VARCHAR(1024) NULL DEFAULT NULL AFTER `EstimateInvoiceId`;

ALTER TABLE `makalu`.`estimateinvoiceservice` 
ADD COLUMN `notes` varchar(1028) NULL  AFTER `InvoiceNumber`;

ALTER TABLE `makalu`.`estimateinvoiceservice` 
ADD COLUMN `StoneType2` varchar(1028) NULL  AFTER `InvoiceNumber`;

