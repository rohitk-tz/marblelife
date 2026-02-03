ALTER TABLE `franchisee` 
ADD COLUMN `SchedulerFirstName` VARCHAR(512) NULL ,
ADD COLUMN `SchedulerLastName` VARCHAR(512) NULL ,
ADD COLUMN `SchedulerEmail` VARCHAR(256) NULL;



UPDATE `makalu`.`servicetype` SET `Name` = 'MAINTENANCE:MONTHLY' WHERE (`Id` = '6');
UPDATE `makalu`.`servicetype` SET `Name` = 'MAINTENANCE:BI-MONTHLY' WHERE (`Id` = '7');
UPDATE `makalu`.`servicetype` SET `Name` = 'MAINTENANCE:QUARTERLY' WHERE (`Id` = '8');
