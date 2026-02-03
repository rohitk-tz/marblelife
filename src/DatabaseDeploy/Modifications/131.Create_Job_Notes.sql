CREATE TABLE `jobnote` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `JobId` BIGINT(20) NULL ,
  `StatusId` BIGINT(20) NULL DEFAULT NULL ,
  `EstimateId` BIGINT(20) NULL DEFAULT NULL ,
  `Note` VARCHAR(512) NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`) );

ALTER TABLE `jobnote` 
ADD COLUMN `DataRecorderMetaDataId` BIGINT(20) NOT NULL ,
ADD INDEX `fk_jobNote_datarecorderMetaData_idx` (`DataRecorderMetaDataId` ASC) ;
ALTER TABLE `jobnote` 
ADD CONSTRAINT `fk_jobNote_datarecorderMetaData`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
