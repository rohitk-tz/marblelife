 CREATE TABLE `mastermarketingclass` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name`  varchar(1024) DEFAULT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `description`  varchar(1024) DEFAULT NULL,
  `colorcode`  varchar(100) DEFAULT NULL,
   PRIMARY KEY (`Id`)
 );
 
 
  CREATE TABLE `subclassmarketingclass` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(1024) DEFAULT NULL,
  `alias` varchar(1024) DEFAULT NULL,
  `marketingclassId` bigint(20)  NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
   PRIMARY KEY (`Id`),
   KEY `fk_subclassmarketingclass_mastermarketingclass_idx` (`marketingclassId`),
   CONSTRAINT `fk_subclassmarketingclass_mastermarketingclass` FOREIGN KEY (`marketingclassId`) REFERENCES `mastermarketingclass` (`Id`)
 );
 
 
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('COMMERCIAL', b'0');
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('RESIDENTIAL', b'0');
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('WOODLIFE', b'0');
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('NATIONAL', b'0');
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('DETROIT', b'0');
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('UNCLASSIFIED', b'0');
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('SHERATON', b'0');
 INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('PRODUCT', b'0');
  INSERT INTO `mastermarketingclass` (`name`, `IsDeleted`) VALUES ('HOTEL', b'0');
 
 select Id into @commericalId  from mastermarketingclass where name ='COMMERCIAL';
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('AUTO','',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('FINANCIAL','BANK, ACCOUNTING',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('CHURCH','TEMPLE',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('CLUB','FITNESS,ATHETIC CLUB,ATHLETIC,FITNESS CLUB,COUNTRY CLUB, SPA',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('CORPORATE','HEADQUARTERS',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('GOVERNMENT','',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('GROCERY','',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('INSURANCE','',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('JANITORIAL','MAID,JANITOR,CLEANING,BUILDING SERVICES',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('BUILDING SERVICES','',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('OFFICE','CLASS A',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('RESTAURANT','',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('RETAILSTORE','RETAIL, MALL, RETAIL STORE',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('THEATER','THEATRE',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('YACHT','',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('EDUCATION','HIGH-SCHOOL,COLLEGE,UNIVERSITY,MIDDLE-SCHOOL,SCHOOL, PRE-SCHOOL,PRESCHOOL',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('MEDICAL','DOCTOR, DENTIST, HOSPITAL, MEDICAL OFFICE, URGENT CARE, CHIROPRACTOR',@commericalId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('PROFESSIONAL','LAWYER, LEGAL, ENGINEER',@commericalId,b'0');
 
 
 
 select Id into @residentialId  from mastermarketingclass where name ='RESIDENTIAL';
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('CONDO','CONOMINIUM, APPARTMENT, DUPLEX',@residentialId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('FLOORING (CONTRACTOR & RETAIL)','FLOORING,FLOORING&COUNTERTOPS,FABRICATORS,COUNTERTOP,COUNTERTOPS,FLOOR,FABRICATORS,TILE,TILE STORE,TILE INSTALLER,INSTALLER',@residentialId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('HOMEMANGEMENT','HOME MGMT,HOME MANAGAGEMENT,RESIDENTIAL PROPERTY MGMT,RESIDENTIAL PROPERTY MANAGEMENT,RESIDENTIALPROPERTYMGMT',@residentialId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('INTERIORDESIGN','BUILDER&DESIGN,INTERIOR DESIGN',@residentialId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('BUILDER','BUILDER&TILES',@residentialId,b'0');
 
 
 
 select Id into @woodId  from mastermarketingclass where name ='WOODLIFE';
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('WOOD','WOOD',@woodId,b'0');
 
 
 
  select Id into @productId  from mastermarketingclass where name ='PRODUCT';
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('AMAZON','',@productId,b'0');
 INSERT INTO `subclassmarketingclass` (`name`, `alias`,`marketingclassId`,`IsDeleted`) VALUES ('HARDWARE','',@productId,b'0');
 
 
 


 
 
 
 
 
 