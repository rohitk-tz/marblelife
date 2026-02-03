INSERT INTO `makalu`.`documenttype` (`Id`, `Name`, `CategoryId`, `IsDeleted`) VALUES ('20', 'Business License', '201', b'0');
INSERT INTO `makalu`.`documenttype` (`Id`, `Name`, `CategoryId`, `IsDeleted`) VALUES ('21', 'Contractor License', '201', b'0');
INSERT INTO `makalu`.`documenttype` (`Id`, `Name`, `CategoryId`, `IsDeleted`) VALUES ('22', 'Building License', '201', b'0');


UPDATE `makalu`.`documenttype` SET `order` = '20' WHERE (`Id` = '8');
UPDATE `makalu`.`documenttype` SET `order` = '8' WHERE (`Id` = '20');
UPDATE `makalu`.`documenttype` SET `order` = '9' WHERE (`Id` = '21');
UPDATE `makalu`.`documenttype` SET `order` = '10' WHERE (`Id` = '22');
UPDATE `makalu`.`documenttype` SET `order` = '11' WHERE (`Id` = '11');
