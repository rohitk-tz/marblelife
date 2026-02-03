ALTER TABLE `franchisee` 
CHANGE COLUMN `IsDeleted` `IsDeleted` BIT(1) NOT NULL DEFAULT b'0' AFTER `BusinessId`,
ADD COLUMN `DisplayName` VARCHAR(512) NULL DEFAULT NULL AFTER `IsReviewFeedbackEnabled`;
