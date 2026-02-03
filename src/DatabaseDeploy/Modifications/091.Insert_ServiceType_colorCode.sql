ALTER TABLE `servicetype` 
ADD COLUMN `ColorCode` VARCHAR(45) NULL AFTER `Name`;

UPDATE  `servicetype` SET `ColorCode`='#e35b5a' WHERE `Id`='1';
UPDATE  `servicetype` SET `ColorCode`='#B9783F' WHERE `Id`='2';
UPDATE  `servicetype` SET `ColorCode`='#B7B83F' WHERE `Id`='3';
UPDATE  `servicetype` SET `ColorCode`='#44b6ae' WHERE `Id`='4';
UPDATE  `servicetype` SET `ColorCode`='#00FFF0' WHERE `Id`='5';
UPDATE  `servicetype` SET `ColorCode`='#0087FF' WHERE `Id`='6';
UPDATE  `servicetype` SET `ColorCode`='#8300FF' WHERE `Id`='7';
UPDATE  `servicetype` SET `ColorCode`='#FF00AA' WHERE `Id`='8';
UPDATE  `servicetype` SET `ColorCode`='#FF0000' WHERE `Id`='9';
UPDATE  `servicetype` SET `ColorCode`='#CD82AD' WHERE `Id`='10';
UPDATE  `servicetype` SET `ColorCode`='#0055FF' WHERE `Id`='11';
UPDATE  `servicetype` SET `ColorCode`='#00FFAE' WHERE `Id`='12';
UPDATE  `servicetype` SET `ColorCode`='#BEB1DB' WHERE `Id`='13';
UPDATE  `servicetype` SET `ColorCode`='#DBB1CC' WHERE `Id`='14';

UPDATE  `servicetype` SET `ColorCode`='#E58A16' WHERE `Id`='15';
UPDATE  `servicetype` SET `ColorCode`='#E516D8' WHERE `Id`='16';
UPDATE  `servicetype` SET `ColorCode`='#32E516' WHERE `Id`='17';
UPDATE  `servicetype` SET `ColorCode`='#161CE5' WHERE `Id`='18';
UPDATE  `servicetype` SET `ColorCode`='#0E0E16' WHERE `Id`='19';
