CREATE TABLE `reviewMarketingImageHistry` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `jobestimateimageId` BIGINT(20) NOT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`) ,
  INDEX `fk_reviewMarketingImageHistry_jobestimateimageId_idx` (`jobestimateimageId` ASC) ,
  INDEX `fk_reviewMarketingImageHistry_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC) ,
  CONSTRAINT `fk_reviewMarketingImageHistry_jobestimateimageI`
    FOREIGN KEY (`jobestimateimageId`)
    REFERENCES `jobestimateimage` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_reviewMarketingImageHistry_datarecordermetadata`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `datarecordermetadata` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


