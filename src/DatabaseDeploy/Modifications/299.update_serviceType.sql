

UPDATE `servicetype` SET `Alias`='Concrete, Concrete Restoration, Concerte Maintenance,CONCRETE POLISHING' WHERE `Id`='2';

INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `IsActive`, `IsDeleted`) VALUES ('33', 'CONCRETE COATINGS', '#FF6347', '101', b'1', b'0');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `IsActive`, `IsDeleted`) VALUES ('34', 'CONCRETE COUNTERTOPS', '#FF4500', '101', b'1', b'0');
INSERT INTO `servicetype` (`Id`, `Name`, `ColorCode`, `CategoryId`, `IsActive`, `IsDeleted`) VALUES ('35', 'CONCRETE OVERLAYMENTS', '#FFD700', '101', b'1', b'0');




ALTER TABLE `servicetype` 
ADD COLUMN `OrderBy` BIGINT(20) NULL AFTER `Alias`;

UPDATE `servicetype` SET `OrderBy`='1' WHERE `Id`='1';
UPDATE `servicetype` SET `OrderBy`='2' WHERE `Id`='2';
UPDATE `servicetype` SET `OrderBy`='2' WHERE `Id`='34';
UPDATE `servicetype` SET `OrderBy`='2' WHERE `Id`='35';
UPDATE `servicetype` SET `OrderBy`='2' WHERE `Id`='33';
UPDATE `servicetype` SET `OrderBy`='3' WHERE `Id`='3';
UPDATE `servicetype` SET `OrderBy`='4' WHERE `Id`='4';
UPDATE `servicetype` SET `OrderBy`='5' WHERE `Id`='5';
UPDATE `servicetype` SET `OrderBy`='6' WHERE `Id`='6';
UPDATE `servicetype` SET `OrderBy`='7' WHERE `Id`='7';
UPDATE `servicetype` SET `OrderBy`='8' WHERE `Id`='8';
UPDATE `servicetype` SET `OrderBy`='9' WHERE `Id`='9';
UPDATE `servicetype` SET `OrderBy`='10' WHERE `Id`='10';
UPDATE `servicetype` SET `OrderBy`='11' WHERE `Id`='11';
UPDATE `servicetype` SET `OrderBy`='12' WHERE `Id`='12';
UPDATE `servicetype` SET `OrderBy`='13' WHERE `Id`='13';
UPDATE `servicetype` SET `OrderBy`='14' WHERE `Id`='14';
UPDATE `servicetype` SET `OrderBy`='15' WHERE `Id`='15';
UPDATE `servicetype` SET `OrderBy`='16' WHERE `Id`='16';
UPDATE `servicetype` SET `OrderBy`='17' WHERE `Id`='17';
UPDATE `servicetype` SET `OrderBy`='18' WHERE `Id`='18';
UPDATE `servicetype` SET `OrderBy`='19' WHERE `Id`='19';
UPDATE `servicetype` SET `OrderBy`='20' WHERE `Id`='20';
UPDATE `servicetype` SET `OrderBy`='21' WHERE `Id`='21';
UPDATE `servicetype` SET `OrderBy`='22' WHERE `Id`='22';
UPDATE `servicetype` SET `OrderBy`='23' WHERE `Id`='23';
UPDATE `servicetype` SET `OrderBy`='24' WHERE `Id`='24';
UPDATE `servicetype` SET `OrderBy`='25' WHERE `Id`='25';
UPDATE `servicetype` SET `OrderBy`='26' WHERE `Id`='26';
UPDATE `servicetype` SET `OrderBy`='27' WHERE `Id`='27';
UPDATE `servicetype` SET `OrderBy`='28' WHERE `Id`='28';
UPDATE `servicetype` SET `OrderBy`='29' WHERE `Id`='29';
UPDATE `servicetype` SET `OrderBy`='30' WHERE `Id`='30';
UPDATE `servicetype` SET `OrderBy`='31' WHERE `Id`='31';
UPDATE `servicetype` SET `OrderBy`='33' WHERE `Id`='32';

UPDATE `servicetype` SET `Name`='CONCRETE-COATINGS' WHERE `Id`='33';
UPDATE `servicetype` SET `Name`='CONCRETE-COUNTERTOPS' WHERE `Id`='34';
UPDATE `servicetype` SET `Name`='CONCRETE-OVERLAYMENTS' WHERE `Id`='35';


UPDATE `servicetype` SET `Alias`='CONCRETECOATINGS,CONCRETE- COATINGS' WHERE `Id`='33';
UPDATE `servicetype` SET `Alias`='CONCRETECOUNTERTOPS,CONCRETE COUNTERTOPS' WHERE `Id`='34';
UPDATE `servicetype` SET `Alias`='CONCRETEOVERLAYMENTS,CONCRETE OVERLAYMENTS' WHERE `Id`='35';
