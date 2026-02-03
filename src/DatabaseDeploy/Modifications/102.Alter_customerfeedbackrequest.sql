ALTER TABLE `customerfeedbackrequest` 
ADD COLUMN `ResponseId` BIGINT(20) NULL DEFAULT NULL AFTER `IsDeleted`,
ADD INDEX `fk_customerFeedbackRequest_customerFeedbackResponse_idx` (`ResponseId` ASC);
ALTER TABLE `customerfeedbackrequest` 
ADD CONSTRAINT `fk_customerFeedbackRequest_customerFeedbackResponse`
  FOREIGN KEY (`ResponseId`)
  REFERENCES `customerfeedbackresponse` (`ID`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
