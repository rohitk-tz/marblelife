CREATE TABLE `FranchiseeNotes` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NOT NULL ,
  `Description` VARCHAR(1024) NULL ,
  `CreatedOn` DATETIME NOT NULL ,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`)  ,
  INDEX `FK_FranchiseeNotes_Franchisee_idx` (`FranchiseeId` ASC)  ,
  CONSTRAINT `FK_FranchiseeNotes_Franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);