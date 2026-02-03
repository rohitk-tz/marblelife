CREATE TABLE `bulkUploadImages` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) DEFAULT NULL,
  `FranchiseeName` Varchar(128) DEFAULT NULL,
  `CustomerId` bigint(20) DEFAULT NULL,
  `CustomerName` Varchar(256) DEFAULT NULL,
  `ZipFileUploadId` bigint(20) DEFAULT NULL,
  `DateCreated` datetime default NULL,
  `CreatedBy` bigint(20) DEFAULT NULL,
  `DateModified` datetime default NULL,
  `ModifiedBy` bigint(20) DEFAULT NULL,
  `MarketingClassId` bigint(20) DEFAULT NULL,
  `SurfaceMaterial` Varchar(128) DEFAULT NULL,
  `ServiceType` Varchar(128) DEFAULT NULL,
  `SurfaceType` Varchar(128) DEFAULT NULL,
  `SurfaceColor` Varchar(128) DEFAULT NULL,
  `FinishType` Varchar(128) DEFAULT NULL,
  `BuildingLocation` Varchar(128) DEFAULT NULL,
  `ManagementCompany` Varchar(128) DEFAULT NULL,
  `MaidService` Varchar(128) DEFAULT NULL,
  `S3BucketURL` Varchar(128) DEFAULT NULL,
  `S3BucketThumbURL` Varchar(128) DEFAULT NULL,
  `Rotation` bigint(20) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  PRIMARY KEY (`Id`),
  
  KEY `fk_bulkUploadImages_customer_idx` (`CustomerId`),
  CONSTRAINT `fk_bulkUploadImages_customer` 
  FOREIGN KEY (`CustomerId`)
  REFERENCES `customer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_bulkUploadImages_franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_bulkUploadImages_franchisee` 
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_bulkUploadImages_zipFileForBulkUpload_idx` (`ZipFileUploadId`),
  CONSTRAINT `fk_bulkUploadImages_zipFileForBulkUpload` 
  FOREIGN KEY (`ZipFileUploadId`)
  REFERENCES `zipFileForBulkUpload` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_bulkUploadImages_marketingclass_idx` (`MarketingClassId`),
  CONSTRAINT `fk_bulkUploadImages_marketingclass` 
  FOREIGN KEY (`MarketingClassId`)
  REFERENCES `marketingclass` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE bulkUploadZipFile
MODIFY COLUMN S3BucketURL VARCHAR(500);