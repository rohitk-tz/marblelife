ALTER TABLE `organizationroleuser` 
ADD COLUMN `ColorCodeSale` VARCHAR(45) NULL DEFAULT NULL AFTER `IsActive`;


UPDATE `makalu`.`emailtemplate` SET `Subject` = 'Best Pair marked images by Franchisees Report Week @Model.StartDate to @Model.EndDate' WHERE (`Id` = '42');

UPDATE `makalu`.`documenttype` SET `Name` = 'Resale / Sales Tax Certificate' WHERE (`Id` = '11');


UPDATE `makalu`.`emailtemplate` SET `Subject` = 'Best Pair marked Images Report Week @Model.StartDate to @Model.EndDate' WHERE (`Id` = '41');
