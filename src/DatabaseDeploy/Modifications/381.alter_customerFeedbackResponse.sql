ALTER TABLE `customerfeedbackresponse` 
ADD COLUMN `url` varchar(9024) NULL DEFAULT NULL AFTER `showReview`,
ADD COLUMN `dateOfDataInDataBase` datetime NULL DEFAULT NULL AFTER `showReview`,
ADD COLUMN `ReviewId` BIGINT(20) NULL DEFAULT null AFTER `showReview`,
ADD COLUMN `IsFromNewReviewSystem` bit(1) NULL DEFAULT b'0' AFTER `showReview`;


ALTER TABLE `makalu`.`customerfeedbackrequest` 
DROP FOREIGN KEY `fk_customerfeedbackRecord_customer`;
ALTER TABLE `makalu`.`customerfeedbackrequest` 
CHANGE COLUMN `CustomerId` `CustomerId` BIGINT(20) NULL ;
ALTER TABLE `makalu`.`customerfeedbackrequest` 
ADD CONSTRAINT `fk_customerfeedbackRecord_customer`
  FOREIGN KEY (`CustomerId`)
  REFERENCES `makalu`.`customer` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
