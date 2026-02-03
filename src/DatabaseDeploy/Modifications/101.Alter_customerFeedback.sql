ALTER TABLE `customerfeedbackresponse` 
DROP COLUMN `CustomerEmail`;


ALTER TABLE `customerfeedbackrequest` 
ADD COLUMN `CustomerEmail` VARCHAR(512) NOT NULL AFTER `CustomerReviewSystemRecordId`;
