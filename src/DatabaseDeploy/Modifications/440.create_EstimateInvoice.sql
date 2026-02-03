CREATE TABLE `EstimateInvoice` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `FranchiseeId` bigint(20) NOT NULL,
  `CustomerId` bigint(20)  NULL,
  `InvoiceCustomerId` bigint(20) NULL,
  `PriceOfService`  decimal(18,2) DEFAULT NULL,
  `LessDeposit`  decimal(18,2) DEFAULT NULL,
  `ClassTypeId` bigint(20) NOT NULL,
  `ServiceTypeId` bigint(20) NOT NULL,
   `EstimateId` bigint(20)  NULL,
  `SchedulerId` bigint(20)  NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_EstimateInvoice_datarecordermetadata1_idx` (`DataRecorderMetaDataId`),
  KEY `fk_EstimateInvoice_franchisee1_idx` (`FranchiseeId`),
  KEY `fk_EstimateInvoice_jobCustomer_idx` (`CustomerId`),
  KEY `fk_EstimateInvoice_jobCustomer1_idx` (`InvoiceCustomerId`),
  KEY `fk_EstimateInvoice_MarketingClass1_idx` (`ClassTypeId`),
  KEY `fk_EstimateInvoice_ServiceTypeId_idx` (`ServiceTypeId`),
   KEY `fk_EstimateInvoice_JobEstimate_idx` (`EstimateId`),
  KEY `fk_EstimateInvoice_JobScheduler_idx` (`SchedulerId`),
  CONSTRAINT `fk_EstimateInvoice_datarecordermetadata1` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EstimateInvoice_franchisee1` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EstimateInvoice_jobCustomer` FOREIGN KEY (`CustomerId`) REFERENCES `jobcustomer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EstimateInvoice_jobCustomer1` FOREIGN KEY (`InvoiceCustomerId`) REFERENCES `EstimateInvoiceCustomer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EstimateInvoice_MarketingClass1` FOREIGN KEY (`ClassTypeId`) REFERENCES `marketingclass` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EstimateInvoice_ServiceTypeId` FOREIGN KEY (`ServiceTypeId`) REFERENCES `servicetype` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
   CONSTRAINT `fk_EstimateInvoice_JobEstimate` FOREIGN KEY (`EstimateId`) REFERENCES `JobEstimate` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_EstimateInvoice_JobScheduler` FOREIGN KEY (`SchedulerId`) REFERENCES `JobScheduler` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;


ALTER TABLE `makalu`.`estimateinvoice` 
ADD COLUMN `NumberOfInvoices` VARCHAR(45) NULL AFTER `SchedulerId`,
ADD COLUMN `estimateinvoicecol` INT NULL DEFAULT NULL AFTER `NumberOfInvoices`;


ALTER TABLE `makalu`.`estimateinvoice` 
ADD COLUMN `Option` varchar(1028) NULL  AFTER `NumberOfInvoices`;
ALTER TABLE `makalu`.`estimateinvoice` 
DROP FOREIGN KEY `fk_EstimateInvoice_ServiceTypeId`;
ALTER TABLE `makalu`.`estimateinvoice` 
DROP COLUMN `ServiceTypeId`,
DROP INDEX `fk_EstimateInvoice_ServiceTypeId_idx` ;