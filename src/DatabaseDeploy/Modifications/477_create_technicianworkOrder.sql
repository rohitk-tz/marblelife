CREATE TABLE `technicianWorkOrder` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` varchar(300) not null,
  `WorkOrderId` bigint(20) NOT NULL,
  `IsActive` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_technicianWorkOrder_lookup_idx` (`WorkOrderId`),
  CONSTRAINT `fk_technicianWorkOrder_lookup` FOREIGN KEY (`WorkOrderId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION 
) ENGINE=InnoDB AUTO_INCREMENT=120 DEFAULT CHARSET=latin1;


CREATE TABLE `technicianWorkOrderForInvoice` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `TechnicianWorkOrderId` bigint(20) NOT NULL,
  `IsActive` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
`EstimateinvoiceId` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
   KEY `fk_technicianWorkOrderForInvoice_serviceId_idx` (`EstimateinvoiceId`),
  KEY `fk_technicianWorkOrderForInvoice_lookup_idx` (`TechnicianWorkOrderId`),
  CONSTRAINT `fk_technicianWorkOrderForInvoice_lookup` FOREIGN KEY (`TechnicianWorkOrderId`) REFERENCES `technicianWorkOrder` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_technicianWorkOrderForInvoice_serviceId` FOREIGN KEY (`EstimateinvoiceId`) REFERENCES `estimateinvoice` (`ID`) ON DELETE NO ACTION 
) ENGINE=InnoDB AUTO_INCREMENT=120 DEFAULT CHARSET=latin1;


ALTER TABLE `technicianWorkOrderForInvoice` 
ADD COLUMN `InvoiceNumber` BIGINT(20) NULL DEFAULT 0 AFTER `isDeleted`;