CREATE TABLE `franchisedocument` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FileId` BIGINT(20) NOT NULL,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `ExpiryDate` DATE NULL DEFAULT NULL,
  `DataRecorderMetaDataId` BIGINT(20) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  INDEX `fk_franchiseDocument_file_idx` (`FileId` ASC),
  INDEX `fk_FranchiseDocument_Franchisee_idx` (`FranchiseeId` ASC),
  INDEX `fk_franchiseDocument_datarecordermetadata_idx` (`DataRecorderMetaDataId` ASC),
  CONSTRAINT `fk_franchiseDocument_file`
    FOREIGN KEY (`FileId`)
    REFERENCES `file` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseDocument_Franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_franchiseDocument_datarecordermetadata`
    FOREIGN KEY (`DataRecorderMetaDataId`)
    REFERENCES `datarecordermetadata` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);
