UPDATE `makalu`.`servicetype` SET `Alias` = 'FABRICATORS, FABRICATION' WHERE (`Id` = '14' and `Name` = 'FABRICATORS');

ALTER TABLE `invoice` 
ADD COLUMN `reconciliationNotes` varchar(1024) NULL DEFAULT NULL;


ALTER TABLE `marketingleadcalldetail` 
ADD COLUMN `IsFromInvoca` Bit(1) DEFAULT b'0';

ALTER TABLE `marketingleadcalldetailv2`
ADD COLUMN `FindMeList` Varchar(224) Null DEFAULT Null;