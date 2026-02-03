CREATE TABLE `calldetailsreportnotes` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `CallerId` varchar(30) NOT NULL,
  `Notes` varchar(1024) default null,
  `Timestamp` datetime NULL,
  `DataRecorderMetaDataId` bigint(20) NOT NULL,
  `IsActive` bit(1) NOT NULL DEFAULT b'0',
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  KEY `fk_CallDetailsReportNotes_DataRecorderMetaData_idx` (`DataRecorderMetaDataId`),
  CONSTRAINT `fk_CallDetailsReportNotes_DataRecorderMetaData` 
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=latin1