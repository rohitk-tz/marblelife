CREATE TABLE `CustomerReviewSystemRecord` (
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `CustomerId` BIGINT(20) NOT NULL ,
  `FranchiseeId` BIGINT(20) NOT NULL ,
  `BusinessId` BIGINT(20) NOT NULL ,
  `ReviewSystemCustomerId` BIGINT(20) NOT NULL , 
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`Id`)  ,
  INDEX `fk_customerreviewsystemRecord_franchisee_idx` (`FranchiseeId` ASC)  ,
  INDEX `fk_customerReviewSystem_customer_idx` (`CustomerId` ASC)  ,
  CONSTRAINT `fk_customerreviewsystemRecord_franchisee`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_customerReviewSystem_customer`
    FOREIGN KEY (`CustomerId`)
    REFERENCES `Customer` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION);

