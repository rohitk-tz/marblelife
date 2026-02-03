CREATE TABLE `priceestimatefileupload` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FileId` bigint(20) NOT NULL,
  `StatusId` bigint(20) NOT NULL,
  `Notes` varchar(1024) default null,
  `ParsedLogFileId` BIGINT(20) NULL,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsFranchiseeAdmin` bit(1) NOT NULL DEFAULT b'0',
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_PriceEstimateFileUpload_DataRecorderMetaData_idx` (`DataRecorderMetaDataId`),
  KEY `fk_PriceEstimateFileUpload_File_idx` (`FileId`),
  KEY `fk_PriceEstimateFileUpload_lookup_idx` (`StatusId`),
  KEY  `fk_PriceEstimateFileUpload_parsedLogFileId_idx` (`ParsedLogFileId`),
  CONSTRAINT `fk_PriceEstimateFileUpload_DataRecorderMetaData` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_PriceEstimateFileUpload_File` FOREIGN KEY (`FileId`) REFERENCES `file` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_PriceEstimateFileUpload_lookup` FOREIGN KEY (`StatusId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_PriceEstimateFileUpload_parsedLogFileId_idx` FOREIGN KEY (`ParsedLogFileId`) REFERENCES `file` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1