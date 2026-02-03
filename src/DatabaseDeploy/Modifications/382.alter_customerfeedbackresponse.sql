ALTER TABLE `customerfeedbackresponse` 
ADD COLUMN `franchiseeId` BIGINT(20) NULL DEFAULT NULL AFTER `IsDeleted`,
ADD INDEX `fk_franchisee_customerfeedbackresponse_idx` (`franchiseeId` ASC);
ALTER TABLE `customerfeedbackresponse` 
ADD CONSTRAINT `fk_franchisee_customerfeedbackresponse`
  FOREIGN KEY (`franchiseeId`)
  REFERENCES `Franchisee` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;