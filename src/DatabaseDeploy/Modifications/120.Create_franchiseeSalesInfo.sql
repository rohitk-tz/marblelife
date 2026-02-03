CREATE TABLE `franchiseeSalesInfo` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `FranchiseeId` BIGINT(20) NOT NULL ,
  `Month` INT NOT NULL ,
  `Year` INT NOT NULL ,
  `SalesAmount` DECIMAL(10,2) NOT NULL ,
  `AmountInLocalCurrency` DECIMAL(10,2) NOT NULL,
  `UpdatedDate` Date NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_franchiseeSalesInfo_franchisee_idx` (`FranchiseeId` ASC) ,
  CONSTRAINT `fk_franchiseeSalesInfo_franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);