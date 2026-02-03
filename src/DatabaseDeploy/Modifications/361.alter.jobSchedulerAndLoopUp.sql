ALTER TABLE `jobscheduler` 
DROP COLUMN `isConfirmed`;


INSERT INTO `makalu`.`lookuptype` (`Id`, `Name`, `IsDeleted`) VALUES ('27', 'Customer Mail Status', b'0');


INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('216', '27', 'Not Responded', 'NOTRESPONDED', '4', b'1', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('217', '27', 'Not Confirmed', 'NOTCONFIRMED', '4', b'1', b'0');

INSERT INTO `makalu`.`lookup` (`Id`, `LookupTypeId`, `Name`, `Alias`, `RelativeOrder`, `IsActive`, `IsDeleted`) VALUES ('218', '27', 'Confirmed', 'CONFIRMED', '4', b'1', b'0');

ALTER TABLE `jobscheduler` 
ADD COLUMN `schedulerStatus` BIGINT(20) NOT NULL DEFAULT 216;


ALTER TABLE `jobscheduler` 
ADD CONSTRAINT `fk_jobscheduler_lookup8`
FOREIGN KEY (`schedulerStatus`)
REFERENCES `lookup` (`Id`)
ON DELETE NO ACTION
ON UPDATE NO ACTION;



