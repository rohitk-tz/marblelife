CREATE TABLE `updateMarketingClassfileupload` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FileId` bigint(20) NOT NULL,
  `StatusId` bigint(20) NOT NULL,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  `notes` varchar(1024) DEFAULT NULL,
  `ParsedLogFileId` bigint(20) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `fk_updateMarketingClassfileupload_DataRecorderMetaData_idx` (`DataRecorderMetaDataId`),
  KEY `fk_updateMarketingClassfileupload_File_idx` (`FileId`),
  KEY `fk_updateMarketingClassfileupload_lookup_idx` (`StatusId`),
  KEY `fk_updateMarketingClassfileupload_subClassTypeId_idx` (`ParsedLogFileId`),
  CONSTRAINT `fk_updateMarketingClassfileupload_DataRecorderMetaData` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_updateMarketingClassfileupload_File` FOREIGN KEY (`FileId`) REFERENCES `file` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_updateMarketingClassfileupload_lookup` FOREIGN KEY (`StatusId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_updateMarketingClassfileupload_subClassTypeId` FOREIGN KEY (`ParsedLogFileId`) REFERENCES `file` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=120 DEFAULT CHARSET=latin1