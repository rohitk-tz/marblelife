ALTER TABLE `customerfeedbackrequest` 
ADD COLUMN `AuditActionId` BIGINT(20) NULL DEFAULT 153 AFTER `isDeleted`,
ADD INDEX `fk_customerfeedbackrequest_auditActionId_idx` (`AuditActionId` ASC);
ALTER TABLE `customerfeedbackrequest` 
ADD CONSTRAINT `fk_customerfeedbackrequest_auditActionId`
  FOREIGN KEY (`AuditActionId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  
  ALTER TABLE `customerfeedbackresponse` 
ADD COLUMN `AuditActionId` BIGINT(20) NULL DEFAULT 153 AFTER `isDeleted`,
ADD INDEX `fk_customerfeedbackresponse_auditActionId_idx` (`AuditActionId` ASC);
ALTER TABLE `customerfeedbackresponse` 
ADD CONSTRAINT `fk_customerfeedbackresponse_auditActionId`
  FOREIGN KEY (`AuditActionId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;
  
  
   ALTER TABLE `reviewpushcustomerfeedback` 
ADD COLUMN `AuditActionId` BIGINT(20) NULL DEFAULT 153 AFTER `isDeleted`,
ADD INDEX `fk_reviewpushcustomerfeedback_auditActionId_idx` (`AuditActionId` ASC);
ALTER TABLE `reviewpushcustomerfeedback` 
ADD CONSTRAINT `fk_reviewpushcustomerfeedback_auditActionId`
  FOREIGN KEY (`AuditActionId`)
  REFERENCES `lookup` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;



  ALTER TABLE `customerfeedbackresponse` 
ADD COLUMN `IsFromGoogleAPI` bit(1) NULL DEFAULT false;


ALTER TABLE `customerfeedbackresponse` 
ADD COLUMN `IsFromSystemReviewSystem` bit(1) NULL DEFAULT false;



ALTER TABLE `makalu`.`customerfeedbackresponse` 
CHANGE COLUMN `FeedbackId` `FeedbackId` BIGINT(20) NULL ;




ALTER TABLE `makalu`.`customerfeedbackresponse` 
CHANGE COLUMN `FeedbackId` `FeedbackId` BIGINT(20) NULL ;