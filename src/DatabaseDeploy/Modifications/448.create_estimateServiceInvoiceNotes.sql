CREATE TABLE `EstimateServiceInvoiceNotes` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   `Notes` text DEFAULT NULL,
  `UserId` bigint(20) NOT NULL,
  `EstimateinvoiceId` bigint(20) NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `fk_estimateServiceInvoiceNotes_datarecordermetadata_idx` (`DataRecorderMetaDataId`),
  KEY `fk_estimateServiceInvoiceNotes_userId_idx` (`UserId`),
  KEY `fk_estimateServiceInvoiceNotes_serviceId_idx` (`EstimateinvoiceId`),
  CONSTRAINT `fk_estimateServiceInvoiceNotes_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_estimateServiceInvoiceNotes_userId` FOREIGN KEY (`UserId`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
   CONSTRAINT `fk_estimateServiceInvoiceNotes_serviceId` FOREIGN KEY (`EstimateinvoiceId`) REFERENCES `estimateinvoice` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1