CREATE TABLE `markbeforeAfterImagesHistry` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `ServiceId` BIGINT(20) NOT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`) ,
  INDEX `fk_markbeforeAfterImagesHistry_serviceId_idx` (`ServiceId` ASC) ,
  INDEX `fk_markbeforeAfterImagesHistry_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC) ,
  CONSTRAINT `fk_markbeforeAfterImagesHistry_serviceId`
    FOREIGN KEY (`ServiceId`)
    REFERENCES `jobestimateservices` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_markbeforeAfterImagesHistry_datarecordermetadata`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `datarecordermetadata` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


