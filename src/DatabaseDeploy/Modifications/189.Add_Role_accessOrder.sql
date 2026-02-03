INSERT INTO `role` (`Id`, `Name`, `Alias`, `OrganizationTypeId`) VALUES ('6', 'Operations Manager', 'opsMgr', '22');

ALTER TABLE `role` 
ADD COLUMN `AccessOrder` INT(20) NOT NULL;

UPDATE `role` SET `AccessOrder`='1' WHERE `Id`='1';
UPDATE `role` SET `AccessOrder`='2' WHERE `Id`='2';
UPDATE `role` SET `AccessOrder`='3' WHERE `Id`='3';
UPDATE `role` SET `AccessOrder`='4' WHERE `Id`='6';
UPDATE `role` SET `AccessOrder`='5' WHERE `Id`='5';
UPDATE `role` SET `AccessOrder`='6' WHERE `Id`='4';



