ALTER TABLE `servicefeeinvoiceitem` 
ADD COLUMN `Percentage` DECIMAL(10,2) NULL DEFAULT NULL AFTER `IsDeleted`;

ALTER TABLE `franchiseeloanschedule` 
ADD COLUMN `LoanTerm` INT NOT NULL;

ALTER TABLE `franchiseeloanschedule` 
ADD COLUMN `CalculateReschedule` BIT(1) NOT NULL DEFAULT b'0';

ALTER TABLE `franchiseeloanschedule` 
CHANGE COLUMN `PaidAmount` `OverPaidAmount` DECIMAL(10,2) NULL DEFAULT NULL ;

ALTER TABLE `franchiseeloanschedule` 
ADD COLUMN `TotalPrincipal` DECIMAL(10,2) NOT NULL;







