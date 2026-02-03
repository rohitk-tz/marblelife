CREATE TABLE `onetimeprojectfeeAddFundRoyality` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `franchiseeId` BIGINT(20) NOT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `IsInRoyality` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`ID`) ,
  INDEX `fk_reviewMarketingImageHistry_organization_idx` (`franchiseeId` ASC) ,
  CONSTRAINT `fk_reviewMarketingImageHistry_organization`
    FOREIGN KEY (`franchiseeId`)
    REFERENCES `organization` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);



    ALTER TABLE `jobScheduler` 
ADD COLUMN `IsCancellationMailSend` bool Default true;
