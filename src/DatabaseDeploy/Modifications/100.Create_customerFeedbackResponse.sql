CREATE TABLE `customerfeedbackresponse` (
  `ID` BIGINT(20) NOT NULL AUTO_INCREMENT ,
  `FeedbackId` BIGINT(20) NOT NULL ,
  `CustomerId` BIGINT(20) NOT NULL ,
  `CustomerEmail` VARCHAR(512) NULL,
  `ResponseContent` TEXT NULL ,
  `DateOfReview` DATETIME NOT NULL ,
  `Rating` DECIMAL(4,2) NULL ,
  `Recommend` INT NULL DEFAULT NULL,
  `ShowReview` BIT(1) NOT NULL DEFAULT b'0' ,
  `Isdeleted` BIT(1) NOT NULL DEFAULT b'0' ,
  PRIMARY KEY (`ID`) );

