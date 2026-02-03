ALTER TABLE `FranchiseeNotes` 
DROP COLUMN `CreatedOn`,
ADD COLUMN `DataRecorderMetadataId` BIGINT(20) NOT NULL ,
ADD INDEX `FK_FranchiseeNotes_dataRecorderMetaData_idx` (`DataRecorderMetadataId` ASC) ;
ALTER TABLE `FranchiseeNotes` 
ADD CONSTRAINT `FK_FranchiseeNotes_dataRecorderMetaData`
  FOREIGN KEY (`DataRecorderMetadataId`)
  REFERENCES `DataRecorderMetaData` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
