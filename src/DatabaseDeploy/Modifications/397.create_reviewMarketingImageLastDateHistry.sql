CREATE TABLE `reviewMarketingImageLastDateHistry` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `LastDateChecked`Datetime NOT NULL,
  `StartDate`Datetime NOT NULL,
  `EndDate`Datetime NOT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`) ,
  INDEX `fk_reviewMarketingImageLastDateHistry_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC) ,
  CONSTRAINT `fk_reviewMarketingImageLastDateHistry_datarecordermetadata`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `datarecordermetadata` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);


