ALTER TABLE `notificationqueue` 
ADD COLUMN `FranchiseeId` BIGINT(20) NULL,
ADD INDEX `fk_notificationqueue_organization_idx` (`FranchiseeId`);
ALTER TABLE `notificationqueue` 
ADD CONSTRAINT `fk_notificationqueue_organization`
  FOREIGN KEY (`FranchiseeId`)
  REFERENCES `organization` (`Id`)
  ON DELETE NO ACTION
  ON UPDATE NO ACTION;