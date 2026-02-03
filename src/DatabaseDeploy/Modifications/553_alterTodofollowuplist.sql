ALTER TABLE `todofollowuplist` 
ADD COLUMN `DataRecorderMetaDataId` BIGINT(20) NULL DEFAULT NULL,
ADD INDEX `fk_todofollowuplist_datarecordermetadata_idx` (`DataRecorderMetaDataId`);
ALTER TABLE `todofollowuplist` 
ADD CONSTRAINT `fk_todofollowuplist_datarecordermetadata`
  FOREIGN KEY (`DataRecorderMetaDataId`)
  REFERENCES `datarecordermetadata` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;