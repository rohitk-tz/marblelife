CREATE TABLE `CustomerSignature` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `EstimateCustomerId` bigint(20)  NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `CustomerId` bigint(20) NOT NULL,
    `EstimateInvoiceId` bigint(20)  NULL,
  `Signature` text NOT NULL,
  `Name` varchar(128)  NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_CustomerSignature_estimateCustomerId_idx` (`EstimateCustomerId`),
  KEY `fk_CustomerSignature_customerId_idx` (`CustomerId`),
   KEY `fk_CustomerSignature_estimateInvoiceId_idx` (`EstimateInvoiceId`),
  CONSTRAINT `fk_CustomerSignature_estimateCustomerId` FOREIGN KEY (`EstimateCustomerId`) REFERENCES `estimateinvoicecustomer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_CustomerSignature_customerId` FOREIGN KEY (`CustomerId`) REFERENCES `JobCustomer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_CustomerSignature_estimateInvoiceId` FOREIGN KEY (`EstimateInvoiceId`) REFERENCES `EstimateInvoice` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;


INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('37', 'CustomerSignature', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('242', '37', 'Before Completion', 'Before Completion', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('243', '37', 'After Completion', 'After Completion', '2', b'1', b'0');
