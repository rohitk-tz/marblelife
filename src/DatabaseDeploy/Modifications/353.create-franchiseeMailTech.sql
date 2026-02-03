CREATE TABLE `FranchiseeTechMailService` (
 `ID` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `FranchiseeId` BIGINT(20) NOT NULL,
  `Amount` decimal(10,2) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
   `TechCount` bigint(20) DEFAULT NULL,
   `IsGeneric` bool,
  PRIMARY KEY (`Id`) ,
  INDEX `FK_franchieeTechMailService_type_idx` (`ID` ASC) ,
  CONSTRAINT `FK_franchieeTechMailService_type`
    FOREIGN KEY (`FranchiseeId`)
    REFERENCES `Franchisee` (`Id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);


ALTER TABLE `franchiseetechmailservice` 
ADD COLUMN `multiplicationFactor` bigint(20) DEFAULT NULL ;