CREATE TABLE `debuggerLogs` (
  `ID` bigint(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` bigint(20) NOT NULL,
  `ActionId`  bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `UserId` bigint(20) NOT NULL,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `PageId`  bigint(20) NOT NULL,
  `description` varchar(5020) null,
  `JobSchedulerId` bigint(20)  NULL,
  `JobestimateimagecategoryId` bigint(20) NOT NULL,
  `jobestimateservicesId` bigint(20) NOT NULL,
  `TypeId` bigint(20) NOT NULL,
   PRIMARY KEY (`ID`),
  KEY `fk_debuggerLogs_datarecordermetadata_idx` (`DataRecorderMetaDataId`),
  KEY `FK_debuggerLogs_type_idx` (`ID`),
  KEY `FK_debuggerLogs_franchisee_idx` (`FranchiseeId`),
  KEY `FK_debuggerLogs_person_idx` (`UserId`),
  KEY `FK_debuggerLogs_lookUp_idx` (`ActionId`),
  KEY `FK_debuggerLogs_lookUp2_idx` (`PageId`),
  KEY `FK_debuggerLogs_scheduler_idx` (`JobSchedulerId`),
  KEY `FK_debuggerLogs_Jobestimateimagecategory_idx` (`JobestimateimagecategoryId`),
  KEY `FK_debuggerLogs_JobestimateservicesId_idx` (`jobestimateservicesId`),
  KEY `FK_debuggerLogs_lookup3_idx` (`typeId`),
  CONSTRAINT `FK_debuggerLogs_franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_debuggerLogs_person` FOREIGN KEY (`UserId`) REFERENCES `person` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_debuggerLogs_datarecordermetadata` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_debuggerLogs_lookUp` FOREIGN KEY (`ActionId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION, 
  CONSTRAINT `FK_debuggerLogs_lookUp2` FOREIGN KEY (`PageId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_debuggerLogs_scheduler` FOREIGN KEY (`JobSchedulerId`) REFERENCES `jobscheduler` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_debuggerLogs_Jobestimateimagecategory` FOREIGN KEY (`JobestimateimagecategoryId`) REFERENCES `jobestimateimagecategory` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_debuggerLogs_JobestimateservicesId` FOREIGN KEY (`jobestimateservicesId`) REFERENCES `jobestimateservices` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `FK_debuggerLogs_lookup3` FOREIGN KEY (`typeId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;



INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `Alias`, `IsDeleted`) VALUES ('41', 'BeforeAfterLogs', 'BeforeAfterLogs', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('262', '41', 'ADDING-NEW-VALUE', 'ADDINGNEWVALUE', '1', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('263', '41', 'EDITING-VALUE', 'EDITINGVALUE', '2', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('264', '41', 'ADDING-NEW-BEFORE-IMAGE', 'ADDINGNEWBEFOREIMAGE', '3', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('265', '41', 'ADDING-NEW-AFTER-IMAGE', 'ADDINGNEWAFTERIMAGE', '4', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('266', '41', 'EDITING-NEW-BEFORE-IMAGE', 'EDITINGNEWBEFOREIMAGE', '5', b'1', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('267', '41', 'EDITING-NEW-AFTERIMAGE', 'EDITINGNEWAFTERIMAGE', '6', b'1', b'0');
INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `Alias`, `IsDeleted`) VALUES ('42', 'PageType', 'PageType', b'0');
INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('268', '42', 'BEFORE-AFTER-IMAGE', 'BEFOREAFTERIMAGE', '1', b'1', b'0');



ALTER TABLE `jobscheduler` 
ADD COLUMN `IsCustomerMailSend` Bit(1) default 0  AFTER `IsDeleted`;
