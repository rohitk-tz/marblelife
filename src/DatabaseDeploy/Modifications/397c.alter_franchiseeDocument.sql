ALTER TABLE `documenttype` 
ADD COLUMN `order` BIGINT(20) NULL DEFAULT null AFTER `categoryId`;

UPDATE `makalu`.`documenttype` SET `order` = '1' WHERE (`Id` = '3');
UPDATE `makalu`.`documenttype` SET `order` = '2' WHERE (`Id` = '15');
UPDATE `makalu`.`documenttype` SET `order` = '3' WHERE (`Id` = '16');
UPDATE `makalu`.`documenttype` SET `order` = '4' WHERE (`Id` = '17');
UPDATE `makalu`.`documenttype` SET `order` = '5' WHERE (`Id` = '18');
UPDATE `makalu`.`documenttype` SET `order` = '6' WHERE (`Id` = '19');
UPDATE `makalu`.`documenttype` SET `order` = '7' WHERE (`Id` = '5');
UPDATE `makalu`.`documenttype` SET `order` = '8' WHERE (`Id` = '8');
UPDATE `makalu`.`documenttype` SET `order` = '9' WHERE (`Id` = '11');
UPDATE `makalu`.`documenttype` SET `order` = '10' WHERE (`Id` = '1');
UPDATE `makalu`.`documenttype` SET `order` = '11' WHERE (`Id` = '2');
UPDATE `makalu`.`documenttype` SET `order` = '12' WHERE (`Id` = '4');
UPDATE `makalu`.`documenttype` SET `order` = '13' WHERE (`Id` = '6');
UPDATE `makalu`.`documenttype` SET `order` = '14' WHERE (`Id` = '7');
UPDATE `makalu`.`documenttype` SET `order` = '15' WHERE (`Id` = '9');
UPDATE `makalu`.`documenttype` SET `order` = '16' WHERE (`Id` = '10');
UPDATE `makalu`.`documenttype` SET `order` = '17' WHERE (`Id` = '12');
UPDATE `makalu`.`documenttype` SET `order` = '18' WHERE (`Id` = '13');
UPDATE `makalu`.`documenttype` SET `order` = '19' WHERE (`Id` = '14');

call migration_franchiseeDocumentMigration();


UPDATE `makalu`.`documenttype` SET `Name` = 'COI ( Certificate of Insurance )' WHERE (`Id` = '5');
