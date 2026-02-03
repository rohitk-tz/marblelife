ALTER TABLE `customerfeedbackrequest` 
ADD COLUMN `QBInvoiceId` VARCHAR(128) NOT NULL AFTER `IsQueued`;
