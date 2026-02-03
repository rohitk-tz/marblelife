ALTER TABLE `franchisee` 
ADD COLUMN `IsReviewFeedbackEnabled` BIT(1) NOT NULL DEFAULT b'0' AFTER `Currency`;

ALTER TABLE `makalu`.`franchisee` 
ADD COLUMN `BusinessId` BIGINT(20) NULL AFTER `IsReviewFeedbackEnabled`;


