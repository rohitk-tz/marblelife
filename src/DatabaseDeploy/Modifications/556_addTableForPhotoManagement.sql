
CREATE TABLE `photomanagementforbulkupload` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `BeforeImageId` bigint(20) DEFAULT NULL,  
  `AfterImageId` bigint(20) DEFAULT NULL,  
  `ExteriorImageId` bigint(20) DEFAULT NULL,  
  `DateCreated` datetime default NULL,
  `DateModified` datetime default NULL,
  `RoleId` bigint(20) DEFAULT NULL, 
  `UserIdId` bigint(20) DEFAULT NULL,
  `FranchiseeId` bigint(20) DEFAULT NULL, 
  `CustomerName` Varchar(256) DEFAULT NULL,
  `MarketingClassId` bigint(20) DEFAULT NULL,
  `SurfaceMaterial` Varchar(128) DEFAULT NULL,
  `ServiceType` Varchar(128) DEFAULT NULL,
  `SurfaceType` Varchar(128) DEFAULT NULL,
  `SurfaceColor` Varchar(128) DEFAULT NULL,
  `FinishType` Varchar(128) DEFAULT NULL,
  `BuildingLocation` Varchar(128) DEFAULT NULL,
  `ManagementCompany` Varchar(128) DEFAULT NULL,
  `MaidService` Varchar(128) DEFAULT NULL,
  `IsBestPair` bit(1) DEFAULT b'0',
  `BestPairMarkedBy` bigint(20) DEFAULT NULL,  
  `IsStarMark` bit(1) DEFAULT b'0',
  `IsFlagMark` bit(1) DEFAULT b'0',
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'1',
  PRIMARY KEY (`Id`),
  
  KEY `fk_photomanagementforbulkupload_franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_photomanagementforbulkupload_franchisee` 
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_photomanagementforbulkupload_bulkUploadImagesBeforeImage_idx` (`BeforeImageId`),
  CONSTRAINT `fk_photomanagementforbulkupload_bulkUploadImagesBeforeImage` 
  FOREIGN KEY (`BeforeImageId`)
  REFERENCES `bulkUploadImages` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_photomanagementforbulkupload_bulkUploadImagesAfterImage_idx` (`AfterImageId`),
  CONSTRAINT `fk_photomanagementforbulkupload_bulkUploadImagesAfterImage` 
  FOREIGN KEY (`AfterImageId`)
  REFERENCES `bulkUploadImages` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_photomanagementforbulkupload_bulkUploadImages_idx` (`ExteriorImageId`),
  CONSTRAINT `fk_photomanagementforbulkupload_bulkUploadImagesExteriorImage` 
  FOREIGN KEY (`ExteriorImageId`)
  REFERENCES `bulkUploadImages` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

ALTER TABLE photomanagementforbulkupload
ADD IsReviewed bit(1) DEFAULT b'0';