INSERT INTO `marketingclass` (`Id`, `Name`, `ColorCode`) VALUES ('20', 'RETAIL STORE', '#800000');
INSERT INTO `marketingclass` (`Id`, `Name`, `ColorCode`) VALUES ('21', 'RESIDENTIAL PROPERTY MGMT', '#900000');


UPDATE `marketingclass` SET `Alias`='RETAIL' WHERE `Id`='20';
UPDATE `marketingclass` SET `Alias`='HOME MGMT' WHERE `Id`='21';


UPDATE `marketingclass` SET `Alias`='INTERIOR DESIGN' WHERE `Id`='16';
UPDATE `marketingclass` SET `Alias`='BUILDER' WHERE `Id`='5';

ALTER TABLE `marketingclass` 
CHANGE COLUMN `Alias` `Alias` VARCHAR(124) NULL DEFAULT NULL ;

UPDATE `marketingclass` SET `Alias`='FLOORING,FLOORING&COUNTERTOPS,FABRICATORS,COUNTERTOP,COUNTERTOPS,FLOOR,FABRICATORS' WHERE `Id`='15';

INSERT INTO `marketingclass` (`Id`, `Name`, `ColorCode`) VALUES ('22', 'AUTO', '#C0C0C0');
INSERT INTO `marketingclass` (`Id`, `Name`, `ColorCode`) VALUES ('23', 'INSURANCE', '#808080');
INSERT INTO  `marketingclass` (`Id`, `Name`, `ColorCode`) VALUES ('24', 'YACHT', '#FA8072');
