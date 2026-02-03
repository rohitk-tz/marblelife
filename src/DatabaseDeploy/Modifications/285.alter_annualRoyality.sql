ALTER TABLE `annualroyality` 
ADD COLUMN `MonthlyRoyality` DECIMAL(10,2) NULL AFTER `IsDeleted`;


ALTER TABLE `annualroyality` 
ADD COLUMN `payment` DECIMAL(10,2) NULL DEFAULT NULL AFTER `MonthlyRoyality`;
