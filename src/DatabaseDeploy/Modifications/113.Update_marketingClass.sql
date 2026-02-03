ALTER TABLE `marketingclass` 
ADD COLUMN `Alias` VARCHAR(64) NULL DEFAULT NULL AFTER `ColorCode`;

INSERT INTO `marketingclass` (`Id`, `Name`, `ColorCode`, `Alias`, `IsDeleted`) VALUES ('15', 'FLOORING(CONTRACTOR&RETAIL)', '#E3E31F', 'FLOORING', 0);

UPDATE `marketingclass` SET `Name`='BUILDER&DESIGN', `Alias`='BUILDER' WHERE `Id`='5';
