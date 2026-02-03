Create Table `estimatePriceNotes`(
`Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
`honingmeasurementId` BIGINT(20) NULL ,
`notes` varchar(1024) NULL ,
`DataRecorderMetaDataId` BigInt(20) null,
`IsDeleted` bit(1) NOT NULL DEFAULT b'0',
PRIMARY KEY (`Id`),
CONSTRAINT `fk_seoPriceNotes_honingmeasurementId_idx`
FOREIGN KEY (`honingmeasurementId`)
REFERENCES `honingmeasurement` (`Id`),
INDEX `fk_seoPriceNotes_datarecordermetadataId_idx` (`DataRecorderMetaDataId` ASC) ,
CONSTRAINT `fk_seoPriceNotes_datarecordermetadataId_idx`
FOREIGN KEY (`DataRecorderMetaDataId`)
REFERENCES `DataRecorderMetaData` (`Id`)
);