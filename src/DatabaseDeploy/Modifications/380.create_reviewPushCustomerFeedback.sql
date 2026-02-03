CREATE TABLE `reviewPushCustomerFeedback`(
  `Id` BIGINT(20) NOT NULL AUTO_INCREMENT,
  `Review_Id` BIGINT(20) NOT NULL ,
  `Name` varchar(100) NOT NULL ,
  `Review` varchar(1028) NOT NULL ,
  `Email` varchar(200) NOT NULL ,
  `url` varchar(400) NOT NULL ,
  `FranchiseeId` BIGINT(20) NOT NULL ,
  `FranchiseeName` varchar(100) NOT NULL ,
  `rating` BIGINT(20) NOT NULL ,
  `Location_Id` BIGINT(20) NOT NULL ,
  `Rp_ID` BIGINT(20) NOT NULL ,
  `Rp_date`date NOT NULL ,
  `Db_date`date NOT NULL ,
  `IsDeleted` BIT Not NULL DEFAULT 0 ,
  PRIMARY KEY (`Id`),
  KEY `fk_reviewPushCustomerFeedback_franchisee_idx` (`FranchiseeId`),
  CONSTRAINT `fk_reviewPushCustomerFeedback_franchisee` FOREIGN KEY (`FranchiseeId`) REFERENCES `franchisee` (`Id`) ON DELETE NO ACTION ON UPDATE NO ACTION
  );
  
ALTER TABLE `makalu`.`reviewpushcustomerfeedback` 
DROP FOREIGN KEY `fk_reviewPushCustomerFeedback_franchisee`;
ALTER TABLE `makalu`.`reviewpushcustomerfeedback` 
CHANGE COLUMN `Review_Id` `Review_Id` BIGINT(20) NULL ,
CHANGE COLUMN `Name` `Name` VARCHAR(100) NULL ,
CHANGE COLUMN `Review` `Review` VARCHAR(1028) NULL ,
CHANGE COLUMN `Email` `Email` VARCHAR(200) NULL ,
CHANGE COLUMN `url` `url` VARCHAR(400) NULL ,
CHANGE COLUMN `FranchiseeId` `FranchiseeId` BIGINT(20) NULL ,
CHANGE COLUMN `FranchiseeName` `FranchiseeName` VARCHAR(100) NULL ,
CHANGE COLUMN `Location_Id` `Location_Id` BIGINT(20) NULL ,
CHANGE COLUMN `Rp_ID` `Rp_ID` BIGINT(20) NULL ,
CHANGE COLUMN `Rp_date` `Rp_date` DATE NULL ,
CHANGE COLUMN `Db_date` `Db_date` DATE NULL ;
ALTER TABLE `makalu`.`reviewpushcustomerfeedback` 
ADD CONSTRAINT `fk_reviewPushCustomerFeedback_franchisee`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `makalu`.`franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
