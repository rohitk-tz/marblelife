CREATE TABLE `geoCodefileupload` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `FileId` bigint(20) NOT NULL,
  `StatusId` bigint(20) NOT NULL,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_GeoFileUplaod_DataRecorderMetaData_idx` (`DataRecorderMetaDataId`),
  KEY `fk_GeoFileUpload_File_idx` (`FileId`),
  KEY `fk_GeoFileUpload_lookup_idx` (`StatusId`),
  CONSTRAINT `fk_GeoFileUplaod_DataRecorderMetaData` FOREIGN KEY (`DataRecorderMetaDataId`) REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_GeoFileUpload_File` FOREIGN KEY (`FileId`) REFERENCES `file` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  CONSTRAINT `fk_GeoFileUpload_lookup` FOREIGN KEY (`StatusId`) REFERENCES `lookup` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1