ALTER TABLE `marketingclass` 
ADD COLUMN `ColorCode` VARCHAR(45) NULL AFTER `Name`;

UPDATE  `marketingclass` SET `ColorCode`='#44b6ae' WHERE `Id`='1';
UPDATE  `marketingclass` SET `ColorCode`='#B9783F' WHERE `Id`='2';
UPDATE  `marketingclass` SET `ColorCode`='#B7B83F' WHERE `Id`='3';
UPDATE  `marketingclass` SET `ColorCode`='#e35b5a' WHERE `Id`='4';
UPDATE  `marketingclass` SET `ColorCode`='#00FFF0' WHERE `Id`='5';
UPDATE  `marketingclass` SET `ColorCode`='#0087FF' WHERE `Id`='6';
UPDATE  `marketingclass` SET `ColorCode`='#8300FF' WHERE `Id`='7';
UPDATE  `marketingclass` SET `ColorCode`='#FF00AA' WHERE `Id`='8';
UPDATE  `marketingclass` SET `ColorCode`='#FF0000' WHERE `Id`='9';
UPDATE  `marketingclass` SET `ColorCode`='#CD82AD' WHERE `Id`='10';
UPDATE  `marketingclass` SET `ColorCode`='#0055FF' WHERE `Id`='11';
UPDATE  `marketingclass` SET `ColorCode`='#00FFAE' WHERE `Id`='12';
UPDATE  `marketingclass` SET `ColorCode`='#BEB1DB' WHERE `Id`='13';
UPDATE  `marketingclass` SET `ColorCode`='#DBB1CC' WHERE `Id`='14';