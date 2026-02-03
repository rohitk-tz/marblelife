
CREATE TABLE `surfacematerial` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` Varchar(128) DEFAULT NULL,
  `Alias` Varchar(128) DEFAULT NULL,
  `Description` Varchar(128) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  `OrderBy` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `surfacetype` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` Varchar(128) DEFAULT NULL,
  `Alias` Varchar(128) DEFAULT NULL,
  `Description` Varchar(128) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  `OrderBy` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `surfacecolor` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` Varchar(128) DEFAULT NULL,
  `Alias` Varchar(128) DEFAULT NULL,
  `Description` Varchar(128) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  `OrderBy` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


CREATE TABLE `finishtype` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` Varchar(128) DEFAULT NULL,
  `Alias` Varchar(128) DEFAULT NULL,
  `Description` Varchar(128) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  `OrderBy` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `buildinglocation` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `Name` Varchar(128) DEFAULT NULL,
  `Alias` Varchar(128) DEFAULT NULL,
  `Description` Varchar(128) DEFAULT NULL,
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  `OrderBy` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Surface Material
insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(1, "Carpet", "Carpet", "", b'0', b'1', 1);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(2, "Ceramic (tile and grout)", "Ceramic (tile and grout)", "", b'0', b'1', 2);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(3, "Enduracrete: concrete", "Enduracrete: concrete", "", b'0', b'1', 3);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(4, "Glass", "Glass", "", b'0', b'1', 4);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(5, "Granite", "Granite", "", b'0', b'1', 5);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(6, "Limestone", "Limestone", "", b'0', b'1', 6);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(7, "Marble", "Marble", "", b'0', b'1', 7);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(8, "Metal", "Metal", "", b'0', b'1', 8);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(9, "Other", "Other", "", b'0', b'1', 9);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(10, "Porcelain", "Porcelain", "", b'0', b'1', 10);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(11, "Quartz", "Quartz", "", b'0', b'1', 11);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(12, "Slate", "Slate", "", b'0', b'1', 12);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(13, "Terrazzo", "Terrazzo", "", b'0', b'1', 13);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(14, "Travertine", "Travertine", "", b'0', b'1', 14);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(15, "Vinyl", "Vinyl", "", b'0', b'1', 15);

insert into surfacematerial(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(16, "Wood", "Wood", "", b'0', b'1', 16);

-- Surface Type
insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(1, "Building Exterior", "Building Exterior", "", b'0', b'1', 1);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(2, "Concrete", "Concrete", "", b'0', b'1', 2);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(3, "Counter (Kitchen)", "Counter (Kitchen)", "", b'0', b'1', 3);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(4, "Fireplace", "Fireplace", "", b'0', b'1', 4);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(5, "Floor", "Floor", "", b'0', b'1', 5);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(6, "Other", "Other", "", b'0', b'1', 6);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(7, "Patio", "Patio", "", b'0', b'1', 7);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(8, "Pool Deck", "Pool Deck", "", b'0', b'1', 8);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(9, "Shower Wall", "Shower Wall", "", b'0', b'1', 9);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(10, "Sign", "Sign", "", b'0', b'1', 10);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(11, "Threshold", "Threshold", "", b'0', b'1', 11);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(12, "Tub Deck", "Tub Deck", "", b'0', b'1', 12);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(13, "Vanity (Bathroom)", "Vanity (Bathroom)", "", b'0', b'1', 13);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(14, "Walkway", "Walkway", "", b'0', b'1', 14);

insert into surfacetype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(15, "Wall", "Wall", "", b'0', b'1', 15);


-- Surface Color
insert into surfacecolor(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(1, "Black", "Black", "", b'0', b'1', 1);

insert into surfacecolor(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(2, "Brown", "Brown", "", b'0', b'1', 2);

insert into surfacecolor(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(3, "Green", "Green", "", b'0', b'1', 3);

insert into surfacecolor(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(4, "Other", "Other", "", b'0', b'1', 4);

insert into surfacecolor(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(5, "Red", "Red", "", b'0', b'1', 5);

insert into surfacecolor(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(6, "Tan", "Tan", "", b'0', b'1', 6);

insert into surfacecolor(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(7, "White", "White", "", b'0', b'1', 7);


-- Finish Type
insert into finishtype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(1, "Gloss", "Gloss", "", b'0', b'1', 1);

insert into finishtype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(2, "Matte", "Matte", "", b'0', b'1', 2);

insert into finishtype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(3, "Satin/Semi-Gloss", "Satin/Semi-Gloss", "", b'0', b'1', 3);

insert into finishtype(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(4, "Ultimate Finish", "Ultimate Finish", "", b'0', b'1', 4);


-- Building Location
insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(1, "Baseboards", "Baseboards", "", b'0', b'1', 1);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(2, "Basement", "Basement", "", b'0', b'1', 2);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(3, "Bathroom", "Bathroom", "", b'0', b'1', 3);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(4, "Bedroom", "Bedroom", "", b'0', b'1', 4);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(5, "Dining", "Dining", "", b'0', b'1', 5);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(6, "Elevator", "Elevator", "", b'0', b'1', 6);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(7, "Exterior Feature", "Exterior Feature", "", b'0', b'1', 7);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(8, "Floor", "Floor", "", b'0', b'1', 8);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(9, "Foyer", "Foyer", "", b'0', b'1', 9);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(10, "Garage", "Garage", "", b'0', b'1', 10);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(11, "Hall", "Hall", "", b'0', b'1', 11);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(12, "Kitchen", "Kitchen", "", b'0', b'1', 12);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(13, "Laundry", "Laundry", "", b'0', b'1', 13);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(14, "Lobby", "Lobby", "", b'0', b'1', 14);

insert into buildinglocation(Id, Name, Alias, Description, IsDeleted, IsActive, OrderBy)
value(15, "Living", "Living", "", b'0', b'1', 15);


CREATE TABLE `bulkUploadZipFile` (
`Id` bigint(20) NOT NULL AUTO_INCREMENT,
`Name` Varchar(128) DEFAULT NULL,
`Caption` Varchar(128) DEFAULT NULL,
`Size` bigint(20) DEFAULT NULL,
`S3BucketURL` Varchar(128) DEFAULT NULL,
`Extension` Varchar(128) DEFAULT NULL,
`DateCreated` datetime default NULL,
`IsDeleted` bit(1) DEFAULT b'0',
PRIMARY KEY (`Id`)
)ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `zipFileForBulkUpload` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) DEFAULT NULL,
  `FranchiseeName` Varchar(128) DEFAULT NULL,
  `CustomerId` bigint(20) DEFAULT NULL,
  `CustomerName` Varchar(256) DEFAULT NULL,
  `ZipFileId` bigint(20) DEFAULT NULL,
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
  `IsDeleted` bit(1) DEFAULT b'0',
  `IsActive` bit(1) DEFAULT b'0',
  PRIMARY KEY (`Id`),
  
  KEY `fk_zipFileForBulkUpload_customer_idx` (`CustomerId`),
  CONSTRAINT `fk_zipFileForBulkUpload_customer` 
  FOREIGN KEY (`CustomerId`)
  REFERENCES `customer` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_zipFileForBulkUpload_franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_zipFileForBulkUpload_franchisee` 
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_zipFileForBulkUpload_bulkUploadZipFile_idx` (`ZipFileId`),
  CONSTRAINT `fk_zipFileForBulkUpload_bulkUploadZipFile` 
  FOREIGN KEY (`ZipFileId`)
  REFERENCES `bulkUploadZipFile` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  
  KEY `fk_zipFileForBulkUpload_marketingclass_idx` (`MarketingClassId`),
  CONSTRAINT `fk_zipFileForBulkUpload_marketingclass` 
  FOREIGN KEY (`MarketingClassId`)
  REFERENCES `marketingclass` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=latin1;


ALTER TABLE zipFileForBulkUpload
ADD S3BucketURL Varchar(128) DEFAULT NULL;

ALTER TABLE zipFileForBulkUpload
ADD S3BucketThumbURL Varchar(128) DEFAULT NULL;

ALTER TABLE zipFileForBulkUpload
ADD Rotation bigint(20) DEFAULT NULL;

ALTER TABLE bulkUploadZipFile
ADD IsFetchedFromS3 bit(1) DEFAULT b'0';