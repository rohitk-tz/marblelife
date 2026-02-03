CREATE TABLE `bulkuploadmarketingclass` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `MarketingClassId` bigint(20) DEFAULT NULL,
  `BulkUploadImageId` bigint(20) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'1',
  PRIMARY KEY (`Id`),
  
  KEY `fk_bulkuploadmarketingclass_marketingclass_idx` (`MarketingClassId`),
  CONSTRAINT `fk_bulkuploadmarketingclass_marketingclass` 
  FOREIGN KEY (`MarketingClassId`)
  REFERENCES `marketingclass` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_bulkuploadmarketingclass_bulkuploadimages_idx` (`BulkUploadImageId`),
  CONSTRAINT `fk_bulkuploadmarketingclass_bulkuploadimages` 
  FOREIGN KEY (`BulkUploadImageId`)
  REFERENCES `bulkuploadimages` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;